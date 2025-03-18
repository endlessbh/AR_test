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
    /// 滚动矩形
    /// </summary>
    [StyleLink(typeof(Scrollbar), typeof(EParamsSetting))]
    [Name("滚动条")]
    public class ScrollBarStyleElement : SelectableStyleElement
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

            [Name("移动步长")]
            [EnumFieldName("移动步长")]
            NumberOfSteps = 1 << 6,
        }

        /// <summary>
        /// 方向
        /// </summary>
        [Name("方向")]
        public Scrollbar.Direction _direction = Scrollbar.Direction.BottomToTop;

        /// <summary>
        /// 移动步长
        /// </summary>
        [Name("移动步长")]
        [Range(0, 11)]
        public int _numberOfSteps = 0;

        /// <summary>
        /// 样式应用对象
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="paramsMask"></param>
        /// <returns></returns>
        protected override bool OnStyleToObject(UnityEngine.Object obj, int paramsMask)
        {
            var rs1 = base.OnStyleToObject(obj, paramsMask);
            var rs2 = StyleHelper.ModifyObjectPropertyWithMask(obj as Scrollbar, typeof(EParamsSetting), paramsMask, (scrollbar, paramsSetting) =>
            {
                switch (paramsSetting)
                {
                    case EParamsSetting.Direction: scrollbar.direction = _direction; break;
                    case EParamsSetting.NumberOfSteps: scrollbar.numberOfSteps = _numberOfSteps; break;
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
            var rs2 = StyleHelper.ModifyObjectPropertyWithMask(obj as Scrollbar, typeof(EParamsSetting), paramsMask, (scrollbar, paramsSetting) =>
            {
                switch (paramsSetting)
                {
                    case EParamsSetting.Direction: _direction = scrollbar.direction; break;
                    case EParamsSetting.NumberOfSteps: _numberOfSteps = scrollbar.numberOfSteps; break;
                }
            });
            return rs1 || rs2;
        }
    }
}
