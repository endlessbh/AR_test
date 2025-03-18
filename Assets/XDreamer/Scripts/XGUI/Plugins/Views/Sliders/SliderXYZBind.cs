using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginXGUI.Base;

namespace XCSJ.PluginXGUI.Views.Sliders
{
    /// <summary>
    /// 滑动条XYZ分量控制类
    /// </summary>
    public abstract class SliderXYZBind : View
    {
        /// <summary>
        /// XYZ滑动条
        /// </summary>
        [Name("XYZ滑动条")]
        public XYZSlider xyzSlider;

        /// <summary>
        /// xyz滑动条变化前的上一次值
        /// </summary>
        protected Vector3 xyzSliderLastValue = Vector3.zero;

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            if (xyzSlider.x) xyzSlider.x.onValueChanged.AddListener(OnSliderXValueChanged);
            if (xyzSlider.y) xyzSlider.y.onValueChanged.AddListener(OnSliderYValueChanged);
            if (xyzSlider.z) xyzSlider.z.onValueChanged.AddListener(OnSliderZValueChanged);

            InitSliderValue();

            RecordLastSliderValue();
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            if (xyzSlider.x) xyzSlider.x.onValueChanged.RemoveListener(OnSliderXValueChanged);
            if (xyzSlider.y) xyzSlider.y.onValueChanged.RemoveListener(OnSliderYValueChanged);
            if (xyzSlider.z) xyzSlider.z.onValueChanged.RemoveListener(OnSliderZValueChanged);
        }

        /// <summary>
        /// 记录上次滑竿值
        /// </summary>
        protected void RecordLastSliderValue()
        {
            RecordLastSliderXValue();
            RecordLastSliderYValue();
            RecordLastSliderZValue();
        }

        /// <summary>
        /// 清除上一次记录值
        /// </summary>
        protected void ClearLastSliderValue()
        {
            RecordLastSliderValue(Vector3.zero);
        }

        /// <summary>
        /// 设置上一次滑竿值
        /// </summary>
        /// <param name="xyz"></param>
        protected void RecordLastSliderValue(Vector3 xyz)
        {
            xyzSliderLastValue = xyz;
        }

        /// <summary>
        /// 记录上一次滑竿X值
        /// </summary>
        protected void RecordLastSliderXValue()
        {
            if (xyzSlider.x) xyzSliderLastValue.x = xyzSlider.x.value;
        }

        /// <summary>
        /// 记录上一次滑竿Y值
        /// </summary>
        protected void RecordLastSliderYValue()
        {
            if (xyzSlider.y) xyzSliderLastValue.y = xyzSlider.y.value;
        }

        /// <summary>
        /// 记录上一次滑竿Z值
        /// </summary>
        protected void RecordLastSliderZValue()
        {
            if (xyzSlider.z) xyzSliderLastValue.z = xyzSlider.z.value;
        }

        /// <summary>
        /// 初始化滑竿值
        /// </summary>
        protected abstract void InitSliderValue();

        /// <summary>
        /// X滑动条变化
        /// </summary>
        /// <param name="value"></param>
        protected abstract void OnSliderXValueChanged(float value);

        /// <summary>
        /// Y滑动条变化
        /// </summary>
        /// <param name="value"></param>
        protected abstract void OnSliderYValueChanged(float value);

        /// <summary>
        /// Z滑动条变化
        /// </summary>
        /// <param name="value"></param>
        protected abstract void OnSliderZValueChanged(float value);
    }
}
