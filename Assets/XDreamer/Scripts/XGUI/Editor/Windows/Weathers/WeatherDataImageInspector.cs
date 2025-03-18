using UnityEditor;
using XCSJ.Attributes;
using XCSJ.EditorTools.Base;
using XCSJ.EditorTools.PropertyDatas;
using XCSJ.PluginXGUI.Windows.Weathers;

namespace XCSJ.EditorXGUI.Windows.Weathers
{
    /// <summary>
    /// 天气数据图像检查器
    /// </summary>
    [Name("天气数据图像检查器")]
    [CustomEditor(typeof(WeatherDataImage))]
    public class WeatherDataImageInspector : InteractProviderInspector<WeatherDataImage>
    {

    }
}
