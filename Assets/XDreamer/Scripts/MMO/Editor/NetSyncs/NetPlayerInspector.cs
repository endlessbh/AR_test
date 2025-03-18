using System;
using UnityEditor;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginMMO.NetSyncs;

namespace XCSJ.EditorMMO.NetSyncs
{
    /// <summary>
    /// 网络玩家检查器
    /// </summary>
    [Name("网络玩家检查器")]
    [CustomEditor(typeof(NetPlayer), true)]
    public class NetPlayerInspector : NetPropertyInspector<NetPlayer>
    {
        /// <summary>
        /// 当绘制成员
        /// </summary>
        /// <param name="serializedProperty"></param>
        protected override void OnDrawMember(SerializedProperty serializedProperty, PropertyData propertyData)
        {
            switch (serializedProperty.name)
            {
                case nameof(NetProperty.propertys):
                    {
                        EditorGUI.BeginChangeCheck();
                        var nickName = EditorGUILayout.DelayedTextField(CommonFun.NameTip(mb.GetType(), nameof(NetPlayer.nickName)), mb.nickName);
                        if (EditorGUI.EndChangeCheck())
                        {
                            mb.nickName = nickName;
                        }
                        break;
                    }
            }
            base.OnDrawMember(serializedProperty, propertyData);
        }
    }
}
