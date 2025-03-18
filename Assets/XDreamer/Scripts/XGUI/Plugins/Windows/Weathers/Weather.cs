using System;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Extension.GenericStandards.Managers;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Base.Kernel;
using XCSJ.PluginTools;

namespace XCSJ.PluginXGUI.Windows.Weathers
{
    /// <summary>
    /// 天气：用于获取天气数据的核心组件，使用本组件必须已连接有效互联网；
    /// </summary>
    [Name("天气")]
    [Tip("用于获取天气数据的核心组件，使用本组件必须已连接有效互联网；", "The core component used to obtain weather data, which must be connected to the effective Internet;")]
    [DisallowMultipleComponent]
    [RequireManager(typeof(XGUIManager))]
    public class Weather : InteractProvider
    {
        /// <summary>
        /// URL：期望获取天气数据的网络URL路径
        /// </summary>
        [Name("URL")]
        [Tip("期望获取天气数据的网络URL路径", "The network URL path where the weather data is expected to be obtained")]
        [ValidityCheck(EValidityCheckType.NotNullOrEmpty)]
        public string url = "http://t.weather.itboy.net/api/weather/city/";

        [Name("城市代码")]
        [Tip("获取天气数据使用的城市代码", "City code used to obtain weather data")]
        [ValidityCheck(EValidityCheckType.NotNullOrEmpty)]
        public string cityCode = "";

        /// <summary>
        /// URI
        /// </summary>
        public string uri => url + cityCode;

        private WeatherData Default => Default<WeatherData>.Instance;

        private WeatherData _weatherData = null;

        /// <summary>
        /// 天气数据
        /// </summary>
        public WeatherData weatherData
        {
            get
            {
                if (_weatherData == null)
                {
                    UpdateWeather();
                }
                if (_weatherData == Default) return null;
                return _weatherData;
            }
        }

        /// <summary>
        /// 判断天气数据是否有效
        /// </summary>
        public bool dataValid => _weatherData != null && _weatherData != Default;

        /// <summary>
        /// 天气更新后回调
        /// </summary>
        public Action<Weather> onWeatherUpdated;

        /// <summary>
        /// 获取天气
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="onSuccess"></param>
        /// <param name="onError"></param>
        public static void Get(string uri, Action<WeatherData> onSuccess, Action onError = null)
        {
            WebDataManager.Request(uri, EDataType.Text, null, (data, tag) =>
            {
                if (JsonHelper.ToObject<WeatherData>(data.dataInfo.text) is WeatherData weatherData)
                {
                    onSuccess?.Invoke(weatherData);
                }
                else
                {
                    onError?.Invoke();
                }
            }, (data, tag, e) => onError?.Invoke(), null);
        }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            UpdateWeatheAgain();
        }

        /// <summary>
        /// 根据需要更新天气
        /// </summary>
        /// <param name="callbackEventIfValid">如果天气数据有效时，执行回调</param>
        public void UpdateWeatherIfNeed(bool callbackEventIfValid)
        {
            if (_weatherData == null)
            {
                UpdateWeather();
                return;
            }
            if (_weatherData == Default) return;

            if (callbackEventIfValid)
            {
                onWeatherUpdated?.Invoke(this);
            }
        }

        /// <summary>
        /// 重新更新天气
        /// </summary>
        public void UpdateWeatheAgain()
        {
            _weatherData = null;
            UpdateWeather();
        }

        /// <summary>
        /// 更新天气
        /// </summary>
        public void UpdateWeather()
        {
            if (_weatherData != null) return;
            _weatherData = Default;
            Get(uri, wd =>
            {
                _weatherData = wd;
                onWeatherUpdated?.Invoke(this);
            }, () => { _weatherData = null; });
        }

        /// <summary>
        /// 尝试获取数据
        /// </summary>
        /// <param name="weatherDataType"></param>
        /// <param name="forcastIndex"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool TryGetData(EWeatherDataType weatherDataType, int forcastIndex, out string data)
        {
            if (dataValid)
            {
                data = weatherData?.GetData(weatherDataType, forcastIndex) ?? "";
                return true;
            }
            data = null;
            return false;
        }
    }
}