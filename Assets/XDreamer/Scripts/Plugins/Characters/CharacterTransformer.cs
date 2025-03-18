using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.CommonUtils.PluginCharacters;
using XCSJ.Extension.Characters.Base;

namespace XCSJ.Extension.Characters
{
    /// <summary>
    /// 角色变换器
    /// </summary>
    [Name("角色变换器")]
    public class CharacterTransformer : BaseCharacterTransformer
    {
        /// <summary>
        /// 更新
        /// </summary>
        protected virtual void Update()
        {
            mainController.jump = jump;

            foreach (var kv in directions)
            {
                switch (kv.Key)
                {
                    case ERotateMode.MoveDirection:
                        {
                            mainController.RotateTowardsMoveDirection();
                            break;
                        }
                    case ERotateMode.Velocity:
                        {
                            mainController.RotateTowardsVelocity();
                            break;
                        }
                    case ERotateMode.VectorDirection:
                        {
                            mainController.RotateTowards(kv.Value);
                            break;
                        }
                    case ERotateMode.Self_WorldY:
                        {
                            var rotation = characterTransform.rotation;
                            var targetRotation = rotation * Quaternion.Euler(kv.Value);
                            var dir = targetRotation * Vector3.forward;
                            mainController.RotateTowardsDirect(dir);
                            break;
                        }
                }
            }
            directions.Clear();
        }

        #region 移动

        private Vector3 _moveDirection = new Vector3();

        private bool jump = false;

        /// <summary>
        /// 移动
        /// </summary>
        /// <param name="value"></param>
        /// <param name="moveMode"></param>
        public override void Move(Vector3 value, int moveMode) => Move(value, (EMoveMode)moveMode);

        /// <summary>
        /// 移动
        /// </summary>
        /// <param name="value"></param>
        /// <param name="moveMode"></param>
        public void Move(Vector3 value, EMoveMode moveMode)
        {
            if (value.y > JumpThresholdValue)
            {
                value.y = 0;
                jump = true;
            }
            else
            {
                jump = false;
            }
            switch (moveMode)
            {
                case EMoveMode.Local:
                    {
                        _moveDirection += characterTransform.TransformDirection(value);
                        break;
                    }
                case EMoveMode.World:
                    {
                        _moveDirection += value;
                        break;
                    }
            }
        }

        /// <summary>
        /// 计算所需的移动速度。
        /// 例如：将输入（移动方向）转换为运动速度矢量， 如使用导航网格代理所需的速度等。   
        /// 如组件未启用，返回默认值；
        /// </summary>
        public override Vector3 CalcDesiredVelocity()
        {
            if (!enabled) return Vector3.zero;
            try
            {
                var mainController = this.mainController;
                mainController.moveDirection = _moveDirection;

                // 如果正在应用根运动和根运动（例如：接地）
                // 使用动画速度作为动画完全控制
                if (mainController.useRootMotion && mainController.applyRootMotion)
                {
                    return mainController.rootMotionController.animVelocity;
                }

                // 将输入（移动方向）转换为速度矢量
                return _moveDirection * mainController.speed;
            }
            finally
            {
                _moveDirection = Vector3.zero;
            }
        }

        /// <summary>
        /// 跳跃阈值
        /// </summary>
        public const float JumpThresholdValue = 1000;

        /// <summary>
        /// 跳跃值
        /// </summary>
        public const float JumpValue = JumpThresholdValue + 1;

        #endregion

        #region 旋转

        /// <summary>
        /// 旋转
        /// </summary>
        /// <param name="value"></param>
        /// <param name="rotateMode"></param>
        public override void Rotate(Vector3 value, int rotateMode) => Rotate(value, (ERotateMode)rotateMode);

        /// <summary>
        /// 旋转
        /// </summary>
        /// <param name="value"></param>
        /// <param name="rotateMode"></param>
        public void Rotate(Vector3 value, ERotateMode rotateMode)
        {
            directions[rotateMode] = value;
        }

        private Dictionary<ERotateMode, Vector3> directions = new Dictionary<ERotateMode, Vector3>();

        #endregion
    }

    /// <summary>
    /// 移动模式
    /// </summary>
    [Name("移动模式")]
    public enum EMoveMode
    {
        /// <summary>
        /// 无
        /// </summary>
        [Name("无")]
        None,

        /// <summary>
        /// 本地
        /// </summary>
        [Name("本地")]
        Local,

        /// <summary>
        /// 世界
        /// </summary>
        [Name("世界")]
        World,
    }



    /// <summary>
    /// 旋转模式
    /// </summary>
    [Name("旋转模式")]
    public enum ERotateMode
    {
        /// <summary>
        /// 无
        /// </summary>
        [Name("无")]
        None,

        /// <summary>
        /// 向移动方向向量（输入）旋转角色.即与<see cref="XCharacterController.moveDirection"/>方向一致；
        /// </summary>
        [Name("移动方向")]
        MoveDirection,

        /// <summary>
        /// 将角色朝其速度向量旋转.即与<see cref="CharacterMovement.velocity"/>方向一致；
        /// </summary>
        [Name("速度方向")]
        Velocity,

        /// <summary>
        /// 将角色朝参数指定向量方向旋转
        /// </summary>
        [Name("向量方向")]
        VectorDirection,

        /// <summary>
        /// 自身世界Y:以自身为旋转中心，世界坐标系的Y轴(上轴up对应Y轴)为旋转轴，执行旋转逻辑;
        /// </summary>
        [Name("自身世界Y")]
        [Tip("以自身为旋转中心，世界坐标系的Y轴(上轴up对应Y轴)为旋转轴，执行旋转逻辑;", "Take itself as the rotation center and the Y axis of the world coordinate system (the upper axis up corresponds to the Y axis) as the rotation axis to execute the rotation logic;")]
        Self_WorldY,
    }
}
