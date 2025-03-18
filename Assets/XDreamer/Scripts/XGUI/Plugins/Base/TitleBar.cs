using UnityEngine.Serialization;
using XCSJ.Attributes;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginXGUI.Base
{
    /// <summary>
    /// 标题栏
    /// </summary>
    [Name("标题栏")]
    public class TitleBar : View
    {
        /// <summary>
        /// 标题栏
        /// </summary>
        [Name("标题")]
        public RectTransform _title;

        /// <summary>
        /// 锁定
        /// </summary>
        [Name("锁定")]
        public Toggle _locker;

        /// <summary>
        /// 展开
        /// </summary>
        [Name("展开")]
        public Toggle _expand;

        /// <summary>
        /// 自动旋转折叠图像
        /// </summary>
        [Name("自动旋转折叠图像")]
        public bool _autoRotateExpandImage = true;

        /// <summary>
        /// 展开对象
        /// </summary>
        [Name("展开对象")]
        [HideInSuperInspector(nameof(_autoRotateExpandImage), EValidityCheckType.False)]
        public RectTransform _expandObject;

        /// <summary>
        /// 折叠对象
        /// </summary>
        [Name("折叠对象")]
        [HideInSuperInspector(nameof(_autoRotateExpandImage), EValidityCheckType.False)]
        public RectTransform _unexpandObject;

        /// <summary>
        /// 最大化
        /// </summary>
        [Name("最大化")]
        public RectTransform _maxSize;

        /// <summary>
        /// 关闭
        /// </summary>
        [Name("关闭")]
        public Button _closeButton;

        /// <summary>
        /// 布局元素
        /// </summary>
        [Name("布局顺序")]
        [FormerlySerializedAs("layoutUnit")]
        public List<RectTransform> _layoutUnit = new List<RectTransform>();

        /// <summary>
        /// 子窗口
        /// </summary>
        protected SubWindow subWindow
        {
            get
            {
                if (!_subWindow)
                {
                    _subWindow = GetComponentInParent<SubWindow>();
                }
                return _subWindow;
            }
        }
        private SubWindow _subWindow;

        /// <summary>
        /// 布局
        /// </summary>
        /// <param name="isHorizontal">水平还是垂直</param>
        /// <param name="fixSize">单个元素偏移量</param>
        public void Layout(bool isHorizontal, float fixSize)
        {
            // 根据水平和垂直布局，设置文字方向
            var titleText = _title.GetComponent<Text>();
            if (titleText)
            {
                titleText.alignment = isHorizontal ? TextAnchor.MiddleLeft : TextAnchor.UpperCenter;
            }

            var rect = rectTransform.rect;
            int index = 0;
            var offset = fixSize / 2;
            for (int i = _layoutUnit.Count - 1; i >= 0; i--)
            {
                var unit = _layoutUnit[i];
                if (unit.gameObject.activeSelf)
                {
                    // 水平方向，按钮控制元素是右中对齐
                    if (isHorizontal)
                    {
                        unit.anchorMin = unit.anchorMax = new Vector2(1, 0.5f);
                        unit.anchoredPosition = new Vector2(-offset, 0);
                    }
                    else // 垂直方向，按钮控制元素是底中对齐
                    {
                        unit.anchorMin = unit.anchorMax = new Vector2(0.5f, 0);
                        unit.anchoredPosition = new Vector2(0, offset);
                    }
                    ++index;
                    offset += fixSize;
                }
            }
        }

        /// <summary>
        /// 设置折叠图标随着在窗口中的方位进行旋转
        /// </summary>
        /// <param name="titleDirection"></param>
        public void SetExpandRotation(EFourDirection titleDirection)
        {
            if (!_expandObject || !_unexpandObject || !_autoRotateExpandImage) return;

            var expandAngle = Vector3.zero;

            switch (titleDirection)
            {
                case EFourDirection.Top:
                    {
                        expandAngle = new Vector3(0, 0, -90);
                        break;
                    }
                case EFourDirection.Bottom:
                    {
                        expandAngle = new Vector3(0, 0, 90);
                        break;
                    }
                case EFourDirection.Left:
                    {
                        expandAngle = new Vector3(0, 0, 0);
                        break;
                    }
                case EFourDirection.Right:
                    {
                        expandAngle = new Vector3(0, 0, -180);
                        break;
                    }
            }
            _expandObject.eulerAngles = expandAngle;
            expandAngle.z += 180;
            _unexpandObject.eulerAngles = expandAngle;
        }
    }
}
