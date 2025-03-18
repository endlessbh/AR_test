using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Caches;
using XCSJ.Collections;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorStereoView;
using XCSJ.EditorTools;
using XCSJ.EditorTools.Base;
using XCSJ.EditorXXR.Interaction.Toolkit;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginPeripheralDevice;
using XCSJ.PluginStereoView;
using XCSJ.PluginStereoView.Tools;
using XCSJ.PluginXBox;
using XCSJ.PluginXRSpaceSolution;
using XCSJ.PluginXRSpaceSolution.Tools;
using XCSJ.PluginXXR.Interaction.Toolkit;

namespace XCSJ.EditorXRSpaceSolution
{
    /// <summary>
    /// XR空间解决方案检查器
    /// </summary>
    [CustomEditor(typeof(XRSpaceSolutionManager))]
    [Name("XR空间解决方案检查器")]
    public class XRSpaceSolutionManagerInspector:BaseManagerInspector<XRSpaceSolutionManager>
    {
        bool allStarted = false;

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            if (manager && manager.hasAccess)
            {
                allStarted = XDreamer.StartManager(typeof(PeripheralDeviceInputManager), typeof(XXRInteractionToolkitManager), typeof(StereoViewManager), typeof(XBoxManager));
            }
            else
            {
                allStarted = false;
            }
        }

        /// <summary>
        /// 当绘制检查器GUI
        /// </summary>
        [LanguageTuple("Enable Required Plug-ins", "一键启用所需插件")]
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (!allStarted && manager.hasAccess)
            {
                if (GUILayout.Button(Tr("Enable Required Plug-ins")))
                {
                    XDreamer.StartManager(typeof(PeripheralDeviceInputManager), typeof(XXRInteractionToolkitManager), typeof(StereoViewManager), typeof(XBoxManager));
                }
            }            

            EditorXRITHelper.DrawSelectManager();
            EditorStereoViewHelper.DrawSelectManager();
            EditorStereoViewHelper.DrawSettingActiveStereoConfig();

            DrawXRSpaces();
        }

        /// <summary>
        /// XR空间列表
        /// </summary>
        [Name("XR空间列表")]
        [Tip("当前场景中所有的XR空间对象", "All XR space objects in the current scene")]
        public bool _display = true;

        /// <summary>
        /// XR空间
        /// </summary>
        [Name("XR空间")]
        [Tip("XR空间组件所在的游戏对象；本项只读；", "The game object where the XRSpace component is located; This item is read-only;")]
        public bool xrSpace;

        private void DrawXRSpaces()
        {
            _display = UICommonFun.Foldout(_display, CommonFun.NameTip(GetType(), nameof(_display)));
            if (!_display) return;

            CommonFun.BeginLayout();

            EditorGUILayout.BeginHorizontal(GUI.skin.box);
            GUILayout.Label("NO.", UICommonOption.Width32);
            GUILayout.Label(TrLabel(nameof(xrSpace)));

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Separator();

            var cache = ComponentCache.Get(typeof(XRSpace), true);
            for (int i = 0; i < cache.components.Length; i++)
            {
                var component = cache.components[i] as XRSpace;

                UICommonFun.BeginHorizontal(i);

                //编号
                EditorGUILayout.LabelField((i + 1).ToString(), UICommonOption.Width32);

                //窗口组件
                EditorGUILayout.ObjectField(component.gameObject, typeof(GameObject), true);

                UICommonFun.EndHorizontal();
            }

            CommonFun.EndLayout();
        }
    }
}
