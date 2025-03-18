using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;
using XCSJ.Scripts;

namespace XCSJ.PluginSMS.States.Components
{
    /// <summary>
    /// 渲染器属性设置:设置渲染器的颜色、属性等
    /// </summary>
    [ComponentMenu(SMSHelperExtension.ComponentCategoryName + "/" + Title, typeof(SMSManager))]
    [Name(Title, nameof(RenderPropertySet))]
    [Tip("设置材质的颜色、自发光等属性", "Set the color, self illumination and other properties of the material")]
    [XCSJ.Attributes.Icon(EIcon.Property)]
    public class MaterialPropertySet : LifecycleExecutor<MaterialPropertySet>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "材质属性设置";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(SMSHelperExtension.ComponentCategoryName, typeof(SMSManager))]
        [Name(Title, nameof(RenderPropertySet))]
        [Tip("设置材质的颜色、自发光等属性", "Set the color, self illumination and other properties of the material")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 材质列表
        /// </summary>
        [Name("材质列表")] 
        public List<Material> _materials = new List<Material>();

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
            /// 贴图
            /// </summary>
            [Name("贴图")]
            Texture,

            /// <summary>
            /// 自发光启用禁用
            /// </summary>
            [Name("自发光启用禁用")]
            EmissionEnable,

            /// <summary>
            /// 颜色
            /// </summary>
            [Name("自发光颜色")]
            EmissionColor,
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
        /// 主贴图
        /// </summary>
        [Name("主贴图")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Texture)]
        [OnlyMemberElements]
        public TexturePropertyValue _mainTexture = new TexturePropertyValue();

        /// <summary>
        /// 自发光启用禁用
        /// </summary>
        [Name("自发光启用禁用")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.EmissionEnable)]
        [OnlyMemberElements]
        public EBoolPropertyValue _emissionEnable = new EBoolPropertyValue();

        /// <summary>
        /// 自发光颜色
        /// </summary>
        [Name("自发光颜色")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.EmissionColor)]
        [OnlyMemberElements]
        public ColorPropertyValue _emissionColor = new ColorPropertyValue();

        /// <summary>
        /// 提示
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString()
        {
            return CommonFun.Name(_propertyName);
        }

        /// <summary>
        /// 设置
        /// </summary>
        /// <param name="stateData"></param>
        /// <param name="executeMode"></param>
        public override void Execute(StateData stateData, EExecuteMode executeMode)
        {
            foreach (var m in _materials)
            {
                if (!m) continue;

                switch (_propertyName)
                {
                    case EPropertyName.Color:
                        {
                            m.color = _color.propertyValue;
                            break;
                        }
                    case EPropertyName.Texture:
                        {
                            m.mainTexture = _mainTexture.propertyValue;
                            break;
                        }
                    case EPropertyName.EmissionEnable:
                        {
                            switch (_emissionEnable.GetValue(EBool.None))
                            {
                                case EBool.Yes:
                                    {
                                        m.EnableKeyword("_EMISSION"); 
                                        break;
                                    }
                                case EBool.No:
                                    {
                                        m.DisableKeyword("_EMISSION"); 
                                        break;
                                    }
                                case EBool.Switch:
                                    {
                                        if (m.IsKeywordEnabled("_EMISSION"))
                                        {
                                            m.DisableKeyword("_EMISSION");
                                        }
                                        else
                                        {
                                            m.EnableKeyword("_EMISSION");
                                        }
                                        break;
                                    }
                            }
                            break;
                        }
                    case EPropertyName.EmissionColor:
                        {
                            m.SetColor("_EmissionColor", _emissionColor.propertyValue);
                            break;
                        }
                }
            }
        }
    }
}
