using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.Extension.Base.Dataflows.DataBinders;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginPhysicses.Tools.Gadgets;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.PluginPhysicses.States
{
    /// <summary>
    /// 物理关节触发器：物理机关档位与指定值相等时触发状态切换为完成态
    /// </summary>
    [ComponentMenu(PhysicsManager.Title + "/" + Title, typeof(PhysicsManager))]
    [Name(Title, nameof(PhysicsJointTrigger))]
    [Tip("物理关节档位与指定值相等时触发状态切换为完成态", "When the gear of the physical joint is equal to the specified value, the trigger state is switched to the completed state")]
    [XCSJ.Attributes.Icon(EIcon.JoyStick)]
    [DisallowMultipleComponent]
    [Owner(typeof(PhysicsManager))]
    public class PhysicsJointTrigger : Trigger<PhysicsJointTrigger>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "物理关节触发器";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(PhysicsManager.Title, typeof(PhysicsManager))]
        [StateComponentMenu(PhysicsManager.Title + "/" + Title, typeof(PhysicsManager))]
        [Name(Title, nameof(PhysicsJointTrigger))]
        [Tip("物理关节档位与指定值相等时触发状态切换为完成态", "When the gear of the physical joint is equal to the specified value, the trigger state is switched to the completed state")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 物理关节
        /// </summary>
        [Name("物理关节")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ComponentPopup]
        public PhysicsJoint _physicsJoint;

        /// <summary>
        /// 比较档位值
        /// </summary>
        [Name("比较档位值")]
        public bool _compareStepIndex = false;

        /// <summary>
        /// 档位值
        /// </summary>
        [Name("档位值")]
        [Tip("值需要在物理关节所设定的档位区间范围内")]
        [HideInSuperInspector(nameof(_compareStepIndex), EValidityCheckType.False)]
        public PositiveIntPropertyValue _stepIndex = new PositiveIntPropertyValue(0);

        /// <summary>
        /// 档位值变量
        /// </summary>
        [Name("档位值变量")]
        [VarString]
        public string _stepVar;

        /// <summary>
        /// 进入
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnEntry(StateData stateData)
        {
            base.OnEntry(stateData);

            PhysicsJoint._onStepChanged += OnStepChanged;
        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnExit(StateData stateData)
        {
            base.OnExit(stateData);

            PhysicsJoint._onStepChanged -= OnStepChanged;
        }

        /// <summary>
        /// 数据有效
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity() => _physicsJoint;

        /// <summary>
        /// 提示字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString()
        {
            var tip = _physicsJoint ? _physicsJoint.name : "";
            if (_compareStepIndex && _stepIndex.TryGetValue(out var value))
            {
                tip += ":" + value;
            }
            return tip;
        }

        private void OnStepChanged(PhysicsJoint physicsJoint, int stepIndex)
        {
            if (_physicsJoint == physicsJoint)
            {
                if (_compareStepIndex)
                {
                    if (_stepIndex.TryGetValue(out var value))
                    {
                        finished = value == stepIndex;
                    }
                }
                else
                {
                    finished = true;
                }

                if (finished)
                {
                    _stepVar.TrySetOrAddSetHierarchyVarValue(_stepIndex);
                }
            }
        }
    }
}