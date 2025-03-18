using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginsCameras.Base;

namespace XCSJ.PluginsCameras.Tools.Base
{
    /// <summary>
    /// 基础相机限定器
    /// </summary>
    public abstract class BaseCameraLimiter : BaseCameraToolController
    {
        /// <summary>
        /// 在编辑态限定生效
        /// </summary>
        [Name("在编辑态限定生效")]
        [Tip("为True时，在编辑器的非运行态下（即编辑态）,会尝试执行对应的限定机制，但具体的限定机制能否在编辑态下有效工作，需要查阅具体功能的限定器组件；", "If it is true, when the editor is not in the running state (i.e. editing state), it will try to execute the corresponding qualification mechanism, but whether the specific qualification mechanism can work effectively in the editing state depends on the qualifier component of the specific function;")]
        public bool _limitEffectInEdit = true;

        /// <summary>
        /// 绘制Gizmos
        /// </summary>
        protected virtual void OnDrawGizmos()
        {
            if (Application.isPlaying || !_limitEffectInEdit) return;
            ExcuteLimitInEdit();
        }

        /// <summary>
        /// 在编辑态执行限定
        /// </summary>
        protected virtual void ExcuteLimitInEdit() { }
    }
}
