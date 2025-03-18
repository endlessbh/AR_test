using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using XCSJ.Caches;
using XCSJ.Collections;
using XCSJ.Helper;

namespace XCSJ.EditorExtension.Base
{
    /// <summary>
    /// 宏
    /// </summary>
    public class Macro
    {
        /// <summary>
        /// 分隔符
        /// </summary>
        public const char Separator = ';';

        /// <summary>
        /// 分隔符字符串
        /// </summary>
        public const string SeparatorString = ";";

        /// <summary>
        /// 连接符
        /// </summary>
        public const char Connector = '_';

        /// <summary>
        /// 连接符字符串
        /// </summary>
        public const string ConnectorString = "_";

        /// <summary>
        /// 定义名称
        /// </summary>
        public string defineName { get; private set; }

        /// <summary>
        /// 编译目标组
        /// </summary>
        public BuildTargetGroup[] buildTargetGroups { get; private set; }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="defineName"></param>
        /// <param name="targetGroups"></param>
        public Macro(string defineName, params BuildTargetGroup[] targetGroups)
        {
            if (string.IsNullOrEmpty(defineName)) throw new ArgumentNullException(nameof(defineName));
            if (targetGroups == null) throw new ArgumentNullException(nameof(targetGroups));

            this.defineName = defineName;
            this.buildTargetGroups = targetGroups;
        }

        /// <summary>
        /// 如果未定义则定义
        /// </summary>
        public void DefineIfNoDefined()
        {
            foreach (var targetGroup in buildTargetGroups)
            {
                DefineIfNoDefined(targetGroup);
            }
        }

        /// <summary>
        /// 如果未定义则定义
        /// </summary>
        /// <param name="targetGroup"></param>
        /// <param name="enable"></param>
        public void DefineIfNoDefined(BuildTargetGroup targetGroup, bool enable = true)
        {
            if (!Defined(targetGroup)) Define(enable);
        }

        /// <summary>
        /// 定义
        /// </summary>
        /// <param name="enable"></param>
        public void Define(bool enable)
        {
            Define(defineName, enable, buildTargetGroups);
        }

        /// <summary>
        /// 定义
        /// </summary>
        public void Define()
        {
            Define(true);
        }

        /// <summary>
        /// 取消定义
        /// </summary>
        public void Undefine()
        {
            Define(false);
        }

        /// <summary>
        /// 从选择的编译目标组中移除
        /// </summary>
        public void UndefineFromSelectedBuildTargetGroup()
        {
            Define(defineName, false, EditorUserBuildSettings.selectedBuildTargetGroup);
        }

        /// <summary>
        /// 取消定义，同时选择的编译目标组中移除
        /// </summary>
        public void UndefineWithSelectedBuildTargetGroup()
        {
            Undefine();
            UndefineFromSelectedBuildTargetGroup();
        }

        /// <summary>
        /// 取消定义全部
        /// </summary>
        public void UndefineAll()
        {
            Define(defineName, false, EnumCache<BuildTargetGroup>.Array);
        }

        /// <summary>
        /// 定义
        /// </summary>
        /// <param name="targetGroup"></param>
        /// <returns></returns>
        public bool Defined(BuildTargetGroup targetGroup = BuildTargetGroup.Standalone)
        {
            return Defined(targetGroup, defineName);
        }

        /// <summary>
        /// 定义到选择的
        /// </summary>
        /// <returns></returns>
        public bool DefindInSelected()
        {
            return Defined(EditorUserBuildSettings.selectedBuildTargetGroup);
        }

        /// <summary>
        /// 能否定义
        /// </summary>
        /// <param name="targetGroup"></param>
        /// <returns></returns>
        public bool CanDefind(BuildTargetGroup targetGroup = BuildTargetGroup.Standalone)
        {
            return buildTargetGroups.IndexOf(targetGroup) >= 0;
        }

        /// <summary>
        /// 在选择的能否定义
        /// </summary>
        /// <returns></returns>
        public bool CanDefindInSelected()
        {
            return CanDefind(EditorUserBuildSettings.selectedBuildTargetGroup);
        }

        /// <summary>
        /// 定义
        /// </summary>
        /// <param name="defineName"></param>
        /// <param name="enable"></param>
        /// <param name="targetGroups"></param>
        public static void Define(string defineName, bool enable, params BuildTargetGroup[] targetGroups)
        {
            foreach (var group in targetGroups)
            {
                Define(group, defineName, enable);
            }
        }

        /// <summary>
        /// 定义
        /// </summary>
        /// <param name="targetGroup"></param>
        /// <param name="defineName"></param>
        /// <param name="enable"></param>
        public static void Define(BuildTargetGroup targetGroup, string defineName, bool enable)
        {
            var defines = GetScriptingDefineSymbols(targetGroup);
            if (enable)
            {
                if (defines.Contains(defineName))
                {
                    return;
                }
                defines.Add(defineName);
            }
            else
            {
                if (!defines.Contains(defineName))
                {
                    return;
                }
                while (defines.Contains(defineName))
                {
                    defines.Remove(defineName);
                }
            }
            string definesString = string.Join(Separator.ToString(), defines.ToArray());
            SetScriptingDefineSymbolsForGroup(targetGroup, definesString);
        }

