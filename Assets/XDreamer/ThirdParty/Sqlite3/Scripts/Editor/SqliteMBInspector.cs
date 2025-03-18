using System;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorDataBase.Tools;
using XCSJ.EditorExtension;
using XCSJ.EditorExtension.Base;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSqlite;

namespace XCSJ.EditorSqlite
{
    /// <summary>
    /// SQLite数据库类的编辑器类
    /// </summary>
#if UNITY_STANDALONE || UNITY_IOS || UNITY_ANDROID

#elif UNITY_EDITOR
    [Obsolete("SQLite数据库不支持当前的运行时环境(但可在编辑器环境中使用)，请使用其他类型的数据库替代")]
#else
    [Obsolete("SQLite数据库不支持当前的运行时环境，请使用其他类型的数据库替代", true)]
#endif
    [CustomEditor(typeof(SqliteMB))]
    [Name("SQLite数据库检查器")]
    public class SqliteMBInspector : DBMBInspector<SqliteMB>
    {
        /// <summary>
        /// 宏
        /// </summary>
        public static readonly Macro XDREAMER_SQLITE = new Macro(nameof(XDREAMER_SQLITE), BuildTargetGroup.Standalone, BuildTargetGroup.iOS, BuildTargetGroup.Android);

        static bool CheckType(Type type)
        {
            if (type == null) return false;
            var typePath = type.Assembly.Location;
            typePath = PathHelper.Format(typePath);
            if (typePath.Contains(":"))
            {
                return typePath.StartsWith(Application.dataPath);
            }
            if (typePath.StartsWith("Asset/") || typePath.StartsWith("/Asset/"))
            {
                return true;
            }
            return typePath.StartsWith(Application.dataPath);
        }

        /// <summary>
        /// 初始化宏
        /// </summary>
        [InitializeOnLoadMethod]
        [Macro]
        public static void InitMacro()
        {
            //编辑器运行时不处理编译宏
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;

#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_IOS || UNITY_ANDROID
            if (TypeHelper.ExistsAndAssemblyFileExists("Mono.Data.Sqlite.SqliteConnection", out var type) && CheckType(type))
            {
                XDREAMER_SQLITE.DefineIfNoDefined();
            }
            else
#endif
            {
                XDREAMER_SQLITE.UndefineWithSelectedBuildTargetGroup();
            }

#if UNITY_2018_3_OR_NEWER
            //需要移除 System.Data.dll文件
            FileHelper.Delete(Application.dataPath + "/Plugins/System.Data.dll");
            FileHelper.Delete(Application.dataPath + "/Plugins/System.Data.dll.meta");
#endif
        }

        private const string UnityPackagePath = "Assets/XDreamer-ThirdPartyUnityPackage/XDreamer_SQLite.unitypackage";

        /// <summary>
        /// 当绘制成员
        /// </summary>
        /// <param name="serializedProperty"></param>
        protected override void OnDrawMember(SerializedProperty serializedProperty, PropertyData propertyData)
        {
            switch (serializedProperty.name)
            {
                case nameof(ResourceFileInfo.uniqueName):
                    {
                        EditorGUILayout.BeginHorizontal();
                        base.OnDrawMember(serializedProperty, propertyData);

                        if (GUILayout.Button(UICommonOption.Update, EditorStyles.miniButtonRight, UICommonOption.WH24x16))
                        {
                            serializedProperty.stringValue = GuidHelper.GetNewGuid();
                        }
                        EditorGUILayout.EndHorizontal();
                        return;
                    }
            }
            base.OnDrawMember(serializedProperty, propertyData);
        }

        /// <summary>
        /// 当绘制脚本
        /// </summary>
        /// <param name="serializedProperty"></param>
        protected override void OnDrawScript(SerializedProperty serializedProperty)
        {
            base.OnDrawScript(serializedProperty);

#if UNITY_STANDALONE || UNITY_IOS || UNITY_ANDROID
            #region 检测是否需要导入UnityPackage

            EditorHelper.ImportPackageIfNeedWithButton(XDREAMER_SQLITE, UnityPackagePath);

            #endregion
#else
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField(new GUIContent("不支持当前选择的运行时平台!", "不支持当前选择的运行时平台,但可在编辑器环境中使用!^_^"), UICommonOption.labelRedBG, GUILayout.ExpandWidth(true));

#endif
        }
    }
}
