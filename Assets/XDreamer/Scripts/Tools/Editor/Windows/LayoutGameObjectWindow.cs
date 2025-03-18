using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension;
using XCSJ.EditorExtension.EditorWindows;
using XCSJ.EditorTools.Windows.Layouts;
using XCSJ.Helper;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.Tools;

namespace XCSJ.EditorTools.Windows
{
    [Name("排名")]
    public enum ERankOrder
    {
        [Name("无")]
        [Tip("按默认顺序", "In default order")]
        None,

        [Name("名称升序")]
        [Tip("按照名称升序排列", "Sort by name in ascending order")]
        Ascending,

        [Name("名称降序")]
        [Tip("按照名称降序排列", "按照名称降序排列")]
        Descending,
    }

    [Name(Title)]
    [XCSJ.Attributes.Icon(EIcon.Layout)]
    [XDreamerEditorWindow(nameof(TrHelper.Other))]
    public class LayoutGameObjectWindow : XEditorWindowWithScrollView<LayoutGameObjectWindow>
    {
        public const string Title = "布局-游戏对象";

        [MenuItem(XDreamerMenu.EditorWindow + Title)]
        public static void Init() => OpenAndFocus();

        private static XGUIContent GetXGUIContent(string propertyName) => new XGUIContent(typeof(LayoutGameObjectWindow), propertyName, true);

        public const int noWidth = 25;

        public GUILayoutOption[] toolButtonSizeOption;

        public GUILayoutOption[] contentButtonSizeOption;

        #region 基础操作

        public TransformUndo undo = new TransformUndo();

        public bool isLocked = false;

        public void ShowBaseOperation()
        {
            EditorGUILayout.BeginHorizontal("box", GUILayout.ExpandWidth(true));
            EditorGUI.BeginDisabledGroup(!undo.CanUndo());
            if (GUILayout.Button(CommonFun.NameTip(EIcon.Undo), toolButtonSizeOption))
            {
                if (undo.Undo()) UICommonFun.MarkSceneDirty();
            }
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(!undo.CanDo());
            if (GUILayout.Button(CommonFun.NameTip(EIcon.Redo), toolButtonSizeOption))
            {
                if (undo.Do()) UICommonFun.MarkSceneDirty();
            }
            EditorGUI.EndDisabledGroup();

            isLocked = UICommonFun.ButtonToggle(CommonFun.NameTip(EIcon.Lock), isLocked, GUI.skin.button, toolButtonSizeOption);

            GUILayout.FlexibleSpace();

            if (GUILayout.Button(CommonFun.NameTip(EIcon.Config), toolButtonSizeOption))
            {
                XDreamerPreferences.OpenWindow<ToolEditorWindowOption>();
                XDreamerPreferences.OpenWindow<LayoutOption>();
            }

            EditorGUILayout.EndHorizontal();
        }

        #endregion

        #region 基础布局

        [Name("基础布局")]
        public bool showBaseLayout = true;

        [Name("等坐标X")]
        [Tip("以 标准变换1 为基准进行 等坐标X 操作", "Coordinate X operation based on standard transformation 1")]
        [XCSJ.Attributes.Icon(index = 36230)]
        public XGUIContent SamePositionX { get; } = GetXGUIContent(nameof(SamePositionX));

        [Name("等坐标Y")]
        [Tip("以 标准变换1 为基准进行 等坐标Y 操作", "Coordinate y operation based on standard transformation 1")]
        [XCSJ.Attributes.Icon(index = 36231)]
        public XGUIContent SamePositionY { get; } = GetXGUIContent(nameof(SamePositionY));

        [Name("等坐标Z")]
        [Tip("以 标准变换1 为基准进行 等坐标Z 操作", "Coordinate Z operation based on standard transformation 1")]
        [XCSJ.Attributes.Icon(index = 36232)]
        public XGUIContent SamePositionZ { get; } = GetXGUIContent(nameof(SamePositionZ));

        [Name("等本地缩放X")]
        [Tip("以 标准变换1 为基准进行 等本地缩放X 操作", "Local scaling x operation based on standard transformation 1")]
        [XCSJ.Attributes.Icon(index = 36227)]
        public XGUIContent SameLocalScaleX { get; } = GetXGUIContent(nameof(SameLocalScaleX));

        [Name("等本地缩放Y")]
        [Tip("以 标准变换1 为基准进行 等本地缩放Y 操作", "Perform local scaling y operation based on standard transformation 1")]
        [XCSJ.Attributes.Icon(index = 36228)]
        public XGUIContent SameLocalScaleY { get; } = GetXGUIContent(nameof(SameLocalScaleY));

