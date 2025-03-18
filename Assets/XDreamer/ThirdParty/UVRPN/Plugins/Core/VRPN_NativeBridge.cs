using UnityEngine;
using System.Runtime.InteropServices;

namespace XCSJ.UVRPN.Core
{
    /// <summary>
    /// This class is the interface to the native VRPN library.
    /// </summary>
    public static class VRPN_NativeBridge
    {
        [DllImport("unityVrpn")]
        private static extern double vrpnAnalogExtern(string address, int channel, int frameCount);

        [DllImport("unityVrpn")]
        private static extern bool vrpnButtonExtern(string address, int channel, int frameCount);

        [DllImport("unityVrpn")]
        private static extern double vrpnTrackerExtern(string address, int channel, int component, int frameCount);

        internal static double Analog(string address, int channel)
        {
            return vrpnAnalogExtern(address, channel, Time.frameCount);
        }

        internal static bool Button(string address, int channel)
        {
            return vrpnButtonExtern(address, channel, Time.frameCount);
        }

        internal static Vector3 TrackerPos(string address, int channel)
        {
            return new Vector3(
                (float) vrpnTrackerExtern(address, channel, 0, Time.frameCount),
                (float) vrpnTrackerExtern(address, channel, 1, Time.frameCount),
                (float) vrpnTrackerExtern(address, channel, 2, Time.frameCount));
        }

        internal static Quaternion TrackerQuat(string address, int channel)
        {
            return new Quaternion(
                (float) vrpnTrackerExtern(address, channel, 3, Time.frameCount),
                (float) vrpnTrackerExtern(address, channel, 4, Time.frameCount),
                (float) vrpnTrackerExtern(address, channel, 5, Time.frameCount),
                (float) vrpnTrackerExtern(address, channel, 6, Time.frameCount));
        }
    }
}