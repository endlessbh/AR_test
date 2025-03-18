using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Extension.Interactions.Base;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginTools.GameObjects;
using XCSJ.PluginTools.Items;

namespace XCSJ.PluginXGUI.Windows.Tables
{
    #region 基础接口

    /// <summary>
    /// 贴图接口
    /// </summary>
    public interface ITexture2D
    {
        /// <summary>
        /// 贴图
        /// </summary>
        Texture2D texture2D { get; set; }
    }

    /// <summary>
    /// 排序关键字类型
    /// </summary>
    public enum ESortKey
    {
        /// <summary>
        /// 标题
        /// </summary>
        [Name("标题")]
        Title,
    }

    /// <summary>
    /// 排序关键字信息
    /// </summary>
    [Serializable]
    public class SortInfo
    {
        /// <summary>
        /// 排序关键字类型
        /// </summary>
        [Name("排序关键字类型")]
        [EnumPopup]
        public ESortKey _sortKey = ESortKey.Title;

        /// <summary>
        /// 降序
        /// </summary>
        [Name("降序")]
        public bool _descOrder = false;

        /// <summary>
        /// 构造函数
        /// </summary>
        public SortInfo() { }

        /// <summary>
        /// 比较表数据模型
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int Compare(TableData_Model x, TableData_Model y) => _descOrder ? CompareDesc(x, y) : CompareAsc(x, y);

        /// <summary>
        /// 升序比较表数据模型
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int CompareAsc(TableData_Model x, TableData_Model y)
        {
            switch (_sortKey)
            {
                case ESortKey.Title:
                    {
                        return x.title.CompareTo(y.title);
                    }
                default: return 0;
            }
        }

        /// <summary>
        /// 降序比较表数据模型
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int CompareDesc(TableData_Model x, TableData_Model y) => -CompareAsc(x, y);
    }

    /// <summary>
    /// 分类名称接口
    /// </summary>
    public interface ITableDataCategoryName
    {
        string tableDataCategoryName { get; }
    }

    /// <summary>
    /// 游戏对象获取器接口
    /// </summary>
    public interface IGameObjectGetter
    {
        /// <summary>
        /// 游戏对象
        /// </summary>
        GameObject gameObject { get; }
    }

    /// <summary>
    /// 可抓对象获取器接口
    /// </summary>
    public interface IGrabbableGetter
    {
        /// <summary>
        /// 可抓对象
        /// </summary>
        Grabbable grabbable { get; }
    }

    /// <summary>
    /// 组件获取器接口
    /// </summary>
    public interface IComponentGetter
    {
        /// <summary>
        /// 组件
        /// </summary>
        Component component { get; }
    }

    #endregion

    #region TableData_Model

    /// <summary>
    /// 表数据
    /// </summary>
    [LanguageFileOutput]
    public abstract class TableData_Model : ITexture2D
    {
        /// <summary>
        /// 标题
        /// </summary>
        [Name("标题")]
        public string _title;

        /// <summary>
        /// 标题
        /// </summary>
        public virtual string title
        {
            get => _title;
            set
            {
                if (_title != value)
                {
                    _title = value;
                    SendUpdate();
                }
            }
        }

        /// <summary>
        /// 描述
        /// </summary>
        public virtual string description { get; set; }

        /// <summary>
        /// 选择
        /// </summary>
        public virtual bool selected
        {
            get => _selected;
            set
            {
                if (_selected != value)
                {
                    _selected = value;
                    SendUpdate();
                }
            }
        }
        [NonSerialized]
        private bool _selected = false;

        /// <summary>
        /// 可交互性
        /// </summary>
        public virtual bool interactable
        {
            get => !_uninteractable;
            set
            {
                var un = !value;
                if (_uninteractable != un)
                {
                    _uninteractable = un;
                    SendUpdate();
                }
            }
        }
        [NonSerialized]
        private bool _uninteractable = false;