        /// <summary>
        /// 定义
        /// </summary>
        /// <param name="defineName"></param>
        /// <returns></returns>
        public static bool Defined(string defineName)
        {
            return Defined(EditorUserBuildSettings.selectedBuildTargetGroup, defineName);
        }

        /// <summary>
        /// 定义
        /// </summary>
        /// <param name="targetGroup"></param>
        /// <param name="defineName"></param>
        /// <returns></returns>
        public static bool Defined(BuildTargetGroup targetGroup, string defineName)
        {
            return GetScriptingDefineSymbols(targetGroup).Contains(defineName);
        }

        /// <summary>
        /// 获取脚本定义符号列表
        /// </summary>
        /// <param name="targetGroup"></param>
        /// <returns></returns>
        public static List<string> GetScriptingDefineSymbols(BuildTargetGroup targetGroup)
        {
            try
            {
                return PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup).Split(Separator).ToList();
            }
            catch { return new List<string>(); }
        }

        /// <summary>
        /// 设置脚本定义符号列表
        /// </summary>
        /// <param name="targetGroup"></param>
        /// <param name="defines"></param>
        public static void SetScriptingDefineSymbolsForGroup(BuildTargetGroup targetGroup, string defines)
        {
            try
            {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, defines);
            }
            catch { }
        }

        /// <summary>
        /// 获取宏列表
        /// </summary>
        /// <param name="targetGroup"></param>
        /// <returns></returns>
        public static List<Macro> GetMacros(BuildTargetGroup targetGroup)
        {
            return GetScriptingDefineSymbols(targetGroup).Where(defineName => !string.IsNullOrEmpty(defineName)).ToList(defineName => new Macro(defineName, targetGroup));
        }

        /// <summary>
        /// 查找宏
        /// </summary>
        /// <param name="defineName"></param>
        /// <returns></returns>
        public static Macro FindMacro(string defineName)
        {
            return new Macro(defineName, EnumCache<BuildTargetGroup>.Array.Where(targetGroup => Defined(targetGroup, defineName)).ToArray());
        }

        /// <summary>
        /// 类型转定义名称
        /// </summary>
        /// <param name="type"></param>
        /// <param name="toUpper"></param>
        /// <returns></returns>
        public static string TypeToDefineName(Type type, bool toUpper)
        {
            if (type == null) return "";
            return TypeStringToDefineName(type.FullName, toUpper);
        }

        /// <summary>
        /// 类型字符串转定义名称
        /// </summary>
        /// <param name="typeString"></param>
        /// <param name="toUpper"></param>
        /// <returns></returns>
        public static string TypeStringToDefineName(string typeString, bool toUpper)
        {
            if (string.IsNullOrEmpty(typeString)) return "";
            var name = typeString.Replace('.', '_').Replace('+', '_');
            return toUpper ? name.ToUpper() : name;
        }

        /// <summary>
        /// 创建定义名称
        /// </summary>
        /// <param name="defineNamePrefix"></param>
        /// <param name="type"></param>
        /// <param name="toUpper"></param>
        /// <returns></returns>
        public static string CreateDefineName(string defineNamePrefix, Type type, bool toUpper = true)
        {
            if (type == null) return defineNamePrefix;
            return CreateDefineName(defineNamePrefix, TypeToDefineName(type, toUpper));
        }

        /// <summary>
        /// 创建定义名称
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        public static string CreateDefineName(params string[] names)
        {
            return CreateDefineName(Connector, names);
        }

        /// <summary>
        /// 创建定义名称
        /// </summary>
        /// <param name="connector"></param>
        /// <param name="names"></param>
        /// <returns></returns>
        public static string CreateDefineName(char connector, params string[] names)
        {
            var sb = new StringBuilder();
            foreach (var name in names)
            {
                sb.AppendFormat("{0}{1}", name, connector);
            }
            return sb.ToString().TrimEnd(connector);
        }

        /// <summary>
        /// 移除宏
        /// </summary>
        /// <param name="needRemoveFunc"></param>
        /// <param name="targetGroups"></param>
        public static void RemoveMacro(Func<BuildTargetGroup, string, bool> needRemoveFunc, params BuildTargetGroup[] targetGroups)
        {
            if (needRemoveFunc == null) return;
            try
            {
                foreach (var group in targetGroups)
                {
                    var defines = GetScriptingDefineSymbols(group).Where(defineName => !needRemoveFunc(group, defineName));
                    string definesString = string.Join(Separator.ToString(), defines.ToArray());
                    SetScriptingDefineSymbolsForGroup(group, definesString);
                }

            }
            catch { }
        }
    }

    /// <summary>
    /// 宏特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class MacroAttribute : Attribute
    {
        /// <summary>
        /// 调用宏函数
        /// </summary>
        public static void InvokeMacroMethod()
        {
            foreach (var method in MethodHelper.GetStaticMethods<MacroAttribute>(false))
            {
                try
                {
                    method.Invoke(null, null);
                }
                catch { }
            }
        }
    }
}
