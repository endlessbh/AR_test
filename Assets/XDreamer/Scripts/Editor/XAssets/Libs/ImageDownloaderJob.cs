using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace XCSJ.EditorExtension.XAssets.Libs
{
    public class ImageDownloaderJob
    {
        /// <summary>
        /// 结束回调事件
        /// </summary>
        public Action<Texture2D> processAction;

        /// <summary>
        /// 工作完成
        /// </summary>
        public bool workDone = false;

        /// <summary>
        /// 贴图
        /// </summary>
        public Texture2D texture;

        /// <summary>
        /// 缓存
        /// </summary>
        public bool cache = false;

        /// <summary>
        /// 是否网络图片
        /// </summary>
        public bool webImage = false;

        /// <summary>
        /// 文件存储路径
        /// </summary>
        public string fileStorePath = "";

        /// <summary>
        /// 图片URL
        /// </summary>
        public string imageUrl = "";

        /// <summary>
        /// 文件是否加载
        /// </summary>
        public bool fileLoaded = false;

        /// <summary>
        /// 缓存时间
        /// </summary>
        private const int CACHE_IMAGE_TIME_TO_LIVE = 30;

        public ImageDownloaderJob(string filepath)
        {
            bool flag = File.Exists(filepath);
            if (flag)
            {
                this.imageUrl = filepath;
                this.fileLoaded = true;
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        public void Update()
        {
            bool flag = this.fileLoaded;
            if (flag)
            {
                if (File.Exists(this.imageUrl))
                {
                    this.texture = new Texture2D(2, 2, TextureFormat.ARGB32, false, false);
                    bool loaded = ImageConversion.LoadImage(this.texture, File.ReadAllBytes(this.imageUrl), false);
                }
                this.workDone = true;
            }
        }
    }
}
