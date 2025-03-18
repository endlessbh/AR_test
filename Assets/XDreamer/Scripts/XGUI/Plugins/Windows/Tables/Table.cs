using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Extension.Interactions.Base;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginTools.PropertyDatas;
using XCSJ.PluginTools.SelectionUtils;
using XCSJ.PluginXGUI.Base;
using XCSJ.Scripts;

namespace XCSJ.PluginXGUI.Windows.Tables
{
    #region 表数据命令

    /// <summary>
    /// 表数据命令枚举
    /// </summary>
    public enum ETableDataCmd
    {
        /// <summary>
        /// 点击
        /// </summary>
        [Name("点击")]
        Click,

        /// <summary>
        /// 拖拽开始
        /// </summary>
        [Name("拖拽开始")]
        DragStart,

        /// <summary>
        /// 拖拽进行中
        /// </summary>
        [Name("拖拽进行中")]
        Draging,

        /// <summary>
        /// 拖拽结束
        /// </summary>
        [Name("拖拽结束")]
        DragEnd,

        /// <summary>
        /// 添加
        /// </summary>
        [Name("添加")]
        Add,

        /// <summary>
        /// 移除
        /// </summary>
        [Name("移除")]
        Remove,

        /// <summary>
        /// 重新排序
        /// </summary>
        [Name("重新排序")]
        Sort,
    }

    /// <summary>
    /// 表数据命令
    /// </summary>
    [Serializable]
    public class TableDataCmd : Cmd<ETableDataCmd> { }

    /// <summary>
    /// 表数据命令列表
    /// </summary>
    [Serializable]
    public class TableDataCmds : Cmds<ETableDataCmd, TableDataCmd> { }

    #endregion

    #region 表

    /// <summary>
    /// 表
    /// </summary>
    [Name("表")]
    [XCSJ.Attributes.Icon(EIcon.List)]
    [DisallowMultipleComponent]
    [RequireManager(typeof(XGUIManager))]
    [Owner(typeof(XGUIManager))]
    [Tool(XGUICategory.Table, rootType = typeof(XGUIManager), index = IndexAttribute.DefaultIndex - 1)]
    public sealed class Table : Interactor
    {
        #region 表标签

        /// <summary>
        /// 表名称
        /// </summary>
        [PropertyKey]
        public const string TableName = "表名称";

        /// <summary>
        /// 表名
        /// </summary>
        public string tableName
        {
            get => _tagProperty.GetFirstValue(Table.TableName);
            set => _tagProperty.SetFirstValue(Table.TableName, value);
        }

        #endregion

        #region 交互输入

        /// <summary>
        /// 表数据命令列表
        /// </summary>
        [Name("表数据命令列表")]
        [OnlyMemberElements]
        public TableDataCmds _tableDataCmds = new TableDataCmds();

        /// <summary>
        /// 命令字符串
        /// </summary>
        public override List<string> cmds => _tableDataCmds.cmdNames;

        #endregion

        #region 属性

        /// <summary>
        /// 数据集合
        /// </summary>
        public List<TableData_Model> models
        {
            get
            {
                if (_models == null)
                {
                    _models = categoryList.Cast(c => c.firstModel).Where(m => m != null).ToList();
                    Sort();
                }
                return _models;
            }
        }
        private List<TableData_Model> _models = null;

        /// <summary>
        /// 表格数据数量
        /// </summary>
        public int tableDataCount => categoryList.Count;

        private List<TableData_ModelCategory> categoryList = new List<TableData_ModelCategory>();

        private List<TableProcessor> tableProcessors = new List<TableProcessor>();

        /// <summary>
        /// 数据处理规则
        /// </summary>
        [Group("模型设置", textEN = "Model Settings")]
        [Name("数据处理规则")]
        [EnumPopup]
        public EDataHandleRule _modelHandleRule = (EDataHandleRule)(-1);

        /// <summary>
        /// 数据处理规则
        /// </summary>
        [Flags]
        public enum EDataHandleRule
        {
            /// <summary>
            /// 添加时非激活游戏对象
            /// </summary>
            [Name("添加时非激活游戏对象")]
            InactiveGameObjectOnAdd = 1 << 1,

