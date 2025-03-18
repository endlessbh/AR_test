using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.PluginSMS.States.Components
{
    [Serializable]
    [ComponentMenu(SMSHelperExtension.CommonUseCategoryName + "/" + Title, typeof(SMSManager))]
    [Name(Title, nameof(ComponentSet))]
    [Tip(Title, "Component set")]
    [XCSJ.Attributes.Icon(EIcon.Component)]
    [DisallowMultipleComponent]
    public class ComponentSet : ObjectSet<ComponentSet, UnityEngine.Component>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "组件集合";

        [Name(Title, nameof(ComponentSet))]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        [Tip(Title, "Component set")]
        public static void Create(IGetStateCollection obj) => CreateNormalState(obj);
    }
}
