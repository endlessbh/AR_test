using System.Collections.Generic;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Caches;
using XCSJ.Extension.Base.Maths;

namespace XCSJ.Extension.Base.Units
{
    /// <summary>
    /// 角度单位
    /// </summary>
    [Name("角度单位")]
    public enum EAngleUnit
    {
        [Name("度")]
        [Unit("°")]
        Degrees = 0,

        [Name("弧度")]
        Radian,
    }

    /// <summary>
    /// 角度单位扩展
    /// </summary>
    public static class AngleUnitExtension
    {
        /// <summary>
        /// 单位缓存
        /// </summary>
        public class UnitCache : TICache<UnitCache, EAngleUnit, string>
        {
            protected override KeyValuePair<bool, string> CreateValue(EAngleUnit key1)
            {
                return new KeyValuePair<bool, string>(true, AttributeCache<UnitAttribute>.Get(EnumFieldInfoCache.GetFieldInfo(key1))?.unit ?? "");
            }
        }

        /// <summary>
        /// 由from单位转换到to单位
        /// </summary>
        /// <param name="from"></param>
        /// <param name="fromUnit"></param>
        /// <param name="toUnit"></param>
        /// <returns></returns>
        public static double ConvetTo(this double from, EAngleUnit fromUnit, EAngleUnit toUnit)
        {
            return ConvetFromDefault(ConvetToDefault(from, fromUnit), toUnit);
        }

        /// <summary>
        /// 由from单位转换到默认单位
        /// </summary>
        /// <param name="from"></param>
        /// <param name="fromUnit"></param>
        /// <returns></returns>
        public static double ConvetToDefault(this double from, EAngleUnit fromUnit)
        {
            switch (fromUnit)
            {
                case EAngleUnit.Radian: return from * Mathf.Rad2Deg;
                default: return from;
            }
        }

        /// <summary>
        /// 由默认单位转换到to单位
        /// </summary>
        /// <param name="from"></param>
        /// <param name="toUnit"></param>
        /// <returns></returns>
        public static double ConvetFromDefault(this double from, EAngleUnit toUnit)
        {
            switch (toUnit)
            {
                case EAngleUnit.Radian: return from * Mathf.Deg2Rad;
                default: return from;
            }
        }

        /// <summary>
        /// 获取单位
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        public static string Unit(this EAngleUnit unit) => UnitCache.GetCacheValue(unit, "");

        /// <summary>
        /// 转字符串
        /// </summary>
        /// <param name="from"></param>
        /// <param name="fromUnit"></param>
        /// <param name="toUnit"></param>
        /// <param name="decimalPlaces"></param>
        /// <param name="displayUnit"></param>
        /// <returns></returns>
        public static string ToString(this double from, EAngleUnit fromUnit, EAngleUnit toUnit, int decimalPlaces, bool displayUnit = true)
        {
            var value = ConvetTo(from, fromUnit, toUnit);
            return value.ToString(decimalPlaces.ToDecimalFormat()) + (displayUnit ? toUnit.Unit() : "");
        }
    }
}
