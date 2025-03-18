using System;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXGUI.Styles.Base;
using XCSJ.PluginXGUI.Styles.Tools;

namespace XCSJ.EditorXGUI.Styles.Tools
{
    /// <summary>
    /// 样式链接器检查器
    /// </summary>
    [Name("样式链接器检查器")]
    [CustomEditor(typeof(StyleLinker))]
    [CanEditMultipleObjects]
    public class StyleLinkerInspector : MBInspector<StyleLinker>
    {
        private string[] styleNameArray => XStyleCache.GetNames();

        /// <summary>
        /// 当绘制成员
        /// </summary>
        /// <param name="serializedProperty"></param>
        protected override void OnDrawMember(SerializedProperty serializedProperty, PropertyData propertyData)
        {
            switch (serializedProperty.name)
            {
                case nameof(StyleLinker._customStyleName):
                    {
                        if (string.IsNullOrEmpty(targetObject._customStyleName) && styleNameArray.Length > 0)
                        {
                            targetObject._customStyleName = styleNameArray[0];
                        }
                        EditorGUI.BeginChangeCheck();
                        serializedProperty.stringValue = UICommonFun.Popup(CommonFun.NameTip(targetObject.GetType(), nameof(StyleLinker._customStyleName)), serializedProperty.stringValue, styleNameArray, false);
                        if (EditorGUI.EndChangeCheck())
                        {
                            targetObject.TryStyleToObject();
                        }
                        return;
                    }
                case nameof(StyleLinker._styleElementCollectionName):
                    {
                        DrawStyleElementCollectionName();
                        return;
                    }
                case nameof(StyleLinker._componentStyles):
                    {
                        DrawComponentElementList();
                        return;
                    }
                case nameof(StyleLinker._styleRule):
                    {
                        EditorGUILayout.BeginHorizontal();
                        base.OnDrawMember(serializedProperty, propertyData);
                        GUILayout.Label(targetObject.style?.name);
                        EditorGUILayout.EndHorizontal();
                        return;
                    }
            }
            base.OnDrawMember(serializedProperty, propertyData);
        }

        private void DrawStyleElementCollectionName()
        {
            if (!targetObject.style) return;

            var names = targetObject.style.GetElementNames(typeof(StyleElementCollection));
            if (names.Length > 0)
            {
                EditorGUI.BeginChangeCheck();
                var cn = UICommonFun.Popup(new GUIContent(CommonFun.Name(targetObject, nameof(targetObject._styleElementCollectionName))), targetObject._styleElementCollectionName, names, false);
                if (EditorGUI.EndChangeCheck())
                {
                    targetObject.XModifyProperty(() =>
                    {
                        targetObject._styleElementCollectionName = cn;
                        targetObject.UpdateComponentStyleName(targetObject.style.GetElement<StyleElementCollection>(cn));
                    });
                }
            }
        }

        /// <summary>
        /// 组件样式列表
        /// </summary>
        private bool _display = true;

