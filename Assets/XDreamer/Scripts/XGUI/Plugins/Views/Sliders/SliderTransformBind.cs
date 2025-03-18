using UnityEngine;
using UnityEngine.Serialization;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginXGUI.Views.Sliders
{
    /// <summary>
    /// 滑动条变换绑定类
    /// </summary>
    public abstract class SliderTransformBind : SliderXYZBind
    {
        /// <summary>
        /// 目标转换
        /// </summary>
        [Name("目标转换")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ComponentPopup]
        [FormerlySerializedAs("bindTransform")]
        public Transform _targetTransform = null;

        /// <summary>
        /// 禁用时还原数据
        /// </summary>
        [Name("禁用时还原数据")]
        public bool resetDataWhenDisable = true;

        /// <summary>
        /// 唤醒
        /// </summary>
        protected virtual void Awake()
        {
            if(!_targetTransform)
            {
                _targetTransform = transform;
            }

            RecordData();
        }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            if(resetDataWhenDisable)
            {
                Debug.Log("重置值：");
                ClearLastSliderValue();
                RecoverData();
            }
        }

        /// <summary>
        /// 记录数据
        /// </summary>
        protected abstract void RecordData();

        /// <summary>
        /// 重置数据
        /// </summary>
        protected abstract void RecoverData();
    }
}
