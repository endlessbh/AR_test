using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Extension.Interactions.Base;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginTools.Base;

namespace XCSJ.PluginTools.Inputs
{
    #region 输入交互器

    /// <summary>
    /// 基础输入
    /// </summary>
    public abstract class BaseInput : Interactor { }

    /// <summary>
    /// 基础输入模版类
    /// </summary>
    /// <typeparam name="TInputCmd"></typeparam>
    public abstract class BaseInput<TInputCmd> : BaseInput where TInputCmd : InputCmd, new()
    {
        /// <summary>
        /// 命令触发器列表
        /// </summary>
        protected abstract IEnumerable<TInputCmd> inputCmds { get; }
    }

    /// <summary>
    /// 射线规则
    /// </summary>
    public enum ERayRule
    {
        /// <summary>
        /// 无
        /// </summary>
        [Name("无")]
        None = 0,

        /// <summary>
        /// 使用射线碰撞器
        /// </summary>
        [Name("使用射线碰撞器")]
        UseRay = 1,
    }

    /// <summary>
    /// 射线输入：产生射线并检测命令是否触发
    /// </summary>
    public abstract class RayInput<TRayCmd> : BaseInput<TRayCmd> where TRayCmd : RayCmd, new()
    {
        /// <summary>
        /// 射线规则
        /// </summary>
        [Name("射线规则")]
        [EnumPopup]
        public ERayRule _rayRule = ERayRule.UseRay;

        /// <summary>
        /// 射线碰撞器
        /// </summary>
        [Name("射线碰撞器")]
        [HideInSuperInspector(nameof(_rayRule), EValidityCheckType.NotEqual, ERayRule.UseRay)]
        public RayHitter _rayHitter = new RayHitter();

        /// <summary>
        /// 重置
        /// </summary>
        public virtual void Reset() => _rayHitter.OnReset(transform);

        /// <summary>
        /// 更新
        /// </summary>
        protected virtual void Update()
        {
            OnCheckCmds();
        }

        /// <summary>
        /// 检查触发器
        /// </summary>
        protected virtual void OnCheckCmds()
        {
            foreach (var inputCmd in inputCmds)
            {
                OnCheckCmd(inputCmd);
            }
        }

        /// <summary>
        /// 尝试创建射线交互数据
        /// </summary>
        /// <param name="rayCmd"></param>
        protected virtual void OnCheckCmd(TRayCmd rayCmd)
        {
            if (rayCmd.CanStandardInput())
            {
                if (!rayCmd.needInvokeInteract) return;

                var rayInteractData = CreateRayInteractData(this, rayCmd, rayCmd.cmdName, rayCmd.GetRay());

                if (rayCmd.CanInteract(rayInteractData))
                {
                    TryInteract(rayInteractData, out _);
                }
            }
        }

        /// <summary>
        /// 能否交互:默认不使用基类交互验证，允许可交互对象为空的情况
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        public override bool CanInteract(InteractData interactData) => true; 

        /// <summary>
        /// 进行交互:默认不是使用基类交互处理，返回完成态<see cref="EInteractResult.Finished"/>
        /// </summary>
        /// <param name="interactData"></param>
        protected override EInteractResult OnInteract(InteractData interactData) => EInteractResult.Finished;

        /// <summary>
        /// 创建交互数据
        /// </summary>
        /// <param name="cmdName"></param>
        /// <param name="interactables"></param>
        /// <returns></returns>
        protected override InteractData CreateInteractData(string cmdName, params InteractObject[] interactables)
        {
            return new RayInteractData(cmdName, this, interactables);
        }

