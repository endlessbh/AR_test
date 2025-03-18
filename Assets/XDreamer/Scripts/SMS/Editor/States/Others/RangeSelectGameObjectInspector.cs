using System;
using UnityEditor;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorSMS.States.Base;
using XCSJ.PluginSMS.States.Base;
using XCSJ.PluginSMS.States.Others;
using XCSJ.PluginSMS.States.Selections;

namespace XCSJ.EditorSMS.States.Others
{
    /// <summary>
    /// 区间选择游戏对象检查器
    /// </summary>
    [Name("区间选择游戏对象检查器")]
    [CustomEditor(typeof(RangeSelectGameObject))]
    public class RangeSelectGameObjectInspector : WorkClipInspector<RangeSelectGameObject>
    {

    }
}

