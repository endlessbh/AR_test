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
    /// 网络属性比较检查器
    /// </summary>
    [Name("网络属性比较检查器")]
    [CustomEditor(typeof(NetPropertyCompare), true)]
    public class NetPropertyCompareInspector : StateComponentInspector<NetPropertyCompare>
    {

    }
}
