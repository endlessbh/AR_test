using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCSJ.Attributes;
using XCSJ.EditorSMS.Inspectors;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.EditorSMS.States.Base
{
    /// <summary>
    /// 触发器检查器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class TriggerInspector<T> : StateComponentInspector<T> where T : Trigger<T>
    {
    }
}
