using static XCSJ.PluginStereoView.Tools.CameraProjection;
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
    /// 相机透视属性设置: 相机透视属性设置
    /// </summary>
    [ComponentMenu("立体显示/" + Title, typeof(StereoViewManager))]
    [Name(Title, nameof(CameraProjectionPropertySet))]
    [Tip("相机透视属性设置", "Camera perspective property settings")]
    [XCSJ.Attributes.Icon(EIcon.Property)]
    [Owner(typeof(StereoViewManager))]
    public class CameraProjectionPropertySet : BasePropertySet<CameraProjectionPropertySet>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "相机透视属性设置";
        
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib("立体显示", typeof(StereoViewManager))]
        [StateComponentMenu("立体显示/" + Title, typeof(StereoViewManager))]
        [Name(Title, nameof(CameraProjectionPropertySet))]
        [Tip("相机透视属性设置", "Camera perspective property settings")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);
        
        /// <summary>
        /// 相机透视:根据屏幕与相机位置实时更新相机透视矩阵的工具组件
        /// </summary>
        [Name("相机透视")]
        [Tip("根据屏幕与相机位置实时更新相机透视矩阵的工具组件", "Tool component for updating camera perspective matrix in real time according to screen and camera position")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ComponentPopup]
        public CameraProjection _CameraProjection;
        
        /// <summary>
        /// 相机透视:根据屏幕与相机位置实时更新相机透视矩阵的工具组件
        /// </summary>
        public CameraProjection CameraProjection => this.XGetComponentInGlobal(ref _CameraProjection, true);
        
        /// <summary>
        /// 相机透视列表:根据屏幕与相机位置实时更新相机透视矩阵的工具组件列表
        /// </summary>
        [Name("相机透视列表")]
        [Tip("根据屏幕与相机位置实时更新相机透视矩阵的工具组件列表", "Update the tool component list of the camera perspective matrix in real time according to the screen and camera position")]
        public List<CameraProjection> _CameraProjections = new List<CameraProjection>();
        
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
            /// 预估视锥(字段):使相机总是朝向屏幕中心，并且根据相机位置与屏幕纵横比动态调整相机的FOV；
            /// </summary>
            [Name("预估视锥(字段)")]
            [Tip("使相机总是朝向屏幕中心，并且根据相机位置与屏幕纵横比动态调整相机的FOV；", "Make the camera always face the center of the screen, and dynamically adjust the FOV of the camera according to the camera position and the aspect ratio of the screen;")]
            [EnumFieldName("字段/预估视锥")]
            Field__estimateViewFrustum = 1,
            
            /// <summary>
            /// 屏幕(字段):屏幕
            /// </summary>
            [Name("屏幕(字段)")]
            [Tip("屏幕", "screen")]
            [EnumFieldName("字段/屏幕")]
            Field__screen,
            
            /// <summary>
            /// 禁用时设置矩阵规则(字段):禁用时设置矩阵规则
            /// </summary>
            [Name("禁用时设置矩阵规则(字段)")]
            [Tip("禁用时设置矩阵规则", "Set matrix rules when disabled")]
            [EnumFieldName("字段/禁用时设置矩阵规则")]
            Field__setMatrixRuleOnDisable,
            
            #endregion
            
            #region 属性
            
            /// <summary>
            /// 启用(属性):启用
            /// </summary>
            [Name("启用(属性)")]
            [Tip("启用", "Enabled")]
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
            /// 屏幕(属性):屏幕
            /// </summary>
            [Name("屏幕(属性)")]
            [Tip("屏幕")]
            [EnumFieldName("属性/屏幕")]
            Property_screen,
            
            /// <summary>
            /// 屏幕尺寸(属性):屏幕尺寸
            /// </summary>
            [Name("屏幕尺寸(属性)")]
            [Tip("屏幕尺寸")]
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
            /// 从中复制数据(方法):从中复制数据
            /// </summary>
            [Name("从中复制数据(方法)")]
            [Tip("从中复制数据")]
            [EnumFieldName("方法/从中复制数据")]
            Method_CopyDataFrom_CameraProjection,
            
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
        /// 预估视锥:使相机总是朝向屏幕中心，并且根据相机位置与屏幕纵横比动态调整相机的FOV；
        /// </summary>
        [Name("预估视锥")]
        [Tip("使相机总是朝向屏幕中心，并且根据相机位置与屏幕纵横比动态调整相机的FOV；", "Make the camera always face the center of the screen, and dynamically adjust the FOV of the camera according to the camera position and the aspect ratio of the screen;")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Field__estimateViewFrustum)]
        [OnlyMemberElements]
        public BoolPropertyValue _Field__estimateViewFrustum = new BoolPropertyValue();
        
        /// <summary>
        /// 屏幕:屏幕
        /// </summary>
        [Name("屏幕")]
        [Tip("屏幕")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Field__screen)]
        [OnlyMemberElements]
        public VirtualScreenPropertyValue _Field__screen = new VirtualScreenPropertyValue();
        
        /// <summary>
        /// E Set Matrix Rule On Disable属性值
        /// </summary>
        [Serializable]
        [Name("禁用时设置矩阵规则属性值")]
        public class ESetMatrixRuleOnDisablePropertyValue : EnumPropertyValue<ESetMatrixRuleOnDisable>
        {
        }
        
        /// <summary>
        /// 禁用时设置矩阵规则:禁用时设置矩阵规则
        /// </summary>
        [Name("禁用时设置矩阵规则")]
        [Tip("禁用时设置矩阵规则")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Field__setMatrixRuleOnDisable)]
        [OnlyMemberElements]
        public ESetMatrixRuleOnDisablePropertyValue _Field__setMatrixRuleOnDisable = new ESetMatrixRuleOnDisablePropertyValue();
        
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
        /// 屏幕:屏幕
        /// </summary>
        [Name("屏幕")]
        [Tip("屏幕")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Property_screen)]
        [OnlyMemberElements]
        public VirtualScreenPropertyValue _Property_screen = new VirtualScreenPropertyValue();
        
        /// <summary>
        /// 屏幕尺寸:屏幕尺寸
        /// </summary>
        [Name("屏幕尺寸")]
        [Tip("屏幕尺寸")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Property_screenSize)]
        [OnlyMemberElements]
        public Vector2PropertyValue _Property_screenSize = new Vector2PropertyValue();
        
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
        /// 相机透视:根据屏幕与相机位置实时更新相机透视矩阵的工具组件
        /// </summary>
        [Name("相机透视")]
        [Tip("根据屏幕与相机位置实时更新相机透视矩阵的工具组件", "Tool component for updating camera perspective matrix in real time according to screen and camera position")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_CopyDataFrom_CameraProjection)]
        [OnlyMemberElements]
        public UnityObjectPropertyValue _Method_CopyDataFrom_CameraProjection__cameraProjection = new UnityObjectPropertyValue();
        
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
                case EPropertyName.Field__estimateViewFrustum:
                    {
                        var value = _Field__estimateViewFrustum.GetValue();
                        if (_CameraProjection != null) _CameraProjection._estimateViewFrustum = value;
                        foreach(var obj in _CameraProjections)
                        {
                            if (obj != null) obj._estimateViewFrustum = value;
                        }
                        break;
                    }
                case EPropertyName.Field__screen:
                    {
                        var value = _Field__screen.GetValue();
                        if (_CameraProjection != null) _CameraProjection._screen = value;
                        foreach(var obj in _CameraProjections)
                        {
                            if (obj != null) obj._screen = value;
                        }
                        break;
                    }
                case EPropertyName.Field__setMatrixRuleOnDisable:
                    {
                        var value = _Field__setMatrixRuleOnDisable.GetValue();
                        if (_CameraProjection != null) _CameraProjection._setMatrixRuleOnDisable = value;
                        foreach(var obj in _CameraProjections)
                        {
                            if (obj != null) obj._setMatrixRuleOnDisable = value;
                        }
                        break;
                    }
                case EPropertyName.Property_enabled:
                    {
                        var value = _Property_enabled.GetValue();
                        if (_CameraProjection != null) _CameraProjection.enabled = value;
                        foreach(var obj in _CameraProjections)
                        {
                            if (obj != null) obj.enabled = value;
                        }
                        break;
                    }
                case EPropertyName.Property_hideFlags:
                    {
                        var value = _Property_hideFlags.GetValue();
                        if (_CameraProjection != null) _CameraProjection.hideFlags = value;
                        foreach(var obj in _CameraProjections)
                        {
                            if (obj != null) obj.hideFlags = value;
                        }
                        break;
                    }
                case EPropertyName.Property_name:
                    {
                        var value = _Property_name.GetValue();
                        if (_CameraProjection != null) _CameraProjection.name = value;
                        foreach(var obj in _CameraProjections)
                        {
                            if (obj != null) obj.name = value;
                        }
                        break;
                    }
                case EPropertyName.Property_screen:
                    {
                        var value = _Property_screen.GetValue();
                        if (_CameraProjection != null) _CameraProjection.screen = value;
                        foreach(var obj in _CameraProjections)
                        {
                            if (obj != null) obj.screen = value;
                        }
                        break;
                    }
                case EPropertyName.Property_screenSize:
                    {
                        var value = _Property_screenSize.GetValue();
                        if (_CameraProjection != null) _CameraProjection.screenSize = value;
                        foreach(var obj in _CameraProjections)
                        {
                            if (obj != null) obj.screenSize = value;
                        }
                        break;
                    }
                case EPropertyName.Property_tag:
                    {
                        var value = _Property_tag.GetValue();
                        if (_CameraProjection != null) _CameraProjection.tag = value;
                        foreach(var obj in _CameraProjections)
                        {
                            if (obj != null) obj.tag = value;
                        }
                        break;
                    }
                case EPropertyName.Property_useGUILayout:
                    {
                        var value = _Property_useGUILayout.GetValue();
                        if (_CameraProjection != null) _CameraProjection.useGUILayout = value;
                        foreach(var obj in _CameraProjections)
                        {
                            if (obj != null) obj.useGUILayout = value;
                        }
                        break;
                    }
                case EPropertyName.Method_BroadcastMessage_String:
                    {
                        var methodName = _Method_BroadcastMessage_String__methodName.GetValue();
                        if (_CameraProjection != null) _CameraProjection.BroadcastMessage(methodName);
                        foreach(var obj in _CameraProjections)
                        {
                            if (obj != null) obj.BroadcastMessage(methodName);
                        }
                        break;
                    }
                case EPropertyName.Method_BroadcastMessage_String_SendMessageOptions:
                    {
                        var methodName = _Method_BroadcastMessage_String_SendMessageOptions__methodName.GetValue();
                        var options = _Method_BroadcastMessage_String_SendMessageOptions__options.GetValue();
                        if (_CameraProjection != null) _CameraProjection.BroadcastMessage(methodName, options);
                        foreach(var obj in _CameraProjections)
                        {
                            if (obj != null) obj.BroadcastMessage(methodName, options);
                        }
                        break;
                    }
                case EPropertyName.Method_CancelInvoke:
                    {
                        if (_CameraProjection != null) _CameraProjection.CancelInvoke();
                        foreach(var obj in _CameraProjections)
                        {
                            if (obj != null) obj.CancelInvoke();
                        }
                        break;
                    }
                case EPropertyName.Method_CancelInvoke_String:
                    {
                        var methodName = _Method_CancelInvoke_String__methodName.GetValue();
                        if (_CameraProjection != null) _CameraProjection.CancelInvoke(methodName);
                        foreach(var obj in _CameraProjections)
                        {
                            if (obj != null) obj.CancelInvoke(methodName);
                        }
                        break;
                    }
                case EPropertyName.Method_CompareTag_String:
                    {
                        var tag = _Method_CompareTag_String__tag.GetValue();
                        if (_CameraProjection != null) _CameraProjection.CompareTag(tag);
                        foreach(var obj in _CameraProjections)
                        {
                            if (obj != null) obj.CompareTag(tag);
                        }
                        break;
                    }
                case EPropertyName.Method_CopyDataFrom_CameraProjection:
                    {
                        var cameraProjection = _Method_CopyDataFrom_CameraProjection__cameraProjection.GetValue() as CameraProjection;
                        if (_CameraProjection != null) _CameraProjection.CopyDataFrom(cameraProjection);
                        foreach(var obj in _CameraProjections)
                        {
                            if (obj != null) obj.CopyDataFrom(cameraProjection);
                        }
                        break;
                    }
                case EPropertyName.Method_GetComponent_String:
                    {
                        var type = _Method_GetComponent_String__type.GetValue();
                        if (_CameraProjection != null) _CameraProjection.GetComponent(type);
                        foreach(var obj in _CameraProjections)
                        {
                            if (obj != null) obj.GetComponent(type);
                        }
                        break;
                    }
                case EPropertyName.Method_GetHashCode:
                    {
                        if (_CameraProjection != null) _CameraProjection.GetHashCode();
                        foreach(var obj in _CameraProjections)
                        {
                            if (obj != null) obj.GetHashCode();
                        }
                        break;
                    }
                case EPropertyName.Method_GetInstanceID:
                    {
                        if (_CameraProjection != null) _CameraProjection.GetInstanceID();
                        foreach(var obj in _CameraProjections)
                        {
                            if (obj != null) obj.GetInstanceID();
                        }
                        break;
                    }
                case EPropertyName.Method_GetType:
                    {
                        if (_CameraProjection != null) _CameraProjection.GetType();
                        foreach(var obj in _CameraProjections)
                        {
                            if (obj != null) obj.GetType();
                        }
                        break;
                    }
                case EPropertyName.Method_Invoke_String_Single:
                    {
                        var methodName = _Method_Invoke_String_Single__methodName.GetValue();
                        var time = _Method_Invoke_String_Single__time.GetValue();
                        if (_CameraProjection != null) _CameraProjection.Invoke(methodName, time);
                        foreach(var obj in _CameraProjections)
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
                        if (_CameraProjection != null) _CameraProjection.InvokeRepeating(methodName, time, repeatRate);
                        foreach(var obj in _CameraProjections)
                        {
                            if (obj != null) obj.InvokeRepeating(methodName, time, repeatRate);
                        }
                        break;
                    }
                case EPropertyName.Method_IsInvoking:
                    {
                        if (_CameraProjection != null) _CameraProjection.IsInvoking();
                        foreach(var obj in _CameraProjections)
                        {
                            if (obj != null) obj.IsInvoking();
                        }
                        break;
                    }
                case EPropertyName.Method_IsInvoking_String:
                    {
                        var methodName = _Method_IsInvoking_String__methodName.GetValue();
                        if (_CameraProjection != null) _CameraProjection.IsInvoking(methodName);
                        foreach(var obj in _CameraProjections)
                        {
                            if (obj != null) obj.IsInvoking(methodName);
                        }
                        break;
                    }
                case EPropertyName.Method_SendMessage_String:
                    {
                        var methodName = _Method_SendMessage_String__methodName.GetValue();
                        if (_CameraProjection != null) _CameraProjection.SendMessage(methodName);
                        foreach(var obj in _CameraProjections)
                        {
                            if (obj != null) obj.SendMessage(methodName);
                        }
                        break;
                    }
                case EPropertyName.Method_SendMessage_String_SendMessageOptions:
                    {
                        var methodName = _Method_SendMessage_String_SendMessageOptions__methodName.GetValue();
                        var options = _Method_SendMessage_String_SendMessageOptions__options.GetValue();
                        if (_CameraProjection != null) _CameraProjection.SendMessage(methodName, options);
                        foreach(var obj in _CameraProjections)
                        {
                            if (obj != null) obj.SendMessage(methodName, options);
                        }
                        break;
                    }
                case EPropertyName.Method_SendMessageUpwards_String:
                    {
                        var methodName = _Method_SendMessageUpwards_String__methodName.GetValue();
                        if (_CameraProjection != null) _CameraProjection.SendMessageUpwards(methodName);
                        foreach(var obj in _CameraProjections)
                        {
                            if (obj != null) obj.SendMessageUpwards(methodName);
                        }
                        break;
                    }
                case EPropertyName.Method_SendMessageUpwards_String_SendMessageOptions:
                    {
                        var methodName = _Method_SendMessageUpwards_String_SendMessageOptions__methodName.GetValue();
                        var options = _Method_SendMessageUpwards_String_SendMessageOptions__options.GetValue();
                        if (_CameraProjection != null) _CameraProjection.SendMessageUpwards(methodName, options);
                        foreach(var obj in _CameraProjections)
                        {
                            if (obj != null) obj.SendMessageUpwards(methodName, options);
                        }
                        break;
                    }
                case EPropertyName.Method_StartCoroutine_String:
                    {
                        var methodName = _Method_StartCoroutine_String__methodName.GetValue();
                        if (_CameraProjection != null) _CameraProjection.StartCoroutine(methodName);
                        foreach(var obj in _CameraProjections)
                        {
                            if (obj != null) obj.StartCoroutine(methodName);
                        }
                        break;
                    }
                case EPropertyName.Method_StopAllCoroutines:
                    {
                        if (_CameraProjection != null) _CameraProjection.StopAllCoroutines();
                        foreach(var obj in _CameraProjections)
                        {
                            if (obj != null) obj.StopAllCoroutines();
                        }
                        break;
                    }
                case EPropertyName.Method_StopCoroutine_String:
                    {
                        var methodName = _Method_StopCoroutine_String__methodName.GetValue();
                        if (_CameraProjection != null) _CameraProjection.StopCoroutine(methodName);
                        foreach(var obj in _CameraProjections)
                        {
                            if (obj != null) obj.StopCoroutine(methodName);
                        }
                        break;
                    }
                case EPropertyName.Method_ToString:
                    {
                        if (_CameraProjection != null) _CameraProjection.ToString();
                        foreach(var obj in _CameraProjections)
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
                case EPropertyName.Field__estimateViewFrustum:
                    {
                        return CommonFun.Name(_propertyName) + " = " + _Field__estimateViewFrustum.ToFriendlyString();
                    }
                case EPropertyName.Field__screen:
                    {
                        return CommonFun.Name(_propertyName) + " = " + _Field__screen.ToFriendlyString();
                    }
                case EPropertyName.Field__setMatrixRuleOnDisable:
                    {
                        return CommonFun.Name(_propertyName) + " = " + _Field__setMatrixRuleOnDisable.ToFriendlyString();
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
                case EPropertyName.Property_screen:
                    {
                        return CommonFun.Name(_propertyName) + " = " + _Property_screen.ToFriendlyString();
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
                case EPropertyName.Method_CopyDataFrom_CameraProjection:
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
                case EPropertyName.Field__estimateViewFrustum:
                    {
                        return _CameraProjection && _Field__estimateViewFrustum.DataValidity();
                    }
                case EPropertyName.Field__screen:
                    {
                        return _CameraProjection && _Field__screen.DataValidity();
                    }
                case EPropertyName.Field__setMatrixRuleOnDisable:
                    {
                        return _CameraProjection && _Field__setMatrixRuleOnDisable.DataValidity();
                    }
                case EPropertyName.Property_enabled:
                    {
                        return _CameraProjection && _Property_enabled.DataValidity();
                    }
                case EPropertyName.Property_hideFlags:
                    {
                        return _CameraProjection && _Property_hideFlags.DataValidity();
                    }
                case EPropertyName.Property_name:
                    {
                        return _CameraProjection && _Property_name.DataValidity();
                    }
                case EPropertyName.Property_screen:
                    {
                        return _CameraProjection && _Property_screen.DataValidity();
                    }
                case EPropertyName.Property_screenSize:
                    {
                        return _CameraProjection && _Property_screenSize.DataValidity();
                    }
                case EPropertyName.Property_tag:
                    {
                        return _CameraProjection && _Property_tag.DataValidity();
                    }
                case EPropertyName.Property_useGUILayout:
                    {
                        return _CameraProjection && _Property_useGUILayout.DataValidity();
                    }
                case EPropertyName.Method_BroadcastMessage_String:
                    {
                        return _CameraProjection && _Method_BroadcastMessage_String__methodName.DataValidity();
                    }
                case EPropertyName.Method_BroadcastMessage_String_SendMessageOptions:
                    {
                        if(!_CameraProjection) return false;
                        if(!_Method_BroadcastMessage_String_SendMessageOptions__methodName.DataValidity()) return false;
                        if(!_Method_BroadcastMessage_String_SendMessageOptions__options.DataValidity()) return false;
                        break;
                    }
                case EPropertyName.Method_CancelInvoke:
                    {
                        return _CameraProjection;
                    }
                case EPropertyName.Method_CancelInvoke_String:
                    {
                        return _CameraProjection && _Method_CancelInvoke_String__methodName.DataValidity();
                    }
                case EPropertyName.Method_CompareTag_String:
                    {
                        return _CameraProjection && _Method_CompareTag_String__tag.DataValidity();
                    }
                case EPropertyName.Method_CopyDataFrom_CameraProjection:
                    {
                        return _CameraProjection && _Method_CopyDataFrom_CameraProjection__cameraProjection.DataValidity();
                    }
                case EPropertyName.Method_GetComponent_String:
                    {
                        return _CameraProjection && _Method_GetComponent_String__type.DataValidity();
                    }
                case EPropertyName.Method_GetHashCode:
                    {
                        return _CameraProjection;
                    }
                case EPropertyName.Method_GetInstanceID:
                    {
                        return _CameraProjection;
                    }
                case EPropertyName.Method_GetType:
                    {
                        return _CameraProjection;
                    }
                case EPropertyName.Method_Invoke_String_Single:
                    {
                        if(!_CameraProjection) return false;
                        if(!_Method_Invoke_String_Single__methodName.DataValidity()) return false;
                        if(!_Method_Invoke_String_Single__time.DataValidity()) return false;
                        break;
                    }
                case EPropertyName.Method_InvokeRepeating_String_Single_Single:
                    {
                        if(!_CameraProjection) return false;
                        if(!_Method_InvokeRepeating_String_Single_Single__methodName.DataValidity()) return false;
                        if(!_Method_InvokeRepeating_String_Single_Single__time.DataValidity()) return false;
                        if(!_Method_InvokeRepeating_String_Single_Single__repeatRate.DataValidity()) return false;
                        break;
                    }
                case EPropertyName.Method_IsInvoking:
                    {
                        return _CameraProjection;
                    }
                case EPropertyName.Method_IsInvoking_String:
                    {
                        return _CameraProjection && _Method_IsInvoking_String__methodName.DataValidity();
                    }
                case EPropertyName.Method_SendMessage_String:
                    {
                        return _CameraProjection && _Method_SendMessage_String__methodName.DataValidity();
                    }
                case EPropertyName.Method_SendMessage_String_SendMessageOptions:
                    {
                        if(!_CameraProjection) return false;
                        if(!_Method_SendMessage_String_SendMessageOptions__methodName.DataValidity()) return false;
                        if(!_Method_SendMessage_String_SendMessageOptions__options.DataValidity()) return false;
                        break;
                    }
                case EPropertyName.Method_SendMessageUpwards_String:
                    {
                        return _CameraProjection && _Method_SendMessageUpwards_String__methodName.DataValidity();
                    }
                case EPropertyName.Method_SendMessageUpwards_String_SendMessageOptions:
                    {
                        if(!_CameraProjection) return false;
                        if(!_Method_SendMessageUpwards_String_SendMessageOptions__methodName.DataValidity()) return false;
                        if(!_Method_SendMessageUpwards_String_SendMessageOptions__options.DataValidity()) return false;
                        break;
                    }
                case EPropertyName.Method_StartCoroutine_String:
                    {
                        return _CameraProjection && _Method_StartCoroutine_String__methodName.DataValidity();
                    }
                case EPropertyName.Method_StopAllCoroutines:
                    {
                        return _CameraProjection;
                    }
                case EPropertyName.Method_StopCoroutine_String:
                    {
                        return _CameraProjection && _Method_StopCoroutine_String__methodName.DataValidity();
                    }
                case EPropertyName.Method_ToString:
                    {
                        return _CameraProjection;
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
            if (CameraProjection) { }
        }
        
    }
}
