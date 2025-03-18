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
using XCSJ.Interfaces;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginXGUI.Styles.Base
{
    /// <summary>
    /// 样式元素集合接口
    /// </summary>
    public interface IStyleElementCollection
    {
        /// <summary>
        /// 样式元素集合
        /// </summary>
        List<BaseStyleElement> elements { get; }

        /// <summary>
        /// 包含
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        bool Contains(BaseStyleElement element);
    }

    /// <summary>
    /// 组合元素
    /// </summary>
    [Name("组合元素")]
    public class StyleElementCollection : BaseStyleElement, IStyleElementCollection, IClone
    {
        /// <summary>
        /// 样式元素列表
        /// </summary>
        [Name("元素列表")]
        [SerializeField]
        public List<BaseStyleElement> _elements = new List<BaseStyleElement>();

        /// <summary>
        /// 样式元素列表
        /// </summary>
        public virtual List<BaseStyleElement> elements => _elements;

        /// <summary>
        /// 非集合类型的元素
        /// </summary>
        public List<BaseStyleElement> elementsExculdeCollection => elements.Where(e => !typeof(IStyleElementCollection).IsAssignableFrom(e.GetType())).ToList();

        /// <summary>
        /// 名称有效性检测
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool ValidName(string name)
        {
            return !string.IsNullOrEmpty(name) && !elements.Exists(e => e.name == name);
        }

        /// <summary>
        /// 通过类型获取有效名称
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public string GetValidName<T>() where T : BaseStyleElement
        {
            return GetValidName(typeof(T));
        }

        /// <summary>
        /// 获取有效类型名称
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public string GetValidName(Type type)
        {
            if (type==null)
            {
                Debug.LogErrorFormat("输入类型参数为空！");
                return null;
            }
            var parentType = typeof(BaseStyleElement);
            if (parentType.IsAssignableFrom(type))
            {
                return GetValidName(CommonFun.Name(type));
            }
            else
            {
                Debug.LogErrorFormat("类型{0}未继承自{1}", type.FullName, parentType.FullName);
                return null;
            }
        }

        /// <summary>
        /// 获取有效名称：该名称不为空，并且不能重复
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetValidName(string name)
        {
            int indexName = 1;
            var newName = name;
            while(elements.Exists(e => e.name == newName))
            {
                newName = name + (indexName++).ToString();
            }
            return newName;
        }

        /// <summary>
        /// 包含
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public bool Contains(BaseStyleElement element)
        {
            foreach (var item in elements)
            {
                if (item==element)
                {
                    return true;
                }
                else if (item is IStyleElementCollection sec)
                {
                    if (sec.Contains(element))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 创建样式元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public BaseStyleElement CreateElement<T>(string name = "") where T : BaseStyleElement
        {
            return CreateElement(typeof(T), name);
        }

        /// <summary>
        /// 创建样式元素
        /// </summary>
        /// <param name="elementType"></param>
        /// <param name="name"></param>
        public BaseStyleElement CreateElement(Type elementType, string name = "")
        {
            if (!ValidName(name))
            {
                name = GetValidName(elementType);
            }

            var newElement = ScriptableObject.CreateInstance(elementType) as BaseStyleElement;
            newElement.name = name;
            AddElement(newElement);
            return newElement;
        }

        /// <summary>
        /// 创建新的样式元素
        /// </summary>
        public static event Action<StyleElementCollection, BaseStyleElement> onAddStyleElement;

        /// <summary>
        /// 移除旧样式元素
        /// </summary>
        public static event Action<StyleElementCollection, BaseStyleElement> onRemoveStyleElement;

        /// <summary>
        /// 添加样式元素
        /// </summary>
        /// <param name="element"></param>
        public virtual bool AddElement(BaseStyleElement element)
        {
            this.XModifyProperty(() => _elements.Add(element));

            onAddStyleElement?.Invoke(this, element);
            return true;
        }

        /// <summary>
        /// 删除样式元素
        /// </summary>
        /// <param name="element"></param>
        public virtual bool DeleteElement(BaseStyleElement element)
        {
            bool rs = false;
            if (element && _elements.Contains(element))
            {
                this.XModifyProperty(() => rs = _elements.Remove(element));

                onRemoveStyleElement?.Invoke(this, element);
                rs = true;
            }
            return rs;
        }

        /// <summary>
        /// 移除空对象
        /// </summary>
        public void RemoveNullElement()
        {
            this.XModifyProperty(() =>
            {
                for (int i = _elements.Count - 1; i >= 0; i--)
                {
                    var current = _elements[i];
                    if (current)
                    {
                        if (current is StyleElementCollection sec && sec)
                        {
                            sec.RemoveNullElement();
                        }
                    }
                    else
                    {
                        _elements.RemoveAt(i);
                    }
                }
            });
        }

        /// <summary>
        /// 克隆样式元素
        /// </summary>
        /// <param name="element"></param>
        /// <param name="newName"></param>
        /// <returns></returns>
        public BaseStyleElement CloneElement(BaseStyleElement element, string newName="")
        {
            var newElement = Instantiate(element);
            newElement.name = string.IsNullOrEmpty(newName) ? GetValidName(element.name) : newName;
            AddElement(newElement);
            return newElement;
        }

        /// <summary>
        /// 克隆样式
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            var newObject = CreateInstance(this.GetType()) as StyleElementCollection;

            // 深度克隆链表
            foreach (var e in this.elements)
            {
                var obj = e.Clone() as BaseStyleElement;
                obj.name = e.name;
                newObject.elements.Add(obj);
            }
            return newObject;
        }

        #region 获取元素样式

        /// <summary>
        /// 获取类型相同的样式元素对象列表
        /// </summary>
        public IEnumerable<T> GetElements<T>() where T : BaseStyleElement
        {
            return GetElements(typeof(T)).Cast<T>();
        }

        /// <summary>
        /// 获取类型相同的样式元素对象列表
        /// </summary>
        /// <param name="elementType">样式元素类型</param>
        /// <returns></returns>
        public IEnumerable<BaseStyleElement> GetElements(Type elementType)
        {
            if (elementType == null) return new List<BaseStyleElement>();

            return elements.Where(e => e ? elementType.IsAssignableFrom(e.GetType()): false);
        }

        /// <summary>
        /// 根据名称和样式元素类型获取样式元素
        /// </summary>
        /// <param name="name"></param>
        /// <param name="elementType">BaseUIStyleElement子类</param>
        /// <returns></returns>
        public BaseStyleElement GetElement(string name, Type elementType)
        {
            if (elementType!=null) 
            {
                return elements.Find(item => item && item.name == name && elementType.IsAssignableFrom(item.GetType()));
            }
            return null;
        }

        /// <summary>
        /// 获取类型相同的样式元素对象名称列表
        /// </summary>
        /// <param name="elementType"></param>
        /// <returns></returns>
        public string[] GetElementNames(Type elementType) 
        {
            var es = GetElements(elementType);
            if (es.Count()>0)
            {
                return es.Select(e => e.name).ToArray();
            }
            else
            {
                return new string[0];
            }
        }

        /// <summary>
        /// 根据名称和样式类型信息获取样式元素
        /// </summary>
        /// <typeparam name="T">BaseStyleElement子类</typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public T GetElement<T>(string name) where T : BaseStyleElement
        {
            return GetElement(name, typeof(T)) as T;
        }

        /// <summary>
        /// 根据名称和对象获取样式元素
        /// </summary>
        /// <param name="name"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public BaseStyleElement GetElement(string name, UnityEngine.Object obj)
        {
            return obj ? GetElement(name, StyleLinkAttribute.GetStyleElementType(obj.GetType())) : null;
        }

        /// <summary>
        /// 通过传入对象返回第一个匹配类型的样式元素对象
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public BaseStyleElement GetElement(UnityEngine.Object obj)
        {
            if (obj)
            {
                return GetElements(StyleLinkAttribute.GetStyleElementType(obj.GetType())).FirstOrDefault();
            }
            return null;
        }

        /// <summary>
        /// 获取对象类型相关联的样式元素列表
        /// </summary>
        /// <param name="obj">可应用样式的组件对象</param>
        /// <returns></returns>
        public IEnumerable<BaseStyleElement> GetElements(UnityEngine.Object obj)
        {
            return obj ? GetElements(StyleLinkAttribute.GetStyleElementType(obj.GetType())) : null;
        } 

        #endregion

        /// <summary>
        /// 样式应用对象
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="paramsMask">参数掩码：子类根据比特位为0或1来设置对应的参数</param>
        /// <returns></returns>
        protected override bool OnStyleToObject(UnityEngine.Object obj, int paramsMask)
        {
            foreach (var e in elements)
            {
                if (e.StyleToObject(obj, paramsMask))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 对象提取样式
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="paramsMask">参数掩码：子类根据比特位为0或1来设置对应的参数</param>
        /// <returns></returns>
        protected override bool OnObjectToStyle(UnityEngine.Object obj, int paramsMask)
        {
            foreach (var e in elements)
            {
                if (e.ObjectToStyle(obj, paramsMask))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
