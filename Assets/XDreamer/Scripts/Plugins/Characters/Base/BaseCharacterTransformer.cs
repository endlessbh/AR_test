using UnityEngine;
using XCSJ.Attributes;

namespace XCSJ.Extension.Characters.Base
{
    /// <summary>
    /// 基础角色变换器
    /// </summary>
    [Name("基础角色变换器")]
    public abstract class BaseCharacterTransformer : BaseCharacterCoreController
    {
        /// <summary>
        /// 移动
        /// </summary>
        /// <param name="value"></param>
        /// <param name="moveMode"></param>
        public abstract void Move(Vector3 value, int moveMode);

        /// <summary>
        /// 计算期望的矢量速度
        /// </summary>
        /// <returns></returns>
        public abstract Vector3 CalcDesiredVelocity();

        /// <summary>
        /// 旋转
        /// </summary>
        /// <param name="value"></param>
        /// <param name="rotateMode"></param>
        public abstract void Rotate(Vector3 value, int rotateMode);
    }
}
