using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;

namespace XCSJ.Extension.OSInteracts
{
    /// <summary>
    /// OS交互
    /// </summary>
    [DisallowMultipleComponent]
    [Name("OS交互")]
    [RequireManager(typeof(OSInteractManager))]
    public class OSInteract : InteractProvider, IOnEnable
    {
        /// <summary>
        /// 默认名
        /// </summary>
        public const string DefaultName = Product.Name + "_" + nameof(OSInteract);

        #region Unity --> OS


        public void UnityToOS(string jsonString)
        {
            try
            {
#if !UNITY_EDITOR

#if UNITY_IPHONE
            
                OSInteractManager.UnityToIOS(jsonString);
           
#elif UNITY_ANDROID
        
			    AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
                jo.Call("unityToAndroid", jsonString);

#elif UNITY_WEBGL

#if UNITY_5_6_OR_NEWER
                OSInteractManager.UnityToJS(jsonString);
#else
                Application.ExternalCall("UnityToJS", jsonString);
#endif// UNITY_Version

#endif

#endif// !UNITY_EDITOR
            }
            catch (Exception ex)
            {
                Log.Exception("Unity --> OS异常: " + ex.ToString());
            }
        }

        #endregion

        #region OS --> Unity

        public event Action<string> onOSToUnity;

        public void OSToUnity(string jsonString)
        {
            try
            {
                onOSToUnity?.Invoke(jsonString);
            }
            catch (Exception ex)
            {
                Log.Exception("OS --> Unity异常: " + ex.ToString());
            }
        }

        #endregion

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            name = DefaultName;
            base.OnEnable();
        }

        /// <summary>
        /// 查找或创建
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        public static OSInteract FindOrCreate(OSInteractManager manager)
        {
            return FindOrCreate(manager ? manager.gameObject : null);
        }

        /// <summary>
        /// 查找或创建
        /// </summary>
        /// <param name="parentGO"></param>
        /// <returns></returns>
        public static OSInteract FindOrCreate(GameObject parentGO)
        {
            if (!parentGO) return null;
            var interact = parentGO.GetComponentInChildren<OSInteract>();
            if (interact) return interact;

            var go = CommonFun.GetChildGameObject(parentGO.transform, DefaultName);
            if (go) return CommonFun.GetOrAddComponent<OSInteract>(go);

            go = CommonFun.CreateGameObjectWithUniqueName(parentGO.gameObject, DefaultName);
            return CommonFun.GetOrAddComponent<OSInteract>(go);
        }
    }
}
