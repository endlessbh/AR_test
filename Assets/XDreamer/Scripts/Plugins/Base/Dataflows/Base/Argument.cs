using System;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Attributes;
using XCSJ.Extension.Base.Dataflows.Binders;
using XCSJ.Extension.Base.Helpers;
using XCSJ.Interfaces;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.Scripts;

namespace XCSJ.Extension.Base.Dataflows.Base
{
    /// <summary>
    /// 实参：用于存储多种不同类型参数的容器类（包括中文脚本变量）；如期望仅存储单一类型参数与中文脚本变量，请参考属性值<see cref="PropertyValue{T}"/>
    /// </summary>
    [Serializable]
    [LanguageFileOutput]
    [Name("实参")]
    public sealed class Argument : IToFriendlyString
    {
        /// <summary>
        /// 实参类型
        /// </summary>
        [Name("实参类型")]
        [EnumPopup]
        public EArgumentType _argumentType = EArgumentType.String;

        /// <summary>
        /// 全局变量名
        /// </summary>
        [Name("全局变量名")]
        [HideInSuperInspector(nameof(_argumentType), EValidityCheckType.NotEqual, EArgumentType.Variable)]
        [ValidityCheck(EValidityCheckType.NotNullOrEmpty)]
        [GlobalVariable]
        public string _variableName;

        /// <summary>
        /// 对象值
        /// </summary>
        [Name("对象值")]
        [HideInSuperInspector(nameof(_argumentType), EValidityCheckType.NotEqual, EArgumentType.UnityObject)]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ObjectPopup]
        public UnityEngine.Object _objectValue;

        /// <summary>
        /// Unity对象类型
        /// </summary>
        public enum EUnityObjectType
        {
            /// <summary>
            /// Unity对象
            /// </summary>
            [Name("Unity对象")]
            UnityObject = 0,

            /// <summary>
            /// Unity对象名称
            /// </summary>
            [Name("Unity对象名称")]
            UnityObjectName,

            /// <summary>
            /// Unity对象名称路径
            /// </summary>
            [Name("Unity对象名称路径")]
            UnityObjectNamePath,
        }

        /// <summary>
        /// Unity对象类型
        /// </summary>
        [Name("Unity对象类型")]
        [HideInSuperInspector(nameof(_argumentType), EValidityCheckType.NotEqual, EArgumentType.UnityObject)]
        [EnumPopup]
        public EUnityObjectType _unityObjectType = EUnityObjectType.UnityObject;

        /// <summary>
        /// 布尔值
        /// </summary>
        [Name("布尔值")]
        [HideInSuperInspector(nameof(_argumentType), EValidityCheckType.NotEqual, EArgumentType.Bool)]
        public bool _boolValue;

        /// <summary>
        /// 整形值
        /// </summary>
        [Name("整形值")]
        [HideInSuperInspector(nameof(_argumentType), EValidityCheckType.NotEqual, EArgumentType.Int)]
        public int _intValue;

        /// <summary>
        /// 长整形值
        /// </summary>
        [Name("长整形值")]
        [HideInSuperInspector(nameof(_argumentType), EValidityCheckType.NotEqual, EArgumentType.Long)]
        public long _longValue;

        /// <summary>
        /// 浮点值
        /// </summary>
        [Name("浮点值")]
        [HideInSuperInspector(nameof(_argumentType), EValidityCheckType.NotEqual, EArgumentType.Float)]
        public float _floatValue;

        /// <summary>
        /// 双精度浮点值
        /// </summary>
        [Name("双精度浮点值")]
        [HideInSuperInspector(nameof(_argumentType), EValidityCheckType.NotEqual, EArgumentType.Double)]
        public double _doubleValue;

        /// <summary>
        /// 字符串值
        /// </summary>
        [Name("字符串值")]
        [HideInSuperInspector(nameof(_argumentType), EValidityCheckType.NotEqual, EArgumentType.String)]
        public string _stringValue;