            /// <summary>
            /// 添加时取消选择游戏对象
            /// </summary>
            [Name("添加时取消选择游戏对象")]
            UnselectGameObjectOnAdd = 1 << 2,

            /// <summary>
            /// 移除时激活游戏对象
            /// </summary>
            [Name("移除时激活游戏对象")]
            ActiveGameObjectOnRemove = 1 << 3,

            /// <summary>
            /// 拖拽结束后移除表数据
            /// </summary>
            [Name("拖拽结束后移除表数据")]
            RemoveTableDataOnEndDrag = 1 << 4,
        }

        /// <summary>
        /// 排序信息列表
        /// </summary>
        [Name("排序信息列表")]
        public SortInfo[] _sortInfos = new SortInfo[] { new SortInfo() };

        private bool reflashTableData = true;

        #endregion

        #region Unity 消息

        /// <summary>
        /// 重置命令
        /// </summary>
        public void Reset()
        {
            _tableDataCmds.Reset();

            if (template) { }

            _tagProperty._tagPropertyDatas.Add(new TagPropertyData(TableName, name));
        }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            var gridLayoutGroup = GetComponentInChildren<GridLayoutGroup>();
            if (gridLayoutGroup)
            {
                // 初始化网格行列设置
                gridLayoutGroup.constraint = (GridLayoutGroup.Constraint)_rowColumnConstraint;
                switch (_rowColumnConstraint)
                {
                    case ERowColumnConstraint.FixedColumnCount: gridLayoutGroup.constraintCount = _columnCount; break;
                    case ERowColumnConstraint.FixedRowCount: gridLayoutGroup.constraintCount = _rowCount; break;
                }
            }
            else
            {
                enabled = false;
                Debug.LogErrorFormat("【{0}】表子级游戏对象缺少【{1}】类型组件！", CommonFun.ObjectToString(this), nameof(GridLayoutGroup));
                return;
            }

            if (tableDataPool != null) { }
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
        }

        #endregion

        #region 交互处理

        /// <summary>
        /// 注册表数据集合提供器
        /// </summary>
        /// <param name="tableProcessor"></param>
        public void AddTableProcessor(TableProcessor tableProcessor) => tableProcessors.Add(tableProcessor);

        /// <summary>
        /// 注销表数据集合提供器
        /// </summary>
        /// <param name="tableProcessor"></param>
        public void RemoveTableProcessor(TableProcessor tableProcessor) => tableProcessors.Remove(tableProcessor);

        /// <summary>
        /// 能否交互
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        public override bool CanInteract(InteractData interactData)
        {
            if (_tableDataCmds.TryGetECmd(interactData.cmdName, out var eCmd))
            {
                if (eCmd == ETableDataCmd.Sort)
                {
                    return tableDataCount > 0;
                }
            }
            return tableProcessors.Exists(p => p.CanInteract(interactData));
        }