        /// <summary>
        /// 绘制组件样式关联样式元素列表
        /// </summary>
        private void DrawComponentElementList()
        {
            _display = UICommonFun.Foldout(_display, CommonFun.NameTip(typeof(StyleLinker), nameof(StyleLinker._componentStyles)), () =>
            {
                if (GUILayout.Button("对象=>样式", EditorStyles.miniButtonLeft, UICommonOption.Width80))
                {
                    targetObject.TryObjectToStyle();
                }
                if (GUILayout.Button(new GUIContent("自动关联样式", "将所有子游戏对象的样式链接器的【样式规则】设置为父级；将【样式元素链接规则】设置为组合；将【组合元素名称】设置为父级相同名称；"), EditorStyles.miniButtonRight, UICommonOption.Width80))
                {
                    AutoLinkStyle();
                }
            });
            if (!_display) return;

            CommonFun.BeginLayout();
            {
                // 标题
                EditorGUILayout.BeginHorizontal(GUI.skin.box);
                {
                    // 编号
                    GUILayout.Label("NO.", UICommonOption.Width24);

                    // 组件对象
                    GUILayout.Label(CommonFun.NameTip(typeof(ComponentStyle), nameof(ComponentStyle._component)), UICommonOption.Width100);

                    // 样式元素名称
                    GUILayout.Label(CommonFun.NameTip(typeof(ComponentStyle), nameof(ComponentStyle._elementName)));

                    // 样式参数设定
                    GUILayout.Label(CommonFun.NameTip(typeof(ComponentStyle), nameof(ComponentStyle._paramsMask)), UICommonOption.Width100);
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Separator();

                // 样式元素项
                EditorGUI.BeginChangeCheck();
                {
                    var styleElementCollection = targetObject.styleElementCollection; // 缓存
                    for (int i = 0; i < targetObject._componentStyles.Count; i++)
                    {
                        EditorGUILayout.BeginHorizontal();
                        {
                            // 编号
                            GUILayout.Label((i + 1).ToString(), UICommonOption.Width24);

                            // 组件对象
                            var item = targetObject._componentStyles[i];
                            EditorGUILayout.ObjectField(item._component, item._component.GetType(), true, UICommonOption.Width100);

                            var elementType = StyleLinkAttribute.GetStyleElementType(item._component.GetType());
                            var enumType = StyleHelper.GetParamsSettingEnumType(elementType);

                            if (styleElementCollection && item._component)
                            {
                                // 样式元素名称：根据元素类型生成下拉元素列表
                                EditorGUI.BeginChangeCheck();
                                var elementName = UICommonFun.Popup(item._elementName, styleElementCollection.GetElementNames(elementType), false);
                                if (EditorGUI.EndChangeCheck())
                                {
                                    targetObject.XModifyProperty(() =>
                                    {
                                        item._elementName = elementName;
                                    });
                                }

                                // 样式参数设定：根据元素类型生成下拉参数设置列表
                                if (enumType!=null)
                                {
                                    EditorGUI.BeginChangeCheck();
                                    var ne = UICommonFun.EnumPopup(EnumHelper.ToObject(enumType, item._paramsMask) as Enum, UICommonOption.Width100);
                                    if (EditorGUI.EndChangeCheck())
                                    {
                                        targetObject.XModifyProperty(() =>
                                        {
                                            item._paramsMask = ne.Int();
                                        });
                                    }
                                }
                            }
                            else
                            {     
                                var org = GUI.color;
                                GUI.color = Color.red;
                                GUILayout.Label(item._elementName);
                                UICommonFun.EnumPopup(EnumHelper.ToObject(enumType, item._paramsMask) as Enum, UICommonOption.Width100);
                                GUI.color = org;
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }
                if (EditorGUI.EndChangeCheck())
                {
                    targetObject.TryStyleToObject();
                }
            }

            CommonFun.EndLayout();
        }

        private void AutoLinkStyle()
        {
            // 遍历所有的子对象，修改所有样式规则、组合名称和样式名称
            var linkers = targetObject.GetComponentsInChildren<StyleLinker>(true);
            foreach (var l in linkers)
            {
                if (l!=targetObject)
                {
                    l.XModifyProperty(() => 
                    {
                        l._styleRule = StyleLinker.EStyleRule.Parent;
                        l._elementLinkRule = StyleLinker.EStyleElementLinkRule.Collection;
                        l._styleElementCollectionName = targetObject._styleElementCollectionName;
                    });
                }

                SetElementName(GetNamePath(l), l);
            }
            foreach (var l in linkers)
            {
                l.TryStyleToObject();
            }
        }

        private string GetNamePath(StyleLinker linker)
        {
            string path = "";
            var current = linker.transform;
            while (current!=targetObject.transform)
            {
                path = "_" + current.name + path;
                current = current.parent;
            }
            return path;
        }

        private void SetElementName(string parentName, StyleLinker linker)
        {
            if (!string.IsNullOrEmpty(parentName))
            {
                parentName = parentName + ".";
            }
            var styleElementCollection = linker.styleElementCollection;
            foreach (var cs in linker._componentStyles)
            {
                if (cs._component)
                {
                    var namePath = parentName + cs._component.GetType().Name;
                    var e = styleElementCollection.GetElement(namePath, cs._component);
                    if (e)
                    {
                        cs._elementName = namePath;
                    }
                }
            }
        }
    }
}
