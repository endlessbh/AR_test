
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Attributes;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginEmbeddedBrowser;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Base;

#if XDREAMER_EMBEDDED_BROWSER
using ZenFulcrum.EmbeddedBrowser;
using static ZenFulcrum.EmbeddedBrowser.Browser;
using static ZenFulcrum.EmbeddedBrowser.BrowserNative;
#endif

namespace XCSJ.PluginEmbeddedBrowser.States
{
    /// <summary>
    /// 浏览器属性设置: 浏览器属性设置
    /// </summary>
    [ComponentMenu(EmbeddedBrowserHelper.Title + "/" + Title, typeof(EmbeddedBrowserManager))]
    [Name(Title, nameof(BrowserPropertySet))]
    [Tip("浏览器属性设置")]
    [XCSJ.Attributes.Icon(EIcon.Property)]
    [Owner(typeof(EmbeddedBrowserManager))]
    public class BrowserPropertySet : BasePropertySet<BrowserPropertySet>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "浏览器属性设置";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(EmbeddedBrowserHelper.Title, typeof(EmbeddedBrowserManager))]
        [StateComponentMenu(EmbeddedBrowserHelper.Title + "/" + Title, typeof(EmbeddedBrowserManager))]
        [Name(Title, nameof(BrowserPropertySet))]
        [Tip("浏览器属性设置")]
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

        /// <summary>
        /// 浏览器列表:浏览器列表
        /// </summary>
        [Name("浏览器列表")]
        [Tip("浏览器列表", "Browser List")]
        public List<Browser> _Browsers = new List<Browser>();

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
            /// Generate Mipmap(字段):
            /// </summary>
            [Name("Generate Mipmap(字段)")]
            [EnumFieldName("字段/Generate Mipmap")]
            Field_generateMipmap,

#endregion

