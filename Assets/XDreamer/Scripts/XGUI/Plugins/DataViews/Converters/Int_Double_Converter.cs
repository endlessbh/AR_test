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
    /// 整型与双精度浮点数转换器
    /// </summary>
    [Name("整型与双精度浮点数转换器")]
    [Tool(XGUICategory.Data, nameof(BaseDataConverter))]
    public class Int_Double_Converter : BaseDataConverter, IConverter<int, double>, IConverter<double, int>
    {
        [Serializable]
        public class MapData0 : MapData<int, double> { }

        [Serializable]
        public class MapData1 : MapData<double, int> { }

        [Name("整型到双精度浮点数映射列表")]
        public List<MapData0> int_Double_Map = new List<MapData0>();

        [Name("双精度浮点数到整型映射列表")]
        public List<MapData1> double_Int_Map = new List<MapData1>();

        public override bool TryConvertTo(object input, Type outputType, out object output)
        {
            if (input is int i && outputType == typeof(double))
            {
                var data = int_Double_Map.FirstOrDefault(d => d.inputValue == i);
                if (data != null)
                {
                    output = data.outputValue;
                    return true;
                }
            }
            else if (input is double s && outputType == typeof(int))
            {
                var data = double_Int_Map.FirstOrDefault(d => d.inputValue == s);
                if (data != null)
                {
                    output = data.outputValue;
                    return true;
                }
            }
            return base.TryConvertTo(input, outputType, out output);
        }
    }
}
