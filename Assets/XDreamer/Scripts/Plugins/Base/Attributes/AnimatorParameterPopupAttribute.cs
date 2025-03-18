using System;
using UnityEngine;

namespace XCSJ.Extension.Base.Attributes
{
    /// <summary>
    /// 动画器参数弹出式菜单特性,需要与<see cref="IAnimatorParameter"/>接口组合使用；
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class AnimatorParameterPopupAttribute : PropertyAttribute
    {
        /// <summary>
        /// 弹出菜单宽度
        /// </summary>
        public float width = 80;

        /// <summary>
        /// 动画器控制器参数类型
        /// </summary>
        public AnimatorControllerParameterType? parameterType { get; private set; } = null;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="parameterType"></param>
        public AnimatorParameterPopupAttribute(Type parameterType = null)
        {
            if (parameterType == null) return;

            var fullName = parameterType.FullName;
            if (fullName == typeof(float).FullName)
            {
                this.parameterType = AnimatorControllerParameterType.Float;
            }
            else if (fullName == typeof(int).FullName)
            {
                this.parameterType = AnimatorControllerParameterType.Int;
            }
            else if (fullName == typeof(bool).FullName)
            {
                this.parameterType = AnimatorControllerParameterType.Bool;
            }
        }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="parameterType"></param>
        public AnimatorParameterPopupAttribute(AnimatorControllerParameterType parameterType)
        {
            this.parameterType = parameterType;
        }
    }

    /// <summary>
    /// 动画器参数接口
    /// </summary>
    public interface IAnimatorParameter
    {
        /// <summary>
        /// 尝试获取动画器
        /// </summary>
        /// <param name="propertyPath"></param>
        /// <param name="animator"></param>
        /// <returns></returns>
        bool TryGetAnimator(string propertyPath, out Animator animator);
    }
}
