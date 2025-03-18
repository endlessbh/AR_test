
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Caches;
using XCSJ.Extension.Base.Attributes;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginEmbeddedBrowser;
using XCSJ.PluginSMS.Base;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Base;
using XCSJ.Scripts;

#if XDREAMER_EMBEDDED_BROWSER
using ZenFulcrum.EmbeddedBrowser;
using static ZenFulcrum.EmbeddedBrowser.BrowserNative;
#endif

namespace XCSJ.PluginEmbeddedBrowser.States
{
    /// <summary>
    /// 浏览器属性获取: 浏览器属性获取
    /// </summary>
    [ComponentMenu(EmbeddedBrowserHelper.Title + "/" + Title, typeof(EmbeddedBrowserManager))]
    [Name(Title, nameof(BrowserPropertyGet))]
    [Tip("浏览器属性获取")]
    [XCSJ.Attributes.Icon(EIcon.Property)]
    [Owner(typeof(EmbeddedBrowserManager))]
    public class BrowserPropertyGet : BasePropertyGet<BrowserPropertyGet>, IDropdownPopupAttribute, IPropertyPathList
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "浏览器属性获取";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(EmbeddedBrowserHelper.Title, typeof(EmbeddedBrowserManager))]
        [StateComponentMenu(EmbeddedBrowserHelper.Title + "/" + Title, typeof(EmbeddedBrowserManager))]
        [Name(Title, nameof(BrowserPropertyGet))]
        [Tip("浏览器属性获取")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

#if XDREAMER_EMBEDDED_BROWSER

        /// <summary>
        /// 浏览器:浏览器
        /// </summary>
        [Name("浏览器")]
        [Tip("浏览器")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ComponentPopup]
        public Browser _Browser;

        /// <summary>
        /// 浏览器:浏览器
        /// </summary>
        public Browser Browser => this.XGetComponentInGlobal(ref _Browser, true);

#endif

        /// <summary>
        /// 属性名称
        /// </summary>
        [Name("属性名称")]
#if UNITY_2019_3_OR_NEWER
        //[EnumPopup]
#else
        [EnumPopup]
#endif
        public EPropertyName _propertyName = EPropertyName.None;

        /// <summary>
        /// 属性名称
        /// </summary>
        [Name("属性名称")]
        public enum EPropertyName
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            [Tip("无", "None")]
            [EnumFieldName("无")]
            None,

#region 字段

            /// <summary>
            /// Allow Context Menu On(字段):
            /// </summary>
            [Name("Allow Context Menu On(字段)")]
            [EnumFieldName("字段/Allow Context Menu On")]
            Field_allowContextMenuOn = 1,

            /// <summary>
            /// Base Color(字段):
            /// </summary>
            [Name("Base Color(字段)")]
            [EnumFieldName("字段/Base Color")]
            Field_baseColor,

            /// <summary>
            /// Focus State(字段):
            /// </summary>
            [Name("Focus State(字段)")]
            [EnumFieldName("字段/Focus State")]
            Field_focusState,

            /// <summary>
            /// Generate Mipmap(字段):
            /// </summary>
            [Name("Generate Mipmap(字段)")]
            [EnumFieldName("字段/Generate Mipmap")]
            Field_generateMipmap,

            /// <summary>
            /// On Download Started(字段):
            /// </summary>
            [Name("On Download Started(字段)")]
            [EnumFieldName("字段/On Download Started")]
            Field_onDownloadStarted,

#endregion

#region 属性

            /// <summary>
            /// Local Url Prefix(属性):
            /// </summary>
            [Name("Local Url Prefix(属性)")]
            [EnumFieldName("属性/Local Url Prefix")]
            Property_LocalUrlPrefix = 1000,

            /// <summary>
            /// UI Handler(属性):
            /// </summary>
            [Name("UI Handler(属性)")]
            [EnumFieldName("属性/UI Handler")]
            Property_UIHandler,

            /// <summary>
            /// Enable Rendering(属性):
            /// </summary>
            [Name("Enable Rendering(属性)")]
            [EnumFieldName("属性/Enable Rendering")]
            Property_EnableRendering,

            /// <summary>
            /// Enable Input(属性):
            /// </summary>
            [Name("Enable Input(属性)")]
            [EnumFieldName("属性/Enable Input")]
            Property_EnableInput,

            /// <summary>
            /// Cookie Manager(属性):
            /// </summary>
            [Name("Cookie Manager(属性)")]
            [EnumFieldName("属性/Cookie Manager")]
            Property_CookieManager,

            /// <summary>
            /// Texture(属性):
            /// </summary>
            [Name("Texture(属性)")]
            [EnumFieldName("属性/Texture")]
            Property_Texture,

            /// <summary>
            /// Is Ready(属性):
            /// </summary>
            [Name("Is Ready(属性)")]
            [EnumFieldName("属性/Is Ready")]
            Property_IsReady,

            /// <summary>
            /// Url(属性):
            /// </summary>
            [Name("Url(属性)")]
            [EnumFieldName("属性/Url")]
            Property_Url,

            /// <summary>
            /// Can Go Back(属性):
            /// </summary>
            [Name("Can Go Back(属性)")]
            [EnumFieldName("属性/Can Go Back")]
            Property_CanGoBack,

            /// <summary>
            /// Can Go Forward(属性):
            /// </summary>
            [Name("Can Go Forward(属性)")]
            [EnumFieldName("属性/Can Go Forward")]
            Property_CanGoForward,

            /// <summary>
            /// Is Loading Raw(属性):
            /// </summary>
            [Name("Is Loading Raw(属性)")]
            [EnumFieldName("属性/Is Loading Raw")]
            Property_IsLoadingRaw,

            /// <summary>
            /// Is Loaded(属性):
            /// </summary>
            [Name("Is Loaded(属性)")]
            [EnumFieldName("属性/Is Loaded")]
            Property_IsLoaded,

            /// <summary>
            /// Size(属性):
            /// </summary>
            [Name("Size(属性)")]
            [EnumFieldName("属性/Size")]
            Property_Size,

            /// <summary>
            /// Zoom(属性):
            /// </summary>
            [Name("Zoom(属性)")]
            [EnumFieldName("属性/Zoom")]
            Property_Zoom,

            /// <summary>
            /// 使用GUI布局(属性):
            /// </summary>
            [Name("使用GUI布局(属性)")]
            [EnumFieldName("属性/使用GUI布局")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Property_useGUILayout,

            /// <summary>
            /// 启用(属性):
            /// </summary>
            [Name("启用(属性)")]
            [EnumFieldName("属性/启用")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Property_enabled,

            /// <summary>
            /// 是激活并启用(属性):
            /// </summary>
            [Name("是激活并启用(属性)")]
            [EnumFieldName("属性/是激活并启用")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Property_isActiveAndEnabled,

            /// <summary>
            /// Transform(属性):
            /// </summary>
            [Name("Transform(属性)")]
            [EnumFieldName("属性/Transform")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Property_transform,

            /// <summary>
            /// Game Object(属性):
            /// </summary>
            [Name("Game Object(属性)")]
            [EnumFieldName("属性/Game Object")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Property_gameObject,

            /// <summary>
            /// 标签(属性):
            /// </summary>
            [Name("标签(属性)")]
            [EnumFieldName("属性/标签")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Property_tag,

            /// <summary>
            /// 名称(属性):
            /// </summary>
            [Name("名称(属性)")]
            [EnumFieldName("属性/名称")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Property_name,

            /// <summary>
            /// 隐藏标识(属性):
            /// </summary>
            [Name("隐藏标识(属性)")]
            [EnumFieldName("属性/隐藏标识")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Property_hideFlags,

#endregion

#region 方法

            /// <summary>
            /// 比较标签(标签)(方法):
            /// </summary>
            [Name("比较标签(标签)(方法)")]
            [EnumFieldName("方法/比较标签(标签)")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_CompareTag_String = 10000,

            /// <summary>
            /// Eval JS(方法):
            /// </summary>
            [Name("Eval JS(方法)")]
            [EnumFieldName("方法/Eval JS")]
            Method_EvalJS_String_String,

            /// <summary>
            /// Eval JSCSP(方法):
            /// </summary>
            [Name("Eval JSCSP(方法)")]
            [EnumFieldName("方法/Eval JSCSP")]
            Method_EvalJSCSP_String_String,

            /// <summary>
            /// 获取组件(类型)(方法):
            /// </summary>
            [Name("获取组件(类型)(方法)")]
            [EnumFieldName("方法/获取组件(类型)")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_GetComponent_String,

            /// <summary>
            /// 获取哈希码(方法):
            /// </summary>
            [Name("获取哈希码(方法)")]
            [EnumFieldName("方法/获取哈希码")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_GetHashCode,

            /// <summary>
            /// 获取实例ID(方法):
            /// </summary>
            [Name("获取实例ID(方法)")]
            [EnumFieldName("方法/获取实例ID")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_GetInstanceID,

            /// <summary>
            /// 获取类型(方法):
            /// </summary>
            [Name("获取类型(方法)")]
            [EnumFieldName("方法/获取类型")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_GetType,

            /// <summary>
            /// 是否调用(方法):
            /// </summary>
            [Name("是否调用(方法)")]
            [EnumFieldName("方法/是否调用")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_IsInvoking,

            /// <summary>
            /// 是否调用(方法名)(方法):
            /// </summary>
            [Name("是否调用(方法名)(方法)")]
            [EnumFieldName("方法/是否调用(方法名)")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_IsInvoking_String,

            /// <summary>
            /// 启动协程(方法):
            /// </summary>
            [Name("启动协程(方法)")]
            [EnumFieldName("方法/启动协程")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_StartCoroutine_String,

            /// <summary>
            /// 转字符串(方法):
            /// </summary>
            [Name("转字符串(方法)")]
            [EnumFieldName("方法/转字符串")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_ToString,

#endregion

        }

#region 方法

        /// <summary>
        /// 标签:
        /// </summary>
        [Name("标签")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_CompareTag_String)]
        public StringPropertyValue _Method_CompareTag_String__tag = new StringPropertyValue();

        /// <summary>
        /// script:
        /// </summary>
        [Name("script")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_EvalJS_String_String)]
        public StringPropertyValue _Method_EvalJS_String_String__script = new StringPropertyValue();

        /// <summary>
        /// scriptURL:
        /// </summary>
        [Name("scriptURL")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_EvalJS_String_String)]
        public StringPropertyValue _Method_EvalJS_String_String__scriptURL = new StringPropertyValue();

        /// <summary>
        /// script:
        /// </summary>
        [Name("script")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_EvalJSCSP_String_String)]
        public StringPropertyValue _Method_EvalJSCSP_String_String__script = new StringPropertyValue();

        /// <summary>
        /// scriptURL:
        /// </summary>
        [Name("scriptURL")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_EvalJSCSP_String_String)]
        public StringPropertyValue _Method_EvalJSCSP_String_String__scriptURL = new StringPropertyValue();

        /// <summary>
        /// 类型:
        /// </summary>
        [Name("类型")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_GetComponent_String)]
        public StringPropertyValue _Method_GetComponent_String__type = new StringPropertyValue();

        /// <summary>
        /// 方法名:
        /// </summary>
        [Name("方法名")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_IsInvoking_String)]
        public StringPropertyValue _Method_IsInvoking_String__methodName = new StringPropertyValue();

        /// <summary>
        /// 方法名:
        /// </summary>
        [Name("方法名")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_StartCoroutine_String)]
        public StringPropertyValue _Method_StartCoroutine_String__methodName = new StringPropertyValue();

#endregion

        /// <summary>
        /// 属性路径列表
        /// </summary>
        [Name("属性路径列表")]
        public PropertyPathList _propertyPathList = new PropertyPathList();

        /// <summary>
        /// 变量字符串:将获取到的属性值存储在变量字符串对应的变量中
        /// </summary>
        [Name("变量字符串")]
        [Tip("将获取到的属性值存储在变量字符串对应的变量中", "Store the obtained attribute value in the variable corresponding to the variable string")]
        [ValidityCheck(EValidityCheckType.NotNullOrEmpty)]
        [VarString(EVarStringHierarchyKeyMode.Set)]
        public string _varString;

        /// <summary>
        /// 变量字符串列表
        /// </summary>
        [Name("变量字符串列表")]
        [Tip("将获取到的属性值存储在变量字符串列表内每个变量字符串对应的变量中", "Store the obtained attribute value in the variable string list and the variable corresponding to each variable string")]
        [VarString(EVarStringHierarchyKeyMode.Set)]
        public List<string> _varStrings = new List<string>();

        /// <summary>
        /// 将值设置到变量
        /// </summary>
        /// <param name="value">值</param>
        protected void SetToVariable(object value)
        {
            var scriptManager = ScriptManager.instance;
            if (scriptManager && Converter.instance.TryConvertTo<string>(value, out string output))
            {
                scriptManager.TrySetOrAddSetHierarchyVarValue(_varString, output);
                foreach (var vn in _varStrings)
                {
                    scriptManager.TrySetOrAddSetHierarchyVarValue(vn, output);
                }
            }
        }

        /// <summary>
        /// 获取属性路径值
        /// </summary>
        /// <param name="value" ></param>
        /// <returns></returns>
        public bool TryGetPropertyPathValue(out object value)
        {
            if (TryGetPropertyValue(out var instance) && _propertyPathList.TryGetPropertyPathValue(instance, out value))
            {
                return true;
            }
            value = default;
            return false;
        }

        /// <summary>
        /// 尝试获取属性路径值类型
        /// </summary>
        /// <param name="type" ></param>
        /// <returns></returns>
        public bool TryGetPropertyPathValueType(out Type type)
        {
            if (TryGetPropertyValueType(out var instance) && _propertyPathList.TryGetTypeMemberCacheValue(instance, out var value))
            {
                type = value.memberValueType;
                return true;
            }
            type = default;
            return false;
        }

        /// <summary>
        /// 尝试获取属性路径值
        /// </summary>
        /// <param name="index" ></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetPropertyPathValue(int index, out object value)
        {
            if (TryGetPropertyValue(out var instance) && _propertyPathList.TryGetPropertyPathValue(instance, index, out value))
            {
                return true;
            }
            value = default;
            return false;
        }

        /// <summary>
        /// 尝试获取属性路径值类型
        /// </summary>
        /// <param name="index" ></param>
        /// <param name="type" ></param>
        /// <returns></returns>
        public bool TryGetPropertyPathValueType(int index, out Type type)
        {
            if (TryGetPropertyValueType(out var instance) && _propertyPathList.TryGetTypeMemberCacheValue(instance, index, out var value))
            {
                type = value.memberValueType;
                return true;
            }
            type = default;
            return false;
        }

        /// <summary>
        /// 尝试获取属性值
        /// </summary>
        /// <param name="value">值</param>
        public bool TryGetPropertyValue(out object value)
        {
#if XDREAMER_EMBEDDED_BROWSER
            switch (_propertyName)
            {
                case EPropertyName.Field_allowContextMenuOn:
                    {
                        value = _Browser.allowContextMenuOn;
                        return true;
                    }
                case EPropertyName.Field_baseColor:
                    {
                        value = _Browser.baseColor;
                        return true;
                    }
                case EPropertyName.Field_focusState:
                    {
                        value = _Browser.focusState;
                        return true;
                    }
                case EPropertyName.Field_generateMipmap:
                    {
                        value = _Browser.generateMipmap;
                        return true;
                    }
                case EPropertyName.Field_onDownloadStarted:
                    {
                        value = _Browser.onDownloadStarted;
                        return true;
                    }
                case EPropertyName.Property_LocalUrlPrefix:
                    {
                        value = Browser.LocalUrlPrefix;
                        return true;
                    }
                case EPropertyName.Property_UIHandler:
                    {
                        value = _Browser.UIHandler;
                        return true;
                    }
                case EPropertyName.Property_EnableRendering:
                    {
                        value = _Browser.EnableRendering;
                        return true;
                    }
                case EPropertyName.Property_EnableInput:
                    {
                        value = _Browser.EnableInput;
                        return true;
                    }
                case EPropertyName.Property_CookieManager:
                    {
                        value = _Browser.CookieManager;
                        return true;
                    }
                case EPropertyName.Property_Texture:
                    {
                        value = _Browser.Texture;
                        return true;
                    }
                case EPropertyName.Property_IsReady:
                    {
                        value = _Browser.IsReady;
                        return true;
                    }
                case EPropertyName.Property_Url:
                    {
                        value = _Browser.Url;
                        return true;
                    }
                case EPropertyName.Property_CanGoBack:
                    {
                        value = _Browser.CanGoBack;
                        return true;
                    }
                case EPropertyName.Property_CanGoForward:
                    {
                        value = _Browser.CanGoForward;
                        return true;
                    }
                case EPropertyName.Property_IsLoadingRaw:
                    {
                        value = _Browser.IsLoadingRaw;
                        return true;
                    }
                case EPropertyName.Property_IsLoaded:
                    {
                        value = _Browser.IsLoaded;
                        return true;
                    }
                case EPropertyName.Property_Size:
                    {
                        value = _Browser.Size;
                        return true;
                    }
                case EPropertyName.Property_Zoom:
                    {
                        value = _Browser.Zoom;
                        return true;
                    }
                case EPropertyName.Property_useGUILayout:
                    {
                        value = _Browser.useGUILayout;
                        return true;
                    }
                case EPropertyName.Property_enabled:
                    {
                        value = _Browser.enabled;
                        return true;
                    }
                case EPropertyName.Property_isActiveAndEnabled:
                    {
                        value = _Browser.isActiveAndEnabled;
                        return true;
                    }
                case EPropertyName.Property_transform:
                    {
                        value = _Browser.transform;
                        return true;
                    }
                case EPropertyName.Property_gameObject:
                    {
                        value = _Browser.gameObject;
                        return true;
                    }
                case EPropertyName.Property_tag:
                    {
                        value = _Browser.tag;
                        return true;
                    }
                case EPropertyName.Property_name:
                    {
                        value = _Browser.name;
                        return true;
                    }
                case EPropertyName.Property_hideFlags:
                    {
                        value = _Browser.hideFlags;
                        return true;
                    }
                case EPropertyName.Method_CompareTag_String:
                    {
                        value = _Browser.CompareTag(_Method_CompareTag_String__tag.GetValue());
                        return true;
                    }
                case EPropertyName.Method_EvalJS_String_String:
                    {
                        value = _Browser.EvalJS(_Method_EvalJS_String_String__script.GetValue(), _Method_EvalJS_String_String__scriptURL.GetValue());
                        return true;
                    }
                case EPropertyName.Method_EvalJSCSP_String_String:
                    {
                        value = _Browser.EvalJSCSP(_Method_EvalJSCSP_String_String__script.GetValue(), _Method_EvalJSCSP_String_String__scriptURL.GetValue());
                        return true;
                    }
                case EPropertyName.Method_GetComponent_String:
                    {
                        value = _Browser.GetComponent(_Method_GetComponent_String__type.GetValue());
                        return true;
                    }
                case EPropertyName.Method_GetHashCode:
                    {
                        value = _Browser.GetHashCode();
                        return true;
                    }
                case EPropertyName.Method_GetInstanceID:
                    {
                        value = _Browser.GetInstanceID();
                        return true;
                    }
                case EPropertyName.Method_GetType:
                    {
                        value = _Browser.GetType();
                        return true;
                    }
                case EPropertyName.Method_IsInvoking:
                    {
                        value = _Browser.IsInvoking();
                        return true;
                    }
                case EPropertyName.Method_IsInvoking_String:
                    {
                        value = _Browser.IsInvoking(_Method_IsInvoking_String__methodName.GetValue());
                        return true;
                    }
                case EPropertyName.Method_StartCoroutine_String:
                    {
                        value = _Browser.StartCoroutine(_Method_StartCoroutine_String__methodName.GetValue());
                        return true;
                    }
                case EPropertyName.Method_ToString:
                    {
                        value = _Browser.ToString();
                        return true;
                    }
            }
#endif
            value = default;
            return false;
        }

        /// <summary>
        /// 尝试获取属性值类型
        /// </summary>
        /// <param name="type">类型</param>
        public bool TryGetPropertyValueType(out Type type)
        {
#if XDREAMER_EMBEDDED_BROWSER
            switch (_propertyName)
            {
                case EPropertyName.Field_allowContextMenuOn:
                    {
                        type = TypeMemberCache.Get(typeof(Browser), nameof(Browser.allowContextMenuOn))?.memberValueType;
                        return type != null;
                    }
                case EPropertyName.Field_baseColor:
                    {
                        type = TypeMemberCache.Get(typeof(Browser), nameof(Browser.baseColor))?.memberValueType;
                        return type != null;
                    }
                case EPropertyName.Field_focusState:
                    {
                        type = TypeMemberCache.Get(typeof(Browser), nameof(Browser.focusState))?.memberValueType;
                        return type != null;
                    }
                case EPropertyName.Field_generateMipmap:
                    {
                        type = TypeMemberCache.Get(typeof(Browser), nameof(Browser.generateMipmap))?.memberValueType;
                        return type != null;
                    }
                case EPropertyName.Field_onDownloadStarted:
                    {
                        type = TypeMemberCache.Get(typeof(Browser), nameof(Browser.onDownloadStarted))?.memberValueType;
                        return type != null;
                    }
                case EPropertyName.Property_LocalUrlPrefix:
                    {
                        type = TypeMemberCache.Get(typeof(Browser), nameof(Browser.LocalUrlPrefix))?.memberValueType;
                        return type != null;
                    }
                case EPropertyName.Property_UIHandler:
                    {
                        type = TypeMemberCache.Get(typeof(Browser), nameof(Browser.UIHandler))?.memberValueType;
                        return type != null;
                    }
                case EPropertyName.Property_EnableRendering:
                    {
                        type = TypeMemberCache.Get(typeof(Browser), nameof(Browser.EnableRendering))?.memberValueType;
                        return type != null;
                    }
                case EPropertyName.Property_EnableInput:
                    {
                        type = TypeMemberCache.Get(typeof(Browser), nameof(Browser.EnableInput))?.memberValueType;
                        return type != null;
                    }
                case EPropertyName.Property_CookieManager:
                    {
                        type = TypeMemberCache.Get(typeof(Browser), nameof(Browser.CookieManager))?.memberValueType;
                        return type != null;
                    }
                case EPropertyName.Property_Texture:
                    {
                        type = TypeMemberCache.Get(typeof(Browser), nameof(Browser.Texture))?.memberValueType;
                        return type != null;
                    }
                case EPropertyName.Property_IsReady:
                    {
                        type = TypeMemberCache.Get(typeof(Browser), nameof(Browser.IsReady))?.memberValueType;
                        return type != null;
                    }
                case EPropertyName.Property_Url:
                    {
                        type = TypeMemberCache.Get(typeof(Browser), nameof(Browser.Url))?.memberValueType;
                        return type != null;
                    }
                case EPropertyName.Property_CanGoBack:
                    {
                        type = TypeMemberCache.Get(typeof(Browser), nameof(Browser.CanGoBack))?.memberValueType;
                        return type != null;
                    }
                case EPropertyName.Property_CanGoForward:
                    {
                        type = TypeMemberCache.Get(typeof(Browser), nameof(Browser.CanGoForward))?.memberValueType;
                        return type != null;
                    }
                case EPropertyName.Property_IsLoadingRaw:
                    {
                        type = TypeMemberCache.Get(typeof(Browser), nameof(Browser.IsLoadingRaw))?.memberValueType;
                        return type != null;
                    }
                case EPropertyName.Property_IsLoaded:
                    {
                        type = TypeMemberCache.Get(typeof(Browser), nameof(Browser.IsLoaded))?.memberValueType;
                        return type != null;
                    }
                case EPropertyName.Property_Size:
                    {
                        type = TypeMemberCache.Get(typeof(Browser), nameof(Browser.Size))?.memberValueType;
                        return type != null;
                    }
                case EPropertyName.Property_Zoom:
                    {
                        type = TypeMemberCache.Get(typeof(Browser), nameof(Browser.Zoom))?.memberValueType;
                        return type != null;
                    }
                case EPropertyName.Property_useGUILayout:
                    {
                        type = TypeMemberCache.Get(typeof(Browser), nameof(Browser.useGUILayout))?.memberValueType;
                        return type != null;
                    }
                case EPropertyName.Property_enabled:
                    {
                        type = TypeMemberCache.Get(typeof(Browser), nameof(Browser.enabled))?.memberValueType;
                        return type != null;
                    }
                case EPropertyName.Property_isActiveAndEnabled:
                    {
                        type = TypeMemberCache.Get(typeof(Browser), nameof(Browser.isActiveAndEnabled))?.memberValueType;
                        return type != null;
                    }
                case EPropertyName.Property_transform:
                    {
                        type = TypeMemberCache.Get(typeof(Browser), nameof(Browser.transform))?.memberValueType;
                        return type != null;
                    }
                case EPropertyName.Property_gameObject:
                    {
                        type = TypeMemberCache.Get(typeof(Browser), nameof(Browser.gameObject))?.memberValueType;
                        return type != null;
                    }
                case EPropertyName.Property_tag:
                    {
                        type = TypeMemberCache.Get(typeof(Browser), nameof(Browser.tag))?.memberValueType;
                        return type != null;
                    }
                case EPropertyName.Property_name:
                    {
                        type = TypeMemberCache.Get(typeof(Browser), nameof(Browser.name))?.memberValueType;
                        return type != null;
                    }
                case EPropertyName.Property_hideFlags:
                    {
                        type = TypeMemberCache.Get(typeof(Browser), nameof(Browser.hideFlags))?.memberValueType;
                        return type != null;
                    }
                case EPropertyName.Method_CompareTag_String:
                    {
                        var types = new Type[] { _Method_CompareTag_String__tag.valueType };
                        type = typeof(Browser).GetMethod(nameof(Browser.CompareTag), types)?.ReturnType;
                        return type != null;
                    }
                case EPropertyName.Method_EvalJS_String_String:
                    {
                        var types = new Type[] { _Method_EvalJS_String_String__script.valueType, _Method_EvalJS_String_String__scriptURL.valueType };
                        type = typeof(Browser).GetMethod(nameof(Browser.EvalJS), types)?.ReturnType;
                        return type != null;
                    }
                case EPropertyName.Method_EvalJSCSP_String_String:
                    {
                        var types = new Type[] { _Method_EvalJSCSP_String_String__script.valueType, _Method_EvalJSCSP_String_String__scriptURL.valueType };
                        type = typeof(Browser).GetMethod(nameof(Browser.EvalJSCSP), types)?.ReturnType;
                        return type != null;
                    }
                case EPropertyName.Method_GetComponent_String:
                    {
                        var types = new Type[] { _Method_GetComponent_String__type.valueType };
                        type = typeof(Browser).GetMethod(nameof(Browser.GetComponent), types)?.ReturnType;
                        return type != null;
                    }
                case EPropertyName.Method_GetHashCode:
                    {
                        var types = Empty<Type>.Array;
                        type = typeof(Browser).GetMethod(nameof(Browser.GetHashCode), types)?.ReturnType;
                        return type != null;
                    }
                case EPropertyName.Method_GetInstanceID:
                    {
                        var types = Empty<Type>.Array;
                        type = typeof(Browser).GetMethod(nameof(Browser.GetInstanceID), types)?.ReturnType;
                        return type != null;
                    }
                case EPropertyName.Method_GetType:
                    {
                        var types = Empty<Type>.Array;
                        type = typeof(Browser).GetMethod(nameof(Browser.GetType), types)?.ReturnType;
                        return type != null;
                    }
                case EPropertyName.Method_IsInvoking:
                    {
                        var types = Empty<Type>.Array;
                        type = typeof(Browser).GetMethod(nameof(Browser.IsInvoking), types)?.ReturnType;
                        return type != null;
                    }
                case EPropertyName.Method_IsInvoking_String:
                    {
                        var types = new Type[] { _Method_IsInvoking_String__methodName.valueType };
                        type = typeof(Browser).GetMethod(nameof(Browser.IsInvoking), types)?.ReturnType;
                        return type != null;
                    }
                case EPropertyName.Method_StartCoroutine_String:
                    {
                        var types = new Type[] { _Method_StartCoroutine_String__methodName.valueType };
                        type = typeof(Browser).GetMethod(nameof(Browser.StartCoroutine), types)?.ReturnType;
                        return type != null;
                    }
                case EPropertyName.Method_ToString:
                    {
                        var types = Empty<Type>.Array;
                        type = typeof(Browser).GetMethod(nameof(Browser.ToString), types)?.ReturnType;
                        return type != null;
                    }
            }
#endif
            type = default;
            return false;
        }

        bool IDropdownPopupAttribute.TryGetOptions(string purpose, string propertyPath, out string[] options)
        {
            if (TryGetPropertyValueType(out var type))
            {
                _propertyPathList.SetInstance(type, this);
                return _propertyPathList.TryGetOptions(purpose, propertyPath, out options);
            }
            options = default;
            return false;
        }

        bool IDropdownPopupAttribute.TryGetOption(string purpose, string propertyPath, string[] options, object propertyValue, out string option)
        {
            if (TryGetPropertyValueType(out var type))
            {
                _propertyPathList.SetInstance(type, this);
                return _propertyPathList.TryGetOption(purpose, propertyPath, options, propertyValue, out option);
            }
            option = default;
            return false;
        }

        bool IDropdownPopupAttribute.TryGetPropertyValue(string purpose, string propertyPath, string[] options, string option, out object propertyValue)
        {
            if (TryGetPropertyValueType(out var type))
            {
                _propertyPathList.SetInstance(type, this);
                return _propertyPathList.TryGetPropertyValue(purpose, propertyPath, options, option, out propertyValue);
            }
            propertyValue = default;
            return false;
        }

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="stateData">数据信息</param>
        /// <param name="executeMode">执行模式</param>
        public override void Execute(StateData stateData, EExecuteMode executeMode)
        {
            if (TryGetPropertyPathValue(out var value))
            {
                SetToVariable(value);
            }
        }

        /// <summary>
        /// 输出友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString()
        {
            return _varString + " = " + CommonFun.Name(_propertyName);
        }

        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity()
        {
            return base.DataValidity();
        }

        /// <summary>
        /// 重置
        /// </summary>
        /// <returns></returns>
        public override void Reset()
        {
            base.Reset();
#if XDREAMER_EMBEDDED_BROWSER
            if (Browser) { }
#endif
        }

    }
}
