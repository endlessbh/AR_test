using System.Collections;
using XCSJ.Attributes;

namespace XCSJ.Extension.Base.Dataflows.Models
{
    /// <summary>
    /// 集合变化事件
    /// </summary>
    public class XCollectionChangeEventArgs : XValueEventArg
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="collectionAction"></param>
        public XCollectionChangeEventArgs(object sender, ECollectionAction collectionAction) : base(sender)
        {
            this.collectionAction = collectionAction;
        }

        /// <summary>
        /// 集合发生改变动作类型
        /// </summary>
        public ECollectionAction collectionAction { get; private set; } = ECollectionAction.None;

        /// <summary>
        /// 旧集合
        /// </summary>
        public IEnumerable oldItems { get; private set; }

        /// <summary>
        /// 新集合
        /// </summary>
        public IEnumerable newItems { get; private set; }

        /// <summary>
        /// 有值
        /// </summary>
        public override bool hasValue => collectionAction != ECollectionAction.None;

        /// <summary>
        /// 值
        /// </summary>
        public override object value => _value;

        private object _value = null;

        /// <summary>
        /// 添加事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="newItems"></param>
        /// <returns></returns>
        public static XCollectionChangeEventArgs AddEvent(object sender, IEnumerable newItems) => Create(sender, newItems, ECollectionAction.Add);

        /// <summary>
        /// 移除事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="removeItems"></param>
        /// <returns></returns>
        public static XCollectionChangeEventArgs RemoveEvent(object sender, IEnumerable removeItems) => Create(sender, null, ECollectionAction.Remove, removeItems);

        /// <summary>
        /// 替换事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="oldItems"></param>
        /// <param name="newItems"></param>
        /// <returns></returns>
        public static XCollectionChangeEventArgs ReplaceEvent(object sender, IEnumerable oldItems, IEnumerable newItems) => Create(sender, newItems, ECollectionAction.Replace, oldItems);

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="sortItems"></param>
        /// <returns></returns>
        public static XCollectionChangeEventArgs SortEvent(object sender, IEnumerable sortItems) => Create(sender, sortItems, ECollectionAction.Sort);

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="sortItems"></param>
        /// <returns></returns>
        public static XCollectionChangeEventArgs InitEvent(object sender, IEnumerable newItems) => Create(sender, newItems, ECollectionAction.Init);

        /// <summary>
        /// 清除事件
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        public static XCollectionChangeEventArgs ClearEvent(object sender)
        {
            return new XCollectionChangeEventArgs(sender, ECollectionAction.Clear);
        }

        /// <summary>
        /// 数值事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static XCollectionChangeEventArgs CountEvent(object sender, int count)
        {
            var eventArg = new XCollectionChangeEventArgs(sender, ECollectionAction.Count);
            eventArg._value = count;
            return eventArg;
        }

        /// <summary>
        /// 创建事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="newItems"></param>
        /// <param name="collectionAction"></param>
        /// <param name="oldItems"></param>
        /// <returns></returns>
        public static XCollectionChangeEventArgs Create(object sender, IEnumerable newItems, ECollectionAction collectionAction, IEnumerable oldItems = null)
        {
            var eventArgs = new XCollectionChangeEventArgs(sender, collectionAction);
            eventArgs.oldItems = oldItems;
            eventArgs.newItems = newItems;
            eventArgs._value = newItems != null ? newItems : oldItems;
            return eventArgs;
        }
    }

    /// <summary>
    /// 集合修改动作类型
    /// </summary>

    [Name("集合修改动作类型")]
    public enum ECollectionAction
    {
        /// <summary>
        /// 无
        /// </summary>
        [Name("无")]
        None,

        /// <summary>
        /// 初始化数据
        /// </summary>
        [Name("初始化数据")]
        Init,

        /// <summary>
        /// 增加
        /// </summary>
        [Name("增加")]
        Add,

        /// <summary>
        /// 移除
        /// </summary>
        [Name("移除")]
        Remove,

        /// <summary>
        /// 替换
        /// </summary>
        [Name("替换")]
        Replace,

        /// <summary>
        /// 排序
        /// </summary>
        [Name("排序")]
        Sort,

        /// <summary>
        /// 清除
        /// </summary>
        [Name("清除")]
        Clear,

        /// <summary>
        /// 集合数量
        /// </summary>
        [Name("集合数量")]
        Count,
    }
}
