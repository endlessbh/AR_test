using XCSJ.Attributes;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.Interfaces;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.States.TimeLine;

namespace XCSJ.PluginSMS.States.Show.UI
{
    /// <summary>
    /// 步骤组信息界面
    /// </summary>
    [RequireManager(typeof(SMSManager))]
    public abstract class GUIStepGroupInfo : InteractProvider
    {
        [Name("自动查找步骤组")]
        public bool autoFindStepGroup = true;

        [Name("步骤组")]
        [Tip("为空时,自动查找正在运行的播放器对应的播放内容的步骤组", "When empty, automatically find the step group of playing content corresponding to the running player")]
        [StateComponentPopup(typeof(StepGroup), stateCollectionType = EStateCollectionType.Root)]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [HideInSuperInspector(nameof(autoFindStepGroup), EValidityCheckType.Equal, true)]
        public StepGroup stepGroup = null;

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            StepGroup.onStepChanged += OnStepChanged;

            TimeLinePlayer.onPlayerStateChanged += OnPlayerStateChanged;
            TimeLinePlayer.onPlayerPercentChanged += OnPlayerPercentChanged;
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            StepGroup.onStepChanged -= OnStepChanged;

            TimeLinePlayer.onPlayerStateChanged -= OnPlayerStateChanged;
            TimeLinePlayer.onPlayerPercentChanged -= OnPlayerPercentChanged;
        }

        protected virtual void Start()
        {
            OnStepChanged(stepGroup);
        }

        private void OnStepChanged(StepGroup stepGroup, Step oldStep, Step newStep)
        {
            // 自动适配当前播放器
            if (autoFindStepGroup)
            {
                OnStepChanged(StepGroupHelper.GetRoot(stepGroup) as StepGroup);
            }
            else if (this.stepGroup == stepGroup)
            {
                OnStepChanged(stepGroup);
            }
        }

        public void OnPlayerStateChanged(TimeLinePlayer timeLinePlayer, EPlayerState oldState, EPlayerState newState)
        {
            if (!autoFindStepGroup && !timeLinePlayer && !timeLinePlayer.playContent)
            {
                return;
            }

            switch (newState)
            {
                case EPlayerState.Play:
                    {
                        SetPlayer(timeLinePlayer);
                        OnStepChanged(stepGroupOfCurrentPlayer);
                        break;
                    }
                case EPlayerState.Finished:
                    {
                        currentPlayer = null;
                        stepGroupOfCurrentPlayer = null;
                        break;
                    }
            }
        }

        public void OnPlayerPercentChanged(TimeLinePlayer player, double percent)
        {
            if (!currentPlayer)
            {
                SetPlayer(player);
            }
            else if (currentPlayer != player)
            {
                return;
            }

            OnStepChanged(stepGroupOfCurrentPlayer);
        }

        private void SetPlayer(TimeLinePlayer player)
        {
            currentPlayer = player;
            stepGroupOfCurrentPlayer = currentPlayer.playContent.GetComponent<StepGroup>();
        }

        private TimeLinePlayer currentPlayer = null;
        private StepGroup stepGroupOfCurrentPlayer = null;

        protected abstract void OnStepChanged(StepGroup group);
    }
}
