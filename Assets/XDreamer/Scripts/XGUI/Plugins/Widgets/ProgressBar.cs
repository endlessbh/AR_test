using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginTools.Base;
using XCSJ.PluginXGUI.Base;

namespace XCSJ.PluginXGUI.Widgets
{
    /// <summary>
    /// 进度条
    /// </summary>
    [Name("进度条")]
    public class ProgressBar : View
    {
        /// <summary>
        /// 进度完成后激活
        /// </summary>
        [Name("进度完成后激活")]
        public bool _finishActive = false;

        /// <summary>
        /// 进度百分比小数点位数
        /// </summary>
        [Name("进度百分比小数点位数")]
        [Range(0, 3)]
        public int _decimalPoinDigit = 1;

        /// <summary>
        /// 标题
        /// </summary>
        public string title => progress?.progressTitle;

        /// <summary>
        /// 进度文本
        /// </summary>
        public string progressText => (progressValue * 100).ToString("F" + _decimalPoinDigit) + "%";

        /// <summary>
        /// 进度值
        /// </summary>
        public float progressValue => progress != null ? progress.progressValue : 0;

        /// <summary>
        /// 进度对象
        /// </summary>
        private IProgress progress;

        public void SetData(IProgress progress)
        {
            this.progress = progress;
        }

        public void ResetData()
        {
            this.progress = null;
        }

        private void Update()
        {
            if (progress != null && progress.progressValue >= 1)
            {
                gameObject.SetActive(_finishActive);
            }
        }
    }
    
}
