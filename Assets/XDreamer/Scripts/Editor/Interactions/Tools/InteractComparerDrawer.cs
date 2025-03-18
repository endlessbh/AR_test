using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension.Base.Attributes;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;

namespace XCSJ.EditorExtension.Base.Interactions.Tools
{
    /// <summary>
    /// 数据项
    /// </summary>
    public class InteractComparerData : ArrayElementData
    {
        public SerializedProperty interactStateSP;
        public SerializedProperty matchRuleSP;
        public SerializedProperty interactorSP;
        public SerializedProperty cmdNameSP;
        public SerializedProperty interactableEntitySP;

        public bool display = true;

        public override void Init(SerializedProperty property)
        {
            base.Init(property);

            interactStateSP = property.FindPropertyRelative(nameof(InteractComparer._interactState));
            matchRuleSP = property.FindPropertyRelative(nameof(InteractComparer._matchRule));
            interactorSP = property.FindPropertyRelative(nameof(InteractComparer._interactor));
            cmdNameSP = property.FindPropertyRelative(nameof(InteractComparer._cmdName));
            interactableEntitySP = property.FindPropertyRelative(nameof(InteractComparer._interactableEntity));
        }

        /// <summary>
        /// 获取绘制的行数
        /// </summary>
        /// <returns></returns>
        public virtual int GetRowCount()
        {
            int rowCount = 1;// 包含标题

            if (display)
            {
                rowCount += 2;// 交互状态和比较规则

                switch ((EInteractMatchRule)matchRuleSP.intValue)
                {
                    case EInteractMatchRule.InteractorEqual:
                    case EInteractMatchRule.CmdEqual:
                    case EInteractMatchRule.InteractableEqual: ++rowCount; break;

                    case EInteractMatchRule.InteractorEqual_And_CmdEqual:
                    case EInteractMatchRule.InteractorEqual_Or_CmdEqual:
                    case EInteractMatchRule.InteractorEqual_And_InteractableEqual:
                    case EInteractMatchRule.InteractorEqual_Or_InteractableEqual:
                    case EInteractMatchRule.CmdEqual_And_InteractableEqual:
                    case EInteractMatchRule.CmdEqual_Or_InteractableEqual:
                    case EInteractMatchRule.Cmd_And_InteractorNotEqual: rowCount += 2; break;

                    case EInteractMatchRule.InteractorEqual_And_CmdEqual_And_InteractableEqual:
                    case EInteractMatchRule.InteractorEqual_Or_CmdEqual_Or_InteractableEqual: rowCount += 3; break;
                }
            }

            return rowCount;
        }

