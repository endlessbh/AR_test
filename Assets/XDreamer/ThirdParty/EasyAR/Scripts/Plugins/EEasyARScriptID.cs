using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension;
using XCSJ.Scripts;
#if XDREAMER_EASYAR_4_0_1 || XDREAMER_EASYAR_3_0_1
using easyar;
#elif XDREAMER_EASYAR_2_3_0
using EasyAR;
#endif

namespace XCSJ.PluginEasyAR
{
    public static class EasyARIDRange
    {
        public const int Begin = (int)EExtensionID._0x2;//33024
        public const int End = (int)EExtensionID._0x3 - 1;

        public const int Fragment = 0x18;//24

        public const int Common = Begin + Fragment * 0;//33024
        public const int MonoBehaviour = Begin + Fragment * 1;//33048
        public const int StateLib = Begin + Fragment * 2;//33072
        public const int Tools = Begin + Fragment * 3;//33096
        public const int Editor = Begin + Fragment * 4;//33120
    }

    /// <summary>
    /// EasyAR脚本ID
    /// </summary>
    [Name("EasyAR脚本ID")]
    public enum EEasyARScriptID
    {
        _Begin = EasyARIDRange.Begin,

        #region EasyAR-目录
        /// <summary>
        /// EasyAR<br />
        /// </summary>
        [ScriptName(nameof(EasyAR), nameof(EasyAR), EGrammarType.Category)]
        #endregion
        EasyAR,

        [ScriptName("EasyAR相机设备打开并启动", nameof(EasyARCameraDeviceOpenAndStart))]
        [ScriptDescription("EasyAR相机设备打开并启动，即打开当前硬件上的相机（会进行权限检查）并开启捕获功能；")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(0, EParamType.GameObjectComponent, "相机设备(为空时使用zSpace管理器中已设定的):",
#if XDREAMER_EASYAR_4_0_1
            typeof(VideoCameraDevice))]
#elif XDREAMER_EASYAR_3_0_1
            typeof(ExtendARSession))]
#elif XDREAMER_EASYAR_2_3_0
            typeof(CameraDeviceBaseBehaviour))]
#else
            typeof(Component))]
#endif
        EasyARCameraDeviceOpenAndStart,

        [ScriptName("EasyAR相机设备关闭", nameof(EasyARCameraDeviceClose))]
        [ScriptDescription("EasyAR相机设备关闭，即停止捕获并关闭设备；")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(0, EParamType.GameObjectComponent, "相机设备(为空时使用zSpace管理器中已设定的):",
#if XDREAMER_EASYAR_4_0_1
            typeof(VideoCameraDevice))]
#elif XDREAMER_EASYAR_3_0_1
            typeof(ExtendARSession))]
#elif XDREAMER_EASYAR_2_3_0
            typeof(CameraDeviceBaseBehaviour))]
#else
            typeof(Component))]
#endif
        EasyARCameraDeviceClose,

        [ScriptName("EasyAR相机设备开始捕获", nameof(EasyARCameraDeviceStartCapture))]
        [ScriptDescription("EasyAR相机设备开始捕获，即开始进行图像捕获与流分析；开始捕获前必须保证EasyAR相机设备打开并已经启动；")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(0, EParamType.GameObjectComponent, "相机设备(为空时使用zSpace管理器中已设定的):",
#if XDREAMER_EASYAR_4_0_1
            typeof(ARSession))]
#elif XDREAMER_EASYAR_3_0_1
            typeof(ExtendARSession))]
#elif XDREAMER_EASYAR_2_3_0
            typeof(CameraDeviceBaseBehaviour))]
#else
            typeof(Component))]
#endif
        EasyARCameraDeviceStartCapture,

        [ScriptName("EasyAR相机设备停止捕获", nameof(EasyARCameraDeviceStopCapture))]
        [ScriptDescription("EasyAR相机设备停止捕获，即停止进行图像捕获与流分析；")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(0, EParamType.GameObjectComponent, "相机设备(为空时使用zSpace管理器中已设定的):",
#if XDREAMER_EASYAR_4_0_1
            typeof(ARSession))]
#elif XDREAMER_EASYAR_3_0_1
            typeof(ExtendARSession))]
#elif XDREAMER_EASYAR_2_3_0
            typeof(CameraDeviceBaseBehaviour))]
#else
            typeof(Component))]
#endif
        EasyARCameraDeviceStopCapture,

#region EasyAR切换相机设备类型
        [ScriptName("EasyAR切换相机设备类型", nameof(EasyARSwitchCameraDeviceType))]
        [ScriptDescription("EasyAR切换相机设备类型；切换时会先关闭，然后再打开并启动设备；如果切换前后摄像头类型相同,则不执行切换操作；")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(0, EParamType.GameObjectComponent, "相机设备(为空时使用zSpace管理器中已设定的):",
#if XDREAMER_EASYAR_4_0_1
            typeof(VideoCameraDevice))]
#elif XDREAMER_EASYAR_3_0_1
            typeof(ExtendARSession))]
#elif XDREAMER_EASYAR_2_3_0
            typeof(CameraDeviceBaseBehaviour))]
#else
            typeof(Component))]
#endif
        [ScriptParams(1, EParamType.Combo, "相机设备(摄像头)类型:", "默认", "前置摄像头", "后置摄像头", "切换")]
#endregion EasyAR切换相机设备类型
        EasyARSwitchCameraDeviceType,

        /// <summary>
        /// 当前已使用的脚本最大ID
        /// </summary>
        MaxCurrent,
    }
}

