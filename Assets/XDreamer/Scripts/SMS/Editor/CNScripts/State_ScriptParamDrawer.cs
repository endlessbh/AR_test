using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using XCSJ.Algorithms;
using XCSJ.Caches;
using XCSJ.Collections;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.CNScripts;
using XCSJ.Helper;
using XCSJ.PluginSMS.CNScripts;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.Scripts;

namespace XCSJ.EditorSMS.CNScripts
{
    /// <summary>
    /// 状态脚本参数绘制器
    /// </summary>
    [ScriptParamType(State_ScriptParam.State)]
    public class State_ScriptParamDrawer : ScriptParamDrawer
    {
        /// <summary>
        /// 当绘制值
        /// </summary>
        public override void OnDrawValue()
        {
            //base.OnDrawValue();
            EditorGUI.indentLevel = 2;

            Func<State, bool> predicate = null;
            if (param.limitType != null)
            {
                predicate = s => s.GetComponent(param.limitType, true) != null;
            }
            paramObject = EditorSMSHelper.StatePopup((State)paramObject, null, true, true, predicate);
        }
    }

    /// <summary>
    /// 状态组件脚本参数绘制器
    /// </summary>
    [ScriptParamType(StateComponent_ScriptParam.StateComponent)]
    public class StateComponent_ScriptParamDrawer : ScriptParamDrawer
    {
        /// <summary>
        /// 当绘制值
        /// </summary>
        public override void OnDrawValue()
        {
            //base.OnDrawValue();
            EditorGUI.indentLevel = 2;

            Func<StateComponent, bool> predicate = null;
            if (param.limitType != null)
            {
                predicate = s => param.limitType.IsAssignableFrom(s.GetType());
            }
            paramObject = EditorSMSHelper.StateComponentPopup((StateComponent)paramObject, null, true, true, true, predicate);
        }
    }

    /// <summary>
    /// 跳转脚本参数绘制器
    /// </summary>
    [ScriptParamType(Transition_ScriptParam.Transition)]
    public class Transition_ScriptParamDrawer : ScriptParamDrawer
    {
        /// <summary>
        /// 当绘制值
        /// </summary>
        public override void OnDrawValue()
        {
            //base.OnDrawValue();
            EditorGUI.indentLevel = 2;

            Func<Transition, bool> predicate = null;
            if (param.limitType != null)
            {
                predicate = s => s.GetComponent(param.limitType, true) != null;
            }
            paramObject = EditorSMSHelper.TransitionPopup((Transition)paramObject, null, true, true, predicate);
        }
    }

    /// <summary>
    /// 跳转组件脚本参数绘制器
    /// </summary>
    [ScriptParamType(TransitionComponent_ScriptParam.TransitionComponent)]
    public class TransitionComponent_ScriptParamDrawer : ScriptParamDrawer
    {
        /// <summary>
        /// 当绘制值
        /// </summary>
        public override void OnDrawValue()
        {
            //base.OnDrawValue();
            EditorGUI.indentLevel = 2;

            Func<TransitionComponent, bool> predicate = null;
            if (param.limitType != null)
            {
                predicate = s => param.limitType.IsAssignableFrom(s.GetType());
            }
            paramObject = EditorSMSHelper.TransitionComponentPopup((TransitionComponent)paramObject, null, true, true, true, predicate);
        }
    }

    /// <summary>
    /// 子状态机脚本参数绘制器
    /// </summary>
    [ScriptParamType(SubStateMachine_ScriptParam.SubStateMachine)]
    public class SubStateMachine_ScriptParamDrawer : ScriptParamDrawer
    {
        /// <summary>
        /// 当绘制值
        /// </summary>
        public override void OnDrawValue()
        {
            //base.OnDrawValue();
            EditorGUI.indentLevel = 2;

            Func<SubStateMachine, bool> predicate = null;
            if (param.limitType != null)
            {
                predicate = s => s.GetComponent(param.limitType, true) != null;
            }
            paramObject = EditorSMSHelper.SubStateMachinePopup((SubStateMachine)paramObject, null, true, true, predicate);
        }
    }

    /// <summary>
    /// 状态机脚本参数绘制器
    /// </summary>
    [ScriptParamType(StateMachine_ScriptParam.StateMachine)]
    public class StateMachine_ScriptParamDrawer : ScriptParamDrawer
    {
        /// <summary>
        /// 当绘制值
        /// </summary>
        public override void OnDrawValue()
        {
            //base.OnDrawValue();
            EditorGUI.indentLevel = 2;

            Func<StateMachine, bool> predicate = null;
            if (param.limitType != null)
            {
                predicate = s => s.GetComponent(param.limitType, true) != null;
            }
            paramObject = EditorSMSHelper.StateMachinePopup((StateMachine)paramObject, true, predicate);
        }
    }

    /// <summary>
    /// 状态组脚本参数绘制器
    /// </summary>
    [ScriptParamType(StateGroup_ScriptParam.StateGroup)]
    public class StateGroup_ScriptParamDrawer : ScriptParamDrawer
    {
        /// <summary>
        /// 当绘制值
        /// </summary>
        public override void OnDrawValue()
        {
            //base.OnDrawValue();
            EditorGUI.indentLevel = 2;

            Func<StateGroup, bool> predicate = null;
            if (param.limitType != null)
            {
                predicate = s => s.GetComponent(param.limitType, true) != null;
            }
            paramObject = EditorSMSHelper.StateGroupPopup((StateGroup)paramObject, null, true, true, predicate);
        }
    }

