using System.Collections.Generic;
using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Inputs;
using XCSJ.Interfaces;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.CNScripts;

namespace XCSJ.CommonUtils.PluginCharacters
{
    /// <summary>
    /// MouseLook.
    /// 
    /// Component used to 'look around' with the mouse.
    /// This rotate the character along its y-axis (yaw) and a child camera along its local x-axis (pitch).
    /// 
    /// This must be attached to the game object with 'CharacterMovement' component.
    /// </summary>
    [Name("鼠标查看")]
    [Tip("将相机方向与鼠标进行关联锁定；专用于第一人称控制器；", "Associate and lock the camera direction with the mouse; Dedicated to the first person controller;")]
    //[Obsolete("不再推荐使用,请使用其他方法替代")]
    public class ObsoleteMouseLook : BaseCharacterMB, IInputEventSender, IOnEnable,IOnDisable
    {
        #region 编辑字段

        [Name("保持按下")]
        [Tip("保持鼠标按键按下时才执行视角变换", "Keep the mouse button pressed before performing the angle transformation")]
        public bool keepDown = true;

        [Name("锁定光标")]
        [Tip("是否应锁定鼠标光标（例如：隐藏）", "Whether the mouse cursor should be locked (for example: hidden)")]
        [SerializeField]
        private bool _lockCursor = true;

        [Name("鼠标按钮类型")]
        [Tip("标识哪个鼠标按钮按下时触发鼠标锁定", "Identifies which mouse button triggers mouse lock when pressed")]
        [EnumPopup]
        public EMouseButtonType mouseButtonType = EMouseButtonType.Left;

        [Name("解锁光标键码")]
        [Tip("用于解锁鼠标光标的键盘键", "Keyboard keys for unlocking the mouse cursor")]
        [SerializeField]
        private KeyCode _unlockCursorKey = KeyCode.Escape;

        [Name("横向灵敏度")]
        [Tip("光标响应鼠标横向（X轴）移动的速度；", "The speed at which the cursor responds to the lateral (x-axis) movement of the mouse;")]
        [SerializeField]
        [Range(0, 10)]
        private float _lateralSensitivity = 2.0f;

        [Name("垂直灵敏度")]
        [Tip("光标响应鼠标垂直（Y轴）移动的速度", "The speed at which the cursor responds to the vertical (Y-axis) movement of the mouse")]
        [SerializeField]
        [Range(0, 10)]
        private float _verticalSensitivity = 2.0f;

        [Name("平滑")]
        [Tip("旋转是否应该平滑（例如：插值）", "Should the rotation be smooth (for example, interpolation)")]
        [SerializeField]
        private bool _smooth;

        [Name("平滑时间")]
        [Tip("接近到达目标所需的时间（秒）；较小的值将更快地到达目标", "Time required to approach the target (seconds); Smaller values will reach the target faster")]
        [SerializeField]
        [Range(0, 30)]
        public float _smoothTime = 5.0f;

        [Name("俯仰约束")]
        [Tip("标识围绕x轴的旋转是否应该被约束", "Identifies whether rotation around the X axis should be constrained")]
        [SerializeField]
        private bool _clampPitch = true;

        [Name("最小俯仰角")]
        [Tip("最小俯仰角；单位：度；", "Minimum pitch angle; Unit: degree;")]
        [SerializeField]
        [Range(-180, 180)]
        private float _minPitchAngle = -90.0f;

        [Name("最大俯仰角")]
        [Tip("最大俯仰角；单位：度；", "Maximum pitch angle; Unit: degree;")]
        [Range(-180, 180)]
        [SerializeField]
        private float _maxPitchAngle = 90.0f;

        [Group("输入控制")]
        [Name("启用输入")]
        [Tip("标识对角色输入控制是否启用", "Identifies whether the role input control is enabled")]
        public bool _enableInput = true;

        [Name("偏航输入")]
        [Tip("偏航主要作用在角色游戏对象的Y轴旋转；", "Yaw mainly acts on the y-axis rotation of the character game object;")]
        [Input]
        public string yawInput = "Mouse X";

