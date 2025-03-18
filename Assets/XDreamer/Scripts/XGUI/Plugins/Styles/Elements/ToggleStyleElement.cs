using System;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Attributes;
using XCSJ.PluginXGUI.Styles.Base;

namespace XCSJ.PluginXGUI.Styles.Elements
{
    /// <summary>
    /// 切换样式
    /// </summary>
    [StyleLink(typeof(Toggle), typeof(EParamsSetting))]
    [Name("切换")]
    public class ToggleStyleElement : SelectableStyleElement
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

            [Name("切换过渡")]
            [EnumFieldName("切换过渡")]
            ToggleTransition = 1 << 4,
        }

        /// <summary>
        /// 切换过渡
        /// </summary>
        [Name("切换过渡")]
        public Toggle.ToggleTransition _toggleTransition;

        /// <summary>
        /// 样式应用对象
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="paramsMask"></param>
        /// <returns></returns>
        protected override bool OnStyleToObject(UnityEngine.Object obj, int paramsMask)
        {
            var rs1 = base.OnStyleToObject(obj, paramsMask);
            var rs2 = StyleHelper.ModifyObjectPropertyWithMask(obj as Toggle, typeof(EParamsSetting), paramsMask, (toggle, paramsSetting) =>
            {
                switch (paramsSetting)
                {
                    case EParamsSetting.ToggleTransition: toggle.toggleTransition = _toggleTransition; break;
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
            var rs2 = StyleHelper.ModifyObjectPropertyWithMask(obj as Toggle, typeof(EParamsSetting), paramsMask, (toggle, paramsSetting) =>
            {
                switch (paramsSetting)
                {
                    case EParamsSetting.ToggleTransition: _toggleTransition = toggle.toggleTransition; break;
                }
            });
            return rs1 || rs2;
        }
    }

}
