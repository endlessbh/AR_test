using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginTools;

namespace XCSJ.PluginTools.Gif
{
    /// <summary>
    /// Gif纹理渲染到渲染器
    /// </summary>
    [Name("Gif纹理渲染到渲染器")]
    [XCSJ.Attributes.Icon(EIcon.GIF)]
    [Tool("多媒体", rootType = typeof(ToolsManager), groupRule = EToolGroupRule.None)]
    public class GifTextureRenderToRenderer : GifTextureRenderToTarget<Renderer>
    {
        /// <summary>
        /// 当更新帧纹理时
        /// </summary>
        /// <param name="texture2D"></param>
        protected override void OnUpdateFrameTexture(Texture2D texture2D)
        {
            if (!texture2D) return;

            _targetObjects.ForEach(render =>
            {
                if (render && render.enabled && render.material)
                {
                    render.material.mainTexture = texture2D;
                }
            });
        }
    }
}
