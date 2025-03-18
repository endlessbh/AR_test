using System;
using UnityEditor;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginSMS.States.Base;
using XCSJ.PluginSMS.States.Motions;

namespace XCSJ.EditorSMS.States.Motions
{
    /// <summary>
    /// 渲染器颜色区间检查器
    /// </summary>
    [Name("渲染器颜色区间检查器")]
    [CustomEditor(typeof(RendererColorRange))]
    public class RendererColorRangeInspector : RendererRangeHandleInspector<RendererColorRange>
    {

    }
}
