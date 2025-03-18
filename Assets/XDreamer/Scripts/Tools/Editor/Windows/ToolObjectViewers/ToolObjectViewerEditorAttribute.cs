using System;
using XCSJ.Algorithms;

namespace XCSJ.EditorTools.Windows
{
    /// <summary>
    /// 工具对象查看器编辑器特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class ToolObjectViewerEditorAttribute : LinkedTypeAttribute
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="type"></param>
        /// <param name="forChildClasses"></param>
        public ToolObjectViewerEditorAttribute(Type type, bool forChildClasses = false) : base(type, forChildClasses, nameof(ToolObjectViewerEditorAttribute)) { }

        /// <summary>
        /// 获取编辑器类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Type GetEditorType(Type type) => GetLinkedType(type, nameof(ToolObjectViewerEditorAttribute));
    }
}
