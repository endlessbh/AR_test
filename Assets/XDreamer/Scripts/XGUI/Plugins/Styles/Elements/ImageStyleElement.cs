using System;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXGUI.Styles.Base;

namespace XCSJ.PluginXGUI.Styles.Elements
{
    /// <summary>
    /// 图像样式
    /// </summary>
    [StyleLink(typeof(Image), typeof(EParamsSetting))]
    [Name("图像")]
    public class ImageStyleElement : BaseStyleElement
    {
        /// <summary>
        /// 样式参数设置
        /// </summary>
        [Flags]
        [Name("样式参数设置")]
        private enum EParamsSetting
        {
            [Name("精灵图像")]
            [EnumFieldName("精灵图像")]
            Sprite = 1 << 0,

            [Name("颜色")]
            [EnumFieldName("颜色")]
            Color = 1 << 1,

            [Name("材质")]
            [EnumFieldName("材质")]
            Material = 1 << 2,

            [Name("图像填充")]
            [EnumFieldName("图像填充")]
            Fill = 1 << 3,
        }

        /// <summary>
        /// 精灵图像
        /// </summary>
        [Name("精灵图像")]
        public Sprite _sourceImage = null;

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
        /// 图像填充类型
        /// </summary>
        [Name("图像填充类型")]
        [HideInSuperInspector(nameof(_sourceImage), EValidityCheckType.Null)]
        public Image.Type _imageFillType = Image.Type.Sliced;

        #region 图像填充类型=Simple

        /// <summary>
        /// 使用精灵网格
        /// </summary>
        [Name("使用精灵网格")]
        [HideInSuperInspector(nameof(_sourceImage), EValidityCheckType.Equal | EValidityCheckType.Or, null, nameof(_imageFillType), EValidityCheckType.NotEqual, Image.Type.Simple)]
        public bool _useSpriteMesh = false;

        /// <summary>
        /// 保持比例
        /// </summary>
        [Name("保持比例")]
        [HideInSuperInspector(nameof(_sourceImage), EValidityCheckType.Equal | EValidityCheckType.Or, null, nameof(_imageFillType), EValidityCheckType.NotEqual, Image.Type.Simple)]
        public bool _preserveAspect = false;

        #endregion

        #region 图像填充类型=Sliced

        /// <summary>
        /// 中心填充
        /// </summary>
        [Name("中心填充")]
        [HideInSuperInspector(nameof(_sourceImage), EValidityCheckType.Null)]
        public bool _fillCenter = true;

        #endregion

        #region 图像填充类型=Filled

        /// <summary>
        /// 填充方法
        /// </summary>
        [Name("填充方法")]
        [HideInSuperInspector(nameof(_sourceImage), EValidityCheckType.Equal | EValidityCheckType.Or, null, nameof(_imageFillType), EValidityCheckType.NotEqual, Image.Type.Filled)]
        public Image.FillMethod _fillMethod = Image.FillMethod.Radial360;

        /// <summary>
        /// 填充起始
        /// </summary>
        [Name("填充起始")]
        [HideInInspector]
        public int _fillOrigin = 0;

        public int fillOrigin
        {
            set => this.XModifyProperty(ref _fillOrigin, value);
        }

        /// <summary>
        /// 填充量
        /// </summary>
        [Name("填充量")]
        [Range(0,1)]
        [HideInSuperInspector(nameof(_sourceImage), EValidityCheckType.Equal | EValidityCheckType.Or, null, nameof(_imageFillType), EValidityCheckType.NotEqual, Image.Type.Filled)]
        public float _fillAmount = 1;

        /// <summary>
        /// 顺时针方向
        /// </summary>
        [Name("顺时针方向")]
        [HideInSuperInspector(nameof(_sourceImage), EValidityCheckType.Equal | EValidityCheckType.Or, null, nameof(_imageFillType), EValidityCheckType.NotEqual, Image.Type.Filled)]
        public bool _clockwise = true;

        #endregion

        /// <summary>
        /// 样式应用对象
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="paramsMask">参数掩码：子类根据比特位为0或1来设置对应的参数</param>
        /// <returns></returns>
        protected override bool OnStyleToObject(UnityEngine.Object obj, int paramsMask)
        {
            return StyleHelper.ModifyObjectPropertyWithMask(obj as Image, typeof(EParamsSetting), paramsMask, (image, paramsSetting) =>
            {
                switch (paramsSetting)
                {
                    case EParamsSetting.Sprite: image.sprite = _sourceImage; break;
                    case EParamsSetting.Color: image.color = _color; break;
                    case EParamsSetting.Material: image.material = _material; break;
                    case EParamsSetting.Fill:
                        {
                            image.type = _imageFillType;

                            image.useSpriteMesh = _useSpriteMesh;
                            image.preserveAspect = _preserveAspect;

                            image.fillCenter = _fillCenter;

                            image.fillMethod = _fillMethod;
                            image.fillOrigin = _fillOrigin;

                            image.fillClockwise = _clockwise;
                            image.fillAmount = _fillAmount;
                            break;
                        }
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
            return StyleHelper.ModifyObjectPropertyWithMask(obj as Image, typeof(EParamsSetting), paramsMask, (image, paramsSetting) =>
            {
                switch (paramsSetting)
                {
                    case EParamsSetting.Sprite: _sourceImage = image.sprite; break;
                    case EParamsSetting.Color: _color = image.color; break;
                    case EParamsSetting.Material: _material = image.material; break;
                    case EParamsSetting.Fill:
                        {
                            _imageFillType = image.type;

                            _useSpriteMesh = image.useSpriteMesh;
                            _preserveAspect = image.preserveAspect;

                            _fillCenter = image.fillCenter;

                            _fillMethod = image.fillMethod;
                            _fillOrigin = image.fillOrigin;

                            _clockwise = image.fillClockwise;
                            _fillAmount = image.fillAmount;
                            break;
                        }
                }
            });
        }
    }

}
