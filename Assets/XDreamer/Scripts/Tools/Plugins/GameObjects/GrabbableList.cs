using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Extension.Base.Recorders;
using XCSJ.Extension.Interactions.Base;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginCommonUtils.Runtime;
using XCSJ.PluginsCameras;
using XCSJ.PluginTools.Draggers;
using XCSJ.PluginTools.Items;
using XCSJ.PluginTools.SelectionUtils;
using XCSJ.PluginXGUI.Windows.Tables;
using Dragger = XCSJ.PluginTools.Draggers.Dragger;

namespace XCSJ.PluginTools.GameObjects
{
    #region 可抓对象列表命令

    /// <summary>
    /// 可抓对象列表命令枚举
    /// </summary>
    public enum EGrabbableListCmd
    {
        [Name("拾取选择游戏对象")]
        PickSelectedGameObject,
    }

    /// <summary>
    /// 可抓对象列表命令
    /// </summary>
    [Serializable]
    public class GrabbableListCmd : Cmd<EGrabbableListCmd> { }

    /// <summary>
    /// 可抓对象列表命令列表
    /// </summary>
    [Serializable]
    public class GrabbableListCmds : Cmds<EGrabbableListCmd, GrabbableListCmd> { }

    #endregion

    #region 可抓对象列表

    /// <summary>
    /// 可抓对象列表
    /// </summary>
    [Name("可抓对象列表")]
    [DisallowMultipleComponent]
    [RequireManager(typeof(ToolsManager))]
    public class GrabbableList : TableProcessor
    {
        /// <summary>
        /// 命令列表
        /// </summary>
        [Name("命令列表")]
        [OnlyMemberElements]
        public GrabbableListCmds _grabbableListCmds = new GrabbableListCmds();

        /// <summary>
        /// 可抓对象创建器列表
        /// </summary>
        [Name("可抓对象创建器列表")]
        public List<CloneGrabbableTableDataMaker> _cloneGrabbableTableDataMakers = new List<CloneGrabbableTableDataMaker>();

        /// <summary>
        /// 启用时加载数据
        /// </summary>
        protected override IEnumerable<TableData_Model> prefabModels
        {
            get
            {
                var list = new List<TableData_Model>();
                foreach (var maker in _cloneGrabbableTableDataMakers)
                {
                    list.AddRange(maker.Generate());
                }
                return list;
            }
        }

        /// <summary>
        /// 重置
        /// </summary>
        public void Reset() => _grabbableListCmds.Reset();

        /// <summary>
        /// 能否交互
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        public override bool CanInteract(InteractData interactData)
        {
            if (_grabbableListCmds.TryGetECmd(interactData.cmdName, out var eCmd))
            {
                switch (eCmd)
                {
                    case EGrabbableListCmd.PickSelectedGameObject: return true;
                }
            }
            return base.CanInteract(interactData) && (interactData as TableInteractData).tableData_Model is DraggableTableData_Model;
        }

        /// <summary>
        /// 交互
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        protected override EInteractResult OnInteract(InteractData interactData)
        {
            if (_grabbableListCmds.TryGetECmd(interactData.cmdName, out var eCmd))
            {
                switch (eCmd)
                {
                    case EGrabbableListCmd.PickSelectedGameObject:
                        {
                            return PickSelectedGameObjectsInternal() ? EInteractResult.Finished : EInteractResult.Aborted;
                        }
                }
            }
            return base.OnInteract(interactData);
        }

        /// <summary>
        /// 点击
        /// </summary>
        /// <param name="viewItemData"></param>
        internal override void OnClick(TableInteractData viewItemData) { }

        private Dragger dragger = null;
        private SelectionModify selectionModify = null;
        private GameObjectRecorder gameObjectRecorder = new GameObjectRecorder();

        /// <summary>
        /// 开始拖拽
        /// </summary>
        /// <param name="tableInteractData"></param>
        internal override void OnDragStart(TableInteractData tableInteractData)
        {
            base.OnDragStart(tableInteractData);

            if (tableInteractData.tableData_Model is DraggableTableData_Model model && model is IGrabbableGetter getter && getter.grabbable)
            {
                selectionModify = ComponentCache.GetComponent<SelectionModify>(false);
                if (!selectionModify)
                {
                    Debug.LogErrorFormat("[{0}]未找到有效的选择集修改器!", CommonFun.ObjectToString(this));
                    return;
                }

                dragger = null;
                var t = selectionModify.transform;
                while (t)
                {
                    dragger = t.GetComponentInChildren<Dragger>(false);
                    if (dragger) break;
                    t = t.parent;
                }
                if (!dragger)
                {
                    Debug.LogErrorFormat("[{0}]未找到有效的主动拖拽器!", CommonFun.ObjectToString(this));
                    return;
                }
                dragger.ResetData();

                var grabbable = getter.grabbable;

                // 设置初始位置为相机后方
                var cam = CameraHelperExtension.currentCamera;
                if (cam)
                {
                    grabbable.transform.position = cam.transform.position - cam.transform.forward * 5000;
                }

                // 激活游戏对象
                gameObjectRecorder.Record(grabbable.gameObject);
                getter.grabbable.gameObject.SetActive(true);

                // 设置为选中状态
                selectionModify.Select(grabbable.gameObject);

                // 启用拖拽
                dragger.TryInteract(dragger._grabCmds.GetCmdName(EGrabCmd.Grab), out var result, grabbable);

                model.OnDragStart(tableInteractData);
            }
        }

