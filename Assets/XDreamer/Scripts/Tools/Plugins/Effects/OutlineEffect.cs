using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginCommonUtils.Tools;

namespace XCSJ.PluginTools.Effects
{
    /// <summary>
    /// 轮廓线可视化效果
    /// </summary>
    [Name("轮廓线效果")]
    [XCSJ.Attributes.Icon(EIcon.GameObjectActive)]
    public class OutlineEffect : BaseEffect, IOutlineData, IColorEffect
    {
        #region 轮廓线数据

        /// <summary>
        /// 轮廓线颜色
        /// </summary>
        [Name("轮廓线颜色")]
        public Color _color = Color.green;

        /// <summary>
        /// 轮廓线颜色
        /// </summary>
        [Name("轮廓线宽度")]
        [Min(0)]
        public float _width = 2;

        /// <summary>
        /// 无遮挡
        /// </summary>
        [Name("无遮挡")]
        public bool _overlay = true;

        /// <summary>
        /// 轮廓线颜色
        /// </summary>
        public Color color 
        { 
            get => _color; 
            set
            {
                _color = value;

                //强行刷新颜色
                var go = GameObjectEffectCache.FindGameObject(this);
                if (go)
                {
                    var controller = go.GetComponent<OutlineController>();
                    if (controller)
                    {
                        controller.StartDisplay(this);
                    }
                }
            }
        } 

        /// <summary>
        /// 轮廓线宽度
        /// </summary>
        public float width => _width;

        /// <summary>
        /// 轮廓线遮挡
        /// </summary>
        bool IOutlineData.overlay => _overlay;

        #endregion

        #region 特效处理

        private OutlineController outlineController
        {
            get
            {
                if (!_outlineController)
                {
                    _outlineController = this.XGetOrAddComponent<OutlineController>();
                }
                return _outlineController;
            }
        }
        private OutlineController _outlineController;

        /// <summary>
        /// 启用轮廓线效果
        /// </summary>
        /// <param name="interactData"></param>
        /// <param name="gameObject"></param>
        public override void EnableEffect(InteractData interactData, GameObject gameObject)
        {
            if (gameObject)
            {
                var controller = CommonFun.GetOrAddComponent<OutlineController>(gameObject);
                controller.StartDisplay(this);
            }
        }

        /// <summary>
        /// 禁用轮廓线效果
        /// </summary>
        /// <param name="interactData"></param>
        /// <param name="gameObject"></param>
        public override void DisableEffect(InteractData interactData, GameObject gameObject)
        {
            if (gameObject)
            {
                var controller = gameObject.GetComponent<OutlineController>();
                if (controller)
                {
                    controller.StopDisplay();
                }
            }
        }

        #endregion
    }
}
