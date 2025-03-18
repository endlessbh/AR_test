using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.Interfaces;
using XCSJ.LitJson;
using XCSJ.PluginCommonUtils;

namespace XCSJ.EditorTools.Windows.RichTexts
{
    [Serializable]
    [Import]
    public class Element<T> : IOnAfterDeserialize
    {
        public string guid = Guid.NewGuid().ToString();
        public T value = default;

        public Element() { }
        public Element(T value)
        {
            this.value = value;
        }

        public override string ToString()
        {
            return Converter.instance.TryConvertTo(value, out string text) ? text : "";
        }

        public virtual void OnAfterDeserialize(ISerializeContext serializeContext) { }

        #region 字符串处理

        public static Element<T> Parse(string text)
        {
            if (Converter.instance.TryConvertTo(text, out T value))
            {
                return new Element<T>(value);
            }
            return null;
        }

        public static bool TryParse(string text, out Element<T> element)
        {
            if (Converter.instance.TryConvertTo(text, out T value))
            {
                element = new Element<T>(value);
                return true;
            }
            element = null;
            return false;
        }

        public static implicit operator Element<T>(string text) => Parse(text);

        #endregion
    }

    [Serializable]
    [Name("元素")]
    public class Element : Element<string>
    {
        public Element() { value = ""; }
        public Element(string value) : base(value) { }

        public new static Element Parse(string text)
        {
            if (Converter.instance.TryConvertTo(text, out string value))
            {
                return new Element(value);
            }
            return null;
        }

        public static bool TryParse(string text, out Element element)
        {
            if (Converter.instance.TryConvertTo(text, out string value))
            {
                element = new Element(value);
                return true;
            }
            element = null;
            return false;
        }

        public static implicit operator Element(string text)
        {
            return Parse(text);
        }
    }
}
