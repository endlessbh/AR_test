using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Base;
using XCSJ.Languages;

#if XDREAMER_VUFORIA
using Vuforia;
#endif

namespace XCSJ.PluginVuforia.States
{
    /// <summary>
    /// Vuforia应用事件
    /// </summary>
    [Name(Title, nameof(VuforiaApplicationEvent))]
    [RequireManager(typeof(VuforiaManager))]
    [Owner(typeof(VuforiaManager))]
    public class VuforiaApplicationEvent : Trigger<VuforiaApplicationEvent>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "Vuforia应用事件";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [Name(Title, nameof(VuforiaApplicationEvent))]
#if UNITY_EDITOR && XDREAMER_EDITION_DEVELOPER
        [StateLib(VuforiaHelper.Title, typeof(VuforiaManager))]
        [StateComponentMenu(VuforiaHelper.Title + "/" + Title, typeof(VuforiaManager))]
#endif
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

#if XDREAMER_VUFORIA

        /// <summary>
        /// 事件类型
        /// </summary>
        public enum EEventType
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None,

            /// <summary>
            /// 当Vuforia初始化之前
            /// </summary>
            [Name("当Vuforia初始化之前")]
            OnBeforeVuforiaInitialized,

            /// <summary>
            /// 当Vuforia初始化
            /// </summary>
            [Name("当Vuforia初始化")]
            OnVuforiaInitialized,

            /// <summary>
            /// 当Vuforia启动
            /// </summary>
            [Name("当Vuforia启动")]
            OnVuforiaStarted,

            /// <summary>
            /// 当Vuforia暂停
            /// </summary>
            [Name("当Vuforia暂停")]
            OnVuforiaPaused,

            /// <summary>
            /// 当Vuforia停止
            /// </summary>
            [Name("当Vuforia停止")]
            OnVuforiaStopped,

            /// <summary>
            /// 当Vuforia取消初始化
            /// </summary>
            [Name("当Vuforia取消初始化")]
            OnVuforiaDeinitialized,
        }

        /// <summary>
        /// 事件类型
        /// </summary>
        [Name("事件类型")]
        [EnumPopup]
        public EEventType _eventType = EEventType.OnVuforiaStarted;

        /// <summary>
        /// Vuforia初始化错误
        /// </summary>
        [Name("Vuforia初始化错误")]
        [HideInSuperInspector(nameof(_eventType), EValidityCheckType.NotEqual, EEventType.OnVuforiaInitialized)]
        public VuforiaInitError _vuforiaInitError = VuforiaInitError.NONE;

        /// <summary>
        /// 暂停
        /// </summary>
        [Name("暂停")]
        [HideInSuperInspector(nameof(_eventType), EValidityCheckType.NotEqual, EEventType.OnVuforiaPaused)]
        public bool _paused = true;

        /// <summary>
        /// 当进入
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnEntry(StateData stateData)
        {
            base.OnEntry(stateData);
            VuforiaApplication.Instance.OnBeforeVuforiaInitialized += OnBeforeVuforiaInitialized;
            VuforiaApplication.Instance.OnVuforiaInitialized += OnVuforiaInitialized;
            VuforiaApplication.Instance.OnVuforiaStarted += OnVuforiaStarted;
            VuforiaApplication.Instance.OnVuforiaPaused += OnVuforiaPaused;
            VuforiaApplication.Instance.OnVuforiaStopped += OnVuforiaStopped;
            VuforiaApplication.Instance.OnVuforiaDeinitialized += OnVuforiaDeinitialized;
        }

        /// <summary>
        /// 当退出
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnExit(StateData stateData)
        {
            base.OnExit(stateData);
            VuforiaApplication.Instance.OnBeforeVuforiaInitialized -= OnBeforeVuforiaInitialized;
            VuforiaApplication.Instance.OnVuforiaInitialized -= OnVuforiaInitialized;
            VuforiaApplication.Instance.OnVuforiaStarted -= OnVuforiaStarted;
            VuforiaApplication.Instance.OnVuforiaPaused -= OnVuforiaPaused;
            VuforiaApplication.Instance.OnVuforiaStopped -= OnVuforiaStopped;
            VuforiaApplication.Instance.OnVuforiaDeinitialized -= OnVuforiaDeinitialized;
        }

        private void OnBeforeVuforiaInitialized()
        {
            if (finished) return;
            if (_eventType != EEventType.OnBeforeVuforiaInitialized) return;
            finished = true;
        }

        private void OnVuforiaInitialized(VuforiaInitError vuforiaInitError)
        {
            if (finished) return;
            if (_eventType != EEventType.OnVuforiaInitialized) return;
            if (vuforiaInitError != _vuforiaInitError) return;
            finished = true;
        }

        private void OnVuforiaStarted()
        {
            if (finished) return;
            if (_eventType != EEventType.OnVuforiaStarted) return;
            finished = true;
        }

        private void OnVuforiaPaused(bool paused)
        {
            if (finished) return;
            if (_eventType != EEventType.OnVuforiaPaused) return;
            if (paused != _paused) return;
            finished = true;
        }

        private void OnVuforiaStopped()
        {
            if (finished) return;
            if (_eventType != EEventType.OnVuforiaStopped) return;
            finished = true;
        }
        private void OnVuforiaDeinitialized()
        {
            if (finished) return;
            if (_eventType != EEventType.OnVuforiaDeinitialized) return;
            finished = true;
        }

#endif
    }
}
