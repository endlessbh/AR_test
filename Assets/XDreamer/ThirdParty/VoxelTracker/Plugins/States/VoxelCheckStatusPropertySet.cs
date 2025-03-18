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
    /// Voxel检测状态属性设置: Voxel检测状态属性设置
    /// </summary>
    [ComponentMenu(VoxelTrackerHelper.Title + "/" + Title, typeof(VoxelTrackerManager))]
    [Name(Title, nameof(VoxelCheckStatusPropertySet))]
    [Tip("Voxel检测状态属性设置", "Voxel detection status attribute settings")]
    [XCSJ.Attributes.Icon(EIcon.Property)]
    [Owner(typeof(VoxelTrackerManager))]
    public class VoxelCheckStatusPropertySet : BasePropertySet<VoxelCheckStatusPropertySet>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "Voxel检测状态属性设置";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(VoxelTrackerHelper.Title, typeof(VoxelTrackerManager))]
        [StateComponentMenu(VoxelTrackerHelper.Title + "/" + Title, typeof(VoxelTrackerManager))]
        [Name(Title, nameof(VoxelCheckStatusPropertySet))]
        [Tip("Voxel检测状态属性设置", "Voxel detection status attribute settings")]
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
        public EPropertyName _propertyName = EPropertyName.Method_SetVoxelState;

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
            /// 初始化组件:执行
            /// </summary>
            [Name("执行")]
            [Tip("执行")]
            [EnumFieldName("方法/执行")]
            Method_Execute = 10000,

            /// <summary>
            /// 注销组件:注销组件
            /// </summary>
            [Name("注销组件")]
            [Tip("注销组件")]
            [EnumFieldName("方法/注销组件")]
            Method_PostFunction,

            /// <summary>
            /// 设置Voxel状态:设置Voxel状态
            /// </summary>
            [Name("设置Voxel状态")]
            [Tip("设置Voxel状态")]
            [EnumFieldName("方法/设置Voxel状态")]
            Method_SetVoxelState,

            #endregion
        }

        #region 方法

        /// <summary>
        /// 命令:命令
        /// </summary>
        [Name("命令")]
        [Tip("命令")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_Execute)]
        [OnlyMemberElements]
        public StringPropertyValue _Method_Execute_String_Int32__command = new StringPropertyValue();

        /// <summary>
        /// 秒数:秒数
        /// </summary>
        [Name("秒数")]
        [Tip("秒数")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_Execute)]
        [OnlyMemberElements]
        public IntPropertyValue _Method_Execute_String_Int32__seconds = new IntPropertyValue(0);

        /// <summary>
        /// 状态:状态
        /// </summary>
        [Name("状态")]
        [Tip("状态")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Method_SetVoxelState)]
        [OnlyMemberElements]
        public IntPropertyValue _Method_SetVoxelState_Int32__state = new IntPropertyValue();

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
                case EPropertyName.Method_Execute:
                    {
                        VoxelCheckStatus.Instance.Execute(_Method_Execute_String_Int32__command.GetValue(), _Method_Execute_String_Int32__seconds.GetValue());
                        break;
                    }
                case EPropertyName.Method_PostFunction:
                    {
                        VoxelCheckStatus.Instance.PostFunction();
                        break;
                    }
                case EPropertyName.Method_SetVoxelState:
                    {
                        VoxelCheckStatus.Instance.SetVoxelState(_Method_SetVoxelState_Int32__state.GetValue());
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
            switch (_propertyName)
            {
                case EPropertyName.Method_Execute: return _Method_Execute_String_Int32__command.DataValidity() && _Method_Execute_String_Int32__seconds.DataValidity();
                case EPropertyName.Method_PostFunction:
                    {
                        break;
                    }
                case EPropertyName.Method_SetVoxelState: return _Method_SetVoxelState_Int32__state.DataValidity();
            }
            return base.DataValidity();
        }
    }
}
