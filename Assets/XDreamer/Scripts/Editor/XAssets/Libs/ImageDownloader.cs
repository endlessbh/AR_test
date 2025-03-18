using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace XCSJ.EditorExtension.XAssets.Libs
{
    public class ImageDownloader
    {
        private static ImageDownloader _instance;

        private List<ImageDownloaderJob> jobList = new List<ImageDownloaderJob>();

        public int maxJobCount = 10;

        public static ImageDownloader Instance
        {
            get
            {
                bool flag = ImageDownloader._instance == null;
                if (flag)
                {
                    ImageDownloader._instance = new ImageDownloader();
                }
                return ImageDownloader._instance;
            }
        }

        /// <summary>
        /// 获取任务数量
        /// </summary>
        /// <returns></returns>
        public int GetJobCount()
        {
            return this.jobList.Count;
        }

        /// <summary>
        /// 添加下载任务
        /// </summary>
        /// <param name="job"></param>
        /// <param name="callback"></param>
        public void AddJob(ImageDownloaderJob job, Action<Texture2D> callback)
        {
            job.processAction = callback;
            this.jobList.Add(job);
        }

        /// <summary>
        /// 更新任务
        /// </summary>
        public void UpdateJobs()
        {
            for (int i = this.jobList.Count - 1; i >= 0; i--)
            {
                this.jobList[i].Update();
                bool mWorkDone = this.jobList[i].workDone;
                if (mWorkDone)
                {
                    this.jobList[i].processAction(this.jobList[i].texture);
                    this.jobList.RemoveAt(i);
                }
            }
        }
    }
}
