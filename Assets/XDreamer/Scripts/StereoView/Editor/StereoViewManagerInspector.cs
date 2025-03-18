using System.Linq;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Rendering;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension.Base;
using XCSJ.EditorXRSpaceSolution;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginStereoView;
using XCSJ.PluginStereoView.Tools;

namespace XCSJ.EditorStereoView
{
    /// <summary>
    /// 立体显示管理器检查器
    /// </summary>
    [Name("立体显示管理器检查器")]
    [CustomEditor(typeof(StereoViewManager))]
    public class StereoViewManagerInspector : BaseManagerInspector<StereoViewManager>
    {
        /// <summary>
        /// 当绘制检查器GUI
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorXRSpaceSolutionHelper.DrawSelectManager();
            EditorStereoViewHelper.DrawSettingActiveStereoConfig();

            DrawDetailInfos();
            DrawCameraProjectionDetailInfos();
        }

        /// <summary>
        /// 虚拟屏幕列表
        /// </summary>
        [Name("虚拟屏幕列表")]
        [Tip("当前场景中所有的虚拟屏幕对象", "All virtual screen objects in the current scene")]
        private static bool _display = true;

        [Name("虚拟屏幕")]
        [Tip("虚拟屏幕所在的游戏对象；本项只读；", "The game object where the virtual screen is located; This item is read-only;")]
        public bool virtualScreen;

        [Name("屏幕尺寸")]
        [Tip("虚拟屏幕的真实物理尺寸；X为宽，Y为高,Z为厚度；单位：米；", "The real physical size of the virtual screen; X is width, y is height and Z is thickness; Unit: meter;")]
        public bool screenSize;

        private void DrawDetailInfos()
        {
            _display = UICommonFun.Foldout(_display, CommonFun.NameTip(GetType(), nameof(_display)));
            if (!_display) return;

            CommonFun.BeginLayout();

#region 标题            

            EditorGUILayout.BeginHorizontal(GUI.skin.box);

            GUILayout.Label("NO.", UICommonOption.Width32);
            GUILayout.Label(TrLabel(nameof(virtualScreen)), UICommonOption.Width200);
            GUILayout.Label(TrLabel(nameof(screenSize)));

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Separator();

#endregion

            var cache = ComponentCache.Get(typeof(VirtualScreen), true);
            for (int i = 0; i < cache.components.Length; i++)
            {
                var component = cache.components[i] as VirtualScreen;

                UICommonFun.BeginHorizontal(i);

                //编号
                EditorGUILayout.LabelField((i + 1).ToString(), UICommonOption.Width32);

                //虚拟屏幕
                var gameObject = component.gameObject;
                EditorGUILayout.ObjectField(gameObject, typeof(GameObject), true, UICommonOption.Width200);

                //屏幕尺寸
                EditorGUI.BeginChangeCheck();
                var screenSize = EditorGUILayout.Vector3Field("", component.screenSize);
                if (EditorGUI.EndChangeCheck())
                {
                    component.screenSize = screenSize;
                }

                UICommonFun.EndHorizontal();
            }

            CommonFun.EndLayout();
        }

        /// <summary>
        /// 相机透视列表
        /// </summary>
        [Name("相机透视列表")]
        [Tip("当前场景中所有的相机透视组件对象", "All CameraProjection component objects in the current scene")]
        public bool cameraProjections = true;

        [Name("相机透视")]
        [Tip("相机透视所在的游戏对象；本项只读；", "The game object where the CameraProjection is located; This item is read-only;")]
        public bool cameraProjection;

        [Name("关联屏幕")]
        [Tip("相机透视的关联屏幕；", "Associated screen for camera projection;")]
        public bool linkScreen;

        private void DrawCameraProjectionDetailInfos()
        {
            cameraProjections = UICommonFun.Foldout(cameraProjections, TrLabel(nameof(cameraProjections)));
            if (!cameraProjections) return;

            CommonFun.BeginLayout();

#region 标题            

            EditorGUILayout.BeginHorizontal(GUI.skin.box);

            GUILayout.Label("NO.", UICommonOption.Width32);
            GUILayout.Label(TrLabel(nameof(cameraProjection)));
            GUILayout.Label(TrLabel(nameof(linkScreen)), UICommonOption.Width120);

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Separator();

#endregion

            var cache = ComponentCache.Get(typeof(CameraProjection), true);
            for (int i = 0; i < cache.components.Length; i++)
            {
                var component = cache.components[i] as CameraProjection;

                UICommonFun.BeginHorizontal(i);

                //编号
                EditorGUILayout.LabelField((i + 1).ToString(), UICommonOption.Width32);

                //相机透视
                var gameObject = component.gameObject;
                EditorGUILayout.ObjectField(gameObject, typeof(GameObject), true);

                //关联屏幕
                EditorGUILayout.ObjectField(component.screen, typeof(BaseScreen), true, UICommonOption.Width120);

                UICommonFun.EndHorizontal();
            }

            CommonFun.EndLayout();
        }
    }
}