        /// <summary>
        /// 创建射线交互数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="rayCmd"></param>
        /// <param name="cmdName"></param>
        /// <param name="ray"></param>
        /// <returns></returns>
        public RayInteractData CreateRayInteractData(UnityEngine.Object sender, TRayCmd rayCmd, string cmdName, Ray? ray = null)
        {
            Ray? hitRay = null;
            RaycastHit? raycastHit = null;
            var interactables = Empty<InteractObject>.Array;

            switch (_rayRule)
            {
                case ERayRule.UseRay:
                    {
                        if (ray.HasValue)
                        {
                            hitRay = ray.Value;
                        }
                        else if (_rayHitter.TryGetRay(out var r))
                        {
                            hitRay = r;
                        }

                        if (hitRay.HasValue && _rayHitter.TryGetHit(hitRay.Value, out var hit))
                        {
                            raycastHit = hit;

                            // 优先使用碰撞体去获取组件更为准确。因为碰撞体所在游戏对象本身没有刚体，而父级有刚体时 hit.transform 为父级对象
                            var entity = hit.collider.GetComponentInParent<InteractableEntity>();
                            if (entity)
                            {
                                interactables = new InteractObject[] { entity };
                            }
                            else
                            {
                                entity = hit.transform.GetComponentInParent<InteractableEntity>();
                                if (entity)
                                {
                                    interactables = new InteractObject[] { entity };
                                }
                            }

                        }
                        break;
                    }
            }

            return CreateRayInteractData(sender, rayCmd, hitRay, raycastHit, _rayHitter._maxDistance, _rayHitter._layerMask, cmdName, this, interactables);
        }

        /// <summary>
        /// 创建射线交互数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="rayCmd"></param>
        /// <param name="ray"></param>
        /// <param name="raycastHit"></param>
        /// <param name="rayMaxDistance"></param>
        /// <param name="layerMask"></param>
        /// <param name="cmdName"></param>
        /// <param name="interactor"></param>
        /// <param name="interactables"></param>
        /// <returns></returns>
        protected virtual RayInteractData CreateRayInteractData(UnityEngine.Object sender, TRayCmd rayCmd, Ray? ray, RaycastHit? raycastHit, float rayMaxDistance, LayerMask layerMask, string cmdName, InteractObject interactor, params InteractObject[] interactables)
        {
            return new RayInteractData(sender, ray, raycastHit, rayMaxDistance, layerMask, cmdName, this, interactables);
        }
    }

    /// <summary>
    /// 射线输入：按压触发命令需有按下、保持和弹起状态
    /// </summary>
    /// <typeparam name="TRayCmd"></typeparam>
    /// <typeparam name="TCmds"></typeparam>
    public abstract class RayInput<TRayCmd, TCmds> : RayInput<TRayCmd>
        where TRayCmd : RayCmd, new()
        where TCmds : Cmds<TRayCmd>, new()
    {
        /// <summary>
        /// 命令列表字符串
        /// </summary>
        public override List<string> cmds => _cmds.cmdNames;

        /// <summary>
        /// 命令列表
        /// </summary>
        [Name("命令列表")]
        [OnlyMemberElements]
        public TCmds _cmds = new TCmds();

        /// <summary>
        /// 命令触发器列表
        /// </summary>
        protected override IEnumerable<TRayCmd> inputCmds => _cmds._cmds;

        /// <summary>
        /// 批量设置命令输入模式
        /// </summary>
        /// <param name="inputMode"></param>
        public void SetInputMode(RayCmd.EInputMode inputMode)
        {
            foreach (var item in _cmds._cmds)
            {
                item._inputMode = inputMode;
            }
        }

        /// <summary>
        /// 批量设置命令UI规则
        /// </summary>
        /// <param name="uiRule"></param>
        public void SetUIRule(RayCmd.EUIRule uiRule)
        {
            foreach (var item in _cmds._cmds)
            {
                item._uiRule = uiRule;
            }
        }
    }

    /// <summary>
    /// 模拟射线输入
    /// </summary>
    /// <typeparam name="TComponent"></typeparam>
    /// <typeparam name="TComponentProvider"></typeparam>
    public abstract class AnalogRayInput<TComponent, TComponentProvider> : ComponentInteractor<TComponent, TComponentProvider>
        where TComponent : Component, new()
        where TComponentProvider : ComponentProvider<TComponent>, new()
    {
        /// <summary>
        /// 射线生成器
        /// </summary>
        [Name("射线生成器")]
        public RayGenerater _rayGenerater = new RayGenerater();

        /// <summary>
        /// 重置
        /// </summary>
        public virtual void Reset()
        {
            // 默认设置为点和方向类型
            _rayGenerater._referenceRule = PluginTools.Base.EReferenceRule.PointDirectionData;
            _rayGenerater.OnReset(transform);
        }
    }

