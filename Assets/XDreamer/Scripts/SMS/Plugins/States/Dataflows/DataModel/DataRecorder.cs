using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Attributes;
using XCSJ.Extension.Base.Recorders;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.Base;
using XCSJ.PluginSMS.Kernel;

namespace XCSJ.PluginSMS.States.Dataflows.DataModel
{
    /// <summary>
    /// 数据记录器
    /// </summary>
    public abstract class DataRecorder : StateComponent
    {
        /// <summary>
        /// 数据记录模式:用于标识数据记录模式，即记录状态组件生命周期中哪些时刻的数据；
        /// </summary>
        [Name("数据记录模式")]
        [Tip("用于标识数据记录模式，即记录状态组件生命周期中哪些时刻的数据；", "It is used to identify the data recording mode, that is, to record the data at which time in the life cycle of the state component;")]
        [EnumPopup]
        public EDataRecordMode dataRecordMode = EDataRecordMode.Default;

        /// <summary>
        /// 数据记录作用域
        /// </summary>
        [Name("数据记录作用域")]
        [Tip("数据记录后可生效的作用域", "Scope that can take effect after data recording")]
        [EnumPopup]
        public EDataRecordScope _dataRecordScope = EDataRecordScope.Scene;

        /// <summary>
        /// 获取可恢复的数据记录器对象
        /// </summary>
        /// <param name="dataRecordMode"></param>
        /// <param name="dataRecordScope"></param>
        /// <returns></returns>
        public abstract IRecoverableDataRecorder GetRecoverableDataRecorder(EDataRecordMode dataRecordMode, EDataRecordScope dataRecordScope = EDataRecordScope.Scene);

        /// <summary>
        /// 恢复
        /// </summary>
        /// <param name="dataRecoveryMode"></param>
        public virtual void Recovery(EDataRecordMode dataRecoveryMode, EDataRecoveryRule dataRecoveryRule = EDataRecoveryRule.All, string dataRecoveryRuleValue = "", EDataRecordScope dataRecordScope = EDataRecordScope.Scene)
        {
            var recorder = GetRecoverableDataRecorder(dataRecoveryMode, dataRecordScope);
            if (recorder != null)
            {
                recorder.Recovery(dataRecoveryRule, dataRecoveryRuleValue);
            }
        }

        /// <summary>
        /// 是否处于完成态
        /// </summary>
        /// <returns></returns>
        public override bool Finished() => true;
    }

    /// <summary>
    /// 数据记录作用域
    /// </summary>
    [Name("数据记录作用域")]
    public enum EDataRecordScope
    {
        /// <summary>
        /// 场景
        /// </summary>
        [Name("场景")]
        [Tip("记录的数据信息仅在本场景内有效，场景切换后数据失效；数据信息存储在基于场景的全局变量中；")]
        Scene,

        /// <summary>
        /// 静态
        /// </summary>
        [Name("静态")]
        [Tip("记录的数据信息在本程序执行期间有效，场景切换后数据有效，在程序关闭后失效；数据信息存储在静态变量中；")]
        Static,

        /// <summary>
        /// App
        /// </summary>
        [Name("App")]
        [Tip("记录的数据信息在本程序执行期间有效，场景切换、程序关闭后数据有效，在程序卸载后失效；数据信息存储在物理磁盘文件中；")]
        App,
    }

    /// <summary>
    /// 可恢复的数据记录器
    /// </summary>
    public interface IRecoverableDataRecorder : IRecorder
    {
        /// <summary>
        /// 恢复
        /// </summary>
        /// <param name="dataRecoveryRule"></param>
        /// <param name="dataRecoveryRuleValue"></param>
        void Recovery(EDataRecoveryRule dataRecoveryRule, string dataRecoveryRuleValue);
    }

    /// <summary>
    /// 可恢复的数据记录器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRecoverableDataRecorder<T> : IRecoverableDataRecorder, IRecorder<T>
    {
    }

