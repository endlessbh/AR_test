using System;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXGUI.Styles.Base;
using XCSJ.PluginXGUI.Styles.Tools;

namespace XCSJ.EditorXGUI.Styles.Tools
{
    /// <summary>
    /// 样式配置器检查器
    /// </summary>
    [CustomEditor(typeof(StyleConfiger))]
    [CanEditMultipleObjects]
    [Name("样式配置器检查器")]
    public class StyleConfigerInspector : MBInspector<StyleConfiger>
    {
        /// <summary>
        /// 当绘制成员
        /// </summary>
        /// <param name="serializedProperty"></param>
        protected override void OnDrawMember(SerializedProperty serializedProperty, PropertyData propertyData)
        {
            switch (serializedProperty.name)
            {
                case nameof(StyleConfiger._styleName):
                    {
                        var name = UICommonFun.Popup(CommonFun.NameTip(targetObject.GetType(), nameof(StyleConfiger._styleName)), targetObject._styleName, XStyleCache.GetNames(), false, GUILayout.ExpandWidth(true));
                        if (!string.IsNullOrEmpty(name) && name != targetObject._styleName)
                        {
                            XStyleCache.SetDefaultStyle(name);
                        }
                        return ;
                    }
            }
            base.OnDrawMember(serializedProperty, propertyData);
        }
    }
}
