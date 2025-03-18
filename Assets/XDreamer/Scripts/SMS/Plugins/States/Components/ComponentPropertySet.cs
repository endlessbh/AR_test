using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;
using XCSJ.PluginSMS.States.GameObjects;

namespace XCSJ.PluginSMS.States.Components
{
    /// <summary>
    /// 组件属性设置
    /// </summary>
    [RequireComponent(typeof(GameObjectSet))]
    public abstract class ComponentPropertySet<T, TComponent> : BasePropertySet<T> 
        where T : ComponentPropertySet<T, TComponent>
        where TComponent : UnityEngine.Component
    {
        /// <summary>
        /// 包含子游戏对象
        /// </summary>
        [Name("包含子游戏对象")]
        public bool _includeChildren = true;

        /// <summary>
        /// 游戏对象集合
        /// </summary>
        protected GameObjectSet gameObjectSet = null;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="stateData"></param>
        /// <returns></returns>
        public override bool Init(StateData stateData)
        {
            gameObjectSet = GetComponent<GameObjectSet>(true);
            return base.Init(stateData);
        }

        /// <summary>
        /// 执行设置
        /// </summary>
        /// <param name="stateData"></param>
        /// <param name="executeMode"></param>
        public override void Execute(StateData stateData, EExecuteMode executeMode)
        {
            foreach (var c in GetTargetComponents())
            {
                SetComponentProperty(c);
            }
        }

        /// <summary>
        /// 设置组件属性
        /// </summary>
        /// <param name="component"></param>
        protected abstract void SetComponentProperty(TComponent component);

        /// <summary>
        /// 获取模版类组件列
        /// </summary>
        /// <returns></returns>
        protected List<TComponent> GetTargetComponents()
        {
            var components = new List<TComponent>();
            foreach (var go in gameObjectSet.objects)
            {
                if (_includeChildren)
                {
                    components.AddRange(go.GetComponentsInChildren<TComponent>());
                }
                else
                {
                    components.Add(go.GetComponent<TComponent>());
                }
            }
            return components;
        }

    }
}
