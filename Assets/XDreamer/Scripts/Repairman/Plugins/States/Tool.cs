using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginRepairman.Machines;
using XCSJ.PluginSMS;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;

namespace XCSJ.PluginRepairman.States
{
    [XCSJ.Attributes.Icon(EIcon.Tool)]
    [ComponentMenu(RepairmanHelperExtension.DataModelStateLibName + "/" + Title, typeof(RepairmanManager))]
    [Name(Title, nameof(Tool))]
    [Tip("工具组件是可关联三维模型对象的容器。配合拆装步骤使用，用状态来实现。是一个数据组织对象、其中数据提供给其他状态组件使用。", "A tool component is a container that associates 3D model objects. It is used in conjunction with the disassembly and assembly steps and realized by status. Is a data organization object in which data is provided to other state components for use.")]
    [DisallowMultipleComponent]
    [RequireManager(typeof(RepairmanManager))]
    public class Tool : Item, ITool
    {
        /// <summary>
        /// 名称
        /// </summary>
        public const string Title = "工具";

        [Name(Title, nameof(Tool))]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        [StateLib(RepairmanHelperExtension.DataModelStateLibName, typeof(RepairmanManager))]
        [RequireManager(typeof(RepairmanManager))]
        [StateComponentMenu(RepairmanHelperExtension.DataModelStateLibName + "/" + Title, typeof(RepairmanManager))]
        [Tip("工具组件是可关联三维模型对象的容器。配合拆装步骤使用，用状态来实现。是一个数据组织对象、其中数据提供给其他状态组件使用。", "A tool component is a container that associates 3D model objects. It is used in conjunction with the disassembly and assembly steps and realized by status. Is a data organization object in which data is provided to other state components for use.")]
        public static State CreateTool(IGetStateCollection obj)
        {
            return obj?.CreateNormalState(CommonFun.Name(typeof(Tool)), null, typeof(Tool));
        }

        public override bool selected
        {
            get
            {
                return ToolSelection.selections.Contains(this);
            }
            set
            {
                if (value)
                {
                    ToolSelection.AddTool(this);
                }
                else
                {
                    ToolSelection.Remove(this);
                }
            }
        }

        public override bool Init(StateData data)
        {
            if (gameObject)
            {
                gameObjectToolDic.Add(gameObject, this);
            }
            return base.Init(data);
        }

        private static Dictionary<GameObject, Tool> gameObjectToolDic = new Dictionary<GameObject, Tool>();

        public static Tool FindTool(GameObject go)
        {
            gameObjectToolDic.TryGetValue(go, out Tool tool);
            return tool;
        }

        /// <summary>
        /// 当前工具能够操作的零件类型列表
        /// </summary>
        public List<string> _partCategoryNames = new List<string>();
    }
}
