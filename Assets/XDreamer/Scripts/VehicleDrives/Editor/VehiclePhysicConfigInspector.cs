using System;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginVehicleDrive.Controllers;

namespace XCSJ.EditorVehicleDrive
{
    /// <summary>
    /// 车辆物理属性配置检查器
    /// </summary>
    [Name("车辆物理属性配置检查器")]
    [CustomEditor(typeof(VehiclePhysicConfig))]
    public class VehiclePhysicConfigInspector : MBInspector<VehiclePhysicConfig>
    {
        /// <summary>
        /// 当绘制成员
        /// </summary>
        /// <param name="serializedProperty"></param>
        protected override void OnDrawMember(SerializedProperty serializedProperty, PropertyData propertyData)
        {
            base.OnDrawMember(serializedProperty, propertyData);
            switch (serializedProperty.name)
            {
                case nameof(VehiclePhysicConfig.COM):
                    {
                        var com = targetObject.COM;
                        if (com && targetObject.vehicleDriver && !com.transform.IsChildOf(targetObject.vehicleDriver.transform))
                        {
                            EditorGUILayout.HelpBox(string.Format("质量中心[{0}]必须是[{1}]的子对象！", com.name, targetObject.vehicleDriver.name), MessageType.Error);
                        }
                        break;
                    }
            }
        }
    }
}
