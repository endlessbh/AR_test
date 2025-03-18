using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginTools.PropertyDatas;

namespace XCSJ.PluginTools.Effects
{
    public interface IColorEffect
    {
        Color color { get; set; }
    }

    /// <summary>
    /// 效果基类
    /// </summary>
    [RequireManager(typeof(ToolsManager))]
    [Owner(typeof(ToolsManager))]
    [Tool(InteractionCategory.Effect, nameof(BaseEffect), rootType = typeof(ToolsExtensionManager))]
    public abstract class BaseEffect : InteractTagProvider
    {
        /// <summary>
        /// 属性值数据查找规则
        /// </summary>
        public enum MatchTagPropertyValueDataRule
        {
            [Name("自动")]
            [Tip("优先从特效交互器正在交互的可交互对象上查找属性值；当目标可交互对象无匹配的属性值时，从当前组件所在的游戏对象上查找属性值", "Prioritize searching for attribute values from the interactive objects being interacted with by the special effects interactive device; When there is no matching attribute value for the target interactive object, search for attribute values from the game object where the current component is located")]
            Auto,

            [Name("可交互对象的交互属性")]
            [Tip("从特效交互器正在交互的可交互对象上查找匹配属性关键字的属性值", "Find property values that match property keywords on the interactive object being interacted with by the special effects interactive tool")]
            TargetPropertyValue,

            [Name("自身的交互属性")]
            [Tip("从当前组件所在的游戏对象上查找匹配属性关键字的属性值", "Find attribute values that match attribute keywords from the game object where the current component is located")]
            Self,
        }

        /// <summary>
        /// 与标签键值匹配属性数据查找规则
        /// </summary>
        [Name("与标签键值匹配属性数据查找规则")]
        [EnumPopup]
        public MatchTagPropertyValueDataRule _matchTagPropertyValueDataRule = MatchTagPropertyValueDataRule.Auto;

        /// <summary>
        /// 叠加效果
        /// </summary>
        [Name("叠加效果")]
        public bool _overlayEffect = false;

        /// <summary>
        /// 叠加效果
        /// </summary>
        public bool overlayEffect => _overlayEffect;

        /// <summary>
        /// 通过数据源规则获取交互属性值
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        protected InteractPropertyData GetInteractPropertyValueByRule(GameObject go)
        {
            var key = _tagProperty.firstKey;
            if (string.IsNullOrEmpty(key)) return null;
                
            return GetInteractPropertyValueByRule(_matchTagPropertyValueDataRule, go, key);
        }

        /// <summary>
        /// 通过数据源规则获取交互属性值
        /// </summary>
        /// <param name="dataSourceRule"></param>
        /// <param name="go"></param>
        /// <returns></returns>
        protected InteractPropertyData GetInteractPropertyValueByRule(MatchTagPropertyValueDataRule dataSourceRule, GameObject go, string key)
        {
            switch (dataSourceRule)
            {
                case MatchTagPropertyValueDataRule.Auto:
                    {
                        var data = GetInteractPropertyValueByRule(MatchTagPropertyValueDataRule.TargetPropertyValue, go, key);
                        if (data == null && go != gameObject)
                        {
                            return GetInteractPropertyValueByRule(MatchTagPropertyValueDataRule.Self, gameObject, key);
                        }
                        return data;
                    }
                case MatchTagPropertyValueDataRule.TargetPropertyValue: return InteractPropertyHelper.GetInteractPropertyValue(go, key);
                case MatchTagPropertyValueDataRule.Self: return InteractPropertyHelper.GetInteractPropertyValue(gameObject, key);
                default: return null;
            }
        }

        /// <summary>
        /// 启用可视化特效
        /// </summary>
        /// <param name="interactData"></param>
        /// <param name="gameObject"></param>
        public abstract void EnableEffect(InteractData interactData, GameObject gameObject);

        /// <summary>
        /// 禁用可视化特效
        /// </summary>
        /// <param name="interactData"></param>
        /// <param name="gameObject"></param>
        public abstract void DisableEffect(InteractData interactData, GameObject gameObject);

        /// <summary>
        /// 特效工作中
        /// </summary>
        /// <param name="interactData"></param>
        /// <param name="gameObject"></param>
        public virtual void EffectWorking(InteractData interactData, GameObject gameObject) { }
    }
}
