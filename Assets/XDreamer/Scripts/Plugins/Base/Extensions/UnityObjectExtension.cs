using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Extension.Base.Maths;
using XCSJ.Maths;
using XCSJ.PluginCommonUtils;

namespace XCSJ.Extension.Base.Extensions
{
    /// <summary>
    /// Unity对象扩展：针对<see cref="UnityObjectHelper"/>的明文扩展类；所有方法均会回调相应事件，详细参考类Unity对象处理器事件<see cref="UnityObjectHandlerEvent"/>；所有方法支持在Unity编辑器中执行撤销与重做；
    /// </summary>
    public static class UnityObjectExtension
    {
        /// <summary>
        /// 创建画布
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parent"></param>
        /// <param name="needUniqueName"></param>
        /// <param name="resetLocalPRS"></param>
        /// <returns></returns>
        public static Canvas CreateCanvas(string name = "", Transform parent = null, bool needUniqueName = true, bool resetLocalPRS = true)
        {
            var canvas = CreateGameObject<Canvas>(name, parent, needUniqueName, resetLocalPRS);
            canvas.XGetOrAddComponent<CanvasScaler>();
            canvas.XGetOrAddComponent<GraphicRaycaster>();
            return canvas;
        }

        /// <summary>
        /// 创建游戏对象，并同步添加指定类型组件、设置层级
        /// </summary>
        /// <typeparam name="TComponent"></typeparam>
        /// <param name="name"></param>
        /// <param name="needUniqueName"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static TComponent CreateGameObject<TComponent>(string name = "", Transform parent = null, bool needUniqueName = true, bool resetLocalPRS = true) where TComponent : Component
        {
            if (string.IsNullOrEmpty(name))
            {
                name = CommonFun.Name(typeof(TComponent));
            }
            var gameObject = UnityObjectHelper.CreateGameObject(name);
            if (parent)
            {
                gameObject.XSetParent(parent);
            }
            if (needUniqueName)
            {
                gameObject.XSetUniqueName(name);
            }
            if (resetLocalPRS)
            {
                gameObject.transform.XResetLocalPRS();
            }
            return gameObject.XGetOrAddComponent<TComponent>();
        }

        /// <summary>
        /// 矩形变换设置为水平垂直拉伸
        /// </summary>
        /// <param name="rectTransform"></param>
        public static void XStretchHV(this RectTransform rectTransform)
        {
            rectTransform.XSetAnchorMin(Vector2.zero);
            rectTransform.XSetAnchorMax(Vector2.one);

            rectTransform.XSetOffsetMin(Vector2.zero);
            rectTransform.XSetOffsetMax(Vector2.zero);

            rectTransform.XScaleToOne();
        }

        /// <summary>
        /// 矩形变换设置为水平垂直居中
        /// </summary>
        /// <param name="rectTransform"></param>
        public static void XCenterHV(this RectTransform rectTransform)
        {
            rectTransform.XSetAnchoredPosition(Vector2.zero);

            rectTransform.XScaleToOne();
        }

        /// <summary>
        /// 缩放至1
        /// </summary>
        /// <param name="rectTransform"></param>
        public static void XScaleToOne(this RectTransform rectTransform)
        {
            rectTransform.XSetLocalScale(Vector3.one);
        }

        /// <summary>
        /// 为组件所在游戏对象创建带指定类型组件的子级游戏对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="component">本对象必须有效</param>
        /// <param name="name">子级游戏对象的名字</param>
        /// <param name="resetLocalPRS">重置本地PRS</param>
        /// <returns></returns>
        public static T XCreateChild<T>(this Component component, string name = null, bool resetLocalPRS = true, bool needUniqueName = true) where T : Component
        {
            if (!component) return default;
            return XCreateChild<T>(component.gameObject, name, resetLocalPRS, needUniqueName);
        }

        /// <summary>
        /// 为游戏对象创建带指定类型组件的子级游戏对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="gameObject">本对象必须有效</param>
        /// <param name="name">子级游戏对象的名字</param>
        /// <returns></returns>
        public static T XCreateChild<T>(this GameObject gameObject, string name = null, bool resetLocalPRS = true, bool needUniqueName = true) where T : Component
        {
            if (!gameObject) return default;
            name = string.IsNullOrEmpty(name) ? CommonFun.Name(typeof(T)) : name;
            var go = UnityObjectHelper.CreateGameObject(name);
            go.XSetParent(gameObject);
            if (resetLocalPRS) go.transform.XResetLocalPRS();
            if (needUniqueName) go.XSetUniqueName(name);
            return go.XGetOrAddComponent<T>();
        }

