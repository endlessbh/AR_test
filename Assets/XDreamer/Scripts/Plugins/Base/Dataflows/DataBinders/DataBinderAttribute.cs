using System;
using XCSJ.Algorithms;

namespace XCSJ.Extension.Base.Dataflows.DataBinders
{
    /// <summary>
    /// 目标绑定器特性
    /// 用于修饰绑定器，并指定绑定目标类型的特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=true, Inherited = false)]
    public class DataBinderAttribute : LinkedTypeAttribute
    {
        /// <summary>
        /// 被绑定类型
        /// </summary>
        public Type targetType { get; } = null;

        /// <summary>
        /// 被绑定类型成员名称
        /// </summary>
        public string memberName { get; protected set; } = "";

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="type"></param>
        public DataBinderAttribute(Type type, string memberName = "", bool forChildClasses=true) : base(type, forChildClasses, ToPurpose(memberName))
        {
            targetType = type;
            this.memberName = memberName;
        }

        /// <summary>
        /// 获取目标字符串
        /// </summary>
        /// <param name="memberName"></param>
        /// <returns></returns>
        public static string ToPurpose(string memberName)
        {
            return nameof(DataBinderAttribute) + "." + memberName;
        }

        /// <summary>
        /// 获取绑定类型
        /// </summary>
        /// <param name="type">被绑定类型</param>
        /// <param name="memberName">被绑定类型成员</param>
        /// <returns></returns>
        public static Type GetBinderType(Type type, string memberName = "")
        {
            return LinkedTypeCache.Get(type, ToPurpose(memberName));
        }
    }
}