        /// <summary>
        /// 绘制回调
        /// </summary>
        /// <param name="inRect"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        public virtual Rect OnGUI(Rect inRect, GUIContent label)
        {
            label = isArrayElement ? indexContent : label;

            // 标题
            var rect = new Rect(inRect.x, inRect.y, inRect.width, 18);

            GUI.Label(rect, "", XGUIStyleLib.Get(EGUIStyle.Box));
            display = GUI.Toggle(rect, display, label, EditorStyles.foldout);
            if (!display) return rect;

            // 匹配规则
            rect.xMin += 18;

            rect = PropertyDrawerHelper.DrawProperty(rect, interactStateSP);
            rect = PropertyDrawerHelper.DrawProperty(rect, matchRuleSP);

            switch ((EInteractMatchRule)matchRuleSP.intValue)
            {
                case EInteractMatchRule.InteractorEqual:
                    {
                        rect = PropertyDrawerHelper.DrawProperty(rect, interactorSP);
                        break;
                    }
                case EInteractMatchRule.InteractorEqual_And_CmdEqual:
                case EInteractMatchRule.InteractorEqual_Or_CmdEqual:
                    {
                        rect = PropertyDrawerHelper.DrawProperty(rect, interactorSP);
                        rect = DrawInteractorCmd(rect);
                        break;
                    }
                case EInteractMatchRule.InteractorEqual_And_InteractableEqual:
                case EInteractMatchRule.InteractorEqual_Or_InteractableEqual:
                    {
                        rect = PropertyDrawerHelper.DrawProperty(rect, interactorSP);
                        rect = PropertyDrawerHelper.DrawProperty(rect, interactableEntitySP);
                        break;
                    }
                case EInteractMatchRule.InteractorEqual_And_CmdEqual_And_InteractableEqual:
                case EInteractMatchRule.InteractorEqual_Or_CmdEqual_Or_InteractableEqual:
                    {
                        rect = PropertyDrawerHelper.DrawProperty(rect, interactorSP);
                        rect = DrawInteractorCmd(rect);
                        rect = PropertyDrawerHelper.DrawProperty(rect, interactableEntitySP);
                        break;
                    }
                case EInteractMatchRule.CmdEqual:
                    {
                        rect = DrawInteractorCmd(rect);
                        break;
                    }
                case EInteractMatchRule.CmdEqual_And_InteractableEqual:
                case EInteractMatchRule.CmdEqual_Or_InteractableEqual:
                    {
                        rect = DrawInteractorCmd(rect);
                        rect = PropertyDrawerHelper.DrawProperty(rect, interactableEntitySP);
                        break;
                    }
                case EInteractMatchRule.Cmd_And_InteractorNotEqual:
                    {
                        rect = DrawInteractorCmd(rect);
                        rect = PropertyDrawerHelper.DrawProperty(rect, interactorSP);
                        break;
                    }
                case EInteractMatchRule.InteractableEqual:
                    {
                        rect = PropertyDrawerHelper.DrawProperty(rect, interactorSP);
                        break;
                    }
            }
            return rect;
        }

        protected static List<string> emptyCmds = new List<string>();

        protected Rect DrawInteractorCmd(Rect rect)
        {
            rect.y += EditorGUIUtility.singleLineHeight + 2; 
            var interactor = interactorSP.objectReferenceValue as InteractObject;
            DrawInteractorCmd(rect, cmdNameSP, interactor ? interactor.resultCmds : emptyCmds);
            return rect;
        }

        private const int CmdDropdownWith = 80;

        /// <summary>
        /// 使用交互器上有的交互命令来设置命令名称
        /// </summary>
        protected void DrawInteractorCmd(Rect rect, SerializedProperty property, List<string> cmds)
        {
            var strPropertyValue = (EPropertyValueType)property.FindPropertyRelative(nameof(StringPropertyValue._propertyValueType)).intValue;

            // 字符串属性等于值时，才启用下拉列表
            if (strPropertyValue == EPropertyValueType.Value)
            {
                // 绘制原有属性
                rect.width -= CmdDropdownWith;
                EditorGUI.PropertyField(rect, property, PropertyData.GetPropertyData(property).trLabel);

                // 绘制右侧下拉列表
                var valueSP = property.FindPropertyRelative(nameof(StringPropertyValue._value));
                var index = cmds.IndexOf(valueSP.stringValue);

                rect.x += rect.width;
                rect.width = CmdDropdownWith;

                var newIndex = EditorGUI.Popup(rect, index, cmds.ToArray());
                if (newIndex != index)
                {
                    valueSP.stringValue = cmds[newIndex];
                }
            }
            else
            {
                EditorGUI.PropertyField(rect, property, PropertyData.GetPropertyData(property).trLabel);
            }
        }
    }

    /// <summary>
    /// 交互器输入绘制器
    /// </summary>
    [CustomPropertyDrawer(typeof(InteractComparer))]
    public class InteractComparerDrawer : PropertyDrawerAsArrayElement<InteractComparerData>
    {
        /// <summary>
        /// 获取对象绘制高度
        /// </summary>
        /// <param name="property"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var data = cache.GetData(property);

            if (data == null) return 6;

            return (base.GetPropertyHeight(property, label) + 2) * data.GetRowCount();
        }

