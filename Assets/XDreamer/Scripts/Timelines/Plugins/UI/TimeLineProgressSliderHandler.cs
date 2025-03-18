using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginTimelines.Tools;
using XCSJ.PluginXGUI.Base;

namespace XCSJ.PluginTimelines.UI
{
    /// <summary>
    /// 时间轴进度条处理器：
    /// 当进度滑动条拖拽时包含【音频状态组件】播放声音会出现混乱，因此滑动条按下时需暂停播放，弹起时重设为按下状态
    /// </summary>
    [Name("时间轴进度条处理器")]
    [RequireComponent(typeof(Slider))]
    [RequireManager(typeof(TimelineManager))]
    public sealed class TimeLineProgressSliderHandler : View, IPointerDownHandler, IPointerUpHandler
    {
        /// <summary>
        /// 播放控制器
        /// </summary>
        [Name("播放控制器")]
        public PlayerController _playerController;

        /// <summary>
        /// 播放控制器
        /// </summary>
        public PlayerController playerController => this.XGetComponentInChildrenOrGlobal(ref _playerController);

        private bool _playing = false;

        /// <summary>
        /// 指针按下
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerDown(PointerEventData eventData)
        {
            if (!playerController) return;

            _playing = playerController.IsPlaying();

            if (_playing)
            {
                playerController.Pause();
            }
        }

        /// <summary>
        /// 指针弹起
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerUp(PointerEventData eventData)
        {
            if (!playerController) return;

            if (_playing)
            {
                playerController.Resume();
            }
        }
    }
}
