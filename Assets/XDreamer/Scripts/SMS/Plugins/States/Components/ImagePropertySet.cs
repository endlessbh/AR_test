using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.PluginSMS.States.Components
{
    /// <summary>
    /// 图像属性设置:设置图像的可射线点击、颜色、图像和材质等
    /// </summary>
    [ComponentMenu(SMSHelperExtension.ComponentCategoryName + "/" + Title, typeof(SMSManager))]
    [Name(Title, nameof(ImagePropertySet))]
    [Tip("设置图像的可射线点击、颜色、图像和材质等", "Clickable colors, images, etc")]
    public class ImagePropertySet : ComponentPropertySet<ImagePropertySet, Graphic>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "图像属性设置";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(SMSHelperExtension.ComponentCategoryName, typeof(SMSManager))]
        [Name(Title, nameof(ImagePropertySet))]
        [Tip("设置图像的可射线点击、颜色、图像和材质等", "Clickable colors, images, etc")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 属性名称
        /// </summary>
        [Name("属性名称")]
        public enum EPropertyName
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None,

            /// <summary>
            /// 射线目标
            /// </summary>
            [Name("射线目标")]
            RaycastTarget,

            /// <summary>
            /// 颜色
            /// </summary>
            [Name("颜色")]
            Color,

            /// <summary>
            /// 图像
            /// </summary>
            [Name("图像")]
            Image,

            /// <summary>
            /// 材质
            /// </summary>
            [Name("材质")]
            Material,
        }

        /// <summary>
        /// 属性名称
        /// </summary>
        [Name("属性名称")]
        [EnumPopup]
        public EPropertyName _propertyName = EPropertyName.Color;

        /// <summary>
        /// 射线目标
        /// </summary>
        [Name("射线目标")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.RaycastTarget)]
        [OnlyMemberElements]
        public EBoolPropertyValue _raycastTarget = new EBoolPropertyValue();

        /// <summary>
        /// 颜色
        /// </summary>
        [Name("颜色")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Color)]
        [OnlyMemberElements]
        public ColorPropertyValue _color = new ColorPropertyValue();


        /// <summary>
        /// 图像类型
        /// </summary>
        [Name("图像类型")]
        public enum GraphicType
        {
            /// <summary>
            /// 图像
            /// </summary>
            [Name("图像")]
            Image,

            /// <summary>
            /// 贴图
            /// </summary>
            [Name("贴图")]
            RawImage,
        }

        /// <summary>
        /// 图像类型
        /// </summary>
        [Name("图像类型")]
        [EnumPopup]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Image)]
        [OnlyMemberElements]
        public GraphicType _graphicType = GraphicType.Image;

        /// <summary>
        /// 精灵
        /// </summary>
        [Name("精灵")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual | EValidityCheckType.And, EPropertyName.Image, nameof(_graphicType), EValidityCheckType.NotEqual, GraphicType.Image)]
        [OnlyMemberElements]
        public SpritePropertyValue _sprite = new SpritePropertyValue();

        /// <summary>
        /// 贴图
        /// </summary>
        [Name("贴图")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual | EValidityCheckType.And, EPropertyName.Image, nameof(_graphicType), EValidityCheckType.NotEqual, GraphicType.RawImage)]
        [OnlyMemberElements]
        public TexturePropertyValue _texture = new TexturePropertyValue();

        /// <summary>
        /// 材质
        /// </summary>
        [Name("材质")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Material)]
        [OnlyMemberElements]
        public MaterialPropertyValue _material = new MaterialPropertyValue();

        /// <summary>
        /// 设置组件属性
        /// </summary>
        /// <param name="graphic"></param>
        protected override void SetComponentProperty(Graphic graphic)
        {
            switch (_propertyName)
            {
                case EPropertyName.RaycastTarget:
                    {
                        graphic.raycastTarget = _raycastTarget.GetValue(graphic.raycastTarget);
                        break;
                    }
                case EPropertyName.Color:
                    {
                        graphic.color = _color.propertyValue;
                        break;
                    }
                case EPropertyName.Image:
                    {
                        switch (_graphicType)
                        {
                            case GraphicType.Image:
                                {
                                    var image = graphic as Image;
                                    if (image)
                                    {
                                        image.sprite = _sprite.propertyValue;
                                    }
                                    break;
                                }
                            case GraphicType.RawImage:
                                {
                                    var rawImage = graphic as RawImage;
                                    if (rawImage)
                                    {
                                        rawImage.texture = _texture.propertyValue;
                                    }
                                    break;
                                }
                        }
                        break;
                    }
                case EPropertyName.Material:
                    {
                        graphic.material = _material.propertyValue;
                        break;
                    }
            }
        }

        /// <summary>
        /// 提示
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString()
        {
            return CommonFun.Name(_propertyName);
        }
    }
}
