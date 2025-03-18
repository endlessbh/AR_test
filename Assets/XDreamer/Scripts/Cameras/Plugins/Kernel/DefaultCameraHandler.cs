using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.PluginCamera;
using XCSJ.PluginCamera.Cameras;
using XCSJ.PluginCamera.Kernel;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginsCameras.Base;
using XCSJ.PluginsCameras.CNScripts;
using XCSJ.PluginsCameras.Controllers;
using XCSJ.Scripts;

namespace XCSJ.PluginsCameras.Kernel
{
    /// <summary>
    /// 默认相机处理器
    /// </summary>
    public class DefaultCameraHandler : InstanceClass<DefaultCameraHandler>, ICameraHandler
    {
        /// <summary>
        /// 初始化
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Init()
        {
            CameraHandler.handler = instance;
        }

        /// <summary>
        /// 获取脚本
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        public List<Script> GetScripts(CameraManager manager) => Script.GetScriptsOfEnum<ECameraScriptID>(manager);

        /// <summary>
        /// 运行脚本
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="id"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public ReturnValue ExecuteScript(CameraManager manager, int id, ScriptParamList param)
        {
            switch ((ECameraScriptID)id)
            {
                #region 新版相机

                case ECameraScriptID.SwitchCameraController:
                    {
                        float duration = (float)param[2];

                        string funcCallback = param[3] as string;
                        System.Action onComplete = () => ScriptManager.CallUDF(funcCallback);

                        var muse = ((EBool)(param[4])) == EBool.Yes;
                        var switchRule = param[5] as string;

                        bool returnValue = false;
                        switch (switchRule)
                        {
                            case "使用目标相机控制器对象":
                                {
                                    returnValue = manager.SwitchCameraController(param[1] as GameObject, duration, onComplete, muse);
                                    break;
                                }
                            case "上一个相机":
                                {
                                    returnValue = CameraHelperExtension.SwitchPreviousCamera(duration, onComplete, muse);
                                    break;
                                }
                            case "下一个相机":
                                {
                                    returnValue = CameraHelperExtension.SwitchNextCamera(duration, onComplete, muse);
                                    break;
                                }
                        }
                        return returnValue ? ReturnValue.Yes : ReturnValue.No;
                    }
                case ECameraScriptID.SwitchCameraControllerByName:
                    {
                        float duration = (float)param[2];

                        string funcCallback = param[3] as string;
                        System.Action onComplete = () => ScriptManager.CallUDF(funcCallback);

                        var muse = ((EBool)(param[4])) == EBool.Yes;
                        var switchRule = param[5] as string;

                        bool returnValue = false;
                        switch (switchRule)
                        {
                            case "使用目标相机控制器对象":
                                {
                                    var name = param[1] as string;
                                    var cc = ComponentCache.Get(typeof(BaseCameraMainController), true).components.FirstOrDefault(c => c.name == name) as BaseCameraMainController;
                                    returnValue = manager.SwitchCameraController(cc, duration, onComplete, muse);
                                    break;
                                }
                            case "上一个相机":
                                {
                                    returnValue = CameraHelperExtension.SwitchPreviousCamera(duration, onComplete, muse);
                                    break;
                                }
                            case "下一个相机":
                                {
                                    returnValue = CameraHelperExtension.SwitchNextCamera(duration, onComplete, muse);
                                    break;
                                }
                        }
                        return returnValue ? ReturnValue.Yes : ReturnValue.No;
                    }
                case ECameraScriptID.SetCameraControllerMainTarget:
                    {
                        var cameraController = GetCameraController(manager, param[10] as string, param[1] as GameObject);
                        if (!cameraController) break;
                        var mainTarget = param[2] as GameObject;
                        if (!mainTarget) break;
                        cameraController.mainTarget = mainTarget.transform;
                        return ReturnValue.Yes;
                    }
                case ECameraScriptID.SetCameraControllerSpeedCoefficient:
                    {
                        var cameraController = GetCameraController(manager, param[10] as string, param[1] as GameObject);
                        if (!cameraController) break;
                        switch (param[3] as string)
                        {
                            case "移动":
                                {
                                    cameraController.moveSpeedCoefficient = (Vector3)param[2];
                                    return ReturnValue.Yes;
                                }
                            case "旋转":
                                {
                                    cameraController.rotateSpeedCoefficient = (Vector3)param[2];
                                    return ReturnValue.Yes;
                                }
                        }
                        break;
                    }
                case ECameraScriptID.SetCameraControllerDampingCoefficient:
                    {
                        var cameraController = GetCameraController(manager, param[10] as string, param[1] as GameObject);
                        if (!cameraController) break;
                        switch (param[3] as string)
                        {
                            case "移动":
                                {
                                    cameraController.moveDampingCoefficient = (float)param[2];
                                    return ReturnValue.Yes;
                                }
                            case "旋转":
                                {
                                    cameraController.rotateDampingCoefficient = (float)param[2];
                                    return ReturnValue.Yes;
                                }
                        }
                        break;
                    }
                case ECameraScriptID.RecoverCameraControllerState:
                    {
                        var cameraController = GetCameraController(manager, param[10] as string, param[1] as GameObject);
                        if (!cameraController) break;
                        switch (param[2] as string)
                        {
                            case "原始状态":
                                {
                                    cameraController.cameraTransformer.RecoverToOriginal();
                                    return ReturnValue.Yes;
                                }
                            case "上一次状态":
                                {
                                    cameraController.cameraTransformer.RecoverToLast();
                                    return ReturnValue.Yes;
                                }
                        }
                        break;
                    }

                    #endregion
            }
            return ReturnValue.No;
        }

        private BaseCameraMainController GetCameraController(CameraManager manager, string rule,GameObject gameObject)
        {
            switch(rule)
            {
                case "指定":
                    {
                        if (!gameObject) break;
                        return gameObject.GetComponent<BaseCameraMainController>();
                    }
                case "当前":
                    {
                        return manager.GetProvider().currentCameraController;
                    }
            }
            return default;
        }

        /// <summary>
        /// 获取相机管理器提供者
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        public BaseCameraManagerProvider GetCameraManagerProvider(CameraManager manager)
        {
            return manager.XAddComponent<CameraManagerProvider>();
        }
    }
}
