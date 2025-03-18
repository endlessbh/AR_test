using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Caches;
using XCSJ.Collections;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorTools;
using XCSJ.EditorTools.Base;
using XCSJ.EditorTools.PropertyDatas;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginStereoView;
using XCSJ.PluginStereoView.Tools;
using XCSJ.PluginXRSpaceSolution;
using XCSJ.PluginXRSpaceSolution.Base;
using XCSJ.PluginXRSpaceSolution.Tools;
using XCSJ.PluginXXR.Interaction.Toolkit;

namespace XCSJ.EditorXRSpaceSolution.Base
{
    /// <summary>
    /// 动作名弹出式特性绘制器
    /// </summary>
    [CustomPropertyDrawer(typeof(ActionNamePopupAttribute))]
    public class ActionNamePopupAttributeDrawer : PropertyDrawer<ActionNamePopupAttribute>
    {
        private const int PopupWidth = 80;

        /// <summary>
        /// 绘制GUI
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            switch(property.propertyType)
            {
                case SerializedPropertyType.String:
                    {
                        EditorGUI.PropertyField(new Rect(position.x, position.y, position.width - PopupWidth, position.height), property, PropertyData.GetPropertyData(property).trLabel);
                        property.stringValue = UICommonFun.Popup(new Rect(position.xMax - PopupWidth, position.y, PopupWidth, position.height), property.stringValue, EnumStringsCache<EActionName>.Get());
                        return;
                    }
                case SerializedPropertyType.Generic:
                    {
                        var vsp = property.FindPropertyRelative(nameof(StringPropertyValue._value));
                        if (vsp == null) break;
                        if (vsp.propertyType == SerializedPropertyType.String)
                        {
                            EditorGUI.PropertyField(new Rect(position.x, position.y, position.width - PopupWidth, position.height), vsp, PropertyData.GetPropertyData(property).trLabel);
                            vsp.stringValue = UICommonFun.Popup(new Rect(position.xMax - PopupWidth, position.y, PopupWidth, position.height), vsp.stringValue, EnumStringsCache<EActionName>.Get());
                            return;
                        }
                        break;
                    }
            }

            EditorGUI.PropertyField(position, property);
        }
    }
}
