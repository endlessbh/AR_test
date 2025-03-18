using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginsCameras.Base;
using XCSJ.PluginsCameras.Tools.Base;

namespace XCSJ.PluginsCameras.Controllers
{
    /// <summary>
    /// 相机变换器
    /// </summary>
    [Name("相机变换器")]
    [DisallowMultipleComponent]
    public class CameraTransformer : BaseCameraTransformer
    {
        #region 相机切换补间器

        /// <summary>
        /// 相机切换补间器
        /// </summary>
        [Group("相机切换补间器", textEN = "Camera Switch Tweener")]
        [Name("相机切换补间器")]
        [Tip("用于覆盖默认相机切换补间操作的相机切换补间器对象；如果本对象无效，则使用默认规则；仅影响当前相机控制器切换到其他相机控制器时的补间效果；", "A camera switch mender object used to override the default camera switch mending operation; If this object is invalid, the default rule will be used; It only affects the gap effect when the current camera controller is switched to other camera controllers;")]
        [ComponentPopup]
        public BaseCameraSwitchTweener _cameraSwitchTweener;

        /// <summary>
        /// 相机切换补间器
        /// </summary>
        public BaseCameraSwitchTweener cameraSwitchTweener
        {
            get => _cameraSwitchTweener;
            set => this.XModifyProperty(ref _cameraSwitchTweener, value, nameof(cameraSwitchTweener));
        }

        /// <summary>
        /// 当将要开始强制切换时回调
        /// </summary>
        /// <param name="targetCameraController">期望切换到的目标相机控制器</param>
        /// <param name="directSwitch">标识是直接切换还是补间切换</param>
        /// <returns>支持强制切换返回True，不支持强制切换返回False;</returns>
        public override bool OnWillBeginMustSwitch(BaseCameraMainController targetCameraController, bool directSwitch)
        {
            if(cameraSwitchTweener)
            {
                return cameraSwitchTweener.OnWillBeginMustSwitch(targetCameraController, directSwitch);
            }
            else
            {
                return base.OnWillBeginMustSwitch(targetCameraController, directSwitch);
            }
        }

        /// <summary>
        /// 当开始切换补间时回调
        /// </summary>
        /// <param name="targetCameraController">期望切换到的目标相机控制器</param>
        /// <param name="duration">补间的持续时间</param>
        /// <param name="onCompleted">补间完成后的回调</param>
        public override void OnBeginSwitchTween(BaseCameraMainController targetCameraController, float duration, Action onCompleted)
        {
            if (cameraSwitchTweener)
            {
                cameraSwitchTweener.OnBeginSwitchTween(targetCameraController, duration, onCompleted);
            }
            else
            {
                base.OnBeginSwitchTween(targetCameraController, duration, onCompleted);
            }
        }

        /// <summary>
        /// 当结束切换补间后回调
        /// </summary>
        /// <param name="newCurrentCameraController">新的当前相机控制器</param>
        public override void OnEndSwitchTween(BaseCameraMainController newCurrentCameraController)
        {
            if (cameraSwitchTweener)
            {
                cameraSwitchTweener.OnEndSwitchTween(newCurrentCameraController);
            }
            else
            {
                base.OnEndSwitchTween(newCurrentCameraController);
            }
        }

        #endregion

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            offsets.Clear();
        }

        /// <summary>
        /// 更新
        /// </summary>
        protected virtual void Update()
        {
            foreach (var kv in offsets)
            {
                TRInternal(kv.Value, kv.Key);
            }
            offsets.Clear();
        }

        /// <summary>
        /// 一帧内需要处理的便宜列表；在更新时做处理；
        /// </summary>
        Dictionary<ETRMode, Vector3> offsets = new Dictionary<ETRMode, Vector3>();

        /// <summary>
        /// 移动
        /// </summary>
        /// <param name="value"></param>
        /// <param name="moveMode"></param>
        public override void Move(Vector3 value, int moveMode) => TR(value, (ETRMode)moveMode);

