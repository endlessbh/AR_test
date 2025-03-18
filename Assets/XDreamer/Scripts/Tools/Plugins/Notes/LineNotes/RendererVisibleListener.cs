using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Interactions.Tools;

namespace XCSJ.PluginTools.LineNotes
{
    /// <summary>
    /// 渲染器可视监听器
    /// </summary>
    [Name("渲染器可视监听器")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Renderer))]
    public class RendererVisibleListener : InteractProvider
    {
        /// <summary>
        /// 线性批注
        /// </summary>
        public LineNote lineNote { get; set; }

        /// <summary>
        /// 设置
        /// </summary>
        /// <param name="ln"></param>
        public void Set(LineNote ln)
        {
            this.lineNote = ln;
        }

        /// <summary>
        /// 渲染器可见
        /// </summary>
        protected void OnBecameVisible()
        {
            lineNote.targetVisable = true;
        }

        /// <summary>
        /// 渲染器不可见
        /// </summary>
        protected void OnBecameInvisible()
        {
            lineNote.targetVisable = false;
        }
    }
}
