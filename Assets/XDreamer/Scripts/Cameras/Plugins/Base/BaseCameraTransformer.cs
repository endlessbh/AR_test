using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Components;
using XCSJ.Extension.Base.Recorders;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Base.Kernel;

namespace XCSJ.PluginsCameras.Base
{
    /// <summary>
    /// 基础相机变换器
    /// </summary>
    public abstract class BaseCameraTransformer : BaseCameraCoreController, ICameraTransformer
    {
        #region 变换

        [Name("主变换")]
        [Tip("如果当前参数对象无效，则使用相机主控制器所在游戏对象的变换为本参数对象；", "If the current parameter object is invalid, use the transformation of the game object where the camera main controller is located to this parameter object;")]
        public Transform _mainTransform;

        /// <summary>
        /// 主变换
        /// </summary>
        public Transform mainTransform
        {
            get
            {
                if (!_mainTransform)
                {
                    _mainTransform = mainController.transform;
                }
                return _mainTransform;
            }
        }

        public Transform[] transforms => new Transform[] { mainTransform };

        public Transform firstTransform => mainTransform;

        #endregion

        #region 变换记录器

        /// <summary>
        /// 变换记录器列表
        /// </summary>
        public List<TransformRecorder> transformRecorders { get; } = new List<TransformRecorder>();

        ITransformRecorder[] ITransformer.transformRecorders => transformRecorders.ToArray();

        ITransformRecorder ITransformer.transformRecorder => transformRecorders.FirstOrDefault();

        ITransformRecord ITransformer.firstTransformRecord => transformRecorders.FirstOrDefault()?.firstRecod;

        ITransformRecorder ITransformer.lastTransformRecorder => lastTransformRecorder;

        /// <summary>
        /// 变换记录器列表中末一个记录器
        /// </summary>
        public TransformRecorder lastTransformRecorder => transformRecorders.LastOrDefault();

        /// <summary>
        /// 原始变换记录器；记录着原始变换信息；即变换记录器列表中第一个记录；
        /// </summary>
        public TransformRecorder originalTransformRecorder => transformRecorders.FirstOrDefault();

        /// <summary>
        /// 记录：记录主变换的当前PRS信息,添加到变换记录器列表<see cref="transformRecorders"/>的末尾；
        /// </summary>
        public void Record()
        {
            var mainTransform = this.mainTransform;
            if (mainTransform)
            {
                var transformRecorder = new TransformRecorder();
                transformRecorder.Record(mainTransform);
                transformRecorders.Add(transformRecorder);
            }
        }

        /// <summary>
        /// 恢复到原始状态:将相机控制器的变换恢复到程序启动时记录的状态
        /// </summary>
        public void RecoverToOriginal() => originalTransformRecorder?.Recover();

        /// <summary>
        /// 恢复到上一次状态:将相机控制器的变换恢复到上一次记录的状态
        /// </summary>
        public void RecoverToLast() => lastTransformRecorder?.Recover();

        #endregion

        #region 变换操作

        /// <summary>
        /// 右
        /// </summary>
        public Vector3 right => mainTransform.right;

        /// <summary>
        /// 上
        /// </summary>
        public Vector3 up => mainTransform.up;

        /// <summary>
        /// 前
        /// </summary>
        public Vector3 forward => mainTransform.forward;

        /// <summary>
        /// 相机位置
        /// </summary>
        public Vector3 position
        {
            get => mainTransform.position;
            set => mainTransform.position = value;
        }

        /// <summary>
        /// 旋转量
        /// </summary>
        public Quaternion rotation
        {
            get => mainTransform.rotation;
            set => mainTransform.rotation = value;
        }

        /// <summary>
        /// 欧拉角度
        /// </summary>
        public Vector3 eulerAngles => mainTransform.eulerAngles;

        /// <summary>
        /// 本地位置量
        /// </summary>
        public Vector3 localPosition
        {
            get => mainTransform.localPosition;
            set => mainTransform.localPosition = value;
        }

