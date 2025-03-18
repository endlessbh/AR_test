using System;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.EditorCommonUtils;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginRepairman.States;
using XCSJ.EditorExtension.Base;
using System.Collections.Generic;

namespace XCSJ.EditorRepairman.States
{
    /// <summary>
    /// 模块检查器
    /// </summary>
    [Name("模块检查器")]
    [CustomEditor(typeof(Module), true)]
    public class ModuleInspector : PartInspector
    {
        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            if (_moduleSC)
            {
                moduleTool = _moduleSC.interactPart as PluginRepairman.Tools.Module;
                if (moduleTool) { }

                var list = new List<Part>();
                list.AddRange(_moduleSC.childrenParts);
                list.Remove(target as Module);
                parts = list.ToArray();
            }
        }

        private Part[] parts = new Part[0];

        private Module _moduleSC => target as Module;
        private PluginRepairman.Tools.Module moduleTool;

        private bool expanded = true;

        /// <summary>
        /// 当绘制检查器GUI
        /// </summary>
        [LanguageTuple("Add the selected game object as [part]", "添加选中游戏对象为[零件]")]
        [LanguageTuple("Add the selected game object as [module]", "添加选中游戏对象为[模块]")]
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            expanded = UICommonFun.Foldout(expanded, "零件列表");

            if (expanded)
            {
                CommonFun.BeginLayout();
                {
                    
                    TreeView.Draw(parts, TreeView.DefaultDrawExpandedFunc, TreeView.DefaultPrefixFunc, (node, content) =>
                    {
                        var part = node as Part;

                        if (GUILayout.Button(content, GUI.skin.label))
                        {
                            node.OnClick();

                            Selection.activeObject = part;
                        }
                    });
                }
                CommonFun.EndLayout();
            }

            if (GUILayout.Button(new GUIContent("自动创建子级对象为[零件]", EditorIconHelper.GetIconInLib(EIcon.Add)), UICommonOption.Height18))
            {
                if (moduleTool)
                {
                    CreateGameObjectToComponent<Part, XCSJ.PluginRepairman.Tools.Part>(CommonFun.GetChildGameObjects(moduleTool.transform), () => Part.CreatePart(_moduleSC.parent));
                }
            }

            EditorGUI.BeginDisabledGroup(!Selection.activeGameObject);
            if (GUILayout.Button(new GUIContent("添加选中游戏对象为[零件]", EditorIconHelper.GetIconInLib(EIcon.Add)), UICommonOption.Height18))
            {
                CreateGameObjectToComponent<Part, XCSJ.PluginRepairman.Tools.Part>(Selection.gameObjects, () => Part.CreatePart(_moduleSC.parent));
            }

            // 批量添加选中游戏对象为零件
            if (GUILayout.Button(new GUIContent("添加选中游戏对象为[模块]", EditorIconHelper.GetIconInLib(EIcon.Add)), UICommonOption.Height18))
            {
                CreateGameObjectToComponent<Module, XCSJ.PluginRepairman.Tools.Module>(Selection.gameObjects, () => Module.CreateModule(_moduleSC.parent));
            }
            EditorGUI.EndDisabledGroup();

            // 绘制模块组件的零件数据和约束关系
            if (moduleTool)
            {
                XCSJ.EditorRepairman.Tools.ModuleInspector.DrawCreatePartData(moduleTool); 
                XCSJ.EditorRepairman.Tools.ModuleInspector.DrawCreateAssemblyConstraints(moduleTool);
            }
        }

        private void CreateGameObjectToComponent<TStatePart, TInteractPart>(IEnumerable<GameObject> gameObjects, Func<State> createFun) 
            where TStatePart : Part
            where TInteractPart : XCSJ.PluginRepairman.Tools.Part
        {
            gameObjects.Foreach(go =>
            {
                if (go)
                {
                    // 绑定交互零件组件
                    go.XGetOrAddComponent<TInteractPart>();

                    var state = createFun.Invoke();
                    if (state)
                    {
                        var part = state.GetComponent<TStatePart>();
                        if (part)
                        {
                            part.go = go;
                            state.XSetName(go.name);
                        }
                    }
                }
            });
        }
    }
}
