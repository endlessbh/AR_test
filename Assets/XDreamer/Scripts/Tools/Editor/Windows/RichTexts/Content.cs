using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.LitJson;

namespace XCSJ.EditorTools.Windows.RichTexts
{
    /// <summary>
    /// 内容
    /// </summary>
    [Name("内容")]
    [Serializable]
    public class Content : Element
    {
        public Element name = new Element(nameof(name));
        public Element tip = new Element(nameof(tip));

        [Json(false)]
        public GUIContent content => new GUIContent(name.value, tip.value);
    }
}
