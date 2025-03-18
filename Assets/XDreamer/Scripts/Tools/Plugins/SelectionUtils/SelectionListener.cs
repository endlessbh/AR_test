using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Runtime;

namespace XCSJ.PluginTools.SelectionUtils
{
    /// <summary>
    /// 选择集监听组件
    /// </summary>
    [XCSJ.Attributes.Icon(EIcon.Select)]
    [RequireManager(typeof(ToolsManager))]
    [Owner(typeof(ToolsManager))]
    public abstract class SelectionListener : Interactor
    {
        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            //计算对象
            Selection.selectionChanged += OnSelectionChanged;

            OnSelectionChanged(new GameObject[0], false);
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            Selection.selectionChanged -= OnSelectionChanged;
        }

        /// <summary>
        /// 当选择集变更
        /// </summary>
        /// <param name="oldSelections"></param>
        /// <param name="flag"></param>
        protected virtual void OnSelectionChanged(GameObject[] oldSelections, bool flag) { }
    }
}