#region 属性

            /// <summary>
            /// 启用(属性):
            /// </summary>
            [Name("启用(属性)")]
            [EnumFieldName("属性/启用")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Property_enabled = 1000,

            /// <summary>
            /// Enable Input(属性):
            /// </summary>
            [Name("Enable Input(属性)")]
            [EnumFieldName("属性/Enable Input")]
            Property_EnableInput,

            /// <summary>
            /// Enable Rendering(属性):
            /// </summary>
            [Name("Enable Rendering(属性)")]
            [EnumFieldName("属性/Enable Rendering")]
            Property_EnableRendering,

            /// <summary>
            /// 隐藏标识(属性):
            /// </summary>
            [Name("隐藏标识(属性)")]
            [EnumFieldName("属性/隐藏标识")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Property_hideFlags,

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
            /// 标签(属性):
            /// </summary>
            [Name("标签(属性)")]
            [EnumFieldName("属性/标签")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Property_tag,

            /// <summary>
            /// Url(属性):
            /// </summary>
            [Name("Url(属性)")]
            [EnumFieldName("属性/Url")]
            Property_Url,

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
            /// Zoom(属性):
            /// </summary>
            [Name("Zoom(属性)")]
            [EnumFieldName("属性/Zoom")]
            Property_Zoom,

#endregion

#region 方法

            /// <summary>
            /// 广播消息(方法名)(方法):
            /// </summary>
            [Name("广播消息(方法名)(方法)")]
            [EnumFieldName("方法/广播消息(方法名)")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_BroadcastMessage_String = 10000,

            /// <summary>
            /// 广播消息(方法名+发送消息选项)(方法):
            /// </summary>
            [Name("广播消息(方法名+发送消息选项)(方法)")]
            [EnumFieldName("方法/广播消息(方法名+发送消息选项)")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_BroadcastMessage_String_SendMessageOptions,

            /// <summary>
            /// 取消调用(方法):
            /// </summary>
            [Name("取消调用(方法)")]
            [EnumFieldName("方法/取消调用")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_CancelInvoke,

            /// <summary>
            /// 取消调用(方法名)(方法):
            /// </summary>
            [Name("取消调用(方法名)(方法)")]
            [EnumFieldName("方法/取消调用(方法名)")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_CancelInvoke_String,

            /// <summary>
            /// 比较标签(标签)(方法):
            /// </summary>
            [Name("比较标签(标签)(方法)")]
            [EnumFieldName("方法/比较标签(标签)")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_CompareTag_String,

            /// <summary>
            /// Download Command(方法):
            /// </summary>
            [Name("Download Command(方法)")]
            [EnumFieldName("方法/Download Command")]
            Method_DownloadCommand_Int32_DownloadAction_String,

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
            /// Go Back(方法):
            /// </summary>
            [Name("Go Back(方法)")]
            [EnumFieldName("方法/Go Back")]
            Method_GoBack,

            /// <summary>
            /// Go Forward(方法):
            /// </summary>
            [Name("Go Forward(方法)")]
            [EnumFieldName("方法/Go Forward")]
            Method_GoForward,

            /// <summary>
            /// 是否调用(方法名+时间)(方法):
            /// </summary>
            [Name("是否调用(方法名+时间)(方法)")]
            [EnumFieldName("方法/是否调用(方法名+时间)")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_Invoke_String_Single,

            /// <summary>
            /// 重复调用(方法):
            /// </summary>
            [Name("重复调用(方法)")]
            [EnumFieldName("方法/重复调用")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_InvokeRepeating_String_Single_Single,

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
            /// LoadDataURI(text+mimeType)(方法):
            /// </summary>
            [Name("LoadDataURI(text+mimeType)(方法)")]
            [EnumFieldName("方法/LoadDataURI(text+mimeType)")]
            Method_LoadDataURI_String_String,

            /// <summary>
            /// Load HTML(方法):
            /// </summary>
            [Name("Load HTML(方法)")]
            [EnumFieldName("方法/Load HTML")]
            Method_LoadHTML_String_String,

            /// <summary>
            /// Load URL(方法):
            /// </summary>
            [Name("Load URL(方法)")]
            [EnumFieldName("方法/Load URL")]
            Method_LoadURL_String_Boolean,

            /// <summary>
            /// Press Key(方法):
            /// </summary>
            [Name("Press Key(方法)")]
            [EnumFieldName("方法/Press Key")]
            Method_PressKey_KeyCode_KeyAction,

            /// <summary>
            /// Reload(方法):
            /// </summary>
            [Name("Reload(方法)")]
            [EnumFieldName("方法/Reload")]
            Method_Reload_Boolean,

            /// <summary>
            /// Resize(width+height)(方法):
            /// </summary>
            [Name("Resize(width+height)(方法)")]
            [EnumFieldName("方法/Resize(width+height)")]
            Method_Resize_Int32_Int32,

            /// <summary>
            /// Resize(newTexture)(方法):
            /// </summary>
            [Name("Resize(newTexture)(方法)")]
            [EnumFieldName("方法/Resize(newTexture)")]
            Method_Resize_Texture2D,

            /// <summary>
            /// Send Frame Command(方法):
            /// </summary>
            [Name("Send Frame Command(方法)")]
            [EnumFieldName("方法/Send Frame Command")]
            Method_SendFrameCommand_FrameCommand,

            /// <summary>
            /// 发送消息(方法名)(方法):
            /// </summary>
            [Name("发送消息(方法名)(方法)")]
            [EnumFieldName("方法/发送消息(方法名)")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_SendMessage_String,

            /// <summary>
            /// 发送消息(方法名+发送消息选项)(方法):
            /// </summary>
            [Name("发送消息(方法名+发送消息选项)(方法)")]
            [EnumFieldName("方法/发送消息(方法名+发送消息选项)")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_SendMessage_String_SendMessageOptions,

            /// <summary>
            /// 向上发送消息(方法名)(方法):
            /// </summary>
            [Name("向上发送消息(方法名)(方法)")]
            [EnumFieldName("方法/向上发送消息(方法名)")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_SendMessageUpwards_String,

            /// <summary>
            /// 向上发送消息(方法名+发送消息选项)(方法):
            /// </summary>
            [Name("向上发送消息(方法名+发送消息选项)(方法)")]
            [EnumFieldName("方法/向上发送消息(方法名+发送消息选项)")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_SendMessageUpwards_String_SendMessageOptions,

            /// <summary>
            /// Set Overlay(方法):
            /// </summary>
            [Name("Set Overlay(方法)")]
            [EnumFieldName("方法/Set Overlay")]
            Method_SetOverlay_Browser,

            /// <summary>
            /// Show Dev Tools(方法):
            /// </summary>
            [Name("Show Dev Tools(方法)")]
            [EnumFieldName("方法/Show Dev Tools")]
            Method_ShowDevTools_Boolean,

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
            /// Stop(方法):
            /// </summary>
            [Name("Stop(方法)")]
            [EnumFieldName("方法/Stop")]
            Method_Stop,

            /// <summary>
            /// 停止全部协程(方法):
            /// </summary>
            [Name("停止全部协程(方法)")]
            [EnumFieldName("方法/停止全部协程")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_StopAllCoroutines,

            /// <summary>
            /// 停止协程(方法):
            /// </summary>
            [Name("停止协程(方法)")]
            [EnumFieldName("方法/停止协程")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_StopCoroutine_String,

            /// <summary>
            /// 转字符串(方法):
            /// </summary>
            [Name("转字符串(方法)")]
            [EnumFieldName("方法/转字符串")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_ToString,

            /// <summary>
            /// Type Text(方法):
            /// </summary>
            [Name("Type Text(方法)")]
            [EnumFieldName("方法/Type Text")]
            Method_TypeText_String,

            /// <summary>
            /// Update Cursor(方法):
            /// </summary>
            [Name("Update Cursor(方法)")]
            [EnumFieldName("方法/Update Cursor")]
            Method_UpdateCursor,

#endregion

        }

#region 字段

#if XDREAMER_EMBEDDED_BROWSER

        /// <summary>
        /// Context Menu Origin属性值
        /// </summary>
        [Serializable]
        public class ContextMenuOriginPropertyValue : EnumPropertyValue<ContextMenuOrigin>
        {
        }

        /// <summary>
        /// Allow Context Menu On:
        /// </summary>
        [Name("Allow Context Menu On")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Field_allowContextMenuOn)]
        public ContextMenuOriginPropertyValue _Field_allowContextMenuOn = new ContextMenuOriginPropertyValue();

#endif

        /// <summary>
        /// Base Color:
        /// </summary>
        [Name("Base Color")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Field_baseColor)]
        public Color32PropertyValue _Field_baseColor = new Color32PropertyValue();

        /// <summary>
        /// Generate Mipmap:
        /// </summary>
        [Name("Generate Mipmap")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Field_generateMipmap)]
        public BoolPropertyValue _Field_generateMipmap = new BoolPropertyValue();

#endregion

#region 属性

        /// <summary>
        /// 启用:
        /// </summary>
        [Name("启用")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Property_enabled)]
        public BoolPropertyValue _Property_enabled = new BoolPropertyValue();

        /// <summary>
        /// Enable Input:
        /// </summary>
        [Name("Enable Input")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Property_EnableInput)]
        public BoolPropertyValue _Property_EnableInput = new BoolPropertyValue();

        /// <summary>
        /// Enable Rendering:
        /// </summary>
        [Name("Enable Rendering")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Property_EnableRendering)]
        public BoolPropertyValue _Property_EnableRendering = new BoolPropertyValue();

        /// <summary>
        /// 隐藏标识:
        /// </summary>
        [Name("隐藏标识")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Property_hideFlags)]
        public HideFlagsPropertyValue _Property_hideFlags = new HideFlagsPropertyValue();

        /// <summary>
        /// 名称:
        /// </summary>
        [Name("名称")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Property_name)]
        public StringPropertyValue _Property_name = new StringPropertyValue();

        /// <summary>
        /// 标签:
        /// </summary>
        [Name("标签")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Property_tag)]
        public StringPropertyValue _Property_tag = new StringPropertyValue();

        /// <summary>
        /// Url:
        /// </summary>
        [Name("Url")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Property_Url)]
        public StringPropertyValue _Property_Url = new StringPropertyValue();

        /// <summary>
        /// 使用GUI布局:
        /// </summary>
        [Name("使用GUI布局")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Property_useGUILayout)]
        public BoolPropertyValue _Property_useGUILayout = new BoolPropertyValue();

        /// <summary>
        /// Zoom:
        /// </summary>
        [Name("Zoom")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Property_Zoom)]
        public FloatPropertyValue _Property_Zoom = new FloatPropertyValue();

#endregion

#region 方法

        /// <summary>
        /// 方法名:
        /// </summary>
        [Name("方法名")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_BroadcastMessage_String)]
        public StringPropertyValue _Method_BroadcastMessage_String__methodName = new StringPropertyValue();

        /// <summary>
        /// 方法名:
        /// </summary>
        [Name("方法名")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_BroadcastMessage_String_SendMessageOptions)]
        public StringPropertyValue _Method_BroadcastMessage_String_SendMessageOptions__methodName = new StringPropertyValue();

        /// <summary>
        /// 选项:
        /// </summary>
        [Name("选项")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_BroadcastMessage_String_SendMessageOptions)]
        public SendMessageOptionsPropertyValue _Method_BroadcastMessage_String_SendMessageOptions__options = new SendMessageOptionsPropertyValue();

        /// <summary>
        /// 方法名:
        /// </summary>
        [Name("方法名")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_CancelInvoke_String)]
        public StringPropertyValue _Method_CancelInvoke_String__methodName = new StringPropertyValue();

        /// <summary>
        /// 标签:
        /// </summary>
        [Name("标签")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_CompareTag_String)]
        public StringPropertyValue _Method_CompareTag_String__tag = new StringPropertyValue();

        /// <summary>
        /// downloadId:
        /// </summary>
        [Name("downloadId")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_DownloadCommand_Int32_DownloadAction_String)]
        public IntPropertyValue _Method_DownloadCommand_Int32_DownloadAction_String__downloadId = new IntPropertyValue();

