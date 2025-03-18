using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Base.Kernel;
using XCSJ.EditorXGUI;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginTools.LineNotes;
using XCSJ.PluginTools.SelectionUtils;

namespace XCSJ.EditorTools.Inspectors
{
    /// <summary>
    /// 3D批注检查器
    /// </summary>
    [Name("3D批注检查器")]
    [CustomEditor(typeof(LineNote3D), true)]
    public class LineNote3DInspector : LineNoteInspector
    {
        protected LineNote3D lineNote3D => targetObject as LineNote3D;

        /// <summary>
        /// 当绘制成员
        /// </summary>
        /// <param name="serializedProperty"></param>
        protected override void OnDrawMember(SerializedProperty serializedProperty, PropertyData propertyData)
        {
            switch (serializedProperty.name)
            {
                case nameof(LineNote3D.endTarget):
                    {
                        EditorGUILayout.BeginHorizontal();
                        base.OnDrawMember(serializedProperty, propertyData);
                        if (GUILayout.Button(UICommonOption.Insert, UICommonOption.WH24x16))
                        {
                            CreateNoteGameObjectAndFocus(CommonFun.Name(typeof(LineNote3D), nameof(LineNote3D.endTarget)), serializedProperty, targetObject.target);
                        }
                        EditorGUILayout.EndHorizontal();
                        return;
                    }
            }
            base.OnDrawMember(serializedProperty, propertyData);
        }

        public void OnSceneGUI()
        {
            if (lineNote3D.target && lineNote3D.endTarget)
            {
                var orgColor = Handles.color;
                if(lineNote3D.lineStyle) Handles.color = lineNote3D.lineStyle.color;
                Handles.DrawPolyLine(lineNote3D.beginPoint, lineNote3D.endPoint);
                Handles.color = orgColor;
            }
        }
    }

}
