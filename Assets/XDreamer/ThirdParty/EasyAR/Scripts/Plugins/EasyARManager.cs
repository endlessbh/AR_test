using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.ComponentModel;
using XCSJ.PluginCommonUtils;
using XCSJ.Scripts;
using System.Runtime.InteropServices;
using XCSJ.Attributes;
using XCSJ.Algorithms;
#if XDREAMER_EASYAR_4_0_1 || XDREAMER_EASYAR_3_0_1
using easyar;
#elif XDREAMER_EASYAR_2_3_0
using EasyAR;
#endif

namespace XCSJ.PluginEasyAR
{
    /// <summary>
    /// EasyAR:用于与第三方插件EasyAR对接的管理器插件
    /// </summary>
    [Serializable]
    [DisallowMultipleComponent]
    [Name("EasyAR")]
    [Tip("用于与第三方插件EasyAR对接的管理器插件", "Manager plug-in for interfacing with third-party plug-in EasyAR")]
    [Guid("95F33829-4331-4C07-91B8-AFFEDF1CBD28")]
    [ComponentOption(EComponentOption.Optional)]
    [Version("23.428")]
    [Index(index = IndexAttribute.DefaultIndex + 20)]
    public class EasyARManager : BaseManager<EasyARManager>
    {
        [Name("EasyAR组件")]
        [Tip("EasyAR的根节点核心组件", "The root node core component of EasyAR")]
#if XDREAMER_EASYAR_4_0_1
        public EasyARController easyAR;
#elif XDREAMER_EASYAR_2_3_0 || XDREAMER_EASYAR_3_0_1
        public EasyARBehaviour easyAR;
#else
        public Component easyAR;
#endif

        [Name("相机设备")]
        [Tip("EasyAR的相机设备组件", "EasyAR camera device components")]
#if XDREAMER_EASYAR_4_0_1
        public VideoCameraDevice cameraDevice;
#elif XDREAMER_EASYAR_3_0_1
        public ExtendARSession cameraDevice;
#elif XDREAMER_EASYAR_2_3_0
        public CameraDeviceBaseBehaviour cameraDevice;
#else
        public Component cameraDevice;
#endif

#if XDREAMER_EASYAR_4_0_1
        private ARSession arSession;
#else
        private Component arSession;
#endif

        /// <summary>
        /// 获取脚本
        /// </summary>
        /// <returns></returns>
        public override List<Script> GetScripts()
        {
            return Script.GetScriptsOfEnum<EEasyARScriptID>(this, (int)EEasyARScriptID._Begin + 1, (int)EEasyARScriptID.MaxCurrent);
        }

