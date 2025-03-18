using System;
using UnityEngine;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginXGUI.Styles.Base
{
    /// <summary>
    /// 样式元素基类
    /// </summary>
    public abstract class BaseStyleElement : SO
    {
        /// <summary>
        /// 默认名称
        /// </summary>
        public const string DefaultName = "默认";

        /// <summary>
        /// 重置
        /// </summary>
        protected virtual void Reset() => name = DefaultName + CommonFun.Name(GetType());

        /// <summary>
        /// 属性修改
        /// </summary>
        public static event Action<BaseStyleElement> onPropertyChanged;

        /// <summary>
        /// 发送属性修改事件
        /// </summary>
        public void SendPropertyChangedEvent()
        {
            onPropertyChanged?.Invoke(this);
            XStyleCache.UpdateStyle();// 更新样式
        }

        /// <summary>
        /// 应用样式:将样式应用到目标对象上
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="paramsMask">参数掩码</param>
        public virtual bool StyleToObject(UnityEngine.Object obj, int paramsMask)
        {
            if (!obj) return false;

            onBeforeStyleToObject?.Invoke(this, obj);

            var rs = OnStyleToObject(obj, paramsMask);

            onAfterStyleToObject?.Invoke(this, obj);

            return rs;
        }

        /// <summary>
        /// 获取样式：从目标对象上提取样式
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="paramsMask"></param>
        /// <returns></returns>
        public virtual bool ObjectToStyle(UnityEngine.Object obj, int paramsMask)
        {
            if (!obj) return false;

            onBeforeObjectToStyle?.Invoke(this, obj);

            var rs = OnObjectToStyle(obj, paramsMask);

            onAfterObjectToStyle?.Invoke(this, obj);

            return rs;
        }

        /// <summary>
        /// 样式应用对象
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="paramsMask">参数掩码：子类根据比特位为0或1来设置对应的参数</param>
        /// <returns></returns>
        protected abstract bool OnStyleToObject(UnityEngine.Object obj, int paramsMask);

        /// <summary>
        /// 样式应用前事件回调
        /// </summary>
        public static event Action<BaseStyleElement, UnityEngine.Object> onBeforeStyleToObject;

        /// <summary>
        /// 样式应用后事件回调
        /// </summary>
        public static event Action<BaseStyleElement, UnityEngine.Object> onAfterStyleToObject;

        /// <summary>
        /// 对象提取样式
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="paramsMask">参数掩码：子类根据比特位为0或1来设置对应的参数</param>
        /// <returns></returns>
        protected abstract bool OnObjectToStyle(UnityEngine.Object obj, int paramsMask);

        /// <summary>
        /// 从对象中提取样式前事件回调
        /// </summary>
        public static event Action<BaseStyleElement, UnityEngine.Object> onBeforeObjectToStyle;

        /// <summary>
        /// 从对象中提取样式后事件回调
        /// </summary>
        public static event Action<BaseStyleElement, UnityEngine.Object> onAfterObjectToStyle;

        /// <summary>
        /// 获取样式元素类型缺省名称
        /// </summary>
        /// <param name="componentType">样式关联组件</param>
        /// <returns>成功返回有效字符串；失败返回空字符串</returns>
        public static string GetDefaultName(Component component)
        {
            if (!component) return "";

            return GetDefaultName(StyleLinkAttribute.GetStyleElementType(component.GetType()));
        }

        /// <summary>
        /// 获取样式元素类型缺省名称
        /// </summary>
        /// <param name="componentType">样式元素类型</param>
        /// <returns>成功返回有效字符串；失败返回空字符串</returns>
        public static string GetDefaultName(Type elementType)
        {
            return elementType != null ? BaseStyleElement.DefaultName + CommonFun.Name(elementType) : "";
        }

        /// <summary>
        /// 克隆样式
        /// </summary>
        /// <returns></returns>
        public virtual object Clone()
        {
            var newObject = Instantiate(this);
            newObject.name = name;
            return newObject;
        }
    }


}
