using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.CNScripts;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginXGUI.Base;
using XCSJ.PluginXGUI.DataViews.Base;

namespace XCSJ.PluginXGUI.DataViews.Converters
{
    /// <summary>
    /// 中文脚本转换器
    /// </summary>
    [Name("中文脚本转换器")]
    [XCSJ.Attributes.Icon(EIcon.CNScript)]
    [Tool(XGUICategory.Data, nameof(BaseDataConverter))]
    public class CNScriptConverter : BaseDataConverter, IConverter<object, object>
    {
        public enum EFunctionType
        {
            [Name("用户自定义函数")]
            UserDefineFun,

            [Name("本地函数")]
            LocalFunction,
        }

        /// <summary>
        /// 函数类型
        /// </summary>
        [Name("函数类型")]
        [EnumPopup]
        public EFunctionType _functionType = EFunctionType.LocalFunction;

        /// <summary>
        /// 用户自定义转换函数
        /// </summary>
        [Name("用户自定义转换函数")]
        [UserDefineFun]
        [HideInSuperInspector(nameof(_functionType), EValidityCheckType.NotEqual, EFunctionType.UserDefineFun)]
        public string _cnScriptFunction;

        /// <summary>
        /// 本地转换函数
        /// </summary>
        [Name("本地转换函数")]
        [HideInSuperInspector(nameof(_functionType), EValidityCheckType.NotEqual, EFunctionType.LocalFunction)]
        public CustomFunction _localFunction = new CustomFunction();

        private ReturnValue CallFunction(string inValue)
        {
            try
            {
                switch (_functionType)
                {
                    case EFunctionType.UserDefineFun: return ScriptManager.instance.ExecuteFunction(_cnScriptFunction, inValue);
                    case EFunctionType.LocalFunction: return ScriptManager.instance.ExecuteFunction(_localFunction, inValue);
                }
            }
            catch(Exception e)
            {
                Debug.LogException(e);
            }
            return ReturnValue.No;
        }

        public override bool TryConvertTo(object input, Type outputType, out object output)
        {
            if (input == null)
            {
                return base.TryConvertTo(input, outputType, out output);
            }
            var result = ReturnValue.No;
            if (input is string str)
            {
                result = CallFunction(str);
            }
            else if (Converter.instance.TryConvertTo(input, typeof(string), out var strValue))
            {
                result = CallFunction(strValue as string);
            }

            if (!result.valid) return base.TryConvertTo(input, outputType, out output);

            return base.TryConvertTo(result.Value<string>(), outputType, out output);
        }
    }
}
