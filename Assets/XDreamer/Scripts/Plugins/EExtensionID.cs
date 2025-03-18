using System;
using XCSJ.Attributes;

namespace XCSJ.Extension
{
    /// <summary>
    /// ID区间
    /// </summary>
    public static class IDRange
    {
        /// <summary>
        /// 开始32768
        /// </summary>
        public const int Begin = 0x8000;

        /// <summary>
        /// 结束98303
        /// </summary>
        public const int End = 0x17fff;

        /// <summary>
        /// 片段：扩展脚本的ID片段数目，每个片段可有128个ID；[0x8000, 0x17fff] = 65536 = 4096 * 16  = 0x80 * 0x200,即可有0x200 = 512个片段;
        /// </summary>
        public const int Fragment = 0x80;//128
    }

    /// <summary>
    /// 扩展ID
    /// </summary>
    [Name("扩展ID")]
    public enum EExtensionID
    {
        /// <summary>
        /// 开始32768
        /// </summary>
        _Begin = IDRange.Begin,

        /// <summary>
        /// <see cref="CommonUIManager"/> 占用, 32768
        /// </summary>
        _0x0 = _Begin + IDRange.Fragment * 0x0,

        /// <summary>
        /// <see cref="OSInteractManager"/> 占用, 32896
        /// </summary>
        _0x1 = _Begin + IDRange.Fragment * 0x1,

        /// <summary>
        /// <see cref="PluginEasyAR.EasyARManager"/> 占用 (即 EasyAR ), 33024
        /// </summary>
        _0x2 = _Begin + IDRange.Fragment * 0x2,

        /// <summary>
        /// <see cref="PluginGoogleVR.GvrManager"/> 占用 (即 GoogleVR ), 33152
        /// </summary>
        _0x3 = _Begin + IDRange.Fragment * 0x3,

        _0x4 = _Begin + IDRange.Fragment * 0x4,//33280

        /// <summary>
        /// <see cref="TemporaryObjectManager"/> 占用, 33408
        /// </summary>
        _0x5 = _Begin + IDRange.Fragment * 0x5,

        /// <summary>
        /// <see cref="PluginSMS.SMSExtensionManager"/> 占用, 33536
        /// </summary>
        _0x6 = _Begin + IDRange.Fragment * 0x6,

        /// <summary>
        /// <see cref="PluginSMS.SMSExtensionManager"/> 占用, 33664
        /// </summary>
        _0x7 = _Begin + IDRange.Fragment * 0x7,

        /// <summary>
        /// <see cref="PluginSMS.SMSExtensionManager"/> 占用, 33792
        /// </summary>
        _0x8 = _Begin + IDRange.Fragment * 0x8,

        /// <summary>
        /// <see cref="PluginSMS.SMSExtensionManager"/> 占用, 33920
        /// </summary>
        _0x9 = _Begin + IDRange.Fragment * 0x9,

        /// <summary>
        /// 扩展开发案例管理器<see cref="ExtensionExampleManager"/> 类占用 - 预留, 34048
        /// </summary>
        _0xa = _Begin + IDRange.Fragment * 0xa,

        _0xb = _Begin + IDRange.Fragment * 0xb,//34176

        /// <summary>
        /// <see cref="PluginTimelines.TimelineManager"/> 占用, 34304
        /// </summary>
        _0xc = _Begin + IDRange.Fragment * 0xc,

        _0xd = _Begin + IDRange.Fragment * 0xd,//34432

        _0xe = _Begin + IDRange.Fragment * 0xe,//34560

        _0xf = _Begin + IDRange.Fragment * 0xf,//34688

        /// <summary>
        /// BFKJXXYJS210 占用, 34816
        /// </summary>
        _0x10 = _Begin + IDRange.Fragment * 0x10,

        /// <summary>
        /// <see cref="PluginHoloLens.HoloLensManager"/> 占用, 34944
        /// </summary>
        _0x11 = _Begin + IDRange.Fragment * 0x11,

        _0x12 = _Begin + IDRange.Fragment * 0x12,//35072