        /// <summary>
        /// 执行脚本
        /// </summary>
        /// <param name="id"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public override ReturnValue ExecuteScript(int id, ScriptParamList param)
        {
#if XDREAMER_EASYAR_4_0_1
            switch ((EEasyARScriptID)id)
                {
                    case EEasyARScriptID.EasyARCameraDeviceOpenAndStart:
                        {
                            var cameraDevice = GetCameraDevice(param[0] as VideoCameraDevice);
                            if (!cameraDevice) break;
                            cameraDevice.Open();
                            return ReturnValue.Yes;
                        }
                    case EEasyARScriptID.EasyARCameraDeviceClose:
                        {
                            var cameraDevice = GetCameraDevice(param[0] as VideoCameraDevice);
                            if (!cameraDevice) break;
                            cameraDevice.Close();
                            return ReturnValue.Yes;
                        }
                    case EEasyARScriptID.EasyARCameraDeviceStartCapture:
                        {
                            var arSession = GetARSession(param[0] as ARSession);
                            if (!arSession) break;
                            var trackers = arSession.GetComponentsInChildren<FrameFilter>(true);
                            foreach(var tracker in trackers) tracker.enabled = true;
                            return ReturnValue.Yes; 
                        }
                    case EEasyARScriptID.EasyARCameraDeviceStopCapture:
                        {
                            var arSession = GetARSession(param[0] as ARSession);
                            if (!arSession) break;
                            var trackers = arSession.GetComponentsInChildren<FrameFilter>(true);
                            foreach (var tracker in trackers) tracker.enabled = false;
                            return ReturnValue.Yes;
                        }
                    case EEasyARScriptID.EasyARSwitchCameraDeviceType:
                        {
                            var cameraDevice = GetCameraDevice(param[0] as VideoCameraDevice);
                            if (!cameraDevice) break;

                            cameraDevice.CameraOpenMethod = VideoCameraDevice.CameraDeviceOpenMethod.DeviceType;
                            CameraDeviceType devicetype = cameraDevice.CameraType;
                            switch (param[1] as string)
                            {
                                case "前置摄像头":
                                    {
                                        devicetype = CameraDeviceType.Front;
                                        break;
                                    }
                                case "后置摄像头":
                                    {
                                        devicetype = CameraDeviceType.Back;
                                        break;
                                    }
                                case "切换":
                                    {
                                        switch (cameraDevice.CameraType)
                                        {
                                            case CameraDeviceType.Front:
                                                {
                                                    devicetype = CameraDeviceType.Back;
                                                    break;
                                                }
                                            case CameraDeviceType.Back:
                                            case CameraDeviceType.Unknown:
                                            default:
                                                {
                                                    devicetype = CameraDeviceType.Front;
                                                    break;
                                                }
                                        }
                                        break;
                                    }
                            }

                            if (cameraDevice.CameraType == devicetype) break;

                            cameraDevice.CameraType = devicetype;

                            cameraDevice.Close();
                            cameraDevice.Open();
                            return ReturnValue.Yes;
                        }
                    default: break;
                }
#elif XDREAMER_EASYAR_3_0_1
            switch ((EEasyARScriptID)id)
            {
                case EEasyARScriptID.EasyARSwitchCameraDeviceType:
                    {
                        var cameraDevice = GetCameraDevice(param[0] as ExtendARSession);
                        if (!cameraDevice) break;
                        switch (param[1] as string)
                        {
                            case "前置摄像头":
                                {
                                    cameraDevice.OpenCamera(false);
                                    break;
                                }
                            case "后置摄像头":
                                {
                                    cameraDevice.OpenCamera(true);
                                    break;
                                }
                            case "切换":
                                {
                                    cameraDevice.SwitchCamera();
                                    break;
                                }
                        }
                        return ReturnValue.Yes;
                    }
                default: break;
            }
#elif XDREAMER_EASYAR_2_3_0
            switch ((EEasyARScriptID)id)
            {
                case EEasyARScriptID.EasyARCameraDeviceOpenAndStart:
                    {
                        var cameraDevice = GetCameraDevice(param[0] as CameraDeviceBaseBehaviour);
                        if (!cameraDevice) break;

                        cameraDevice.OpenAndStart();
                        return ReturnValue.Yes;
                    }
                case EEasyARScriptID.EasyARCameraDeviceClose:
                    {
                        var cameraDevice = GetCameraDevice(param[0] as CameraDeviceBaseBehaviour);
                        if (!cameraDevice) break;

                        cameraDevice.Close();
                        return ReturnValue.Yes;
                    }
                case EEasyARScriptID.EasyARCameraDeviceStartCapture:
                    {
                        var cameraDevice = GetCameraDevice(param[0] as CameraDeviceBaseBehaviour);
                        if (!cameraDevice) break;

                        return ReturnValue.Create(cameraDevice.StartCapture());
                    }
                case EEasyARScriptID.EasyARCameraDeviceStopCapture:
                    {
                        var cameraDevice = GetCameraDevice(param[0] as CameraDeviceBaseBehaviour);
                        if (!cameraDevice) break;

                        return ReturnValue.Create(cameraDevice.StopCapture());
                    }
                case EEasyARScriptID.EasyARSwitchCameraDeviceType:
                    {
                        var cameraDevice = GetCameraDevice(param[0] as CameraDeviceBaseBehaviour);
                        if (!cameraDevice) break;

                        CameraDeviceBaseBehaviour.DeviceType devicetype = CameraDeviceBaseBehaviour.DeviceType.Default;
                        switch (param[1] as string)
                        {
                            case "前置摄像头":
                                {
                                    devicetype = CameraDeviceBaseBehaviour.DeviceType.Front;
                                    break;
                                }
                            case "后置摄像头":
                                {
                                    devicetype = CameraDeviceBaseBehaviour.DeviceType.Back;
                                    break;
                                }
                            case "切换":
                                {
                                    switch (cameraDevice.CameraDeviceType)
                                    {
                                        case CameraDeviceBaseBehaviour.DeviceType.Front:
                                            {
                                                devicetype = CameraDeviceBaseBehaviour.DeviceType.Back;
                                                break;
                                            }
                                        case CameraDeviceBaseBehaviour.DeviceType.Back:
                                        case CameraDeviceBaseBehaviour.DeviceType.Default:
                                        default:
                                            {
                                                devicetype = CameraDeviceBaseBehaviour.DeviceType.Front;
                                                break;
                                            }
                                    }
                                    break;
                                }
                        }

                        if (cameraDevice.CameraDeviceType == devicetype) break;

                        cameraDevice.Close();
                        cameraDevice.CameraDeviceType = devicetype;
                        cameraDevice.OpenAndStart();
                        return ReturnValue.Yes;
                    }
                default: break;
            }
#endif
            return ReturnValue.No;
        }

