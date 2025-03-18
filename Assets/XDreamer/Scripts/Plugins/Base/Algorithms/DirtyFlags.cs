using System;

namespace XCSJ.Extension.Base.Algorithms
{
    /// <summary>
    /// 脏标记
    /// </summary>
    public class DirtyFlags
    {
        /// <summary>
        /// 默认为脏
        /// </summary>
        public bool isDirty { get; private set; } = true;

        /// <summary>
        /// 设置为脏
        /// </summary>
        public void SetDirty() => isDirty = true;

        /// <summary>
        /// 清除脏
        /// </summary>
        public void ClearDirty() => isDirty = false;
    }

    /// <summary>
    /// 脏数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DirtyData<T> : DirtyFlags
    {
        /// <summary>
        /// 数据
        /// </summary>
        public T data { get; set; } = default;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="data"></param>
        public DirtyData(T data = default)
        {
            this.data = data;
        }

        /// <summary>
        /// 获取数据：如果数据为脏，则调用更新；
        /// </summary>
        /// <param name="update">更新：如无效，报出异常</param>
        /// <returns></returns>
        public T GetData(Func<T> update)
        {
            if (isDirty)
            {
                data = update();
                ClearDirty();
            }
            return data;
        }
    }

    /// <summary>
    /// 脏字符串
    /// </summary>
    public class DirtyString : DirtyData<string>
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="data"></param>
        public DirtyString(string data = "") : base(data) { }
    }
}
