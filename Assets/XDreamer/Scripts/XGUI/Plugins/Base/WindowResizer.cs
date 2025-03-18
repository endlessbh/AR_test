using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using XCSJ.Attributes;
using UnityEngine;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginXGUI.Base
{
    /// <summary>
    /// 窗口尺寸修改器
    /// </summary>
    [Name("窗口尺寸修改器")]
    public class WindowResizer : DraggableView, IPointerEnterHandler, IPointerExitHandler
    {
        /// <summary>
        /// 控制窗口
        /// </summary>
        [Name("控制窗口")]
        [Readonly(EEditorMode.Runtime)]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public SubWindow _subWindow;

        private Rect screenRect = new Rect(0, 0, 0, 0);
        private Image image = null;

        public enum EShowRule
        {
            [Name("无")]
            None,

            [Name("移入显示移出隐藏")]
            EnterShow_ExitHide,
        }

        [Name("显示规则")]
        [EnumPopup]
        public EShowRule _showRule = EShowRule.EnterShow_ExitHide;

        /// <summary>
        /// 唤醒
        /// </summary>
        protected virtual void Awake()
        {
            image = GetComponent<Image>();

            OnPointerExit(null); // 初始化
        }

        /// <summary>
        /// 开始拖拽
        /// </summary>
        /// <param name="eventData"></param>
        public override void OnBeginDrag(PointerEventData eventData)
        {
            base.OnBeginDrag(eventData);

            screenRect.width = Screen.width;
            screenRect.height = Screen.height;

            SetWindowLeftTopPivot();
        }

        /// <summary>
        /// 拖拽中
        /// </summary>
        /// <param name="eventData"></param>
        public override void OnDrag(PointerEventData eventData)
        {
            //base.OnDrag(eventData);

            if (!_subWindow) return;

            var offset = new Vector2(eventData.delta.x, -eventData.delta.y);// rectTransform坐标系是左下为原点

            var newSize = _subWindow.rectTransform.sizeDelta + offset;

            var r = _subWindow.rectTransform.GetScreenRect();

            // 宽度小于最小，或超过了屏幕宽度，则将偏移量设置为0
            if (newSize.x < _subWindow._minSize.x || (r.x + r.width > screenRect.width))
            {
                offset.x = 0;
            }

            // 高度小于最小，或超过了屏幕宽度，则将偏移量设置为0
            if (newSize.y < _subWindow._minSize.y || (r.y - r.height < 0))
            {
                offset.y = 0;
            }

            _subWindow.rectTransform.sizeDelta += offset;

        }

        /// <summary>
        /// 结束拖拽
        /// </summary>
        /// <param name="eventData"></param>
        public override void OnEndDrag(PointerEventData eventData)
        {
            base.OnEndDrag(eventData);

            RecoverWindowPivot();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_showRule == EShowRule.EnterShow_ExitHide && image)
            {
                var color = image.color;
                color.a = 1;
                image.color = color;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_showRule == EShowRule.EnterShow_ExitHide && image)
            {
                var color = image.color;
                color.a = 0;
                image.color = color;
            }
        }

        #region 窗口中心和位置设定

        private Vector2 orgPivot = Vector2.zero;
        private Vector3 pivotOffset = Vector2.zero;

        private void SetWindowLeftTopPivot()
        {
            if (_subWindow)
            {
                // 拖动窗口前，将窗口的中心设置为左上角
                orgPivot = _subWindow.rectTransform.pivot;
                _subWindow.rectTransform.pivot = new Vector2(0, 1);
                pivotOffset = orgPivot - _subWindow.rectTransform.pivot;

                // 因修改了中心点，所以需要重新设置位置
                _subWindow.rectTransform.position -= (Vector3)(pivotOffset * _subWindow.rectTransform.sizeDelta);
            }
        }

        private void RecoverWindowPivot()
        {
            if (_subWindow)
            {
                // 因修改了中心点，所以需要重新设置位置
                _subWindow.rectTransform.position += (Vector3)(pivotOffset * _subWindow.rectTransform.sizeDelta);
                _subWindow.rectTransform.pivot = orgPivot;
            }
        }
        #endregion
    }
}
