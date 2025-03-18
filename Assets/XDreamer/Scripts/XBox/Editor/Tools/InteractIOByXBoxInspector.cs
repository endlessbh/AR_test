using UnityEditor;
using XCSJ.Attributes;
using XCSJ.EditorXXR.Interaction.Toolkit.Tools.Controllers;
using XCSJ.PluginXBox.Tools;

namespace XCSJ.EditorXBox.Tools
{
    /// <summary>
    /// 交互IO通过XBox检查器
    /// </summary>
    [Name("交互IO通过XBox检查器")]
    [CustomEditor(typeof(InteractIOByXBox), true)]
    public class InteractIOByXBoxInspector: BaseAnalogControllerIOInspector<InteractIOByXBox>
    {
        /// <summary>
        /// 当绘制检查器GUI
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorXBoxHelper.DrawSelectXBoxManager();
        }
    }
}
