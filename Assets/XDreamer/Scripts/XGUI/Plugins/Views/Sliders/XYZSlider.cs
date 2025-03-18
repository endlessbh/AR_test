using System;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;

namespace XCSJ.PluginXGUI.Views.Sliders
{
    /// <summary>
    /// XYZ滑块面板
    /// </summary>
    [Name("XYZ滑块面板")]
    [Serializable]
    public class XYZSlider
    {
        [Name("XYZ面板")]
        public RectTransform root = null;

        [Name("X")]
        public Slider x = null;

        [Name("Y")]
        public Slider y = null;

        [Name("Z")]
        public Slider z = null;

        public void SetRootActive(bool active)
        {
            if (root)
            {
                root.gameObject.SetActive(active);
            }
        }

        public void GetValue(ref Vector3 value)
        {
            if (x)
            {
                value.x = x.value;
            }

            if (y)
            {
                value.y = y.value;
            }

            if (z)
            {
                value.z = z.value;
            }
        }

        public void SetValue(Vector3 value)
        {
            if (x)
            {
                x.value = value.x;
            }

            if (y)
            {
                y.value = value.y;
            }

            if (z)
            {
                z.value = value.z;
            }
        }

        private Vector3 recordValue = Vector3.zero;

        public void RecordData()
        {
            GetValue(ref recordValue);
        }

        public void RecoverData()
        {
            SetValue(recordValue);
        }

        public void SetRange(Vector2 rangeX, Vector2 rangeY, Vector2 rangeZ)
        {
            if (x)
            {
                x.minValue = rangeX.x;
                x.minValue = rangeX.y;
            }

            if (y)
            {
                y.minValue = rangeY.x;
                y.minValue = rangeY.y;
            }

            if (z)
            {
                z.minValue = rangeZ.x;
                z.minValue = rangeZ.y;
            }
        }
    }
}
