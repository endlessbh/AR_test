using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.ComponentModel;
using XCSJ.EditorExtension.Base;
using XCSJ.EditorSMS.Inspectors;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;

namespace XCSJ.EditorSMS
{
    /// <summary>
    /// 组件头部菜单类
    /// </summary>
    public class ComponentHeaderMenu
    {
        /// <summary>
        /// 重置组件
        /// </summary>
        /// <param name="context"></param>
        [ComponentHeaderMenu(EditorSMSHelper.StateComponentHeaderMenuName, "重置", index = 30)]
        [ComponentHeaderMenu(EditorSMSHelper.TransitionComponentHeaderMenuName, "重置", index = 30)]
        [ComponentHeaderMenu(EditorSMSHelper.StateGroupComponentHeaderMenuName, "重置", index = 30)]
        public static void ResetComponent(ComponentHeaderMenuContext context)
        {
            context.component?.Reset();
        }

        /// <summary>
        /// 编辑脚本
        /// </summary>
        /// <param name="context"></param>
        [ComponentHeaderMenu(EditorSMSHelper.StateComponentHeaderMenuName, "编辑脚本", index = 50)]
        [ComponentHeaderMenu(EditorSMSHelper.TransitionComponentHeaderMenuName, "编辑脚本", index = 50)]
        [ComponentHeaderMenu(EditorSMSHelper.StateGroupComponentHeaderMenuName, "编辑脚本", index = 50)]
        public static void EditScript(ComponentHeaderMenuContext context) => EditorHelper.OpenMonoScript(context.component);

        /// <summary>
        /// 选中脚本
        /// </summary>
        /// <param name="context"></param>
        [ComponentHeaderMenu(EditorSMSHelper.StateComponentHeaderMenuName, "选中脚本", index = 51)]
        [ComponentHeaderMenu(EditorSMSHelper.TransitionComponentHeaderMenuName, "选中脚本", index = 51)]
        [ComponentHeaderMenu(EditorSMSHelper.StateGroupComponentHeaderMenuName, "选中脚本", index = 51)]
        public static void SelectScript(ComponentHeaderMenuContext context)
        {
            var script = MonoScript.FromScriptableObject(context.component);
            if (script) Selection.activeObject = script;
        }

        /// <summary>
        /// 编辑检查器脚本
        /// </summary>
        /// <param name="context"></param>
        [ComponentHeaderMenu(EditorSMSHelper.StateComponentHeaderMenuName, "编辑检查器脚本", index = 52)]
        [ComponentHeaderMenu(EditorSMSHelper.TransitionComponentHeaderMenuName, "编辑检查器脚本", index = 52)]
        [ComponentHeaderMenu(EditorSMSHelper.StateGroupComponentHeaderMenuName, "编辑检查器脚本", index = 52)]
        public static void EditInspectorScript(ComponentHeaderMenuContext context) => EditorHelper.OpenInspectorMonoScript(context.component);

        /// <summary>
        /// 选中检查器脚本
        /// </summary>
        /// <param name="context"></param>
        [ComponentHeaderMenu(EditorSMSHelper.StateComponentHeaderMenuName, "选中检查器脚本", index = 53)]
        [ComponentHeaderMenu(EditorSMSHelper.TransitionComponentHeaderMenuName, "选中检查器脚本", index = 53)]
        [ComponentHeaderMenu(EditorSMSHelper.StateGroupComponentHeaderMenuName, "选中检查器脚本", index = 53)]
        public static void SelectInspectorScript(ComponentHeaderMenuContext context)
        {
            var script = MonoScript.FromScriptableObject(BaseInspector.GetEditor(context.component));
            if (script) Selection.activeObject = script;
        }

        /// <summary>
        /// 复制组件
        /// </summary>
        /// <param name="context">组件头部菜单语境类对象</param>
        [ComponentHeaderMenu(EditorSMSHelper.StateComponentHeaderMenuName, "复制", index = 101)]
        [ComponentHeaderMenu(EditorSMSHelper.TransitionComponentHeaderMenuName, "复制", index = 101)]
        [ComponentHeaderMenu(EditorSMSHelper.StateGroupComponentHeaderMenuName, "复制", index = 101)]
        public static void CopyComponent(ComponentHeaderMenuContext context)
        {
            if (context.component is StateComponent stateComponent)
            {
                EditorCMHelper.CopyComponent(stateComponent);
            }
            else if (context.component is TransitionComponent transitionComponent)
            {
                EditorCMHelper.CopyComponent(transitionComponent);
            }
            else if (context.component is StateGroupComponent stateGroupComponent)
            {
                EditorCMHelper.CopyComponent(stateGroupComponent);
            }
        }

