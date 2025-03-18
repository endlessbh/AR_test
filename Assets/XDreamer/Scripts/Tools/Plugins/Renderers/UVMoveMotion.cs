using UnityEngine;
using System.Collections;
using XCSJ;
using XCSJ.PluginCommonUtils;
using XCSJ.Tools;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.Extension.Interactions.Tools;

namespace XCSJ.PluginTools.Renderers
{
    /// <summary>
    /// 移动UV坐标组件
    /// </summary>
    [Tool("渲染器", rootType = typeof(ToolsManager))]
    [Name("UV平移")]
    [XCSJ.Attributes.Icon(EIcon.Material)]
    [RequireManager(typeof(ToolsManager))]
    [Owner(typeof(ToolsManager))]
    public class UVMoveMotion: InteractProvider
    {
        /// <summary>
        /// UV平移对象
        /// </summary>
        [Name("UV平移对象")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Renderer _renderer;

        /// <summary>
        /// UV移动速度
        /// </summary>
        [Name("UV移动速度")]
        public Vector2 uvOffsetSpeed = new Vector2(0.1f, 0.1f);

        protected void Start() { }

        protected void Update()
        {
            if (_renderer && _renderer.material)
            {
                _renderer.material.mainTextureOffset += Time.deltaTime * uvOffsetSpeed;
            }
        }
    }
}
