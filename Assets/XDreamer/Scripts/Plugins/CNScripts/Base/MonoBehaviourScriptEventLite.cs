using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.CNScripts;

namespace XCSJ.Extension.CNScripts.Base
{
    /// <summary>
    /// MonoBehaviour脚本事件简版类型
    /// </summary>
    [Name("MonoBehaviour脚本事件简版类型")]
    public enum EMonoBehaviourScriptEventLiteType
    {
        /// <summary>
        /// 启动时
        /// </summary>
        [Name("启动时执行")]
        Start = 0,

        /// <summary>
        /// 更新时
        /// </summary>
        [Name("更新时执行")]
        Update,
    }

    /// <summary>
    /// MonoBehaviour脚本事件简版函数
    /// </summary>
    [Name("MonoBehaviour脚本事件简版函数")]
    [Serializable]
    public class MonoBehaviourScriptEventLiteFunction : EnumFunction<EMonoBehaviourScriptEventLiteType> { }

    /// <summary>
    /// MonoBehaviour脚本事件简版函数集合
    /// </summary>
    [Serializable]
    [Name("MonoBehaviour脚本事件简版函数集合")]
    public class MonoBehaviourScriptEventLiteFunctionCollection : EnumFunctionCollection<EMonoBehaviourScriptEventLiteType, MonoBehaviourScriptEventLiteFunction> { }

    /// <summary>
    /// MonoBehaviour脚本事件简版
    /// </summary>
    [Serializable]
    [Name(Title)]
    [DisallowMultipleComponent]
    [AddComponentMenu(CNScriptHelper.CNScriptMenu + Title)]
    public class MonoBehaviourScriptEventLite : BaseScriptEvent<EMonoBehaviourScriptEventLiteType, MonoBehaviourScriptEventLiteFunction, MonoBehaviourScriptEventLiteFunctionCollection>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "MonoBehaviour脚本事件简版";

        /// <summary>
        /// 启动
        /// </summary>
        protected override void Start()
        {
            base.Start();
            ExecuteScriptEvent(EMonoBehaviourScriptEventLiteType.Start);
        }

        /// <summary>
        /// 更新（Update is called once per frame)
        /// </summary>
        protected virtual void Update()
        {
            ExecuteScriptEvent(EMonoBehaviourScriptEventLiteType.Update);
        }
    }
}
