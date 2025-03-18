using System.Collections.Generic;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using static UnityEngine.UI.Dropdown;

namespace XCSJ.PluginXGUI.Views.Dropdowns
{
    /// <summary>
    /// 基础设置下拉框选项列表
    /// </summary>
    public abstract class BaseSetDropdownOptions : BaseSetDropdownProperty, ISetDropdownOptions
    {
        /// <summary>
        /// 设置选项列表规则
        /// </summary>
        [Name("设置选项列表规则")]
        [Tip("设置选项列表时需遵守的规则", "Rules to follow when setting the option list")]
        [EnumPopup]
        public ESetOptionsRule _setOptionRule = ESetOptionsRule.ClearAndAdd;

        /// <summary>
        /// 设置选项列表规则
        /// </summary>
        public ESetOptionsRule setOptionsRule { get => _setOptionRule; set => _setOptionRule = value; }

        /// <summary>
        /// 选项数据列表：用于设置下拉框选项的选项数据列表
        /// </summary>
        public abstract List<OptionData> optionDatas { get; }

        /// <summary>
        /// 启用时
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
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
    }

    /// <summary>
    /// 设置下拉框选项列表接口
    /// </summary>
    public interface ISetDropdownOptions : ISetDropdownProperty
    {
        /// <summary>
        /// 设置选项列表规则
        /// </summary>
        ESetOptionsRule setOptionsRule { get; set; }

        /// <summary>
        /// 选项数据列表：用于设置下拉框选项的选项数据列表
        /// </summary>
        List<OptionData> optionDatas { get; }
    }

    /// <summary>
    /// 设置选项列表规则
    /// </summary>
    [Name("设置选项列表规则")]
    public enum ESetOptionsRule
    {
        /// <summary>
        /// 无：不做处理
        /// </summary>
        [Name("无")]
        [Tip("不做处理", "Do not handle")]
        None,

        /// <summary>
        /// 添加：将选项数据列表直接添加到下拉框选项中
        /// </summary>
        [Name("添加")]
        [Tip("将选项数据列表直接添加到下拉框选项中", "Add the option data list directly to the drop-down box options")]
        Add,

        /// <summary>
        /// 清理并添加：将下拉框中原选项全部清理，然后将选项数据列表直接添加到下拉框选项中
        /// </summary>
        [Name("清理并添加")]
        [Tip("将下拉框中原选项全部清理，然后将选项数据列表直接添加到下拉框选项中", "Clear all the options in the drop-down box, and then add the option data list directly to the drop-down box options")]
        ClearAndAdd,

        /// <summary>
        /// 清理：将下拉框中原选项全部清理
        /// </summary>
        [Name("清理")]
        [Tip("将下拉框中原选项全部清理", "Clear all the options in the drop-down box")]
        Clear,
    }
}
