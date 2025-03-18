using System;
using System.Collections;
using XCSJ.PluginCommonUtils;
using UnityEngine;
using XCSJ.Collections;
using XCSJ.Attributes;
#if UNITY_2018_1_OR_NEWER
using UnityEngine.XR;
#endif

namespace XCSJ.Extension.Base.Helpers
{
    /// <summary>
    /// XR设备枚举
    /// </summary>
    [Name("XR设备")]
    public enum EXRDevice
    {
        [Name("None")]
        None = 0,

        [Name("Oculus")]
        Oculus,

        [Name("OpenVR")]
        OpenVR,

        [Name("MockHMD")]
        [Tip("Mock HMD", "Mock HMD")]
        Mock_HMD,

        [Name("cardboard")]
        Cardboard,

        [Name("stereo")]
        [Tip("Stereo Display (non head-mounted)", "Stereo Display (non head-mounted)")]
        Stereo_Display_Non_Head_Mounted,
    }

    /// <summary>
    /// XR处理器
    /// </summary>
    public class XRHelper
    {
        public static bool VREnable
        {
            get
            {
#if UNITY_2018_1_OR_NEWER
                return XRSettings.enabled && !(string.IsNullOrEmpty(XRSettings.loadedDeviceName) || XRSettings.loadedDeviceName == "None");
#else
                return false;
#endif
            }
        }

        public static bool DeviceValid(string deviceName)
        {
            if (string.IsNullOrEmpty(deviceName)) return false;
#if UNITY_2018_1_OR_NEWER
           // Debug.Log("XRSettings.enabled:"+ XRSettings.enabled);
           // Debug.Log("deviceName:" + deviceName);
           // Debug.Log("XRSettings.supportedDevices,:" + XRSettings.supportedDevices.ToStringDirect());
           //Debug.Log("Array.IndexOf(XRSettings.supportedDevices, deviceName):" + Array.IndexOf(XRSettings.supportedDevices, deviceName));
            return /*XRSettings.enabled && */Array.IndexOf(XRSettings.supportedDevices, deviceName) >= 0;
#else
            return true;
#endif
        }

        public static void SwitchDevice(EXRDevice device, bool on = true)
        {            
            SwitchDevice(CommonFun.Name(device), on);
        }

        public static void SwitchDevice(string deviceName, bool on = true)
        {
            if (!DeviceValid(deviceName)) return;
#if UNITY_2018_1_OR_NEWER
            if (XRSettings.loadedDeviceName == deviceName)
            {
                if (on || !VREnable) return;
                deviceName = "None";
            }
            else
            {
                if (!on) return;
            }
#endif
            DeviceOn(deviceName);
        }

        public static void DeviceOnIfOff(string deviceName)
        {
            if (DeviceValid(deviceName) && XRSettings.loadedDeviceName != deviceName)
            {
                DeviceOn(deviceName);
            }
        }

        private static void DeviceOn(string deviceName)
        {
            GlobalMB.instance.StartCoroutine(LoadDevice(deviceName));
        }

        private static IEnumerator LoadDevice(string newDevice)
        {
#if UNITY_2018_1_OR_NEWER
            XRSettings.LoadDeviceByName(newDevice);
            yield return null;
            XRSettings.enabled = true;
#endif
        }
    }
}
