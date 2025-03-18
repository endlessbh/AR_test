using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Serialization;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.Extension.Base.Dataflows.Binders;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;
using XCSJ.Scripts;

namespace XCSJ.PluginSMS.States.CNScripts
{
    /// <summary>
    /// 变量赋值：变量赋值组件是用于执行变量赋值的执行体
    /// </summary>
    [ComponentMenu(SMSHelperExtension.CNScriptCategoryName + "/" + Title, typeof(SMSManager))]
    [Name(Title, nameof(VariableAssignment))]
    [XCSJ.Attributes.Icon(EIcon.Variable)]
    [Tip("变量赋值组件是用于执行变量赋值的执行体", "The variable assignment component is the executor used to perform variable assignment")]
    public class VariableAssignment : LifecycleExecutor<VariableAssignment>, ISerializationCallbackReceiver, IDynamicLabel, ITypeBinderGetter
    {       

        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "变量赋值";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(SMSHelperExtension.CNScriptCategoryName, typeof(SMSManager))]
        [StateComponentMenu(SMSHelperExtension.CNScriptCategoryName + "/" + Title, typeof(SMSManager))]
        [Name(Title, nameof(VariableAssignment))]
        [Tip("变量赋值组件是用于执行变量赋值的执行体", "The variable assignment component is the executor used to perform variable assignment")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 变量
        /// </summary>
        [Name("变量")]
        [Tip("待赋值的变量", "Variable to be assigned")]
        [VarString(EVarStringHierarchyKeyMode.Set)]
        [ValidityCheck(EValidityCheckType.NotNullOrEmpty)]
        [FormerlySerializedAs("variable")]
        public string _variable;

        #region ISerializationCallbackReceiver

        void ISerializationCallbackReceiver.OnBeforeSerialize() { }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            CommonFun.VarNameToVarString(ref _variable);
        }

        #endregion

        /// <summary>
        /// 新值
        /// </summary>
        [Name("新值")]
        public Argument _newValue = new Argument();

        /// <summary>
        /// 变量赋值元组列表
        /// </summary>
        [Name("变量赋值元组列表")]
        [OnlyMemberElements]
        [ArrayElement]
        public List<VariableAssignmentTuple> _variableAssignmentTuples = new List<VariableAssignmentTuple>();

        /// <summary>
        /// 信息
        /// </summary>
        [Name("变量赋值元组")]
        [Serializable]
        [LanguageFileOutput]
        public class VariableAssignmentTuple
        {
            /// <summary>
            /// 变量
            /// </summary>
            [Name("变量")]
            [Tip("待赋值的变量", "Variable to be assigned")]
            [VarString(EVarStringHierarchyKeyMode.Set)]
            [ValidityCheck(EValidityCheckType.NotNullOrEmpty)]
            public string _variable;

            /// <summary>
            /// 新值
            /// </summary>
            [Name("新值")]
            public Argument _newValue = new Argument();

            /// <summary>
            /// 设置值
            /// </summary>
            public void SetValue()
            {
                _variable.TrySetOrAddSetHierarchyVarValue(_newValue.GetValueToString() ?? "");
            }
        }


        PropertyPathCache cache = new PropertyPathCache();

        /// <summary>
        /// 获取动态标签
        /// </summary>
        /// <param name="propertyPath"></param>
        /// <param name="fieldInfo"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        public GUIContent GetDynamicLabel(string propertyPath, FieldInfo fieldInfo, GUIContent label)
        {
            if (fieldInfo.Name == nameof(_variableAssignmentTuples) && cache.TryGetArrayElementData(propertyPath, out var data) && data.index < _variableAssignmentTuples.Count)
            {
                var variable = _variableAssignmentTuples[data.index];
                data.label.text = data.indexString + "." + variable._variable;
                data.label.tooltip = variable._variable + "=" + variable._newValue.ToFriendlyString();
                return data.label;
            }
            return label;
        }

        /// <summary>
        /// 完成
        /// </summary>
        /// <returns></returns>
        public override bool Finished() => true;

        /// <summary>
        /// 转友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString()
        {
            return _variable + VariableCompareHelper.ToAbbreviations(ECompareOperator.Equal) + _newValue.ToFriendlyString();
        }

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="stateData"></param>
        /// <param name="executeMode"></param>
        public override void Execute(StateData stateData, EExecuteMode executeMode)
        {
            _variable.TrySetOrAddSetHierarchyVarValue(_newValue.GetValueToString() ?? "");
            foreach (var data in _variableAssignmentTuples)
            {
                data.SetValue();
            }
        }


        #region ITypeBinderGetter

        /// <summary>
        /// 获取器所有者
        /// </summary>
        public UnityEngine.Object owner => this;

        /// <summary>
        /// 类型绑定器获取器
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ITypeBinder> GetTypeBinders() => _variableAssignmentTuples.Cast(t => t._newValue._fieldPropertyMethodBinderValue);

        #endregion
    }
}
