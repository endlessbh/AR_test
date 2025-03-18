using System;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorTools.Base;
using XCSJ.EditorTools.PropertyDatas;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginTools.AI.Autos;

namespace XCSJ.EditorTools.AI.Autos
{
    /// <summary>
    /// 自动启用组件(根据运行时平台)检查器
    /// </summary>
    [Name("自动启用组件(根据运行时平台)检查器")]
    [CustomEditor(typeof(AutoEnableComponentByRuntimePlatform), true)]
    public class AutoEnableComponentByRuntimePlatformInspector : InteractProviderInspector<AutoEnableComponentByRuntimePlatform>
    {
        
    }
}
