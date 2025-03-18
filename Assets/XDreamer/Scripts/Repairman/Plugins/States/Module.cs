using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginRepairman.Base;
using XCSJ.PluginSMS;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;

namespace XCSJ.PluginRepairman.States
{
    /// <summary>
    /// 模块
    /// </summary>
    [ComponentMenu(RepairmanHelperExtension.DataModelStateLibName + "/" + Title, typeof(RepairmanManager))]
    [Name(Title, nameof(Module))]
    [XCSJ.Attributes.Icon(EIcon.Engine)]
    [DisallowMultipleComponent]
    [RequireState(typeof(SubStateMachine))]
    [RequireManager(typeof(RepairmanManager))]
    [Tip("模块组件包含多个零件组件的容器。用子状态机实现。是一个数据组织对象、其中数据提供给其他状态组件使用。模块是介于设备与零件之间的中间层概念，模块可以嵌套模块。", "A module assembly is a container that contains multiple part assemblies. It is realized by sub state machine. Is a data organization object in which data is provided to other state components for use. Module is an intermediate concept between equipment and parts. Modules can be nested.")]
    public class Module : Part
    {
        /// <summary>
        /// 名称
        /// </summary>
        public new const string Title = "模块";

        /// <summary>
        /// 创建模块
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [Name(Title, nameof(Module))]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        [StateLib(RepairmanHelperExtension.DataModelStateLibName, typeof(RepairmanManager), stateType = EStateType.SubStateMachine)]
        [StateComponentMenu(RepairmanHelperExtension.DataModelStateLibName + "/" + Title, typeof(RepairmanManager))]
        [Tip("模块组件包含多个零件组件的容器。用子状态机实现。是一个数据组织对象、其中数据提供给其他状态组件使用。模块是介于设备与零件之间的中间层概念，模块可以嵌套模块。", "A module assembly is a container that contains multiple part assemblies. It is realized by sub state machine. Is a data organization object in which data is provided to other state components for use. Module is an intermediate concept between equipment and parts. Modules can be nested.")]
        public static State CreateModule(IGetStateCollection obj)
        {
            return obj?.CreateSubStateMachine(CommonFun.Name(typeof(Module)), null, typeof(Module));
        }

        #region 零件父子级关系

        /// <summary>
        /// 当前状态组件所依附的子状态机
        /// </summary>
        protected SubStateMachine subStateMachine => parent as SubStateMachine;

        /// <summary>
        /// 子零件
        /// </summary>
        public Part[] childrenParts => subStateMachine.GetComponentsInChildren<Part>();

        /// <summary>
        /// 子模块
        /// </summary>
        public Module[] childrenModules => subStateMachine.GetComponentsInChildren<Module>();

        /// <summary>
        /// 节点类型
        /// </summary>
        public override ETreeNodeType nodeType => ETreeNodeType.Sub;

        /// <summary>
        /// 包含零件
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        public bool ContainPart(Part part) => childrenParts.Count(p => p == part) > 0;

        /// <summary>
        /// 装配状态
        /// </summary>
        public override EAssembleState assembleState
        {
            get
            {
                var parts = childrenParts.ToList();
                if (parts.Exists(p => (p.assembleState == EAssembleState.AssemblyInProgress)))
                {
                    return EAssembleState.AssemblyInProgress;
                }

                return (parts.All(p => p.assembleState == EAssembleState.Assembled) && base.assembleState == EAssembleState.Assembled) ? EAssembleState.Assembled : EAssembleState.Disassembled;
            }
        }

        private List<Part> parts => childrenParts.ToList();

        /// <summary>
        /// 关联模块可交互组件
        /// </summary>
        public override Tools.Part interactPart
        {
            get
            {
                if (!gameObject) return null;

                var module = gameObject.XGetOrAddComponent<Tools.Module>();
                module._moduleSC = this;
                return module;
            }
        }

        #endregion

        #region 零件装配约束

        /// <summary>
        /// 通过状态机连接关系分析零件装配关系
        /// </summary>
        public virtual List<(Part, Part)> GetAssemblyConstraints()
        {
            var result = new List<(Part, Part)>();

            var needBreadthSearchPart = new List<(Part, Transition)>();
            foreach (var t in subStateMachine.transitions)
            {
                // 将变换指向方向作为装配顺序方向
                var firstPart = t.inState.GetComponent<Part>();
                var secondPart = t.outState.GetComponent<Part>();
                if (firstPart)
                {
                    if (secondPart)
                    {
                        result.Add((firstPart, secondPart));
                    }
                    else
                    {
                        needBreadthSearchPart.Add((firstPart, t));
                    }
                }
            }

            foreach (var pt in needBreadthSearchPart)
            {
                var firstPart = pt.Item1;
                var outState = pt.Item2.outState;
                FindConnectParts(outState).Foreach(secondPart => result.Add((firstPart, secondPart)));
            }

            // 去重添加
            var resultDistinct = new List<(Part, Part)>();
            foreach (var data in result)
            {
                if (resultDistinct.Exists(r => r.Item1 == data.Item1 && r.Item2 == data.Item2)) continue;

                resultDistinct.Add((data.Item1, data.Item2));
            }

            return resultDistinct;
        }

        /// <summary>
        /// 从输入状态开始查找与之相连的后续零件组件对象
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        private List<Part> FindConnectParts(State state)
        {
            var result = new List<Part>();
            var visitedStates = new HashSet<State>();

            var queue = new Queue<State>();
            queue.Enqueue(state);
            while (queue.Count > 0)
            {
                var currentState = queue.Dequeue();
                if (!visitedStates.Contains(currentState))
                {
                    visitedStates.Add(currentState);

                    var part = currentState.GetComponent<Part>();
                    if (part)//有零件组件则终止遍历
                    {
                        result.Add(part);
                    }
                    else// 无零件组件继续使用后续跳转状态进行遍历
                    {
                        currentState.outStates.Foreach(s =>
                        {
                            if (!visitedStates.Contains(s))
                            {
                                queue.Enqueue(s);
                            }
                        });
                    }
                }
            }
            return result;
        }

        #endregion
    }
}