        [Name("等本地缩放Z")]
        [Tip("以 标准变换1 为基准进行 等本地缩放Z 操作", "Local scaling Z operation based on standard transformation 1")]
        [XCSJ.Attributes.Icon(index = 36229)]
        public XGUIContent SameLocalScaleZ { get; } = GetXGUIContent(nameof(SameLocalScaleZ));

        [Name("等欧拉角X")]
        [Tip("以 标准变换1 为基准进行 等欧拉角X 操作", "Equal Euler angle X operation based on standard transformation 1")]
        [XCSJ.Attributes.Icon(index = 36224)]
        public XGUIContent SameEulerAnglesX { get; } = GetXGUIContent(nameof(SameEulerAnglesX));

        [Name("等欧拉角Y")]
        [Tip("以 标准变换1 为基准进行 等欧拉角Y 操作", "Equal Euler angle y operation based on standard transformation 1")]
        [XCSJ.Attributes.Icon(index = 36225)]
        public XGUIContent SameEulerAnglesY { get; } = GetXGUIContent(nameof(SameEulerAnglesY));

        [Name("等欧拉角Z")]
        [Tip("以 标准变换1 为基准进行 等欧拉角Z 操作", "Equal Euler angle Z operation based on standard transformation 1")]
        [XCSJ.Attributes.Icon(index = 36226)]
        public XGUIContent SameEulerAnglesZ { get; } = GetXGUIContent(nameof(SameEulerAnglesZ));

        [Name("中心等间隔")]
        [Tip("以 标准变换1 与 标准变换2 为基准进行 中心等间隔 线性补间操作", "Take standard transformation 1 and standard transformation 2 as the benchmark for center equidistant linear interpolation operation")]
        [Attributes.Icon]
        public XGUIContent CenterSameSpace { get; } = GetXGUIContent(nameof(CenterSameSpace));

        public void ShowBaseLayout()
        {
            if (showBaseLayout = UICommonFun.Foldout(showBaseLayout, CommonFun.NameTooltip(this, nameof(showBaseLayout))))
            {
                CommonFun.BeginLayout();

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(SamePositionX, GUI.skin.button, contentButtonSizeOption))
                {
                    var ts = effectiveTransforms;
                    undo.Record(ts, () => LayoutHelper.SamePositionX(ts, standardTransform1));
                }
                if (GUILayout.Button(SamePositionY, GUI.skin.button, contentButtonSizeOption))
                {
                    var ts = effectiveTransforms;
                    undo.Record(ts, () => LayoutHelper.SamePositionY(ts, standardTransform1));
                }
                if (GUILayout.Button(SamePositionZ, GUI.skin.button, contentButtonSizeOption))
                {
                    var ts = effectiveTransforms;
                    undo.Record(ts, () => LayoutHelper.SamePositionZ(ts, standardTransform1));
                }


                if (GUILayout.Button(SameLocalScaleX, GUI.skin.button, contentButtonSizeOption))
                {
                    var ts = effectiveTransforms;
                    undo.Record(ts, () => LayoutHelper.SameLocalScaleX(ts, standardTransform1));
                }
                if (GUILayout.Button(SameLocalScaleY, GUI.skin.button, contentButtonSizeOption))
                {
                    var ts = effectiveTransforms;
                    undo.Record(ts, () => LayoutHelper.SameLocalScaleY(ts, standardTransform1));
                }
                if (GUILayout.Button(SameLocalScaleZ, GUI.skin.button, contentButtonSizeOption))
                {
                    var ts = effectiveTransforms;
                    undo.Record(ts, () => LayoutHelper.SameLocalScaleZ(ts, standardTransform1));
                }


                if (GUILayout.Button(SameEulerAnglesX, GUI.skin.button, contentButtonSizeOption))
                {
                    var ts = effectiveTransforms;
                    undo.Record(ts, () => LayoutHelper.SameEulerAnglesX(ts, standardTransform1));
                }
                if (GUILayout.Button(SameEulerAnglesY, GUI.skin.button, contentButtonSizeOption))
                {
                    var ts = effectiveTransforms;
                    undo.Record(ts, () => LayoutHelper.SameEulerAnglesY(ts, standardTransform1));
                }
                if (GUILayout.Button(SameEulerAnglesZ, GUI.skin.button, contentButtonSizeOption))
                {
                    var ts = effectiveTransforms;
                    undo.Record(ts, () => LayoutHelper.SameEulerAnglesZ(ts, standardTransform1));
                }


                if (GUILayout.Button(CenterSameSpace, GUI.skin.button, contentButtonSizeOption))
                {
                    var ts = effectiveTransforms;
                    undo.Record(ts, () => LayoutHelper.CenterSameSpace(ts, standardTransform1, standardTransform2));
                }

                EditorGUILayout.EndHorizontal();
                CommonFun.EndLayout();
            }
        }