        /// <summary>
        /// 旋转
        /// </summary>
        /// <param name="value"></param>
        /// <param name="rotateMode"></param>
        public override void Rotate(Vector3 value, int rotateMode) => TR(value, (ETRMode)(rotateMode + RotateModeOffset));

        /// <summary>
        /// 移动旋转
        /// </summary>
        /// <param name="value"></param>
        /// <param name="trMode"></param>
        public void TR(Vector3 value, ETRMode trMode)
        {
            if (offsets.TryGetValue(trMode, out var tmpValue))//有则累加
            {
                offsets[trMode] = tmpValue + value;
            }
            else//无则赋值
            {
                offsets[trMode] = value;
            }
        }

        private void TRInternal(Vector3 offset, ETRMode trMode)
        {
            switch (trMode)
            {
                #region 移动

                case ETRMode.Translate__Self_Local:
                    {
                        cameraTransformer.Translate(offset, Space.Self);
                        break;
                    }
                case ETRMode.Translate__Self_World:
                    {
                        cameraTransformer.Translate(offset, Space.World);
                        break;
                    }
                case ETRMode.Translate__Self_LocalYProjectToWorldX0Z:
                    {
                        cameraTransformer.Translate(new Vector3(offset.x, 0, offset.z), Space.Self);

                        cameraTransformer.ProjectOnPlane(Vector3.up, out var forward, out _);
                        cameraTransformer.Translate(forward.normalized * offset.y, Space.World);
                        break;
                    }
                case ETRMode.Translate__Self_LocalXZWorldY:
                    {
                        cameraTransformer.Translate(new Vector3(offset.x, 0, offset.z), Space.Self);
                        cameraTransformer.Translate(new Vector3(0, offset.y, 0), Space.World);
                        break;
                    }
                case ETRMode.Translate__Self_LocalXYProjectToWorldX0Z:
                    {
                        cameraTransformer.Translate(new Vector3(0, 0, offset.z), Space.Self);

                        cameraTransformer.ProjectOnPlane(Vector3.up, out var forward, out var right);
                        cameraTransformer.Translate(forward.normalized * offset.y + right.normalized * offset.x, Space.World);
                        break;
                    }

                #endregion

                #region 旋转
                case ETRMode.Rotate__Self_Local:
                    {
                        cameraTransformer.Rotate(offset, Space.Self);
                        break;
                    }
                case ETRMode.Rotate__Self_World:
                    {
                        cameraTransformer.Rotate(offset, Space.World);
                        break;
                    }
                case ETRMode.Rotate__Self_LocalXZ__Self_WorldY:
                    {
                        var cameraTransformer = this.cameraTransformer;

                        cameraTransformer.Rotate(new Vector3(offset.x, 0, offset.z), Space.Self);
                        cameraTransformer.Rotate(new Vector3(0, offset.y, 0), Space.World);
                        break;
                    }
                case ETRMode.Rotate__Target_Local:
                    {
                        var cameraTransformer = this.cameraTransformer;
                        var cameraTargetController = this.cameraController.cameraTargetController;
                        if (!cameraTransformer || !cameraTargetController) break;

                        var targetPosition = cameraTargetController.targetPosition;

                        cameraTransformer.RotateAround(targetPosition, cameraTransformer.right, offset.x);
                        cameraTransformer.RotateAround(targetPosition, cameraTransformer.up, offset.y);
                        cameraTransformer.RotateAround(targetPosition, cameraTransformer.forward, offset.z);
                        break;
                    }
                case ETRMode.Rotate__Target_World:
                    {
                        var cameraTransformer = this.cameraTransformer;
                        var cameraTargetController = this.cameraController.cameraTargetController;
                        if (!cameraTransformer || !cameraTargetController) break;

                        var targetPosition = cameraTargetController.targetPosition;

                        cameraTransformer.RotateAround(targetPosition, Vector3.right, offset.x);
                        cameraTransformer.RotateAround(targetPosition, Vector3.up, offset.y);
                        cameraTransformer.RotateAround(targetPosition, Vector3.forward, offset.z);
                        break;
                    }
                case ETRMode.Rotate__Target_LocalX__Targert_WorldY:
                    {
                        var cameraTransformer = this.cameraTransformer;
                        var cameraTargetController = this.cameraController.cameraTargetController;
                        if (!cameraTransformer || !cameraTargetController) break;

                        var targetPosition = cameraTargetController.targetPosition;

                        cameraTransformer.RotateAround(targetPosition, cameraTransformer.right, offset.x);
                        cameraTransformer.RotateAround(targetPosition, Vector3.up, offset.y);
                        break;
                    }
                case ETRMode.Rotate__Target_LocalXZ__Targert_WorldY:
                    {
                        var cameraTransformer = this.cameraTransformer;
                        var cameraTargetController = this.cameraController.cameraTargetController;
                        if (!cameraTransformer || !cameraTargetController) break;

                        var targetPosition = cameraTargetController.targetPosition;

                        cameraTransformer.RotateAround(targetPosition, cameraTransformer.right, offset.x);
                        cameraTransformer.RotateAround(targetPosition, Vector3.up, offset.y);
                        cameraTransformer.RotateAround(targetPosition, cameraTransformer.forward, offset.z);
                        break;
                    }
                case ETRMode.Rotate__Target_LocalXZProjectToWorldX0Z__Targert_WorldY:
                    {
                        var cameraTransformer = this.cameraTransformer;
                        var cameraTargetController = this.cameraController.cameraTargetController;
                        if (!cameraTransformer || !cameraTargetController) break;

                        var targetPosition = cameraTargetController.targetPosition;

                        cameraTransformer.ProjectOnPlane(Vector3.up, out var forward, out var right);

                        cameraTransformer.RotateAround(targetPosition, right, offset.x);
                        cameraTransformer.RotateAround(targetPosition, Vector3.up, offset.y);
                        cameraTransformer.RotateAround(targetPosition, forward, offset.z);
                        break;
                    }
                    #endregion
            }
        }

