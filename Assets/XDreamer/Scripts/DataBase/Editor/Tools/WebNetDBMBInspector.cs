using System;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.Net.Http;
using XCSJ.PluginDataBase.Tools;

namespace XCSJ.EditorDataBase.Tools
{
    /// <summary>
    /// 基础Web数据库组件检查器
    /// </summary>
    [CustomEditor(typeof(WebNetDBMB), true)]
    [Name("基础Web数据库组件检查器")]
    public class BaseWebDBMBInspector : DBMBInspector<WebNetDBMB>
    {
        protected override void OnDrawMember(SerializedProperty serializedProperty, PropertyData propertyData)
        {
            switch (serializedProperty.name)
            {
                case nameof(targetObject.serverPort):
                    {
                        EditorGUILayout.BeginHorizontal();
                        base.OnDrawMember(serializedProperty, propertyData);
                        if (GUILayout.Button("默认", EditorStyles.miniButtonRight, GUILayout.Width(40)))
                        {
                            serializedProperty.intValue = HttpHelper.DefaultPort;
                        }
                        EditorGUILayout.EndHorizontal();
                        return;
                    }
            }
            base.OnDrawMember(serializedProperty, propertyData);
        }
    }
}
