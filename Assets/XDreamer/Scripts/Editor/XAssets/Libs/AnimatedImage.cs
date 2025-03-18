using System;
using UnityEngine;

namespace XCSJ.EditorExtension.XAssets.Libs
{
    public class AnimatedImage : Image
    {
        public Texture2D[] sprites = null;

        public int numFrames = 0;

        public int framesPerSecond = 0;

        public int framesPerRow = 0;

        public int frameWidth = 0;

        public int frameHeight = 0;

        public int currFrameIndex = 0;

        public static float deltime = 0f;

        public AnimatedImage(string imageID = "", string label = "", bool isAssetImage = false, int numframes = 0, int fps = 0, int framesperrow = 0, int framewidth = 0, int frameheight = 0) : base(imageID, label, isAssetImage, 100, 100)
        {
            this.numFrames = numframes;
            this.framesPerSecond = fps;
            this.framesPerRow = framesperrow;
            this.frameWidth = framewidth;
            this.frameHeight = frameheight;
        }

        public void SplitSpriteSheet()
        {
            this.sprites = new Texture2D[this.numFrames];
            for (int i = 0; i < this.numFrames; i++)
            {
                int num = (int)(Math.Ceiling((double)this.numFrames / (double)this.framesPerRow) - 1.0) * this.frameHeight;
                int num2 = i % this.framesPerRow;
                int num3 = i / this.framesPerRow;
                this.sprites[i] = new Texture2D(this.frameWidth, this.frameHeight, TextureFormat.ARGB32, false, true);
                this.sprites[i].SetPixels(this.image.GetPixels(num2 * this.frameWidth, num - num3 * this.frameHeight, this.frameWidth, this.frameHeight));
                this.sprites[i].Apply();
            }
        }

        public void Update()
        {
            AnimatedImage.deltime += Time.fixedDeltaTime;
            bool flag = AnimatedImage.deltime > 1f / (float)this.framesPerSecond;
            if (flag)
            {
                this.currFrameIndex++;
                AnimatedImage.deltime = 0f;
            }
            bool flag2 = this.currFrameIndex >= this.numFrames;
            if (flag2)
            {
                this.currFrameIndex = 0;
            }
        }

        public override void PostLoadProcess()
        {
            base.PostLoadProcess();
            this.SplitSpriteSheet();
        }
    }
}
