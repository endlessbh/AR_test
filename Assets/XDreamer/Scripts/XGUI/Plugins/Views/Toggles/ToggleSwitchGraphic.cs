using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXGUI.Base;

namespace XCSJ.PluginXGUI.Views.Toggles
{
    /// <summary>
    /// Toggle切换自身的目标图形
    /// </summary>
    [Name(" Toggle切换自身的目标图形")]
    public class ToggleSwitchGraphic : View
    {
        /// <summary>
        /// 切换
        /// </summary>
        [Name("切换")]
        [Tip("如当前参数无效，会从当前组件所在游戏对象上查找当前参数类型的组件", "If the current parameter is invalid, it will find the component of the current parameter type from the game object where the current component is located")]
        [ComponentPopup]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Toggle _toggle = null;

        [Name("切换图形类型")]
        [EnumPopup]
        public EGraphicType graphicType = EGraphicType.TargetGraphic;

        [Name("开时目标图形")]
        [ComponentPopup]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Graphic graphicWhenOn = null;

        [Name("关时目标图形")]
        [ComponentPopup]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Graphic graphicWhenOff = null;

        [Name("禁用时恢复原始目标图形")]
        public bool recoverWhenDisable = true;

        private Graphic _orgBackgroundGraphic = null;

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            if (!_toggle) _toggle = GetComponent<Toggle>();
            if (_toggle)
            {
                _toggle.onValueChanged.AddListener(OnValueChanged);
                _orgBackgroundGraphic = _toggle.targetGraphic;
            }
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            if (_toggle)
            {
                _toggle.onValueChanged.RemoveListener(OnValueChanged);
                if (recoverWhenDisable)
                {
                    switch (graphicType)
                    {
                        case EGraphicType.TargetGraphic: _toggle.targetGraphic = _orgBackgroundGraphic; break;
                        case EGraphicType.Graphic: _toggle.graphic = _orgBackgroundGraphic; break;
                        default: break;
                    }
                }
            }
        }

        /// <summary>
        /// 切换值变更事件
        /// </summary>
        /// <param name="value"></param>
        protected void OnValueChanged(bool value)
        {
            var graph = value ? graphicWhenOn : graphicWhenOff;
            switch (graphicType)
            {
                case EGraphicType.TargetGraphic: _toggle.targetGraphic = graph; break;
                case EGraphicType.Graphic: _toggle.graphic = graph; break;
                default: break;
            }
        }

        /// <summary>
        /// 图形类型
        /// </summary>
        [Name("图像类型")]
        public enum EGraphicType
        {
            /// <summary>
            /// 目标图形
            /// </summary>
            [Name("目标图形")]
            [Tip("Toggle背景图", "Toggle background")]
            TargetGraphic,

            /// <summary>
            /// 图形
            /// </summary>
            [Name("图形")]
            [Tip("Toggle前景图", "Toggle foreground")]
            Graphic,
        }
    }
}
