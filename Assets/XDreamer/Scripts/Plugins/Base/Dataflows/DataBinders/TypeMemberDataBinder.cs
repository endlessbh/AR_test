using System;
using System.Reflection;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Dataflows.Binders;
using XCSJ.Extension.Base.Dataflows.Models;

namespace XCSJ.Extension.Base.Dataflows.DataBinders
{
    /// <summary>
    /// 基础类型成员数据绑定器
    /// </summary>
    public abstract class BaseTypeMemberDataBinder : DataBinder, ITypeMemberDataBinder
    {
        /// <summary>
        /// 主类型
        /// </summary>
        public override Type mainType => typeMemberBinder?.mainType;

        /// <summary>
        /// 成员名
        /// </summary>
        public override string memberName
        {
            get => typeMemberBinder?.memberName;
            set
            {
                if (typeMemberBinder != null)
                {
                    typeMemberBinder.memberName = value;
                }
            }
        }

        /// <summary>
        /// 反射标记量:用于获取成员时使用
        /// </summary>
        public override BindingFlags bindingFlags => typeMemberBinder != null ? typeMemberBinder.bindingFlags : base.bindingFlags;

        /// <summary>
        /// 包含基础类型
        /// </summary>
        public override bool includeBaseType => typeMemberBinder != null ? typeMemberBinder.includeBaseType : base.includeBaseType;

        /// <summary>
        /// 类型成员绑定器对象
        /// </summary>
        public ITypeMemberBinder typeMemberBinder { get; private set; }

        /// <summary>
        /// 封装类的事件函数
        /// </summary>
        protected event Action<XValueEventArg> _sendEvent = null;

        /// <summary>
        /// 事件绑定接口
        /// </summary>
        public event Action<XValueEventArg> sendEvent
        {
            add => _sendEvent += value;
            remove => _sendEvent -= value;
        }

        /// <summary>
        /// 发送事件
        /// </summary>
        /// <param name="eventArg"></param>
        protected virtual void SendEvent(XValueEventArg eventArg)
        {
            _sendEvent?.Invoke(eventArg);
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="typeMemberBinder"></param>
        public virtual void Init(ITypeMemberBinder typeMemberBinder)
        {
            this.typeMemberBinder = typeMemberBinder;
        }

        /// <summary>
        /// 初始化值
        /// </summary>
        public abstract void InitValue();

        /// <summary>
        /// 绑定
        /// </summary>
        /// <param name="onReceiveEvent"></param>
        public virtual void Bind(Action<XValueEventArg> onReceiveEvent)
        {
            _sendEvent += onReceiveEvent;
        }

        /// <summary>
        /// 解绑
        /// </summary>
        /// <param name="onReceiveEvent"></param>
        public virtual void Unbind(Action<XValueEventArg> onReceiveEvent)
        {
            _sendEvent -= onReceiveEvent;
        }

        /// <summary>
        /// 尝试获取值
        /// </summary>
        /// <param name="eventArg"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual bool TryGet(XValueEventArg eventArg, out object value)
        {
            // 优先从参数中，获取数据
            if (eventArg != null && eventArg.hasValue)
            {
                value = eventArg.value;
                return true;
            }
            value = default;
            return false;
        }

        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="linkedBindData"></param>
        /// <param name="eventArg"></param>
        public abstract void Set(ITypeMemberDataBinder linkedBindData, XValueEventArg eventArg);

        /// <summary>
        /// 设置数据绑定器的主体对象 ：主要用于动态绑定真实数据项
        /// </summary>
        /// <param name="mainObject">主体对象</param>
        public bool SetMainObject(object mainObject)
        {
            // 类型不匹配
            if (mainObject != null && !typeMemberBinder.mainType.IsAssignableFrom(mainObject.GetType()))
            {
                return false;
            }

            typeMemberBinder.mainObject = mainObject;
            return true;
        }
    }

    /// <summary>
    /// 类型成员数据绑定器 : 使用反射方式获取和设置值
    /// 识别类型成员主体对象和成员对象是否具有触发事件接口，如果有，则绑定
    /// </summary>
    public class TypeMemberDataBinder : BaseTypeMemberDataBinder
    {
        /// <summary>
        /// 主体发生事件接口
        /// </summary>
        protected ISendEvent _mainObjectSendEvent = null;

        /// <summary>
        /// 成员发生事件接口
        /// </summary>
        protected ISendEvent _memberValueSendEvent = null;

        /// <summary>
        /// 绑定
        /// </summary>
        /// <param name="onReceiveEvent"></param>
        public override void Bind(Action<XValueEventArg> onReceiveEvent)
        {
            base.Bind(onReceiveEvent);

            BindMainObjectSendEvent();

            BindMemberSendEvent();
        }