        /// <summary>
        /// 交互
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        protected override EInteractResult OnInteract(InteractData interactData)
        {
            if (_tableDataCmds.TryGetECmd(interactData.cmdName, out var eCmd))
            {
                if (eCmd == ETableDataCmd.Sort)// 排序命令不需要【TableInteractData】类型交互数据即可执行
                {
                    return TryReflashTableData() ? EInteractResult.Finished : EInteractResult.Aborted;
                }
                else// 非排序命令依赖传入的交互数据类型为【TableInteractData】
                {
                    TableInteractData tableInteractData = interactData as TableInteractData;
                    if (tableInteractData == null)// 转换交互数据
                    {
                        tableInteractData = new TableInteractData(this, new ComponentTableData_Model(interactData.interactable), interactData.cmdName, interactData, interactData.interactables);
                        TryInteractDelay(tableInteractData);
                    }
                    else
                    {
                        switch (eCmd)
                        {
                            case ETableDataCmd.Click:
                                {
                                    foreach (var item in tableProcessors)
                                    {
                                        item.OnClick(tableInteractData);
                                    }
                                    return EInteractResult.Finished;
                                }
                            case ETableDataCmd.DragStart:
                                {
                                    foreach (var item in tableProcessors)
                                    {
                                        item.OnDragStart(tableInteractData);
                                    }
                                    return EInteractResult.Finished;
                                }
                            case ETableDataCmd.Draging:
                                {
                                    foreach (var item in tableProcessors)
                                    {
                                        item.OnDrag(tableInteractData);
                                    }
                                    return EInteractResult.Finished;
                                }
                            case ETableDataCmd.DragEnd:
                                {
                                    foreach (var item in tableProcessors)
                                    {
                                        item.OnDragEnd(tableInteractData);
                                    }
                                    HandleOnEndDrag(tableInteractData);
                                    return EInteractResult.Finished;
                                }
                            case ETableDataCmd.Add:
                                {
                                    if (AddDataInternal(tableInteractData.tableData_Model))
                                    {
                                        foreach (var item in tableProcessors)
                                        {
                                            item.OnAdd(tableInteractData);
                                        }

                                        HandleOnAdd(tableInteractData);
                                        TryReflashTableData();
                                        return EInteractResult.Finished;
                                    }

                                    return EInteractResult.Aborted;
                                }
                            case ETableDataCmd.Remove:
                                {
                                    if (RemoveDataInteranal(tableInteractData.tableData_Model))
                                    {
                                        foreach (var item in tableProcessors)
                                        {
                                            item.OnRemove(tableInteractData);
                                        }

                                        HandleOnRemove(tableInteractData);
                                        TryReflashTableData();
                                        return EInteractResult.Finished;
                                    }
                                    return EInteractResult.Aborted;
                                }
                        }
                    }
                }

            }
            return base.OnInteract(interactData);
        }

        private EInteractResult TryInteractInternal(TableData_Model tableData_Model, ETableDataCmd cmd)
        {
            InteractableEntity interactableEntity = null;
            if (tableData_Model is IGameObjectGetter gameObjectGetter && gameObjectGetter.gameObject)
            {
                interactableEntity = gameObjectGetter.gameObject.GetComponent<InteractableEntity>();
            }
            TryInteract(new TableInteractData(this, tableData_Model, cmd, interactableEntity), out var result);
            return result;
        }

        /// <summary>
        /// 包含数据
        /// </summary>
        /// <param name="tableData_Model"></param>
        /// <returns></returns>
        public bool Contains(TableData_Model tableData_Model)
        {
            return categoryList.Exists(c => c.Contains(tableData_Model));
        }

        /// <summary>
        /// 添加表数据
        /// </summary>
        /// <param name="tableData_Models"></param>
        public void AddDatas(IEnumerable<TableData_Model> tableData_Models)
        {
            try
            {
                reflashTableData = false;// 暂停刷新界面
                foreach (var item in tableData_Models)
                {
                    AddData(item);
                }
            }
            finally
            {
                reflashTableData = true;
                TryReflashTableData();
            }
        }

        /// <summary>
        /// 添加表数据
        /// </summary>
        /// <param name="component"></param>
        /// <param name="title"></param>
        /// <param name="texture2D"></param>
        public void AddData(Component component, string title = "", Texture2D texture2D = null)
        {
            if (!component) return;
            AddData(new ComponentTableData_Model(component, title, texture2D));
        }

        /// <summary>
        /// 添加表数据
        /// </summary>
        /// <param name="tableData_Model"></param>
        public EInteractResult AddData(TableData_Model tableData_Model) => TryInteractInternal(tableData_Model, ETableDataCmd.Add);

        private bool AddDataInternal(TableData_Model tableData_Model)
        {
            if (tableData_Model == null || Contains(tableData_Model)) return false;

            var category = categoryList.Find(g => g.IsSameCategory(tableData_Model));
            if (category == null)
            {
                category = new TableData_ModelCategory();
                categoryList.Add(category);
            }
            category.Add(tableData_Model);

            return true;
        }

        /// <summary>
        /// 移除表数据
        /// </summary>
        /// <param name="tableDatas"></param>
        public void RemoveDatas(IEnumerable<TableData_Model> tableDatas)
        {
            try
            {
                reflashTableData = false;// 暂停刷新界面
                foreach (var item in tableDatas)
                {
                    RemoveData(item);
                }
            }
            finally
            {
                reflashTableData = true;
                TryReflashTableData();
            }
        }

