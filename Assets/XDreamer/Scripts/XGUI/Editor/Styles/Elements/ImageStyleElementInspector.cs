using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorXGUI.Styles.Base;
using XCSJ.Helper;
using XCSJ.PluginXGUI.Styles.Elements;

namespace XCSJ.EditorXGUI.Styles.Elements
{
    /// <summary>
    /// 图像样式元素检查器
    /// </summary>
    [Name("图像样式元素检查器")]
    [CustomEditor(typeof(ImageStyleElement))]
    public class ImageStyleElementInspector : BaseStyleElementInspector
    {
        /// <summary>
        /// 当绘制成员
        /// </summary>
        /// <param name="serializedProperty"></param>
        protected override void OnDrawMember(SerializedProperty serializedProperty, PropertyData propertyData)
        {
            switch (serializedProperty.name)
            {
                case nameof(ImageStyleElement._fillCenter):
                    {
                        var imageStyle = target as ImageStyleElement;
                        switch (imageStyle._imageFillType)
                        {
                            case Image.Type.Simple:
                            case Image.Type.Filled: return;
                            case Image.Type.Sliced:
                            case Image.Type.Tiled: break;
                        }
                        break;
                    }
                case nameof(ImageStyleElement._fillOrigin):
                    {
                        var imageStyle = target as ImageStyleElement;
                        var fillOrigin = imageStyle._fillOrigin;
                        EditorGUI.BeginChangeCheck();

                        switch (imageStyle._fillMethod)
                        {
                            case Image.FillMethod.Horizontal:
                                {
                                    fillOrigin = UICommonFun.EnumPopup(new GUIContent("水平"), (Image.OriginHorizontal)fillOrigin).Int();
                                    break;
                                }
                            case Image.FillMethod.Vertical:
                                {
                                    fillOrigin = UICommonFun.EnumPopup(new GUIContent("垂直"), (Image.OriginVertical)fillOrigin).Int();
                                    break;
                                }
                            case Image.FillMethod.Radial90:
                                {
                                    fillOrigin = UICommonFun.EnumPopup(new GUIContent("径向90"), (Image.Origin90)fillOrigin).Int();
                                    break;
                                }
                            case Image.FillMethod.Radial180:
                                {
                                    fillOrigin = UICommonFun.EnumPopup(new GUIContent("径向180"), (Image.Origin180)fillOrigin).Int();
                                    break;
                                }
                            case Image.FillMethod.Radial360:
                                {
                                    fillOrigin = UICommonFun.EnumPopup(new GUIContent("径向360"), (Image.Origin360)fillOrigin).Int();
                                    break;
                                }
                        }
                        if (EditorGUI.EndChangeCheck())
                        {
                            imageStyle.fillOrigin = fillOrigin;
                        }
                        break;
                    }
                default:
                    break;
            }
            base.OnDrawMember(serializedProperty, propertyData);
        }
    }
}
