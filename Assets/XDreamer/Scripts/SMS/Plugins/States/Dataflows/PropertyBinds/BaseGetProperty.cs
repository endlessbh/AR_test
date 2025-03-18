using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Attributes;
using XCSJ.Extension.Base.Dataflows.Binders;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;
using XCSJ.Scripts;

namespace XCSJ.PluginSMS.States.Dataflows.PropertyBinds
{
    /// <summary>
    /// 基础获取属性
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseGetProperty<T> : BasePropertyGet<T>, ISerializationCallbackReceiver, ITypeBinderGetter
        where T : BaseGetProperty<T>
    {
        /// <summary>
        /// 绑定器:用于绑定对象的字段或属性信息的对象
        /// </summary>
        [Name("绑定器")]
        [Tip("用于绑定对象的字段或属性信息的对象", "An object used to bind field or property information of an object")]
        [FormerlySerializedAs(nameof(_binder))]
        public FieldPropertyMethodMemberBinder _binder = new FieldPropertyMethodMemberBinder();

        /// <summary>
        /// 绑定器
        /// </summary>
        public FieldPropertyMethodMemberBinder binder => _binder;

        #region ITypeBinderGetter

        /// <summary>
        /// 获取器所有者
        /// </summary>
        public UnityEngine.Object owner => this;

        /// <summary>
        /// 类型绑定器获取器
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<ITypeBinder> GetTypeBinders() => new ITypeBinder[] { binder };

        #endregion

        /// <summary>
        /// 结果变量字符串
        /// </summary>
        [Name("结果变量字符串")]
        [Tip("将成功执行的结果信息存储在结果变量字符串对应的变量中", "Store the successful execution result information in the variable corresponding to the result variable string")]
        [VarString(EVarStringHierarchyKeyMode.Set)]
        [ValidityCheck(EValidityCheckType.NotNullOrEmpty)]
        [FormerlySerializedAs("_variableName")]
        public string _resultVarString;

        /// <summary>
        /// 变量名
        /// </summary>
        public string resultVarString => _resultVarString;

        /// <summary>
        /// 将值设置到变量
        /// </summary>
        /// <param name="value"></param>
        protected void SetToVariable(object value)
        {
            _resultVarString.TrySetOrAddSetHierarchyVarValue(value);
        }

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="stateData"></param>
        /// <param name="executeMode"></param>
        public override void Execute(StateData stateData, EExecuteMode executeMode) => SetToVariable(_binder.memberValue);

        /// <summary>
        /// 输出友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString()
        {
            return _resultVarString + "=" + _binder.ToFriendlyString();
        }

        /// <summary>
        /// 判断数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity()
        {
            return base.DataValidity() && _binder.DataValidity();
        }

        #region ISerializationCallbackReceiver

        void ISerializationCallbackReceiver.OnBeforeSerialize() { }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            CommonFun.VarNameToVarString(ref _resultVarString);
        }

        #endregion
    }
}
