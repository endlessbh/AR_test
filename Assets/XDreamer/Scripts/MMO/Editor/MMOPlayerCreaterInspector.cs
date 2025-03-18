using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginMMO;

namespace XCSJ.EditorMMO
{
    /// <summary>
    /// 多人在线MMO-玩家生成器检查器
    /// </summary>
    [Name("多人在线MMO-玩家生成器检查器")]
    [CustomEditor(typeof(MMOPlayerCreater))]
    public class MMOPlayerCreaterInspector:MBInspector<MMOPlayerCreater>
    {
        /// <summary>
        /// 当绘制检查器GUI
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (!mb.GetComponent<MMOPlayerCreaterHUD>())
            {
                if (GUILayout.Button(CommonFun.NameTip(typeof(MMOPlayerCreaterHUD))))
                {
                    Undo.AddComponent<MMOPlayerCreaterHUD>(mb.gameObject);
                }
            }
        }
    }
}
