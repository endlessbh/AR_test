using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace XCSJ.EditorExtension.XAssets.Libs
{
    public class Image
    {
        public bool imageLoaded = false;

        public bool imageStartedLoading = false;

        public Texture2D image = null;

        public Image(string imageID = "", string label = "", bool isAssetImage = false, int height = 100, int width = 100)
        {
            bool flag = imageID == "";
            if (!flag)
            {
                this.imageStartedLoading = true;
                ImageDownloaderJob job;
                job = new ImageDownloaderJob(imageID);

                ImageDownloader.Instance.AddJob(job, delegate (Texture2D image)
                {
                    this.image = image;
                    this.imageLoaded = true;
                    this.imageStartedLoading = false;
                    if (this.image != null)
                    {
                        this.PostLoadProcess();
                    }
                });
            }
        }

        public virtual void PostLoadProcess()
        {
        }
    }
}
