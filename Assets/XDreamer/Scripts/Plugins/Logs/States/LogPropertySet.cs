using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.Extension.Logs.States
{
    /// <summary>
    /// 日志属性设置
    /// </summary>
    [Name(Title, nameof(LogPropertySet))]
    [RequireManager(typeof(LogManager))]
    [Owner(typeof(LogManager))]
    public class LogPropertySet : BasePropertySet<LogPropertySet>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "日志属性设置";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [Name(Title, nameof(LogPropertySet))]
#if UNITY_EDITOR && XDREAMER_EDITION_DEVELOPER
        [StateLib("其它", typeof(LogManager))]
        [StateComponentMenu("其它/" + Title, typeof(LogManager), categoryIndex = IndexAttribute.DefaultIndex + 1)]
#endif
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

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
            /// 设置可见性
            /// </summary>
            [Name("设置可见性")]
            [Tip("设置日志查看器窗口的可见性", "Set the visibility of the log viewer window")]
            SetVisable,

            /// <summary>
            /// 清空:清空日志查看器窗口的所有信息
            /// </summary>
            [Name("清空")]
            [Tip("清空日志查看器窗口的所有信息", "Clear all information in the log viewer window")]
            Clear,

            /// <summary>
            /// 设置全屏:设置日志查看器窗口全屏
            /// </summary>
            [Name("设置全屏")]
            [Tip("设置日志查看器窗口全屏", "Set log viewer window full screen")]
            SetFullScreen,
        }

        /// <summary>
        /// 设置可见性:设置日志窗口的可见性
        /// </summary>
        [Name("设置可见性")]
        [Tip("设置日志窗口的可见性", "Set the visibility of the log window")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.SetVisable)]
        [OnlyMemberElements]
        public EBoolPropertyValue _SetVisable = new EBoolPropertyValue();

        /// <summary>
        /// 设置全屏属性值
        /// </summary>
        [Serializable]
        [Name("设置全屏属性值")]
        public class SetFullScreenPropertyValue : EBoolPropertyValue
        {
            Rect lastRect = new Rect();
            bool isFull = false;

            /// <summary>
            /// 设置
            /// </summary>
            public void Set()
            {
                var newIsFull = CommonFun.BoolChange(isFull, GetValue());
                if (newIsFull)
                {
                    if (!isFull)
                    {
                        lastRect = LogViewerWindow.instance.rect;
                    }
                    LogViewerWindow.instance.rect = new Rect(0, 0, Screen.width, Screen.height);
                }
                else
                {
                    if(isFull)
                    {
                        LogViewerWindow.instance.rect = lastRect;
                    }
                }
                isFull = newIsFull;
            }
        }

        /// <summary>
        /// 设置全屏:设置日志查看器窗口全屏
        /// </summary>
        [Name("设置全屏")]
        [Tip("设置日志查看器窗口全屏", "Set log viewer window full screen")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.SetFullScreen)]
        [OnlyMemberElements]
        public SetFullScreenPropertyValue _SetFullScreen = new SetFullScreenPropertyValue();

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="stateData"></param>
        /// <param name="executeMode"></param>
        public override void Execute(StateData stateData, EExecuteMode executeMode)
        {
            switch(_propertyName)
            {
                case EPropertyName.SetVisable:
                    {
                        LogViewerWindow.SetVisable(_SetVisable.GetValue());
                        break;
                    }
                case EPropertyName.Clear:
                    {
                        LogViewerWindow.instance.Clear();
                        break;
                    }
                case EPropertyName.SetFullScreen:
                    {
                        _SetFullScreen.Set();
                        break;
                    }
            }
        }

        private DirtyString dirtyString = new DirtyString();

        /// <summary>
        /// 输出友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString() => dirtyString.GetData(_ToFriendlyString);

        private string _ToFriendlyString()
        {
            switch (_propertyName)
            {
                case EPropertyName.SetVisable:
                    {
                        return CommonFun.Name(_propertyName) + "=" + _SetVisable.ToFriendlyString();
                    }
                case EPropertyName.Clear:
                    {
                        return CommonFun.Name(_propertyName) ;
                    }
                case EPropertyName.SetFullScreen:
                    {
                        return CommonFun.Name(_propertyName) + "=" + _SetFullScreen.ToFriendlyString();
                    }
                default: return CommonFun.Name(_propertyName);
            }
        }

        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity()
        {
            switch (_propertyName)
            {
                case EPropertyName.SetVisable: return _SetVisable.DataValidity();
                case EPropertyName.SetFullScreen: return _SetFullScreen.DataValidity();
                case EPropertyName.Clear: return true;
            }
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
