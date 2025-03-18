using UnityEditor;
using UnityEngine;
using XCSJ.EditorCommonUtils;
using XCSJ.Extension.Base.Dataflows.Binders;
using XCSJ.Scripts;

namespace XCSJ.EditorExtension.Base.Dataflows.Binders
{
    /// <summary>
    /// 别名特性绘制器
    /// </summary>
    [CustomPropertyDrawer(typeof(AliasAttribute))]
    public class AliasAttributeDrawer : PropertyDrawer<AliasAttribute>
    {
        private const int PopupWidth = 80;

        /// <summary>
        /// 绘制GUI
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            label = EditorGUI.BeginProperty(rect, label, property);
            {
                switch (propertyAttribute.aliasDataType)
                {
                    case EAliasDataType.Get:
                        {
                            var textRect = rect;
                            textRect.width -= PopupWidth;
                            property.stringValue = EditorGUI.TextField(textRect, label, property.stringValue);

                            var popupRect = rect;
                            popupRect.x += rect.width - PopupWidth;
                            popupRect.width = PopupWidth;

                            // 从缓存中获取别名列表
                            var aliasName = property.stringValue;
                            EditorGUI.BeginChangeCheck();
                            {
                                aliasName = UICommonFun.Popup(popupRect, aliasName, AliasCache.instance.GetAliases());
                            }
                            if (EditorGUI.EndChangeCheck())
                            {
                                property.stringValue = aliasName;
                            }
                            break;
                        }
                    case EAliasDataType.Set:
                        {
                            var textRect = rect;
                            textRect.width -= PopupWidth;

                            var obj = property.serializedObject.targetObject;
                            var aliasName = property.stringValue;
                            EditorGUI.BeginChangeCheck();
                            {
                                var ogrColor = GUI.color;
                                {
                                    // 别名重复
                                    if (!string.IsNullOrEmpty(aliasName) && AliasCache.instance.TryGetOwner(aliasName, out var owner) && owner != obj)
                                    {
                                        //string info = string.Format("[{0}]不允许重复，场景中对象[{1}]已包含别名!", aliasName, owner.ToScriptParamString());
                                        GUI.color = Color.red;
                                    }
                                    aliasName = EditorGUI.DelayedTextField(textRect, label, property.stringValue);
                                }
                                GUI.color = ogrColor;

                                var buttonRect = rect;
                                buttonRect.x += rect.width - PopupWidth;
                                buttonRect.width = PopupWidth;

                                if (GUI.Button(buttonRect, "创建别名") && AliasCache.instance.TryCreateUniqueAlias(aliasName, obj, out var uniqueAlias))
                                {
                                    aliasName = uniqueAlias;
                                }
                            }
                            if (EditorGUI.EndChangeCheck())
                            {
                                if (!AliasCache.instance.Contains(aliasName))
                                {
                                    property.stringValue = aliasName;

                                    UICommonFun.DelayCall(() => AliasCache.instance.UpdateWithOwner(obj));
                                }
                            }
                            break;
                        }
                }
            }
            EditorGUI.EndProperty();
        }

    }
}
