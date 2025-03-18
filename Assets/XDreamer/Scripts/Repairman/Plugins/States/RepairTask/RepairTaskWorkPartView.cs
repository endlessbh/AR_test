using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Extension.Base.Recorders;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginRepairman.Base;
using XCSJ.PluginSMS;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Show;
using XCSJ.PluginTools.GameObjects;
using XCSJ.PluginTools.Items;
using XCSJ.PluginXGUI.Windows.Tables;

namespace XCSJ.PluginRepairman.States.RepairTask
{
    #region 拆装任务零件视图

    /// <summary>
    /// 拆装任务零件视图：
    /// 1、用于生成装配任务步骤中相关的UI零件列表，并更新零件视图数量
    /// 2、动态的生成对应的模块视图
    /// </summary>
    [ComponentMenu(RepairmanHelperExtension.StepStateLibName + "/" + Title, typeof(RepairmanManager))]
    [Name(Title, nameof(RepairTaskWorkPartView))]
    [XCSJ.Attributes.Icon(EIcon.Task)]
    [DisallowMultipleComponent]
    [Tip("用于生成装配任务步骤中相关的UI零件列表，追踪零件装配的完成度;动态的生成对应的模块视图", "It is used to generate the relevant UI parts list in the assembly task step and track the completion of part assembly; Dynamically generate the corresponding module view")]
    [RequireComponent(typeof(RepairTaskWork))]
    [Owner(typeof(RepairmanManager))]
    public sealed class RepairTaskWorkPartView : StateComponent<RepairTaskWorkPartView>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "拆装任务零件视图";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(RepairmanHelperExtension.StepStateLibName, typeof(RepairmanManager))]
        [StateComponentMenu(RepairmanHelperExtension.StepStateLibName + "/" + Title, typeof(RepairmanManager))]
        [Name(Title, nameof(RepairTaskWorkPartView))]
        [Tip("用于生成装配任务步骤中相关的UI零件列表，追踪零件装配的完成度;动态的生成对应的模块视图", "It is used to generate the relevant UI parts list in the assembly task step and track the completion of part assembly; Dynamically generate the corresponding module view")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State CreateTaskWork(IGetStateCollection obj)
        {
            return obj?.CreateSubStateMachine(CommonFun.Name(typeof(RepairTaskWorkPartView)), null, typeof(RepairTaskWorkPartView));
        }

        #region 表数据集合

        /// <summary>
        /// 表数据集合
        /// </summary>
        [Name("表数据集合")]
        [Tip("用于将设备对象中的零件生成列表的管理器", "Manager for generating a list of parts from equipment objects")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [Readonly(EEditorMode.Runtime)]
        public Table _table;

        /// <summary>
        /// 表数据集合
        /// </summary>
        public Table table
        {
            get
            {
                if (!_table)
                {
                    _table = UnityObjectExtension.GetComponentInGlobal<Table>();
                }
                return _table;
            }
        }

        #endregion

        #region 任务管理

        /// <summary>
        /// 修理任务
        /// </summary>
        public RepairTaskWork repairTaskWork
        {
            get
            {
                if (!_repairTaskWork)
                {
                    _repairTaskWork = GetComponent<RepairTaskWork>();
                }
                return _repairTaskWork;
            }
        }
        private RepairTaskWork _repairTaskWork;

        /// <summary>
        /// 零件表数据集
        /// </summary>
        private List<PartTableData_Model> _partTableDatas = new List<PartTableData_Model>();

        /// <summary>
        /// 游戏对象激活记录器
        /// </summary>
        private GameObjectRecorder _gameObjectRecorder = new GameObjectRecorder();

        /// <summary>
        /// 进度
        /// </summary>
        public float taskProgress { get => _taskProgress; private set => _taskProgress = value; }
        private float _taskProgress = 0;

        private int totalPartCount = 0;
        private int finishPartCount = 0;

        #endregion

        #region 状态组件方法

        /// <summary>
        /// 重置状态
        /// </summary>
        public override void Reset()
        {
            base.Reset();

            if (table) { }
        }

        private bool dataValidOnEntry = false;

        /// <summary>
        /// 进入
        /// </summary>
        /// <param name="data"></param>
        public override void OnEntry(StateData data)
        {
            base.OnEntry(data);

            dataValidOnEntry = DataValidity();
            if (dataValidOnEntry)
            {
                CreatePartTable();

                // 增加零件拆装状态改变监听
                Tools.Part.onAssembleStateChanged += OnPartStateChanged;
                StepGroup.onStepActive += OnStepActive;
                StepGroup.onStepFinish += OnStepFinish;
            }
        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="data"></param>
        public override void OnExit(StateData data)
        {
            base.OnExit(data);

            if (dataValidOnEntry)
            {
                // 移除零件拆装状态改变监听
                Tools.Part.onAssembleStateChanged -= OnPartStateChanged;
                StepGroup.onStepActive -= OnStepActive;
                StepGroup.onStepFinish -= OnStepFinish;

                // 移除所有零件视图
                DeletePartTableDatas();

                // 还原所有零件游戏对象激活状态
                _gameObjectRecorder.Recover();
            }
        }

        /// <summary>
        /// 有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity() => table;

        #endregion

        #region 步骤激活完成或零件装配状态发生改变事件

        /// <summary>
        /// 零件状态回调
        /// </summary>
        /// <param name="part"></param>
        /// <param name="oldState"></param>
        private void OnPartStateChanged(Tools.Part part, EAssembleState oldState)
        {
            var partOfState = repairTaskWork.parts.Find(p => p.interactPart == part);
            if (!partOfState) return;

            ++finishPartCount;
            taskProgress = 1.0f * finishPartCount / totalPartCount; // 这里不需要判断除0错误

            // 检测零件状态，并修改其在UI列表中的状态
            for (int i = 0; i < _partTableDatas.Count;)
            {
                var item = _partTableDatas[i];
                if (item.part.assembleState == Base.EAssembleState.Assembled)
                {
                    table.RemoveData(item);
                    _partTableDatas.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }

            // 构成模块的所有子零件都完成了装配，模块本身未装配，则生成零件视图
            foreach (var m in repairTaskWork.modules)
            {
                if (m.ContainPart(partOfState) && m.assembleState == EAssembleState.Assembled && part.assembleState == EAssembleState.Disassembled)
                {
                    // 生成模块贴图
                    if (!string.IsNullOrEmpty(m.replacePartTypeTag))
                    {
                        var data = _partTableDatas.Find(d => d.tableDataCategoryName == m.replacePartTypeTag);
                        if (data != null)
                        {
                            ++data.count;
                            return;
                        }
                    }
                    m.gameObject.SetActive(false);

                    var partModel = new PartTableData_Model(m.interactPart, m.replacePartTypeTag);
                    _partTableDatas.Add(partModel);
                    if (table)
                    {
                        table.AddData(partModel);
                    }
                    break;
                }
            }
        }

        private void OnStepActive(StepGroup stepGroup, Step step) => UpdatePartTableDataValid(stepGroup, step, true);

        private void OnStepFinish(StepGroup stepGroup, Step step) => UpdatePartTableDataValid(stepGroup, step, false);

        private void UpdatePartTableDataValid(StepGroup stepGroup, Step step, bool valid)
        {
            if (stepGroup != repairTaskWork || !repairTaskWork.repairSteps.Contains(step)) return;

            var repairStep = step as RepairStep;
            if (valid)
            {
                CreateSockets(repairStep);
            }
            else
            {
                DeleteSockets();
            }

            foreach (var part in repairStep.selectedParts)
            {
                _partTableDatas.ForEach(d =>
                {
                    if (d.tableDataCategoryName == part.replacePartTypeTag || d.unityObject == part.interactPart)
                    {
                        d.interactable = valid;
                    }
                });
            }
        }

        /// <summary>
        /// 零件插槽，用于拖拽拼装
        /// </summary>
        private GameObjectSocket[] _partSockets = Empty<GameObjectSocket>.Array;

        private void CreateSockets(RepairStep repairStep)
        {
            var sockets = new List<GameObjectSocket>();
            foreach (var p in repairStep.selectedParts)
            {
                if (p)
                {
                    var partMono = p.interactPart;
                    if (partMono)
                    {
                        var partSocket = partMono.device.FindNearestEmptyPartSocket(partMono.replacePartTypeTag, partMono.transform.position);
                        if (partSocket != null)
                        {
                            sockets.Add(new GameObjectSocket(partMono.GetComponent<Grabbable>(), partMono.replacePartTypeTag, partSocket.GetWorldPose()));
                        }
                    }
                }
            }
            _partSockets = sockets.ToArray();
            GameObjectSocketCache.RegisterSockets(_partSockets);
        }

        private void DeleteSockets()
        {
            GameObjectSocketCache.UnregisterSockets(_partSockets);
            _partSockets = Empty<GameObjectSocket>.Array;
        }

        #endregion

        #region 零件表

        private void CreatePartTable()
        {
            // 设定拆装任务零件数量和完成拼装零件数量
            totalPartCount = repairTaskWork.parts.Count + repairTaskWork.modules.Count;
            finishPartCount = 0;

            // 设置零件非激活并更新其拆装态
            var device = InactiveParts();
            if (device)
            {
                device.CheckPartAssemblyState();
            }

            // 创建零件表数据
            CreatePartTableDatas();

            // 创建模块组, 因为组后面步骤动态创建的是图，因此需在零件视图创建之后
            //CreateModuleGroup();
        }

        private Tools.Device InactiveParts()
        {
            _gameObjectRecorder.Clear();

            Tools.Device device = null;
            foreach (var p in repairTaskWork.parts)
            {
                if (!p.gameObject) continue;

                _gameObjectRecorder.Record(p.gameObject);
                p.gameObject.SetActive(false);

                if (!device)
                {
                    device = p.interactPart.device;
                }
                p.assembleState = EAssembleState.None;
            }
            return device;
        }

        private void CreatePartTableDatas()
        {
            var firstPart = repairTaskWork.parts.FirstOrDefault();
            if (firstPart && firstPart.interactPart)
            {
                var device = firstPart.interactPart.device;
                if (device)
                {
                    if (device.partCategoryMap.Count == 0)
                    {
                        XCSJ.PluginRepairman.Tools.Module.CreateSockets(device);
                        device.CreatePartCategoryMap();
                    }
                    foreach (var item in device.partCategoryMap)
                    {
                        foreach (var part in item.Value)
                        {
                            _partTableDatas.Add(new PartTableData_Model(part, item.Key, firstPart.icon));
                        }
                    }
                }
            }
            table.AddDatas(_partTableDatas);
        }

        /// <summary>
        /// 删除零件表
        /// </summary>
        private void DeletePartTableDatas()
        {
            if (table)
            {
                table.RemoveDatas(_partTableDatas);
            }
            _partTableDatas.Clear();
        }

        #endregion
    }

    #endregion

    #region 零件表数据

    /// <summary>
    /// 零件表数据
    /// </summary>
    [Name("零件表数据")]
    public class PartTableData_Model : ComponentTableData_Model<Tools.Part>
    {
        /// <summary>
        /// 零件
        /// </summary>
        public Tools.Part part => unityObject;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="part"></param>
        public PartTableData_Model(Tools.Part part) : base(part) { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="part"></param>
        /// <param name="title"></param>
        /// <param name="texture2D"></param>
        /// <param name="interactable"></param>
        public PartTableData_Model(Tools.Part part, string title, Texture2D texture2D = null, bool interactable = false) : base(part, title, texture2D)
        {
            this.interactable = interactable;
        }
    } 
    #endregion
}
