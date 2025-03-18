using System;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginXGUI.Base;

namespace XCSJ.PluginXGUI.DataViews.UIViews
{
    /// <summary>
    /// 切换视图
    /// </summary>
    [Name("切换视图")]
    [XCSJ.Attributes.Icon(EIcon.Toggle)]
    [Tool(XGUICategory.Data, rootType = typeof(XGUIManager), index = 1)]
    public class ToggleView : BaseUIView<Toggle>
    {
        /// <summary>
        /// 视图成员
        /// </summary>
        [Name("视图成员")]
        public enum EViewMember
        {
            [Name("开关")]
            IsOn,
        }

        /// <summary>
        /// 视图成员
        /// </summary>
        [Name("视图成员")]
        [EnumPopup]
        public EViewMember _viewMember = EViewMember.IsOn;

        /// <summary>
        /// 视图数据类型
        /// </summary>
        public override Type viewValueType
        {
            get
            {
                switch (_viewMember)
                {
                    case EViewMember.IsOn: return typeof(bool);
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
                    case EViewMember.IsOn: return view.isOn;
                    default: return null;
                }
            }
            set
            {
                switch (_viewMember)
                {
                    case EViewMember.IsOn: view.isOn = (bool)value; break;
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

        private void OnValueChanged(bool value)
        {
            if (_viewMember == EViewMember.IsOn)
            {
                ViewToModelIfCanAndTrigger();
            }
        }
    }
}
