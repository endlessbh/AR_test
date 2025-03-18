using System.Linq;
using UnityEditor;
using XCSJ.Collections;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.CNScripts;
using XCSJ.PluginCamera;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginsCameras.Base;
using XCSJ.Scripts;

namespace XCSJ.PluginsCameras.CNScripts
{
    /// <summary>
    /// 相机控制器名称脚本参数绘制器
    /// </summary>
    [ScriptParamType(CameraControllerName_ScriptParam.ScriptParamType)]
    class CameraControllerName_ScriptParamDrawer : StringScriptParamDrawer
    {
        /// <summary>
        /// 当绘制值
        /// </summary>
        public override void OnDrawValue()
        {
            EditorGUI.indentLevel = 2;
            paramObject = UICommonFun.Popup(paramObject as string, ComponentCache.Get(typeof(BaseCameraMainController), true).components.Cast(c => c.name).Distinct().ToArray());
            //base.OnDrawValue();
        }
    }
}
