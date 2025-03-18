using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Units;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginTools.Notes.Dimensionings
{
    /// <summary>
    /// 尺寸数字
    /// </summary>
    [Name("尺寸数字")]
    [Serializable]
    public class SizeDigital
    {
        /// <summary>
        /// 画布
        /// </summary>
        [Name("画布")]
        [SerializeField]
        protected Canvas _canvas;

        /// <summary>
        /// 画布
        /// </summary>
        public Canvas canvas { get => _canvas; set => Update(value); }

        /// <summary>
        /// 文本
        /// </summary>
        [Name("文本")]
        [Readonly]
        public Text text;

        /// <summary>
        /// 缩放值
        /// </summary>
        [Name("缩放值")]
        [Range(0.0001f, 1f)]
        public float canvasScale = 0.04f;

        /// <summary>
        /// 更新文本
        /// </summary>
        [Name("更新文本")]
        public bool updateText = true;

        /// <summary>
        /// 小数位数
        /// </summary>
        [Name("小数位数")]
        [Range(0, 8)]
        public int decimalPlaces = 2;

        /// <summary>
        /// 显示单位
        /// </summary>
        [Name("显示单位")]
        public bool displayUnit = true;

        /// <summary>
        /// 文本内容
        /// </summary>
        [Name("文本内容")]
        public string textContent = "";

        /// <summary>
        /// 更新旋转
        /// </summary>
        [Name("更新旋转")]
        public bool updateRotation = true;

        /// <summary>
        /// 反向
        /// </summary>
        [Name("反向")]
        [HideInSuperInspector(nameof(updateRotation), EValidityCheckType.Equal, false)]
        public bool oppositeDirection = false;

        private const string DefaultName = nameof(SizeDigital) + "_" + nameof(Canvas);

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="mb"></param>
        /// <param name="parentTransform"></param>
        /// <param name="name"></param>
        public void Create(MB mb, Transform parentTransform, string name = DefaultName)
        {
            if (!text || !_canvas)
            {
                _canvas = new GameObject(string.IsNullOrEmpty(name) ? DefaultName : name, typeof(Canvas)).GetComponent<Canvas>();
                _canvas.renderMode = RenderMode.WorldSpace;

                {
                    text = AddTextToCanvas("", new GameObject("Text"));
                    text.alignment = TextAnchor.MiddleCenter;
                    text.horizontalOverflow = HorizontalWrapMode.Overflow;
                    text.verticalOverflow = VerticalWrapMode.Overflow;
                    text.transform.SetParent(_canvas.transform);
                    text.transform.localPosition = Vector3.zero;
                }

                if (parentTransform) _canvas.transform.SetParent(parentTransform);
                _canvas.transform.localPosition = Vector3.zero;
                _canvas.transform.localScale = Vector3.one * canvasScale;
            }
        }

        /// <summary>
        /// 启用
        /// </summary>
        /// <param name="mb"></param>
        public void OnEnable(MB mb)
        {
            if (_canvas) _canvas.enabled = true;
        }

        /// <summary>
        /// 禁用
        /// </summary>
        /// <param name="mb"></param>
        public void OnDisable(MB mb)
        {
            if (_canvas) _canvas.enabled = false;
        }

        /// <summary>
        /// 更新画布
        /// </summary>
        /// <param name="canvas"></param>
        public void Update(Canvas canvas)
        {
            this._canvas = canvas;
            text = canvas ? canvas.GetComponentInChildren<Text>() : null;
        }

        private Text AddTextToCanvas(string textString, GameObject textGameObject)
        {
            Text text = textGameObject.AddComponent<Text>();
            text.text = textString;

            Font ArialFont = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
            text.font = ArialFont;
            text.material = ArialFont.material;

            return text;
        }

        /// <summary>
        /// 可见与否
        /// </summary>
        /// <param name="isVisible"></param>
        public void SetVisible(bool isVisible)
        {
            if (canvas) canvas.enabled = isVisible;
        }
    }

    /// <summary>
    /// 距离尺寸数字
    /// </summary>
    [Name("距离尺寸数字")]
    [Serializable]
    public class DistanceSizeDigital : SizeDigital
    {
        /// <summary>
        /// 距离单位
        /// </summary>
        [Name("距离单位")]
        [EnumPopup]
        public ELengthUnits distanceUnit = ELengthUnits.M;

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <param name="direction"></param>
        public void Update(Vector3 begin, Vector3 end, Vector3 direction)
        {
            var disVector = end - begin;
            if (updateText)
            {
                textContent = Convert.ToDouble(disVector.magnitude).ToString(ELengthUnits.M, distanceUnit, decimalPlaces, displayUnit);
            }
            if (text) text.text = textContent;
            if (canvas)
            {
                var transform = canvas.transform;
                transform.position = (begin + end) / 2;
                transform.localScale = Vector3.one * canvasScale;

                if (updateRotation)
                {
                    var lookat = Vector3.Cross(disVector, direction);
                    if (lookat != Vector3.zero)
                    {
                        transform.rotation = Quaternion.LookRotation((oppositeDirection ? -1 : 1) * lookat, direction);
                    }
                }
            }            
        }
    }

    /// <summary>
    /// 角度尺寸数字
    /// </summary>
    [Name("角度尺寸数字")]
    [Serializable]
    public class AngleSizeDigital : SizeDigital
    {
        /// <summary>
        /// 角度单位
        /// </summary>
        [Name("角度单位")]
        [EnumPopup]
        public EAngleUnit angleUnit = EAngleUnit.Degrees;

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <param name="center"></param>
        /// <param name="angle"></param>
        public void Update(Vector3 begin, Vector3 end, Vector3 center, float angle)
        {
            if (updateText)
            {              
                textContent = Convert.ToDouble(angle).ToString(EAngleUnit.Degrees, angleUnit, decimalPlaces, displayUnit);
            }
            if (text) text.text = textContent;
            if (canvas)
            {
                var pos = (begin + end) / 2;
                var transform = canvas.transform;
                transform.position = pos;
                transform.localScale = Vector3.one * canvasScale;

                if (updateRotation)
                {
                    var disVector = end - begin;
                    var direction = pos - center;

                    var lookat = Vector3.Cross(disVector, direction);
                    if (lookat != Vector3.zero)
                    {
                        transform.rotation = Quaternion.LookRotation((oppositeDirection ? -1 : 1) * lookat, direction);
                    }
                }
            }
        }
    }
}
