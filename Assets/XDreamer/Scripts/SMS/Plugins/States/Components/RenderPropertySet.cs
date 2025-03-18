using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;

namespace XCSJ.PluginSMS.States.Components
{
    /// <summary>
    /// 渲染器属性设置:设置渲染器的颜色、属性等
    /// </summary>
    [ComponentMenu(SMSHelperExtension.ComponentCategoryName + "/" + Title, typeof(SMSManager))]
    [Name(Title, nameof(RenderPropertySet))]
    [Tip("设置渲染器的颜色、属性等", "Set the colors, properties, and so on of the renderer")]
    public class RenderPropertySet : ComponentPropertySet<RenderPropertySet, Renderer>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "渲染器属性设置";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(SMSHelperExtension.ComponentCategoryName, typeof(SMSManager))]
        [Name(Title, nameof(RenderPropertySet))]
        [Tip("设置渲染器的颜色、属性等", "Set the colors, properties, and so on of the renderer")]
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
            /// 材质
            /// </summary>
            [Name("材质")]
            Material,

            /// <summary>
            /// 批量材质
            /// </summary>
            [Name("批量材质")]
            MaterialBatch,
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
        /// 材质
        /// </summary>
        [Name("材质")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Material)]
        [OnlyMemberElements]
        public MaterialPropertyValue _material = new MaterialPropertyValue();

        /// <summary>
        /// 批量材质
        /// </summary>
        [Name("批量材质")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.MaterialBatch)]
        public Material[] _materials = null;

        /// <summary>
        /// 材质名称过滤器 ：当对象有多个材质的时候，可使用过滤规则精确定位需要修改的材质颜色
        /// </summary>
        [Name("材质名称过滤器")]
        [Tip("当对象有多个材质的时候，可使用过滤规则精确定位需要修改的材质颜色", "When an object has multiple materials, you can use filtering rules to accurately locate the material color that needs to be modified")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.Equal, EPropertyName.MaterialBatch)]
        public XStringComparer _comparer = new XStringComparer();

        /// <summary>
        /// 设置组件属性
        /// </summary>
        /// <param name="renderer"></param>
        protected override void SetComponentProperty(Renderer renderer)
        {
            switch (_propertyName)
            {
                case EPropertyName.Color:
                    {
                        foreach (var m in renderer.materials)
                        {
                            if (m && _comparer.IsMatch(m.name))
                            {
                                m.color = _color.propertyValue;
                            }
                        }
                        break;
                    }
                case EPropertyName.Material:
                    {
                        var newMats = renderer.materials.ToArray();
                        for (int i = 0; i < newMats.Length; i++)
                        {
                            var m = renderer.materials[i];
                            if (m && _comparer.IsMatch(m.name))
                            {
                                newMats[i] = _material.propertyValue;
                            }
                        }
                        renderer.materials = newMats;
                        break;
                    }
                case EPropertyName.MaterialBatch:
                    {
                        renderer.materials = _materials;
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