        /// <summary>
        /// <see cref="PluginPeripheralDevice.PeripheralDeviceInputManager"/> 占用, 35200
        /// </summary>
        _0x13 = _Begin + IDRange.Fragment * 0x13,

        /// <summary>
        /// <see cref="PluginXBox.XBoxManager"/>占用, 35328
        /// </summary>
        _0x14 = _Begin + IDRange.Fragment * 0x14,

        /// <summary>
        /// <see cref="PluginStereoTrack.StereoTrackManager"/> 占用, 35456
        /// </summary>
        _0x15 = _Begin + IDRange.Fragment * 0x15,

        /// <summary>
        /// <see cref="PluginStereoView.StereoViewManager"/> 占用, 35584
        /// </summary>
        _0x16 = _Begin + IDRange.Fragment * 0x16,

        /// <summary>
        /// <see cref="PluginHTCVive.HTCViveManager"/> 占用, 35712
        /// </summary>
        _0x17 = _Begin + IDRange.Fragment * 0x17,

        /// <summary>
        /// <see cref="PluginTools.ToolsExtensionManager"/> 占用, 35840
        /// </summary>
        _0x18 = _Begin + IDRange.Fragment * 0x18,

        /// <summary>
        /// <see cref="PluginTools.ToolsExtensionManager"/> 占用, 35968
        /// </summary>
        _0x19 = _Begin + IDRange.Fragment * 0x19,

        /// <summary>
        /// <see cref="PluginTools.ToolsExtensionManager"/> 占用, 36096
        /// </summary>
        _0x1a = _Begin + IDRange.Fragment * 0x1a,

        /// <summary>
        /// <see cref="PluginTools.ToolsExtensionManager"/> 占用, 36224
        /// </summary>
        _0x1b = _Begin + IDRange.Fragment * 0x1b,

        /// <summary>
        /// MachineryMotion 占用, 36352
        /// </summary>
        _0x1c = _Begin + IDRange.Fragment * 0x1c,

        /// <summary>
        /// <see cref="PluginXXR.Interaction.Toolkit.XXRInteractionToolkitManager"/> 占用, 36480
        /// </summary>
        _0x1d = _Begin + IDRange.Fragment * 0x1d,

        /// <summary>
        /// <see cref="PluginXGUI.XGUIManager"/> 占用, 36608
        /// </summary>
        _0x1e = _Begin + IDRange.Fragment * 0x1e,

        /// <summary>
        /// <see cref="PluginDataflows.DataflowManager"/> 占用, 36736
        /// </summary>
        _0x1f = _Begin + IDRange.Fragment * 0x1f,

        /// <summary>
        /// <see cref="PluginXAR.Foundation.XARFoundationManager"/> 占用, 36864
        /// </summary>
        _0x20 = _Begin + IDRange.Fragment * 0x20,

        /// <summary>
        /// <see cref="PluginVehicleDrive.VehicleDriveManger"/> 占用, 36992
        /// </summary>
        _0x21 = _Begin + IDRange.Fragment * 0x21,

        /// <summary>
        /// 通用标准脚本类<see cref="GenericStandardScriptManager"/> 占用, 37120
        /// </summary>
        _0x22 = _Begin + IDRange.Fragment * 0x22,

        /// <summary>
        /// 通用标准脚本类<see cref="GenericStandardScriptManager"/> 占用, 37248
        /// </summary>
        _0x23 = _Begin + IDRange.Fragment * 0x23,

        /// <summary>
        /// 通用标准脚本类<see cref="GenericStandardScriptManager"/> 占用, 37376
        /// </summary>
        _0x24 = _Begin + IDRange.Fragment * 0x24,

        /// <summary>
        /// 通用标准脚本类<see cref="GenericStandardScriptManager"/> 占用, 37504
        /// </summary>
        _0x25 = _Begin + IDRange.Fragment * 0x25,

        /// <summary>
        /// 通用标准脚本类<see cref="GenericStandardScriptManager"/> 占用, 37632
        /// </summary>
        _0x26 = _Begin + IDRange.Fragment * 0x26,

