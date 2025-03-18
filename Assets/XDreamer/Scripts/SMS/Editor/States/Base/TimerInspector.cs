using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.EditorSMS.States.Base
{
    /// <summary>
    /// 定时器检查器
    /// </summary>
    [Name("定时器检查器")]
    [CustomEditor(typeof(Timer))]
    public class TimerInspector : WorkClipInspector<Timer>
    {
        /// <summary>
        /// 当绘制成员时总是回调
        /// </summary>
        /// <param name="serializedProperty"></param>
        /// <param name="propertyData"></param>
        protected override void OnDrawMemberAlways(SerializedProperty serializedProperty, PropertyData propertyData)
        {            
            switch (serializedProperty.name)
            {
                case nameof(WorkClip.useInitData):
                case nameof(WorkClip.setPercentOnEntry):
                case nameof(WorkClip.percentOnEntry):
                case nameof(WorkClip.setPercentOnExit):
                case nameof(WorkClip.percentOnExit):
                case nameof(WorkClip.loopType):
                case nameof(WorkClip.workCurve):
                    {
                        return;
                    }
            }
            base.OnDrawMemberAlways(serializedProperty, propertyData);
        }
    }
}