    /// <summary>
    /// 状态组组件脚本参数绘制器
    /// </summary>
    [ScriptParamType(StateGroupComponent_ScriptParam.StateGroupComponent)]
    public class StateGroupComponent_ScriptParamDrawer : ScriptParamDrawer
    {
        /// <summary>
        /// 当绘制值
        /// </summary>
        public override void OnDrawValue()
        {
            //base.OnDrawValue();
            EditorGUI.indentLevel = 2;

            Func<StateGroupComponent, bool> predicate = null;
            if (param.limitType != null)
            {
                predicate = s => s.GetComponent(param.limitType, true) != null;
            }
            paramObject = EditorSMSHelper.StateGroupComponentPopup((StateGroupComponent)paramObject, null, true, true, true, predicate);
        }
    }

    /// <summary>
    /// 状态组件成员脚本参数绘制器
    /// </summary>
    [ScriptParamType(StateComponentMember_ScriptParam.StateComponentMember)]
    public class StateComponentMember_ScriptParamDrawer : ScriptParamDrawer
    {
        /// <summary>
        /// 当绘制值
        /// </summary>
        public override void OnDrawValue()
        {
            //base.OnDrawValue();
            EditorGUI.indentLevel = 2;

            if (!(paramObject is ReflectionInfo info))
            {
                paramObject = new ReflectionInfo();
                return;
            }
            EditorGUILayout.BeginHorizontal();
            Func<StateComponent, bool> predicate = null;
            if (param.limitType != null)
            {
                predicate = s => param.limitType.IsAssignableFrom(s.GetType());
            }
            info.component = EditorSMSHelper.StateComponentPopup(info.component as StateComponent, null, true, true, true, predicate);
            info.memberName = UICommonFun.Popup(info.memberName, ReflectionCache.Get(info.component?.GetType()));
            EditorGUILayout.EndHorizontal();
        }
    }

    /// <summary>
    /// 跳转组件成员脚本参数绘制器
    /// </summary>
    [ScriptParamType(TransitionComponentMember_ScriptParam.TransitionComponentMember)]
    public class TransitionComponentMember_ScriptParamDrawer : ScriptParamDrawer
    {
        /// <summary>
        /// 当绘制值
        /// </summary>
        public override void OnDrawValue()
        {
            //base.OnDrawValue();
            EditorGUI.indentLevel = 2;

            if (!(paramObject is ReflectionInfo info))
            {
                paramObject = new ReflectionInfo();
                return;
            }
            EditorGUILayout.BeginHorizontal();
            Func<TransitionComponent, bool> predicate = null;
            if (param.limitType != null)
            {
                predicate = s => param.limitType.IsAssignableFrom(s.GetType());
            }
            info.component = EditorSMSHelper.TransitionComponentPopup(info.component as TransitionComponent, null, true, true, true, predicate);
            info.memberName = UICommonFun.Popup(info.memberName, ReflectionCache.Get(info.component?.GetType()));
            EditorGUILayout.EndHorizontal();
        }
    }

    /// <summary>
    /// 状态组组件成员脚本参数绘制器
    /// </summary>
    [ScriptParamType(StateGroupComponentMember_ScriptParam.StateGroupComponentMember)]
    public class StateGroupComponentMember_ScriptParamDrawer : ScriptParamDrawer
    {
        /// <summary>
        /// 当绘制值
        /// </summary>
        public override void OnDrawValue()
        {
            //base.OnDrawValue();
            EditorGUI.indentLevel = 2;

            if (!(paramObject is ReflectionInfo info))
            {
                paramObject = new ReflectionInfo();
                return;
            }
            EditorGUILayout.BeginHorizontal();
            Func<StateGroupComponent, bool> predicate = null;
            if (param.limitType != null)
            {
                predicate = s => param.limitType.IsAssignableFrom(s.GetType());
            }
            info.component = EditorSMSHelper.StateGroupComponentPopup(info.component as StateGroupComponent, null, true, true, true, predicate);
            info.memberName = UICommonFun.Popup(info.memberName, ReflectionCache.Get(info.component?.GetType()));
            EditorGUILayout.EndHorizontal();
        }
    }

    /// <summary>
    /// 反射缓存
    /// </summary>
    class ReflectionCache : TICache<ReflectionCache, Type, string[]>
    {
        /// <summary>
        /// 创建值
        /// </summary>
        /// <param name="key1"></param>
        /// <returns></returns>
        protected override KeyValuePair<bool, string[]> CreateValue(Type key1)
        {
            var fields = FieldInfosCache.Get(key1, TypeHelper.DefaultLookupHierarchy | BindingFlags.NonPublic).Cast(fi => fi.Name);
            var properties = PropertyInfosCache.Get(key1, TypeHelper.DefaultLookupHierarchy | BindingFlags.NonPublic).Cast(pi => pi.Name);
            return new KeyValuePair<bool, string[]>(true, fields.Concat(properties).Distinct().ToArray());
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string[] Get(Type type)
        {
            if (type == null) return Empty<string>.Array;
            Cache.TryGetValue(type, out string[] value, true);
            return value;
        }
    }
}
