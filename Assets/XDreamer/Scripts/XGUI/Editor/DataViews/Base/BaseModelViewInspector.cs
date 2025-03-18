using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Base.CategoryViews;
using XCSJ.EditorTools;
using XCSJ.EditorXGUI.Base;
using XCSJ.PluginXGUI.DataViews.Base;
using XCSJ.PluginXGUI.DataViews.GroupViews;
using static XCSJ.PluginXGUI.DataViews.DataViewHelper;

namespace XCSJ.EditorXGUI.DataViews.Base
{
    /// <summary>
    /// 基础数据视图检查器
    /// </summary>
    [Name("基础数据视图检查器")]
    [CustomEditor(typeof(BaseModelView), true)]
    [CanEditMultipleObjects]
    public class BaseModelViewInspector : BaseViewControllerInspector<BaseModelView>
    {
        private CategoryList mvCategoryList = null;

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            FindCategoryList();
        }

        /// <summary>
        /// 成员绘制
        /// </summary>
        /// <param name="serializedProperty"></param>
        /// <param name="propertyData"></param>
        protected override void OnDrawMember(SerializedProperty serializedProperty, PropertyData propertyData)
        {
            base.OnDrawMember(serializedProperty, propertyData);
            switch (serializedProperty.name)
            {
                case nameof(BaseModelView._modelToViewConverter):
                    {
                        var view = targetObject as BaseModelView;
                        if (view._modelToViewConverter && !view.TryGetValidModelToViewConverter(out _))
                        {
                            EditorGUILayout.HelpBox("无效的模型到视图转换器组件对象", MessageType.Error);
                        }
                        break;
                    }
                case nameof(BaseModelView._viewToModelConverter):
                    {
                        var view = targetObject as BaseModelView;
                        if (view._viewToModelConverter && !view.TryGetValidViewToModelConverter(out _))
                        {
                            EditorGUILayout.HelpBox("无效的视图到模型转换器组件对象", MessageType.Error);
                        }
                        break;
                    }
            }
        }

        private void FindCategoryList()
        {
            mvCategoryList = null;
            var view = targetObject as BaseModelView;

            mvCategoryList = EditorToolsHelper.GetWithPurposes((c, b) =>
            {
                if (b is ComponentToolItem componentToolItem)
                {
                    return ConverterCache.Get(view.modelValueType, view.viewValueType, componentToolItem.type).canInputToOutputOrOutputToInput;
                }
                return false;
            }, nameof(BaseDataConverter));
            Repaint();
        }


        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            base.OnInspectorGUI();
            if (EditorGUI.EndChangeCheck())// View的参数修改，也会导致匹配类型发生变化
            {
                UICommonFun.DelayCall(FindCategoryList);
            }
            mvCategoryList?.DrawVertical();
        }

        protected override bool displayHelpInfo => true;//base.displayHelpInfo;

        public override StringBuilder GetHelpInfo()
        {
            var stringBuilder = base.GetHelpInfo();
            var view = targetObject as BaseModelView;
            var modelDataType = view.modelValueType;
            var viewDataType = view.viewValueType;
            stringBuilder.AppendFormat("模型数据类型:\t{0}\n", modelDataType?.FullName);
            stringBuilder.AppendFormat("视图数据类型:\t{0}", viewDataType?.FullName);
            return stringBuilder;
        }
    }
}