        /// <summary>
        /// 唤醒初始化
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
#if XDREAMER_EASYAR_4_0_1
            easyAR = InitEasyAR(easyAR);

            cameraDevice = cameraDevice ? cameraDevice : FindObjectOfType<VideoCameraDevice>();

            arSession = arSession ? arSession: FindObjectOfType<ARSession>();
#if !UNITY_EDITOR
            if (!easyAR)
            {
                Log.DebugFormat("未找到: {0} 组件!", typeof(EasyARController));
            }
#endif
#elif XDREAMER_EASYAR_3_0_1
            easyAR = InitEasyAR(easyAR);

            cameraDevice = cameraDevice ? cameraDevice : FindObjectOfType<ExtendARSession>();
#if !UNITY_EDITOR
            if (!easyAR)
            {
                Log.DebugFormat("未找到: {0} 组件!", typeof(EasyARBehaviour));
            }
#endif
#elif XDREAMER_EASYAR_2_3_0
            easyAR = InitEasyAR(easyAR);

            cameraDevice = cameraDevice ? cameraDevice : FindObjectOfType<CameraDeviceBaseBehaviour>();
#if !UNITY_EDITOR
            if (!easyAR)
            {
                Log.DebugFormat("未找到: {0} 组件!", typeof(EasyARBehaviour));
            }
#endif

#endif
        }

#if XDREAMER_EASYAR_4_0_1
        public VideoCameraDevice GetCameraDevice(VideoCameraDevice cameraDevice)
        {
            return cameraDevice ? cameraDevice : this.cameraDevice;
        }

        public ARSession GetARSession(ARSession arSession)
        {
            return arSession ? arSession : this.arSession;
        }

        public static EasyARController InitEasyAR(EasyARController easyAR)
        {
            return easyAR ? easyAR : ((instance && instance.easyAR) ? instance.easyAR : FindObjectOfType<EasyARController>());
        }

        public static VideoCameraDevice InitCameraDevice(VideoCameraDevice cameraDevice)
        {
            return cameraDevice ? cameraDevice : ((instance && instance.cameraDevice) ? instance.cameraDevice : FindObjectOfType<VideoCameraDevice>());
        }

#elif XDREAMER_EASYAR_3_0_1
        public ExtendARSession GetCameraDevice(ExtendARSession cameraDevice)
        {
            return cameraDevice ? cameraDevice : this.cameraDevice;
        }

        public static EasyARBehaviour InitEasyAR(EasyARBehaviour easyAR)
        {
            return easyAR ? easyAR : ((instance && instance.easyAR) ? instance.easyAR : FindObjectOfType<EasyARBehaviour>());
        }

        public static ExtendARSession InitCameraDevice(ExtendARSession cameraDevice)
        {
            return cameraDevice ? cameraDevice : ((instance && instance.cameraDevice) ? instance.cameraDevice : FindObjectOfType<ExtendARSession>());
        }

#elif XDREAMER_EASYAR_2_3_0

        public CameraDeviceBaseBehaviour GetCameraDevice(CameraDeviceBaseBehaviour cameraDevice)
        {
            return cameraDevice ? cameraDevice : this.cameraDevice;
        }

        public static EasyARBehaviour InitEasyAR(EasyARBehaviour easyAR)
        {
            return easyAR ? easyAR : ((instance && instance.easyAR) ? instance.easyAR : FindObjectOfType<EasyARBehaviour>());
        }

        public static CameraDeviceBaseBehaviour InitCameraDevice(CameraDeviceBaseBehaviour cameraDevice)
        {
            return cameraDevice ? cameraDevice : ((instance && instance.cameraDevice) ? instance.cameraDevice : FindObjectOfType<CameraDeviceBaseBehaviour>());
        }
#else

        public MonoBehaviour GetCameraDevice(MonoBehaviour cameraDevice) => null;

        public static MonoBehaviour InitEasyAR(MonoBehaviour easyAR) => null;

        public static MonoBehaviour InitCameraDevice(MonoBehaviour cameraDevice) => null;

#endif
    }
}