        /// <summary>
        /// 通用标准脚本类<see cref="GenericStandardScriptManager"/> 占用, 37760
        /// </summary>
        _0x27 = _Begin + IDRange.Fragment * 0x27,

        /// <summary>
        /// OptiTrack管理器<see cref="PluginOptiTrack.OptiTrackManager"/>占用, 37888
        /// </summary>
        _0x28 = _Begin + IDRange.Fragment * 0x28,

        /// <summary>
        /// ZVR管理器<see cref="PluginZVR.ZVRManager"/>占用, 38016
        /// </summary>
        _0x29 = _Begin + IDRange.Fragment * 0x29,

        /// <summary>
        /// 物理系统<see cref="PluginPhysicses.PhysicsManager"/>占用, 38144
        /// </summary>
        _0x2a = _Begin + IDRange.Fragment * 0x2a,

        /// <summary>
        /// ART管理器<see cref="PluginART.ARTManager"/>占用，38272
        /// </summary>
        _0x2b = _Begin + IDRange.Fragment * 0x2b,

        /// <summary>
        /// VoxelTracker管理器<see cref="PluginVoxelTracker.VoxelTrackerManager"/>38400
        /// </summary>
        _0x2c = _Begin + IDRange.Fragment * 0x2c,

        /// <summary>
        /// 三星玄龙WMR管理器<see cref="PluginSamsungWMR.SamsungWMRManager"/>38528
        /// </summary>
        _0x2d = _Begin + IDRange.Fragment * 0x2d,

        /// <summary>
        /// XR空间方案管理器<see cref="PluginXRSpaceSolution.XRSpaceSolutionManager"/>38656
        /// </summary>
        _0x2e = _Begin + IDRange.Fragment * 0x2e,

        /// <summary>
        /// 网络交互管理器<see cref="PluginNetInteract.NetInteractManager"/>38784
        /// </summary>
        _0x2f = _Begin + IDRange.Fragment * 0x2f,

        /// <summary>
        /// 森林消防<see cref="PluginForestFireFighting.ForestFireFightingManager"/>38912
        /// </summary>
        _0x30 = _Begin + IDRange.Fragment * 0x30,

        /// <summary>
        /// 机械运动<see cref="PluginMechanicalMotion.MechanicalMotionManager"/>39040
        /// </summary>
        _0x31 = _Begin + IDRange.Fragment * 0x31,

        /// <summary>
        /// Vuforia管理器<see cref="PluginVuforia.VuforiaManager"/>39168
        /// </summary>
        _0x32 = _Begin + IDRange.Fragment * 0x32,

        /// <summary>
        /// 嵌入式浏览器管理器<see cref="PluginEmbeddedBrowser.EmbeddedBrowserManager"/>39296
        /// </summary>
        _0x33 = _Begin + IDRange.Fragment * 0x33,

        /// <summary>
        /// PICO管理器<see cref="PluginPico.PicoManager"/>39424
        /// </summary>
        _0x34 = _Begin + IDRange.Fragment * 0x34,

        _0x35 = _Begin + IDRange.Fragment * 0x35,//39552

        _0x36 = _Begin + IDRange.Fragment * 0x36,//39680

        _0x37 = _Begin + IDRange.Fragment * 0x37,//39808

        _0x38 = _Begin + IDRange.Fragment * 0x38,//39936

        _0x39 = _Begin + IDRange.Fragment * 0x39,//40064

        _0x3a = _Begin + IDRange.Fragment * 0x3a,//40192

        _0x3b = _Begin + IDRange.Fragment * 0x3b,//40320

        _0x3c = _Begin + IDRange.Fragment * 0x3c,//40448

        _0x3d = _Begin + IDRange.Fragment * 0x3d,//40576

        _0x3e = _Begin + IDRange.Fragment * 0x3e,//40704

        _0x3f = _Begin + IDRange.Fragment * 0x3f,//40832

        /// <summary>
        /// 98176：扩展ID可达的最大片段值
        /// </summary>
        _0x1ff = _Begin + IDRange.Fragment * 0x1ff,

        /// <summary>
        /// 结束
        /// </summary>
        _End = IDRange.End,
    }
}
