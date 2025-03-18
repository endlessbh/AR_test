using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorTools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginSMS;
using XCSJ.PluginVehicleDrive;
using XCSJ.PluginVehicleDrive.States;
using XCSJ.PluginVehicleDrive.Controllers;
using XCSJ.EditorXGUI;
using XCSJ.Languages;
using XCSJ.PluginPhysicses.Tools.Weapons;

namespace XCSJ.EditorVehicleDrive
{
    /// <summary>
    /// 工具库菜单
    /// </summary>
    [LanguageFileOutput]
    public static class ToolsMenu
    {
        /// <summary>
        /// 二轮车
        /// </summary>
        /// <param name="toolContext"></param>
        /// <returns></returns>
        [Tool(VehicleDriveHelper.CategoryName, nameof(VehicleDriveManger), rootType = typeof(VehicleDriveManger), index = 1, groupRule = EToolGroupRule.None)]
        [XCSJ.Attributes.Icon(EIcon.Motorcycle)]
        [Name("二轮车")]
        [RequireManager(typeof(VehicleDriveManger))]
        [Manual(typeof(VehicleController))]
        public static void CreateTwoWheelVehicle(ToolContext toolContext)
        {
            var go = EditorToolsHelperExtension.LoadPrefab_DefaultXDreamerPath("车辆驾驶/二轮车.prefab");
            if (go)
            {
                EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, go);
                InitData(go);
            }
        }

        /// <summary>
        /// 三轮车
        /// </summary>
        /// <param name="toolContext"></param>
        /// <returns></returns>
        [Tool(VehicleDriveHelper.CategoryName, nameof(VehicleDriveManger), rootType = typeof(VehicleDriveManger), index = 2, groupRule = EToolGroupRule.None)]
        [XCSJ.Attributes.Icon(EIcon.Tricycle)]
        [Name("三轮车")]
        [RequireManager(typeof(VehicleDriveManger))]
        [Manual(typeof(VehicleController))]
        public static void CreateThreeWheelVehicle(ToolContext toolContext)
        {
            var go = EditorToolsHelperExtension.LoadPrefab_DefaultXDreamerPath("车辆驾驶/三轮车.prefab");
            if (go)
            {
                EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, go);
                InitData(go);
            }
        }

        /// <summary>
        /// 四轮车
        /// </summary>
        /// <param name="toolContext"></param>
        /// <returns></returns>
        [Tool(VehicleDriveHelper.CategoryName, nameof(VehicleDriveManger), rootType = typeof(VehicleDriveManger), index = 3, groupRule = EToolGroupRule.None)]
        [XCSJ.Attributes.Icon(EIcon.SpotCar)]
        [Name("四轮车")]
        [RequireManager(typeof(VehicleDriveManger))]
        [Manual(typeof(VehicleController))]
        public static void CreateFourWheelVehicle(ToolContext toolContext)
        {
            var go = EditorToolsHelperExtension.LoadPrefab_DefaultXDreamerPath("车辆驾驶/四轮车.prefab");
            if (go)
            {
                EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, go);
                InitData(go);
            }
        }

        /// <summary>
        /// 六轮车
        /// </summary>
        /// <param name="toolContext"></param>
        /// <returns></returns>
        [Tool(VehicleDriveHelper.CategoryName, nameof(VehicleDriveManger), rootType = typeof(VehicleDriveManger), index = 4, groupRule = EToolGroupRule.None)]
        [XCSJ.Attributes.Icon(EIcon.Truck)]
        [Name("六轮车")]
        [RequireManager(typeof(VehicleDriveManger))]
        [Manual(typeof(VehicleController))]
        public static void CreateSixWheelVehicle(ToolContext toolContext)
        {
            var go = EditorToolsHelperExtension.LoadPrefab_DefaultXDreamerPath("车辆驾驶/六轮车.prefab");
            if (go)
            {
                EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, go);
                InitData(go);
            }
        }

        /// <summary>
        /// 卡车
        /// </summary>
        /// <param name="toolContext"></param>
        /// <returns></returns>
        [Tool(VehicleDriveHelper.CategoryName, nameof(VehicleDriveManger), rootType = typeof(Canvas), needRootParentIsNull = true, index = 4, groupRule = EToolGroupRule.None)]
        [XCSJ.Attributes.Icon(EIcon.UI)]
        [Name("车辆输入界面")]
        [RequireManager(typeof(VehicleDriveManger))]
        [Manual(typeof(VehicleController))]
        public static void CreateVehicleUIController(ToolContext toolContext)
        {
            EditorXGUI.ToolsMenu.CreateUIToolAndStretchHV(toolContext, () => EditorToolsHelperExtension.LoadPrefab_DefaultXDreamerPath("车辆驾驶/车辆输入界面.prefab"));
        }

        /// <summary>
        /// 因为状态机从资产中克隆出来，所以状态机中的引用是无效的。
        /// 此时需要重新在车辆对象的子对象中查找对应的引用对象，重新赋值
        /// </summary>
        /// <param name="go"></param>
        private static void InitData(GameObject go)
        {
            foreach (var smc in go.GetComponentsInChildren<SMController>())
            {
                // 设置车辆控制器对象
                var states = smc.stateMachine.GetComponentsInChildren<VehicleDriverPropertySet>();
                if (states.Length > 0)
                {
                    var vc = smc.transform.parent.GetComponentInChildren<VehicleDriver>();
                    foreach (var s in states)
                    {
                        s._vehicleDriver = vc;
                    }
                }
            }
        }
    }
}
