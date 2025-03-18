using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension.EditorWindows;
using XCSJ.EditorSMS.States;
using XCSJ.EditorSMS.Toolkit.PathWindowCore;
using XCSJ.EditorSMS.Utils;
using XCSJ.EditorTools;
using XCSJ.EditorTools.Windows;
using XCSJ.Helper;
using XCSJ.Languages;
using XCSJ.Maths;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.Base;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginTools.ExplodedViews;
using XCSJ.PluginTools.ExplodedViews.States;
using XCSJ.Tools;

namespace XCSJ.EditorSMS.Toolkit
{
    /// <summary>
    /// 路径编辑器(状态机专用)
    /// </summary>
    [Name(Title)]
    [XCSJ.Attributes.Icon(EIcon.Path)]
    [XDreamerEditorWindow(TrHelper.StateMachine_EN, index = 3)]
    public class PathWindow : XEditorWindowWithScrollView<PathWindow>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "路径编辑器(状态机专用)";

        /// <summary>
        /// 打开窗口
        /// </summary>
        [MenuItem(XDreamerMenu.EditorWindow + Title, false)]
        public static void OpenWindow() => OpenAndFocus();

        /// <summary>
        /// 路径数据管理器
        /// </summary>
        public PathInfoManager pathInfoManager = new PathInfoManager();

        private bool dataChanging = false;

        /// <summary>
        /// 记录状态
        /// </summary>
        protected bool recording
        {
            get
            {
                return _recording;
            }
            set
            {
                _recording = value;
                StopSimulate();
                StopExplodeSimulate();
                if (SMSEditor.hasInstance)
                {
                    if (_recording)
                    {
                        SetPathObjsSelected();
                        orgLockCanvas = SMSEditor.instance.lockCanvas;
                        SMSEditor.instance.lockCanvas = _recording;
                    }
                    else
                    {
                        SMSEditor.instance.lockCanvas = orgLockCanvas;
                    }
                }
                pathInfoManager.OnRecordChange(_recording);
            }
        }

        private bool _recording = false;

        private void UpdateDataBeforeOnGUI()
        {
            if (Event.current.type == EventType.Layout)
            {
                if (useCurrentSM && lockSM == false)
                {
                    LoadCurrentSM();
                }
                pathInfoManager.Update();
            }

            if (!recording && !dataChanging)
            {
                pathInfoManager.ImportData();
            }
        }

        private void UpdateDataAfterGUI()
        {
            if (dataChanging)
            {
                dataChanging = false;

                pathInfoManager.ExportData();
                UICommonFun.MarkSceneDirty();
            }
        }

        private void LoadCurrentSM()
        {
            if (!SMSEditor.hasInstance) return;

            var smsEditor = SMSEditor.instance;
            if (!smsEditor || smsEditor.selectedCanvas == null) return;

            var currentSM = (smsEditor.selectedCanvas as SubStateMachineNodeCanvas)?.subStateMachine;
            if (pathInfoManager.subSM && pathInfoManager.subSM == currentSM) return;

            pathInfoManager.subSM = currentSM;
        }

        private void SetSelectedPath(PathInfo pathInfo)
        {
            if (pathInfoManager.selectedPathInfo != pathInfo)
            {
                selectedKeyPointIndex = -1;
            }
            // 设置路径被选中
            pathInfoManager.selectedPathInfo = pathInfo;

            // 设置状态机编辑器节点被选中
            if (SMSEditor.instance != null
                && SMSEditor.instance.selectedCanvas != null
                && SMSEditor.instance.selectedCanvas.data == pathInfoManager.subSM
                && pathInfo != null)
            {
                var subSMCanvas = SMSEditor.instance.selectedCanvas as SubStateMachineNodeCanvas;
                if (subSMCanvas != null)
                {
                    StateComponent sc = pathInfo.pathObj as StateComponent;
                    if (sc)
                    {
                        subSMCanvas.FindNode(sc.parent as State).Select();
                        SMSEditor.instance.Repaint();
                    }
                }
            }
        }

        /// <summary>
        /// 弹出路径类型菜单
        /// </summary>
        /// <param name="menuName"></param>
        /// <param name="onClick"></param>
        private void PopupPathTypeMenu(string menuName, Action<Type> onClick)
        {
            MenuHelper.DrawMenu(menuName, m =>
            {
                pathInfoManager.pathTypes.ForEach(pt =>
                {
                    m.AddMenuItem(CommonFun.Name(pt), obj =>
                    {
                        onClick?.Invoke(obj as Type);
                    }, pt);
                });
            });
        }

        #region 窗口的重载函数

        #region 界面元素

        protected XGUIStyle recordingInSceneViewStyle = new XGUIStyle(nameof(EditorStyles.label), style =>
        {
            style.fontSize = 20;
            style.normal.textColor = UnityEngine.Color.red;
        });

        private XGUIStyle keyPointLabelStyle = new XGUIStyle(nameof(GUI.Label), style =>
        {
            style.fontSize = PathWindowOption.weakInstance.labelFontSize;
            style.normal.textColor = PathWindowOption.weakInstance.labelColor;
        });

        #endregion

        /// <summary>
        /// 选择关键点，用于在场景视图中拖拽路径关键点
        /// </summary>
        private int selectedKeyPointIndex = -1;

        protected override void OnEnable()
        {
            base.OnEnable();

            recording = false;
            pathInfoManager.OnEnable();

            TypeHelper.FindTypeInAppWithInterface(typeof(IPathLayout)).ForEach(t => pathLayouts.Add(TypeHelper.CreateInstance(t) as IPathLayout));

            OnOptionModified(ToolEditorWindowOption.weakInstance);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            StopSimulate();
            StopExplodeSimulate();
        }

        public void OnLostFocus()
        {
            StopSimulate();
            StopExplodeSimulate();
        }

        protected void OnSelectionChange()
        {
            SetPathObjsSelected();
            UpdateGameObjects();
        }

        public override void OnSceneOpened(Scene scene, OpenSceneMode mode)
        {
            OnSceneChange();
        }

        public override void OnNewSceneCreated(Scene scene, NewSceneSetup setup, NewSceneMode mode)
        {
            OnSceneChange();
        }

        protected void OnSceneChange()
        {
            lockSM = false;
            _recording = false;
            pathInfoManager.Reset();
        }

        public override void OnSceneGUI(SceneView sceneView)
        {
            OnKeyboardEvent();

            if (recording)
            {
                Handles.BeginGUI();
                GUI.Label(new Rect(10, 10, 200, 30), "路径编辑器 录制中...", recordingInSceneViewStyle);
                Handles.EndGUI();
            }
            else
            {
                OnSceneGUIForExplodeView(sceneView);
            }

            var pathOption = PathWindowOption.weakInstance;
            Vector3 scale = Vector3.one * pathOption.virtualObjectSize;
            UnityEngine.Color orgColor = Handles.color;

            keyPointLabelStyle.obj.fontSize = pathOption.labelFontSize;
            keyPointLabelStyle.obj.normal.textColor = pathOption.labelColor;

            try
            {
                EditorGUI.BeginChangeCheck();

                float keyPointSizeValue = pathOption.keyPointSizeValue;
                for (int i = 0; i < pathInfoManager.pathCount; ++i)
                {
                    var pathInfo = pathInfoManager.pathInfos[i];

                    if (!pathInfo.displayPath) continue;

                    bool selected = (pathInfoManager.selectedPathInfo == pathInfo);

                    // 绘制状态名
                    Handles.Label(pathInfo.virtualPointPosition + pathOption.namePositionOffset, pathInfo.name, keyPointLabelStyle);

                    // 绘制路径
                    var pathData = pathInfo.transformDatas;
                    int pointCount = pathData.Count;
                    if (pointCount == 0) continue;

                    // 增加一个点，用于路径连接到游戏对象上
                    Vector3[] path = new Vector3[pointCount + 1];
                    Handles.color = pathOption.pathKeyPointBoxColor;
                    for (int j = 0; j < pointCount; ++j)
                    {
                        var td = pathData[j];
                        path[j] = td.keyPoint;

                        // 路径点编号和方体
                        Handles.Label(td.keyPoint, (j + 1).ToString(), keyPointLabelStyle);

                        if (EditorApplication.isPlaying) continue;

                        if (Handles.Button(td.keyPoint, Quaternion.identity, keyPointSizeValue, keyPointSizeValue, Handles.SphereHandleCap))
                        {
                            selectedKeyPointIndex = j;
                        }

                        // 绘制拖拽轴
                        if (selected && selectedKeyPointIndex == j)
                        {
                            td.rotation = Handles.RotationHandle(td.rotation, td.keyPoint);
                            td.keyPoint = Handles.PositionHandle(td.keyPoint, td.rotation);
                        }
                    }

                    // 绘制路径
                    Handles.color = pathOption.pathLineColor;
                    path[pointCount] = recording ? pathInfo.virtualPointPosition : path[pointCount - 1];
                    Handles.DrawPolyLine(path);
                }

                if (EditorGUI.EndChangeCheck())
                {
                    dataChanging = true;
                }
            }
            catch { }
            finally
            {
                Handles.color = orgColor;
            }
        }

