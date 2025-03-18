using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginCommonUtils.Runtime;
using XCSJ.PluginRepairman.Machines;
using XCSJ.PluginSMS;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Show;
using XCSJ.PluginTools.SelectionUtils;

namespace XCSJ.PluginRepairman.States.RepairTask
{
    [ComponentMenu(RepairmanHelperExtension.StepPath + "/" + Title, typeof(RepairmanManager))]
    [Name(Title, nameof(RepairStep))]
    [XCSJ.Attributes.Icon(index = 34484)]
    [KeyNode(nameof(ITrigger), "触发器")]
    [KeyNode(nameof(IStep), "步骤")]
    [DisallowMultipleComponent]
    [RequireManager(typeof(RepairmanManager))]
    [Tip("拆装步骤组件是零件选择、工具选择和动画组成的触发器。用状态来实现。是一个数据组织对象、其中数据提供给其他状态组件使用。用户在场景中选择对应的零件与工具即可触发状态完成。时间轴可播放修理步骤所关联的动画，并同步界面步骤的选中状态。", "The disassembly step component is a trigger composed of part selection, tool selection and animation. It is realized by state. Is a data organization object in which data is provided to other state components for use. The user can trigger the completion of the status by selecting the corresponding parts and tools in the scene. The timeline can play the animation associated with the repair step and synchronize the selected status of the interface step.")]
    public class RepairStep : BaseTask, IDynamicLabel, ITrigger
    {
        /// <summary>
        /// 名称
        /// </summary>
        public new const string Title = "拆装步骤";

        [Name(Title, nameof(RepairStep))]
        [StateLib(RepairmanHelperExtension.StepStateLibName, typeof(RepairmanManager))]
        [StateComponentMenu(RepairmanHelperExtension.StepPath + "/" + Title, typeof(RepairmanManager))]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        [Tip("拆装步骤组件是零件选择、工具选择和动画组成的触发器。用状态来实现。是一个数据组织对象、其中数据提供给其他状态组件使用。用户在场景中选择对应的零件与工具即可触发状态完成。时间轴可播放修理步骤所关联的动画，并同步界面步骤的选中状态。", "The disassembly step component is a trigger composed of part selection, tool selection and animation. It is realized by state. Is a data organization object in which data is provided to other state components for use. The user can trigger the completion of the status by selecting the corresponding parts and tools in the scene. The timeline can play the animation associated with the repair step and synchronize the selected status of the interface step.")]
        public static State CreateRepairStepTask(IGetStateCollection obj)
        {
            return obj?.CreateNormalState(CommonFun.Name(typeof(RepairStep)), null, typeof(RepairStep));
        }

        /// <summary>
        /// 选择规则
        /// </summary>
        [Name("选择规则")]
        public enum ESelectRule
        {
            [Name("无")]
            [Tip("表示不需要使用任何对象", "Indicates that no objects are required")]
            None,

            [Name("全部")]
            [Tip("选中对象集全部元素", "Select all elements of the object set")]
            All,

            [Name("任意")]
            [Tip("选中对象集其中一个元素", "Select one of the elements of the object set")]
            Any
        }

        /// <summary>
        /// 零件选择规则
        /// </summary>
        [Group("零件设置", textEN = "Part Settings")]
        [Name("零件选择规则")]
        [EnumPopup]
        public ESelectRule _partSelectRule = ESelectRule.All;

        /// <summary>
        /// 零件列表
        /// </summary>
        [Name("零件列表")]
        [ValidityCheck(EValidityCheckType.ElementCountGreater, 0)]
        [ArrayElement(EArrayElementHandleRule.CanDelete)]
        [Readonly(EEditorMode.Runtime)]
        public List<Part> selectedParts = new List<Part>();

        /// <summary>
        /// 工具选择规则
        /// </summary>
        [Group("工具设置", textEN = "Tool Settings")]
        [Name("工具选择规则")]
        [EnumPopup]
        public ESelectRule _toolSelectRule = ESelectRule.All;

        /// <summary>
        /// 工具列表
        /// </summary>
        [Name("工具列表")]
        [ArrayElement(EArrayElementHandleRule.CanDelete)]
        public List<Tool> selectedTools = new List<Tool>();

