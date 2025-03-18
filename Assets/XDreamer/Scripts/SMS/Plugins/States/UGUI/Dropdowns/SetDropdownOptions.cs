using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;

namespace XCSJ.PluginSMS.States.UGUI.Dropdowns
{
    /// <summary>
    /// 设置下拉框选项列表：设置下拉框选项列表可用于设置下拉框的下拉选项列表内容
    /// </summary>
    [ComponentMenu("UGUI/" + Title, typeof(SMSManager))]
    [Name(Title, nameof(SetDropdownOptions))]
    [Tip("设置下拉框选项列表可用于设置下拉框的下拉选项列表内容", "Set drop-down box option list can be used to set the contents of the drop-down option list of the drop-down box")]
    [XCSJ.Attributes.Icon(EIcon.Dropdown)]
    public class SetDropdownOptions : BaseSetDropdownOptions<SetDropdownOptions>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "设置下拉框选项列表";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib("UGUI", typeof(SMSManager))]
        [StateComponentMenu("UGUI/" + Title, typeof(SMSManager))]
        [Name(Title, nameof(SetDropdownOptions))]
        [Tip("设置下拉框选项列表可用于设置下拉框的下拉选项列表内容", "Set drop-down box option list can be used to set the contents of the drop-down option list of the drop-down box")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 选项数据列表
        /// </summary>
        [Name("选项数据列表")]
        public List<CustomOptionData> customOptionDatas = new List<CustomOptionData>();

        /// <summary>
        /// 选项数据列表：用于设置下拉框选项的选项数据列表
        /// </summary>
        public override List<Dropdown.OptionData> optionDatas => customOptionDatas.ToList(data => new Dropdown.OptionData(data.text, data.image));

        /// <summary>
        /// 自定义选项数据
        /// </summary>
        [Serializable]
        [Name("自定义选项数据")]
        public class CustomOptionData
        {
            /// <summary>
            /// 文本
            /// </summary>
            [Name("文本")]
            public string _text = "";

            /// <summary>
            /// 文本
            /// </summary>
            public string text { get => _text; set => _text = value; }

            /// <summary>
            /// 图片
            /// </summary>
            [Name("图片")]
            public Sprite _image = null;

            /// <summary>
            /// 图片
            /// </summary>
            public Sprite image { get => _image; set => _image = value; }
        }
    }
}
