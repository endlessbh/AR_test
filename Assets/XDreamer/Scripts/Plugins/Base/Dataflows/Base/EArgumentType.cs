using System;
using System.Collections.Generic;
using UnityEngine.Events;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Caches;
using XCSJ.Extension.Base.Dataflows.Binders;

namespace XCSJ.Extension.Base.Dataflows.Base
{
    /// <summary>
    /// 实参类型枚举
    /// </summary>
    [Name("实参类型")]
    public enum EArgumentType
    {
        /// <summary>
        /// 空
        /// </summary>
        [Name("空")]
        [ArgumentType(typeof(void))]
        [ArgumentValueFieldName]
        Void,

        /// <summary>
        /// 变量:中文脚本中的全局变量
        /// </summary>
        [Name("变量")]
        [Tip("中文脚本中的全局变量", "Global variables in Chinese scripts")]
        [ArgumentType(typeof(string))]
        [ArgumentValueFieldName(nameof(Argument._variableName))]
        Variable,

        /// <summary>
        /// Unity对象
        /// </summary>
        [Name("Unity对象")]
        [ArgumentType(typeof(UnityEngine.Object))]
        [ArgumentValueFieldName(nameof(Argument._objectValue))]
        UnityObject,

        /// <summary>
        /// 布尔型
        /// </summary>
        [Name("布尔型")]
        [ArgumentType(typeof(bool))]
        [ArgumentValueFieldName(nameof(Argument._boolValue))]
        Bool,

        /// <summary>
        /// 整形
        /// </summary>
        [Name("整形")]
        [ArgumentType(typeof(int))]
        [ArgumentValueFieldName(nameof(Argument._intValue))]
        Int,

        /// <summary>
        /// 长整型
        /// </summary>
        [Name("长整型")]
        [ArgumentType(typeof(long))]
        [ArgumentValueFieldName(nameof(Argument._longValue))]
        Long,

        /// <summary>
        /// 浮点型
        /// </summary>
        [Name("浮点型")]
        [ArgumentType(typeof(float))]
        [ArgumentValueFieldName(nameof(Argument._floatValue))]
        Float,

        /// <summary>
        /// 双精度浮点型
        /// </summary>
        [Name("双精度浮点型")]
        [ArgumentType(typeof(double))]
        [ArgumentValueFieldName(nameof(Argument._doubleValue))]
        Double,

        /// <summary>
        /// 字符串
        /// </summary>
        [Name("字符串")]
        [ArgumentType(typeof(string))]
        [ArgumentValueFieldName(nameof(Argument._stringValue))]
        String,

        /// <summary>
        /// 枚举长整型
        /// </summary>
        [Name("枚举长整型")]
        [ArgumentType(typeof(long))]
        [ArgumentValueFieldName(nameof(Argument._enumLongValue))]
        EnumLong,

        /// <summary>
        /// 枚举字符串
        /// </summary>
        [Name("枚举字符串")]
        [ArgumentType(typeof(string))]
        [ArgumentValueFieldName(nameof(Argument._enumStringValue))]
        EnumString,

        /// <summary>
        /// 变量字符串
        /// </summary>
        [Name("变量字符串")]
        [ArgumentType(typeof(string))]
        [ArgumentValueFieldName(nameof(Argument._varStringValue))]
        VarString,

        /// <summary>
        /// 颜色
        /// </summary>
        [Name("颜色")]
        [ArgumentType(typeof(UnityEngine.Color))]
        [ArgumentValueFieldName(nameof(Argument._colorValue))]
        Color,

        /// <summary>
        /// 二维向量
        /// </summary>
        [Name("二维向量")]
        [ArgumentType(typeof(UnityEngine.Vector2))]
        [ArgumentValueFieldName(nameof(Argument._vector2Value))]
        Vector2,

        /// <summary>
        /// 三维向量
        /// </summary>
        [Name("三维向量")]
        [ArgumentType(typeof(UnityEngine.Vector3))]
        [ArgumentValueFieldName(nameof(Argument._vector3Value))]
        Vector3,

        /// <summary>
        /// 四维向量
        /// </summary>
        [Name("四维向量")]
        [ArgumentType(typeof(UnityEngine.Vector4))]
        [ArgumentValueFieldName(nameof(Argument._vector4Value))]
        Vector4,

        /// <summary>
        /// 矩形
        /// </summary>
        [Name("矩形")]
        [ArgumentType(typeof(UnityEngine.Rect))]
        [ArgumentValueFieldName(nameof(Argument._rectValue))]
        Rect,

