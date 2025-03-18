using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.PluginCamera;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginsCameras.Base;
using XCSJ.PluginsCameras.Controllers;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.PluginsCameras.States
{
    /// <summary>
    /// 相机控制器属性设置
    /// </summary>
    [Name(Title, nameof(CameraControllerSwitchEvent))]
    [ComponentMenu(CameraHelperExtension.StateLibCategoryName + "/" + Title, typeof(CameraManager))]
    [XCSJ.Attributes.Icon(EIcon.Property)]
    [Owner(typeof(CameraManager))]
    public class CameraControllerPropertySet : BasePropertySet<CameraControllerPropertySet>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "相机控制器属性设置";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(CameraHelperExtension.StateLibCategoryName, typeof(CameraManager))]
        [StateComponentMenu(CameraHelperExtension.StateLibCategoryName + "/" + Title, typeof(CameraManager))]
        [Name(Title, nameof(CameraControllerPropertySet))]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 相机控制器查找规则
        /// </summary>
        [Name("相机控制器查找规则")]
        public enum ECameraControllerSearchRule
        {
            /// <summary>
            /// 指定
            /// </summary>
            [Name("指定")]
            Definite,

            /// <summary>
            /// 当前
            /// </summary>
            [Name("当前")]
            Current,
        }

        /// <summary>
        /// 相机控制器查找规则
        /// </summary>
        [Name("相机控制器查找规则")]
        [EnumPopup]
        public ECameraControllerSearchRule _cameraControllerSearchRule = ECameraControllerSearchRule.Definite;

        /// <summary>
        /// 相机控制器
        /// </summary>
        [Name("相机控制器")]
        [ComponentPopup]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [HideInSuperInspector(nameof(_cameraControllerSearchRule), EValidityCheckType.Equal, ECameraControllerSearchRule.Current)]
        public BaseCameraMainController _cameraController;

        /// <summary>
        /// 相机控制器
        /// </summary>
        public BaseCameraMainController cameraController
        {
            get
            {
                switch (_cameraControllerSearchRule)
                {
                    case ECameraControllerSearchRule.Current:
                        {
                            return CameraManager.instance.GetCurrentCameraController();
                        }
                    default: return _cameraController;
                }
            }
        }

        /// <summary>
        /// 属性名称
        /// </summary>
        [Name("属性名称")]
        [EnumPopup]
        public EPropertyName _propertyName = EPropertyName.None;

        /// <summary>
        /// 属性名称
        /// </summary>
        [Name("属性名称")]
        public enum EPropertyName
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None,

            /// <summary>
            /// 移动速度系数
            /// </summary>
            [Name("移动速度系数")]
            MoveSpeedCoefficient,

            /// <summary>
            /// 移动阻尼系数
            /// </summary>
            [Name("移动阻尼系数")]
            MoveDampingCoefficient,

            /// <summary>
            /// 旋转速度系数
            /// </summary>
            [Name("旋转速度系数")]
            RotateSpeedCoefficient,

            /// <summary>
            /// 旋转阻尼系数
            /// </summary>
            [Name("旋转阻尼系数")]
            RotateDampingCoefficient,

            /// <summary>
            /// 主目标
            /// </summary>
            [Name("主目标")]
            MainTarget,

            /// <summary>
            /// 恢复到原始状态:将相机控制器的变换恢复到程序启动时记录的状态
            /// </summary>
            [Name("恢复到原始状态")]
            [Tip("将相机控制器的变换恢复到程序启动时记录的状态", "Restore the transformation of the camera controller to the state recorded when the program is started")]
            RecoverToOriginal = 1000,

            /// <summary>
            /// 恢复到上一次状态:将相机控制器的变换恢复到上一次记录的状态
            /// </summary>
            [Name("恢复到上一次状态")]
            [Tip("将相机控制器的变换恢复到上一次记录的状态", "Restore the transformation of the camera controller to the last recorded state")]
            RecoverToLast,

            /// <summary>
            /// 尝试聚焦目标
            /// </summary>
            [Name("尝试聚焦目标")]
            TryFocusTarget,

            /// <summary>
            /// 尝试同步变换
            /// </summary>
            [Name("尝试同步变换")]
            TrySyncTransfrom,

            /// <summary>
            /// 尝试变换到(通过朝向)
            /// </summary>
            [Name("尝试变换到(通过朝向)")]
            TryTransformToByLookAt,
        }

        /// <summary>
        /// 移动速度系数
        /// </summary>
        [Name("移动速度系数")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.MoveSpeedCoefficient)]
        [OnlyMemberElements]
        public Vector3PropertyValue _moveSpeedCoefficient = new Vector3PropertyValue(Vector3.one);

        /// <summary>
        /// 移动阻尼系数
        /// </summary>
        [Name("移动阻尼系数")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.MoveDampingCoefficient)]
        [OnlyMemberElements]
        public FloatPropertyValue _moveDampingCoefficient = new FloatPropertyValue(1);

        /// <summary>
        /// 旋转速度系数
        /// </summary>
        [Name("旋转速度系数")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.RotateSpeedCoefficient)]
        [OnlyMemberElements]
        public Vector3PropertyValue _rotateSpeedCoefficient = new Vector3PropertyValue(Vector3.one);

        /// <summary>
        /// 旋转阻尼系数
        /// </summary>
        [Name("旋转阻尼系数")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.RotateDampingCoefficient)]
        [OnlyMemberElements]
        public FloatPropertyValue _rotateDampingCoefficient = new FloatPropertyValue(1);

        /// <summary>
        /// 主目标
        /// </summary>
        [Name("主目标")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.MainTarget)]
        [OnlyMemberElements]
        public TransformPropertyValue _mainTarget = new TransformPropertyValue();

        /// <summary>
        /// 主目标
        /// </summary>
        [Name("主目标")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.TryFocusTarget)]
        [OnlyMemberElements]
        public TransformPropertyValue _TryFocusTarget__mainTarget = new TransformPropertyValue();

        /// <summary>
        /// 相机聚焦目标模式
        /// </summary>
        [Name("相机聚焦目标模式")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.TryFocusTarget)]
        [OnlyMemberElements]
        public ECameraFocusTargetModePropertyValue _TryFocusTarget__cameraFocusTargetMode = new ECameraFocusTargetModePropertyValue();

        /// <summary>
        /// 相机聚焦位置
        /// </summary>
        [Name("相机聚焦位置")]
        [Tip("仅在‘相机聚焦目标模式’为‘包围盒锚点’时生效", "Effective only when 'camera focus target mode' is' bounding box anchor '")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.TryFocusTarget)]
        [OnlyMemberElements]
        public EBoundsAnchorPropertyValue _TryFocusTarget__cameraFocusPosition = new EBoundsAnchorPropertyValue();

        /// <summary>
        /// 距离系数
        /// </summary>
        [Name("距离系数")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.TryFocusTarget)]
        [OnlyMemberElements]
        public FloatPropertyValue _TryFocusTarget__distanceScale = new FloatPropertyValue(1.732f);

        /// <summary>
        /// 时间
        /// </summary>
        [Name("时间")]
        [Tip("聚焦到目标所需时间；单位：秒；", "Time required to focus on the target; Unit: second;")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.TryFocusTarget)]
        [OnlyMemberElements]
        public FloatPropertyValue _TryFocusTarget__time = new FloatPropertyValue(1f);

        /// <summary>
        /// 主目标
        /// </summary>
        [Name("主目标")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.TrySyncTransfrom)]
        [OnlyMemberElements]
        public TransformPropertyValue _TrySyncTransfrom__mainTarget = new TransformPropertyValue();

        /// <summary>
        /// 同步变换
        /// </summary>
        [Name("同步变换")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.TrySyncTransfrom)]
        [OnlyMemberElements]
        public TransformPropertyValue _TrySyncTransfrom__syncTransform = new TransformPropertyValue();

        /// <summary>
        /// 时间
        /// </summary>
        [Name("时间")]
        [Tip("同步变换所需时间；单位：秒；", "Time required for synchronous transformation; Unit: second;")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.TrySyncTransfrom)]
        [OnlyMemberElements]
        public FloatPropertyValue _TrySyncTransfrom__time = new FloatPropertyValue(1f);

        /// <summary>
        /// 主目标
        /// </summary>
        [Name("主目标")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.TryTransformToByLookAt)]
        [OnlyMemberElements]
        public TransformPropertyValue _TryTransformToByLookAt__mainTarget = new TransformPropertyValue();

        /// <summary>
        /// 朝向位置
        /// </summary>
        [Name("朝向位置")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.TryTransformToByLookAt)]
        [OnlyMemberElements]
        public Vector3PropertyValue _TryTransformToByLookAt__lookAtPosition = new Vector3PropertyValue();

        /// <summary>
        /// 相机旋转
        /// </summary>
        [Name("相机旋转")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.TryTransformToByLookAt)]
        [OnlyMemberElements]
        public Vector3PropertyValue _TryTransformToByLookAt__cameraRotation = new Vector3PropertyValue();

        /// <summary>
        /// 朝向距离
        /// </summary>
        [Name("朝向距离")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.TryTransformToByLookAt)]
        [OnlyMemberElements]
        public FloatPropertyValue _TryTransformToByLookAt__lookAtDistance = new FloatPropertyValue(3f);

        /// <summary>
        /// 时间
        /// </summary>
        [Name("时间")]
        [Tip("变换到所需时间；单位：秒；", "Change to the required time; Unit: second;")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.TryTransformToByLookAt)]
        [OnlyMemberElements]
        public FloatPropertyValue _TryTransformToByLookAt__time = new FloatPropertyValue(1f);

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="stateData"></param>
        /// <param name="executeMode"></param>
        public override void Execute(StateData stateData, EExecuteMode executeMode)
        {
            var cameraController = this.cameraController;
            if (!cameraController) return;
            switch (_propertyName)
            {
                case EPropertyName.MoveSpeedCoefficient:
                    {
                        if (_moveSpeedCoefficient.TryGetValue(out var value))
                        {
                            cameraController.moveSpeedCoefficient = value;
                        }
                        break;
                    }
                case EPropertyName.MoveDampingCoefficient:
                    {
                        if (_moveDampingCoefficient.TryGetValue(out var value))
                        {
                            cameraController.moveDampingCoefficient = value;
                        }
                        break;
                    }
                case EPropertyName.RotateSpeedCoefficient:
                    {
                        if (_rotateSpeedCoefficient.TryGetValue(out var value))
                        {
                            cameraController.rotateSpeedCoefficient = value;
                        }
                        break;
                    }
                case EPropertyName.RotateDampingCoefficient:
                    {
                        if (_rotateDampingCoefficient.TryGetValue(out var value))
                        {
                            cameraController.rotateDampingCoefficient = value;
                        }
                        break;
                    }
                case EPropertyName.MainTarget:
                    {
                        cameraController.mainTarget = _mainTarget.GetValue();
                        break;
                    }
                case EPropertyName.RecoverToOriginal:
                    {
                        cameraController.cameraTransformer.RecoverToOriginal();
                        break;
                    }
                case EPropertyName.RecoverToLast:
                    {
                        cameraController.cameraTransformer.RecoverToLast();
                        break;
                    }
                case EPropertyName.TryFocusTarget:
                    {
                        cameraController.TryFocusTarget(_TryFocusTarget__mainTarget.GetValue(), _TryFocusTarget__cameraFocusTargetMode.GetValue(), _TryFocusTarget__cameraFocusPosition.GetValue(), _TryFocusTarget__distanceScale.GetValue(), _TryFocusTarget__time.GetValue());
                        break;
                    }
                case EPropertyName.TrySyncTransfrom:
                    {
                        cameraController.TrySyncTransfrom(_TrySyncTransfrom__mainTarget.GetValue(), _TrySyncTransfrom__syncTransform.GetValue(), _TrySyncTransfrom__time.GetValue());
                        break;
                    }
                case EPropertyName.TryTransformToByLookAt:
                    {
                        cameraController.TryTransformToByLookAt(_TryTransformToByLookAt__mainTarget.GetValue(), _TryTransformToByLookAt__lookAtPosition.GetValue(), _TryTransformToByLookAt__cameraRotation.GetValue(), _TryTransformToByLookAt__lookAtDistance.GetValue(), _TryTransformToByLookAt__time.GetValue());
                        break;
                    }
            }
        }

        private DirtyString dirtyString = new DirtyString();

        /// <summary>
        /// 输出友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString() => dirtyString.GetData(_ToFriendlyString);

        private string _ToFriendlyString()
        {
            switch (_propertyName)
            {
                case EPropertyName.MoveSpeedCoefficient:
                    {
                        return CommonFun.Name(_propertyName) + "=" + _moveSpeedCoefficient.ToFriendlyString();
                    }
                case EPropertyName.MoveDampingCoefficient:
                    {
                        return CommonFun.Name(_propertyName) + "=" + _moveDampingCoefficient.ToFriendlyString();
                    }
                case EPropertyName.RotateSpeedCoefficient:
                    {
                        return CommonFun.Name(_propertyName) + "=" + _rotateSpeedCoefficient.ToFriendlyString();
                    }
                case EPropertyName.RotateDampingCoefficient:
                    {
                        return CommonFun.Name(_propertyName) + "=" + _rotateDampingCoefficient.ToFriendlyString();
                    }
                case EPropertyName.MainTarget:
                    {
                        return CommonFun.Name(_propertyName) + "=" + _mainTarget.ToFriendlyString();
                    }
                case EPropertyName.TryFocusTarget:
                    {
                        return CommonFun.Name(_propertyName) + "=" + _TryFocusTarget__mainTarget.ToFriendlyString();
                    }
                case EPropertyName.TrySyncTransfrom:
                    {
                        return CommonFun.Name(_propertyName) + "=" + _TrySyncTransfrom__syncTransform.ToFriendlyString();
                    }
                case EPropertyName.TryTransformToByLookAt:
                    {
                        return CommonFun.Name(_propertyName) + "=" + _TryTransformToByLookAt__lookAtPosition.ToFriendlyString();
                    }
                default: return CommonFun.Name(_propertyName);
            }
        }

        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity()
        {
            if (_cameraControllerSearchRule != ECameraControllerSearchRule.Current && !_cameraController) return false;
            switch (_propertyName)
            {
                case EPropertyName.MoveSpeedCoefficient: return _moveSpeedCoefficient.DataValidity();
                case EPropertyName.MoveDampingCoefficient: return _moveDampingCoefficient.DataValidity();
                case EPropertyName.RotateSpeedCoefficient: return _rotateSpeedCoefficient.DataValidity();
                case EPropertyName.RotateDampingCoefficient: return _rotateDampingCoefficient.DataValidity();
                case EPropertyName.MainTarget: return _mainTarget.DataValidity();
            }
            return true;
        }

        /// <summary>
        /// 验证
        /// </summary>
        public void OnValidate()
        {
            dirtyString.SetDirty();
        }
    }
}
