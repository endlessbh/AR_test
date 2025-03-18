using UnityEngine;
using XCSJ.Attributes;

namespace XCSJ.PluginXGUI.Styles.Base
{
    public interface IStyleRoot { }

    /// <summary>
    /// 风格
    /// </summary>
    [Name("样式")]
    public sealed class XStyle : StyleElementCollection, IStyleRoot
    {
        /// <summary>
        /// 新建
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static XStyle New(string name)
        {
            var style = ScriptableObject.CreateInstance<XStyle>();
            style.name = name;
            style.Init();
            XStyleCache.Register(style);
            return style;
        }

        /// <summary>
        /// 创建默认样式
        /// </summary>
        public void Init()
        {
            StyleHelper.baseStyleElementTypes.ForEach(t => CreateElement(t, GetDefaultName(t)));
        }

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            var obj = base.Clone();
            if (obj != null)
            {
                XStyleCache.Register(obj as XStyle);
            }
            return obj;
        }
    }
}
