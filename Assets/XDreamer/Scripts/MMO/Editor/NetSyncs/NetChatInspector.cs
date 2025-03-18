using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginMMO.NetSyncs;

namespace XCSJ.EditorMMO.NetSyncs
{
    /// <summary>
    /// 网络聊天检查器
    /// </summary>
    [Name("网络聊天检查器")]
    [CustomEditor(typeof(NetChat), true)]
    public class NetChatInspector : NetMBInspector<NetChat>
    {
        /// <summary>
        /// 当绘制检查器GUI
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (!mb.GetComponent<VoiceChat>())
            {
                if (GUILayout.Button(CommonFun.NameTip(typeof(VoiceChat))))
                {
                    mb.XAddComponent<VoiceChat>();
                }
            }
        }
    }
}
