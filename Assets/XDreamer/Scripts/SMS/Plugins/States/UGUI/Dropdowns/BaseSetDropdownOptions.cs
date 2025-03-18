using System.Collections.Generic;
using XCSJ.Attributes;
using XCSJ.Extension.XGUI.Dropdowns;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;
using XCSJ.PluginXGUI.Views.Dropdowns;
using static UnityEngine.UI.Dropdown;

namespace XCSJ.PluginSMS.States.UGUI.Dropdowns
{
    /// <summary>
    /// 基础设置下拉框选项列表
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseSetDropdownOptions<T> : BaseSetDropdownProperty<T>, ISetDropdownOptions
        where T : BaseSetDropdownOptions<T>
    {
        /// <summary>
        /// 设置选项列表规则
        /// </summary>
        [Name("设置选项列表规则")]
        [Tip("设置选项列表时需遵守的规则", "Rules to follow when setting the option list")]
        [EnumPopup]
        public ESetOptionsRule _setOptionsRule = ESetOptionsRule.ClearAndAdd;

        /// <summary>
        /// 设置选项列表规则
        /// </summary>
        public ESetOptionsRule setOptionsRule { get => _setOptionsRule; set => _setOptionsRule = value; }

        /// <summary>
        /// 选项数据列表：用于设置下拉框选项的选项数据列表
        /// </summary>
        public abstract List<OptionData> optionDatas { get; }

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="stateData"></param>
        /// <param name="executeMode"></param>
        public override void Execute(StateData stateData, EExecuteMode executeMode)
        {
            SetOptions();
        }

        /// <summary>
        /// 设置选项列表
        /// </summary>
        protected virtual void SetOptions()
        {
            if (!dropdown) return;

            switch (setOptionsRule)
            {
                case ESetOptionsRule.Add:
                    {
                        var optionDatas = this.optionDatas;
                        if (optionDatas == null) return;

                        dropdown.AddOptions(optionDatas);
                        break;
                    }
                case ESetOptionsRule.ClearAndAdd:
                    {
                        var optionDatas = this.optionDatas;
                        if (optionDatas == null) return;

                        dropdown.ClearOptions();
                        dropdown.AddOptions(optionDatas);
                        break;
                    }
                case ESetOptionsRule.Clear:
                    {
                        dropdown.ClearOptions();
                        break;
                    }
            }
        }

        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity() => base.DataValidity() && dropdown;

        /// <summary>
        /// 输出友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString() => dropdown ? dropdown.name : "";
    }
}
