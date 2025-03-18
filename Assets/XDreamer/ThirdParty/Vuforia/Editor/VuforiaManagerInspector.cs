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
using XCSJ.PluginVuforia;

#if XDREAMER_VUFORIA
using Vuforia;
#endif

namespace XCSJ.EditorVuforia
{
    /// <summary>
    /// Vuforia管理器检查器
    /// </summary>
    [CustomEditor(typeof(VuforiaManager))]
    public class VuforiaManagerInspector : BaseManagerInspector<VuforiaManager>
    {
        #region 编译宏

        /// <summary>
        /// 宏
        /// </summary>
        public static readonly Macro XDREAMER_VUFORIA = new Macro(nameof(XDREAMER_VUFORIA), BuildTargetGroup.Standalone, BuildTargetGroup.Android, BuildTargetGroup.iOS, BuildTargetGroup.WSA);

        /// <summary>
        /// 初始化宏
        /// </summary>
        [Macro]
        public static void InitMacro()
        {
            //编辑器运行时不处理编译宏
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;

#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_ANDROID || UNITY_ISO || UNITY_WSA
            if (TypeHelper.Exists("Vuforia.VuforiaBehaviour")
                && UICommonFun.NaturalCompare(VuforiaHelper.VersionWeak, "10.7.2") >= 0)
            {
                XDREAMER_VUFORIA.DefineIfNoDefined();
            }
            else
#endif
            {
                XDREAMER_VUFORIA.UndefineWithSelectedBuildTargetGroup();
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
            XDreamerInspector.onCreatedManager += (t) =>
            {
                if (t == typeof(VuforiaManager))
                {
                    EditorHelper.ImportPackageIfNeedWithDialog(XDREAMER_VUFORIA, UnityPackagePath, true);
                }
            };

            EditorSceneManager.sceneOpened += (scene, mode) =>
            {
                UICommonFun.DelayCall(() =>
                {
                    if (VuforiaManager.instance)
                    {
                        EditorHelper.ImportPackageIfNeedWithDialog(XDREAMER_VUFORIA, UnityPackagePath, false);
                    }
                });
            };
        }

        private const string UnityPackagePath = "Assets/XDreamer-ThirdPartyUnityPackage/add-vuforia-package-10-8-4.unitypackage";

        #endregion

        private static CategoryList categoryList = null;

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            if (categoryList == null) categoryList = EditorToolsHelper.GetWithPurposes(nameof(VuforiaManager));
        }

        /// <summary>
        /// Vuforia许可证密钥
        /// </summary>
        [Name("Vuforia许可证密钥")]
        [Tip("使用XDreaemr申请的基础版本的Vuforia许可证密钥", "Use the Vuforia license key of the base version requested by XDreaemr")]
        public string VuforiaLicenseKey = "AcWmVPn/////AAABmbsoP5bpfkMIrB7a9oPI1uV2FP97vjI2tZWZTUDD9e87RvqgQbU4GW2y/y7H9whl4Ap3GwJeD4+0IAKpm6RTCj19xGfmXkpysKhlBIwSCgcJ0DyhTT6d0fBQJ+FOK3kibhofucUvDDkNiHsH/uIfYAJnE9TLumHuX0MWald0AD5phcfyuTtP7qOuqXpqy755Deml5tPOfxu5bXB0hJwmYRzHdIM5Jo0EMnpmUUMoveXhObyodMvIQHxV7B+giVQQYB0QkCQiP657P5ofV8E44Xao1aU8UMv1HJSpJp+oqpNWP9eD3+Y2Crjh4GcwLncylxkI3ZY4SdjfzvC125tlZYAxJGrDxoX0uvXZz9BhHo76";

        /// <summary>
        /// 当绘制检查器GUI
        /// </summary>
        [LanguageTuple("Vuforia Configuration", "Vuforia配置")]
        [LanguageTuple("Use WebCam", "使用网络摄像头")]
        [LanguageTuple("Please install (or update) to Unity's [{0}] package [10.7.2] (included) or later!", "请安装(或更新)到Unity的[{0}]包[10.7.2](含)或更高版本！")]
        [LanguageTuple("The [{1}] package based plug-in extension provided by {0} only supports [Unity2021.3.2] (included) or later versions!", "{0}提供的基于[{1}]包的插件扩展，仅支持[Unity2021.3.2](含)或更高版本中使用！")]
        public override void OnInspectorGUI()
        {
#if UNITY_2021_3_OR_NEWER

            #region 检测是否需要导入UnityPackage

            EditorHelper.ImportPackageIfNeedWithButton(XDREAMER_VUFORIA, UnityPackagePath);

            #endregion

#if !XDREAMER_VUFORIA
            UICommonFun.RichHelpBox("<color=red>" + string.Format(Tr("Please install (or update) to Unity's [{0}] package [10.7.2] (included) or later!"), VuforiaHelper.Title) + "</color>", MessageType.Warning);
#endif

#else
            UICommonFun.RichHelpBox("<color=red>" + string.Format(Tr("The [{1}] package based plug-in extension provided by {0} only supports [Unity2021.3.2] (included) or later versions!"), Product.Name, VuforiaHelper.Title) + "</color>", MessageType.Warning);

#endif

            base.OnInspectorGUI();

#if XDREAMER_VUFORIA

            if (string.IsNullOrEmpty(VuforiaConfiguration.Instance.Vuforia.LicenseKey) && GUILayout.Button(TrLabel(nameof(VuforiaLicenseKey))))
            {
                VuforiaConfiguration.Instance.XModifyProperty(() => { VuforiaConfiguration.Instance.Vuforia.LicenseKey = VuforiaLicenseKey; });
            }

            if (GUILayout.Button(Tr("Vuforia Configuration")))
            {
                Selection.activeObject = VuforiaConfiguration.Instance;
            }

            if (VuforiaConfiguration.Instance.PlayMode.PlayModeType != PlayModeType.WEBCAM 
                && GUILayout.Button(Tr("Use WebCam")))
            {
                VuforiaConfiguration.Instance.PlayMode.PlayModeType = PlayModeType.WEBCAM;
            }
#endif

            categoryList.DrawVertical();
        }
    }
}
