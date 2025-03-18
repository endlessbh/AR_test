using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.Extension.Base.Helpers;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.Interfaces;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginCommonUtils.Runtime;

namespace XCSJ.PluginTools.Items
{
    /// <summary>
    /// 基础物品
    /// </summary>

    [RequireManager(typeof(ToolsManager))]
    public abstract class BaseItem : InteractableVirtual, IItem
    {
        /// <summary>
        /// 原型
        /// </summary>
        [Name("原型")]
        [Tip("当前物品对象的克隆源对象；如果本参数为空，那么当前物品即为原型物品；", "Clone source object of the current item object; If this parameter is empty, the current item is the prototype item;")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ComponentPopup]
        public BaseItem _prototype;

        /// <summary>
        /// 原型物品
        /// </summary>
        public IItem prototypeItem
        {
            get => _prototype;
            set => _prototype = value as BaseItem;
        }

        /// <summary>
        /// 获取有效原型
        /// </summary>
        /// <returns></returns>
        public BaseItem GetValidPrototypeItem() => _prototype ? _prototype : this;

        #region 选择

        /// <summary>
        /// 可选择的
        /// </summary>
        [Group("选择设置", defaultIsExpanded = false)]
        [Name("可选择的")]
        [Tip("标识是否允许修改其选择状态", "Indicates whether the selection status is allowed to be modified")]
        public bool _selectable = true;

        /// <summary>
        /// 可选择的
        /// </summary>
        public bool canSelect => _selectable;

        /// <summary>
        /// 已选择
        /// </summary>
        [Name("已选择")]
        [Tip("标识当前物品是否处于选择状态", "Identifies whether the current item is in the selected state")]
        [Readonly]
        public bool _isSelected = false;

