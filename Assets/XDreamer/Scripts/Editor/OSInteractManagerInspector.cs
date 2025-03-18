using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;
using XCSJ.Extension;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginCommonUtils;
using XCSJ.Helper;
using XCSJ.EditorExtension.XGUI;
using XCSJ.EditorXGUI;
using XCSJ.Attributes;
using XCSJ.Languages;

namespace XCSJ.EditorExtension
{
    /// <summary>
    /// OS交互检查器
    /// </summary>
    [CustomEditor(typeof(OSInteractManager))]
    [Name("OS交互检查器")]
    public class OSInteractManagerInspector : BaseManagerInspector<OSInteractManager>
    {
        /// <summary>
        /// 当绘制检查器GUI
        /// </summary>
        [LanguageTuple("Add [Back OS] Button", "添加[返回OS]按钮")]
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button(Tr("Add [Back OS] Button")))
            {
                var go = UICommonFun.LoadAndInstantiateFromAssets<GameObject>(backOSPrefabPath);
                if (go)
                {
                    EditorXGUIHelper.SetObjectToCanvas(go);
                    EditorGUIUtility.PingObject(go);
                }
            }
        }

        private const string backOSPrefabPath = "Assets/XDreamer-Assets/基础/Prefabs/常用/返回OS按钮.prefab";
    }
}
