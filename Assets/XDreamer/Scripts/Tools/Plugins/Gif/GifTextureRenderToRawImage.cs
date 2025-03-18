using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginTools;
using XCSJ.Tools;

namespace XCSJ.PluginTools.Gif
{
    /// <summary>
    /// Gif纹理渲染到原始图像
    /// </summary>
    [Name("Gif纹理渲染到原始图像")]
    [XCSJ.Attributes.Icon(EIcon.GIF)]
    [Tool("多媒体", rootType = typeof(ToolsManager), groupRule = EToolGroupRule.None)]
    public class GifTextureRenderToRawImage : GifTextureRenderToTarget<RawImage>
    {
        /// <summary>
        /// 当更新帧纹理时
        /// </summary>
        /// <param name="texture2D"></param>
        protected override void OnUpdateFrameTexture(Texture2D texture2D)
        {
            if (!texture2D) return;
            _targetObjects.ForEach(img=>
            {
                if (img && img.isActiveAndEnabled)
                {
                    img.texture = texture2D;
                }
            });
        }
    }
}
