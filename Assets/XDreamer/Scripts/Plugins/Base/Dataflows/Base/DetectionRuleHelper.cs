using System;
using System.Collections;
using XCSJ.Algorithms;
using XCSJ.Extension.Base.Attributes;
using XCSJ.Extension.Base.Helpers;

namespace XCSJ.Extension.Base.Dataflows.Base
{
    /// <summary>
    /// 检测规则辅助类
    /// </summary>
    public static class DetectionRuleHelper
    {
        /// <summary>
        /// 检查左值与右值是否符合检测规则
        /// </summary>
        /// <param name="detectionRule"></param>
        /// <param name="leftValue"></param>
        /// <param name="rightValue"></param>
        /// <returns></returns>
        public static bool Check(this EDetectionRule detectionRule, object leftValue, object rightValue)
        {
            return Check(leftValue, detectionRule, rightValue);
        }

        /// <summary>
        /// 判断对象是否为null
        /// </summary>
        /// <param name="objectValue"></param>
        /// <returns></returns>
        private static bool ObjectIsNull(object objectValue) => ObjectHelper.ObjectIsNull(objectValue);

        /// <summary>
        /// 检查左值与右值是否符合检测规则
        /// </summary>
        /// <param name="leftValue"></param>
        /// <param name="detectionRule"></param>
        /// <param name="rightValue"></param>
        /// <returns></returns>
        public static bool Check(object leftValue, EDetectionRule detectionRule, object rightValue)
        {
            switch (detectionRule)
            {
                case EDetectionRule.None: return true;
                case EDetectionRule.True:
                    {
                        if (leftValue is bool b)
                        {
                            return b;
                        }
                        if (leftValue is UnityEngine.Object obj)
                        {
                            return obj;
                        }
                        break;
                    }
                case EDetectionRule.False: return !Check(leftValue, EDetectionRule.True, rightValue);
                case EDetectionRule.Default:
                    {
                        if (ObjectIsNull(leftValue)) return true;
                        if (!leftValue.GetType().IsValueType) return false;
                        return Converter.instance.ConvertTo<int>(leftValue, 1) == 0;
                    }
                case EDetectionRule.NotDefault: return !Check(leftValue, EDetectionRule.Default, rightValue);
                case EDetectionRule.Zero:
                    {
                        if (ObjectIsNull(leftValue)) return false;
                        if (!leftValue.GetType().IsValueType) return false;
                        return Converter.instance.ConvertTo<int>(leftValue, 1) == 0;
                    }
                case EDetectionRule.NotZero: return !Check(leftValue, EDetectionRule.Zero, rightValue);
                case EDetectionRule.Null:
                    {
                        return ObjectIsNull(leftValue);
                    }
                case EDetectionRule.NotNull: return !Check(leftValue, EDetectionRule.Null, rightValue);
                case EDetectionRule.NullOrEmpty:
                    {
                        if (ObjectIsNull(leftValue)) return true;
                        if (leftValue is string str) return string.IsNullOrEmpty(str);
                        if (leftValue is Array array) return array.Length == 0;
                        if (leftValue is IList list) return list.Count == 0;
                        return false;
                    }
                case EDetectionRule.NotNullOrEmpty: return !Check(leftValue, EDetectionRule.NullOrEmpty, rightValue);
                case EDetectionRule.Less:
                    {
                        if (leftValue == rightValue) return false;
                        if (ObjectIsNull(leftValue)) return true;
                        if (ObjectIsNull(rightValue)) return false;

                        if (leftValue is IComparable objectCompare)
                        {
                            return objectCompare.CompareTo(Converter.instance.ConvertTo(rightValue, objectCompare.GetType())) < 0;
                        }
                        return false;
                    }
                case EDetectionRule.LessEqual:
                    {
                        if (leftValue == rightValue) return true;
                        if (ObjectIsNull(leftValue)) return true;
                        if (ObjectIsNull(rightValue)) return false;

                        if (leftValue is IComparable objectCompare)
                        {
                            return objectCompare.CompareTo(Converter.instance.ConvertTo(rightValue, objectCompare.GetType())) <= 0;
                        }
                        return false;
                    }
                case EDetectionRule.Equal:
                    {
                        if (leftValue == rightValue) return true;

                        if (ObjectIsNull(leftValue)) return ObjectIsNull(rightValue);
                        if (ObjectIsNull(rightValue)) return false;

                        if (leftValue is IComparable objectCompare)
                        {
                            return objectCompare.CompareTo(Converter.instance.ConvertTo(rightValue, objectCompare.GetType())) == 0;
                        }

                        return Equals(leftValue, rightValue);
                    }
                case EDetectionRule.NotEqual: return !Check(leftValue, EDetectionRule.Equal, rightValue);
                case EDetectionRule.Greater: return !Check(leftValue, EDetectionRule.LessEqual, rightValue);
                case EDetectionRule.GreaterEqual: return !Check(leftValue, EDetectionRule.Less, rightValue);
                case EDetectionRule.ElementCountLess:
                    {
                        if (ObjectIsNull(leftValue)) return false;
                        if (!Converter.instance.TryConvertTo(rightValue, out int value)) return false;

                        if (leftValue is string str) return str.Length < value;
                        if (leftValue is Array array) return array.Length < value;
                        if (leftValue is IList list) return list.Count < value;

                        return false;
                    }
                case EDetectionRule.ElementCountGreater:
                    {
                        if (ObjectIsNull(leftValue)) return false;
                        if (!Converter.instance.TryConvertTo(rightValue, out int value)) return false;

                        if (leftValue is string str) return str.Length > value;
                        if (leftValue is Array array) return array.Length > value;
                        if (leftValue is IList list) return list.Count > value;

                        return false;
                    }
            }
            return false;
        }

        /// <summary>
        /// 检测规则简写
        /// </summary>
        /// <param name="detectionRule"></param>
        /// <returns></returns>
        public static string ToAbbreviations(this EDetectionRule detectionRule) => AbbreviationAttribute.GetAbbreviation(detectionRule);
    }
}