        /// <summary>
        /// 拖拽中
        /// </summary>
        /// <param name="tableInteractData"></param>
        internal override void OnDrag(TableInteractData tableInteractData)
        {
            base.OnDrag(tableInteractData);

            // 通过外部【鼠标输入】产生【保持】命令（因为需要获取射线对象）
            if (dragger && tableInteractData.tableData_Model is DraggableTableData_Model model)
            {
                model.OnDrag(tableInteractData);
            }
        }

        /// <summary>
        /// 结束拖拽
        /// </summary>
        /// <param name="tableInteractData"></param>
        internal override void OnDragEnd(TableInteractData tableInteractData)
        {
            base.OnDragEnd(tableInteractData);

            if (dragger && tableInteractData.tableData_Model is DraggableTableData_Model model && model is IGrabbableGetter getter && getter.grabbable)
            {
                // 结束拖拽交互
                dragger.TryInteract(dragger._grabCmds.GetCmdName(EGrabCmd.Release), out var result, getter.grabbable);

                // 反选
                selectionModify.UnselectCurrent();

                model.OnDragEnd(tableInteractData);
            }
            dragger = null;

            if (CommonFun.IsOnUI())
            {
                gameObjectRecorder.Recover();
                gameObjectRecorder.Clear();
            }
        }

        /// <summary>
        /// 拾取选中游戏对象
        /// </summary>
        public void PickSelectedGameObjects() => PickSelectedGameObjectsInternal();

        private bool PickSelectedGameObjectsInternal()
        {
            bool result = false;
            foreach (var go in Selection.selections)
            {
                if (go)
                {
                    var grabbable = go.GetComponent<Grabbable>();
                    if (grabbable)
                    {
                        EInteractResult addResult = EInteractResult.None;
                        var provider = go.GetComponents<TableDataProvider>().Find(item => item._componentTableData_Model._unityObject == grabbable);
                        if (provider)
                        {
                            addResult = table.AddData(new GrabbableTableData_Model(grabbable, provider._componentTableData_Model.title, provider._componentTableData_Model.texture2D));
                        }
                        else
                        {
                            addResult = table.AddData(new GrabbableTableData_Model(grabbable));
                        }

                        if (!result && addResult == EInteractResult.Finished)
                        {
                            result = true;
                        }
                    }
                }
            }
            return result;
        }
    }

    #endregion

    #region 可抓表数据模型

    /// <summary>
    /// 可抓对象
    /// </summary>
    public class GrabbableTableData_Model : ComponentTableData_Model<Grabbable>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="grabbable"></param>
        public GrabbableTableData_Model(Grabbable grabbable) : base(grabbable) { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="grabbable"></param>
        /// <param name="title"></param>
        /// <param name="texture2D"></param>
        public GrabbableTableData_Model(Grabbable grabbable, string title, Texture2D texture2D = null) : base(grabbable, title, texture2D) { }
    }

    /// <summary>
    /// 克隆类型可抓对象:在容器中，并不实行真实克隆。只有从容器中移出才克隆可抓对象
    /// </summary>
    public class CloneGrabbableTableData_Model : GrabbableTableData_Model
    {
        /// <summary>
        /// 克隆原型
        /// </summary>
        public Grabbable prototype { get; private set; }

        /// <summary>
        /// 构造函数:克隆原型不会赋值给基类的component对象
        /// </summary>
        /// <param name="prototype"></param>
        /// <param name="title"></param>
        /// <param name="texture2D"></param>
        public CloneGrabbableTableData_Model(Grabbable prototype, string title, Texture2D texture2D) : base(null, title, texture2D)
        {
            this.prototype = prototype;
        }

        /// <summary>
        /// 标题
        /// </summary>
        public override string title
        {
            get
            {
                return string.IsNullOrEmpty(base.title) ? prototype.name : base.title;
            }
            set
            {
                base.title = value;
            }
        }

