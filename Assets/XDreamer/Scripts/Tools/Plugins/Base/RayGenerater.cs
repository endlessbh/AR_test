using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.Extension.Base.Maths;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginsCameras;

namespace XCSJ.PluginTools.Base
{
    /// <summary>
    /// 射线生成器
    /// </summary>
    [Serializable]
    [Name("射线生成器")]
    public class RayGenerater
    {
        /// <summary>
        /// 参考规则
        /// </summary>
        [Name("参考规则")]
        [EnumPopup]
        public EReferenceRule _referenceRule = EReferenceRule.CameraScreenMousePositionRay;

        /// <summary>
        /// 相机位置本地坐标系偏移量
        /// </summary>
        [Name("相机位置本地坐标系偏移量")]
        [HideInSuperInspector(nameof(_referenceRule), EValidityCheckType.NotEqual, EReferenceRule.Camera)]
        public Vector3PropertyValue _camPositionOffset = new Vector3PropertyValue(Vector3.zero);

        /// <summary>
        /// 点与方向数据
        /// </summary>
        [Name("点与方向数据")]
        [HideInSuperInspector(nameof(_referenceRule), EValidityCheckType.NotEqual, EReferenceRule.PointDirectionData)]
        public PointDirectionData _pointDirectionData = new PointDirectionData();

        /// <summary>
        /// 尝试获取射线
        /// </summary>
        /// <param name="ray"></param>
        /// <returns></returns>
        public bool TryGetRay(out Ray ray)
        {
            switch (_referenceRule)
            {
                case EReferenceRule.Camera:
                    {
                        var tmpCam = CameraHelperExtension.currentCamera;
                        if (tmpCam)
                        {
                            var t = tmpCam.transform;
                            var position = t.position;
                            if (_camPositionOffset.TryGetValue(out var positionOffset))
                            {
                                position += t.right * positionOffset.x;
                                position += t.up * positionOffset.y;
                                position += t.forward * positionOffset.z;
                            }
                            ray = new Ray(position, t.forward);
                            return true;
                        }
                        break;
                    }
                case EReferenceRule.CameraScreenMousePositionRay:
                    {
                        var tmpCam = CameraHelperExtension.currentCamera;
                        if (tmpCam)
                        {
                            ray = tmpCam.ScreenPointToRay(Input.mousePosition);
                            return true;
                        }
                        break;
                    }
                case EReferenceRule.PointDirectionData:
                    {
                        ray = _pointDirectionData.CreateRay();
                        return true;
                    }
            }
            ray = default;
            return false;
        }

        /// <summary>
        /// 重置回调
        /// </summary>
        public void OnReset(Transform transform)
        {
            _pointDirectionData._directionData._dataType = Extension.Base.Maths.EVector3DataType.TransformForward;

            SetPointDirectionTarget(transform);
        }

        /// <summary>
        /// 获取方向的变换对象
        /// </summary>
        public Transform pointTagrget => _pointDirectionData.pointTransform;

        /// <summary>
        /// 获取方向的变换对象
        /// </summary>
        public Transform directTagrget => _pointDirectionData.directionTransform;

        /// <summary>
        /// 设置点与方向数据的变换对象
        /// </summary>
        /// <param name="transform"></param>
        public void SetPointDirectionTarget(Transform transform)
        {
            _pointDirectionData._pointData.SetTransform(transform);
            _pointDirectionData._directionData.SetTransform(transform);
        }
    }

    /// <summary>
    /// 参考点规则
    /// </summary>
    [Name("参考点规则")]
    public enum EReferenceRule
    {
        [Name("当前相机")]
        Camera,

        [Name("当前相机屏幕鼠标点射线")]
        CameraScreenMousePositionRay,

        [Name("点与方向数据")]
        PointDirectionData,
    }
}