        /// <summary>
        /// 粘贴组件，克隆组件时携带数据信息
        /// </summary>
        /// <param name="context">组件头部菜单语境类对象</param>
        /// <returns>当前项可点击返回True；否则False</returns>
        [ComponentHeaderMenu(EditorSMSHelper.StateComponentHeaderMenuName, "粘贴(带数据)", index = 102)]
        [ComponentHeaderMenu(EditorSMSHelper.TransitionComponentHeaderMenuName, "粘贴(带数据)", index = 102)]
        [ComponentHeaderMenu(EditorSMSHelper.StateGroupComponentHeaderMenuName, "粘贴(带数据)", index = 102)]
        public static bool PasteComponentWithData(ComponentHeaderMenuContext context)
        {
            switch (context.funcType)
            {
                case EFuncType.Valid:
                    {
                        if (context.component is StateComponent stateComponent)
                        {
                            return EditorCMHelper.ComponentInClicpboard<StateComponent>();
                        }
                        else if (context.component is TransitionComponent transitionComponent)
                        {
                            return EditorCMHelper.ComponentInClicpboard<TransitionComponent>();
                        }
                        else if (context.component is StateGroupComponent stateGroupComponent)
                        {
                            return EditorCMHelper.ComponentInClicpboard<StateGroupComponent>();
                        }
                        break;
                    }
                case EFuncType.Click:
                    {
                        if (context.component is StateComponent stateComponent)
                        {
                            EditorCMHelper.PasteComponent<StateComponent>(stateComponent.parent, true);
                        }
                        else if (context.component is TransitionComponent transitionComponent)
                        {
                            EditorCMHelper.PasteComponent<TransitionComponent>(transitionComponent.parent, true);
                        }
                        else if (context.component is StateGroupComponent stateGroupComponent)
                        {
                            EditorCMHelper.PasteComponent<StateGroupComponent>(stateGroupComponent.parent, true);
                        }
                        break;
                    }
            }
            return true;
        }

        /// <summary>
        /// 粘贴组件，克隆组件时
        /// </summary>
        /// <param name="context">组件头部菜单语境类对象</param>
        /// <returns>当前项可点击返回True；否则False</returns>
        [ComponentHeaderMenu(EditorSMSHelper.StateComponentHeaderMenuName, "粘贴(新建)", index = 103)]
        [ComponentHeaderMenu(EditorSMSHelper.TransitionComponentHeaderMenuName, "粘贴(新建)", index = 103)]
        [ComponentHeaderMenu(EditorSMSHelper.StateGroupComponentHeaderMenuName, "粘贴(新建)", index = 103)]
        public static bool PasteComponentAsNew(ComponentHeaderMenuContext context)
        {
            switch (context.funcType)
            {
                case EFuncType.Valid:
                    {
                        if (context.component is StateComponent stateComponent)
                        {
                            return EditorCMHelper.ComponentInClicpboard<StateComponent>();
                        }
                        else if (context.component is TransitionComponent transitionComponent)
                        {
                            return EditorCMHelper.ComponentInClicpboard<TransitionComponent>();
                        }
                        else if (context.component is StateGroupComponent stateGroupComponent)
                        {
                            return EditorCMHelper.ComponentInClicpboard<StateGroupComponent>();
                        }
                        break;
                    }
                case EFuncType.Click:
                    {
                        if (context.component is StateComponent stateComponent)
                        {
                            EditorCMHelper.PasteComponent<StateComponent>(stateComponent.parent, false);
                        }
                        else if (context.component is TransitionComponent transitionComponent)
                        {
                            EditorCMHelper.PasteComponent<TransitionComponent>(transitionComponent.parent, false);
                        }
                        else if (context.component is StateGroupComponent stateGroupComponent)
                        {
                            EditorCMHelper.PasteComponent<StateGroupComponent>(stateGroupComponent.parent, false);
                        }
                        break;
                    }
            }
            return true;
        }

        /// <summary>
        /// 清空剪贴板信息
        /// </summary>
        /// <param name="context">组件头部菜单语境类对象</param>
        /// <returns>当前项可点击返回True；否则False</returns>
        [ComponentHeaderMenu(EditorSMSHelper.StateComponentHeaderMenuName, "清空剪贴板", index = 104)]
        [ComponentHeaderMenu(EditorSMSHelper.TransitionComponentHeaderMenuName, "清空剪贴板", index = 104)]
        [ComponentHeaderMenu(EditorSMSHelper.StateGroupComponentHeaderMenuName, "清空剪贴板", index = 104)]
        public static bool ClearClicpboard(ComponentHeaderMenuContext context)
        {
            switch (context.funcType)
            {
                case EFuncType.Valid:
                    {
                        if (context.component is StateComponent stateComponent)
                        {
                            return EditorCMHelper.ComponentInClicpboard<StateComponent>();
                        }
                        else if (context.component is TransitionComponent transitionComponent)
                        {
                            return EditorCMHelper.ComponentInClicpboard<TransitionComponent>();
                        }
                        else if (context.component is StateGroupComponent stateGroupComponent)
                        {
                            return EditorCMHelper.ComponentInClicpboard<StateGroupComponent>();
                        }
                        break;
                    }
                case EFuncType.Click:
                    {
                        if (context.component is StateComponent stateComponent)
                        {
                            EditorCMHelper.CopyComponent((StateComponent)null);
                        }
                        else if (context.component is TransitionComponent transitionComponent)
                        {
                            EditorCMHelper.CopyComponent((TransitionComponent)null);
                        }
                        else if (context.component is StateGroupComponent stateGroupComponent)
                        {
                            EditorCMHelper.CopyComponent((StateGroupComponent)null);
                        }
                        break;
                    }
            }
            return true;
        }
    }
}
