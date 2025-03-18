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
    /// 别名窗口
    /// </summary>
    [Name(Title)]
    [XCSJ.Attributes.Icon(EIcon.Edit)]
    [XDreamerEditorWindow(nameof(TrHelper.Other))]
    public class AliasWindow : XEditorWindowWithScrollView<AliasWindow>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "别名窗口";

        /// <summary>
        /// 初始化
        /// </summary>
        //[MenuItem(XDreamerEditor.EditorWindowMenu + Title)]
        public static void Init() => OpenAndFocus();

        /// <summary>
        /// 启用时
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            //Alias.Init();
            FindAlias();
            XDreamerEvents.onSceneAnyAssetsChanged += FindAlias;
        }

        /// <summary>
        /// 禁用时
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            XDreamerEvents.onSceneAnyAssetsChanged += FindAlias;
        }

        //private List<Alias> aliases = new List<Alias>();

        private void FindAlias()
        {
            //aliases.Clear();
            //aliases.AddRange(Resources.FindObjectsOfTypeAll(typeof(Alias)).Where(o => o is Alias && string.IsNullOrEmpty(AssetDatabase.GetAssetPath(o))).Cast(o => o as Alias));
        }

        private const int noWidth = 25;
        private const int width = 240;
        private const int btnWidth = 60;

        /// <summary>
        /// 开启定时重绘
        /// </summary>
        public override bool timedRepaint => true;

        //private bool goAsc = true;
        //private bool namePathAsc = true;
        //private bool alscAsc = true;

        /// <summary>
        /// 绘制GUI
        /// </summary>
        protected override void OnGUI()
        {
            //#region 标题
            //EditorGUILayout.BeginHorizontal("box");
            //GUILayout.Label("NO.", GUILayout.Width(noWidth));
            //if (GUILayout.Button(CommonFun.TempContent("游戏对象(只读)", "点击可根据游戏对象名称排序"), UICommonOption.buttonToLabelStyle, GUILayout.Width(width)))
            //{
            //    CommonFun.FocusControl();
            //    aliases.Sort((x, y) => goAsc ? string.Compare(x.name, y.name) : string.Compare(y.name, x.name));
            //    goAsc = !goAsc;
            //}
            //if (GUILayout.Button(CommonFun.TempContent("名称路径(只读)", "点击可根据游戏对象名称路径排序"), UICommonOption.buttonToLabelStyle))
            //{
            //    CommonFun.FocusControl();
            //    aliases.Sort((x, y) => namePathAsc ? string.Compare(CommonFun.GameObjectToString(x.gameObject), CommonFun.GameObjectToString(y.gameObject)) : string.Compare(CommonFun.GameObjectToString(y.gameObject), CommonFun.GameObjectToString(x.gameObject)));
            //    namePathAsc = !namePathAsc;
            //}
            //if (GUILayout.Button(CommonFun.TempContent("别名", "点击可根据别名排序"), UICommonOption.buttonToLabelStyle, GUILayout.Width(width)))
            //{
            //    CommonFun.FocusControl();
            //    aliases.Sort((x, y) => alscAsc ? string.Compare(x.alias, y.alias) : string.Compare(y.alias, x.alias));
            //    alscAsc = !alscAsc;
            //}
            //if (GUILayout.Button("重置", GUILayout.Width(btnWidth)))
            //{
            //    CommonFun.FocusControl();
            //    aliases.ForEach(a => a.Reset());
            //}
            //EditorGUILayout.EndHorizontal();
            //EditorGUILayout.Separator();
            //#endregion

            base.OnGUI();
        }

        /// <summary>
        /// 绘制带滚动视图的GUI
        /// </summary>
        public override void OnGUIWithScrollView()
        {
            //for (int i = 0; i < aliases.Count; ++i)
            //{
            //    UICommonFun.BeginHorizontal(i % 2 == 1);

            //    var alias = aliases[i];

            //    //"NO."
            //    GUILayout.Label((i + 1).ToString(), GUILayout.Width(noWidth));

            //    EditorGUILayout.ObjectField(alias, typeof(GameObject), true, GUILayout.Width(width));

            //    EditorGUILayout.TextField(CommonFun.GameObjectToString(alias.gameObject));

            //    EditorGUI.BeginChangeCheck();
            //    var a = EditorGUILayout.DelayedTextField(alias.alias, GUILayout.Width(width));
            //    if (EditorGUI.EndChangeCheck())
            //    {
            //        alias.alias = a;
            //    }

            //    if (GUILayout.Button("重置", GUILayout.Width(btnWidth)))
            //    {
            //        CommonFun.FocusControl();
            //        alias.Reset();
            //    }

            //    UICommonFun.EndHorizontal();
            //}
        }
    }
}
