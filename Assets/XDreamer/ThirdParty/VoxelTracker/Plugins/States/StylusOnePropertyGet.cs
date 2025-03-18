using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Attributes;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Base;
using XCSJ.Scripts;
using UnityEngine;

#if XDREAMER_VOXELTRACKER
using VoxelStationUtil;
#endif

namespace XCSJ.PluginVoxelTracker.States
{
    /// <summary>
    /// 交互笔属性获取: 交互笔属性获取
    /// </summary>
    [ComponentMenu(VoxelTrackerHelper.Title + "/" + Title)]
    [Name(Title, nameof(StylusOnePropertyGet))]
    [Tip("交互笔属性获取")]
    [XCSJ.Attributes.Icon(EIcon.Property)]
    [Owner(typeof(VoxelTrackerManager))]
    public class StylusOnePropertyGet : BasePropertyGet<StylusOnePropertyGet>, ISerializationCallbackReceiver
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "交互笔属性获取";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(VoxelTrackerHelper.Title, typeof(VoxelTrackerManager))]
        [StateComponentMenu(VoxelTrackerHelper.Title + "/" + Title, typeof(VoxelTrackerManager))]
        [Name(Title, nameof(StylusOnePropertyGet))]
        [Tip("交互笔属性获取")]
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
            /// 交互笔类型:交互笔类型
            /// </summary>
            [Name("交互笔类型")]
            [Tip("交互笔类型")]
            [EnumFieldName("属性/交互笔类型")]
            Property_StylusType = 1000,
            
            /// <summary>
            /// 抓取对象:抓取对象
            /// </summary>
            [Name("抓取对象")]
            [Tip("抓取对象")]
            [EnumFieldName("属性/抓取对象")]
            Property_GrabObject,
            
            /// <summary>
            /// UI按钮数量:UI按钮数量
            /// </summary>
            [Name("UI按钮数量")]
            [Tip("UI按钮数量")]
            [EnumFieldName("属性/UI按钮数量")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Property_UIButtonCount,
            
            /// <summary>
            /// ID:ID
            /// </summary>
            [Name("ID")]
            [Tip("ID")]
            [EnumFieldName("属性/ID")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Property_ID,
            
            /// <summary>
            /// 滚动增量:滚动增量
            /// </summary>
            [Name("滚动增量")]
            [Tip("滚动增量")]
            [EnumFieldName("属性/滚动增量")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Property_ScrollDelta,
            
            /// <summary>
            /// 任意按钮按压:任意按钮按压
            /// </summary>
            [Name("任意按钮按压")]
            [Tip("任意按钮按压")]
            [EnumFieldName("属性/任意按钮按压")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Property_AnyButtonPressed,
            
            /// <summary>
            /// 默认击中距离:默认击中距离
            /// </summary>
            [Name("默认击中距离")]
            [Tip("默认击中距离")]
            [EnumFieldName("属性/默认击中距离")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Property_DefaultHitDistance,
            
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
            /// 是激活并启用:是激活并启用
            /// </summary>
            [Name("是激活并启用")]
            [Tip("是激活并启用")]
            [EnumFieldName("属性/是激活并启用")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Property_isActiveAndEnabled,
            
            /// <summary>
            /// 变换:变换
            /// </summary>
            [Name("变换")]
            [Tip("变换")]
            [EnumFieldName("属性/变换")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Property_transform,
            
            /// <summary>
            /// 游戏对象:游戏对象
            /// </summary>
            [Name("游戏对象")]
            [Tip("游戏对象")]
            [EnumFieldName("属性/游戏对象")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Property_gameObject,
            
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
            /// 隐藏标识:隐藏标识
            /// </summary>
            [Name("隐藏标识")]
            [Tip("隐藏标识")]
            [EnumFieldName("属性/隐藏标识")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Property_hideFlags,

            #endregion

            #region 方法

            /// <summary>
            /// 比较标签:比较标签
            /// </summary>
            [Name("比较标签")]
            [Tip("比较标签")]
            [EnumFieldName("方法/比较标签")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_CompareTag_String = 10000,
            
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
            /// 是否调用中:是否调用中
            /// </summary>
            [Name("是否调用中")]
            [Tip("是否调用中")]
            [EnumFieldName("方法/是否调用中")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_IsInvoking_String,
            
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
            /// 转字符串:转字符串
            /// </summary>
            [Name("转字符串")]
            [Tip("转字符串")]
            [EnumFieldName("方法/转字符串")]
#if !XDREAMER_EDITION_DEVELOPER
            [HideInSuperInspector]
#endif
            Method_ToString,

            #endregion
        }

        #region 方法

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
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_IsInvoking_String)]
        [OnlyMemberElements]
        public StringPropertyValue _Method_IsInvoking_String__methodName = new StringPropertyValue();

        /// <summary>
        /// 方法名:方法名
        /// </summary>
        [Name("方法名")]
        [Tip("方法名")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_StartCoroutine_String)]
        [OnlyMemberElements]
        public StringPropertyValue _Method_StartCoroutine_String__methodName = new StringPropertyValue();

        #endregion

        /// <summary>
        /// 变量名:将获取到的属性值存储在变量名对应的全局变量中
        /// </summary>
        [Name("变量名")]
        [Tip("将获取到的属性值存储在变量名对应的全局变量中")]
        [ValidityCheck(EValidityCheckType.NotNullOrEmpty)]
        [VarString(EVarStringHierarchyKeyMode.Set)]
        public string _variableName;

        #region ISerializationCallbackReceiver

        void ISerializationCallbackReceiver.OnBeforeSerialize() { }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            CommonFun.VarNameToVarString(ref _variableName);
        }

        #endregion

        /// <summary>
        /// 将值设置到变量
        /// </summary>
        /// <param name="value">值</param>
        protected void SetToVariable(object value)
        {
            _variableName.TrySetOrAddSetHierarchyVarValue(value);
        }
        
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
                    SetToVariable(_StylusOne.buttonCount);
                    break;
                }
                case EPropertyName.Field_buttonID:
                {
                    SetToVariable(_StylusOne.buttonID);
                    break;
                }
                case EPropertyName.Field_ClickTimeThreshold:
                {
                    SetToVariable(_StylusOne.ClickTimeThreshold);
                    break;
                }
                case EPropertyName.Field_DefaultDragPolicy:
                {
                    SetToVariable(_StylusOne.DefaultDragPolicy);
                    break;
                }
                case EPropertyName.Field_headTrans:
                {
                    SetToVariable(_StylusOne.headTrans);
                    break;
                }
                case EPropertyName.Field_hit:
                {
                    SetToVariable(_StylusOne.hit);
                    break;
                }
                case EPropertyName.Field_hoverObjectDefaultEnable:
                {
                    SetToVariable(_StylusOne.hoverObjectDefaultEnable);
                    break;
                }
                case EPropertyName.Field_Instance:
                {
                    SetToVariable(StylusOne.Instance);
                    break;
                }
                case EPropertyName.Field_ObjectDragPolicy:
                {
                    SetToVariable(_StylusOne.ObjectDragPolicy);
                    break;
                }
                case EPropertyName.Field_rayState:
                {
                    SetToVariable(_StylusOne.rayState);
                    break;
                }
                case EPropertyName.Field_ScrollMetersPerUnit:
                {
                    SetToVariable(_StylusOne.ScrollMetersPerUnit);
                    break;
                }
                case EPropertyName.Field_stylusLength:
                {
                    SetToVariable(_StylusOne.stylusLength);
                    break;
                }
                case EPropertyName.Field_stylusLengthLock:
                {
                    SetToVariable(_StylusOne.stylusLengthLock);
                    break;
                }
                case EPropertyName.Field_UIDragPolicy:
                {
                    SetToVariable(_StylusOne.UIDragPolicy);
                    break;
                }
                case EPropertyName.Field_VEventCamera:
                {
                    SetToVariable(_StylusOne.VEventCamera);
                    break;
                }
                case EPropertyName.Property_StylusType:
                {
                    SetToVariable(_StylusOne.StylusType);
                    break;
                }
                case EPropertyName.Property_GrabObject:
                {
                    SetToVariable(_StylusOne.GrabObject);
                    break;
                }
                case EPropertyName.Property_UIButtonCount:
                {
                    SetToVariable(_StylusOne.UIButtonCount);
                    break;
                }
                case EPropertyName.Property_ID:
                {
                    SetToVariable(_StylusOne.ID);
                    break;
                }
                case EPropertyName.Property_ScrollDelta:
                {
                    SetToVariable(_StylusOne.ScrollDelta);
                    break;
                }
                case EPropertyName.Property_AnyButtonPressed:
                {
                    SetToVariable(_StylusOne.AnyButtonPressed);
                    break;
                }
                case EPropertyName.Property_DefaultHitDistance:
                {
                    SetToVariable(_StylusOne.DefaultHitDistance);
                    break;
                }
                case EPropertyName.Property_useGUILayout:
                {
                    SetToVariable(_StylusOne.useGUILayout);
                    break;
                }
                case EPropertyName.Property_enabled:
                {
                    SetToVariable(_StylusOne.enabled);
                    break;
                }
                case EPropertyName.Property_isActiveAndEnabled:
                {
                    SetToVariable(_StylusOne.isActiveAndEnabled);
                    break;
                }
                case EPropertyName.Property_transform:
                {
                    SetToVariable(_StylusOne.transform);
                    break;
                }
                case EPropertyName.Property_gameObject:
                {
                    SetToVariable(_StylusOne.gameObject);
                    break;
                }
                case EPropertyName.Property_tag:
                {
                    SetToVariable(_StylusOne.tag);
                    break;
                }
                case EPropertyName.Property_name:
                {
                    SetToVariable(_StylusOne.name);
                    break;
                }
                case EPropertyName.Property_hideFlags:
                {
                    SetToVariable(_StylusOne.hideFlags);
                    break;
                }
                case EPropertyName.Method_CompareTag_String:
                {
                    SetToVariable(_StylusOne.CompareTag(_Method_CompareTag_String__tag.GetValue()));
                    break;
                }
                case EPropertyName.Method_GetButton_Int32:
                {
                    SetToVariable(_StylusOne.GetButton(_Method_GetButton_Int32__id.GetValue()));
                    break;
                }
                case EPropertyName.Method_GetButtonDown_Int32:
                {
                    SetToVariable(_StylusOne.GetButtonDown(_Method_GetButtonDown_Int32__id.GetValue()));
                    break;
                }
                case EPropertyName.Method_GetButtonMapping_Int32:
                {
                    SetToVariable(_StylusOne.GetButtonMapping(_Method_GetButtonMapping_Int32__id.GetValue()));
                    break;
                }
                case EPropertyName.Method_GetButtonUp_Int32:
                {
                    SetToVariable(_StylusOne.GetButtonUp(_Method_GetButtonUp_Int32__id.GetValue()));
                    break;
                }
                case EPropertyName.Method_GetComponent_String:
                {
                    SetToVariable(_StylusOne.GetComponent(_Method_GetComponent_String__type.GetValue()));
                    break;
                }
                case EPropertyName.Method_GetHashCode:
                {
                    SetToVariable(_StylusOne.GetHashCode());
                    break;
                }
                case EPropertyName.Method_GetInstanceID:
                {
                    SetToVariable(_StylusOne.GetInstanceID());
                    break;
                }
                case EPropertyName.Method_GetType:
                {
                    SetToVariable(_StylusOne.GetType());
                    break;
                }
                case EPropertyName.Method_IsInvoking:
                {
                    SetToVariable(_StylusOne.IsInvoking());
                    break;
                }
                case EPropertyName.Method_IsInvoking_String:
                {
                    SetToVariable(_StylusOne.IsInvoking(_Method_IsInvoking_String__methodName.GetValue()));
                    break;
                }
                case EPropertyName.Method_StartCoroutine_String:
                {
                    SetToVariable(_StylusOne.StartCoroutine(_Method_StartCoroutine_String__methodName.GetValue()));
                    break;
                }
                case EPropertyName.Method_ToString:
                {
                    SetToVariable(_StylusOne.ToString());
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
            return ScriptOption.VarFlag + _variableName + " = " + CommonFun.Name(_propertyName);
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
#if XDREAMER_VOXELTRACKER
            if (StylusOne) { }
#endif
        }
    }
}
