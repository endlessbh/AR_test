using System.Collections.Generic;
using XCSJ.Extension.Base.Dataflows.DataBinders;
using XCSJ.Extension.Base.Dataflows.Models;

namespace XCSJ.Extension.Base.Dataflows.Links
{
    /// <summary>
    /// 数据链
    /// 用于记录相关连的源数据和目标数据的对象
    /// </summary>
    public interface IDataLink
    {
        /// <summary>
        /// 源绑定数据
        /// </summary>
        ITypeMemberDataBinder sourceBindData { get;}

        /// <summary>
        /// 目标绑定数据
        /// </summary>
        ITypeMemberDataBinder targetBindData { get;}

        /// <summary>
        /// 数据链接模式
        /// </summary>
        EDataLinkMode dataLinkMode { get; }

        /// <summary>
        /// 绑定
        /// </summary>
        void Bind();

        /// <summary>
        /// 解除绑定
        /// </summary>
        void Unbind();

        /// <summary>
        /// 响应源事件
        /// </summary>
        /// <param name="eventArgs"></param>
        void OnReceiveSourceEvent(XValueEventArg eventArgs);

        /// <summary>
        /// 响应目标事件
        /// </summary>
        /// <param name="eventArgs"></param>
        void OnReceiveTargetEvent(XValueEventArg eventArgs);

        /// <summary>
        /// 更新值 : 用于更新关联数据的值
        /// </summary>
        void UpdateValue();

        /// <summary>
        /// 设置源或目标数据的主体对象 ：用于动态实例化主体对象
        /// </summary>
        /// <param name="mainObject"></param>
        /// <returns></returns>
        bool SetDataMainObject(object mainObject);
    }

    /// <summary>
    /// 数据链列表
    /// </summary>
    public interface IDataLinkSet
    {
        IEnumerable<IDataLink> links { get; }
    }
}