        #endregion

        #region 其它布局

        public List<ITransformLayoutWindow> layouts = new List<ITransformLayoutWindow>();

        [Name("其它布局")]
        public bool showOtherLayout = false;

        public void ShowOtherLayout()
        {
            if (showOtherLayout = UICommonFun.Foldout(showOtherLayout, CommonFun.NameTooltip(this, nameof(showOtherLayout))))
            {
                CommonFun.BeginLayout();
                var ts = effectiveTransforms;
                foreach (var l in layouts)
                {
                    if (l.expanded = UICommonFun.Foldout(l.expanded, CommonFun.NameTooltip(l.GetType())))
                    {
                        try
                        {
                            CommonFun.BeginLayout();
                            undo.Record(ts, () => l.OnGUI(effectiveTransforms, standardTransform1, standardTransform2));
                        }
                        finally
                        {
                            CommonFun.EndLayout();
                        }
                    }//end expanded
                }//end foreach
                CommonFun.EndLayout();
            }
        }

        #endregion

        #region 基础信息

        [Name("基础信息")]
        public bool showBaseInfo = true;

        [Name("当前游戏对象")]
        public GameObject currentGO = null;

        [Name("使用选择集")]
        public bool useSelection = true;

        [Name("使用完整选择集")]
        [Tip("勾选,对所有处于选择集中的游戏对象进行布局;不勾选，仅对选择集中激活游戏对象的子级游戏对象进行布局;", "Check to layout all game objects in the selection set; If unchecked, only the child game objects of the selected game objects will be laid out;")]
        public bool useFullSelection = true;

        [Name("自动清理撤销")]
        [Tip("当前游戏对象发生变化时，会自动将撤销操作信息清空;", "当前游戏对象发生变化时，会自动将撤销操作信息清空;")]
        public bool autoClearUndo = true;

        [Name("当前变换(只读)")]
        public Transform currentTransform = null;

        [Name("锁定")]
        [Tip("锁定标准变换1", "Lock standard transform 1")]
        public bool lockStandardTransform1 = false;

        private Transform _standardTransform1 = null;

        [Name("标准变换1")]
        public Transform standardTransform1
        {
            get => _standardTransform1;
            set
            {
                if (!lockStandardTransform1)
                {
                    _standardTransform1 = value;

                    var i = infos.FindIndex(info => info.transform == _standardTransform1);
                    if (i > 0)
                    {
                        var tmp = infos[i];
                        infos.RemoveAt(i);
                        infos.Insert(0, tmp);
                    }
                }
            }
        }

        [Name("锁定")]
        [Tip("锁定标准变换2", "Lock transformation standard 2")]
        public bool lockStandardTransform2 = false;

        private Transform _standardTransform2 = null;

        [Name("标准变换2")]
        public Transform standardTransform2
        {
            get => _standardTransform2;
            set
            {
                if (!lockStandardTransform2)
                {
                    _standardTransform2 = value;

                    var i = infos.FindIndex(info => info.transform == _standardTransform2);
                    if (i >= 0 && i < infos.Count - 1)
                    {
                        var tmp = infos[i];
                        infos.RemoveAt(i);
                        infos.Add(tmp);
                    }
                }
            }
        }

