using UnityEditor;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension.Base.Tools;
using XCSJ.EditorTools;
using XCSJ.PluginsCameras.Base;

namespace XCSJ.EditorCameras.Tools
{
    /// <summary>
    /// 基础相机子控制器工具项点击
    /// </summary>
    [ToolItemClick(typeof(BaseCameraSubController), true)]
    public class BaseCameraSubComtrollerToolItemClick : ToolItemClickForUnityEditorSelection<BaseCameraMainController>
    {
        /// <summary>
        /// 在播放状态下允许添加
        /// </summary>
        public override bool canClickInPlaying => true;

        /// <summary>
        /// 如果父级中存在组件则认为有组件
        /// </summary>
        /// <typeparam name="TComponent"></typeparam>
        /// <returns></returns>
        public override bool HasComponent<TComponent>()
        {
            return Selection.activeGameObject.GetComponentInParent(typeof(TComponent));
        }
    }
}
