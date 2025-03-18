using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace XCSJ.PluginTools.Gif
{
    /// <summary>
    /// GIF组手
    /// </summary>
    public static class GifHelper
    {
        /// <summary>
        /// 获取或创建GIF纹理更新器
        /// </summary>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        public static GifTextureUpdater GetOrCreateGifTextureUpdater(GameObject gameObject)
        {
            if (!gameObject) return default;

            var gifTextureUpdater = gameObject.GetComponent<GifTextureUpdater>();
            if (gifTextureUpdater) return gifTextureUpdater;

            var image = gameObject.GetComponent<Image>();
            if (image)
            {
                return gameObject.AddComponent<GifTextureRenderToImage>();
            }

            var rawImage = gameObject.GetComponent<RawImage>();
            if (rawImage)
            {
                return gameObject.AddComponent<GifTextureRenderToRawImage>();
            }

            var renderer = gameObject.GetComponent<Renderer>();
            if (renderer)
            {
                return gameObject.AddComponent<GifTextureRenderToRenderer>();
            }

            return default;
        }
    }
}