        [Name("俯仰输入")]
        [Tip("俯仰主要作用在角色的相机游戏对象的X轴旋转；", "Pitch mainly acts on the x-axis rotation of the character's camera and game object;")]
        [Input]
        public string pitchInput = "Mouse Y";

        [Name("组件禁用后使用虚拟输入")]
        [Tip("当鼠标查看组件被禁用后，标识是否仍使用虚拟输入继续处理视角变化", "When the mouse viewing component is disabled, it indicates whether the virtual input is still used to continue to process the change of viewing angle")]
        public bool useVritualInputOnDisable = true;

        [Name("鼠标按键未按下时使用虚拟输入")]
        [Tip("当鼠标按键未保持按下状态时，标识是否仍使用虚拟输入继续处理视角变化", "When the mouse button is not kept pressed, identify whether the virtual input is still used to continue to process the change of viewing angle")]
        public bool useVritualInputOnNoDown = true;

        #endregion

        #region FIELDS

        protected Quaternion characterTargetRotation;
        protected Quaternion cameraTargetRotation;

        #endregion

        #region PROPERTIES

        /// <summary>
        /// Should the mouse cursor be locked (eg: hidden)?
        /// </summary>
        public bool lockCursor
        {
            get { return _lockCursor; }
            set { _lockCursor = value; }
        }

        /// <summary>
        /// The keyboard key to unlock the mouse cursor.
        /// </summary>
        public KeyCode unlockCursorKey
        {
            get { return _unlockCursorKey; }
            set { _unlockCursorKey = value; }
        }

        /// <summary>
        /// How fast the cursor moves in response to mouse lateral (x-axis) movement.
        /// </summary>
        public float lateralSensitivity
        {
            get { return _lateralSensitivity; }
            set { _lateralSensitivity = Mathf.Max(0.0f, value); }
        }

        /// <summary>
        /// How fast the cursor moves in response to mouse vertical (y-axis) movement.
        /// </summary>
        public float verticalSensitivity
        {
            get { return _verticalSensitivity; }
            set { _verticalSensitivity = Mathf.Max(0.0f, value); }
        }

        /// <summary>
        /// Should the rotation be smoothed (eg: interpolated)?
        /// </summary>
        public bool smooth
        {
            get { return _smooth; }
            set { _smooth = value; }
        }

        /// <summary>
        /// Approximately the time (in seconds) it will take to reach the target.
        /// A smaller value will reach the target faster.
        /// </summary>
        public float smoothTime
        {
            get { return _smoothTime; }
            set { _smoothTime = Mathf.Max(0.0f, value); }
        }

        /// <summary>
        /// Should the rotation around x-axis be clamped?
        /// </summary>
        public bool clampPitch
        {
            get { return _clampPitch; }
            set { _clampPitch = value; }
        }

        /// <summary>
        /// The minimum pitch angle (in degrees).
        /// </summary>
        public float minPitchAngle
        {
            get { return _minPitchAngle; }
            set { _minPitchAngle = Mathf.Clamp(value, -180.0f, 180.0f); }
        }

        /// <summary>
        /// The maximum pitch angle (in degrees).
        /// </summary>
        public float maxPitchAngle
        {
            get { return _maxPitchAngle; }
            set { _maxPitchAngle = Mathf.Clamp(value, -180.0f, 180.0f); }
        }

        #endregion

        #region METHODS

        public virtual void Init(Transform characterTransform, Transform cameraTransform)
        {
            characterTargetRotation = characterTransform.localRotation;
            cameraTargetRotation = cameraTransform.localRotation;
        }

