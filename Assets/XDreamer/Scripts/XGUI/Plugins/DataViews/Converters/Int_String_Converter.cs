using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginXGUI.Base;
using XCSJ.PluginXGUI.DataViews.Base;

namespace XCSJ.PluginXGUI.DataViews.Converters
{
    /// <summary>
    /// 整型与字符串转换器
    /// </summary>
    [Name("整型与字符串转换器")]
    [Tool(XGUICategory.Data, nameof(BaseDataConverter))]
    public class Int_String_Converter : BaseDataConverter, IConverter<int, string>, IConverter<string, int>
    {
        [Serializable]
        public class MapData0 : MapData<int, string> { }

        [Serializable]
        public class MapData1 : MapData<string, int> { }

        [Name("整型到字符串映射列表")]
        public List<MapData0> int_String_Map = new List<MapData0>();

        [Name("字符串到整型映射列表")]
        public List<MapData1> string_Int_Map = new List<MapData1>();

        public override bool TryConvertTo(object input, Type outputType, out object output)
        {
            if (input is int i && outputType == typeof(string))
            {
                var data = int_String_Map.FirstOrDefault(d=>d.inputValue == i);
                if (data != null)
                {
                    output = data.outputValue;
                    return true;    
                }
            }
            else if(input is string s && outputType == typeof(int))
            {
                var data = string_Int_Map.FirstOrDefault(d => d.inputValue == s);
                if (data != null)
                {
                    output = data.outputValue;
                    return true;
                }
            }
            return base.TryConvertTo(input, outputType, out output);
        }
    }

    [Name("映射数据")]
    [Serializable]
    public class MapData<TInput,TOutput>
    {
        [Name("输入值")]
        public TInput inputValue;

        [Name("输出值")]
        public TOutput outputValue;
    }
}
