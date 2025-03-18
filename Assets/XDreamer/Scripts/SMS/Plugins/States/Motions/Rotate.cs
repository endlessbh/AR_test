using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Extension.Base.Recorders;
using XCSJ.Maths;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;

namespace XCSJ.PluginSMS.States.Motions
{
    /// <summary>
    /// 旋转:旋转组件是游戏对象的旋转动画。在给定的时间内，游戏对象做旋转运动，旋转完成后，组件切换为完成态。
    /// </summary>
    [ComponentMenu("动作/"+ Title, typeof(SMSManager))]
    [Name(Title, nameof(Rotate))]
    [Tip("旋转组件是游戏对象的旋转动画。在给定的时间内，游戏对象做旋转运动，旋转完成后，组件切换为完成态。", "The rotation component is the rotation animation of the game object. Within a given time, the game object makes a rotation movement. After the rotation is completed, the component switches to the completed state.")]
    [Attributes.Icon]
    [DisallowMultipleComponent]
    public class Rotate : TransformMotion<Rotate>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "旋转";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib("动作", typeof(SMSManager))]
        [StateComponentMenu("动作/"+ Title, typeof(SMSManager))]
        [Name(Title, nameof(Rotate))]
        [Tip("旋转组件是游戏对象的旋转动画。在给定的时间内，游戏对象做旋转运动，旋转完成后，组件切换为完成态。", "The rotation component is the rotation animation of the game object. Within a given time, the game object makes a rotation movement. After the rotation is completed, the component switches to the completed state.")]
        [XCSJ.Attributes.Icon(EIcon.Rotate)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        [Name("旋转规则")]
        public enum ERotateRule
        {
            [Name("无动作")]
            None = 0,

            [Name("本地")]
            [Tip("以游戏对象的自身坐标系的三轴为基准，线性旋转指定角度；此时 值 表示期望的三轴旋转角度；", "Linearly rotate the specified angle based on the three axes of the game object's own coordinate system; At this time, the value represents the desired triaxial rotation angle;")]
            Local,

            [Name("世界")]
            [Tip("以游戏对象的世界坐标系的三轴为基准，线性旋转指定角度；此时 值 表示期望的三轴旋转角度；", "Linearly rotate the specified angle based on the three axes of the game object's world coordinate system; At this time, the value represents the desired triaxial rotation angle;")]
            World,

            [Name("注视")]
            [Tip("将游戏对象在结束时指向特定的方向；即将游戏对象自身坐标系Z轴与注视方向重合；此时 值 表示期望注视的目标点坐标(世界坐标系)；", "Point the game object in a specific direction at the end; That is, the Z axis of the game object's own coordinate system coincides with the gaze direction; At this time, the value represents the target point coordinate of the desired gaze (world coordinate system);")]
            LookAt,

            [Name("本地轴")]
            [Tip("游戏对象以指定的方向轴为基准，旋转中心为游戏对象的变换中心，线性旋转特定角度；此时 值 表示方向轴(本地坐标系)；", "The game object takes the specified direction axis as the benchmark, the rotation center is the transformation center of the game object, and rotates a specific angle linearly; At this time, the value represents the direction axis (local coordinate system);")]
            LocalAxis,

            [Name("世界轴")]
            [Tip("游戏对象以指定的方向轴为基准，旋转中心为游戏对象的变换中心，线性旋转特定角度；此时 值 表示方向轴(世界坐标系)；", "The game object takes the specified direction axis as the benchmark, the rotation center is the transformation center of the game object, and rotates a specific angle linearly; At this time, the value represents the direction axis (world coordinate system);")]
            WorldAxis,

            [Name("世界点轴")]
            [Tip("游戏对象以指定的方向轴为基准，旋转中心为轴点，线性旋转特定角度；此时 值 表示方向轴(世界坐标系)；", "The game object rotates linearly at a specific angle with the specified direction axis as the benchmark and the rotation center as the axis point; At this time, the value represents the direction axis (world coordinate system);")]
            WorldPointAxis,

            [Name("世界点轴后本地")]
            [Tip("游戏对象以指定的方向轴为基准，旋转中心为轴点，线性旋转特定角度；旋转完成后游戏对象的旋转设置为原始的本地旋转；此时 值 表示方向轴(世界坐标系)；", "The game object rotates linearly at a specific angle with the specified direction axis as the benchmark and the rotation center as the axis point; After the rotation is completed, the rotation of the game object is set to the original local rotation; At this time, the value represents the direction axis (world coordinate system);")]
            WorldPointAxisThenLocal,
        }

        [Name("旋转规则")]
        [EnumPopup]
        public ERotateRule rotateRule = ERotateRule.Local;

        [Name("值")]
        [Tip("根据 旋转规则 不同本项信息具有不同解释；具体查阅 旋转规则 参数的说明；", "This information has different interpretations according to different rotation rules; Refer to the description of rotation rule parameters for details;")]
        public Vector3 value = new Vector3();

        [Name("向上")]
        [Tip("注视时的上方向(世界坐标系)；", "The upward direction of gaze (world coordinate system);")]
        [HideInSuperInspector(nameof(rotateRule), EValidityCheckType.NotEqual, ERotateRule.LookAt)]
        public Vector3 upwards = Vector3.up;

        [Name("轴角度")]
        [Tip("绕旋转轴发生旋转的角度；左手法则；", "The angle of rotation about the axis of rotation; Left hand rule;")]
        [HideInSuperInspector(nameof(rotateRule), EValidityCheckType.Less | EValidityCheckType.Or, ERotateRule.LocalAxis, nameof(rotateRule), EValidityCheckType.Greater, ERotateRule.WorldPointAxisThenLocal)]
        public float axisAngle = 0;

        [Name("轴点")]
        [Tip("点轴旋转方式的轴点坐标(世界坐标系)；", "Axis point coordinates of point axis rotation mode (world coordinate system);")]
        [HideInSuperInspector(nameof(rotateRule), EValidityCheckType.Less | EValidityCheckType.Or, ERotateRule.WorldPointAxis, nameof(rotateRule), EValidityCheckType.Greater, ERotateRule.WorldPointAxisThenLocal)]
        public Vector3 axisPoint = new Vector3();

        [Name("朝向轴点")]
        [HideInSuperInspector(nameof(rotateRule), EValidityCheckType.NotEqual, ERotateRule.WorldPointAxis)]
        public bool lookatAxisPoint = false;

        private double lastAxisAngle = 0;

        public override void OnEntry(StateData data)
        {
            base.OnEntry(data);

            lastAxisAngle = 0;
        }

        protected override void SetPercent(TransformRecorder.Info info, Percent percent)
        {
            switch (rotateRule)
            {
                case ERotateRule.Local:
                    {
                        info.transform.localRotation = info.localRotation;
                        info.transform.Rotate(Vector3.Lerp(Vector3.zero, value, (float)percent.percent01OfWorkCurve), Space.Self);
                        break;
                    }
                case ERotateRule.World:
                    {
                        info.transform.rotation = info.worldRotation;
                        info.transform.Rotate(Vector3.Lerp(Vector3.zero, value, (float)percent.percent01OfWorkCurve), Space.World);
                        break;
                    }
                case ERotateRule.LookAt:
                    {
                        info.transform.localRotation = Quaternion.Lerp(info.localRotation, Quaternion.LookRotation(value, upwards), (float)percent.percent01OfWorkCurve);
                        break;
                    }
                case ERotateRule.LocalAxis:
                    {
                        info.transform.localRotation = info.localRotation;
                        info.transform.Rotate(value, (float)MathX.Lerp(0, axisAngle, percent.percent01OfWorkCurve), Space.Self);
                        break;
                    }
                case ERotateRule.WorldAxis:
                    {
                        info.transform.rotation = info.worldRotation;
                        info.transform.Rotate(value, (float)MathX.Lerp(0, axisAngle, percent.percent01OfWorkCurve), Space.World);
                        break;
                    }
                case ERotateRule.WorldPointAxis:
                    {
                        var currentAngle = MathX.Lerp(0, axisAngle, percent.percent01OfWorkCurve);
                        info.transform.RotateAround(axisPoint, value, (float)(currentAngle - lastAxisAngle));
                        lastAxisAngle = currentAngle;
                        if (lookatAxisPoint)
                        {
                            info.transform.LookAt(axisPoint);
                        }
                        break;
                    }
                case ERotateRule.WorldPointAxisThenLocal:
                    {
                        info.transform.rotation = info.worldRotation;
                        info.transform.RotateAround(axisPoint, value, axisAngle);
                        info.transform.localRotation = info.localRotation;
                        break;
                    }
            }
        }
    }
}
