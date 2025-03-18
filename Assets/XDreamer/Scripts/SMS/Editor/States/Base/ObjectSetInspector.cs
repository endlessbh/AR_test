using System;
using UnityEditor;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension.Base.Tools;
using XCSJ.EditorSMS.Inspectors;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.EditorSMS.States.Base
{
    /// <summary>
    /// 对象集检查器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObjectSetInspector<T> : StateComponentInspector<T> where T : StateComponent, IObjectSet
    {
        internal const string ObjectsString = "_" + nameof(IObjectSet.objects);

        /// <summary>
        /// 当绘制成员时总是回调
        /// </summary>
        /// <param name="serializedProperty"></param>
        /// <param name="propertyData"></param>
        protected override void OnDrawMemberAlways(SerializedProperty serializedProperty, PropertyData propertyData)
        {
            switch (serializedProperty.name)
            {
                case ObjectsString:
                    {
                        EditorSerializedObjectHelper.DrawArrayHandleRule(serializedProperty);
                        break;
                    }
            }
            base.OnDrawMemberAlways(serializedProperty, propertyData);
        }

        public override void OnDrawHelpInfo()
        {
            //base.OnDrawHelpInfo();
        }
    }
}
