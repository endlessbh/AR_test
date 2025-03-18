using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension;
using XCSJ.EditorExtension.EditorWindows;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.Tools;

namespace XCSJ.EditorTools.Windows
{
    /// <summary>
    /// 批量命名窗口
    /// </summary>
    [Name(Title)]
    [XCSJ.Attributes.Icon(EIcon.Edit)]
    [XDreamerEditorWindow(nameof(TrHelper.Other))]
    public class BatchNamingWindow : XEditorWindowWithScrollView<BatchNamingWindow>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "批量命名窗口";

        /// <summary>
        /// 初始化
        /// </summary>
        [MenuItem(XDreamerEditor.EditorWindowMenu + Title)]
        public static void Init() => OpenAndFocus();

        /// <summary>
        /// 当前游戏对象
        /// </summary>
        [Name("当前游戏对象")]
        public GameObject currentGO = null;

        /// <summary>
        /// 使用选择集
        /// </summary>
        [Name("使用选择集")]
        public bool useSelection = true;

        /// <summary>
        /// 前缀名
        /// </summary>
        [Name("前缀名")]
        public string prefixName = "";

        /// <summary>
        /// 后缀名
        /// </summary>
        [Name("后缀名")]
        public string suffixName = "";

        /// <summary>
        /// 数字格式
        /// </summary>
        [Name("数字格式")]
        public string numFormat = NumberSymbol;

        /// <summary>
        /// 开始值
        /// </summary>
        [Name("开始值")]
        public int beginValue = 1;

        /// <summary>
        /// 步长值
        /// </summary>
        [Name("步长值")]
        public int stepValue = 1;

        /// <summary>
        /// 前缀分隔符
        /// </summary>
        [Name("前缀分隔符")]
        public string prefixSeparator = DefaultSeparator;

        /// <summary>
        /// 后缀分隔符
        /// </summary>
        [Name("后缀分隔符")]
        public string suffixSeparator = "";

        /// <summary>
        /// 显示细节
        /// </summary>
        [Name("显示细节")]
        public bool showDetail = true;

        /// <summary>
        /// 自身名称
        /// </summary>
        public const string SelfName = "<self>";

        /// <summary>
        /// 数字标识符
        /// </summary>
        public const string NumberSymbol = "*";

        /// <summary>
        /// 默认分隔符
        /// </summary>
        public const string DefaultSeparator = "_";

        private int noWidth = 25;
        private int btnWidth = 60;

        /// <summary>
        /// 开启定时重绘
        /// </summary>
        public override bool timedRepaint => true;