        /// <summary>
        /// 分类名称
        /// </summary>
        public override string tableDataCategoryName => prototype is ITableDataCategoryName obj1 ? obj1.tableDataCategoryName : base.tableDataCategoryName;

        /// <summary>
        /// 操作游戏对象
        /// </summary>
        public override GameObject gameObject => _cloneGrabbable ? _cloneGrabbable.gameObject : (prototype ? prototype.gameObject : null);

        public override bool interactable { get => prototype; set { } }

        /// <summary>
        /// 有效性
        /// </summary>
        public override bool valid { get => prototype; set { } }

        /// <summary>
        /// 原型相同或可抓对象分类相同
        /// </summary>
        /// <param name="tableData"></param>
        /// <returns></returns>
        public override bool IsSameCategory(TableData_Model tableData)
        {
            if (prototype && tableData is CloneGrabbableTableData_Model model && model.prototype == prototype)
            {
                return true;
            }

            return base.IsSameCategory(tableData);
        }

        /// <summary>
        /// 可抓对象：此时才克隆对象
        /// </summary>
        public override Grabbable grabbable
        {
            get
            {
                if (!_cloneGrabbable)
                {
                    _cloneGrabbable = Clone();
                }
                return _cloneGrabbable;
            }
        }

        /// <summary>
        /// 可抓对象
        /// </summary>
        public override Grabbable unityObject => _cloneGrabbable;
        private Grabbable _cloneGrabbable;

        /// <summary>
        /// 结束拖拽
        /// </summary>
        /// <param name="tableInteractData"></param>
        public override void OnDragEnd(TableInteractData tableInteractData)
        {
            base.OnDragEnd(tableInteractData);

            if (CommonFun.IsOnUGUI())
            {
                if (_cloneGrabbable)
                {
                    UnityEngine.Object.Destroy(_cloneGrabbable.gameObject);
                    _cloneGrabbable = null;
                }
            }
        }

        private Grabbable Clone()
        {
            if (prototype && count > 0)
            {
                var newGO = prototype.gameObject.XCloneObject();
                if (newGO)
                {
                    newGO.XSetParent(prototype.gameObject.transform.parent);
                    newGO.XSetUniqueName(prototype.gameObject.name);
                    return newGO.GetComponent<Grabbable>();
                }
            }
            return null;
        }
    }

    /// <summary>
    /// 克隆型可抓对象表数据创建器
    /// </summary>
    [Serializable]
    public class CloneGrabbableTableDataMaker : TableData_Model
    {
        /// <summary>
        /// 原型
        /// </summary>
        [Name("原型")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Grabbable _prototype;

        /// <summary>
        /// 原型处理规则
        /// </summary>
        public enum EPrototypeHandleRule
        {
            /// <summary>
            /// 禁用
            /// </summary>
            [Name("禁用")]
            Disable = -1,

            /// <summary>
            /// 使用原型
            /// </summary>
            [Name("使用原型")]
            UsePrototype = 0,

            /// <summary>
            /// 使用克隆体
            /// </summary>
            [Name("使用克隆体")]
            UseClone,
        }

        /// <summary>
        /// 原型处理规则
        /// </summary>
        [Name("原型处理规则")]
        [EnumPopup]
        public EPrototypeHandleRule _prototypeHandleRule = EPrototypeHandleRule.UsePrototype;

        /// <summary>
        /// 允许克隆数量
        /// </summary>
        [Name("允许克隆数量")]
        [Min(1)]
        [HideInSuperInspector(nameof(_prototypeHandleRule), EValidityCheckType.NotEqual, EPrototypeHandleRule.UseClone)]
        public int _allowCloneCount = 1;

        private List<TableData_Model> models = new List<TableData_Model>();

        /// <summary>
        /// 构造函数
        /// </summary>
        public CloneGrabbableTableDataMaker() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="prototype"></param>
        /// <param name="count"></param>
        public CloneGrabbableTableDataMaker(Grabbable prototype, int count)
        {
            _prototype = prototype;
            _allowCloneCount = count;
        }

        /// <summary>
        /// 生产克隆可抓模型
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TableData_Model> Generate()
        {
            if (_prototype)
            {
                switch (_prototypeHandleRule)
                {
                    case EPrototypeHandleRule.UsePrototype:
                        {
                            models.Add(new GrabbableTableData_Model(_prototype, title, texture2D));
                            break;
                        }
                    case EPrototypeHandleRule.UseClone:
                        {
                            if (models.Count >= _allowCloneCount) return Empty<GrabbableTableData_Model>.Array;

                            for (int i = models.Count; i < _allowCloneCount; i++)
                            {
                                models.Add(new CloneGrabbableTableData_Model(_prototype, title, texture2D));
                            }
                            break;
                        }
                }
            }
            return models;
        }
    } 
    #endregion
}
