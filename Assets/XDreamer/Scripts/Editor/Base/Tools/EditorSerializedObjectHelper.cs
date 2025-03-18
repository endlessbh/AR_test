using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginCommonUtils;

namespace XCSJ.EditorExtension.Base.Tools
{
    /// <summary>
    /// 编辑器序列化对象组手
    /// </summary>
    public static class EditorSerializedObjectHelper
    {
        /// <summary>
        /// 处理规则:对数组（列表）中的元素进行处理的规则
        /// </summary>
        [Name("处理规则")]
        [Tip("对数组（列表）中的元素进行处理的规则", "Rules for processing elements in an array (list)")]
        public enum EArrayHandleRule
        {
            /// <summary>
            /// 名称排序：根据元素的名称对对象数组（列表）执行升序排序
            /// </summary>
            [Name("名称")]
            [Tip("根据元素的名称对对象数组（列表）执行升序排序", "Performs an ascending sort on the object array (list) based on the name of the element")]
            [XCSJ.Attributes.Icon(EIcon.NameAscendingOrder)]
            NameSort,

            /// <summary>
            /// 名称路径排序：根据元素的名称路径对对象数组（列表）执行升序排序
            /// </summary>
            [Name("名称路径")]
            [Tip("根据元素的名称路径对对象数组（列表）执行升序排序", "Performs an ascending sort on the object array (list) based on the name path of the element")]
            [XCSJ.Attributes.Icon(EIcon.NameAscendingOrder)]
            NamePathSort,

            /// <summary>
            /// 逆序：将对象数组（列表）中元素逆序
            /// </summary>
            [Name("逆序")]
            [Tip("将对象数组（列表）中元素逆序", "Invert the elements in the object array (list)")]
            [XCSJ.Attributes.Icon(EIcon.ReverseOrder)]
            Reverse,

            /// <summary>
            /// 无效：将对象数组（列表）中无效元素移除
            /// </summary>
            [Name("无效")]
            [Tip("将对象数组（列表）中无效元素移除", "Remove invalid elements from object array (list)")]
            [XCSJ.Attributes.Icon(EIcon.Delete)]
            DeleteInvalid,

            /// <summary>
            /// 去重：将对象数组（列表）中重复元素移除
            /// </summary>
            [Name("去重")]
            [Tip("将对象数组（列表）中重复元素移除", "Remove duplicate elements from the object array (list)")]
            [XCSJ.Attributes.Icon(EIcon.Delete)]
            Distinct,
        }

        /// <summary>
        /// 绘制数组处理规则
        /// </summary>
        /// <param name="arraySerializedProperty"></param>
        public static void DrawArrayHandleRule(this SerializedProperty arraySerializedProperty)
        {
            DrawArrayHandleRule(arraySerializedProperty, ArrayHandle);
        }

        /// <summary>
        /// 绘制数组处理规则
        /// </summary>
        /// <param name="arraySerializedProperty"></param>
        /// <param name="onClick"></param>
        private static void DrawArrayHandleRule(SerializedProperty arraySerializedProperty, Action<EArrayHandleRule, SerializedProperty> onClick)
        {
            UICommonFun.EnumButton<EArrayHandleRule>(sr => onClick?.Invoke(sr, arraySerializedProperty), true, false, null, null, null, null, ENameTip.Image, GUILayout.ExpandWidth(true), GUILayout.Height(20));
        }

        /// <summary>
        /// 数组处理
        /// </summary>
        /// <param name="sortRule"></param>
        /// <param name="arraySerializedProperty"></param>
        private static void ArrayHandle(EArrayHandleRule rule, SerializedProperty arraySerializedProperty)
        {
            switch (rule)
            {
                case EArrayHandleRule.NameSort:
                    {
                        SerializedObjectHelper.ArrayElementSort(arraySerializedProperty, (a, b) =>
                        {
                            if (a.serializedProperty.propertyType == SerializedPropertyType.ObjectReference)
                            {
                                var oa = a.serializedProperty.objectReferenceValue;
                                var ob = b.serializedProperty.objectReferenceValue;

                                if (!oa && !ob) return 0;
                                if (!oa) return -1;
                                if (!ob) return 1;

                                return UICommonFun.NaturalCompare(oa.name, ob.name);
                            }
                            else
                            {
                                var oa = a.serializedProperty.GetSerializedPropertyValue();
                                var ob = b.serializedProperty.GetSerializedPropertyValue();
                                if (oa == null && ob == null) return 0;
                                if (oa == null) return -1;
                                if (ob == null) return 1;

                                return UICommonFun.NaturalCompare(CommonFun.ObjectToString(oa), CommonFun.ObjectToString(ob));
                            }                            
                        });
                        break;
                    }
                case EArrayHandleRule.NamePathSort:
                    {
                        SerializedObjectHelper.ArrayElementSort(arraySerializedProperty, (a, b) =>
                        {
                            if (a.serializedProperty.propertyType == SerializedPropertyType.ObjectReference)
                            {
                                var oa = a.serializedProperty.objectReferenceValue;
                                var ob = b.serializedProperty.objectReferenceValue;

                                if (!oa && !ob) return 0;
                                if (!oa) return -1;
                                if (!ob) return 1;

                                return UICommonFun.NaturalCompare(CommonFun.ObjectToString(oa), CommonFun.ObjectToString(ob));
                            }
                            else
                            {
                                var oa = a.serializedProperty.GetSerializedPropertyValue();
                                var ob = b.serializedProperty.GetSerializedPropertyValue();
                                if (oa == null && ob == null) return 0;
                                if (oa == null) return -1;
                                if (ob == null) return 1;

                                return UICommonFun.NaturalCompare(CommonFun.ObjectToString(oa), CommonFun.ObjectToString(ob));
                            }                            
                        });
                        break;
                    }
                case EArrayHandleRule.Reverse:
                    {
                        SerializedObjectHelper.ArrayElementReverse(arraySerializedProperty);
                        break;
                    }
                case EArrayHandleRule.Distinct:
                    {
                        SerializedObjectHelper.ArrayElementDistinct(arraySerializedProperty, (x, y) =>
                        {
                            if (x.serializedProperty.propertyType == SerializedPropertyType.ObjectReference)
                            {
                                return x.serializedProperty.objectReferenceValue == y.serializedProperty.objectReferenceValue;
                            }
                            else
                            {
                                var oa = x.serializedProperty.GetSerializedPropertyValue();
                                var ob = y.serializedProperty.GetSerializedPropertyValue();
                                if (oa == null && ob == null) return true;
                                return oa != null ? oa.Equals(ob) : ob.Equals(oa);
                            }
                        });
                        break;
                    }
                case EArrayHandleRule.DeleteInvalid:
                    {
                        SerializedObjectHelper.DeleteArrayInvalidElements(arraySerializedProperty);
                        break;
                    }
            }
            GUI.FocusControl("");
        }
    }
}
