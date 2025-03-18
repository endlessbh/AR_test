using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.Languages;
using XCSJ.Maths;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginMMO.NetSyncs
{
    /// <summary>
    /// 网络属性HUD
    /// </summary>
    [DisallowMultipleComponent]
    [ExecuteInEditMode]
    [Name("网络属性HUD", "Net Property HUD")]
    [RequireComponent(typeof(NetProperty))]
    [RequireManager(typeof(MMOManager))]
    public class NetPropertyHUD : InteractProvider, IAwake, IStart, IOnGUI, IReset, IOnEnable, IOnDisable
    {
        /// <summary>
        /// 属性
        /// </summary>
        public NetProperty property { get; private set; }

        /// <summary>
        /// HUD窗口
        /// </summary>
        [Name("HUD窗口")]
        public HUDWindow window = new HUDWindow();

        /// <summary>
        /// 唤醒初始化
        /// </summary>
        public virtual void Awake()
        {
            window.HUD = this;
            window.property = property = GetComponent<NetProperty>();
        }

        /// <summary>
        /// 启用
        /// </summary>
        public virtual void Start()
        {
            window.Start();
        }

        /// <summary>
        /// 绘制GUI
        /// </summary>
        public virtual void OnGUI()
        {
            window.OnGUI();
        }

        /// <summary>
        /// 窗口
        /// </summary>
        public virtual void Reset()
        {
            window.visable = true;
            window.rect = new Rect(0, 0, 270, 180);
            window._alignMode = ERectAnchor.Center;
        }

        /// <summary>
        /// HUD窗口
        /// </summary>
        [Serializable]
        [Name("HUD窗口")]
        public class HUDWindow : BaseGUIWindow, IStart
        {
            internal NetPropertyHUD HUD;

            internal NetProperty property;

            /// <summary>
            /// 启动时对齐
            /// </summary>
            [Name("启动时对齐")]
            public bool alignOnStart = true;

            public override bool autoLayout => true;

            private GUIStyle m_BoxStyle = null;

            public GUIStyle boxStyle
            {
                get
                {
                    if (m_BoxStyle == null)
                    {
                        m_BoxStyle = new GUIStyle(GUI.skin.box);
                        m_BoxStyle.alignment = TextAnchor.UpperLeft;
                    }
                    return m_BoxStyle;
                }
            }

            public void Start()
            {
#if UNITY_WEBGL && !UNITY_EDITOR
                _title = CommonFun.Name(HUD.GetType(), ELanguageType.English);
#else
                _title = CommonFun.Name(HUD.GetType(), ELanguageType.Chinese);
#endif
                if (alignOnStart) SetWindowPositionInScreen(_alignMode);
                scrollPosition = new Vector2(1, 1);
            }

            protected override void OnDrawContentLayout()
            {
                if (!HUD || !property) return;

                GUILayout.BeginHorizontal(GUI.skin.box);
#if UNITY_WEBGL && !UNITY_EDITOR
                GUILayout.Label("Name", GUILayout.Width(120));
                GUILayout.Label("Value");
#else
                GUILayout.Label("名称", GUILayout.Width(120));
                GUILayout.Label("值");
#endif
                GUILayout.EndHorizontal();

                for (int i = 0; i < property.propertys.Count; i++)
                {
                    var p = property.propertys[i];

                    GUILayout.BeginHorizontal();
                    GUILayout.Button(p.name, GUI.skin.box, GUILayout.Width(120));
                    GUILayout.Button(p.value, CommonGUIStyle.middleLeftBox);
                    GUILayout.EndHorizontal();
                }
            }
        }
    }
}
