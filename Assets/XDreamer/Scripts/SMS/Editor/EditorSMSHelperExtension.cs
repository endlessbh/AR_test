using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.ComponentModel;
using XCSJ.EditorExtension.Base;
using XCSJ.EditorExtension.XGUI;
using XCSJ.EditorSMS.Inspectors;
using XCSJ.EditorSMS.States;
using XCSJ.EditorSMS.States.Motions;
using XCSJ.EditorSMS.States.MultiMedia;
using XCSJ.EditorSMS.States.Nodes;
using XCSJ.EditorXGUI;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Motions;
using XCSJ.PluginSMS.States.MultiMedia;
using XCSJ.PluginSMS.States.TimeLine;
using XCSJ.Scripts;

namespace XCSJ.EditorSMS
{
    /// <summary>
    /// 编辑器SMS辅助类扩展
    /// </summary>
    public static class EditorSMSHelperExtension
    {
        /// <summary>
        /// 初始化
        /// </summary>
        [InitializeOnLoadMethod]
        public static void Init()
        {
            GameObjectPath.onDrawGizmos += GameObjectPathInspector.OnDrawGizmos;
            PathAnimation.onDrawGizmos += PathAnimationInspector.OnDrawGizmos;

            StateInspector.onBeforeComponentsVertical += OnBeforeComponentsVertical;
            StateInspector.onAfterComponentsVertical += OnAfterComponentsVertical;

            TransitionInspector.onBeforeComponentsVertical += OnBeforeComponentsVertical;
            TransitionInspector.onAfterComponentsVertical += OnAfterComponentsVertical;

            XDreamerEvents.onSceneAnyAssetsChanged += Clear;

            SMControllerInspector.onValidateSMS += OnValidateSMS;
        }

        #region 组件集操作

        [Name("操作")]
        private enum EOperationRule
        {
            [Name("展开")]
            [XCSJ.Attributes.Icon(EIcon.Expand)]
            Expand,

            [Name("折叠")]
            [XCSJ.Attributes.Icon(EIcon.Unexpand)]
            Unexpand,

            [Name("名称")]
            [Tip("对列表内组件元按名称进行排序", "Sort the component elements in the list by name")]
            [XCSJ.Attributes.Icon(EIcon.NameAscendingOrder)]
            Name,

            [Name("逆序")]
            [Tip("对列表内组件元素进行逆序排序", "Sort the component elements in the list in reverse order")]
            [XCSJ.Attributes.Icon(EIcon.ReverseOrder)]
            Reverse,
        }

        private static void Operation(EOperationRule rule, ComponentCollection componentCollection, SerializedProperty memberProperty)
        {
            switch (rule)
            {
                case EOperationRule.Expand:
                    {
                        ExpandAllComponent(true, componentCollection, memberProperty);
                        break;
                    }
                case EOperationRule.Unexpand:
                    {
                        ExpandAllComponent(false, componentCollection, memberProperty);
                        break;
                    }
                case EOperationRule.Name:
                    {
                        SerializedObjectHelper.ArrayElementSort(memberProperty, (a, b) =>
                        {
                            var oa = a.serializedProperty.objectReferenceValue;
                            var ob = b.serializedProperty.objectReferenceValue;

                            if (!oa && !ob) return 0;
                            if (!oa) return -1;
                            if (!ob) return 1;
                            return UICommonFun.NaturalCompare(oa.name, ob.name);
                        });
                        break;
                    }
                case EOperationRule.Reverse:
                    {
                        SerializedObjectHelper.ArrayElementReverse(memberProperty);
                        break;
                    }
            }
        }

        private static void ExpandAllComponent(bool isExpanded, ComponentCollection componentCollection, SerializedProperty serializedProperty)
        {
            for (var i = 0; i < serializedProperty.arraySize; i++)
            {
                if (i >= componentCollection.components.Length) break;
                var sp = serializedProperty.GetArrayElementAtIndex(i);
                sp.isExpanded = isExpanded;
            }
        }

        #endregion

        #region 状态Inspector扩展

        private static bool hasWorkClipConflict = false;

        private static XGUIContent GetXGUIContent(string propertyName) => new XGUIContent(typeof(EditorSMSHelperExtension), propertyName);

        private static void OnBeforeComponentsVertical(ComponentCollectionInspector componentCollectionInspector, State state, SerializedProperty serializedProperty)
        {
            EditorGUI.BeginDisabledGroup(state.components.Length == 0);
            {
                UICommonFun.EnumButton<EOperationRule>((e) => Operation(e, state, serializedProperty), true, false, null, null, null, null, ENameTip.Image, GUILayout.ExpandWidth(true), GUILayout.Height(20));
            }
            EditorGUI.EndDisabledGroup();
        }

        private static void OnAfterComponentsVertical(ComponentCollectionInspector componentCollectionInspector, State state, SerializedProperty serializedProperty)
        {
            if (!state) return;
            if (Event.current.type == EventType.Layout)
            {
                hasWorkClipConflict = state.hasWorkClipConflict;
            }
            if (!hasWorkClipConflict) return;
            var bk = GUI.backgroundColor;
            try
            {
                EditorGUILayout.BeginHorizontal();
                GUI.backgroundColor = XDreamerBaseOption.weakInstance.errorColor;
                EditorGUILayout.PrefixLabel(CommonFun.TempContent("工作剪辑冲突", "当前状态所属状态组件(工作剪辑型)的数据有效性校验无效时出现的冲突问题!"));
                if (GUILayout.Button(CommonFun.TempContent("解决冲突", "点击尝试解决状态组件的工作剪辑有效性校验无效的冲突问题")))
                {
                    if (state is SubStateMachine subStateMachine)
                    {
                        Log.ErrorFormat("状态机[{0}]上的工作剪辑型状态组数据冲突(如多个状态组件时长不一致)或无效(如时长为0)!请根据对应情况自行尝试解决!", state.GetNamePath());
                    }
                    else if (!state.GetComponent<WorkClipSet>(true) && state.GetComponents<IWorkClip>().Length >= 2)
                    {
                        //添加工作剪辑集状态组件用于解决工作剪辑总时长冲突问题
                        EditorCMHelper.AddComponent<WorkClipSet>(state);
                    }
                    else
                    {
                        Log.ErrorFormat("状态[{0}]上的工作剪辑型状态组数据冲突(如多个状态组件时长不一致)或无效(如时长为0)!请根据对应情况自行尝试解决!", state.GetNamePath());
                    }
                }
            }
            catch { }
            finally
            {
                EditorGUILayout.EndHorizontal();
                GUI.backgroundColor = bk;
            }
        }

