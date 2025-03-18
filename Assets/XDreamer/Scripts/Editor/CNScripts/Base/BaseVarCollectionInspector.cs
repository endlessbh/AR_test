using System;
using UnityEditor;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.Extension.CNScripts.Base;
using XCSJ.PluginCommonUtils;
using XCSJ.Scripts;

namespace XCSJ.EditorExtension.CNScripts.Base
{
    /// <summary>
    /// 基础变量集合检查器
    /// </summary>
    [CustomEditor(typeof(BaseVarCollection), true)]
    [Name("基础变量集合检查器")]
    public class BaseVarCollectionInspector : MBInspector<BaseVarCollection>
    {
        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            Variable.onValueChanged -= OnVariableValueChanged;
            Variable.onValueChanged += OnVariableValueChanged;
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            //Debug.Log("OnDisable");
            base.OnDisable();
            Variable.onValueChanged -= OnVariableValueChanged;
        }

        /// <summary>
        /// 当变量值变化后回调
        /// </summary>
        /// <param name="variable"></param>
        protected void OnVariableValueChanged(Variable variable)
        {
            var mb = this.mb;
            if (mb && variable.varScope == mb.varCollection.varScope)
            {
                UICommonFun.DelayCall(Repaint);
            }
        }

        /// <summary>
        /// 当执行撤消重做时
        /// </summary>
        protected override void OnUndoRedoPerformed()
        {
            base.OnUndoRedoPerformed();

            mb.varCollection.RefreshVarDictionary();
        }
    }
}
