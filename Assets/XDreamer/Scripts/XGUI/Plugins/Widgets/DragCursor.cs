using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.PluginsCameras;
using XCSJ.PluginXGUI.Base;

namespace XCSJ.PluginXGUI.Widgets
{
    /// <summary>
    /// 跟随鼠标的拖拽光标
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class DragCursor : View
    {
        [Name("光标偏移量")]
        public Vector2 _offset;

        private Image _image;

        private void Awake()
        {
            _image = GetComponent<Image>();
        }

        public bool dragging = false;

        public void SetData(Sprite icon, bool dragging)
        {
            _image.sprite = icon;
            this.dragging = dragging;
        }

        public void ResetData()
        {
            SetData(null, false);
            gameObject.SetActive(false);
        }

        private void Update()
        {
            if (dragging)
            {
                var cam = CameraHelperExtension.currentCamera;
                rectTransform.anchorMax = rectTransform.anchorMin = cam.ScreenToViewportPoint(Input.mousePosition);
            }
        }
    }
}
