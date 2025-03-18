using System;
using UnityEngine;
using XCSJ.Attributes;

#if UNITY_EDITOR
#endif

namespace XCSJ.PluginXGUI.Styles.Base
{
    /// <summary>
    /// 组件集合接口
    /// </summary>
    public interface IComponentCollection
    {
        Component[] components { get; }
    }

    /// <summary>
    /// 组件样式关联类
    /// </summary>
    [Serializable]
    [Name("组件样式")]
    public class ComponentStyle
    {
        /// <summary>
        /// 组件
        /// </summary>
        [Name("组件")]
        public Component _component = null;

        /// <summary>
        /// 样式元素名称
        /// </summary>
        [Name("样式元素名称")]
        public string _elementName = "";

        /// <summary>
        /// 样式参数设定掩码
        /// </summary>
        [Name("样式参数设定")]
        public int _paramsMask = -1;

        /// <summary>
        /// 设置元素名称
        /// </summary>
        /// <param name="styleElementCollection"></param>
        /// <param name="name"></param>
        public void SetElementName(StyleElementCollection styleElementCollection, string name="")
        {
            if (_component && styleElementCollection)
            {
                if (string.IsNullOrEmpty(name))
                {
                    name = _elementName;
                }
                var element = string.IsNullOrEmpty(name) ? styleElementCollection.GetElement(_component): styleElementCollection.GetElement(name, _component);
                _elementName = element?.name;
            }
        }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public ComponentStyle()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="component"></param>
        public ComponentStyle(Component component) : this(component, BaseStyleElement.GetDefaultName(component))
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="c"></param>
        public ComponentStyle(Component component, string elementName, int paramsMask = -1)
        {
            _component = component;
            _elementName = elementName;
            _paramsMask = paramsMask;
        }

        /// <summary>
        /// 数据有效
        /// </summary>
        public bool valid => _component && !string.IsNullOrEmpty(_elementName);

        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="styleElementCollection"></param>
        /// <param name="isStyleToObject"></param>
        /// <returns></returns>
        public bool UpdateData(StyleElementCollection styleElementCollection, bool isStyleToObject)
        {
            if (styleElementCollection && valid)
            {
                var element = styleElementCollection.GetElement(_elementName, _component);
                if (element)
                {
                    return isStyleToObject ? element.StyleToObject(_component, _paramsMask) : element.ObjectToStyle(_component, _paramsMask);
                }
            }
            return false;
        }
    }
}