        /// <summary>
        /// 解绑
        /// </summary>
        /// <param name="onReceiveEvent"></param>
        public override void Unbind(Action<XValueEventArg> onReceiveEvent)
        {
            base.Unbind(onReceiveEvent);

            UnbindMainObjectSendEvent();

            UnbindMemberSendEvent();
        }

        /// <summary>
        /// 绑定是事件发送接口的对象主体
        /// </summary>
        protected virtual void BindMainObjectSendEvent()
        {
            _mainObjectSendEvent = typeMemberBinder.mainObject as ISendEvent;
            if (_mainObjectSendEvent != null)
            {
                _mainObjectSendEvent.sendEvent += OnMainObjectSendEvent;
            }
        }

        protected virtual void UnbindMainObjectSendEvent()
        {
            if (_mainObjectSendEvent != null)
            {
                _mainObjectSendEvent.sendEvent -= OnMainObjectSendEvent;
            }
        }

        /// <summary>
        /// 绑定是事件发送接口的对象成员
        /// </summary>
        protected virtual void BindMemberSendEvent()
        {
            if (typeMemberBinder.hasMember)
            {
                _memberValueSendEvent = typeMemberBinder.memberValue as ISendEvent;
                if (_memberValueSendEvent != null)
                {
                    _memberValueSendEvent.sendEvent += OnMemberValueSendEvent;
                }
            }
        }

        /// <summary>
        /// 解绑成员对象
        /// </summary>
        protected virtual void UnbindMemberSendEvent()
        {
            if (_memberValueSendEvent != null)
            {
                _memberValueSendEvent.sendEvent -= OnMemberValueSendEvent;
            }
        }

        /// <summary>
        /// 主对象事件发送
        /// </summary>
        /// <param name="eventArg"></param>
        protected void OnMainObjectSendEvent(XValueEventArg eventArg)
        {
            SendEvent(eventArg);
        }

        /// <summary>
        /// 中转成员对象事件发送到当前函数，再发至上层        
        /// </summary>
        /// <param name="eventArg"></param>
        protected void OnMemberValueSendEvent(XValueEventArg eventArg)
        {
            SendEvent(eventArg);
        }

        /// <summary>
        /// 发送值
        /// </summary>
        /// <param name="eventArg"></param>
        protected override void SendEvent(XValueEventArg eventArg)
        {
            eventArg.sender = this;
            base.SendEvent(eventArg);
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public override void InitValue()
        {
            if (TryGet(null, out object value))
            {
                base.SendEvent(new XValueEventArg(this, EDefaultEventType.Init, value));
            }
        }

        /// <summary>
        /// 尝试获取数值
        /// </summary>
        /// <param name="eventArg"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool TryGet(XValueEventArg eventArg, out object value)
        {
            if (base.TryGet(eventArg, out value))
            {
                return true;
            }

            if (IsFieldOrProperty)
            {
                value = typeMemberBinder.memberValue;
                return true;
            }

            value = default;
            return false;
        }

        /// <summary>
        /// 设置数值
        /// </summary>
        /// <param name="linkedBindData"></param>
        /// <param name="eventArg"></param>
        public override void Set(ITypeMemberDataBinder linkedBindData, XValueEventArg eventArg)
        {
            // 当对象是字段和属性时，尝试获取值，并赋值
            if (IsFieldOrProperty)
            {
                if (linkedBindData.TryGet(eventArg, out object value))
                {
                    typeMemberBinder.memberValue = value;
                }
            }
            else // 对象是事件或方法时，尝试直接调用，当前只支持空方法调用
            {
                typeMemberBinder.memberValue = null;
            }
        }

        /// <summary>
        /// 属性或字段
        /// </summary>
        protected bool IsFieldOrProperty => typeMemberBinder.memberInfo is FieldInfo || typeMemberBinder.memberInfo is PropertyInfo;
    }

    /// <summary>
    /// 类型成员数据绑定器泛型类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class TypeMemberDataBinder<T> : TypeMemberDataBinder where T : class
    {
        /// <summary>
        /// 目标对象
        /// </summary>
        public T target => typeMemberBinder.mainObject as T;
    }

    /// <summary>
    /// 缺省类型成员数据绑定器
    /// </summary>
    [Serializable]
    [DataBinder(typeof(object))]
    [Name("缺省类型成员数据绑定器")]
    public class DefaultTypeMemberDataBinder : TypeMemberDataBinder<object> { }
}
