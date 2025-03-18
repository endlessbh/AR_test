using UnityEditor;
using System.Collections.Generic;
using XCSJ.Algorithms;
using XCSJ.EditorCameras.Base;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Base.Controls;
using XCSJ.EditorCommonUtils.Base.CategoryViews;
using XCSJ.PluginsCameras.Controllers;
using XCSJ.EditorTools;
using XCSJ.PluginVehicleDrive.Controllers;
using UnityEngine;
using XCSJ.PluginVehicleDrive;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.EditorExtension.Base;

namespace XCSJ.EditorVehicleDrive
{
    /// <summary>
    /// 相机移动器检查器
    /// </summary>
    [CustomEditor(typeof(VehicleDriveManger), true)]
    public class VehicleDriveMangerInspector : MBInspector<VehicleDriveManger>
    {
        /// <summary>
        /// 目录列表
        /// </summary>
        public static XObject<CategoryList> categoryList { get; } = new XObject<CategoryList>(cl => cl != null, x => EditorToolsHelper.GetWithPurposes(nameof(VehicleDriveManger)));

        /// <summary>
        /// 当绘制检查器GUI
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            CategoryListExtension.DrawVertical(categoryList);

            DrawVehicleControllerList();
        }

        /// <summary>
        /// 车辆控制器列表
        /// </summary>
        [Name("车辆控制器列表")]
        [Tip("当前场景中所有的车辆控制器对象", "All vehicle controller objects in the current scene")]
        public bool vehicleControllers = true;

        private static XGUIStyle _centerLabelStyle { get; } = new XGUIStyle(nameof(GUI.skin.label), s =>
          {
              s.name = XGUIStyleLib.GetXDreamerStyleName(nameof(VehicleDriveMangerInspector) + nameof
                  (_centerLabelStyle));
              s.alignment = TextAnchor.MiddleCenter;
          });

        [Name("车辆控制器")]
        [Tip("车辆控制器所在的游戏对象；本项只读；", "The game object where the vehicle controller is located; This item is read-only;")]
        public bool vehicleController;

        [Name("车轮数量")]
        [Tip("车辆的车轮数量", "Number of wheels of the vehicle")]
        public bool wheelsCount;

        private void DrawVehicleControllerList()
        {
            vehicleControllers = UICommonFun.Foldout(vehicleControllers, TrLabel(nameof(vehicleControllers)));
            if (!vehicleControllers) return;

            CommonFun.BeginLayout();

            // 标题
            EditorGUILayout.BeginHorizontal(GUI.skin.box);
            {
                GUILayout.Label("NO.", UICommonOption.Width32);
                GUILayout.Label(TrLabel(nameof(vehicleController)));
                GUILayout.Label(TrLabel(nameof(wheelsCount)), UICommonOption.Width128);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Separator();

            // 列表
            var cache = ComponentCache.Get(typeof(VehicleController), true);
            for (int i = 0; i < cache.components.Length; i++)
            {
                var component = cache.components[i] as VehicleController;

                UICommonFun.BeginHorizontal(i);

                //编号
                EditorGUILayout.LabelField((i + 1).ToString(), UICommonOption.Width32);

                //车辆控制器
                EditorGUILayout.ObjectField(component.gameObject, typeof(GameObject), true);

                //车轮数量
                EditorGUILayout.LabelField(component.GetComponentsInChildren<WheelDriver>().Length.ToString(), _centerLabelStyle, UICommonOption.Width128);

                UICommonFun.EndHorizontal();
            }

            CommonFun.EndLayout();
        }
    }
}