        /// <summary>
        /// 本地旋转量
        /// </summary>
        public Quaternion localRotation
        {
            get => mainTransform.localRotation;
            set => mainTransform.localRotation = value;
        }

        /// <summary>
        /// 设置变换
        /// </summary>
        /// <param name="spaceType"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        public void SetTransform(ESpaceType spaceType, Vector3 position, Quaternion rotation)
        {
            switch (spaceType)
            {
                case ESpaceType.World:
                    {
                        var mainTransform = this.mainTransform;
                        mainTransform.position = position;
                        mainTransform.rotation = rotation;
                        break;
                    }
                case ESpaceType.Local:
                    {
                        var mainTransform = this.mainTransform;
                        mainTransform.localPosition = position;
                        mainTransform.localRotation = rotation;
                        break;
                    }
            }
        }

        /// <summary>
        /// 获取旋转量
        /// </summary>
        /// <param name="spaceType"></param>
        /// <returns></returns>
        public Quaternion GetRotation(ESpaceType spaceType)
        {
            switch (spaceType)
            {
                case ESpaceType.World:
                    {
                        return mainTransform.rotation;
                    }
                case ESpaceType.Local:
                    {
                        return mainTransform.localRotation;
                    }
                default:
                    {
                        return Quaternion.identity;
                    }
            }
        }

        /// <summary>
        /// 设置旋转量
        /// </summary>
        /// <param name="spaceType"></param>
        /// <param name="position"></param>
        public void SetPosition(ESpaceType spaceType, Vector3 position)
        {
            switch (spaceType)
            {
                case ESpaceType.World:
                    {
                        mainTransform.position = position;
                        break;
                    }
                case ESpaceType.Local:
                    {
                        mainTransform.localPosition = position;
                        break;
                    }
            }
        }

        /// <summary>
        /// 设置旋转量
        /// </summary>
        /// <param name="spaceType"></param>
        /// <param name="rotation"></param>
        public void SetRotation(ESpaceType spaceType, Quaternion rotation)
        {
            switch (spaceType)
            {
                case ESpaceType.World:
                    {
                        mainTransform.rotation = rotation;
                        break;
                    }
                case ESpaceType.Local:
                    {
                        mainTransform.localRotation = rotation;
                        break;
                    }
            }
        }

        /// <summary>
        /// 旋转
        /// </summary>
        /// <param name="eulers"></param>
        /// <param name="relativeTo"></param>
        /// <param name="callback"></param>
        public void Rotate(Vector3 eulers, Space relativeTo, bool callback = true)
        {
            if (callback) onBeforeRotate?.Invoke(this, eulers, relativeTo);
            mainTransform.Rotate(eulers, relativeTo);
            if (callback) onAfterRotate?.Invoke(this, eulers, relativeTo);
        }

        /// <summary>
        /// 旋转前事件：当相机变换将要执行发生<see cref="Transform.Rotate(Vector3, Space)"/>操作前的回调事件；参数依次为：相机变换器、旋转角度、旋转角度空间
        /// </summary>
        public static event Action<BaseCameraTransformer, Vector3, Space> onBeforeRotate;

        /// <summary>
        /// 旋转后事件：当相机变换已执行<see cref="Transform.Rotate(Vector3, Space)"/>操作后的回调事件；参数依次为：相机变换器、旋转角度、旋转角度空间
        /// </summary>
        public static event Action<BaseCameraTransformer, Vector3, Space> onAfterRotate;

        /// <summary>
        /// 朝向
        /// </summary>
        /// <param name="worldPosition"></param>
        public void LookAt(Vector3 worldPosition, Vector3 worldUp)
        {
            mainTransform.LookAt(worldPosition, worldUp);
        }

        /// <summary>
        /// 朝向
        /// </summary>
        /// <param name="worldPosition"></param>
        public void LookAt(Vector3 worldPosition) => LookAt(worldPosition, Vector3.up);

        /// <summary>
        /// 朝向
        /// </summary>
        /// <param name="target"></param>
        public void LookAt(Transform target) => mainTransform.LookAt(target);

