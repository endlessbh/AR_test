using static XCSJ.PluginStereoView.Tools.VirtualScreen;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Attributes;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Base;
using XCSJ.PluginStereoView;
using XCSJ.PluginStereoView.Tools;

namespace XCSJ.PluginStereoView.Tools.States
{
    /// <summary>
    /// 虚拟屏幕属性设置: 虚拟屏幕属性设置
    /// </summary>
    [ComponentMenu("立体显示/" + Title, typeof(StereoViewManager))]
    [Name(Title, nameof(VirtualScreenPropertySet))]
    [Tip("虚拟屏幕属性设置")]
    [XCSJ.Attributes.Icon(EIcon.Property)]
    [Owner(typeof(StereoViewManager))]
    public class VirtualScreenPropertySet : BasePropertySet<VirtualScreenPropertySet>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "虚拟屏幕属性设置";
        
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib("立体显示", typeof(StereoViewManager))]
        [StateComponentMenu("立体显示/" + Title, typeof(StereoViewManager))]
        [Name(Title, nameof(VirtualScreenPropertySet))]
        [Tip("虚拟屏幕属性设置")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);
        
        /// <summary>
        /// 虚拟屏幕:现实世界屏幕在虚拟世界中的虚拟对象
        /// </summary>
        [Name("虚拟屏幕")]
        [Tip("现实世界屏幕在虚拟世界中的虚拟对象")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ComponentPopup]
        public VirtualScreen _VirtualScreen;
        
        /// <summary>
        /// 虚拟屏幕:现实世界屏幕在虚拟世界中的虚拟对象
        /// </summary>
        public VirtualScreen VirtualScreen => this.XGetComponentInGlobal(ref _VirtualScreen, true);
        
        /// <summary>
        /// 虚拟屏幕列表:现实世界屏幕在虚拟世界中的虚拟对象列表
        /// </summary>
        [Name("虚拟屏幕列表")]
        [Tip("现实世界屏幕在虚拟世界中的虚拟对象列表")]
        public List<VirtualScreen> _VirtualScreens = new List<VirtualScreen>();
        
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
            [Tip("无")]
            [EnumFieldName("无")]
            None,
            
            #region 字段
            
            /// <summary>
            /// 屏幕尺寸(字段):X为宽，Y为高,Z为厚度；单位：米；
            /// </summary>
            [Name("屏幕尺寸(字段)")]
            [Tip("X为宽，Y为高,Z为厚度；单位：米；")]
            [EnumFieldName("字段/屏幕尺寸")]
            Field__screenSize = 1,
            
            /// <summary>
            /// 屏幕尺寸规则(字段):屏幕尺寸规则
            /// </summary>
            [Name("屏幕尺寸规则(字段)")]
            [Tip("屏幕尺寸规则")]
            [EnumFieldName("字段/屏幕尺寸规则")]
            Field__screenSizeRule,
            
            #endregion
            
            #region 属性
            
            /// <summary>
            /// 启用(属性):启用
            /// </summary>
            [Name("启用(属性)")]
            [Tip("启用")]
            [EnumFieldName("属性/启用")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Property_enabled = 1000,
            
            /// <summary>
            /// 隐藏标识(属性):隐藏标识
            /// </summary>
            [Name("隐藏标识(属性)")]
            [Tip("隐藏标识")]
            [EnumFieldName("属性/隐藏标识")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Property_hideFlags,
            
            /// <summary>
            /// 名称(属性):名称
            /// </summary>
            [Name("名称(属性)")]
            [Tip("名称")]
            [EnumFieldName("属性/名称")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Property_name,
            
            /// <summary>
            /// 屏幕尺寸(属性):X为宽，Y为高,Z为厚度；单位：米；
            /// </summary>
            [Name("屏幕尺寸(属性)")]
            [Tip("X为宽，Y为高,Z为厚度；单位：米；")]
            [EnumFieldName("属性/屏幕尺寸")]
            Property_screenSize,
            
            /// <summary>
            /// 标签(属性):标签
            /// </summary>
            [Name("标签(属性)")]
            [Tip("标签")]
            [EnumFieldName("属性/标签")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Property_tag,
            
            /// <summary>
            /// 使用GUI布局(属性):使用GUI布局
            /// </summary>
            [Name("使用GUI布局(属性)")]
            [Tip("使用GUI布局")]
            [EnumFieldName("属性/使用GUI布局")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Property_useGUILayout,
            
            #endregion
            
            #region 方法
            
            /// <summary>
            /// 广播消息(方法名)(方法):广播消息(方法名)
            /// </summary>
            [Name("广播消息(方法名)(方法)")]
            [Tip("广播消息(方法名)")]
            [EnumFieldName("方法/广播消息(方法名)")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_BroadcastMessage_String = 10000,
            
            /// <summary>
            /// 广播消息(方法名+发送消息选项)(方法):广播消息(方法名+发送消息选项)
            /// </summary>
            [Name("广播消息(方法名+发送消息选项)(方法)")]
            [Tip("广播消息(方法名+发送消息选项)")]
            [EnumFieldName("方法/广播消息(方法名+发送消息选项)")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_BroadcastMessage_String_SendMessageOptions,
            
            /// <summary>
            /// 调用屏幕变更(方法):调用屏幕变更
            /// </summary>
            [Name("调用屏幕变更(方法)")]
            [Tip("调用屏幕变更")]
            [EnumFieldName("方法/调用屏幕变更")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_CallScreenChanged,
            
            /// <summary>
            /// 取消调用(方法):取消调用
            /// </summary>
            [Name("取消调用(方法)")]
            [Tip("取消调用")]
            [EnumFieldName("方法/取消调用")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_CancelInvoke,
            
            /// <summary>
            /// 取消调用(方法名)(方法):取消调用(方法名)
            /// </summary>
            [Name("取消调用(方法名)(方法)")]
            [Tip("取消调用(方法名)")]
            [EnumFieldName("方法/取消调用(方法名)")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_CancelInvoke_String,
            
            /// <summary>
            /// 比较标签(标签)(方法):比较标签(标签)
            /// </summary>
            [Name("比较标签(标签)(方法)")]
            [Tip("比较标签(标签)")]
            [EnumFieldName("方法/比较标签(标签)")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_CompareTag_String,
            
            /// <summary>
            /// 创建屏幕(方法):创建屏幕
            /// </summary>
            [Name("创建屏幕(方法)")]
            [Tip("创建屏幕")]
            [EnumFieldName("方法/创建屏幕")]
            Method_CreateScreen_String_Transform,
            
            /// <summary>
            /// 获取锚点本地位置(方法):获取锚点本地位置
            /// </summary>
            [Name("获取锚点本地位置(方法)")]
            [Tip("获取锚点本地位置")]
            [EnumFieldName("方法/获取锚点本地位置")]
            Method_GetAnchorLocalPosition_ERectAnchor,
            
            /// <summary>
            /// 获取锚点位置(方法):获取锚点位置
            /// </summary>
            [Name("获取锚点位置(方法)")]
            [Tip("获取锚点位置")]
            [EnumFieldName("方法/获取锚点位置")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_GetAnchorPosition_ERectAnchor_Vector3_ESpaceType,
            
            /// <summary>
            /// 获取组件(类型)(方法):获取组件(类型)
            /// </summary>
            [Name("获取组件(类型)(方法)")]
            [Tip("获取组件(类型)")]
            [EnumFieldName("方法/获取组件(类型)")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_GetComponent_String,
            
            /// <summary>
            /// 获取哈希码(方法):获取哈希码
            /// </summary>
            [Name("获取哈希码(方法)")]
            [Tip("获取哈希码")]
            [EnumFieldName("方法/获取哈希码")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_GetHashCode,
            
            /// <summary>
            /// 获取实例ID(方法):获取实例ID
            /// </summary>
            [Name("获取实例ID(方法)")]
            [Tip("获取实例ID")]
            [EnumFieldName("方法/获取实例ID")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_GetInstanceID,
            
            /// <summary>
            /// 获取类型(方法):获取类型
            /// </summary>
            [Name("获取类型(方法)")]
            [Tip("获取类型")]
            [EnumFieldName("方法/获取类型")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_GetType,
            
            /// <summary>
            /// 是否调用(方法名+时间)(方法):是否调用(方法名+时间)
            /// </summary>
            [Name("是否调用(方法名+时间)(方法)")]
            [Tip("是否调用(方法名+时间)")]
            [EnumFieldName("方法/是否调用(方法名+时间)")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_Invoke_String_Single,
            
            /// <summary>
            /// 重复调用(方法):重复调用
            /// </summary>
            [Name("重复调用(方法)")]
            [Tip("重复调用")]
            [EnumFieldName("方法/重复调用")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_InvokeRepeating_String_Single_Single,
            
            /// <summary>
            /// 是否调用(方法):是否调用
            /// </summary>
            [Name("是否调用(方法)")]
            [Tip("是否调用")]
            [EnumFieldName("方法/是否调用")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_IsInvoking,
            
            /// <summary>
            /// 是否调用(方法名)(方法):是否调用(方法名)
            /// </summary>
            [Name("是否调用(方法名)(方法)")]
            [Tip("是否调用(方法名)")]
            [EnumFieldName("方法/是否调用(方法名)")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_IsInvoking_String,
            
            /// <summary>
            /// 布局相机(方法):布局相机
            /// </summary>
            [Name("布局相机(方法)")]
            [Tip("布局相机")]
            [EnumFieldName("方法/布局相机")]
            Method_LayoutCamera_Int32_Camera_EScreenLayoutMode,
            
            /// <summary>
            /// 重置(方法):重置
            /// </summary>
            [Name("重置(方法)")]
            [Tip("重置")]
            [EnumFieldName("方法/重置")]
            Method_Reset,

            /// <summary>
            /// 发送消息(方法名)(方法):发送消息(方法名)
            /// </summary>
            [Name("发送消息(方法名)(方法)")]
            [Tip("发送消息(方法名)")]
            [EnumFieldName("方法/发送消息(方法名)")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_SendMessage_String,

            /// <summary>
            /// 发送消息(方法名+发送消息选项)(方法):发送消息(方法名+发送消息选项)
            /// </summary>
            [Name("发送消息(方法名+发送消息选项)(方法)")]
            [Tip("发送消息(方法名+发送消息选项)")]
            [EnumFieldName("方法/发送消息(方法名+发送消息选项)")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_SendMessage_String_SendMessageOptions,

            /// <summary>
            /// 向上发送消息(方法名)(方法):向上发送消息(方法名)
            /// </summary>
            [Name("向上发送消息(方法名)(方法)")]
            [Tip("向上发送消息(方法名)")]
            [EnumFieldName("方法/向上发送消息(方法名)")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_SendMessageUpwards_String,

            /// <summary>
            /// 向上发送消息(方法名+发送消息选项)(方法):向上发送消息(方法名+发送消息选项)
            /// </summary>
            [Name("向上发送消息(方法名+发送消息选项)(方法)")]
            [Tip("向上发送消息(方法名+发送消息选项)")]
            [EnumFieldName("方法/向上发送消息(方法名+发送消息选项)")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_SendMessageUpwards_String_SendMessageOptions,

            /// <summary>
            /// 启动协程(方法):启动协程
            /// </summary>
            [Name("启动协程(方法)")]
            [Tip("启动协程")]
            [EnumFieldName("方法/启动协程")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_StartCoroutine_String,
            
            /// <summary>
            /// 停止全部协程(方法):停止全部协程
            /// </summary>
            [Name("停止全部协程(方法)")]
            [Tip("停止全部协程")]
            [EnumFieldName("方法/停止全部协程")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_StopAllCoroutines,
            
            /// <summary>
            /// 停止协程(方法):停止协程
            /// </summary>
            [Name("停止协程(方法)")]
            [Tip("停止协程")]
            [EnumFieldName("方法/停止协程")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_StopCoroutine_String,
            
            /// <summary>
            /// 转字符串(方法):转字符串
            /// </summary>
            [Name("转字符串(方法)")]
            [Tip("转字符串")]
            [EnumFieldName("方法/转字符串")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_ToString,
            
            #endregion
            
        }
        
        #region 字段
        
        /// <summary>
        /// 屏幕尺寸:X为宽，Y为高,Z为厚度；单位：米；
        /// </summary>
        [Name("屏幕尺寸")]
        [Tip("X为宽，Y为高,Z为厚度；单位：米；")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Field__screenSize)]
        [OnlyMemberElements]
        public Vector3PropertyValue _Field__screenSize = new Vector3PropertyValue();
        
        /// <summary>
        /// 屏幕尺寸规则属性值
        /// </summary>
        [Serializable]
        [Name("屏幕尺寸规则属性值")]
        public class EScreenSizeRulePropertyValue : EnumPropertyValue<EScreenSizeRule>
        {
        }
        
        /// <summary>
        /// 屏幕尺寸规则:屏幕尺寸规则
        /// </summary>
        [Name("屏幕尺寸规则")]
        [Tip("屏幕尺寸规则")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Field__screenSizeRule)]
        [OnlyMemberElements]
        public EScreenSizeRulePropertyValue _Field__screenSizeRule = new EScreenSizeRulePropertyValue();
        
        #endregion
        
        #region 属性
        
        /// <summary>
        /// 启用:启用
        /// </summary>
        [Name("启用")]
        [Tip("启用")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Property_enabled)]
        [OnlyMemberElements]
        public BoolPropertyValue _Property_enabled = new BoolPropertyValue();
        
        /// <summary>
        /// 隐藏标识:隐藏标识
        /// </summary>
        [Name("隐藏标识")]
        [Tip("隐藏标识")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Property_hideFlags)]
        [OnlyMemberElements]
        public HideFlagsPropertyValue _Property_hideFlags = new HideFlagsPropertyValue();
        
        /// <summary>
        /// 名称:名称
        /// </summary>
        [Name("名称")]
        [Tip("名称")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Property_name)]
        [OnlyMemberElements]
        public StringPropertyValue _Property_name = new StringPropertyValue();
        
        /// <summary>
        /// 屏幕尺寸:X为宽，Y为高,Z为厚度；单位：米；
        /// </summary>
        [Name("屏幕尺寸")]
        [Tip("X为宽，Y为高,Z为厚度；单位：米；")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Property_screenSize)]
        [OnlyMemberElements]
        public Vector3PropertyValue _Property_screenSize = new Vector3PropertyValue();
        
        /// <summary>
        /// 标签:标签
        /// </summary>
        [Name("标签")]
        [Tip("标签")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Property_tag)]
        [OnlyMemberElements]
        public StringPropertyValue _Property_tag = new StringPropertyValue();
        
        /// <summary>
        /// 使用GUI布局:使用GUI布局
        /// </summary>
        [Name("使用GUI布局")]
        [Tip("使用GUI布局")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Property_useGUILayout)]
        [OnlyMemberElements]
        public BoolPropertyValue _Property_useGUILayout = new BoolPropertyValue();
        
        #endregion
        
        #region 方法
        
        /// <summary>
        /// 方法名:方法名
        /// </summary>
        [Name("方法名")]
        [Tip("方法名")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_BroadcastMessage_String)]
        [OnlyMemberElements]
        public StringPropertyValue _Method_BroadcastMessage_String__methodName = new StringPropertyValue();
        
        /// <summary>
        /// 方法名:方法名
        /// </summary>
        [Name("方法名")]
        [Tip("方法名")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_BroadcastMessage_String_SendMessageOptions)]
        [OnlyMemberElements]
        public StringPropertyValue _Method_BroadcastMessage_String_SendMessageOptions__methodName = new StringPropertyValue();
        
        /// <summary>
        /// 选项:选项
        /// </summary>
        [Name("选项")]
        [Tip("选项")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_BroadcastMessage_String_SendMessageOptions)]
        [OnlyMemberElements]
        public SendMessageOptionsPropertyValue _Method_BroadcastMessage_String_SendMessageOptions__options = new SendMessageOptionsPropertyValue();
        
        /// <summary>
        /// 方法名:方法名
        /// </summary>
        [Name("方法名")]
        [Tip("方法名")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_CancelInvoke_String)]
        [OnlyMemberElements]
        public StringPropertyValue _Method_CancelInvoke_String__methodName = new StringPropertyValue();
        
        /// <summary>
        /// 标签:标签
        /// </summary>
        [Name("标签")]
        [Tip("标签")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_CompareTag_String)]
        [OnlyMemberElements]
        public StringPropertyValue _Method_CompareTag_String__tag = new StringPropertyValue();
        
        /// <summary>
        /// 名称:名称
        /// </summary>
        [Name("名称")]
        [Tip("名称")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_CreateScreen_String_Transform)]
        [OnlyMemberElements]
        public StringPropertyValue _Method_CreateScreen_String_Transform__name = new StringPropertyValue();
        
        /// <summary>
        /// 父级:父级
        /// </summary>
        [Name("父级")]
        [Tip("父级")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_CreateScreen_String_Transform)]
        [OnlyMemberElements]
        public TransformPropertyValue _Method_CreateScreen_String_Transform__parent = new TransformPropertyValue();
        
        /// <summary>
        /// 矩形锚点:按照上中下，左中右顺序定义矩形特征锚点；以处于矩形中心方式，理解上中下，左中右；
        /// </summary>
        [Name("矩形锚点")]
        [Tip("按照上中下，左中右顺序定义矩形特征锚点；以处于矩形中心方式，理解上中下，左中右；")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_GetAnchorLocalPosition_ERectAnchor)]
        [OnlyMemberElements]
        public ERectAnchorPropertyValue _Method_GetAnchorLocalPosition_ERectAnchor__screenAnchor = new ERectAnchorPropertyValue();
        
        /// <summary>
        /// 矩形锚点:按照上中下，左中右顺序定义矩形特征锚点；以处于矩形中心方式，理解上中下，左中右；
        /// </summary>
        [Name("矩形锚点")]
        [Tip("按照上中下，左中右顺序定义矩形特征锚点；以处于矩形中心方式，理解上中下，左中右；")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_GetAnchorPosition_ERectAnchor_Vector3_ESpaceType)]
        [OnlyMemberElements]
        public ERectAnchorPropertyValue _Method_GetAnchorPosition_ERectAnchor_Vector3_ESpaceType__screenAnchor = new ERectAnchorPropertyValue();
        
        /// <summary>
        /// anchorOffset:anchorOffset
        /// </summary>
        [Name("anchorOffset")]
        [Tip("anchorOffset")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_GetAnchorPosition_ERectAnchor_Vector3_ESpaceType)]
        [OnlyMemberElements]
        public Vector3PropertyValue _Method_GetAnchorPosition_ERectAnchor_Vector3_ESpaceType__anchorOffset = new Vector3PropertyValue();
        
        /// <summary>
        /// 空间类型:空间类型
        /// </summary>
        [Name("空间类型")]
        [Tip("空间类型")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_GetAnchorPosition_ERectAnchor_Vector3_ESpaceType)]
        [OnlyMemberElements]
        public ESpaceTypePropertyValue _Method_GetAnchorPosition_ERectAnchor_Vector3_ESpaceType__spaceType = new ESpaceTypePropertyValue();
        
        /// <summary>
        /// 类型:类型
        /// </summary>
        [Name("类型")]
        [Tip("类型")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_GetComponent_String)]
        [OnlyMemberElements]
        public StringPropertyValue _Method_GetComponent_String__type = new StringPropertyValue();
        
        /// <summary>
        /// 方法名:方法名
        /// </summary>
        [Name("方法名")]
        [Tip("方法名")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_Invoke_String_Single)]
        [OnlyMemberElements]
        public StringPropertyValue _Method_Invoke_String_Single__methodName = new StringPropertyValue();
        
        /// <summary>
        /// 时间:时间
        /// </summary>
        [Name("时间")]
        [Tip("时间")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_Invoke_String_Single)]
        [OnlyMemberElements]
        public FloatPropertyValue _Method_Invoke_String_Single__time = new FloatPropertyValue();
        
        /// <summary>
        /// 方法名:方法名
        /// </summary>
        [Name("方法名")]
        [Tip("方法名")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_InvokeRepeating_String_Single_Single)]
        [OnlyMemberElements]
        public StringPropertyValue _Method_InvokeRepeating_String_Single_Single__methodName = new StringPropertyValue();
        
        /// <summary>
        /// 时间:时间
        /// </summary>
        [Name("时间")]
        [Tip("时间")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_InvokeRepeating_String_Single_Single)]
        [OnlyMemberElements]
        public FloatPropertyValue _Method_InvokeRepeating_String_Single_Single__time = new FloatPropertyValue();
        
        /// <summary>
        /// 重复率:重复率
        /// </summary>
        [Name("重复率")]
        [Tip("重复率")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_InvokeRepeating_String_Single_Single)]
        [OnlyMemberElements]
        public FloatPropertyValue _Method_InvokeRepeating_String_Single_Single__repeatRate = new FloatPropertyValue();
        
        /// <summary>
        /// 方法名:方法名
        /// </summary>
        [Name("方法名")]
        [Tip("方法名")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_IsInvoking_String)]
        [OnlyMemberElements]
        public StringPropertyValue _Method_IsInvoking_String__methodName = new StringPropertyValue();
        
        /// <summary>
        /// 索引:索引
        /// </summary>
        [Name("索引")]
        [Tip("索引")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_LayoutCamera_Int32_Camera_EScreenLayoutMode)]
        [OnlyMemberElements]
        public IntPropertyValue _Method_LayoutCamera_Int32_Camera_EScreenLayoutMode__index = new IntPropertyValue();
        
        /// <summary>
        /// 相机:相机
        /// </summary>
        [Name("相机")]
        [Tip("相机")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_LayoutCamera_Int32_Camera_EScreenLayoutMode)]
        [OnlyMemberElements]
        public CameraPropertyValue _Method_LayoutCamera_Int32_Camera_EScreenLayoutMode__camera = new CameraPropertyValue();
        
        /// <summary>
        /// screenLayoutMode:screenLayoutMode
        /// </summary>
        [Name("screenLayoutMode")]
        [Tip("screenLayoutMode")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_LayoutCamera_Int32_Camera_EScreenLayoutMode)]
        [OnlyMemberElements]
        public EScreenLayoutModePropertyValue _Method_LayoutCamera_Int32_Camera_EScreenLayoutMode__screenLayoutMode = new EScreenLayoutModePropertyValue();
        
        /// <summary>
        /// 方法名:方法名
        /// </summary>
        [Name("方法名")]
        [Tip("方法名")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_SendMessage_String)]
        [OnlyMemberElements]
        public StringPropertyValue _Method_SendMessage_String__methodName = new StringPropertyValue();
        
        /// <summary>
        /// 方法名:方法名
        /// </summary>
        [Name("方法名")]
        [Tip("方法名")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_SendMessage_String_SendMessageOptions)]
        [OnlyMemberElements]
        public StringPropertyValue _Method_SendMessage_String_SendMessageOptions__methodName = new StringPropertyValue();
        
        /// <summary>
        /// 选项:选项
        /// </summary>
        [Name("选项")]
        [Tip("选项")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_SendMessage_String_SendMessageOptions)]
        [OnlyMemberElements]
        public SendMessageOptionsPropertyValue _Method_SendMessage_String_SendMessageOptions__options = new SendMessageOptionsPropertyValue();
        
        /// <summary>
        /// 方法名:方法名
        /// </summary>
        [Name("方法名")]
        [Tip("方法名")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_SendMessageUpwards_String)]
        [OnlyMemberElements]
        public StringPropertyValue _Method_SendMessageUpwards_String__methodName = new StringPropertyValue();
        
        /// <summary>
        /// 方法名:方法名
        /// </summary>
        [Name("方法名")]
        [Tip("方法名")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_SendMessageUpwards_String_SendMessageOptions)]
        [OnlyMemberElements]
        public StringPropertyValue _Method_SendMessageUpwards_String_SendMessageOptions__methodName = new StringPropertyValue();
        
        /// <summary>
        /// 选项:选项
        /// </summary>
        [Name("选项")]
        [Tip("选项")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_SendMessageUpwards_String_SendMessageOptions)]
        [OnlyMemberElements]
        public SendMessageOptionsPropertyValue _Method_SendMessageUpwards_String_SendMessageOptions__options = new SendMessageOptionsPropertyValue();
        
        /// <summary>
        /// 方法名:方法名
        /// </summary>
        [Name("方法名")]
        [Tip("方法名")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_StartCoroutine_String)]
        [OnlyMemberElements]
        public StringPropertyValue _Method_StartCoroutine_String__methodName = new StringPropertyValue();
        
        /// <summary>
        /// 方法名:方法名
        /// </summary>
        [Name("方法名")]
        [Tip("方法名")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_StopCoroutine_String)]
        [OnlyMemberElements]
        public StringPropertyValue _Method_StopCoroutine_String__methodName = new StringPropertyValue();
        
        #endregion
        
        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="stateData">数据信息</param>
        /// <param name="executeMode">执行模式</param>
        public override void Execute(StateData stateData, EExecuteMode executeMode)
        {
            switch (_propertyName)
            {
                case EPropertyName.Field__screenSize:
                    {
                        var value = _Field__screenSize.GetValue();
                        if (_VirtualScreen != null) _VirtualScreen._screenSize = value;
                        foreach(var obj in _VirtualScreens)
                        {
                            if (obj != null) obj._screenSize = value;
                        }
                        break;
                    }
                case EPropertyName.Field__screenSizeRule:
                    {
                        var value = _Field__screenSizeRule.GetValue();
                        if (_VirtualScreen != null) _VirtualScreen._screenSizeRule = value;
                        foreach(var obj in _VirtualScreens)
                        {
                            if (obj != null) obj._screenSizeRule = value;
                        }
                        break;
                    }
                case EPropertyName.Property_enabled:
                    {
                        var value = _Property_enabled.GetValue();
                        if (_VirtualScreen != null) _VirtualScreen.enabled = value;
                        foreach(var obj in _VirtualScreens)
                        {
                            if (obj != null) obj.enabled = value;
                        }
                        break;
                    }
                case EPropertyName.Property_hideFlags:
                    {
                        var value = _Property_hideFlags.GetValue();
                        if (_VirtualScreen != null) _VirtualScreen.hideFlags = value;
                        foreach(var obj in _VirtualScreens)
                        {
                            if (obj != null) obj.hideFlags = value;
                        }
                        break;
                    }
                case EPropertyName.Property_name:
                    {
                        var value = _Property_name.GetValue();
                        if (_VirtualScreen != null) _VirtualScreen.name = value;
                        foreach(var obj in _VirtualScreens)
                        {
                            if (obj != null) obj.name = value;
                        }
                        break;
                    }
                case EPropertyName.Property_screenSize:
                    {
                        var value = _Property_screenSize.GetValue();
                        if (_VirtualScreen != null) _VirtualScreen.screenSize = value;
                        foreach(var obj in _VirtualScreens)
                        {
                            if (obj != null) obj.screenSize = value;
                        }
                        break;
                    }
                case EPropertyName.Property_tag:
                    {
                        var value = _Property_tag.GetValue();
                        if (_VirtualScreen != null) _VirtualScreen.tag = value;
                        foreach(var obj in _VirtualScreens)
                        {
                            if (obj != null) obj.tag = value;
                        }
                        break;
                    }
                case EPropertyName.Property_useGUILayout:
                    {
                        var value = _Property_useGUILayout.GetValue();
                        if (_VirtualScreen != null) _VirtualScreen.useGUILayout = value;
                        foreach(var obj in _VirtualScreens)
                        {
                            if (obj != null) obj.useGUILayout = value;
                        }
                        break;
                    }
                case EPropertyName.Method_BroadcastMessage_String:
                    {
                        var methodName = _Method_BroadcastMessage_String__methodName.GetValue();
                        if (_VirtualScreen != null) _VirtualScreen.BroadcastMessage(methodName);
                        foreach(var obj in _VirtualScreens)
                        {
                            if (obj != null) obj.BroadcastMessage(methodName);
                        }
                        break;
                    }
                case EPropertyName.Method_BroadcastMessage_String_SendMessageOptions:
                    {
                        var methodName = _Method_BroadcastMessage_String_SendMessageOptions__methodName.GetValue();
                        var options = _Method_BroadcastMessage_String_SendMessageOptions__options.GetValue();
                        if (_VirtualScreen != null) _VirtualScreen.BroadcastMessage(methodName, options);
                        foreach(var obj in _VirtualScreens)
                        {
                            if (obj != null) obj.BroadcastMessage(methodName, options);
                        }
                        break;
                    }
                case EPropertyName.Method_CallScreenChanged:
                    {
                        if (_VirtualScreen != null) _VirtualScreen.CallScreenChanged();
                        foreach(var obj in _VirtualScreens)
                        {
                            if (obj != null) obj.CallScreenChanged();
                        }
                        break;
                    }
                case EPropertyName.Method_CancelInvoke:
                    {
                        if (_VirtualScreen != null) _VirtualScreen.CancelInvoke();
                        foreach(var obj in _VirtualScreens)
                        {
                            if (obj != null) obj.CancelInvoke();
                        }
                        break;
                    }
                case EPropertyName.Method_CancelInvoke_String:
                    {
                        var methodName = _Method_CancelInvoke_String__methodName.GetValue();
                        if (_VirtualScreen != null) _VirtualScreen.CancelInvoke(methodName);
                        foreach(var obj in _VirtualScreens)
                        {
                            if (obj != null) obj.CancelInvoke(methodName);
                        }
                        break;
                    }
                case EPropertyName.Method_CompareTag_String:
                    {
                        var tag = _Method_CompareTag_String__tag.GetValue();
                        if (_VirtualScreen != null) _VirtualScreen.CompareTag(tag);
                        foreach(var obj in _VirtualScreens)
                        {
                            if (obj != null) obj.CompareTag(tag);
                        }
                        break;
                    }
                case EPropertyName.Method_CreateScreen_String_Transform:
                    {
                        var name = _Method_CreateScreen_String_Transform__name.GetValue();
                        var parent = _Method_CreateScreen_String_Transform__parent.GetValue();
                        VirtualScreen.CreateScreen(name, parent);
                        break;
                    }
                case EPropertyName.Method_GetAnchorLocalPosition_ERectAnchor:
                    {
                        var screenAnchor = _Method_GetAnchorLocalPosition_ERectAnchor__screenAnchor.GetValue();
                        if (_VirtualScreen != null) _VirtualScreen.GetAnchorLocalPosition(screenAnchor);
                        foreach(var obj in _VirtualScreens)
                        {
                            if (obj != null) obj.GetAnchorLocalPosition(screenAnchor);
                        }
                        break;
                    }
                case EPropertyName.Method_GetAnchorPosition_ERectAnchor_Vector3_ESpaceType:
                    {
                        var screenAnchor = _Method_GetAnchorPosition_ERectAnchor_Vector3_ESpaceType__screenAnchor.GetValue();
                        var anchorOffset = _Method_GetAnchorPosition_ERectAnchor_Vector3_ESpaceType__anchorOffset.GetValue();
                        var spaceType = _Method_GetAnchorPosition_ERectAnchor_Vector3_ESpaceType__spaceType.GetValue();
                        if (_VirtualScreen != null) _VirtualScreen.GetAnchorPosition(screenAnchor, anchorOffset, spaceType);
                        foreach(var obj in _VirtualScreens)
                        {
                            if (obj != null) obj.GetAnchorPosition(screenAnchor, anchorOffset, spaceType);
                        }
                        break;
                    }
                case EPropertyName.Method_GetComponent_String:
                    {
                        var type = _Method_GetComponent_String__type.GetValue();
                        if (_VirtualScreen != null) _VirtualScreen.GetComponent(type);
                        foreach(var obj in _VirtualScreens)
                        {
                            if (obj != null) obj.GetComponent(type);
                        }
                        break;
                    }
                case EPropertyName.Method_GetHashCode:
                    {
                        if (_VirtualScreen != null) _VirtualScreen.GetHashCode();
                        foreach(var obj in _VirtualScreens)
                        {
                            if (obj != null) obj.GetHashCode();
                        }
                        break;
                    }
                case EPropertyName.Method_GetInstanceID:
                    {
                        if (_VirtualScreen != null) _VirtualScreen.GetInstanceID();
                        foreach(var obj in _VirtualScreens)
                        {
                            if (obj != null) obj.GetInstanceID();
                        }
                        break;
                    }
                case EPropertyName.Method_GetType:
                    {
                        if (_VirtualScreen != null) _VirtualScreen.GetType();
                        foreach(var obj in _VirtualScreens)
                        {
                            if (obj != null) obj.GetType();
                        }
                        break;
                    }
                case EPropertyName.Method_Invoke_String_Single:
                    {
                        var methodName = _Method_Invoke_String_Single__methodName.GetValue();
                        var time = _Method_Invoke_String_Single__time.GetValue();
                        if (_VirtualScreen != null) _VirtualScreen.Invoke(methodName, time);
                        foreach(var obj in _VirtualScreens)
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
                        if (_VirtualScreen != null) _VirtualScreen.InvokeRepeating(methodName, time, repeatRate);
                        foreach(var obj in _VirtualScreens)
                        {
                            if (obj != null) obj.InvokeRepeating(methodName, time, repeatRate);
                        }
                        break;
                    }
                case EPropertyName.Method_IsInvoking:
                    {
                        if (_VirtualScreen != null) _VirtualScreen.IsInvoking();
                        foreach(var obj in _VirtualScreens)
                        {
                            if (obj != null) obj.IsInvoking();
                        }
                        break;
                    }
                case EPropertyName.Method_IsInvoking_String:
                    {
                        var methodName = _Method_IsInvoking_String__methodName.GetValue();
                        if (_VirtualScreen != null) _VirtualScreen.IsInvoking(methodName);
                        foreach(var obj in _VirtualScreens)
                        {
                            if (obj != null) obj.IsInvoking(methodName);
                        }
                        break;
                    }
                case EPropertyName.Method_LayoutCamera_Int32_Camera_EScreenLayoutMode:
                    {
                        var index = _Method_LayoutCamera_Int32_Camera_EScreenLayoutMode__index.GetValue();
                        var camera = _Method_LayoutCamera_Int32_Camera_EScreenLayoutMode__camera.GetValue();
                        var screenLayoutMode = _Method_LayoutCamera_Int32_Camera_EScreenLayoutMode__screenLayoutMode.GetValue();
                        VirtualScreen.LayoutCamera(index, camera, screenLayoutMode);
                        break;
                    }
                case EPropertyName.Method_Reset:
                    {
                        if (_VirtualScreen != null) _VirtualScreen.Reset();
                        foreach(var obj in _VirtualScreens)
                        {
                            if (obj != null) obj.Reset();
                        }
                        break;
                    }
                case EPropertyName.Method_SendMessage_String:
                    {
                        var methodName = _Method_SendMessage_String__methodName.GetValue();
                        if (_VirtualScreen != null) _VirtualScreen.SendMessage(methodName);
                        foreach(var obj in _VirtualScreens)
                        {
                            if (obj != null) obj.SendMessage(methodName);
                        }
                        break;
                    }
                case EPropertyName.Method_SendMessage_String_SendMessageOptions:
                    {
                        var methodName = _Method_SendMessage_String_SendMessageOptions__methodName.GetValue();
                        var options = _Method_SendMessage_String_SendMessageOptions__options.GetValue();
                        if (_VirtualScreen != null) _VirtualScreen.SendMessage(methodName, options);
                        foreach(var obj in _VirtualScreens)
                        {
                            if (obj != null) obj.SendMessage(methodName, options);
                        }
                        break;
                    }
                case EPropertyName.Method_SendMessageUpwards_String:
                    {
                        var methodName = _Method_SendMessageUpwards_String__methodName.GetValue();
                        if (_VirtualScreen != null) _VirtualScreen.SendMessageUpwards(methodName);
                        foreach(var obj in _VirtualScreens)
                        {
                            if (obj != null) obj.SendMessageUpwards(methodName);
                        }
                        break;
                    }
                case EPropertyName.Method_SendMessageUpwards_String_SendMessageOptions:
                    {
                        var methodName = _Method_SendMessageUpwards_String_SendMessageOptions__methodName.GetValue();
                        var options = _Method_SendMessageUpwards_String_SendMessageOptions__options.GetValue();
                        if (_VirtualScreen != null) _VirtualScreen.SendMessageUpwards(methodName, options);
                        foreach(var obj in _VirtualScreens)
                        {
                            if (obj != null) obj.SendMessageUpwards(methodName, options);
                        }
                        break;
                    }
                case EPropertyName.Method_StartCoroutine_String:
                    {
                        var methodName = _Method_StartCoroutine_String__methodName.GetValue();
                        if (_VirtualScreen != null) _VirtualScreen.StartCoroutine(methodName);
                        foreach(var obj in _VirtualScreens)
                        {
                            if (obj != null) obj.StartCoroutine(methodName);
                        }
                        break;
                    }
                case EPropertyName.Method_StopAllCoroutines:
                    {
                        if (_VirtualScreen != null) _VirtualScreen.StopAllCoroutines();
                        foreach(var obj in _VirtualScreens)
                        {
                            if (obj != null) obj.StopAllCoroutines();
                        }
                        break;
                    }
                case EPropertyName.Method_StopCoroutine_String:
                    {
                        var methodName = _Method_StopCoroutine_String__methodName.GetValue();
                        if (_VirtualScreen != null) _VirtualScreen.StopCoroutine(methodName);
                        foreach(var obj in _VirtualScreens)
                        {
                            if (obj != null) obj.StopCoroutine(methodName);
                        }
                        break;
                    }
                case EPropertyName.Method_ToString:
                    {
                        if (_VirtualScreen != null) _VirtualScreen.ToString();
                        foreach(var obj in _VirtualScreens)
                        {
                            if (obj != null) obj.ToString();
                        }
                        break;
                    }
            }
        }
        
        /// <summary>
        /// 输出友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString()
        {
            switch (_propertyName)
            {
                case EPropertyName.Field__screenSize:
                    {
                        return CommonFun.Name(_propertyName) + " = " + _Field__screenSize.ToFriendlyString();
                    }
                case EPropertyName.Field__screenSizeRule:
                    {
                        return CommonFun.Name(_propertyName) + " = " + _Field__screenSizeRule.ToFriendlyString();
                    }
                case EPropertyName.Property_enabled:
                    {
                        return CommonFun.Name(_propertyName) + " = " + _Property_enabled.ToFriendlyString();
                    }
                case EPropertyName.Property_hideFlags:
                    {
                        return CommonFun.Name(_propertyName) + " = " + _Property_hideFlags.ToFriendlyString();
                    }
                case EPropertyName.Property_name:
                    {
                        return CommonFun.Name(_propertyName) + " = " + _Property_name.ToFriendlyString();
                    }
                case EPropertyName.Property_screenSize:
                    {
                        return CommonFun.Name(_propertyName) + " = " + _Property_screenSize.ToFriendlyString();
                    }
                case EPropertyName.Property_tag:
                    {
                        return CommonFun.Name(_propertyName) + " = " + _Property_tag.ToFriendlyString();
                    }
                case EPropertyName.Property_useGUILayout:
                    {
                        return CommonFun.Name(_propertyName) + " = " + _Property_useGUILayout.ToFriendlyString();
                    }
                case EPropertyName.Method_BroadcastMessage_String:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_BroadcastMessage_String_SendMessageOptions:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_CallScreenChanged:
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
                case EPropertyName.Method_CreateScreen_String_Transform:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_GetAnchorLocalPosition_ERectAnchor:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_GetAnchorPosition_ERectAnchor_Vector3_ESpaceType:
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
                case EPropertyName.Method_LayoutCamera_Int32_Camera_EScreenLayoutMode:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_Reset:
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
                case EPropertyName.Method_StartCoroutine_String:
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
            }
            return base.ToFriendlyString();
        }
        
        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity()
        {
            switch (_propertyName)
            {
                case EPropertyName.Field__screenSize:
                    {
                        return _VirtualScreen && _Field__screenSize.DataValidity();
                    }
                case EPropertyName.Field__screenSizeRule:
                    {
                        return _VirtualScreen && _Field__screenSizeRule.DataValidity();
                    }
                case EPropertyName.Property_enabled:
                    {
                        return _VirtualScreen && _Property_enabled.DataValidity();
                    }
                case EPropertyName.Property_hideFlags:
                    {
                        return _VirtualScreen && _Property_hideFlags.DataValidity();
                    }
                case EPropertyName.Property_name:
                    {
                        return _VirtualScreen && _Property_name.DataValidity();
                    }
                case EPropertyName.Property_screenSize:
                    {
                        return _VirtualScreen && _Property_screenSize.DataValidity();
                    }
                case EPropertyName.Property_tag:
                    {
                        return _VirtualScreen && _Property_tag.DataValidity();
                    }
                case EPropertyName.Property_useGUILayout:
                    {
                        return _VirtualScreen && _Property_useGUILayout.DataValidity();
                    }
                case EPropertyName.Method_BroadcastMessage_String:
                    {
                        return _VirtualScreen && _Method_BroadcastMessage_String__methodName.DataValidity();
                    }
                case EPropertyName.Method_BroadcastMessage_String_SendMessageOptions:
                    {
                        if(!_VirtualScreen) return false;
                        if(!_Method_BroadcastMessage_String_SendMessageOptions__methodName.DataValidity()) return false;
                        if(!_Method_BroadcastMessage_String_SendMessageOptions__options.DataValidity()) return false;
                        break;
                    }
                case EPropertyName.Method_CallScreenChanged:
                    {
                        return _VirtualScreen;
                    }
                case EPropertyName.Method_CancelInvoke:
                    {
                        return _VirtualScreen;
                    }
                case EPropertyName.Method_CancelInvoke_String:
                    {
                        return _VirtualScreen && _Method_CancelInvoke_String__methodName.DataValidity();
                    }
                case EPropertyName.Method_CompareTag_String:
                    {
                        return _VirtualScreen && _Method_CompareTag_String__tag.DataValidity();
                    }
                case EPropertyName.Method_CreateScreen_String_Transform:
                    {
                        if(!_VirtualScreen) return false;
                        if(!_Method_CreateScreen_String_Transform__name.DataValidity()) return false;
                        if(!_Method_CreateScreen_String_Transform__parent.DataValidity()) return false;
                        break;
                    }
                case EPropertyName.Method_GetAnchorLocalPosition_ERectAnchor:
                    {
                        return _VirtualScreen && _Method_GetAnchorLocalPosition_ERectAnchor__screenAnchor.DataValidity();
                    }
                case EPropertyName.Method_GetAnchorPosition_ERectAnchor_Vector3_ESpaceType:
                    {
                        if(!_VirtualScreen) return false;
                        if(!_Method_GetAnchorPosition_ERectAnchor_Vector3_ESpaceType__screenAnchor.DataValidity()) return false;
                        if(!_Method_GetAnchorPosition_ERectAnchor_Vector3_ESpaceType__anchorOffset.DataValidity()) return false;
                        if(!_Method_GetAnchorPosition_ERectAnchor_Vector3_ESpaceType__spaceType.DataValidity()) return false;
                        break;
                    }
                case EPropertyName.Method_GetComponent_String:
                    {
                        return _VirtualScreen && _Method_GetComponent_String__type.DataValidity();
                    }
                case EPropertyName.Method_GetHashCode:
                    {
                        return _VirtualScreen;
                    }
                case EPropertyName.Method_GetInstanceID:
                    {
                        return _VirtualScreen;
                    }
                case EPropertyName.Method_GetType:
                    {
                        return _VirtualScreen;
                    }
                case EPropertyName.Method_Invoke_String_Single:
                    {
                        if(!_VirtualScreen) return false;
                        if(!_Method_Invoke_String_Single__methodName.DataValidity()) return false;
                        if(!_Method_Invoke_String_Single__time.DataValidity()) return false;
                        break;
                    }
                case EPropertyName.Method_InvokeRepeating_String_Single_Single:
                    {
                        if(!_VirtualScreen) return false;
                        if(!_Method_InvokeRepeating_String_Single_Single__methodName.DataValidity()) return false;
                        if(!_Method_InvokeRepeating_String_Single_Single__time.DataValidity()) return false;
                        if(!_Method_InvokeRepeating_String_Single_Single__repeatRate.DataValidity()) return false;
                        break;
                    }
                case EPropertyName.Method_IsInvoking:
                    {
                        return _VirtualScreen;
                    }
                case EPropertyName.Method_IsInvoking_String:
                    {
                        return _VirtualScreen && _Method_IsInvoking_String__methodName.DataValidity();
                    }
                case EPropertyName.Method_LayoutCamera_Int32_Camera_EScreenLayoutMode:
                    {
                        if(!_VirtualScreen) return false;
                        if(!_Method_LayoutCamera_Int32_Camera_EScreenLayoutMode__index.DataValidity()) return false;
                        if(!_Method_LayoutCamera_Int32_Camera_EScreenLayoutMode__camera.DataValidity()) return false;
                        if(!_Method_LayoutCamera_Int32_Camera_EScreenLayoutMode__screenLayoutMode.DataValidity()) return false;
                        break;
                    }
                case EPropertyName.Method_Reset:
                    {
                        return _VirtualScreen;
                    }
                case EPropertyName.Method_SendMessage_String:
                    {
                        return _VirtualScreen && _Method_SendMessage_String__methodName.DataValidity();
                    }
                case EPropertyName.Method_SendMessage_String_SendMessageOptions:
                    {
                        if(!_VirtualScreen) return false;
                        if(!_Method_SendMessage_String_SendMessageOptions__methodName.DataValidity()) return false;
                        if(!_Method_SendMessage_String_SendMessageOptions__options.DataValidity()) return false;
                        break;
                    }
                case EPropertyName.Method_SendMessageUpwards_String:
                    {
                        return _VirtualScreen && _Method_SendMessageUpwards_String__methodName.DataValidity();
                    }
                case EPropertyName.Method_SendMessageUpwards_String_SendMessageOptions:
                    {
                        if(!_VirtualScreen) return false;
                        if(!_Method_SendMessageUpwards_String_SendMessageOptions__methodName.DataValidity()) return false;
                        if(!_Method_SendMessageUpwards_String_SendMessageOptions__options.DataValidity()) return false;
                        break;
                    }
                case EPropertyName.Method_StartCoroutine_String:
                    {
                        return _VirtualScreen && _Method_StartCoroutine_String__methodName.DataValidity();
                    }
                case EPropertyName.Method_StopAllCoroutines:
                    {
                        return _VirtualScreen;
                    }
                case EPropertyName.Method_StopCoroutine_String:
                    {
                        return _VirtualScreen && _Method_StopCoroutine_String__methodName.DataValidity();
                    }
                case EPropertyName.Method_ToString:
                    {
                        return _VirtualScreen;
                    }
            }
            return base.DataValidity();
        }
        
        /// <summary>
        /// 重置
        /// </summary>
        /// <returns></returns>
        public override void Reset()
        {
            base.Reset();
            if (VirtualScreen) { }
        }
        
    }
}