        /// <summary>
        /// 枚举长整型值
        /// </summary>
        [Name("枚举长整型值")]
        [HideInSuperInspector(nameof(_argumentType), EValidityCheckType.NotEqual, EArgumentType.EnumLong)]
        public long _enumLongValue;

        /// <summary>
        /// 枚举字符串值
        /// </summary>
        [Name("枚举字符串值")]
        [HideInSuperInspector(nameof(_argumentType), EValidityCheckType.NotEqual, EArgumentType.EnumString)]
        public string _enumStringValue;

        /// <summary>
        /// 变量字符串值
        /// </summary>
        [Name("变量字符串值")]
        [HideInSuperInspector(nameof(_argumentType), EValidityCheckType.NotEqual, EArgumentType.VarString)]
        [ValidityCheck(EValidityCheckType.NotNullOrEmpty)]
        [VarString(EVarStringHierarchyKeyMode.Get)]
        public string _varStringValue;

        /// <summary>
        /// 颜色值
        /// </summary>
        [Name("颜色值")]
        [HideInSuperInspector(nameof(_argumentType), EValidityCheckType.NotEqual, EArgumentType.Color)]
        public UnityEngine.Color _colorValue;

        /// <summary>
        /// 二维向量值
        /// </summary>
        [Name("二维向量值")]
        [HideInSuperInspector(nameof(_argumentType), EValidityCheckType.NotEqual, EArgumentType.Vector2)]
        public UnityEngine.Vector2 _vector2Value;

        /// <summary>
        /// 三维向量值
        /// </summary>
        [Name("三维向量值")]
        [HideInSuperInspector(nameof(_argumentType), EValidityCheckType.NotEqual, EArgumentType.Vector3)]
        public UnityEngine.Vector3 _vector3Value;

        /// <summary>
        /// 四维向量值
        /// </summary>
        [Name("四维向量值")]
        [HideInSuperInspector(nameof(_argumentType), EValidityCheckType.NotEqual, EArgumentType.Vector4)]
        public UnityEngine.Vector4 _vector4Value;

        /// <summary>
        /// 矩形值
        /// </summary>
        [Name("矩形值")]
        [HideInSuperInspector(nameof(_argumentType), EValidityCheckType.NotEqual, EArgumentType.Rect)]
        public UnityEngine.Rect _rectValue;

        /// <summary>
        /// 动画曲线值
        /// </summary>
        [Name("动画曲线值")]
        [HideInSuperInspector(nameof(_argumentType), EValidityCheckType.NotEqual, EArgumentType.AnimationCurve)]
        public UnityEngine.AnimationCurve _animationCurveValue;

        /// <summary>
        /// 包围盒值
        /// </summary>
        [Name("包围盒值")]
        [HideInSuperInspector(nameof(_argumentType), EValidityCheckType.NotEqual, EArgumentType.Bounds)]
        public UnityEngine.Bounds _boundsValue;

        /// <summary>
        /// 渐变色值
        /// </summary>
        [Name("渐变色值")]
        [HideInSuperInspector(nameof(_argumentType), EValidityCheckType.NotEqual, EArgumentType.Gradient)]
        public UnityEngine.Gradient _gradientValue;

        /// <summary>
        /// 四元数值
        /// </summary>
        [Name("四元数值")]
        [HideInSuperInspector(nameof(_argumentType), EValidityCheckType.NotEqual, EArgumentType.Quaternion)]
        public UnityEngine.Quaternion _quaternionValue;

        /// <summary>
        /// 二维向量整型值
        /// </summary>
        [Name("二维向量整型值")]
        [HideInSuperInspector(nameof(_argumentType), EValidityCheckType.NotEqual, EArgumentType.Vector2Int)]
        public UnityEngine.Vector2Int _vector2IntValue;

        /// <summary>
        /// 三维向量整型值
        /// </summary>
        [Name("三维向量整型值")]
        [HideInSuperInspector(nameof(_argumentType), EValidityCheckType.NotEqual, EArgumentType.Vector3Int)]
        public UnityEngine.Vector3Int _vector3IntValue;