        /// <summary>
        /// 已选择
        /// </summary>
        public bool isSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    onSelectStateChanged?.Invoke(this);
                }
            }
        }

        /// <summary>
        /// 当选择状态已切换事件
        /// </summary>
        public static event Action<BaseItem> onSelectStateChanged;

        /// <summary>
        /// 能否选择
        /// </summary>
        /// <param name="interactor"></param>
        /// <param name="interactInput"></param>
        /// <returns></returns>
        public virtual bool CanSelected(InteractData interactData)
        {
            return canSelect;
        }

        /// <summary>
        /// 处理选择
        /// </summary>
        /// <param name="interactor"></param>
        /// <param name="interactInput"></param>
        /// <returns></returns>
        public virtual bool HandleSelected(InteractData interactData)
        {
            if (canSelect)
            {
                isSelected = !isSelected;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 设置为选择集
        /// </summary>
        [ContextMenu("设置为选择集")]
        public void SetAsSelection() => Selection.selection = gameObject;

        /// <summary>
        /// 添加到选择集
        /// </summary>
        [ContextMenu("添加到选择集")]
        public void AddToSelection() => Selection.AddIfNotContains(gameObject);

        #endregion

        #region 激活

        /// <summary>
        /// 可激活的
        /// </summary>
        [Group("激活设置", defaultIsExpanded = false)]
        [Name("可激活的")]
        [Tip("标识是否允许修改其激活状态", "Indicates whether the activation status is allowed to be modified")]
        public bool _activatable = true;

        /// <summary>
        /// 可激活的
        /// </summary>
        public bool canActivated => _activatable;

        /// <summary>
        /// 已激活
        /// </summary>
        [Name("已激活")]
        [Tip("标识当前物品是否处于激活状态", "Identifies whether the current item is active")]
        [Readonly]
        [EndGroup(true)]
        public bool _isActivated = false;

        /// <summary>
        /// 已激活
        /// </summary>
        public bool isActivated
        {
            get => _isActivated;
            set
            {
                if (_isActivated != value)
                {
                    _isActivated = value;
                    onActiveStateChanged?.Invoke(this);
                }
            }
        }

        public override List<string> cmds => throw new NotImplementedException();

        /// <summary>
        /// 当激活状态已切换事件
        /// </summary>
        public static event Action<BaseItem> onActiveStateChanged;

        /// <summary>
        /// 能否激活
        /// </summary>
        /// <param name="interactor"></param>
        /// <param name="interactInput"></param>
        /// <returns></returns>
        public virtual bool CanActivated(IActivator interactor, IActivatorInput interactInput)
        {
            return canActivated;
        }

        /// <summary>
        /// 处理激活
        /// </summary>
        /// <param name="interactor"></param>
        /// <param name="interactInput"></param>
        /// <returns></returns>
        public virtual bool HandleActivated(IActivator interactor, IActivatorInput interactInput)
        {
            if (canActivated)
            {
                isActivated = interactInput.active;
                return true;
            }
            return false;
        }

        #endregion

        /// <summary>
        /// 能否交互
        /// </summary>
        /// <param name="interactor"></param>
        /// <param name="interactInput"></param>
        /// <returns></returns>
        public bool CanInteractable(InteractData interactData)
        {
            //if (interactor is IActivator activator && interactInput is IActivatorInput activatorInput)
            //{
            //    return CanActivated(activator, activatorInput, out canInteractableResult);
            //}

            return CanSelected(interactData);
        }

        /// <summary>
        /// 尝试处理交互
        /// </summary>
        /// <param name="interactor"></param>
        /// <param name="interactInput"></param>
        /// <returns></returns>
        public bool TryHandleInteractable(InteractData interactData)
        {
            //if (interactor is ISelector selector && interactInput is ISelectorInput selectorInput)
            //{
            //    return HandleSelected(selector, selectorInput, out handleInteractableResult);
            //}
            //else if (interactor is IActivator activator && interactInput is IActivatorInput activatorInput)
            //{
            //    return HandleActivated(activator, activatorInput, out handleInteractableResult);
            //}

            ////针对物品的处理
            //if(context is IItemContext itemContext && interactor is IItemInteractor itemInteractor && interactInput is IItemInput itemInput)
            //{
            //    return TryHandleItemInteractable(itemContext, itemInteractor, itemInput, out handleInteractableResult);
            //}
            //handleInteractableResult = InvalidInteractor.Default;
            return false;
        }

        /// <summary>
        /// 获取默认交互用途
        /// </summary>
        /// <param name="itemContext"></param>
        /// <returns></returns>
        public virtual string GetDefaultItemInteractableUsage(IItemContext itemContext) => nameof(DefaultHandle);

        protected List<string> interactableUsages = new List<string>();

        /// <summary>
        /// 获取交互用途
        /// </summary>
        /// <param name="itemContext"></param>
        /// <returns></returns>
        public virtual List<string> GetItemInteractableUsages(IItemContext itemContext)
        {
            interactableUsages.Clear();
            interactableUsages.Add(nameof(DefaultHandle));
            return interactableUsages;
        }

        /// <summary>
        /// 尝试处理交互
        /// </summary>
        /// <param name="itemContext"></param>
        /// <param name="itemInteractor"></param>
        /// <param name="itemInput"></param>
        /// <param name="handleInteractableResult"></param>
        /// <returns></returns>
        public virtual bool TryHandleItemInteractable(IItemContext itemContext, IItemInteractor itemInteractor)
        {
            return DefaultHandle(itemContext, itemInteractor);
        }

        /// <summary>
        /// 默认处理
        /// </summary>
        /// <param name="itemContext"></param>
        /// <param name="itemInteractor"></param>
        /// <param name="itemInput"></param>
        /// <param name="handleInteractableResult"></param>
        /// <returns></returns>
        public virtual bool DefaultHandle(IItemContext itemContext, IItemInteractor itemInteractor)
        {
            this.DefaultDebugLog();
            SetAsSelection();
            return true;
        }

        /// <summary>
        /// 当克隆之后回调
        /// </summary>
        /// <param name="itemContext"></param>
        public virtual void OnAfterClone(IItemContext itemContext) { }

        /// <summary>
        /// 重置
        /// </summary>
        public virtual void Reset() { }

        public override List<string> GetWorkCmds(InteractData interactData) => new List<string>();

        public override bool CanInteractAsInteractable(InteractData interactData) => true;

        protected override EInteractResult OnInteractAsInteractable(InteractData interactData) => EInteractResult.Finished;
    }

    /// <summary>
    /// 物品接口：用于修饰一切可交互的物品
    /// </summary>
    public interface IItem : IName, IInteractable, ISelectable, IActivatable
    {
        /// <summary>
        /// 原型物品
        /// </summary>
        IItem prototypeItem { get; set; }

        /// <summary>
        /// 获取物品默认交互用途
        /// </summary>
        /// <param name="itemContext"></param>
        /// <returns></returns>
        string GetDefaultItemInteractableUsage(IItemContext itemContext);

        /// <summary>
        /// 获取物品交互用途
        /// </summary>
        /// <param name="itemContext"></param>
        /// <returns></returns>
        List<string> GetItemInteractableUsages(IItemContext itemContext);

        /// <summary>
        /// 尝试处理物品交互
        /// </summary>
        /// <param name="itemContext"></param>
        /// <param name="itemInteractor"></param>
        /// <param name="itemInput"></param>
        /// <param name="handleInteractableResult"></param>
        /// <returns></returns>
        bool TryHandleItemInteractable(IItemContext itemContext, IItemInteractor itemInteractor);

        /// <summary>
        /// 当克隆之后回调
        /// </summary>
        /// <param name="itemContext"></param>
        void OnAfterClone(IItemContext itemContext);
    }

    /// <summary>
    /// 物品语境
    /// </summary>
    public interface IItemContext 
    {
        /// <summary>
        /// 物品
        /// </summary>
        IItem item { get; set; }
    }

    /// <summary>
    /// 语境：目前内置的物品交互语境
    /// </summary>
    [Name("语境")]
    public enum EContext
    {
        /// <summary>
        /// 未知
        /// </summary>
        [Name("未知")]
        Unknow = 0,

        /// <summary>
        /// 实体
        /// </summary>
        [Name("实体")]
        Entity,

        /// <summary>
        /// 背包
        /// </summary>
        [Name("背包")]
        Bag,
    }

    /// <summary>
    /// 背包物品语境
    /// </summary>
    public interface IBagItemContext : IItemContext
    {
        /// <summary>
        /// 所在背包
        /// </summary>
        IBag bag { get; }

        /// <summary>
        /// 背包物品数据信息
        /// </summary>
        IBagItemData bagItemData { get; }
    }

    /// <summary>
    /// 物品语境
    /// </summary>
    public class ItemContext : IItemContext
    {
        public IItem item { get; set; }

        public string context { get; set; }

        public ItemContext(string context) => this.context = context;
        public ItemContext(EContext context) => this.context = context.ToString();
    }

    public class BagItemContext : ItemContext, IBagItemContext
    {

        public IBag bag { get; set; }

        public IBagItemData bagItemData { get; set; }

        public BagItemContext(IBag bag, IBagItemData bagItemData, string context) :base(context)
        {
            this.bag = bag;
            this.bagItemData = bagItemData;
        }

        public BagItemContext(IBag bag, IBagItemData bagItemData, EContext context = EContext.Bag) : this(bag, bagItemData, context.ToString()) { }
    }

    public class ItemInput 
    {
        /// <summary>
        /// 默认实例
        /// </summary>
        public static ItemInput Default => Default<ItemInput>.Instance;

        /// <summary>
        /// 输入用途
        /// </summary>
        public string usage { get; set; }

        /// <summary>
        /// 构造
        /// </summary>
        public ItemInput() : this("") { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="usage"></param>
        public ItemInput(string usage) { this.usage = usage; }
    }

    /// <summary>
    /// 物品交互器
    /// </summary>
    public interface IItemInteractor : IInteractor { }

    #region 物品属性

    /// <summary>
    /// 物品属性：可序列化的物品属性
    /// </summary>
    [Serializable]
    [Name("物品属性")]
    public class ItemProperty : IItemProperty
    {
        /// <summary>
        /// 名称
        /// </summary>
        [Name("名称")]
        public string _name;

        /// <summary>
        /// 名称
        /// </summary>
        public string name => _name;

        /// <summary>
        /// 值
        /// </summary>
        [Name("值")]
        public Argument _value = new Argument();

        /// <summary>
        /// 值
        /// </summary>
        public object value { get => _value.value; set => _value.SetValue(value); }
    }

    /// <summary>
    /// 物品属性集合
    /// </summary>
    [Serializable]
    [LanguageFileOutput]
    [Name("物品属性集合")]
    public class ItemPropertySet : IItemPropertySet
    {
        /// <summary>
        /// 物品属性集合
        /// </summary>
        [Name("物品属性集合")]
        public List<ItemProperty> _itemProperties = new List<ItemProperty>();

        /// <summary>
        /// 物品属性集合
        /// </summary>
        public IItemProperty[] itemProperties => _itemProperties.ToArray();

        /// <summary>
        /// 尝试获取物品属性
        /// </summary>
        /// <param name="name"></param>
        /// <param name="itemProperty"></param>
        /// <returns></returns>
        public bool TryGetItemProperty(string name, out IItemProperty itemProperty)
        {
            itemProperty = _itemProperties.FirstOrDefault(p => p.name == name);
            return itemProperty != null;
        }
    }

    /// <summary>
    /// 物品属性接口
    /// </summary>
    public interface IItemProperty
    {
        /// <summary>
        /// 名称
        /// </summary>
        string name { get; }

        /// <summary>
        /// 值
        /// </summary>
        object value { get; set; }
    }

    /// <summary>
    /// 物品属性集合接口
    /// </summary>
    public interface IItemPropertySet
    {
        /// <summary>
        /// 物品属性集合
        /// </summary>
        IItemProperty[] itemProperties { get; }

        /// <summary>
        /// 尝试获取物品属性
        /// </summary>
        /// <param name="name"></param>
        /// <param name="itemProperty"></param>
        /// <returns></returns>
        bool TryGetItemProperty(string name, out IItemProperty itemProperty);
    }

    public interface IEntityItemPropertySet : IItemPropertySet { }

    public interface IBagItemPropertySet : IItemPropertySet { }

    #endregion

    /// <summary>
    /// 实体物品
    /// </summary>
    public interface IEntityItem : IItem { }

    /// <summary>
    /// 背包物品
    /// </summary>
    public interface IBagItem : IItem { }

    /// <summary>
    /// 背包
    /// </summary>
    public interface IBag : IBagItem
    {
        /// <summary>
        /// 能否添加
        /// </summary>
        /// <param name="bagItem"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        bool CanAdd(IBagItem bagItem, int count);

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="bagItem"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        bool TryAdd(IBagItem bagItem, int count);

        /// <summary>
        /// 能否删除
        /// </summary>
        /// <param name="bagItem"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        bool CanRemove(IBagItem bagItem, int count);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="bagItem"></param>
        bool TryRemove(IBagItem bagItem, int count);

        /// <summary>
        /// 获取默认分类用途
        /// </summary>
        /// <returns></returns>
        string GetDefaultCategoryUsage();

        /// <summary>
        /// 获取分类用途
        /// </summary>
        /// <returns></returns>
        List<string> GetCategoryUsages();

        /// <summary>
        /// 尝试通过用途获取背包物品
        /// </summary>
        /// <param name="usages"></param>
        /// <param name="bagItems"></param>
        /// <returns></returns>
        bool TryGetBagItems(string usage, out List<IBagItem> bagItems);

        /// <summary>
        /// 尝试处理背包物品交互
        /// </summary>
        /// <param name="bagItem"></param>
        /// <param name="itemContext"></param>
        /// <param name="itemInteractor"></param>
        /// <param name="itemInput"></param>
        /// <param name="handleInteractableResult"></param>
        /// <returns></returns>
        bool TryHandleBagItemInteractable(IBagItem bagItem, IItemContext itemContext, IItemInteractor itemInteractor);

        /// <summary>
        /// 获取背包物品的交互用途
        /// </summary>
        /// <param name="bagItem"></param>
        /// <param name="itemContext"></param>
        /// <returns></returns>
        List<string> GetBagItemInteractableUsages(IBagItem bagItem, IItemContext itemContext);
    }

    public interface IBagItemHandleInteractableResult 
    {
        /// <summary>
        /// 物品
        /// </summary>
        IItem item { get; set; }

        /// <summary>
        /// 背包
        /// </summary>
        IBag bag { get; set; }

        /// <summary>
        /// 背包物品
        /// </summary>
        IBagItem bagItem { get; set; }
    }

    /// <summary>
    /// 背包物品克隆结果
    /// </summary>
    public class BagItemCloneResult : IBagItemHandleInteractableResult
    {
        public IItemContext itemContext { get; set; }

        /// <summary>
        /// 源背包
        /// </summary>
        public IBag srcBag { get; set; }

        public IBagItem srcBagItem { get; set; }

        public IItem srcItem { get; set; }

        public IBag bag { get; set; }

        public IBagItem bagItem { get; set; }

        public IItem item { get; set; }
    }

    public interface IClonable
    {
        /// <summary>
        /// 能否克隆
        /// </summary>
        bool canClonable { get; }
    }

    /// <summary>
    /// 物品可克隆接口
    /// </summary>
    public interface IItemClonable
    {
        /// <summary>
        /// 能否克隆
        /// </summary>
        /// <param name="itemContext"></param>
        /// <returns></returns>
        bool CanClonable(IItemContext itemContext);
    }

    public interface IBagItemClonable
    {
        /// <summary>
        /// 能否克隆
        /// </summary>
        /// <param name="itemContext"></param>
        /// <returns></returns>
        bool CanClonable(IBagItemContext bagItemContext);
    }

    /// <summary>
    /// 物品事件参数
    /// </summary>
    public class ItemEventArgs : EventArgs
    {
        /// <summary>
        /// 原型物品
        /// </summary>
        public IItem prototypeItem { get; set; }

        /// <summary>
        /// 物品
        /// </summary>
        public IItem item { get; set; }

        /// <summary>
        /// 物品语境
        /// </summary>
        public IItemContext itemContext { get; set; }

        /// <summary>
        /// 交互器
        /// </summary>
        public IInteractor interactor { get; set; }

        /// <summary>
        /// 用途
        /// </summary>
        public string usage => "";

        /// <summary>
        /// (原型物品,物品)元组转当前对象
        /// </summary>
        /// <param name="item"></param>
        public static implicit operator ItemEventArgs((IItem prototype, IItem item) item) => new ItemEventArgs() { prototypeItem = item.prototype, item = item.item };
    }

    /// <summary>
    /// 物品事件组手
    /// </summary>
    public static class ItemEventHelper
    {
        #region 创建/新建/克隆

        public static event Action<ItemEventArgs> onBeforeCreate;

        public static void CallBeforeCreate(IItem prototype, IItem item) => CallBeforeCreate((prototype, item));

        public static void CallBeforeCreate(ItemEventArgs eventArgs)
        {
            if (eventArgs != null) onBeforeCreate?.Invoke(eventArgs);
        }

        public static event Action<ItemEventArgs> onAfterCreate;

        public static void CallAfterCreate(IItem prototype, IItem item) => CallAfterCreate((prototype, item));

        public static void CallAfterCreate(ItemEventArgs eventArgs)
        {
            if (eventArgs != null) onAfterCreate?.Invoke(eventArgs);
        }

        #endregion

        #region 销毁/析构/删除

        public static event Action<ItemEventArgs> onBeforeDestroy;

        public static void CallBeforeDestroy(IItem prototype, IItem item) => CallBeforeDestroy((prototype, item));

        public static void CallBeforeDestroy(ItemEventArgs eventArgs)
        {
            if (eventArgs != null) onBeforeDestroy?.Invoke(eventArgs);
        }

        public static event Action<ItemEventArgs> onAfterDestroy;

        public static void CallAfterDestroy(IItem prototype, IItem item) => CallAfterDestroy((prototype, item));

        public static void CallAfterDestroy(ItemEventArgs eventArgs)
        {
            if (eventArgs != null) onAfterDestroy?.Invoke(eventArgs);
        }

        #endregion

        #region 变更/修改

        public static event Action<ItemEventArgs> onBeforeChange;
        public static void CallBeforeChange(IItem prototype, IItem item) => CallBeforeChange((prototype, item));

        public static void CallBeforeChange(ItemEventArgs eventArgs)
        {
            if (eventArgs != null) onBeforeChange?.Invoke(eventArgs);
        }


        public static event Action<ItemEventArgs> onAfterChange;

        public static void CallAfterChange(IItem prototype, IItem item) => CallAfterChange((prototype, item));

        public static void CallAfterChange(ItemEventArgs eventArgs)
        {
            if (eventArgs != null) onAfterChange?.Invoke(eventArgs);
        }

        #endregion
    }

    /// <summary>
    /// 物品组手
    /// </summary>
    public static class ItemHelper
    {
        /// <summary>
        /// 有效
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static bool IsValid(this IItem item) => !ObjectHelper.ObjectIsNull(item);

        /// <summary>
        /// 无效
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static bool IsInvalid(this IItem item) => ObjectHelper.ObjectIsNull(item);

        /// <summary>
        /// 克隆物品
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="component"></param>
        /// <param name="itemContext"></param>
        /// <returns></returns>
        public static T CloneItem<T>(this T component, IItemContext itemContext) where T : Component, IItem
        {
            if (!component) return default;
            var prototype = component.prototypeItem;
            if (prototype.IsInvalid()) prototype = component;

            ItemEventHelper.CallBeforeCreate(component.prototypeItem, component);

            var newGameObject = component.gameObject.XCloneObject();
            var newItem = newGameObject.GetComponent<T>();

            //设置原型信息
            newItem.prototypeItem = prototype;

            //回调克隆事件
            newItem.OnAfterClone(itemContext);

            ItemEventHelper.CallAfterCreate(component, newItem);

            return newItem;
        }

        /// <summary>
        /// 销毁物品
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static bool DestroyItem(this IItem item)
        {
            if (item is Component component && component)
            {
                return DestroyItem(component, item);
            }
            return false;
        }

        /// <summary>
        /// 销毁物品
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="component"></param>
        /// <returns></returns>
        public static bool DestroyItem<T>(this T component) where T : Component, IItem
        {
            if (!component) return false;  
            return DestroyItem(component, component);
        }

        private static bool DestroyItem(this Component component, IItem item)
        {
            var prototype = item.prototypeItem;

            //原型无效，则认为当前对象就是原型
            if (prototype.IsInvalid()) return false;

            ItemEventHelper.CallBeforeDestroy(prototype, item);

            //销毁物品游戏对象
            //this.gameObject.XDestoryObject();
            component.gameObject.SetActive(false);

            //延时销毁克隆体
            CommonFun.DelayCall(component.gameObject.XDestoryObject);

            ItemEventHelper.CallAfterDestroy(prototype, item);

            return true;
        }

        ///// <summary>
        ///// 获取用途
        ///// </summary>
        ///// <param name="itemInput"></param>
        ///// <returns></returns>
        //public static string GetUsage(this IItemInput itemInput) => itemInput?.usage ?? "";

        /// <summary>
        /// 字符串转为物品输入
        /// </summary>
        /// <param name="usage"></param>
        /// <returns></returns>
        public static ItemInput ToItemInput(this string usage) => new ItemInput(usage);

        /// <summary>
        /// 默认调试日志
        /// </summary>
        /// <param name="item"></param>
        public static void DefaultDebugLog(this IItem item)
        {
            Debug.Log(CommonFun.ObjectToString(item));
#if UNITY_EDITOR
            if (item is UnityEngine.Object obj && obj) UnityEditor.EditorGUIUtility.PingObject(obj);
#endif
        }
    }

    /// <summary>
    /// 背包物品事件参数
    /// </summary>
    public class BagItemEventArgs : ItemEventArgs
    {
        /// <summary>
        /// 背包
        /// </summary>
        public IBag bag { get; set; }

        /// <summary>
        /// 背包项
        /// </summary>
        public IBagItem bagItem { get; set; }
    }

    /// <summary>
    /// 背包事件参数
    /// </summary>
    public class BagEventArgs : BagItemEventArgs
    {
        /// <summary>
        /// 构造
        /// </summary>
        public BagEventArgs() { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="bag"></param>
        /// <param name="bagItem"></param>
        public BagEventArgs(IBag bag, IBagItem bagItem)
        {
            this.bag = bag;
            this.bagItem = bagItem;

            this.item = item;
            this.prototypeItem = item?.prototypeItem;
        }

        public static implicit operator BagEventArgs((IBag bag, IBagItem bagItem) item) => new BagEventArgs(item.bag, item.bagItem);

        public static implicit operator BagEventArgs((IBag bag, IBagItem bagItem, IItemContext itemContext) item) => new BagEventArgs(item.bag, item.bagItem) { itemContext = item.itemContext };
    }

    /// <summary>
    /// 背包事件组手
    /// </summary>
    public static class BagEventHelper
    {
        #region 增

        public static event Action<BagEventArgs> onBeforeAdd;

        public static void CallBeforeAdd(IBag bag, IBagItem bagItem) => CallBeforeAdd((bag, bagItem));

        public static void CallBeforeAdd(BagEventArgs bagEventArgs)
        {
            if (bagEventArgs != null) onBeforeAdd?.Invoke(bagEventArgs);
        }

        public static event Action<BagEventArgs> onAfterAdd;

        public static void CallAfterAdd(IBag bag, IBagItem bagItem) => CallAfterAdd((bag, bagItem));

        public static void CallAfterAdd(BagEventArgs bagEventArgs)
        {
            if (bagEventArgs != null) onAfterAdd?.Invoke(bagEventArgs);
        }


        #endregion

        #region 删

        public static event Action<BagEventArgs> onBeforeRemove;

        public static void CallBeforeRemove(IBag bag, IBagItem bagItem) => CallBeforeRemove((bag, bagItem));

        public static void CallBeforeRemove(BagEventArgs bagEventArgs)
        {
            if (bagEventArgs != null) onBeforeRemove?.Invoke(bagEventArgs);
        }

        public static event Action<BagEventArgs> onAfterRemove;

        public static void CallAfterRemove(IBag bag, IBagItem bagItem) => CallAfterRemove((bag, bagItem));

        public static void CallAfterRemove(BagEventArgs bagEventArgs)
        {
            if (bagEventArgs != null) onAfterRemove?.Invoke(bagEventArgs);
        }

        #endregion

        #region 改

        public static event Action<BagEventArgs> onBeforeModify;

        public static void CallBeforeModify(IBag bag, IBagItem bagItem) => CallBeforeModify((bag, bagItem));

        public static void CallBeforeModify(BagEventArgs bagEventArgs)
        {
            if (bagEventArgs != null) onBeforeModify?.Invoke(bagEventArgs);
        }

        public static event Action<BagEventArgs> onAfterModify;

        public static void CallAfterModify(IBag bag, IBagItem bagItem) => CallAfterModify((bag, bagItem));

        public static void CallAfterModify(BagEventArgs bagEventArgs)
        {
            if (bagEventArgs != null) onAfterModify?.Invoke(bagEventArgs);
        }

        #endregion
    }

    #region 激活

    /// <summary>
    /// 可激活接口：用于<see cref="IActivator"/>执行操作的对象；
    /// </summary>
    public interface IActivatable : IInteractable
    {
        /// <summary>
        /// 能否激活
        /// </summary>
        bool canActivated { get; }
    }

    /// <summary>
    /// 激活器接口：用于对<see cref="IActivatable"/>执行激活操作
    /// </summary>
    public interface IActivator : IInteractor { }

    /// <summary>
    /// 激活器输入接口
    /// </summary>
    public interface IActivatorInput 
    {
        /// <summary>
        /// 激活
        /// </summary>
        bool active { get; }
    }

    /// <summary>
    /// 激活器输入
    /// </summary>
    public class ActivatorInput : IActivatorInput
    {
        /// <summary>
        /// 选择
        /// </summary>
        public static ActivatorInput Select { get; } = new ActivatorInput(true);

        /// <summary>
        /// 取消选择
        /// </summary>
        public static ActivatorInput Diselect { get; } = new ActivatorInput(false);

        public ActivatorInput(bool active = true)
        {
            this.active = active;
        }

        /// <summary>
        /// 激活
        /// </summary>
        public bool active { get; private set; }

        /// <summary>
        /// 交互用途
        /// </summary>
        public string usage { get => nameof(ActivatorInput); set { } }
    }

    #endregion

    #region 分类

    /// <summary>
    /// 可分类的
    /// </summary>
    public interface ICategorical : IInteractable
    {
        /// <summary>
        /// 能否类别化的
        /// </summary>
        bool canCategorical { get; }
    }

    /// <summary>
    /// 分类器
    /// </summary>
    public interface ICategorizer  { }

    /// <summary>
    /// 分类器输入
    /// </summary>
    public interface ICategorizerInput { }

    #endregion
}
