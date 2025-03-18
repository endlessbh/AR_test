using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Attributes;
using XCSJ.Extension.Base.Inputs;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.PluginSMS.States.Inputs
{
    /// <summary>
    /// 输入键码:输入键码组件是键盘按键的触发器。键盘按下或弹起后，组件切换为完成态。
    /// </summary>
    [ComponentMenu("输入/" + Title, typeof(SMSManager))]
    [Name(Title, nameof(GestureDetector))]
    [Tip("在Android与iOS运行时平台分析单指触摸，其他平台分析鼠标按键按下状态，进行手势检测。满足期望的手势检测条件后，组件切换为完成态。", "When Android and IOS are running, the platform analyzes the single finger touch, and other platforms analyze the mouse button pressing state for gesture detection. After the desired gesture detection conditions are met, the component switches to the completed state.")]
    [XCSJ.Attributes.Icon(EIcon.Put)]
    public class GestureDetector : Trigger<GestureDetector>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "手势检测器";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib("输入", typeof(SMSManager))]
        [StateComponentMenu("输入/" + Title, typeof(SMSManager))]
        [Name(Title, nameof(GestureDetector))]
        [Tip("输入键码组件是键盘按键的触发器。键盘按下或弹起后，组件切换为完成态。", "The input key code component is the trigger of keyboard keys. After the keyboard is pressed or popped up, the component switches to the completed state.")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        #region 采样

        /// <summary>
        /// 采样
        /// </summary>
        [Serializable]
        [Name("采样")]
        public class Samples
        {
            /// <summary>
            /// 鼠标按钮:非Android与iOS运行时平台时，使用本参数对应的鼠标按键数据做手势检测
            /// </summary>
            [Name("鼠标按钮")]
            [Tip("非Android与iOS运行时平台时，使用本参数对应的鼠标按键数据做手势检测", "For non Android and IOS runtime platforms, use the mouse button data corresponding to this parameter for gesture detection")]
            [EnumPopup]
            public EMouseButton _mouseButton = EMouseButton.Right;

            /// <summary>
            /// 采样规则
            /// </summary>
            [Name("采样规则")]
            public enum ESampleRule
            {
                /// <summary>
                /// 总是成立
                /// </summary>
                [Name("总是成立")]
                Alawys,

                /// <summary>
                /// 总是成立
                /// </summary>
                [Name("释放时")]
                [Tip("在Android与iOS运行时平台时,在触摸结束时完成采样；其他平台，在鼠标按钮弹起时完成采样；", "In Android and IOS runtime platforms, sampling is completed at the end of touch; For other platforms, the sampling is completed when the mouse button pops up;")]
                OnRelease,
            }

            /// <summary>
            /// 采样规则
            /// </summary>
            [Name("采样规则")]
            [EnumPopup]
            public ESampleRule _sampleRule = ESampleRule.Alawys;

            /// <summary>
            /// 最低采样点数:做手势形状判断的最低采样点数数目；采样点数数目低于本值时，不做检测；
            /// </summary>
            [Name("最低采样点数")]
            [Tip("做手势形状判断的最低采样点数数目；采样点数数目低于本值时，不做检测；", "The minimum number of sampling points for gesture shape judgment; When the number of sampling points is lower than this value, no detection will be carried out;")]
            [Range(5, 100)]
            public int _minSampleCount = 10;

            /// <summary>
            /// 最小采样间隔:有效采样点的最小间隔距离；单位：像素；
            /// </summary>
            [Name("最小采样间隔")]
            [Tip("有效采样点的最小间隔距离；单位：像素；", "Minimum distance between effective sampling points; Unit: pixel;")]
            [Range(5, 50)]
            public int _minSampleInterval = 10;

            /// <summary>
            /// 采样点列表
            /// </summary>
            [Name("采样点列表")]
            [ArrayElement(EArrayElementHandleRule.None)]
#if !XDREAMER_EDITION_XDREAMERDEVELOPER
            [HideInSuperInspector]
#endif
            public List<Vector2> samples = new List<Vector2>();

            private bool inSample = false;

            /// <summary>
            /// 开始采样
            /// </summary>
            public void Begin()
            {
                if (inSample) return;
                CommonFun.BeginOnUI();
                inSample = true;
            }

            /// <summary>
            /// 结束采样
            /// </summary>
            public void End()
            {
                if (!inSample) return;
                inSample = false;
                CommonFun.EndOnUI();
            }

            /// <summary>
            /// 采样
            /// </summary>
            /// <returns>如果采样点列表数目不低于最低采样点数则返回True；否则返回False；</returns>
            public bool Sample()
            {
                bool valid = _sampleRule == ESampleRule.Alawys;

#if !UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID)
                if (Input.touches.Length != 1)
                {
                    samples.Clear();
                }
                else
                {
                    var touch0 = Input.touches[0];
                    switch(touch0.phase)
                    {
                        case TouchPhase.Canceled:
                            {
                                samples.Clear();
                                End();
                                break;
                            }
                        case TouchPhase.Began:                        
                            {
                                samples.Clear();
                                Begin();
                                break;
                            }
                        case TouchPhase.Moved:
                            {
                                Vector2 p = touch0.position;
                                if (samples.Count == 0 || (p - samples[samples.Count - 1]).magnitude > _minSampleInterval)
                                {
                                    samples.Add(p);
                                }
                                break;
                            }
                        case TouchPhase.Ended:
                            {
                                if(_sampleRule == ESampleRule.OnRelease)
                                {
                                    valid = true;
                                }
                                End();
                                break;
                            }
                    }
                }
#else
                if (Input.GetMouseButtonDown((int)_mouseButton))//按下时
                {
                    samples.Clear();
                    Begin();
                }
                if (Input.GetMouseButton((int)_mouseButton))//按下保持时
                {
                    Vector2 p = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                    if (samples.Count == 0 || (p - samples[samples.Count - 1]).magnitude > _minSampleInterval)
                    {
                        samples.Add(p);
                    }
                }
                if (Input.GetMouseButtonUp((int)_mouseButton))//弹起时
                {
                    if (_sampleRule == ESampleRule.OnRelease)
                    {
                        valid = true;
                    }
                    End();
                }
#endif

                return valid && samples.Count >= _minSampleCount;
            }
        }

        /// <summary>
        /// 采样
        /// </summary>
        [Name("采样")]
        [OnlyMemberElements]
        public Samples _samples = new Samples();

        #endregion

        #region 手势形状

        /// <summary>
        /// 手势形状
        /// </summary>
        [Name("手势形状")]
        [EnumPopup]
        public EGestureShape _gestureShape = EGestureShape.Clockwise;

        /// <summary>
        /// 手势形状
        /// </summary>
        [Name("手势形状")]
        public enum EGestureShape
        {
            /// <summary>
            /// 无:不做任何检测
            /// </summary>
            [Name("无")]
            [Tip("不做任何检测", "No testing")]
            None,

            /// <summary>
            /// 顺时针圆:顺时针绘制圆形的手势
            /// </summary>
            [Name("顺时针圆")]
            [Tip("顺时针绘制圆形的手势", "Gesture of drawing a circle clockwise")]
            Clockwise,

            /// <summary>
            /// 逆时针圆:逆时针绘制圆形的手势
            /// </summary>
            [Name("逆时针圆")]
            [Tip("逆时针绘制圆形的手势", "The gesture of drawing a circle counterclockwise")]
            Anticlockwise,
        }

        #endregion

        /// <summary>
        /// 检测数据
        /// </summary>
        public class DetectionData
        {
            /// <summary>
            /// 检测
            /// </summary>
            /// <param name="samples"></param>
            /// <returns></returns>
            public virtual bool Detect(List<Vector2> samples) => false;
        }

        /// <summary>
        /// 圆形数据
        /// </summary>
        public class CircleData : DetectionData
        {
            /// <summary>
            /// 圆形检测规则
            /// </summary>
            [Flags]
            [Name("圆形检测规则")]
            public enum ECircleDetectionRule
            {
                /// <summary>
                /// 屏幕尺寸：采样点构建的多边形长度和需要达到屏幕尺寸和的1/4
                /// </summary>
                [Name("屏幕尺寸")]
                [EnumFieldName("屏幕尺寸")]
                [Tip("采样点构建的多边形长度和需要达到屏幕尺寸和的1/4", "The gesture of drawing a circle counterclockwise")]
                ScreenSize = 1 << 0,

                /// <summary>
                /// 外角和：采样点构建的多边形外角和大于本值时，认为是有效圆；
                /// </summary>
                [Name("外角和")]
                [EnumFieldName("外角和")]
                [Tip("采样点构建的多边形外角和大于本值时，认为是有效圆；", "When the sum of the outer corners of the polygon constructed by the sampling points is greater than this value, it is considered as an effective circle;")]
                ExteriorAngleSum = 1 << 1,
            }

            /// <summary>
            /// 圆形检测规则:采样点如满足特定的检测规则，则认为是圆形；
            /// </summary>
            [Name("圆形检测规则")]
            [Tip("采样点如满足特定的检测规则，则认为是圆形；", "If the sampling point meets the specific detection rules, it is considered to be circular;")]
            [EnumPopup]
            public ECircleDetectionRule _circleDetectionRule = ECircleDetectionRule.ScreenSize;

            /// <summary>
            /// 最小外角和:采样点构建的多边形外角和大于本值时，认为是有效圆；
            /// </summary>
            [Name("最小外角和")]
            [Tip("采样点构建的多边形外角和大于本值时，认为是有效圆；", "When the sum of the outer corners of the polygon constructed by the sampling points is greater than this value, it is considered as an effective circle;")]
            [HideInSuperInspector(nameof(_circleDetectionRule), EValidityCheckType.NotEqual, ECircleDetectionRule.ExteriorAngleSum)]
            [Range(180, 720)]
            public float _minExteriorAngleSum = 270;
        }

        /// <summary>
        /// 顺时针数据
        /// </summary>
        [Serializable]
        [Name("顺时针数据")]
        public class ClockwiseData : CircleData
        {
            /// <summary>
            /// 检测
            /// </summary>
            /// <param name="samples"></param>
            /// <returns></returns>
            public override bool Detect(List<Vector2> samples)
            {
                Vector2 gestureSum = Vector2.zero;
                float gestureLength = 0;
                float angleSum = 0;
                Vector2 prevDelta = Vector2.zero;
                for (int i = 0; i < samples.Count - 2; i++)
                {
                    Vector2 delta = samples[i + 1] - samples[i];
                    float deltaLength = delta.magnitude;
                    gestureSum += delta;
                    gestureLength += deltaLength;

                    var angle = Vector2.SignedAngle(prevDelta, delta);
                    if (angle == 0)
                    {
                        //不做处理
                    }
                    else if (angle > 0 && angle < 90)//逆时针,必须为锐角
                    {
                        samples.Clear();
                        return false;
                    }
                    else if (angle < 0 && angle > -90)//顺时针,必须为锐角
                    {
                        angleSum += angle;
                    }
                    else//直角或钝角时，无效圆
                    {
                        samples.Clear();
                        return false;
                    }

                    prevDelta = delta;
                }

                var valid = false;
                if (_circleDetectionRule.HasFlag(ECircleDetectionRule.ScreenSize))
                {
                    int gestureBase = (Screen.width + Screen.height) / 4;
                    if (gestureLength > gestureBase && gestureSum.magnitude < gestureBase / 2)
                    {
                        samples.Clear();
                        valid = true;
                    }
                }
                if (_circleDetectionRule.HasFlag(ECircleDetectionRule.ExteriorAngleSum))
                {
                    if (angleSum >= -_minExteriorAngleSum)
                    {
                        valid = false;
                    }
                }

                return valid;
            }
        }

        /// <summary>
        /// 逆时针数据
        /// </summary>
        [Serializable]
        [Name("逆时针数据")]
        public class AnticlockwiseData : CircleData
        {
            /// <summary>
            /// 检测
            /// </summary>
            /// <param name="samples"></param>
            /// <returns></returns>
            public override bool Detect(List<Vector2> samples)
            {
                Vector2 gestureSum = Vector2.zero;
                float gestureLength = 0;
                float angleSum = 0;
                Vector2 prevDelta = Vector2.zero;
                for (int i = 0; i < samples.Count - 2; i++)
                {
                    Vector2 delta = samples[i + 1] - samples[i];
                    float deltaLength = delta.magnitude;
                    gestureSum += delta;
                    gestureLength += deltaLength;

                    var angle = Vector2.SignedAngle(prevDelta, delta);
                    if (angle == 0)
                    {
                        //不做处理
                    }
                    else if (angle > 0 && angle < 90)//逆时针,必须为锐角
                    {
                        angleSum += angle;
                    }
                    else if (angle < 0 && angle > -90)//顺时针,必须为锐角
                    {
                        samples.Clear();
                        return false;
                    }
                    else//直角或钝角时，无效圆
                    {
                        samples.Clear();
                        return false;
                    }

                    prevDelta = delta;
                }

                var valid = false;
                if (_circleDetectionRule.HasFlag(ECircleDetectionRule.ScreenSize))
                {
                    int gestureBase = (Screen.width + Screen.height) / 4;
                    if (gestureLength > gestureBase && gestureSum.magnitude < gestureBase / 2)
                    {
                        samples.Clear();
                        valid = true;
                    }
                }
                if (_circleDetectionRule.HasFlag(ECircleDetectionRule.ExteriorAngleSum))
                {
                    if (angleSum <= _minExteriorAngleSum)
                    {
                        valid = false;
                    }
                }

                return valid;
            }
        }

        private DetectionData _noneData = new DetectionData();

        /// <summary>
        /// 顺时针数据
        /// </summary>
        [Name("顺时针数据")]
        [HideInSuperInspector(nameof(_gestureShape), EValidityCheckType.NotEqual, EGestureShape.Clockwise)]
        [OnlyMemberElements]
        public ClockwiseData _clockwiseData = new ClockwiseData();

        /// <summary>
        /// 逆时针数据
        /// </summary>
        [Name("逆时针数据")]
        [HideInSuperInspector(nameof(_gestureShape), EValidityCheckType.NotEqual, EGestureShape.Anticlockwise)]
        [OnlyMemberElements]
        public AnticlockwiseData _anticlockwiseData = new AnticlockwiseData();

        /// <summary>
        /// 获取检测数据对象
        /// </summary>
        /// <returns></returns>
        public DetectionData GetDetectionData()
        {
            switch (_gestureShape)
            {
                case EGestureShape.Clockwise: return _clockwiseData;
                case EGestureShape.Anticlockwise: return _anticlockwiseData;
                default: return _noneData;
            }
        }

        /// <summary>
        /// 当更新时
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnUpdate(StateData stateData)
        {
            base.OnUpdate(stateData);

            if (finished || !_samples.Sample()) return;

            finished = GetDetectionData().Detect(_samples.samples);
        }

        /// <summary>
        /// 当退出时
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnExit(StateData stateData)
        {
            base.OnExit(stateData);
            _samples.End();
        }

        /// <summary>
        /// 输出友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString() => CommonFun.Name(_gestureShape);
    }
}