        #endregion

        #region 跳转Inspector扩展

        [Name("聚焦")]
        [Tip("聚焦到状态", "Focus to state")]
        [XCSJ.Attributes.Icon(EIcon.Target)]
        private static XGUIContent targetContent = new XGUIContent(typeof(EditorSMSHelperExtension), nameof(targetContent), true);

        [Tip("聚焦到跳转", "Focus on jump")]
        [XCSJ.Attributes.Icon(EIcon.ArrowRight_1)]
        private static XGUIContent arrowContent = new XGUIContent(typeof(EditorSMSHelperExtension), nameof(arrowContent), true);

        private static UnityEngine.Color hierarchyTransitionColor = new UnityEngine.Color(0.4745098f, 0.8156863f, 0.7450981f);

        private static UnityEngine.Color transitionColor = UnityEngine.Color.gray;

        private class WidthInfo
        {
            public ComponentCollectionInspector componentCollectionInspector;
            public float width;
            public bool expand = true;
        }

        private static List<WidthInfo> widthInfos = new List<WidthInfo>();

        private static void Clear()
        {
            widthInfos.Clear();
        }

        private static void OnBeforeComponentsVertical(ComponentCollectionInspector componentCollectionInspector, Transition transition, SerializedProperty serializedProperty)
        {
            var widthInfo = widthInfos.FirstOrNew(w => w.componentCollectionInspector == componentCollectionInspector, w =>
            {
                w.componentCollectionInspector = componentCollectionInspector;
                widthInfos.Add(w);
            });

            DrawTransition(transition, GUILayout.ExpandWidth(true), GUILayout.ExpandWidth(true), transitionColor, true, ref widthInfo.width);

            var transitions = GetUnExpandSMTransitionNodes(transition);
            if (transitions.Count > 1)
            {
                widthInfo.expand = UICommonFun.Foldout(widthInfo.expand, "跳转列表");
                if (widthInfo.expand)
                {
                    var width = GUILayout.Width(widthInfo.width);
                    float tmpWidth = 0;
                    CommonFun.BeginLayout();
                    foreach (var t in transitions)
                    {
                        DrawTransition(t.transition, width, GUILayout.ExpandWidth(true), t.isFromSubSM ? hierarchyTransitionColor : transitionColor, false, ref tmpWidth);
                    }
                    CommonFun.EndLayout();
                }
            }

            EditorGUI.BeginDisabledGroup(transition.components.Length == 0);
            {
                UICommonFun.EnumButton<EOperationRule>((e) => Operation(e, transition, serializedProperty), true, false, null, null, null, null, ENameTip.Image, GUILayout.ExpandWidth(true), GUILayout.Height(20));
            }
            EditorGUI.EndDisabledGroup();
        }

        private static void OnAfterComponentsVertical(ComponentCollectionInspector componentCollectionInspector, Transition transition, SerializedProperty serializedProperty)
        {
        }

        private static void DrawTransition(Transition transition, GUILayoutOption inStateButtonWidth, GUILayoutOption outStateButtonWidth, UnityEngine.Color arrowColor, bool needInStateButtonWidth, ref float isbw)
        {
            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button(targetContent, EditorStyles.miniButtonLeft, UICommonOption.Width24, UICommonOption.Height20))
                {
                    EditorSMSHelper.PingObject(transition.inState, true);
                }

                var inContent = new GUIContent(transition.inState.name, transition.inState.GetNamePath());
                if (GUILayout.Button(inContent, EditorStyles.miniButtonMid, inStateButtonWidth, UICommonOption.Height20))
                {
                    EditorSMSHelper.PingObject(transition.inState);
                }
                if (needInStateButtonWidth)
                {
                    var rect = GUILayoutUtility.GetLastRect();
                    if (rect.width > 2)
                    {
                        isbw = rect.width;
                    }
                }

                var orgColor = GUI.contentColor;
                GUI.contentColor = arrowColor;
                if (GUILayout.Button(CommonFun.TempContent("", transition.GetNamePath() + "\n" + arrowContent.obj.tooltip, arrowContent.obj.image), EditorStyles.miniButtonMid, UICommonOption.Width48, UICommonOption.Height20))
                {
                    EditorSMSHelper.PingObject(transition);
                }
                GUI.contentColor = orgColor;

