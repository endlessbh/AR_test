using System;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Attributes;
using XCSJ.PluginXGUI.Styles.Base;

namespace XCSJ.PluginXGUI.Styles.Elements
{
    /// <summary>
    /// 滑竿样式
    /// </summary>
    [StyleLink(typeof(Slider), typeof(EParamsSetting))]
    [Name("滑竿")]
    public class SliderStyleElement : SelectableStyleElement
    {
        /// <summary>
        /// 样式参数设置
        /// </summary>
        [Flags]
        [Name("样式参数设置")]
        private enum EParamsSetting
        {
            #region 基类参数

            [Name("过渡")]
            [EnumFieldName("过渡")]
            Transition = 1 << 0,

            [Name("颜色过渡")]
            [EnumFieldName("颜色过渡")]
            ColorBlock = 1 << 1,

            [Name("精灵状态")]
            [EnumFieldName("材质")]
            SpriteState = 1 << 2,

            [Name("动画触发器")]
            [EnumFieldName("动画触发器")]
            AnimationTriggers = 1 << 3,

            #endregion

            [Name("方向")]
            [EnumFieldName("方向")]
            Direction = 1 << 4,
        }

        /// <summary>
        /// 方向
        /// </summary>
        [Name("方向")]
        public Slider.Direction _direction = Slider.Direction.LeftToRight;

        /// <summary>
        /// 样式应用对象
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="paramsMask"></param>
        /// <returns></returns>
        protected override bool OnStyleToObject(UnityEngine.Object obj, int paramsMask)
        {
            var rs1 = base.OnStyleToObject(obj, paramsMask);
            var rs2 = StyleHelper.ModifyObjectPropertyWithMask(obj as Slider, typeof(EParamsSetting), paramsMask, (slider, paramsSetting) =>
            {
                switch (paramsSetting)
                {
                    case EParamsSetting.Direction: slider.direction = _direction; break;
                }
            });
            return rs1 || rs2;
        }

        /// <summary>
        /// 从对象中提取样式
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="paramsMask"></param>
        /// <returns></returns>
        protected override bool OnObjectToStyle(UnityEngine.Object obj, int paramsMask)
        {
            var rs1 = base.OnObjectToStyle(obj, paramsMask);
            var rs2 = StyleHelper.ModifyObjectPropertyWithMask(obj as Slider, typeof(EParamsSetting), paramsMask, (slider, paramsSetting) =>
            {
                switch (paramsSetting)
                {
                    case EParamsSetting.Direction: _direction = slider.direction; break;
                }
            });
            return rs1 || rs2;
        }
    }

}
