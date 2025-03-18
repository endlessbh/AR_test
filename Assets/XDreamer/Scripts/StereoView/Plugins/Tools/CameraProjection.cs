using UnityEngine;
using System.Collections;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCamera;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginsCameras;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Extension.Base.Tweens;
using System.IO;
using XCSJ.PluginStereoView.Base;
using XCSJ.Extension.Interactions.Tools;

namespace XCSJ.PluginStereoView.Tools
{
    /// <summary>
    /// 相机投影:根据屏幕与相机位置实时更新相机投影矩阵的工具组件
    /// </summary>
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    [Name(Title)]
    [Tip("根据屏幕与相机位置实时更新相机投影矩阵的工具组件", "Tool component for updating camera projection matrix in real time according to screen and camera position")]
    [RequireManager(typeof(StereoViewManager), typeof(CameraManager))]
    [DisallowMultipleComponent]
    public class CameraProjection : InteractProvider
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "相机投影";

        /// <summary>
        /// 更新模式
        /// </summary>
        [Name("更新模式")]
        public enum EUpdateMode
        {
            /// <summary>
            /// 自定义虚拟屏幕
            /// </summary>
            [Name("自定义虚拟屏幕")]
            [Tip("通过用户自定义的虚拟屏幕更新相机的视图矩阵与透视矩阵，通常为绿幕；", "Update the camera's view matrix and perspective matrix through a user-defined virtual screen, usually with a green screen;")]
            CustomVirtualScreen,

            /// <summary>
            /// 自定义虚拟屏幕
            /// </summary>
            [Name("Unity虚拟屏幕")]
            [Tip("通过Unity虚拟屏幕更新相机的视图矩阵与透视矩阵，通常为蓝幕；", "Update the camera's view matrix and perspective matrix through the Unity virtual screen, usually a blue screen;")]
            UnityVirtualScreen,
        }

        /// <summary>
        /// 更新模式
        /// </summary>
        [Name("更新模式")]
        [EnumPopup]
        public EUpdateMode _updateMode = EUpdateMode.CustomVirtualScreen;

        /// <summary>
        /// 更新模式
        /// </summary>
        public EUpdateMode updateMode
        {
            get => _updateMode;
            set
            {
                _updateMode = value;
                ResetCamera();
            }
        }

        /// <summary>
        /// 左右眼矩阵模式
        /// </summary>
        [Name("左右眼矩阵模式")]
        [Tip("立体渲染启用时本参数有效，用于基于当前相机的左右眼透视矩阵与视图矩阵的不同计算方法；", "This parameter is valid when stereoscopic rendering is enabled, and is used for different calculation methods based on the left and right eye perspective matrix and view matrix of the current camera;")]
        [EnumPopup]
        public ELREyeMatrixMode _LREyeMatrixMode = ELREyeMatrixMode.None;

        /// <summary>
        /// 设置左右眼矩阵
        /// </summary>
        public ELREyeMatrixMode LREyeMatrixMode
        {
            get => _LREyeMatrixMode;
            set
            {
                _LREyeMatrixMode = value;
                ResetCamera();
            }
        }

        /// <summary>
        /// 禁用时设置矩阵规则
        /// </summary>
        [Name("禁用时设置矩阵规则")]
        public enum ESetMatrixRuleOnDisable
        {
            /// <summary>
            /// 无:不做任何处理
            /// </summary>
            [Name("无")]
            [Tip("不做任何处理", "No treatment")]
            None,

            /// <summary>
            /// 重置:重置所有的矩阵信息；
            /// </summary>
            [Name("重置")]
            [Tip("重置所有的矩阵信息；", "Reset all matrix information;")]
            Reset,

            /// <summary>
            /// 到启用:设置为启用时记录的矩阵信息；
            /// </summary>
            [Name("到启用")]
            [Tip("设置为启用时记录的矩阵信息；", "Matrix information recorded when set to enabled;")]
            ToEnable,
        }

        /// <summary>
        /// 禁用时设置矩阵规则
        /// </summary>
        [Name("禁用时设置矩阵规则")]
        [EnumPopup]
        public ESetMatrixRuleOnDisable _setMatrixRuleOnDisable = ESetMatrixRuleOnDisable.Reset;

