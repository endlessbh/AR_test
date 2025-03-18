using System;
using System.Text;
using UnityEditor;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Base.CategoryViews;
using XCSJ.EditorTools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginVehicleDrive.Controllers;

namespace XCSJ.EditorVehicleDrive
{
    /// <summary>
    /// 车轮模型绑定器检查器
    /// </summary>
    [Name("车轮模型绑定器检查器")]
    [CustomEditor(typeof(WheelDriverBinder))]
    public class WheelDriverBinderInspector : MBInspector<WheelDriverBinder>
    {
        private VehicleController vehicleController = null;

        protected override void OnEnable()
        {
            base.OnEnable();

            vehicleController = targetObject.GetComponentInParent<VehicleController>();
        }

        /// <summary>
        /// 当绘制成员
        /// </summary>
        /// <param name="serializedProperty"></param>
        protected override void OnDrawMember(SerializedProperty serializedProperty, PropertyData propertyData)
        {
            switch (serializedProperty.name)
            {
                case nameof(WheelDriverBinder._wheelDriver):
                    {
                        EditorGUILayout.BeginHorizontal();
                        base.OnDrawMember(serializedProperty, propertyData);
                        EditorGUI.BeginDisabledGroup(!vehicleController);
                        if (GUILayout.Button(new GUIContent("", EditorIconHelper.GetIconInLib(EIcon.Add)), EditorStyles.miniButtonLeft, UICommonOption.WH24x16))
                        {
                            DrawWheelDriverMenu(serializedProperty);
                        }
                        EditorGUI.EndDisabledGroup();
                        EditorGUILayout.EndHorizontal();
                        return;
                    }
            }
            base.OnDrawMember(serializedProperty, propertyData);
        }

        private void DrawWheelDriverMenu(SerializedProperty memberProperty)
        {
            var wheelDrivers = vehicleController.GetComponentsInChildren<WheelDriver>();
            if (wheelDrivers.Length > 0)
            {
                MenuHelper.DrawMenu(CommonFun.Name(typeof(WheelDriverBinder)), m =>
                {
                    for (int i = 0; i < wheelDrivers.Length; ++i)
                    {
                        var wd = wheelDrivers[i];
                        m.AddMenuItem((i + 1).ToString() + "." + wd.name, (c) =>
                        {
                            UndoHelper.RegisterCompleteObjectUndo(targetObject);
                            targetObject._wheelDriver = c as WheelDriver;
                        }, wd);
                    }
                });
            }
            else
            {
                Debug.LogErrorFormat("[{0}]车辆控制器中[{1}]组件！", vehicleController.name, nameof(WheelDriver));
            }
        }

        /// <summary>
        /// 显示提示信息
        /// </summary>
        protected override bool displayHelpInfo => !vehicleController;

        /// <summary>
        /// 提示信息类型
        /// </summary>
        protected override MessageType helpInfoMessageType => MessageType.Error;

        /// <summary>
        /// 提示信息
        /// </summary>
        /// <returns></returns>
        public override StringBuilder GetHelpInfo()
        {
            var sb = base.GetHelpInfo();
            sb.Append(string.Format("父级对象中必须包含{0}组件!", nameof(VehicleController)));
            return sb;
        }
    }
}
