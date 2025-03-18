using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Base;
using XCSJ.PluginXAR.Foundation.Base.Tools;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.Scripts;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Extension.Base.Algorithms;

#if XDREAMER_AR_FOUNDATION
using UnityEngine.XR.ARFoundation;
#endif

namespace XCSJ.PluginXAR.Foundation.Base.States
{
    /// <summary>
    /// AR会话属性设置
    /// </summary>
    [Name(Title, nameof(ARSessionPropertySet))]
    [Owner(typeof(XARFoundationManager))]
    public class ARSessionPropertySet : BasePropertySet<ARSessionPropertySet>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "AR会话属性设置";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [Name(Title, nameof(ARSessionPropertySet))]
#if UNITY_EDITOR && XDREAMER_EDITION_DEVELOPER
        [StateLib(XARFoundationHelper.Title, typeof(XARFoundationManager))]
        [StateComponentMenu(XARFoundationHelper.Title + "/" + Title, typeof(XARFoundationManager))]
#endif
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

#if XDREAMER_AR_FOUNDATION

        /// <summary>
        /// AR会话
        /// </summary>
        [Name("AR会话")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ComponentPopup]
        public ARSession _session;

#endif
        /// <summary>
        /// 属性名称
        /// </summary>
        [Name("属性名称")]
        [EnumPopup]
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
            None,

            /// <summary>
            /// 重置
            /// </summary>
            [Name("重置")]
            Reset,

            /// <summary>
            /// 启用：通过对AR会话组件的启用或禁用，实现AR功能的暂停或继续；
            /// </summary>
            [Name("启用")]
            [Tip("通过对AR会话组件的启用或禁用，实现AR功能的暂停或继续；", "Pause or continue the AR function by enabling or disabling the AR session component;")]
            Enable,
        }

        /// <summary>
        /// 启用：通过对AR会话组件的启用或禁用，实现AR功能的暂停或继续；
        /// </summary>
        [Name("启用")]
        [Tip("通过对AR会话组件的启用或禁用，实现AR功能的暂停或继续；", "Pause or continue the AR function by enabling or disabling the AR session component;")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Enable)]
        [OnlyMemberElements]
        public EBoolPropertyValue _enableSession = new EBoolPropertyValue();

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="stateData"></param>
        /// <param name="executeMode"></param>
        public override void Execute(StateData stateData, EExecuteMode executeMode)
        {
#if XDREAMER_AR_FOUNDATION
            if (!_session) return;
            switch (_propertyName)
            {
                case EPropertyName.Reset:
                    {
                        _session.Reset();
                        break;
                    }
                case EPropertyName.Enable:
                    {
                        _session.XSetEnable(_enableSession.GetValue());
                        break;
                    }
            }
#endif
        }

        private DirtyString dirtyString = new DirtyString();

        /// <summary>
        /// 输出友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString() => dirtyString.GetData(_ToFriendlyString);

        private string _ToFriendlyString()
        {
#if XDREAMER_AR_FOUNDATION
            switch (_propertyName)
            {
                case EPropertyName.Reset:
                    {
                        return CommonFun.Name(_propertyName);
                    }
                case EPropertyName.Enable:
                    {
                        return CommonFun.Name(_propertyName) + "=" + _enableSession.ToFriendlyString();
                    }
                default: return CommonFun.Name(_propertyName);
            }
#else
            return XARFoundationHelper.Title + "功能未启用！";
#endif
        }

        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity()
        {
#if XDREAMER_AR_FOUNDATION
            if (!_session) return false;
            switch (_propertyName)
            {
                case EPropertyName.Reset: return true;
                case EPropertyName.Enable: return _enableSession.DataValidity();
            }
#endif
            return false;
        }

        /// <summary>
        /// 验证
        /// </summary>
        public void OnValidate()
        {
            dirtyString.SetDirty();
        }
    }
}
