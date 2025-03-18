using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginXGUI.Base;
using XCSJ.PluginXGUI.DataViews.Base;

namespace XCSJ.PluginXGUI.DataViews.GroupViews
{
    /// <summary>
    /// 组件数据视图
    /// </summary>
    [Name("组件数据视图")]
    [DataViewAttribute(typeof(Component))]
#if UNITY_EDITOR && XDREAMER_EDITION_DEVELOPER
    //[Tool(XGUICategory.DataGroupView, rootType = typeof(XGUIManager))]
#endif
    public class ComponentView : GroupView
    {
        public enum EViewLabelNameRule
        {
            [Name("组件名称")]
            ComponentName,

            [Name("组件所在游戏对象名称")]
            GameObjectName,
        }
        [Name("组件名称规则")]
        [EnumPopup]
        public EViewLabelNameRule _viewLabelNameRule = EViewLabelNameRule.GameObjectName;

        /// <summary>
        /// 关联组件
        /// </summary>
        [Name("关联组件")]
        [ComponentPopup]
        public Component _component;

        /// <summary>
        /// 开始
        /// </summary>
        protected override void Start()
        {
            if (modelTarget)
            {
                BindModel(modelTarget);
            }
        }

        public override UnityEngine.Object modelTarget => base.modelTarget ? base.modelTarget : _component;

        /// <summary>
        /// 标签文本
        /// </summary>
        public override string modelDataLabelText 
        {
            get
            {
                if (_component)
                {
                    switch (_viewLabelNameRule)
                    {
                        case EViewLabelNameRule.ComponentName:
                            {
                                return _component.GetType().Name;
                            }
                        case EViewLabelNameRule.GameObjectName:
                            {
                                return _component.name;
                            }
                    }
                }
                return default;
            }
        }

        public override Type viewValueType => _component ? _component.GetType() : null;

        public override object viewValue { get => _component; set => _component = (Component)value; }
    }

}
