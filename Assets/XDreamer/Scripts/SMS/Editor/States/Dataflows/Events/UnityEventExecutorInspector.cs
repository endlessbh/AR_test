using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorSMS.Inspectors;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.States.Dataflows.Events;

namespace XCSJ.EditorSMS.States.Dataflows.Events
{
    /// <summary>
    /// Unity事件执行器检查器
    /// </summary>
    [CustomEditor(typeof(UnityEventExecutor), true)]
    public class UnityEventExecutorInspector : StateComponentInspector
    {
    }
}
