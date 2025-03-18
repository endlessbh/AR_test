using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;

namespace XCSJ.CommonUtils.PluginCharacters
{
    /// <summary>
    ///
    /// RootMotionController.
    /// 
    /// Helper component to get 'Animator' root-motion velocity vector (animVelocity).
    /// This must be attached to a game object with an 'Animator' component.
    /// 
    /// </summary>
    [Name("根运动控制器")]
    [RequireComponent(typeof(Animator))]
    public sealed class RootMotionController : BaseCharacterMB
    {
        #region 字段

        public Animator animator
        {
            get
            {
                
                return _animator;
            }
        }
        private Animator _animator;

        #endregion

        #region 属性

        /// <summary>
        /// The animation velocity vector.
        /// </summary>

        public Vector3 animVelocity { get; private set; }

        #endregion

        #region MB方法

        public void Awake()
        {
            if (!_animator)
            {
                _animator = GetComponent<Animator>();

                if (_animator == null)
                {
                    Debug.LogError(
                        string.Format(
                            "RootMotionController: There is no 'Animator' attached to the '{0}' game object.\n" +
                            "Please attach a 'Animator' to the '{0}' game object",
                            name));
                }
            }
        }

        public void OnAnimatorMove()
        {
            // Compute movement velocity from animation

            var deltaTime = Time.deltaTime;
            if (deltaTime > 0.0f && animator)
                animVelocity = animator.deltaPosition / deltaTime;
        }

        #endregion
    }
}