    #endregion

    #region 输入交互数据

    /// <summary>
    /// 输入交互数据
    /// </summary>
    public class InputInteractData : InteractData<InputInteractData>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public InputInteractData() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="cmdName"></param>
        /// <param name="interactor"></param>
        /// <param name="interactables"></param>
        public InputInteractData(string cmdName, InteractObject interactor, params InteractObject[] interactables) : base(cmdName, interactor, interactables) { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="cmdName"></param>
        /// <param name="parent"></param>
        /// <param name="interactor"></param>
        /// <param name="interactables"></param>
        public InputInteractData(string cmdName, InteractData parent, InteractObject interactor, params InteractObject[] interactables) : base(cmdName, parent, interactor, interactables) { }
    }

    /// <summary>
    /// 射线输入交互数据
    /// </summary>
    public class RayInteractData : InputInteractData
    {
        /// <summary>
        /// 发送者：空时不是模拟数据
        /// </summary>
        public UnityEngine.Object sender { get; private set; }

        /// <summary>
        /// 射线
        /// </summary>
        public Ray? ray { get; internal set; } = null;

        /// <summary>
        /// 产生射线碰撞结果
        /// </summary>
        public RaycastHit? raycastHit { get; internal set; } = null;

        /// <summary>
        /// 射线最大距离
        /// </summary>
        public float rayMaxDistance { get; private set; } = Mathf.Infinity;

        /// <summary>
        /// 射线层
        /// </summary>
        public LayerMask layerMask { get; private set; } = Physics.DefaultRaycastLayers;

