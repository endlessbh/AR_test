using UnityEditor;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginART.Tools;

namespace XCSJ.EditorART.Tools
{
    /// <summary>
    /// 基础变换通过ART检查器
    /// </summary>
    [CustomEditor(typeof(BaseTransformByART), true)]
    [Name("基础变换通过ART检查器 ")]
    public class BaseTransformByARTInspector : BaseTransformByARTInspector<BaseTransformByART>
    {
    }

    /// <summary>
    /// 基础变换通过ART检查器泛型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseTransformByARTInspector<T> : MBInspector<T>
        where T : BaseTransformByART
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