        internal const int RotateModeOffset = 100;
    }

    /// <summary>
    /// 移动旋转模式：Self自身表示<see cref="CameraTransformer"/>指向的真实对象；Target目标表示<see cref="CameraTargetController"/>指向的真实目标对象；
    /// </summary>
    [Name("移动旋转模式")]
    public enum ETRMode
    {
        /// <summary>
        /// 无
        /// </summary>
        [Name("无")]
        None = 0,

        #region 移动

        /// <summary>
        /// 自身本地
        /// </summary>
        [Name("移动/自身本地")]
        [Tip("以相机控制器自身本地坐标系为基准，进行移动控制", "Take the local coordinate system of the camera controller as the benchmark to control the movement")]
        Translate__Self_Local,

        /// <summary>
        /// 自身世界
        /// </summary>
        [Name("移动/自身世界")]
        [Tip("以相机控制器自身世界坐标系为基准，进行移动控制", "Take the camera controller's own world coordinate system as the benchmark to control the movement")]
        Translate__Self_World,

        /// <summary>
        /// 自身本地Y投影到世界X0Z面
        /// </summary>
        [Name("移动/自身本地Y投影到世界X0Z面")]
        [Tip("本地轴XZ不变,本地Y轴投影到世界X0Z面", "The local axis XZ remains unchanged, and the local Y axis is projected to the world x0z plane")]
        Translate__Self_LocalYProjectToWorldX0Z,

        /// <summary>
        /// 自身本地Y投影到世界X0Z面
        /// </summary>
        [Name("移动/自身本地Y投影到世界Y")]
        [Tip("本地轴X不变,本地Y轴投影到世界Y轴，本地Z不变")]
        Translate__Self_LocalXZWorldY,

        /// <summary>
        /// 自身本地XY投影到世界X0Z面
        /// </summary>
        [Name("移动/自身本地XY投影到世界X0Z面")]
        [Tip("本地XY轴投影到世界X0Z面，本地Z不变", "The local XY axis is projected to the world x0z plane, and the local Z remains unchanged")]
        Translate__Self_LocalXYProjectToWorldX0Z,

        #endregion

        #region 旋转

        /// <summary>
        /// 自身本地:以自身为旋转中心，本地坐标系的三轴(右轴right对应X轴,上轴up对应Y轴,forward对应Z轴)为旋转轴，执行XYZ轴旋转逻辑;
        /// </summary>
        [Name("旋转/自身本地")]
        [Tip("以自身为旋转中心，本地坐标系的三轴(右轴right对应X轴,上轴up对应Y轴,forward对应Z轴)为旋转轴，执行XYZ轴旋转逻辑;", "Take itself as the rotation center and the three axes of the local coordinate system (right axis corresponds to X axis, up axis corresponds to y axis and forward axis corresponds to Z axis) as the rotation axis to execute the XYZ axis rotation logic;")]
        Rotate__Self_Local = CameraTransformer.RotateModeOffset + 1,

        /// <summary>
        /// 自身世界:以自身为旋转中心，世界坐标系的三轴(右轴right对应X轴,上轴up对应Y轴,forward对应Z轴)为旋转轴，执行XYZ轴旋转逻辑
        /// </summary>
        [Name("旋转/自身世界")]
        [Tip("以自身为旋转中心，世界坐标系的三轴(右轴right对应X轴,上轴up对应Y轴,forward对应Z轴)为旋转轴，执行XYZ轴旋转逻辑", "Taking itself as the rotation center, the three axes of the world coordinate system (right axis corresponds to X axis, up axis corresponds to y axis, and forward axis corresponds to Z axis) are the rotation axes, and the XYZ axis rotation logic is executed")]
        Rotate__Self_World,

        /// <summary>
        /// 自身本地XZ与自身世界Y:以自身为旋转中心，本地坐标系的XZ轴(右轴right对应X轴,forward对应Z轴)为旋转轴，执行旋转逻辑;以自身为旋转中心，世界坐标系的Y轴(上轴up对应Y轴)为旋转轴，执行旋转逻辑;
        /// </summary>
        [Name("旋转/自身本地XZ与自身世界Y")]
        [Tip("以自身为旋转中心，本地坐标系的XZ轴(右轴right对应X轴,forward对应Z轴)为旋转轴，执行旋转逻辑;以自身为旋转中心，世界坐标系的Y轴(上轴up对应Y轴)为旋转轴，执行旋转逻辑;", "Take itself as the rotation center and the XZ axis of the local coordinate system (right axis corresponds to X axis and forward corresponds to Z axis) as the rotation axis to execute the rotation logic; Take itself as the rotation center and the Y axis of the world coordinate system (the upper axis up corresponds to the Y axis) as the rotation axis to execute the rotation logic;")]
        Rotate__Self_LocalXZ__Self_WorldY,

        /// <summary>
        /// 目标本地:以目标为旋转中心，本地坐标系的三轴(右轴right对应X轴,上轴up对应Y轴,forward对应Z轴)为旋转轴，执行XYZ轴旋转逻辑;
        /// </summary>
        [Name("旋转/目标本地")]
        [Tip("以目标为旋转中心，本地坐标系的三轴(右轴right对应X轴,上轴up对应Y轴,forward对应Z轴)为旋转轴，执行XYZ轴旋转逻辑;", "Take the target as the rotation center and the three axes of the local coordinate system (right axis corresponds to X axis, up axis corresponds to y axis and forward axis corresponds to Z axis) as the rotation axis to execute the XYZ axis rotation logic;")]
        Rotate__Target_Local,

        /// <summary>
        /// 目标世界:以目标为旋转中心，世界坐标系的三轴(右轴right对应X轴,上轴up对应Y轴,forward对应Z轴)为旋转轴，执行XYZ轴旋转逻辑;
        /// </summary>
        [Name("旋转/目标世界")]
        [Tip("以目标为旋转中心，世界坐标系的三轴(右轴right对应X轴,上轴up对应Y轴,forward对应Z轴)为旋转轴，执行XYZ轴旋转逻辑;", "Take the target as the rotation center and the three axes of the world coordinate system (right axis corresponds to X axis, up axis corresponds to y axis and forward axis corresponds to Z axis) as the rotation axis to execute the XYZ axis rotation logic;")]
        Rotate__Target_World,

        /// <summary>
        /// 目标本地X与目标世界Y
        /// </summary>
        [Name("旋转/目标本地X与目标世界Y")]
        [Tip("以目标为旋转中心，本地坐标系的X轴(右轴right对应X轴)为旋转轴，执行旋转逻辑;以目标为旋转中心，世界坐标系的Y轴(上轴up对应Y轴)为旋转轴，执行旋转逻辑;忽略Z轴的旋转控制;", "Take the target as the rotation center, and the X axis of the local coordinate system (the right axis corresponds to the X axis) as the rotation axis, and execute the rotation logic; Take the target as the rotation center, and the Y axis of the world coordinate system (the upper axis up corresponds to the Y axis) as the rotation axis, and execute the rotation logic; Ignore the rotation control of Z axis;")]
        Rotate__Target_LocalX__Targert_WorldY,

        /// <summary>
        /// 目标本地XZ投影到世界X0Z面与目标世界Y
        /// </summary>
        [Name("旋转/目标本地XZ投影到世界X0Z面与目标世界Y")]
        [Tip("以目标为旋转中心，本地坐标系的XZ轴(右轴right对应X轴,forward对应Z轴)投影到世界X0Z面的轴为旋转轴，执行旋转逻辑;以目标为旋转中心，世界坐标系的Y轴(上轴up对应Y轴)为旋转轴，执行旋转逻辑;", "With the target as the rotation center, the XZ axis of the local coordinate system (right axis corresponds to X axis and forward corresponds to Z axis) projected onto the world x0z plane is the rotation axis, and the rotation logic is executed; Take the target as the rotation center and the Y axis of the world coordinate system (the upper axis up corresponds to the Y axis) as the rotation axis to execute the rotation logic;")]
        Rotate__Target_LocalXZProjectToWorldX0Z__Targert_WorldY,

        /// <summary>
        /// 目标本地XZ与目标世界Y:以目标为旋转中心，本地坐标系的XZ轴(右轴right对应X轴,forward对应Z轴)为旋转轴，执行旋转逻辑;以目标为旋转中心，世界坐标系的Y轴(上轴up对应Y轴)为旋转轴，执行旋转逻辑;
        /// </summary>
        [Name("旋转/目标本地XZ与目标世界Y")]
        [Tip("以目标为旋转中心，本地坐标系的XZ轴(右轴right对应X轴,forward对应Z轴)为旋转轴，执行旋转逻辑;以目标为旋转中心，世界坐标系的Y轴(上轴up对应Y轴)为旋转轴，执行旋转逻辑;", "Take the target as the rotation center and the XZ axis of the local coordinate system (right axis corresponds to X axis and forward corresponds to Z axis) as the rotation axis to execute the rotation logic; Take the target as the rotation center and the Y axis of the world coordinate system (the upper axis up corresponds to the Y axis) as the rotation axis to execute the rotation logic;")]
        Rotate__Target_LocalXZ__Targert_WorldY

        #endregion
    }

    #region 移动模式

    /// <summary>
    /// 移动模式：Self自身表示<see cref="CameraTransformer"/>指向的真实对象；Target目标表示<see cref="CameraTargetController"/>指向的真实目标对象；
    /// </summary>
    [Name("移动模式")]
    public enum EMoveMode
    {
        /// <summary>
        /// 无
        /// </summary>
        [Name("无")]
        None = 0,

        /// <summary>
        /// 自身本地
        /// </summary>
        [Name("自身本地")]
        [Tip("以相机控制器自身本地坐标系为基准，进行移动控制", "Take the local coordinate system of the camera controller as the benchmark to control the movement")]
        Self_Local,

        /// <summary>
        /// 自身世界
        /// </summary>
        [Name("自身世界")]
        [Tip("以相机控制器自身世界坐标系为基准，进行移动控制", "Take the camera controller's own world coordinate system as the benchmark to control the movement")]
        Self_World,

        /// <summary>
        /// 自身本地Y投影到世界X0Z面
        /// </summary>
        [Name("自身本地Y投影到世界X0Z面")]
        [Tip("本地轴XZ不变,本地Y轴投影到世界X0Z面", "The local axis XZ remains unchanged, and the local Y axis is projected to the world x0z plane")]
        Self_LocalYProjectToWorldX0Z,

        /// <summary>
        /// 自身本地Y投影到世界X0Z面
        /// </summary>
        [Name("自身本地Y投影到世界Y")]
        [Tip("本地轴X不变,本地Y轴投影到世界Y轴，本地Z不变")]
        Self_LocalXZWorldY,

        /// <summary>
        /// 自身本地XY投影到世界X0Z面
        /// </summary>
        [Name("自身本地XY投影到世界X0Z面")]
        [Tip("本地XY轴投影到世界X0Z面，本地Z不变", "The local XY axis is projected to the world x0z plane, and the local Z remains unchanged")]
        Self_LocalXYProjectToWorldX0Z,
    }

    #endregion

    #region 旋转模式

    /// <summary>
    /// 旋转模式：Self自身表示<see cref="CameraTransformer"/>指向的真实对象；Target目标表示<see cref="CameraTargetController"/>指向的真实目标对象；
    /// </summary>
    [Name("旋转模式")]
    public enum ERotateMode
    {
        /// <summary>
        /// 无:不做任何旋转
        /// </summary>
        [Name("无")]
        [Tip("不做任何旋转", "No rotation")]
        None = 0,

        /// <summary>
        /// 自身本地:以自身为旋转中心，本地坐标系的三轴(右轴right对应X轴,上轴up对应Y轴,forward对应Z轴)为旋转轴，执行XYZ轴旋转逻辑;
        /// </summary>
        [Name("自身本地")]
        [Tip("以自身为旋转中心，本地坐标系的三轴(右轴right对应X轴,上轴up对应Y轴,forward对应Z轴)为旋转轴，执行XYZ轴旋转逻辑;", "Take itself as the rotation center and the three axes of the local coordinate system (right axis corresponds to X axis, up axis corresponds to y axis and forward axis corresponds to Z axis) as the rotation axis to execute the XYZ axis rotation logic;")]
        Self_Local,

        /// <summary>
        /// 自身世界:以自身为旋转中心，世界坐标系的三轴(右轴right对应X轴,上轴up对应Y轴,forward对应Z轴)为旋转轴，执行XYZ轴旋转逻辑
        /// </summary>
        [Name("自身世界")]
        [Tip("以自身为旋转中心，世界坐标系的三轴(右轴right对应X轴,上轴up对应Y轴,forward对应Z轴)为旋转轴，执行XYZ轴旋转逻辑", "Taking itself as the rotation center, the three axes of the world coordinate system (right axis corresponds to X axis, up axis corresponds to y axis, and forward axis corresponds to Z axis) are the rotation axes, and the XYZ axis rotation logic is executed")]
        Self_World,

        /// <summary>
        /// 自身本地XZ与自身世界Y:以自身为旋转中心，本地坐标系的XZ轴(右轴right对应X轴,forward对应Z轴)为旋转轴，执行旋转逻辑;以自身为旋转中心，世界坐标系的Y轴(上轴up对应Y轴)为旋转轴，执行旋转逻辑;
        /// </summary>
        [Name("自身本地XZ与自身世界Y")]
        [Tip("以自身为旋转中心，本地坐标系的XZ轴(右轴right对应X轴,forward对应Z轴)为旋转轴，执行旋转逻辑;以自身为旋转中心，世界坐标系的Y轴(上轴up对应Y轴)为旋转轴，执行旋转逻辑;", "Take itself as the rotation center and the XZ axis of the local coordinate system (right axis corresponds to X axis and forward corresponds to Z axis) as the rotation axis to execute the rotation logic; Take itself as the rotation center and the Y axis of the world coordinate system (the upper axis up corresponds to the Y axis) as the rotation axis to execute the rotation logic;")]
        Self_LocalXZ__Self_WorldY,

        /// <summary>
        /// 目标本地:以目标为旋转中心，本地坐标系的三轴(右轴right对应X轴,上轴up对应Y轴,forward对应Z轴)为旋转轴，执行XYZ轴旋转逻辑;
        /// </summary>
        [Name("目标本地")]
        [Tip("以目标为旋转中心，本地坐标系的三轴(右轴right对应X轴,上轴up对应Y轴,forward对应Z轴)为旋转轴，执行XYZ轴旋转逻辑;", "Take the target as the rotation center and the three axes of the local coordinate system (right axis corresponds to X axis, up axis corresponds to y axis and forward axis corresponds to Z axis) as the rotation axis to execute the XYZ axis rotation logic;")]
        Target_Local,

        /// <summary>
        /// 目标世界:以目标为旋转中心，世界坐标系的三轴(右轴right对应X轴,上轴up对应Y轴,forward对应Z轴)为旋转轴，执行XYZ轴旋转逻辑;
        /// </summary>
        [Name("目标世界")]
        [Tip("以目标为旋转中心，世界坐标系的三轴(右轴right对应X轴,上轴up对应Y轴,forward对应Z轴)为旋转轴，执行XYZ轴旋转逻辑;", "Take the target as the rotation center and the three axes of the world coordinate system (right axis corresponds to X axis, up axis corresponds to y axis and forward axis corresponds to Z axis) as the rotation axis to execute the XYZ axis rotation logic;")]
        Target_World,

        /// <summary>
        /// 目标本地X与目标世界Y
        /// </summary>
        [Name("目标本地X与目标世界Y")]
        [Tip("以目标为旋转中心，本地坐标系的X轴(右轴right对应X轴)为旋转轴，执行旋转逻辑;以目标为旋转中心，世界坐标系的Y轴(上轴up对应Y轴)为旋转轴，执行旋转逻辑;忽略Z轴的旋转控制;", "Take the target as the rotation center, and the X axis of the local coordinate system (the right axis corresponds to the X axis) as the rotation axis, and execute the rotation logic; Take the target as the rotation center, and the Y axis of the world coordinate system (the upper axis up corresponds to the Y axis) as the rotation axis, and execute the rotation logic; Ignore the rotation control of Z axis;")]
        Target_LocalX__Targert_WorldY,

        /// <summary>
        /// 目标本地XZ投影到世界X0Z面与目标世界Y
        /// </summary>
        [Name("目标本地XZ投影到世界X0Z面与目标世界Y")]
        [Tip("以目标为旋转中心，本地坐标系的XZ轴(右轴right对应X轴,forward对应Z轴)投影到世界X0Z面的轴为旋转轴，执行旋转逻辑;以目标为旋转中心，世界坐标系的Y轴(上轴up对应Y轴)为旋转轴，执行旋转逻辑;", "With the target as the rotation center, the XZ axis of the local coordinate system (right axis corresponds to X axis and forward corresponds to Z axis) projected onto the world x0z plane is the rotation axis, and the rotation logic is executed; Take the target as the rotation center and the Y axis of the world coordinate system (the upper axis up corresponds to the Y axis) as the rotation axis to execute the rotation logic;")]
        Target_LocalXZProjectToWorldX0Z__Targert_WorldY,

        /// <summary>
        /// 目标本地XZ与目标世界Y:以目标为旋转中心，本地坐标系的XZ轴(右轴right对应X轴,forward对应Z轴)为旋转轴，执行旋转逻辑;以目标为旋转中心，世界坐标系的Y轴(上轴up对应Y轴)为旋转轴，执行旋转逻辑;
        /// </summary>
        [Name("目标本地XZ与目标世界Y")]
        [Tip("以目标为旋转中心，本地坐标系的XZ轴(右轴right对应X轴,forward对应Z轴)为旋转轴，执行旋转逻辑;以目标为旋转中心，世界坐标系的Y轴(上轴up对应Y轴)为旋转轴，执行旋转逻辑;", "Take the target as the rotation center and the XZ axis of the local coordinate system (right axis corresponds to X axis and forward corresponds to Z axis) as the rotation axis to execute the rotation logic; Take the target as the rotation center and the Y axis of the world coordinate system (the upper axis up corresponds to the Y axis) as the rotation axis to execute the rotation logic;")]
        Target_LocalXZ__Targert_WorldY
    }

    #endregion
}
