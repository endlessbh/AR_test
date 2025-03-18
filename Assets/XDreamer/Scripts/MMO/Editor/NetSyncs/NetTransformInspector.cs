using UnityEditor;
using XCSJ.Attributes;
using XCSJ.PluginMMO.NetSyncs;

namespace XCSJ.EditorMMO.NetSyncs
{
    /// <summary>
    /// 网络变换检查器
    /// </summary>
    [Name("网络变换检查器")]
    [CustomEditor(typeof(NetTransform), true)]
    public class NetTransformInspector: NetMBInspector<NetTransform>
    {
    }
}