        /// <summary>
        /// 矩形整型值
        /// </summary>
        [Name("矩形整型值")]
        [HideInSuperInspector(nameof(_argumentType), EValidityCheckType.NotEqual, EArgumentType.RectInt)]
        public UnityEngine.RectInt _rectIntValue;

        /// <summary>
        /// 包围盒整型值
        /// </summary>
        [Name("包围盒整型值")]
        [HideInSuperInspector(nameof(_argumentType), EValidityCheckType.NotEqual, EArgumentType.BoundsInt)]
        public UnityEngine.BoundsInt _boundsIntValue;

        /// <summary>
        /// 颜色32值
        /// </summary>
        [Name("颜色32值")]
        [HideInSuperInspector(nameof(_argumentType), EValidityCheckType.NotEqual, EArgumentType.Color32)]
        public UnityEngine.Color32 _color32Value;

        /// <summary>
        /// 字段属性方法绑定器
        /// </summary>
        [Name("字段属性方法绑定器")]
        [HideInSuperInspector(nameof(_argumentType), EValidityCheckType.NotEqual, EArgumentType.FieldPropertyMethodBinder)]
        public FieldPropertyMethodBinder _fieldPropertyMethodBinderValue = new FieldPropertyMethodBinder();

        /// <summary>
        /// 表达式字符串
        /// </summary>
        [Name("表达式字符串")]
        [HideInSuperInspector(nameof(_argumentType), EValidityCheckType.NotEqual, EArgumentType.ExpressionString)]
        [ValidityCheck(EValidityCheckType.NotNullOrEmpty)]
        public string _expressionStringValue;

        /// <summary>
        /// 绑定器别名值
        /// </summary>
        [Name("绑定器别名值")]
        [HideInSuperInspector(nameof(_argumentType), EValidityCheckType.NotEqual, EArgumentType.BinderAlias)]
        [ValidityCheck(EValidityCheckType.NotNullOrEmpty)]
        [XCSJ.Extension.Base.Dataflows.Binders.AliasAttribute(EAliasDataType.Get)]
        public string _binderAliasValue;

        /// <summary>
        /// 值
        /// </summary>
        public object value { get => GetValue(); set => SetValue(value); }

        /// <summary>
        /// 获取值
        /// </summary>
        /// <returns></returns>
        public object GetValue()
        {
            switch (_argumentType)
            {
                case EArgumentType.Variable: return ScriptManager.GetGlobalVariableValue(_variableName);
                case EArgumentType.VarString: return _varStringValue.TryGetHierarchyVarValue(out var varValue) ? varValue : default;
                case EArgumentType.UnityObject:
                    {
                        switch (_unityObjectType)
                        {
                            case EUnityObjectType.UnityObjectName: return _objectValue ? _objectValue.name : null;
                            case EUnityObjectType.UnityObjectNamePath: return CommonFun.ObjectToString(_objectValue);
                            default: return _objectValue;
                        }
                    }
                case EArgumentType.Bool: return _boolValue;
                case EArgumentType.Int: return _intValue;
                case EArgumentType.Long: return _longValue;
                case EArgumentType.Float: return _floatValue;
                case EArgumentType.Double: return _doubleValue;
                case EArgumentType.String: return _stringValue;
                case EArgumentType.EnumLong: return _enumLongValue;
                case EArgumentType.EnumString: return _enumStringValue;
                case EArgumentType.Color: return _colorValue;
                case EArgumentType.Vector2: return _vector2Value;
                case EArgumentType.Vector3: return _vector3Value;
                case EArgumentType.Vector4: return _vector4Value;
                case EArgumentType.Rect: return _rectValue;
                case EArgumentType.AnimationCurve: return _animationCurveValue;
                case EArgumentType.Bounds: return _boundsValue;
                case EArgumentType.Gradient: return _gradientValue;
                case EArgumentType.Quaternion: return _quaternionValue;
                case EArgumentType.Vector2Int: return _vector2IntValue;
                case EArgumentType.Vector3Int: return _vector3IntValue;
                case EArgumentType.RectInt: return _rectIntValue;
                case EArgumentType.BoundsInt: return _boundsIntValue;
                case EArgumentType.Color32: return _color32Value;
                case EArgumentType.FieldPropertyMethodBinder: return _fieldPropertyMethodBinderValue.GetMemberValue();
                case EArgumentType.ExpressionString: return _expressionStringValue.TryCalculateExpression(out var calculateResult) ? calculateResult : default;
                case EArgumentType.BinderAlias:return AliasCache.instance.GetTypeBinder(_binderAliasValue)?.entityObject;
                case EArgumentType.Void:
                default: return null;
            }
        }

