using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorSMS.Transitions.Base;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.Transitions.Dataflows.Events;
using static XCSJ.Extension.Base.Dataflows.Binders.TypeBinder;

namespace XCSJ.EditorSMS.Transitions.Dataflows.Events
{
    /// <summary>
    /// Action事件触发器检查器
    /// </summary>
    [Name("Action事件触发器检查器")]
    [CustomEditor(typeof(ActionEventTrigger))]
    public class ActionEventTriggerInspector : TriggerInspector<ActionEventTrigger>
    {
        /// <summary>
        /// 当启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
        }

        /// <summary>
        /// 当禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
        }

        private string GetSelectedText(UnityEngine.Object model, UnityEngine.Object targetModel, string targetTypeFullName, EBinderRule binderRule, string selectedText, string hopeSelectedText)
        {
            switch (binderRule)
            {
                case EBinderRule.Static:
                    {
                        if (model.GetType().FullNameToHierarchyString() == targetTypeFullName) return hopeSelectedText;
                        break;
                    }
                case EBinderRule.Instance:
                    {
                        if (model == targetModel) return hopeSelectedText;
                        break;
                    }
            }
            return selectedText;
        }

        /// <summary>
        /// 当绘制成员
        /// </summary>
        /// <param name="serializedProperty"></param>
        protected override void OnDrawMember(SerializedProperty serializedProperty, PropertyData propertyData)
        {
            switch (serializedProperty.name)
            {
                case nameof(ActionEventTrigger._actionEventListener):
                    {
                        var trigger = this.trigger;
                        var inState = trigger.parent.inState;

                        var targetModel = trigger.target;
                        var targetTypeFullName = trigger.targetTypeFullName;
                        var typeBindRule = trigger.typeBindRule;

                        var dictionary = new Dictionary<string, Model>();
                        dictionary.Add(inState.name + "(入状态)", inState);
                        var selectText = GetSelectedText(inState, targetModel, targetTypeFullName, typeBindRule, "", inState.name + "(入状态)");
                        inState.GetComponents().Foreach(c =>
                        {
                            dictionary.Add("组件/" + c.name, c);
                            selectText = GetSelectedText(c, targetModel, targetTypeFullName, typeBindRule, selectText, "组件/" + c.name);
                        });
                        var newSelectText = UICommonFun.Popup(CommonFun.TempContent("快捷设置", "快捷设置入状态(组件)为监听器目标对象(或目标类型)"), selectText, dictionary.Keys.ToArray(), false);
                        if (newSelectText != selectText)
                        {
                            if (dictionary.TryGetValue(newSelectText, out var model))
                            {
                                switch (typeBindRule)
                                {
                                    case EBinderRule.Instance:
                                        {
                                            trigger.target = model;
                                            break;
                                        }
                                    case EBinderRule.Static:
                                        {
                                            trigger.targetType = model.GetType();
                                            if (trigger._actionEventListener.eventMethodInfo is MethodInfo method)
                                            {
                                                var parameters = method.GetParameters();
                                                var count = parameters.Length;
                                                if (count > 0)
                                                {
                                                    trigger.checkArguments = true;
                                                    trigger.XModifyProperty(() => {
                                                        trigger._actionEventListener.argumentDetections.Clear();
                                                    });

                                                    for (int i = 0; i < parameters.Length; i++)
                                                    {
                                                        var p = parameters[i];
                                                        if (p.ParameterType.IsAssignableFrom(model.GetType()))
                                                        {
                                                            trigger.XModifyProperty(() =>
                                                            {
                                                                var data = new ArgumentDetection();
                                                                data._index = i;
                                                                data._argument._argumentType = EArgumentType.UnityObject;
                                                                data._argument._objectValue = model;
                                                                trigger._actionEventListener.argumentDetections.Add(data);
                                                            });
                                                        }
                                                    }
                                                }

                                            }
                                            break;
                                        }
                                }
                            }
                        }
                        break;
                    }
            }
            base.OnDrawMember(serializedProperty, propertyData);
        }

        /// <summary>
        /// 显示辅助信息
        /// </summary>
        protected override bool displayHelpInfo => true;

        /// <summary>
        /// 获取辅助信息
        /// </summary>
        /// <returns></returns>
        public override StringBuilder GetHelpInfo()
        {
            var info = base.GetHelpInfo();
            if (trigger._actionEventListener.eventMethodInfo is MethodInfo method)
            {
                var parameters = method.GetParameters();
                info.AppendFormat("事件参数数目:\t{0}", parameters.Length.ToString());
                for (int i = 0; i < parameters.Length; i++)
                {
                    var p = parameters[i];
                    info.AppendFormat("\n事件参数[{0}]类型:\t{1}", i.ToString(), p.ParameterType.FullName);
                }
            }
            else
            {
                info.AppendFormat("<color=#FF0000FF>事件字段无效</color>");
            }
            return info;
        }
    }
}
