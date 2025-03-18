using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginTools.Dashboards
{
    /// <summary>
    /// 预警仪表盘
    /// </summary>
    [Name("预警仪表盘")]
    public abstract class WarmingDashboard : Dashboard
    {
        [Name("使用UGUI警示")]
        [Readonly(EEditorMode.Runtime)]
        public bool useUGUIForIndicator = true;

        [Name("预警图像")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Image _image = null;

        [Name("预警渲染器")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Renderer _renderer = null;

        [Name("预警值")]
        [Range(0, 10000)]
        public float _warmingValue = 100;

        [Name("预警颜色")]
        public Color _warmingColor = Color.red;

        [Name("预警比较规则")]
        [EnumPopup]
        public EWarmingCompareRule _warmingCompareRule = EWarmingCompareRule.Greater;

        private Color _orgColor;

        /// <summary>
        /// 开始
        /// </summary>
        protected override void Start()
        {
            base.Start();

            if (useUGUIForIndicator)
            {
                if (_image)
                {
                    _orgColor = _image.color;
                }
            }
            else
            {
                if (_renderer)
                {
                    _orgColor = _renderer.material.color;
                }
            }
        }

        protected override void LateUpdate()
        {
            base.LateUpdate();

            if (_valid && ((useUGUIForIndicator && _image) ||(!useUGUIForIndicator && _renderer)))
            {
                bool warming = false;
                switch (_warmingCompareRule)
                {
                    case EWarmingCompareRule.Less: warming = needleAngle < _warmingValue; break;
                    case EWarmingCompareRule.LessEqual: warming = needleAngle <= _warmingValue; break;
                    case EWarmingCompareRule.Greater: warming = needleAngle > _warmingValue; break;
                    case EWarmingCompareRule.GreaterEqual: warming = needleAngle >= _warmingValue; break;
                    default: break;
                }

                if (useUGUIForIndicator)
                {
                    _image.color = warming ? _warmingColor : _orgColor;
                }
                else if (_renderer.material)
                {
                    _renderer.material.color = warming ? _warmingColor : _orgColor;
                }
            }
        }

        [Name("预警比较规则")]
        public enum EWarmingCompareRule
        {
            [Name("小于")]
            Less = 0,

            [Name("小于等于")]
            LessEqual,

            [Name("大于")]
            Greater,

            [Name("大于等于")]
            GreaterEqual,
        }
    }
}
