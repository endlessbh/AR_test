using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginRepairman.Tools;
using XCSJ.PluginTools;
using XCSJ.PluginTools.Draggers;
using XCSJ.PluginTools.Items;

namespace XCSJ.PluginPhysicses.Tools.Gadgets
{
    /// <summary>
    /// 物理关节：
    /// 1、用于模拟具有若干个档位的小配件
    /// 2、可使用Unity物理系统的【关节】进行模拟
    /// </summary>
    [RequireManager(typeof(PhysicsManager))]
    [Owner(typeof(PhysicsManager))]
    public abstract class PhysicsJoint : GrabbableUser
    {
        #region 关节属性

        [Group("关节设置", textEN = "Mechanism Settings")]
        /// <summary>
        /// 使用物理关节
        /// </summary>
        [Name("使用物理关节")]
        [Tip("为True时，使用物理系统的【关节】进行模拟，该组件以质量为计算基础模拟推力、弹力、阻力和震动；为False时直接设置变换位置", "")]
        public bool _useJoint = false;

        #region 档位

        /// <summary>
        /// 档位数
        /// </summary>
        [Name("档位数")]
        [Min(1)]
        [HideInSuperInspector(nameof(_useJoint), EValidityCheckType.True)]
        public int _stepCount = 1;

        /// <summary>
        /// 档位吸附规则
        /// </summary>
        [Flags]
        public enum ESnapRule
        {
            [Name("启用时吸附")]
            Enable,

            [Name("拖拽时吸附")]
            [Tip("每帧计算都执行吸附，对象运动呈现非连续状态；", "each frame of calculation is adsorbed, and the object motion is discontinuous;")]
            Hold,

            [Name("放手时吸附")]
            Release,
        }

        /// <summary>
        /// 档位吸附规则
        /// </summary>
        [Name("档位吸附规则")]
        [EnumPopup]
        [HideInSuperInspector(nameof(_useJoint), EValidityCheckType.True)]
        public ESnapRule _snapRule = ESnapRule.Enable | ESnapRule.Hold | ESnapRule.Release;

        /// <summary>
        /// 档位吸附音频
        /// </summary>
        [Name("档位吸附音频")]
        [HideInSuperInspector(nameof(_useJoint), EValidityCheckType.True)]
        public AudioClip _snapStepAudioClip;

        #endregion

        /// <summary>
        /// 全局刻度数变化回调
        /// </summary>
        public static event Action<PhysicsJoint, int> _onStepChanged;

        /// <summary>
        /// 当前刻度
        /// </summary>
        public virtual int currentStep
        {
            get => _currentStep;
            set
            {
                value = Mathf.Clamp(value, 0, _stepCount);
                if (_currentStep == value) return;

                _currentStep = value;

                if (_snapStepAudioClip)
                {
                    AudioSource.PlayClipAtPoint(_snapStepAudioClip, grabbable.position);
                }

                _onStepChanged?.Invoke(this, _currentStep);
            }
        }
        private int _currentStep = 0;

        /// <summary>
        /// 最小值
        /// </summary>
        public abstract float minValue { get; }

        /// <summary>
        /// 最大值
        /// </summary>
        public abstract float maxValue { get; }

        /// <summary>
        /// 当前值，在[minValue, maxValue]之间
        /// </summary>
        public abstract float currentValue { get; }

        /// <summary>
        /// 范围大小
        /// </summary>
        public float rangeSize => maxValue - minValue;

        /// <summary>
        /// 单刻度大小
        /// </summary>
        public float stepSize => rangeSize / _stepCount;

        /// <summary>
        /// 参考原点: 创建一个同父级下的不动点作为计算参考, 即使父级发生位移或旋转，该点仍然是正确的参考点
        /// </summary>
        public Transform orgin { get; private set; }

        #endregion

        #region Unity 消息

