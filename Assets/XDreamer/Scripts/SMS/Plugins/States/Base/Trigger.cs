using XCSJ.Attributes;
using XCSJ.PluginSMS.Kernel;

namespace XCSJ.PluginSMS.States.Base
{
    /// <summary>
    /// 触发器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Name("触发器")]
    [KeyNode(nameof(ITrigger), "触发器")]
    [XCSJ.Attributes.Icon(EIcon.Trigger)]
    public abstract class Trigger<T> : StateComponent<T>, ITrigger
        where T : Trigger<T>
    {

    }
}