        /// <summary>
        /// 克隆游戏对象并设置父级
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="parent"></param>
        /// <param name="name"></param>
        /// <param name="resetLocalPRS"></param>
        /// <param name="needUniqueName"></param>
        /// <returns></returns>
        public static GameObject XCloneAndSetParent(this GameObject gameObject, GameObject parent, string name = null, bool resetLocalPRS = true, bool needUniqueName = true)
        {
            return XCloneAndSetParent(gameObject, parent? parent.transform:null, name, resetLocalPRS, needUniqueName);
        }

        /// <summary>
        /// 克隆游戏对象并设置父级
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="parent"></param>
        /// <param name="name"></param>
        /// <param name="resetLocalPRS"></param>
        /// <param name="needUniqueName"></param>
        /// <returns></returns>
        public static GameObject XCloneAndSetParent(this GameObject gameObject, Transform parent, string name = null, bool resetLocalPRS = true, bool needUniqueName = true)
        {
            if (!gameObject) return default;

            var newGO = gameObject.XCloneObject();

            name = string.IsNullOrEmpty(name) ? gameObject.name : name;
            newGO.XSetParent(parent);
            if (resetLocalPRS) newGO.transform.XResetLocalPRS();
            if (needUniqueName) newGO.XSetUniqueName(name);
            return newGO;
        }

        /// <summary>
        /// 获取组件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mainComponent"></param>
        /// <param name="component"></param>
        /// <returns></returns>
        public static T XGetComponent<T>(this Component mainComponent, ref T component) where T : Component
        {
            if (!component)
            {
                var newComponent = mainComponent.GetComponent<T>();
                if (newComponent) mainComponent.XModifyProperty(ref component, newComponent);
            }
            return component;
        }

        /// <summary>
        /// 获取组件从子级
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mainComponent">主组件：存储组件的主组件</param>
        /// <param name="component">组件：如果本值有效，直接返回；否者设置本值并返回；</param>
        /// <param name="includeInactive">包含非激活的</param>
        /// <returns></returns>
        public static T XGetComponentInChildren<T>(this Component mainComponent, ref T component, bool includeInactive = true) where T : Component
        {
            if (!component)
            {
                var newComponent = mainComponent.GetComponentInChildren<T>(includeInactive);
                if (newComponent) mainComponent.XModifyProperty(ref component, newComponent);
            }
            return component;
        }

        /// <summary>
        /// 获取组件从子级或全局
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mainComponent">主组件：存储组件的主组件</param>
        /// <param name="component">组件：如果本值有效，直接返回；否者设置本值并返回；</param>
        /// <param name="includeInactive">包含非激活的</param>
        /// <returns></returns>
        public static T XGetComponentInChildrenOrGlobal<T>(this Component mainComponent, ref T component, bool includeInactive = true) where T : Component
        {
            if (!component)
            {
                var newComponent = XGetComponentInChildrenOrGlobal<T>(mainComponent, includeInactive);
                if (newComponent) mainComponent.XModifyProperty(ref component, newComponent);
            }
            return component;
        }

        /// <summary>
        /// 获取组件从子级或全局
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mainComponent">主组件：存储组件的主组件</param>
        /// <param name="includeInactive">包含非激活的</param>
        /// <returns></returns>
        public static T XGetComponentInChildrenOrGlobal<T>(this Component mainComponent, bool includeInactive = true) where T : Component
        {
            var newComponent = mainComponent.GetComponentInChildren<T>(includeInactive);
            if (newComponent) return newComponent;

#if UNITY_2020_3_OR_NEWER
            newComponent = UnityEngine.Object.FindObjectOfType<T>(includeInactive);
#else
            newComponent = UnityEngine.Object.FindObjectOfType<T>();
#endif
            if (newComponent) return newComponent;

            return CommonFun.GetComponentsInChildren<T>(includeInactive).FirstOrDefault();
        }

        /// <summary>
        /// 获取组件从子级或全局
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="gameObject">游戏对象</param>
        /// <param name="includeInactive">包含非激活的</param>
        /// <returns></returns>
        public static T XGetComponentInChildrenOrGlobal<T>(this GameObject gameObject, bool includeInactive = true) where T : Component
        {
            return gameObject ? XGetComponentInChildrenOrGlobal<T>(gameObject.transform, includeInactive): XGetComponentInGlobal<T>(includeInactive);
        }

        /// <summary>
        /// 获取组件从父级、父级的子级或全局
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mainComponent">主组件：存储组件的主组件</param>
        /// <param name="component">组件：如果本值有效，直接返回；否者设置本值并返回；</param>
        /// <param name="includeInactive">包含非激活的</param>
        /// <returns></returns>
        public static T XGetComponentInParentOrParentChildrenOrGlobal<T>(this Component mainComponent, ref T component, bool includeInactive = true) where T : Component
        {
            if (!component)
            {
                var newComponent = XGetComponentInParentOrParentChildrenOrGlobal<T>(mainComponent, includeInactive);
                if (newComponent) mainComponent.XModifyProperty(ref component, newComponent);
            }
            return component;
        }