        /// <summary>
        /// 移除表数据
        /// </summary>
        /// <param name="tableData"></param>
        public EInteractResult RemoveData(TableData_Model tableData_Model) => TryInteractInternal(tableData_Model, ETableDataCmd.Remove);

        private bool RemoveDataInteranal(TableData_Model tableData_Model)
        {
            if (tableData_Model == null || !Contains(tableData_Model)) return false;

            var category = categoryList.Find(g => g.Contains(tableData_Model));
            if (category != null && category.Remove(tableData_Model))
            {
                if (category.count == 0)
                {
                    categoryList.Remove(category);
                }
                return true;
            }
            return false;
        }

        public bool TryReflashTableData()
        {
            if (reflashTableData)
            {
                _models = null;
                ReflashTableData();
                return true;
            }
            return false;
        }

        /// <summary>
        /// 清除表数据
        /// </summary>
        public void Clear()
        {
            RemoveDatas(new List<TableData_Model>(models));

            models.Clear();
        }

        private void Sort() => models.Sort((x, y) => SortCompare(x, y));

        private int SortCompare(TableData_Model x, TableData_Model y)
        {
            foreach (var info in _sortInfos)
            {
                var rs = info.Compare(x, y);
                if (rs != 0)
                {
                    return rs;
                }
            }
            return 0;
        }

        private void HandleOnEndDrag(TableInteractData tableInteractData)
        {
            if (!CommonFun.IsOnUGUI())
            {
                if ((_modelHandleRule & EDataHandleRule.RemoveTableDataOnEndDrag) == EDataHandleRule.RemoveTableDataOnEndDrag)
                {
                    TableInteractData removeData = tableInteractData.Clone() as TableInteractData;
                    removeData.SetCmd(_tableDataCmds.GetCmdName(ETableDataCmd.Remove), this, ETableDataCmd.Remove);
                    TryInteractDelay(removeData);
                }
            }
        }

        private void HandleOnAdd(TableInteractData tableInteractData)
        {
            // 取消选择
            if ((_modelHandleRule & EDataHandleRule.UnselectGameObjectOnAdd) == EDataHandleRule.UnselectGameObjectOnAdd)
            {
                if (tableInteractData.tableData_Model is IGameObjectGetter getter && getter.gameObject)
                {
                    foreach (var selectionModify in ComponentCache.GetComponents<SelectionModify>(false))
                    {
                        if (selectionModify._currentSelection == getter.gameObject)
                        {
                            selectionModify.UnselectCurrent();
                        }
                    }
                }
            }

            // 非激活游戏对象
            if ((_modelHandleRule & EDataHandleRule.InactiveGameObjectOnAdd) == EDataHandleRule.InactiveGameObjectOnAdd)
            {
                if (tableInteractData.tableData_Model is IGameObjectGetter getter && getter.gameObject)
                {
                    getter.gameObject.SetActive(false);
                }
            }
        }

        private void HandleOnRemove(TableInteractData tableInteractData)
        {
            // 激活游戏对象
            if (tableInteractData.tableData_Model is IGameObjectGetter getter)
            {
                var go = getter.gameObject;
                if (go)
                {
                    if ((_modelHandleRule & EDataHandleRule.ActiveGameObjectOnRemove) == EDataHandleRule.ActiveGameObjectOnRemove)
                    {
                        go.SetActive(true);
                    }

                    // 尝试创建新的表数据提供器，并将模型信息绑定到其中
                    if (tableInteractData.tableData_Model is IComponentGetter componentGetter && componentGetter.component)
                    {
                        TryCreateNewTableDataProviderWithModel(go, tableInteractData.tableData_Model, componentGetter.component);
                    }
                }
            }
        }

