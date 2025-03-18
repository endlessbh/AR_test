using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Maths;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginTools;

namespace XCSJ.CommonUtils.PluginCrossSectionShader
{
    /// <summary>
    /// 三剖面控制器
    /// </summary>
    [Name("三剖面控制器")]
    public class ThreePlanesCuttingController : BasePlaneCuttingMB
    {
        [Name("X剖面")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ComponentPopup]
        public PlaneCuttingInfo planeX;

        [Name("Y剖面")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ComponentPopup]
        public PlaneCuttingInfo planeY;

        [Name("Z剖面")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ComponentPopup]
        public PlaneCuttingInfo planeZ;

        public static event Action<ThreePlanesCuttingController, PlaneCuttingInfo> cuttingPlaneActiveChanged = null;

        public PlaneCuttingInfo[] planes => new PlaneCuttingInfo[] { planeX, planeY, planeZ };

        protected Material[] materials { get; set; }

        /// <summary>
        /// 唤醒初始化
        /// </summary>
        protected void Awake()
        {
            InitShaderProperty();
        }

        /// <summary>
        /// 启动
        /// </summary>
        protected void Start()
        {
            
        }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            PlaneCuttingInfo.onChanged += OnPlaneCuttingInfoChanged;

            UpdatePlaneCuttingInfos();

            // 检查三个剖面
            if (!planeX)
            {
                Debug.LogError(CommonFun.Name(typeof(ThreePlanesCuttingController), nameof(planeX)) + "为空对象！");
                return;
            }

            if (!planeY)
            {
                Debug.LogError(CommonFun.Name(typeof(ThreePlanesCuttingController), nameof(planeY)) + "为空对象！");
                return;
            }

            if (!planeZ)
            {
                Debug.LogError(CommonFun.Name(typeof(ThreePlanesCuttingController), nameof(planeZ)) + "为空对象！");
                return;
            }            
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            PlaneCuttingInfo.onChanged -= OnPlaneCuttingInfoChanged;

            try
            {
                SetInvalidPlaneCuttingInfos();
            }
            catch
            {

            }
        }

        protected void Update()
        {
            CheckChange();
        }

        private Vector3 lastPosition = Vector3.zero;

        public static event Action<ThreePlanesCuttingController> onChanged = null;

        public void SetMaterials(Material[] materials) { this.materials = materials; }

        public void CheckChange()
        {
            // 向量和位置没有变化，则不设置Shader
            if (MathX.ApproximatelyZero((lastPosition - transform.position).sqrMagnitude))
            {
                return;
            }

            CallChanged(transform);
        }

        protected void CallChanged(Transform transform)
        {
            UpdatePlaneCuttingInfos();

            onChanged?.Invoke(this);

            lastPosition = transform.position;
        }

        public void InitShaderProperty()
        {
            if (planeX)
            {
                planeX.SetShaderPropertyName(ShaderHelper.GenericThreePlanesBSP._Plane1Normal, ShaderHelper.GenericThreePlanesBSP._Plane1Position);
            }

            if (planeY)
            {
                planeY.SetShaderPropertyName(ShaderHelper.GenericThreePlanesBSP._Plane2Normal, ShaderHelper.GenericThreePlanesBSP._Plane2Position);
            }

            if (planeZ)
            {
                planeZ.SetShaderPropertyName(ShaderHelper.GenericThreePlanesBSP._Plane3Normal, ShaderHelper.GenericThreePlanesBSP._Plane3Position);
            }
        }

        public PlaneCuttingInfo GetActivePlane()
        {
            if (planeX && planeX.isActiveAndEnabled) return planeX;
            if (planeY && planeY.isActiveAndEnabled) return planeY;
            if (planeZ && planeZ.isActiveAndEnabled) return planeZ;

            return null;            
        }

        protected void OnPlaneCuttingInfoChanged(PlaneCuttingInfo planeCuttingInfo)
        {
            UpdatePlaneCuttingInfos();
        }

        public void SetInvalidPlaneCuttingInfos()
        {
            if (planeX) planeX.SetRendererInfoInvalid();
            if (planeY) planeY.SetRendererInfoInvalid();
            if (planeZ) planeZ.SetRendererInfoInvalid();
        }

        public void UpdatePlaneCuttingInfos()
        {
            if (materials == null) return;

            var plane = GetActivePlane();

            if (plane)
            {
                UpdatePlaneCuttingInfo(planeX, plane);
                UpdatePlaneCuttingInfo(planeY, plane);
                UpdatePlaneCuttingInfo(planeZ, plane);
            }
            else
            {
                SetInvalidPlaneCuttingInfos();
            }
        }

        private void UpdatePlaneCuttingInfo(PlaneCuttingInfo planeCuttingInfo, PlaneCuttingInfo activePlane)
        {
            if (!planeCuttingInfo) return;
            
            if (planeCuttingInfo.isActiveAndEnabled)
            {
                planeCuttingInfo.SetMaterialInfo(materials);
            }
            // 当前剖面无效，则使用一个有效剖面的向量与位置去设置无效剖面的Shader属性
            else if (activePlane)
            {
                
                planeCuttingInfo.SetMaterialInfo(activePlane.Normal, activePlane.Position, materials);
            }
        }

        public void ActivePlaneX(bool active)
        {
            if (planeX)
            {
                planeX.gameObject.SetActive(active);
                cuttingPlaneActiveChanged?.Invoke(this, planeX);
            }
        }

        public void ActivePlaneY(bool active)
        {
            if (planeY)
            {
                planeY.gameObject.SetActive(active);
                cuttingPlaneActiveChanged?.Invoke(this, planeY);
            }
        }

        public void ActivePlaneZ(bool active)
        {
            if (planeZ)
            {
                planeZ.gameObject.SetActive(active);
                cuttingPlaneActiveChanged?.Invoke(this, planeZ);
            }
        }

        public void ActiveAixs(bool active)
        {
            if (planeX && planeX.axis) planeX.axis.SetActive(active);
            if (planeY && planeY.axis) planeY.axis.SetActive(active);
            if (planeZ && planeZ.axis) planeZ.axis.SetActive(active);
        }

        public void ActivePlane(bool active)
        {
            if (planeX && planeX.plane) planeX.plane.SetActive(active);
            if (planeY && planeY.plane) planeY.plane.SetActive(active);
            if (planeZ && planeZ.plane) planeZ.plane.SetActive(active);
        }

        public void ActiveCuttingPlane(bool active)
        {
            if (planeX && planeX.cuttingPlane) planeX.cuttingPlane.SetActive(active);
            if (planeY && planeY.cuttingPlane) planeY.cuttingPlane.SetActive(active);
            if (planeZ && planeZ.cuttingPlane) planeZ.cuttingPlane.SetActive(active);
        }
    }
}