        /// <summary>
        /// 绕物旋转
        /// </summary>
        /// <param name="point"></param>
        /// <param name="axis"></param>
        /// <param name="angle"></param>
        /// <param name="callback"></param>
        public void RotateAround(Vector3 point, Vector3 axis, float angle, bool callback = true)
        {
            if (callback) onBeforeRotateAround?.Invoke(this, point, axis, angle);
            mainTransform.RotateAround(point, axis, angle);
            if (callback) onAfterRotateAround?.Invoke(this, point, axis, angle);
        }

        /// <summary>
        /// 绕物旋转前事件：当相机变换将要执行发生<see cref="Transform.RotateAround(Vector3, Vector3, float)"/>操作前的回调事件；参数依次为：相机变换器、旋转点、旋转轴、旋转角度
        /// </summary>
        public static event Action<BaseCameraTransformer, Vector3, Vector3, float> onBeforeRotateAround;

        /// <summary>
        /// 绕物旋转后事件：当相机变换已执行<see cref="Transform.RotateAround(Vector3, Vector3, float)"/>操作后的回调事件；参数依次为：相机变换器、旋转点、旋转轴、旋转角度
        /// </summary>
        public static event Action<BaseCameraTransformer, Vector3, Vector3, float> onAfterRotateAround;

        /// <summary>
        /// 变换
        /// </summary>
        /// <param name="translation"></param>
        /// <param name="relativeTo"></param>
        /// <param name="callback"></param>
        public void Translate(Vector3 translation, Space relativeTo, bool callback = true)
        {
            if (callback) onBeforeTranslate?.Invoke(this, translation, relativeTo);
            mainTransform.Translate(translation, relativeTo);
            if (callback) onAfterTranslate?.Invoke(this, translation, relativeTo);
        }

        /// <summary>
        /// 设置位置
        /// </summary>
        /// <param name="position"></param>
        /// <param name="relativeTo"></param>
        /// <param name="callback"></param>
        public void SetPosition(Vector3 position, Space relativeTo, bool callback = true)
        {
            var offset = position - (relativeTo == Space.World ? cameraController.transform.position : cameraController.transform.localPosition);
            Translate(offset, relativeTo, callback);
        }

        /// <summary>
        /// 变换前事件：当相机变换将要执行发生<see cref="Transform.Translate(Vector3, Space)"/>操作前的回调事件；参数依次为：相机变换器、变换移动量、变换移动量空间
        /// </summary>
        public static event Action<BaseCameraTransformer, Vector3, Space> onBeforeTranslate;

        /// <summary>
        /// 变换后事件：当相机变换已执行<see cref="Transform.Translate(Vector3, Space)"/>操作后的回调事件；参数依次为：相机变换器、变换移动量、变换移动量空间
        /// </summary>
        public static event Action<BaseCameraTransformer, Vector3, Space> onAfterTranslate;

        /// <summary>
        /// 变换到补间对象
        /// </summary>
        private object _transformToTween = null;

        /// <summary>
        /// 是否正在执行变换到补间动画
        /// </summary>
        public bool inTransformToTween => _transformToTween != null;

        /// <summary>
        /// 变换到
        /// </summary>
        /// <param name="dstPosition">目标位置：世界坐标系</param>
        /// <param name="dstRotation">目标旋转：世界坐标系</param>
        /// <param name="time"></param>
        /// <param callback="time"></param>
        public void TransformTo(Vector3 dstPosition, Quaternion dstRotation, float time, bool callback = true)
        {
            if (inTransformToTween) return;

            var cameraTransformer = this.cameraTransformer;
            var tweenTransform = cameraTransformer.mainTransform;

            Vector3 srcPosition = default;
            Quaternion srcRotation = default;
            if (callback)
            {
                srcPosition = tweenTransform.position;
                srcRotation = tweenTransform.rotation;
                onBeforeTransformTo?.Invoke(cameraTransformer, dstPosition, dstRotation, time);
            }

            _transformToTween = TweenHandler.To(tweenTransform, dstPosition, dstRotation, time, (o, os) =>
            {
                _transformToTween = null;
                if (callback) onAfterTransformTo?.Invoke(cameraTransformer, srcPosition, srcRotation, time);
            });
        }

