using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginsCameras.Controllers;
using XCSJ.PluginsCameras.Tools.Base;
using XCSJ.Tools;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.Scripts;

namespace XCSJ.PluginsCameras.Tools.Controllers
{
    /// <summary>
    /// 相机移动通过触摸：默认通过双指触摸移动与缩放的偏移量控制相机的移动
    /// </summary>
    [Name("相机移动通过触摸")]
    [Tip("默认通过双指触摸移动与缩放的偏移量控制相机的移动", "By default, the movement of the camera is controlled by the offset between the movement and zoom of the two finger touch")]
    [Tool(CameraHelperExtension.ControllersCategoryName_Move, /*nameof(CameraController),*/ nameof(CameraTransformer))]
    [XCSJ.Attributes.Icon(EIcon.Move)]
    public class CameraMoveByTouch : BaseCameraMoveController
    {
        #region 触摸

        /// <summary>
        /// 单指触摸模式
        /// </summary>
        [Name("单指触摸模式")]
        [EnumPopup]
        public EOneTouchMode _oneTouchMode = EOneTouchMode.None;

        /// <summary>
        /// 双指触摸模式
        /// </summary>
        [Name("双指触摸模式")]
        [EnumPopup]
        public ETwoTouchMode _twoTouchMode = ETwoTouchMode.Pan_XY__Zoom_Z;

        #endregion

        /// <summary>
        /// 在任意UI上时忽略:为True时，如鼠标（手势）当前正在任意的GUI(即IM GUI)或UGUI上时则忽略处理（即不做任何处理），否则继续处理输入；为False时，处理输入；
        /// </summary>
        [Name("在任意UI上时忽略")]
        [Tip("为True时，如鼠标（手势）当前正在任意的GUI(即IM GUI)或UGUI上时则忽略处理（即不做任何处理），否则继续处理输入；为False时，处理输入；", "When it is true, if the mouse (gesture) is currently on any GUI (i.e. im GUI) or ugui, ignore the processing (i.e. do not do any processing), otherwise continue to process the input; When false, process the input;")]
        public bool _ignoreWhenOnAnyUI = true;

        /// <summary>
        /// 唤醒初始化
        /// </summary>
        public override void Awake()
        {
            base.Awake();
            _twoTouchZoomDistanceInfo.Init();
        }

        /// <summary>
        /// 更新
        /// </summary>
        protected override void Update()
        {
            if (_ignoreWhenOnAnyUI && CommonFun.IsOnAnyUI())
            {
                return;
            }

            //分析触摸
            switch (Input.touchCount)
            {
                case 1:
                    {
                        base.Update();
                        if (One(Input.GetTouch(0)))
                        {
                            Move();
                        }
                        return;
                    }
                case 2:
                    {
                        base.Update();
                        if (Two(Input.GetTouch(0), Input.GetTouch(1)))
                        {
                            Move();
                        }
                        return;
                    }
            }
        }

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();

            var speedInfo = this.speedInfo;
            speedInfo.Reset(1f);
            speedInfo.Reset(RuntimePlatform.IPhonePlayer, 0.01f);
            speedInfo.Reset(RuntimePlatform.Android, 0.03125f);
            speedInfo.Reset(RuntimePlatform.WebGLPlayer, -1f);

            _twoTouchZoomDistanceInfo.Reset(1);
            _twoTouchZoomDistanceInfo.Reset(Application.platform);

            _enableRuleByRuntimePlatform.Reset(EBool.No);
            _enableRuleByRuntimePlatform.Reset(RuntimePlatform.Android, EBool.Yes);
            _enableRuleByRuntimePlatform.Reset(RuntimePlatform.IPhonePlayer, EBool.Yes);

        }

        #region 单指

        /// <summary>
        /// 单指触摸模式
        /// </summary>
        [Name("单指触摸模式")]
        public enum EOneTouchMode
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None,

            /// <summary>
            /// 横向X
            /// </summary>
            [Name("横向X")]
            X__X,

            /// <summary>
            /// 纵向X
            /// </summary>
            [Name("纵向X")]
            Y__X,

            /// <summary>
            /// 横向Y
            /// </summary>
            [Name("横向Y")]
            X__Y,

            /// <summary>
            /// 纵向Y
            /// </summary>
            [Name("纵向Y")]
            Y__Y,

            /// <summary>
            /// 横向Z
            /// </summary>
            [Name("横向Z")]
            X__Z,

            /// <summary>
            /// 纵向Z
            /// </summary>
            [Name("纵向Z")]
            Y__Z,