        /// <summary>
        /// 默认构造函数
        /// </summary>
        protected RayInteractData() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="cmdName"></param>
        /// <param name="interactor"></param>
        /// <param name="interactables"></param>
        public RayInteractData(string cmdName, InteractObject interactor, params InteractObject[] interactables) : base(cmdName, interactor, interactables) { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="cmdName"></param>
        /// <param name="parent"></param>
        /// <param name="interactor"></param>
        /// <param name="interactables"></param>
        public RayInteractData(string cmdName, InteractData parent, InteractObject interactor, params InteractObject[] interactables) : base(cmdName, parent, interactor, interactables) { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="ray"></param>
        /// <param name="raycastHit"></param>
        /// <param name="rayMaxDistance"></param>
        /// <param name="layerMask"></param>
        /// <param name="cmdName"></param>
        /// <param name="interactor"></param>
        /// <param name="interactables"></param>
        public RayInteractData(UnityEngine.Object sender, Ray? ray, RaycastHit? raycastHit, float rayMaxDistance, LayerMask layerMask, string cmdName, InteractObject interactor, params InteractObject[] interactables) : this(sender, ray, raycastHit, rayMaxDistance, layerMask, cmdName, null, interactor, interactables)
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="ray"></param>
        /// <param name="raycastHit"></param>
        /// <param name="rayMaxDistance"></param>
        /// <param name="layerMask"></param>
        /// <param name="cmdName"></param>
        /// <param name="parent"></param>
        /// <param name="interactor"></param>
        /// <param name="interactables"></param>
        public RayInteractData(UnityEngine.Object sender, Ray? ray, RaycastHit? raycastHit, float rayMaxDistance, LayerMask layerMask, string cmdName, InteractData parent, InteractObject interactor, params InteractObject[] interactables) : base(cmdName, parent, interactor, interactables)
        {
            this.sender = sender ? sender : interactor as UnityEngine.Object;// 为空时使用传入交互器
            this.ray = ray;
            this.raycastHit = raycastHit;
            this.rayMaxDistance = rayMaxDistance;
            this.layerMask = layerMask;
        }

        /// <summary>
        /// 创建实例
        /// </summary>
        /// <returns></returns>
        protected override InteractData CreateInstance() => new RayInteractData();

        /// <summary>
        /// 复制
        /// </summary>
        /// <param name="interactData"></param>
        public override void CopyTo(InteractData interactData)
        {
            base.CopyTo(interactData);

            if (interactData is RayInteractData rayInteractData)
            {
                rayInteractData.sender = sender;
                rayInteractData.ray = ray;
                rayInteractData.raycastHit = raycastHit;
                rayInteractData.rayMaxDistance = rayMaxDistance;
                rayInteractData.layerMask = layerMask;
            }
        }

        /// <summary>
        /// 射线拾取的变换
        /// </summary>
        public Transform rayHitTransform
        {
            get
            {
                return raycastHit.HasValue ? raycastHit.Value.transform : null;
            }
        }

        /// <summary>
        /// 尝试获取碰撞点的距离
        /// </summary>
        /// <param name="to"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public bool TryGetSameHitEntityDistance(RayInteractData to, out float distance)
        {
            distance = default;
            if (to == null) return false;

            var hit = rayHitTransform;
            var toHit = to.rayHitTransform;

            if (!hit || !toHit) return false;
            if (hit != toHit) return false;

            distance = Vector3.Distance(raycastHit.Value.point, to.raycastHit.Value.point);
            return true;
        }
    }

    #endregion

    #region 输入命令

    /// <summary>
    /// 命令触发器接口
    /// </summary>
    public interface IInputCmd : ICmdName
    {
        /// <summary>
        /// 发送者
        /// </summary>
        UnityEngine.Object sender { get; }

        /// <summary>
        /// 启用
        /// </summary>
        bool enabled { get; }

        /// <summary>
        /// 是否需要执行交互
        /// </summary>
        bool needInvokeInteract { get; }

        /// <summary>
        /// 能否交互
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        bool CanInteract(InteractData interactData);

        /// <summary>
        /// 尝试获取射线
        /// </summary>
        /// <param name="ray"></param>
        /// <returns></returns>
        Ray? GetRay();
    }

    /// <summary>
    /// 输入命令
    /// </summary>
    public abstract class InputCmd : Cmd, IInputCmd
    {
        /// <summary>
        /// 发送者
        /// </summary>
        public UnityEngine.Object sender { get; protected set; }

        /// <summary>
        /// 启用
        /// </summary>
        public abstract bool enabled { get; }

        /// <summary>
        /// 需调用交互
        /// </summary>
        public abstract bool needInvokeInteract { get; }

        /// <summary>
        /// 能否交互
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        public abstract bool CanInteract(InteractData interactData);

        /// <summary>
        /// 获取射线
        /// </summary>
        /// <returns></returns>
        public virtual Ray? GetRay() => default;
    }

    /// <summary>
    /// 按下状态
    /// </summary>
    public enum EPressState
    {
        [Name("总是成立")]
        Always = -1,

        [Name("无")]
        None,

        [Name("按下")]
        Pressed,

        [Name("保持")]
        Keeping,

        [Name("弹起")]
        Released,

        [Name("按下并弹起")]
        [Tip("按下和弹起时射线拾取到的对象为同一个对象，并且两次点击点偏差距离小于限定值，则成立", "If the object picked up by ray is the same when pressed and popped, and the deviation distance between two clicks is less than the limit value, then it is true")]
        PressedAndReleased,

        [Name("非保持")]
        NoKeeping,

        [Name("保持延时且存在可交互对象")]
        [Tip("保持态下延时固定时间且存在可交互对象进行触发", "In the hold state, the delay is fixed and there are interactive objects for triggering")]
        KeepingAndDelayTimeAndHasInteractable,

        [Name("非保持延时且存在可交互对象")]
        [Tip("非保持态下延时固定时间且存在可交互对象进行触发", "In the non hold state, the delay is fixed and there are interactive objects for triggering")]
        NoKeepingAndDelayTimeAndHasInteractable,
    }

    /// <summary>
    /// 按压命令触发器
    /// </summary>
    public abstract class RayCmd : InputCmd
    {
        /// <summary>
        /// 输入模式
        /// </summary>
        [Flags]
        public enum EInputMode
        {
            [Name("标准")]
            [Tip("直接读取本地硬件输入", "Read local hardware input directly")]
            Standard = 1 << 0,

            [Name("模拟")]
            [Tip("外部发送的模拟输入", "Analog inputs sent externally")]
            Analog = 1 << 1,
        }

        /// <summary>
        /// 输入模式
        /// </summary>
        [Name("输入模式")]
        [EnumPopup]
        public EInputMode _inputMode = EInputMode.Standard | EInputMode.Analog;

        /// <summary>
        /// UI处理规则
        /// </summary>
        public enum EUIRule
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None = 0,

            /// <summary>
            /// 在任意UI上无效
            /// </summary>
            [Name("在任意UI上无效")]
            InvalidOnAnyUI,

            /// <summary>
            /// 仅标准时在任意UI上无效
            /// </summary>
            [Name("仅标准时在任意UI上无效")]
            InvalidOnAnyUIOnlyStandard,

            /// <summary>
            /// 仅模拟时在任意UI上无效
            /// </summary>
            [Name("仅模拟时在任意UI上无效")]
            InvalidOnAnyUIOnlyOnAnalog,

            /// <summary>
            /// 在任意UI上有效
            /// </summary>
            [Name("在任意UI上有效")]
            ValidOnAnyUI,

            /// <summary>
            /// 仅标准在任意UI上有效
            /// </summary>
            [Name("仅标准在任意UI上有效")]
            ValidOnAnyUIOnlyStandard,

            /// <summary>
            /// 仅模拟在任意UI上有效
            /// </summary>
            [Name("仅模拟在任意UI上有效")]
            ValidOnAnyUIOnlyOnAnalog,
        }

        /// <summary>
        /// UI规则
        /// </summary>
        [Name("UI规则")]
        [EnumPopup]
        public EUIRule _uiRule = EUIRule.InvalidOnAnyUI;

        /// <summary>
        /// 可使用标准输入
        /// </summary>
        public bool CanStandardInput() => (_inputMode & EInputMode.Standard) == EInputMode.Standard && CanUIRule(true);

        /// <summary>
        /// 可使用模拟输入
        /// </summary>
        public bool CanAnalogInput() => (_inputMode & EInputMode.Analog) == EInputMode.Analog && CanUIRule(false);

        private bool CanUIRule(bool standardMode)
        {
            switch (_uiRule)
            {
                case EUIRule.None: return true;// 不处理UI
                case EUIRule.InvalidOnAnyUI: return !CommonFun.IsOnUINow();
                case EUIRule.InvalidOnAnyUIOnlyStandard: return standardMode ? !CommonFun.IsOnUINow() : true;
                case EUIRule.InvalidOnAnyUIOnlyOnAnalog: return !standardMode ? !CommonFun.IsOnUINow() : true;
                case EUIRule.ValidOnAnyUI: return CommonFun.IsOnUINow();
                case EUIRule.ValidOnAnyUIOnlyStandard: return standardMode ? CommonFun.IsOnUINow() : true;
                case EUIRule.ValidOnAnyUIOnlyOnAnalog: return !standardMode ? CommonFun.IsOnUINow() : true;
                default: return false;
            }
        }

        /// <summary>
        /// 启用
        /// </summary>
        public override bool enabled => _inputMode != 0;

        /// <summary>
        /// 触发状态
        /// </summary>
        [Name("触发状态")]
        [EnumPopup]
        public EPressState _triggerState = EPressState.Released;

        /// <summary>
        /// 识别拾取最大偏移距离
        /// </summary>
        [Name("识别拾取最大偏移距离")]
        [Tip("开始和结束拾取的位置距离小于当前值为有效拾取，否则认为是无效拾取", "If the distance between the start and end pickup positions is less than the current value, it is considered as valid pickup, otherwise it is considered as invalid pickup")]
        [Min(0)]
        [HideInSuperInspector(nameof(_triggerState), EValidityCheckType.NotEqual, EPressState.PressedAndReleased)]
        [Readonly(EEditorMode.Runtime)]
        public float _pressMaxDistance = 10;

        /// <summary>
        /// 固定时长
        /// </summary>
        [Name("固定时长")]
        [Min(0)]
        public float _fixedTime = 3;

        /// <summary>
        /// 按下
        /// </summary>
        protected abstract bool Pressed();

        /// <summary>
        /// 保持
        /// </summary>
        protected abstract bool Keep();

        /// <summary>
        /// 弹起
        /// </summary>
        protected abstract bool Release();

        private bool triggerPressed { get; set; } = false;

        /// <summary>
        /// 能否调用交互
        /// </summary>
        public override bool needInvokeInteract
        {
            get
            {
                switch (_triggerState)
                {
                    case EPressState.None: return false;
                    case EPressState.Always: return true;
                    case EPressState.Pressed: return Pressed();
                    case EPressState.Keeping: return Keep();
                    case EPressState.Released: return Release();
                    case EPressState.PressedAndReleased:
                        {
                            if (Pressed())
                            {
                                triggerPressed = true;
                                return true;
                            }

                            if (Release())
                            {
                                triggerPressed = false;
                                return true;
                            }
                            return false;
                        }
                    case EPressState.NoKeeping: return !Keep();
                    case EPressState.KeepingAndDelayTimeAndHasInteractable: return Keep();
                    case EPressState.NoKeepingAndDelayTimeAndHasInteractable: return !Keep();
                    default: return false;
                }
            }
        }

        private RayInteractData lastInteractData = null;
        private float timeCounter;

        public override bool CanInteract(InteractData interactData)
        {
            switch (_triggerState)
            {
                case EPressState.None: return false;
                case EPressState.Always:
                case EPressState.Pressed:
                case EPressState.Keeping:
                case EPressState.Released: return true;
                case EPressState.PressedAndReleased:
                    {
                        if (triggerPressed)
                        {
                            lastInteractData = interactData as RayInteractData;
                            return false;
                        }

                        if (lastInteractData != null)
                        {
                            var beginData = lastInteractData;
                            lastInteractData = null;

                            var endData = interactData as RayInteractData;

                            if (endData == null) return false;

                            return (beginData.interactable == null && endData.interactable == null) ||
                                beginData.TryGetSameHitEntityDistance(endData, out var dis) && dis <= Mathf.Max(0.01f, _pressMaxDistance);
                        }
                        return false;
                    }
                case EPressState.NoKeeping: return true;
                case EPressState.KeepingAndDelayTimeAndHasInteractable:
                case EPressState.NoKeepingAndDelayTimeAndHasInteractable:
                    {
                        if (interactData.interactable)
                        {
                            // 当对象不同时重新开始计时
                            if (lastInteractData == null || lastInteractData.interactable != interactData.interactable)
                            {
                                lastInteractData = interactData as RayInteractData;
                                timeCounter = 0;
                            }
                            // 计算运动
                            if (lastInteractData != null)
                            {
                                timeCounter += Time.deltaTime;
                            }
                        }
                        else// 当可交互对象为空时重新计时
                        {
                            timeCounter = 0;
                        }
                        return timeCounter > _fixedTime;
                    }
                default: return false;
            }
        }
    }

