using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Extension.Base.Inputs;
using XCSJ.Extension.Interactions.Base;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginTools.SelectionUtils;

namespace XCSJ.PluginTools.Effects
{
    /// <summary>
    /// 特效枚举
    /// </summary>
    public enum EEffectCmd
    {
        [Name("启用")]
        Enable,

        [Name("禁用")]
        Disable,

        [Name("工作中")]
        Working,
    }

    /// <summary>
    /// 特效命令
    /// </summary>
    [Serializable]
    public class EffectCmd : Cmd<EEffectCmd> { }

    /// <summary>
    /// 特效命令列表
    /// </summary>
    [Serializable]
    public class EffectCmds : Cmds<EEffectCmd, EffectCmd> { }

    /// <summary>
    /// 特效控制器:由命令驱动将本组件所在游戏对象上的特效组件应用到可交互对象上；当可交互对象上有【特效交互器】组件时，则特效由该组件实现。
    /// </summary>
    [Name("特效控制器")]
    [DisallowMultipleComponent]
    [Tool(InteractionCategory.Effect, rootType = typeof(ToolsManager), index = InteractionCategory.InteractableVisualIndex)]
    [RequireManager(typeof(ToolsManager))]
    [Owner(typeof(ToolsManager))]
    public class EffectController : Interactor
    {
        /// <summary>
        /// 特效命令列表
        /// </summary>
        [Name("特效命令列表")]
        [OnlyMemberElements]
        public EffectCmds _effectCmds = new EffectCmds();

        /// <summary>
        /// 命令列表
        /// </summary>
        public override List<string> cmds => _effectCmds.cmdNames;

        /// <summary>
        /// 重置
        /// </summary>
        public void Reset() => _effectCmds.Reset();

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            // 禁用特效
            foreach (var e in effects)
            {
                var go = GameObjectEffectCache.FindGameObject(e);
                if (go)
                {
                    var entity = go.GetComponentInChildren<InteractableEntity>();
                    if (entity)
                    {
                        TryInteract(_effectCmds.GetCmdName(EEffectCmd.Disable), out _, entity);
                    }
                }
            }
        }

        /// <summary>
        /// 进入交互更新器
        /// </summary>
        protected override void OnEntryInteractUpdater()
        {
            base.OnEntryInteractUpdater();

            foreach (var ve in GetComponents<BaseEffect>())
            {
                if (ve is IColorEffect colorEffect)
                {
                    if (!colorEffects.Contains(colorEffect)) colorEffects.Add(colorEffect);
                }
                effects.Add(ve);
            }
        }

        /// <summary>
        /// 退出交互更新器
        /// </summary>
        protected override void OnExitInteractUpdater()
        {
            base.OnExitInteractUpdater();

            colorEffects.Clear();
            effects.Clear();
        }

        /// <summary>
        /// 可交互
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        public override bool CanInteract(InteractData interactData) => true;// 特效可无条件触发

        /// <summary>
        /// 响应交互
        /// </summary>
        /// <param name="interactData"></param>
        protected override EInteractResult OnInteract(InteractData interactData)
        {
            if (_effectCmds.TryGetECmd(interactData.cmdName, out var eCmd))
            {
                switch (eCmd)
                {
                    case EEffectCmd.Enable:
                        {
                            InteractableEntity entity = interactData.interactable as InteractableEntity;
                            if (!entity)
                            {
                                entity = interactData.interactor.GetComponent<InteractableEntity>();
                            }
                            if (entity)
                            {
                                var effector = entity.GetComponent<EffectController>();
                                if (effector)
                                {
                                    if (effector.TryInteract(interactData, out _))
                                    {
                                        return EInteractResult.Finished;
                                    }
                                }
                            }
                            // 缺省使用当前交互器所在效果
                            EffectEnable(interactData, entity ? entity.gameObject : null);
                            return EInteractResult.Finished;
                        }
                    case EEffectCmd.Disable:
                        {
                            InteractableEntity entity = interactData.interactable as InteractableEntity;
                            if (!entity)
                            {
                                entity = interactData.interactor.GetComponent<InteractableEntity>();
                            }
                            if (entity)
                            {
                                var effector = entity.GetComponent<EffectController>();
                                if (effector)
                                {
                                    if (effector.TryInteract(interactData, out _))
                                    {
                                        return EInteractResult.Finished;
                                    }
                                }
                            }

                            // 缺省使用当前交互器所在效果
                            EffectDisable(interactData, entity ? entity.gameObject : null);
                            return EInteractResult.Finished;
                        }
                    case EEffectCmd.Working:
                        {
                            var entity = interactData.interactable as InteractableEntity;

                            EffectWorking(interactData, entity ? entity.gameObject : null);
                            return EInteractResult.Finished;
                        }
                }
            }

            return EInteractResult.Aborted;
        }