#if XDREAMER_EMBEDDED_BROWSER

        /// <summary>
        /// Download Action属性值
        /// </summary>
        [Serializable]
        public class DownloadActionPropertyValue : EnumPropertyValue<DownloadAction>
        {
        }

        /// <summary>
        /// action:
        /// </summary>
        [Name("action")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_DownloadCommand_Int32_DownloadAction_String)]
        public DownloadActionPropertyValue _Method_DownloadCommand_Int32_DownloadAction_String__action = new DownloadActionPropertyValue();

#endif

        /// <summary>
        /// fileName:
        /// </summary>
        [Name("fileName")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_DownloadCommand_Int32_DownloadAction_String)]
        public StringPropertyValue _Method_DownloadCommand_Int32_DownloadAction_String__fileName = new StringPropertyValue();

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
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_Invoke_String_Single)]
        public StringPropertyValue _Method_Invoke_String_Single__methodName = new StringPropertyValue();

        /// <summary>
        /// 时间:
        /// </summary>
        [Name("时间")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_Invoke_String_Single)]
        public FloatPropertyValue _Method_Invoke_String_Single__time = new FloatPropertyValue();

        /// <summary>
        /// 方法名:
        /// </summary>
        [Name("方法名")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_InvokeRepeating_String_Single_Single)]
        public StringPropertyValue _Method_InvokeRepeating_String_Single_Single__methodName = new StringPropertyValue();

        /// <summary>
        /// 时间:
        /// </summary>
        [Name("时间")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_InvokeRepeating_String_Single_Single)]
        public FloatPropertyValue _Method_InvokeRepeating_String_Single_Single__time = new FloatPropertyValue();

        /// <summary>
        /// 重复率:
        /// </summary>
        [Name("重复率")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_InvokeRepeating_String_Single_Single)]
        public FloatPropertyValue _Method_InvokeRepeating_String_Single_Single__repeatRate = new FloatPropertyValue();

        /// <summary>
        /// 方法名:
        /// </summary>
        [Name("方法名")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_IsInvoking_String)]
        public StringPropertyValue _Method_IsInvoking_String__methodName = new StringPropertyValue();

        /// <summary>
        /// text:
        /// </summary>
        [Name("text")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_LoadDataURI_String_String)]
        public StringPropertyValue _Method_LoadDataURI_String_String__text = new StringPropertyValue();

        /// <summary>
        /// mimeType:
        /// </summary>
        [Name("mimeType")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_LoadDataURI_String_String)]
        public StringPropertyValue _Method_LoadDataURI_String_String__mimeType = new StringPropertyValue();

        /// <summary>
        /// html:
        /// </summary>
        [Name("html")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_LoadHTML_String_String)]
        public StringPropertyValue _Method_LoadHTML_String_String__html = new StringPropertyValue();

        /// <summary>
        /// url:
        /// </summary>
        [Name("url")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_LoadHTML_String_String)]
        public StringPropertyValue _Method_LoadHTML_String_String__url = new StringPropertyValue();

        /// <summary>
        /// url:
        /// </summary>
        [Name("url")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_LoadURL_String_Boolean)]
        public StringPropertyValue _Method_LoadURL_String_Boolean__url = new StringPropertyValue();

        /// <summary>
        /// force:
        /// </summary>
        [Name("force")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_LoadURL_String_Boolean)]
        public BoolPropertyValue _Method_LoadURL_String_Boolean__force = new BoolPropertyValue();

        /// <summary>
        /// key:
        /// </summary>
        [Name("key")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_PressKey_KeyCode_KeyAction)]
        public KeyCodePropertyValue _Method_PressKey_KeyCode_KeyAction__key = new KeyCodePropertyValue();

#if XDREAMER_EMBEDDED_BROWSER

        /// <summary>
        /// Key Action属性值
        /// </summary>
        [Serializable]
        public class KeyActionPropertyValue : EnumPropertyValue<KeyAction>
        {
        }

        /// <summary>
        /// action:
        /// </summary>
        [Name("action")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_PressKey_KeyCode_KeyAction)]
        public KeyActionPropertyValue _Method_PressKey_KeyCode_KeyAction__action = new KeyActionPropertyValue();

#endif

        /// <summary>
        /// force:
        /// </summary>
        [Name("force")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_Reload_Boolean)]
        public BoolPropertyValue _Method_Reload_Boolean__force = new BoolPropertyValue();

        /// <summary>
        /// width:
        /// </summary>
        [Name("width")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_Resize_Int32_Int32)]
        public IntPropertyValue _Method_Resize_Int32_Int32__width = new IntPropertyValue();

        /// <summary>
        /// height:
        /// </summary>
        [Name("height")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_Resize_Int32_Int32)]
        public IntPropertyValue _Method_Resize_Int32_Int32__height = new IntPropertyValue();

        /// <summary>
        /// newTexture:
        /// </summary>
        [Name("newTexture")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_Resize_Texture2D)]
        public TexturePropertyValue _Method_Resize_Texture2D__newTexture = new TexturePropertyValue();

#if XDREAMER_EMBEDDED_BROWSER

        /// <summary>
        /// Frame Command属性值
        /// </summary>
        [Serializable]
        public class FrameCommandPropertyValue : EnumPropertyValue<FrameCommand>
        {
        }

        /// <summary>
        /// command:
        /// </summary>
        [Name("command")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_SendFrameCommand_FrameCommand)]
        public FrameCommandPropertyValue _Method_SendFrameCommand_FrameCommand__command = new FrameCommandPropertyValue();

