using System;
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
    [StyleLink(typeof(ScrollRect), typeof(EParamsSetting))]
    [Name("滚动矩形")]
    public class ScrollRectStyleElement : BaseStyleElement
    {
        /// <summary>
        /// 样式参数设置
        /// </summary>
        [Flags]
        [Name("样式参数设置")]
        private enum EParamsSetting
        {
            [Name("水平")]
            [EnumFieldName("水平")]
            Horizontal = 1 << 0,

            [Name("垂直")]
            [EnumFieldName("垂直")]
            Vertical = 1 << 1,

            [Name("运动类型")]
            [EnumFieldName("运动类型")]
            MovementType = 1 << 2,

            [Name("惯性")]
            [EnumFieldName("惯性")]
            Inertia = 1 << 3,

            [Name("减速率")]
            [EnumFieldName("减速率")]
            DecelerationRate = 1 << 4,

            [Name("滚动灵敏度")]
            [EnumFieldName("滚动灵敏度")]
            ScrollSensitivity = 1 << 5,

            [Name("可见性")]
            [EnumFieldName("可见性")]
            Visibility = 1 << 6,

            [Name("间距")]
            [EnumFieldName("间距")]
            Spacing = 1 << 7
        }

        /// <summary>
        /// 水平
        /// </summary>
        [Name("水平")]
        public bool _horizontal = false;

        /// <summary>
        /// 垂直
        /// </summary>
        [Name("垂直")]
        public bool _vertical = true;

        /// <summary>
        /// 运动类型
        /// </summary>
        [Name("运动类型")]
        public ScrollRect.MovementType _movementType = ScrollRect.MovementType.Clamped;

        /// <summary>
        /// 惯性
        /// </summary>
        [Name("惯性")]
        public bool _inertia = true;

        /// <summary>
        /// 减速率
        /// </summary>
        [Name("减速率")]
        public float _decelerationRate = 0.135f;

        /// <summary>
        /// 滚动灵敏度
        /// </summary>
        [Name("滚动灵敏度")]
        public float _scrollSensitivity = 1;

        /// <summary>
        /// 水平滚动条可见性
        /// </summary>
        [Name("水平滚动条可见性")]
        [HideInSuperInspector(nameof(_horizontal), EValidityCheckType.False)]
        public ScrollRect.ScrollbarVisibility _horizontalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;

        /// <summary>
        /// 水平滚动条
        /// </summary>
        [Name("水平滚动条间距")]
        [HideInSuperInspector(nameof(_horizontal), EValidityCheckType.False)]
        public float _horizontalScrollbarSpacing = -3;

        /// <summary>
        /// 垂直滚动条可见性
        /// </summary>
        [Name("垂直滚动条可见性")]
        [HideInSuperInspector(nameof(_vertical), EValidityCheckType.False)]
        public ScrollRect.ScrollbarVisibility _verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;

        /// <summary>
        /// 垂直滚动条间距
        /// </summary>
        [Name("垂直滚动条间距")]
        [HideInSuperInspector(nameof(_vertical), EValidityCheckType.False)]
        public float _verticalScrollbarSpacing = -3;

        /// <summary>
        /// 样式应用对象
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="paramsMask"></param>
        /// <returns></returns>
        protected override bool OnStyleToObject(UnityEngine.Object obj, int paramsMask)
        {
            return StyleHelper.ModifyObjectPropertyWithMask(obj as ScrollRect, typeof(EParamsSetting), paramsMask, (scrollRect, paramsSetting) =>
            {
                switch (paramsSetting)
                {
                    case EParamsSetting.Horizontal: scrollRect.horizontal = _horizontal; break;
                    case EParamsSetting.Vertical: scrollRect.vertical = _vertical; break;
                    case EParamsSetting.MovementType: scrollRect.movementType = _movementType; break;
                    case EParamsSetting.Inertia: scrollRect.inertia = _inertia; break;
                    case EParamsSetting.DecelerationRate: scrollRect.decelerationRate = _decelerationRate; break;
                    case EParamsSetting.ScrollSensitivity: scrollRect.scrollSensitivity = _scrollSensitivity; break;
                    case EParamsSetting.Visibility:
                        {
                            scrollRect.horizontalScrollbarVisibility = _horizontalScrollbarVisibility;
                            scrollRect.verticalScrollbarVisibility = _verticalScrollbarVisibility;
                            break;
                        }
                    case EParamsSetting.Spacing:
                        {
                            scrollRect.horizontalScrollbarSpacing = _horizontalScrollbarSpacing;
                            scrollRect.verticalScrollbarSpacing = _verticalScrollbarSpacing;
                            break;
                        }
                }
            });

        }

        /// <summary>
        /// 从对象中提取样式
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="paramsMask"></param>
        /// <returns></returns>
        protected override bool OnObjectToStyle(UnityEngine.Object obj, int paramsMask)
        {
            return StyleHelper.ModifyObjectPropertyWithMask(obj as ScrollRect, typeof(EParamsSetting), paramsMask, (scrollRect, paramsSetting) =>
            {
                switch (paramsSetting)
                {
                    case EParamsSetting.Horizontal: _horizontal = scrollRect.horizontal; break;
                    case EParamsSetting.Vertical: _vertical = scrollRect.vertical; break;
                    case EParamsSetting.MovementType: _movementType = scrollRect.movementType; break;
                    case EParamsSetting.Inertia: _inertia = scrollRect.inertia; break;
                    case EParamsSetting.DecelerationRate: _decelerationRate = scrollRect.decelerationRate; break;
                    case EParamsSetting.ScrollSensitivity: _decelerationRate = scrollRect.decelerationRate; break;
                    case EParamsSetting.Visibility:
                        {
                            _horizontalScrollbarVisibility = scrollRect.horizontalScrollbarVisibility;
                            _verticalScrollbarVisibility = scrollRect.verticalScrollbarVisibility;
                            break;
                        }
                    case EParamsSetting.Spacing:
                        {
                            _horizontalScrollbarSpacing = scrollRect.horizontalScrollbarSpacing;
                            _verticalScrollbarSpacing = scrollRect.verticalScrollbarSpacing;
                            break;
                        }
                }
            });
        }
    }
}
