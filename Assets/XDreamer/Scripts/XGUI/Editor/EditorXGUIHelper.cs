using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginCommonUtils;

namespace XCSJ.EditorXGUI
{
    /// <summary>
    /// XGUI编辑器助手
    /// </summary>
    public static class EditorXGUIHelper
    {
        #region XGUI创建

        public const int menuIndex = 20;

        /// <summary>
        /// UI层名称
        /// </summary>
        public const string UILayerName = "UI";

        /// <summary>
        /// 资源路径
        /// </summary>
        public const string UIResourcePath = UILayerName + "/Skin/";

        /// <summary>
        /// unity内置资源
        /// </summary>
        public static DefaultControls.Resources defaultResources = new DefaultControls.Resources();

        [InitializeOnLoadMethod]
        public static void InitResource()
        {
            defaultResources.standard = AssetDatabase.GetBuiltinExtraResource<Sprite>(UIResourcePath + "UISprite.psd");
            defaultResources.background = AssetDatabase.GetBuiltinExtraResource<Sprite>(UIResourcePath + "Background.psd");
            defaultResources.inputField = AssetDatabase.GetBuiltinExtraResource<Sprite>(UIResourcePath + "InputFieldBackground.psd");
            defaultResources.knob = AssetDatabase.GetBuiltinExtraResource<Sprite>(UIResourcePath + "Knob.psd");
            defaultResources.checkmark = AssetDatabase.GetBuiltinExtraResource<Sprite>(UIResourcePath + "Checkmark.psd");
            defaultResources.dropdown = AssetDatabase.GetBuiltinExtraResource<Sprite>(UIResourcePath + "DropdownArrow.psd");
            defaultResources.mask = AssetDatabase.GetBuiltinExtraResource<Sprite>(UIResourcePath + "UIMask.psd");
        }

