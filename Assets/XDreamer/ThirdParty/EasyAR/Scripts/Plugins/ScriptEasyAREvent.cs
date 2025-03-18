using UnityEngine;
using System.Collections;
using System;
using XCSJ.PluginCommonUtils;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils.CNScripts;
#if XDREAMER_EASYAR_4_0_1 || XDREAMER_EASYAR_3_0_1
using easyar;
#elif XDREAMER_EASYAR_2_3_0
using EasyAR;
#endif

namespace XCSJ.PluginEasyAR
{
    /// <summary>
    /// EasyAR脚本事件类型
    /// </summary>
    [Name("EasyAR脚本事件类型")]
    public enum EEasyARScriptEventType
    {
        [Name("启动时 执行")]
        Start = 0,

        /// <summary>
        /// 选择时
        /// </summary>
        [Name("目标识别时 执行")]
        TargetFound,

        [Name("目标丢失时 执行")]
        TargetLost,

        [Name("目标加载时 执行")]
        TargetLoad,

        [Name("目标卸载时 执行")]
        TargetUnload,

        [Name("文本消息时 执行")]
        [Tip("文本消息，比如二维码扫描结果的文本字符串", "Text message, such as text string of QR code scanning result")]
        TextMessage,
    }

    /// <summary>
    /// EasyAR脚本事件函数
    /// </summary>
    [Name("EasyAR脚本事件函数")]
    [Serializable]
    public class EasyARScriptEventFunction : EnumFunction<EEasyARScriptEventType> { }

    /// <summary>
    /// EasyAR脚本事件函数集合
    /// </summary>
    [Name("EasyAR脚本事件函数集合")]
    [Serializable]
    public class EasyARScriptEventFunctionCollection : EnumFunctionCollection<EEasyARScriptEventType, EasyARScriptEventFunction> { }

    /// <summary>
    /// 脚本EasyAR事件
    /// </summary>
    [Serializable]
    [Name("脚本EasyAR事件")]
    [DisallowMultipleComponent]
    [AddComponentMenu(Product.Name + "/EasyAR/Script EasyAR Event")]
    [RequireManager(typeof(EasyARManager))]
    public class ScriptEasyAREvent : BaseScriptEvent<EEasyARScriptEventType, EasyARScriptEventFunction, EasyARScriptEventFunctionCollection>
    {
        [Name("EasyAR组件")]
        [Tip("EasyAR的根节点核心组件", "The root node core component of EasyAR")]
#if XDREAMER_EASYAR_2_3_0
        public EasyARBehaviour easyAR;
#else
        public Component easyAR;
#endif

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
#if XDREAMER_EASYAR_2_3_0
            if (!easyAR) easyAR = EasyARManager.InitEasyAR(easyAR);
            if (easyAR)
            {
                easyAR.Initialize();

                foreach (var behaviour in ARBuilder.Instance.ARCameraBehaviours)
                {
                    behaviour.TargetFound += OnTargetFound;
                    behaviour.TargetLost += OnTargetLost;
                    behaviour.TextMessage += OnTextMessage;
                }
                foreach (var behaviour in ARBuilder.Instance.ImageTrackerBehaviours)
                {
                    behaviour.TargetLoad += OnTargetLoad;
                    behaviour.TargetUnload += OnTargetUnload;
                }
            }
            else
            {
                Log.Error("未找到EasyARBehaviour组件");
            }
#endif
        }

        /// <summary>
        /// 启动
        /// </summary>
        protected override void Start()
        {
            base.Start();
            ExecuteScriptEvent(EEasyARScriptEventType.Start);
        }

#if XDREAMER_EASYAR_2_3_0

        protected void OnTargetFound(ARCameraBaseBehaviour arcameraBehaviour, TargetAbstractBehaviour targetBehaviour, Target target)
        {
            ExecuteScriptEvent(EScriptEasyAREventType.TargetFound, target.Id.ToString());
        }

        protected void OnTargetLost(ARCameraBaseBehaviour arcameraBehaviour, TargetAbstractBehaviour targetBehaviour, Target target)
        {
            ExecuteScriptEvent(EScriptEasyAREventType.TargetFound, target.Id.ToString());
        }

        protected void OnTargetLoad(ImageTrackerBaseBehaviour trackerBehaviour, ImageTargetBaseBehaviour targetBehaviour, Target target, bool status)
        {
            ExecuteScriptEvent(EScriptEasyAREventType.TargetFound, target.Id.ToString());
        }

        protected void OnTargetUnload(ImageTrackerBaseBehaviour trackerBehaviour, ImageTargetBaseBehaviour targetBehaviour, Target target, bool status)
        {
            ExecuteScriptEvent(EScriptEasyAREventType.TargetFound, target.Id.ToString());
        }

        protected void OnTextMessage(ARCameraBaseBehaviour arcameraBehaviour, string text)
        {
            ExecuteScriptEvent(EScriptEasyAREventType.TextMessage, text);
        }
#else
        protected void OnTargetFound(MonoBehaviour arcameraBehaviour, MonoBehaviour targetBehaviour, object target) { }

        protected void OnTargetLost(MonoBehaviour arcameraBehaviour, MonoBehaviour targetBehaviour, object target) { }

        protected void OnTargetLoad(MonoBehaviour trackerBehaviour, MonoBehaviour targetBehaviour, object target, bool status) { }

        protected void OnTargetUnload(MonoBehaviour trackerBehaviour, MonoBehaviour targetBehaviour, object target, bool status) { }

        protected void OnTextMessage(MonoBehaviour arcameraBehaviour, string text) { }
#endif
    }
}