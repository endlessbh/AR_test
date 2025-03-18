using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Base.Kernel;
using XCSJ.EditorXGUI;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginTools.LineNotes;
using XCSJ.PluginTools.SelectionUtils;

namespace XCSJ.EditorTools.Inspectors
{
    /// <summary>
    /// 批注-3D检查器
    /// </summary>
    [Name("批注-3D检查器")]
    [CustomEditor(typeof(UGUILineNote3D), true)]
    [CanEditMultipleObjects]
    public class UGUILineNote3DInspector : LineNote3DInspector
    {
        private ENote3DError note3DError = ENote3DError.None;

        private UGUILineNote3D line => target as UGUILineNote3D;

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            CheckUIValid();
        }

        /// <summary>
        /// 当绘制成员
        /// </summary>
        /// <param name="serializedProperty"></param>
        protected override void OnDrawMember(SerializedProperty serializedProperty, PropertyData propertyData)
        {
            switch (serializedProperty.name)
            {
                case nameof(UGUILineNote3D.rectTransform):
                    {
                        EditorGUILayout.BeginHorizontal();
                        base.OnDrawMember(serializedProperty, propertyData);
                        if (GUILayout.Button(UICommonOption.Insert, UICommonOption.WH24x16) && target is UGUILineNote3D line)
                        {
                            line.rectTransform = UGUILineHelper.CreateButtonNote();
                        }
                        EditorGUILayout.EndHorizontal();
                        return;
                    }
            }
            base.OnDrawMember(serializedProperty, propertyData);
            switch (serializedProperty.name)
            {
                case nameof(UGUILineNote3D.rectTransform):
                    {
                        switch (note3DError)
                        {
                            case ENote3DError.None:
                                break;
                            case ENote3DError.ParentIsCanva:
                                {
                                    if (line.rectTransform)
                                    {
                                        UICommonFun.RichHelpBox("UI对象[" + line.rectTransform.name + "]的父对象不能是" + nameof(Canvas), MessageType.Error);
                                    }
                                    break;
                                }
                            case ENote3DError.ParentIsNotStretchHV:
                                {
                                    UICommonFun.RichHelpBox("父对象的数据设置错误", MessageType.Error);
                                    if (GUILayout.Button("纠正", GUILayout.Width(60)))
                                    {
                                        if (line.rectTransform)
                                        {
                                            StretchHVParent(line.rectTransform.parent as RectTransform);
                                        }
                                        CheckUIValid();
                                    }
                                    break;
                                }
                        }
                        break;
                    }
            }
        }

        /// <summary>
        /// 标注3D
        /// </summary>
        protected enum ENote3DError
        {
            /// <summary>
            /// 无
            /// </summary>
            None,

            /// <summary>
            /// 父节点为画布
            /// </summary>
            ParentIsCanva,

            /// <summary>
            /// 父节点非缩放
            /// </summary>
            ParentIsNotStretchHV,
        }

        private void CheckUIValid()
        {
            note3DError = ENote3DError.None;
            if (line && line.rectTransform)
            {
                if (line.rectTransform.parent is RectTransform parentRectTransform)
                {
                    if (parentRectTransform.GetComponent<Canvas>())
                    {
                        note3DError = ENote3DError.ParentIsCanva;
                    }

                    if (note3DError == ENote3DError.None)
                    {
                        CheckUIAndParentNotStretchHV(parentRectTransform, out bool isNotStretchHV);
                        if (isNotStretchHV)
                        {
                            note3DError = ENote3DError.ParentIsNotStretchHV;
                        }
                    }
                }
            }
        }

        private void CheckUIAndParentNotStretchHV(RectTransform rectTransform, out bool isNotStretchHV)
        {
            if (!rectTransform || rectTransform.GetComponent<Canvas>())
            {
                isNotStretchHV = false;
                return;
            }
            if (rectTransform.anchorMin != Vector2.zero || rectTransform.anchorMax != Vector2.one
                || rectTransform.offsetMin != Vector2.zero || rectTransform.offsetMax != Vector2.zero)
            {
                isNotStretchHV = true;
            }
            else
            {
                CheckUIAndParentNotStretchHV(rectTransform.parent as RectTransform, out isNotStretchHV);
            }
        }

        private void StretchHVParent(RectTransform rectTransform)
        {
            if (!rectTransform || rectTransform.GetComponent<Canvas>())
            {
                return;
            }
            else
            {
                rectTransform.XStretchHV();
                StretchHVParent(rectTransform.parent as RectTransform);
            }
        }
    }

}
