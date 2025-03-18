using XCSJ.Attributes;

namespace XCSJ.PluginXGUI.Windows.Weathers
{
    /// <summary>
    /// 天气数据类型
    /// </summary>
    [Name("天气数据类型")]
    public enum EWeatherDataType
    {
        [Name("信息")]
        message,
        [Name("状态值")]
        status,
        [Name("日期")]
        date,
        [Name("时间")]
        time,

        [Name("城市信息-城市")]
        cityInfo_city,
        [Name("城市信息-城市代码")]
        cityInfo_citykey,
        [Name("城市信息-省份")]
        cityInfo_parent,
        [Name("城市信息-更新时间")]
        cityInfo_updateTime,

        [Name("数据-湿度")]
        data_shidu,
        [Name("数据-PM2.5")]
        data_pm25,
        [Name("数据-PM10")]
        data_pm10,
        [Name("数据-空气质量")]
        data_quality,
        [Name("数据-温度")]
        data_wendu,
        [Name("数据-感冒指数")]
        data_ganmao,

        [Name("数据-预报-日期")]
        data_forcast_date,
        [Name("数据-预报-高温")]
        data_forcast_high,
        [Name("数据-预报-低温")]
        data_forcast_low,
        [Name("数据-预报-年月日")]
        data_forcast_ymd,
        [Name("数据-预报-星期")]
        data_forcast_week,
        [Name("数据-预报-日出")]
        data_forcast_sunrise,
        [Name("数据-预报-日落")]
        data_forcast_sunset,
        [Name("数据-预报-空气质量指数")]
        data_forcast_aqi,
        [Name("数据-预报-风向")]
        data_forcast_fx,
        [Name("数据-预报-风力")]
        data_forcast_fl,
        [Name("数据-预报-天气类型")]
        data_forcast_type,
        [Name("数据-预报-提示")]
        data_forcast_notice,

        [Name("数据-昨天-日期")]
        data_yesterday_date,
        [Name("数据-昨天-高温")]
        data_yesterday_high,
        [Name("数据-昨天-低温")]
        data_yesterday_low,
        [Name("数据-昨天-年月日")]
        data_yesterday_ymd,
        [Name("数据-昨天-星期")]
        data_yesterday_week,
        [Name("数据-昨天-日出")]
        data_yesterday_sunrise,
        [Name("数据-昨天-日落")]
        data_yesterday_sunset,
        [Name("数据-昨天-空气质量指数")]
        data_yesterday_aqi,
        [Name("数据-昨天-风向")]
        data_yesterday_fx,
        [Name("数据-昨天-风力")]
        data_yesterday_fl,
        [Name("数据-昨天-天气类型")]
        data_yesterday_type,
        [Name("数据-昨天-提示")]
        data_yesterday_notice,
    }

    public static class WeatherDataHelper
    {
        public static string GetData(this WeatherData weatherData, EWeatherDataType weatherDataType, int forcastIndex = 0)
        {
            if (weatherData == null) return "";
            switch (weatherDataType)
            {
                case EWeatherDataType.message: return weatherData.message;
                case EWeatherDataType.status: return weatherData.status.ToString();
                case EWeatherDataType.date: return weatherData.date;
                case EWeatherDataType.time: return weatherData.time;
                case EWeatherDataType.cityInfo_city: return weatherData.cityInfo.city;
                case EWeatherDataType.cityInfo_citykey: return weatherData.cityInfo.citykey;
                case EWeatherDataType.cityInfo_parent: return weatherData.cityInfo.parent;
                case EWeatherDataType.cityInfo_updateTime: return weatherData.cityInfo.updateTime;
                case EWeatherDataType.data_shidu: return weatherData.data.shidu;
                case EWeatherDataType.data_pm25: return weatherData.data.pm25.ToString();
                case EWeatherDataType.data_pm10: return weatherData.data.pm10.ToString();
                case EWeatherDataType.data_quality: return weatherData.data.quality;
                case EWeatherDataType.data_wendu: return weatherData.data.wendu;
                case EWeatherDataType.data_ganmao: return weatherData.data.ganmao;
                case EWeatherDataType.data_forcast_date: return weatherData.data.forecast[forcastIndex].date;
                case EWeatherDataType.data_forcast_high: return weatherData.data.forecast[forcastIndex].high;
                case EWeatherDataType.data_forcast_low: return weatherData.data.forecast[forcastIndex].low;
                case EWeatherDataType.data_forcast_ymd: return weatherData.data.forecast[forcastIndex].ymd;
                case EWeatherDataType.data_forcast_week: return weatherData.data.forecast[forcastIndex].week;
                case EWeatherDataType.data_forcast_sunrise: return weatherData.data.forecast[forcastIndex].sunrise;
                case EWeatherDataType.data_forcast_sunset: return weatherData.data.forecast[forcastIndex].sunset;
                case EWeatherDataType.data_forcast_aqi: return weatherData.data.forecast[forcastIndex].aqi.ToString();
                case EWeatherDataType.data_forcast_fx: return weatherData.data.forecast[forcastIndex].fx;
                case EWeatherDataType.data_forcast_fl: return weatherData.data.forecast[forcastIndex].fl;
                case EWeatherDataType.data_forcast_type: return weatherData.data.forecast[forcastIndex].type;
                case EWeatherDataType.data_forcast_notice: return weatherData.data.forecast[forcastIndex].notice;
                case EWeatherDataType.data_yesterday_date: return weatherData.data.yesterday.date;
                case EWeatherDataType.data_yesterday_high: return weatherData.data.yesterday.high;
                case EWeatherDataType.data_yesterday_low: return weatherData.data.yesterday.low;
                case EWeatherDataType.data_yesterday_ymd: return weatherData.data.yesterday.ymd;
                case EWeatherDataType.data_yesterday_week: return weatherData.data.yesterday.week;
                case EWeatherDataType.data_yesterday_sunrise: return weatherData.data.yesterday.sunrise;
                case EWeatherDataType.data_yesterday_sunset: return weatherData.data.yesterday.sunset;
                case EWeatherDataType.data_yesterday_aqi: return weatherData.data.yesterday.aqi.ToString();
                case EWeatherDataType.data_yesterday_fx: return weatherData.data.yesterday.fx;
                case EWeatherDataType.data_yesterday_fl: return weatherData.data.yesterday.fl;
                case EWeatherDataType.data_yesterday_type: return weatherData.data.yesterday.type;
                case EWeatherDataType.data_yesterday_notice: return weatherData.data.yesterday.notice;
                default:return "";
            }
        }
    }

}