#endif

        /// <summary>
        /// 方法名:
        /// </summary>
        [Name("方法名")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_SendMessage_String)]
        public StringPropertyValue _Method_SendMessage_String__methodName = new StringPropertyValue();

        /// <summary>
        /// 方法名:
        /// </summary>
        [Name("方法名")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_SendMessage_String_SendMessageOptions)]
        public StringPropertyValue _Method_SendMessage_String_SendMessageOptions__methodName = new StringPropertyValue();

        /// <summary>
        /// 选项:
        /// </summary>
        [Name("选项")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_SendMessage_String_SendMessageOptions)]
        public SendMessageOptionsPropertyValue _Method_SendMessage_String_SendMessageOptions__options = new SendMessageOptionsPropertyValue();

        /// <summary>
        /// 方法名:
        /// </summary>
        [Name("方法名")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_SendMessageUpwards_String)]
        public StringPropertyValue _Method_SendMessageUpwards_String__methodName = new StringPropertyValue();

        /// <summary>
        /// 方法名:
        /// </summary>
        [Name("方法名")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_SendMessageUpwards_String_SendMessageOptions)]
        public StringPropertyValue _Method_SendMessageUpwards_String_SendMessageOptions__methodName = new StringPropertyValue();

        /// <summary>
        /// 选项:
        /// </summary>
        [Name("选项")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_SendMessageUpwards_String_SendMessageOptions)]
        public SendMessageOptionsPropertyValue _Method_SendMessageUpwards_String_SendMessageOptions__options = new SendMessageOptionsPropertyValue();

        /// <summary>
        /// overlayBrowser:
        /// </summary>
        [Name("overlayBrowser")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_SetOverlay_Browser)]
        public UnityObjectPropertyValue _Method_SetOverlay_Browser__overlayBrowser = new UnityObjectPropertyValue();

        /// <summary>
        /// show:
        /// </summary>
        [Name("show")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_ShowDevTools_Boolean)]
        public BoolPropertyValue _Method_ShowDevTools_Boolean__show = new BoolPropertyValue();

        /// <summary>
        /// 方法名:
        /// </summary>
        [Name("方法名")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_StartCoroutine_String)]
        public StringPropertyValue _Method_StartCoroutine_String__methodName = new StringPropertyValue();

        /// <summary>
        /// 方法名:
        /// </summary>
        [Name("方法名")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_StopCoroutine_String)]
        public StringPropertyValue _Method_StopCoroutine_String__methodName = new StringPropertyValue();

        /// <summary>
        /// text:
        /// </summary>
        [Name("text")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_TypeText_String)]
        public StringPropertyValue _Method_TypeText_String__text = new StringPropertyValue();

