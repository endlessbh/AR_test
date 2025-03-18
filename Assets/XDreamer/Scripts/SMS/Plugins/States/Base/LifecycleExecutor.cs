using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.Kernel;

namespace XCSJ.PluginSMS.States.Base
{
    /// <summary>
    /// 生命周期执行器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Name("生命周期执行器")]
    public abstract class LifecycleExecutor : StateComponent, ILifecycleExecutor
    {
        /// <summary>
        /// 执行模式:用于标识执行模式，即在状态组件生命周期中哪些时刻的执行逻辑；
        /// </summary>
        [Name("执行模式")]
        [Tip("用于标识执行模式，即在状态组件生命周期中哪些时刻的执行逻辑；", "It is used to identify the execution mode, that is, the execution logic at which time in the life cycle of the state component;")]
        [EnumPopup]
        public EExecuteMode _executeMode = EExecuteMode.OnEntry;

        /// <summary>
        /// 执行模式
        /// </summary>
        public EExecuteMode executeMode => _executeMode;

        /// <summary>
        /// 判断能否执行
        /// </summary>
        /// <param name="executeMode"></param>
        /// <returns></returns>
        public bool CanExecute(EExecuteMode executeMode) => (this.executeMode & executeMode) != 0;

        private void ExecuteInternal(StateData stateData, EExecuteMode executeMode)
        {
            if (CanExecute(executeMode))
            {
                Execute(stateData, executeMode);
            }
        }

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="stateData"></param>
        /// <param name="executeMode"></param>
        public abstract void Execute(StateData stateData, EExecuteMode executeMode);

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            _executeMode = EExecuteMode.OnEntry;
        }

        /// <summary>
        /// 标记完成
        /// </summary>
        /// <returns></returns>
        public override bool Finished() => true;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="stateData"></param>
        /// <returns></returns>
        public override bool Init(StateData stateData)
        {
            ExecuteInternal(stateData, EExecuteMode.Init);
            return base.Init(stateData);
        }

        /// <summary>
        /// 预进入
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnBeforeEntry(StateData stateData)
        {
            base.OnBeforeEntry(stateData);
            ExecuteInternal(stateData, EExecuteMode.OnBeforeEntry);
        }

        /// <summary>
        /// 进入
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnEntry(StateData stateData)
        {
            base.OnEntry(stateData);
            ExecuteInternal(stateData, EExecuteMode.OnEntry);
        }

        /// <summary>
        /// 已进入
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnAfterEntry(StateData stateData)
        {
            base.OnAfterEntry(stateData);
            ExecuteInternal(stateData, EExecuteMode.OnAfterEntry);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnUpdate(StateData stateData)
        {
            base.OnUpdate(stateData);
            ExecuteInternal(stateData, EExecuteMode.OnUpdate);
        }

        /// <summary>
        /// 预退出
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnBeforeExit(StateData stateData)
        {
            base.OnBeforeExit(stateData);
            ExecuteInternal(stateData, EExecuteMode.OnBeforeExit);
        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnExit(StateData stateData)
        {
            base.OnExit(stateData);
            ExecuteInternal(stateData, EExecuteMode.OnExit);
        }

        /// <summary>
        /// 已退出
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnAfterExit(StateData stateData)
        {
            base.OnAfterExit(stateData);
            ExecuteInternal(stateData, EExecuteMode.OnAfterExit);
        }
    }

    /// <summary>
    /// 生命周期执行器泛型类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class LifecycleExecutor<T> : LifecycleExecutor where T : LifecycleExecutor<T>
    {
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
    }

    /// <summary>
    /// 生命周期执行器接口
    /// </summary>
    public interface ILifecycleExecutor
    {
        /// <summary>
        /// 执行模式
        /// </summary>
        EExecuteMode executeMode { get; }

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="data">状态数据</param>
        /// <param name="executeMode">执行模式</param>
        void Execute(StateData stateData, EExecuteMode executeMode);
    }

    /// <summary>
    /// 执行模式
    /// </summary>
    [Name("执行模式")]
    [Flags]
    public enum EExecuteMode
    {
        /// <summary>
        /// 初始化
        /// </summary>
        [Name("初始化")]
        [EnumFieldName("初始化")]
        [Tip("初始化时执行", "Execute on initialization")]
        Init = 1 << 0,

        /// <summary>
        /// 预进入
        /// </summary>
        [Name("预进入")]
        [EnumFieldName("预进入")]
        [Tip("预进入时执行", "Execute on pre entry")]
        OnBeforeEntry = 1 << 1,

        /// <summary>
        /// 进入
        /// </summary>
        [Name("进入")]
        [EnumFieldName("进入")]
        [Tip("进入时执行", "Execute on entry")]
        OnEntry = 1 << 2,

        /// <summary>
        /// 已进入
        /// </summary>
        [Name("已进入")]
        [EnumFieldName("已进入")]
        [Tip("已进入时执行", "Execute when entered")]
        OnAfterEntry = 1 << 3,

        /// <summary>
        /// 更新
        /// </summary>
        [Name("更新")]
        [EnumFieldName("更新")]
        [Tip("更新时执行", "Execute on update")]
        OnUpdate = 1 << 4,

        /// <summary>
        /// 预退出
        /// </summary>
        [Name("预退出")]
        [EnumFieldName("预退出")]
        [Tip("预退出时执行", "Execute on pre exit")]
        OnBeforeExit = 1 << 5,

        /// <summary>
        /// 退出
        /// </summary>
        [Name("退出")]
        [EnumFieldName("退出")]
        [Tip("退出时执行", "Execute on exit")]
        OnExit = 1 << 6,

        /// <summary>
        /// 已退出
        /// </summary>
        [Name("已退出")]
        [EnumFieldName("已退出")]
        [Tip("已退出时执行", "Execute when exited")]
        OnAfterExit = 1 << 7,
    }
}
