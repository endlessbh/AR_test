using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.EditorCommonUtils;
using XCSJ.Languages;
using XCSJ.PluginTools.LineNotes;

namespace XCSJ.EditorTools.Inspectors
{
    /// <summary>
    /// 线样式检查器
    /// </summary>
    [Name("线样式检查器")]
    [CustomEditor(typeof(LineStyle))]
    public class LineStyleInspector : MBInspector<LineStyle>
    {
        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            if (targetObject && targetObject.mat == null)
            {
                targetObject.mat = AssetDatabase.LoadAssetAtPath(UICommonFun.Assets + "/XDreamer-Assets/基础/Materials/常用/ColoredBlended.mat", typeof(Material)) as Material;
            }
        }
    }
}
