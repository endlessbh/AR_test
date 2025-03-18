using System.Collections.Generic;
using XCSJ.PluginCommonUtils;

namespace XCSJ.Extension.Base.Dataflows.Models
{
    #region 属性变化接口

    /// <summary>
    /// 属性修改通知接口
    /// 包括将要改变和已改变通知事件与函数
    /// </summary>
    public interface ISendPropertyChangeEvent : ISendEvent
    {
        /// <summary>
        /// 属性将要改变
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="newValue"></param>
        void SendPropertyWillChange(XPropertyChangeEventArgs eventArgs);

        /// <summary>
        /// 属性已改变
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        void SendPropertyChanged(XPropertyChangeEventArgs eventArgs);
    }

    /// <summary>
    /// 通知改变扩展
    /// </summary>
    public static class SendPropertyChangeExtension
    {
        /// <summary>
        /// 通知改变接口扩展函数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sendPropertyChange">通知改变接口</param>
        /// <param name="oldValue">旧值</param>
        /// <param name="newValue">新值</param>
        /// <param name="changeValueAction">改变函数</param>
        /// <param name="propertyName">属性名称</param>
        /// <returns></returns>
        public static bool SetProperty<T>(this ISendPropertyChangeEvent sendPropertyChange, ref T property, T newValue, string propertyName)
        {
            if (EqualityComparer<T>.Default.Equals(property, newValue)) return false;

            var willEventArg = XPropertyChangeEventArgs.CreateWillChangeEvent(sendPropertyChange, propertyName, newValue, property);
            sendPropertyChange.SendPropertyWillChange(willEventArg);

            // 可以改变
            if (willEventArg.canChange)
            {
                property = newValue;

                sendPropertyChange.SendPropertyChanged(XPropertyChangeEventArgs.CreateChangedEvent(sendPropertyChange, propertyName, newValue, property));
                return true;
            }

            return false;
        }

        /// <summary>
        /// 通知属性修改扩展函数
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="sendPropertyChange"></param>
        /// <param name="property"></param>
        /// <param name="newValue"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static bool SetProperty<TObject, T>(this TObject sendPropertyChange, ref T property, T newValue, string propertyName)
            where TObject : UnityEngine.Object, ISendPropertyChangeEvent
        {
            if (EqualityComparer<T>.Default.Equals(property, newValue)) return false;

            var willEventArg = XPropertyChangeEventArgs.CreateWillChangeEvent(sendPropertyChange, propertyName, newValue, property);
            sendPropertyChange.SendPropertyWillChange(willEventArg);

            // 可以改变
            if (willEventArg.canChange)
            {
                sendPropertyChange.XModifyProperty(ref property, newValue, propertyName);

                sendPropertyChange.SendPropertyChanged(XPropertyChangeEventArgs.CreateChangedEvent(sendPropertyChange, propertyName, newValue, property));
                return true;
            }

            return false;
        }

        /// <summary>
        /// 主动发送属性变化事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sendPropertyChange"></param>
        /// <param name="newValue"></param>
        /// <param name="propertyName"></param>
        public static void SendPropertyChanged<T>(this ISendPropertyChangeEvent sendPropertyChange, T newValue, string propertyName)
        {
            sendPropertyChange.SendPropertyChanged(XPropertyChangeEventArgs.CreateChangedEvent(sendPropertyChange, propertyName, newValue));
        }
    }

    #endregion
}
