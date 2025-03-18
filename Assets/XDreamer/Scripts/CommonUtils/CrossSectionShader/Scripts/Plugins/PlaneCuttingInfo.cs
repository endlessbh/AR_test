using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Maths;

namespace XCSJ.CommonUtils.PluginCrossSectionShader
{
    /// <summary>
    /// 剖面信息类
    /// </summary>
    [Name("剖面信息")]
    public class PlaneCuttingInfo : BasePlaneCuttingMB
    {
        private Transform thisTransform;

        /// <summary>
        /// 轴
        /// </summary>
        [Name("轴")]
        public GameObject axis = null;

        /// <summary>
        /// 面
        /// </summary>
        [Name("面")]
        public GameObject plane = null;

        /// <summary>
        /// 剖面
        /// </summary>
        [Name("剖面")]
        public GameObject cuttingPlane = null;

        /// <summary>
        /// 法线名
        /// </summary>
        public string normalName { get; set; }

        /// <summary>
        /// 位置名
        /// </summary>
        public string positionName { get; set; }

        /// <summary>
        /// 位置
        /// </summary>
        public Vector3 Position => transform.position;
            
        /// <summary>
        /// 法线
        /// </summary>
        public Vector3 Normal => transform.forward;

        /// <summary>
        /// 上次法线
        /// </summary>
        private Vector3 lastNormal = Vector3.zero;

        private Vector3 lastPosition = Vector3.zero;

        /// <summary>
        /// 向量与位置变化回调函数
        /// </summary>
        public static event Action<PlaneCuttingInfo> onChanged = null;

        /// <summary>
        /// 启动
        /// </summary>
        protected void Start() { thisTransform = this.transform; }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            CallChanged(transform);
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            try
            {
                CallChanged(transform);
            }
            catch
            {

            }
        }

        protected void Update()
        {
            CheckChange();
        }

        /// <summary>
        /// 检查变化
        /// </summary>
        public void CheckChange()
        {
            // 向量和位置没有变化，则不设置Shader
            if (MathX.ApproximatelyZero((lastNormal - thisTransform.forward).sqrMagnitude)
                && MathX.ApproximatelyZero((lastPosition - thisTransform.position).sqrMagnitude))
            {
                return;
            }

            CallChanged(thisTransform);
        }

        /// <summary>
        /// 调用变化
        /// </summary>
        /// <param name="transform"></param>
        protected void CallChanged(Transform transform)
        {
            onChanged?.Invoke(this);

            lastNormal = transform.forward;
            lastPosition = transform.position;
        }

        #region Sharder

        /// <summary>
        /// 设置着色器属性名
        /// </summary>
        /// <param name="normalName"></param>
        /// <param name="positionName"></param>
        public void SetShaderPropertyName(string normalName, string positionName)
        {
            this.normalName = normalName;
            this.positionName = positionName;
        }

        /// <summary>
        /// 设置渲染器信息
        /// </summary>
        /// <param name="normal"></param>
        /// <param name="position"></param>
        /// <param name="renderers"></param>
        public void SetRendererInfo(Vector3 normal, Vector3 position, params Renderer[] renderers)
        {
            foreach (var r in renderers)
            {
                if (r) SetMaterialInfo(normal, position, r.materials);
            }
        }

        /// <summary>
        /// 设置材质信息
        /// </summary>
        /// <param name="materials"></param>
        public void SetMaterialInfo(params Material[] materials)
        {
            foreach (var m in materials)
            {
                SetMaterialInfo(m, Normal, Position);
            }
        }

        /// <summary>
        /// 设置材质信息
        /// </summary>
        /// <param name="normal"></param>
        /// <param name="position"></param>
        /// <param name="materials"></param>
        public void SetMaterialInfo(Vector3 normal, Vector3 position, params Material[] materials)
        {
            foreach (var m in materials)
            {
                SetMaterialInfo(m, normal, position);
            }
        }

        /// <summary>
        /// 设置渲染器信息无效
        /// </summary>
        /// <param name="renderers"></param>
        public void SetRendererInfoInvalid(params Renderer[] renderers)
        {
            foreach (var r in renderers)
            {
                SetMaterialInfoInvalid(r.materials);
            }
        }

        /// <summary>
        /// 设置材质信息无效
        /// </summary>
        /// <param name="materials"></param>
        public void SetMaterialInfoInvalid(params Material[] materials)
        {
            foreach (var m in materials)
            {
                SetMaterialInfo(m, Vector3.zero, Vector3.zero);
            }
        }

        /// <summary>
        /// 设置材质信息
        /// </summary>
        /// <param name="material"></param>
        /// <param name="normal"></param>
        /// <param name="position"></param>
        public void SetMaterialInfo(Material material, Vector3 normal, Vector3 position)
        {
            if (material)
            {
                material.SetVector(normalName, normal);
                material.SetVector(positionName, position);
            }
        }

        #endregion

    }
}
