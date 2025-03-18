using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCSJ.Attributes;
using XCSJ.Caches;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginXGUI.Base;
using XCSJ.PluginXGUI.DataViews.Base;

namespace XCSJ.PluginXGUI.DataViews.Converters
{
    /// <summary>
    /// 枚举与字符串转换器
    /// </summary>
    [Name("枚举与字符串转换器")]
    [Tool(XGUICategory.Data, nameof(BaseDataConverter))]
    public class Enum_String_Converter : BaseDataConverter, IConverter<Enum, string>, IConverter<string, Enum>
    {
        /// <summary>
        /// 枚举字符串类型
        /// </summary>
        [Name("枚举字符串类型")]
        [EnumPopup]
        public EEnumStringType _enumStringType = EEnumStringType.Default;

        public override bool TryConvertTo(object input, Type outputType, out object output)
        {
            if (input is Enum e && outputType == typeof(string))
            {
                output = EnumStringCache.Get(e, _enumStringType);
                return true;
            }
            else if (input is string s && outputType.IsEnum)
            {
                try
                {
                    output = EnumValueCache.Get(outputType, s, _enumStringType);
                    if (output != null)
                    {
                        return true;
                    }
                }
                catch
                {
                    
                }
            }
            return base.TryConvertTo(input, outputType, out output);
        }
    }
}
