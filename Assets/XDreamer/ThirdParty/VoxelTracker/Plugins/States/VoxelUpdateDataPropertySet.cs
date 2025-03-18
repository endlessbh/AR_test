using XCSJ.Attributes;
using XCSJ.Extension.Base.Attributes;
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
    /// Voxel更新数据属性设置: Voxel更新数据属性设置
    /// </summary>
    [ComponentMenu(VoxelTrackerHelper.Title + "/" + Title, typeof(VoxelTrackerManager))]
    [Name(Title, nameof(VoxelUpdateDataPropertySet))]
    [Tip("Voxel更新数据属性设置")]
    [XCSJ.Attributes.Icon(EIcon.Property)]
    [Owner(typeof(VoxelTrackerManager))]
    public class VoxelUpdateDataPropertySet : BasePropertySet<VoxelUpdateDataPropertySet>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "Voxel更新数据属性设置";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(VoxelTrackerHelper.Title, typeof(VoxelTrackerManager))]
        [StateComponentMenu(VoxelTrackerHelper.Title + "/" + Title, typeof(VoxelTrackerManager))]
        [Name(Title, nameof(VoxelUpdateDataPropertySet))]
        [Tip("Voxel更新数据属性设置")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 属性名称
        /// </summary>
        [Name("属性名称")]
#if UNITY_2019_3_OR_NEWER
        //[EnumPopup]
#else
        [EnumPopup]
#endif
        public EPropertyName _propertyName = EPropertyName.Method_SetStylusFeature;

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

            #region 方法

            /// <summary>
            /// 初始化组件:初始化组件
            /// </summary>
            [Name("初始化组件")]
            [Tip("初始化组件")]
            [EnumFieldName("方法/初始化组件")]
            Method_InitComponent = 10000,

            /// <summary>
            /// 注销组件:注销组件
            /// </summary>
            [Name("注销组件")]
            [Tip("注销组件")]
            [EnumFieldName("方法/注销组件")]
            Method_LogoutComponent,

            /// <summary>
            /// 设置交互笔功能:设置交互笔功能
            /// </summary>
            [Name("设置交互笔功能")]
            [Tip("设置交互笔功能")]
            [EnumFieldName("方法/设置交互笔功能")]
            Method_SetStylusFeature,

            /// <summary>
            /// 套接字发送:套接字发送
            /// </summary>
            [Name("套接字发送")]
            [Tip("套接字发送")]
            [EnumFieldName("方法/套接字发送")]
            Method_SocketSend,

            #endregion
        }

        #region 方法

#if XDREAMER_VOXELTRACKER

        /// <summary>
        /// 交互笔功能:交互笔功能
        /// </summary>
        [Name("交互笔功能")]
        [Tip("交互笔功能")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_SetStylusFeature)]
        public StylusFeaturePropertyValue _Method_SetStylusFeature_String_Single__feature = new StylusFeaturePropertyValue();

#endif

        /// <summary>
        /// 持续时间:持续时间单位为秒
        /// </summary>
        [Name("持续时间")]
        [Tip("持续时间单位为秒")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_SetStylusFeature)]
        [OnlyMemberElements]
        public PositiveFloatPropertyValue _Method_SetStylusFeature_String_Single__duration = new PositiveFloatPropertyValue(1);

        /// <summary>
        /// 发送字符串:发送字符串
        /// </summary>
        [Name("发送字符串")]
        [Tip("发送字符串")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_SocketSend)]
        [OnlyMemberElements]
        public StringPropertyValue _Method_SocketSend_String__sendStr = new StringPropertyValue();

#endregion

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="stateData"></param>
        /// <param name="executeMode"></param>
        public override void Execute(StateData stateData, EExecuteMode executeMode)
        {
#if XDREAMER_VOXELTRACKER
            switch (_propertyName)
            {
                case EPropertyName.Method_InitComponent:
                    {
                        VoxelUpdateData.Instance.InitComponent();
                        break;
                    }
                case EPropertyName.Method_LogoutComponent:
                    {
                        VoxelUpdateData.Instance.InitComponent();
                        break;
                    }
                case EPropertyName.Method_SetStylusFeature:
                    {
                        VoxelUpdateData.Instance.SetStylusFeature(_Method_SetStylusFeature_String_Single__feature.GetValue(), _Method_SetStylusFeature_String_Single__duration.GetValue());
                        break;
                    }
                case EPropertyName.Method_SocketSend:
                    {
                        VoxelUpdateData.Instance.SocketSend(_Method_SocketSend_String__sendStr.GetValue());
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
            return CommonFun.Name(_propertyName);
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
                case EPropertyName.Method_InitComponent:
                case EPropertyName.Method_LogoutComponent:
                    {
                        break;
                    }
                case EPropertyName.Method_SetStylusFeature: return _Method_SetStylusFeature_String_Single__feature.DataValidity() && _Method_SetStylusFeature_String_Single__duration.DataValidity();
                case EPropertyName.Method_SocketSend: return _Method_SocketSend_String__sendStr.DataValidity();
            }
#endif
            return base.DataValidity();
        }
    }
}