                var outContent = new GUIContent(transition.outState.name, transition.outState.GetNamePath());
                if (GUILayout.Button(outContent, EditorStyles.miniButtonMid, outStateButtonWidth, UICommonOption.Height20))
                {
                    EditorSMSHelper.PingObject(transition.outState);
                }
                if (GUILayout.Button(targetContent, EditorStyles.miniButtonRight, UICommonOption.Width24, UICommonOption.Height20))
                {
                    EditorSMSHelper.PingObject(transition.outState, true);
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 获取状态机编辑器当前画布的折叠的状态跳转节点
        /// </summary>
        /// <param name="transition"></param>
        /// <returns></returns>
        public static List<TransitionNode> GetUnExpandSMTransitionNodes(Transition transition)
        {
            var list = new List<TransitionNode>();
            if (!transition) return list;

            if (!SMSEditor.hasInstance) return list;

            var canvas = SMSEditor.instance.selectedCanvas;
            if (canvas == null) return list;

            var node = canvas.FindNode(transition) as TransitionNode;
            if (node == null) return list;

            var inGroup = node.isFromSubSM ? transition.inState.stateCollection.stateGroup : transition.inState.stateGroup;
            var outGroup = transition.outState.stateGroup;

            if ((inGroup && !inGroup.expand) || (outGroup && !outGroup.expand))
            {
                foreach (var t in canvas.nodes.Where(n => n is TransitionNode).Cast(n => (TransitionNode)n))
                {
                    if (t.from.visualNode == node.from.visualNode && t.to.visualNode == node.to.visualNode
                        && !(t.to.visualNode is ParentStateMachineNode))
                    {
                        list.Add(t);
                    }
                }

                list.Sort((x, y) =>
                {
                    var compare = string.Compare(x.transition.inState.GetNamePath(), y.transition.inState.GetNamePath());
                    if (compare == 0)
                    {
                        compare = string.Compare(x.transition.outState.GetNamePath(), y.transition.outState.GetNamePath());
                    }
                    if (compare == 0)
                    {
                        compare = string.Compare(x.transition.GetNamePath(), y.transition.GetNamePath());
                    }
                    return compare;
                });
            }

            return list;
        }

        #endregion

        #region SMControllerInspector扩展

        private static void OnValidateSMS(SMControllerInspector inspector)
        {
            if (!RootStateMachine.instance) return;
            ValidateObject();
            ValidateConnection();
        }

        /// <summary>
        /// 验证对象的有效性
        /// </summary>
        private static void ValidateObject()
        {
            invalidTransitionComponent = 0;
            invalidTransition = 0;
            invalidStateComponent = 0;
            invalidState = 0;

            ComponentCollection.onComponentInvalid += OnComponentInvalid;
            StateCollection.onStateInvalid += OnStateInvalid;
            StateCollection.onTransitionInvalid += OnTransitionInvalid;

            foreach (var s in RootStateMachine.instance.states)
            {
                if (s) s.DataValidate();
            }

            Debug.LogFormat("经校验共计: 无效状态:{0}, 无效状态组件:{1}, 无效跳转:{2}, 无效跳转组件:{3}", invalidState, invalidStateComponent, invalidTransition, invalidTransitionComponent);


            ComponentCollection.onComponentInvalid -= OnComponentInvalid;
            StateCollection.onStateInvalid -= OnStateInvalid;
            StateCollection.onTransitionInvalid -= OnTransitionInvalid;
        }

        private static int invalidTransitionComponent = 0;
        private static int invalidTransition = 0;
        private static int invalidStateComponent = 0;
        private static int invalidState = 0;

        private static void OnComponentInvalid(ComponentCollection componentCollection, int i, PluginCommonUtils.ComponentModel.Component transitionComponent)
        {
            if (componentCollection is State state)
            {
                Debug.LogWarningFormat("状态:[{0}]发现无效状态组件:[{1}]", state.GetNamePath(), i.ToString());
                invalidStateComponent++;
            }
            else if (componentCollection is Transition transition)
            {
                Debug.LogWarningFormat("跳转:[{0}]发现无效跳转组件:[{1}]", transition.GetNamePath(), i.ToString());
                invalidTransitionComponent++;
            }
        }

        private static void OnStateInvalid(StateCollection stateCollection, int i, State state)
        {
            Debug.LogWarningFormat("状态集:[{0}]发现无效状态:[{1}]", stateCollection.GetNamePath(), i.ToString());
            invalidState++;
        }

        private static void OnTransitionInvalid(StateCollection stateCollection, int i, Transition transition)
        {
            Debug.LogWarningFormat("状态集:[{0}]发现无效跳转:[{1}]", stateCollection.GetNamePath(), i.ToString());
            invalidTransition++;
        }

        /// <summary>
        /// 验证连接的有效性
        /// </summary>
        private static void ValidateConnection()
        {
            invalidSuccessConnections = 0;
            invalidFailConnections = 0;
            ValidateConnection(RootStateMachine.instance);
            Debug.LogFormat("经校验共计: 无效连接:{0}, 修复成功:{1}, 修复失败:{2}", (invalidSuccessConnections + invalidFailConnections).ToString(), invalidSuccessConnections, invalidFailConnections);
        }

        private static int invalidSuccessConnections = 0;
        private static int invalidFailConnections = 0;

        private static void ValidateConnection(SubStateMachine subStateMachine)
        {
            if (!subStateMachine) return;
            foreach (var t in subStateMachine.transitions)
            {
                if (!t.inState.outTransitions.Contains(t))
                {
                    if (t.UpdateInState(t.inState))
                    {
                        invalidSuccessConnections++;
                        Debug.LogWarningFormat("跳转:[{0}]入状态连接无效:[{1}]-->[{2}],修复成功!", t.GetNamePath(), t.inState.GetNamePath(), t.outState.GetNamePath());
                    }
                    else
                    {
                        invalidFailConnections++;
                        Debug.LogErrorFormat("跳转:[{0}]入状态连接无效:[{1}]-->[{2}],修复失败!", t.GetNamePath(), t.inState.GetNamePath(), t.outState.GetNamePath());
                    }
                }
                if (!t.outState.inTransitions.Contains(t))
                {
                    if (t.UpdateOutState(t.outState))
                    {
                        invalidSuccessConnections++;
                        Debug.LogWarningFormat("跳转:[{0}]出状态连接无效:[{1}]-->[{2}],修复成功!", t.GetNamePath(), t.inState.GetNamePath(), t.outState.GetNamePath());
                    }
                    else
                    {
                        invalidFailConnections++;
                        Debug.LogErrorFormat("跳转:[{0}]出状态连接无效:[{1}]-->[{2}],修复失败!", t.GetNamePath(), t.inState.GetNamePath(), t.outState.GetNamePath());
                    }
                }
            }

            //遍历成员子状态机
            foreach (var s in subStateMachine.states)
            {
                ValidateConnection(s as SubStateMachine);
            }
        }

        #endregion

        #region StatePopup

        private static void DrawStateMenu(Type componentType, State state, StateCollection stateCollection, ESearchFlags searchFlags, Action<State> onMenuItemClicked, UnityEngine.Object targetObject)
        {
            var hostGameObject = CommonFun.GetHostGameObject(targetObject);
            var scenePath = hostGameObject ? hostGameObject.scene.path : "";
            var info = SMSCache.StateComponentCache3.Get(componentType, stateCollection, searchFlags, scenePath);

            if (info != null)
            {
                var selectText = info.TrimStart(state);
                var values = info.namePathOfComponentCollections;

                EditorHelper.DrawMenu(selectText, values, newSelectText =>
                {
                    onMenuItemClicked?.Invoke(SMSHelper.StringToState(info.start + newSelectText));
                });
            }
        }

        private static void StateMenu(Rect position, Type componentType, State state, StateCollection stateCollection, ESearchFlags searchFlags, Action<State> onMenuItemClicked, UnityEngine.Object targetObject)
        {
            var text = state ? state.name : "";
            if (GUI.Button(position, CommonFun.TempContent(text, text), EditorObjectHelper.MiniPopup))
            {
                DrawStateMenu(componentType, state, stateCollection, searchFlags, onMenuItemClicked, targetObject);
            }
        }

        /// <summary>
        /// 状态弹出式菜单
        /// </summary>
        /// <param name="position"></param>
        /// <param name="componentType"></param>
        /// <param name="property"></param>
        /// <param name="stateCollection"></param>
        /// <param name="searchFlags"></param>
        public static void StatePopup(Rect position, Type componentType, SerializedProperty property, StateCollection stateCollection, ESearchFlags searchFlags)
        {
            var propertyTmp = property.Copy();
            StateMenu(position, componentType, propertyTmp.objectReferenceValue as State, stateCollection, searchFlags, obj =>
            {
                propertyTmp.objectReferenceValue = obj;
                propertyTmp.serializedObject.ApplyModifiedProperties();
            }, propertyTmp.serializedObject.targetObject);
        }

        private static void DrawStateMenu(State state, StateCollection stateCollection, ESearchFlags searchFlags, Action<State> onMenuItemClicked, UnityEngine.Object targetObject)
        {
            var hostGameObject = CommonFun.GetHostGameObject(targetObject);
            var scenePath = hostGameObject ? hostGameObject.scene.path : "";
            var selectText = state.GetNamePath().TrimStart(ScriptOption.ValueDelimiterChar);
            var list = SMSCache.GetStates(stateCollection, searchFlags, scenePath);
            var values = list.Cast(s => s.GetNamePath().TrimStart(ScriptOption.ValueDelimiterChar)).ToArray();

            EditorHelper.DrawMenu(selectText, values, (newIndex, newSelectText) =>
            {
                onMenuItemClicked?.Invoke(list[newIndex]);
            });
        }

        private static void StateMenu(Rect position, State state, StateCollection stateCollection, ESearchFlags searchFlags, Action<State> onMenuItemClicked, UnityEngine.Object targetObject)
        {
            var text = state ? state.name : "";
            if (GUI.Button(position, CommonFun.TempContent(text, text), EditorObjectHelper.MiniPopup))
            {
                DrawStateMenu(state, stateCollection, searchFlags, onMenuItemClicked, targetObject);
            }
        }

        /// <summary>
        /// 状态弹出式菜单
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        /// <param name="stateCollection"></param>
        /// <param name="searchFlags"></param>
        public static void StatePopup(Rect position, SerializedProperty property, StateCollection stateCollection, ESearchFlags searchFlags)
        {
            var propertyTmp = property.Copy();
            StateMenu(position, propertyTmp.objectReferenceValue as State, stateCollection, searchFlags, obj =>
            {
                propertyTmp.objectReferenceValue = obj;
                propertyTmp.serializedObject.ApplyModifiedProperties();
            }, propertyTmp.serializedObject.targetObject);
        }

        private static void DrawStateMachineMenu(StateMachine stateMachine, Action<State> onMenuItemClicked, UnityEngine.Object targetObject)
        {
            var hostGameObject = CommonFun.GetHostGameObject(targetObject);
            var scenePath = hostGameObject ? hostGameObject.scene.path : "";
            var selectText = stateMachine.GetNamePath().TrimStart(ScriptOption.ValueDelimiterChar);
            var list = SMSCache.GetStateMachines(scenePath);
            var values = list.Cast(s => s.GetNamePath().TrimStart(ScriptOption.ValueDelimiterChar)).ToArray();

            EditorHelper.DrawMenu(selectText, values, (newIndex, newSelectText) =>
            {
                onMenuItemClicked?.Invoke(list[newIndex]);
            });
        }

        private static void StateMachineMenu(Rect position, StateMachine stateMachine, Action<State> onMenuItemClicked, UnityEngine.Object targetObject)
        {
            var text = stateMachine ? stateMachine.name : "";
            if (GUI.Button(position, CommonFun.TempContent(text, text), EditorObjectHelper.MiniPopup))
            {
                DrawStateMachineMenu(stateMachine, onMenuItemClicked, targetObject);
            }
        }

        /// <summary>
        /// 状态机弹出式菜单
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        public static void StateMachinePopup(Rect position, SerializedProperty property)
        {
            var propertyTmp = property.Copy();
            StateMachineMenu(position, propertyTmp.objectReferenceValue as StateMachine, obj =>
            {
                propertyTmp.objectReferenceValue = obj;
                propertyTmp.serializedObject.ApplyModifiedProperties();
            }, propertyTmp.serializedObject.targetObject);
        }

        private static void DrawSubStateMachineMenu(SubStateMachine stateMachine, SubStateMachine subStateMachineCollection, ESearchFlags searchFlags, Action<State> onMenuItemClicked, UnityEngine.Object targetObject)
        {
            var hostGameObject = CommonFun.GetHostGameObject(targetObject);
            var scenePath = hostGameObject ? hostGameObject.scene.path : "";
            var selectText = stateMachine.GetNamePath().TrimStart(ScriptOption.ValueDelimiterChar);
            var list = SMSCache.GetSubStateMachines(subStateMachineCollection, searchFlags, scenePath);
            var values = list.Cast(s => s.GetNamePath().TrimStart(ScriptOption.ValueDelimiterChar)).ToArray();

            EditorHelper.DrawMenu(selectText, values, (newIndex, newSelectText) =>
            {
                onMenuItemClicked?.Invoke(list[newIndex]);
            });
        }

        private static void SubStateMachineMenu(Rect position, SubStateMachine stateMachine, SubStateMachine subStateMachineCollection, ESearchFlags searchFlags, Action<State> onMenuItemClicked, UnityEngine.Object targetObject)
        {
            var text = stateMachine ? stateMachine.name : "";
            if (GUI.Button(position, CommonFun.TempContent(text, text), EditorObjectHelper.MiniPopup))
            {
                DrawSubStateMachineMenu(stateMachine, subStateMachineCollection, searchFlags, onMenuItemClicked, targetObject);
            }
        }

        /// <summary>
        /// 子状态机弹出式菜单
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        /// <param name="subStateMachineCollection"></param>
        /// <param name="searchFlags"></param>
        public static void SubStateMachinePopup(Rect position, SerializedProperty property, SubStateMachine subStateMachineCollection, ESearchFlags searchFlags)
        {
            var propertyTmp = property.Copy();
            SubStateMachineMenu(position, propertyTmp.objectReferenceValue as SubStateMachine, subStateMachineCollection, searchFlags, obj =>
            {
                propertyTmp.objectReferenceValue = obj;
                propertyTmp.serializedObject.ApplyModifiedProperties();
            }, propertyTmp.serializedObject.targetObject);
        }

        #endregion

        #region StateComponentPopup

        private static void DrawStateComponentMenu(Type componentType, StateComponent component, StateCollection stateCollection, ESearchFlags searchFlags, Action<StateComponent> onMenuItemClicked, UnityEngine.Object targetObject)
        {
            var hostGameObject = CommonFun.GetHostGameObject(targetObject);
            var scenePath = hostGameObject ? hostGameObject.scene.path : "";
            var info = SMSCache.StateComponentCache3.Get(componentType, stateCollection, searchFlags, scenePath);

            if (info != null)
            {
                var selectText = info.TrimStart(component);
                var values = info.namePathOfComponents;

                EditorHelper.DrawMenu(selectText, values, newSelectText =>
                {
                    onMenuItemClicked?.Invoke(SMSHelper.StringToStateComponent(info.start + newSelectText));
                });
            }
        }

        private static void StateComponentMenu(Rect position, Type componentType, StateComponent component, StateCollection stateCollection, ESearchFlags searchFlags, Action<StateComponent> onMenuItemClicked, UnityEngine.Object targetObject)
        {
            var text = component ? component.name : "";
            if (GUI.Button(position, CommonFun.TempContent(text, text), EditorObjectHelper.MiniPopup))
            {
                DrawStateComponentMenu(componentType, component, stateCollection, searchFlags, onMenuItemClicked, targetObject);
            }
        }

        /// <summary>
        /// 状态组件弹出式菜单
        /// </summary>
        /// <param name="position"></param>
        /// <param name="componentType"></param>
        /// <param name="property"></param>
        /// <param name="stateCollection"></param>
        /// <param name="searchFlags"></param>
        public static void StateComponentPopup(Rect position, Type componentType, SerializedProperty property, StateCollection stateCollection, ESearchFlags searchFlags)
        {
            var component = property.objectReferenceValue as StateComponent;

            if (searchFlags.HasFlag(ESearchFlags.FirstComponent | ESearchFlags.OptimizeComponent))
            {
                var state = component ? component.parent : null;

                var propertyTmp = property.Copy();
                StateMenu(position, componentType, state, stateCollection, searchFlags, obj =>
                {
                    propertyTmp.objectReferenceValue = obj ? obj.SearchComponents(searchFlags, componentType).FirstOrDefault() : null;
                    propertyTmp.serializedObject.ApplyModifiedProperties();
                }, propertyTmp.serializedObject.targetObject);
            }
            else
            {
                var propertyTmp = property.Copy();
                StateComponentMenu(position, componentType, component, stateCollection, searchFlags, obj =>
                {
                    propertyTmp.objectReferenceValue = obj;
                    propertyTmp.serializedObject.ApplyModifiedProperties();
                }, propertyTmp.serializedObject.targetObject);
            }
        }

        /// <summary>
        /// 状态组件弹出式菜单
        /// </summary>
        /// <param name="position"></param>
        /// <param name="componentType"></param>
        /// <param name="component"></param>
        /// <param name="stateCollection"></param>
        /// <param name="searchFlags"></param>
        /// <returns></returns>
        public static StateComponent StateComponentPopup(Rect position, Type componentType, StateComponent component, StateCollection stateCollection, ESearchFlags searchFlags)
        {
            if (searchFlags.HasFlag(ESearchFlags.FirstComponent | ESearchFlags.OptimizeComponent))
            {
                var state = component ? component.parent : null;
                var newValue = EditorSMSHelper.Popup(position, componentType, state, stateCollection, searchFlags);
                if (state != newValue)
                {
                    return newValue ? newValue.SearchComponents(searchFlags, componentType).FirstOrDefault() : null;
                }
            }
            else
            {
                return EditorSMSHelper.Popup(position, componentType, component, stateCollection, searchFlags);
            }
            return component;
        }

        #endregion

        #region TransitionPopup

        private static void DrawTransitionMenu(Type componentType, Transition transition, StateCollection stateCollection, ESearchFlags searchFlags, Action<Transition> onMenuItemClicked, UnityEngine.Object targetObject)
        {
            var hostGameObject = CommonFun.GetHostGameObject(targetObject);
            var scenePath = hostGameObject ? hostGameObject.scene.path : "";
            var info = SMSCache.TransitionComponentCache3.Get(componentType, stateCollection, searchFlags, scenePath);

            if (info != null)
            {
                var selectText = info.TrimStart(transition);
                var values = info.namePathOfComponentCollections;

                EditorHelper.DrawMenu(selectText, values, newSelectText =>
                {
                    onMenuItemClicked?.Invoke(SMSHelper.StringToTransition(info.start + newSelectText));
                });
            }
        }

        private static void TransitionMenu(Rect position, Type componentType, Transition transition, StateCollection stateCollection, ESearchFlags searchFlags, Action<Transition> onMenuItemClicked, UnityEngine.Object targetObject)
        {
            var text = transition ? transition.name : "";
            if (GUI.Button(position, CommonFun.TempContent(text, text), EditorObjectHelper.MiniPopup))
            {
                DrawTransitionMenu(componentType, transition, stateCollection, searchFlags, onMenuItemClicked, targetObject);
            }
        }

        /// <summary>
        /// 跳转弹出式菜单
        /// </summary>
        /// <param name="position"></param>
        /// <param name="componentType"></param>
        /// <param name="property"></param>
        /// <param name="stateCollection"></param>
        /// <param name="searchFlags"></param>
        public static void TransitionPopup(Rect position, Type componentType, SerializedProperty property, StateCollection stateCollection, ESearchFlags searchFlags)
        {
            var propertyTmp = property.Copy();
            TransitionMenu(position, componentType, propertyTmp.objectReferenceValue as Transition, stateCollection, searchFlags, obj =>
            {
                propertyTmp.objectReferenceValue = obj;
                propertyTmp.serializedObject.ApplyModifiedProperties();
            }, propertyTmp.serializedObject.targetObject);
        }

        private static void DrawTransitionMenu(Transition transition, StateCollection stateCollection, ESearchFlags searchFlags, Action<Transition> onMenuItemClicked, UnityEngine.Object targetObject)
        {
            var hostGameObject = CommonFun.GetHostGameObject(targetObject);
            var scenePath = hostGameObject ? hostGameObject.scene.path : "";
            var selectText = transition.GetNamePath().TrimStart(ScriptOption.ValueDelimiterChar);
            var list = SMSCache.GetTransitions(stateCollection, searchFlags, scenePath);
            var values = list.Cast(s => s.GetNamePath().TrimStart(ScriptOption.ValueDelimiterChar)).ToArray();

            EditorHelper.DrawMenu(selectText, values, (newIndex, newSelectText) =>
            {
                onMenuItemClicked?.Invoke(list[newIndex]);
            });
        }

        private static void TransitionMenu(Rect position, Transition transition, StateCollection stateCollection, ESearchFlags searchFlags, Action<Transition> onMenuItemClicked, UnityEngine.Object targetObject)
        {
            var text = transition ? transition.name : "";
            if (GUI.Button(position, CommonFun.TempContent(text, text), EditorObjectHelper.MiniPopup))
            {
                DrawTransitionMenu(transition, stateCollection, searchFlags, onMenuItemClicked, targetObject);
            }
        }

        /// <summary>
        /// 跳转弹出式菜单
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        /// <param name="stateCollection"></param>
        /// <param name="searchFlags"></param>
        public static void TransitionPopup(Rect position, SerializedProperty property, StateCollection stateCollection, ESearchFlags searchFlags)
        {
            var propertyTmp = property.Copy();
            TransitionMenu(position, propertyTmp.objectReferenceValue as Transition, stateCollection, searchFlags, obj =>
            {
                propertyTmp.objectReferenceValue = obj;
                propertyTmp.serializedObject.ApplyModifiedProperties();
            }, propertyTmp.serializedObject.targetObject);
        }

        #endregion

        #region TransitionComponentPopup

        private static void DrawTransitionComponentMenu(Type componentType, TransitionComponent component, StateCollection stateCollection, ESearchFlags searchFlags, Action<TransitionComponent> onMenuItemClicked, UnityEngine.Object targetObject)
        {
            var hostGameObject = CommonFun.GetHostGameObject(targetObject);
            var scenePath = hostGameObject ? hostGameObject.scene.path : "";
            var info = SMSCache.TransitionComponentCache3.Get(componentType, stateCollection, searchFlags, scenePath);

            if (info != null)
            {
                var selectText = info.TrimStart(component);
                var values = info.namePathOfComponents;

                EditorHelper.DrawMenu(selectText, values, newSelectText =>
                {
                    onMenuItemClicked?.Invoke(SMSHelper.StringToTransitionComponent(info.start + newSelectText));
                });
            }
        }

        private static void TransitionComponentMenu(Rect position, Type componentType, TransitionComponent component, StateCollection stateCollection, ESearchFlags searchFlags, Action<TransitionComponent> onMenuItemClicked, UnityEngine.Object targetObject)
        {
            var text = component ? component.name : "";
            if (GUI.Button(position, CommonFun.TempContent(text, text), EditorObjectHelper.MiniPopup))
            {
                DrawTransitionComponentMenu(componentType, component, stateCollection, searchFlags, onMenuItemClicked, targetObject);
            }
        }

        /// <summary>
        /// 跳转组件弹出式菜单
        /// </summary>
        /// <param name="position"></param>
        /// <param name="componentType"></param>
        /// <param name="property"></param>
        /// <param name="stateCollection"></param>
        /// <param name="searchFlags"></param>
        public static void TransitionComponentPopup(Rect position, Type componentType, SerializedProperty property, StateCollection stateCollection, ESearchFlags searchFlags)
        {
            var component = property.objectReferenceValue as TransitionComponent;

            if (searchFlags.HasFlag(ESearchFlags.FirstComponent | ESearchFlags.OptimizeComponent))
            {
                var transition = component ? component.parent : null;

                var propertyTmp = property.Copy();
                TransitionMenu(position, componentType, transition, stateCollection, searchFlags, obj =>
                {
                    propertyTmp.objectReferenceValue = obj ? obj.SearchComponents(searchFlags, componentType).FirstOrDefault() : null;
                    propertyTmp.serializedObject.ApplyModifiedProperties();
                }, propertyTmp.serializedObject.targetObject);
            }
            else
            {
                var propertyTmp = property.Copy();
                TransitionComponentMenu(position, componentType, component, stateCollection, searchFlags, obj =>
                {
                    propertyTmp.objectReferenceValue = obj;
                    propertyTmp.serializedObject.ApplyModifiedProperties();
                }, propertyTmp.serializedObject.targetObject);
            }
        }

        /// <summary>
        /// 跳转组件弹出式菜单
        /// </summary>
        /// <param name="position"></param>
        /// <param name="componentType"></param>
        /// <param name="component"></param>
        /// <param name="stateCollection"></param>
        /// <param name="searchFlags"></param>
        /// <returns></returns>
        public static TransitionComponent TransitionComponentPopup(Rect position, Type componentType, TransitionComponent component, StateCollection stateCollection, ESearchFlags searchFlags)
        {
            if (searchFlags.HasFlag(ESearchFlags.FirstComponent | ESearchFlags.OptimizeComponent))
            {
                var state = component ? component.parent : null;
                var newValue = EditorSMSHelper.Popup(position, componentType, state, stateCollection, searchFlags);
                if (state != newValue)
                {
                    return newValue ? newValue.SearchComponents(searchFlags, componentType).FirstOrDefault() : null;
                }
            }
            else
            {
                return EditorSMSHelper.Popup(position, componentType, component, stateCollection, searchFlags);
            }
            return component;
        }


        #endregion

        #region StateGroupPopup

        private static void DrawStateGroupMenu(Type componentType, StateGroup stateGroup, StateCollection stateCollection, ESearchFlags searchFlags, Action<StateGroup> onMenuItemClicked, UnityEngine.Object targetObject)
        {
            var hostGameObject = CommonFun.GetHostGameObject(targetObject);
            var scenePath = hostGameObject ? hostGameObject.scene.path : "";
            var info = SMSCache.StateGroupComponentCache3.Get(componentType, stateCollection, searchFlags, scenePath);

            if (info != null)
            {
                var selectText = info.TrimStart(stateGroup);
                var values = info.namePathOfComponentCollections;

                EditorHelper.DrawMenu(selectText, values, newSelectText =>
                {
                    onMenuItemClicked?.Invoke(SMSHelper.StringToStateGroup(info.start + newSelectText));
                });
            }
        }

        private static void StateGroupMenu(Rect position, Type componentType, StateGroup stateGroup, StateCollection stateCollection, ESearchFlags searchFlags, Action<StateGroup> onMenuItemClicked, UnityEngine.Object targetObject)
        {
            var text = stateGroup ? stateGroup.name : "";
            if (GUI.Button(position, CommonFun.TempContent(text, text), EditorObjectHelper.MiniPopup))
            {
                DrawStateGroupMenu(componentType, stateGroup, stateCollection, searchFlags, onMenuItemClicked, targetObject);
            }
        }

        /// <summary>
        /// 状态组弹出式菜单
        /// </summary>
        /// <param name="position"></param>
        /// <param name="componentType"></param>
        /// <param name="property"></param>
        /// <param name="stateCollection"></param>
        /// <param name="searchFlags"></param>
        public static void StateGroupPopup(Rect position, Type componentType, SerializedProperty property, StateCollection stateCollection, ESearchFlags searchFlags)
        {
            var propertyTmp = property.Copy();
            StateGroupMenu(position, componentType, propertyTmp.objectReferenceValue as StateGroup, stateCollection, searchFlags, obj =>
            {
                propertyTmp.objectReferenceValue = obj;
                propertyTmp.serializedObject.ApplyModifiedProperties();
            }, propertyTmp.serializedObject.targetObject);
        }

        private static void DrawStateGroupMenu(StateGroup stateGroup, StateCollection stateCollection, ESearchFlags searchFlags, Action<StateGroup> onMenuItemClicked, UnityEngine.Object targetObject)
        {
            var hostGameObject = CommonFun.GetHostGameObject(targetObject);
            var scenePath = hostGameObject ? hostGameObject.scene.path : "";
            var selectText = stateGroup.GetNamePath().TrimStart(ScriptOption.ValueDelimiterChar);
            var list = SMSCache.GetStateGroups(stateCollection, searchFlags, scenePath);
            var values = list.Cast(s => s.GetNamePath().TrimStart(ScriptOption.ValueDelimiterChar)).ToArray();

            EditorHelper.DrawMenu(selectText, values, (newIndex, newSelectText) =>
            {
                onMenuItemClicked?.Invoke(list[newIndex]);
            });
        }

        private static void StateGroupMenu(Rect position, StateGroup stateGroup, StateCollection stateCollection, ESearchFlags searchFlags, Action<StateGroup> onMenuItemClicked, UnityEngine.Object targetObject)
        {
            var text = stateGroup ? stateGroup.name : "";
            if (GUI.Button(position, CommonFun.TempContent(text, text), EditorObjectHelper.MiniPopup))
            {
                DrawStateGroupMenu(stateGroup, stateCollection, searchFlags, onMenuItemClicked, targetObject);
            }
        }

        /// <summary>
        /// 状态组弹出式菜单
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        /// <param name="stateCollection"></param>
        /// <param name="searchFlags"></param>
        public static void StateGroupPopup(Rect position, SerializedProperty property, StateCollection stateCollection, ESearchFlags searchFlags)
        {
            var propertyTmp = property.Copy();
            StateGroupMenu(position, propertyTmp.objectReferenceValue as StateGroup, stateCollection, searchFlags, obj =>
            {
                propertyTmp.objectReferenceValue = obj;
                propertyTmp.serializedObject.ApplyModifiedProperties();
            }, propertyTmp.serializedObject.targetObject);
        }

        #endregion

        #region StateGroupComponentPopup

        private static void DrawStateGroupComponentMenu(Type componentType, StateGroupComponent component, StateCollection stateCollection, ESearchFlags searchFlags, Action<StateGroupComponent> onMenuItemClicked, UnityEngine.Object targetObject)
        {
            var hostGameObject = CommonFun.GetHostGameObject(targetObject);
            var scenePath = hostGameObject ? hostGameObject.scene.path : "";
            var info = SMSCache.StateGroupComponentCache3.Get(componentType, stateCollection, searchFlags, scenePath);

            if (info != null)
            {
                var selectText = info.TrimStart(component);
                var values = info.namePathOfComponents;

                EditorHelper.DrawMenu(selectText, values, newSelectText =>
                {
                    onMenuItemClicked?.Invoke(SMSHelper.StringToStateGroupComponent(info.start + newSelectText));
                });
            }
        }

        private static void StateGroupComponentMenu(Rect position, Type componentType, StateGroupComponent component, StateCollection stateCollection, ESearchFlags searchFlags, Action<StateGroupComponent> onMenuItemClicked, UnityEngine.Object targetObject)
        {
            var text = component ? component.name : "";
            if (GUI.Button(position, CommonFun.TempContent(text, text), EditorObjectHelper.MiniPopup))
            {
                DrawStateGroupComponentMenu(componentType, component, stateCollection, searchFlags, onMenuItemClicked, targetObject);
            }
        }

        /// <summary>
        /// 状态组组件弹出式菜单
        /// </summary>
        /// <param name="position"></param>
        /// <param name="componentType"></param>
        /// <param name="property"></param>
        /// <param name="stateCollection"></param>
        /// <param name="searchFlags"></param>
        public static void StateGroupComponentPopup(Rect position, Type componentType, SerializedProperty property, StateCollection stateCollection, ESearchFlags searchFlags)
        {
            var component = property.objectReferenceValue as StateGroupComponent;

            if (searchFlags.HasFlag(ESearchFlags.FirstComponent | ESearchFlags.OptimizeComponent))
            {
                var transition = component ? component.parent : null;

                var propertyTmp = property.Copy();
                StateGroupMenu(position, componentType, transition, stateCollection, searchFlags, obj =>
                {
                    propertyTmp.objectReferenceValue = obj ? obj.SearchComponents(searchFlags, componentType).FirstOrDefault() : null;
                    propertyTmp.serializedObject.ApplyModifiedProperties();
                }, propertyTmp.serializedObject.targetObject);
            }
            else
            {
                var propertyTmp = property.Copy();
                StateGroupComponentMenu(position, componentType, component, stateCollection, searchFlags, obj =>
                {
                    propertyTmp.objectReferenceValue = obj;
                    propertyTmp.serializedObject.ApplyModifiedProperties();
                }, propertyTmp.serializedObject.targetObject);
            }
        }

        /// <summary>
        /// 状态组组件弹出式菜单
        /// </summary>
        /// <param name="position"></param>
        /// <param name="componentType"></param>
        /// <param name="component"></param>
        /// <param name="stateCollection"></param>
        /// <param name="searchFlags"></param>
        /// <returns></returns>
        public static StateGroupComponent StateGroupComponentPopup(Rect position, Type componentType, StateGroupComponent component, StateCollection stateCollection, ESearchFlags searchFlags)
        {
            if (searchFlags.HasFlag(ESearchFlags.FirstComponent | ESearchFlags.OptimizeComponent))
            {
                var state = component ? component.parent : null;
                var newValue = EditorSMSHelper.Popup(position, componentType, state, stateCollection, searchFlags);
                if (state != newValue)
                {
                    return newValue ? newValue.SearchComponents(searchFlags, componentType).FirstOrDefault() : null;
                }
            }
            else
            {
                return EditorSMSHelper.Popup(position, componentType, component, stateCollection, searchFlags);
            }
            return component;
        }


        #endregion
    }
}
