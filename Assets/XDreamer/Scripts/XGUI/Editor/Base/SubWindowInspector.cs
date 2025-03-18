using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginXGUI;
using XCSJ.PluginXGUI.Base;
using static XCSJ.PluginXGUI.Base.SubWindow;

namespace XCSJ.EditorXGUI.Base
{
    /// <summary>
    /// 子窗口检查器
    /// </summary>
    [Name("子窗口检查器")]
    [CustomEditor(typeof(SubWindow), true)]
    public class SubWindowInspector : DraggableViewInteractorInspector<SubWindow>
    {
        /// <summary>
        /// 当绘制检查器GUI
        /// </summary>
        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            base.OnInspectorGUI();
            if (EditorGUI.EndChangeCheck())
            {
                _lastTitleDirection = targetObject._titleDirection;

                EditorApplicationExtension.DelayCall(0.1f, null, obj => OnWindowLayoutChanged());
            }
        }

        /// <summary>
        /// 当绘制成员
        /// </summary>
        /// <param name="serializedProperty"></param>
        protected override void OnDrawMember(SerializedProperty serializedProperty, PropertyData propertyData)
        {
            switch (serializedProperty.name)
            {
                case nameof(SubWindow._horizontalPosition):
                case nameof(SubWindow._titleWidthOnHorizontal):
                    {
                        if (targetObject._titleDirection != EFourDirection.Top
                             && targetObject._titleDirection != EFourDirection.Bottom
                             || targetObject._widthRule == SubWindow.EWidthRule.WindowWidth)
                        {
                            return;
                        }
                        break;
                    }
                case nameof(SubWindow._verticalPosition):
                case nameof(SubWindow._titleHeightOnVertical):
                    {
                        if (targetObject._titleDirection != EFourDirection.Left
                             && targetObject._titleDirection != EFourDirection.Right
                             || targetObject._heightRule == SubWindow.EHeightRule.WindowHeight)
                        {
                            return;
                        }
                        break;
                    }
            }
            base.OnDrawMember(serializedProperty, propertyData);
        }

        private EFourDirection _lastTitleDirection = EFourDirection.None;

        /// <summary>
        /// 窗口布局发生变化
        /// </summary>
        private void OnWindowLayoutChanged()
        {
            var title = targetObject._title;
            var content = targetObject._content;
            if (!title || !content) return;

            var objList = new List<UnityEngine.Object>() { targetObject.rectTransform, title, content };
            var tb = title.GetComponent<TitleBar>();
            if (tb)
            {
                tb._layoutUnit.ForEach(u => { if (u) objList.Add(u); });
                var text = tb.GetComponent<Text>();
                if (text)
                {
                    objList.Add(text);
                }
            }
            UndoHelper.RegisterCompleteObjectUndo(objList.ToArray());

            var direction = targetObject._titleDirection;
            switch (direction)
            {
                case EFourDirection.None: break;
                case EFourDirection.Top:
                    {
                        targetObject.rectTransform.SetPivot(ENineDirection.MiddleTop);

                        switch (targetObject._widthRule)
                        {
                            case EWidthRule.Fixed:
                            default:
                                {
                                    title.SetMinMaxAnchoredAndFitPosition(direction, new Vector2(targetObject._titleWidthOnHorizontal, targetObject._titleHeightOnHorizontal), targetObject._horizontalPosition);
                                    break;
                                }
                            case EWidthRule.WindowWidth:
                                {
                                    title.SetMinMaxAnchoredAndFitPosition(direction, new Vector2(0, targetObject._titleHeightOnHorizontal));
                                    break;
                                }
                        }

                        targetObject.LayoutTitleBar(true);

                        content.offsetMin = Vector2.zero;
                        content.offsetMax = new Vector2(0, -(targetObject._titleHeightOnHorizontal + targetObject._titleAndContentSpace));

                        break;
                    }
                case EFourDirection.Bottom:
                    {
                        targetObject.rectTransform.SetPivot(ENineDirection.MiddleBottom);

                        switch (targetObject._widthRule)
                        {
                            case EWidthRule.Fixed:
                            default:
                                {
                                    title.SetMinMaxAnchoredAndFitPosition(direction, new Vector2(targetObject._titleWidthOnHorizontal, targetObject._titleHeightOnHorizontal), targetObject._horizontalPosition);
                                    break;
                                }
                            case EWidthRule.WindowWidth:
                                {
                                    title.SetMinMaxAnchoredAndFitPosition(direction, new Vector2(0, targetObject._titleHeightOnHorizontal));
                                    break;
                                }
                        }
                        targetObject.LayoutTitleBar(true);

                        content.offsetMin = new Vector2(0, (targetObject._titleHeightOnHorizontal + targetObject._titleAndContentSpace));
                        content.offsetMax = Vector2.zero;
                        break;
                    }
                case EFourDirection.Left:
                    {
                        targetObject.rectTransform.SetPivot(ENineDirection.LeftMiddle);

                        switch (targetObject._heightRule)
                        {
                            case EHeightRule.Fixed:
                            default:
                                {
                                    title.SetMinMaxAnchoredAndFitPosition(direction, new Vector2(targetObject._titleWidthOnVertical, targetObject._titleHeightOnVertical), targetObject._verticalPosition);
                                    break;
                                }
                            case EHeightRule.WindowHeight:
                                {
                                    title.SetMinMaxAnchoredAndFitPosition(direction, new Vector2(targetObject._titleWidthOnVertical, 0));
                                    break;
                                }
                        }
                        targetObject.LayoutTitleBar(false);

                        content.offsetMin = new Vector2(targetObject._titleWidthOnVertical + targetObject._titleAndContentSpace, 0);
                        content.offsetMax = Vector2.zero;
                        break;
                    }
                case EFourDirection.Right:
                    {
                        targetObject.rectTransform.SetPivot(ENineDirection.RightMiddle);

                        switch (targetObject._heightRule)
                        {
                            case EHeightRule.Fixed:
                            default:
                                {
                                    title.SetMinMaxAnchoredAndFitPosition(direction, new Vector2(targetObject._titleWidthOnVertical, targetObject._titleHeightOnVertical), targetObject._verticalPosition);
                                    break;
                                }
                            case EHeightRule.WindowHeight:
                                {
                                    title.SetMinMaxAnchoredAndFitPosition(direction, new Vector2(targetObject._titleWidthOnVertical, 0));
                                    break;
                                }
                        }
                        targetObject.LayoutTitleBar(false);

                        content.offsetMin = Vector2.zero;
                        content.offsetMax = new Vector2(-(targetObject._titleWidthOnVertical + targetObject._titleAndContentSpace), 0);
                        break;
                    }
            }

            MoveWindow();
        }

