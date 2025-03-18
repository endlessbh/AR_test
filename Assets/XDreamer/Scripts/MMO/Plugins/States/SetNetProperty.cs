using System.Linq;
using UnityEngine.Serialization;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Extension.Base.Attributes;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginMMO.NetSyncs;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.PluginMMO.States
{
    /// <summary>
    /// 设置网络属性的值；设置之后会执行网络同步，本地到网络；
    /// </summary>
    [XCSJ.Attributes.Icon(EIcon.Property)]
    [ComponentMenu(MMOHelper.CategoryName + "/" + Title, typeof(MMOManager))]
    [Name(Title, nameof(SetNetProperty))]
    [Tip("设置网络属性的值；设置之后会执行网络同步，本地到网络；", "Set the value of network attribute; After setting, network synchronization will be performed, local to network;")]
    [RequireManager(typeof(MMOManager))]
    [Owner(typeof(MMOManager))]
    public class SetNetProperty : LifecycleExecutor<SetNetProperty>, IDropdownPopupAttribute
    {
        public const string Title = "设置网络属性";

        [StateLib(MMOHelper.CategoryName, typeof(MMOManager))]
        [StateComponentMenu(MMOHelper.CategoryName + "/" + Title, typeof(MMOManager))]
        [Name(Title, nameof(SetNetProperty))]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        [Tip("设置网络属性的值；设置之后会执行网络同步，本地到网络；", "Set the value of network attribute; After setting, network synchronization will be performed, local to network;")]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        [Name("网络属性")]
        [ComponentPopup]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [FormerlySerializedAs(nameof(netProperty))]
        public NetProperty _netProperty;

        /// <summary>
        /// 网络属性
        /// </summary>
        public NetProperty netProperty { get => _netProperty; set => _netProperty = value; }

        [Name("属性名")]
        [Tip("期望设置属性的名称", "The name of the property you want to set")]
        [NetPropertyName]
        [FormerlySerializedAs(nameof(propertyName))]
        public string _propertyName;

        /// <summary>
        /// 属性名
        /// </summary>
        public string propertyName { get => _propertyName; set => _propertyName = value; }

        /// <summary>
        /// 尝试状态切换
        /// </summary>
        [Name("尝试状态切换")]
        [Tip("尝试状态切换；会将属性进行状态切换操作；即 0 与 1、#True 与 #False、True 与 False 、Yes 与 No 、Y 与 N 字符串信息互相切换；如果属性为空字符串或为不可作为状态状态的则使用输入的属性值进行设置；", "Try state switching; It will switch the attribute state; That is, 0 and 1, #True and #False, True and False, Yes and No, Y and N string information are switched with each other; If the attribute is an empty string or cannot be used as a status, set it with the entered attribute value;")]
        public bool _tryStateSwitch = false;

        /// <summary>
        /// 强制设置
        /// </summary>
        [Name("强制设置")]
        [Tip("新值与旧值相同时，是否强制设置网络属性", "When the new value is the same as the old value, whether to force the setting of network properties")]
        [FormerlySerializedAs(nameof(mustSet))]
        public bool _mustSet = false;

        /// <summary>
        /// 强制设置
        /// </summary>
        public bool mustSet { get => _mustSet; set => _mustSet = value; }

        /// <summary>
        /// 属性值
        /// </summary>
        [Name("属性值")]
        public Argument _propertyValue = new Argument();

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="stateData"></param>
        /// <param name="executeMode"></param>
        public override void Execute(StateData stateData, EExecuteMode executeMode)
        {
            if (netProperty)
            {
                netProperty.SetProperty(propertyName, _tryStateSwitch ? SwitchState() : _propertyValue.GetValueToString(), mustSet);
            }
        }

        private string SwitchState()
        {
            if (netProperty.GetProperty(propertyName) is Property property)
            {
                string rs = property.value;
                switch (rs)
                {
                    case "0": rs = "1"; break;
                    case "1": rs = "0"; break;
                    case "#True": rs = "#False"; break;
                    case "#False": rs = "#True"; break;
                    case "True": rs = "False"; break;
                    case "False": rs = "True"; break;
                    case "Yes": rs = "No"; break;
                    case "No": rs = "Yes"; break;
                    case "Y": rs = "N"; break;
                    case "N": rs = "Y"; break;
                    default: rs = _propertyValue.GetValueToString(); break;
                }
                return rs;
            }
            return "";
        }

        /// <summary>
        /// 完成
        /// </summary>
        /// <returns></returns>
        public override bool Finished() => true;

        /// <summary>
        /// 转友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString()
        {
            return propertyName + ".属性值" + (_tryStateSwitch ? ":状态切换" : (VariableCompareHelper.ToAbbreviations(ECompareOperator.Equal) + _propertyValue.ToFriendlyString()));
        }

        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity()
        {
            return base.DataValidity() && netProperty;
        }

        bool IDropdownPopupAttribute.TryGetOptions(string purpose, string propertyPath, out string[] options)
        {
            switch (purpose)
            {
                case nameof(NetPropertyNameAttribute):
                    {
                        options = netProperty ? netProperty.propertys.Cast(p => p.name).ToArray() : Empty<string>.Array;
                        return true;
                    }
            }
            options = default;
            return false;
        }

        bool IDropdownPopupAttribute.TryGetOption(string purpose, string propertyPath, string[] options, object propertyValue, out string option)
        {
            switch (purpose)
            {
                case nameof(NetPropertyNameAttribute):
                    {
                        option = (propertyValue as string) ?? "";
                        return true;
                    }
            }
            option = default;
            return false;
        }

        bool IDropdownPopupAttribute.TryGetPropertyValue(string purpose, string propertyPath, string[] options, string option, out object propertyValue)
        {
            switch (purpose)
            {
                case nameof(NetPropertyNameAttribute):
                    {
                        propertyValue = option;
                        return true;
                    }
            }
            propertyValue = default;
            return false;
        }
    }
}
