using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Events;
using XCSJ.Extension.Base.Inputs;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.Interfaces;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;

namespace XCSJ.PluginTimelines.Tools
{
    /// <summary>
    /// 输入等待播放器
    /// </summary>
    [Name("输入等待播放器", nameof(InputWaitPlayer))]
    [Tool(TimelineManager.Title, rootType = typeof(TimelineManager), purposes = new string[] { TimelineManager.Title })]
    [XCSJ.Attributes.Icon(EIcon.Play)]
    [RequireManager(typeof(TimelineManager))]
    [Owner(typeof(TimelineManager))]
    public class InputWaitPlayer : Interactor
    {
        /// <summary>
        /// 可播放内容
        /// </summary>
        [Name("可播放内容")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public PlayableContent _playableContent;

        /// <summary>
        /// 启用后等待时间:组件启用后，等待指定时间，才执行对应的自动逻辑；单位：秒；
        /// </summary>
        [Name("启用后等待时间")]
        [Tip("组件启用后，等待指定时间，才执行对应的自动逻辑；单位：秒；", "After the component is enabled, wait for the specified time before executing the corresponding automatic logic; Unit: second;")]
        [Range(0, 600)]
        public float _waitTimeOfEnable = 3;

        /// <summary>
        /// 输入等待时间:无输入操作后，等待指定时间，才执行对应的自动逻辑；单位：秒；
        /// </summary>
        [Name("输入等待时间")]
        [Tip("无输入操作后，等待指定时间，才执行对应的自动逻辑；单位：秒；")]
        [Range(0, 600)]
        [EndGroup(true)]
        public float _waitTimeOfInput = 3;

        private float waitedTime = 0;

        /// <summary>
        /// 标识能否更新
        /// </summary>
        public bool canUpdate => wait == EWait.Update;

        /// <summary>
        /// 等待标记量
        /// </summary>
        public EWait wait { get; private set; } = EWait.None;

        /// <summary>
        /// 等待枚举
        /// </summary>
        [Name("等待")]
        public enum EWait
        {
            /// <summary>
            /// 无
            /// </summary>
            None,

            /// <summary>
            /// 等待启用
            /// </summary>
            WaitForEnable,

            /// <summary>
            /// 等待输入
            /// </summary>
            WaitForInput,

            /// <summary>
            /// 更新
            /// </summary>
            Update,
        }

        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            _playableContent = GetComponent<PlayableContent>();
        }

        /// <summary>
        /// 启用时
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            if (!_playableContent)
            {
                enabled = false;
                return;
            }

            wait = EWait.WaitForEnable;
            waitedTime = 0;
        }

        /// <summary>
        /// 更新
        /// </summary>
        protected virtual void Update()
        {
            //优先检测输入事件
            if (Input.anyKey)
            {
                if (_playableContent && _playableContent.player.playerState == EPlayerState.Playing)
                {
                    _playableContent.Pause();
                }
                wait = EWait.WaitForInput;
                waitedTime = 0;
            }

            switch (wait)
            {
                case EWait.WaitForEnable:
                    {
                        waitedTime += Time.deltaTime;
                        if (waitedTime >= _waitTimeOfEnable)
                        {
                            wait = EWait.Update;
                        }
                        break;
                    }
                case EWait.WaitForInput:
                    {
                        waitedTime += Time.deltaTime;
                        if (waitedTime >= _waitTimeOfInput)
                        {
                            wait = EWait.Update;
                        }
                        break;
                    }
                case EWait.Update:
                    {
                        if (_playableContent && _playableContent.player.playerState != EPlayerState.Playing)
                        {
                            _playableContent.ResumeOrPlay();
                        }
                        break;
                    }
            }
        }
    }
}
