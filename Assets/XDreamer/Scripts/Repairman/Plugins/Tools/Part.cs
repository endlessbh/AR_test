using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginRepairman.Base;
using XCSJ.PluginTools;
using XCSJ.PluginTools.Draggers;
using XCSJ.PluginTools.Items;
using XCSJ.PluginTools.PropertyDatas;
using XCSJ.PluginXGUI.Windows.Tables;
using XCSJ.Scripts;

namespace XCSJ.PluginRepairman.Tools
{
    /// <summary>
    /// 零件
    /// 1、具有分类名称属性
    /// 2、继承可抓对象，天然具有抓放功能
    /// 3、在机械组织层级中处于叶子层级上，其下不能再存在零件
    /// 4、当零件与插槽位置超过吸附距离时为拆卸态，或者零件所在游戏对象非激活时，也为拆卸态
    /// </summary>
    [Name("零件")]
    [RequireManager(typeof(RepairmanManager))]
    [Owner(typeof(RepairmanManager))]
    [Tool(RepairmanHelperExtension.DataModelStateLibName, nameof(InteractableVirtual), rootType = typeof(ToolsManager))]
    public class Part : GrabbableUser, ITableDataCategoryName
    {
        #region 替换零件种类标签

        /// <summary>
        /// 替换零件种类标签
        /// </summary>
        [PropertyKey]
        public const string ReplacePartTypeTag = "可替换零件种类标签";

        /// <summary>
        /// 分类名称
        /// </summary>
        public string replacePartTypeTag => _tagProperty.GetFirstValue(ReplacePartTypeTag);

        /// <summary>
        /// 视图中的分类
        /// </summary>
        public string tableDataCategoryName => replacePartTypeTag;

        #endregion

        #region 零件组织

        /// <summary>
        /// 所属模块
        /// </summary>
        public Module module => this.XGetComponentInParent(ref _module);
        private Module _module;

        /// <summary>
        /// 设备
        /// </summary>
        public Device device => this.XGetComponentInParent(ref _device);
        private Device _device;

        /// <summary>
        /// 零件插槽
        /// </summary>
        internal PartSocket partSocket { get; set; }

        /// <summary>
        /// 能否抓:有父级，并且当前零件被装配约束，则使用父级对象进行拖拽
        /// </summary>
        /// <param name="dragger"></param>
        /// <returns></returns>
        protected virtual bool CanGrabbed(Dragger dragger)
        {
            return canDisassembly;
        }

        #endregion

        #region Unity 消息

        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            _tagProperty._tagPropertyDatas.Add(new TagPropertyData(ReplacePartTypeTag, name));
        }

        /// <summary>
        /// 开始
        /// </summary>
        protected virtual void Start()
        {
            // 零件父级没有模块
            if (!module) { }
        }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
        }

        #endregion

        #region 装配状态及约束关系

        /// <summary>
        /// 零件状态
        /// </summary>
        [Name("零件状态")]
        [EnumPopup]
        [Readonly]
        public EAssembleState _assembleState = EAssembleState.None;

        /// <summary>
        /// 零件状态
        /// </summary>
        public virtual EAssembleState assembleState
        {
            get => _assembleState;
            internal set
            {
                if (_assembleState != value)
                {
                    var oldState = _assembleState;
                    _assembleState = value;
                    onAssembleStateChanged?.Invoke(this, oldState);
                }
            }
        }

        public static event Action<Part, EAssembleState> onAssembleStateChanged;

        /// <summary>
        /// 能否装配：当装配约束零件都装配，当前零件才能装配
        /// </summary>
        public bool canAssembly => assemblyParts.Count == 0 || assemblyParts.All(p => p.assembleState == EAssembleState.Assembled);

        /// <summary>
        /// 装配约束零件列表：只有列表内所有零件状态都为装配，当前零件才能进行装配
        /// </summary>
        public List<Part> assemblyParts { get; private set; } = new List<Part>();

        /// <summary>
        /// 能否拆卸：当拆卸约束零件都已拆下，当前零件才能拆卸
        /// </summary>
        public bool canDisassembly => disassemblyParts.Count == 0 || disassemblyParts.All(p => p.assembleState == EAssembleState.Disassembled);

        /// <summary>
        /// 拆卸约束零件列表：只有列表内所有零件状态都为拆卸，当前零件才能拆卸
        /// </summary>
        public List<Part> disassemblyParts { get; private set; } = new List<Part>();

        /// <summary>
        /// 重置数据
        /// </summary>
        public void ResetData()
        {
            assembleState = EAssembleState.None;
            assemblyParts.Clear();
            disassemblyParts.Clear();
        }

        #endregion
    }
}
