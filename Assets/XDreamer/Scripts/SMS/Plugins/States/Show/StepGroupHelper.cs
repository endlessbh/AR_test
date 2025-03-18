using System.Collections.Generic;
using XCSJ.PluginSMS.Kernel;

namespace XCSJ.PluginSMS.States.Show
{
    public class StepGroupHelper
    {
        public static Step GetRoot(Step step)
        {
            if (!step) return null;
            return step.parentStep == null ? step : GetRoot(step.parentStep);
        }

        public static void GotoState(StepGroup stepGroup, State state)
        {
            var root = GetRoot(stepGroup) as StepGroup;
            if (root)
            {
                root.GotoState(state);
            }
        }

        /// <summary>
        /// 获取选中步骤全局索引
        /// </summary>
        public static int FindSelectedIndexInGlobal(StepGroup stepGroup, bool includeGroup=false)
        {
            var list = GetAllStep(stepGroup);
            if(list.Exists(s=>s.selected==true))
            {
                int count = 0;
                int selectedIndex = 0;
                foreach(var item in list)
                {
                    if (item is StepGroup && !includeGroup) continue;

                    ++count;
                    if(item.selected)
                    {
                        selectedIndex = count;
                    }
                }
                return selectedIndex;
            }
            return -1;
        }

        /// <summary>
        /// 构建步骤树形结构
        /// </summary>
        public static List<Step> InitStepTree(StepGroup stepGroup)
        {
            if (!stepGroup) return new List<Step>();

            stepGroup.ClearChildren();
            stepGroup.steps = SMSHelper.GetStateComponentsWithBreadthFirst<Step>(stepGroup.parent as StateCollection);
            foreach (var s in stepGroup.steps)
            {
                stepGroup.AddChildren(s);

                if (s is StepGroup group && group)
                {
                    InitStepTree(group);
                }
            }

            return stepGroup.steps;
        }

        private static Dictionary<StepGroup, List<Step>> allStepDic = new Dictionary<StepGroup, List<Step>>();

        public static Step GetCurrentStepInGlobal(StepGroup stepGroup)
        {
            var steps = GetAllStep(stepGroup);
            return steps.FindLast(s => s.stepState == EStepState.Active);
        }

        public static Step GetNextStepInGlobal(StepGroup stepGroup)
        {
            var steps = GetAllStep(stepGroup);
            var index = steps.FindLastIndex(s=>s.stepState==EStepState.Active);
            while ((++index) >= 0 && index < steps.Count)
            {
                var step = steps[index];
                if (!(step is StepGroup))
                {
                    return step;
                }
            }
            return null;
        }

        public static Step GetPreviousStepInGlobal(StepGroup stepGroup)
        {
            var steps = GetAllStep(stepGroup);
            var index = steps.FindLastIndex(s => s.stepState == EStepState.Active);
            while((--index)>=0)
            {
                var step = steps[index];
                if (!(step is StepGroup))
                {
                    return step;
                }
            }
            return null;
        }

        public static List<Step> GetAllStep(StepGroup stepGroup)
        {
            List<Step> allStep = new List<Step>();
            if (!stepGroup) return allStep;

            if (!allStepDic.TryGetValue(stepGroup, out allStep))
            {
                allStep = GetStepsInChildren(stepGroup);
                allStepDic.Add(stepGroup, allStep);
            }
            return allStep;
        }

        // 将stepGroup下所有步骤，构建为一整个链表
        private static List<Step> GetStepsInChildren(StepGroup stepGroup)
        {
            var stepList = new List<Step>();
            if (!stepGroup) return stepList;

            foreach (var s in stepGroup.steps)
            {               
                stepList.Add(s);

                if (s is StepGroup group && group)
                {
                    stepList.AddRange(GetStepsInChildren(group));
                }
            }
            return stepList;
        }

        public static List<Step> FindStepListAndUpdateChildren(StepGroup group)
        {
            if (!group) return new List<Step>();

            group.ClearChildren();
            group.steps = SMSHelper.GetStateComponentsWithBreadthFirst<Step>(group.parent as StateCollection);
            foreach (var s in group.steps)
            {
                group.AddChildren(s);

                if (s is StepGroup g && g)
                {
                    FindStepListAndUpdateChildren(g);
                }
            }
            return group.steps;
        }
    }
}
