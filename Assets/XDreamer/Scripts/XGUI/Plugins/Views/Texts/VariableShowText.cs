using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginTools;
using XCSJ.PluginXGUI.Base;
using XCSJ.Scripts;

namespace XCSJ.PluginXGUI.Views.Texts
{
    [Name("变量显示文本")]
    [DisallowMultipleComponent]
    [XCSJ.Attributes.Icon(EIcon.Variable)]
    [Tip("将变量值显示在Text上，文本会随着变量变化而变化", "Display the variable value on the text, and the text will change with the change of the variable")]
    [Tool(XGUICategory.Component, nameof(XGUIManager))]
    [RequireManager(typeof(XGUIManager), typeof(ToolsManager))]
    public class VariableShowText : View, ISerializationCallbackReceiver
    {
        [Name("变量")]
        [VarString(EVarStringHierarchyKeyMode.Get)]
        [ValidityCheck(EValidityCheckType.NotNullOrEmpty)]
        public string variable;

        #region ISerializationCallbackReceiver

        void ISerializationCallbackReceiver.OnBeforeSerialize() { }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            CommonFun.VarNameToVarString(ref variable);
        }

        #endregion

        [Name("文本")]
        [ComponentPopup]
        public Text text;

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            if (!text) text = GetComponent<Text>();

            HierarchyVarEvent.onChanged += OnChanged;
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            HierarchyVarEvent.onChanged -= OnChanged;
        }

        protected void OnChanged(IHierarchyVar hierarchyVar)
        {
            if (hierarchyVar.varString == variable)
            {
                if(text) text.text = hierarchyVar.stringValue;
            }
        }
    }
}