using System;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorTools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXGUI;
using XCSJ.PluginXGUI.Base;

namespace XCSJ.EditorXGUI
{
    /// <summary>
    /// XGUI管理器检查器
    /// </summary>
    [Name("XGUI管理器检查器")]
    [CustomEditor(typeof(XGUIManager), true)]
    public class XGUIManagerInspector : BaseManagerInspector<XGUIManager>
    {
        /// <summary>
        /// 当绘制检查器GUI
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            DrawSubWindowList();
        }

        /// <summary>
        /// 窗口列表
        /// </summary>
        [Name("窗口列表")]
        [Tip("当前场景中所有的子窗口对象", "All child window objects in the current scene")]
        public bool _display = true;

        /// <summary>
        /// 窗口
        /// </summary>
        [Name("窗口")]
        [Tip("窗口组件所在的游戏对象；本项只读；", "The game object where the window component is located; This item is read-only;")]
        public bool window;

        private void DrawSubWindowList()
        {
            _display = UICommonFun.Foldout(_display, CommonFun.NameTip(GetType(), nameof(_display)));
            if (!_display) return;

            CommonFun.BeginLayout();
            {
                // 标题
                EditorGUILayout.BeginHorizontal(GUI.skin.box);
                {
                    GUILayout.Label("NO.", UICommonOption.Width32);
                    GUILayout.Label(TrLabel(nameof(window)).text);
                }
                EditorGUILayout.EndHorizontal();

                // 子级内容
                int i = 0;
                Type type = null;
                foreach (var component in CommonFun.GetComponentsInChildren<SubWindow>(true))
                {
                    if (type == null) type = component.GetType();

                    UICommonFun.BeginHorizontal(i);
                    {
                        //编号
                        EditorGUILayout.LabelField((++i).ToString(), UICommonOption.Width32);

                        //组件
                        EditorGUILayout.ObjectField(component, type, true);
                    }
                    UICommonFun.EndHorizontal();
                }
            }
            CommonFun.EndLayout();
        }

    }
}
