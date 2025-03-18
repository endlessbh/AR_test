using System;
using System.Collections;
using System.Collections.Generic;

namespace XCSJ.Extension.Base.Dataflows.Models
{
    /// <summary>
    /// 可观察列表
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObservableList<T> : IList, IList<T>, ISendCollectionChangeEvent
    {
        private List<T> list;

        /// <summary>
        /// 构造函数
        /// </summary>
        public ObservableList()
        {
            list = new List<T>();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ObservableList(int capacity)
        {
            list = new List<T>(capacity);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ObservableList(IEnumerable<T> collection)
        {
            list = new List<T>(collection);
        }

        /// <summary>
        /// 初始化链表函数 ：链表指针从外部传入
        /// </summary>
        /// <param name="list"></param>
        public void Init(List<T> list)
        {
            this.list = list;
            OnCollectionChanged(XCollectionChangeEventArgs.InitEvent(this, list));
        }

        /// <summary>
        /// 批量添加链表元素
        /// </summary>
        /// <param name="collection"></param>
        public void AddRange(IEnumerable<T> collection)
        {
            list.AddRange(collection);

            OnCollectionChanged(XCollectionChangeEventArgs.AddEvent(this, collection));
        }

        /// <summary>
        /// 批量移除
        /// </summary>
        /// <param name="index"></param>
        /// <param name="count"></param>
        public void RemoveRange(int index, int count)
        {
            List<T> oldItems = list.GetRange(index, count);
            list.RemoveRange(index, count);

            OnCollectionChanged(XCollectionChangeEventArgs.RemoveEvent(this, oldItems));
        }

        /// <summary>
        /// 获取一定范围值
        /// </summary>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public List<T> GetRange(int index, int count)
        {
            return list.GetRange(index, count);
        }

        /// <summary>
        /// 迭代器
        /// </summary>
        /// <returns></returns>
        public List<T>.Enumerator GetEnumerator()
        {
            return list.GetEnumerator();
        }

        #region IList

        bool IList.IsFixedSize
        {
            get { return false; }
        }

        bool ICollection.IsSynchronized
        {
            get { return false; }
        }

        object ICollection.SyncRoot
        {
            get
            {
                return ((ICollection)list).SyncRoot;
            }
        }

        int IList.Add(object value)
        {
            try
            {
                Add((T)value);
            }
            catch (InvalidCastException)
            {
                throw new ArgumentException();
            }

            return Count - 1;
        }

        bool IList.Contains(object value)
        {
            if (IsCompatibleObject(value))
            {
                return Contains((T)value);
            }

            return false;
        }

        int IList.IndexOf(object value)
        {
            if (IsCompatibleObject(value))
            {
                return IndexOf((T)value);
            }

            return -1;
        }

        void IList.Insert(int index, object value)
        {
            try
            {
                Insert(index, (T)value);
            }
            catch (InvalidCastException)
            {
                throw new ArgumentException();
            }
        }

        void IList.Remove(object value)
        {
            if (IsCompatibleObject(value))
            {
                Remove((T)value);
            }
        }

        void ICollection.CopyTo(Array array, int index)
        {
            T[] tArray = array as T[];
            if (tArray != null)
            {
                list.CopyTo(tArray, index);
            }
            else
            {
                Type targetType = array.GetType().GetElementType();
                Type sourceType = typeof(T);
                if (!(targetType.IsAssignableFrom(sourceType) || sourceType.IsAssignableFrom(targetType)))
                {
                    throw new ArgumentException("Invalid array type");
                }

                object[] objects = array as object[];
                if (objects == null)
                {
                    throw new ArgumentException("Invalid array type");
                }

                int count = list.Count;
                try
                {
                    for (int i = 0; i < count; i++)
                    {
                        objects[index++] = list[i];
                    }
                }
                catch (ArrayTypeMismatchException)
                {
                    throw new ArgumentException("Invalid array type");
                }
            }
        }

        object IList.this[int index]
        {
            get
            {
                return list[index];
            }
            set
            {
                try
                {
                    this[index] = (T)value;
                }
                catch (InvalidCastException)
                {
                    throw new ArgumentException();
                }
            }
        }

        #endregion

        #region IList<T> Members

        /// <summary>
        /// 索引查询
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int IndexOf(T item)
        {
            return list.IndexOf(item);
        }

        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        public void Insert(int index, T item)
        {
            list.Insert(index, item);

            OnCollectionChanged(XCollectionChangeEventArgs.AddEvent(this, new T[] { item }));
        }

        /// <summary>
        /// 通过索引移除元素
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            T oldItem = list[index];
            list.RemoveAt(index);

            OnCollectionChanged(XCollectionChangeEventArgs.RemoveEvent(this, new T[] { oldItem }));
        }

        /// <summary>
        /// 索引访问
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T this[int index]
        {
            get
            {
                return list[index];
            }
            set
            {
                T oldItem = list[index];
                list[index] = value;

                OnCollectionChanged(XCollectionChangeEventArgs.ReplaceEvent(this, new T[] { oldItem }, new T[] { value }));
            }
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return list.GetEnumerator();
        }

        #endregion

        #region IEnumerable<T> Members

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return list.GetEnumerator();
        }

        #endregion

        #region 集合操作 ICollection<T> Members 

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            list.Add(item);

            OnCollectionChanged(XCollectionChangeEventArgs.AddEvent(this, new T[] { item }));
        }

