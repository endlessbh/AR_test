using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Runtime;
using XCSJ.PluginRepairman.Machines;
using XCSJ.PluginRepairman.States;
using XCSJ.PluginRepairman.States.RepairTask;
using XCSJ.PluginRepairman.Tasks;
using XCSJ.PluginSMS;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Show;

namespace XCSJ.PluginRepairman
{
    /// <summary>
    /// 拆专指引
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [RequireManager(typeof(RepairmanManager))]
    [Owner(typeof(RepairmanManager))]
    [Serializable]
    public class RepairGuide<T> : StateComponent<T>, ITask where T : RepairGuide<T>
    {
        [Name("拆装任务")]
        [Tip("拆装任务是拆装步骤和修理步骤组连线的组合", "Disassembly task is the combination of disassembly steps and repair steps")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [StateComponentPopup(typeof(RepairTaskWork), stateCollectionType = EStateCollectionType.Root)]
        public RepairTaskWork repairTaskWork;

        /// <summary>
        /// 结果展示列表
        /// </summary>
        [Name("结果展示列表")]
        [Tip("任务开始时隐藏，任务完成后展示", "Hide at the beginning of the task and display after the task is completed")]
        public List<GameObject> resultList = new List<GameObject>();

        [Name("开始调用函数")]
        [UserDefineFun]
        public string startCallbackFun;

        [Name("完成调用函数")]
        [UserDefineFun]
        public string finishCallbackFun;
        
        protected List<Part> parts = new List<Part>();

        protected List<Tool> tools = new List<Tool>();

        public string showName
        {
            get
            {
                return parent.name;
            }
            set
            {
                parent.name = value;
            }
        }

        public string description
        {
            get
            {
                return description;
            }
            set
            {
                description = value;
            }
        }

        public ETaskState taskState { get; set; }

        public override void OnCreated()
        {
            base.OnCreated();

            repairTaskWork = SMSHelper.GetStateComponents<RepairTaskWork>().FirstOrDefault(); 
        }

        public override void Reset()
        {
            base.Reset();

            SetResultListActive(false);
        }

        private void SetResultListActive(bool active)
        {
            resultList.ForEach(r =>
            {
                if (r)
                {
                    r.SetActive(active);
                }
            });
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override bool Init(StateData data)
        {
            base.Init(data);
            
            parts = SMSHelper.GetStateComponents<Part>();
            tools = SMSHelper.GetStateComponents<Tool>();

            return true;
        }

        private bool orgAutoSelectPartAndTool = false;

        /// <summary>
        /// 进入
        /// </summary>
        /// <param name="data"></param>
        public override void OnEntry(StateData data)
        {
            base.OnEntry(data);

            Selection.selectionChanged += OnSelectionPartChanged;
            ToolSelection.selectionChanged += OnSelectionToolChanged;

            Selection.selection = null;
            ToolSelection.Clear();
            ScriptManager.CallUDF(startCallbackFun);

            orgAutoSelectPartAndTool = RepairStep.autoSelectPartAndTool;
            RepairStep.autoSelectPartAndTool = false;

            SetResultListActive(false);
        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="data"></param>
        public override void OnExit(StateData data)
        {
            Selection.selectionChanged -= OnSelectionPartChanged;
            ToolSelection.selectionChanged -= OnSelectionToolChanged;

            RepairStep.autoSelectPartAndTool = orgAutoSelectPartAndTool;
            SetResultListActive(true);

            Selection.Clear();

            base.OnExit(data);
        }

        /// <summary>
        /// 完成
        /// </summary>
        /// <returns></returns>
        public override bool Finished() => repairTaskWork ? repairTaskWork.stepState == PluginSMS.States.Show.EStepState.Finished : true;

        public RepairStep GetCurrentStep() => StepGroupHelper.GetCurrentStepInGlobal(repairTaskWork) as RepairStep;

        public bool Check() => true;

        public virtual bool Skip() => true;

        public override bool DataValidity() => repairTaskWork;

        protected virtual void OnSelectionPartChanged(GameObject[] oldSelections, bool flag){ }

        protected virtual void OnSelectionToolChanged(ITool[] oldSelections, bool flag) { }

        protected bool IsCurrentActiveTaskPart(GameObject go)
        {
            if (!go) return false;

            var curStep = GetCurrentStep();
            return curStep ? curStep.selectedParts.Exists(p => p.gameObject == go):false;
        }

        protected bool IsGameObjectCurrentActiveTaskTool(GameObject go)
        {
            if (!go) return false;

            var curStep = GetCurrentStep();
            return curStep ? curStep.selectedTools.Exists(t => t.gameObject == go) : false;
        }

        protected bool IsCurrentActiveTaskTool(Tool tool)
        {
            if (tool==null) return false;

            var curStep = GetCurrentStep() as RepairStep;
            return curStep ? curStep.selectedTools.Exists(t => t.displayName==tool.displayName) : false;
        }

        void ITask.Help()
        {
            
        }
    }
}
