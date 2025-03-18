using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginXGUI.Base;
using XCSJ.PluginXGUI.DataViews.Base;
using XCSJ.PluginXGUI.Views.Sliders;

namespace XCSJ.PluginXGUI.DataViews.GroupViews
{
    /// <summary>
    /// 灯光视图
    /// </summary>
    [Name("灯光视图")]
    [DataViewAttribute(typeof(Transform))]
#if UNITY_EDITOR && XDREAMER_EDITION_DEVELOPER
    //[Tool(XGUICategory.DataGroupView, rootType = typeof(XGUIManager))]
#endif
    public class LightView : ComponentView
    {
        /// <summary>
        /// 灯光
        /// </summary>
        [Name("灯光")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Light _light;

        /// <summary>
        /// 灯光
        /// </summary>
        public Light lightObject => this.XGetComponentInParentOrGlobal(ref _light);

        /// <summary>
        /// 因Unity存储角度使用四元数，因此每次转化为角度时数值是不确定的，因此采用当前值作为唯一参考值
        /// </summary>
        private Vector3 recordAngle = Vector3.zero;
        private bool initRecordAngle = true;

        /// <summary>
        /// 灯光角度
        /// </summary>
        [Name("灯光角度")]
        public Vector3 angle
        {
            get
            {
                if (initRecordAngle)
                {
                    initRecordAngle = false;
                    recordAngle = SliderTransformRotationBind.AngleClampN180ToP180(lightObject.transform.eulerAngles);
                }
                return recordAngle;
            }
            set
            {
                lightObject.transform.eulerAngles = recordAngle = value;
            }
        }

        /// <summary>
        /// 灯光颜色
        /// </summary>
        [Name("灯光颜色")]
        public Color color
        {
            get
            {
                return lightObject.color;
            }
            set
            {
                lightObject.color = value;
            }
        }

        /// <summary>
        /// 灯光强度
        /// </summary>
        [Name("灯光强度")]
        public float intensity
        {
            get
            {
                return lightObject.intensity;
            }
            set
            {
                lightObject.intensity = value;
            }
        }
    }
}