        private void TryCreateNewTableDataProviderWithModel(GameObject go, TableData_Model model, Component component)
        {
            var tableDataProviders = go.GetComponents<TableDataProvider>();
            foreach (var item in tableDataProviders)
            {
                if (item._componentTableData_Model == model || item._componentTableData_Model.unityObject == component)
                {
                    return;
                }
            }

            var oldActive = go.activeSelf;
            go.SetActive(false);
            var newProvider = go.AddComponent<TableDataProvider>();
            if (newProvider)
            {
                newProvider.Reset();
                newProvider.enabled = false;
                newProvider.needAddModel = false;
                newProvider.tableName = _tagProperty.firstValue;
                newProvider._componentTableData_Model.title = model.title;
                newProvider._componentTableData_Model.texture2D = model.texture2D;
                newProvider._componentTableData_Model._unityObject = component;
            }
            go.SetActive(oldActive);
        }

        #endregion

        #region UI控制

        [Group("视图设置", textEN = "View Settings")]
        /// <summary>
        /// 表数据模板
        /// </summary>
        [Name("表数据模板")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [Readonly(EEditorMode.Runtime)]
        public TableData _template;

        /// <summary>
        /// 表单元格模板
        /// </summary>
        private TableData template => this.XGetComponentInChildrenOrGlobal(ref _template);

        /// <summary>
        /// 表单元格选中色
        /// </summary>
        [Name("表单元格选中色")]
        public Color _selectedColor = new Color(1, 0.4f, 0, 1);// 当前值与Unity编辑器选中对象颜色保持一致

        /// <summary>
        /// 表行列约束枚举：与GridLayoutGroup的Constraint保持一致
        /// </summary>
        public enum ERowColumnConstraint
        {
            [Name("弹性")]
            Flexible,

            [Name("固定列数")]
            FixedColumnCount,

            [Name("固定行数")]
            FixedRowCount
        }

        /// <summary>
        /// 行列数约束规则
        /// </summary>
        [Name("行列数约束规则")]
        [EnumPopup]
        public ERowColumnConstraint _rowColumnConstraint = ERowColumnConstraint.FixedColumnCount;

        /// <summary>
        /// 行数
        /// </summary>
        [Name("行数")]
        [Min(1)]
        [HideInSuperInspector(nameof(_rowColumnConstraint), EValidityCheckType.NotEqual, ERowColumnConstraint.FixedRowCount)]
        public int _rowCount = 1;

        /// <summary>
        /// 列数
        /// </summary>
        [Name("列数")]
        [Min(1)]
        [HideInSuperInspector(nameof(_rowColumnConstraint), EValidityCheckType.NotEqual, ERowColumnConstraint.FixedColumnCount)]
        public int _columnCount = 1;

        /// <summary>
        /// 表单元格缓存池
        /// </summary>
        private WorkObjectPool<TableData> tableDataPool
        {
            get
            {
                if (_tableDataPool == null && _template)
                {
                    _tableDataPool = new WorkObjectPool<TableData>();
                    _template.gameObject.SetActive(false);

                    _tableDataPool.Init(() =>
                        {
                            TableData newItem = null;
                            if (_template)
                            {
                                var newGO = _template.gameObject.XCloneAndSetParent(_template.transform.parent);
                                newGO.transform.localScale = _template.transform.localScale;
                                newItem = newGO.GetComponent<TableData>();
                                newItem.table = this;
                            }
                            return newItem;
                        },
                        item =>
                        {
                            if (item)
                            {
                                item.gameObject.SetActive(true);
                            }
                        },
                        item =>
                        {
                            if (item)
                            {
                                item.model = null;
                                item.gameObject.SetActive(false);
                            }
                        },
                        item => item);
                }
                return _tableDataPool;
            }
        }
        private WorkObjectPool<TableData> _tableDataPool = null;

        private void ReflashTableData()
        {
            tableDataPool.Clear();

            foreach (var model in models)
            {
                CreateTableData(model);
            }
        }

        private void CreateTableData(TableData_Model tableData_Model)
        {
            // 分配表UI
            var tableData = tableDataPool.FindOrAlloc(t => t.model == tableData_Model);
            if (tableData)
            {
                tableData.model = tableData_Model;

                // 根据数据顺序，对视图进行排序
                for (int i = 0; i < tableDataPool.workObjects.Count; i++)
                {
                    var item = tableDataPool.workObjects[i];
                    item.transform.SetSiblingIndex(models.IndexOf(item.model));
                }
            }
        }

        private void RemoveTableData(TableData_Model tableData_Model)
        {
            tableDataPool?.Free(td => td ? td.model == tableData_Model : false);
        }

        private void UpdateTableData(TableData_Model tableData_Model)
        {
            var td = tableDataPool.Find(t => t.model.IsSameCategory(tableData_Model));
            if (td != null)
            {
                td.UpdateData();
            }
        }

        #endregion
    }