    /// <summary>
    /// 模拟命令：用于外部模拟输入
    /// </summary>
    public class AnalogCmd
    {
        /// <summary>
        /// 获取射线
        /// </summary>
        /// <param name="ray"></param>
        /// <returns></returns>
        public Ray? GetRay()
        {
            return (_currentRay != null && _currentRay.HasValue) ? _currentRay : _defaultRay;
        }

        private bool lastPressedValue = false;

        public void SetPressState(bool inPressed, Ray? ray)
        {
            // 上次值为真
            if (lastPressedValue)
            {
                // pressed这次为假;keep为这次值 released为这次值取反
                SetState(false, inPressed, !inPressed, ray);
            }
            else// 上次值为假
            {
                SetState(inPressed, false, false, ray);
            }

            lastPressedValue = inPressed;// 更新上次值
        }

        /// <summary>
        /// 外部模拟设置命令的按压或射线状态
        /// </summary>
        /// <param name="pressed"></param>
        /// <param name="keep"></param>
        /// <param name="released"></param>
        /// <param name="ray"></param>
        private void SetState(bool pressed, bool keep, bool released, Ray? ray)
        {
            if (pressed)
            {
                _pressed = pressed;
                _pressedRay = ray;
                return;
            }
            else if (released)
            {
                _released = released;
                _releasedRay = ray;
                return;
            }
            else if (keep)
            {
                _keep = keep;
            }
            _defaultRay = ray;
        }

