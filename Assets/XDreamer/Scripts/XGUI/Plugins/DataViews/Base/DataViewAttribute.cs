using System;
using XCSJ.Algorithms;

namespace XCSJ.PluginXGUI.DataViews.Base
{
    /// <summary>
    /// 修饰数据视图子类对象：将当前视图与类型数据进行关联
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class DataViewAttribute : LinkedTypeAttribute
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="type"></param>
        /// <param name="forChildClasses"></param>
        /// <param name="purpose"></param>
        public DataViewAttribute(Type type, bool forChildClasses = false, string purpose = "") : base(type, forChildClasses, ToPurpose(type, purpose))
        {
            
        }

        /// <summary>
        /// 获取目标字符串
        /// </summary>
        /// <param name="type"></param>
        /// <param name="purpose">目的</param>
        /// <returns></returns>
        public static string ToPurpose(Type type, string purpose = "") => nameof(DataViewAttribute) + "." + type.Name;

        /// <summary>
        /// 获取数据视图类型
        /// </summary>
        /// <param name="dataType">被关联的数据类型</param>
        /// <param name="purpose">目的</param>
        /// <returns></returns>
        public static Type GetDataViewType(Type dataType, string purpose = "") => LinkedTypeCache.Get(dataType, ToPurpose(dataType, purpose));
    }
}