        /// <summary>
        /// 有效性
        /// </summary>
        public virtual bool valid
        {
            get => _valid;
            set
            {
                if (_valid != value)
                {
                    _valid = value;
                    SendUpdate();
                }
            }
        }
        [NonSerialized]
        private bool _valid = true;

        /// <summary>
        /// 图片
        /// </summary>
        [Name("图片")]
        public Texture2D _texture2D;

        /// <summary>
        /// 封面图片
        /// </summary>
        public Texture2D texture2D
        {
            get => _texture2D;
            set
            {
                _texture2D = value;
                SendUpdate();
            }
        }

        /// <summary>
        /// 数据变化事件
        /// </summary>
        public static event Action<TableData_Model> onUpdate;

        /// <summary>
        /// 数据变化事件
        /// </summary>
        protected void SendUpdate()
        {
            onUpdate?.Invoke(this);
        }

        /// <summary>
        /// 数量
        /// </summary>
        public virtual int count { get => category != null ? category.count : 1; set { } }

        /// <summary>
        /// 是否同组
        /// </summary>
        /// <returns></returns>
        public virtual bool IsSameCategory(TableData_Model viewItemModel) => false;

        /// <summary>
        /// 相同类型
        /// </summary>
        internal TableData_ModelCategory category { get; set; }

        /// <summary>
        /// 尝试使用同分类下的图片，减少重复渲染
        /// </summary>
        public void TryUseTexture2DInCategory()
        {
            if (texture2D) return;

            var fm = category.firstModel;
            if (fm != this && fm.texture2D)
            {
                texture2D = fm.texture2D;
            }
        }
    }

    /// <summary>
    /// 表格数据分类：分类是靠模型自身提供方法来比较是否同组
    /// </summary>
    public class TableData_ModelCategory
    {
        /// <summary>
        /// 模型列表
        /// </summary>
        public List<TableData_Model> models { get; } = new List<TableData_Model>();

        /// <summary>
        /// 第一个数据
        /// </summary>
        public TableData_Model firstModel => models.FirstOrDefault();

        /// <summary>
        /// 数量
        /// </summary>
        public int count => models.Count;

        /// <summary>
        /// 包含
        /// </summary>
        /// <param name="tableData"></param>
        /// <returns></returns>
        public bool Contains(TableData_Model tableData) => models.Contains(tableData);

        /// <summary>
        /// 同组
        /// </summary>
        /// <param name="tableData"></param>
        /// <returns></returns>
        public bool IsSameCategory(TableData_Model tableData)
        {
            return firstModel.IsSameCategory(tableData);
        }

        /// <summary>
        /// 加入组内
        /// </summary>
        /// <param name="tableData_Model"></param>
        /// <returns></returns>
        public bool Add(TableData_Model tableData_Model)
        {
            if (!Contains(tableData_Model) && (count == 0 || IsSameCategory(tableData_Model)))
            {
                models.Add(tableData_Model);
                tableData_Model.category = this;

                tableData_Model.TryUseTexture2DInCategory();
                return true;
            }
            return false;
        }

