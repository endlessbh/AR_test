using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXGUI.Base;

namespace XCSJ.PluginXGUI.Views.InputFields
{
    /// <summary>
    /// 输入检查器关联可交互对象
    /// </summary>
    [DisallowMultipleComponent]
    [Name("输入框校验器关联可交互对象")]
    public class InputFieldValidatorToSelectable : View
    {
        [Name("输入框校验器")]
        [ComponentPopup]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public InputFieldValidator _inputFieldValidator = null;

        [Name("按钮")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Selectable _selectable = null;

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            if (_inputFieldValidator)
            {
                _inputFieldValidator.onValueChanged += OnInputFieldValueChanged;

                // 初始检查
                _inputFieldValidator.CheckAndTip();
            }
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            if (_inputFieldValidator)
            {
                _inputFieldValidator.onValueChanged -= OnInputFieldValueChanged;
            }
        }

        protected void OnInputFieldValueChanged(bool valid, string value)
        {
            if (_selectable)
            {
                _selectable.interactable = valid;
            }
        }
    }
}
