using System;
using System.Reflection;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginXGUI.Base;
using XCSJ.PluginXGUI.DataViews.Base;

namespace XCSJ.PluginXGUI.DataViews.TypeViews
{
    /// <summary>
    /// 方法按钮数据视图：通过按钮点击调用绑定方法
    /// </summary>
    [Name("方法按钮数据视图")]
    [DataViewAttribute(typeof(MethodInfo))]
#if UNITY_EDITOR && XDREAMER_EDITION_DEVELOPER
    //[Tool(XGUICategory.DataTypeView, rootType = typeof(XGUIManager))]
#endif
    public sealed class MethodButtonView : BaseModelViewCache
    {
        /// <summary>
        /// 按钮
        /// </summary>
        [Name("按钮")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Button _callMethodButton;

        private Button buttonOnEnable;

        /// <summary>
        /// 启用：绑定UI事件
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            _modelViewDataLinkMode = 0;

            if (_callMethodButton)
            {
                buttonOnEnable = _callMethodButton;
                buttonOnEnable.onClick.AddListener(OnClick);
            }
        }

        /// <summary>
        /// 禁用：解除UI事件绑定
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            if (buttonOnEnable)
            {
                buttonOnEnable.onClick.RemoveListener(OnClick);
                buttonOnEnable = null;
            }
        }

        private void OnClick()
        {
            _fieldPropertyMethodBinder.methodInfo?.Invoke(_fieldPropertyMethodBinder.mainObject, null);
        }

        /// <summary>
        /// 视图值类型：按钮无视图值类型
        /// </summary>
        public override Type viewValueType => null;

        /// <summary>
        /// 视图值：按钮无视图值
        /// </summary>
        public override object viewValue { get => null; set { } }

        /// <summary>
        /// 设置标题
        /// </summary>
        /// <param name="title"></param>
        public override void SetLabelText(string title)
        {
            base.SetLabelText(title);

            if (_callMethodButton.TryGetComponent<Text>(out var text))
            {
                text.text = _fieldPropertyMethodBinder.memberName;
            }
        }
    }
}
