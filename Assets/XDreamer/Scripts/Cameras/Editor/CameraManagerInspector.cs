using System;
using UnityEditor;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Base.Controls;
using XCSJ.EditorCommonUtils.Base.CategoryViews;
using XCSJ.EditorTools;
using XCSJ.PluginCamera;
using XCSJ.Attributes;

namespace XCSJ.EditorCameras
{
    /// <summary>
    /// 相机管理器检查器
    /// </summary>
    [CustomEditor(typeof(CameraManager))]
    [Name("相机管理器检查器")]
    public class CameraManagerInspector : BaseManagerInspector<CameraManager>
    {
        private static CategoryList categoryList = null;

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            if (categoryList == null) categoryList = EditorToolsHelper.GetWithPurposes(nameof(CameraManager));
            var targetObject = this.targetObject;
            if (targetObject)
            {
                if (!targetObject.cameraManagerProvider) { }
            }
        }

        /// <summary>
        /// 当绘制脚本
        /// </summary>
        /// <param name="serializedProperty"></param>
        protected override void OnDrawScript(SerializedProperty serializedProperty)
        {
            base.OnDrawScript(serializedProperty);
            categoryList.DrawVertical();
        }
    }
}
