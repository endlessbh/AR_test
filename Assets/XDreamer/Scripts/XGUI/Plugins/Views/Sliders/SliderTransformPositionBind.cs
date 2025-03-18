using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginTools;
using XCSJ.PluginXGUI.Base;

namespace XCSJ.PluginXGUI.Views.Sliders
{
    /// <summary>
    /// 滑动条控制位置
    /// </summary>
    [Tool(XGUICategory.Component, nameof(XGUIManager), rootType = typeof(ToolsManager))]
    [Name("滑动条控制位置")]
    [XCSJ.Attributes.Icon(EIcon.Slider)]
    [RequireManager(typeof(XGUIManager), typeof(ToolsManager))]
    public class SliderTransformPositionBind : SliderTransformBind
    {
        [Name("累加模式")]
        [Tip("勾选时:在转换原有位置坐标上累加滑动条偏移值;不勾选时:直接使用滑动条偏移值设置转换位置坐标;", "When checked: accumulate the slider offset value on the converted original position coordinates; If unchecked: directly use the slider offset value to set the conversion position coordinates;")]
        public bool addMode = true;

        private Vector3 recordPosition;

        protected override void InitSliderValue()
        {
            if (_targetTransform)
            {
                xyzSlider.SetValue(_targetTransform.position);
            }
        }

        protected override void RecordData()
        {
            if (_targetTransform)
            {
                recordPosition = _targetTransform.position;
            }
        }

        protected override void RecoverData()
        {
            if (_targetTransform)
            {
                _targetTransform.position = recordPosition;
            }
        }

        protected override void OnSliderXValueChanged(float value)
        {
            if (_targetTransform)
            {
                if (addMode)
                {
                    _targetTransform.position = recordPosition + new Vector3(value, xyzSlider.y ? xyzSlider.y.value : 0, xyzSlider.z ? xyzSlider.z.value : 0);
                }
                else
                {
                    _targetTransform.position = new Vector3(value, _targetTransform.position.y, _targetTransform.position.z);
                }
            }
        }

        protected override void OnSliderYValueChanged(float value)
        {
            if (_targetTransform)
            {
                if (addMode)
                {
                    _targetTransform.position = recordPosition + new Vector3(xyzSlider.x ? xyzSlider.x.value : 0, value, xyzSlider.z ? xyzSlider.z.value : 0);
                }
                else
                {
                    _targetTransform.position = new Vector3(_targetTransform.position.x, value, _targetTransform.position.z);
                }
            }
        }

        protected override void OnSliderZValueChanged(float value)
        {
            if (_targetTransform)
            {
                if (addMode)
                {
                    _targetTransform.position = recordPosition + new Vector3(xyzSlider.x ? xyzSlider.x.value : 0, xyzSlider.y ? xyzSlider.y.value : 0, value);
                }
                else
                {
                    _targetTransform.position = new Vector3(_targetTransform.position.x, _targetTransform.position.y, value);
                }
            }
        }
    }

}