#endregion

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="stateData">数据信息</param>
        /// <param name="executeMode">执行模式</param>
        public override void Execute(StateData stateData, EExecuteMode executeMode)
        {
#if XDREAMER_EMBEDDED_BROWSER
            switch (_propertyName)
            {
                case EPropertyName.Field_allowContextMenuOn:
                    {
                        var value = _Field_allowContextMenuOn.GetValue();
                        if (_Browser != null) _Browser.allowContextMenuOn = value;
                        foreach (var obj in _Browsers)
                        {
                            if (obj != null) obj.allowContextMenuOn = value;
                        }
                        break;
                    }
                case EPropertyName.Field_baseColor:
                    {
                        var value = _Field_baseColor.GetValue();
                        if (_Browser != null) _Browser.baseColor = value;
                        foreach (var obj in _Browsers)
                        {
                            if (obj != null) obj.baseColor = value;
                        }
                        break;
                    }
                case EPropertyName.Field_generateMipmap:
                    {
                        var value = _Field_generateMipmap.GetValue();
                        if (_Browser != null) _Browser.generateMipmap = value;
                        foreach (var obj in _Browsers)
                        {
                            if (obj != null) obj.generateMipmap = value;
                        }
                        break;
                    }
                case EPropertyName.Property_enabled:
                    {
                        var value = _Property_enabled.GetValue();
                        if (_Browser != null) _Browser.enabled = value;
                        foreach (var obj in _Browsers)
                        {
                            if (obj != null) obj.enabled = value;
                        }
                        break;
                    }
                case EPropertyName.Property_EnableInput:
                    {
                        var value = _Property_EnableInput.GetValue();
                        if (_Browser != null) _Browser.EnableInput = value;
                        foreach (var obj in _Browsers)
                        {
                            if (obj != null) obj.EnableInput = value;
                        }
                        break;
                    }
                case EPropertyName.Property_EnableRendering:
                    {
                        var value = _Property_EnableRendering.GetValue();
                        if (_Browser != null) _Browser.EnableRendering = value;
                        foreach (var obj in _Browsers)
                        {
                            if (obj != null) obj.EnableRendering = value;
                        }
                        break;
                    }
                case EPropertyName.Property_hideFlags:
                    {
                        var value = _Property_hideFlags.GetValue();
                        if (_Browser != null) _Browser.hideFlags = value;
                        foreach (var obj in _Browsers)
                        {
                            if (obj != null) obj.hideFlags = value;
                        }
                        break;
                    }
                case EPropertyName.Property_name:
                    {
                        var value = _Property_name.GetValue();
                        if (_Browser != null) _Browser.name = value;
                        foreach (var obj in _Browsers)
                        {
                            if (obj != null) obj.name = value;
                        }
                        break;
                    }
                case EPropertyName.Property_tag:
                    {
                        var value = _Property_tag.GetValue();
                        if (_Browser != null) _Browser.tag = value;
                        foreach (var obj in _Browsers)
                        {
                            if (obj != null) obj.tag = value;
                        }
                        break;
                    }
                case EPropertyName.Property_Url:
                    {
                        var value = _Property_Url.GetValue();
                        if (_Browser != null) _Browser.Url = value;
                        foreach (var obj in _Browsers)
                        {
                            if (obj != null) obj.Url = value;
                        }
                        break;
                    }
                case EPropertyName.Property_useGUILayout:
                    {
                        var value = _Property_useGUILayout.GetValue();
                        if (_Browser != null) _Browser.useGUILayout = value;
                        foreach (var obj in _Browsers)
                        {
                            if (obj != null) obj.useGUILayout = value;
                        }
                        break;
                    }
                case EPropertyName.Property_Zoom:
                    {
                        var value = _Property_Zoom.GetValue();
                        if (_Browser != null) _Browser.Zoom = value;
                        foreach (var obj in _Browsers)
                        {
                            if (obj != null) obj.Zoom = value;
                        }
                        break;
                    }
                case EPropertyName.Method_BroadcastMessage_String:
                    {
                        var methodName = _Method_BroadcastMessage_String__methodName.GetValue();
                        if (_Browser != null) _Browser.BroadcastMessage(methodName);
                        foreach (var obj in _Browsers)
                        {
                            if (obj != null) obj.BroadcastMessage(methodName);
                        }
                        break;
                    }
                case EPropertyName.Method_BroadcastMessage_String_SendMessageOptions:
                    {
                        var methodName = _Method_BroadcastMessage_String_SendMessageOptions__methodName.GetValue();
                        var options = _Method_BroadcastMessage_String_SendMessageOptions__options.GetValue();
                        if (_Browser != null) _Browser.BroadcastMessage(methodName, options);
                        foreach (var obj in _Browsers)
                        {
                            if (obj != null) obj.BroadcastMessage(methodName, options);
                        }
                        break;
                    }
                case EPropertyName.Method_CancelInvoke:
                    {
                        if (_Browser != null) _Browser.CancelInvoke();
                        foreach (var obj in _Browsers)
                        {
                            if (obj != null) obj.CancelInvoke();
                        }
                        break;
                    }
                case EPropertyName.Method_CancelInvoke_String:
                    {
                        var methodName = _Method_CancelInvoke_String__methodName.GetValue();
                        if (_Browser != null) _Browser.CancelInvoke(methodName);
                        foreach (var obj in _Browsers)
                        {
                            if (obj != null) obj.CancelInvoke(methodName);
                        }
                        break;
                    }
                case EPropertyName.Method_CompareTag_String:
                    {
                        var tag = _Method_CompareTag_String__tag.GetValue();
                        if (_Browser != null) _Browser.CompareTag(tag);
                        foreach (var obj in _Browsers)
                        {
                            if (obj != null) obj.CompareTag(tag);
                        }
                        break;
                    }
                case EPropertyName.Method_DownloadCommand_Int32_DownloadAction_String:
                    {
                        var downloadId = _Method_DownloadCommand_Int32_DownloadAction_String__downloadId.GetValue();
                        var action = _Method_DownloadCommand_Int32_DownloadAction_String__action.GetValue();
                        var fileName = _Method_DownloadCommand_Int32_DownloadAction_String__fileName.GetValue();
                        if (_Browser != null) _Browser.DownloadCommand(downloadId, action, fileName);
                        foreach (var obj in _Browsers)
                        {
                            if (obj != null) obj.DownloadCommand(downloadId, action, fileName);
                        }
                        break;
                    }
                case EPropertyName.Method_EvalJS_String_String:
                    {
                        var script = _Method_EvalJS_String_String__script.GetValue();
                        var scriptURL = _Method_EvalJS_String_String__scriptURL.GetValue();
                        if (_Browser != null) _Browser.EvalJS(script, scriptURL);
                        foreach (var obj in _Browsers)
                        {
                            if (obj != null) obj.EvalJS(script, scriptURL);
                        }
                        break;
                    }
                case EPropertyName.Method_EvalJSCSP_String_String:
                    {
                        var script = _Method_EvalJSCSP_String_String__script.GetValue();
                        var scriptURL = _Method_EvalJSCSP_String_String__scriptURL.GetValue();
                        if (_Browser != null) _Browser.EvalJSCSP(script, scriptURL);
                        foreach (var obj in _Browsers)
                        {
                            if (obj != null) obj.EvalJSCSP(script, scriptURL);
                        }
                        break;
                    }
                case EPropertyName.Method_GetComponent_String:
                    {
                        var type = _Method_GetComponent_String__type.GetValue();
                        if (_Browser != null) _Browser.GetComponent(type);
                        foreach (var obj in _Browsers)
                        {
                            if (obj != null) obj.GetComponent(type);
                        }
                        break;
                    }
                case EPropertyName.Method_GetHashCode:
                    {
                        if (_Browser != null) _Browser.GetHashCode();
                        foreach (var obj in _Browsers)
                        {
                            if (obj != null) obj.GetHashCode();
                        }
                        break;
                    }
                case EPropertyName.Method_GetInstanceID:
                    {
                        if (_Browser != null) _Browser.GetInstanceID();
                        foreach (var obj in _Browsers)
                        {
                            if (obj != null) obj.GetInstanceID();
                        }
                        break;
                    }
                case EPropertyName.Method_GetType:
                    {
                        if (_Browser != null) _Browser.GetType();
                        foreach (var obj in _Browsers)
                        {
                            if (obj != null) obj.GetType();
                        }
                        break;
                    }
                case EPropertyName.Method_GoBack:
                    {
                        if (_Browser != null) _Browser.GoBack();
                        foreach (var obj in _Browsers)
                        {
                            if (obj != null) obj.GoBack();
                        }
                        break;
                    }
                case EPropertyName.Method_GoForward:
                    {
                        if (_Browser != null) _Browser.GoForward();
                        foreach (var obj in _Browsers)
                        {
                            if (obj != null) obj.GoForward();
                        }
                        break;
                    }
                case EPropertyName.Method_Invoke_String_Single:
                    {
                        var methodName = _Method_Invoke_String_Single__methodName.GetValue();
                        var time = _Method_Invoke_String_Single__time.GetValue();
                        if (_Browser != null) _Browser.Invoke(methodName, time);
                        foreach (var obj in _Browsers)
                        {
                            if (obj != null) obj.Invoke(methodName, time);
                        }
                        break;
                    }
                case EPropertyName.Method_InvokeRepeating_String_Single_Single:
                    {
                        var methodName = _Method_InvokeRepeating_String_Single_Single__methodName.GetValue();
                        var time = _Method_InvokeRepeating_String_Single_Single__time.GetValue();
                        var repeatRate = _Method_InvokeRepeating_String_Single_Single__repeatRate.GetValue();
                        if (_Browser != null) _Browser.InvokeRepeating(methodName, time, repeatRate);
                        foreach (var obj in _Browsers)
                        {
                            if (obj != null) obj.InvokeRepeating(methodName, time, repeatRate);
                        }
                        break;
                    }
                case EPropertyName.Method_IsInvoking:
                    {
                        if (_Browser != null) _Browser.IsInvoking();
                        foreach (var obj in _Browsers)
                        {
                            if (obj != null) obj.IsInvoking();
                        }
                        break;
                    }
                case EPropertyName.Method_IsInvoking_String:
                    {
                        var methodName = _Method_IsInvoking_String__methodName.GetValue();
                        if (_Browser != null) _Browser.IsInvoking(methodName);
                        foreach (var obj in _Browsers)
                        {
                            if (obj != null) obj.IsInvoking(methodName);
                        }
                        break;
                    }
                case EPropertyName.Method_LoadDataURI_String_String:
                    {
                        var text = _Method_LoadDataURI_String_String__text.GetValue();
                        var mimeType = _Method_LoadDataURI_String_String__mimeType.GetValue();
                        if (_Browser != null) _Browser.LoadDataURI(text, mimeType);
                        foreach (var obj in _Browsers)
                        {
                            if (obj != null) obj.LoadDataURI(text, mimeType);
                        }
                        break;
                    }
                case EPropertyName.Method_LoadHTML_String_String:
                    {
                        var html = _Method_LoadHTML_String_String__html.GetValue();
                        var url = _Method_LoadHTML_String_String__url.GetValue();
                        if (_Browser != null) _Browser.LoadHTML(html, url);
                        foreach (var obj in _Browsers)
                        {
                            if (obj != null) obj.LoadHTML(html, url);
                        }
                        break;
                    }
                case EPropertyName.Method_LoadURL_String_Boolean:
                    {
                        var url = _Method_LoadURL_String_Boolean__url.GetValue();
                        var force = _Method_LoadURL_String_Boolean__force.GetValue();
                        if (_Browser != null) _Browser.LoadURL(url, force);
                        foreach (var obj in _Browsers)
                        {
                            if (obj != null) obj.LoadURL(url, force);
                        }
                        break;
                    }
                case EPropertyName.Method_PressKey_KeyCode_KeyAction:
                    {
                        var key = _Method_PressKey_KeyCode_KeyAction__key.GetValue();
                        var action = _Method_PressKey_KeyCode_KeyAction__action.GetValue();
                        if (_Browser != null) _Browser.PressKey(key, action);
                        foreach (var obj in _Browsers)
                        {
                            if (obj != null) obj.PressKey(key, action);
                        }
                        break;
                    }
                case EPropertyName.Method_Reload_Boolean:
                    {
                        var force = _Method_Reload_Boolean__force.GetValue();
                        if (_Browser != null) _Browser.Reload(force);
                        foreach (var obj in _Browsers)
                        {
                            if (obj != null) obj.Reload(force);
                        }
                        break;
                    }
                case EPropertyName.Method_Resize_Int32_Int32:
                    {
                        var width = _Method_Resize_Int32_Int32__width.GetValue();
                        var height = _Method_Resize_Int32_Int32__height.GetValue();
                        if (_Browser != null) _Browser.Resize(width, height);
                        foreach (var obj in _Browsers)
                        {
                            if (obj != null) obj.Resize(width, height);
                        }
                        break;
                    }
                case EPropertyName.Method_Resize_Texture2D:
                    {
                        var newTexture = _Method_Resize_Texture2D__newTexture.GetValue() as Texture2D;
                        if (_Browser != null) _Browser.Resize(newTexture);
                        foreach (var obj in _Browsers)
                        {
                            if (obj != null) obj.Resize(newTexture);
                        }
                        break;
                    }
                case EPropertyName.Method_SendFrameCommand_FrameCommand:
                    {
                        var command = _Method_SendFrameCommand_FrameCommand__command.GetValue();
                        if (_Browser != null) _Browser.SendFrameCommand(command);
                        foreach (var obj in _Browsers)
                        {
                            if (obj != null) obj.SendFrameCommand(command);
                        }
                        break;
                    }
                case EPropertyName.Method_SendMessage_String:
                    {
                        var methodName = _Method_SendMessage_String__methodName.GetValue();
                        if (_Browser != null) _Browser.SendMessage(methodName);
                        foreach (var obj in _Browsers)
                        {
                            if (obj != null) obj.SendMessage(methodName);
                        }
                        break;
                    }
                case EPropertyName.Method_SendMessage_String_SendMessageOptions:
                    {
                        var methodName = _Method_SendMessage_String_SendMessageOptions__methodName.GetValue();
                        var options = _Method_SendMessage_String_SendMessageOptions__options.GetValue();
                        if (_Browser != null) _Browser.SendMessage(methodName, options);
                        foreach (var obj in _Browsers)
                        {
                            if (obj != null) obj.SendMessage(methodName, options);
                        }
                        break;
                    }
                case EPropertyName.Method_SendMessageUpwards_String:
                    {
                        var methodName = _Method_SendMessageUpwards_String__methodName.GetValue();
                        if (_Browser != null) _Browser.SendMessageUpwards(methodName);
                        foreach (var obj in _Browsers)
                        {
                            if (obj != null) obj.SendMessageUpwards(methodName);
                        }
                        break;
                    }
                case EPropertyName.Method_SendMessageUpwards_String_SendMessageOptions:
                    {
                        var methodName = _Method_SendMessageUpwards_String_SendMessageOptions__methodName.GetValue();
                        var options = _Method_SendMessageUpwards_String_SendMessageOptions__options.GetValue();
                        if (_Browser != null) _Browser.SendMessageUpwards(methodName, options);
                        foreach (var obj in _Browsers)
                        {
                            if (obj != null) obj.SendMessageUpwards(methodName, options);
                        }
                        break;
                    }
                case EPropertyName.Method_SetOverlay_Browser:
                    {
                        var overlayBrowser = _Method_SetOverlay_Browser__overlayBrowser.GetValue() as Browser;
                        if (_Browser != null) _Browser.SetOverlay(overlayBrowser);
                        foreach (var obj in _Browsers)
                        {
                            if (obj != null) obj.SetOverlay(overlayBrowser);
                        }
                        break;
                    }
                case EPropertyName.Method_ShowDevTools_Boolean:
                    {
                        var show = _Method_ShowDevTools_Boolean__show.GetValue();
                        if (_Browser != null) _Browser.ShowDevTools(show);
                        foreach (var obj in _Browsers)
                        {
                            if (obj != null) obj.ShowDevTools(show);
                        }
                        break;
                    }
                case EPropertyName.Method_StartCoroutine_String:
                    {
                        var methodName = _Method_StartCoroutine_String__methodName.GetValue();
                        if (_Browser != null) _Browser.StartCoroutine(methodName);
                        foreach (var obj in _Browsers)
                        {
                            if (obj != null) obj.StartCoroutine(methodName);
                        }
                        break;
                    }
                case EPropertyName.Method_Stop:
                    {
                        if (_Browser != null) _Browser.Stop();
                        foreach (var obj in _Browsers)
                        {
                            if (obj != null) obj.Stop();
                        }
                        break;
                    }
                case EPropertyName.Method_StopAllCoroutines:
                    {
                        if (_Browser != null) _Browser.StopAllCoroutines();
                        foreach (var obj in _Browsers)
                        {
                            if (obj != null) obj.StopAllCoroutines();
                        }
                        break;
                    }
                case EPropertyName.Method_StopCoroutine_String:
                    {
                        var methodName = _Method_StopCoroutine_String__methodName.GetValue();
                        if (_Browser != null) _Browser.StopCoroutine(methodName);
                        foreach (var obj in _Browsers)
                        {
                            if (obj != null) obj.StopCoroutine(methodName);
                        }
                        break;
                    }
                case EPropertyName.Method_ToString:
                    {
                        if (_Browser != null) _Browser.ToString();
                        foreach (var obj in _Browsers)
                        {
                            if (obj != null) obj.ToString();
                        }
                        break;
                    }
                case EPropertyName.Method_TypeText_String:
                    {
                        var text = _Method_TypeText_String__text.GetValue();
                        if (_Browser != null) _Browser.TypeText(text);
                        foreach (var obj in _Browsers)
                        {
                            if (obj != null) obj.TypeText(text);
                        }
                        break;
                    }
                case EPropertyName.Method_UpdateCursor:
                    {
                        if (_Browser != null) _Browser.UpdateCursor();
                        foreach (var obj in _Browsers)
                        {
                            if (obj != null) obj.UpdateCursor();
                        }
                        break;
                    }
            }
#endif
        }

        /// <summary>
        /// 输出友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString()
        {
#if XDREAMER_EMBEDDED_BROWSER
            switch (_propertyName)
            {
                case EPropertyName.Field_allowContextMenuOn:
                    {
                        return CommonFun.Name(_propertyName) + " = " + _Field_allowContextMenuOn.ToFriendlyString();
                    }
                case EPropertyName.Field_baseColor:
                    {
                        return CommonFun.Name(_propertyName) + " = " + _Field_baseColor.ToFriendlyString();
                    }
                case EPropertyName.Field_generateMipmap:
                    {
                        return CommonFun.Name(_propertyName) + " = " + _Field_generateMipmap.ToFriendlyString();
                    }
                case EPropertyName.Property_enabled:
                    {
                        return CommonFun.Name(_propertyName) + " = " + _Property_enabled.ToFriendlyString();
                    }
                case EPropertyName.Property_EnableInput:
                    {
                        return CommonFun.Name(_propertyName) + " = " + _Property_EnableInput.ToFriendlyString();
                    }
                case EPropertyName.Property_EnableRendering:
                    {
                        return CommonFun.Name(_propertyName) + " = " + _Property_EnableRendering.ToFriendlyString();
                    }
                case EPropertyName.Property_hideFlags:
                    {
                        return CommonFun.Name(_propertyName) + " = " + _Property_hideFlags.ToFriendlyString();
                    }
                case EPropertyName.Property_name:
                    {
                        return CommonFun.Name(_propertyName) + " = " + _Property_name.ToFriendlyString();
                    }
                case EPropertyName.Property_tag:
                    {
                        return CommonFun.Name(_propertyName) + " = " + _Property_tag.ToFriendlyString();
                    }
                case EPropertyName.Property_Url:
                    {
                        return CommonFun.Name(_propertyName) + " = " + _Property_Url.ToFriendlyString();
                    }
                case EPropertyName.Property_useGUILayout:
                    {
                        return CommonFun.Name(_propertyName) + " = " + _Property_useGUILayout.ToFriendlyString();
                    }
                case EPropertyName.Property_Zoom:
                    {
                        return CommonFun.Name(_propertyName) + " = " + _Property_Zoom.ToFriendlyString();
                    }
                case EPropertyName.Method_BroadcastMessage_String:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_BroadcastMessage_String_SendMessageOptions:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_CancelInvoke:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_CancelInvoke_String:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_CompareTag_String:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_DownloadCommand_Int32_DownloadAction_String:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_EvalJS_String_String:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_EvalJSCSP_String_String:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_GetComponent_String:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_GetHashCode:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_GetInstanceID:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_GetType:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_GoBack:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_GoForward:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_Invoke_String_Single:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_InvokeRepeating_String_Single_Single:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_IsInvoking:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_IsInvoking_String:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_LoadDataURI_String_String:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_LoadHTML_String_String:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_LoadURL_String_Boolean:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_PressKey_KeyCode_KeyAction:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_Reload_Boolean:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_Resize_Int32_Int32:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_Resize_Texture2D:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_SendFrameCommand_FrameCommand:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_SendMessage_String:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_SendMessage_String_SendMessageOptions:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_SendMessageUpwards_String:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_SendMessageUpwards_String_SendMessageOptions:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_SetOverlay_Browser:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_ShowDevTools_Boolean:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_StartCoroutine_String:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_Stop:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_StopAllCoroutines:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_StopCoroutine_String:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_ToString:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_TypeText_String:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_UpdateCursor:
                    {
                        return CommonFun.Name(_propertyName);
                    }
            }
#endif
            return base.ToFriendlyString();
        }

        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity()
        {
#if XDREAMER_EMBEDDED_BROWSER
            switch (_propertyName)
            {
                case EPropertyName.Field_allowContextMenuOn:
                    {
                        return _Browser && _Field_allowContextMenuOn.DataValidity();
                    }
                case EPropertyName.Field_baseColor:
                    {
                        return _Browser && _Field_baseColor.DataValidity();
                    }
                case EPropertyName.Field_generateMipmap:
                    {
                        return _Browser && _Field_generateMipmap.DataValidity();
                    }
                case EPropertyName.Property_enabled:
                    {
                        return _Browser && _Property_enabled.DataValidity();
                    }
                case EPropertyName.Property_EnableInput:
                    {
                        return _Browser && _Property_EnableInput.DataValidity();
                    }
                case EPropertyName.Property_EnableRendering:
                    {
                        return _Browser && _Property_EnableRendering.DataValidity();
                    }
                case EPropertyName.Property_hideFlags:
                    {
                        return _Browser && _Property_hideFlags.DataValidity();
                    }
                case EPropertyName.Property_name:
                    {
                        return _Browser && _Property_name.DataValidity();
                    }
                case EPropertyName.Property_tag:
                    {
                        return _Browser && _Property_tag.DataValidity();
                    }
                case EPropertyName.Property_Url:
                    {
                        return _Browser && _Property_Url.DataValidity();
                    }
                case EPropertyName.Property_useGUILayout:
                    {
                        return _Browser && _Property_useGUILayout.DataValidity();
                    }
                case EPropertyName.Property_Zoom:
                    {
                        return _Browser && _Property_Zoom.DataValidity();
                    }
                case EPropertyName.Method_BroadcastMessage_String:
                    {
                        return _Browser && _Method_BroadcastMessage_String__methodName.DataValidity();
                    }
                case EPropertyName.Method_BroadcastMessage_String_SendMessageOptions:
                    {
                        if (!_Browser) return false;
                        if (!_Method_BroadcastMessage_String_SendMessageOptions__methodName.DataValidity()) return false;
                        if (!_Method_BroadcastMessage_String_SendMessageOptions__options.DataValidity()) return false;
                        break;
                    }
                case EPropertyName.Method_CancelInvoke:
                    {
                        return _Browser;
                    }
                case EPropertyName.Method_CancelInvoke_String:
                    {
                        return _Browser && _Method_CancelInvoke_String__methodName.DataValidity();
                    }
                case EPropertyName.Method_CompareTag_String:
                    {
                        return _Browser && _Method_CompareTag_String__tag.DataValidity();
                    }
                case EPropertyName.Method_DownloadCommand_Int32_DownloadAction_String:
                    {
                        if (!_Browser) return false;
                        if (!_Method_DownloadCommand_Int32_DownloadAction_String__downloadId.DataValidity()) return false;
                        if (!_Method_DownloadCommand_Int32_DownloadAction_String__action.DataValidity()) return false;
                        if (!_Method_DownloadCommand_Int32_DownloadAction_String__fileName.DataValidity()) return false;
                        break;
                    }
                case EPropertyName.Method_EvalJS_String_String:
                    {
                        if (!_Browser) return false;
                        if (!_Method_EvalJS_String_String__script.DataValidity()) return false;
                        if (!_Method_EvalJS_String_String__scriptURL.DataValidity()) return false;
                        break;
                    }
                case EPropertyName.Method_EvalJSCSP_String_String:
                    {
                        if (!_Browser) return false;
                        if (!_Method_EvalJSCSP_String_String__script.DataValidity()) return false;
                        if (!_Method_EvalJSCSP_String_String__scriptURL.DataValidity()) return false;
                        break;
                    }
                case EPropertyName.Method_GetComponent_String:
                    {
                        return _Browser && _Method_GetComponent_String__type.DataValidity();
                    }
                case EPropertyName.Method_GetHashCode:
                    {
                        return _Browser;
                    }
                case EPropertyName.Method_GetInstanceID:
                    {
                        return _Browser;
                    }
                case EPropertyName.Method_GetType:
                    {
                        return _Browser;
                    }
                case EPropertyName.Method_GoBack:
                    {
                        return _Browser;
                    }
                case EPropertyName.Method_GoForward:
                    {
                        return _Browser;
                    }
                case EPropertyName.Method_Invoke_String_Single:
                    {
                        if (!_Browser) return false;
                        if (!_Method_Invoke_String_Single__methodName.DataValidity()) return false;
                        if (!_Method_Invoke_String_Single__time.DataValidity()) return false;
                        break;
                    }
                case EPropertyName.Method_InvokeRepeating_String_Single_Single:
                    {
                        if (!_Browser) return false;
                        if (!_Method_InvokeRepeating_String_Single_Single__methodName.DataValidity()) return false;
                        if (!_Method_InvokeRepeating_String_Single_Single__time.DataValidity()) return false;
                        if (!_Method_InvokeRepeating_String_Single_Single__repeatRate.DataValidity()) return false;
                        break;
                    }
                case EPropertyName.Method_IsInvoking:
                    {
                        return _Browser;
                    }
                case EPropertyName.Method_IsInvoking_String:
                    {
                        return _Browser && _Method_IsInvoking_String__methodName.DataValidity();
                    }
                case EPropertyName.Method_LoadDataURI_String_String:
                    {
                        if (!_Browser) return false;
                        if (!_Method_LoadDataURI_String_String__text.DataValidity()) return false;
                        if (!_Method_LoadDataURI_String_String__mimeType.DataValidity()) return false;
                        break;
                    }
                case EPropertyName.Method_LoadHTML_String_String:
                    {
                        if (!_Browser) return false;
                        if (!_Method_LoadHTML_String_String__html.DataValidity()) return false;
                        if (!_Method_LoadHTML_String_String__url.DataValidity()) return false;
                        break;
                    }
                case EPropertyName.Method_LoadURL_String_Boolean:
                    {
                        if (!_Browser) return false;
                        if (!_Method_LoadURL_String_Boolean__url.DataValidity()) return false;
                        if (!_Method_LoadURL_String_Boolean__force.DataValidity()) return false;
                        break;
                    }
                case EPropertyName.Method_PressKey_KeyCode_KeyAction:
                    {
                        if (!_Browser) return false;
                        if (!_Method_PressKey_KeyCode_KeyAction__key.DataValidity()) return false;
                        if (!_Method_PressKey_KeyCode_KeyAction__action.DataValidity()) return false;
                        break;
                    }
                case EPropertyName.Method_Reload_Boolean:
                    {
                        return _Browser && _Method_Reload_Boolean__force.DataValidity();
                    }
                case EPropertyName.Method_Resize_Int32_Int32:
                    {
                        if (!_Browser) return false;
                        if (!_Method_Resize_Int32_Int32__width.DataValidity()) return false;
                        if (!_Method_Resize_Int32_Int32__height.DataValidity()) return false;
                        break;
                    }
                case EPropertyName.Method_Resize_Texture2D:
                    {
                        return _Browser && _Method_Resize_Texture2D__newTexture.DataValidity();
                    }
                case EPropertyName.Method_SendFrameCommand_FrameCommand:
                    {
                        return _Browser && _Method_SendFrameCommand_FrameCommand__command.DataValidity();
                    }
                case EPropertyName.Method_SendMessage_String:
                    {
                        return _Browser && _Method_SendMessage_String__methodName.DataValidity();
                    }
                case EPropertyName.Method_SendMessage_String_SendMessageOptions:
                    {
                        if (!_Browser) return false;
                        if (!_Method_SendMessage_String_SendMessageOptions__methodName.DataValidity()) return false;
                        if (!_Method_SendMessage_String_SendMessageOptions__options.DataValidity()) return false;
                        break;
                    }
                case EPropertyName.Method_SendMessageUpwards_String:
                    {
                        return _Browser && _Method_SendMessageUpwards_String__methodName.DataValidity();
                    }
                case EPropertyName.Method_SendMessageUpwards_String_SendMessageOptions:
                    {
                        if (!_Browser) return false;
                        if (!_Method_SendMessageUpwards_String_SendMessageOptions__methodName.DataValidity()) return false;
                        if (!_Method_SendMessageUpwards_String_SendMessageOptions__options.DataValidity()) return false;
                        break;
                    }
                case EPropertyName.Method_SetOverlay_Browser:
                    {
                        return _Browser && _Method_SetOverlay_Browser__overlayBrowser.DataValidity();
                    }
                case EPropertyName.Method_ShowDevTools_Boolean:
                    {
                        return _Browser && _Method_ShowDevTools_Boolean__show.DataValidity();
                    }
                case EPropertyName.Method_StartCoroutine_String:
                    {
                        return _Browser && _Method_StartCoroutine_String__methodName.DataValidity();
                    }
                case EPropertyName.Method_Stop:
                    {
                        return _Browser;
                    }
                case EPropertyName.Method_StopAllCoroutines:
                    {
                        return _Browser;
                    }
                case EPropertyName.Method_StopCoroutine_String:
                    {
                        return _Browser && _Method_StopCoroutine_String__methodName.DataValidity();
                    }
                case EPropertyName.Method_ToString:
                    {
                        return _Browser;
                    }
                case EPropertyName.Method_TypeText_String:
                    {
                        return _Browser && _Method_TypeText_String__text.DataValidity();
                    }
                case EPropertyName.Method_UpdateCursor:
                    {
                        return _Browser;
                    }
            }
#endif
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
