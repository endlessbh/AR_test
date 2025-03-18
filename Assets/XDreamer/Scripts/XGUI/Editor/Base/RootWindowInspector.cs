using System;
using UnityEditor;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginXGUI.Base;

namespace XCSJ.EditorXGUI.Base
{
    /// <summary>
    /// 根窗口检查器
    /// </summary>
    [Name("根窗口检查器")]
    [CustomEditor(typeof(RootWindow), true)]
    public class RootWindowInspector: SubWindowInspector
    {
        /// <summary>
        /// 当绘制成员
        /// </summary>
        /// <param name="serializedProperty"></param>
        protected override void OnDrawMember(SerializedProperty serializedProperty, PropertyData propertyData)
        {
            switch (serializedProperty.name)
            {
                case nameof(SubWindow._title):
                case nameof(SubWindow._content):
                    {
                        return;
                    }
            }
            base.OnDrawMember(serializedProperty, propertyData);
        }
    }
}