        public void ShowBaseInfo()
        {
            if (showBaseInfo = UICommonFun.Foldout(showBaseInfo, CommonFun.NameTooltip(this, nameof(showBaseInfo))))
            {
                CommonFun.BeginLayout();

                EditorGUI.BeginDisabledGroup(isLocked);
                var go = EditorToolkitHelper.GameObjectField(CommonFun.NameTooltip(this, nameof(currentGO)), currentGO, CommonFun.NameTooltip(this, nameof(useSelection)), ref useSelection);
                useFullSelection = EditorGUILayout.Toggle(CommonFun.NameTooltip(this, nameof(useFullSelection)), useFullSelection);
                EditorGUI.EndDisabledGroup();

                if (currentGO != go && !isLocked)
                {
                    if (autoClearUndo) undo.Clear();

                    currentTransform = null;
                    standardTransform1 = null;
                    standardTransform2 = null;
                    infos.Clear();
                    currentGO = go;

                    if (go)
                    {
                        currentTransform = go.GetComponent<Transform>();
                        UpdateInfos();
                    }
                }

                autoClearUndo = EditorGUILayout.Toggle(CommonFun.NameTooltip(this, nameof(autoClearUndo)), autoClearUndo);

                //EditorGUILayout.ObjectField(CommonFun.NameTooltip(this, nameof(currentTransform)), currentTransform, typeof(Transform), true);

                EditorGUILayout.BeginHorizontal();
                standardTransform1 = (Transform)EditorGUILayout.ObjectField(CommonFun.NameTooltip(this, nameof(standardTransform1)), standardTransform1, typeof(Transform), true);
                lockStandardTransform1 = UICommonFun.ButtonToggle(CommonFun.NameTooltip(this, nameof(lockStandardTransform1)), lockStandardTransform1, EditorStyles.miniButtonRight, GUILayout.Width(60));
                EditorGUILayout.EndHorizontal();



                EditorGUILayout.BeginHorizontal();
                standardTransform2 = (Transform)EditorGUILayout.ObjectField(CommonFun.NameTooltip(this, nameof(standardTransform2)), standardTransform2, typeof(Transform), true);
                lockStandardTransform2 = UICommonFun.ButtonToggle(CommonFun.NameTooltip(this, nameof(lockStandardTransform2)), lockStandardTransform2, EditorStyles.miniButtonRight, GUILayout.Width(60));
                EditorGUILayout.EndHorizontal();

                CommonFun.EndLayout();
            }
        }

        #endregion

        #region 详细信息

        public class Info
        {
            public Transform transform;
            public bool ignore = false;
        }

        [Name("生效类型")]
        [Tip("查找游戏对象上存在某些特定类型名称的组件", "Find components with certain type names on the GameObject")]
        public string effectiveType = "";

