using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Recorders;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;

namespace XCSJ.PluginTools.Effects
{
    /// <summary>
    /// 修改渲染器属性：切换材质或修改材质上的颜色
    /// </summary>
    [Name("修改渲染器属性")]
    [Tip("切换材质或修改材质上的颜色", "Toggle materials or change colors on materials")]
    [XCSJ.Attributes.Icon(EIcon.Material)]
    public class RendererPropertyModify : BaseEffect, IColorEffect
    {
        [Flags]
        public enum ESetPropertyRule
        {
            [Name("颜色")]
            Color = 1 << 0,

            [Name("材质")]
            Materails = 1 << 1,
        }

        /// <summary>
        /// 设置属性规则
        /// </summary>
        [Name("设置属性规则")]
        [EnumPopup]
        public ESetPropertyRule _setPropertyRule = ESetPropertyRule.Color;

        /// <summary>
        /// 颜色
        /// </summary>
        [Name("颜色")]
        [HideInSuperInspector(nameof(_setPropertyRule), EValidityCheckType.NotHasFlag, ESetPropertyRule.Color)]
        public Color _color = Color.green;

        /// <summary>
        /// 颜色特效接口
        /// </summary>
        public Color color { get => _color; set => _color = value; }

        /// <summary>
        /// 材质组
        /// </summary>
        [Name("材质组")]
        [ValidityCheck(EValidityCheckType.ElementCountGreater, 0)]
        [HideInSuperInspector(nameof(_setPropertyRule), EValidityCheckType.NotHasFlag, ESetPropertyRule.Materails)]
        public Material[] _materials;

        private RendererRecorder rendererRecorder = new RendererRecorder();

        /// <summary>
        /// 可视化效果启用时记录渲染器属性并设定对应的可视化效果
        /// </summary>
        /// <param name="interactData"></param>
        /// <param name="gameObject"></param>
        public override void EnableEffect(InteractData interactData, GameObject gameObject)
        {
            if(!gameObject) return;
            rendererRecorder.Record(gameObject, true);

            var setColor = (_setPropertyRule & ESetPropertyRule.Color) == ESetPropertyRule.Color;
            var setMaterail = (_setPropertyRule & ESetPropertyRule.Materails) == ESetPropertyRule.Materails;
            foreach (var info in rendererRecorder._records)
            {
                if (setColor)
                {
                    info.SetColor(_color);
                }
                if (setMaterail)
                {
                    info.SetMaterial(_materials);
                }
            }
        }

        /// <summary>
        /// 可视化效果禁用时还原渲染器修改前的属性
        /// </summary>
        /// <param name="interactData"></param>
        /// <param name="gameObject"></param>
        public override void DisableEffect(InteractData interactData, GameObject gameObject)
        {
            if (!gameObject) return;
            rendererRecorder.RecoverAndRemove(r => r.renderer.transform.IsChildOf(gameObject.transform));
        }
    }
}
