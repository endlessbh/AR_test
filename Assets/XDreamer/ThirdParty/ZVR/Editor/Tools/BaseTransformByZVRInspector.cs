using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCameras.Base;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginZVR;
using XCSJ.PluginZVR.Tools;

namespace XCSJ.EditorZVR.Tools
{
    /// <summary>
    /// 基础变换通过ZVR检查器
    /// </summary>
    [Name("基础变换通过ZVR检查器")]
    [CustomEditor(typeof(BaseTransformByZVR), true)]
    public class BaseTransformByZVRInspector : BaseTransformByZVRInspector<BaseTransformByZVR>
    {
    }

    /// <summary>
    /// 基础变换通过ZVR检查器泛型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseTransformByZVRInspector<T> : MBInspector<T>
        where T : BaseTransformByZVR
    {
        /// <summary>
        /// 当绘制检查器GUI
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorZVRHelper.DrawSelectZVRManager();
        }
    }
}
