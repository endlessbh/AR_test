using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils.Base.CategoryViews;
using XCSJ.EditorCommonUtils.Base.Controls;
using XCSJ.PluginMMO;

namespace XCSJ.EditorMMO
{
    /// <summary>
    /// 网络标识检查器
    /// </summary>
    [Name("网络标识检查器")]
    [CustomEditor(typeof(NetIdentity))]
    public class NetIdentityInspector : MMOMBInspector<NetIdentity>
    {
        /// <summary>
        /// 当绘制检查器GUI
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (Application.isPlaying) return;
            categoryList.DrawVertical();
        }     
    }
}