        /// <summary>
        /// 当变换到前事件：当相机变换将要执行发生<see cref="TransformTo(Vector3, Quaternion, float)"/>操作前的回调事件；参数依次为：相机变换器、目标世界位置、目标世界旋转、时间
        /// </summary>
        public static event Action<BaseCameraTransformer, Vector3, Quaternion, float> onBeforeTransformTo;

        /// <summary>
        /// 当变换到后事件：当相机变换将要执行发生<see cref="TransformTo(Vector3, Quaternion, float)"/>操作前的回调事件；参数依次为：相机变换器、源世界位置、源世界旋转、时间
        /// </summary>
        public static event Action<BaseCameraTransformer, Vector3, Quaternion, float> onAfterTransformTo;

        /// <summary>
        /// 投影到平面
        /// </summary>
        /// <param name="up">待投影平面的法向量Y</param>
        /// <param name="forwardProjection">前向量投影Z</param>
        /// <param name="rightProjection">右向量投影X</param>
        public void ProjectOnPlane(Vector3 planeNormal, out Vector3 forwardProjection, out Vector3 rightProjection)
        {
            forwardProjection = Vector3.ProjectOnPlane(forward, planeNormal);//z

            if (forwardProjection == Vector3.zero)
            {
                rightProjection = Vector3.ProjectOnPlane(right, planeNormal);//x
                forwardProjection = Vector3.Cross(rightProjection, Vector3.up);//z
            }
            else
            {
                rightProjection = Vector3.Cross(Vector3.up, forwardProjection);//x
            }
        }

        #endregion

        #region 移动

        /// <summary>
        /// 移动速度系数
        /// </summary>
        [Group("移动", textEN = "Move")]
        [Name("移动速度系数")]
        public Vector3 _moveSpeedCoefficient = Vector3.one;

        /// <summary>
        /// 移动速度系数
        /// </summary>
        public Vector3 moveSpeedCoefficient
        {
            get => _moveSpeedCoefficient;
            set => this.XModifyProperty(ref _moveSpeedCoefficient, value);
        }

        /// <summary>
        /// 移动阻尼系数
        /// </summary>
        [Name("移动阻尼系数")]
        [Range(0, CameraHelperExtension.MaxDampingCoefficient)]
        public float _moveDampingCoefficient = 1;

        /// <summary>
        /// 移动阻尼系数
        /// </summary>
        public float moveDampingCoefficient
        {
            get => _moveDampingCoefficient;
            set => this.XModifyProperty(ref _moveDampingCoefficient, value);
        }

        /// <summary>
        /// 移动
        /// </summary>
        /// <param name="value"></param>
        /// <param name="moveMode"></param>
        public abstract void Move(Vector3 value, int moveMode);

        #endregion

        #region 旋转

        /// <summary>
        /// 旋转速度系数
        /// </summary>
        [Group("旋转", textEN = "Rotate")]
        [Name("旋转速度系数")]
        public Vector3 _rotatespeedCoefficient = Vector3.one;

        /// <summary>
        /// 旋转速度系数
        /// </summary>
        public Vector3 rotateSpeedCoefficient
        {
            get => _rotatespeedCoefficient;
            set => this.XModifyProperty(ref _rotatespeedCoefficient, value);
        }

        /// <summary>
        /// 旋转阻尼系数
        /// </summary>
        [Name("旋转阻尼系数")]
        [Range(0, CameraHelperExtension.MaxDampingCoefficient)]
        public float _rotateDampingCoefficient = 1;

        /// <summary>
        /// 旋转阻尼系数
        /// </summary>
        public float rotateDampingCoefficient
        {
            get => _rotateDampingCoefficient;
            set => this.XModifyProperty(ref _rotateDampingCoefficient, value);
        }

        /// <summary>
        /// 旋转
        /// </summary>
        /// <param name="value"></param>
        /// <param name="rotateMode"></param>
        public abstract void Rotate(Vector3 value, int rotateMode);

