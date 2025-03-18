using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCameras.Base;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginsCameras.Base;
using XCSJ.PluginsCameras.Controllers;

namespace XCSJ.EditorCameras.Controllers
{
    /// <summary>
    /// 相机管理器提供者检查器
    /// </summary>
    [Name("相机管理器提供者检查器")]
    [CustomEditor(typeof(CameraManagerProvider), true)]
    public class CameraManagerProviderInspector : BaseCameraManagerProviderInspector<CameraManagerProvider>
    {
        /// <summary>
        /// 当绘制检查器GUI
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            DrawCameraControllers();
        }

        /// <summary>
        /// 相机控制器列表
        /// </summary>
        [Name("相机控制器列表")]
        [Tip("当前场景中所有的相机控制器对象", "All camera controller objects in the current scene")]
        private static bool _display = true;

        private void DrawCameraControllers()
        {
            _display = UICommonFun.Foldout(_display, CommonFun.NameTip(GetType(), nameof(_display)));
            if (!_display) return;
            CommonFun.BeginLayout();

            DrawCameraControllersInternal(targetObject);

            CommonFun.EndLayout();
        }

        internal static void DrawCameraControllersInternal(CameraManagerProvider provider)
        {
            DrawCameraControllersInternal(provider, ComponentCache.Get(typeof(BaseCameraMainController), true).components);
        }

        /// <summary>
        /// 相机控制器
        /// </summary>
        [Name("相机控制器")]
        [Tip("相机控制器所在的游戏对象；本项只读；", "The game object where the camera controller is located; This item is read-only;")]
        public bool cameraController;

        /// <summary>
        /// 相机拥有者
        /// </summary>
        [Name("相机拥有者")]
        [Tip("相机控制器拥有者所在的游戏对象；本项只读；", "The game object of the camera controller owner; This item is read-only;")]
        public bool cameraOwner;

        /// <summary>
        /// 激活
        /// </summary>
        [Name("激活")]
        [Tip("相机控制器组件与所在游戏对象（包含父级游戏对象）是否激活并启用；如父层级不激活，当前相机控制器也不激活；本项只读；", "Whether the camera controller component and the game object (including the parent game object) are activated and enabled; If the current level controller is not activated, the parent camera is not activated; This item is read-only;")]
        public bool activation;

        /// <summary>
        /// 启用
        /// </summary>
        [Name("启用")]
        [Tip("相机控制器组件与当前所在游戏对象（不考虑父级游戏对象）是否激活并启用；编辑态可修改，运行态只读；", "Whether the camera controller component and the current game object (regardless of the parent game object) are activated and enabled; It can be modified in editing status and read-only in running status;")]
        public bool enable;

        /// <summary>
        /// 当前
        /// </summary>
        [Name("当前")]
        [Tip("表示是否是当前相机控制器；本项只读；", "")]
        public bool current;

        /// <summary>
        /// 初始
        /// </summary>
        [Name("初始")]
        [Tip("设置为初始相机控制器；同一时间至多仅有一个相机控制器可被设置为初始相机控制器；编辑态可修改，运行态只读；", "Set as the initial camera controller; At most one camera controller can be set as the initial camera controller at the same time; It can be modified in editing status and read-only in running status;")]
        public bool initial;

        internal static void DrawCameraControllersInternal(CameraManagerProvider provider, IList<Component> components)
        {
            if (!provider || components == null) return;

            #region 标题            

            EditorGUILayout.BeginHorizontal(GUI.skin.box);

            GUILayout.Label("NO.", UICommonOption.Width32);
            GUILayout.Label(typeof(CameraManagerProviderInspector).TrLabel(nameof(CameraManagerProviderInspector.cameraController)));
            GUILayout.Label(typeof(CameraManagerProviderInspector).TrLabel(nameof(CameraManagerProviderInspector.cameraOwner)));
            GUILayout.Label(typeof(CameraManagerProviderInspector).TrLabel(nameof(CameraManagerProviderInspector.activation)), UICommonOption.Width64);
            GUILayout.Label(typeof(CameraManagerProviderInspector).TrLabel(nameof(CameraManagerProviderInspector.enable)), UICommonOption.Width64);
            GUILayout.Label(typeof(CameraManagerProviderInspector).TrLabel(nameof(CameraManagerProviderInspector.current)), UICommonOption.Width64);
            GUILayout.Label(typeof(CameraManagerProviderInspector).TrLabel(nameof(CameraManagerProviderInspector.initial)), UICommonOption.Width64);

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Separator();

            #endregion

            var isPlaying = Application.isPlaying;
            var current = provider.currentCameraController;
            var init = provider.initCameraController;
            var count = components.Count;
            for (int i = 0; i < count; i++)
            {
                var component = components[i] as BaseCameraMainController;
                if (!init && component.isActiveAndEnabled)
                {
                    //如果初始相机控制器无效，则使用列表中第一个激活并启用的相机控制器作为初始相机控制器
                    provider.initCameraController = init = component;
                }

                UICommonFun.BeginHorizontal(i);

                //编号
                EditorGUILayout.LabelField((i + 1).ToString(), UICommonOption.Width32);

                //相机控制器
                var gameObject = component.gameObject;
                EditorGUILayout.ObjectField(gameObject, typeof(GameObject), true);

                //相机拥有者
                var cameraOwner = component.cameraOwner.ownerGameObject;
                EditorGUILayout.ObjectField(cameraOwner, typeof(GameObject), true);

                EditorGUI.BeginDisabledGroup(isPlaying);
                {
                    //激活
                    EditorGUILayout.Toggle(component.isActiveAndEnabled, UICommonOption.Width64);

                    //启用
                    var enable = component.enabled && component.gameObject.activeSelf;
                    if (EditorGUILayout.Toggle(enable, UICommonOption.Width64) != enable)
                    {
                        if (enable)
                        {
                            //相机控制器所在游戏对象取消激活
                            component.gameObject.XSetActive(false);
                            component.XSetEnable(false);
                        }
                        else
                        {
                            //相机控制器所在游戏对象激活
                            component.gameObject.XSetActive(true);
                            component.XSetEnable(true);
                        }
                    }

                    //当前
                    EditorGUILayout.Toggle(component == current, UICommonOption.Width64);

                    //初始
                    if (EditorGUILayout.Toggle(component == init, UICommonOption.Width64))
                    {
                        if (component != init)
                        {
                            provider.initCameraController = component;
                            init = provider.initCameraController;
                        }
                    }
                }
                EditorGUI.EndDisabledGroup();

                UICommonFun.EndHorizontal();
            }
        }
    }
}
