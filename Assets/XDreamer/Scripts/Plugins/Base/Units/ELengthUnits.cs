using System.Collections.Generic;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Caches;
using XCSJ.Extension.Base.Maths;

namespace XCSJ.Extension.Base.Units
{
    /// <summary>
    /// 长度单位:长度单位是指丈量空间距离上的基本单元
    /// </summary>
    [Name("长度单位")]
    public enum ELengthUnits
    {
        [Name("米")]
        [Tip("默认长度基本单位", "Default length basic unit")]
        [Unit("m", 1)]
        M = 0,

        [Name("分米")]
        [Unit("dm", 0.1)]
        DM,

        [Name("厘米")]
        [Unit("cm", 0.01)]
        CM,

        [Name("毫米")]
        [Unit("mm", 0.001)]
        MM,

        [Name("千米")]
        [Unit("km", 1000)]
        KM,
    }

    /// <summary>
    /// 长度单位扩展
    /// </summary>
    public static class LengthUnitsExtension
    {
        /// <summary>
        /// 单位缓存值
        /// </summary>
        public class UnitsCacheValue : TIVCacheValue<UnitsCacheValue, ELengthUnits>
        {
            /// <summary>
            /// 单位
            /// </summary>
            public string unit { get; private set; } = "";

            /// <summary>
            /// 由当前1单位转为默认标准1单位（米）时的值；
            /// </summary>
            public double scaleToDefault { get; private set; } = 1;

            /// <summary>
            /// 由默认标准1单位（米）转为当前1单位时的值；
            /// </summary>
            public double scaleFromDefault { get; private set; } = 1;

            /// <summary>
            /// 初始化
            /// </summary>
            /// <returns></returns>
            public override bool Init()
            {
                var attribute = AttributeCache<UnitAttribute>.Get(EnumFieldInfoCache.GetFieldInfo(key1));
                if (attribute != null)
                {
                    unit = attribute.unit ?? "";
                    scaleToDefault = attribute.scale;
                    scaleFromDefault = 1 / scaleToDefault;
                }
                return true;
            }
        }

        /// <summary>
        /// 单位缓存
        /// </summary>
        public class UnitsCache : TIVCache<UnitsCache, ELengthUnits, UnitsCacheValue> { }

        /// <summary>
        /// 由from单位转换到to单位
        /// </summary>
        /// <param name="from"></param>
        /// <param name="fromUnit"></param>
        /// <param name="toUnit"></param>
        /// <returns></returns>
        public static double ConvetTo(this double from, ELengthUnits fromUnit, ELengthUnits toUnit)
        {
            return ConvetFromDefault(ConvetToDefault(from, fromUnit), toUnit);
        }

        /// <summary>
        /// 由from单位转换到默认单位
        /// </summary>
        /// <param name="from"></param>
        /// <param name="fromUnit"></param>
        /// <returns></returns>
        public static double ConvetToDefault(this double from, ELengthUnits fromUnit) => UnitsCache.GetCacheValue(fromUnit).scaleToDefault * from;

        /// <summary>
        /// 由默认单位转换到to单位
        /// </summary>
        /// <param name="from"></param>
        /// <param name="toUnit"></param>
        /// <returns></returns>
        public static double ConvetFromDefault(this double from, ELengthUnits toUnit) => UnitsCache.GetCacheValue(toUnit).scaleFromDefault * from;

        /// <summary>
        /// 获取单位
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        public static string Unit(this ELengthUnits unit) => UnitsCache.GetCacheValue(unit).unit;

        /// <summary>
        /// 转为默认的比例尺
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        public static double ScaleToDefault(this ELengthUnits unit) => UnitsCache.GetCacheValue(unit).scaleToDefault;

        /// <summary>
        /// 转为默认的比例尺
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        public static double ScaleFromDefault(this ELengthUnits unit) => UnitsCache.GetCacheValue(unit).scaleFromDefault;

#if UNITY_2018_1_OR_NEWER

        /// <summary>
        /// 转字符串
        /// </summary>
        /// <param name="from"></param>
        /// <param name="fromUnit"></param>
        /// <param name="toUnit"></param>
        /// <param name="decimalPlaces"></param>
        /// <param name="displayUnit"></param>
        /// <returns></returns>
        public static string ToString(this double from, ELengthUnits fromUnit, ELengthUnits toUnit, int decimalPlaces, bool displayUnit = true)
        {
            var value = ConvetTo(from, fromUnit, toUnit);
            return value.ToString(decimalPlaces.ToDecimalFormat()) + (displayUnit ? toUnit.Unit() : "");
        }

#endif
    }
}
