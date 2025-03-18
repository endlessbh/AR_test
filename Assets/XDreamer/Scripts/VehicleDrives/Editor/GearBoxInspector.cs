using System;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginVehicleDrive.Controllers;

namespace XCSJ.EditorVehicleDrive
{
    /// <summary>
    /// 变速箱检查器
    /// </summary>
    [Name("变速箱检查器")]
    [CustomEditor(typeof(GearBox))]
    public class GearBoxInspector : MBInspector<GearBox>
    {
        /// <summary>
        /// 当绘制成员
        /// </summary>
        /// <param name="serializedProperty"></param>
        protected override void OnDrawMember(SerializedProperty serializedProperty, PropertyData propertyData)
        {
            switch (serializedProperty.name)
            {
                case nameof(GearBox._totalGearCount):
                    {
                        EditorGUILayout.BeginHorizontal();
                        base.OnDrawMember(serializedProperty, propertyData);
                        if (GUILayout.Button("创建齿轮组", EditorStyles.miniButtonRight, UICommonOption.Width60))
                        {
                            UndoHelper.RegisterCompleteObjectUndo(targetObject);
                            targetObject.InitGears();
                        }
                        EditorGUILayout.EndHorizontal();
                        return;
                    }
            }
            base.OnDrawMember(serializedProperty, propertyData);
        }
    }
}
