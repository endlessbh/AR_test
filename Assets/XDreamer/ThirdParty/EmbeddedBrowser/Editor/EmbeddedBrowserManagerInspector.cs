using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Base.CategoryViews;
using XCSJ.EditorExtension.Base;
using XCSJ.EditorTools;
using XCSJ.Helper;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginEmbeddedBrowser;

#if XDREAMER_EMBEDDED_BROWSER
using ZenFulcrum.EmbeddedBrowser;
#endif

namespace XCSJ.EditorEmbeddedBrowser
{
    /// <summary>
    /// 嵌入式浏览器管理器检查器
    /// </summary>
    [Name("嵌入式浏览器管理器检查器")]
    [CustomEditor(typeof(EmbeddedBrowserManager))]
    public class EmbeddedBrowserManagerInspector : BaseManagerInspector<EmbeddedBrowserManager>
    {
#region 编译宏

        /// <summary>
        /// 宏
        /// </summary>
        public static readonly Macro XDREAMER_EMBEDDED_BROWSER = new Macro(nameof(XDREAMER_EMBEDDED_BROWSER), BuildTargetGroup.Standalone, BuildTargetGroup.Android, BuildTargetGroup.iOS, BuildTargetGroup.WSA);

        /// <summary>
        /// 初始化宏
        /// </summary>
        [Macro]
        public static void InitMacro()
        {
            //编辑器运行时不处理编译宏
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;

#if UNITY_EDITOR || UNITY_STANDALONE
            if (TypeHelper.Exists("ZenFulcrum.EmbeddedBrowser.Browser"))
            {
                XDREAMER_EMBEDDED_BROWSER.DefineIfNoDefined();
            }
            else
#endif
            {
                XDREAMER_EMBEDDED_BROWSER.UndefineWithSelectedBuildTargetGroup();
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        [InitializeOnLoadMethod]
        public static void Init()
        {
            //编辑器运行时不处理
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;

            InitMacro();
            //XDreamerInspector.onCreatedManager += (t) =>
            //{
            //    if (t == typeof(EmbeddedBrowserManager))
            //    {
            //        EditorHelper.ImportPackageIfNeedWithDialog(XDREAMER_EMBEDDED_BROWSER, UnityPackagePath, true);
            //    }
            //};

            //EditorSceneManager.sceneOpened += (scene, mode) =>
            //{
            //    UICommonFun.DelayCall(() =>
            //    {
            //        if (EmbeddedBrowserManager.instance)
            //        {
            //            EditorHelper.ImportPackageIfNeedWithDialog(XDREAMER_EMBEDDED_BROWSER, UnityPackagePath, false);
            //        }
            //    });
            //};
        }

        private const string UnityPackagePath = "";

#endregion

        private static CategoryList categoryList = null;

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            if (categoryList == null) categoryList = EditorToolsHelper.GetWithPurposes(nameof(EmbeddedBrowserManager));
        }

        /// <summary>
        /// 当绘制检查器GUI
        /// </summary>
        [LanguageTuple("Please install (or update) [Embedded Browser] plug-in [3.1.0] (included) or higher!", "请安装(或更新)【嵌入式浏览器】插件[3.1.0](含)或更高版本！")]
        public override void OnInspectorGUI()
        {
#if !XDREAMER_EMBEDDED_BROWSER

            EditorHelper.ImportPackageIfNeedWithButton(XDREAMER_EMBEDDED_BROWSER, "Embedded Browser", () =>
            {
                Application.OpenURL("https://assetstore.unity.com/packages/tools/gui/embedded-browser-55459");
            });

            UICommonFun.RichHelpBox("<color=red>" + Tr("Please install (or update) [Embedded Browser] plug-in [3.1.0] (included) or higher!") + "</color>", MessageType.Warning);
            
#endif


            base.OnInspectorGUI();
            DrawDetailInfos();
            categoryList.DrawVertical();
        }

        [Name("浏览器")]
        [Tip("嵌入式浏览器对象；", "Embedded browser object")]
        public bool browser;

        /// <summary>
        /// 浏览器列表
        /// </summary>
        [Name("浏览器列表")]
        [Tip("当前场景中所有嵌入式浏览器对象", "All embedded browser objects in the current scene")]
        private static bool _display = true;

        private void DrawDetailInfos()
        {
            _display = UICommonFun.Foldout(_display, TrLabel(nameof(_display)));
            if (!_display) return;

            CommonFun.BeginLayout();

#region 标题            

            EditorGUILayout.BeginHorizontal(GUI.skin.box);

            GUILayout.Label("NO.", UICommonOption.Width32);
            GUILayout.Label(TrLabel(nameof(browser)));

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Separator();

#endregion

#if XDREAMER_EMBEDDED_BROWSER

            var cache = ComponentCache.Get(typeof(Browser), true);
            for (int i = 0; i < cache.components.Length; i++)
            {
                var component = cache.components[i] as Browser;

                UICommonFun.BeginHorizontal(i);

                //编号
                EditorGUILayout.LabelField((i + 1).ToString(), UICommonOption.Width32);

                //ART关联对象
                EditorGUILayout.ObjectField(component, component.GetType(), true);

                UICommonFun.EndHorizontal();
            }

#endif

            CommonFun.EndLayout();
        }
    }
}
