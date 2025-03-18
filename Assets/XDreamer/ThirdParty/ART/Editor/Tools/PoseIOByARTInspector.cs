using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorXXR.Interaction.Toolkit.Tools.Controllers;
using XCSJ.PluginART;
using XCSJ.PluginART.Tools;

namespace XCSJ.EditorART.Tools
{
    /// <summary>
    /// 姿态IO通过ART检查器
    /// </summary>
    [CustomEditor(typeof(PoseIOByART))]
    [Name("姿态IO通过ART检查器 ")]
    public class PoseIOByARTInspector : BaseAnalogControllerIOInspector<PoseIOByART>
    {
        /// <summary>
        /// 当绘制检查器GUI
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorARTHelper.DrawSelectARTManager();
        }
    }
}
