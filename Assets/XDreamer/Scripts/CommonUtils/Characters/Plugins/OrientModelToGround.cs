using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;

namespace XCSJ.CommonUtils.PluginCharacters
{
    /// <summary>
    /// 
    /// OrientModelToGround.
    /// 
    /// Helper component used to orient a model to ground.
    /// This must be attached to the game object with 'CharacterMovement' component.
    /// 
    /// </summary>
    [Name("定位模型到地面")]
    [RequireComponent(typeof(CharacterMovement))]
    public sealed class OrientModelToGround : BaseCharacterMB
    {
        #region EDITOR EXPOSED FIELDS

        [Name("定位到地面")]
        [Tip("标识模型是否应该遵循地面坡度", "Identifies whether the model should follow the ground slope")]
        [SerializeField]
        private bool _orientToGround = true;

        [Name("模型变换")]
        [Tip("待定位的模型变换", "Model transformation to be located")]
        public Transform modelTransform;

        [Name("最小坡度角")]
        [Tip("导致方向改变的最小坡度角；单位：度；", "The minimum slope angle causing the change of direction; Unit: degree;")]
        [SerializeField]
        private float _minAngle = 5.0f;

        [Name("最大转弯速度")]
        [Tip("最大转弯速度；单位：度/秒；", "Maximum turning speed; Unit: degree / second;")]
        [SerializeField]
        private float _rotationSpeed = 240.0f;

        #endregion

        #region FIELDS

        private CharacterMovement _movement;

        private Quaternion _groundRotation = Quaternion.identity;

        #endregion

        #region PROPERTIES

        /// <summary>
        /// Determines if the character will change its up vector to match the ground normal.
        /// </summary>

        public bool orientToGround
        {
            get { return _orientToGround; }
            set
            {
                _orientToGround = value;

                // Reset model transform

                if (modelTransform)
                    modelTransform.rotation = transform.rotation;
            }
        }

        /// <summary>
        /// Minimum slope angle (in degrees) to cause an orientation change. 
        /// </summary>

        public float minAngle
        {
            get { return _minAngle; }
            set { _minAngle = Mathf.Clamp(value, 0.0f, 360.0f); }
        }

        /// <summary>
        /// Maximum turning speed (in deg/s).
        /// </summary>

        public float rotationSpeed
        {
            get { return _rotationSpeed; }
            set { _rotationSpeed = Mathf.Max(0.0f, value); }
        }

        #endregion

        #region MONOBEHAVIOUR

        public void OnValidate()
        {
            orientToGround = _orientToGround;

            minAngle = _minAngle;
            rotationSpeed = _rotationSpeed;
        }

        public void Awake()
        {
            _movement = GetComponent<CharacterMovement>();

            _groundRotation = modelTransform.rotation;
        }

        public void Update()
        {
            if (!orientToGround)
                return;

            // Compute target ground normal rotation

            var targetGroundRotation = _movement.groundAngle < minAngle
                ? Quaternion.identity
                : Quaternion.FromToRotation(Vector3.up, _movement.surfaceNormal);

            // Interpolate ground orientation

            var maxRadiansDelta = rotationSpeed * Mathf.Deg2Rad * Time.deltaTime;
            _groundRotation = Quaternion.Slerp(_groundRotation, targetGroundRotation, maxRadiansDelta);

            // Concatenate ground and parent facing rotation

            modelTransform.rotation = _groundRotation * transform.rotation;
        }

        #endregion
    }
}