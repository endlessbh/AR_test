using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.XR;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension.Base;
using XCSJ.EditorExtension.Base.XUnityEditor.PackageManager;
using XCSJ.Helper;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXXR.Interaction.Toolkit;
using XCSJ.EditorXRSpaceSolution;

#if XDREAMER_XR_INTERACTION_TOOLKIT
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.UI;
#endif

#if XDREAMER_XR_INTERACTION_TOOLKIT_2_0_0_OR_NEWER
using Unity.XR.CoreUtils;
#endif

namespace XCSJ.EditorXXR.Interaction.Toolkit
{
    /// <summary>
    /// XR交互工具包检查器
    /// </summary>
    [Name("XR交互工具包检查器")]
    [CustomEditor(typeof(XXRInteractionToolkitManager))]
    public class XXRInteractionToolkitManagerInspector : BaseManagerInspector<XXRInteractionToolkitManager>
    {
        /// <summary>
        /// 需要的所有依赖包;需要调用包管理器
        /// </summary>
        public const string PackageName = "com.unity.xr.interaction.toolkit";

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
                if (t == typeof(XXRInteractionToolkitManager))
                {
                    EditorHelper.OpenPackageManagerIfNeedWithDialog(XDREAMER_XR_INTERACTION_TOOLKIT, PackageName, true);
                }
            };

