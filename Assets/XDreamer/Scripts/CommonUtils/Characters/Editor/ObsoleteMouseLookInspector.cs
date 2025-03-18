using System;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.CommonUtils.PluginCharacters;
using XCSJ.EditorCommonUtils;
using XCSJ.Maths;
using XCSJ.PluginCommonUtils;

namespace XCSJ.CommonUtils.EditorCharacters
{
    /// <summary>
    /// 鼠标查看检查器
    /// </summary>
    [Name("鼠标查看检查器")]
    [CustomEditor(typeof(ObsoleteMouseLook), true)]
    public class MouseLookInspector : MBInspector<ObsoleteMouseLook>
    {
    }
}