        private bool _pressed = false;
        private bool _keep = false;
        private bool _released = false;

        private Ray? _currentRay;
        private Ray? _pressedRay;
        private Ray? _defaultRay;
        private Ray? _releasedRay;

        /// <summary>
        /// 按下
        /// </summary>
        public bool pressed
        {
            get
            {
                var rs = _pressed;
                _pressed = false;// 使用完重置为false
                _currentRay = _pressedRay;
                return rs;
            }
        }

        /// <summary>
        /// 保持
        /// </summary>
        public bool keep
        {
            get
            {
                var rs = _keep;
                _keep = false;// 使用完重置为false
                _currentRay = _defaultRay;
                return rs;
            }
        }

        /// <summary>
        /// 释放
        /// </summary>
        public bool released
        {
            get
            {
                var rs = _released;
                _released = false;// 使用完重置为false
                _currentRay = _releasedRay;
                return rs;
            }
        }

        private bool triggerPressed { get; set; } = false;

        public bool NeedInvokeInteract(RayCmd rayCmd)
        {
            switch (rayCmd._triggerState)
            {
                case EPressState.None: return false;
                case EPressState.Always: return true;
                case EPressState.Pressed: return pressed;
                case EPressState.Keeping: return keep;
                case EPressState.Released: return released;
                case EPressState.PressedAndReleased:
                    {
                        if (pressed)
                        {
                            triggerPressed = true;
                            return true;
                        }

                        if (released)
                        {
                            triggerPressed = false;
                            return true;
                        }
                        return false;
                    }
                case EPressState.NoKeeping: return !keep;
                case EPressState.KeepingAndDelayTimeAndHasInteractable: return keep;
                case EPressState.NoKeepingAndDelayTimeAndHasInteractable: return !keep;
                default: return false;
            }
        }

