using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.Extension.Interactions.Tools;

namespace XCSJ.PluginTools.Dashboards
{
    /// <summary>
    /// 仪表盘
    /// </summary>
    [Name("仪表盘")]
    public abstract class Dashboard : InteractProvider
    {
        /// <summary>
        /// 指针
        /// </summary>
        [Name("指针对象")]
        [Tip("默认旋转轴为指针对象的Z轴", "The default rotation axis is the Z axis of the pointer object")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [Readonly(EEditorMode.Runtime)]
        public Transform _needle = null;

        /// <summary>
        /// 旋转角范围
        /// </summary>
        [Name("旋转角范围")]
        public Vector2 _rotationAngleRange = new Vector2(-360, 360);

        /// <summary>
        /// 旋转角范围
        /// </summary>
        [Name("旋转角乘积")]
        public float _rotationAngleMultiplier = 1f;

        /// <summary>
        /// 单位
        /// </summary>
        [Name("单位")]
        public string _unit = "";

        /// <summary>
        /// 有效性
        /// </summary>
        protected bool _valid  = false;

        public virtual float needleAngle { get; set; }

        private float _initAngleZ = 0;

        /// <summary>
        /// 重置
        /// </summary>
        protected virtual void Reset()
        {
            ResetData();
        }

        /// <summary>
        /// 初始化单位
        /// </summary>
        protected virtual void ResetData() { }

        /// <summary>
        /// 开始
        /// </summary>
        protected virtual void Start()
        {
            _valid = _needle;
            if (_valid)
            {
                _initAngleZ = _needle.localEulerAngles.z;
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        protected virtual void LateUpdate()
        {
            if (_valid)
            {
                RotateNeedle(needleAngle);
            }
        }

        /// <summary>
        /// 选择指针
        /// </summary>
        /// <param name="rotateAngle"></param>
        public void RotateNeedle(float rotateAngle)
        {
            var zAngel = Mathf.Clamp(rotateAngle * _rotationAngleMultiplier, _rotationAngleRange.x, _rotationAngleRange.y);
            _needle.localEulerAngles = new Vector3(_needle.localEulerAngles.x, _needle.localEulerAngles.y, (_initAngleZ + zAngel));
        }
    }
}