        /// <summary>
        /// 屏幕
        /// </summary>
        [Name("屏幕")]
        [Tip("用户自定义的虚拟屏幕，通常为绿幕；", "A user-defined virtual screen, usually with a green screen;")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [HideInSuperInspector(nameof(_updateMode), EValidityCheckType.NotEqual, EUpdateMode.CustomVirtualScreen)]
        [ComponentPopup]
        public VirtualScreen _screen;

        /// <summary>
        /// 屏幕
        /// </summary>
        public VirtualScreen screen
        {
            get => this.XGetComponentInParentOrParentChildrenOrGlobal(ref _screen);
            set => this.XModifyProperty(ref _screen, value);
        }

        /// <summary>
        /// 预估视锥:使相机总是朝向屏幕中心，并且根据相机位置与屏幕纵横比动态调整相机的FOV；
        /// </summary>
        [Name("预估视锥")]
        [Tip("使相机总是朝向屏幕中心，并且根据相机位置与屏幕纵横比动态调整相机的FOV；", "Make the camera always face the center of the screen, and dynamically adjust the FOV of the camera according to the camera position and the aspect ratio of the screen;")]
        [HideInSuperInspector(nameof(_updateMode), EValidityCheckType.NotEqual, EUpdateMode.CustomVirtualScreen)]
        public bool _estimateViewFrustum = true;

        /// <summary>
        /// 屏幕尺寸
        /// </summary>
        [Name("屏幕尺寸")]
        public Vector2 screenSize
        {
            get => _screen.screenSize;
            set => _screen.screenSize = value;
        }

        /// <summary>
        /// 半屏幕尺寸
        /// </summary>
        [Name("半屏幕尺寸")]
        public Vector2 halfScreenSize => _screen.screenSize * 0.5f;

        /// <summary>
        /// 当前相机：与当前组件在同一游戏对象上
        /// </summary>
        public Camera thisCamera => this.XGetComponent(ref _thisCamera);

        /// <summary>
        /// 当前相机：与当前组件在同一游戏对象上
        /// </summary>
        private Camera _thisCamera;

        /// <summary>
        /// 当前变换：当前组件（相机）所在游戏对象的变换组件
        /// </summary>
        private Transform thisTransform;

        /// <summary>
        /// 屏幕左下角世界坐标
        /// </summary>
        Vector3 pa;

        /// <summary>
        /// 屏幕右下角世界坐标
        /// </summary>
        Vector3 pb;

        /// <summary>
        /// 屏幕左上角世界坐标
        /// </summary>
        Vector3 pc;

        /// <summary>
        /// 近裁剪
        /// </summary>
        float n;

        /// <summary>
        /// 远裁剪
        /// </summary>
        float f;

        /// <summary>
        /// 唤醒初始化
        /// </summary>
        public void Awake()
        {
            thisTransform = this.transform;
            if (thisCamera) { }
        }

        private Matrix4x4 projectionMatrixBak;
        private Matrix4x4 worldToCameraMatrixBak;
        private float fieldOfViewBak;
        private Quaternion localRotationBak;

        /// <summary>
        /// 启动
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            if (!screen)
            {
                CommonFun.DelayCall(() =>
                {
                    if (!screen) Debug.LogWarningFormat("当前场景无有效屏幕,游戏对象[{0}]上功能组件[{1}]({2})无法有效工作！", CommonFun.GameObjectToString(gameObject), CommonFun.Name(GetType()), GetType().Name);
                });                
            }
            if (_thisCamera)
            {
                projectionMatrixBak = _thisCamera.projectionMatrix;
                worldToCameraMatrixBak = _thisCamera.worldToCameraMatrix;
                fieldOfViewBak = _thisCamera.fieldOfView;
                localRotationBak = _thisCamera.transform.localRotation;
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            switch(_setMatrixRuleOnDisable)
            {
                case ESetMatrixRuleOnDisable.ToEnable:
                    {
                        if (_thisCamera)
                        {
                            _thisCamera.ResetStereoProjectionMatrices();
                            _thisCamera.ResetStereoViewMatrices();

                            _thisCamera.projectionMatrix = projectionMatrixBak;
                            _thisCamera.worldToCameraMatrix = worldToCameraMatrixBak;
                            _thisCamera.fieldOfView = fieldOfViewBak;
                            _thisCamera.transform.localRotation = localRotationBak;
                        }
                        break;
                    }
                case ESetMatrixRuleOnDisable.Reset:
                    {
                        ResetCamera();
                        break;
                    }
            }
            
        }

        private void ResetCamera()
        {
            if (_thisCamera)
            {
                _thisCamera.ResetStereoProjectionMatrices();
                _thisCamera.ResetStereoViewMatrices();

                _thisCamera.ResetProjectionMatrix();
                _thisCamera.ResetWorldToCameraMatrix();
                _thisCamera.fieldOfView = 60;
                _thisCamera.transform.localRotation = Quaternion.identity;
            }
        }

        private void OnValidate()
        {
            ResetCamera();
        }

        /// <summary>
        /// 延后更新
        /// </summary>
        public void LateUpdate()
        {
            switch (_updateMode)
            {
                case EUpdateMode.CustomVirtualScreen:
                    {
                        UpdateByCustomVirtualScreen();
                        break;
                    }
                default:
                    {
                        UpdateByUnityVirtualScreen();
                        break;
                    }
            }
        }

        /// <summary>
        /// 通过Unity虚拟屏幕更新
        /// </summary>
        private void UpdateByUnityVirtualScreen()
        {
            if (_LREyeMatrixMode == ELREyeMatrixMode.None || !_thisCamera || !_thisCamera.stereoEnabled) return;

            //计算左右眼
            switch (_LREyeMatrixMode)
            {
                case ELREyeMatrixMode.SameWithCenter:
                    {
                        var cp = _thisCamera.projectionMatrix;
                        var cv = _thisCamera.worldToCameraMatrix;

                        _thisCamera.SetStereoProjectionMatrix(Camera.StereoscopicEye.Left, cp);
                        _thisCamera.SetStereoProjectionMatrix(Camera.StereoscopicEye.Right, cp);
                        _thisCamera.SetStereoViewMatrix(Camera.StereoscopicEye.Left, cv);
                        _thisCamera.SetStereoViewMatrix(Camera.StereoscopicEye.Right, cv);
                        break;
                    }
                case ELREyeMatrixMode.UseCenterProjectionMatrix:
                    {
                        var cp = _thisCamera.projectionMatrix;
                        var cv = _thisCamera.worldToCameraMatrix;

                        _thisCamera.SetStereoProjectionMatrix(Camera.StereoscopicEye.Left, cp);
                        _thisCamera.SetStereoProjectionMatrix(Camera.StereoscopicEye.Right, cp);

                        var eyeOffset = _thisCamera.stereoSeparation / 2;
                        var viewL = cv;
                        var viewR = cv;
                        viewL[12] += eyeOffset;
                        viewR[12] -= eyeOffset;
                        _thisCamera.SetStereoViewMatrix(Camera.StereoscopicEye.Left, viewL);
                        _thisCamera.SetStereoViewMatrix(Camera.StereoscopicEye.Right, viewR);
                        break;
                    }
                case ELREyeMatrixMode.CompleteCalculation:
                    {
                        var dis = _thisCamera.stereoConvergence;

                        //屏幕左下角世界坐标
                        pa = _thisCamera.ViewportToWorldPoint(new Vector3(0, 0, dis));

                        //屏幕右下角世界坐标
                        pb = _thisCamera.ViewportToWorldPoint(new Vector3(1, 0, dis));

                        //屏幕左上角世界坐标
                        pc = _thisCamera.ViewportToWorldPoint(new Vector3(0, 1, dis));

                        //近裁剪
                        n = _thisCamera.nearClipPlane;

                        //远裁剪
                        f = _thisCamera.farClipPlane;

                        var eyeOffset = _thisCamera.stereoSeparation / 2;

                        //左眼
                        var leftEyePosition = thisTransform.TransformPoint(new Vector3(-eyeOffset, 0, 0));
                        CalMatrix(leftEyePosition, pa, pb, pc, n, f, out var vr, out var vu, out var vn, out var lp, out var lv, out Vector3 va, out Vector3 vb, out Vector3 vc);
                        _thisCamera.SetStereoProjectionMatrix(Camera.StereoscopicEye.Left, lp);
                        _thisCamera.SetStereoViewMatrix(Camera.StereoscopicEye.Left, lv);


                        //右眼
                        var rightEyePosition = thisTransform.TransformPoint(new Vector3(eyeOffset, 0, 0));
                        CalMatrix(rightEyePosition, pa, pb, pc, n, f, vr, vu, vn, out var rp, out var rv);
                        _thisCamera.SetStereoProjectionMatrix(Camera.StereoscopicEye.Right, rp);
                        _thisCamera.SetStereoViewMatrix(Camera.StereoscopicEye.Right, rv);
                        break;
                    }
            }
        }

        /// <summary>
        /// 通过自定义虚拟屏幕更新
        /// </summary>
        private void UpdateByCustomVirtualScreen()
        {
            if (!_screen || !_thisCamera) return;
            {
                var halfScreenSize = this.halfScreenSize;
                var halfWidth = halfScreenSize.x;
                var halfHeight = halfScreenSize.y;

                //如果屏幕尺寸近似为0，不做处理
                if (Mathf.Approximately(halfWidth, 0) || Mathf.Approximately(halfHeight, 0)) return;

                var screenTransform = _screen.transform;
                var mat = Matrix4x4.TRS(screenTransform.position, screenTransform.rotation, Vector3.one);

                //屏幕左下角世界坐标
                pa = mat.MultiplyPoint(new Vector3(-halfWidth, -halfHeight, 0.0f));

                //屏幕右下角世界坐标
                pb = mat.MultiplyPoint(new Vector3(halfWidth, -halfHeight, 0.0f));

                //屏幕左上角世界坐标
                pc = mat.MultiplyPoint(new Vector3(-halfWidth, halfHeight, 0.0f));

                //近裁剪
                n = _thisCamera.nearClipPlane;

                //远裁剪
                f = _thisCamera.farClipPlane;
            }

            {
                //相机位置-双眼中心位置
                var pe = thisTransform.position;
                CalMatrix(pe, pa, pb, pc, n, f, out Vector3 vr, out Vector3 vu, out Vector3 vn, out var cp, out var cv, out Vector3 va, out Vector3 vb, out Vector3 vc);

                // set matrices
                _thisCamera.projectionMatrix = cp;
                _thisCamera.worldToCameraMatrix = cv;

                if (_thisCamera.stereoEnabled)//立体启用的前提下
                {
                    switch (_LREyeMatrixMode)
                    {
                        case ELREyeMatrixMode.SameWithCenter:
                            {
                                _thisCamera.SetStereoProjectionMatrix(Camera.StereoscopicEye.Left, cp);
                                _thisCamera.SetStereoProjectionMatrix(Camera.StereoscopicEye.Right, cp);
                                _thisCamera.SetStereoViewMatrix(Camera.StereoscopicEye.Left, cv);
                                _thisCamera.SetStereoViewMatrix(Camera.StereoscopicEye.Right, cv);
                                break;
                            }
                        case ELREyeMatrixMode.UseCenterProjectionMatrix:
                            {
                                _thisCamera.SetStereoProjectionMatrix(Camera.StereoscopicEye.Left, cp);
                                _thisCamera.SetStereoProjectionMatrix(Camera.StereoscopicEye.Right, cp);

                                var eyeOffset = _thisCamera.stereoSeparation / 2;
                                var viewL = cv;
                                var viewR = cv;
                                viewL[12] += eyeOffset;
                                viewR[12] -= eyeOffset;
                                _thisCamera.SetStereoViewMatrix(Camera.StereoscopicEye.Left, viewL);
                                _thisCamera.SetStereoViewMatrix(Camera.StereoscopicEye.Right, viewR);
                                break;
                            }
                        case ELREyeMatrixMode.CompleteCalculation:
                            {
                                var eyeOffset = _thisCamera.stereoSeparation / 2;

                                //左眼
                                var leftEyePosition = thisTransform.TransformPoint(new Vector3(-eyeOffset, 0, 0));
                                CalMatrix(leftEyePosition, pa, pb, pc, n, f, vr, vu, vn, out var lp, out var lv);
                                _thisCamera.SetStereoProjectionMatrix(Camera.StereoscopicEye.Left, lp);
                                _thisCamera.SetStereoViewMatrix(Camera.StereoscopicEye.Left, lv);


                                //右眼
                                var rightEyePosition = thisTransform.TransformPoint(new Vector3(eyeOffset, 0, 0));
                                CalMatrix(rightEyePosition, pa, pb, pc, n, f, vr, vu, vn, out var rp, out var rv);
                                _thisCamera.SetStereoProjectionMatrix(Camera.StereoscopicEye.Right, rp);
                                _thisCamera.SetStereoViewMatrix(Camera.StereoscopicEye.Right, rv);
                                break;
                            }
                    }
                }


                if (_estimateViewFrustum)
                {
                    // rotate camera to screen for culling to work
                    Quaternion q = new Quaternion();
                    q.SetLookRotation((0.5f * (pb + pc) - pe), vu);
                    // look at center of screen
                    thisTransform.rotation = q;

                    // set fieldOfView to a conservative estimate 
                    // to make frustum tall enough
                    if (_thisCamera.aspect >= 1.0)
                    {
                        _thisCamera.fieldOfView = Mathf.Rad2Deg *
                           Mathf.Atan(((pb - pa).magnitude + (pc - pa).magnitude)
                           / va.magnitude);
                    }
                    else
                    {
                        // take the camera aspect into account to 
                        // make the frustum wide enough 
                        _thisCamera.fieldOfView =
                           Mathf.Rad2Deg / _thisCamera.aspect *
                           Mathf.Atan(((pb - pa).magnitude + (pc - pa).magnitude)
                           / va.magnitude);
                    }
                }
            }
        }

        static void CalMatrix(Vector3 pe, Vector3 pa, Vector3 pb, Vector3 pc, float n, float f, out Vector3 screenRight, out Vector3 screenUp, out Vector3 screenNormal, out Matrix4x4 projectionMatrix, out Matrix4x4 viewMatrix, out Vector3 va, out Vector3 vb, out Vector3 vc)
        {
            var vr = pb - pa;//屏幕右轴向量X
            var vu = pc - pa;//屏幕上轴向量Y
            vr.Normalize();
            vu.Normalize();
            var vn = -Vector3.Cross(vr, vu);//屏幕法向量-使用左手坐标系计算的屏幕法向量Z轴
            // we need the minus sign because Unity 
            // uses a left-handed coordinate system
            vn.Normalize();

            screenRight = vr;
            screenUp = vu;
            screenNormal = vn;

            CalMatrix(pe, pa, pb, pc, n, f, vr, vu, vn, out projectionMatrix, out viewMatrix, out va, out vb, out vc);
        }

        static void CalMatrix(Vector3 pe, Vector3 pa, Vector3 pb, Vector3 pc, float n, float f, Vector3 vr, Vector3 vu, Vector3 vn, out Matrix4x4 projectionMatrix, out Matrix4x4 viewMatrix)
        {
            CalMatrix(pe, pa, pb, pc, n, f, vr, vu, vn, out projectionMatrix, out viewMatrix, out _, out _, out _);
        }

        static void CalMatrix(Vector3 pe, Vector3 pa, Vector3 pb, Vector3 pc, float n, float f, Vector3 vr, Vector3 vu, Vector3 vn, out Matrix4x4 projectionMatrix, out Matrix4x4 viewMatrix, out Vector3 va, out Vector3 vb, out Vector3 vc)
        {
            va = pa - pe; // from pe to pa
            vb = pb - pe; // from pe to pb 
            vc = pc - pe; // from pe to pc

            var d = -Vector3.Dot(va, vn);// distance from eye to screen 
            var l = Vector3.Dot(vr, va) * n / d;// distance to left screen edge
            var r = Vector3.Dot(vr, vb) * n / d;// distance to right screen edge
            var b = Vector3.Dot(vu, va) * n / d;// distance to bottom screen edge
            var t = Vector3.Dot(vu, vc) * n / d;// distance to top screen edge

            Matrix4x4 p = new Matrix4x4(); // projection matrix 
            p[0, 0] = 2.0f * n / (r - l);
            p[0, 1] = 0.0f;
            p[0, 2] = (r + l) / (r - l);
            p[0, 3] = 0.0f;

            p[1, 0] = 0.0f;
            p[1, 1] = 2.0f * n / (t - b);
            p[1, 2] = (t + b) / (t - b);
            p[1, 3] = 0.0f;

            p[2, 0] = 0.0f;
            p[2, 1] = 0.0f;
            p[2, 2] = (f + n) / (n - f);
            p[2, 3] = 2.0f * f * n / (n - f);

            p[3, 0] = 0.0f;
            p[3, 1] = 0.0f;
            p[3, 2] = -1.0f;
            p[3, 3] = 0.0f;


            Matrix4x4 rm = new Matrix4x4(); // rotation matrix;
            rm[0, 0] = vr.x;
            rm[0, 1] = vr.y;
            rm[0, 2] = vr.z;
            rm[0, 3] = 0.0f;

            rm[1, 0] = vu.x;
            rm[1, 1] = vu.y;
            rm[1, 2] = vu.z;
            rm[1, 3] = 0.0f;

            rm[2, 0] = vn.x;
            rm[2, 1] = vn.y;
            rm[2, 2] = vn.z;
            rm[2, 3] = 0.0f;

            rm[3, 0] = 0.0f;
            rm[3, 1] = 0.0f;
            rm[3, 2] = 0.0f;
            rm[3, 3] = 1.0f;

            Matrix4x4 tm = new Matrix4x4(); // translation matrix;
            tm[0, 0] = 1.0f;
            tm[0, 1] = 0.0f;
            tm[0, 2] = 0.0f;
            tm[0, 3] = -pe.x;

            tm[1, 0] = 0.0f;
            tm[1, 1] = 1.0f;
            tm[1, 2] = 0.0f;
            tm[1, 3] = -pe.y;

            tm[2, 0] = 0.0f;
            tm[2, 1] = 0.0f;
            tm[2, 2] = 1.0f;
            tm[2, 3] = -pe.z;

            tm[3, 0] = 0.0f;
            tm[3, 1] = 0.0f;
            tm[3, 2] = 0.0f;
            tm[3, 3] = 1.0f;

            // The original paper puts everything into the projection 
            // matrix (i.e. sets it to p * rm * tm and the other 
            // matrix to the identity), but this doesn't appear to 
            // work with Unity's shadow maps.
            projectionMatrix = p;
            viewMatrix = rm * tm;
        }

        /// <summary>
        /// 计算矩阵
        /// </summary>
        /// <param name="pe">相机位置-眼睛位置</param>
        /// <param name="pa">屏幕左下角世界坐标</param>
        /// <param name="pb">屏幕右下角世界坐标</param>
        /// <param name="pc">屏幕左上角世界坐标</param>
        /// <param name="n">近裁剪</param>
        /// <param name="f">远裁剪</param>
        /// <param name="projectionMatrix">投影矩阵</param>
        /// <param name="viewMatrix">视图矩阵</param>
        static void CalMatrix(Vector3 pe, Vector3 pa, Vector3 pb, Vector3 pc, float n, float f, out Matrix4x4 projectionMatrix, out Matrix4x4 viewMatrix)
        {
            var vr = pb - pa;//屏幕右轴向量X
            var vu = pc - pa;//屏幕上轴向量Y
            vr.Normalize();
            vu.Normalize();
            var vn = -Vector3.Cross(vr, vu);//屏幕法向量-使用左手坐标系计算的屏幕法向量Z轴
            // we need the minus sign because Unity 
            // uses a left-handed coordinate system
            vn.Normalize();

            var va = pa - pe; // from pe to pa
            var vb = pb - pe; // from pe to pb 
            var vc = pc - pe; // from pe to pc

            var d = -Vector3.Dot(va, vn);// distance from eye to screen 
            var l = Vector3.Dot(vr, va) * n / d;// distance to left screen edge
            var r = Vector3.Dot(vr, vb) * n / d;// distance to right screen edge
            var b = Vector3.Dot(vu, va) * n / d;// distance to bottom screen edge
            var t = Vector3.Dot(vu, vc) * n / d;// distance to top screen edge

            Matrix4x4 p = new Matrix4x4(); // projection matrix 
            p[0, 0] = 2.0f * n / (r - l);
            p[0, 1] = 0.0f;
            p[0, 2] = (r + l) / (r - l);
            p[0, 3] = 0.0f;

            p[1, 0] = 0.0f;
            p[1, 1] = 2.0f * n / (t - b);
            p[1, 2] = (t + b) / (t - b);
            p[1, 3] = 0.0f;

            p[2, 0] = 0.0f;
            p[2, 1] = 0.0f;
            p[2, 2] = (f + n) / (n - f);
            p[2, 3] = 2.0f * f * n / (n - f);

            p[3, 0] = 0.0f;
            p[3, 1] = 0.0f;
            p[3, 2] = -1.0f;
            p[3, 3] = 0.0f;


            Matrix4x4 rm = new Matrix4x4(); // rotation matrix;
            rm[0, 0] = vr.x;
            rm[0, 1] = vr.y;
            rm[0, 2] = vr.z;
            rm[0, 3] = 0.0f;

            rm[1, 0] = vu.x;
            rm[1, 1] = vu.y;
            rm[1, 2] = vu.z;
            rm[1, 3] = 0.0f;

            rm[2, 0] = vn.x;
            rm[2, 1] = vn.y;
            rm[2, 2] = vn.z;
            rm[2, 3] = 0.0f;

            rm[3, 0] = 0.0f;
            rm[3, 1] = 0.0f;
            rm[3, 2] = 0.0f;
            rm[3, 3] = 1.0f;

            Matrix4x4 tm = new Matrix4x4(); // translation matrix;
            tm[0, 0] = 1.0f;
            tm[0, 1] = 0.0f;
            tm[0, 2] = 0.0f;
            tm[0, 3] = -pe.x;

            tm[1, 0] = 0.0f;
            tm[1, 1] = 1.0f;
            tm[1, 2] = 0.0f;
            tm[1, 3] = -pe.y;

            tm[2, 0] = 0.0f;
            tm[2, 1] = 0.0f;
            tm[2, 2] = 1.0f;
            tm[2, 3] = -pe.z;

            tm[3, 0] = 0.0f;
            tm[3, 1] = 0.0f;
            tm[3, 2] = 0.0f;
            tm[3, 3] = 1.0f;

            // The original paper puts everything into the projection 
            // matrix (i.e. sets it to p * rm * tm and the other 
            // matrix to the identity), but this doesn't appear to 
            // work with Unity's shadow maps.
            projectionMatrix = p;
            viewMatrix = rm * tm;
        }

        /// <summary>
        /// 选中时绘制Gizmos
        /// </summary>
        public void OnDrawGizmosSelected()
        {
            switch (_updateMode)
            {
                case EUpdateMode.CustomVirtualScreen:
                    {
                        if (screen)
                        {
                            var screenCenter = screen.transform.position;
                            XGizmos.DrawPath(Color.blue, thisTransform.position, screenCenter);

                            var bakColor = Gizmos.color;
                            Gizmos.color = Color.blue;
                            Gizmos.DrawSphere(screenCenter, 0.05f);
                            Gizmos.color = bakColor;
                        }
                        break;
                    }
                default:
                    {
                        if(_thisCamera)
                        {
                            var screenCenter = _thisCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, _thisCamera.stereoConvergence));

                            var dis = _thisCamera.stereoConvergence;
                            var leftBottom = _thisCamera.ViewportToWorldPoint(new Vector3(0, 0, dis));
                            var leftTop = _thisCamera.ViewportToWorldPoint(new Vector3(0, 1, dis));
                            var rightBottom = _thisCamera.ViewportToWorldPoint(new Vector3(1, 0, dis));

                            var mat = Matrix4x4.TRS(screenCenter, _thisCamera.transform.rotation, Vector3.one);

                            //绘制Gizmos
                            var bak = Gizmos.matrix;
                            var bakColor = Gizmos.color;

                            Gizmos.matrix = mat;
                            Gizmos.color = Color.blue;
                            Gizmos.DrawCube(Vector3.zero, new Vector3((rightBottom - leftBottom).magnitude, (leftTop - leftBottom).magnitude, 0.01f));
                            Gizmos.matrix = bak;

                            Gizmos.color = Color.green;
                            Gizmos.DrawSphere(screenCenter, 0.05f);
                            Gizmos.color = bakColor;

                            XGizmos.DrawPath(Color.green, thisTransform.position, screenCenter);
                        }
                        break;
                    }
            }
        }

        /// <summary>
        /// 从指定相机透视拷贝数据
        /// </summary>
        /// <param name="cameraProjection"></param>
        public void CopyDataFrom(CameraProjection cameraProjection)
        {
            if (!cameraProjection) return;
            this.XModifyProperty(() =>
            {
                this._updateMode = cameraProjection._updateMode;
                this._LREyeMatrixMode = cameraProjection._LREyeMatrixMode;
                this._setMatrixRuleOnDisable = cameraProjection._setMatrixRuleOnDisable;
                this._screen = cameraProjection._screen;
                this._estimateViewFrustum = cameraProjection._estimateViewFrustum;
            });
        }

        /// <summary>
        /// 为主动立体设置相机属性
        /// </summary>
        public void SetCameraPropertyForActiveStereo() => _thisCamera.SetCameraPropertyForActiveStereo();
    }
}
