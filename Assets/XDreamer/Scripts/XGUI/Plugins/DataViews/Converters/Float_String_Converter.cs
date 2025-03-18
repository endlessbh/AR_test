using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginXGUI.Base;
using XCSJ.PluginXGUI.DataViews.Base;

namespace XCSJ.PluginXGUI.DataViews.Converters
{
    /// <summary>
    /// 浮点数与字符串转换器
    /// </summary>
    [Name("浮点数与字符串转换器")]
    [Tool(XGUICategory.Data, nameof(BaseDataConverter))]
    public class Float_String_Converter : BaseDataConverter, IConverter<float, string>, IConverter<string, float>
    {
        public enum EFloatToStringRule
        {
            [Name("默认")]
            Default,

            [Name("转字符串F")]
            ToStringF,

            [Name("映射")]
            Map,
        }

        //浮点数到字符串规则
        [Name("浮点数到字符串规则")]
        [EnumPopup]    
        public EFloatToStringRule _floatToStringRule = EFloatToStringRule.ToStringF;

        [Serializable]
        public class MapData0 : MapData<float, string> { }

        [Serializable]
        public class MapData1 : MapData<string, float> { }

        [Name("浮点数到字符串映射列表")]
        public List<MapData0> float_String_Map = new List<MapData0>();

        [Name("字符串到浮点数映射列表")]
        public List<MapData1> string_float_Map = new List<MapData1>();

        public override bool TryConvertTo(object input, Type outputType, out object output)
        {
            if (input is float i && outputType == typeof(string))
            {
                //将浮点数转为字符串时问题,如：将 0.0000000123456 浮点数默认转为字符串为 1.23456E-08 科学计数法类型
                //UGUI InputField.text在处理科学计数法类型浮点数时，会将1.23456E-08中E-当作非法字符做处理，所以转为浮点数转为字符串时，不可使用科学计数法！！！
                switch (_floatToStringRule)
                {
                    case EFloatToStringRule.Default:
                        {
                            output = i.ToString();
                            return true;
                        }
                    case EFloatToStringRule.ToStringF:
                        {
                            output = Mathf.Abs(i) < 0.00001 ? i.ToString("F") : i.ToString();
                            return true;
                        }
                    case EFloatToStringRule.Map:
                        {
                            var data = float_String_Map.FirstOrDefault(d => Mathf.Approximately(d.inputValue, i));
                            if (data != null)
                            {
                                output = data.outputValue;
                                return true;
                            }
                            break;
                        }
                }
            }
            else if (input is string s && outputType == typeof(float))
            {
                var data = string_float_Map.FirstOrDefault(d => d.inputValue == s);
                if (data != null)
                {
                    output = data.outputValue;
                    return true;
                }
            }
            return base.TryConvertTo(input, outputType, out output);
        }

        public void Reset()
        {
            var view = GetComponent<BaseModelView>();
            if (view)
            {
                if (!view._modelToViewConverter) view.XModifyProperty(ref view._modelToViewConverter, this);
                if (!view._viewToModelConverter) view.XModifyProperty(ref view._viewToModelConverter, this);
            }
        }
    }
}
