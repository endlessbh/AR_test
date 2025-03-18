using UnityEditor;
using UnityEngine;
using XCSJ.Collections;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension.Base;
using XCSJ.EditorExtension.Base.XUnityEditor;
using XCSJ.PluginXBox;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXBox.Tools;
using XCSJ.Languages;
using XCSJ.EditorExtension;
using XCSJ.Helper;

#if XDREAMER_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XInput;
#endif

namespace XCSJ.EditorXBox
{
    /// <summary>
    /// <see cref="XBoxManager"/>检查器
    /// </summary>
    [CustomEditor(typeof(XBoxManager))]
    [Name("XBox管理器检查器")]
    public class XBoxManagerInspector : BaseManagerInspector<XBoxManager>
    {
        #region 编译宏

        /// <summary>
        /// 输入系统XBOX的Windows宏
        /// </summary>
        public static readonly Macro XDREAMER_INPUT_SYSTEM_XBOX_WINDOWS = new Macro(nameof(XDREAMER_INPUT_SYSTEM_XBOX_WINDOWS), BuildTargetGroup.Standalone);

        /// <summary>
        /// 初始化宏
        /// </summary>
        [Macro]
        public static void InitMacro()
        {
            //编辑器运行时不处理编译宏
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;

#if UNITY_STANDALONE_WIN //仅允许在Windows平台中使用
            if (TypeHelper.ExistsAndAssemblyFileExists("UnityEngine.InputSystem.XInput.XInputControllerWindows"))
            {
                XDREAMER_INPUT_SYSTEM_XBOX_WINDOWS.DefineIfNoDefined();
            }
            else
#endif
            {
                XDREAMER_INPUT_SYSTEM_XBOX_WINDOWS.UndefineWithSelectedBuildTargetGroup();
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        [InitializeOnLoadMethod]
        public static void Init()
        {
            InitMacro();
        }

        #endregion

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
#if XDREAMER_INPUT_SYSTEM_XBOX_WINDOWS
            validXBoxDevice = XBoxHelper.HasXBox360();
            InputSystem.onDeviceChange += OnDeviceChange;
#else
            checkXBoxDevice = true;
            CheckDevice();
#endif
            validXBoxInput = XBoxManagerAssetGenerator.CheckAxisPresets();
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
#if XDREAMER_INPUT_SYSTEM_XBOX_WINDOWS
            InputSystem.onDeviceChange -= OnDeviceChange;
#else
            checkXBoxDevice = false;
#endif
        }

#if XDREAMER_INPUT_SYSTEM_XBOX_WINDOWS

        private void OnDeviceChange(InputDevice inputDevice, InputDeviceChange inputDeviceChange)
        {
            if (inputDevice is XInputControllerWindows)
            {
                validXBoxDevice = XBoxHelper.HasXBox360();
            }
        }

#else
        private void CheckDevice()
        {
            if (checkXBoxDevice)
            {
                validXBoxDevice = XBoxHelper.HasXBox360();
                if (lastValidXBoxDevice && !validXBoxDevice)
                {
                    Debug.LogWarning(Tr("XBox device disconnected!"));
                }
                else if (!lastValidXBoxDevice && validXBoxDevice)
                {
                    Debug.Log(Tr("XBox device connected!"));
                }
                lastValidXBoxDevice = validXBoxDevice;
                UICommonFun.DelayCall(CheckDevice, 5);
            }
        }
        private static bool checkXBoxDevice = false;
        private bool lastValidXBoxDevice = false;

#endif

        private bool validXBoxDevice = false;
        private bool validXBoxInput = false;

        /// <summary>
        /// 当绘制检查器GUI
        /// </summary>
        [LanguageTuple("XBox device disconnected!", "XBox设备连接已断开！")]
        [LanguageTuple("XBox device connected!", "XBox设备已连接！")]
        [LanguageTuple("No valid XBox device found!", "未找到有效的XBox设备！")]
        [LanguageTuple("The new input system is enabled! Xbox handle is no longer recommended to be configured to the input manager! It is recommended to remove the corresponding configuration!", "新版输入系统已启用！XBox手柄不再推荐配置到输入管理器！推荐移除对应配置！")]
        [LanguageTuple("Detect XBox Devices", "检测XBox设备")]
        [LanguageTuple("Detect XBox Settings In Input Manager", "检测输入管理器中XBox配置")]
        [LanguageTuple("Configure XBox Handle To Input Manager (* Not Recommended *)", "配置XBox手柄到输入管理器（*不推荐*）")]
        [LanguageTuple("Xbox handle is not configured to input manager!", "XBox手柄未配置到输入管理器！")]
        [LanguageTuple("Configure XBox Handle To Input Manager", "配置XBox手柄到输入管理器")]
        [LanguageTuple("Open [Input Manager] Project Settings", "打开[输入管理器]项目配置")]
        [LanguageTuple("Open [Input System Package] Project Settings", "打开[输入系统包]项目配置")]
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (!validXBoxDevice)
            {
                UICommonFun.RichHelpBox("<color=red>" + Tr("No valid XBox device found!") + "</color>", MessageType.Error);
            }
            if (GUILayout.Button(Tr("Detect XBox Devices")))
            {
                validXBoxDevice = XBoxHelper.HasXBox360();
                if (validXBoxDevice)
                {
                    Debug.Log(Tr("XBox device connected!"));
                }
                else
                {
                    Debug.LogError(Tr("No valid XBox device found!"));
                }
            }


#if ENABLE_LEGACY_INPUT_MANAGER || !XDREAMER_INPUT_SYSTEM

            if (validXBoxInput)
            {
#if XDREAMER_INPUT_SYSTEM
                UICommonFun.RichHelpBox(Tr("The new input system is enabled! Xbox handle is no longer recommended to be configured to the input manager! It is recommended to remove the corresponding configuration!"), MessageType.Warning);
#endif
                if (GUILayout.Button(Tr("Detect XBox Settings In Input Manager")))
                {
                    validXBoxInput = XBoxManagerAssetGenerator.CheckInputManagerAsset();
                }
            }
            else
            {
#if XDREAMER_INPUT_SYSTEM
                if (GUILayout.Button(Tr("Configure XBox Handle To Input Manager (* Not Recommended *)")))
#else
                UICommonFun.RichHelpBox("<color=red>" + Tr("Xbox handle is not configured to input manager!") + "</color>", MessageType.Error);
                if (GUILayout.Button(Tr("Configure XBox Handle To Input Manager")))
#endif
                {
                    validXBoxInput = XBoxManagerAssetGenerator.GenerateInputManagerAsset();
                }
            }

#endif

            if (GUILayout.Button(Tr("Open [Input Manager] Project Settings")))
            {
#if UNITY_2019_1_OR_NEWER
                EditorHelper.OpenProjectSettingsWindow("Input Manager");
#else
                EditorHelper.OpenProjectSettingsWindow("Input");
#endif
            }

#if UNITY_2019_1_OR_NEWER && XDREAMER_INPUT_SYSTEM
            if (GUILayout.Button(Tr("Open [Input System Package] Project Settings")))
            {
                EditorHelper.OpenProjectSettingsWindow("Input System Package");
            }
#endif

            DrawXBoxDetailInfos();
        }