        /// <summary>
        /// 创建画布
        /// </summary>
        /// <returns></returns>
        public static GameObject CreateCanvas()
        {
            try
            {
                var go =  UnityObjectHelper.CreateGameObject(GameObjectUtility.GetUniqueNameForSibling(null, nameof(Canvas)));
                go.layer = LayerMask.NameToLayer(UILayerName);
                var canvas = go.XGetOrAddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                go.XGetOrAddComponent<CanvasScaler>();
                go.XGetOrAddComponent<GraphicRaycaster>();
                CreateEventSystem();
                return go;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        /// <summary>
        /// 创建事件系统
        /// </summary>
        public static void CreateEventSystem()
        {
            try
            {
                var esys = UnityEngine.Object.FindObjectOfType<EventSystem>();
                if (!esys)
                {
                    var go = UnityObjectHelper.CreateGameObject(nameof(EventSystem));
                    esys = go.XGetOrAddComponent<EventSystem>();
                    go.XGetOrAddComponent<StandaloneInputModule>();
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        /// <summary>
        /// 创建面板
        /// </summary>
        /// <returns></returns>
        public static GameObject CreatePanel()  => DefaultControls.CreatePanel(defaultResources);

        /// <summary>
        /// 创建UGUI基础元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T CreateUGUI<T>() where T : UIBehaviour
        {
            GameObject go = null;
            if (typeof(T) == typeof(Button))
            {
                go = DefaultControls.CreateButton(defaultResources);
            }
            else if (typeof(T) == typeof(Dropdown))
            {
                go = DefaultControls.CreateDropdown(defaultResources);
            }
            else if (typeof(T) == typeof(InputField))
            {
                go = DefaultControls.CreateInputField(defaultResources);
            }
            else if (typeof(T) == typeof(Image))
            {
                go = DefaultControls.CreateImage(defaultResources);
            }
            else if (typeof(T) == typeof(RawImage))
            {
                go = DefaultControls.CreateRawImage(defaultResources);
            }
            else if (typeof(T) == typeof(Scrollbar))
            {
                go = DefaultControls.CreateScrollbar(defaultResources);
            }
            else if (typeof(T) == typeof(ScrollRect))
            {
                go = DefaultControls.CreateScrollView(defaultResources);
            }
            else if (typeof(T) == typeof(Slider))
            {
                go = DefaultControls.CreateSlider(defaultResources);
            }
            else if (typeof(T) == typeof(Text))
            {
                go = DefaultControls.CreateText(defaultResources);
            }
            else if (typeof(T) == typeof(Toggle))
            {
                go = DefaultControls.CreateToggle(defaultResources);
            }

            var ui = default(T);
            if (go)
            {
                UndoHelper.RegisterCreatedObjectUndo(go);
                ui = go.GetComponent<T>();
            }

            return ui;
        }

        /// <summary>
        /// 创建滚动条
        /// </summary>
        /// <returns></returns>
        public static GameObject CreateScrollView()
        {
            var go = DefaultControls.CreateScrollView(defaultResources);
            if (go)
            {
                UndoHelper.RegisterCreatedObjectUndo(go);
            }
            return go;
        }

        /// <summary>
        /// 创建滚动视图
        /// </summary>
        /// <returns></returns>
        public static GameObject CreateScrollViewWithGridLayoutGroup()
        {
            try
            {
                var root = CreateScrollView();
                var content = root.transform.Find("Viewport/Content");
                if (content)
                {
                    content.gameObject.AddComponent<GridLayoutGroup>();
                    content.gameObject.AddComponent<ContentSizeFitter>();
                }
                return root;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        /// <summary>
        /// 创建滚动视图
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resources"></param>
        /// <param name="scrollSize"></param>
        /// <param name="cellSize"></param>
        /// <param name="CellSpaceSize"></param>
        /// <returns></returns>
        public static T CreateScrollView<T>(DefaultControls.Resources resources,
            Vector2 scrollSize, Vector2 cellSize, Vector2 CellSpaceSize) where T : MB
        {
            try
            {
                // 滚动视图            
                var root = CreateScrollViewWithGridLayoutGroup();
                root.GetComponent<RectTransform>().sizeDelta = scrollSize;
                var newComponent = root.AddComponent<T>();

                // 设置网格布局组件
                var glg = root.GetComponentInChildren<GridLayoutGroup>();
                glg.cellSize = cellSize;
                glg.spacing = CellSpaceSize;

                // 设置内容适配组件
                var csf = root.GetComponentInChildren<ContentSizeFitter>();
                csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

                return newComponent;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        /// <summary>
        /// 查找或创建在场景中，父节点为空的画布
        /// </summary>
        /// <returns></returns>
        public static GameObject FindOrCreateRootCanvas()
        {
            var canvas = UnityEngine.Object.FindObjectsOfType<Canvas>().Find(c => c.transform.parent == null);
            return canvas ? canvas.gameObject : CreateCanvas();
        }

        /// <summary>
        /// 设置游戏对象为画布子节点
        /// </summary>
        /// <returns></returns>
        public static void SetObjectToCanvas(GameObject obj)
        {
            try
            {
                SetGameObjectParent(obj, FindOrCreateRootCanvas());
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        /// <summary>
        /// 设置游戏对象为游戏对象子节点
        /// </summary>
        /// <returns></returns>
        public static void SetGameObjectParent(GameObject child, GameObject parent)
        {
            if (!child || !parent) return;

            child.name = GameObjectUtility.GetUniqueNameForSibling(parent.transform, child.name);

            SetLayerRecursively(child, parent.layer);

            child.transform.XSetTransformParent(parent.transform);
        }

        /// <summary>
        /// 设置循环层
        /// </summary>
        /// <returns></returns>
        private static void SetLayerRecursively(GameObject go, int layer)
        {
            go.layer = layer;
            for (int i = 0; i < go.transform.childCount; i++)
            {
                SetLayerRecursively(go.transform.GetChild(i).gameObject, layer);
            }
        }

        #endregion

        /// <summary>
        /// 创建UGUI按钮，并关联回调
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="buttonEnableFun"></param>
        /// <param name="callback"></param>
        public static void DrawCreateButton(bool buttonEnableFun, Action onClick) 
        {
            try
            {
                EditorGUI.BeginDisabledGroup(buttonEnableFun);
                if (GUILayout.Button(CommonFun.NameTip(EIcon.Add), EditorStyles.miniButtonRight, UICommonOption.WH24x16))
                {
                    onClick?.Invoke();
                }
            }
            finally
            {
                EditorGUI.EndDisabledGroup();
            }
        }
    }

}
