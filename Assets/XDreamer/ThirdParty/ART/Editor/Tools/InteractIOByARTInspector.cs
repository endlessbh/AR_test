using UnityEditor;
using XCSJ.Attributes;
using XCSJ.EditorXXR.Interaction.Toolkit.Tools.Controllers;
using XCSJ.PluginART.Tools;

namespace XCSJ.EditorART.Tools
{
    /// <summary>
    /// 交互IO通过ART检查器
    /// </summary>
    [CustomEditor(typeof(InteractIOByART), true)]
    [Name("交互IO通过ART检查器 ")]
    public class InteractIOByARTInspector: BaseAnalogControllerIOInspector<InteractIOByART>
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
