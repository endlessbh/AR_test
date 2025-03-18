using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginRepairman.Tools;

namespace XCSJ.EditorRepairman.Tools
{
    /// <summary>
    /// 模块检查器
    /// </summary>
    [Name("模块检查器")]
    [CustomEditor(typeof(Module), true)]
    public class ModuleInspector : PartInspector<Module>
    {
        /// <summary>
        /// 绘制成员
        /// </summary>
        /// <param name="serializedProperty"></param>
        /// <param name="propertyData"></param>
        protected override void OnDrawMember(SerializedProperty serializedProperty, PropertyData propertyData)
        {
            switch (serializedProperty.name)
            {
                case nameof(Module._partSockets):
                    {
                        DrawCreatePartData(targetObject);
                        break;
                    }
                case nameof(Module._partAssemblyConstraints):
                    {
                        DrawCreateAssemblyConstraints(targetObject);
                        break;
                    }
            }
            base.OnDrawMember(serializedProperty, propertyData);
        }

        /// <summary>
        /// 绘制创建零件数据按钮
        /// </summary>
        /// <param name="module"></param>
        public static void DrawCreatePartData(Module module)
        {
            if (GUILayout.Button(new GUIContent(CommonFun.Name(typeof(Module), nameof(Module._partSockets)), EditorIconHelper.GetIconInLib(EIcon.Add)), UICommonOption.Height18))
            {
                Module.ClearSockets(module);
                Module.CreateSockets(module);
            }
        }

        /// <summary>
        /// 绘制创建约束数据按钮
        /// </summary>
        /// <param name="module"></param>
        public static void DrawCreateAssemblyConstraints(Module module)
        {
            if (GUILayout.Button(new GUIContent(CommonFun.Name(typeof(Module), nameof(Module._partAssemblyConstraints)), EditorIconHelper.GetIconInLib(EIcon.Add)), UICommonOption.Height18))
            {
                ClearAssemblyConstraints(module);
                CreateAssemblyConstraints(module);
            }
        }

        /// <summary>
        /// 使用状态的零件连线来创建约束关系
        /// </summary>
        private static void CreateAssemblyConstraints(Module module)
        {
            var msc = module.moduleSC;
            if (msc)
            {
                module.XModifyProperty(() =>
                {
                    // 获取约束关系进行关联
                    foreach (var constraint in msc.GetAssemblyConstraints())
                    {
                        var fromPart = constraint.Item1.interactPart;
                        var toPart = constraint.Item2.interactPart;
                        if (fromPart && toPart)
                        {
                            module._partAssemblyConstraints.Add(new PartAssemblyConstraint(fromPart, toPart));
                        }
                    }
                });
            }

            // 子模块构建约束关系
            foreach (var m in GetChildrenModules(module))
            {
                CreateAssemblyConstraints(m);
            }
        }

        private static void ClearAssemblyConstraints(Module module)
        {
            module.XModifyProperty(() =>
            {
                module._partAssemblyConstraints.Clear();

                foreach (var m in GetChildrenModules(module))
                {
                    CreateAssemblyConstraints(m);
                }
            });
        }

        private static IEnumerable<Module> GetChildrenModules(Module module) => module.GetComponentsInChildren<Module>().Where(m => m != module && m.module == module);
    }
}
