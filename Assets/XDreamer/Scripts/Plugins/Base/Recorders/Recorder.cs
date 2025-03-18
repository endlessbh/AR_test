using System;
using System.Collections.Generic;
using System.Linq;
using XCSJ.Interfaces;
using XCSJ.LitJson;

namespace XCSJ.Extension.Base.Recorders
{
    /// <summary>
    /// 记录器：可用于批量记录数据的记录器；
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TRecord"></typeparam>
    public abstract class Recorder<T, TRecord> : IBatchRecorder<T, TRecord>
        where TRecord : class, ISingleRecord<T>, new()
    {
        /// <summary>
        /// 记录信息列表
        /// </summary>
        public List<TRecord> _records = new List<TRecord>();

        /// <summary>
        /// 记录列表
        /// </summary>
        [Json(false)]
        public TRecord[] records => _records.ToArray();

        /// <summary>
        /// 记录列表中第一条记录：如果无记录返回null
        /// </summary>
        [Json(false)]
        public TRecord firstRecod => _records.FirstOrDefault();

        /// <summary>
        /// 记录列表中末一条记录：如果无记录返回null
        /// </summary>
        [Json(false)]
        public TRecord lastRecod => _records.LastOrDefault();

        /// <summary>
        /// 有无记录
        /// </summary>
        /// <returns></returns>
        public bool HasRecord() => recordCount > 0;

        /// <summary>
        /// 记录数量
        /// </summary>
        public int recordCount => _records.Count;

        /// <summary>
        /// 清空记录信息列表<see cref="_records"/>
        /// </summary>
        public void Clear()
        {
            _records.Clear();
        }

        /// <summary>
        /// 记录
        /// </summary>
        /// <param name="obj"></param>
        public virtual void Record(T obj)
        {
            //考虑可能存在的比较函数重载情况
            if (!Equals(obj, null))
            {
                var record = new TRecord();
                record.Record(obj);
                _records.Add(record);
            }
        }

        /// <summary>
        /// 批量记录
        /// </summary>
        /// <param name="objects"></param>
        public virtual void Record(IEnumerable<T> objects)
        {
            if (objects == null) return;
            foreach (var obj in objects)
            {
                Record(obj);
            }
        }

        /// <summary>
        /// 恢复：将所有信息都执行恢复
        /// </summary>
        public virtual void Recover()
        {
            foreach (var i in _records)
            {
                try
                {
                    i.Recover();
                }
                catch { }
            }
        }

        /// <summary>
        /// 逆向恢复全部记录
        /// </summary>
        public virtual void ReverseRecover()
        {
            for (int i = _records.Count - 1; i >= 0; i--)
            {
                try
                {
                    _records[i].Recover();
                }
                catch { }
            }
        }

        /// <summary>
        /// 逆向恢复全部记录并清空记录
        /// </summary>
        public virtual void ReverseRecoverAndClear()
        {
            ReverseRecover();
            Clear();
        }

        /// <summary>
        /// 逆向恢复最后一条记录
        /// </summary>
        public virtual void RecoverLast()
        {
            var count = _records.Count;
            if (count > 0)
            {
                try
                {
                    _records[count - 1].Recover();
                }
                catch { }
            }
        }

        /// <summary>
        /// 移除最后一条记录
        /// </summary>
        public virtual void RemoveLast()
        {
            var count = _records.Count;
            if (count > 0)
            {
                try
                {
                    _records.RemoveAt(count - 1);
                }
                catch { }
            }
        }

        /// <summary>
        /// 恢复和移除最后一个记录
        /// </summary>
        public virtual void RecoverAndRemoveLast()
        {
            var index = _records.Count - 1;
            if (index >= 0)
            { 
                try
                {
                    _records[index].Recover();
                }
                catch { }
                try
                {
                    _records.RemoveAt(index);
                }
                catch { }
            }
        }

        /// <summary>
        /// 条件恢复
        /// </summary>
        /// <param name="canRecoverFunc"></param>
        public virtual void Recover(Func<TRecord, bool> canRecoverFunc)
        {
            if (canRecoverFunc == null) return;
            foreach (var i in _records)
            {
                try
                {
                    if (canRecoverFunc(i))
                    {
                        i.Recover();
                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// 按输入条件恢复记录和移除记录对象
        /// </summary>
        /// <param name="canRecoverFunc"></param>
        public virtual void RecoverAndRemove(Func<TRecord, bool> canRecoverFunc)
        {
            if (canRecoverFunc == null) return;

            var records = new List<TRecord>(_records);
            foreach (var i in records)
            {
                try
                {
                    if (canRecoverFunc(i))
                    {
                        i.Recover();
                        _records.Remove(i);
                    }
                }
                catch { }
            }
        }
    }

    /// <summary>
    /// 批量记录器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IBatchRecorder<T, TRecord> : IBatchRecorder<T>
        where TRecord : class, ISingleRecord<T>, new()
    {
        /// <summary>
        /// 记录列表
        /// </summary>
        TRecord[] records { get; }

        /// <summary>
        /// 记录列表中第一条记录：如果无记录返回null
        /// </summary>
        TRecord firstRecod { get; }
    }

    /// <summary>
    /// 批量记录器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IBatchRecorder<T> : IRecorder<T>, IRecord<IEnumerable<T>> { }

    /// <summary>
    /// 单一记录
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISingleRecord<T> : IRecord<T>, IRecover { }
}
