using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginTools;

namespace XCSJ.PluginTools.Gif
{
    /// <summary>
    /// Gif纹理渲染到图像
    /// </summary>
    [Name("Gif纹理渲染到图像")]
    [XCSJ.Attributes.Icon(EIcon.GIF)]
    [Tool("多媒体", rootType = typeof(ToolsManager), groupRule = EToolGroupRule.None)]
    public class GifTextureRenderToImage : GifTextureRenderToTarget<Image>
    {
        private Dictionary<Texture2D, Sprite> spriteDic = new Dictionary<Texture2D, Sprite>();

        /// <summary>
        /// 当更新帧纹理时
        /// </summary>
        /// <param name="texture2D"></param>
        protected override void OnUpdateFrameTexture(Texture2D texture2D)
        {
            if (!texture2D) return;

            _targetObjects.ForEach(img =>
            {
                if (img && img.isActiveAndEnabled)
                {
                    if (!spriteDic.TryGetValue(texture2D, out Sprite sprite))
                    {
                        sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2());
                        spriteDic.Add(texture2D, sprite);
                    }
                    img.overrideSprite = sprite;
                }
            });
        }
    }
}