        public bool IsEffectiveType(Transform transform, string effectiveType)
        {
            try
            {
                if (string.IsNullOrEmpty(effectiveType)) return true;
                foreach (var c in transform.GetComponents<Component>())
                {
                    if (c && c.GetType().Name.IndexOf(effectiveType, StringComparison.CurrentCultureIgnoreCase) >= 0)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        [Name("名称匹配")]
        [Tip("游戏对象名中存在特定字符串", "There is a specific string in the game object name")]
        public string nameMatch = "";

        public bool IsNameMatch(Transform transform, string nameMatch)
        {
            try
            {
                return string.IsNullOrEmpty(nameMatch) || transform.name.IndexOf(nameMatch, StringComparison.CurrentCultureIgnoreCase) >= 0;
            }
            catch
            {
                return false;
            }
        }

        [Name("详细信息")]
        public bool showDetailInfo = true;

        public List<Info> infos = new List<Info>();

        public List<Transform> transforms => infos.Where(i => !i.ignore).ToList(i => i.transform);

        public List<Transform> effectiveTransforms => infos.Where(i => !i.ignore && IsEffectiveType(i.transform, effectiveType) && IsNameMatch(i.transform, nameMatch)).ToList(i => i.transform);
        
        private void UpdateInfos()
        {
            if (currentTransform)
            {
                if (useFullSelection)
                {
                    infos = Selection.gameObjects.ToList(o => new Info() { transform = o.transform });
                }
                else
                {
                    infos = CommonFun.GetChildGameObjects(currentTransform).ToList(o => new Info() { transform = o.transform });
                }

                if(order != ERankOrder.None) SetInfoOder();

                if (infos.Count > 0)
                {
                    standardTransform1 = infos[0].transform;
                    standardTransform2 = infos[infos.Count - 1].transform;
                }
            }
        }

        [Name("排序方式")]
        [EnumPopup]
        public ERankOrder order = ERankOrder.None;

        void SetInfoOder()
        {
            switch(order)
            {
                case ERankOrder.Ascending:
                    {
                        infos.Sort((a, b) => a.transform.name.CompareTo(b.transform.name));
                        break;
                    }
                case ERankOrder.Descending:
                    {
                        infos.Sort((a, b) => b.transform.name.CompareTo(a.transform.name));
                        break;
                    }
                default:
                    {
                        UpdateInfos();
                        break;
                    }
            }

            if (infos.Count > 0)
            {
                standardTransform1 = infos[0].transform;
                standardTransform2 = infos[infos.Count - 1].transform;
            }
        }

        public void ShowDetailInfo()
        {
            if (showDetailInfo = UICommonFun.Foldout(showDetailInfo, CommonFun.NameTooltip(this, nameof(showDetailInfo))))
            {
                CommonFun.BeginLayout();

                EditorGUILayout.BeginHorizontal();
                effectiveType = EditorGUILayout.TextField(CommonFun.NameTooltip(this, nameof(effectiveType)), effectiveType);
                if (GUILayout.Button("置空", EditorStyles.miniButtonRight, GUILayout.Width(60)))
                {
                    effectiveType = "";
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                nameMatch = EditorGUILayout.TextField(CommonFun.NameTooltip(this, nameof(nameMatch)), nameMatch);
                if (GUILayout.Button("置空", EditorStyles.miniButtonRight, GUILayout.Width(60)))
                {
                    nameMatch = "";
                }
                EditorGUILayout.EndHorizontal();

                var preOder = order;
                order = (ERankOrder)UICommonFun.EnumPopup(CommonFun.NameTooltip(this, nameof(order)), order);

                if (order != preOder)
                {
                    SetInfoOder();
                }

                #region 标题
                EditorGUILayout.BeginHorizontal("box");
                GUILayout.Label("NO.", GUILayout.Width(noWidth));
                GUILayout.Label("矩形变换");
                GUILayout.Label("", GUILayout.Width(58));
                GUILayout.Label("忽略");
                GUILayout.Label("生效类型");
                GUILayout.Label("名称匹配");
                GUILayout.Label("生效");
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Separator();
                #endregion
                
                for (int i = 0; i < infos.Count; ++i)
                {
                    UICommonFun.BeginHorizontal(i % 2 == 1);

                    var info = infos[i];

                    //"NO."
                    GUILayout.Label((i + 1).ToString(), GUILayout.Width(noWidth));

                    EditorGUILayout.ObjectField(info.transform, typeof(Transform), true);

                    EditorGUI.BeginDisabledGroup(i == 0);
                    if (GUILayout.Button("↑", EditorStyles.miniButtonLeft, GUILayout.Width(18)))
                    {
                        infos[i] = infos[i - 1];
                        infos[i - 1] = info;
                        if (i == 1) standardTransform1 = infos[0].transform;
                        if (i == infos.Count - 1) standardTransform2 = infos[infos.Count - 1].transform;
                    }
                    EditorGUI.EndDisabledGroup();

                    EditorGUI.BeginDisabledGroup( i == infos.Count - 1);
                    if (GUILayout.Button("↓", EditorStyles.miniButtonRight, GUILayout.Width(18)))
                    {
                        infos[i] = infos[i + 1];
                        infos[i + 1] = info;
                        if (i == 0) standardTransform1 = infos[0].transform;
                        if (i == infos.Count - 2) standardTransform2 = infos[infos.Count - 1].transform;
                    }
                    EditorGUI.EndDisabledGroup();

                    info.ignore = EditorGUILayout.Toggle(info.ignore);

                    var effective = IsEffectiveType(info.transform, effectiveType);
                    EditorGUILayout.Toggle(effective);

                    var match = IsNameMatch(info.transform, nameMatch);
                    EditorGUILayout.Toggle(match);

                    EditorGUILayout.Toggle(!info.ignore && effective && match);

                    UICommonFun.EndHorizontal();
                }
                CommonFun.EndLayout();
            }

        }

        #endregion        

        protected override void OnEnable()
        {
            base.OnEnable();
            toolButtonSizeOption = ToolEditorWindowOption.weakInstance.toolButtonSizeOption;
            contentButtonSizeOption = ToolEditorWindowOption.weakInstance.contentButtonSizeOption;

            TypeHelper.FindTypeInAppWithInterface(typeof(ITransformLayoutWindow)).ForEach(t => layouts.Add(TypeHelper.CreateInstance(t) as ITransformLayoutWindow));

            EditorApplication.projectChanged += InitData;
            XDreamerEditor.onBeforeCompileAllAssets += InitData;
        }

        protected override void OnDisable()
        {
            XDreamerEditor.onBeforeCompileAllAssets -= InitData;
            EditorApplication.projectChanged -= InitData;
        }

        private void InitData()
        {
            lockStandardTransform1 = false;
            lockStandardTransform2 = false;
            isLocked = false;
        }

        public void OnSelectionChange()
        {
            if (!isLocked && useFullSelection)
            {
                infos.Clear();
                UpdateInfos();
            }
            Repaint();
        }

        private void OnHierarchyChange()
        {
            if (currentTransform && currentTransform.childCount != infos.Count)
            {
                infos.Clear();
                UpdateInfos();
            }
            Repaint();
        }

        protected override void OnGUI()
        {
            ShowBaseOperation();
            ShowBaseLayout();
            ShowOtherLayout();
            base.OnGUI();
        }

        public override void OnGUIWithScrollView()
        {
            ShowBaseInfo();
            ShowDetailInfo();
        }
    }
}