        private int maxSelectionCount = 0;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override bool Init(StateData data)
        {
            // 移除空对象
            RepairmanHelperExtension.RemoveNullObject(selectedParts);
            RepairmanHelperExtension.RemoveNullObject(selectedTools);

            return base.Init(data);
        }

        /// <summary>
        /// 进入
        /// </summary>
        /// <param name="data"></param>
        public override void OnEntry(StateData data)
        {
            base.OnEntry(data);

            Selection.Clear();

            maxSelectionCount = LimitedSelection.maxCount;
            LimitedSelection.maxCount = selectedParts.Count;

            ToolSelection.selectionMaxCount = selectedTools.Count;
        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="data"></param>
        public override void OnExit(StateData data)
        {
            LimitedSelection.maxCount = maxSelectionCount;

            base.OnExit(data);
        }

        /// <summary>
        /// 完成
        /// </summary>
        /// <returns></returns>
        protected override bool DefaultFinish()
        {
            var partSelectResult = false;
            switch (_partSelectRule)
            {
                case ESelectRule.None:
                    {
                        partSelectResult = true;
                        break;
                    }
                case ESelectRule.All:
                    {
                        partSelectResult = selectedParts.All(p => p.interactPart.grabbable.interactableEntity.isSelected);
                        break;
                    }
                case ESelectRule.Any:
                    {
                        partSelectResult = selectedParts.Any(p => p.interactPart.grabbable.interactableEntity.isSelected);
                        break;
                    }
            }

            var toolSelectResult = false;
            switch (_toolSelectRule)
            {
                case ESelectRule.None:
                    {
                        toolSelectResult = true;
                        break;
                    }
                case ESelectRule.All:
                    {
                        toolSelectResult = ToolSelection.ContainAll(selectedTools.Cast<ITool>());
                        break;
                    }
                case ESelectRule.Any:
                    {
                        toolSelectResult = ToolSelection.ContainAny(selectedTools.Cast<ITool>());
                        break;
                    }
            }

            return partSelectResult && toolSelectResult;
        }

        /// <summary>
        /// 友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString()
        {
            var partCount = selectedParts.Count;
            if (partCount>0)
            {
                if (selectedParts[0])
                {
                    return selectedParts[0].showName;
                }
                else
                {
                    return partCount.ToString();
                }
            }
            return "";
        }

        public override bool Skip()
        {
            Help();

            return base.Skip();
        }

        /// <summary>
        /// 设置零件、动画对象和工具被选中，满足步骤条件
        /// </summary>
        public override void Help()
        {
            Selection.Clear();

            selectedParts.ForEach(p => Selection.Add(p.gameObject));

            selectedTools.ForEach(t => ToolSelection.AddTool(t));
        }

        /// <summary>
        /// 是否响应点击
        /// </summary>
        public static bool isOnClick = true; 

        public override void OnClick()
        {
            if (isOnClick)
            {
                base.OnClick();
            }            
        }

        public static bool autoSelectPartAndTool = false;

        public override bool selected
        {
            set
            {
                base.selected = value;

                // 自动选择（在播放模式下需要使用）
                if (value && autoSelectPartAndTool)
                {
                    Help();
                }
            }
        }

        #region IDynamicLabel

        /// <summary>
        /// 获取动态标签
        /// </summary>
        /// <param name="propertyPath"></param>
        /// <param name="fieldInfo"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        public GUIContent GetDynamicLabel(string propertyPath, FieldInfo fieldInfo, GUIContent label)
        {
            if(PropertyPathHelper.TryGetLastArrayElementIndexAndArrayPropertyPath(propertyPath, out var index, out var arrayPropertyPath))
            {
                switch (arrayPropertyPath)
                {
                    case nameof(selectedParts):
                        {
                            var part = selectedParts[index];
                            if (!part) break;
                            var text = (index + 1).ToString() + "." + part.parent.name;
                            return new GUIContent(text, text);
                        }
                    case nameof(selectedTools):
                        {
                            var tool = selectedTools[index];
                            if (!tool) break;
                            var text = (index + 1).ToString() + "." + tool.parent.name;
                            return new GUIContent(text, text);
                        }
                }
            }
            return label;
        }

        #endregion

    }
}
