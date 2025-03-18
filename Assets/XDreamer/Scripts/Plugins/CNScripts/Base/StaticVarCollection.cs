using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.CNScripts;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.Scripts;

namespace XCSJ.Extension.CNScripts.Base
{
    /// <summary>
    /// 静态变量集合
    /// </summary>
    [Name(Title)]
    [Tip("应用程序运行期内均有效的变量；即变量的作用域在整个应用程序的运行期均有效；不会随着场景切换，而导致变量失效；应用程序退出后，变量信息会丢失；本变量存储在应用程序运行期的内存中；", "Variables that are valid during the operation of the application; That is, the scope of the variable is valid throughout the running period of the application; It will not cause variable failure with scene switching; After the application exits, the variable information will be lost; This variable is stored in the memory during the running period of the application;")]
    [Tool(CNScriptHelper.Title, nameof(ScriptManager))]
    [RequireComponent(typeof(ScriptManager))]
    [RequireManager(typeof(ScriptManager))]
    [DisallowMultipleComponent]
    [ExecuteInEditMode]
    public class StaticVarCollection : BaseVarCollection
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "静态变量集合";

        #region Unity方法重写

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            _varCollection.OnEnable();
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            _varCollection.OnDisable();
        }

        private void Awake() => _varCollection.OnInit();

        private void OnDestroy() => _varCollection.OnRelease();

        #endregion

        #region 旧版序列化对象

        /// <summary>
        /// 静态变量列表
        /// </summary>
        [Name("静态变量列表")]
        [HideInInspector]
        public List<CustomVariable> _variableList = new List<CustomVariable>();

        #endregion

        #region IVarCollectionHost

        /// <summary>
        /// 静态变量集合
        /// </summary>
        [Name("静态变量集合")]
        [Tip("应用程序运行期内均有效的变量；即变量的作用域在整个应用程序的运行期均有效；不会随着场景切换，而导致变量失效；应用程序退出后，变量信息会丢失；本变量存储在应用程序运行期的内存中；", "Variables that are valid during the operation of the application; That is, the scope of the variable is valid throughout the running period of the application; It will not cause variable failure with scene switching; After the application exits, the variable information will be lost; This variable is stored in the memory during the running period of the application;")]
        public StaticVarCollectionData _varCollection = new StaticVarCollectionData();

        /// <summary>
        /// 变量集合
        /// </summary>
        public override IVarCollection varCollection => _varCollection;

        #endregion

        #region ISerializationCallbackReceiver

        /// <summary>
        /// 当反序列化之后回调
        /// </summary>
        public override void OnAfterDeserialize()
        {
            base.OnAfterDeserialize();
            _varCollection.LegacyUpgrade(_variableList);
            _varCollection.SetVarCollectionHost(this);
            _varCollection.ListToDictionary();
        }

        #endregion
    }

    /// <summary>
    /// 静态变量字典型变量集合
    /// </summary>
    public abstract class StaticVarDictionary_VarCollection : CustomVarCollection
    {
        /// <summary>
        /// 列表到字典：不清空字典、无则添加、有则不处理
        /// </summary>
        public override void ListToDictionary() => ListToDictionary(false, false);

        /// <summary>
        /// 不将字典数据转到列表中
        /// </summary>
        public override void DictionaryToList()
        {
            //base.DictionaryToList();
        }

        /// <summary>
        /// 刷新变量字典
        /// </summary>
        public override void RefreshVarDictionary()
        {
            //base.RefreshVarDictionary();
            ListToDictionary(false, false);
        }
    }

    /// <summary>
    /// 静态变量集合数据
    /// </summary>
    [Serializable]
    public sealed class StaticVarCollectionData : StaticVarDictionary_VarCollection
    {
        /// <summary>
        /// 变量作用域
        /// </summary>
        public override EVarScope varScope => EVarScope.Static;

        /// <summary>
        /// 变量字典
        /// </summary>
        public static VarDictionary _varDictionary = new VarDictionary();

        /// <summary>
        /// 变量字典
        /// </summary>
        public override VarDictionary varDictionary => _varDictionary;
    }
}
