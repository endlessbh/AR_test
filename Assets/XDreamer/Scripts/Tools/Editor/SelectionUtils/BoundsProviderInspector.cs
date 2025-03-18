using System;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginTools.SelectionUtils;

namespace XCSJ.EditorTools.SelectionUtils
{
    /// <summary>
    /// 包围盒提供器检查器
    /// </summary>
    [Name("包围盒提供器检查器")]
    [CustomEditor(typeof(BoundsProvider))]
    public class BoundsProviderInspector : MBInspector<BoundsProvider>
    {
    }
}