        public override bool timedRepaint => true;

        public override void OnTimedRepaint()
        {
            base.OnTimedRepaint();
            SceneView.RepaintAll();
        }

        protected override void OnGUI()
        {
            if (!SMSEditor.hasInstance)
            {
                UICommonFun.Notification(position.size, "请启动【状态机】编辑器！");
                return;
            }

            UpdateDataBeforeOnGUI();
            {
                EditorGUI.BeginDisabledGroup(EditorApplication.isPlaying);
                {
                    DrawTopTools();

                    DrawSM();
                }
                EditorGUI.EndDisabledGroup();

                base.OnGUI();
            }
            UpdateDataAfterGUI();
        }

        public override void OnGUIWithScrollView()
        {
            EditorGUI.BeginDisabledGroup(EditorApplication.isPlaying || !pathInfoManager.smValid);
            {
                DrawCreatePath();

                DrawPathList();

                DrawPathDetailInfo();
            }
            EditorGUI.EndDisabledGroup();
        }

        private void SetPathObjsSelected()
        {
            // 选中游戏对象属于路径动画，则修改路径选择集
            pathInfoManager.SelectedPathInfo(Selection.activeGameObject);

            if (recording)
            {
                if (pathInfoManager.selectedPathInfo == null)
                {
                    pathInfoManager.SetDefaultSelectedPathInfo();
                }
                else if (pathInfoManager.selectedPathInfo.pathObj != null)
                {
                    // 强制修改游戏对象选择集为当前选择路径对应游戏对象
                    EditorApplicationExtension.DelayCall(0.01f, pathInfoManager.selectedPathInfo.pathObj.transforms,
                   o =>
                   {
                       var objs = o as List<Transform>;
                       if (objs != null && objs.Count > 0)
                       {
                           Selection.activeObject = objs[0];
                       }
                   });
                }
            }
        }

        private GUILayoutOption[] contentButtonSizeOption;
        private GUILayoutOption[] toolButtonSizeOption;

        protected override void OnOptionModified(Option option)
        {
            base.OnOptionModified(option);
            if (option is ToolEditorWindowOption toolkit)
            {
                toolButtonSizeOption = toolkit.toolButtonSizeOption;
                contentButtonSizeOption = toolkit.contentButtonSizeOption;
            }
        }

        #endregion

        #region 顶部工具栏

        private bool orgLockCanvas = false;

        #region 界面元素

        [Name("开始录制")]
        [Tip("开始录制")]
        [XCSJ.Attributes.Icon(index = 33952)]
        public XGUIContent BeginRecord { get; } = GetXGUIContent(nameof(BeginRecord));

        [Name("停止录制")]
        [Tip("停止录制")]
        [XCSJ.Attributes.Icon(index = 33958)]
        public XGUIContent StopRecord { get; } = GetXGUIContent(nameof(StopRecord));

        [Name("记录关键点")]
        [Tip("记录路径关键点，快捷键Ctrl+R", "Record path keys, shortcut Ctrl + R")]
        [XCSJ.Attributes.Icon(index = 33954)]
        public XGUIContent Record { get; } = GetXGUIContent(nameof(Record));

        private bool createKeyPoint = false;

        public bool lockSM = false;

        private static XGUIContent GetXGUIContent(string propertyName) => new XGUIContent(typeof(PathWindow), propertyName, true);

        #endregion