        /// <summary>
        /// 获取组件从父级、父级的子级或全局
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mainComponent">主组件：存储组件的主组件</param>
        /// <param name="component">组件：如果本值有效，直接返回；否者设置本值并返回；</param>
        /// <param name="includeInactive">包含非激活的</param>
        /// <returns></returns>
        public static T XGetComponentInParentOrParentChildrenOrGlobal<T>(this Component mainComponent, bool includeInactive = true) where T : Component
        {
            //父级找
            var newComponent = mainComponent.GetComponentsInParent<T>(includeInactive).FirstOrDefault();
            if (newComponent) return newComponent;

            //父级的子级找
            var transform = mainComponent.transform;
            while (transform)
            {
                newComponent = transform.GetComponentInChildren<T>(includeInactive);
                if (newComponent) return newComponent;

                transform = transform.parent;
            }

            //全局找
#if UNITY_2020_3_OR_NEWER
            newComponent = UnityEngine.Object.FindObjectOfType<T>(includeInactive);
#else
            newComponent = UnityEngine.Object.FindObjectOfType<T>();
#endif
            return newComponent;
        }

        /// <summary>
        /// 获取组件从父级
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mainComponent">主组件：存储组件的主组件</param>
        /// <param name="component">组件：如果本值有效，直接返回；否者设置本值并返回；</param>
        /// <param name="includeInactive">包含非激活的</param>
        /// <returns></returns>
        public static T XGetComponentInParent<T>(this Component mainComponent, ref T component, bool includeInactive = true) where T : Component
        {
            if (!component)
            {
                var newComponent = mainComponent.GetComponentsInParent<T>(includeInactive).FirstOrDefault();
                if (newComponent) mainComponent.XModifyProperty(ref component, newComponent);
            }
            return component;
        }

        /// <summary>
        /// 获取组件从父级或全局
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mainComponent">主组件：存储组件的主组件</param>
        /// <param name="component">组件：如果本值有效，直接返回；否者设置本值并返回；</param>
        /// <param name="includeInactive">包含非激活的</param>
        /// <returns></returns>
        public static T XGetComponentInParentOrGlobal<T>(this Component mainComponent, ref T component, bool includeInactive = true) where T : Component
        {
            if (!component)
            {
                var newComponent = XGetComponentInParentOrGlobal<T>(mainComponent, includeInactive);
                if (newComponent) mainComponent.XModifyProperty(ref component, newComponent);
            }
            return component;
        }

        /// <summary>
        /// 获取组件从父级或全局
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mainComponent">主组件：存储组件的主组件</param>
        /// <param name="includeInactive">包含非激活的</param>
        /// <returns></returns>
        public static T XGetComponentInParentOrGlobal<T>(this Component mainComponent, bool includeInactive = true) where T : Component
        {
            var newComponent = mainComponent.GetComponentsInParent<T>(includeInactive).FirstOrDefault();
            if (newComponent) return newComponent;

#if UNITY_2020_3_OR_NEWER
            newComponent = UnityEngine.Object.FindObjectOfType<T>(includeInactive);
#else
            newComponent = UnityEngine.Object.FindObjectOfType<T>();
#endif
            if (newComponent) return newComponent;

            return CommonFun.GetComponentsInChildren<T>(includeInactive).FirstOrDefault();
        }

        /// <summary>
        /// 获取组件从父级或全局
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mainComponent">主组件：存储组件的主组件</param>
        /// <param name="includeInactive">包含非激活的</param>
        /// <returns></returns>
        public static T XGetComponentInParentOrGlobal<T>(this GameObject gameObject, bool includeInactive = true) where T : Component
        {
            return gameObject ? XGetComponentInParentOrGlobal<T>(gameObject.transform, includeInactive) : XGetComponentInGlobal<T>(includeInactive);
        }

        /// <summary>
        /// 获取组件从全局
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mainObject">主对象：存储组件的主对象</param>
        /// <param name="component">组件：如果本值有效，直接返回；否者设置本值并返回；</param>
        /// <param name="includeInactive">包含非激活的</param>
        /// <returns></returns>
        public static T XGetComponentInGlobal<T>(this UnityEngine.Object mainObject, ref T component, bool includeInactive = true) where T : Component
        {
            if (!component)
            {
                var newComponent = XGetComponentInGlobal<T>(includeInactive);
                if (newComponent) mainObject.XModifyProperty(ref component, newComponent);
            }
            return component;
        }

