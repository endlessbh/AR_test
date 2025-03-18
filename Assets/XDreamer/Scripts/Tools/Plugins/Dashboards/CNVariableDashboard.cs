using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.Scripts;

namespace XCSJ.PluginTools.Dashboards
{
    /// <summary>
    /// 中文变量仪表盘
    /// </summary>
    [Name("中文变量仪表盘")]
    public class CNVariableDashboard : Dashboard, ISerializationCallbackReceiver
    {
        /// <summary>
        /// 变量
        /// </summary>
        [Name("变量")]
        [VarString(EVarStringHierarchyKeyMode.Get)]
        public string _variable;

        #region ISerializationCallbackReceiver

        void ISerializationCallbackReceiver.OnBeforeSerialize() { }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            CommonFun.VarNameToVarString(ref _variable);
        }

        #endregion

        public override float needleAngle
        {
            get
            {
                if (_valid
                    && _variable.TryGetHierarchyVarValue(out var value)
                    && float.TryParse(value.ToScriptParamString(), out float number))
                {
                    return number;
                }
                return 0;
            }
        }

        /// 开始
        /// </summary>
        protected override void Start()
        {
            base.Start();

            _valid = _valid && !string.IsNullOrEmpty(_variable);
        }
    }
}