            EditorSceneManager.sceneOpened += (scene, mode) =>
            {
                UICommonFun.DelayCall(() =>
                {
                    if (XXRInteractionToolkitManager.instance)
                    {
                        EditorHelper.OpenPackageManagerIfNeedWithDialog(XDREAMER_XR_INTERACTION_TOOLKIT, PackageName, false);
                    }
                });
            };
        }

        #region 编译宏

        /// <summary>
        /// 宏
        /// </summary>
        public static readonly Macro XDREAMER_XR_INTERACTION_TOOLKIT = new Macro(nameof(XDREAMER_XR_INTERACTION_TOOLKIT), BuildTargetGroup.Standalone, BuildTargetGroup.Android, BuildTargetGroup.iOS, BuildTargetGroup.WSA);

        /// <summary>
        /// 2.0.0版本宏
        /// </summary>
        public static readonly Macro XDREAMER_XR_INTERACTION_TOOLKIT_2_0_0_OR_NEWER = new Macro(nameof(XDREAMER_XR_INTERACTION_TOOLKIT_2_0_0_OR_NEWER), BuildTargetGroup.Standalone, BuildTargetGroup.Android, BuildTargetGroup.iOS, BuildTargetGroup.WSA);

        /// <summary>
        /// 2.0.4版本宏
        /// </summary>
        public static readonly Macro XDREAMER_XR_INTERACTION_TOOLKIT_2_0_4_OR_NEWER = new Macro(nameof(XDREAMER_XR_INTERACTION_TOOLKIT_2_0_4_OR_NEWER), BuildTargetGroup.Standalone, BuildTargetGroup.Android, BuildTargetGroup.iOS, BuildTargetGroup.WSA);

        /// <summary>
        /// 2.1.1版本宏
        /// </summary>
        public static readonly Macro XDREAMER_XR_INTERACTION_TOOLKIT_2_1_1_OR_NEWER = new Macro(nameof(XDREAMER_XR_INTERACTION_TOOLKIT_2_1_1_OR_NEWER), BuildTargetGroup.Standalone, BuildTargetGroup.Android, BuildTargetGroup.iOS, BuildTargetGroup.WSA);

        /// <summary>
        /// 初始化宏
        /// </summary>
        [Macro]
        public static void InitMacro()
        {
            //编辑器运行时不处理编译宏
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;

#if UNITY_2019_4_OR_NEWER
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA
            if (TypeHelper.Exists("UnityEngine.XR.Interaction.Toolkit.XRInteractionManager")
                && PackageInfo_LinkType.GetPackageInfoByPackageName(PackageName) is UnityEditor.PackageManager.PackageInfo packageInfo)
            {
                //宏-最低支持版本宏
                if(UICommonFun.NaturalCompare(packageInfo.version, "1.0.0") >= 0)
                {
                    XDREAMER_XR_INTERACTION_TOOLKIT.DefineIfNoDefined();
                }
                else
                {
                    XDREAMER_XR_INTERACTION_TOOLKIT.UndefineWithSelectedBuildTargetGroup();
                }

                //2.0.0版本宏
                if (UICommonFun.NaturalCompare(packageInfo.version, "2.0.0") >= 0)
                {
                    XDREAMER_XR_INTERACTION_TOOLKIT_2_0_0_OR_NEWER.DefineIfNoDefined();
                }
                else
                {
                    XDREAMER_XR_INTERACTION_TOOLKIT_2_0_0_OR_NEWER.UndefineWithSelectedBuildTargetGroup();
                }

                //2.0.4版本宏
                if (UICommonFun.NaturalCompare(packageInfo.version, "2.0.4") >= 0)
                {
                    XDREAMER_XR_INTERACTION_TOOLKIT_2_0_4_OR_NEWER.DefineIfNoDefined();
                }
                else
                {
                    XDREAMER_XR_INTERACTION_TOOLKIT_2_0_4_OR_NEWER.UndefineWithSelectedBuildTargetGroup();
                }

                //2.1.1版本宏
                if (UICommonFun.NaturalCompare(packageInfo.version, "2.1.1") >= 0)
                {
                    XDREAMER_XR_INTERACTION_TOOLKIT_2_1_1_OR_NEWER.DefineIfNoDefined();
                }
                else
                {
                    XDREAMER_XR_INTERACTION_TOOLKIT_2_1_1_OR_NEWER.UndefineWithSelectedBuildTargetGroup();
                }
            }
            else
#endif //平台宏
#endif //UNITY_2019_4_OR_NEWER
            {
                XDREAMER_XR_INTERACTION_TOOLKIT.UndefineWithSelectedBuildTargetGroup();
                XDREAMER_XR_INTERACTION_TOOLKIT_2_0_0_OR_NEWER.UndefineWithSelectedBuildTargetGroup();
                XDREAMER_XR_INTERACTION_TOOLKIT_2_0_4_OR_NEWER.UndefineWithSelectedBuildTargetGroup();
                XDREAMER_XR_INTERACTION_TOOLKIT_2_1_1_OR_NEWER.UndefineWithSelectedBuildTargetGroup();
            }
        }

        #endregion

        /// <summary>
        /// 当绘制脚本
        /// </summary>
        /// <param name="serializedProperty"></param>
        protected override void OnDrawScript(SerializedProperty serializedProperty)
        {
            base.OnDrawScript(serializedProperty);
            InstallXRInteractionToolkitPackage();
        }

        /// <summary>
        /// 最佳版本
        /// </summary>
        public const string BestVersion = "2.1.1";

        /// <summary>
        /// 检测是否需要安装XR打开包管理器
        /// </summary>
        [LanguageTuple("At present, the best version of [ XR Interactive Toolkit ] that supports adaptation is: ", "目前已支持适配的【XR交互工具包】最佳版本为：{0}")]
        [LanguageTuple("Please install (or update) to unity's [{0}] package [1.0.0] (included) or later!", "请安装(或更新)到Unity的[{0}]包[1.0.0](含)或更高版本！")]
        [LanguageTuple("The [{1}] package based plug-in extension provided by {0} only supports [unity2019.4.0] (included) or later versions!", "{0}提供的基于[{1}]包的插件扩展，仅支持[Unity2019.4.0](含)或更高版本中使用！")]
        public static void InstallXRInteractionToolkitPackage()
        {
#if UNITY_2019_4_OR_NEWER
            EditorHelper.OpenPackageManagerIfNeedWithButton(XDREAMER_XR_INTERACTION_TOOLKIT, PackageName);

#if !XDREAMER_XR_INTERACTION_TOOLKIT
            UICommonFun.RichHelpBox("<color=red>" + string.Format("Please install (or update) to unity's [{0}] package [1.0.0] (included) or later!".Tr(typeof(XXRInteractionToolkitManagerInspector)), typeof(XXRInteractionToolkitManager).Tr()) + "</color>", MessageType.Warning);
#else

#if !XDREAMER_XR_INTERACTION_TOOLKIT_2_1_1_OR_NEWER //目前已支持适配的最佳版本
            UICommonFun.RichHelpBox("<color=red>" + string.Format("At present, the best version of [ XR Interactive Toolkit ] that supports adaptation is: {0}".Tr(typeof(XXRInteractionToolkitManagerInspector)), BestVersion) + "</color>", MessageType.Warning);
            EditorHelper.OpenPackageManagerIfNeedWithButton(XDREAMER_XR_INTERACTION_TOOLKIT_2_1_1_OR_NEWER, PackageName);
#endif //!XDREAMER_XR_INTERACTION_TOOLKIT_2_1_1_OR_NEWER


#endif //!XDREAMER_XR_INTERACTION_TOOLKIT
#else
            UICommonFun.RichHelpBox("<color=red>" + string.Format("The [{1}] package based plug-in extension provided by {0} only supports [unity2019.4.0] (included) or later versions!".Tr(typeof(XXRInteractionToolkitManagerInspector)), Product.Name, typeof(XXRInteractionToolkitManager).Tr()) + "</color>", MessageType.Warning);
#endif
        }

        /// <summary>
        /// 当绘制检查器GUI
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorXRITHelper.DrawOpenXRInteractionDebugger();
            EditorXRSpaceSolutionHelper.DrawSelectManager();