        private void EffectEnable(InteractData visualData, GameObject go)
        {
            foreach (var e in effects)
            {
                if (!go || GameObjectEffectCache.AddEffect(go, e))
                {
                    e.EnableEffect(visualData, go);
                }
            }
        }

        private void EffectDisable(InteractData visualData, GameObject go)
        {
            foreach (var e in effects)
            {
                if (!go || GameObjectEffectCache.RemoveEffect(go, e))
                {
                    e.DisableEffect(visualData, go);
                }
            }
        }

        private void EffectWorking(InteractData visualData, GameObject go)
        {
            foreach (var e in effects)
            {
                if (!go || GameObjectEffectCache.Contains(go, e))
                {
                    e.EffectWorking(visualData, go);
                }
            }
        }

        /// <summary>
        /// 特效集合
        /// </summary>
        private HashSet<BaseEffect> effects = new HashSet<BaseEffect>();

        /// <summary>
        /// 颜色特效
        /// </summary>
        private List<IColorEffect> colorEffects = new List<IColorEffect>();

        /// <summary>
        /// 获取第一个颜色特效
        /// </summary>
        /// <returns></returns>
        public IColorEffect GetFirstColorEffect() => colorEffects.FirstOrDefault();

        /// <summary>
        /// 特效控制器颜色
        /// </summary>
        public Color color
        {
            get
            {
                var ce = GetFirstColorEffect();
                return ce != null ? ce.color : Color.black;
            }
            set
            {
                var ce = GetFirstColorEffect();
                if (ce != null) 
                {
                    ce.color = value;
                }
            }
        }
    }

    /// <summary>
    /// 游戏对象特效管理缓存
    /// </summary>
    public static class GameObjectEffectCache
    {
        private static Dictionary<GameObject, List<EffectData>> gameObjectEffectMap { get; set; } = new Dictionary<GameObject, List<EffectData>>();

        private class EffectData
        {
            /// <summary>
            /// 特效类型
            /// </summary>
            public Type type { get; private set; }

            /// <summary>
            /// 特效对象
            /// </summary>
            public BaseEffect effect { get; private set;}

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="type"></param>
            /// <param name="effect"></param>
            public EffectData(Type type, BaseEffect effect)
            {
                this.type = type;
                this.effect = effect;
            }

            /// <summary>
            /// 尝试替换特效
            /// </summary>
            /// <param name="inEffect"></param>
            /// <returns></returns>
            public bool TryReplaceEffect(BaseEffect inEffect)
            {
                if (effect.GetType().Equals(type))
                {
                    if (inEffect.overlayEffect || !effect.overlayEffect)
                    {
                        effect = inEffect;
                        return true;
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// 添加特效：
        /// 1、游戏对象可具有不同类型的特效，但同类型特效值有一个
        /// 2、添加同类特效时，覆盖标记量为True的特效可覆盖游戏对象上之前的特效
        /// 3、覆盖标记量为False的特效在游戏对象没有该类型特效时可添加
        /// 4、覆盖标记量为False的特效在游戏对象具有同类型覆盖标记量为False的特效时，可覆盖
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="baseEffect"></param>
        /// <returns></returns>
        public static bool AddEffect(GameObject gameObject, BaseEffect baseEffect)
        {
            if (!gameObject) return false;

            if (!gameObjectEffectMap.TryGetValue(gameObject, out var list))
            {
                gameObjectEffectMap[gameObject] = list = new List<EffectData>();
            }
            var type = baseEffect.GetType();
            var data = list.Find(item => item.type.Equals(type));
            if (data != null)
            {
                return data.TryReplaceEffect(baseEffect);
            }
            else
            {
                list.Add(new EffectData(type, baseEffect));
                return true;
            }
        }

        /// <summary>
        /// 移除特效
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="effect"></param>
        /// <returns></returns>
        public static bool RemoveEffect(GameObject gameObject, BaseEffect baseEffect)
        {
            if (!gameObject) return false;
            if (gameObjectEffectMap.TryGetValue(gameObject, out var list))
            {
                return list.RemoveAll(data => data.effect == baseEffect) > 0;
            }
            return false;
        }

        /// <summary>
        /// 查找使用特效的游戏对象
        /// </summary>
        /// <param name="effect"></param>
        /// <returns></returns>
        public static GameObject FindGameObject(BaseEffect baseEffect)
        {
            foreach (var item in gameObjectEffectMap)
            {
                if (item.Value.Exists(data => data.effect == baseEffect))
                {
                    return item.Key;
                }
            }
            return default;
        }

        /// <summary>
        /// 缓存中包含特效
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="baseEffect"></param>
        /// <returns></returns>
        public static bool Contains(GameObject gameObject, BaseEffect baseEffect)
        {
            if (gameObjectEffectMap.TryGetValue(gameObject, out var effectDatas))
            {
                return effectDatas.Exists(ed => ed.effect == baseEffect);
            }
            return false;
        }
    }
}
