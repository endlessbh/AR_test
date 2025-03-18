using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Maths;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginTools;
using XCSJ.PluginXGUI.Base;

namespace XCSJ.PluginXGUI.Windows.ColorPickers
{
    /// <summary>
    /// 颜色拾取器
    /// </summary>

    [Name("颜色拾取器")]
    [RequireManager(typeof(XGUIManager))]
    public class ColorPicker : View
    {
        /// <summary>
        /// 颜色已变更事件：当颜色发生变化后回调
        /// </summary>
        public static event Action<ColorPicker, Color> onColorChanged;

        /// <summary>
        /// HSV已变更事件：当颜HSV生变化后回调
        /// </summary>
        public static event Action<ColorPicker, Vector3> onHSVChanged;

        /// <summary>
        /// 颜色
        /// </summary>
        public Color color
        {
            get => _color;
            set
            {
                if (_color == value) return;

                _color = value;

                // 在hsv空间中，当R=G=B等于相同值时，H值会变为相同
                var tmpColor = _normalizedHSV.HSVToColor();
                if (_color != tmpColor)
                {
                    _normalizedHSV = _color.ToNormalizedHSV();
                }

                SendChangedEvent();
            }
        }

        /// <summary>
        /// 当前颜色
        /// </summary>
        [Name("当前颜色")]
        [SerializeField]
        private Color _color = Color.red;

        /// <summary>
        /// 色调 X:H 0~360
        /// 饱和度 Y: S 0~100
        /// 明度 Z: V 0~100
        /// </summary>
        public Vector3Int hsv
        {
            get => new Vector3Int(h, s, v);
            set => normalizedHSV = new Vector3(value.x / 360f, value.y / 100f, value.z / 100f);
        }

        /// <summary>
        /// 色调、饱和度、明度 0~1
        /// </summary>
        public Vector3 normalizedHSV
        {
            get => _normalizedHSV;
            set
            {
                _normalizedHSV = value;
                HSVChange();
            }
        }

        private Vector3 _normalizedHSV = Vector3.zero;

        /// <summary>
        /// 红色 0~255
        /// </summary>
        public int r255
        {
            get => (int)(normalizedR * 255);
            set => normalizedR = value / 255f;
        }

        /// <summary>
        /// 红色
        /// </summary>
        public float normalizedR
        {
            get => _color.r;
            set
            {
                if (_color.r == value) return;

                var tmpColor = _color;
                tmpColor.r = value;
                color = tmpColor;
            }
        }

        /// <summary>
        /// 绿色 0~255
        /// </summary>
        public int g255
        {
            get => (int)(normalizedG * 255);
            set => normalizedG = value / 255f;
        }

        /// <summary>
        /// 绿色
        /// </summary>
        public float normalizedG
        {
            get => _color.g;
            set
            {
                if (_color.g == value) return;

                var tmpColor = _color;
                tmpColor.g = value;
                color = tmpColor;
            }
        }

        /// <summary>
        /// 蓝色 0~255
        /// </summary>
        public int b255
        {
            get => (int)(normalizedB * 255);
            set => normalizedB = value / 255f;
        }

        /// <summary>
        /// 蓝色
        /// </summary>
        public float normalizedB
        {
            get => _color.b;
            set
            {
                if (_color.b == value) return;

                var tmpColor = _color;
                tmpColor.b = value;
                color = tmpColor;
            }
        }

        /// <summary>
        /// 透明度 0~100
        /// </summary>
        public int a100
        {
            get => (int)(normalizedA * 100);
            set => normalizedA = value / 100.0f;
        }

        /// <summary>
        /// 透明度
        /// </summary>
        public float normalizedA
        {
            get => _color.a;
            set
            {
                if (_color.a == value) return;

                var tmpColor = _color;
                tmpColor.a = value;
                color = tmpColor;
            }
        }

        /// <summary>
        /// 色调 0~360
        /// </summary>
        public int h
        {
            get => (int)(normalizedH * 360);
            set => normalizedH = value / 360.0f;
        }

        /// <summary>
        /// 色调
        /// </summary>
        public float normalizedH
        {
            get => _normalizedHSV.x;
            set
            {
                if (_normalizedHSV.x == value) return;

                _normalizedHSV.x = value;
                HSVChange();
            }
        }

        /// <summary>
        /// 饱和度 0~100
        /// </summary>
        public int s
        {
            get => (int)(normalizedS * 100);
            set => normalizedS = value / 100.0f;
        }

        /// <summary>
        /// 饱和度
        /// </summary>
        public float normalizedS
        {
            get => _normalizedHSV.y;
            set
            {
                if (_normalizedHSV.y == value) return;

                _normalizedHSV.y = value;
                HSVChange();
            }
        }

        /// <summary>
        /// 明度 0~100
        /// </summary>
        public int v
        {
            get => (int)(normalizedV * 100);
            set => normalizedV = value / 100.0f;
        }

        /// <summary>
        /// 明度
        /// </summary>
        public float normalizedV
        {
            get => _normalizedHSV.z;
            set
            {
                if (_normalizedHSV.z == value) return;

                _normalizedHSV.z = value;
                HSVChange();
            }
        }

        private void HSVChange()
        {
            var tmpColor = _normalizedHSV.HSVToColor();
            tmpColor.a = _color.a;

            _color = tmpColor;
            SendChangedEvent();
        }

        private void SendChangedEvent()
        {
            onColorChanged?.Invoke(this, _color);
            onHSVChanged?.Invoke(this, _normalizedHSV);
        }

        /// <summary>
        /// 设置颜色并显示
        /// </summary>
        /// <param name="color"></param>
        public void Display(Color color)
        {
            this.color = color;
            Display();
        }

        /// <summary>
        /// 显示
        /// </summary>
        public void Display()
        {
            gameObject.SetActive(true);
        }

        /// <summary>
        /// 隐藏
        /// </summary>
        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}