#if XDREAMER_XR_INTERACTION_TOOLKIT_2_0_0_OR_NEWER
            DrawXROriginDetailInfos();
#else
            DrawXRRigDetailInfos();
#endif
            DrawTrackedDeviceGraphicRaycasterDetailInfos();
            DrawTeleportationDetailInfos();
        }

#if XDREAMER_XR_INTERACTION_TOOLKIT_2_0_0_OR_NEWER

        /// <summary>
        /// XR原点列表
        /// </summary>
        [Name("XR原点列表")]
        [Tip("当前场景中所有的XR原点对象", "All XR origin objects in the current scene")]
        private static bool _displayXROrigins = true;        

        /// <summary>
        /// XR原点对象
        /// </summary>
        [Name("XR原点对象")]
        [Tip("XR原点备所在的游戏对象；本项只读；", "The game object where XR origin is located; This item is read-only;")]
        public bool xrOrigin;

        [Name("跟踪原点模式")]
        [Tip("跟踪原点模式", "Tracking origin mode")]
        public bool trackingOriginMode;
        
        private void DrawXROriginDetailInfos()
        {
            _displayXROrigins = UICommonFun.Foldout(_displayXROrigins, CommonFun.NameTip(GetType(), nameof(_displayXROrigins)));
            if (!_displayXROrigins) return;

            CommonFun.BeginLayout();

        #region 标题            

            EditorGUILayout.BeginHorizontal(GUI.skin.box);

            GUILayout.Label("NO.", UICommonOption.Width32);
            GUILayout.Label(TrLabel(nameof(xrOrigin)));
            GUILayout.Label(TrLabel(nameof(trackingOriginMode)), UICommonOption.Width120);

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Separator();

        #endregion

#if XDREAMER_XR_INTERACTION_TOOLKIT

            var cache = ComponentCache.Get(typeof(XROrigin), true);
            for (int i = 0; i < cache.components.Length; i++)
            {
                var component = cache.components[i] as XROrigin;

                UICommonFun.BeginHorizontal(i);

                //编号
                EditorGUILayout.LabelField((i + 1).ToString(), UICommonOption.Width32);

                //XR装备对象
                var gameObject = component.gameObject;
                EditorGUILayout.ObjectField(gameObject, typeof(GameObject), true);

                //跟踪原点模式标志
                EditorGUI.BeginChangeCheck();
                var trackingOriginMode = EditorGUILayout.EnumPopup(component.RequestedTrackingOriginMode, UICommonOption.Width120);
                if (EditorGUI.EndChangeCheck())
                {
                    component.XModifyProperty(() => component.RequestedTrackingOriginMode = (XROrigin.TrackingOriginMode)trackingOriginMode);
                }

                UICommonFun.EndHorizontal();
            }

#endif

            CommonFun.EndLayout();
        }