        /// <summary>
        /// 唤醒
        /// </summary>
        protected virtual void Awake()
        {
            var go = ToolsExtensionHelper.CreateGameObjectAtSameSibling(grabbable.targetTransform.gameObject, grabbable.targetTransform.name + "_" +
            CommonFun.Name(GetType()) + "_原点");
            if (go)
            {
                orgin = go.transform;
            }
        }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            grabbable.isConnectToOther = true;

            if (Mathf.Approximately(rangeSize, 0))
            {
                enabled = false;
                Debug.LogError(CommonFun.GameObjectComponentToString(this) + ":限定区间大小不能为0！");
                return;
            }

            if (!_useJoint)
            {
                if ((_snapRule & ESnapRule.Enable) == ESnapRule.Enable)
                {
                    Snap();
                }
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        protected void Update()
        {
            if (_useJoint)
            {
                UpdateJointState();
            }
        }

        /// <summary>
        /// 销毁
        /// </summary>
        protected virtual void OnDestroy()
        {
            if (orgin)
            {
                Destroy(orgin.gameObject);
            }
        }

        #endregion

        #region 检测关节状态

        private void UpdateJointState()
        {
            var state = CalculateJointState();

            if (_jointstate != state)
            {
                _jointstate = state;

                switch (_jointstate)
                {
                    case EJointState.Min: currentStep = 0; break;
                    case EJointState.Middle: break;
                    case EJointState.Max: currentStep = 1; break;
                }
            }
        }

        private EJointState CalculateJointState()
        {
            var percent = (currentValue - minValue) / rangeSize;

            if (percent < PhysicsHelper.DeadZoneOfPercent)
            {
                return EJointState.Min;
            }
            else if (percent > (1 - PhysicsHelper.DeadZoneOfPercent))
            {
                return EJointState.Max;
            }
            else
            {
                return EJointState.Middle;
            }
        }

        private enum EJointState
        {
            None,

            Min,

            Middle,

            Max,
        }
        private EJointState _jointstate = EJointState.None;

        #endregion

        #region 关节运动

        protected override void OnInputInteract(InteractObject sender, InteractData interactData)
        {
            base.OnInputInteract(sender, interactData);

            if (sender == grabbable)
            {
                if (grabbable._grabResultCmds.TryGetECmd(interactData.cmdName, out var eCmd))
                {
                    switch (eCmd)
                    {
                        case EGrabResultCmd.Grab:
                            {
                                if (interactData is HoldedInteractData hd)
                                {
                                    OnGrab(hd.draggerInteractDatas);
                                }
                            }
                            break;
                        case EGrabResultCmd.Release:
                            {
                                if (interactData is HoldedInteractData hd)
                                {
                                    OnRelease(hd.draggerInteractDatas);
                                }
                                if (!_useJoint)
                                {
                                    if ((_snapRule & ESnapRule.Release) == ESnapRule.Release)
                                    {
                                        Snap();
                                    }
                                }
                                break;
                            }
                        case EGrabResultCmd.Hold:
                            {
                                if (!_useJoint && interactData is HoldedInteractData hd)
                                {
                                    var data = hd.draggerInteractDatas.Values.FirstOrDefault();// 只使用第一个拖拽器作为驱动
                                    if (data != null)
                                    {
                                        DoMotion(data);

                                        if ((_snapRule & ESnapRule.Hold) == ESnapRule.Hold)
                                        {
                                            Snap();
                                        }
                                    }
                                }
                                break;
                            }
                        case EGrabResultCmd.TransformChanged:
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// 抓回调
        /// </summary>
        /// <param name="interactDatas"></param>
        protected virtual void OnGrab(Dictionary<Dragger, GrabInteractData> interactDatas) { }

        /// <summary>
        /// 放回调
        /// </summary>
        /// <param name="interactDatas"></param>
        protected virtual void OnRelease(Dictionary<Dragger, GrabInteractData> interactDatas) { }

        /// <summary>
        /// 执行运动
        /// </summary>
        /// <param name="grabInteractData"></param>
        protected abstract void DoMotion(GrabInteractData grabInteractData);

        /// <summary>
        /// 吸附
        /// </summary>
        protected abstract void Snap(); 

        #endregion
    }
}
