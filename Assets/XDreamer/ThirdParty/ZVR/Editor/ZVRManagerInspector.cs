using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension.Base;
using XCSJ.Helper;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginZVR;
using XCSJ.PluginZVR.Base;
using XCSJ.PluginZVR.Tools;

namespace XCSJ.EditorZVR
{
    /// <summary>
    /// ZVR管理器检查器
    /// </summary>
    [Name("ZVR管理器检查器")]
    [CustomEditor(typeof(ZVRManager))]
    public class ZVRManagerInspector : BaseManagerInspector<ZVRManager>
    {
        #region 编译宏

        /// <summary>
        /// 宏
        /// </summary>
        public static readonly Macro XDREAMER_ZVR = new Macro(nameof(XDREAMER_ZVR), BuildTargetGroup.Standalone);

        /// <summary>
        /// 初始化宏
        /// </summary>
        [Macro]
        public static void InitMacro()
        {
            //编辑器运行时不处理编译宏
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;

#if UNITY_EDITOR || UNITY_STANDALONE
            if (TypeHelper.Exists("ActiveCenter.GokuLib.GokuClient"))
            {
                XDREAMER_ZVR.DefineIfNoDefined();
            }
            else
#endif
            {
                XDREAMER_ZVR.UndefineWithSelectedBuildTargetGroup();
            }
        }

        #endregion

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
                if (t == typeof(ZVRManager))
                {
                    EditorHelper.ImportPackageIfNeedWithDialog(XDREAMER_ZVR, UnityPackagePath, true);
                }
            };

            EditorSceneManager.sceneOpened += (scene, mode) =>
            {
                UICommonFun.DelayCall(() =>
                {
                    if (ZVRManager.instance)
                    {
                        EditorHelper.ImportPackageIfNeedWithDialog(XDREAMER_ZVR, UnityPackagePath, false);
                    }
                });
            };
        }

        private const string UnityPackagePath = "Assets/XDreamer-ThirdPartyUnityPackage/zvrgoku_unity_plugin_v1.4.4.28.unitypackage";

        /// <summary>
        /// 启用时
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
#if XDREAMER_ZVR
            var manager = this.manager;
            if (manager && manager.hasAccess)
            {
                manager.XGetOrAddComponent<ZvrGokuStreamClient>();
            }
#endif
        }

        /// <summary>
        /// 当绘制检查器GUI
        /// </summary>
        [LanguageTuple("Create [ZVR Goku Stream Client]", "创建[ZVRGoku流客户端]")]
        [LanguageTuple("The current project has not imported ZVR plug-in package!", "当前工程未导入ZVR插件包！")]
        public override void OnInspectorGUI()
        {
            #region 检测是否需要导入UnityPackage

            EditorHelper.ImportPackageIfNeedWithButton(XDREAMER_ZVR, UnityPackagePath);

            #endregion
            base.OnInspectorGUI();

#if XDREAMER_ZVR

            if (GUILayout.Button(Tr("Create [ZVR Goku Stream Client]")))
            {
                manager.XGetOrAddComponent<ZvrGokuStreamClient>();
            }
#else
            UICommonFun.RichHelpBox(Tr("The current project has not imported ZVR plug-in package!"), MessageType.Warning);
#endif

            DrawDetailInfos();
        }

        /// <summary>
        /// ZVR刚体关联列表
        /// </summary>
        [Name("ZVR刚体关联列表")]
        [Tip("当前场景中所有与ZVR刚体关联的对象", "All objects associated with ZVR rigid bodies in the current scene")]
        private static bool _display = true;

        [Name("ZVR关联对象")]
        [Tip("ZVR刚体关联的组件；本项只读；", "Components associated with ZVR rigid body; This item is read-only;")]
        public bool ZVRLinkObject;

        [Name("关联对象拥有者")]
        [Tip("ZVR刚体关联对象拥有者所在的游戏对象；本项只读；", "The game object of the owner of the ZVR rigid body association object; This item is read-only;")]
        public bool linkObjectOnwer;

        [Name("激活启用")]
        [Tip("ZVR刚体关联对象是否处于激活并启用的状态；本项只读；", "Whether the ZVR rigid body association object is activated and enabled; This item is read-only;")]
        public bool activateEnable;

        [Name("刚体ID")]
        [Tip("用于与ZVRGoku软件进行数据流通信的刚体ID；", "Rigid body ID for data flow communication with ZVR Goku software;")]
        public bool rigidbodyID;

        private void DrawDetailInfos()
        {
            _display = UICommonFun.Foldout(_display, CommonFun.NameTip(GetType(), nameof(_display)));
            if (!_display) return;

            CommonFun.BeginLayout();

            #region 标题            

            EditorGUILayout.BeginHorizontal(GUI.skin.box);

            GUILayout.Label("NO.", UICommonOption.Width32);
            GUILayout.Label(TrLabel(nameof(ZVRLinkObject)));
            GUILayout.Label(TrLabel(nameof(linkObjectOnwer)));
            GUILayout.Label(TrLabel(nameof(activateEnable)), UICommonOption.Width60);
            GUILayout.Label(TrLabel(nameof(rigidbodyID)), UICommonOption.Width60);

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Separator();

            #endregion

            var cache = ComponentCache.Get(typeof(IZVRObject), true);
            for (int i = 0; i < cache.components.Length; i++)
            {
                var component = cache.components[i] as MonoBehaviour;
                var link = component as IZVRObject;

                UICommonFun.BeginHorizontal(i);

                //编号
                EditorGUILayout.LabelField((i + 1).ToString(), UICommonOption.Width32);

                //ZVR关联对象
                var gameObject = component.gameObject;
                EditorGUILayout.ObjectField(gameObject, typeof(GameObject), true);

                //ZVR关联对象
                var owner = component.GetZVRObjectOwnerGameObject();
                EditorGUILayout.ObjectField(owner, typeof(GameObject), true);

                //激活启用
                EditorGUILayout.Toggle(component.isActiveAndEnabled, UICommonOption.Width60);

                //刚体ID
                EditorGUI.BeginChangeCheck();
                var rigidBodyID = EditorGUILayout.DelayedIntField(link.rigidBodyID, UICommonOption.Width60);
                if (EditorGUI.EndChangeCheck())
                {
                    link.rigidBodyID = rigidBodyID;
                }

                UICommonFun.EndHorizontal();
            }

            CommonFun.EndLayout();
        }
    }
}