    /// <summary>
    /// 数据记录器泛型类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TRecorder"></typeparam>
    public  class DataRecorder<T, TRecorder> : DataRecorder
        where T : DataRecorder<T, TRecorder>
        where TRecorder : class, IRecoverableDataRecorder<T>, new()
    {
        /// <summary>
        /// 自身对象
        /// </summary>
        public T self => (T)this;

        /// <summary>
        /// 场景作用域的记录器对象
        /// </summary>
        private Dictionary<EDataRecordMode, TRecorder> sceneRecorder { get; } = new Dictionary<EDataRecordMode, TRecorder>();

        /// <summary>
        /// 静态作用域的记录器对象
        /// </summary>
        private static Dictionary<string, Dictionary<string, Dictionary<EDataRecordMode, string>>> staticRecorder { get; } = new Dictionary<string, Dictionary<string, Dictionary<EDataRecordMode, string>>>();

        /// <summary>
        /// 创建携带当前状态组件的普通状态
        /// </summary>
        /// <param name="obj">获取状态集接口对象，即新创建的普通状态会添加在本对象指定的对象集中</param>
        /// <param name="init">初始化方法</param>
        /// <param name="stateComponentTypes">其它需同步添加的状态组件类型</param>
        /// <returns>成功返回新创建的普通状态；否则返回null</returns>
        public static NormalState CreateNormalState(IGetStateCollection obj, Action<NormalState> init = null, params Type[] stateComponentTypes)
        {
            return obj.CreateNormalState<T>(init, stateComponentTypes);
        }

        /// <summary>
        /// 创建携带当前状态组件的子状态机
        /// </summary>
        /// <param name="obj">获取状态集接口对象，即新创建的子状态机会添加在本对象指定的对象集中</param>
        /// <param name="init">初始化方法</param>
        /// <param name="stateComponentTypes">其它需同步添加的状态组件类型</param>
        /// <returns>成功返回新创建的普通状态；否则返回null</returns>
        public static SubStateMachine CreateSubStateMachine(IGetStateCollection obj, Action<SubStateMachine> init = null, params Type[] stateComponentTypes)
        {
            return obj.CreateSubStateMachine<T>(init, stateComponentTypes);
        }

        /// <summary>
        /// 获取可恢复的数据记录器对象
        /// </summary>
        /// <param name="dataRecordMode"></param>
        /// <param name="dataRecordScope"></param>
        /// <returns></returns>
        public override IRecoverableDataRecorder GetRecoverableDataRecorder(EDataRecordMode dataRecordMode, EDataRecordScope dataRecordScope = EDataRecordScope.Scene)
        {
            switch (dataRecordScope)
            {
                case EDataRecordScope.Scene:
                    {
                        sceneRecorder.TryGetValue(dataRecordMode, out var recorder);
                        return recorder;
                    }
                case EDataRecordScope.Static:
                    {
                        var scenePath = parent.hostComponent.gameObject.scene.path;
                        if (!staticRecorder.TryGetValue(scenePath, out var sceneDictionary)) break;

                        var thisKey = this.GetNamePath();
                        if (!sceneDictionary.TryGetValue(thisKey, out var dictionary)) break;

                        dictionary.TryGetValue(dataRecordMode, out var recorder);
                        return JsonHelper.ToObject<TRecorder>(recorder);
                    }
                case EDataRecordScope.App:
                    {
                        var scenePath = parent.hostComponent.gameObject.scene.path;
                        var thisKey = this.GetNamePath();

                        var key = scenePath + ":" + thisKey + ":" + dataRecordMode;

                        if (!PlayerPrefs.HasKey(key)) break;
                        var value = PlayerPrefs.GetString(key);
                        return JsonHelper.ToObject<TRecorder>(value);
                    }
            }
            return default;
        }

        /// <summary>
        /// 记录
        /// </summary>
        /// <param name="dataRecordMode"></param>
        protected void Record(EDataRecordMode dataRecordMode)
        {
            if ((this.dataRecordMode & dataRecordMode) == 0) return;

            switch (_dataRecordScope)
            {
                case EDataRecordScope.Scene:
                    {
                        var recorder = new TRecorder();
                        recorder.Record(self);

                        sceneRecorder[dataRecordMode] = recorder;
                        break;
                    }
                case EDataRecordScope.Static:
                    {
                        var recorder = new TRecorder();
                        recorder.Record(self);

                        var scenePath = parent.hostComponent.gameObject.scene.path;
                        if (!staticRecorder.TryGetValue(scenePath, out var sceneDictionary))
                        {
                            staticRecorder[scenePath] = sceneDictionary = new Dictionary<string, Dictionary<EDataRecordMode, string>>();
                        }
                        var thisKey = this.GetNamePath();
                        if (!sceneDictionary.TryGetValue(thisKey, out var dictionary))
                        {
                            sceneDictionary[thisKey] = dictionary = new Dictionary<EDataRecordMode, string>();
                        }

                        dictionary[dataRecordMode] = JsonHelper.ToJson(recorder);
                        break;
                    }
                case EDataRecordScope.App:
                    {
                        var recorder = new TRecorder();
                        recorder.Record(self);

                        var scenePath = parent.hostComponent.gameObject.scene.path;
                        var thisKey = this.GetNamePath();

                        var key = scenePath + ":" + thisKey + ":" + dataRecordMode;
                        var value = JsonHelper.ToJson(recorder);

                        PlayerPrefs.SetString(key, value);
                        break;
                    }
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override bool Init(StateData data)
        {
            Record(EDataRecordMode.Init);
            return base.Init(data);
        }

        /// <summary>
        /// 进入激活态前回调
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnBeforeEntry(StateData stateData)
        {
            base.OnBeforeEntry(stateData);
            Record(EDataRecordMode.OnBeforeEntry);
        }

        /// <summary>
        /// 进入激活态回调
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnEntry(StateData stateData)
        {
            base.OnEntry(stateData);
            Record(EDataRecordMode.OnEntry);
        }

        /// <summary>
        /// 进入激活态后回调
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnAfterEntry(StateData stateData)
        {
            base.OnAfterEntry(stateData);
            Record(EDataRecordMode.OnAfterEntry);
        }

        /// <summary>
        /// 更新时回调
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnUpdate(StateData stateData)
        {
            base.OnUpdate(stateData);
            Record(EDataRecordMode.OnUpdate);
        }

        /// <summary>
        /// 退出激活态前回调
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnBeforeExit(StateData stateData)
        {
            base.OnBeforeExit(stateData);
            Record(EDataRecordMode.OnBeforeExit);
        }

        /// <summary>
        /// 退出激活态回调
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnExit(StateData stateData)
        {
            base.OnExit(stateData);
            Record(EDataRecordMode.OnExit);
        }

        /// <summary>
        /// 退出激活态后回调
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnAfterExit(StateData stateData)
        {
            base.OnAfterExit(stateData);
            Record(EDataRecordMode.OnAfterExit);
        }

        /// <summary>
        /// 重置时调用
        /// </summary>
        /// <param name="data"></param>
        public override void Reset(ResetData data)
        {
            base.Reset(data);

            switch (data.dataRule)
            {
                case EDataRule.Init:
                    {
                        Recovery(EDataRecordMode.Init);
                        break;
                    }
                case EDataRule.Entry:
                    {
                        Recovery(EDataRecordMode.OnEntry);
                        break;
                    }
            }
        }
    }

    /// <summary>
    /// 数据记录模式
    /// </summary>
    [Name("数据记录模式")]
    [Flags]
    public enum EDataRecordMode
    {
        /// <summary>
        /// 初始化
        /// </summary>
        [Name("初始化")]
        [EnumFieldName("初始化")]
        Init = 1 << 0,

        /// <summary>
        /// 预进入
        /// </summary>
        [Name("预进入")]
        [EnumFieldName("预进入")]
        OnBeforeEntry = 1 << 1,

        /// <summary>
        /// 进入
        /// </summary>
        [Name("进入")]
        [EnumFieldName("进入")]
        OnEntry = 1 << 2,

        /// <summary>
        /// 已进入
        /// </summary>
        [Name("已进入")]
        [EnumFieldName("已进入")]
        OnAfterEntry = 1 << 3,

        /// <summary>
        /// 更新
        /// </summary>
        [Name("更新")]
        [EnumFieldName("更新")]
        OnUpdate = 1 << 4,

        /// <summary>
        /// 预退出
        /// </summary>
        [Name("预退出")]
        [EnumFieldName("预退出")]
        OnBeforeExit = 1 << 5,

        /// <summary>
        /// 退出
        /// </summary>
        [Name("退出")]
        [EnumFieldName("退出")]
        OnExit = 1 << 6,

        /// <summary>
        /// 已退出
        /// </summary>
        [Name("已退出")]
        [EnumFieldName("已退出")]
        OnAfterExit = 1 << 7,

        /// <summary>
        /// 默认:包括初始化、进入、退出
        /// </summary>
        [Name("默认")]
        [Tip("包括初始化、进入、退出", "Including initialization, entry and exit")]
        [EnumFieldName("默认")]
        Default = Init | OnEntry | OnExit,
    }
}
