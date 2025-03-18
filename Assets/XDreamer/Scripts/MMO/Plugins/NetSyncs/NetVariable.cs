using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.Scripts;
using XCSJ.Tools;
using XCSJ.PluginTools;
using XCSJ.PluginCommonUtils.Tools;

namespace XCSJ.PluginMMO.NetSyncs
{
    [XCSJ.Attributes.Icon(EIcon.Variable)]
    [DisallowMultipleComponent]
    [Name("网络变量")]
    [Tool(MMOHelper.CategoryName, MMOHelper.ToolPurpose, rootType = typeof(MMOManager))]
    public class NetVariable : NetMB, ISerializationCallbackReceiver
    {
        [Name("变量名")]
        [VarString]
        public string variableName = "";

        #region ISerializationCallbackReceiver

        void ISerializationCallbackReceiver.OnBeforeSerialize() { }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            CommonFun.VarNameToVarString(ref variableName);
        }

        #endregion

        [SyncVar]
        [Readonly]
        [Name("变量值")]
        public string variableValue = "";

        [Readonly]
        [Name("上一次变量值")]
        public string lastVariableValue;

        [Readonly]
        [Name("原始变量值")]
        public string originalVariableValue;

        public override void OnSyncEnable()
        {
            base.OnSyncEnable();

            variableName.TryGetHierarchyVarValue(out var varValue);
            originalVariableValue = lastVariableValue = variableValue = varValue.ToScriptParamString();

            Variable.onValueChanged += OnVariableValueChanged;
        }

        public override void OnSyncDisable()
        {
            base.OnSyncDisable();

            Variable.onValueChanged -= OnVariableValueChanged;

            variableName.TrySetOrAddSetHierarchyVarValue(originalVariableValue);
        }

        protected override bool OnTimedCheckChange()
        {
            return variableValue != lastVariableValue;
        }

        protected override void OnSyncVarChanged()
        {
            base.OnSyncVarChanged();
            variableName.TrySetOrAddSetHierarchyVarValue(variableValue);
            lastVariableValue = variableValue;
        }

        private void OnVariableValueChanged(Variable variable)
        {
            if (variable.name == this.variableName && variable.varScope == EVarScope.Global)
            {
                variableValue = variable.GetValue() as string;
            }
        }
    }
}
