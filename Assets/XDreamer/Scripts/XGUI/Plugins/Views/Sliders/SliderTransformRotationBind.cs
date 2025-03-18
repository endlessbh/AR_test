using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginTools;
using XCSJ.PluginXGUI.Base;

namespace XCSJ.PluginXGUI.Views.Sliders
{
    /// <summary>
    /// 滑动条控制旋转
    /// </summary>
    [Tool(XGUICategory.Component, nameof(XGUIManager), rootType = typeof(ToolsManager))]
    [Name("滑动条控制旋转")]
    [XCSJ.Attributes.Icon(EIcon.Slider)]
    [RequireManager(typeof(XGUIManager), typeof(ToolsManager))]
    public class SliderTransformRotationBind : SliderTransformBind
    {
        /// <summary>
        /// 绕X轴为世界坐标系
        /// </summary>
        [Name("绕X轴为世界坐标系")]
        public bool _xWordSpace = true;

        /// <summary>
        /// 绕Y轴为世界坐标系
        /// </summary>
        [Name("绕Y轴为世界坐标系")]
        public bool _yWordSpace = true;

        /// <summary>
        /// 绕Z轴为世界坐标系
        /// </summary>
        [Name("绕Z轴为世界坐标系")]
        public bool _zWordSpace = true;

        private bool init = false;
        private Quaternion recordRotation;

        /// <summary>
        /// 初始化滑竿值
        /// </summary>
        protected override void InitSliderValue()
        {
            if (_targetTransform)
            {
                var angle = AngleClampN180ToP180(_targetTransform.eulerAngles);
                RecordLastSliderValue(angle);
                xyzSlider.SetValue(angle);

                init = true;
            }
        }

        /// <summary>
        /// 将角度值设定为-180到180之间
        /// </summary>
        /// <returns></returns>
        public static Vector3 AngleClampN180ToP180(Vector3 angle)
        {
            return new Vector3(ClampN180ToP180(angle.x), ClampN180ToP180(angle.y), ClampN180ToP180(angle.z));
        }

        /// <summary>
        /// 将角度值设定为-180到180之间
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static float ClampN180ToP180(float angle)
        {
            angle = angle % 360;
            if (angle > 180)
            {
                angle -= 360;
            }
            else if (angle < -180)
            {
                angle += 360;
            }
            return angle;
        }

        /// <summary>
        /// 记录数据
        /// </summary>
        protected override void RecordData()
        {
            if (_targetTransform)
            {
                recordRotation = _targetTransform.rotation;
            }
        }

        /// <summary>
        /// 还原数据
        /// </summary>
        protected override void RecoverData()
        {
            if (_targetTransform)
            {
                _targetTransform.rotation = recordRotation;
            }
        }

        /// <summary>
        /// X轴滑动条发生变化
        /// </summary>
        /// <param name="value"></param>
        protected override void OnSliderXValueChanged(float value)
        {
            if (_targetTransform && init)
            {
                var offset = value - xyzSliderLastValue.x;
                if (_xWordSpace)
                {
                    _targetTransform.Rotate(Vector3.right, offset, Space.World);
                }
                else
                {
                    _targetTransform.Rotate(offset, 0, 0);
                }
                RecordLastSliderXValue();
            }
        }

        /// <summary>
        /// Y轴滑动条发生变化
        /// </summary>
        /// <param name="value"></param>
        protected override void OnSliderYValueChanged(float value)
        {
            if (_targetTransform && init)
            {
                var offset = value - xyzSliderLastValue.y;
                if (_yWordSpace)
                {
                    _targetTransform.Rotate(Vector3.up, offset, Space.World);
                }
                else
                {
                    _targetTransform.Rotate(0, offset, 0);
                }
                RecordLastSliderYValue();
            }
        }

        /// <summary>
        /// Z轴滑动条发生变化
        /// </summary>
        /// <param name="value"></param>
        protected override void OnSliderZValueChanged(float value)
        {
            if (_targetTransform && init)
            {
                var offset = value - xyzSliderLastValue.z;
                if (_zWordSpace)
                {
                    _targetTransform.Rotate(Vector3.forward, offset, Space.World);
                }
                else
                {
                    _targetTransform.Rotate(0, 0, offset);
                }
                RecordLastSliderZValue();
            }
        }
    }
}