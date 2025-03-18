using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using XCSJ.Collections;
using XCSJ.EditorCommonUtils.CNScripts;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginMechanicalMotion;
using XCSJ.PluginMechanicalMotion.CNScripts;
using XCSJ.PluginMechanicalMotion.Tools;
using XCSJ.Scripts;

namespace XCSJ.EditorMechanicalMotion.CNScripts
{
    /// <summary>
    /// 自定义中文脚本机构参数绘制器
    /// </summary>
    [ScriptParamType(MechanismPathType_ScriptParam.MechanismPathType)]
    public class MechanismPathType_ScriptParamDrawer : ScriptParamDrawer
    {
        /// <summary>
        /// 绘制值
        /// </summary>
        public override void OnDrawValue()
        {
            EditorGUILayout.BeginHorizontal();
            {
                var paths = CommonFun.GetComponentsInChildren<PlaneMechanism>(true).Cast(m => m.path.Substring(1)).ToList();
                paths.Sort();

                EditorGUILayout.LabelField("", GUILayout.Width(8));

                paramObject = EditorGUILayout.ObjectField((PlaneMechanism)paramObject, typeof(PlaneMechanism), true);

                var currentPath = MechanicalMotionHelper.MechanismToString((PlaneMechanism)paramObject);
                var index = currentPath.Length > 0 ? paths.IndexOf(currentPath.Substring(1)) : -1;
                var indexNew = EditorGUILayout.Popup(index, paths.ToArray());
                if (indexNew != index)
                {
                    paramObject = MechanicalMotionHelper.StringToMechanism("/" + paths[indexNew]);
                }
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
