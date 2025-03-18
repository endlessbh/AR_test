using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.CNScripts;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.Scripts;

namespace XCSJ.Extension.CNScripts.Base
{
    /// <summary>
    /// App变量集合
    /// </summary>
    [Name(Title)]
    [Tip("应用程序运行期、非运行期均有效的变量；即变量的作用域在整个应用程序安装后不论运行与否均有效；不会因应用程序重启、场景切换等情况导致失效；本变量会序列化保存在物理磁盘中；", "Variables that are valid during the running and non running periods of the application; That is, the scope of the variable is valid after the entire application is installed, whether running or not; It will not fail due to application restart, scene switching, etc; This variable will be serialized and saved in the physical disk;")]
    [Tool(CNScriptHelper.Title, nameof(ScriptManager))]
    [RequireComponent(typeof(ScriptManager))]
    [DisallowMultipleComponent]
    [ExecuteInEditMode]
    public class AppVarCollection : BaseVarCollection
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "App变量集合";

        #region 保存与加载机制

        /// <summary>
        /// 保存规则
        /// </summary>
        [Name("保存规则")]
        [Tip("对App变量保存时会影响程序性能", "Saving app variables will affect program performance")]
        public enum ESaveRule
        {
            /// <summary>
            /// 无:不处理
            /// </summary>
            [Name("无")]
            [Tip("不处理", "Do not handle")]
            None,

            /// <summary>
            /// 当任意修改:当变量发生任意修改时保存
            /// </summary>
            [Name("当任意修改")]
            [Tip("当变量发生任意修改时保存", "Save in case of any modification of variables")]
            OnAnyChange,

            /// <summary>
            /// 当修改计数:当变量发生修改的次数超过指定值时保存
            /// </summary>
            [Name("当修改计数")]
            [Tip("当变量发生修改的次数超过指定值时保存", "Save when the number of changes to the variable exceeds the specified value")]
            OnChangeCount,

            /// <summary>
            /// 当延后更新:当延后更新时保存
            /// </summary>
            [Name("当延后更新")]
            [Tip("当延后更新时保存", "Save when updating later")]
            OnLateUpdate,

            /// <summary>
            /// 当禁用:当对象禁用时保存
            /// </summary>
            [Name("当禁用")]
            [Tip("当对象禁用时保存", "Save when object is disabled")]
            OnDisable,

            /// <summary>
            /// 当销毁:当对象销毁时保存
            /// </summary>
            [Name("当销毁")]
            [Tip("当对象销毁时保存", "Save when object is destroyed")]
            OnDestroy,

            /// <summary>
            /// 定时的:每过指定时间定时保存一次
            /// </summary>
            [Name("定时的")]
            [Tip("每过指定时间定时保存一次", "Save it regularly after a specified time")]
            Timed,

            /// <summary>
            /// 综合的:在综合考量修改计数、定时、对象禁用等情况下的保存机制
            /// </summary>
            [Name("综合的")]
            [Tip("在综合考量修改计数、定时、对象禁用等情况下的保存机制", "Saving mechanism under the condition of modifying count, timing, object disabling, etc")]
            Overall,
        }

        /// <summary>
        /// 保存规则
        /// </summary>
        [Name("保存规则")]
        [EnumPopup]
        public ESaveRule _saveRule = ESaveRule.OnAnyChange;

        /// <summary>
        /// 修改计数:当变量发生修改的次数超过指定值时保存
        /// </summary>
        [Name("修改计数")]
        [Tip("当变量发生修改的次数超过指定值时保存", "Saving mechanism under the condition of modifying count, timing, object disabling, etc")]
        public int _changeCount = 10;

        private int counted = 0;

        /// <summary>
        /// 保存时间间隔:定时保存App变量的时间间隔；单位：秒；
        /// </summary>
        [Name("保存时间间隔")]
        [Tip("定时保存App变量的时间间隔；单位：秒；", "Time interval for regularly saving app variables; Unit: second;")]
        [Range(0.01f, 300)]
        public float _saveInteval = 3;

        private float passingTime = 0;

        private bool SaveIfTimed()
        {
            passingTime += Time.deltaTime;
            if (passingTime >= _saveInteval)
            {
                Save();
                return true;
            }
            return false;
        }

        private bool SaveIfCountd()
        {
            if (counted >= _changeCount)
            {
                Save();
                return true;
            }
            return false;
        }

        private void Save(ESaveRule saveRule)
        {
            if (saveRule == this._saveRule) Save();
        }

        /// <summary>
        /// 保存
        /// </summary>
        public void Save()
        {
            counted = 0;
            passingTime = 0;

            _varCollection.Save();
        }

        private void Load() => _varCollection.Load(this);

