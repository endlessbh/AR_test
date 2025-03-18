using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCSJ.Algorithms;
using XCSJ.Caches;

namespace XCSJ.Extension.Base.Dataflows.Base
{
    /// <summary>
    /// 实参类型辅助类
    /// </summary>
    public static class ArgumentTypeHelper
    {
        private static Dictionary<Type, bool> canHandleTyeps = new Dictionary<Type, bool>();

        /// <summary>
        /// 判断能否处理传入的类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool CanHandle(Type type)
        {
            if (type == null) return false;
            if (canHandleTyeps.TryGetValue(type, out var value)) return value;

            //枚举类型，可以处理
            if (type.IsEnum)
            {
                canHandleTyeps[type] = true;
                return true;
            }

            //有任意匹配的，可以处理
            if (EnumCache<EArgumentType>.Array.Any(argumentType => argumentType.Match(type)))
            {
                canHandleTyeps[type] = true;
                return true;
            }

            canHandleTyeps[type] = false;
            return false;
        }

        /// <summary>
        /// 判断实参类型能否处理传入的类型
        /// </summary>
        /// <param name="argumentType"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool CanHandle(this EArgumentType argumentType, Type type)
        {
            if (type == null) return false;

            //枚举类型，可以处理
            if (type.IsEnum) return true;

            //可匹配的，可以处理
            if (argumentType.Match(type)) return true;

            //字符串类型的实参类型，做特殊的处理与分析
            if (GetArgumentType(argumentType) == typeof(string))
            {
                return Converter.instance.CanConvert(typeof(string), type);
            }

            return false;
        }

        /// <summary>
        /// 检查实参类型与类型是否匹配
        /// </summary>
        /// <param name="type"></param>
        /// <param name="argumentType"></param>
        /// <returns></returns>
        public static bool Match(this EArgumentType argumentType, Type type)
        {
            if (type == null) return false;
            if (GetArgumentType(argumentType) is Type baseType)
            {
                return baseType.IsAssignableFrom(type);
            }
            return false;
        }

        /// <summary>
        /// 获取实参类型枚举对应的实参类型
        /// </summary>
        /// <param name="argumentType"></param>
        /// <returns></returns>
        public static Type GetArgumentType(this EArgumentType argumentType) => TypeCache.GetCacheValue(argumentType);

        internal class TypeCache : TICache<TypeCache, EArgumentType, Type>
        {
            protected override KeyValuePair<bool, Type> CreateValue(EArgumentType key1)
            {
                return new KeyValuePair<bool, Type>(true, AttributeCache<ArgumentTypeAttribute>.GetOfField(key1)?.type);
            }
        }
    }
}
