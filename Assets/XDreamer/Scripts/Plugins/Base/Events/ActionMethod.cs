using System;
using System.Linq;
using System.Reflection;
using XCSJ.Caches;
using XCSJ.Collections;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Helper;

namespace XCSJ.Extension.Base.Events
{
    /// <summary>
    /// Action方法辅助类
    /// </summary>
    public class ActionMethodHelper
    {
        /// <summary>
        /// 判断是否是Action方法类型的字段信息
        /// </summary>
        /// <param name="fieldInfo"></param>
        /// <returns></returns>
        public static bool IsActionMethodType(FieldInfo fieldInfo)
        {
            if (fieldInfo == null || ObsoleteAttributeCache.Exist(fieldInfo)) return false;

            return IsActionMethodType(fieldInfo.FieldType);
        }

        /// <summary>
        /// 判断是否是Action方法类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsActionMethodType(Type type)
        {
            if (type == null) return false;
            try
            {               
                return typeof(Delegate).IsAssignableFrom(type) && GetActionMethodDelegate(type) != null;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 获取与输入类型匹配的Action方法信息，即Action事件对应执行方法（Invoke）的方法信息
        /// </summary>
        /// <returns></returns>
        public static MethodInfo GetActionMethodInfo(Type type)
        {
            if (type == null) return null;
            return type.GetMethod("Invoke");
        }

        /// <summary>
        /// 获取与输入类型匹配的ActionMethodBase子类类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Type GetActionMethodType(Type type)
        {
            if (GetActionMethodInfo(type) is MethodInfo methodInfo && methodInfo.ReturnType == typeof(void))
            {
                var types = methodInfo.GetParameters().Cast(pi => pi.ParameterType).ToArray();
                switch (types.Length)
                {
                    case 0: return typeof(ActionMethod);
                    case 1: return typeof(ActionMethod<>).MakeGenericType(types);
                    case 2: return typeof(ActionMethod<,>).MakeGenericType(types);
                    case 3: return typeof(ActionMethod<,,>).MakeGenericType(types);
                    case 4: return typeof(ActionMethod<,,,>).MakeGenericType(types);
                    case 5: return typeof(ActionMethod<,,,,>).MakeGenericType(types);
                    case 6: return typeof(ActionMethod<,,,,,>).MakeGenericType(types);
                    case 7: return typeof(ActionMethod<,,,,,,>).MakeGenericType(types);
                    case 8: return typeof(ActionMethod<,,,,,,,>).MakeGenericType(types);
                    default: return null;
                }
            }
            return null;
        }

        /// <summary>
        /// 获取与输入类型匹配的ActionMethodBase子类中对应函数构建的委托；如果输入类型不符合规则时，报出异常；
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Delegate GetActionMethodDelegate(Type type)
        {
            var t = GetActionMethodType(type);
            if (t == null) return null;
            return Delegate.CreateDelegate(type, TypeHelper.CreateInstance(t), t.GetMethod(nameof(ActionMethod.Invoked)));
        }
    }

    /// <summary>
    /// Action方法基础类
    /// </summary>
    public abstract class ActionMethodBase
    {
        /// <summary>
        /// 关联的Action事件回调时，本事件也执行回调
        /// </summary>
        public static event Action<ActionMethodBase, ITupleData> onEventInvoked;

        protected void InvokedInternal(ITupleData tuple)
        {
            onEventInvoked?.Invoke(this, tuple);
        }
    }

    /// <summary>
    /// Action方法类
    /// </summary>
    public class ActionMethod : ActionMethodBase
    {
        /// <summary>
        /// 被调用
        /// </summary>
        public void Invoked() => InvokedInternal(EmptyTupleData.Default);
    }

    /// <summary>
    /// Action方法1泛型类
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    public class ActionMethod<T1> : ActionMethodBase
    {
        /// <summary>
        /// 被调用
        /// </summary>
        /// <param name="arg1"></param>
        public void Invoked(T1 arg1) => InvokedInternal(TupleData.Create(arg1));
    }

    /// <summary>
    /// Action方法2泛型类
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    public class ActionMethod<T1, T2> : ActionMethodBase
    {
        /// <summary>
        /// 被调用
        /// </summary>
        public void Invoked(T1 arg1, T2 arg2) => InvokedInternal(TupleData.Create(arg1, arg2));
    }

    /// <summary>
    /// Action方法3泛型类
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    public class ActionMethod<T1, T2, T3> : ActionMethodBase
    {
        /// <summary>
        /// 被调用
        /// </summary>
        public void Invoked(T1 arg1, T2 arg2, T3 arg3) => InvokedInternal(TupleData.Create(arg1, arg2, arg3));
    }

    /// <summary>
    /// Action方法4泛型类
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    public class ActionMethod<T1, T2, T3, T4> : ActionMethodBase
    {
        /// <summary>
        /// 被调用
        /// </summary>
        public void Invoked(T1 arg1, T2 arg2, T3 arg3, T4 arg4) => InvokedInternal(TupleData.Create(arg1, arg2, arg3, arg4));
    }

    /// <summary>
    /// Action方法5泛型类
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <typeparam name="T5"></typeparam>
    public class ActionMethod<T1, T2, T3, T4, T5> : ActionMethodBase
    {
        /// <summary>
        /// 被调用
        /// </summary>
        public void Invoked(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) => InvokedInternal(TupleData.Create(arg1, arg2, arg3, arg4, arg5));
    }

    /// <summary>
    /// Action方法6泛型类
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <typeparam name="T5"></typeparam>
    /// <typeparam name="T6"></typeparam>
    public class ActionMethod<T1, T2, T3, T4, T5, T6> : ActionMethodBase
    {
        /// <summary>
        /// 被调用
        /// </summary>
        public void Invoked(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6) => InvokedInternal(TupleData.Create(arg1, arg2, arg3, arg4, arg5, arg6));
    }

    /// <summary>
    /// Action方法7泛型类
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <typeparam name="T5"></typeparam>
    /// <typeparam name="T6"></typeparam>
    /// <typeparam name="T7"></typeparam>
    public class ActionMethod<T1, T2, T3, T4, T5, T6, T7> : ActionMethodBase
    {
        /// <summary>
        /// 被调用
        /// </summary>
        public void Invoked(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7) => InvokedInternal(TupleData.Create(arg1, arg2, arg3, arg4, arg5, arg6, arg7));
    }

    /// <summary>
    /// Action方法8泛型类
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <typeparam name="T5"></typeparam>
    /// <typeparam name="T6"></typeparam>
    /// <typeparam name="T7"></typeparam>
    /// <typeparam name="T8"></typeparam>
    public class ActionMethod<T1, T2, T3, T4, T5, T6, T7, T8> : ActionMethodBase
    {
        /// <summary>
        /// 被调用
        /// </summary>
        public void Invoked(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8) => InvokedInternal(TupleData.Create(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8));
    }
}