        /// <summary>
        /// 动画曲线
        /// </summary>
        [Name("动画曲线")]
        [ArgumentType(typeof(UnityEngine.AnimationCurve))]
        [ArgumentValueFieldName(nameof(Argument._animationCurveValue))]
        AnimationCurve,

        /// <summary>
        /// 包围盒
        /// </summary>
        [Name("包围盒")]
        [ArgumentType(typeof(UnityEngine.Bounds))]
        [ArgumentValueFieldName(nameof(Argument._boundsValue))]
        Bounds,

        /// <summary>
        /// 渐变色
        /// </summary>
        [Name("渐变色")]
        [ArgumentType(typeof(UnityEngine.Gradient))]
        [ArgumentValueFieldName(nameof(Argument._gradientValue))]
        Gradient,

        /// <summary>
        /// 四元数
        /// </summary>
        [Name("四元数")]
        [ArgumentType(typeof(UnityEngine.Quaternion))]
        [ArgumentValueFieldName(nameof(Argument._quaternionValue))]
        Quaternion,

        /// <summary>
        /// 二维向量整型
        /// </summary>
        [Name("二维向量整型")]
        [ArgumentType(typeof(UnityEngine.Vector2Int))]
        [ArgumentValueFieldName(nameof(Argument._vector2IntValue))]
        Vector2Int,

        /// <summary>
        /// 三维向量整型
        /// </summary>
        [Name("三维向量整型")]
        [ArgumentType(typeof(UnityEngine.Vector3Int))]
        [ArgumentValueFieldName(nameof(Argument._vector3IntValue))]
        Vector3Int,

        /// <summary>
        /// 矩形整型
        /// </summary>
        [Name("矩形整型")]
        [ArgumentType(typeof(UnityEngine.RectInt))]
        [ArgumentValueFieldName(nameof(Argument._rectIntValue))]
        RectInt,

        /// <summary>
        /// 包围盒整型
        /// </summary>
        [Name("包围盒整型")]
        [ArgumentType(typeof(UnityEngine.BoundsInt))]
        [ArgumentValueFieldName(nameof(Argument._boundsIntValue))]
        BoundsInt,

        /// <summary>
        /// 颜色32
        /// </summary>
        [Name("颜色32")]
        [ArgumentType(typeof(UnityEngine.Color32))]
        [ArgumentValueFieldName(nameof(Argument._color32Value))]
        Color32,

        /// <summary>
        /// 字段属性方法绑定器
        /// </summary>
        [Name("字段属性方法绑定器")]
        [ArgumentType(typeof(FieldPropertyMethodBinder))]
        [ArgumentValueFieldName(nameof(Argument._fieldPropertyMethodBinderValue))]
        FieldPropertyMethodBinder,

        /// <summary>
        /// 表达式字符串
        /// </summary>
        [Name("表达式字符串")]
        [ArgumentType(typeof(string))]
        [ArgumentValueFieldName(nameof(Argument._expressionStringValue))]
        ExpressionString,

        /// <summary>
        /// 绑定器别名
        /// </summary>
        [Name("绑定器别名")]
        [ArgumentType(typeof(string))]
        [ArgumentValueFieldName(nameof(Argument._binderAliasValue))]
        BinderAlias,
    }

    /// <summary>
    /// 实参类型特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class ArgumentTypeAttribute : Attribute
    {
        /// <summary>
        /// 类型
        /// </summary>
        public Type type { get; private set; } = null;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="type"></param>
        public ArgumentTypeAttribute(Type type)
        {
            this.type = type ?? throw new ArgumentNullException(nameof(type));
        }
    }

    /// <summary>
    /// 实参值字段名特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class ArgumentValueFieldNameAttribute : Attribute
    {
        /// <summary>
        /// 字段名
        /// </summary>
        public string fieldName { get; private set; }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="fieldName"></param>
        public ArgumentValueFieldNameAttribute(string fieldName = null)
        {
            this.fieldName = fieldName;
        }

        class Cache : TICache<Cache, EArgumentType, string>
        {
            protected override KeyValuePair<bool, string> CreateValue(EArgumentType key1)
            {
                return new KeyValuePair<bool, string>(true, AttributeCache<ArgumentValueFieldNameAttribute>.GetOfField(key1) is ArgumentValueFieldNameAttribute attribute ? attribute.fieldName : default);
            }
        }

        /// <summary>
        /// 获取实参值字段名
        /// </summary>
        /// <param name="argumentType"></param>
        /// <returns></returns>
        public static string GetArgumentValueFieldName(EArgumentType argumentType) => Cache.GetCacheValue(argumentType);
    }
}
