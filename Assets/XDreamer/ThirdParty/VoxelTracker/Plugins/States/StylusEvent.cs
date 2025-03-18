using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Base;
using XCSJ.PluginCommonUtils;
using XCSJ.Extension.Base.Dataflows.Base;

#if XDREAMER_VOXELTRACKER
using VoxelStationUtil;
#endif

namespace XCSJ.PluginVoxelTracker.States
{
    /// <summary>
    /// 交互笔事件:用于监听并捕获VoxelTracker输入设备交互笔的事件
    /// </summary>
    [ComponentMenu(VoxelTrackerHelper.Title + "/" + Title, typeof(VoxelTrackerManager))]
    [Name(Title, nameof(StylusEvent))]
    [Tip("用于监听并捕获VoxelTracker输入设备交互笔的事件", "It is used to listen and capture the events of VoxelTracker input device interaction pen")]
    [Owner(typeof(VoxelTrackerManager))]
    public class StylusEvent : Trigger<StylusEvent>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "交互笔事件";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [Name(Title, nameof(StylusEvent))]
#if UNITY_EDITOR
        [StateLib(VoxelTrackerHelper.Title, typeof(VoxelTrackerManager))]
        [StateComponentMenu(VoxelTrackerHelper.Title + "/" + Title, typeof(VoxelTrackerManager))]
#endif
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

#if XDREAMER_VOXELTRACKER

        /// <summary>
        /// 交互笔
        /// </summary>
        [Name("交互笔")]
        [ComponentPopup]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public StylusOne _stylusOne;

        /// <summary>
        /// 交互笔
        /// </summary>
        public StylusOne stylusOne => this.XGetComponentInGlobal(ref _stylusOne, true);

        private StylusOne stylusOneOnEntry;

#endif

        /// <summary>
        /// 交互笔事件
        /// </summary>
        [Name("交互笔事件")]
        [EnumPopup]
        public EStylusEvent _stylusEvent = EStylusEvent.stylusButtonPressed;

        private EStylusEvent stylusEventOnEntry;

        /// <summary>
        /// 交互笔事件：枚举字段名与委托事件名保持一致；
        /// </summary>
        [Name("交互笔事件")]
        public enum EStylusEvent
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None,

            /// <summary>
            /// 交互笔按钮按压
            /// </summary>
            [Name("交互笔按钮按压")]
            stylusButtonPressed,

            /// <summary>
            /// 交互笔按钮
            /// </summary>
            [Name("交互笔按钮")]
            stylusButton,

            /// <summary>
            /// 交互笔按钮释放
            /// </summary>
            [Name("交互笔按钮释放")]
            stylusButtonReleased,

            /// <summary>
            /// 当交互笔悬停UI开始
            /// </summary>
            [Name("当交互笔悬停UI开始")]
            onStylusHoverUIBegin,

            /// <summary>
            /// 当交互笔悬停UI结束
            /// </summary>
            [Name("当交互笔悬停UI结束")]
            onStylusHoverUIEnd,

            /// <summary>
            /// 当交互笔按钮1按压UI
            /// </summary>
            [Name("当交互笔按钮1按压UI")]
            onStylusButtonOnePressedUI,

            /// <summary>
            /// 当交互笔按钮2按压UI
            /// </summary>
            [Name("当交互笔按钮2按压UI")]
            onStylusButtonTwoPressedUI,

            /// <summary>
            /// 当交互笔按钮3按压UI
            /// </summary>
            [Name("当交互笔按钮3按压UI")]
            onStylusButtonThreePressedUI,

            /// <summary>
            /// 当交互笔悬停对象开始
            /// </summary>
            [Name("当交互笔悬停对象开始")]
            onStylusHoverObjectBegin,

            /// <summary>
            /// 当交互笔悬停对象结束
            /// </summary>
            [Name("当交互笔悬停对象结束")]
            onStylusHoverObjectEnd,

            /// <summary>
            /// 当交互笔抓取对象开始
            /// </summary>
            [Name("当交互笔抓取对象开始")]
            onStylusGrabObjectBegin,

