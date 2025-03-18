using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.ComponentModel;
using XCSJ.EditorCommonUtils;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginMMO;
using XCSJ.Scripts;
using XCSJ.Tools;

namespace XCSJ.EditorMMO
{
    /// <summary>
    /// 网络MB检查器
    /// </summary>
    [Name("网络MB检查器")]
    [CustomEditor(typeof(NetMB), true)]
    public class NetMBInspector : NetMBInspector<NetMB> { }

    public class NetMBInspector<T> : MMOMBInspector<T> where T : NetMB
    {
        private Type hudType = null;
        private Type windowType = null;
        private MMOOption option = null;
        private List<string> allSyncVars = null;

        private GUIStyle _syncVarGUIStyle;
        private GUIStyle syncVarGUIStyle
        {
            get
            {
                if (_syncVarGUIStyle == null)
                {
                    _syncVarGUIStyle = new GUIStyle(GUI.skin.box);
                    _syncVarGUIStyle.normal.background = Texture2DHelper.GetTexture2D(option.syncVarHighlightColor);
                }
                return _syncVarGUIStyle;
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            if (!targetObject) return;

            hudType = Caches.TypeCache.Get(targetObject.GetType().FullName + "HUD");
            windowType = Caches.TypeCache.Get(targetObject.GetType().FullName + "Window");

            option = MMOOption.weakInstance;
            allSyncVars = mb.GetAllSyncVarNames();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }

        protected override void OnOptionModify(Option option)
        {
            base.OnOptionModify(option);
            if(option is MMOOption mMOOption)
            {
                this.option = mMOOption;
                _syncVarGUIStyle = null;
                Repaint();
            }
        }

        /// <summary>
        /// 当绘制检查器GUI
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (hudType != null && !targetObject.GetComponent(hudType))
            {
                if (GUILayout.Button(CommonFun.NameTip(hudType)))
                {
                    Undo.AddComponent(targetObject.gameObject, hudType);
                }
            }
            //if (windowType != null && !targetObject.GetComponent(windowType))
            //{
            //    //if (GUILayout.Button(CommonFun.NameTip(windowType)))
            //    //{
            //    //    Undo.AddComponent(targetObject.gameObject, windowType);
            //    //}
            //}
        }

        /// <summary>
        /// 当绘制成员
        /// </summary>
        /// <param name="serializedProperty"></param>
        protected override void OnDrawMember(SerializedProperty serializedProperty, PropertyData propertyData)
        {
            if (IsSyncVar(serializedProperty) && option.syncVarHighlight)
            {
                EditorGUILayout.BeginVertical(syncVarGUIStyle);
                base.OnDrawMember(serializedProperty, propertyData);
                EditorGUILayout.EndVertical();
                return;
            }
            base.OnDrawMember(serializedProperty, propertyData);
        }

        private bool IsSyncVar(SerializedProperty memberProperty) => allSyncVars.Contains(memberProperty.name);

        /// <summary>
        /// 开始同步变量
        /// </summary>
        protected void BeginSyncVar()
        {
            if (option.syncVarHighlight) EditorGUILayout.BeginVertical(syncVarGUIStyle);
        }

        /// <summary>
        /// 结束同步变量
        /// </summary>
        protected void EndSyncVar()
        {
            if (option.syncVarHighlight) EditorGUILayout.EndVertical();
        }
    }
}
