using System;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginDataBase.Tools;

namespace XCSJ.EditorDataBase.Tools
{
    /// <summary>
    /// 网络数据库检查器
    /// </summary>
    [CustomEditor(typeof(NetDBMB))]
    [Name("网络数据库检查器")]
    public class NetDBMBInspector : DBMBInspector<NetDBMB>
    {
    }
}
