using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;

namespace XCSJ.PluginSMS.States.Show
{
    /// <summary>
    /// 步骤组:步骤组组件是步骤集合的容器。用于将步骤进行分组，组件激活后随即切换为完成态。
    /// </summary>
    [ComponentMenu("展示/"+ Title, typeof(SMSManager))]
    [Name(Title, nameof(StepGroup))]
    [XCSJ.Attributes.Icon(index = 33646)]
    [RequireState(typeof(SubStateMachine))]
    [DisallowMultipleComponent]
    [Tip("步骤组组件是步骤集合的容器。用于将步骤进行分组，组件激活后随即切换为完成态。", "A step clip component is an object that associates an animation component to a step. Multiple step clips constitute one step, and the component will switch to the completed state immediately after activation.")]
    public class StepGroup : Step
    {
        /// <summary>
        /// 标题
        /// </summary>
        public new const string Title = "步骤组";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib("展示", typeof(SMSManager), stateType = EStateType.SubStateMachine)]
        [StateComponentMenu("展示/"+ Title, typeof(SMSManager))]
        [Name(Title, nameof(StepGroup))]
        [Tip("步骤组组件是步骤集合的容器。用于将步骤进行分组，组件激活后随即切换为完成态。", "A step clip component is an object that associates an animation component to a step. Multiple step clips constitute one step, and the component will switch to the completed state immediately after activation.")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State CreateStepGroup(IGetStateCollection obj)
        {
            return obj?.CreateSubStateMachine(CommonFun.Name(typeof(StepGroup)), null, typeof(StepGroup));
        }

        public SubStateMachine subStateMachine => parent as SubStateMachine;

        public override ETreeNodeType nodeType => ETreeNodeType.Sub;

        public List<Step> steps
        {
            get
            {
                if (_steps == null)
                {
                    InitStep();
                }
                return _steps;
            }
            set => _steps = value;
        }

        private List<Step> _steps = null;

        #region 生命周期函数

        public override bool Init(StateData data)
        {
            InitStep();
            
            return base.Init(data);
        }

        public override void OnEntry(StateData data)
        {
            ResetStepState();

            base.OnEntry(data);
        }

        public override void OnUpdate(StateData data)
        {
            base.OnUpdate(data);

            //CheckActiveLastStepUpdate();
        }

        public override void OnExit(StateData data)
        {
            base.OnExit(data);

            // 不选中不能在步骤中操作！因为步骤状态虽然退出，
            // 但在进入一下个步骤前都属于当前步骤
            if (data != null)
            {
                UnSelect();
            }
        }

        public override string ToFriendlyString()
        {
            if (NeedInStepGroup() && !InStepGroup())
            {
                return "需放置" + CommonFun.Name(typeof(Plan)) + "内!";
            }
            return "";
        }

        #endregion

        #region 步骤创建选择

        private void InitStep()
        {
            ClearChildren();
            steps = SMSHelper.GetStateComponentsWithBreadthFirst<Step>(parent as StateCollection);
            foreach (var s in steps)
            {
                AddChildren(s);
            }
        }        

        public override void SetCurrent(State state) => steps.ForEach(s => s.SetCurrent(state));
        
        public override void UnSelect()
        {
            base.UnSelect();
            steps.ForEach(s => s.UnSelect());
        }

        public override List<State> GetSelectedStates(bool includeWorkClip = true)
        {
            List<State> states = base.GetSelectedStates(includeWorkClip);
            if (selected)
            {
                foreach (var step in steps)
                {
                    states.AddRange(step.GetSelectedStates(includeWorkClip));
                }
            }
            return states;
        }

        public override void OnClick()
        {
            var firstStep = steps.FirstOrDefault();
            if (firstStep)
            {
                firstStep.OnClick();
            }
        }

        public override int count => steps.Sum(s => s.count);

        #endregion

        #region 步骤跳跃操作

        public virtual void GotoFirstStep()
        {
            var current = StepGroupHelper.GetCurrentStepInGlobal(this);
            if (current)
            {
                current.SkipState(0);
            }
                        
            //激活第一个步骤
        }

        public virtual void GotoLastStep()
        {
            var current = StepGroupHelper.GetCurrentStepInGlobal(this);
            if (current)
            {
                current.SkipState(1);
            }
        }

        public virtual bool GotoPreviousStep()
        {
            var current = StepGroupHelper.GetCurrentStepInGlobal(this);
            if (!current) return false;

            var previousStep = StepGroupHelper.GetPreviousStepInGlobal(this);

            UnActiveCurrentStepToTargetStep(current, previousStep);
            current.SetUnfinished();

            GotoStep(previousStep);

            return previousStep;
        }

        public virtual bool GotoNextStep()
        {
            var current = StepGroupHelper.GetCurrentStepInGlobal(this);
            if (!current) return false;

            var nextStep = StepGroupHelper.GetNextStepInGlobal(this);

            UnActiveCurrentStepToTargetStep(current, nextStep);
            current.SetFinished();

            GotoStep(nextStep);

            if (!nextStep)            
            {
                if (!current.parentStep) Debug.Log("currentStep parent==null"+ current.name);
                // 没有下一步，则将整个状态机退出
                current.parentStep.UnSelect();

                // 使用退出状态来退出状态机，让后续的状态对象可以正常处理
                if(current.parentStep.parent is SubStateMachine subSM && subSM)
                {
                    (subSM.exitState as State).active = true;
                }
            }
            return nextStep;
        }

        private void GotoStep(Step step)
        {
            if (!step) return;

            if (step.parentStep)
            {
                var subSM = step.parentStep.parent as SubStateMachine;
                if (subSM)
                {
                    // 让步骤所在状态机激活，但不激活进入步骤
                    subSM.SetActive(true, true, true, false);
                }
            }

            // 使用根跳过前面步骤的动画
            //var state = step.GetState();
            //Debug.Log("state:"+state.name); 
            //StepGroupHelper.GotoState(this, step.GetState());
            
            step.SetStateActive(true);
        }

        private void UnActiveCurrentStepToTargetStep(Step curStep, Step targetStep)
        {
            int currentDepth = curStep.depth;
            int targetDepth = targetStep ? targetStep.depth : 0;
            int unactiveNum = currentDepth - targetDepth;

            var unactiveStep = curStep;
            do
            {
                unactiveStep.SetStateActive(false);
                unactiveStep = unactiveStep.parentStep;
                --unactiveNum;
            } while (unactiveNum >= 0 && unactiveStep);
        }

        public virtual void ResetCurrentStep()
        {
            var current = StepGroupHelper.GetCurrentStepInGlobal(this);
            if (current)
            {
                current.SkipState(0, false, true);
                current.SetStateActive(true);
            }
        } 
              
        public virtual void GotoState(State state) { }

        #endregion

        #region 索引


        #endregion

        #region IStep

        public override Step parentStep
        {
            get
            {
                return base.parentStep;
            }
            set
            {
                base.parentStep = value;
                
            }
        }

        public override void ResetStepState()
        {
            stepState = EStepState.Unfinished;
            steps.ForEach(s => s.ResetStepState());
        }

        public override void CheckFinished()
        {
            if(steps.Count==0 || steps.All(s=>s.stepState== EStepState.Finished))
            {
                stepState = EStepState.Finished;
            }
        }

        #endregion

        #region 步骤及事件

        public void OnStepStateChanged(Step step, EStepState inState)
        {
            // 自身状态变化，调用父类通知其改变
            if (steps.Contains(step))
            {
                // 下级步骤向上层传递改变
                switch (inState)
                {
                    case EStepState.None:
                        break;
                    case EStepState.Unfinished:
                        {
                            stepState = EStepState.Unfinished;

                            onStepInit?.Invoke(this, step);
                            break;
                        }
                    case EStepState.Active:
                        {
                            onStepActive?.Invoke(this, step);
                            onStepChanged?.Invoke(this, currentStep, step);
                            currentStep = step;
                            //if (currentStep)
                            //{
                            //    Debug.Log("当前步骤：" + currentStep.description + ",this:" + description);
                            //}
                            break;
                        }
                    case EStepState.Finished:
                        {
                            CheckFinished();

                            if (steps.LastOrDefault()== step)
                            {
                                currentStep = null;
                            }
                            onStepFinish?.Invoke(this, step);
                            break;
                        }
                    default:
                        break;
                }                
            }            
        }

        public Step currentStep { get; protected set; } = null;

        public Step currentStepInChildren
        {
            get
            {
                if(currentStep is StepGroup group)
                {
                    return group.currentStepInChildren;
                }
                else
                {
                    return currentStep;
                }
            }
        }

        public Step previousStep
        {
            get
            {
                if (steps.Count > 0)
                {
                    if (currentStep)
                    {
                        int index = steps.FindIndex(s => s == currentStep);
                        if (index > 0 && index < steps.Count)
                        {
                            return steps[--index];
                        }
                    }
                    else
                    {
                        return steps.LastOrDefault();
                    }
                }
                return null;
            }
        }

        public Step nextStep
        {
            get
            {
                if (steps.Count > 0)
                {
                    if (currentStep)
                    {
                        int index = steps.FindIndex(s => s == currentStep);
                        if (index >= 0 && index < steps.Count - 1)
                        {
                            return steps[++index];
                        }
                    }
                    else
                    {
                        return steps.FirstOrDefault();
                    }
                }
                return null;    
            }
        }

        public Step FindStepInChildren(string description)
        {
            if (this.description == description)
            {
                return this;
            }
            foreach (var step in steps)
            {
                if(step.description==description)
                {
                    return step;
                }
                else if (step is StepGroup group)
                {
                    return group.FindStepInChildren(description);
                }
            }
            return null;
        }       

        public static event Action<StepGroup, Step> onStepInit = null;

        public static event Action<StepGroup, Step> onStepActive = null;

        public static event Action<StepGroup, Step, Step> onStepChanged = null;

        public static event Action<StepGroup, Step> onStepFinish = null;

        #endregion
    }
}
