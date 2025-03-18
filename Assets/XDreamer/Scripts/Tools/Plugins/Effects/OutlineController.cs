using UnityEngine;
using XCSJ.Attributes;
using XCSJ.CommonUtils.PluginHighlightingSystem;
using XCSJ.CommonUtils.PluginOutline;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginTools.Effects
{
    /// <summary>
    /// 轮廓线控制器:用于动态的添加真实的轮廓线组件（轮廓线组件需继承轮廓线接口）
    /// </summary>
    [DisallowMultipleComponent]
    [Name("轮廓线控制器")]
    [RequireManager(typeof(ToolsManager))]
    public class OutlineController : InteractProvider, IOutline
    {
        /// <summary>
        /// 轮廓线对象
        /// </summary>
        public IOutline outline
        {
            get
            {
                if (_outline==null)
                {
#if XDREAMER_PROJECT_URP
                    _outline = CommonFun.GetOrAddComponent<Outline>(gameObject);
#elif XDREAMER_PROJECT_HDRP

#else
                    _outline = CommonFun.GetOrAddComponent<Highlighter>(gameObject);
#endif
                }
                return _outline;
            }
        }
        private IOutline _outline;

        #region IOutline

        public bool canDisplay { get => outline.canDisplay; set => outline.canDisplay = value; }

        public bool isDisplay { get => outline.isDisplay; set => outline.isDisplay = value; }

        public void StartDisplay(IOutlineData outlineData) => outline.StartDisplay(outlineData);

        public void StopDisplay() => outline.StopDisplay();

        #endregion
    }

}
