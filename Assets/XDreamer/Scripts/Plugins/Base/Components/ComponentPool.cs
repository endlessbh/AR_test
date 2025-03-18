using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;

namespace XCSJ.Extension.Base.Components
{
    /// <summary>
    /// 组件缓存池列表
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ComponentPool<T> : InteractProvider where T : Component
    {
        /// <summary>
        /// 组件模版
        /// </summary>
        [Name("模版")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [Readonly(EEditorMode.Runtime)]
        public T _template;

        /// <summary>
        /// 组件池
        /// </summary>
        protected WorkObjectPool<T> componentPool = new WorkObjectPool<T>();

        /// <summary>
        /// 唤醒
        /// </summary>
        protected virtual void Awake()
        {
            componentPool.Init(DefaultAlloc, DefaultInit, DefaultReset, c => c);
        }

        /// <summary>
        /// 缺省分配
        /// </summary>
        /// <returns></returns>
        protected virtual T DefaultAlloc()
        {
            if (!_template) return default(T);
                
            var clonedGO = _template.gameObject;
            var newGO = GameObject.Instantiate(clonedGO, clonedGO.transform.parent) as GameObject;
            newGO.transform.localScale = clonedGO.transform.localScale;
            return newGO.GetComponent<T>();
        }

        /// <summary>
        /// 缺省初始化 ： 分配后辈初始化
        /// </summary>
        /// <param name="component"></param>
        protected virtual void DefaultInit(T component)
        {
            component.gameObject.SetActive(true);
        }

        /// <summary>
        /// 缺省重置 ：释放时被重置
        /// </summary>
        /// <param name="component"></param>
        protected virtual void DefaultReset(T component)
        {
            component.gameObject.SetActive(false);
        }
    }
}
