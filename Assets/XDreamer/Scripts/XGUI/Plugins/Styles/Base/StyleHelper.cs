using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginXGUI.Styles.Base
{
    /// <summary>
    /// 样式助手
    /// </summary>
    public static class StyleHelper
    {
        #region 样式元素连接组件

        /// <summary>
        /// 是否存在组件对应的样式元素
        /// </summary>
        /// <returns></returns>
        public static bool ExistLinkStyleElement(object obj)
        {
            return StyleLinkAttribute.GetStyleElementType(obj.GetType()) != null;
        }

        /// <summary>
        /// 获取样式关联类型列表
        /// </summary>
        public static List<Type> linkStyleTypes
        {
            get
            {
                if (_linkStyleTypes == null)
                {
                    _linkStyleTypes = new List<Type>();
                    var tmp = TypeHelper.GetTypesHasAttribute<BaseStyleElement, StyleLinkAttribute>(true);
                    foreach (var t in tmp)
                    {
                        var att = AttributeHelper.GetAttribute<StyleLinkAttribute>(t);
                        if (att != null && !_linkStyleTypes.Contains(att.type))
                        {
                            _linkStyleTypes.Add(att.type);
                        }
                    }
                }
                return _linkStyleTypes;
            }
        }
        private static List<Type> _linkStyleTypes = null;

        /// <summary>
        /// 获取参数设定枚举类型
        /// </summary>
        /// <returns></returns>
        public static Type GetParamsSettingEnumType(Type elementType)
        {
            return AttributeHelper.GetAttribute<StyleLinkAttribute>(elementType)?.paramsSettingEnumType;
        } 

        #endregion

        #region 基础样式

        /// <summary>
        /// 基础样式
        /// </summary>
        public static List<Type> baseStyleElementTypes
        {
            get
            {
                if (_baseStyleElementTypes.Count == 0)
                {
                    _baseStyleElementTypes = styleElementTypes.Where(s => !typeof(IStyleElementCollection).IsAssignableFrom(s)).ToList();
                }
                return _baseStyleElementTypes;
            }
        }
        private static List<Type> _baseStyleElementTypes = new List<Type>();


        /// <summary>
        /// 基础样式名称数组
        /// </summary>
        public static string[] baseStyleElementTypeNames
        {
            get
            {
                if (_baseStyleElementTypeNames.Length == 0)
                {
                    _baseStyleElementTypeNames = baseStyleElementTypes.Cast(t => CommonFun.Name(t)).ToArray();
                }
                return _baseStyleElementTypeNames;
            }
        }
        private static string[] _baseStyleElementTypeNames = new string[0];

        #endregion

        #region 组合样式

        /// <summary>
        /// 组合样式 : 排除根类型
        /// </summary>
        public static List<Type> styleElementCollectionTypesWithoutRootType
        {
            get
            {
                if (_styleElementCollectionTypesWithoutRootType.Count == 0)
                {
                    _styleElementCollectionTypesWithoutRootType = styleElementTypes.Where(s => typeof(IStyleElementCollection).IsAssignableFrom(s) && !typeof(IStyleRoot).IsAssignableFrom(s)).ToList();
                }
                return _styleElementCollectionTypesWithoutRootType;
            }
        }
        private static List<Type> _styleElementCollectionTypesWithoutRootType = new List<Type>();


        /// <summary>
        /// 组合样式名称数组
        /// </summary>
        public static string[] styleElementCollectionNamesWithoutRootName
        {
            get
            {
                if (_styleElementCollectionNamesWithoutRootName.Length == 0)
                {
                    _styleElementCollectionNamesWithoutRootName = styleElementCollectionTypesWithoutRootType.Cast(t => CommonFun.Name(t)).ToArray();
                }
                return _styleElementCollectionNamesWithoutRootName;
            }
        }
        private static string[] _styleElementCollectionNamesWithoutRootName = new string[0];

        #endregion

        #region 除根类所有样式

        /// <summary>
        /// 除根类所有样式
        /// </summary>
        public static List<Type> styleElementTypesWithoutRootType
        {
            get
            {
                if (_styleElementTypesWithoutRootType.Count == 0)
                {
                    _styleElementTypesWithoutRootType = styleElementTypes.Where(s => !typeof(IStyleRoot).IsAssignableFrom(s)).ToList();
                }
                return _styleElementTypesWithoutRootType;
            }
        }
        private static List<Type> _styleElementTypesWithoutRootType = new List<Type>();


        /// <summary>
        /// 除根类所有样式名称数组
        /// </summary>
        public static string[] styleElementNamesWithoutRootName
        {
            get
            {
                if (_styleElementNamesWithoutRootName.Length == 0)
                {
                    _styleElementNamesWithoutRootName = styleElementTypesWithoutRootType.Cast(t => CommonFun.Name(t)).ToArray();
                }
                return _styleElementNamesWithoutRootName;
            }
        }
        private static string[] _styleElementNamesWithoutRootName = new string[0];

        #endregion

        #region 所有样式

        /// <summary>
        /// 所有样式
        /// </summary>
        public static List<Type> styleElementTypes
        {
            get
            {
                if (_styleElementTypes.Count == 0)
                {
                    _styleElementTypes = TypeHelper.FindTypeInAppWithClass(typeof(BaseStyleElement));
                }
                return _styleElementTypes;
            }
        }
        private static List<Type> _styleElementTypes = new List<Type>();

        /// <summary>
        /// 获取样式元素类型
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Type GetStyleElementType(string name)
        {
            return styleElementTypes.Find(t => CommonFun.Name(t) == name);
        }

        /// <summary>
        /// 所有样式名称数组
        /// </summary>
        public static string[] styleElementTypeNames
        {
            get
            {
                if (_styleElementTypeNames.Length == 0)
                {
                    _styleElementTypeNames = styleElementTypes.Cast(t => CommonFun.Name(t)).ToArray();
                }
                return _styleElementTypeNames;
            }
        }
        private static string[] _styleElementTypeNames = new string[0];

        #endregion

        /// <summary>
        /// 通过枚举与传入整数与结果，修改对象属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="mask"></param>
        /// <param name="enumType"></param>
        /// <param name="onMatchMask"></param>
        /// <returns></returns>
        public static bool ModifyObjectPropertyWithMask<T>(T obj, Type enumType, int mask, Action<T, Enum> onMatchMask) where T : UnityEngine.Object
        {
            bool rs = false;

            if (obj is T t && t)
            {
                t.XModifyProperty(() => EnumHelper.MaskEnum(enumType, mask, eObj =>
                {
                    onMatchMask.Invoke(t, eObj);
                    rs = true;
                }));
            }
            return rs;
        }

    }
}
