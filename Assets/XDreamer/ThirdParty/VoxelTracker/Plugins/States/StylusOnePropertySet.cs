using System.Collections.Generic;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Attributes;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Base;

#if XDREAMER_VOXELTRACKER
using VoxelStationUtil;
#endif

namespace XCSJ.PluginVoxelTracker.States
{
    /// <summary>
    /// 交互笔属性设置: 交互笔属性设置
    /// </summary>
    [ComponentMenu("Voxel Tracker/" + Title, typeof(VoxelTrackerManager))]
    [Name(Title, nameof(StylusOnePropertySet))]
    [Tip("交互笔属性设置")]
    [XCSJ.Attributes.Icon(EIcon.Property)]
    [Owner(typeof(VoxelTrackerManager))]
    public class StylusOnePropertySet : BasePropertySet<StylusOnePropertySet>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "交互笔属性设置";
        
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib("Voxel Tracker", typeof(VoxelTrackerManager))]
        [StateComponentMenu("Voxel Tracker/" + Title, typeof(VoxelTrackerManager))]
        [Name(Title, nameof(StylusOnePropertySet))]
        [Tip("交互笔属性设置")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

#if XDREAMER_VOXELTRACKER

        /// <summary>
        /// 交互笔:交互笔
        /// </summary>
        [Name("交互笔")]
        [Tip("交互笔")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ComponentPopup]
        public StylusOne _StylusOne;
        
        /// <summary>
        /// 交互笔:交互笔
        /// </summary>
        public StylusOne StylusOne => this.XGetComponentInGlobal(ref _StylusOne, true);
        
        /// <summary>
        /// 交互笔列表:交互笔列表
        /// </summary>
        [Name("交互笔列表")]
        [Tip("交互笔列表")]
        public List<StylusOne> _StylusOnes = new List<StylusOne>();

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
            [Tip("无")]
            [EnumFieldName("无")]
            None,
            
            #region 字段
            
            /// <summary>
            /// 按钮数量:按钮数量
            /// </summary>
            [Name("按钮数量")]
            [Tip("按钮数量")]
            [EnumFieldName("字段/按钮数量")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Field_buttonCount = 1,
            
            /// <summary>
            /// 按钮ID:按钮ID
            /// </summary>
            [Name("按钮ID")]
            [Tip("按钮ID")]
            [EnumFieldName("字段/按钮ID")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Field_buttonID,
            
            /// <summary>
            /// 点击时间阈值:点击时间阈值
            /// </summary>
            [Name("点击时间阈值")]
            [Tip("点击时间阈值")]
            [EnumFieldName("字段/点击时间阈值")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Field_ClickTimeThreshold,
            
            /// <summary>
            /// 默认拖动策略:默认拖动策略
            /// </summary>
            [Name("默认拖动策略")]
            [Tip("默认拖动策略")]
            [EnumFieldName("字段/默认拖动策略")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Field_DefaultDragPolicy,
            
            /// <summary>
            /// 头部变换:头部变换
            /// </summary>
            [Name("头部变换")]
            [Tip("头部变换")]
            [EnumFieldName("字段/头部变换")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Field_headTrans,
            
            /// <summary>
            /// 击中:击中
            /// </summary>
            [Name("击中")]
            [Tip("击中")]
            [EnumFieldName("字段/击中")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Field_hit,
            
            /// <summary>
            /// 悬停对象默认启用:悬停对象默认启用
            /// </summary>
            [Name("悬停对象默认启用")]
            [Tip("悬停对象默认启用")]
            [EnumFieldName("字段/悬停对象默认启用")]
            Field_hoverObjectDefaultEnable,
            
            /// <summary>
            /// 实例:实例
            /// </summary>
            [Name("实例")]
            [Tip("实例")]
            [EnumFieldName("字段/实例")]
            Field_Instance,
            
            /// <summary>
            /// 对象拖动策略:对象拖动策略
            /// </summary>
            [Name("对象拖动策略")]
            [Tip("对象拖动策略")]
            [EnumFieldName("字段/对象拖动策略")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Field_ObjectDragPolicy,
            
            /// <summary>
            /// 射线状态:射线状态
            /// </summary>
            [Name("射线状态")]
            [Tip("射线状态")]
            [EnumFieldName("字段/射线状态")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Field_rayState,
            
            /// <summary>
            /// 每单位滚动米数:每单位滚动米数
            /// </summary>
            [Name("每单位滚动米数")]
            [Tip("每单位滚动米数")]
            [EnumFieldName("字段/每单位滚动米数")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Field_ScrollMetersPerUnit,
            
            /// <summary>
            /// 交互笔长度:交互笔长度
            /// </summary>
            [Name("交互笔长度")]
            [Tip("交互笔长度")]
            [EnumFieldName("字段/交互笔长度")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Field_stylusLength,
            
            /// <summary>
            /// 交互笔长度锁定:交互笔长度锁定
            /// </summary>
            [Name("交互笔长度锁定")]
            [Tip("交互笔长度锁定")]
            [EnumFieldName("字段/交互笔长度锁定")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Field_stylusLengthLock,
            
            /// <summary>
            /// UI拖动策略:UI拖动策略
            /// </summary>
            [Name("UI拖动策略")]
            [Tip("UI拖动策略")]
            [EnumFieldName("字段/UI拖动策略")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Field_UIDragPolicy,
            
            /// <summary>
            /// 事件相机:事件相机
            /// </summary>
            [Name("事件相机")]
            [Tip("事件相机")]
            [EnumFieldName("字段/事件相机")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Field_VEventCamera,
            
            #endregion
            
            #region 属性
            
            /// <summary>
            /// 默认击中距离:默认击中距离
            /// </summary>
            [Name("默认击中距离")]
            [Tip("默认击中距离")]
            [EnumFieldName("属性/默认击中距离")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Property_DefaultHitDistance = 1000,
            
            /// <summary>
            /// 启用:启用
            /// </summary>
            [Name("启用")]
            [Tip("启用")]
            [EnumFieldName("属性/启用")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Property_enabled,
            
            /// <summary>
            /// 隐藏标识:隐藏标识
            /// </summary>
            [Name("隐藏标识")]
            [Tip("隐藏标识")]
            [EnumFieldName("属性/隐藏标识")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Property_hideFlags,
            
            /// <summary>
            /// 名称:名称
            /// </summary>
            [Name("名称")]
            [Tip("名称")]
            [EnumFieldName("属性/名称")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Property_name,
            
            /// <summary>
            /// 标签:标签
            /// </summary>
            [Name("标签")]
            [Tip("标签")]
            [EnumFieldName("属性/标签")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Property_tag,
            
            /// <summary>
            /// 使用GUI布局:使用GUI布局
            /// </summary>
            [Name("使用GUI布局")]
            [Tip("使用GUI布局")]
            [EnumFieldName("属性/使用GUI布局")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Property_useGUILayout,
            
            #endregion
            
            #region 方法
            
            /// <summary>
            /// 广播消息(方法名):广播消息(方法名)
            /// </summary>
            [Name("广播消息(方法名)")]
            [Tip("广播消息(方法名)")]
            [EnumFieldName("方法/广播消息(方法名)")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_BroadcastMessage_String = 10000,
            
            /// <summary>
            /// 广播消息(方法名+发送消息选项):广播消息(方法名+发送消息选项)
            /// </summary>
            [Name("广播消息(方法名+发送消息选项)")]
            [Tip("广播消息(方法名+发送消息选项)")]
            [EnumFieldName("方法/广播消息(方法名+发送消息选项)")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_BroadcastMessage_String_SendMessageOptions,
            
            /// <summary>
            /// 调用交互笔下:调用交互笔下
            /// </summary>
            [Name("调用交互笔下")]
            [Tip("调用交互笔下")]
            [EnumFieldName("方法/调用交互笔下")]
            Method_CallStylusDown_ButtonID,
            
            /// <summary>
            /// 调用交互笔按压:调用交互笔按压
            /// </summary>
            [Name("调用交互笔按压")]
            [Tip("调用交互笔按压")]
            [EnumFieldName("方法/调用交互笔按压")]
            Method_CallStylusPressed_ButtonID,
            
            /// <summary>
            /// 调用交互笔释放:调用交互笔释放
            /// </summary>
            [Name("调用交互笔释放")]
            [Tip("调用交互笔释放")]
            [EnumFieldName("方法/调用交互笔释放")]
            Method_CallStylusReleased_ButtonID,
            
            /// <summary>
            /// 取消调用:取消调用
            /// </summary>
            [Name("取消调用")]
            [Tip("取消调用")]
            [EnumFieldName("方法/取消调用")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_CancelInvoke,
            
            /// <summary>
            /// 取消调用(方法名):取消调用(方法名)
            /// </summary>
            [Name("取消调用(方法名)")]
            [Tip("取消调用(方法名)")]
            [EnumFieldName("方法/取消调用(方法名)")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_CancelInvoke_String,
            
            /// <summary>
            /// 捕获指针:捕获指针
            /// </summary>
            [Name("捕获指针")]
            [Tip("捕获指针")]
            [EnumFieldName("方法/捕获指针")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_CapturePointer_GameObject,
            
            /// <summary>
            /// 从V指针投射光束:从V指针投射光束
            /// </summary>
            [Name("从V指针投射光束")]
            [Tip("从V指针投射光束")]
            [EnumFieldName("方法/从V指针投射光束")]
            Method_CastBeamFromVPointer,
            
            /// <summary>
            /// 比较标签:比较标签
            /// </summary>
            [Name("比较标签")]
            [Tip("比较标签")]
            [EnumFieldName("方法/比较标签")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_CompareTag_String,
            
            /// <summary>
            /// 获取按钮:获取按钮
            /// </summary>
            [Name("获取按钮")]
            [Tip("获取按钮")]
            [EnumFieldName("方法/获取按钮")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_GetButton_Int32,
            
            /// <summary>
            /// 获取按钮按下:获取按钮按下
            /// </summary>
            [Name("获取按钮按下")]
            [Tip("获取按钮按下")]
            [EnumFieldName("方法/获取按钮按下")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_GetButtonDown_Int32,
            
            /// <summary>
            /// 获取按钮隐射:获取按钮隐射
            /// </summary>
            [Name("获取按钮隐射")]
            [Tip("获取按钮隐射")]
            [EnumFieldName("方法/获取按钮隐射")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_GetButtonMapping_Int32,
            
            /// <summary>
            /// 获取按钮抬起:获取按钮抬起
            /// </summary>
            [Name("获取按钮抬起")]
            [Tip("获取按钮抬起")]
            [EnumFieldName("方法/获取按钮抬起")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_GetButtonUp_Int32,
            
            /// <summary>
            /// 获取组件:获取组件
            /// </summary>
            [Name("获取组件")]
            [Tip("获取组件")]
            [EnumFieldName("方法/获取组件")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_GetComponent_String,
            
            /// <summary>
            /// 获取哈希码:获取哈希码
            /// </summary>
            [Name("获取哈希码")]
            [Tip("获取哈希码")]
            [EnumFieldName("方法/获取哈希码")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_GetHashCode,
            
            /// <summary>
            /// 获取实例ID:获取实例ID
            /// </summary>
            [Name("获取实例ID")]
            [Tip("获取实例ID")]
            [EnumFieldName("方法/获取实例ID")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_GetInstanceID,
            
            /// <summary>
            /// 获取类型:获取类型
            /// </summary>
            [Name("获取类型")]
            [Tip("获取类型")]
            [EnumFieldName("方法/获取类型")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_GetType,
            
            /// <summary>
            /// 调用:调用
            /// </summary>
            [Name("调用")]
            [Tip("调用")]
            [EnumFieldName("方法/调用")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_Invoke_String_Single,
            
            /// <summary>
            /// 重复调用:重复调用
            /// </summary>
            [Name("重复调用")]
            [Tip("重复调用")]
            [EnumFieldName("方法/重复调用")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_InvokeRepeating_String_Single_Single,
            
            /// <summary>
            /// 是否调用中:是否调用中
            /// </summary>
            [Name("是否调用中")]
            [Tip("是否调用中")]
            [EnumFieldName("方法/是否调用中")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_IsInvoking,
            
            /// <summary>
            /// 是否调用中(字符串):是否调用中(字符串)
            /// </summary>
            [Name("是否调用中(字符串)")]
            [Tip("是否调用中(字符串)")]
            [EnumFieldName("方法/是否调用中(字符串)")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_IsInvoking_String,
            
            /// <summary>
            /// On Disable:On Disable
            /// </summary>
            [Name("On Disable")]
            [Tip("On Disable")]
            [EnumFieldName("方法/On Disable")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_OnDisable,
            
            /// <summary>
            /// On Enable:On Enable
            /// </summary>
            [Name("On Enable")]
            [Tip("On Enable")]
            [EnumFieldName("方法/On Enable")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_OnEnable,
            
            /// <summary>
            /// 发送消息(方法名):发送消息(方法名)
            /// </summary>
            [Name("发送消息(方法名)")]
            [Tip("发送消息(方法名)")]
            [EnumFieldName("方法/发送消息(方法名)")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_SendMessage_String,
            
            /// <summary>
            /// 发送消息(方法名+发送消息选项):发送消息(方法名+发送消息选项)
            /// </summary>
            [Name("发送消息(方法名+发送消息选项)")]
            [Tip("发送消息(方法名+发送消息选项)")]
            [EnumFieldName("方法/发送消息(方法名+发送消息选项)")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_SendMessage_String_SendMessageOptions,
            
            /// <summary>
            /// 向上发送消息(方法名):向上发送消息(方法名)
            /// </summary>
            [Name("向上发送消息(方法名)")]
            [Tip("向上发送消息(方法名)")]
            [EnumFieldName("方法/向上发送消息(方法名)")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_SendMessageUpwards_String,
            
            /// <summary>
            /// 向上发送消息(方法名+发送消息选项):向上发送消息(方法名+发送消息选项)
            /// </summary>
            [Name("向上发送消息(方法名+发送消息选项)")]
            [Tip("向上发送消息(方法名+发送消息选项)")]
            [EnumFieldName("方法/向上发送消息(方法名+发送消息选项)")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_SendMessageUpwards_String_SendMessageOptions,
            
            /// <summary>
            /// 启动协程:启动协程
            /// </summary>
            [Name("启动协程")]
            [Tip("启动协程")]
            [EnumFieldName("方法/启动协程")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_StartCoroutine_String,
            
            /// <summary>
            /// 停止所有协程:停止所有协程
            /// </summary>
            [Name("停止所有协程")]
            [Tip("停止所有协程")]
            [EnumFieldName("方法/停止所有协程")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_StopAllCoroutines,
            
            /// <summary>
            /// 停止协程:停止协程
            /// </summary>
            [Name("停止协程")]
            [Tip("停止协程")]
            [EnumFieldName("方法/停止协程")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_StopCoroutine_String,
            
            /// <summary>
            /// 转字符串:转字符串
            /// </summary>
            [Name("转字符串")]
            [Tip("转字符串")]
            [EnumFieldName("方法/转字符串")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_ToString,
            
            /// <summary>
            /// 更新按钮:更新按钮
            /// </summary>
            [Name("更新按钮")]
            [Tip("更新按钮")]
            [EnumFieldName("方法/更新按钮")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_UpdateButton_Int32,
            
            /// <summary>
            /// 更新交互笔线:更新交互笔线
            /// </summary>
            [Name("更新交互笔线")]
            [Tip("更新交互笔线")]
            [EnumFieldName("方法/更新交互笔线")]
            Method_UpdateStylusLine,
            
            #endregion            
        }
        
        #region 字段
        
        /// <summary>
        /// 按钮数量:按钮数量
        /// </summary>
        [Name("按钮数量")]
        [Tip("按钮数量")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Field_buttonCount)]
        [OnlyMemberElements]
        public IntPropertyValue _Field_buttonCount = new IntPropertyValue();

#if XDREAMER_VOXELTRACKER

        /// <summary>
        /// 按钮ID:按钮ID
        /// </summary>
        [Name("按钮ID")]
        [Tip("按钮ID")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Field_buttonID)]
        [OnlyMemberElements]
        public ButtonIDPropertyValue _Field_buttonID = new ButtonIDPropertyValue();

#endif

        /// <summary>
        /// 点击时间阈值:点击时间阈值
        /// </summary>
        [Name("点击时间阈值")]
        [Tip("点击时间阈值")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Field_ClickTimeThreshold)]
        [OnlyMemberElements]
        public FloatPropertyValue _Field_ClickTimeThreshold = new FloatPropertyValue();

#if XDREAMER_VOXELTRACKER

        /// <summary>
        /// 默认拖动策略:默认拖动策略
        /// </summary>
        [Name("默认拖动策略")]
        [Tip("默认拖动策略")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Field_DefaultDragPolicy)]
        [OnlyMemberElements]
        public DragPolicyPropertyValue _Field_DefaultDragPolicy = new DragPolicyPropertyValue();

#endif

        /// <summary>
        /// 头部变换:头部变换
        /// </summary>
        [Name("头部变换")]
        [Tip("头部变换")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Field_headTrans)]
        [OnlyMemberElements]
        public TransformPropertyValue _Field_headTrans = new TransformPropertyValue();
        
        /// <summary>
        /// 击中:击中
        /// </summary>
        [Name("击中")]
        [Tip("击中")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Field_hit)]
        [OnlyMemberElements]
        public GameObjectPropertyValue _Field_hit = new GameObjectPropertyValue();
        
        /// <summary>
        /// 悬停对象默认启用:悬停对象默认启用
        /// </summary>
        [Name("悬停对象默认启用")]
        [Tip("悬停对象默认启用")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Field_hoverObjectDefaultEnable)]
        [OnlyMemberElements]
        public BoolPropertyValue _Field_hoverObjectDefaultEnable = new BoolPropertyValue();
        
        /// <summary>
        /// 实例:实例
        /// </summary>
        [Name("实例")]
        [Tip("实例")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Field_Instance)]
        [OnlyMemberElements]
        public UnityObjectPropertyValue _Field_Instance = new UnityObjectPropertyValue();

#if XDREAMER_VOXELTRACKER

        /// <summary>
        /// 对象拖动策略:对象拖动策略
        /// </summary>
        [Name("对象拖动策略")]
        [Tip("对象拖动策略")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Field_ObjectDragPolicy)]
        [OnlyMemberElements]
        public DragPolicyPropertyValue _Field_ObjectDragPolicy = new DragPolicyPropertyValue();
        
        /// <summary>
        /// 射线状态:射线状态
        /// </summary>
        [Name("射线状态")]
        [Tip("射线状态")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Field_rayState)]
        [OnlyMemberElements]
        public RayStatePropertyValue _Field_rayState = new RayStatePropertyValue();

#endif

        /// <summary>
        /// 每单位滚动米数:每单位滚动米数
        /// </summary>
        [Name("每单位滚动米数")]
        [Tip("每单位滚动米数")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Field_ScrollMetersPerUnit)]
        [OnlyMemberElements]
        public FloatPropertyValue _Field_ScrollMetersPerUnit = new FloatPropertyValue();
        
        /// <summary>
        /// 交互笔长度:交互笔长度
        /// </summary>
        [Name("交互笔长度")]
        [Tip("交互笔长度")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Field_stylusLength)]
        [OnlyMemberElements]
        public FloatPropertyValue _Field_stylusLength = new FloatPropertyValue();
        
        /// <summary>
        /// 交互笔长度锁定:交互笔长度锁定
        /// </summary>
        [Name("交互笔长度锁定")]
        [Tip("交互笔长度锁定")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Field_stylusLengthLock)]
        [OnlyMemberElements]
        public BoolPropertyValue _Field_stylusLengthLock = new BoolPropertyValue();

#if XDREAMER_VOXELTRACKER

        /// <summary>
        /// UI拖动策略:UI拖动策略
        /// </summary>
        [Name("UI拖动策略")]
        [Tip("UI拖动策略")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Field_UIDragPolicy)]
        [OnlyMemberElements]
        public DragPolicyPropertyValue _Field_UIDragPolicy = new DragPolicyPropertyValue();

#endif

        /// <summary>
        /// 事件相机:事件相机
        /// </summary>
        [Name("事件相机")]
        [Tip("事件相机")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Field_VEventCamera)]
        [OnlyMemberElements]
        public CameraPropertyValue _Field_VEventCamera = new CameraPropertyValue();
        
#endregion
        
#region 属性
        
        /// <summary>
        /// 默认击中距离:默认击中距离
        /// </summary>
        [Name("默认击中距离")]
        [Tip("默认击中距离")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Property_DefaultHitDistance)]
        [OnlyMemberElements]
        public FloatPropertyValue _Property_DefaultHitDistance = new FloatPropertyValue();
        
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
        /// 发送消息选项:发送消息选项
        /// </summary>
        [Name("发送消息选项")]
        [Tip("发送消息选项")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_BroadcastMessage_String_SendMessageOptions)]
        [OnlyMemberElements]
        public SendMessageOptionsPropertyValue _Method_BroadcastMessage_String_SendMessageOptions__options = new SendMessageOptionsPropertyValue();

#if XDREAMER_VOXELTRACKER

        /// <summary>
        /// 值:值
        /// </summary>
        [Name("值")]
        [Tip("值")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_CallStylusDown_ButtonID)]
        [OnlyMemberElements]
        public ButtonIDPropertyValue _Method_CallStylusDown_ButtonID__value = new ButtonIDPropertyValue();
        
        /// <summary>
        /// 值:值
        /// </summary>
        [Name("值")]
        [Tip("值")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_CallStylusPressed_ButtonID)]
        [OnlyMemberElements]
        public ButtonIDPropertyValue _Method_CallStylusPressed_ButtonID__value = new ButtonIDPropertyValue();
        
        /// <summary>
        /// 值:值
        /// </summary>
        [Name("值")]
        [Tip("值")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_CallStylusReleased_ButtonID)]
        [OnlyMemberElements]
        public ButtonIDPropertyValue _Method_CallStylusReleased_ButtonID__value = new ButtonIDPropertyValue();
        
#endif

        /// <summary>
        /// 方法名:方法名
        /// </summary>
        [Name("方法名")]
        [Tip("方法名")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_CancelInvoke_String)]
        [OnlyMemberElements]
        public StringPropertyValue _Method_CancelInvoke_String__methodName = new StringPropertyValue();
        
        /// <summary>
        /// 捕获对象:捕获对象
        /// </summary>
        [Name("捕获对象")]
        [Tip("捕获对象")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_CapturePointer_GameObject)]
        [OnlyMemberElements]
        public GameObjectPropertyValue _Method_CapturePointer_GameObject__captureObject = new GameObjectPropertyValue();
        
        /// <summary>
        /// 标签:标签
        /// </summary>
        [Name("标签")]
        [Tip("标签")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_CompareTag_String)]
        [OnlyMemberElements]
        public StringPropertyValue _Method_CompareTag_String__tag = new StringPropertyValue();
        
        /// <summary>
        /// id:id
        /// </summary>
        [Name("id")]
        [Tip("id")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_GetButton_Int32)]
        [OnlyMemberElements]
        public IntPropertyValue _Method_GetButton_Int32__id = new IntPropertyValue();
        
        /// <summary>
        /// id:id
        /// </summary>
        [Name("id")]
        [Tip("id")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_GetButtonDown_Int32)]
        [OnlyMemberElements]
        public IntPropertyValue _Method_GetButtonDown_Int32__id = new IntPropertyValue();
        
        /// <summary>
        /// id:id
        /// </summary>
        [Name("id")]
        [Tip("id")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_GetButtonMapping_Int32)]
        [OnlyMemberElements]
        public IntPropertyValue _Method_GetButtonMapping_Int32__id = new IntPropertyValue();
        
        /// <summary>
        /// id:id
        /// </summary>
        [Name("id")]
        [Tip("id")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_GetButtonUp_Int32)]
        [OnlyMemberElements]
        public IntPropertyValue _Method_GetButtonUp_Int32__id = new IntPropertyValue();
        
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
        /// 重复速率:重复速率
        /// </summary>
        [Name("重复速率")]
        [Tip("重复速率")]
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
        /// 发送消息选项:发送消息选项
        /// </summary>
        [Name("发送消息选项")]
        [Tip("发送消息选项")]
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
        /// 发送消息选项:发送消息选项
        /// </summary>
        [Name("发送消息选项")]
        [Tip("发送消息选项")]
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
        
        /// <summary>
        /// 值:值
        /// </summary>
        [Name("值")]
        [Tip("值")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_UpdateButton_Int32)]
        [OnlyMemberElements]
        public IntPropertyValue _Method_UpdateButton_Int32__value = new IntPropertyValue();
        
#endregion
        
        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="stateData">数据信息</param>
        /// <param name="executeMode">执行模式</param>
        public override void Execute(StateData stateData, EExecuteMode executeMode)
        {
#if XDREAMER_VOXELTRACKER
            switch (_propertyName)
            {
                case EPropertyName.Field_buttonCount:
                    {
                        var value = _Field_buttonCount.GetValue();
                        if (_StylusOne != null) _StylusOne.buttonCount = value;
                        foreach(var obj in _StylusOnes)
                        {
                            if (obj != null) obj.buttonCount = value;
                        }
                        break;
                    }
                case EPropertyName.Field_buttonID:
                    {
                        var value = _Field_buttonID.GetValue();
                        if (_StylusOne != null) _StylusOne.buttonID = value;
                        foreach(var obj in _StylusOnes)
                        {
                            if (obj != null) obj.buttonID = value;
                        }
                        break;
                    }
                case EPropertyName.Field_ClickTimeThreshold:
                    {
                        var value = _Field_ClickTimeThreshold.GetValue();
                        if (_StylusOne != null) _StylusOne.ClickTimeThreshold = value;
                        foreach(var obj in _StylusOnes)
                        {
                            if (obj != null) obj.ClickTimeThreshold = value;
                        }
                        break;
                    }
                case EPropertyName.Field_DefaultDragPolicy:
                    {
                        var value = _Field_DefaultDragPolicy.GetValue();
                        if (_StylusOne != null) _StylusOne.DefaultDragPolicy = value;
                        foreach(var obj in _StylusOnes)
                        {
                            if (obj != null) obj.DefaultDragPolicy = value;
                        }
                        break;
                    }
                case EPropertyName.Field_headTrans:
                    {
                        var value = _Field_headTrans.GetValue();
                        if (_StylusOne != null) _StylusOne.headTrans = value;
                        foreach(var obj in _StylusOnes)
                        {
                            if (obj != null) obj.headTrans = value;
                        }
                        break;
                    }
                case EPropertyName.Field_hit:
                    {
                        var value = _Field_hit.GetValue();
                        if (_StylusOne != null) _StylusOne.hit = value;
                        foreach(var obj in _StylusOnes)
                        {
                            if (obj != null) obj.hit = value;
                        }
                        break;
                    }
                case EPropertyName.Field_hoverObjectDefaultEnable:
                    {
                        var value = _Field_hoverObjectDefaultEnable.GetValue();
                        if (_StylusOne != null) _StylusOne.hoverObjectDefaultEnable = value;
                        foreach(var obj in _StylusOnes)
                        {
                            if (obj != null) obj.hoverObjectDefaultEnable = value;
                        }
                        break;
                    }
                case EPropertyName.Field_Instance:
                    {
                        var value = _Field_Instance.GetValue() as StylusOne;
                        StylusOne.Instance = value;
                        break;
                    }
                case EPropertyName.Field_ObjectDragPolicy:
                    {
                        var value = _Field_ObjectDragPolicy.GetValue();
                        if (_StylusOne != null) _StylusOne.ObjectDragPolicy = value;
                        foreach(var obj in _StylusOnes)
                        {
                            if (obj != null) obj.ObjectDragPolicy = value;
                        }
                        break;
                    }
                case EPropertyName.Field_rayState:
                    {
                        var value = _Field_rayState.GetValue();
                        if (_StylusOne != null) _StylusOne.rayState = value;
                        foreach(var obj in _StylusOnes)
                        {
                            if (obj != null) obj.rayState = value;
                        }
                        break;
                    }
                case EPropertyName.Field_ScrollMetersPerUnit:
                    {
                        var value = _Field_ScrollMetersPerUnit.GetValue();
                        if (_StylusOne != null) _StylusOne.ScrollMetersPerUnit = value;
                        foreach(var obj in _StylusOnes)
                        {
                            if (obj != null) obj.ScrollMetersPerUnit = value;
                        }
                        break;
                    }
                case EPropertyName.Field_stylusLength:
                    {
                        var value = _Field_stylusLength.GetValue();
                        if (_StylusOne != null) _StylusOne.stylusLength = value;
                        foreach(var obj in _StylusOnes)
                        {
                            if (obj != null) obj.stylusLength = value;
                        }
                        break;
                    }
                case EPropertyName.Field_stylusLengthLock:
                    {
                        var value = _Field_stylusLengthLock.GetValue();
                        if (_StylusOne != null) _StylusOne.stylusLengthLock = value;
                        foreach(var obj in _StylusOnes)
                        {
                            if (obj != null) obj.stylusLengthLock = value;
                        }
                        break;
                    }
                case EPropertyName.Field_UIDragPolicy:
                    {
                        var value = _Field_UIDragPolicy.GetValue();
                        if (_StylusOne != null) _StylusOne.UIDragPolicy = value;
                        foreach(var obj in _StylusOnes)
                        {
                            if (obj != null) obj.UIDragPolicy = value;
                        }
                        break;
                    }
                case EPropertyName.Field_VEventCamera:
                    {
                        var value = _Field_VEventCamera.GetValue();
                        if (_StylusOne != null) _StylusOne.VEventCamera = value;
                        foreach(var obj in _StylusOnes)
                        {
                            if (obj != null) obj.VEventCamera = value;
                        }
                        break;
                    }
                case EPropertyName.Property_DefaultHitDistance:
                    {
                        var value = _Property_DefaultHitDistance.GetValue();
                        if (_StylusOne != null) _StylusOne.DefaultHitDistance = value;
                        foreach(var obj in _StylusOnes)
                        {
                            if (obj != null) obj.DefaultHitDistance = value;
                        }
                        break;
                    }
                case EPropertyName.Property_enabled:
                    {
                        var value = _Property_enabled.GetValue();
                        if (_StylusOne != null) _StylusOne.enabled = value;
                        foreach(var obj in _StylusOnes)
                        {
                            if (obj != null) obj.enabled = value;
                        }
                        break;
                    }
                case EPropertyName.Property_hideFlags:
                    {
                        var value = _Property_hideFlags.GetValue();
                        if (_StylusOne != null) _StylusOne.hideFlags = value;
                        foreach(var obj in _StylusOnes)
                        {
                            if (obj != null) obj.hideFlags = value;
                        }
                        break;
                    }
                case EPropertyName.Property_name:
                    {
                        var value = _Property_name.GetValue();
                        if (_StylusOne != null) _StylusOne.name = value;
                        foreach(var obj in _StylusOnes)
                        {
                            if (obj != null) obj.name = value;
                        }
                        break;
                    }
                case EPropertyName.Property_tag:
                    {
                        var value = _Property_tag.GetValue();
                        if (_StylusOne != null) _StylusOne.tag = value;
                        foreach(var obj in _StylusOnes)
                        {
                            if (obj != null) obj.tag = value;
                        }
                        break;
                    }
                case EPropertyName.Property_useGUILayout:
                    {
                        var value = _Property_useGUILayout.GetValue();
                        if (_StylusOne != null) _StylusOne.useGUILayout = value;
                        foreach(var obj in _StylusOnes)
                        {
                            if (obj != null) obj.useGUILayout = value;
                        }
                        break;
                    }
                case EPropertyName.Method_BroadcastMessage_String:
                    {
                        var methodName = _Method_BroadcastMessage_String__methodName.GetValue();
                        if (_StylusOne != null) _StylusOne.BroadcastMessage(methodName);
                        foreach(var obj in _StylusOnes)
                        {
                            if (obj != null) obj.BroadcastMessage(methodName);
                        }
                        break;
                    }
                case EPropertyName.Method_BroadcastMessage_String_SendMessageOptions:
                    {
                        var methodName = _Method_BroadcastMessage_String_SendMessageOptions__methodName.GetValue();
                        var options = _Method_BroadcastMessage_String_SendMessageOptions__options.GetValue();
                        if (_StylusOne != null) _StylusOne.BroadcastMessage(methodName, options);
                        foreach(var obj in _StylusOnes)
                        {
                            if (obj != null) obj.BroadcastMessage(methodName, options);
                        }
                        break;
                    }
                case EPropertyName.Method_CallStylusDown_ButtonID:
                    {
                        var value = _Method_CallStylusDown_ButtonID__value.GetValue();
                        if (_StylusOne != null) _StylusOne.CallStylusDown(value);
                        foreach(var obj in _StylusOnes)
                        {
                            if (obj != null) obj.CallStylusDown(value);
                        }
                        break;
                    }
                case EPropertyName.Method_CallStylusPressed_ButtonID:
                    {
                        var value = _Method_CallStylusPressed_ButtonID__value.GetValue();
                        if (_StylusOne != null) _StylusOne.CallStylusPressed(value);
                        foreach(var obj in _StylusOnes)
                        {
                            if (obj != null) obj.CallStylusPressed(value);
                        }
                        break;
                    }
                case EPropertyName.Method_CallStylusReleased_ButtonID:
                    {
                        var value = _Method_CallStylusReleased_ButtonID__value.GetValue();
                        if (_StylusOne != null) _StylusOne.CallStylusReleased(value);
                        foreach(var obj in _StylusOnes)
                        {
                            if (obj != null) obj.CallStylusReleased(value);
                        }
                        break;
                    }
                case EPropertyName.Method_CancelInvoke:
                    {
                        if (_StylusOne != null) _StylusOne.CancelInvoke();
                        foreach(var obj in _StylusOnes)
                        {
                            if (obj != null) obj.CancelInvoke();
                        }
                        break;
                    }
                case EPropertyName.Method_CancelInvoke_String:
                    {
                        var methodName = _Method_CancelInvoke_String__methodName.GetValue();
                        if (_StylusOne != null) _StylusOne.CancelInvoke(methodName);
                        foreach(var obj in _StylusOnes)
                        {
                            if (obj != null) obj.CancelInvoke(methodName);
                        }
                        break;
                    }
                case EPropertyName.Method_CapturePointer_GameObject:
                    {
                        var captureObject = _Method_CapturePointer_GameObject__captureObject.GetValue();
                        if (_StylusOne != null) _StylusOne.CapturePointer(captureObject);
                        foreach(var obj in _StylusOnes)
                        {
                            if (obj != null) obj.CapturePointer(captureObject);
                        }
                        break;
                    }
                case EPropertyName.Method_CastBeamFromVPointer:
                    {
                        if (_StylusOne != null) _StylusOne.CastBeamFromVPointer();
                        foreach(var obj in _StylusOnes)
                        {
                            if (obj != null) obj.CastBeamFromVPointer();
                        }
                        break;
                    }
                case EPropertyName.Method_CompareTag_String:
                    {
                        var tag = _Method_CompareTag_String__tag.GetValue();
                        if (_StylusOne != null) _StylusOne.CompareTag(tag);
                        foreach(var obj in _StylusOnes)
                        {
                            if (obj != null) obj.CompareTag(tag);
                        }
                        break;
                    }
                case EPropertyName.Method_GetButton_Int32:
                    {
                        var id = _Method_GetButton_Int32__id.GetValue();
                        if (_StylusOne != null) _StylusOne.GetButton(id);
                        foreach(var obj in _StylusOnes)
                        {
                            if (obj != null) obj.GetButton(id);
                        }
                        break;
                    }
                case EPropertyName.Method_GetButtonDown_Int32:
                    {
                        var id = _Method_GetButtonDown_Int32__id.GetValue();
                        if (_StylusOne != null) _StylusOne.GetButtonDown(id);
                        foreach(var obj in _StylusOnes)
                        {
                            if (obj != null) obj.GetButtonDown(id);
                        }
                        break;
                    }
                case EPropertyName.Method_GetButtonMapping_Int32:
                    {
                        var id = _Method_GetButtonMapping_Int32__id.GetValue();
                        if (_StylusOne != null) _StylusOne.GetButtonMapping(id);
                        foreach(var obj in _StylusOnes)
                        {
                            if (obj != null) obj.GetButtonMapping(id);
                        }
                        break;
                    }
                case EPropertyName.Method_GetButtonUp_Int32:
                    {
                        var id = _Method_GetButtonUp_Int32__id.GetValue();
                        if (_StylusOne != null) _StylusOne.GetButtonUp(id);
                        foreach(var obj in _StylusOnes)
                        {
                            if (obj != null) obj.GetButtonUp(id);
                        }
                        break;
                    }
                case EPropertyName.Method_GetComponent_String:
                    {
                        var type = _Method_GetComponent_String__type.GetValue();
                        if (_StylusOne != null) _StylusOne.GetComponent(type);
                        foreach(var obj in _StylusOnes)
                        {
                            if (obj != null) obj.GetComponent(type);
                        }
                        break;
                    }
                case EPropertyName.Method_GetHashCode:
                    {
                        if (_StylusOne != null) _StylusOne.GetHashCode();
                        foreach(var obj in _StylusOnes)
                        {
                            if (obj != null) obj.GetHashCode();
                        }
                        break;
                    }
                case EPropertyName.Method_GetInstanceID:
                    {
                        if (_StylusOne != null) _StylusOne.GetInstanceID();
                        foreach(var obj in _StylusOnes)
                        {
                            if (obj != null) obj.GetInstanceID();
                        }
                        break;
                    }
                case EPropertyName.Method_GetType:
                    {
                        if (_StylusOne != null) _StylusOne.GetType();
                        foreach(var obj in _StylusOnes)
                        {
                            if (obj != null) obj.GetType();
                        }
                        break;
                    }
                case EPropertyName.Method_Invoke_String_Single:
                    {
                        var methodName = _Method_Invoke_String_Single__methodName.GetValue();
                        var time = _Method_Invoke_String_Single__time.GetValue();
                        if (_StylusOne != null) _StylusOne.Invoke(methodName, time);
                        foreach(var obj in _StylusOnes)
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
                        if (_StylusOne != null) _StylusOne.InvokeRepeating(methodName, time, repeatRate);
                        foreach(var obj in _StylusOnes)
                        {
                            if (obj != null) obj.InvokeRepeating(methodName, time, repeatRate);
                        }
                        break;
                    }
                case EPropertyName.Method_IsInvoking:
                    {
                        if (_StylusOne != null) _StylusOne.IsInvoking();
                        foreach(var obj in _StylusOnes)
                        {
                            if (obj != null) obj.IsInvoking();
                        }
                        break;
                    }
                case EPropertyName.Method_IsInvoking_String:
                    {
                        var methodName = _Method_IsInvoking_String__methodName.GetValue();
                        if (_StylusOne != null) _StylusOne.IsInvoking(methodName);
                        foreach(var obj in _StylusOnes)
                        {
                            if (obj != null) obj.IsInvoking(methodName);
                        }
                        break;
                    }
                case EPropertyName.Method_OnDisable:
                    {
                        if (_StylusOne != null) _StylusOne.OnDisable();
                        foreach(var obj in _StylusOnes)
                        {
                            if (obj != null) obj.OnDisable();
                        }
                        break;
                    }
                case EPropertyName.Method_OnEnable:
                    {
                        if (_StylusOne != null) _StylusOne.OnEnable();
                        foreach(var obj in _StylusOnes)
                        {
                            if (obj != null) obj.OnEnable();
                        }
                        break;
                    }
                case EPropertyName.Method_SendMessage_String:
                    {
                        var methodName = _Method_SendMessage_String__methodName.GetValue();
                        if (_StylusOne != null) _StylusOne.SendMessage(methodName);
                        foreach(var obj in _StylusOnes)
                        {
                            if (obj != null) obj.SendMessage(methodName);
                        }
                        break;
                    }
                case EPropertyName.Method_SendMessage_String_SendMessageOptions:
                    {
                        var methodName = _Method_SendMessage_String_SendMessageOptions__methodName.GetValue();
                        var options = _Method_SendMessage_String_SendMessageOptions__options.GetValue();
                        if (_StylusOne != null) _StylusOne.SendMessage(methodName, options);
                        foreach(var obj in _StylusOnes)
                        {
                            if (obj != null) obj.SendMessage(methodName, options);
                        }
                        break;
                    }
                case EPropertyName.Method_SendMessageUpwards_String:
                    {
                        var methodName = _Method_SendMessageUpwards_String__methodName.GetValue();
                        if (_StylusOne != null) _StylusOne.SendMessageUpwards(methodName);
                        foreach(var obj in _StylusOnes)
                        {
                            if (obj != null) obj.SendMessageUpwards(methodName);
                        }
                        break;
                    }
                case EPropertyName.Method_SendMessageUpwards_String_SendMessageOptions:
                    {
                        var methodName = _Method_SendMessageUpwards_String_SendMessageOptions__methodName.GetValue();
                        var options = _Method_SendMessageUpwards_String_SendMessageOptions__options.GetValue();
                        if (_StylusOne != null) _StylusOne.SendMessageUpwards(methodName, options);
                        foreach(var obj in _StylusOnes)
                        {
                            if (obj != null) obj.SendMessageUpwards(methodName, options);
                        }
                        break;
                    }
                case EPropertyName.Method_StartCoroutine_String:
                    {
                        var methodName = _Method_StartCoroutine_String__methodName.GetValue();
                        if (_StylusOne != null) _StylusOne.StartCoroutine(methodName);
                        foreach(var obj in _StylusOnes)
                        {
                            if (obj != null) obj.StartCoroutine(methodName);
                        }
                        break;
                    }
                case EPropertyName.Method_StopAllCoroutines:
                    {
                        if (_StylusOne != null) _StylusOne.StopAllCoroutines();
                        foreach(var obj in _StylusOnes)
                        {
                            if (obj != null) obj.StopAllCoroutines();
                        }
                        break;
                    }
                case EPropertyName.Method_StopCoroutine_String:
                    {
                        var methodName = _Method_StopCoroutine_String__methodName.GetValue();
                        if (_StylusOne != null) _StylusOne.StopCoroutine(methodName);
                        foreach(var obj in _StylusOnes)
                        {
                            if (obj != null) obj.StopCoroutine(methodName);
                        }
                        break;
                    }
                case EPropertyName.Method_ToString:
                    {
                        if (_StylusOne != null) _StylusOne.ToString();
                        foreach(var obj in _StylusOnes)
                        {
                            if (obj != null) obj.ToString();
                        }
                        break;
                    }
                case EPropertyName.Method_UpdateButton_Int32:
                    {
                        var value = _Method_UpdateButton_Int32__value.GetValue();
                        if (_StylusOne != null) _StylusOne.UpdateButton(value);
                        foreach(var obj in _StylusOnes)
                        {
                            if (obj != null) obj.UpdateButton(value);
                        }
                        break;
                    }
                case EPropertyName.Method_UpdateStylusLine:
                    {
                        if (_StylusOne != null) _StylusOne.UpdateStylusLine();
                        foreach(var obj in _StylusOnes)
                        {
                            if (obj != null) obj.UpdateStylusLine();
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
#if XDREAMER_VOXELTRACKER
            switch (_propertyName)
            {
                case EPropertyName.Field_buttonCount:
                    {
                        return CommonFun.Name(_propertyName) + " = " + _Field_buttonCount.ToFriendlyString();
                    }
                case EPropertyName.Field_buttonID:
                    {
                        return CommonFun.Name(_propertyName) + " = " + _Field_buttonID.ToFriendlyString();
                    }
                case EPropertyName.Field_ClickTimeThreshold:
                    {
                        return CommonFun.Name(_propertyName) + " = " + _Field_ClickTimeThreshold.ToFriendlyString();
                    }
                case EPropertyName.Field_DefaultDragPolicy:
                    {
                        return CommonFun.Name(_propertyName) + " = " + _Field_DefaultDragPolicy.ToFriendlyString();
                    }
                case EPropertyName.Field_headTrans:
                    {
                        return CommonFun.Name(_propertyName) + " = " + _Field_headTrans.ToFriendlyString();
                    }
                case EPropertyName.Field_hit:
                    {
                        return CommonFun.Name(_propertyName) + " = " + _Field_hit.ToFriendlyString();
                    }
                case EPropertyName.Field_hoverObjectDefaultEnable:
                    {
                        return CommonFun.Name(_propertyName) + " = " + _Field_hoverObjectDefaultEnable.ToFriendlyString();
                    }
                case EPropertyName.Field_Instance:
                    {
                        return CommonFun.Name(_propertyName) + " = " + _Field_Instance.ToFriendlyString();
                    }
                case EPropertyName.Field_ObjectDragPolicy:
                    {
                        return CommonFun.Name(_propertyName) + " = " + _Field_ObjectDragPolicy.ToFriendlyString();
                    }
                case EPropertyName.Field_rayState:
                    {
                        return CommonFun.Name(_propertyName) + " = " + _Field_rayState.ToFriendlyString();
                    }
                case EPropertyName.Field_ScrollMetersPerUnit:
                    {
                        return CommonFun.Name(_propertyName) + " = " + _Field_ScrollMetersPerUnit.ToFriendlyString();
                    }
                case EPropertyName.Field_stylusLength:
                    {
                        return CommonFun.Name(_propertyName) + " = " + _Field_stylusLength.ToFriendlyString();
                    }
                case EPropertyName.Field_stylusLengthLock:
                    {
                        return CommonFun.Name(_propertyName) + " = " + _Field_stylusLengthLock.ToFriendlyString();
                    }
                case EPropertyName.Field_UIDragPolicy:
                    {
                        return CommonFun.Name(_propertyName) + " = " + _Field_UIDragPolicy.ToFriendlyString();
                    }
                case EPropertyName.Field_VEventCamera:
                    {
                        return CommonFun.Name(_propertyName) + " = " + _Field_VEventCamera.ToFriendlyString();
                    }
                case EPropertyName.Property_DefaultHitDistance:
                    {
                        return CommonFun.Name(_propertyName) + " = " + _Property_DefaultHitDistance.ToFriendlyString();
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
                case EPropertyName.Method_CallStylusDown_ButtonID:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_CallStylusPressed_ButtonID:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_CallStylusReleased_ButtonID:
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
                case EPropertyName.Method_CapturePointer_GameObject:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_CastBeamFromVPointer:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_CompareTag_String:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_GetButton_Int32:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_GetButtonDown_Int32:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_GetButtonMapping_Int32:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_GetButtonUp_Int32:
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
                case EPropertyName.Method_OnDisable:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_OnEnable:
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
                case EPropertyName.Method_UpdateButton_Int32:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Method_UpdateStylusLine:
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
#if XDREAMER_VOXELTRACKER
            switch (_propertyName)
            {
                case EPropertyName.Field_buttonCount:
                    {
                        return _StylusOne && _Field_buttonCount.DataValidity();
                    }
                case EPropertyName.Field_buttonID:
                    {
                        return _StylusOne && _Field_buttonID.DataValidity();
                    }
                case EPropertyName.Field_ClickTimeThreshold:
                    {
                        return _StylusOne && _Field_ClickTimeThreshold.DataValidity();
                    }
                case EPropertyName.Field_DefaultDragPolicy:
                    {
                        return _StylusOne && _Field_DefaultDragPolicy.DataValidity();
                    }
                case EPropertyName.Field_headTrans:
                    {
                        return _StylusOne && _Field_headTrans.DataValidity();
                    }
                case EPropertyName.Field_hit:
                    {
                        return _StylusOne && _Field_hit.DataValidity();
                    }
                case EPropertyName.Field_hoverObjectDefaultEnable:
                    {
                        return _StylusOne && _Field_hoverObjectDefaultEnable.DataValidity();
                    }
                case EPropertyName.Field_Instance:
                    {
                        return _StylusOne && _Field_Instance.DataValidity();
                    }
                case EPropertyName.Field_ObjectDragPolicy:
                    {
                        return _StylusOne && _Field_ObjectDragPolicy.DataValidity();
                    }
                case EPropertyName.Field_rayState:
                    {
                        return _StylusOne && _Field_rayState.DataValidity();
                    }
                case EPropertyName.Field_ScrollMetersPerUnit:
                    {
                        return _StylusOne && _Field_ScrollMetersPerUnit.DataValidity();
                    }
                case EPropertyName.Field_stylusLength:
                    {
                        return _StylusOne && _Field_stylusLength.DataValidity();
                    }
                case EPropertyName.Field_stylusLengthLock:
                    {
                        return _StylusOne && _Field_stylusLengthLock.DataValidity();
                    }
                case EPropertyName.Field_UIDragPolicy:
                    {
                        return _StylusOne && _Field_UIDragPolicy.DataValidity();
                    }
                case EPropertyName.Field_VEventCamera:
                    {
                        return _StylusOne && _Field_VEventCamera.DataValidity();
                    }
                case EPropertyName.Property_DefaultHitDistance:
                    {
                        return _StylusOne && _Property_DefaultHitDistance.DataValidity();
                    }
                case EPropertyName.Property_enabled:
                    {
                        return _StylusOne && _Property_enabled.DataValidity();
                    }
                case EPropertyName.Property_hideFlags:
                    {
                        return _StylusOne && _Property_hideFlags.DataValidity();
                    }
                case EPropertyName.Property_name:
                    {
                        return _StylusOne && _Property_name.DataValidity();
                    }
                case EPropertyName.Property_tag:
                    {
                        return _StylusOne && _Property_tag.DataValidity();
                    }
                case EPropertyName.Property_useGUILayout:
                    {
                        return _StylusOne && _Property_useGUILayout.DataValidity();
                    }
                case EPropertyName.Method_BroadcastMessage_String:
                    {
                        return _StylusOne && _Method_BroadcastMessage_String__methodName.DataValidity();
                    }
                case EPropertyName.Method_BroadcastMessage_String_SendMessageOptions:
                    {
                        if(!_StylusOne) return false;
                        if(!_Method_BroadcastMessage_String_SendMessageOptions__methodName.DataValidity()) return false;
                        if(!_Method_BroadcastMessage_String_SendMessageOptions__options.DataValidity()) return false;
                        break;
                    }
                case EPropertyName.Method_CallStylusDown_ButtonID:
                    {
                        return _StylusOne && _Method_CallStylusDown_ButtonID__value.DataValidity();
                    }
                case EPropertyName.Method_CallStylusPressed_ButtonID:
                    {
                        return _StylusOne && _Method_CallStylusPressed_ButtonID__value.DataValidity();
                    }
                case EPropertyName.Method_CallStylusReleased_ButtonID:
                    {
                        return _StylusOne && _Method_CallStylusReleased_ButtonID__value.DataValidity();
                    }
                case EPropertyName.Method_CancelInvoke:
                    {
                        return _StylusOne;
                    }
                case EPropertyName.Method_CancelInvoke_String:
                    {
                        return _StylusOne && _Method_CancelInvoke_String__methodName.DataValidity();
                    }
                case EPropertyName.Method_CapturePointer_GameObject:
                    {
                        return _StylusOne && _Method_CapturePointer_GameObject__captureObject.DataValidity();
                    }
                case EPropertyName.Method_CastBeamFromVPointer:
                    {
                        return _StylusOne;
                    }
                case EPropertyName.Method_CompareTag_String:
                    {
                        return _StylusOne && _Method_CompareTag_String__tag.DataValidity();
                    }
                case EPropertyName.Method_GetButton_Int32:
                    {
                        return _StylusOne && _Method_GetButton_Int32__id.DataValidity();
                    }
                case EPropertyName.Method_GetButtonDown_Int32:
                    {
                        return _StylusOne && _Method_GetButtonDown_Int32__id.DataValidity();
                    }
                case EPropertyName.Method_GetButtonMapping_Int32:
                    {
                        return _StylusOne && _Method_GetButtonMapping_Int32__id.DataValidity();
                    }
                case EPropertyName.Method_GetButtonUp_Int32:
                    {
                        return _StylusOne && _Method_GetButtonUp_Int32__id.DataValidity();
                    }
                case EPropertyName.Method_GetComponent_String:
                    {
                        return _StylusOne && _Method_GetComponent_String__type.DataValidity();
                    }
                case EPropertyName.Method_GetHashCode:
                    {
                        return _StylusOne;
                    }
                case EPropertyName.Method_GetInstanceID:
                    {
                        return _StylusOne;
                    }
                case EPropertyName.Method_GetType:
                    {
                        return _StylusOne;
                    }
                case EPropertyName.Method_Invoke_String_Single:
                    {
                        if(!_StylusOne) return false;
                        if(!_Method_Invoke_String_Single__methodName.DataValidity()) return false;
                        if(!_Method_Invoke_String_Single__time.DataValidity()) return false;
                        break;
                    }
                case EPropertyName.Method_InvokeRepeating_String_Single_Single:
                    {
                        if(!_StylusOne) return false;
                        if(!_Method_InvokeRepeating_String_Single_Single__methodName.DataValidity()) return false;
                        if(!_Method_InvokeRepeating_String_Single_Single__time.DataValidity()) return false;
                        if(!_Method_InvokeRepeating_String_Single_Single__repeatRate.DataValidity()) return false;
                        break;
                    }
                case EPropertyName.Method_IsInvoking:
                    {
                        return _StylusOne;
                    }
                case EPropertyName.Method_IsInvoking_String:
                    {
                        return _StylusOne && _Method_IsInvoking_String__methodName.DataValidity();
                    }
                case EPropertyName.Method_OnDisable:
                    {
                        return _StylusOne;
                    }
                case EPropertyName.Method_OnEnable:
                    {
                        return _StylusOne;
                    }
                case EPropertyName.Method_SendMessage_String:
                    {
                        return _StylusOne && _Method_SendMessage_String__methodName.DataValidity();
                    }
                case EPropertyName.Method_SendMessage_String_SendMessageOptions:
                    {
                        if(!_StylusOne) return false;
                        if(!_Method_SendMessage_String_SendMessageOptions__methodName.DataValidity()) return false;
                        if(!_Method_SendMessage_String_SendMessageOptions__options.DataValidity()) return false;
                        break;
                    }
                case EPropertyName.Method_SendMessageUpwards_String:
                    {
                        return _StylusOne && _Method_SendMessageUpwards_String__methodName.DataValidity();
                    }
                case EPropertyName.Method_SendMessageUpwards_String_SendMessageOptions:
                    {
                        if(!_StylusOne) return false;
                        if(!_Method_SendMessageUpwards_String_SendMessageOptions__methodName.DataValidity()) return false;
                        if(!_Method_SendMessageUpwards_String_SendMessageOptions__options.DataValidity()) return false;
                        break;
                    }
                case EPropertyName.Method_StartCoroutine_String:
                    {
                        return _StylusOne && _Method_StartCoroutine_String__methodName.DataValidity();
                    }
                case EPropertyName.Method_StopAllCoroutines:
                    {
                        return _StylusOne;
                    }
                case EPropertyName.Method_StopCoroutine_String:
                    {
                        return _StylusOne && _Method_StopCoroutine_String__methodName.DataValidity();
                    }
                case EPropertyName.Method_ToString:
                    {
                        return _StylusOne;
                    }
                case EPropertyName.Method_UpdateButton_Int32:
                    {
                        return _StylusOne && _Method_UpdateButton_Int32__value.DataValidity();
                    }
                case EPropertyName.Method_UpdateStylusLine:
                    {
                        return _StylusOne;
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
#if XDREAMER_VOXELTRACKER
            if (StylusOne) { }
#endif
        }
        
    }
}