        #endregion

        #region ICameraTransformer接口实现

        /// <summary>
        /// 将相机的变换信息重置到程序启动时记录的状态：即恢复到原始状态
        /// </summary>
        public void ResetCameraToStart() => RecoverToOriginal();

        /// <summary>
        /// 将相机的变换信息重置到缺省状态，即相对父级位置、旋转均为0，缩放全为1的初始状态；
        /// </summary>
        public void ResetCamera() => mainTransform.XResetLocalPRS();

        #endregion

        #region 相机切换

        /// <summary>
        /// 切换相机时的补间对象
        /// </summary>
        public object switchTweener { get; private set; }

        /// <summary>
        /// 当将要开始强制切换时回调；
        /// </summary>
        /// <param name="targetCameraController">期望切换到的目标相机控制器</param>
        /// <param name="directSwitch">标识是直接切换还是补间切换</param>
        /// <returns>支持强制切换返回True，不支持强制切换返回False;</returns>
        public bool DefaultOnWillBeginMustSwitch(BaseCameraMainController targetCameraController, bool directSwitch)
        {
            if (switchTweener != null)//正在切换中...
            {
                //移除当前正在执行的切换补间
                TweenHandler.Kill(switchTweener);
                switchTweener = null;
            }
            return true;
        }

        /// <summary>
        /// 当开始切换补间时回调
        /// </summary>
        /// <param name="targetCameraController">期望切换到的目标相机控制器</param>
        /// <param name="duration">补间的持续时间</param>
        /// <param name="onCompleted">补间完成后的回调</param>
        public void DefaultOnBeginSwitchTween(BaseCameraMainController targetCameraController, float duration, Action onCompleted)
        {
            //需要补间，记录当前相机控制器的变换信息
            Record();

            //目标变换
            var targetTransform = targetCameraController.cameraTransformer.mainTransform;

            //执行补间切换
            switchTweener = TweenHandler.To(mainTransform, targetTransform.position, targetTransform.rotation, duration, (o, os) =>
            {
                onCompleted?.Invoke();
            });
        }

        /// <summary>
        /// 当结束切换补间后回调
        /// </summary>
        /// <param name="newCurrentCameraController">新的当前相机控制器</param>
        public void DefaultOnEndSwitchTween(BaseCameraMainController newCurrentCameraController)
        {
            //恢复PR
            RecoverToLast();

            //置空对象
            switchTweener = null;
        }

        /// <summary>
        /// 当将要开始强制切换时回调
        /// </summary>
        /// <param name="targetCameraController">期望切换到的目标相机控制器</param>
        /// <param name="directSwitch">标识是直接切换还是补间切换</param>
        /// <returns>支持强制切换返回True，不支持强制切换返回False;</returns>
        public virtual bool OnWillBeginMustSwitch(BaseCameraMainController targetCameraController, bool directSwitch) => DefaultOnWillBeginMustSwitch(targetCameraController, directSwitch);

        /// <summary>
        /// 当开始切换补间时回调
        /// </summary>
        /// <param name="targetCameraController">期望切换到的目标相机控制器</param>
        /// <param name="duration">补间的持续时间</param>
        /// <param name="onCompleted">补间完成后的回调</param>
        public virtual void OnBeginSwitchTween(BaseCameraMainController targetCameraController, float duration, Action onCompleted) => DefaultOnBeginSwitchTween(targetCameraController, duration, onCompleted);

        /// <summary>
        /// 当结束切换补间后回调
        /// </summary>
        /// <param name="newCurrentCameraController">新的当前相机控制器</param>
        public virtual void OnEndSwitchTween(BaseCameraMainController newCurrentCameraController) => DefaultOnEndSwitchTween(newCurrentCameraController);

        #endregion

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            var mainController = this.mainController;
            if (mainController)
            {
                mainController.cameraTransformer = this;
            }
            _transformToTween = null;//变换到补间对象置空
            Record();
        }

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();

            if (!mainTransform) { }
        }
    }
}
