using System;
using UnityEditor;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginSMS.States.Base;
using XCSJ.PluginSMS.States.Motions;

namespace XCSJ.EditorSMS.States.Motions
{
    public class RangeHandleInspector<T> : MotionInspector<T> where T : Motion<T>, IRangeHandle
    {
        /// <summary>
        /// 当绘制成员时总是回调
        /// </summary>
        /// <param name="serializedProperty"></param>
        protected override void OnDrawMemberAlways(SerializedProperty serializedProperty, PropertyData propertyData)
        {
            switch (serializedProperty.name)
            {
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
