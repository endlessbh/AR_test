using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginTools;
using XCSJ.PluginXGUI.ViewControllers;
using XCSJ.PluginXGUI.Views.Sliders;

namespace XCSJ.CommonUtils.PluginCrossSectionShader
{
    /// <summary>
    /// 剖面视图控制器
    /// </summary>
    [Name("剖面视图控制器")]
    [RequireManager(typeof(ToolsExtensionManager))]
    public sealed class PlaneCuttingViewController: BaseViewController
    {
        /// <summary>
        /// 剖面控制器
        /// </summary>
        [Name("剖面控制器")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public PlaneCuttingController _planeCuttingController;

        /// <summary>
        /// 剖面控制器
        /// </summary>
        public PlaneCuttingController planeCuttingController => this.XGetComponentInParentOrGlobal(ref _planeCuttingController);

        /// <summary>
        /// 剖切类型：交集或并集
        /// </summary>
        public ECutMode useCuttingMaterial
        {
            get
            {
                return planeCuttingController.cutMode;
            }
            set
            {
                planeCuttingController.cutMode = value;
            }
        }

        #region 剖面位置控制

        /// <summary>
        /// 三剖面变换
        /// </summary>
        [Name("三剖面变换")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Transform _planeCutting;

        /// <summary>
        /// 三剖面位置
        /// </summary>
        public Vector3 planeCuttingPosition
        {
            get
            {
                return _planeCutting.position;
            }
            set
            {
                _planeCutting.position = value;
            }
        }

        private Vector3 _orgPosition;

        /// <summary>
        /// 重置位置
        /// </summary>
        public void ResetPosition()
        {
            planeCuttingPosition = _orgPosition;
        }

        #endregion

        #region X剖面

        /// <summary>
        /// X剖面 
        /// </summary>
        [Group("剖面设置", textEN = "Plane Cutting Setting", defaultIsExpanded = false)]
        [Name("X剖面")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public GameObject _xPlane;

        /// <summary>
        /// 激活X剖面
        /// </summary>
        public bool activeXPlane
        {
            get
            {
                return _xPlane.activeSelf;
            }
            set
            {
                _xPlane.SetActive(value);
            }
        }

        /// <summary>
        /// 因Unity存储角度使用四元数，因此每次转化为角度时数值是不确定的，因此采用当前值作为唯一参考值
        /// </summary>
        private Vector3 recordXPlaneAngle = Vector3.zero;
        private bool initRecordXPlaneAngle = true;

        /// <summary>
        /// X剖面角度
        /// </summary>
        public Vector3 xPlaneAngle
        {
            get
            {
                if (initRecordXPlaneAngle)
                {
                    initRecordXPlaneAngle = false;
                    recordXPlaneAngle = SliderTransformRotationBind.AngleClampN180ToP180(_xPlane.transform.eulerAngles);
                }
                return recordXPlaneAngle;
            }
            set
            {
                _xPlane.transform.eulerAngles = recordXPlaneAngle = value;
            }
        }

        #endregion

        #region Y剖面

        /// <summary>
        /// Y剖面 
        /// </summary>
        [Name("Y剖面")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public GameObject _yPlane;

        /// <summary>
        /// 激活Y剖面 
        /// </summary>
        public bool activeYPlane
        {
            get
            {
                return _yPlane.activeSelf;
            }
            set
            {
                _yPlane.SetActive(value);
            }
        }

        /// <summary>
        /// 因Unity存储角度使用四元数，因此每次转化为角度时数值是不确定的，因此采用当前值作为唯一参考值
        /// </summary>
        private Vector3 recordYPlaneAngle = Vector3.zero;
        private bool initRecordYPlaneAngle = true;

        /// <summary>
        /// Y剖面角度 
        /// </summary>
        public Vector3 yPlaneAngle
        {
            get
            {
                if (initRecordYPlaneAngle)
                {
                    initRecordYPlaneAngle = false;
                    recordYPlaneAngle = SliderTransformRotationBind.AngleClampN180ToP180(_yPlane.transform.eulerAngles);
                }
                return recordYPlaneAngle;
            }
            set
            {
                _yPlane.transform.eulerAngles = recordYPlaneAngle = value;
            }
        }

        #endregion

        #region Z剖面

        /// <summary>
        /// Z剖面 
        /// </summary>
        [Name("Z剖面")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public GameObject _zPlane;

        /// <summary>
        /// 激活Z剖面 
        /// </summary>
        public bool activeZPlane
        {
            get
            {
                return _zPlane.activeSelf;
            }
            set
            {
                _zPlane.SetActive(value);
            }
        }
        /// <summary>
        /// 因Unity存储角度使用四元数，因此每次转化为角度时数值是不确定的，因此采用当前值作为唯一参考值
        /// </summary>
        private Vector3 recordZPlaneAngle = Vector3.zero;
        private bool initRecordZPlaneAngle = true;

        /// <summary>
        /// Z剖面角度
        /// </summary>
        public Vector3 zPlaneAngle
        {
            get
            {
                if (initRecordZPlaneAngle)
                {
                    initRecordZPlaneAngle = false;
                    recordZPlaneAngle = SliderTransformRotationBind.AngleClampN180ToP180(_zPlane.transform.eulerAngles);
                }
                return recordZPlaneAngle;
            }
            set
            {
                _zPlane.transform.eulerAngles = recordZPlaneAngle = value;
            }
        }

        #endregion

        private Vector3 orgXAngle;
        private Vector3 orgYAngle;
        private Vector3 orgZAngle;

        public void ResetXYZPlaneAngle()
        {
            xPlaneAngle = orgXAngle;
            yPlaneAngle = orgYAngle;
            zPlaneAngle = orgZAngle;
        }

        #region 轴模型

        /// <summary>
        /// 激活轴
        /// </summary>
        [Group("轴设置", textEN = "Axle Settings", defaultIsExpanded = false)]
        [Name("激活轴")]
        public bool _activeAxle = true;

        /// <summary>
        /// 激活轴
        /// </summary>
        public bool activeAxle
        {
            get
            {
                return _activeAxle;
            }
            set
            {
                _activeAxle = value;
                _axles.ForEach(a => a.SetActive(_activeAxle));
            }
        }

        /// <summary>
        /// 轴模型列表
        /// </summary>
        [Name("轴模型列表")]
        public List<GameObject> _axles;

        #endregion

        #region 剖面模型

        /// <summary>
        /// 激活面
        /// </summary>
        [Group("面设置", textEN = "Plane Settings", defaultIsExpanded = false)]
        [Name("激活面")]
        public bool _activePlane = true;

        /// <summary>
        /// 激活面
        /// </summary>
        public bool activePlane
        {
            get
            {
                return _activePlane;
            }
            set
            {
                _activePlane = value;
                _planes.ForEach(p => p.SetActive(_activePlane));
            }
        }

        /// <summary>
        /// 面列表
        /// </summary>
        [Name("面列表")]
        public List<GameObject> _planes;

        #endregion

        #region 带斜纹的剖面模型

        /// <summary>
        /// 激活斜纹面
        /// </summary>
        [Group("斜纹面设置", textEN = "Cross Plane Settings", defaultIsExpanded = false)]
        [Name("激活斜纹面")]
        public bool _activeCrossPlane = true;

        /// <summary>
        /// 激活斜纹面
        /// </summary>
        public bool activeCrossPlane
        {
            get
            {
                return _activeCrossPlane;
            }
            set
            {
                _activeCrossPlane = value;
                _crossPlanes.ForEach(c => c.SetActive(_activeCrossPlane));
            }
        }

        /// <summary>
        /// 斜纹面列表
        /// </summary>
        [Name("斜纹面列表")]
        public List<GameObject> _crossPlanes;

        #endregion

        private void OnValidate()
        {
            UpdateAxlePlaneCrossPlane();
        }

        /// <summary>
        /// 唤醒
        /// </summary>
        public void Awake()
        {
            if (planeCuttingController) { }
        }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            _orgPosition = _planeCutting.position;

            orgXAngle = xPlaneAngle;
            orgYAngle = yPlaneAngle;
            orgZAngle = zPlaneAngle;

            UpdateAxlePlaneCrossPlane();
        }

        private void UpdateAxlePlaneCrossPlane()
        {
            activeAxle = _activeAxle;
            activePlane = _activePlane;
            activeCrossPlane = _activeCrossPlane;
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
        }
    }


}
