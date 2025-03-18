using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCSJ.PluginSMS.States.Dataflows
{
    /// <summary>
    /// 数据流助手
    /// </summary>
    public class DataflowHelper
    {
        /// <summary>
        /// 名称
        /// </summary>
        public const string Name = "数据流-";

        /// <summary>
        /// 数据模型
        /// </summary>
        public const string DataModel = Name + "数据模型";

        /// <summary>
        /// 事件
        /// </summary>
        public const string Events = Name + "事件";

        /// <summary>
        /// 字段绑定
        /// </summary>
        public const string FieldBinds = Name + "字段绑定";

        /// <summary>
        /// 属性绑定
        /// </summary>
        public const string PropertyBinds = Name + "属性绑定";
    }
}
