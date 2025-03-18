using System;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXGUI.Styles.Base;
using static UnityEngine.UI.Selectable;

namespace XCSJ.PluginXGUI.Styles.Elements
{
    /// <summary>
    /// 可选择样式元素
    /// </summary>
    public abstract class SelectableStyleElement : BaseStyleElement
    {
        /// <summary>
        /// 样式参数设置
        /// </summary>
        [Flags]
        [Name("样式参数设置")]
        private enum EParamsSetting
        {
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
        }

        /// <summary>
        /// 过渡
        /// </summary>
        [Name("过渡")]
        public Transition _transition = Transition.ColorTint;

        /// <summary>
        /// 颜色过渡
        /// </summary>
        [Name("颜色过渡")]
        [HideInSuperInspector(nameof(_transition), EValidityCheckType.NotEqual, Transition.ColorTint)]
        public ColorBlock _colorBlock;

        /// <summary>
        /// 精灵状态
        /// </summary>
        [Name("精灵状态")]
        [HideInSuperInspector(nameof(_transition), EValidityCheckType.NotEqual, Transition.SpriteSwap)]
        public SpriteState _spriteState;

        /// <summary>
        /// 动画触发器
        /// </summary>
        [Name("动画触发器")]
        [HideInSuperInspector(nameof(_transition), EValidityCheckType.NotEqual, Transition.Animation)]
        public AnimationTriggers _animationTriggers;

        /// <summary>
        /// 重置
        /// </summary>
        protected override void Reset()
        {
            base.Reset();

            _transition = Transition.ColorTint;
            _colorBlock.normalColor = Color.white;
            var hv = 245.0f / 255;
            _colorBlock.highlightedColor = new Color(hv, hv, hv, 1);

            var pv = 200.0f / 255;
            _colorBlock.pressedColor = new Color(pv, pv, pv, 1);
            _colorBlock.disabledColor = new Color(pv, pv, pv, 128.0f / 255);
            _colorBlock.colorMultiplier = 1;
            _colorBlock.fadeDuration = 0.1f;
        }

        /// <summary>
        /// 样式应用对象
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="paramsMask">参数掩码：子类根据比特位为0或1来设置对应的参数</param>
        /// <returns></returns>
        protected override bool OnStyleToObject(UnityEngine.Object obj, int paramsMask)
        {
            return StyleHelper.ModifyObjectPropertyWithMask(obj as Selectable, typeof(EParamsSetting), paramsMask, (selectable, paramsSetting) =>
            {
                switch (paramsSetting)
                {
                    case EParamsSetting.Transition: selectable.transition = _transition; break;
                    case EParamsSetting.ColorBlock: selectable.colors = _colorBlock; break;
                    case EParamsSetting.SpriteState: selectable.spriteState = _spriteState; break;
                    case EParamsSetting.AnimationTriggers: selectable.animationTriggers = _animationTriggers; break;
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
            return StyleHelper.ModifyObjectPropertyWithMask(obj as Selectable, typeof(Selectable), paramsMask, (selectable, paramsSetting) =>
            {
                switch (paramsSetting)
                {
                    case EParamsSetting.Transition: _transition = selectable.transition; break;
                    case EParamsSetting.ColorBlock: _colorBlock = selectable.colors; break;
                    case EParamsSetting.SpriteState: _spriteState = selectable.spriteState; break;
                    case EParamsSetting.AnimationTriggers: _animationTriggers = selectable.animationTriggers; break;
                }
            });
        }
    }
}