            /// <summary>
            /// 横纵XY
            /// </summary>
            [Name("横纵XY")]
            XY__XY,

            /// <summary>
            /// 横纵YX
            /// </summary>
            [Name("横纵YX")]
            XY__YX,

            /// <summary>
            /// 横纵XZ
            /// </summary>
            [Name("横纵XZ")]
            XY__XZ,

            /// <summary>
            /// 横纵ZX
            /// </summary>
            [Name("横纵ZX")]
            XY__ZX,

            /// <summary>
            /// 横纵YZ
            /// </summary>
            [Name("横纵YZ")]
            XY__YZ,

            /// <summary>
            /// 横纵ZY
            /// </summary>
            [Name("横纵ZY")]
            XY__ZY
        }

        private bool One(Touch touch0)
        {
            if (touch0.phase != TouchPhase.Moved) return false;
            var speedRealtime = this.speedRealtime;
            var deltaPosition = touch0.deltaPosition;
            switch (_oneTouchMode)
            {
                case EOneTouchMode.X__X:
                    {
                        _offset.x += deltaPosition.x * speedRealtime.x;
                        break;
                    }
                case EOneTouchMode.Y__X:
                    {
                        _offset.x += deltaPosition.y * speedRealtime.x;
                        break;
                    }
                case EOneTouchMode.X__Y:
                    {
                        _offset.y += deltaPosition.x * speedRealtime.y;
                        break;
                    }
                case EOneTouchMode.Y__Y:
                    {
                        _offset.y += deltaPosition.y * speedRealtime.y;
                        break;
                    }
                case EOneTouchMode.X__Z:
                    {
                        _offset.z += deltaPosition.x * speedRealtime.z;
                        break;
                    }
                case EOneTouchMode.Y__Z:
                    {
                        _offset.z += deltaPosition.y * speedRealtime.z;
                        break;
                    }
                case EOneTouchMode.XY__XY:
                    {
                        _offset.x += deltaPosition.x * speedRealtime.x;
                        _offset.y += deltaPosition.y * speedRealtime.y;
                        break;
                    }
                case EOneTouchMode.XY__YX:
                    {
                        _offset.y += deltaPosition.x * speedRealtime.y;
                        _offset.x += deltaPosition.y * speedRealtime.x;
                        break;
                    }
                case EOneTouchMode.XY__XZ:
                    {
                        _offset.x += deltaPosition.x * speedRealtime.x;
                        _offset.z += deltaPosition.y * speedRealtime.z;
                        break;
                    }
                case EOneTouchMode.XY__ZX:
                    {
                        _offset.z += deltaPosition.x * speedRealtime.z;
                        _offset.x += deltaPosition.y * speedRealtime.x;
                        break;
                    }
                case EOneTouchMode.XY__YZ:
                    {
                        _offset.y += deltaPosition.x * speedRealtime.y;
                        _offset.z += deltaPosition.y * speedRealtime.z;
                        break;
                    }
                case EOneTouchMode.XY__ZY:
                    {
                        _offset.z += deltaPosition.x * speedRealtime.z;
                        _offset.y += deltaPosition.y * speedRealtime.y;
                        break;
                    }
                default: return false;
            }
            return true;
        }

        #endregion

        #region 双指

        /// <summary>
        /// 双指触摸模式
        /// </summary>
        [Name("双指触摸模式")]
        public enum ETwoTouchMode
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None,

            /// <summary>
            /// 平移XY缩放Z
            /// </summary>
            [Name("平移XY缩放Z")]
            Pan_XY__Zoom_Z,

            /// <summary>
            /// 平移XY
            /// </summary>
            [Name("平移XY")]
            Pan_XY,

            /// <summary>
            /// 缩放Z
            /// </summary>
            [Name("缩放Z")]
            Zoom_Z,
        }

        /// <summary>
        /// 双指触摸缩放最小距离：调用本属性时会同步调用对象的更新函数
        /// </summary>
        public float twoTouchZoomMinDistance
        {
            get
            {
                _twoTouchZoomDistanceInfo.Update(1);
                return _twoTouchZoomDistanceInfo.valueRealtime;
            }
        }

        /// <summary>
        /// 双指触摸缩放距离
        /// </summary>
        [Name("双指触摸缩放距离")]
        [Tip("双指触摸缩放时，移动的距离超过本值时才认为是有效的缩放操作；", "When two fingers touch zoom, it is considered as an effective zoom operation when the moving distance exceeds this value;")]
        [HideInSuperInspector(nameof(_twoTouchMode), EValidityCheckType.Equal, ETwoTouchMode.None)]
        public TwoTouchZoomDistanceInfo _twoTouchZoomDistanceInfo = new TwoTouchZoomDistanceInfo();

        private bool Two(Touch touch0, Touch touch1)
        {
            if (touch0.phase == TouchPhase.Moved || touch1.phase == TouchPhase.Moved)
            {
                var speedRealtime = this.speedRealtime;
                switch (_twoTouchMode)
                {
                    case ETwoTouchMode.Pan_XY__Zoom_Z:
                        {
                            var deltaPosition0 = touch0.deltaPosition;
                            var deltaPosition1 = touch1.deltaPosition;

                            //两个都发生移动
                            if (Vector2.Dot(deltaPosition0, deltaPosition1) > 0)
                            {
                                //Debug.Log("Pan_XY__Zoom_Z  Moved");
                                //双指平移 - 左右上下移动                                    
                                _offset.x += deltaPosition0.x * speedRealtime.x;
                                _offset.y += deltaPosition0.y * speedRealtime.y;
                            }
                            else
                            {
                                var position0 = touch0.position;
                                var position1 = touch1.position;

                                float currentTouchDistance = Vector2.Distance(position0, position1);
                                float lastTouchDistance = Vector2.Distance(position0 - deltaPosition0, position1 - deltaPosition1);

                                float distance = currentTouchDistance - lastTouchDistance;

                                //Debug.LogFormat("pz d0:" + distance + "=d1:" + twoTouchZoomMinDistance);
                                if (Mathf.Abs(distance) > twoTouchZoomMinDistance)
                                {
                                    //Debug.Log("2  Forward");
                                    //双指缩放 - 前后移动
                                    _offset.z += distance * speedRealtime.z;
                                }
                            }
                            return true;
                        }
                    case ETwoTouchMode.Pan_XY:
                        {
                            var deltaPosition0 = touch0.deltaPosition;
                            var deltaPosition1 = touch1.deltaPosition;

                            //两个都发生移动
                            if (Vector2.Dot(deltaPosition0, deltaPosition1) > 0)
                            {
                                //Debug.Log("Pan_XY  Moved");
                                //双指平移 - 左右上下移动                                    
                                _offset.x += deltaPosition0.x * speedRealtime.x;
                                _offset.y += deltaPosition0.y * speedRealtime.y;
                            }
                            return true;
                        }
                    case ETwoTouchMode.Zoom_Z:
                        {
                            var deltaPosition0 = touch0.deltaPosition;
                            var deltaPosition1 = touch1.deltaPosition;

                            //两个都发生移动
                            if (Vector2.Dot(deltaPosition0, deltaPosition1) > 0)
                            {
                                //Debug.Log("Zoom_Z  Moved");
                                //双指平移 - 左右上下移动     
                            }
                            else
                            {
                                var position0 = touch0.position;
                                var position1 = touch1.position;

                                float currentTouchDistance = Vector2.Distance(position0, position1);
                                float lastTouchDistance = Vector2.Distance(position0 - deltaPosition0, position1 - deltaPosition1);

                                float distance = currentTouchDistance - lastTouchDistance;

                                //Debug.LogFormat("z d0:" + distance + "=d1:" + twoTouchZoomMinDistance);
                                if (Mathf.Abs(distance) > twoTouchZoomMinDistance)
                                {
                                    //Debug.Log("2  Forward");
                                    //双指缩放 - 前后移动
                                    _offset.z += distance * speedRealtime.z;
                                }
                            }
                            return true;
                        }
                }
            }
            return false;
        }

        #endregion
    }

    /// <summary>
    /// 双指缩放距离信息
    /// </summary>
    [Serializable]
    [Name("双指缩放距离信息")]
    public class TwoTouchZoomDistanceInfo : BaseUpdateFloatRuntimePlatformInfo<TwoTouchZoomDistanceDetailInfo, float>
    {
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="deltaTime"></param>
        /// <param name="value"></param>
        public override void Update(float deltaTime, float value = 1)
        {
            valueRealtime = _value * detailInfo._value;
        }

        /// <summary>
        /// 转值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected override float ToValue(float value) => value;
    }

    /// <summary>
    /// 双指缩放距离详细信息
    /// </summary>
    [Serializable]
    [Name("双指缩放距离详细信息")]
    public class TwoTouchZoomDistanceDetailInfo : RuntimePlatformDetailInfo<float> { }
}