            /// <summary>
            /// 当交互笔抓取对象更新
            /// </summary>
            [Name("当交互笔抓取对象更新")]
            onStylusGrabObjectUpdate,

            /// <summary>
            /// 当交互笔抓取对象结束
            /// </summary>
            [Name("当交互笔抓取对象结束")]
            onStylusGrabObjectEnd,

            /// <summary>
            /// 当交互笔按钮1拖拽UI
            /// </summary>
            [Name("当交互笔按钮1拖拽UI")]
            onStylusButtonOneDragUI,

            /// <summary>
            /// 当交互笔按钮1释放UI
            /// </summary>
            [Name("当交互笔按钮1释放UI")]
            onStylusButtonOneReleasedUI,
        }

#if XDREAMER_VOXELTRACKER

        /// <summary>
        /// 当进入
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnEntry(StateData stateData)
        {
            base.OnEntry(stateData);

            stylusOneOnEntry = stylusOne;
            stylusEventOnEntry = _stylusEvent;

            if (stylusOneOnEntry)
            {
                switch (stylusEventOnEntry)
                {
                    case EStylusEvent.stylusButtonPressed: stylusOneOnEntry.stylusButtonPressed += OnStylusButtonPress; break;
                    case EStylusEvent.stylusButton: stylusOneOnEntry.stylusButton += OnStylusButton; break;
                    case EStylusEvent.stylusButtonReleased: stylusOneOnEntry.stylusButtonReleased += OnStylusButtonReleased; break;

                    case EStylusEvent.onStylusHoverUIBegin: stylusOneOnEntry.onStylusHoverUIBegin += OnStylusHoverUIBegin; break;
                    case EStylusEvent.onStylusHoverUIEnd: stylusOneOnEntry.onStylusHoverUIEnd += OnStylusHoverUIEnd; break;

                    case EStylusEvent.onStylusButtonOnePressedUI: stylusOneOnEntry.onStylusButtonOnePressedUI += OnStylusButtonOnePressedUI; break;
                    case EStylusEvent.onStylusButtonTwoPressedUI: stylusOneOnEntry.onStylusButtonTwoPressedUI += OnStylusButtonTwoPressedUI; break;
                    case EStylusEvent.onStylusButtonThreePressedUI: stylusOneOnEntry.onStylusButtonThreePressedUI += OnStylusButtonThreePressedUI; break;

                    case EStylusEvent.onStylusHoverObjectBegin: stylusOneOnEntry.onStylusHoverObjectBegin += OnStylusHoverObjectBegin; break;
                    case EStylusEvent.onStylusHoverObjectEnd: stylusOneOnEntry.onStylusHoverObjectEnd += OnStylusHoverObjectEnd; break;

                    case EStylusEvent.onStylusGrabObjectBegin: stylusOneOnEntry.onStylusGrabObjectBegin += OnStylusGrabObjectBegin; break;
                    case EStylusEvent.onStylusGrabObjectUpdate: stylusOneOnEntry.onStylusGrabObjectUpdate += OnStylusGrabObjectUpdate; break;
                    case EStylusEvent.onStylusGrabObjectEnd: stylusOneOnEntry.onStylusGrabObjectEnd += OnStylusGrabObjectEnd; break;

                    case EStylusEvent.onStylusButtonOneDragUI: stylusOneOnEntry.onStylusButtonOneDragUI += OnStylusButtonOneDragUI; break;
                    case EStylusEvent.onStylusButtonOneReleasedUI: stylusOneOnEntry.onStylusButtonOneReleasedUI += OnStylusButtonOneReleasedUI; break;
                }
            }
        }

        /// <summary>
        /// 当退出
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnExit(StateData stateData)
        {
            base.OnExit(stateData);

            if (stylusOneOnEntry)
            {
                switch (stylusEventOnEntry)
                {
                    case EStylusEvent.stylusButtonPressed: stylusOneOnEntry.stylusButtonPressed -= OnStylusButtonPress; break;
                    case EStylusEvent.stylusButton: stylusOneOnEntry.stylusButton -= OnStylusButton; break;
                    case EStylusEvent.stylusButtonReleased: stylusOneOnEntry.stylusButtonReleased -= OnStylusButtonReleased; break;

                    case EStylusEvent.onStylusHoverUIBegin: stylusOneOnEntry.onStylusHoverUIBegin -= OnStylusHoverUIBegin; break;
                    case EStylusEvent.onStylusHoverUIEnd: stylusOneOnEntry.onStylusHoverUIEnd -= OnStylusHoverUIEnd; break;

                    case EStylusEvent.onStylusButtonOnePressedUI: stylusOneOnEntry.onStylusButtonOnePressedUI -= OnStylusButtonOnePressedUI; break;
                    case EStylusEvent.onStylusButtonTwoPressedUI: stylusOneOnEntry.onStylusButtonTwoPressedUI -= OnStylusButtonTwoPressedUI; break;
                    case EStylusEvent.onStylusButtonThreePressedUI: stylusOneOnEntry.onStylusButtonThreePressedUI -= OnStylusButtonThreePressedUI; break;

                    case EStylusEvent.onStylusHoverObjectBegin: stylusOneOnEntry.onStylusHoverObjectBegin -= OnStylusHoverObjectBegin; break;
                    case EStylusEvent.onStylusHoverObjectEnd: stylusOneOnEntry.onStylusHoverObjectEnd -= OnStylusHoverObjectEnd; break;

                    case EStylusEvent.onStylusGrabObjectBegin: stylusOneOnEntry.onStylusGrabObjectBegin -= OnStylusGrabObjectBegin; break;
                    case EStylusEvent.onStylusGrabObjectUpdate: stylusOneOnEntry.onStylusGrabObjectUpdate -= OnStylusGrabObjectUpdate; break;
                    case EStylusEvent.onStylusGrabObjectEnd: stylusOneOnEntry.onStylusGrabObjectEnd -= OnStylusGrabObjectEnd; break;

                    case EStylusEvent.onStylusButtonOneDragUI: stylusOneOnEntry.onStylusButtonOneDragUI -= OnStylusButtonOneDragUI; break;
                    case EStylusEvent.onStylusButtonOneReleasedUI: stylusOneOnEntry.onStylusButtonOneReleasedUI -= OnStylusButtonOneReleasedUI; break;
                }
            }
        }

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            if (stylusOne) { }
        }

        /// <summary>
        /// 按钮ID枚举
        /// </summary>
        public enum EButtonID
        {
            /// <summary>
            /// 任意
            /// </summary>
            [Name("任意")]
            Any = -1,

            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None = 0,

            /// <summary>
            /// 按钮1：中间键
            /// </summary>
            [Name("按钮1")]
            [Tip("中间键", "Intermediate key")]
            One,

            /// <summary>
            /// 按钮2:左键
            /// </summary>
            [Name("按钮2")]
            [Tip("左键", "Left key")]
            Two,

            /// <summary>
            /// 按钮3:右键
            /// </summary>
            [Name("按钮3")]
            [Tip("右键", "Left key")]
            Three,

            /// <summary>
            /// 增加
            /// </summary>
            [Name("增加")]
            V_Add,

            /// <summary>
            /// 减少
            /// </summary>
            [Name("减少")]
            V_Sub

        }

        /// <summary>
        /// 交互笔按钮ID变量名：产生交互笔按钮事件时存储交互笔按钮ID的变量
        /// </summary>
        [Name("交互笔按钮ID变量名")]
        [Tip("产生交互笔按钮事件时存储交互笔按钮ID的变量", "The variable that stores the interactive pen button ID when the interactive pen button event is generated")]
        [GlobalVariable]
        public string _variableNameOfStylusButtonID;

        /// <summary>
        /// 交互笔按钮按压时按钮ID
        /// </summary>
        [Name("交互笔按钮按压时按钮ID")]
        [EnumPopup]
        [HideInSuperInspector(nameof(_stylusEvent), EValidityCheckType.NotEqual, EStylusEvent.stylusButtonPressed)]
        public EButtonID _buttonIDOnStylusButtonPress = EButtonID.One;

        /// <summary>
        /// 对应<see cref="EStylusEvent.stylusButtonPressed"/>
        /// </summary>
        /// <param name="info"></param>
        private void OnStylusButtonPress(StylusEventInfo info)
        {
            if (finished) return;
            switch (_buttonIDOnStylusButtonPress)
            {
                case EButtonID.Any:
                    {
                        finished = true;
                        break;
                    }
                case EButtonID.None: break;
                default:
                    {
                        finished = _buttonIDOnStylusButtonPress == (EButtonID)info.ButtonID;
                        break;
                    }
            }
            if (finished)
            {
                ScriptManager.TrySetGlobalVariableValue(_variableNameOfStylusButtonID, info.ButtonID.ToString());
            }
        }

        /// <summary>
        /// 交互笔按钮时按钮ID
        /// </summary>
        [Name("交互笔按钮时按钮ID")]
        [EnumPopup]
        [HideInSuperInspector(nameof(_stylusEvent), EValidityCheckType.NotEqual, EStylusEvent.stylusButton)]
        public EButtonID _buttonIDOnStylusButton = EButtonID.One;

        /// <summary>
        /// 对应<see cref="EStylusEvent.stylusButton"/>
        /// </summary>
        /// <param name="info"></param>
        private void OnStylusButton(StylusEventInfo info)
        {
            if (finished) return;
            switch (_buttonIDOnStylusButton)
            {
                case EButtonID.Any:
                    {
                        finished = true;
                        break;
                    }
                case EButtonID.None: break;
                default:
                    {
                        finished = _buttonIDOnStylusButton == (EButtonID)info.ButtonID;
                        break;
                    }
            }
            if (finished)
            {
                ScriptManager.TrySetGlobalVariableValue(_variableNameOfStylusButtonID, info.ButtonID.ToString());
            }
        }

        /// <summary>
        /// 交互笔按钮释放时按钮ID
        /// </summary>
        [Name("交互笔按钮释放时按钮ID")]
        [EnumPopup]
        [HideInSuperInspector(nameof(_stylusEvent), EValidityCheckType.NotEqual, EStylusEvent.stylusButtonReleased)]
        public EButtonID _buttonIDOnStylusButtonReleased = EButtonID.One;

        /// <summary>
        /// 对应<see cref="EStylusEvent.stylusButtonReleased"/>
        /// </summary>
        /// <param name="info"></param>
        private void OnStylusButtonReleased(StylusEventInfo info)
        {
            if (finished) return;
            switch (_buttonIDOnStylusButtonReleased)
            {
                case EButtonID.Any:
                    {
                        finished = true;
                        break;
                    }
                case EButtonID.None: break;
                default:
                    {
                        finished = _buttonIDOnStylusButtonReleased == (EButtonID)info.ButtonID;
                        break;
                    }
            }
            if (finished)
            {
                ScriptManager.TrySetGlobalVariableValue(_variableNameOfStylusButtonID, info.ButtonID.ToString());
            }
        }

        /// <summary>
        /// 动作事件数据
        /// </summary>
        [Serializable]
        public class ActionEventData
        {
            /// <summary>
            /// 动作事件规则
            /// </summary>
            [Name("动作事件规则")]
            [EnumPopup]
            public EActionEventRule _actionEventRule = EActionEventRule.CheckActionObject;

            /// <summary>
            /// 动作对象：交互笔正在产生交互动作的游戏对象
            /// </summary>
            [Name("动作对象")]
            [Tip("交互笔正在产生交互动作的游戏对象", "Interactive pen is a game object that is generating interactive action")]
            [HideInSuperInspector(nameof(_actionEventRule), EValidityCheckType.NotEqual, EActionEventRule.CheckActionObject)]
            public GameObjectPropertyValue _actionObject = new GameObjectPropertyValue();

            /// <summary>
            /// 动作对象变量
            /// </summary>
            [Name("动作对象变量")]
            [GlobalVariable]
            public string _actionObjectVariable = "";

            /// <summary>
            /// 检测规则
            /// </summary>
            /// <returns></returns>
            public bool CheckRule(ActionEventInfo info)
            {
                var result = false;
                switch (_actionEventRule)
                {
                    case EActionEventRule.Any:
                        {
                            result = true;
                            break;
                        }
                    case EActionEventRule.CheckActionObject:
                        {
                            result = _actionObject.GetValue() == info.actionObject;
                            break;
                        }
                }
                if (result)
                {
                    ScriptManager.TrySetGlobalVariableValue(_actionObjectVariable, CommonFun.GameObjectToString(info.actionObject));
                }
                return result;
            }
        }

        /// <summary>
        /// 动作事件规则
        /// </summary>
        public enum EActionEventRule
        {
            /// <summary>
            /// 任意：只要事件发生，就标识事件成立；
            /// </summary>
            [Name("任意")]
            [Tip("只要事件发生，就标识事件成立；", "As long as the event occurs, it indicates that the event is established;")]
            Any = -1,

            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None,

            /// <summary>
            /// 检查动作对象:交互笔正在产生交互动作的游戏对象是期望的动作对象时，才标识事件成立；
            /// </summary>
            [Name("检查动作对象")]
            CheckActionObject,
        }

        /// <summary>
        /// 当交互笔悬停UI开始时动作事件数据
        /// </summary>
        [Name("当交互笔悬停UI开始时动作事件数据")]
        [HideInSuperInspector(nameof(_stylusEvent), EValidityCheckType.NotEqual, EStylusEvent.onStylusHoverUIBegin)]
        public ActionEventData _actionEventDataOnStylusHoverUIBegin = new ActionEventData();

        /// <summary>
        /// 对应<see cref="EStylusEvent.onStylusHoverUIBegin"/>
        /// </summary>
        /// <param name="info"></param>
        private void OnStylusHoverUIBegin(ActionEventInfo info)
        {
            if (finished) return;
            finished = _actionEventDataOnStylusHoverUIBegin.CheckRule(info);
        }

        /// <summary>
        /// 当交互笔悬停UI结束时动作事件数据
        /// </summary>
        [Name("当交互笔悬停UI结束时动作事件数据")]
        [HideInSuperInspector(nameof(_stylusEvent), EValidityCheckType.NotEqual, EStylusEvent.onStylusHoverUIEnd)]
        public ActionEventData _actionEventDataOnStylusHoverUIEnd = new ActionEventData();

        /// <summary>
        /// 对应<see cref="EStylusEvent.onStylusHoverUIEnd"/>
        /// </summary>
        /// <param name="info"></param>
        private void OnStylusHoverUIEnd(ActionEventInfo info)
        {
            if (finished) return;
            finished = _actionEventDataOnStylusHoverUIEnd.CheckRule(info);
        }

        /// <summary>
        /// 当交互笔按钮1按压UI时动作事件数据
        /// </summary>
        [Name("当交互笔按钮1按压UI时动作事件数据")]
        [HideInSuperInspector(nameof(_stylusEvent), EValidityCheckType.NotEqual, EStylusEvent.onStylusButtonOnePressedUI)]
        public ActionEventData _actionEventDataOnStylusButtonOnePressedUI = new ActionEventData();

        /// <summary>
        /// 对应<see cref="EStylusEvent.onStylusButtonOnePressedUI"/>
        /// </summary>
        /// <param name="info"></param>
        private void OnStylusButtonOnePressedUI(ActionEventInfo info)
        {
            if (finished) return;
            finished = _actionEventDataOnStylusButtonOnePressedUI.CheckRule(info);
            if (finished)
            {
                ScriptManager.TrySetGlobalVariableValue(_variableNameOfStylusButtonID, ButtonID.One.ToString());
            }
        }

        /// <summary>
        /// 当交互笔按钮2按压UI时动作事件数据
        /// </summary>
        [Name("当交互笔按钮2按压UI时动作事件数据")]
        [HideInSuperInspector(nameof(_stylusEvent), EValidityCheckType.NotEqual, EStylusEvent.onStylusButtonTwoPressedUI)]
        public ActionEventData _actionEventDataOnStylusButtonTwoPressedUI = new ActionEventData();

        /// <summary>
        /// 对应<see cref="EStylusEvent.onStylusButtonTwoPressedUI"/>
        /// </summary>
        /// <param name="info"></param>
        private void OnStylusButtonTwoPressedUI(ActionEventInfo info)
        {
            if (finished) return;
            finished = _actionEventDataOnStylusButtonTwoPressedUI.CheckRule(info);
            if (finished)
            {
                ScriptManager.TrySetGlobalVariableValue(_variableNameOfStylusButtonID, ButtonID.Two.ToString());
            }
        }

        /// <summary>
        /// 当交互笔按钮3按压UI时动作事件数据
        /// </summary>
        [Name("当交互笔按钮3按压UI时动作事件数据")]
        [HideInSuperInspector(nameof(_stylusEvent), EValidityCheckType.NotEqual, EStylusEvent.onStylusButtonThreePressedUI)]
        public ActionEventData _actionEventDataOnStylusButtonThreePressedUI = new ActionEventData();

        /// <summary>
        /// 对应<see cref="EStylusEvent.onStylusButtonThreePressedUI"/>
        /// </summary>
        /// <param name="info"></param>
        private void OnStylusButtonThreePressedUI(ActionEventInfo info)
        {
            if (finished) return;
            finished = _actionEventDataOnStylusButtonThreePressedUI.CheckRule(info);
            if (finished)
            {
                ScriptManager.TrySetGlobalVariableValue(_variableNameOfStylusButtonID, ButtonID.Three.ToString());
            }
        }

        /// <summary>
        /// 当交互笔悬停对象开始时动作事件数据
        /// </summary>
        [Name("当交互笔悬停对象开始时动作事件数据")]
        [HideInSuperInspector(nameof(_stylusEvent), EValidityCheckType.NotEqual, EStylusEvent.onStylusHoverObjectBegin)]
        public ActionEventData _actionEventDataOnStylusHoverObjectBegin = new ActionEventData();

        /// <summary>
        /// 对应<see cref="EStylusEvent.onStylusHoverObjectBegin"/>
        /// </summary>
        /// <param name="info"></param>
        private void OnStylusHoverObjectBegin(ActionEventInfo info)
        {
            if (finished) return;
            finished = _actionEventDataOnStylusHoverObjectBegin.CheckRule(info);
        }

        /// <summary>
        /// 当交互笔悬停对象结束时动作事件数据
        /// </summary>
        [Name("当交互笔悬停对象结束时动作事件数据")]
        [HideInSuperInspector(nameof(_stylusEvent), EValidityCheckType.NotEqual, EStylusEvent.onStylusHoverObjectEnd)]
        public ActionEventData _actionEventDataOnStylusHoverObjectEnd = new ActionEventData();

        /// <summary>
        /// 对应<see cref="EStylusEvent.onStylusHoverObjectEnd"/>
        /// </summary>
        /// <param name="info"></param>
        private void OnStylusHoverObjectEnd(ActionEventInfo info)
        {
            if (finished) return;
            finished = _actionEventDataOnStylusHoverObjectEnd.CheckRule(info);
        }

        /// <summary>
        /// 当交互笔抓取对象开始时动作事件数据
        /// </summary>
        [Name("当交互笔抓取对象开始时动作事件数据")]
        [HideInSuperInspector(nameof(_stylusEvent), EValidityCheckType.NotEqual, EStylusEvent.onStylusGrabObjectBegin)]
        public ActionEventData _actionEventDataOnStylusGrabObjectBegin = new ActionEventData();

        /// <summary>
        /// 对应<see cref="EStylusEvent.onStylusGrabObjectBegin"/>
        /// </summary>
        /// <param name="info"></param>
        private void OnStylusGrabObjectBegin(ActionEventInfo info)
        {
            if (finished) return;
            finished = _actionEventDataOnStylusGrabObjectBegin.CheckRule(info);
        }

        /// <summary>
        /// 当交互笔抓取对象更新时动作事件数据
        /// </summary>
        [Name("当交互笔抓取对象更新时动作事件数据")]
        [HideInSuperInspector(nameof(_stylusEvent), EValidityCheckType.NotEqual, EStylusEvent.onStylusGrabObjectUpdate)]
        public ActionEventData _actionEventDataOnStylusGrabObjectUpdate = new ActionEventData();

        /// <summary>
        /// 对应<see cref="EStylusEvent.onStylusGrabObjectUpdate"/>
        /// </summary>
        /// <param name="info"></param>
        private void OnStylusGrabObjectUpdate(ActionEventInfo info)
        {
            if (finished) return;
            finished = _actionEventDataOnStylusGrabObjectUpdate.CheckRule(info);
        }

        /// <summary>
        /// 当交互笔抓取对象结束时动作事件数据
        /// </summary>
        [Name("当交互笔抓取对象结束时动作事件数据")]
        [HideInSuperInspector(nameof(_stylusEvent), EValidityCheckType.NotEqual, EStylusEvent.onStylusGrabObjectEnd)]
        public ActionEventData _actionEventDataOnStylusGrabObjectEnd = new ActionEventData();

        /// <summary>
        /// 对应<see cref="EStylusEvent.onStylusGrabObjectEnd"/>
        /// </summary>
        /// <param name="info"></param>
        private void OnStylusGrabObjectEnd(ActionEventInfo info)
        {
            if (finished) return;
            finished = _actionEventDataOnStylusGrabObjectEnd.CheckRule(info);
        }

        /// <summary>
        /// 当交互笔按钮1拖拽UI时动作事件数据
        /// </summary>
        [Name("当交互笔按钮1拖拽UI时动作事件数据")]
        [HideInSuperInspector(nameof(_stylusEvent), EValidityCheckType.NotEqual, EStylusEvent.onStylusButtonOneDragUI)]
        public ActionEventData _actionEventDataOnStylusButtonOneDragUI = new ActionEventData();

        /// <summary>
        /// 对应<see cref="EStylusEvent.onStylusButtonOneDragUI"/>
        /// </summary>
        /// <param name="info"></param>
        private void OnStylusButtonOneDragUI(ActionEventInfo info)
        {
            if (finished) return;
            finished = _actionEventDataOnStylusButtonOneDragUI.CheckRule(info);
            if (finished)
            {
                ScriptManager.TrySetGlobalVariableValue(_variableNameOfStylusButtonID, ButtonID.One.ToString());
            }
        }

        /// <summary>
        /// 当交互笔按钮1释放UI时动作事件数据
        /// </summary>
        [Name("当交互笔按钮1释放UI时动作事件数据")]
        [HideInSuperInspector(nameof(_stylusEvent), EValidityCheckType.NotEqual, EStylusEvent.onStylusButtonOneReleasedUI)]
        public ActionEventData _actionEventDataOnStylusButtonOneReleasedUI = new ActionEventData();

        /// <summary>
        /// 对应<see cref="EStylusEvent.onStylusButtonOneReleasedUI"/>
        /// </summary>
        /// <param name="info"></param>
        private void OnStylusButtonOneReleasedUI(ActionEventInfo info)
        {
            if (finished) return;
            finished = _actionEventDataOnStylusButtonOneReleasedUI.CheckRule(info);
            if (finished)
            {
                ScriptManager.TrySetGlobalVariableValue(_variableNameOfStylusButtonID, ButtonID.One.ToString());
            }
        }

#endif
    }
}