#else

        /// <summary>
        /// XR装备列表
        /// </summary>
        [Name("XR装备列表")]
        [Tip("当前场景中所有的XR装备对象", "All XR origin objects in the current scene")]
        private static bool _displayXRRigs = true;

        /// <summary>
        /// XR原点对象
        /// </summary>
        [Name("XR装备对象")]
        [Tip("XR装备所在的游戏对象；本项只读；", "Game object of XR equipment; This item is read-only;")]
        public bool xrRig;

        [Name("跟踪原点模式")]
        [Tip("跟踪原点模式", "Tracking origin mode")]
        public bool trackingOriginMode;

        private void DrawXRRigDetailInfos()
        {
            _displayXRRigs = UICommonFun.Foldout(_displayXRRigs, CommonFun.NameTip(GetType(), nameof(_displayXRRigs)));
            if (!_displayXRRigs) return;

            CommonFun.BeginLayout();

#region 标题            

            EditorGUILayout.BeginHorizontal(GUI.skin.box);

            GUILayout.Label("NO.", UICommonOption.Width32);
            GUILayout.Label(TrLabel(nameof(xrRig)));
            GUILayout.Label(TrLabel(nameof(trackingOriginMode)), UICommonOption.Width120);

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Separator();

#endregion

#if XDREAMER_XR_INTERACTION_TOOLKIT

            var cache = ComponentCache.Get(typeof(XRRig), true);
            for (int i = 0; i < cache.components.Length; i++)
            {
                var component = cache.components[i] as XRRig;

                UICommonFun.BeginHorizontal(i);

                //编号
                EditorGUILayout.LabelField((i + 1).ToString(), UICommonOption.Width32);

                //XR装备对象
                var gameObject = component.gameObject;
                EditorGUILayout.ObjectField(gameObject, typeof(GameObject), true);

                //跟踪原点模式标志
                EditorGUI.BeginChangeCheck();
                var trackingOriginMode = EditorGUILayout.EnumPopup(component.requestedTrackingOriginMode, UICommonOption.Width120);
                if (EditorGUI.EndChangeCheck())
                {
                    component.XModifyProperty(() => component.requestedTrackingOriginMode = (UnityEngine.XR.Interaction.Toolkit.XRRig.TrackingOriginMode)trackingOriginMode);
                }

                UICommonFun.EndHorizontal();
            }

#endif

            CommonFun.EndLayout();
        }

