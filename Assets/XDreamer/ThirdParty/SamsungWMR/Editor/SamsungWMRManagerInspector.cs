using UnityEditor;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginSamsungWMR;

namespace XCSJ.EditorSamsungWMR
{
    /// <summary>
    /// 三星玄龙管理器检查器
    /// </summary>
    [Name("三星玄龙管理器检查器")]
    [CustomEditor(typeof(SamsungWMRManager))]
    public class SamsungWMRManagerInspector : BaseManagerInspector<SamsungWMRManager>
    {
        /// <summary>
        /// 当绘制检查器GUI
        /// </summary>
        public override void OnInspectorGUI()
        {
            // 安装XR交互工具包
            XCSJ.EditorXXR.Interaction.Toolkit.XXRInteractionToolkitManagerInspector.InstallXRInteractionToolkitPackage();
            base.OnInspectorGUI();
        }
    }
}
