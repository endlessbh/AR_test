using System;
using System.Reflection;

namespace XCSJ.Extension.Base.Dataflows.Binders
{
    /// <summary>
    /// 基础主类型接口
    /// </summary>
    public interface IBaseMainType
    {
        /// <summary>
        /// 主类型
        /// </summary>
        Type mainType { get; }
    }

    /// <summary>
    /// 主类型接口
    /// </summary>
    public interface IMainType : IBaseMainType
    {
        /// <summary>
        /// 主对象
        /// </summary>
        object mainObject { get; set; }
    }

    /// <summary>
    /// 基础子成员接口
    /// </summary>
    public interface IBaseSubMember
    {
        /// <summary>
        /// 成员名
        /// </summary>
        string memberName { get; set; }
    }

    /// <summary>
    /// 子成员接口
    /// </summary>
    public interface ISubMember : IBaseSubMember
    {
        /// <summary>
        /// 成员信息
        /// </summary>
        MemberInfo memberInfo { get; }

        /// <summary>
        /// 成员信息类型
        /// </summary>
        Type memberType { get; }

        /// <summary>
        /// 成员值
        /// </summary>
        object memberValue { get; set; }
    }

    /// <summary>
    /// 类型绑定器接口
    /// </summary>
    public interface ITypeBinder : IMainType, ISubMember, IMemberInfoParameters
    {
        /// <summary>
        /// 是否有成员
        /// </summary>
        bool hasMember { get; }

        /// <summary>
        /// 别名
        /// </summary>
        string alias { get; }

        /// <summary>
        /// 实体类型
        /// </summary>
        Type entityType { get; }

        /// <summary>
        /// 实体对象
        /// </summary>
        object entityObject { get; set; }
    }

    /// <summary>
    /// 类型成员绑定器接口
    /// </summary>
    public interface ITypeMemberBinder : ITypeBinder
    {
    }

    /// <summary>
    /// 成员信息参数接口
    /// </summary>
    public interface IMemberInfoParameters : IBaseMainType, IBaseSubMember
    {
        /// <summary>
        /// 反射标记量:用于获取成员时使用
        /// </summary>
        BindingFlags bindingFlags { get; }

        /// <summary>
        /// 包含基础类型
        /// </summary>
        bool includeBaseType { get; }
    }
}
