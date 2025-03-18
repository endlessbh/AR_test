using UnityEditor;
using XCSJ.Attributes;
using XCSJ.EditorTools.Base;
using XCSJ.EditorTools.PropertyDatas;
using XCSJ.PluginXGUI.Windows.Weathers;

namespace XCSJ.EditorXGUI.Windows.Weathers
{
    /// <summary>
    /// 天气数据文本检查器
    /// </summary>
    [Name("天气数据文本检查器")]
    [CustomEditor(typeof(WeatherDataText))]
    public class WeatherDataTextInspector : InteractProviderInspector<WeatherDataText>
    {

    }
}