        /// <summary>
        /// 获取值并转字符串
        /// </summary>
        /// <returns></returns>
        public string GetValueToString() => Converter.instance.TryConvertTo(GetValue(), out string output) ? output : default;

        bool inTrySet = false;

        /// <summary>
        /// 通过当前实参类型尝试设置值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TrySetValueByCurrentArgumentType(string value, out object dstValue)
        {
            if (!inTrySet)
            {
                try
                {
                    inTrySet = true;
                    switch (_argumentType)
                    {
                        case EArgumentType.Void: break;
                        case EArgumentType.UnityObject:
                            {
                                if (_objectValue)
                                {
                                    //前后对象一致，直接返回成功
                                    if (CommonFun.ObjectToString(_objectValue) == value)
                                    {
                                        dstValue = _objectValue;
                                        return true;
                                    }

                                    if (Converter.instance.TryConvertTo(value, _objectValue.GetType(), out dstValue) && dstValue != null)
                                    {
                                        _objectValue = dstValue as UnityEngine.Object;
                                        return true;
                                    }
                                }

                                //尝试转为Unity对象
                                if(ObjectHelper.TryConvertToUnityObject(value, out var unityObject))
                                {
                                    dstValue = _objectValue = unityObject;
                                    return true;
                                }

                                break;
                            }
                        case EArgumentType.FieldPropertyMethodBinder:
                            {
                                if(_fieldPropertyMethodBinderValue.SetMemberValue(value))
                                {
                                    dstValue = value;
                                    return true;
                                }
                                break;
                            }
                        default:
                            {
                                var type = _argumentType.GetArgumentType();
                                //能成功转为根原始值类型类型
                                if(type != null && Converter.instance.TryConvertTo(value, type, out dstValue))
                                {
                                    return SetValue(dstValue) != null;
                                }
                                break;
                            }
                    }
                }
                finally
                {
                    inTrySet = false;
                }
            }
            dstValue = default;
            return false;
        }

        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="value">值</param>
        /// <returns>值:如成功设置返回新值，否则返回null;</returns>
        public object SetValue(object value)
        {
            if (value == null) return default;
            if (value is UnityEngine.Object obj)
            {
                _argumentType = EArgumentType.UnityObject;
                _objectValue = obj;
            }
            else if (value is string str)
            {
                //优先转为当前目标类型
                if (TrySetValueByCurrentArgumentType(str, out var dstValue)) return dstValue;

                if (ExpressionStringAnalysisResult.TryParse(str, out var result0) && result0.hasMarker)
                {
                    _argumentType = EArgumentType.ExpressionString;
                    _expressionStringValue = result0.expressionString;
                }
                else if (VarStringAnalysisResult.TryParse(str, out var result))
                {
                    //针对变量字符串做特殊处理
                    if (result.varString.TryGetHierarchyVarValue(out var varValue))
                    {
                        return SetValue(varValue);
                    }

                    _argumentType = EArgumentType.VarString;
                    _varStringValue = result.varString;
                }
                else if(AliasCache.instance.GetTypeBinder(str) is ITypeMemberBinder)
                {
                    _argumentType = EArgumentType.BinderAlias;
                    _binderAliasValue = str;
                }
                else
                {
                    _argumentType = EArgumentType.String;
                    _stringValue = str;
                }
            }
            else if (value is int i)
            {
                _argumentType = EArgumentType.Int;
                _intValue = i;
            }
            else if (value is float f)
            {
                _argumentType = EArgumentType.Float;
                _floatValue = f;
            }
            else if (value is bool b)
            {
                _argumentType = EArgumentType.Bool;
                _boolValue = b;
            }
            else if (value is double d)
            {
                _argumentType = EArgumentType.Double;
                _doubleValue = d;
            }
            else if (value is long l)
            {
                _argumentType = EArgumentType.Long;
                _longValue = l;
            }
            else if (value is UnityEngine.Color c)
            {
                _argumentType = EArgumentType.Color;
                _colorValue = c;
            }
            else if (value is UnityEngine.Vector2 v2)
            {
                _argumentType = EArgumentType.Vector2;
                _vector2Value = v2;
            }
            else if (value is UnityEngine.Vector3 v3)
            {
                _argumentType = EArgumentType.Vector3;
                _vector3Value = v3;
            }
            else if (value is UnityEngine.Vector4 v4)
            {
                _argumentType = EArgumentType.Vector4;
                _vector4Value = v4;
            }
            else if (value is UnityEngine.Rect r)
            {
                _argumentType = EArgumentType.Rect;
                _rectValue = r;
            }
            else if (value is UnityEngine.AnimationCurve ac)
            {
                _argumentType = EArgumentType.AnimationCurve;
                _animationCurveValue = ac;
            }
            else if (value is UnityEngine.Bounds bo)
            {
                _argumentType = EArgumentType.Bounds;
                _boundsValue = bo;
            }
            else if (value is UnityEngine.Gradient g)
            {
                _argumentType = EArgumentType.Gradient;
                _gradientValue = g;
            }
            else if (value is UnityEngine.Quaternion q)
            {
                _argumentType = EArgumentType.Quaternion;
                _quaternionValue = q;
            }
            else if (value is UnityEngine.Vector2Int v2i)
            {
                _argumentType = EArgumentType.Vector2Int;
                _vector2IntValue = v2i;
            }
            else if (value is UnityEngine.Vector3Int v3i)
            {
                _argumentType = EArgumentType.Vector3Int;
                _vector3IntValue = v3i;
            }
            else if (value is UnityEngine.RectInt ri)
            {
                _argumentType = EArgumentType.RectInt;
                _rectIntValue = ri;
            }
            else if (value is UnityEngine.BoundsInt bi)
            {
                _argumentType = EArgumentType.BoundsInt;
                _boundsIntValue = bi;
            }
            else if (value is UnityEngine.Color32 c32)
            {
                _argumentType = EArgumentType.Color32;
                _color32Value = c32;
            }
            else if(_fieldPropertyMethodBinderValue.SetMemberValue(value))
            {
                _argumentType = EArgumentType.FieldPropertyMethodBinder;
            }
            else
            {
                Log.WarningFormat("实参无法处理{0}类型的数据！实参类型将重置为空！", value.GetType());
                _argumentType = EArgumentType.Void;
                return default;
            }
            return value;
        }

        /// <summary>
        /// 输出友好字符串
        /// </summary>
        /// <returns></returns>
        public string ToFriendlyString() => DefaultFriendlyString();

        /// <summary>
        /// 默认友好字符串
        /// </summary>
        /// <returns></returns>
        public string DefaultFriendlyString()
        {
            switch (_argumentType)
            {
                case EArgumentType.Variable: return ScriptOption.VarFlag + _variableName;
                case EArgumentType.VarString: return _varStringValue;
                case EArgumentType.FieldPropertyMethodBinder: return _fieldPropertyMethodBinderValue.ToFriendlyString();
                case EArgumentType.ExpressionString:return _expressionStringValue;
                case EArgumentType.BinderAlias:return _binderAliasValue;
                default: return CommonFun.ObjectToString(value) ?? "";
            }
        }

        /// <summary>
        /// 判断能否处理传入的类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool CanHandle(Type type) => ArgumentTypeHelper.CanHandle(type);
    }
}
