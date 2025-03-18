using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginsCameras.Controllers;
using XCSJ.PluginsCameras.Tools.Base;
using XCSJ.Tools;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.Scripts;

namespace XCSJ.PluginsCameras.Tools.Controllers
{
    /// <summary>
    /// 相机旋转通过触摸:默认通过单指触摸移动的偏移量控制相机的旋转
    /// </summary>
    [Name("相机旋转通过触摸")]
    [Tip("默认通过单指触摸移动的偏移量控制相机的旋转", "By default, the camera rotation is controlled by the offset of single finger touch movement")]
    [Tool(CameraHelperExtension.ControllersCategoryName_Rotate, /*nameof(CameraController),*/ nameof(CameraTransformer))]
    [XCSJ.Attributes.Icon(EIcon.Rotate)]
    public class CameraRotateByTouch : BaseCameraRotateController
    {
        #region 触摸

        /// <summary>
        /// 单指触摸模式
        /// </summary>
        [Name("单指触摸模式")]
        [EnumPopup]
        public EOneTouchMode _oneTouchMode = EOneTouchMode.XY__YX;

        /// <summary>
        /// 单指触摸模式
        /// </summary>
        public EOneTouchMode oneTouchMode
        {
            get => _oneTouchMode;
            set => this.XModifyProperty(ref _oneTouchMode, value);
        }

        /// <summary>
        /// 双指触摸模式
        /// </summary>
        [Name("双指触摸模式")]
        [EnumPopup]
        public ETwoTouchMode _twoTouchMode = ETwoTouchMode.None;

        #endregion

