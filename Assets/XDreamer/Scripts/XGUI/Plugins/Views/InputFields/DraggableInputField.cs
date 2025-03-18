using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXGUI.Base;
using static UnityEngine.UI.InputField;

namespace XCSJ.PluginXGUI.Views.InputFields
{
    /// <summary>
    /// 可拖拽数值输入框:数值类型的输入框随着拖拽，数值发生变化
    /// </summary>
    [Name("可拖拽数值输入框")]
    [RequireComponent(typeof(InputField))]
    public sealed class DraggableInputField : View, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        /// <summary>
        /// 输入框
        /// </summary>
        [Name("输入框")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public InputField _inputField;

        /// <summary>
        /// 数值增减速度
        /// </summary>
        [Name("数值增减速度")]
        public float _speed = 0.1f;

        [Name("数值拖拽触发按键")]
        public PointerEventData.InputButton _dragInputButton = PointerEventData.InputButton.Right;

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            _inputField.onEndEdit.AddListener(OnEndEdit);

            var contentType = _inputField.contentType;
            if (contentType != ContentType.IntegerNumber && contentType != ContentType.DecimalNumber)
            {
                Debug.LogWarningFormat("{0} contentType must setted to IntegerNumber or DecimalNumber", CommonFun.GameObjectComponentToString(_inputField));
            }
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();


            _inputField.onEndEdit.RemoveListener(OnEndEdit);
        }

        /// <summary>
        /// 当输入框为空时设置为0
        /// </summary>
        /// <param name="value"></param>
        private void OnEndEdit(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                _inputField.text = "0";
            }
        }

        private bool validDragging = false;
        private float value = 0f;

        /// <summary>
        /// 按下
        /// </summary>
        /// <param name="eventData"></param>
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!_inputField.IsInteractable() || eventData.button != _dragInputButton) return;

            if (!float.TryParse(_inputField.text, out value))
            {
                value = 0;
            }
            validDragging = true;
            CommonFun.BeginOnUI();
        }

        /// <summary>
        /// 拖拽
        /// </summary>
        /// <param name="eventData"></param>
        public void OnDrag(PointerEventData eventData)
        {
            if (validDragging)
            {
                float x = eventData.delta.x, y = eventData.delta.y;
                value += (Mathf.Abs(x) > Mathf.Abs(y) ? x : y) * _speed;
                _inputField.text = _inputField.contentType == ContentType.DecimalNumber ? value.ToString("F") : ((int)value).ToString();
            }
        }

        /// <summary>
        /// 弹起
        /// </summary>
        /// <param name="eventData"></param>
        public void OnEndDrag(PointerEventData eventData)
        {
            if (validDragging)
            {
                CommonFun.EndOnUI();
            }
            validDragging = false;
        }
    }
}