        private RayInteractData lastInteractData = null;
        private float timeCounter;

        public bool CanInteract(InteractData interactData, RayCmd rayCmd)
        {
            switch (rayCmd._triggerState)
            {
                case EPressState.None: return false;
                case EPressState.Always:
                case EPressState.Pressed:
                case EPressState.Keeping:
                case EPressState.Released: return true;
                case EPressState.PressedAndReleased:
                    {
                        if (triggerPressed)
                        {
                            lastInteractData = interactData as RayInteractData;
                            return false;
                        }

                        if (lastInteractData != null)
                        {
                            var beginData = lastInteractData;
                            lastInteractData = null;

                            var endData = interactData as RayInteractData;

                            if (endData == null) return false;

                            return (beginData.interactable == null && endData.interactable == null) || beginData.TryGetSameHitEntityDistance(endData, out var dis) && dis <= Mathf.Max(0.01f, rayCmd._pressMaxDistance);
                        }
                        return false;
                    }
                case EPressState.NoKeeping: return true;
                case EPressState.KeepingAndDelayTimeAndHasInteractable:
                case EPressState.NoKeepingAndDelayTimeAndHasInteractable:
                    {
                        if (interactData.interactable)
                        {
                            // 当对象不同时重新开始计时
                            if (lastInteractData == null || lastInteractData.interactable != interactData.interactable)
                            {
                                lastInteractData = interactData as RayInteractData;
                                timeCounter = 0;
                            }
                            // 计算运动
                            if (lastInteractData != null)
                            {
                                timeCounter += Time.deltaTime;
                            }
                        }
                        else// 当可交互对象为空时重新计时
                        {
                            timeCounter = 0;
                        }
                        return timeCounter > rayCmd._fixedTime;
                    }
                default: return false;
            }
        }
    } 

    #endregion
}
