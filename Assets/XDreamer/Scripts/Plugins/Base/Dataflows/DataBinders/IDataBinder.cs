using System;
using XCSJ.Extension.Base.Dataflows.Binders;
using XCSJ.Extension.Base.Dataflows.Models;

namespace XCSJ.Extension.Base.Dataflows.DataBinders
{
    /// <summary>
    /// 类型成员数据绑定器接口 ： 用于封装类型成员信息的接口
    /// </summary>
    public interface ITypeMemberDataBinder : IDataBinder, ISendEvent
    {
        /// <summary>
        /// 类型成员绑定器接口
        /// </summary>
        ITypeMemberBinder typeMemberBinder { get;}

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="typeMemberBinder"></param>
        void Init(ITypeMemberBinder typeMemberBinder);

        /// <summary>
        /// 设置初始值
        /// </summary>
        void InitValue();

        /// <summary>
        /// 绑定
        /// </summary>
        /// <param name="onReceiveEvent"></param>
        void Bind(Action<XValueEventArg> onReceiveEvent);

        /// <summary>
        /// 解绑
        /// </summary>
        /// <param name="onReceiveEvent"></param>
        void Unbind(Action<XValueEventArg> onReceiveEvent);

        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="eventArg"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool TryGet(XValueEventArg eventArg, out object value);

        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="linkedBindData"></param>
        /// <param name="eventArg"></param>
        void Set(ITypeMemberDataBinder linkedBindData, XValueEventArg eventArg);

        /// <summary>
        /// 设置数据绑定器的主体对象 ：主要用于动态绑定真实数据项
        /// </summary>
        /// <param name="mainObject">主体对象</param>
        bool SetMainObject(object mainObject);
    }

    /// <summary>
    /// 数据绑定器接口
    /// </summary>
    public interface IDataBinder : IMemberInfoParameters, IMemberAccessor
    {
    }

    /// <summary>
    /// 成员访问器接口
    /// </summary>
    public interface IMemberAccessor
    {
        /// <summary>
        /// 尝试获取成员值
        /// </summary>
        /// <param name="type"></param>
        /// <param name="obj"></param>
        /// <param name="memberName"></param>
        /// <param name="value"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        bool TryGetMemberValue(Type type, object obj, string memberName, out object value, object[] index = null);

        /// <summary>
        /// 尝试设置成员值
        /// </summary>
        /// <param name="type"></param>
        /// <param name="obj"></param>
        /// <param name="memberName"></param>
        /// <param name="value"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        bool TrySetMemberValue(Type type, object obj, string memberName, object value, object[] index = null);
    }
}
