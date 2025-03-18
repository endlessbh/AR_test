using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorSMS.Inspectors;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginMMO.NetSyncs;
using XCSJ.PluginMMO.States;

namespace XCSJ.EditorMMO.States
{
    /// <summary>
    /// 设置网络属性检查器
    /// </summary>
    [Name("设置网络属性检查器")]
    [CustomEditor(typeof(SetNetProperty), true)]
    public class SetNetPropertyInspector: StateComponentInspector<SetNetProperty>
    {

    }
}
