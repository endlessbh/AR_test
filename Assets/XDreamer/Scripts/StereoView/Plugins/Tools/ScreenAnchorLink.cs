using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.Maths;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginTools;
using XCSJ.PluginTools.Renderers;

namespace XCSJ.PluginStereoView.Tools
{
    /// <summary>
    /// 屏幕锚点关联
    /// </summary>
    [Name(Title)]
    [ExecuteInEditMode]
    [RequireManager(typeof(StereoViewManager))]
    public class ScreenAnchorLink : InteractProvider, IGizmoRendererValue
    {
        Vector3 IGizmoRendererValue.value => Vector3.one;
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "屏幕锚点关联";

        /// <summary>
        /// 基准屏幕
        /// </summary>
        [Name("基准屏幕")]
        [ComponentPopup]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public BaseScreen _standardScreen;

        /// <summary>
        /// 基准屏幕
        /// </summary>
        public BaseScreen standardScreen
        {
            get => _standardScreen;
            set => this.XModifyProperty(ref _standardScreen, value);
        }

        /// <summary>
        /// 基准屏幕锚点
        /// </summary>
        [Name("基准屏幕锚点")]
        [EnumPopup]
        public ERectAnchor _standardScreenAnchor = ERectAnchor.Center;

        /// <summary>
        /// 基准屏幕锚点
        /// </summary>
        public ERectAnchor standardScreenAnchor
        {
            get => _standardScreenAnchor;
            set => this.XModifyProperty(ref _standardScreenAnchor, value);
        }

        /// <summary>
        /// 基准屏幕锚点偏移量
        /// </summary>
        [Name("基准屏幕锚点偏移量")]
        public Vector3 _standardScreenAnchorOffset = default;

        /// <summary>
        /// 基准屏幕锚点偏移空间类型
        /// </summary>
        [Name("基准屏幕锚点偏移空间类型")]
        [EnumPopup]
        public ESpaceType _standardScreenAnchorOffsetSpaceType = ESpaceType.Local;

        /// <summary>
        /// 屏幕
        /// </summary>
        [Name("屏幕")]
        [Tip("默认使用当前游戏对象上的屏幕对象作为当前参数对象", "By default, the screen object on the current GameObject is used as the current parameter object")]
        [ComponentPopup]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public BaseScreen _screen;

        /// <summary>
        /// 当前屏幕
        /// </summary>
        public BaseScreen screen => this.XGetComponent<BaseScreen>(ref _screen);

        /// <summary>
        /// 屏幕锚点
        /// </summary>
        [Name("屏幕锚点")]
        [EnumPopup]
        public ERectAnchor _screenAnchor = ERectAnchor.Center;

        /// <summary>
        /// 屏幕锚点
        /// </summary>
        public ERectAnchor screenAnchor
        {
            get => _screenAnchor;
            set => this.XModifyProperty(ref _screenAnchor, value);
        }

        /// <summary>
        /// 屏幕锚点偏移量
        /// </summary>
        [Name("屏幕锚点偏移量")]
        public Vector3 _screenAnchorOffset = default;

        /// <summary>
        /// 屏幕锚点偏移空间类型
        /// </summary>
        [Name("屏幕锚点偏移空间类型")]
        [EnumPopup]
        public ESpaceType _screenAnchorOffsetSpaceType = ESpaceType.Local;

        /// <summary>
        /// 关联旋转
        /// </summary>
        [Name("关联旋转")]
        public Vector3 _linkRotation = new Vector3();

        /// <summary>
        /// 关联旋转
        /// </summary>
        public Vector3 linkRotation
        {
            get => _linkRotation;
            set
            {
                this.XModifyProperty(ref _linkRotation, value);
                UpdateScreen();
            }
        }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            BaseScreen.onScreenChanged += OnScreenChanged;
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            BaseScreen.onScreenChanged -= OnScreenChanged;
        }

        /// <summary>
        /// 当场景变更时调用
        /// </summary>
        /// <param name="screen"></param>
        protected virtual void OnScreenChanged(BaseScreen screen)
        {
            //Debug.Log("当更新屏幕: " + name);
            if (screen != _standardScreen && screen != this.screen) return;
            UpdateScreen();
        }

        /// <summary>
        /// 更新屏幕
        /// </summary>
        public void UpdateScreen()
        {
            //Debug.Log("更新屏幕: " + name);
            StereoViewHelper.AnchorLink(_standardScreen, _standardScreenAnchor, screen, _screenAnchor, _linkRotation, _standardScreenAnchorOffset, _standardScreenAnchorOffsetSpaceType, _screenAnchorOffset, _screenAnchorOffsetSpaceType);
        }

        public void OnValidate()
        {
            UpdateScreen();
        }

        /// <summary>
        /// 选中时绘制Gizmos
        /// </summary>
        public void OnDrawGizmosSelected()
        {
            var color = Gizmos.color;
            if (_standardScreen)
            {
                var r = Mathf.Max(_standardScreen.screenSize.magnitude * 0.01f, 0.01f);

                Gizmos.color = Color.red;
                Gizmos.DrawSphere(_standardScreen.GetAnchorPosition(_standardScreenAnchor, _standardScreenAnchorOffset, _standardScreenAnchorOffsetSpaceType), r);

                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(_standardScreen.GetAnchorPosition(_standardScreenAnchor), r);
            }

            var screen = this.screen;
            if (screen)
            {
                var r = Mathf.Max(screen.screenSize.magnitude * 0.01f, 0.01f);

                Gizmos.color = Color.red;
                Gizmos.DrawSphere(screen.GetAnchorPosition(_screenAnchor, _screenAnchorOffset, _screenAnchorOffsetSpaceType), r);

                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(screen.GetAnchorPosition(_screenAnchor), r);
            }
            Gizmos.color = color;
        }

        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            if (!screen) { }
        }
    }
}
 