        /// <summary>
        /// 获取组件从全局
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mainObject">主对象：存储组件的主对象</param>
        /// <param name="includeInactive">包含非激活的</param>
        /// <returns></returns>
        public static T XGetComponentInGlobal<T>(bool includeInactive = true) where T : Component
        {
#if UNITY_2020_3_OR_NEWER
            var newComponent = UnityEngine.Object.FindObjectOfType<T>(includeInactive);
#else
            var newComponent = UnityEngine.Object.FindObjectOfType<T>();
#endif
            if (newComponent) return newComponent;

            return CommonFun.GetComponentsInChildren<T>(includeInactive).FirstOrDefault();
        }

        /// <summary>
        /// 获取组件从全局
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mainObject">主对象：存储组件的主对象</param>
        /// <param name="component">组件：如果本值有效，直接返回；否者设置本值并返回；</param>
        /// <param name="func">检测组件是否符合要求</param>
        /// <param name="includeInactive">包含非激活的</param>
        /// <returns></returns>
        public static T XGetComponentInGlobal<T>(this UnityEngine.Object mainObject, ref T component, Func<T, bool> func, bool includeInactive = true) where T : Component
        {
            if (!component)
            {
                var newComponent = GetComponentInGlobal(func, includeInactive);
                if (newComponent) mainObject.XModifyProperty(ref component, newComponent);
            }
            return component;
        }

        /// <summary>
        /// 获取根组件从全局；根组件：挂载在根游戏对象（无父级的游戏对象成为根游戏对象）上的组件；
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mainObject">主对象：存储组件的主对象</param>
        /// <param name="component">组件：如果本值有效，直接返回；否者设置本值并返回；</param>
        /// <param name="includeInactive">包含非激活的</param>
        /// <returns></returns>
        public static T XGetRootComponentInGlobal<T>(this UnityEngine.Object mainObject, ref T component, bool includeInactive = true) where T : Component
        {
            if (!component)
            {
                var newComponent = GetRootComponentInGlobal<T>(includeInactive);
                if (newComponent) mainObject.XModifyProperty(ref component, newComponent);
            }
            return component;
        }


        /// <summary>
        /// 获取组件从全局
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="includeInactive">包含非激活的</param>
        /// <returns></returns>
        public static T GetComponentInGlobal<T>(bool includeInactive = true) where T : Component
        {
#if UNITY_2020_3_OR_NEWER
            return UnityEngine.Object.FindObjectOfType<T>(includeInactive);
#else
            return UnityEngine.Object.FindObjectOfType<T>();
#endif
        }

        /// <summary>
        /// 获取组件从全局
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func">检测组件是否符合要求</param>
        /// <param name="includeInactive">包含非激活的</param>
        /// <returns></returns>
        public static T GetComponentInGlobal<T>(Func<T, bool> func, bool includeInactive = true) where T : Component
        {
#if UNITY_2020_3_OR_NEWER
            return UnityEngine.Object.FindObjectsOfType<T>(includeInactive)?.FirstOrDefault(func);
#else
            return UnityEngine.Object.FindObjectsOfType<T>()?.FirstOrDefault(func);
#endif
        }

        /// <summary>
        /// 从全局获取组件数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="includeInactive">包含非激活的</param>
        /// <returns></returns>
        public static T[] GetComponentsInGlobal<T>(bool includeInactive = true) where T : Component
        {
#if UNITY_2020_3_OR_NEWER
            return UnityEngine.Object.FindObjectsOfType<T>(includeInactive);
#else
            return UnityEngine.Object.FindObjectsOfType<T>();
#endif
        }

        /// <summary>
        /// 获取根组件从全局；根组件：挂载在根游戏对象（无父级的游戏对象成为根游戏对象）上的组件；
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="includeInactive">包含非激活的</param>
        /// <returns></returns>
        public static T GetRootComponentInGlobal<T>(bool includeInactive = true) where T : Component
        {
            return GetComponentInGlobal<T>(c => !c.transform.parent, includeInactive);
        }

        /// <summary>
        /// 设置本地位置
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="localPosition"></param>
        public static void XSetLocalPosition(this Transform transform, V3F localPosition) => transform.XSetLocalPosition(localPosition.ToVector3());

        /// <summary>
        /// 设置本地旋转
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="localPosition"></param>
        public static void XSetLocalRotation(this Transform transform, V3F eulerAngles) => transform.XSetLocalRotation(eulerAngles.ToVector3());

        /// <summary>
        /// 从当前变换对象往父级遍历获取T类型组件，直至根游戏对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="transform"></param>
        /// <param name="action">获取变换当前父级的T类型组件回调函数（返回值为false时停止父级遍历）</param>
        public static void GetComponentByForeachTransformParent<T>(this Transform transform, Func<Transform, T, bool> action)
        {
            if (!transform || action == null) return;

            var p = transform.parent;
            while (p)
            {
                var m = p.GetComponent<T>();
                if (m != null)
                {
                    if (!action(p, m))
                    {
                        return;
                    }
                }
                p = p.parent;
            }
        }
    }
}