        /// <summary>
        /// 绘制顶部工具栏界面
        /// </summary>
        private void DrawTopTools()
        {
            if (createKeyPoint && Event.current.type == EventType.Layout)
            {
                createKeyPoint = false;

                pathInfoManager.RecordSelectedPath();
                dataChanging = true;
            }

            EditorGUILayout.BeginHorizontal(XGUIStyleLib.Get(EGUIStyle.Box));
            {
                if (!recording)
                {
                    if (GUILayout.Button(BeginRecord, toolButtonSizeOption))
                    {
                        recording = true;
                        selectedKeyPointIndex = -1;
                    }
                }
                else
                {
                    if (GUILayout.Button(StopRecord, toolButtonSizeOption))
                    {
                        if (pathInfoManager.ExistsTransformOutofPath()
                            && EditorUtility.DisplayDialog("提示", "对象移动之后未保存关键路径点! 是否保存？", "是", "否"))
                        {
                            pathInfoManager.Record();
                            pathInfoManager.ExportData();
                            recording = false;
                        }
                        else
                        {
                            dataChanging = true;
                            recording = false;
                        }

                    }
                }

                EditorGUI.BeginDisabledGroup(!recording);
                if (GUILayout.Button(Record, toolButtonSizeOption))
                {
                    createKeyPoint = true;
                }
                EditorGUI.EndDisabledGroup();

                GUILayout.FlexibleSpace();

                if (GUILayout.Button(CommonFun.NameTip(EIcon.Config), toolButtonSizeOption))
                {
                    XDreamerPreferences.OpenWindow<PathWindowOption>(true);
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        private void OnKeyboardEvent()
        {
            if (recording
                && ((Event.current.modifiers & EventModifiers.Control) != EventModifiers.None)
                && Event.current.keyCode == KeyCode.R
                && Event.current.type == EventType.KeyUp
                )
            {
                createKeyPoint = true;
            }
        }

        #endregion

        #region 状态机或子状态机

        /// <summary>
        /// 使用当前状态机
        /// </summary>
        public bool useCurrentSM = true;

        /// <summary>
        /// 绘制状态机或子状态机的界面
        /// </summary>
        protected void DrawSM()
        {
            var smValid = pathInfoManager.smValid;

            EditorGUILayout.BeginHorizontal(GUI.skin.box);
            {
                EditorGUI.BeginDisabledGroup(lockSM);
                useCurrentSM = GUILayout.Toggle(useCurrentSM, "当前状态机");
                EditorGUI.EndDisabledGroup();

                EditorGUI.BeginDisabledGroup(useCurrentSM);
                {
                    pathInfoManager.subSM = EditorSMSHelper.SubStateMachinePopup((SubStateMachine)pathInfoManager.subSM, null, true, false);
                }
                EditorGUI.EndDisabledGroup();

                lockSM = UICommonFun.ButtonToggle(CommonFun.NameTip(EIcon.Lock), lockSM, EditorStyles.miniButton, UICommonOption.WH24x16);
            }
            EditorGUILayout.EndHorizontal();

            if (!smValid)
            {
                DrawInfoBox("请打开一个状态机（注意：根状态机无效）！", MessageType.Error);
            }
        }

        #endregion

        #region 路径状态组件待创建列表

        [Name("路径状态组件待创建列表")]
        [Tip("可创建的路径状态组件包括:移动、游戏对象路径、相机对象路径、路径动画等", "Path state components that can be created include: movement, game object path, camera object path, path animation, etc")]
        public bool createPathExpand = true;

        private GameObject _gameObject;

        private GameObject gameObject
        {
            get
            {
                if (!_gameObject)
                {
                    _gameObject = gameObjects.FirstOrDefault();
                }
                return _gameObject;
            }
            set => _gameObject = value;
        }

        class GameObjectInfo
        {
            public GameObject gameObject;

            public bool effective = true;

            public static implicit operator GameObject(GameObjectInfo gameObjectInfo) => gameObjectInfo?.gameObject;
            public static implicit operator GameObjectInfo(GameObject gameObject) => new GameObjectInfo() { gameObject = gameObject };
        }

        private List<GameObjectInfo> gameObjects = new List<GameObjectInfo>();

        private IEnumerable<GameObjectInfo> effectiveGameObjects => gameObjects.Where(go => go.effective);

        /// <summary>
        /// 生效所有的游戏对象
        /// </summary>
        public bool effectiveAllGO
        {
            get
            {
                return _effectiveAllGO;
            }
            set
            {
                if (_effectiveAllGO != value)
                {
                    gameObjects.ForEach(i => i.effective = value);
                    _effectiveAllGO = value;
                }
            }
        }

        private bool _effectiveAllGO = true;

        private bool lockGameObjects = false;

        [Name("游戏对象类型")]
        public enum EGOType
        {
            [Name("选择集")]
            [XCSJ.Attributes.Icon(EIcon.List)]
            Selection,

            [Name("选择集中第一个对象的子级")]
            [XCSJ.Attributes.Icon(EIcon.Category)]
            SelectionFirstChildren,
        }

        public EGOType goType = EGOType.Selection;

        private Type _pathType = null;
        public Type pathType
        {
            get
            {
                if (_pathType == null)
                {
                    _pathType = pathInfoManager.pathTypes.FirstOrDefault(pt => pt.Name == PathWindowOption.weakInstance.defaultPathType);
                }
                return _pathType;
            }
            set => _pathType = value;
        }

        private void UpdateGameObjects()
        {
            if (lockGameObjects) return;
            gameObject = null;
            gameObjects.Clear();
            switch (goType)
            {
                case EGOType.Selection:
                    {
                        gameObjects.AddRange(Selection.gameObjects.Cast(go => (GameObjectInfo)go));
                        break;
                    }
                case EGOType.SelectionFirstChildren:
                    {
                        var go = Selection.activeGameObject;
                        if (go)
                        {
                            gameObject = go;
                            gameObjects.AddRange(CommonFun.GetChildGameObjects(go).Cast(obj => (GameObjectInfo)obj));
                        }
                        break;
                    }
            }
        }

        private bool canCreateStates => gameObjects.Count > 0 && !recording;

        private void CreateStates()
        {
            if (createMultipleState)
            {
                pathInfoManager.CreateMultipleState(effectiveGameObjects.Cast(go => (GameObject)go), pathType);
            }
            else
            {
                pathInfoManager.CreateSingleState(effectiveGameObjects.Cast(go => (GameObject)go), pathType, PathInfoManager.DefaultStateName(gameObject, pathType));
            }
        }

        /// <summary>
        /// 绘制创建路径工具栏界面
        /// </summary>
        private void DrawCreatePathTools()
        {
            GUILayout.BeginHorizontal();
            {
                #region //游戏对象列表与选择集的对应规则
                {
                    EditorGUI.BeginDisabledGroup(lockGameObjects);
                    EditorGUI.BeginChangeCheck();
                    if (UICommonFun.ButtonToggle(CommonFun.NameTip(EGOType.Selection, ENameTip.EmptyTextWhenHasImage), goType == EGOType.Selection, GUI.skin.button, toolButtonSizeOption))
                    {
                        goType = EGOType.Selection;
                    }
                    if (UICommonFun.ButtonToggle(CommonFun.NameTip(EGOType.SelectionFirstChildren, ENameTip.EmptyTextWhenHasImage), goType == EGOType.SelectionFirstChildren, GUI.skin.button, toolButtonSizeOption))
                    {
                        goType = EGOType.SelectionFirstChildren;
                    }
                    if (EditorGUI.EndChangeCheck())
                    {
                        UpdateGameObjects();
                    }
                    EditorGUI.EndDisabledGroup();
                }
                #endregion

                GUILayout.Space(10);

                foreach (var pathType in pathInfoManager.pathTypes)
                {
                    if (UICommonFun.ButtonToggle(CommonFun.NameTip(pathType, ENameTip.EmptyTextWhenHasImage), this.pathType == pathType, GUI.skin.button, toolButtonSizeOption))
                    {
                        this.pathType = pathType;
                    }
                }

                GUILayout.FlexibleSpace();

                EditorGUI.BeginDisabledGroup(!canCreateStates);
                {
                    if (CommonFun.HasAccess(typeof(ExplodedView)))
                    {
                        explodedViewTools = UICommonFun.ButtonToggle(CommonFun.NameTip(this, nameof(explodedViewTools), ENameTip.EmptyTextWhenHasImage), explodedViewTools, GUI.skin.button, toolButtonSizeOption);
                    }
                    else
                    {
                        explodedViewTools = false;
                    }


                    createMultipleState = UICommonFun.ButtonToggle(CommonFun.NameTip(this, nameof(createMultipleState), ENameTip.EmptyTextWhenHasImage), createMultipleState, GUI.skin.button, toolButtonSizeOption);

                    if (GUILayout.Button(addGUIContent, toolButtonSizeOption))
                    {
                        CreateStates();
                    }
                }
                EditorGUI.EndDisabledGroup();

                var lockGOS = UICommonFun.ButtonToggle(CommonFun.NameTip(EIcon.Lock), lockGameObjects, GUI.skin.button, toolButtonSizeOption);
                if (lockGameObjects != lockGOS)
                {
                    lockGameObjects = lockGOS;
                    UpdateGameObjects();
                }
            }
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// 绘制游戏对象列表
        /// </summary>
        private void DrawGameObjects()
        {
            EditorGUILayout.BeginHorizontal(GUI.skin.box);
            {
                GUILayout.Label("编号", noOption);
                GUILayout.Label("游戏对象", goOption);
                effectiveAllGO = GUILayout.Toggle(effectiveAllGO, CommonFun.TempContent("生效", "不勾选:不可创建对应的状态;\n勾选:可尝试创建对应的状态;"), otherOption);
                GUILayout.Label("状态名称", stateOption);
            }
            EditorGUILayout.EndHorizontal();

            int count = gameObjects.Count;
            if (count > 0)
            {
                EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.ExpandWidth(true));

                for (int i = 0; i < count; ++i)
                {
                    var go = gameObjects[i];
                    UICommonFun.BeginHorizontal(i % 2 == 1);

                    // 编号
                    GUILayout.Label((i + 1).ToString(), noOption);

                    // 游戏对象
                    EditorGUILayout.ObjectField(go, typeof(GameObject), true, goOption);

                    // 生效
                    go.effective = GUILayout.Toggle(go.effective, "", otherOption);

                    if (createMultipleState)
                    {
                        // 状态名称
                        EditorGUILayout.LabelField(PathInfoManager.DefaultStateName(go, pathType));
                    }
                    else if (i == 0)
                    {
                        // 状态名称
                        EditorGUILayout.LabelField(PathInfoManager.DefaultStateName(gameObject, pathType));
                    }

                    UICommonFun.EndHorizontal();
                }

                EditorGUILayout.EndVertical();
            }
        }

        /// <summary>
        /// 绘制创建路径界面
        /// </summary>
        private void DrawCreatePath()
        {
            if (!(createPathExpand = UICommonFun.Foldout(createPathExpand, CommonFun.NameTooltip(this, nameof(createPathExpand)))))
            {
                return;
            }

            CommonFun.BeginLayout();
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.BeginVertical();
                    {
                        DrawCreatePathTools();

                        DrawGameObjects();
                    }
                    EditorGUILayout.EndVertical();

                    if (explodedViewTools)
                    {
                        createMultipleState = true;
                        explodedViewToolsArea = UICommonFun.ResizeSeparatorLayout(explodedViewToolsArea, true, false, XGUIStyleLib.Get(EGUIStyle.Separator), GUILayout.Width(4), GUILayout.ExpandHeight(true));
                        EditorGUILayout.BeginVertical(GUILayout.Width(explodedViewToolsArea.x));
                        DrawExplodeViewTools();
                        DrawExplodeViewOptions();
                        EditorGUILayout.EndVertical();
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            CommonFun.EndLayout();
        }

        #endregion

        #region 路径状态组件列表

        [Name("路径状态组件列表")]
        [Tip("路径状态组件包括:移动、游戏对象路径、相机对象路径、路径动画等", "Path state components include: movement, game object path, camera object path, path animation, etc")]
        public bool pathInfoListExpand = true;

        /// <summary>
        /// 生效所有的状态组件
        /// </summary>
        public bool effectiveAllSC
        {
            get
            {
                return _effectiveAllSC;
            }
            set
            {
                if (_effectiveAllSC != value)
                {
                    pathInfoManager.SetEffective(value);
                    _effectiveAllSC = value;
                }
            }
        }

        private bool _effectiveAllSC = true;

        public bool displayAll
        {
            get
            {
                return _displayAll;
            }
            set
            {
                if (_displayAll != value)
                {
                    pathInfoManager.SetDisplayPath(value);
                }
                _displayAll = value;
            }
        }

        private bool _displayAll = true;

        [Name("创建多个状态")]
        [Tip("勾选,使用选择的游戏对象分别创建路径状态;\n不勾选,使用选择的游戏对象创建一个路径状态;", "Check to create the path status using the selected game object; \n uncheck and create a path state using the selected game object;")]
        [XCSJ.Attributes.Icon(EIcon.Organization)]
        public bool createMultipleState = true;

        [Name("清空")]
        [Tip("清空状态组件对象的路径数据", "Clear path data of status component object")]
        [XCSJ.Attributes.Icon(EIcon.Clear)]
        public XGUIContent clearGUIContent { get; } = GetXGUIContent(nameof(clearGUIContent));

        [Name("创建")]
        [Tip("使用场景选中的游戏对象，创建状态", "Use the game object selected in the scene to create a state")]
        [XCSJ.Attributes.Icon(EIcon.Add)]
        public XGUIContent addGUIContent { get; } = GetXGUIContent(nameof(addGUIContent));

        [Name("删除")]
        [Tip("删除状态列表", "Delete status list")]
        [XCSJ.Attributes.Icon(EIcon.Delete)]
        public XGUIContent deleteGUIContent { get; } = GetXGUIContent(nameof(deleteGUIContent));

        /// <summary>
        /// 绘制路径状态组件列表界面
        /// </summary>
        protected void DrawPathList()
        {
            if (!(pathInfoListExpand = UICommonFun.Foldout(pathInfoListExpand, CommonFun.NameTooltip(this, nameof(pathInfoListExpand)), () =>
            {
                EditorGUI.BeginDisabledGroup(recording);
                {
                    if (GUILayout.Button(clearGUIContent, EditorStyles.miniButtonMid, UICommonOption.WH24x16))
                    {
                        if (EditorUtility.DisplayDialog("提示", "清空状态列表中对象的路径数据,操作将无法恢复!", "确定", "取消"))
                        {
                            pathInfoManager.ClearPathData();
                            dataChanging = true;
                        }
                    }

                    if (GUILayout.Button(deleteGUIContent, EditorStyles.miniButtonRight, UICommonOption.WH24x16))
                    {
                        if (EditorUtility.DisplayDialog("提示", "删除状态列表,操作将无法恢复!", "确定", "取消"))
                        {
                            pathInfoManager.Delete();
                        }
                    }
                }
                EditorGUI.EndDisabledGroup();
            })))
            {
                return;
            }

            CommonFun.BeginLayout();
            {
                DrawPathListTools();
                DrawPathListInfo();
            }
            CommonFun.EndLayout();
        }

        private void DrawPathListTools()
        {
            EditorGUI.BeginDisabledGroup(pathInfoManager.effectivePathInfoCount == 0 || Application.isPlaying);
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUI.BeginDisabledGroup(recording);
                {
                    // 路径反向
                    if (GUILayout.Button(OppositePath, contentButtonSizeOption))
                    {
                        dataChanging = true;
                        pathInfoManager.OppositeDirectionPath();
                    }
                    // 动画反向
                    if (GUILayout.Button(ReverseMotion, contentButtonSizeOption))
                    {
                        dataChanging = true;
                        pathInfoManager.ReverseMotion();
                    }
                }
                EditorGUI.EndDisabledGroup();

                {
                    // 定位起点
                    if (GUILayout.Button(SetToBegin, contentButtonSizeOption))
                    {
                        pathInfoManager.MoveToBegin();
                    }
                    // 定位终点
                    if (GUILayout.Button(SetToEnd, contentButtonSizeOption))
                    {
                        pathInfoManager.MoveToEnd();
                    }
                }

                GUILayout.FlexibleSpace();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUI.EndDisabledGroup();
        }

        private GUILayoutOption noOption { get; } = GUILayout.Width(25);
        private GUILayoutOption stateOption { get; } = GUILayout.MinWidth(80);
        private GUILayoutOption goOption { get; } = GUILayout.Width(180);
        private GUILayoutOption pathCountOption { get; } = GUILayout.Width(40);
        private GUILayoutOption operationOption { get; } = GUILayout.Width(50);
        private GUILayoutOption otherOption { get; } = GUILayout.Width(40);

        /// <summary>
        /// 绘制
        /// </summary>
        private void DrawPathListInfo()
        {
            try
            {
                // 数据字段名称
                EditorGUILayout.BeginHorizontal(GUI.skin.box);
                {
                    GUILayout.Label("编号", noOption);
                    GUILayout.Label("状态名称", stateOption);
                    GUILayout.Label("游戏对象", goOption);
                    GUILayout.Label("路径数", pathCountOption);
                    effectiveAllSC = GUILayout.Toggle(effectiveAllSC, CommonFun.TempContent("生效", "不勾选:无法记录路径，并且不能使用路径详细信息编辑按钮;\n勾选:可记录路径，且路能使用路径详细信息编辑按钮;"), otherOption);
                    displayAll = GUILayout.Toggle(displayAll, CommonFun.TempContent("显示", "勾选，渲染对应项路径"), otherOption);
                    GUILayout.Label("操作", operationOption);
                }
                EditorGUILayout.EndHorizontal();

                // 数据列表
                int pathCount = pathInfoManager.pathCount;
                if (pathCount > 0)
                {
                    EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.ExpandWidth(true));

                    var selectedStyle = XGUIStyleLib.Get(EGUIStyle.Label_Selected_14);
                    for (int i = 0; i < pathCount; ++i)
                    {
                        var data = pathInfoManager.pathInfos[i];
                        bool selected = pathInfoManager.selectedPathInfo == data;

                        if (selected)
                        {
                            EditorGUILayout.BeginHorizontal(selectedStyle);
                        }
                        else
                        {
                            UICommonFun.BeginHorizontal(i % 2 == 1);
                        }
                        // 编号
                        GUILayout.Label((i + 1).ToString(), noOption);

                        // 名称
                        if (GUILayout.Button(CommonFun.TempContent(data.name, "双击选中对应游戏对象并启用【生效】项"), EditorStyles.label, stateOption))
                        {
                            SetSelectedPath(data);
                        }

                        // 游戏对象
                        var color = data.firstTransform ? GUI.color : UnityEngine.Color.red;
                        CommonFun.DrawColorGUI(color, () =>
                        {
                            EditorGUILayout.ObjectField(data.firstTransform, typeof(Transform), true, goOption);
                        });

                        // 路径数
                        GUILayout.Label(data.transformDatas.Count.ToString(), pathCountOption);

                        data.effective = GUILayout.Toggle(data.effective, "", otherOption);
                        data.displayPath = GUILayout.Toggle(data.displayPath, "", otherOption);

                        // 清空路径数据
                        EditorGUI.BeginDisabledGroup(recording);
                        if (GUILayout.Button(clearGUIContent, EditorStyles.miniButtonMid, UICommonOption.WH24x16))
                        {
                            if (EditorUtility.DisplayDialog("提示", "清空路径数据,操作将无法恢复!", "确定", "取消"))
                            {
                                data.Clear();
                                dataChanging = true;
                            }
                        }

                        // 删除
                        if (GUILayout.Button(deleteGUIContent, EditorStyles.miniButtonRight, UICommonOption.WH24x16))
                        {
                            if (EditorUtility.DisplayDialog("提示", "删除记录,操作将无法恢复!", "确定", "取消"))
                            {
                                pathInfoManager.Remove(data);
                                break;
                            }
                        }
                        EditorGUI.EndDisabledGroup();

                        if (selected)
                        {
                            EditorGUILayout.EndHorizontal();
                        }
                        else
                        {
                            UICommonFun.EndHorizontal();
                        }
                    }

                    EditorGUILayout.EndVertical();
                }
                else
                {
                    DrawInfoBox("无记录对象！\n选择游戏对象或从状态机中导入。\n", MessageType.Warning);
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
        }

        #endregion

        #region 路径状态组件详细信息

        #region 界面元素

        [Name("路径状态组件详细信息")]
        [Tip("路径状态组件详细信息", "Path status component details")]
        public bool pathInfoExpand = true;

        [Name("路径反向")]
        [Tip("路径以起点为中心，对称反向", "The path is centered on the starting point and symmetrically reversed")]
        [XCSJ.Attributes.Icon(index = 33953)]
        public XGUIContent OppositePath { get; } = GetXGUIContent(nameof(OppositePath));

        [Name("动画反向")]
        [Tip("路径上关键点的顺序变为原来的逆序，游戏对象设置到终点上", "The order of key points on the path changes to the original reverse order, and the game object is set to the end point")]
        [XCSJ.Attributes.Icon(index = 33955)]
        public XGUIContent ReverseMotion { get; } = GetXGUIContent(nameof(ReverseMotion));

        [Name("定位到起点")]
        [Tip("将游戏对象设置到路径的起点", "Set the GameObject to the starting point of the path")]
        [XCSJ.Attributes.Icon(index = 33956)]
        public XGUIContent SetToBegin { get; } = GetXGUIContent(nameof(SetToBegin));

        [Name("定位到终点")]
        [Tip("将游戏对象设置到路径的终点", "Set the GameObject to the end of the path")]
        [XCSJ.Attributes.Icon(index = 33957)]
        public XGUIContent SetToEnd { get; } = GetXGUIContent(nameof(SetToEnd));

        public List<IPathLayout> pathLayouts = new List<IPathLayout>();

        [Name("路径扩展布局")]
        [Tip("路径详细信息", "Path details")]
        public bool pathLayoutExpand = false;

        #endregion 

        #region 路径状态组件的模拟

        [Name("模拟执行")]
        [Tip("非运行态时模拟路径状态组件的执行情况", "Simulate the execution of path state components in non running state")]
        [XCSJ.Attributes.Icon(EIcon.Play)]
        private bool inSimulation = false;

        private double percent = 0;
        private double time = 0;
        private double ttl = 0;

        private void StartSimulate()
        {
            if (inSimulation || pathInfoManager.selectedPathInfo == null) return;

            var path = pathInfoManager.selectedPathInfo.pathObj;
            if (path is IWorkClip workClip)
            {
                ttl = workClip.totalTimeLength;
                inSimulation = true;

                PathInfoManager.onSelectedPathInfoChanged += OnSelectedPathInfoChanged;
            }
        }

        private void StopSimulate()
        {
            PathInfoManager.onSelectedPathInfoChanged -= OnSelectedPathInfoChanged;
            inSimulation = false;
            percent = 0;
            time = 0;
            ttl = 0;
        }

        private void OnSelectedPathInfoChanged(PathInfo newPathInfo,PathInfo oldPathInfo)
        {
            StopSimulate();
        }

        private void UpdatePercent(double percent)
        {
            if (!inSimulation) return;

            this.percent = MathX.Clamp01(percent);
            time = this.percent * ttl;

            pathInfoManager.selectedPathInfo.MoveToPercent(this.percent, true);
        }

        private void DrawSimulationTools()
        {
            GUILayout.BeginHorizontal(GUI.skin.box);
            {
                EditorGUI.BeginChangeCheck();
                percent = EditorGUILayout.Slider((float)percent, 0, 1);
                if (EditorGUI.EndChangeCheck())
                {
                    UpdatePercent(percent);
                }
                EditorGUI.BeginChangeCheck();
                time = EditorGUILayout.DoubleField(time, UICommonOption.Width60);
                if (EditorGUI.EndChangeCheck())
                {
                    UpdatePercent(time / ttl);
                }

                EditorGUILayout.LabelField(ttl.ToString(), UICommonOption.Width60);
            }
            GUILayout.EndHorizontal();
        }

        #endregion

        /// <summary>
        /// 绘制路径状态组件详细信息界面
        /// </summary>
        protected void DrawPathDetailInfo()
        {
            if (!(pathInfoExpand = UICommonFun.Foldout(pathInfoExpand, CommonFun.NameTooltip(this, nameof(pathInfoExpand)), () =>
            {
                //GUILayout.FlexibleSpace();
                //EditorGUILayout.LabelField(pathInfoManager.selectedPathInfo?.stateName ?? "");
                if(GUILayout.Button(pathInfoManager.selectedPathInfo?.stateName ?? "", GUI.skin.label))
                {

                }
            }))) 
            {
                return;
            }

            CommonFun.BeginLayout();
            {
                DrawPathDetailInfoTools();

                GUILayout.Space(2);

                DrawPathLayout();

                GUILayout.Space(2);

                DrawPathKeyPoints();
            }
            CommonFun.EndLayout();
        }

        /// <summary>
        /// 绘制路径详细信息工具栏界面
        /// </summary>
        private void DrawPathDetailInfoTools()
        {
            var disable = pathInfoManager.selectedPathInfo == null || Application.isPlaying;
            GUILayout.BeginHorizontal();
            {
                EditorGUI.BeginDisabledGroup(disable || inExplodedViewSimulation || recording);
                {
                    // 路径反向
                    if (GUILayout.Button(OppositePath, contentButtonSizeOption))
                    {
                        dataChanging = true;
                        pathInfoManager.selectedPathInfo.OppositeDirectionPath(true);
                    }
                    // 动画反向
                    if (GUILayout.Button(ReverseMotion, contentButtonSizeOption))
                    {
                        dataChanging = true;
                        pathInfoManager.selectedPathInfo.ReverseMotion(true);
                    }
                }
                EditorGUI.EndDisabledGroup();

                EditorGUI.BeginDisabledGroup(disable || inExplodedViewSimulation);
                {
                    // 定位起点
                    if (GUILayout.Button(SetToBegin, contentButtonSizeOption))
                    {
                        pathInfoManager.selectedPathInfo.MoveToBegin(true);
                    }
                    // 定位终点
                    if (GUILayout.Button(SetToEnd, contentButtonSizeOption))
                    {
                        pathInfoManager.selectedPathInfo.MoveToEnd(true);
                    }
                }
                EditorGUI.EndDisabledGroup();

                GUILayout.FlexibleSpace();

                //模拟按钮
                EditorGUI.BeginChangeCheck();
                var simulate = UICommonFun.ButtonToggle(CommonFun.NameTip(GetType(), nameof(inSimulation), ENameTip.EmptyTextWhenHasImage), inSimulation, GUI.skin.button, toolButtonSizeOption);
                if (EditorGUI.EndChangeCheck())
                {
                    if (simulate)
                    {
                        StartSimulate();
                    }
                    else
                    {
                        StopSimulate();
                    }
                }              
            }
            GUILayout.EndHorizontal();

            if (inSimulation)//模拟界面
            {
                EditorGUI.BeginDisabledGroup(disable || recording);
                DrawSimulationTools();
                EditorGUI.EndDisabledGroup();
            }
        }

        /// <summary>
        /// 绘制路径关键点列表界面
        /// </summary>
        private void DrawPathKeyPoints()
        {
            EditorGUILayout.BeginHorizontal(GUI.skin.box);
            {
                GUILayout.Label("编号", GUILayout.Width(25));
                GUILayout.Label("X");
                GUILayout.Label("Y");
                GUILayout.Label("Z");
                GUILayout.Label("操作", GUILayout.Width(80));
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            if (pathInfoManager.selectedPathInfo != null)
            {
                var path = pathInfoManager.selectedPathInfo.transformDatas;
                EditorGUI.BeginChangeCheck();

                for (int i = 0; i < path.Count; ++i)
                {
                    bool isBreak = false;

                    GUILayout.BeginHorizontal();
                    {
                        var data = path[i];
                        GUILayout.Label((i + 1).ToString(), GUILayout.Width(25));
                        var x = EditorGUILayout.DelayedFloatField(data.keyPoint.x);
                        var y = EditorGUILayout.DelayedFloatField(data.keyPoint.y);
                        var z = EditorGUILayout.DelayedFloatField(data.keyPoint.z);
                        data.keyPoint = new Vector3(x, y, z);

                        EditorGUI.BeginDisabledGroup(i == 0);
                        if (GUILayout.Button("↑", EditorStyles.miniButtonMid, GUILayout.Width(20)))
                        {
                            path.RemoveAt(i);
                            path.Insert(i - 1, data);
                            isBreak = true;
                        }
                        EditorGUI.EndDisabledGroup();

                        EditorGUI.BeginDisabledGroup(i == path.Count - 1);
                        if (GUILayout.Button("↓", EditorStyles.miniButtonMid, GUILayout.Width(20)))
                        {
                            path.RemoveAt(i);
                            path.Insert(i + 1, data);
                            isBreak = true;
                        }
                        EditorGUI.EndDisabledGroup();

                        if (GUILayout.Button("+", EditorStyles.miniButtonMid, GUILayout.Width(20)))
                        {
                            path.Insert(i, TransformData.Clone(data));
                            isBreak = true;
                        }
                        if (GUILayout.Button("-", EditorStyles.miniButtonRight, GUILayout.Width(20)))
                        {
                            path.RemoveAt(i);
                            isBreak = true;
                        }
                    }
                    GUILayout.EndHorizontal();

                    if (isBreak)
                    {
                        break;
                    }
                } // end for    

                if (EditorGUI.EndChangeCheck())
                {
                    dataChanging = true;
                }
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawInfoBox(string info, MessageType messageType = MessageType.Info, int infoSize = 13)
        {
            var richText = EditorStyles.helpBox.richText;
            var fontSize = EditorStyles.helpBox.fontSize;
            EditorStyles.helpBox.richText = true;
            EditorStyles.helpBox.fontSize = infoSize;
            EditorGUILayout.HelpBox(info, messageType);
            EditorStyles.helpBox.richText = richText;
            EditorStyles.helpBox.fontSize = fontSize;
        }

        /// <summary>
        /// 绘制路径布局；支持针对IPathLayout接口的扩展
        /// </summary>
        public void DrawPathLayout()
        {
            if (pathLayouts.Count == 0) return;

            if (pathLayoutExpand = UICommonFun.Foldout(pathLayoutExpand, CommonFun.NameTooltip(this, nameof(pathLayoutExpand))))
            {
                CommonFun.BeginLayout();
                foreach (var l in pathLayouts)
                {
                    l.OnGUI(pathInfoManager.selectedPathInfo);
                }//end foreach
                CommonFun.EndLayout();
            }
        }

        #endregion

        #region 爆炸图

        #region 爆炸图基础设置

        [Name("爆炸数据")]
        [HideInSuperInspector]
        public List<ExplodeData> datas = new List<ExplodeData>();

        [Name("爆炸视图类型")]
        [EnumPopup]
        public EExplodeType explodeType = EExplodeType.Point;

        /// <summary>
        /// 排序规则
        /// </summary>
        [Name("排序规则")]
        [EnumPopup]
        public ESortRule sortRule = ESortRule.DistanceAsc;

        [Name("中心类型")]
        [EnumPopup]
        public ECenterType centerType = ECenterType.TransformPosition;

        [Name("中心位置")]
        [Tip("爆炸中心的世界坐标", "World coordinates of the explosion center")]
        [HideInSuperInspector(nameof(centerType), EValidityCheckType.NotEqual, ECenterType.Postion)]
        public Vector3 centerPosition = Vector3.zero;

        [Name("中心变换")]
        [Tip("通过中心变换获取爆炸中心的世界坐标", "The world coordinates of the explosion center are obtained through the center transformation")]
        [HideInSuperInspector(nameof(centerType), EValidityCheckType.Less | EValidityCheckType.Or, ECenterType.TransformPosition, nameof(centerType), EValidityCheckType.GreaterEqual, ECenterType.BoundsCenter)]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Transform centerTransofrm;

        [Name("中心偏移量")]
        [Tip("用于额外纠正爆炸中心的偏移量", "Used to additionally correct the offset of the explosion center")]
        public Vector3 centerOffset = Vector3.zero;

        [Name("方向类型")]
        [EnumPopup]
        public EDirectionType directionType = EDirectionType.Vector;

        [Name("方向向量")]
        [HideInSuperInspector(nameof(directionType), EValidityCheckType.NotEqual, EDirectionType.Vector)]
        public Vector3 directionVector = Vector3.right;

        [Name("方向变换")]
        [HideInSuperInspector(nameof(directionType), EValidityCheckType.Equal, EDirectionType.Vector)]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Transform directionTransform;

        [Name("增量间隔值")]
        [Tip("爆炸计算时，对象包围盒更新爆炸时每次的增量间隔距离值；", "During explosion calculation, the object bounding box updates the incremental interval distance value of each explosion;")]
        [Range(0.001f, 1)]
        public float deltaIntervalValue = 0.01f;

        [Name("最小间隔值")]
        [Tip("爆炸完成后，任意两个对象包围盒之间的最小间隔距离", "The minimum distance between any two object bounding boxes after the explosion is completed")]
        [Range(0.001f, 1)]
        public float minIntervalValue = 0.02f;

        [Name("爆炸倍数")]
        [Tip("可用于将计算的爆炸结果做额外爆炸的倍数值", "It can be used to calculate the explosion results as times of additional explosion")]
        [Range(0.001f, 5)]
        public float explodeMultiple = 1;

        /// <summary>
        /// 爆炸中心
        /// </summary>
        public Vector3 center
        {
            get
            {
                switch (centerType)
                {
                    case ECenterType.Postion: return centerPosition;
                    case ECenterType.TransformPosition: return centerTransofrm.position;
                    case ECenterType.TransformBoundsCenter:
                        {
                            if (CommonFun.GetBounds(out Bounds bounds, centerTransofrm.gameObject))
                            {
                                return bounds.center;
                            }
                            throw new InvalidProgramException("无效的变换包围盒中心");
                        }
                    case ECenterType.BoundsCenter:
                        {
                            if (CommonFun.GetBounds(out Bounds bounds, this.gameObject))
                            {
                                return bounds.center;
                            }
                            throw new InvalidProgramException("无效的包围盒中心：" + centerType.ToString());
                        }
                    default: throw new InvalidProgramException("无效中心类型：" + centerType.ToString());
                }
            }
        }

        /// <summary>
        /// 获取爆炸中心
        /// </summary>
        /// <param name="objects"></param>
        /// <returns></returns>
        private Vector3 GetCenter(List<GameObject> objects)
        {
            switch (centerType)
            {
                case ECenterType.Postion: return centerPosition;
                case ECenterType.TransformPosition: return centerTransofrm.position;
                case ECenterType.TransformBoundsCenter:
                    {
                        if (CommonFun.GetBounds(out Bounds bounds, centerTransofrm.gameObject))
                        {
                            return bounds.center;
                        }
                        throw new InvalidProgramException("无效的变换包围盒中心");
                    }
                case ECenterType.BoundsCenter:
                    {
                        if (CommonFun.GetBounds(out Bounds bounds, objects))
                        {
                            return bounds.center;
                        }
                        throw new InvalidProgramException("无效的包围盒中心：" + centerType.ToString());
                    }
                default: throw new InvalidProgramException("无效中心类型：" + centerType.ToString());
            }
        }

        /// <summary>
        /// 爆炸方向
        /// </summary>
        public Vector3 direction
        {
            get
            {
                switch (directionType)
                {
                    case EDirectionType.Vector: return directionVector;
                    case EDirectionType.TransformX: return directionTransform.right;
                    case EDirectionType.TransformY: return directionTransform.up;
                    case EDirectionType.TransformZ: return directionTransform.forward;
                    case EDirectionType.CenterToTransform: return center - directionTransform.position;
                    default: throw new InvalidProgramException("无效方向类型：" + directionType.ToString());
                }
            }
        }

        /// <summary>
        /// 获取爆炸方向
        /// </summary>
        /// <param name="center"></param>
        /// <returns></returns>
        private Vector3 GetDirection(Vector3 center)
        {
            switch (directionType)
            {
                case EDirectionType.Vector: return directionVector;
                case EDirectionType.TransformX: return directionTransform.right;
                case EDirectionType.TransformY: return directionTransform.up;
                case EDirectionType.TransformZ: return directionTransform.forward;
                case EDirectionType.CenterToTransform: return center - directionTransform.position;
                default: throw new InvalidProgramException("无效方向类型：" + directionType.ToString());
            }
        }

        /// <summary>
        /// 记录数据
        /// </summary>
        public void RecordDatas()
        {
            if (datas.Count > 0) return;
            foreach (var i in this.gameObjects)
            {
                datas.Add(new ExplodeData(i.gameObject.transform));
            }
        }

        /// <summary>
        /// 恢复数据
        /// </summary>
        public void RecovryDatas() => datas.ForEach(data => data.Recover());

        /// <summary>
        /// 清除数据；先恢复后清除
        /// </summary>
        public void ClearDatas()
        {
            RecovryDatas();
            datas.Clear();
        }

        /// <summary>
        /// 更新数据对应的变换
        /// </summary>
        public void UpdateDataTranforms()
        {
            datas.ForEach(data => data.UpdateTranform());
        }

        /// <summary>
        /// 更新数据对应的百分比,会同步更新数据对应的变换
        /// </summary>
        /// <param name="percent"></param>
        public void UpdateDataPercent(float percent)
        {
            datas.ForEach(data =>
            {
                data.UpatePercent(percent);
                data.UpdateTranform();
            });
        }

        #endregion

        #region 爆炸图模拟基础设置

        private bool inExplodedViewSimulation = false;

        private Vector3 explodedViewCenter = Vector3.zero;
        private Vector3 explodedViewDirection = Vector3.right;

        private float explodedViewPercent = 0;
        private void StartExplodeSimulate()
        {
            if (!canCreateStatesForExplodeView) return;
            inExplodedViewSimulation = true;
            explodedViewCenter = center;
            explodedViewDirection = direction;
            RecordDatas();
            explodedViewPercent = 0;
        }

        private void StopExplodeSimulate()
        {
            inExplodedViewSimulation = false;
            ClearDatas();
            explodedViewPercent = 0;
        }

        private void Explode(EExplodeType explodeType)
        {
            RecovryDatas();
            this.explodeType = explodeType;
            explodedViewCenter = this.center;
            explodedViewDirection = this.direction;
            datas = ExplodedViewHelper.Explode(explodeType, datas, explodedViewCenter, explodedViewDirection, deltaIntervalValue, minIntervalValue, sortRule);
            UpdateDataTranforms();
            explodedViewPercent = 1;
        }


        /// <summary>
        /// 绘制爆炸图相关的场景GUI；在录制中时，不会调用本函数；
        /// </summary>
        /// <param name="sceneView"></param>
        private void OnSceneGUIForExplodeView(SceneView sceneView)
        {
            if (!inExplodedViewSimulation) return;

            var color = Handles.color;
            {
                //绘制
                foreach (var data in datas)
                {
                    //包围盒
                    Handles.color = Color.white;
                    Handles.DrawWireCube(data.bounds.center, data.bounds.size);

                    //中心与方向
                    Handles.color = Color.green;
                    Handles.DrawLine(data.center, data.center + data.dir);
                }

                //原始爆炸中心与方向
                Handles.color = Color.blue;
                Handles.DrawLine(explodedViewCenter, explodedViewCenter + explodedViewDirection);
            }
            Handles.color = color;
        }

        #endregion

        [Name("爆炸图工具")]
        [XCSJ.Attributes.Icon(EIcon.ExplodedView)]
        public bool explodedViewTools = false;

        [Name("串联创建")]
        [Tip("使用场景选中的游戏对象，创建状态；并从进入状态开始串联连接创建的状态；", "Use the game object selected in the scene to create a state; And the state of series connection creation from the entry state;")]
        [XCSJ.Attributes.Icon(EIcon.Sequential)]
        public XGUIContent seriesAddGUIContent { get; } = GetXGUIContent(nameof(seriesAddGUIContent));


        [Name("并联创建")]
        [Tip("使用场景选中的游戏对象，创建状态；并从进入状态并联并联连接创建的状态；", "Use the game object selected in the scene to create a state; And the state created by parallel connection from the entry state;")]
        [XCSJ.Attributes.Icon(EIcon.Parallel)]
        public XGUIContent parallelAddGUIContent { get; } = GetXGUIContent(nameof(parallelAddGUIContent));

        public bool lockExplodedView = false;

        public Vector2 explodedViewToolsArea = new Vector2(400, 0);

        private void DrawExplodeViewTools()
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(2);

                GUILayout.Label("爆炸配置界面", XGUIStyleLib.Get(EGUIStyle.Label_Normal_16));

                GUILayout.FlexibleSpace();

                EditorGUI.BeginDisabledGroup(!canCreateStatesForExplodeView);

                if (GUILayout.Button(seriesAddGUIContent, toolButtonSizeOption))
                {
                    CreateStatesForExplodeView(ECreateStatesForExplodeViewType.Series);
                }

                if (GUILayout.Button(parallelAddGUIContent, toolButtonSizeOption))
                {
                    CreateStatesForExplodeView(ECreateStatesForExplodeViewType.Parallel);
                }

                if (GUILayout.Button(addGUIContent, toolButtonSizeOption))
                {
                    CreateStatesForExplodeView(ECreateStatesForExplodeViewType.None);
                }
                EditorGUI.EndDisabledGroup();

                lockExplodedView = UICommonFun.ButtonToggle(CommonFun.NameTip(EIcon.Lock), lockExplodedView, GUI.skin.button, toolButtonSizeOption);
            }
            GUILayout.EndHorizontal();
        }

        private void DrawExplodeViewOptions()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                EditorGUI.BeginDisabledGroup(lockExplodedView);
                {
                    explodeType = (EExplodeType)UICommonFun.EnumPopup(CommonFun.NameTip(GetType(), nameof(explodeType)), explodeType);

                    sortRule = (ESortRule)UICommonFun.EnumPopup(CommonFun.NameTip(GetType(), nameof(sortRule)), sortRule);

                    centerType = (ECenterType)UICommonFun.EnumPopup(CommonFun.NameTip(GetType(), nameof(centerType)), centerType);
                    switch (centerType)
                    {
                        case ECenterType.Postion:
                            {
                                centerPosition = EditorGUILayout.Vector3Field(CommonFun.NameTip(GetType(), nameof(centerPosition)), centerPosition);
                                break;
                            }
                        case ECenterType.BoundsCenter:
                            {
                                break;
                            }
                        default:
                            {
                                centerTransofrm = (Transform)EditorGUILayout.ObjectField(CommonFun.NameTip(GetType(), nameof(centerTransofrm)), centerTransofrm, typeof(Transform), true);
                                break;
                            }
                    }
                    centerOffset = EditorGUILayout.Vector3Field(CommonFun.NameTip(GetType(), nameof(centerOffset)), centerOffset);

                    directionType = (EDirectionType)UICommonFun.EnumPopup(CommonFun.NameTip(GetType(), nameof(directionType)), directionType);
                    switch (directionType)
                    {
                        case EDirectionType.Vector:
                            {
                                directionVector = EditorGUILayout.Vector3Field(CommonFun.NameTip(GetType(), nameof(directionVector)), directionVector);
                                break;
                            }
                        default:
                            {
                                directionTransform = (Transform)EditorGUILayout.ObjectField(CommonFun.NameTip(GetType(), nameof(directionTransform)), directionTransform, typeof(Transform), true);
                                break;
                            }
                    }

                    deltaIntervalValue = EditorGUILayout.Slider(CommonFun.NameTip(GetType(), nameof(deltaIntervalValue)), deltaIntervalValue, 0.001f, 1);

                    minIntervalValue = EditorGUILayout.Slider(CommonFun.NameTip(GetType(), nameof(minIntervalValue)), minIntervalValue, 0.001f, 1);

                    explodeMultiple = EditorGUILayout.Slider(CommonFun.NameTip(GetType(), nameof(explodeMultiple)), explodeMultiple, 0.001f, 5);
                }
                EditorGUI.EndDisabledGroup();

                if (Application.isPlaying || recording) return;

                EditorGUI.BeginDisabledGroup(!canCreateStatesForExplodeView);
                if (GUILayout.Button("启用爆炸图模拟并记录数据"))
                {
                    StartExplodeSimulate();
                }
                EditorGUI.EndDisabledGroup();

                EditorGUI.BeginDisabledGroup(!inExplodedViewSimulation);
                {
                    if (GUILayout.Button("停止爆炸图模拟并还原(清除)数据"))
                    {
                        StopExplodeSimulate();
                    }

                    EditorGUI.BeginDisabledGroup(datas.Count == 0);
                    {
                        if (GUILayout.Button("点爆"))
                        {
                            Explode(EExplodeType.Point);
                        }

                        if (GUILayout.Button("线爆"))
                        {
                            Explode(EExplodeType.Line);
                        }

                        if (GUILayout.Button("面爆"))
                        {
                            Explode(EExplodeType.Plane);
                        }

                        EditorGUI.BeginChangeCheck();
                        explodedViewPercent = EditorGUILayout.Slider(explodedViewPercent, 0, explodeMultiple);
                        if (EditorGUI.EndChangeCheck() && inExplodedViewSimulation)
                        {
                            UpdateDataPercent(explodedViewPercent);
                        }
                    }
                    EditorGUI.EndDisabledGroup();
                }
                EditorGUI.EndDisabledGroup();
            }
            EditorGUILayout.EndVertical();
        }

        private bool canCreateStatesForExplodeView => gameObjects.Count > 0 && !inExplodedViewSimulation && !recording && !Application.isPlaying;

        private enum ECreateStatesForExplodeViewType
        {
            None,
            Series,
            Parallel,
        }

        private void CreateStatesForExplodeView(ECreateStatesForExplodeViewType createStatesForExplodeViewType)
        {
            if (createMultipleState && canCreateStatesForExplodeView)
            {
                var pathInfos = pathInfoManager.CreateMultipleState(effectiveGameObjects.Cast(go => (GameObject)go), pathType);

                //记录原始点
                pathInfos.ForEach(pi =>
                {
                    var path = pi.pathObj;
                    var t = path.transforms.FirstOrDefault();
                    if (t)
                    {
                        path.viewRule = EViewRule.None;
                        path.keyPoints = new List<Vector3>() { t.position };
                    }
                });

                //开始模拟爆炸
                StartExplodeSimulate();

                //执行爆炸模拟
                Explode(explodeType);

                //更新到爆炸结束点
                UpdateDataPercent(explodeMultiple);

                //用于记录爆炸数据与路径信息的隐射关系
                Dictionary<ExplodeData, PathInfo> tmpKVS = new Dictionary<ExplodeData, PathInfo>();

                //记录爆炸后的点
                pathInfos.ForEach(pi =>
                {
                    var path = pi.pathObj;
                    var t = path.transforms.FirstOrDefault();
                    if (t)
                    {
                        path.keyPoints = new List<Vector3>() { path.keyPoints[0], t.position };
                        if (datas.FirstOrDefault(d => d.transform == t) is ExplodeData data)
                        {
                            tmpKVS[data] = pi;
                        }
                    }
                });

                //对新创建的状态做连接
                switch (createStatesForExplodeViewType)
                {
                    case ECreateStatesForExplodeViewType.Series:
                        {
                            var subSM = pathInfoManager.subSM;
                            if (subSM)
                            {
                                var inState = subSM.entryState;

                                //逆序
                                datas.Reverse();

                                //外围的先爆炸，内部的最后爆炸
                                foreach (var data in datas)
                                {
                                    if (inState && tmpKVS.TryGetValue(data, out PathInfo pathInfo) && pathInfo.pathObj is StateComponent component && component && component.parent)
                                    {
                                        subSM.CreateTransition(inState, component.parent);
                                        inState = component.parent;
                                    }
                                }
                                if (inState != subSM.entryState)
                                {
                                    subSM.CreateTransition(inState, subSM.exitState);
                                }
                            }
                            break;
                        }
                    case ECreateStatesForExplodeViewType.Parallel:
                        {
                            var subSM = pathInfoManager.subSM;
                            if (subSM)
                            {
                                var inState = subSM.entryState;
                                if (inState)
                                {
                                    foreach (var data in datas)
                                    {
                                        if (tmpKVS.TryGetValue(data, out PathInfo pathInfo) && pathInfo.pathObj is StateComponent component && component && component.parent)
                                        {
                                            subSM.CreateTransition(inState, component.parent);
                                            subSM.CreateTransition(component.parent, subSM.exitState);
                                        }
                                    }
                                }

                            }
                            break;
                        }
                }
                //
                datas.Reverse();

                //结束模拟爆炸
                StopExplodeSimulate();


            }
        }

        #endregion
    }
}