        /// <summary>
        /// 重新加载
        /// </summary>
        public void Reload() => _varCollection.Reload(this);

        private void OnHierarchyVarChanged(IHierarchyVar hierarchyVar)
        {
            if (hierarchyVar.varScope == varCollection.varScope)
            {
                counted++;
                Save(ESaveRule.OnAnyChange);
            }
        }

        #endregion

        #region Unity方法重写

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            Load();
            counted = 0;
            passingTime = 0;
            HierarchyVarEvent.onChanged += OnHierarchyVarChanged;
            _varCollection.OnEnable();
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            HierarchyVarEvent.onChanged -= OnHierarchyVarChanged;
            Save(ESaveRule.OnDisable);
            Save(ESaveRule.Overall);
            _varCollection.OnDisable();
        }

        private void Awake()
        {
            Load();
            _varCollection.OnInit();
        }

        private void OnDestroy()
        {
            Save(ESaveRule.OnDestroy);
            _varCollection.OnRelease();
        }

        private void LateUpdate()
        {
            switch (_saveRule)
            {
                case ESaveRule.OnLateUpdate:
                    {
                        Save();
                        break;
                    }
                case ESaveRule.Timed:
                    {
                        SaveIfTimed();
                        break;
                    }
                case ESaveRule.OnChangeCount:
                    {
                        SaveIfCountd();
                        break;
                    }
                case ESaveRule.Overall:
                    {
                        if (SaveIfTimed() || SaveIfCountd()) { }
                        break;
                    }
            }
        }

        #endregion

        #region 旧版序列化对象

        /// <summary>
        /// App变量列表
        /// </summary>
        [Name("App变量列表")]
        [HideInInspector]
        public List<CustomVariable> _variableList = new List<CustomVariable>();

        #endregion

        #region IVarCollectionHost

        /// <summary>
        /// App变量集合
        /// </summary>
        [Name("App变量集合")]
        [Tip("应用程序运行期、非运行期均有效的变量；即变量的作用域在整个应用程序安装后不论运行与否均有效；不会因应用程序重启、场景切换等情况导致失效；本变量会序列化保存在物理磁盘中；", "Variables that are valid during the running and non running periods of the application; That is, the scope of the variable is valid after the entire application is installed, whether running or not; It will not fail due to application restart, scene switching, etc; This variable will be serialized and saved in the physical disk;")]
        public AppVarCollectionData _varCollection = new AppVarCollectionData();

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
    /// App变量集合数据
    /// </summary>
    [Serializable]
    [Name("App变量集合数据")]
    public sealed class AppVarCollectionData : StaticVarDictionary_VarCollection
    {
        /// <summary>
        /// 变量作用域
        /// </summary>
        public override EVarScope varScope => EVarScope.App;

        /// <summary>
        /// 变量字典
        /// </summary>
        public static VarDictionary _varDictionary = new VarDictionary();

        /// <summary>
        /// 变量字典
        /// </summary>
        public override VarDictionary varDictionary => _varDictionary;

        /// <summary>
        /// 键：用于序列化存储的键名
        /// </summary>
        public const string Key = nameof(XDreamer) + "_" + nameof(AppVarCollection);

        /// <summary>
        /// 保存
        /// </summary>
        public void Save()
        {
            try
            {
                var appVariables = _varDictionary.Values.ToList();
                PlayerPrefs.SetString(Key, JsonHelper.ToJson(appVariables));
                PlayerPrefs.Save();

                //Debug.Log("AppVarCollection Save : " + JsonHelper.ToJson(appVariables));
            }
            //catch (Exception ex) { Debug.LogException(ex); }
            catch { }
        }

        private static bool loaded = false;

        /// <summary>
        /// 加载：加载到字典中
        /// </summary>
        public void Load(IVarCollectionHost varCollectionHost)
        {
            //Debug.Log("AppVarCollection Load 0: " + loaded);
            if (loaded) return;
            try
            {
                var value = PlayerPrefs.GetString(Key);
                var appVariables = JsonHelper.ToObject<List<CustomVariable>>(value);
                foreach (var variable in appVariables)
                {
                    variable.SetVarCollectionHost(varCollectionHost);
                    _varDictionary[variable.name] = variable;
                }
                this.SetVarCollectionHost(varCollectionHost);
                //Debug.Log("AppVarCollection Load: " + JsonHelper.ToJson(value));
            }
            //catch (Exception ex) { Debug.LogException(ex); }
            catch { }
            finally
            {
                loaded = true;
            }
        }

        /// <summary>
        /// 重新加载
        /// </summary>
        /// <param name="varCollectionHost"></param>
        public void Reload(IVarCollectionHost varCollectionHost)
        {
            loaded = false;
            _varDictionary.Clear();
            Load(varCollectionHost);
        }
    }
}
