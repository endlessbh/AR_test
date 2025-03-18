using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginXGUI.Views.Dropdowns
{
    /// <summary>
    /// 自定义下拉框事件
    /// </summary>
    [Name("自定义下拉框事件")]
    [Serializable]
    public class CustomDropdownEvent : BaseCustomDropdownEvent
    {
        /// <summary>
        /// 值类型
        /// </summary>
        [Name("值类型")]
        [EnumPopup]
        public EDropdownValueType _valueType = EDropdownValueType.Value;

        /// <summary>
        /// 值
        /// </summary>
        [Name("值")]
        public int _value = 0;

        /// <summary>
        /// 文本
        /// </summary>
        [Name("文本")]
        public string _text = "";

        /// <summary>
        /// 显示文本
        /// </summary>
        public override string displayText
        {
            get
            {
                switch (_valueType)
                {
                    case EDropdownValueType.Any:
                    case EDropdownValueType.Value: return _value.ToString();
                    case EDropdownValueType.Text: return _text;
                    default: return "";
                }
            }
        }

        /// <summary>
        /// 判断能否执行：根据值类型检测下拉框当前选择项是否符合条件；
        /// </summary>
        /// <param name="dropdown"></param>
        /// <returns></returns>
        public override bool CanInvoke(Dropdown dropdown)
        {
            if (!dropdown) return false;
            switch (_valueType)
            {
                case EDropdownValueType.Any: return true;
                case EDropdownValueType.Value: return dropdown.value == _value;
                case EDropdownValueType.Text: return dropdown.GetTextValue() == _text;
            }
            return false;
        }
    }

    /// <summary>
    /// 下拉框值类型
    /// </summary>
    [Name("下拉框值类型")]
    public enum EDropdownValueType
    {
        /// <summary>
        /// 任意
        /// </summary>
        [Name("任意")]
        Any = -1,

        /// <summary>
        /// 值
        /// </summary>
        [Name("值")]
        Value = 0,

        /// <summary>
        /// 文本
        /// </summary>
        [Name("文本")]
        Text,
    }
}