        private void MoveWindow()
        {
            var rtf = targetObject.rectTransform;
            var size = rtf.rect.size;
            var newDir = targetObject._titleDirection;
            switch (_lastTitleDirection)
            {
                case EFourDirection.Top:
                    {
                        switch (newDir)
                        {
                            case EFourDirection.Bottom:
                                {
                                    rtf.anchoredPosition += new Vector2(0, -size.y);
                                    break;
                                }
                            case EFourDirection.Left:
                                {
                                    rtf.anchoredPosition += new Vector2(-size.x / 2, -size.y / 2);
                                    break;
                                }
                            case EFourDirection.Right:
                                {
                                    rtf.anchoredPosition += new Vector2(size.x / 2, -size.y / 2);
                                    break;
                                }
                        }
                        break;
                    }
                case EFourDirection.Bottom:
                    {
                        switch (newDir)
                        {
                            case EFourDirection.Top:
                                {
                                    rtf.anchoredPosition += new Vector2(0, size.y);
                                    break;
                                }
                            case EFourDirection.Left:
                                {
                                    rtf.anchoredPosition += new Vector2(-size.x / 2, size.y / 2);
                                    break;
                                }
                            case EFourDirection.Right:
                                {
                                    rtf.anchoredPosition += new Vector2(size.x / 2, size.y / 2);
                                    break;
                                }
                        }
                        break;
                    }
                case EFourDirection.Left:
                    {
                        switch (newDir)
                        {
                            case EFourDirection.Top:
                                {
                                    rtf.anchoredPosition += new Vector2(size.x / 2, size.y / 2);
                                    break;
                                }
                            case EFourDirection.Bottom:
                                {
                                    rtf.anchoredPosition += new Vector2(size.x / 2, -size.y / 2);
                                    break;
                                }
                            case EFourDirection.Right:
                                {
                                    rtf.anchoredPosition += new Vector2(size.x, 0);
                                    break;
                                }
                        }
                        break;
                    }
                case EFourDirection.Right:
                    {
                        switch (newDir)
                        {
                            case EFourDirection.Top:
                                {
                                    rtf.anchoredPosition += new Vector2(-size.x / 2, size.y / 2);
                                    break;
                                }
                            case EFourDirection.Bottom:
                                {
                                    rtf.anchoredPosition += new Vector2(-size.x / 2, -size.y / 2);
                                    break;
                                }
                            case EFourDirection.Left:
                                {
                                    rtf.anchoredPosition += new Vector2(-size.x, 0);
                                    break;
                                }
                        }
                        break;
                    }
            }
        }
    }
}
