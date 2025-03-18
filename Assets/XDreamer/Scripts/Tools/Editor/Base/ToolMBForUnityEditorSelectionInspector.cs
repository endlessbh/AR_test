using System;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorTools.PropertyDatas;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginTools;
using XCSJ.PluginTools.Base;

namespace XCSJ.EditorTools.Base
{
    /// <summary>
    /// Unity编辑器选择集专用的工具组件检查器
    /// </summary>
    [Name("Unity编辑器选择集专用的工具组件检查器")]
    [CustomEditor(typeof(ToolMBForUnityEditorSelection), true)]
    public class ToolMBForUnityEditorSelectionInspector : ToolMBForUnityEditorSelectionInspector<ToolMBForUnityEditorSelection>
    {
    }

    /// <summary>
    /// Unity编辑器选择集专用的工具组件检查器泛型类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ToolMBForUnityEditorSelectionInspector<T> : InteractProviderInspector<T> where T : ToolMBForUnityEditorSelection
    {
    }
}
