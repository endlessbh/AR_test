using UnityEditor;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Base.CategoryViews;
using XCSJ.EditorTools;
using XCSJ.PluginXXR.Interaction.Toolkit.Tools.Controllers;

#if XDREAMER_XR_INTERACTION_TOOLKIT
using UnityEditor.XR.Interaction.Toolkit;
#endif

namespace XCSJ.EditorXXR.Interaction.Toolkit.Tools.Controllers
{
    /// <summary>
    /// 模拟控制器检查器
    /// </summary>
    [Name("模拟控制器检查器")]
    [CustomEditor(typeof(AnalogController))]
    public class AnalogControllerInspector
#if XDREAMER_XR_INTERACTION_TOOLKIT
        : XRBaseControllerEditor
#else
        : MBInspector<AnalogController>        
#endif
    {

#if XDREAMER_XR_INTERACTION_TOOLKIT

        private static CategoryList categoryList = null;

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            if (categoryList == null) categoryList = EditorToolsHelper.GetWithPurposes(nameof(AnalogController));
        }

        /// <summary>
        /// 绘制检查器
        /// </summary>
        protected override void DrawInspector()
        {
            base.DrawInspector();

            categoryList.DrawVertical();
        }

#endif
    }
}
