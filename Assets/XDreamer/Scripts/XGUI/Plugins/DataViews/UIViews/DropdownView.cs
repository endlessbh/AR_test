using System;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginXGUI.Base;
using XCSJ.PluginXGUI.Views.Dropdowns;

namespace XCSJ.PluginXGUI.DataViews.UIViews
{
    /// <summary>
    /// 下拉列表视图
    /// </summary>
    [Name("下拉列表视图")]
    [XCSJ.Attributes.Icon(EIcon.Dropdown)]
    [Tool(XGUICategory.Data, rootType = typeof(XGUIManager), index = 1)]
    public class DropdownView : BaseUIView<Dropdown>
    {
        /// <summary>
        /// 视图成员
        /// </summary>
        [Name("视图成员")]
        public enum EViewMember
        {
            [Name("值")]
            Value,

            [Name("显示文本")]
            DisplayText,
        }

        /// <summary>
        /// 视图成员
        /// </summary>
        [Name("视图成员")]
        [EnumPopup]
        public EViewMember _viewMember = EViewMember.Value;

        /// <summary>
        /// 视图数据类型
        /// </summary>
        public override Type viewValueType
        {
            get
            {
                switch (_viewMember)
                {
                    case EViewMember.Value: return typeof(int);
                    case EViewMember.DisplayText: return typeof(string);

                    default: return null;
                }
            }
        }

        /// <summary>
        /// 视图数据值
        /// </summary>
        public override object viewValue
        {
            get
            {
                switch (_viewMember)
                {
                    case EViewMember.Value: return view.value;
                    case EViewMember.DisplayText: return view.options[view.value].text;
                    default: return null;
                }
            }
            set
            {
                {
                    switch (_viewMember)
                    {
                        case EViewMember.Value: view.value = (int)value; break;
                        case EViewMember.DisplayText: view.SetTextValue(value as string); break;
                    }
                }
            }
        }

        /// <summary>
        /// 启用：绑定UI事件
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            view.onValueChanged.AddListener(OnValueChanged);
        }

        /// <summary>
        /// 禁用：解除UI事件绑定
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            view.onValueChanged.RemoveListener(OnValueChanged);
        }

        private void OnValueChanged(int value)
        {
            ViewToModelIfCanAndTrigger();
        }
    }
}