        /// <summary>
        /// 绘制GUI
        /// </summary>
        protected override void OnGUI()
        {
            EditorGUI.BeginChangeCheck();
            currentGO = EditorToolkitHelper.GameObjectField(CommonFun.NameTooltip(this, nameof(currentGO)), currentGO, CommonFun.NameTooltip(this, nameof(useSelection)), ref useSelection, null, () => {
                if (GUILayout.Button("全部重置", GUILayout.Width(btnWidth)))
                {
                    prefixName = "";
                    suffixName = "";
                    numFormat = NumberSymbol;
                    beginValue = 1;
                    stepValue = 1;
                    prefixSeparator = DefaultSeparator;
                    suffixSeparator = "";
                }
            });
            if(EditorGUI.EndChangeCheck() || useSelection)
            {
                if(currentGO)
                {
                    gameObjects = CommonFun.GetChildGameObjects(currentGO ? currentGO.transform : null);
                    isXDreamer = currentGO.GetComponent<XDreamer>();
                }
                else
                {
                    gameObjects = new List<GameObject>(); ;
                    isXDreamer = false;
                }
            }

            EditorGUILayout.BeginHorizontal();
            prefixName = EditorGUILayout.TextField(CommonFun.NameTooltip(this, nameof(prefixName)), prefixName);
            if (GUILayout.Button("各自名称", GUILayout.Width(btnWidth))) prefixName = SelfName;
            if (GUILayout.Button("对象名", GUILayout.Width(btnWidth)) && currentGO) prefixName = currentGO.name;
            if (GUILayout.Button("重置", GUILayout.Width(btnWidth))) prefixName = "";
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            prefixSeparator = EditorGUILayout.TextField(CommonFun.NameTooltip(this, nameof(prefixSeparator)), prefixSeparator);
            if (GUILayout.Button("置空", GUILayout.Width(btnWidth))) prefixSeparator = "";
            if (GUILayout.Button("重置", GUILayout.Width(btnWidth))) prefixSeparator = DefaultSeparator;
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.TextField(CommonFun.NameTooltip(this, nameof(numFormat)), numFormat);
            EditorGUILayout.LabelField(numFormat.Length.ToString(), GUILayout.Width(btnWidth));
            if (GUILayout.Button("增加", GUILayout.Width(btnWidth))) numFormat += NumberSymbol;
            if (GUILayout.Button("减少", GUILayout.Width(btnWidth)) && numFormat.Length > 1)
            {
                numFormat = numFormat.Substring(0, numFormat.Length - 1);
            }
            if (GUILayout.Button("重置", GUILayout.Width(btnWidth))) numFormat = NumberSymbol;
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            beginValue = EditorGUILayout.IntField(CommonFun.NameTooltip(this, nameof(beginValue)), beginValue);
            if (GUILayout.Button("重置", GUILayout.Width(btnWidth))) beginValue = 1;
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            stepValue = EditorGUILayout.IntField(CommonFun.NameTooltip(this, nameof(stepValue)), stepValue);
            if (GUILayout.Button("重置", GUILayout.Width(btnWidth))) stepValue = 1;
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            suffixSeparator = EditorGUILayout.TextField(CommonFun.NameTooltip(this, nameof(suffixSeparator)), suffixSeparator);
            if (GUILayout.Button("重置", GUILayout.Width(btnWidth))) suffixSeparator = "";
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            suffixName = EditorGUILayout.TextField(CommonFun.NameTooltip(this, nameof(suffixName)), suffixName);
            if (GUILayout.Button("各自名称", GUILayout.Width(btnWidth))) suffixName = SelfName;
            if (GUILayout.Button("对象名", GUILayout.Width(btnWidth)) && currentGO) suffixName = currentGO.name;
            if (GUILayout.Button("重置", GUILayout.Width(btnWidth))) suffixName = "";
            EditorGUILayout.EndHorizontal();

            if (isXDreamer)
            {
                ShowNotification(CommonFun.TempContent("禁止修改 " + nameof(XDreamer) + " 及其子级游戏对象的名称!"));
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("操作");
            EditorGUI.BeginDisabledGroup(isXDreamer);
            if (GUILayout.Button("批量命名"))
            {
                for (int i = 0; i < gameObjects.Count; ++i)
                {
                    gameObjects[i].name = NewName(gameObjects[i].name, i);
                }
                UICommonFun.MarkSceneDirty();
            }
            if (GUILayout.Button("批量命名并尝试同步更新UGUI-Text"))
            {
                for (int i = 0; i < gameObjects.Count; ++i)
                {
                    var go = gameObjects[i];
                    if (!go) continue;

                    var newName = NewName(gameObjects[i].name, i);
                    go.name = newName;

                    var text = go.GetComponent<Text>();
                    if (!text) text = CommonFun.GetChildGameObjects(go.transform).FirstOrDefault(t => t.GetComponent<Text>(), t => t.GetComponent<Text>());
                    if (text) text.text = newName;
                }
                UICommonFun.MarkSceneDirty();
            }
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField("数目", (gameObjects == null ? 0 : gameObjects.Count).ToString());
            var str = string.Format("{0} {1} {2} {3} {4}",
                CommonFun.Name(this, nameof(prefixName)),
                CommonFun.Name(this, nameof(prefixSeparator)),
                CommonFun.Name(this, nameof(numFormat)),
                CommonFun.Name(this, nameof(suffixSeparator)),
                CommonFun.Name(this, nameof(suffixName)));
            EditorGUILayout.LabelField("形式", str);


            if (showDetail = UICommonFun.Foldout(showDetail, CommonFun.NameTooltip(this, nameof(showDetail))))
            {
                CommonFun.BeginLayout();

                #region 标题
                EditorGUILayout.BeginHorizontal("box");
                GUILayout.Label("NO.", GUILayout.Width(noWidth));
                GUILayout.Label("游戏对象");
                GUILayout.Label("新名称");
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Separator();
                #endregion

                base.OnGUI();
                CommonFun.EndLayout();
            }
        }

        private List<GameObject> gameObjects = new List<GameObject>();
        private bool isXDreamer = false;

        /// <summary>
        /// 绘制带滚动视图的GUI
        /// </summary>
        public override void OnGUIWithScrollView()
        {
            for (int i = 0; i < gameObjects.Count; ++i)
            {
                UICommonFun.BeginHorizontal(i % 2 == 1);

                var go = gameObjects[i];

                //"NO."
                GUILayout.Label((i + 1).ToString(), GUILayout.Width(noWidth));

                EditorGUILayout.ObjectField(go, typeof(GameObject), true);

                EditorGUILayout.TextField(NewName(go.name, i));

                UICommonFun.EndHorizontal();
            }
        }

        private string NewName(string name, int i)
        {
            var index = i * stepValue + beginValue;
            var format = "{0}{1}{2:D" + numFormat.Length + "}{3}{4}";
            return string.Format(format, prefixName.Replace(SelfName, name), prefixSeparator, index, suffixSeparator, suffixName.Replace(SelfName, name));
        }

        /// <summary>
        /// 选择集变化时
        /// </summary>
        public void OnSelectionChange() => Repaint();
    }
}
