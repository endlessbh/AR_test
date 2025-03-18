using System;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Attributes;
using XCSJ.PluginXGUI.Styles.Base;

namespace XCSJ.PluginXGUI.Styles.Elements
{
    /// <summary>
    /// 贴图样式
    /// </summary>
    [StyleLink(typeof(RawImage), typeof(EParamsSetting))]
    [Name("贴图")]
    public class RawImageStyleElement : BaseStyleElement
    {
        /// <summary>
        /// 样式参数设置
        /// </summary>
        [Flags]
        [Name("样式参数设置")]
        public enum EParamsSetting
        {
            [Name("贴图")]
            [EnumFieldName("贴图")]
            Texture = 1 << 0,

            [Name("颜色")]
            [EnumFieldName("颜色")]
            Color = 1 << 1,

            [Name("材质")]
            [EnumFieldName("材质")]
            Material = 1 << 2,

            [Name("UV矩形")]
            [EnumFieldName("UV矩形")]
            UVRect = 1 << 3,
        }

        /// <summary>
        /// 精灵图像
        /// </summary>
        [Name("贴图")]
        public Texture _texture = null;

        /// <summary>
        /// 颜色
        /// </summary>
        [Name("颜色")]
        public Color _color = Color.white;

        /// <summary>
        /// 材质
        /// </summary>
        [Name("材质")]
        public Material _material;

        /// <summary>
        /// UV矩形
        /// </summary>
        [Name("UV矩形")]
        public Rect _uvRect;

        /// <summary>
        /// 样式应用对象
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="paramsMask">参数掩码：子类根据比特位为0或1来设置对应的参数</param>
        /// <returns></returns>
        protected override bool OnStyleToObject(UnityEngine.Object obj, int paramsMask)
        {
            return StyleHelper.ModifyObjectPropertyWithMask(obj as RawImage, typeof(EParamsSetting), paramsMask, (rawImage, paramsSetting) =>
            {
                switch (paramsSetting)
                {
                    case EParamsSetting.Texture: rawImage.texture = _texture; break;
                    case EParamsSetting.Color: rawImage.color = _color; break;
                    case EParamsSetting.Material: rawImage.material = _material; break;
                    case EParamsSetting.UVRect: rawImage.uvRect = _uvRect; break;
                }
            });
        }

        /// <summary>
        /// 对象提取样式
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="paramsMask">参数掩码：子类根据比特位为0或1来设置对应的参数</param>
        /// <returns></returns>
        protected override bool OnObjectToStyle(UnityEngine.Object obj, int paramsMask)
        {
            return StyleHelper.ModifyObjectPropertyWithMask(obj as RawImage, typeof(EParamsSetting), paramsMask, (rawImage, paramsSetting) =>
            {
                switch (paramsSetting)
                {
                    case EParamsSetting.Texture: _texture = rawImage.texture; break;
                    case EParamsSetting.Color: _color = rawImage.color; break;
                    case EParamsSetting.Material: _material = rawImage.material; break;
                    case EParamsSetting.UVRect: _uvRect = rawImage.uvRect; break;
                }
            });
        }
    }

}
