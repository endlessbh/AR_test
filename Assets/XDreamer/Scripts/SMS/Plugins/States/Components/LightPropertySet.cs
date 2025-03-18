using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;

namespace XCSJ.PluginSMS.States.Components
{
    /// <summary>
    /// 灯光属性设置:设置灯光的颜色、强度等
    /// </summary>
    [ComponentMenu(SMSHelperExtension.ComponentCategoryName + "/" + Title, typeof(SMSManager))]
    [Name(Title, nameof(LightPropertySet))]
    [Tip("设置灯光的颜色、强度等", "Set the color, intensity, etc. of the light")]
    public class LightPropertySet : ComponentPropertySet<LightPropertySet, Light>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "灯光属性设置";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(SMSHelperExtension.ComponentCategoryName, typeof(SMSManager))]
        [Name(Title, nameof(LightPropertySet))]
        [Tip("设置灯光的颜色、强度等", "Set the color, intensity, etc. of the light")]
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
            /// 颜色
            /// </summary>
            [Name("颜色")]
            Color,

            /// <summary>
            /// 强度
            /// </summary>
            [Name("强度")]
            Intensity,

            /// <summary>
            /// 范围
            /// </summary>
            [Name("范围")]
            [Tip("仅点光源与聚光灯有效", "Only point and spot lights are valid")]
            Range,

            /// <summary>
            /// 投射角度
            /// </summary>
            [Name("投射角度")]
            [Tip("仅聚光灯有效", "Spotlight only")]
            SpotAngle,
        }

        /// <summary>
        /// 属性名称
        /// </summary>
        [Name("属性名称")]
        [EnumPopup]
        public EPropertyName _propertyName = EPropertyName.Color;

        /// <summary>
        /// 颜色
        /// </summary>
        [Name("颜色")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Color)]
        [OnlyMemberElements]
        public ColorPropertyValue _color = new ColorPropertyValue();

        /// <summary>
        /// 强度
        /// </summary>
        [Name("强度")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Intensity)]
        [OnlyMemberElements]
        public FloatPropertyValue _intensity = new FloatPropertyValue();

        /// <summary>
        /// 范围
        /// </summary>
        [Name("范围")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Range)]
        [OnlyMemberElements]
        public FloatPropertyValue _range = new FloatPropertyValue();

        /// <summary>
        /// 投射角度
        /// </summary>
        [Name("投射角度")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.SpotAngle)]
        [OnlyMemberElements]
        public FloatPropertyValue _spotAngle = new FloatPropertyValue();

        /// <summary>
        /// 设置组件属性
        /// </summary>
        /// <param name="light"></param>
        protected override void SetComponentProperty(Light light)
        {
            switch (_propertyName)
            {
                case EPropertyName.Color: light.color = _color.propertyValue; break;
                case EPropertyName.Intensity: light.intensity = _intensity.propertyValue; break;
                case EPropertyName.Range: light.range = _range.propertyValue; break;
                case EPropertyName.SpotAngle: light.spotAngle = _spotAngle.propertyValue; break;
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
