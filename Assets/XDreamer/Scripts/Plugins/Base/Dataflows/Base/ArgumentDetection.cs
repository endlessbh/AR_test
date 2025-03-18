using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Interfaces;
using XCSJ.PluginCommonUtils;

namespace XCSJ.Extension.Base.Dataflows.Base
{
    /// <summary>
    /// 实参检测:用于检测形参值(传入的参数值)与实参值(当前对象的存储值）是否符合检测规则
    /// </summary>
    [Serializable]
    [Name("实参检测")]
    [Tip("用于检测形参值(传入的参数值)与实参值(当前对象的存储值）是否符合检测规则", "It is used to detect whether the formal parameter value (the passed in parameter value) and the actual parameter value (the stored value of the current object) comply with the detection rules")]
    public sealed class ArgumentDetection: IToFriendlyString
    {
        /// <summary>
        /// 实参
        /// </summary>
        [Name("实参")]
        public Argument _argument = new Argument();

        /// <summary>
        /// 检测规则:对形参值(传入的参数值)与实参值(当前对象的存储值）执行检测的检测规则
        /// </summary>
        [Name("检测规则")]
        [Tip("对形参值(传入的参数值)与实参值(当前对象的存储值）执行检测的检测规则", "Detection rules for detecting formal parameter values (passed in parameter values) and actual parameter values (stored values of the current object)")]
        [EnumPopup]
        public EDetectionRule _detectionRule = EDetectionRule.Equal;

        /// <summary>
        /// 检测顺序:明确形参值(传入的参数值)与实参值(当前对象的存储值）在检测规则中的左值与右值的对应关系
        /// </summary>
        [Name("检测顺序")]
        [Tip("明确形参值(传入的参数值)与实参值(当前对象的存储值）在检测规则中的左值与右值的对应关系", "Specify the correspondence between the left and right values of the formal parameter value (the passed in parameter value) and the actual parameter value (the stored value of the current object) in the detection rule")]
        [EnumPopup]
        public EDetectionOrder _detectionOrder = EDetectionOrder.Parameter_Argument;

        /// <summary>
        /// 索引:待检测的形参索引
        /// </summary>
        [Name("索引")]
        [Tip("待检测的形参索引", "Parameter index to be detected")]
        [Range(0, 7)]
        public int _index = 0;

        /// <summary>
        /// 检查形参值(传入的参数值)与实参值(当前对象的存储值）是否符合检测规则
        /// </summary>
        /// <param name="value">形参值(传入的参数值)</param>
        /// <returns></returns>
        public bool Check(object value)
        {
            switch (_detectionOrder)
            {
                case EDetectionOrder.Parameter_Argument: return _detectionRule.Check(value, this._argument.value);
                case EDetectionOrder.Argument_Parameter: return _detectionRule.Check(this._argument.value, value);
                default: return false;
            }
        }

        /// <summary>
        /// 输出友好字符串
        /// </summary>
        /// <returns></returns>
        public string ToFriendlyString()
        {
            switch (_detectionOrder)
            {
                case EDetectionOrder.Parameter_Argument: return string.Format("{0} {1}", _detectionRule.ToAbbreviations(), _argument.ToFriendlyString());
                case EDetectionOrder.Argument_Parameter: return string.Format("{0} {1}", _argument.ToFriendlyString(), _detectionRule.ToAbbreviations());
                default: return "";
            }
        }
    }

    /// <summary>
    /// 检测顺序
    /// </summary>
    [Name("检测顺序")]
    public enum EDetectionOrder
    {
        /// <summary>
        /// 形参-实参:将形参与实参根据检测规则做检测，形参为左值,实参为右值
        /// </summary>
        [Name("形参-实参")]
        [Tip("将形参与实参根据检测规则做检测，形参为左值,实参为右值", "Detect the shape participating arguments according to the detection rules. The formal parameters are left-hand values and the arguments are right-hand values")]
        Parameter_Argument,

        /// <summary>
        /// 实参-形参:将实参与形参根据检测规则做检测，实参为左值,形参为右值
        /// </summary>
        [Name("实参-形参")]
        [Tip("将实参与形参根据检测规则做检测，实参为左值,形参为右值", "Detect the real participating formal parameters according to the detection rules. The real parameters are lvalues and the formal parameters are lvalues")]
        Argument_Parameter,
    }
}