        /// <summary>
        /// 绘制
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            var data = cache.GetData(property);
            if (data != null) 
            {
                data.OnGUI(rect, label);
            }
        }
    }

    /// <summary>
    /// 交互信息绘制器
    /// </summary>
    [CustomPropertyDrawer(typeof(InteractInfo))]
    public class InteractInfoDrawer : PropertyDrawerAsArrayElement<InteractInfoDrawer.Data>
    {
        /// <summary>
        /// 数据项
        /// </summary>
        public class Data : ArrayElementData
        {
            public SerializedProperty interactorSP;
            public SerializedProperty cmdNameSP;
            public SerializedProperty interactableEntitySP;

            public bool display = true;

            public override void Init(SerializedProperty property)
            {
                base.Init(property);

                interactorSP = property.FindPropertyRelative(nameof(InteractComparer._interactor));
                cmdNameSP = property.FindPropertyRelative(nameof(InteractComparer._cmdName));
                interactableEntitySP = property.FindPropertyRelative(nameof(InteractComparer._interactableEntity));
            }

            /// <summary>
            /// 绘制回调
            /// </summary>
            /// <param name="inRect"></param>
            /// <param name="label"></param>
            /// <returns></returns>
            public virtual Rect OnGUI(Rect inRect, GUIContent label)
            {
                label = isArrayElement ? indexContent : label;

                // 标题
                var rect = new Rect(inRect.x, inRect.y, inRect.width, 18);

                GUI.Label(rect, "", XGUIStyleLib.Get(EGUIStyle.Box));
                display = GUI.Toggle(rect, display, label, EditorStyles.foldout);
                if (!display) return rect;

                // 匹配规则
                rect.xMin += 18;
                rect = PropertyDrawerHelper.DrawProperty(rect, interactorSP);
                rect = DrawInteractorCmd(rect);
                rect = PropertyDrawerHelper.DrawProperty(rect, interactableEntitySP);
                return rect;
            }

            protected static List<string> emptyCmds = new List<string>();

            protected Rect DrawInteractorCmd(Rect rect)
            {
                rect.y += EditorGUIUtility.singleLineHeight + 2;
                var interactor = interactorSP.objectReferenceValue as InteractObject;
                DrawInteractorCmd(rect, cmdNameSP, interactor ? interactor.cmds : emptyCmds);
                return rect;
            }

            private const int CmdDropdownWith = 80;

            /// <summary>
            /// 使用交互器上有的交互命令来设置命令名称
            /// </summary>
            protected void DrawInteractorCmd(Rect rect, SerializedProperty property, List<string> cmds)
            {
                var strPropertyValue = (EPropertyValueType)property.FindPropertyRelative(nameof(StringPropertyValue._propertyValueType)).intValue;

                // 字符串属性等于值时，才启用下拉列表
                if (strPropertyValue == EPropertyValueType.Value)
                {
                    // 绘制原有属性
                    rect.width -= CmdDropdownWith;
                    EditorGUI.PropertyField(rect, property, PropertyData.GetPropertyData(property).trLabel);

                    // 绘制右侧下拉列表
                    var valueSP = property.FindPropertyRelative(nameof(StringPropertyValue._value));
                    var index = cmds.IndexOf(valueSP.stringValue);

                    rect.x += rect.width;
                    rect.width = CmdDropdownWith;

                    var newIndex = EditorGUI.Popup(rect, index, cmds.ToArray());
                    if (newIndex != index)
                    {
                        valueSP.stringValue = cmds[newIndex];
                    }
                }
                else
                {
                    EditorGUI.PropertyField(rect, property, PropertyData.GetPropertyData(property).trLabel);
                }
            }
        }

        /// <summary>
        /// 获取对象绘制高度
        /// </summary>
        /// <param name="property"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            int rowCount = 1;
            if (cache.GetData(property).display)
            {
                rowCount += 3;
            }

            return (base.GetPropertyHeight(property, label) + 2) * rowCount;
        }

        /// <summary>
        /// 绘制
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            var data = cache.GetData(property);
            if (data != null)
            {
                data.OnGUI(rect, label);
            }
        }
    }
}
