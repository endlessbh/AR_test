using XCSJ.Attributes;

namespace XCSJ.PluginSMS.States.Base
{
    /// <summary>
    /// 基础属性比较：用于两属性值比较的情况，即左值与右值根据检测规则的比较情况；
    /// </summary>
    [XCSJ.Attributes.Icon(EIcon.Property)]
    public abstract class BasePropertyCompare<T> : Trigger<T>
        where T : BasePropertyCompare<T>
    {
        /// <summary>
        /// 左值
        /// </summary>
        //public abstract object leftValue { get; }

        /// <summary>
        /// 检测规则
        /// </summary>
        //public EDetectionRule detectionRule { get; }

        /// <summary>
        /// 右值
        /// </summary>
        //public abstract object rightValue { get; }
    }

    /// <summary>
    /// 基础属性获取
    /// </summary>
    [XCSJ.Attributes.Icon(EIcon.Property)]
    public abstract class BasePropertyGet<T> : LifecycleExecutor<T>
        where T : BasePropertyGet<T>
    {
    }

    /// <summary>
    /// 基础属性设置
    /// </summary>
    [XCSJ.Attributes.Icon(EIcon.Property)]
    public abstract class BasePropertySet<T> : LifecycleExecutor<T>
        where T : BasePropertySet<T>
    {
    }
}
