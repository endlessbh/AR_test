using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using XCSJ.Attributes;
using XCSJ.EditorSMS.States.Base;
using XCSJ.PluginPeripheralDevice.XR.States;

namespace XCSJ.EditorPeripheralDevice.XR.States
{
    /// <summary>
    /// XR输入设备事件检查器
    /// </summary>
    [Name("XR输入设备事件检查器")]
    [CustomEditor(typeof(XRInputDeviceEvent))]
    public class XRInputDeviceEventInspector : TriggerInspector<XRInputDeviceEvent>
    {
        /// <summary>
        /// 显示辅助信息
        /// </summary>
        protected override bool displayHelpInfo => true;

        /// <summary>
        /// 获取辅助信息
        /// </summary>
        /// <returns></returns>
        public override StringBuilder GetHelpInfo()
        {
            var info = base.GetHelpInfo();
            if(stateComponent._eventType== XRInputDeviceEvent.EEventType.CommonUsages)
            {
                var usage = stateComponent.inputFeatureUsage;
                if (usage != null)
                {
                    var type = usage.GetType();
                    var type0 = type.GetGenericArguments().FirstOrDefault();
                    info.AppendFormat("常用功能值类型:\t{0}", type0?.Name ?? "未知");
                }
                else
                {
                    info.AppendFormat("<color=#FF0000FF>常用功能的输入特征功能无效</color>");
                }
            }            
            return info;
        }
    }
}