#endif

        /// <summary>
        /// 跟踪设备图形射线检测器列表
        /// </summary>
        [Name("跟踪设备图形射线检测器列表")]
        [Tip("当前场景中所有的跟踪设备图形射线检测器对象", "All tracking devices and graphic ray detector objects in the current scene")]
        private static bool _displayTrackedDeviceGraphicRaycasters = false;

        [Name("射线检测器")]
        [Tip("跟踪设备图形射线检测器所在的游戏对象；本项只读；", "Tracking the game object where the graphic ray detector of the equipment is located; This item is read-only;")]
        public bool rayDetector;

        [Name("事件相机")]
        [Tip("跟踪设备图形射线检测器所在的游戏对象上的画布在世界空间绘制模式时的事件相机；", "An event camera that tracks the canvas on the game object where the graphics ray detector of the device is located in the world space drawing mode;")]
        public bool eventCamera;

        [Name("渲染模式")]
        [Tip("跟踪设备图形射线检测器所在的游戏对象上的画布的渲染模式；", "The rendering mode of the canvas on the game object where the graphics ray detector of the tracking device is located;")]
        public bool renderMode;

        private void DrawTrackedDeviceGraphicRaycasterDetailInfos()
        {
            _displayTrackedDeviceGraphicRaycasters = UICommonFun.Foldout(_displayTrackedDeviceGraphicRaycasters, CommonFun.NameTip(GetType(), nameof(_displayTrackedDeviceGraphicRaycasters)));
            if (!_displayTrackedDeviceGraphicRaycasters) return;

            CommonFun.BeginLayout();

#region 标题            

            EditorGUILayout.BeginHorizontal(GUI.skin.box);

            GUILayout.Label("NO.", UICommonOption.Width32);
            GUILayout.Label(TrLabel(nameof(rayDetector)));
            GUILayout.Label(TrLabel(nameof(eventCamera)));
            GUILayout.Label(TrLabel(nameof(renderMode)), UICommonOption.Width120);

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Separator();

#endregion

#if XDREAMER_XR_INTERACTION_TOOLKIT

            var cache = ComponentCache.Get(typeof(TrackedDeviceGraphicRaycaster), true);
            for (int i = 0; i < cache.components.Length; i++)
            {
                var component = cache.components[i] as TrackedDeviceGraphicRaycaster;

                UICommonFun.BeginHorizontal(i);

                //编号
                EditorGUILayout.LabelField((i + 1).ToString(), UICommonOption.Width32);

                //射线检测器
                var gameObject = component.gameObject;
                EditorGUILayout.ObjectField(gameObject, typeof(GameObject), true);

                //事件相机
                var canvas = component.GetComponent<Canvas>();
                EditorGUI.BeginChangeCheck();
                var worldCamera = EditorGUILayout.ObjectField(canvas.worldCamera, typeof(Camera), true);
                if (EditorGUI.EndChangeCheck())
                {
                    canvas.XModifyProperty(() => canvas.worldCamera = worldCamera as Camera);
                }

                //绘制模式
                EditorGUI.BeginChangeCheck();
                var renderMode = EditorGUILayout.EnumPopup(canvas.renderMode, UICommonOption.Width120);
                if (EditorGUI.EndChangeCheck())
                {
                    canvas.XModifyProperty(() => canvas.renderMode = (RenderMode)renderMode);
                }

                UICommonFun.EndHorizontal();
            }
#endif

            CommonFun.EndLayout();
        }

        /// <summary>
        /// 传送锚点与区域列表
        /// </summary>
        [Name("传送锚点与区域列表")]
        [Tip("当前场景中所有的传送锚点与区域对象", "All transfer anchors and area objects in the current scene")]
        private static bool _displayTeleportations = false;

        /// <summary>
        /// 传送对象
        /// </summary>
        [Name("传送对象")]
        [Tip("传送锚点与区域组件对象；本项只读；", "Transfer anchor point and area component object; This item is read-only;")]
        public bool teleportationObject;

        private void DrawTeleportationDetailInfos()
        {
            _displayTeleportations = UICommonFun.Foldout(_displayTeleportations, CommonFun.NameTip(GetType(), nameof(_displayTeleportations)));
            if (!_displayTeleportations) return;

            CommonFun.BeginLayout();

#region 标题            

            EditorGUILayout.BeginHorizontal(GUI.skin.box);

            GUILayout.Label("NO.", UICommonOption.Width32);
            GUILayout.Label(TrLabel(nameof(teleportationObject)));

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Separator();

#endregion

#if XDREAMER_XR_INTERACTION_TOOLKIT

            var cache = ComponentCache.Get(typeof(BaseTeleportationInteractable), true);
            for (int i = 0; i < cache.components.Length; i++)
            {
                var component = cache.components[i] as BaseTeleportationInteractable;

                UICommonFun.BeginHorizontal(i);

                //编号
                EditorGUILayout.LabelField((i + 1).ToString(), UICommonOption.Width32);

                //传送对象
                EditorGUILayout.ObjectField(component, component.GetType(), true);

                UICommonFun.EndHorizontal();
            }
#endif

            CommonFun.EndLayout();
        }
    }
}
