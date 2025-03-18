using System;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Attributes;
using XCSJ.PluginXGUI.Styles.Base;

namespace XCSJ.PluginXGUI.Styles.Elements
{
    /// <summary>
    /// 按钮样式
    /// </summary>
    [StyleLink(typeof(Button), typeof(EParamsSetting))]
    [Name("按钮")]
    public class ButtonStyleElement : SelectableStyleElement
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
        }
    }
}
