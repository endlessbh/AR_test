using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.Scripts;

namespace XCSJ.PluginTools.PropertyDatas
{
    /// <summary>
    /// 交互属性数据
    /// </summary>
    [Serializable]
    public class InteractPropertyData : BaseInteractPropertyData
    {
        /// <summary>
        /// 颜色
        /// </summary>
        [Name("颜色")]
        public ColorPropertyValue _color = new ColorPropertyValue();

        /// <summary>
        /// 材质
        /// </summary>
        [Name("材质")]
        public MaterialPropertyValue _material = new MaterialPropertyValue();

        /// <summary>
        /// 图片
        /// </summary>
        [Name("图片")]
        public TexturePropertyValue _texture = new TexturePropertyValue();

        /// <summary>
        /// 音频剪辑
        /// </summary>
        [Name("音频剪辑")]
        public AudioClipPropertyValue _audioClip = new AudioClipPropertyValue();

        /// <summary>
        /// 视频剪辑
        /// </summary>
        [Name("视频剪辑")]
        [EndGroup(true)]
        public VideoClipPropertyValue _videoClip = new VideoClipPropertyValue();

        /// <summary>
        /// 颜色
        /// </summary>
        public Color color => _color.GetValue();

        /// <summary>
        /// 材质
        /// </summary>
        public Material material => _material.GetValue();

        /// <summary>
        /// 图片
        /// </summary>
        public Texture texture => _texture.GetValue();

        /// <summary>
        /// 音频剪辑
        /// </summary>
        public AudioClip audioClip => _audioClip.GetValue();

        /// <summary>
        /// 视频剪辑
        /// </summary>
        public VideoClip videoClip => _videoClip.GetValue();

        /// <summary>
        /// 构造函数
        /// </summary>
        public InteractPropertyData() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public InteractPropertyData(string key, string value) : base(key, value) { }

        /// <summary>
        /// 匹配
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool IsMatch(string key) => _key.TryGetValue(out var keyValue) && keyValue == key;

        /// <summary>
        /// 匹配
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool IsMatch(string key, string text) => IsMatch(key) && _value.TryGetValue(out var textValue) && textValue == text;
    }

    /// <summary>
    /// 交互属性数据特性：主要用于修饰存储交互属性数据的字符串
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class InteractPropertyDataAttribute : PropertyAttribute
    {
        /// <summary>
        /// 交互属性数据类型
        /// </summary>
        public EInteractPropertyDataType interactPropertyDataType { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="interactPropertyDataType"></param>
        public InteractPropertyDataAttribute(EInteractPropertyDataType interactPropertyDataType = EInteractPropertyDataType.Both)
        {
            this.interactPropertyDataType = interactPropertyDataType;
        }
    }

    /// <summary>
    /// 交互属性数据类型
    /// </summary>
    public enum EInteractPropertyDataType
    {
        Key,

        Value,

        Both,
    }
}
