using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Scripts;
using XCSJ.PluginXXR.Interaction.Toolkit.Base;

#if XDREAMER_STEAMVR
using Valve.VR;
using static Valve.VR.SteamVR;
#endif

namespace XCSJ.PluginHTCVive.Base
{
    /// <summary>
    /// SteamVR助手
    /// </summary>
    public static class SteamVRHelper
    {
        /// <summary>
        /// 初始化SteamVR输入
        /// </summary>
        /// <returns></returns>
        public static bool InitSteamVRInput()
        {
#if XDREAMER_STEAMVR
            if (SteamVR.initializedState != InitializedStates.InitializeSuccess)
            {
                SteamVR.Initialize();
            }
            if (SteamVR.initializedState == InitializedStates.InitializeSuccess)
            {
                if (!SteamVR_Input.initialized)
                {
                    SteamVR_Input.Initialize(true);
                }
            }

            return SteamVR_Input.initialized;
#else
            return default;
#endif
        }

        /// <summary>
        /// 激活动作集
        /// </summary>
        /// <param name="actionSetName"></param>
        /// <returns></returns>
        public static void ActiveActionSet(string actionSetName, EBool active)
        {
#if XDREAMER_STEAMVR
            var actionSet = SteamVR_Input.GetActionSet(actionSetName);
            if (actionSet == null)
            {
                Debug.LogErrorFormat("SteamVR输入中未找到[{0}]动作集！" + actionSetName);
                return;
            }
            switch (active)
            {
                case EBool.Yes:
                    {
                        actionSet.Activate();
                        break;
                    }
                case EBool.No:
                    {
                        actionSet.Deactivate();
                        break;
                    }
                case EBool.Switch:
                    {
                        if (actionSet.IsActive())
                        {
                            actionSet.Deactivate();
                        }
                        else
                        {
                            actionSet.Activate();
                        }
                        break;
                    }
            }
#endif
        }


#if XDREAMER_STEAMVR

        private static SteamVR_ActionSet platformer = null;

        /// <summary>
        /// SteamVR默认配置只激活第一个动作集（一般是default）
        /// </summary>
        /// <param name="inputSource"></param>
        /// <returns></returns>
        public static Vector2 GetMoveAction(SteamVR_Input_Sources inputSource)
        {
            // 激活platformer动作集
            if (platformer == null)
            {
                platformer = SteamVR_Input.GetActionSet(nameof(platformer));
            }
            if (platformer != null && !platformer.IsActive())
            {
                platformer.Activate();
            }
#if XDREAMER_STEAMVR_INPUT
            return SteamVR_Actions.platformer_Move.GetAxis(inputSource);
#else
            return Vector2.zero;
#endif
        }

        private static SteamVR_ActionSet buggy = null;

        /// <summary>
        /// 获取brake键值
        /// </summary>
        /// <param name="inputSource"></param>
        /// <returns></returns>
        public static bool GetBrakeButtonState(SteamVR_Input_Sources inputSource)
        {
            ActiveBuggyActionSet();
#if XDREAMER_STEAMVR_INPUT
            return SteamVR_Actions.buggy_Brake.GetState(inputSource);
#else
            return false;
#endif
        }

        /// <summary>
        /// 获取Reset键值
        /// </summary>
        /// <param name="inputSource"></param>
        /// <returns></returns>
        public static bool GetResetButtonState(SteamVR_Input_Sources inputSource)
        {
            ActiveBuggyActionSet();
#if XDREAMER_STEAMVR_INPUT
            return SteamVR_Actions.buggy_Reset.GetState(inputSource);
#else
            return false;
#endif
        }

        private static void ActiveBuggyActionSet()
        {
            // 激活buggy动作集
            if (buggy == null)
            {
                buggy = SteamVR_Input.GetActionSet(nameof(buggy));
            }
            if (buggy != null && !buggy.IsActive())
            {
                buggy.Activate();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deviceType"></param>
        /// <param name="inputSource"></param>
        /// <returns></returns>
        public static bool TryConvertDeviceTypeToInputSource(EVRDeviceType deviceType, out SteamVR_Input_Sources inputSource)
        {
            switch (deviceType)
            {
                case EVRDeviceType.HMD:
                    {
                        inputSource = SteamVR_Input_Sources.Head;
                        return true;
                    }
                case EVRDeviceType.Left:
                    {
                        inputSource = SteamVR_Input_Sources.LeftHand;
                        return true;
                    }
                case EVRDeviceType.Right:
                    {
                        inputSource = SteamVR_Input_Sources.RightHand;
                        return true;
                    }
            }
            inputSource = default;
            return false;
        }

        /// <summary>
        /// 返回动作数值：bool型转化为1或0
        /// </summary>
        /// <param name="actionName"></param>
        /// <param name="inputSource"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool TryGetAction(string actionName, SteamVR_Input_Sources inputSource, out float value)
        {
            var rs = SteamVRHelper.TryGetActionState(actionName, inputSource, out var state);
            if (rs)
            {
                value = state ? 1 : 0;
            }
            else
            {
                value = default;
            }
            return rs;
        }

        /// <summary>
        /// 获取动作状态：返回值为真或假
        /// </summary>
        /// <param name="actionName"></param>
        /// <param name="inputSource"></param>
        /// <param name="state"></param>
        /// <returns>获取成功或失败</returns>
        public static bool TryGetActionState(string actionName, SteamVR_Input_Sources inputSource, out bool state)
        {
            try
            {
                var menuAction = SteamVR_Input.GetBooleanAction(actionName);
                if (menuAction != null)
                {
                    state = menuAction.GetState(inputSource);
                    return true;
                }
            }
            catch
            {
                Debug.LogErrorFormat("SteamVR Input中未配置菜单{0}按键映射!", actionName);
            }
            state = default;
            return false;
        }
#endif
        }
    }