        /// <summary>
        /// 清除
        /// </summary>
        public void Clear()
        {
            list.Clear();

            OnCollectionChanged(XCollectionChangeEventArgs.ClearEvent(this));
        }

        /// <summary>
        /// 包含
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(T item)
        {
            return list.Contains(item);
        }

        /// <summary>
        /// 复制
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            list.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// 数量
        /// </summary>
        public int Count
        {
            get { return list.Count; }
        }

        /// <summary>
        /// 仅读取
        /// </summary>
        public bool IsReadOnly
        {
            get { return ((IList<T>)list).IsReadOnly; }
        }

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(T item)
        {
            bool result = list.Remove(item);

            OnCollectionChanged(XCollectionChangeEventArgs.RemoveEvent(this, new T[] { item }));
            return result;
        }

        /// <summary>
        /// 排序 ：默认
        /// </summary>
        public void Sort()
        {
            list.Sort();

            OnCollectionChanged(XCollectionChangeEventArgs.SortEvent(this, list));
        }


        /// <summary>
        /// 排序 ： 通过外围传入比较器
        /// </summary>
        public void Sort(Comparison<T> comparison)
        {
            list.Sort(comparison);

            OnCollectionChanged(XCollectionChangeEventArgs.SortEvent(this, list));
        }


        /// <summary>
        /// 排序 ： 通过外围传入比较器
        /// </summary>
        public void Sort(IComparer<T> comparer)
        {
            list.Sort(comparer);

            OnCollectionChanged(XCollectionChangeEventArgs.SortEvent(this, list));
        }

        #endregion

        private Action<XValueEventArg> _sendEvent;

        /// <summary>
        /// 集合事件通知
        /// </summary>
        public event Action<XValueEventArg> sendEvent
        {
            add => _sendEvent += value;
            remove => _sendEvent -= value;
        }

        /// <summary>
        /// 集合发生变化
        /// </summary>
        /// <param name="eventArgs"></param>
        protected void OnCollectionChanged(XCollectionChangeEventArgs eventArgs)
        {
            // 发送集合本身事件消息
            _sendEvent?.Invoke(eventArgs);

            // 发送集合数量属性变化消息
            _sendEvent?.Invoke(XCollectionChangeEventArgs.CountEvent(this, Count));
        }

        private static bool IsCompatibleObject(object value)
        {
            // Non-null values are fine. Only accept nulls if T is a class or Nullable<U>.
            // Note that default(T) is not equal to null for value types except when T is Nullable<U>. 
            return ((value is T) || (value == null && default(T) == null));
        }
    }
}