        public virtual bool HasInput(out float yaw, out float pitch)
        {
            if (!enableInput)//输入不启用，直接返回
            {
                yaw = 0;
                pitch = 0;
                return false;
            }

            if (enabled)//当前组件可用
            {
                if (keepDown)//要求保持鼠标按下
                {
                    if (!CommonFun.IsOnAnyUI() && XInput.GetMouseButton((int)mouseButtonType))//鼠标按下保持中
                    {
                        SetCursorLock(lockCursor);
                        yaw = XInput.GetAxis(yawInput);
                        pitch = XInput.GetAxis(pitchInput);
                        return true;
                    }
                    else
                    {
                        SetCursorLock(false);
                        if (useVritualInputOnNoDown)//未保持按下时仍处理虚拟输入
                        {
                            yaw = VirtualInput.instance.GetAxis(yawInput);
                            pitch = VirtualInput.instance.GetAxis(pitchInput);
                            return true;
                        }
                    }
                }
                else
                {
                    if (Input.GetKeyUp(unlockCursorKey))//解锁
                    {
                        SetCursorLock(false);
                    }
                    else if (Input.GetMouseButtonUp((int)mouseButtonType))
                    {
                        SetCursorLock(lockCursor);
                    }
                    yaw = XInput.GetAxis(yawInput);
                    pitch = XInput.GetAxis(pitchInput);
                    return true;
                }
            }
            else if (useVritualInputOnDisable)//组件禁用后仍处理虚拟输入
            {
                yaw = VirtualInput.instance.GetAxis(yawInput);
                pitch = VirtualInput.instance.GetAxis(pitchInput);
                return true;
            }
            yaw = 0;
            pitch = 0;
            return false;
        }

        /// <summary>
        /// 执行“Look”旋转
        /// 这将沿角色的Y轴（偏航）旋转角色，并沿角色的局部X轴（俯仰）旋转子摄影机。
        /// </summary>
        /// <param name="movement">角色移动组件</param>
        /// <param name="cameraTransform">相机变换</param>
        public virtual void LookRotation(CharacterMovement movement, Transform cameraTransform)
        {           
            if (!HasInput(out float yaw, out float pitch)) return;
            
            yaw *= lateralSensitivity;
            pitch *= verticalSensitivity;

            var yawRotation = Quaternion.Euler(0.0f, yaw, 0.0f);
            var pitchRotation = Quaternion.Euler(-pitch, 0.0f, 0.0f);

            characterTargetRotation *= yawRotation;
            cameraTargetRotation *= pitchRotation;

            if (smooth)
            {
                // On a rotating platform, append platform rotation to target rotation

                if (movement.isOnPlatform && movement.platformAngularVelocity != Vector3.zero)
                {
                    characterTargetRotation *=
                        Quaternion.Euler(movement.platformAngularVelocity * Mathf.Rad2Deg * Time.deltaTime);
                }

                movement.rotation = Quaternion.Slerp(movement.rotation, characterTargetRotation,
                    smoothTime * Time.deltaTime);

                cameraTransform.localRotation = Quaternion.Slerp(cameraTransform.localRotation, cameraTargetRotation,
                    smoothTime * Time.deltaTime);
            }
            else
            {
                movement.rotation *= yawRotation;
                cameraTransform.localRotation *= pitchRotation;
            }

            if (clampPitch)
                cameraTransform.localRotation = ClampPitch(cameraTransform.localRotation);
        }

        public void SetCursorLock(bool value)
        {
            if(value)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        protected Quaternion ClampPitch(Quaternion q)
        {
            q.x /= q.w;
            q.y /= q.w;
            q.z /= q.w;
            q.w = 1.0f;

            var pitch = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

            pitch = Mathf.Clamp(pitch, minPitchAngle, maxPitchAngle);

            q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * pitch);

            return q;
        }

        #endregion

        #region MONOBEHAVIOUR

        /// <summary>
        /// Validate editor exposed fields.
        /// 
        /// NOTE: If you override this, it is important to call the parent class' version of method
        /// eg: base.OnValidate, in the derived class method implementation, in order to fully validate the class.  
        /// </summary>

        public virtual void OnValidate()
        {
            lateralSensitivity = _lateralSensitivity;
            verticalSensitivity = _verticalSensitivity;

            smoothTime = _smoothTime;

            minPitchAngle = _minPitchAngle;
            maxPitchAngle = _maxPitchAngle;
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            SetCursorLock(false);
        }

        #endregion

        #region IInputEventSender

        public bool enableSend { get; set; } = true;

        public bool enableInput { get => _enableInput; set => _enableInput = value; }

        public List<IEventHandler> handlers { get; } = new List<IEventHandler>();

        #endregion
    }
}