        /// <summary>
        /// 从组内移除
        /// </summary>
        /// <param name="tableData"></param>
        /// <returns></returns>
        public bool Remove(TableData_Model tableData)
        {
            if (models.Remove(tableData))
            {
                tableData.category = null;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 尝试在分类下共享图片
        /// </summary>
        public void ShareTexture2D()
        {
            var fm = firstModel;
            if (fm == null || !fm.texture2D || count <= 1) return;

            for (int i = 1; i < models.Count; i++)
            {
                models[i].TryUseTexture2DInCategory();
            }
        }
    }

    #endregion

    #region DraggableTableData_Model

    /// <summary>
    /// 可拖拽表数据
    /// </summary>
    public abstract class DraggableTableData_Model : TableData_Model
    {
        /// <summary>
        /// 拖拽开始
        /// </summary>
        public virtual void OnDragStart(TableInteractData viewItemData) { }

        /// <summary>
        /// 拖拽中
        /// </summary>
        public virtual void OnDrag(TableInteractData viewItemData) { }

        /// <summary>
        /// 拖拽结束
        /// </summary>
        public virtual void OnDragEnd(TableInteractData viewItemData) { }
    }

    #endregion

    #region UnityObjectTableData_Model

    /// <summary>
    /// Unity Object 表数据
    /// </summary>
    public class UnityObjectTableData_Model<T> : DraggableTableData_Model, ITableDataCategoryName where T : UnityEngine.Object
    {
        /// <summary>
        /// Unity Object
        /// </summary>
        [Name("关联对象")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public T _unityObject;

        /// <summary>
        /// 获取Unity Object
        /// </summary>
        public virtual T unityObject { get => _unityObject; protected set => _unityObject = value; }

        /// <summary>
        /// 有效性
        /// </summary>
        public override bool valid { get => unityObject; set { } }

        /// <summary>
        /// 标题
        /// </summary>
        public override string title
        {
            get => (string.IsNullOrEmpty(base.title) && unityObject) ? unityObject.name : base.title;
            set => base.title = value;
        }

        /// <summary>
        /// 分类名
        /// </summary>
        public virtual string tableDataCategoryName
        {
            get => unityObject is ITableDataCategoryName obj ? obj.tableDataCategoryName : title;
            set { }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public UnityObjectTableData_Model() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="component"></param>
        public UnityObjectTableData_Model(T unityObject)
        {
            this.unityObject = unityObject;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="component"></param>
        public UnityObjectTableData_Model(T component, string title, Texture2D texture2D) : this(component)
        {
            this.title = title;
            this.texture2D = texture2D;
        }

        /// <summary>
        /// 获取哈希Code
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() => unityObject ? unityObject.GetHashCode() : base.GetHashCode();

        /// <summary>
        /// 相等比较
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return (unityObject && obj is UnityObjectTableData_Model<T> model) ? unityObject == model.unityObject : base.Equals(obj);
        }

        /// <summary>
        /// 是否相同组
        /// </summary>
        /// <param name="viewItemModel"></param>
        /// <returns></returns>
        public override bool IsSameCategory(TableData_Model viewItemModel)
        {
            if (!string.IsNullOrEmpty(tableDataCategoryName))
            {
                if (viewItemModel is ITableDataCategoryName obj)
                {
                    return tableDataCategoryName == obj.tableDataCategoryName;
                }
            }

            return base.IsSameCategory(viewItemModel);
        }
    }

    #endregion

    #region ComponentTableData_Model

    /// <summary>
    ///  组件表数据
    /// </summary>
    public class ComponentTableData_Model<T> : UnityObjectTableData_Model<T>, IGameObjectGetter, IGrabbableGetter, IComponentGetter where T : Component
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ComponentTableData_Model() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="component"></param>
        public ComponentTableData_Model(T component) : base(component) { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="component"></param>
        /// <param name="title"></param>
        /// <param name="texture2D"></param>
        public ComponentTableData_Model(T component, string title, Texture2D texture2D = null) : base(component, title, texture2D) { }

        /// <summary>
        /// 游戏对象
        /// </summary>
        public virtual GameObject gameObject => unityObject ? unityObject.gameObject : null;

        /// <summary>
        /// 可抓对象
        /// </summary>
        public virtual Grabbable grabbable
        {
            get
            {
                if (unityObject is Grabbable g) { return g; }
                return unityObject.GetComponent<Grabbable>();
            }
        }

        /// <summary>
        /// 组件
        /// </summary>
        public Component component => unityObject;
    }

    /// <summary>
    /// 组件表数据
    /// </summary>
    [Serializable]
    public class ComponentTableData_Model : ComponentTableData_Model<Component>, IComponentGetter
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ComponentTableData_Model() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="component"></param>
        public ComponentTableData_Model(Component component) : base(component) { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="component"></param>
        /// <param name="title"></param>
        /// <param name="texture2D"></param>
        public ComponentTableData_Model(Component component, string title, Texture2D texture2D = null) : base(component, title, texture2D) { }
    }

    #endregion
}
