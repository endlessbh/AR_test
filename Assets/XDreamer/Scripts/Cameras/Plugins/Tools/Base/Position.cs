using System;
using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginsCameras.Base;

namespace XCSJ.PluginsCameras.Tools.Base
{
    /// <summary>
    /// 位置
    /// </summary>
    [Name("位置")]
    [Serializable]
    public class Position
    {
        /// <summary>
        /// 位置类型
        /// </summary>
        [Name("位置类型")]
        [EnumPopup]
        public EPositionType _positionType = EPositionType.Target;

        /// <summary>
        /// 位置
        /// </summary>
        [Name("位置")]
        [HideInSuperInspector(nameof(_positionType), EValidityCheckType.NotEqual, EPositionType.Postion)]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Vector3 _position = new Vector3();

        /// <summary>
        /// 位置变换
        /// </summary>
        [Name("位置变换")]
        [HideInSuperInspector(nameof(_positionType), EValidityCheckType.NotEqual, EPositionType.Transform)]
        public Transform _positionTransform;

        /// <summary>
        /// 尝试获取位置
        /// </summary>
        /// <param name="cameraController"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public bool TryGetPosition(BaseCameraMainController cameraController, out Vector3 position)
        {
            switch (_positionType)
            {
                case EPositionType.Postion:
                    {
                        position = _position;
                        return true;
                    }
                case EPositionType.Transform:
                    {
                        if (_positionTransform)
                        {
                            position = _positionTransform.position;
                            return true;
                        }
                        break;
                    }
                case EPositionType.Original: return TryGetOriginalPosition(cameraController, out position);
                case EPositionType.Target: return TryGetTargetPosition(cameraController, out position);
            }
            position = default;
            return false;
        }

        /// <summary>
        /// 尝试获取原始位置
        /// </summary>
        /// <param name="cameraController"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public static bool TryGetOriginalPosition(BaseCameraMainController cameraController, out Vector3 position)
        {
            if (!cameraController)
            {
                position = default;
                return false;
            }

            var cameraTransformer = cameraController.cameraTransformer;
            if (!cameraTransformer)
            {
                position = default;
                return false;
            }

            var recorder = cameraTransformer.originalTransformRecorder;
            if (recorder != null)
            {
                var info = recorder._records.FirstOrDefault();
                if (info != null)
                {
                    position = info.worldPosition;
                    return true;
                }
            }

#if UNITY_EDITOR
            var cameraTransform = cameraTransformer.mainTransform;
            if (cameraTransform)
            {
                position = cameraTransform.position;
                return true;
            }
#endif

            position = default;
            return false;
        }

        /// <summary>
        /// 尝试获取目标位置
        /// </summary>
        /// <param name="cameraController"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public static bool TryGetTargetPosition(BaseCameraMainController cameraController, out Vector3 position)
        {
            if (!cameraController)
            {
                position = default;
                return false;
            }

            var cameraTargetController = cameraController.cameraTargetController;
            if (!cameraTargetController)
            {
                position = default;
                return false;
            }

            position = cameraTargetController.targetPosition;
            return true;
        }
    }

    /// <summary>
    /// 位置类型
    /// </summary>
    [Name("位置类型")]
    public enum EPositionType
    {
        /// <summary>
        /// 无
        /// </summary>
        [Name("无")]
        None,

        /// <summary>
        /// 位置：坐标位置
        /// </summary>
        [Name("位置")]
        [Tip("坐标位置", "Coordinate position")]
        Postion,

        /// <summary>
        /// 变换：变换的位置
        /// </summary>
        [Name("变换")]
        [Tip("变换的位置", "Position of transformation")]
        Transform,

        /// <summary>
        /// 原始：相机变换器中记录的原始位置
        /// </summary>
        [Name("原始")]
        [Tip("相机变换器中记录的原始位置", "The original position recorded in the camera converter")]
        Original,

        /// <summary>
        /// 目标：目标控制器中的目标位置
        /// </summary>
        [Name("目标")]
        [Tip("目标控制器中的目标位置", "Target position in target controller")]
        Target,
    }
}
