using System;
using UnityEditor;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.EditorExtension.Base;
using XCSJ.PluginCommonUtils;

namespace XCSJ.EditorExtension
{
    /// <summary>
    /// XDreamer版本类
    /// </summary>
    public class XDreamerEdition
    {
        /// <summary>
        /// 编译宏前缀
        /// </summary>
        public const string CompileMacroPrefix = XDreamerEditor.CompileMacroPrefix + Macro.ConnectorString + nameof(EDITION);

        /// <summary>
        /// 版本宏对象
        /// </summary>
        public static readonly Macro EDITION = new Macro(CompileMacroPrefix, XDreamerEditor.DefaultBuildTargetGroups);

        /// <summary>
        /// 获取当前版本宏字符串
        /// </summary>
        /// <returns>前版本宏字符串</returns>
        public static string[] CurrentEditionMacro()
        {
            var edition = XDreamerExtensionOption.weakInstance.edition;
            switch (edition)
            {
                case EXDreamerEdition.XDreamerDeveloper:
                    {
                        return new string[] { GetEditionMacro(edition), GetEditionMacro(EXDreamerEdition.Developer) };
                    }
                default:
                    {
                        return new string[] { GetEditionMacro(edition) };
                    }
            }
        }

        private static string GetEditionMacro(EXDreamerEdition edition)
        {
            return Macro.CreateDefineName(CompileMacroPrefix, edition.ToString().ToUpper());
        }

        /// <summary>
        /// 更新版本宏
        /// </summary>
        public static void UpdateEditionMacro()
        {
            //编辑器运行时不处理编译宏
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;
            //Debug.Log("更新XDreamer版本宏");
            try
            {
                var macros = CurrentEditionMacro();
                Macro.RemoveMacro((group, name) => name.StartsWith(CompileMacroPrefix) && Array.IndexOf(macros, name) < 0, XDreamerEditor.DefaultBuildTargetGroups);

                foreach (var macroString in macros)
                {
                    var currentMacro = new Macro(macroString, XDreamerEditor.DefaultBuildTargetGroups);
                    currentMacro.DefineIfNoDefined();
                }
            }
            catch { }
        }

        /// <summary>
        /// 初始化宏
        /// </summary>
        [InitializeOnLoadMethod]
        [Macro]
        public static void InitMacro()
        {
            UpdateEditionMacro();
        }
    }

    /// <summary>
    /// XDreamer版本枚举
    /// </summary>
    [Name("XDreamer版本")]
    public enum EXDreamerEdition
    {
        /// <summary>
        /// 默认版本,默认部分功能可见;根据大多数用户习惯定义的显示界面
        /// </summary>
        [Name("默认")]
        [Tip("默认部分功能可见;根据大多数用户习惯定义的显示界面", "Some functions are visible by default; Display interface defined according to most user habits")]
        Default = 0,

        /// <summary>
        /// 开发者版本，全部功能可见;界面显示内容复杂多样;
        /// </summary>
        [Name("开发者")]
        [Tip("全部功能可见;界面显示内容复杂多样;", "All functions are visible; The content displayed on the interface is complex and diverse;")]
        Developer = ushort.MaxValue,

        /// <summary>
        /// XDreamer开发者版本，全部功能可见;界面显示内容复杂多样;
        /// </summary>
        [Name("XDreamer开发者")]
        [Tip("XDreamer开发者版本，普通用户谨慎使用！", "XDreamer developer version, ordinary users should use it with caution!")]
        XDreamerDeveloper = 0x7fffffff,
    }
}
