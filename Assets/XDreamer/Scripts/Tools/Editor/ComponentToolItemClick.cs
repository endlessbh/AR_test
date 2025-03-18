using UnityEngine;
using XCSJ.EditorCommonUtils.Base.CategoryViews;

namespace XCSJ.EditorTools
{
    /// <summary>
    /// 组件工具项点击
    /// </summary>
    [ToolItemClick(typeof(Component), true)]
    public class ComponentToolItemClick : IItemClick
    {
        /// <summary>
        /// 能否点击
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool CanClick(IItem item)
        {
            return true;
        }

        /// <summary>
        /// 点击
        /// </summary>
        /// <param name="item"></param>
        public void OnClick(IItem item)
        {
            if (item is ToolItem toolItem)
            {
                if (EditorToolsHelperExtension.CanCreateTool(toolItem))
                {
                    EditorToolsHelperExtension.CreateTool(item as ToolItem);
                }
            }
        }
    }

}
