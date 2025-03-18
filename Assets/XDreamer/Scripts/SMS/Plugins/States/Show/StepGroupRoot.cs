using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;
using XCSJ.PluginSMS.States.TimeLine;

namespace XCSJ.PluginSMS.States.Show
{
    /// <summary>
    /// 根步骤组
    /// </summary>
    public abstract class StepGroupRoot : StepGroup
    {
        [Name("播放内容")]
        [Tip("关联播放内容，让计划随着时间轴播放器播放移动播放步骤", "Associate the playback content and let the plan move the playback step with the timeline player")]
        //[ValidityCheck(EValidityCheckType.NotNull)]
        [StateComponentPopup(typeof(TimeLinePlayContent), stateCollectionType = EStateCollectionType.Root)]
        public TimeLinePlayContent timeLinePlayContent;

        public override ETreeNodeType nodeType => ETreeNodeType.Root;

        public override void OnCreated()
        {
            base.OnCreated();

            if (!timeLinePlayContent) timeLinePlayContent = GetComponent<TimeLinePlayContent>();
        }

        public override bool Init(StateData data)
        {
            BindPlayContent();
            return base.Init(data);
        }

        public override void OnEntry(StateData data)
        {
            // 状态激活后，不在与播放内容关联，防止出现错误播放
            UnBindPlayContent();
            
            base.OnEntry(data);
        }

        public override void OnExit(StateData data)
        {
            BindPlayContent();
            
            base.OnExit(data);
        }

        public override bool DataValidity() => !string.IsNullOrEmpty(description);

        public override string ToFriendlyString() => "";

        private event Action<State[]> onStateChanged;

        private void BindPlayContent()
        {
            if (timeLinePlayContent)
            {
                // 播放器 => 步骤组
                timeLinePlayContent.onPlayContentElementChanged += OnPlayContentElementChanged;
                timeLinePlayContent.onPlay += OnPlay;
                timeLinePlayContent.onStop += OnStop;

                // 步骤组 => 播放内容
                this.onStateChanged += timeLinePlayContent.PlayContentElements;
            }
        }

        private void UnBindPlayContent()
        {
            if (timeLinePlayContent)
            {
                timeLinePlayContent.onPlayContentElementChanged -= OnPlayContentElementChanged;
                timeLinePlayContent.onPlay -= OnPlay;
                timeLinePlayContent.onStop -= OnStop;
                this.onStateChanged -= timeLinePlayContent.PlayContentElements;
            }
        }

        /// <summary>
        /// 播放器 => 步骤组
        /// </summary>
        public void OnPlayContentElementChanged(TimeLinePlayContent timeLinePlayContent, List<State> lastElements, List<State> newElements, double percent)
        {
            playContentChanged = true;
            foreach (var s in newElements)
            {
                SetCurrent(s);
            }
            playContentChanged = false;
        }

        private bool playContentChanged = false;

        public override void Select()
        {
            base.Select();

            if (!playContentChanged)
            {
                onStateChanged?.Invoke(GetSelectedStates().ToArray());
            }
        }

        public void OnPlay(TimeLinePlayContent timeLinePlayContent) { }

        public void OnStop(TimeLinePlayContent timeLinePlayContent) { UnSelect(); }

        public override void GotoState(State state)
        {
            if (timeLinePlayContent)
            {
                timeLinePlayContent.SetPercent(state);
            }
        }
    }
}
