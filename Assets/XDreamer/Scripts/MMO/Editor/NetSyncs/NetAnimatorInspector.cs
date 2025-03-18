using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginMMO.NetSyncs;

namespace XCSJ.EditorMMO.NetSyncs
{
    /// <summary>
    /// 网络动画检查器
    /// </summary>
    [Name("网络动画检查器")]
    [CustomEditor(typeof(NetAnimator), true)]
    public class NetAnimatorInspector : NetMBInspector<NetAnimator>
    {
        public Animator animator => targetObject.animator;

        public AnimatorController animatorController => animator ? animator.runtimeAnimatorController as AnimatorController : null;

        public int index = -1;

        private NetAnimator.AnimatorData GetAnimatorData(SerializedProperty memberProperty)
        {
            if (memberProperty.propertyPath.StartsWith(nameof(targetObject.data) + ".")) return targetObject.data;
            if (memberProperty.propertyPath.StartsWith(nameof(targetObject.prevData) + ".")) return targetObject.prevData;
            if (memberProperty.propertyPath.StartsWith(nameof(targetObject.targetData) + ".")) return targetObject.targetData;
            if (memberProperty.propertyPath.StartsWith(nameof(targetObject.originalData) + ".")) return targetObject.originalData;
            return null;
        }

        protected override void OnDrawMember(SerializedProperty serializedProperty, PropertyData propertyData)
        {
            switch (serializedProperty.name)
            {
                case nameof(NetAnimator.AnimatorData.parameters):
                    {
                        serializedProperty.isExpanded = UICommonFun.Foldout(serializedProperty.isExpanded, propertyData.trLabel);
                        if (!serializedProperty.isExpanded) return;

                        var data = GetAnimatorData(serializedProperty);
                        if (data == null) return;

                        CommonFun.BeginLayout();

                        #region 添加

                        EditorGUILayout.BeginHorizontal();

                        EditorGUILayout.PrefixLabel("参数");

                        var animatorController = this.animatorController;
                        var array = animatorController.parameters.ToList(p => string.Format("{0}({1})", p.name, p.type.ToString())).ToArray();

                        index = EditorGUILayout.Popup(index, array);

                        if (GUILayout.Button(UICommonOption.Insert, EditorStyles.miniButtonRight, UICommonOption.WH24x16) && index >= 0)
                        {
                            var par = animatorController.parameters[index];
                            if (!data.parameters.Exists(p => p.name == par.name))//防止重复添加
                            {
                                var sp = SerializedObjectHelper.AddArrayElement(serializedProperty);
                                sp.FindPropertyRelative(nameof(NetAnimator.Parameter.type)).intValue = (int)par.type;
                                sp.FindPropertyRelative(nameof(NetAnimator.Parameter.name)).stringValue = par.name;
                                sp.FindPropertyRelative(nameof(NetAnimator.Parameter.value)).stringValue = "";
                            }
                        }
                        EditorGUILayout.EndHorizontal();

                        #endregion

                        EditorGUILayout.BeginHorizontal(GUI.skin.box);
                        EditorGUILayout.LabelField("No.", UICommonOption.Width24);
                        EditorGUILayout.LabelField("名称", UICommonOption.Width80);
                        EditorGUILayout.LabelField("类型", UICommonOption.Width60);
                        EditorGUILayout.LabelField("值");
                        EditorGUILayout.LabelField("操作", UICommonOption.Width24);
                        EditorGUILayout.EndHorizontal();

                        #region 标题

                        #endregion

                        #region 内容

                        for (int i = 0; i < data.parameters.Count; ++i)
                        {
                            var p = data.parameters[i];
                            try
                            {
                                UICommonFun.BeginHorizontal(i);
                                EditorGUILayout.LabelField((i + 1).ToString(), UICommonOption.Width24);
                                EditorGUILayout.TextField(p.name, UICommonOption.Width80);
                                EditorGUILayout.TextField(p.type.ToString(), UICommonOption.Width60);
                                EditorGUILayout.TextField(p.value);
                                if (GUILayout.Button(UICommonOption.Delete, EditorStyles.miniButtonRight, UICommonOption.WH24x16))
                                {
                                    SerializedObjectHelper.DeleteArrayElement(serializedProperty, i);
                                    break;
                                }
                            }
                            finally
                            {
                                UICommonFun.EndHorizontal();
                            }
                        }

                        #endregion

                        CommonFun.EndLayout();

                        return;
                    }
            }
            base.OnDrawMember(serializedProperty, propertyData);
        }
    }
}