        /// <summary>
        /// XBox功能组件列表
        /// </summary>
        [Name("XBox功能组件列表")]
        [Tip("当前场景中所有使用XBox功能的组件对象", "All component objects using Xbox function in the current scene")]
        private static bool _displayXBoxs = true;

        [Name("XBox功能组件")]
        [Tip("XBox功能组件游戏对象；本项只读；", "XBox function component game object; This item is read-only;")]
        public bool XBoxFunctionalComponents;

        private void DrawXBoxDetailInfos()
        {
            EditorGUILayout.Separator();
            _displayXBoxs = UICommonFun.Foldout(_displayXBoxs, CommonFun.NameTip(GetType(), nameof(_displayXBoxs)));
            if (!_displayXBoxs) return;

            CommonFun.BeginLayout();

            #region 标题            

            EditorGUILayout.BeginHorizontal(GUI.skin.box);

            GUILayout.Label("NO.", UICommonOption.Width32);
            GUILayout.Label(TrLabel(nameof(XBoxFunctionalComponents)));

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Separator();

            #endregion

            var cache = ComponentCache.Get(typeof(IXBox), true);
            for (int i = 0; i < cache.components.Length; i++)
            {
                var component = cache.components[i];

                UICommonFun.BeginHorizontal(i);

                //编号
                EditorGUILayout.LabelField((i + 1).ToString(), UICommonOption.Width32);

                //XBox功能组件
                EditorGUILayout.ObjectField(component, component.GetType(), true);

                UICommonFun.EndHorizontal();
            }

            CommonFun.EndLayout();
        }
    }
}