    #endregion

    #region TableInteractData

    /// <summary>
    /// 表交互数据
    /// </summary>
    public class TableInteractData : InteractData<TableInteractData>
    {
        /// <summary>
        /// 指针事件数据
        /// </summary>
        public PointerEventData pointerEventData { get; private set; }

        /// <summary>
        /// 表
        /// </summary>
        public Table table { get; private set; }

        /// <summary>
        /// 表数据
        /// </summary>
        public TableData tableData { get; private set; }

        /// <summary>
        /// 表数据模型
        /// </summary>
        public TableData_Model tableData_Model { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public TableInteractData() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="table"></param>
        /// <param name="tableData"></param>
        /// <param name="tableData_Model"></param>
        /// <param name="cmd"></param>
        /// <param name="interactables"></param>
        public TableInteractData(Table table, TableData tableData, TableData_Model tableData_Model, ETableDataCmd cmd, params InteractObject[] interactables) : this(table, tableData, tableData_Model, table._tableDataCmds.GetCmdName(cmd), interactables)
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="pointerEventData"></param>
        /// <param name="table"></param>
        /// <param name="tableData"></param>
        /// <param name="tableData_Model"></param>
        /// <param name="cmd"></param>
        /// <param name="interactables"></param>
        public TableInteractData(PointerEventData pointerEventData, Table table, TableData tableData, TableData_Model tableData_Model, ETableDataCmd cmd, params InteractObject[] interactables) : this(pointerEventData, table, tableData, tableData_Model, table._tableDataCmds.GetCmdName(cmd), interactables)
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="table"></param>
        /// <param name="tableData_Model"></param>
        /// <param name="cmd"></param>
        /// <param name="interactables"></param>
        public TableInteractData(Table table, TableData_Model tableData_Model, ETableDataCmd cmd, params InteractObject[] interactables) : this(table, tableData_Model, table._tableDataCmds.GetCmdName(cmd), interactables)
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="table"></param>
        /// <param name="tableData_Model"></param>
        /// <param name="cmdName"></param>
        /// <param name="interactables"></param>
        public TableInteractData(Table table, TableData_Model tableData_Model, string cmdName, params InteractObject[] interactables) : this(table, tableData_Model, cmdName, null, interactables)
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="table"></param>
        /// <param name="tableData_Model"></param>
        /// <param name="cmdName"></param>
        /// <param name="parent"></param>
        /// <param name="interactables"></param>
        public TableInteractData(Table table, TableData_Model tableData_Model, string cmdName, InteractData parent, params InteractObject[] interactables) : base(cmdName, parent, table, interactables)
        {
            this.tableData_Model = tableData_Model;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="table"></param>
        /// <param name="tableData"></param>
        /// <param name="tableData_Model"></param>
        /// <param name="cmdName"></param>
        /// <param name="interactables"></param>
        public TableInteractData(Table table, TableData tableData, TableData_Model tableData_Model, string cmdName, params InteractObject[] interactables) : this(table, tableData_Model, cmdName, interactables)
        {
            this.table = table;
            this.tableData = tableData;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="pointerEventData"></param>
        /// <param name="table"></param>
        /// <param name="tableData"></param>
        /// <param name="tableData_Model"></param>
        /// <param name="cmdName"></param>
        /// <param name="interactables"></param>
        public TableInteractData(PointerEventData pointerEventData, Table table, TableData tableData, TableData_Model tableData_Model, string cmdName, params InteractObject[] interactables) : this(table, tableData, tableData_Model, cmdName, interactables)
        {
            this.pointerEventData = pointerEventData;
        }

        /// <summary>
        /// 拷贝构造函数
        /// </summary>
        /// <param name="interactData"></param>
        public override void CopyTo(InteractData interactData)
        {
            base.CopyTo(interactData);

            if (interactData is TableInteractData tableInteractData)
            {
                tableInteractData.pointerEventData = pointerEventData;
                tableInteractData.table = table;
                tableInteractData.tableData = tableData;
                tableInteractData.tableData_Model = tableData_Model;
            }
        }
    }

    #endregion

    #region TableProcessor

    /// <summary>
    /// 基础表处理器
    /// </summary>
    [RequireManager(typeof(XGUIManager))]
    [Owner(typeof(XGUIManager))]
    public abstract class BaseTableProcessor : Interactor
    {
        private Table _table;

        /// <summary>
        /// 视图项数据列表
        /// </summary>
        protected Table table
        {
            get
            {
                if (!_table)
                {
                    _table = this.XGetOrAddComponent<Table>();
                }
                return _table;
            }
        }
    }

    /// <summary>
    /// 表处理器
    /// </summary>
    [XCSJ.Attributes.Icon(EIcon.List)]
    [RequireComponent(typeof(Table))]
    [Tool(XGUICategory.Table, nameof(TableProcessor), rootType = typeof(XGUIManager), index = IndexAttribute.DefaultIndex - 1)]
    public abstract class TableProcessor : BaseTableProcessor
    {
        /// <summary>
        /// 启用时预加载模型
        /// </summary>
        protected virtual IEnumerable<TableData_Model> prefabModels => Empty<TableData_Model>.Array;

        private bool loadPrefabModel = true;

        /// <summary>
        /// 表格处理器管理的模型对象
        /// </summary>
        protected List<TableData_Model> models = new List<TableData_Model>();

        /// <summary>
        /// 当前选择表数据模型
        /// </summary>
        protected TableData_Model currentSelectedModel = null;

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            table.AddTableProcessor(this);

            if (loadPrefabModel)
            {
                loadPrefabModel = false;
                models.AddRangeWithDistinct(prefabModels);
            }
            table.AddDatas(models);
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            table.RemoveTableProcessor(this);
        }

        /// <summary>
        /// 能否交互
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        public override bool CanInteract(InteractData interactData) => interactData is TableInteractData tableInteractData;

        /// <summary>
        /// 点击
        /// </summary>
        /// <param name="tableInteractData"></param>
        internal virtual void OnClick(TableInteractData tableInteractData) => SelectModel(tableInteractData);

        /// <summary>
        /// 选择模型
        /// </summary>
        /// <param name="tableInteractData"></param>
        protected virtual void SelectModel(TableInteractData tableInteractData)
        {
            if (!models.Contains(tableInteractData.tableData_Model) || currentSelectedModel == tableInteractData.tableData_Model) return;

            // 上次记录选择模型设置为不选中
            if (currentSelectedModel != null)
            {
                currentSelectedModel.selected = false;
            }

            // 将当前模型设置为选中
            currentSelectedModel = tableInteractData.tableData_Model;
            currentSelectedModel.selected = true;
        }

        /// <summary>
        /// 拖拽开始
        /// </summary>
        /// <param name="tableInteractData"></param>
        internal virtual void OnDragStart(TableInteractData tableInteractData) { }

        /// <summary>
        /// 拖拽中
        /// </summary>
        /// <param name="tableInteractData"></param>
        internal virtual void OnDrag(TableInteractData tableInteractData) { }

        /// <summary>
        /// 拖拽结束
        /// </summary>
        /// <param name="tableInteractData"></param>
        internal virtual void OnDragEnd(TableInteractData tableInteractData) { }

        /// <summary>
        /// 添加回调
        /// </summary>
        /// <param name="tableInteractData"></param>
        internal virtual void OnAdd(TableInteractData tableInteractData) { }

        /// <summary>
        /// 移除回调
        /// </summary>
        /// <param name="tableInteractData"></param>
        internal virtual void OnRemove(TableInteractData tableInteractData)
        {
            models.Remove(tableInteractData.tableData_Model);
        }
    }

    #endregion
}
