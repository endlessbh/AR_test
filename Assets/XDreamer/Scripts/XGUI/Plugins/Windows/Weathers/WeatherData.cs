using System;
using System.Collections.Generic;
using XCSJ.Attributes;
using XCSJ.LitJson;

namespace XCSJ.PluginXGUI.Windows.Weathers
{
    [Serializable]
    [Import]
    [Name("天气数据")]
    public class WeatherData
    {
        public string message;
        public int status;
        public string date;
        public string time;
        public CityInfo cityInfo;
        public Data data;
    }

    [Serializable]
    [Import]
    [Name("城市信息")]
    public class CityInfo
    {
        public string city;
        public string citykey;
        public string parent;
        public string updateTime;
    }
    [Serializable]
    [Import]
    [Name("数据")]
    public class Data
    {
        public string shidu;
        public float pm25;
        public float pm10;
        public string quality;
        public string wendu;
        public string ganmao;
        public List<Forecast> forecast;
        public Forecast yesterday;
    }

    [Serializable]
    [Import]
    [Name("预报")]
    public class Forecast
    {
        public string date;
        public string high;
        public string low;
        public string ymd;
        public string week;
        public string sunrise;
        public string sunset;
        public int aqi;
        public string fx;
        public string fl;
        public string type;
        public string notice;
    }
}
