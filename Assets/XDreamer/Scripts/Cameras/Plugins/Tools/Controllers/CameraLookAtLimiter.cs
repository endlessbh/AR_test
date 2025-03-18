using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginsCameras.Controllers;
using XCSJ.PluginsCameras.Tools.Base;
using XCSJ.Tools;
using XCSJ.PluginTools;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginsCameras.Tools.Controllers
{
    /// <summary>
    /// 相机查看限定器：默认将相机的观察点每帧定位为目标控制器中的目标位置
    /// </summary>
    [Name("相机查看限定器")]
    [Tip("默认将相机的观察点每帧定位为目标控制器中的目标位置", "By default, the camera's observation point is positioned as the target position in the target controller every frame")]
    [Tool(CameraHelperExtension.ControllersCategoryName, /*nameof(CameraController), */nameof(CameraTargetController))]
    [XCSJ.Attributes.Icon(EIcon.Target)]
    [DisallowMultipleComponent]
    public class CameraLookAtLimiter : BaseCameraLimiter
    {
        /// <summary>
        /// 查看位置
        /// </summary>
        [Name("查看位置")]
        public Position _lookAtPosition = new Position();

        /// <summary>
        /// 上方向
        /// </summary>
        [Name("上方向")]
        public enum EUpDirection
        {
            /// <summary>
            /// 自定义
            /// </summary>
            [Name("自定义")]
            Custom = -1,

            /// <summary>
            /// 相机上
            /// </summary>
            [Name("相机上")]
            [Tip("使用相机当前的上向量作为设置相机朝向时使用的上方向向量值", "Use the current up vector of the camera as the up vector value used when setting the camera orientation")]
            CameraUp,
        }

        /// <summary>
        /// 上方向:设置相机朝向时使用的上方向类型
        /// </summary>
        [Name("上方向")]
        [Tip("设置相机朝向时使用的上方向类型", "Sets the type of up direction to use when facing the camera")]
        [EnumPopup]
        public EUpDirection _upDirection = EUpDirection.CameraUp;

        /// <summary>
        /// 上方向向量:设置相机朝向时使用的上方向向量值
        /// </summary>
        [Name("上方向向量")]
        [Tip("设置相机朝向时使用的上方向向量值", "Sets the up vector value used when facing the camera")]
        [HideInSuperInspector(nameof(_upDirection), EValidityCheckType.NotEqual, EUpDirection.Custom)]
        [ValidityCheck(EValidityCheckType.NotDefault)]
        public Vector3 _upDirectionVector = Vector3.up;

        /// <summary>
        /// 上方向向量
        /// </summary>
        public Vector3 upDirectionVector
        {
            get
            {
                switch (_upDirection)
                {
                    case EUpDirection.CameraUp: return cameraTransformer.up;
                    default: return _upDirectionVector;
                }
            }
        }

        /// <summary>
        /// 延后更新
        /// </summary>
        public void LateUpdate()
        {
            if (_lookAtPosition.TryGetPosition(cameraController, out Vector3 position))
            {
                cameraTransformer.LookAt(position, upDirectionVector);
            }
        }

        /// <summary>
        /// 在编辑态执行限定
        /// </summary>
        protected override void ExcuteLimitInEdit()
        {
            //base.ExcuteLimitInEdit();
            if (_lookAtPosition.TryGetPosition(cameraController, out Vector3 position))
            {
                cameraTransformer.LookAt(position);
            }
        }
    }
}
