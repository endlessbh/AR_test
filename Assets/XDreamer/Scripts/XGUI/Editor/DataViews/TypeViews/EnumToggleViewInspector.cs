using System.Text;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Base.CategoryViews;
using XCSJ.EditorTools;
using XCSJ.EditorXGUI.Base;
using XCSJ.EditorXGUI.DataViews.Base;
using XCSJ.PluginCommonUtils.Base;
using XCSJ.PluginXGUI.DataViews.Base;
using XCSJ.PluginXGUI.DataViews.GroupViews;
using XCSJ.PluginXGUI.DataViews.TypeViews;

namespace XCSJ.EditorXGUI.DataViews.TypeViews
{
    /// <summary>
    /// 基础数据视图检查器
    /// </summary>
    [Name("基础数据视图检查器")]
    [CustomEditor(typeof(EnumToggleView), true)]
    [CanEditMultipleObjects]
    public class EnumToggleViewInspector : BaseModelViewInspector
    {
        [Languages.LanguageTuple("The number of toggle list must be equal to {0}", "切换列表数量需等于{0}")]
        protected override void OnDrawMember(SerializedProperty serializedProperty, PropertyData propertyData)
        {
            base.OnDrawMember(serializedProperty, propertyData);

            switch (serializedProperty.name)
            {
                case nameof(EnumToggleView._toggleList):
                    {
                        var enumToggleView = (targetObject as EnumToggleView);
                        var enumType = enumToggleView.modelValueType;
                        if (enumType!=null)
                        {
                            var enumTypeData = EnumTypeData.GetEnumTypeData(enumType);
                            if (enumTypeData != null)
                            {
                                var enumDataLength = enumTypeData.displayNames.Length;
                                if (enumToggleView._toggleList.Count != enumDataLength)
                                {
                                    EditorGUILayout.HelpBox(string.Format(Tr("The number of toggle list must be equal to {0}"), enumDataLength), MessageType.Error);
                                }
                            }
                        }
                        break;
                    }
            }
        }
    }
}
