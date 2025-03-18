using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using XCSJ.Attributes;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.Scripts;

namespace XCSJ.Extension.CNScripts.Base
{
    /// <summary>
    /// 基础变量集合
    /// </summary>
    [RequireManager(typeof(ScriptManager))]
    [Owner(typeof(ScriptManager))]
    [XCSJ.Attributes.Icon(EIcon.Variable)]
    public abstract class BaseVarCollection : InteractProvider, IVarCollectionHost, ISerializationCallbackReceiver
    {
        #region Unity方法重写

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            var manager = GetComponent<ScriptManager>();
            if (manager)
            {
                manager.RegisterVarCollection(varCollection);
            }
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            var manager = GetComponent<ScriptManager>();
            if (manager)
            {
                manager.UnegisterVarCollection(varCollection);
            }
        }

        #endregion

        #region ISerializationCallbackReceiver

        /// <summary>
        /// 当序列化之前回调
        /// </summary>
        public virtual void OnBeforeSerialize() { }

        /// <summary>
        /// 当反序列化之后回调
        /// </summary>
        public virtual void OnAfterDeserialize() { }

        #endregion

        #region IVarCollectionHost

        /// <summary>
        /// 变量集合
        /// </summary>
        public abstract IVarCollection varCollection { get; }

        /// <summary>
        /// 获取宿主上下文信息
        /// </summary>
        /// <returns></returns>
        public string GetHostContext() => string.Format("组件型[{0}]({1})对象[{2}]", CommonFun.Name(GetType()), GetType().FullName, CommonFun.GameObjectToString(gameObject));

        #endregion
    }
}