        /// <summary>
        /// 在任意UI上时忽略:为True时，如鼠标（手势）当前正在任意的GUI(即IM GUI)或UGUI上时则忽略处理（即不做任何处理），否则继续处理输入；为False时，处理输入；
        /// </summary>
        [Name("在任意UI上时忽略")]
        [Tip("为True时，如鼠标（手势）当前正在任意的GUI(即IM GUI)或UGUI上时则忽略处理（即不做任何处理），否则继续处理输入；为False时，处理输入；", "When it is true, if the mouse (gesture) is currently on any GUI (i.e. im GUI) or ugui, ignore the processing (i.e. do not do any processing), otherwise continue to process the input; When false, process the input;")]
        public bool _ignoreWhenOnAnyUI = true;

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
                            Rotate();
                        }
                        return;
                    }
                case 2:
                    {
                        base.Update();
                        if(Two(Input.GetTouch(0), Input.GetTouch(1)))
                        {
                            Rotate();
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

            speedInfo.Reset(new Vector3(-10, 10, 10));
            speedInfo.Reset(RuntimePlatform.IPhonePlayer, 0.01f);
            speedInfo.Reset(RuntimePlatform.Android, 0.125f);
            speedInfo.Reset(RuntimePlatform.WebGLPlayer, -0.1f);
            speedInfo.Reset(RuntimePlatform.WindowsEditor, 0.125f);
            speedInfo.Reset(RuntimePlatform.WindowsPlayer, 0.125f);

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
            [Tip("不做任何处理", "No treatment")]
            None,

            /// <summary>
            /// 横向X：横向偏移量控制相机的X轴旋转
            /// </summary>
            [Name("横向X")]
            [Tip("横向偏移量控制相机的X轴旋转", "The lateral offset controls the x-axis rotation of the camera")]
            X__X,

            /// <summary>
            /// 纵向X：纵向偏移量控制相机的X轴旋转
            /// </summary>
            [Name("纵向X")]
            [Tip("纵向偏移量控制相机的X轴旋转", "The vertical offset controls the x-axis rotation of the camera")]
            Y__X,

            /// <summary>
            /// 横向Y：横向偏移量控制相机的Y轴旋转
            /// </summary>
            [Name("横向Y")]
            [Tip("横向偏移量控制相机的Y轴旋转", "The lateral offset controls the y-axis rotation of the camera")]
            X__Y,

            /// <summary>
            /// 纵向Y：纵向偏移量控制相机的Y轴旋转
            /// </summary>
            [Name("纵向Y")]
            [Tip("纵向偏移量控制相机的Y轴旋转", "The vertical offset controls the y-axis rotation of the camera")]
            Y__Y,

            /// <summary>
            /// 横向Z：横向偏移量控制相机的Z轴旋转
            /// </summary>
            [Name("横向Z")]
            [Tip("横向偏移量控制相机的Z轴旋转", "The lateral offset controls the z-axis rotation of the camera")]
            X__Z,

            /// <summary>
            /// 纵向Z：纵向偏移量控制相机的Z轴旋转
            /// </summary>
            [Name("纵向Z")]
            [Tip("纵向偏移量控制相机的Z轴旋转", "The vertical offset controls the z-axis rotation of the camera")]
            Y__Z,

            /// <summary>
            /// 横X纵Y：横向偏移量控制相机的X轴旋转,纵向偏移量控制相机的Y轴旋转
            /// </summary>
            [Name("横X纵Y")]
            [Tip("横向偏移量控制相机的X轴旋转,纵向偏移量控制相机的Y轴旋转", "The lateral offset controls the x-axis rotation of the camera, and the longitudinal offset controls the y-axis rotation of the camera")]
            XY__XY,

            /// <summary>
            /// 横Y纵X：横向偏移量控制相机的Y轴旋转,纵向偏移量控制相机的X轴旋转
            /// </summary>
            [Name("横Y纵X")]
            [Tip("横向偏移量控制相机的Y轴旋转,纵向偏移量控制相机的X轴旋转", "The lateral offset controls the y-axis rotation of the camera, and the longitudinal offset controls the x-axis rotation of the camera")]
            XY__YX,

            /// <summary>
            /// 横X纵Z：横向偏移量控制相机的X轴旋转,纵向偏移量控制相机的Z轴旋转
            /// </summary>
            [Name("横X纵Z")]
            [Tip("横向偏移量控制相机的X轴旋转,纵向偏移量控制相机的Z轴旋转", "The lateral offset controls the x-axis rotation of the camera, and the longitudinal offset controls the z-axis rotation of the camera")]
            XY__XZ,

            /// <summary>
            /// 横Z纵X：横向偏移量控制相机的Z轴旋转,纵向偏移量控制相机的X轴旋转
            /// </summary>
            [Name("横Z纵X")]
            [Tip("横向偏移量控制相机的Z轴旋转,纵向偏移量控制相机的X轴旋转", "The lateral offset controls the z-axis rotation of the camera, and the longitudinal offset controls the x-axis rotation of the camera")]
            XY__ZX,

            /// <summary>
            /// 横Y纵Z：横向偏移量控制相机的Y轴旋转,纵向偏移量控制相机的Z轴旋转
            /// </summary>
            [Name("横Y纵Z")]
            [Tip("横向偏移量控制相机的Y轴旋转,纵向偏移量控制相机的Z轴旋转", "The lateral offset controls the y-axis rotation of the camera, and the longitudinal offset controls the z-axis rotation of the camera")]
            XY__YZ,

            /// <summary>
            /// 横Z纵Y：横向偏移量控制相机的Z轴旋转,纵向偏移量控制相机的Y轴旋转
            /// </summary>
            [Name("横Z纵Y")]
            [Tip("横向偏移量控制相机的Z轴旋转,纵向偏移量控制相机的Y轴旋转", "The lateral offset controls the y-axis rotation of the camera, and the longitudinal offset controls the z-axis rotation of the camera")]
            XY__ZY
        }

        /// <summary>
        ///单指
        /// </summary>
        /// <param name="touch0"></param>
        /// <returns></returns>
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
                default:return false;
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
        }

        /// <summary>
        /// 双指触摸缩放最小距离
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
        /// 双指触摸缩放距离:双指触摸缩放时，移动的距离超过本值时才认为是有效的缩放操作；
        /// </summary>
        [Name("双指触摸缩放距离")]
        [Tip("双指触摸缩放时，移动的距离超过本值时才认为是有效的缩放操作；", "When two fingers touch zoom, it is considered as an effective zoom operation when the moving distance exceeds this value;")]
        [HideInSuperInspector(nameof(_twoTouchMode), EValidityCheckType.Equal, ETwoTouchMode.None)]
        public TwoTouchZoomDistanceInfo _twoTouchZoomDistanceInfo = new TwoTouchZoomDistanceInfo();

        private bool Two(Touch touch0, Touch touch1)
        {
            //if (touch0.phase == TouchPhase.Moved || touch1.phase == TouchPhase.Moved)
            //{
            //    //var speedRealtime = this.speedRealtime;
            //}
            return false;
        }

        #endregion
    }
}
