using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Presets;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Caches;
using XCSJ.Collections;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Base.CategoryViews;
using XCSJ.EditorCommonUtils.CNScripts;
using XCSJ.EditorExtension.Base.Attributes;
using XCSJ.EditorExtension.Base.Kernel;
using XCSJ.EditorExtension.Base.XUnityEditor;
using XCSJ.EditorExtension.Base.XUnityEditor.PackageManager.UI;
using XCSJ.EditorExtension.CNScripts;
using XCSJ.Extension.Base;
using XCSJ.Extension.CNScripts;
using XCSJ.Helper;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.CNScripts;
using XCSJ.PluginCommonUtils.Menus;
using XCSJ.Scripts;
using static UnityEditor.SearchableEditorWindow;

namespace XCSJ.EditorExtension.Base
{
    /// <summary>
    /// 编辑器辅助类
    /// </summary>
    [LanguageFileOutput]
    public static class EditorHelper
    {
        /// <summary>
        /// UnityEditor前缀
        /// </summary>
        public const string UnityEditorPrefix = nameof(UnityEditor) + ".";

        /// <summary>
        /// UnityEditorInternal前缀
        /// </summary>
        public const string UnityEditorInternalPrefix = nameof(UnityEditorInternal) + ".";

        /// <summary>
        /// 初始化
        /// </summary>
        [InitializeOnLoadMethod]
        public static void Init()
        {
            //优先插件初始化
            PlguinsHelper.Init();
            EditorHandlerExtension.Init();
            EditorInputSystemHelper.Init();
            //EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemCallback;

            XDreamerEvents.onSceneAnyAssetsChanged += EditorObjectHelper.ComponentCollectionCache.Clear;

            EditorScriptHelper.Init();

#if XDREAMER_EDITION_XDREAMERDEVELOPER
            CategoryListExtension.onBeforeDrawItem += OnBeforeDrawItem_CategoryList;
            ScriptViewerWindow.onAfterCommandTitle += OnAfterCommandTitle;
#endif
        }

        /// <summary>
        /// 编辑
        /// </summary>
        [Name("编辑")]
        [Tip("编辑脚本命令所在的脚本", "The script in which the edit script command resides")]
        [XCSJ.Attributes.Icon(EIcon.Edit)]
        public static XGUIContent editGUIContent { get; } = new XGUIContent(typeof(EditorHelper), nameof(editGUIContent), true);

        private static void OnAfterCommandTitle(ScriptViewerWindow scriptViewerWindow, ScriptStringInfo scriptStringInfo, ScriptString scriptString, IVarCollectionHost varCollectionHost)
        {
            if (GUILayout.Button(editGUIContent, EditorStyles.miniButton, UICommonOption.Width32, UICommonOption.Height20))
            {
                OpenMonoScript(scriptStringInfo.script?.owner?.GetType());
            }
        }

        /// <summary>
        /// 打开首选项窗口
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static EditorWindow OpenPreferencesWindow(string name = "")
        {
            return DefaultEditorHandler.instance.OpenPreferencesWindow(name);
        }

        /// <summary>
        /// 打开项目设置窗口
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static EditorWindow OpenProjectSettingsWindow(string name = "")
        {
            return DefaultEditorHandler.instance.OpenProjectSettingsWindow(name);
        }

        /// <summary>
        /// 如果宏可以定义但未定义时回调
        /// </summary>
        /// <param name="macro"></param>
        /// <param name="callback"></param>
        /// <param name="confirmImportFunc"></param>
        public static void CallbackIfMacroCanDefindButUndefined(this Macro macro, Action callback, Func<bool> confirmImportFunc = null)
        {
            if (macro == null || callback == null) return;
            if (confirmImportFunc == null) confirmImportFunc = () => true;

            //可以定义但是又没有定义对应的Macro
            if (macro.CanDefindInSelected() && !macro.DefindInSelected() && confirmImportFunc())
            {
                callback();
            }
        }

        #region 导入UnityPackage

        /// <summary>
        /// 如果需要导入包
        /// </summary>
        /// <param name="macro"></param>
        /// <param name="packagePath"></param>
        /// <param name="confirmImportFunc"></param>
        public static void ImportPackageIfNeed(this Macro macro, string packagePath, Func<bool> confirmImportFunc = null)
        {
            if (string.IsNullOrEmpty(packagePath)) return;
            CallbackIfMacroCanDefindButUndefined(macro, () =>
            {
                var fullPath = GetValidFullPath(packagePath);
                //Debug.Log(GetValidFullPath(packagePath));
                if (FileHelper.Exists(fullPath))
                {
                    AssetDatabase.ImportPackage(fullPath, true);
                }
                else
                {
                    Log.WarningFormat("导入UnityPackage时未找到文件:{0}", fullPath);
                }
            }, confirmImportFunc);
        }

        /// <summary>
        /// 按钮点击后，如果需要导入包
        /// </summary>
        /// <param name="macro"></param>
        /// <param name="packagePath"></param>
        public static void ImportPackageIfNeedWithButton(Macro macro, string packagePath)
        {
            ImportPackageIfNeed(macro, packagePath, () => DrawImportPackageButton(packagePath));
        }

        /// <summary>
        /// 绘制导入包按钮
        /// </summary>
        /// <param name="packageName"></param>
        /// <returns></returns>
        [LanguageTuple("Library Missing", "库缺失")]
        [LanguageTuple("The library required for logical execution is missing", "逻辑执行需要的库缺失")]
        [LanguageTuple("Import", "导入")]
        [LanguageTuple("Import [{0}] assets", "导入[{0}]资产")]
        private static bool DrawImportPackageButton(string packageName)
        {
            var bk = GUI.backgroundColor;
            GUI.backgroundColor = XDreamerBaseOption.weakInstance.errorColor;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(CommonFun.TempContent("Library Missing".Tr(typeof(EditorHelper)), "The library required for logical execution is missing".Tr(typeof(EditorHelper))));
            var ret = GUILayout.Button(CommonFun.TempContent("Import".Tr(typeof(EditorHelper)), string.Format("Import [{0}] assets".Tr(typeof(EditorHelper)), packageName)));
            EditorGUILayout.EndHorizontal();
            GUI.backgroundColor = bk;
            return ret;
        }

        /// <summary>
        /// 按钮点击后，如果需要导入包
        /// </summary>
        /// <param name="macro"></param>
        /// <param name="packageName"></param>
        /// <param name="onWantImportPackage"></param>
        public static void ImportPackageIfNeedWithButton(Macro macro, string packageName, Action onWantImportPackage)
        {
            CallbackIfMacroCanDefindButUndefined(macro, onWantImportPackage, () => DrawImportPackageButton(packageName ?? ""));
        }

        /// <summary>
        /// 弹出对话框并确认后，如果需要导入包
        /// </summary>
        /// <param name="macro"></param>
        /// <param name="packagePath"></param>
        public static void ImportPackageIfNeedWithDialog(Macro macro, string packagePath)
        {
            ImportPackageIfNeed(macro, packagePath, () => EditorUtility.DisplayDialog("Library Missing".Tr(typeof(EditorHelper)), string.Format("Import [{0}] assets".Tr(typeof(EditorHelper)), packagePath), "Import".Tr(typeof(EditorHelper)), TrHelper.Cancel));
        }

        /// <summary>
        ///  弹出对话框并确认后，如果需要导入包
        /// </summary>
        /// <param name="macro"></param>
        /// <param name="packagePath"></param>
        /// <param name="onCreatedManagerOrSceneOpened"></param>
        public static void ImportPackageIfNeedWithDialog(Macro macro, string packagePath, bool onCreatedManagerOrSceneOpened)
        {
            if (onCreatedManagerOrSceneOpened)
            {
                if (!XDreamerExtensionOption.weakInstance.askImportPackageOnCreatedManager) return;
            }
            else
            {
                if (!XDreamerExtensionOption.weakInstance.askImportPackageOnSceneOpened) return;
            }
            ImportPackageIfNeedWithDialog(macro, packagePath);
        }

        /// <summary>
        /// 获取有效的全路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetValidFullPath(string path)
        {
            if (string.IsNullOrEmpty(path)) return "";
            if (path.IndexOf("/Assets/") == 0)
            {
                path = Application.dataPath + path.Substring("/Assets".Length);
            }
            else if (path.IndexOf("Assets/") == 0)
            {
                path = Application.dataPath + path.Substring("Assets".Length);
            }
            return path;
        }

        #endregion

        #region 打开包管理器

        /// <summary>
        /// 如果需要打开包管理器
        /// </summary>
        /// <param name="macro"></param>
        /// <param name="packageNameOrDisplayName"></param>
        /// <param name="confirmImportFunc"></param>
        public static void OpenPackageManagerIfNeed(Macro macro, string packageNameOrDisplayName, Func<bool> confirmImportFunc = null)
        {
            if (macro == null || string.IsNullOrEmpty(packageNameOrDisplayName)) return;
            if (confirmImportFunc == null) confirmImportFunc = () => true;

            //可以定义但是又没有定义对应的Macro,那么可能是包管理器中的某些包未导入
            if (macro.CanDefindInSelected() && !macro.DefindInSelected() && confirmImportFunc())
            {
                PackageManagerWindow.OpneWindow(packageNameOrDisplayName);
            }
        }

        /// <summary>
        /// 按钮点击后，如果需要打开包管理器
        /// </summary>
        /// <param name="macro"></param>
        /// <param name="packageNameOrDisplayName"></param>
        [LanguageTuple("Open", "打开")]
        [LanguageTuple("Open [{0}] Package manager", "打开[{0}]包管理器")]
        public static void OpenPackageManagerIfNeedWithButton(Macro macro, string packageNameOrDisplayName)
        {
            OpenPackageManagerIfNeed(macro, packageNameOrDisplayName, () =>
            {
                var bk = GUI.backgroundColor;
                GUI.backgroundColor = XDreamerBaseOption.weakInstance.errorColor;
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel(CommonFun.TempContent("Library Missing".Tr(typeof(EditorHelper)), "The library required for logical execution is missing".Tr(typeof(EditorHelper))));
                var ret = GUILayout.Button(CommonFun.TempContent("Open".Tr(typeof(EditorHelper)), string.Format("Open [{0}] Package manager", packageNameOrDisplayName)));
                EditorGUILayout.EndHorizontal();
                GUI.backgroundColor = bk;
                return ret;
            });
        }

        /// <summary>
        /// 弹出对话框并确认后，如果需要打开包管理器
        /// </summary>
        /// <param name="macro"></param>
        /// <param name="packageNameOrDisplayName"></param>
        public static void OpenPackageManagerIfNeedWithDialog(Macro macro, string packageNameOrDisplayName)
        {
            OpenPackageManagerIfNeed(macro, packageNameOrDisplayName, () => EditorUtility.DisplayDialog("Library Missing".Tr(typeof(EditorHelper)), string.Format("Open [{0}] Package manager", packageNameOrDisplayName), "Open".Tr(typeof(EditorHelper)), TrHelper.Cancel));
        }

        /// <summary>
        /// 弹出对话框并确认后，如果需要打开包管理器
        /// </summary>
        /// <param name="macro"></param>
        /// <param name="packageNameOrDisplayName"></param>
        /// <param name="onCreatedManagerOrSceneOpened"></param>
        public static void OpenPackageManagerIfNeedWithDialog(Macro macro, string packageNameOrDisplayName, bool onCreatedManagerOrSceneOpened)
        {
            if (onCreatedManagerOrSceneOpened)
            {
                if (!XDreamerExtensionOption.weakInstance.askImportPackageOnCreatedManager) return;
            }
            else
            {
                if (!XDreamerExtensionOption.weakInstance.askImportPackageOnSceneOpened) return;
            }
            OpenPackageManagerIfNeedWithDialog(macro, packageNameOrDisplayName);
        }

        #endregion

        /// <summary>
        /// 设置图标
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="icon"></param>
        /// <param name="overwrite"></param>
        public static void SetIcon(UnityEngine.Object obj, Texture2D icon, bool overwrite = true)
        {
            if (!obj || !icon) return;
            var setIcon = EditorGUIUtility_LinkType.GetIconForObject(obj);
            if (setIcon != null && !overwrite) return;

            EditorGUIUtility_LinkType.SetIconForObject(obj, icon);
            EditorUtility_LinkType.ForceReloadInspectors();
            MonoImporter_LinkType.CopyMonoScriptIconToImporters(obj as MonoScript);
        }

        #region 层级窗口

        /// <summary>
        /// 当层级窗口绘制的Gizmos图标被点击时回调
        /// </summary>
        public static event Action<Component, Rect, Texture2D> onHierarchyWindowItemGizmosIconClicked;

        /// <summary>
        /// 调用层级窗口绘制的Gizmos图标被点击事件
        /// </summary>
        /// <param name="component"></param>
        /// <param name="rect"></param>
        /// <param name="texture2D"></param>
        private static void CallHierarchyWindowItemGizmosIconClicked(Component component, Rect rect, Texture2D texture2D)
        {
            onHierarchyWindowItemGizmosIconClicked?.Invoke(component, rect, texture2D);
            EditorApplicationExtension.CallHierarchyWindowItemGizmosIconClicked(component, rect, texture2D);
        }

        /// <summary>
        /// 层级窗口项的事件处理
        /// </summary>
        private static void HierarchyWindowItemOnEvent()
        {
            Event e = Event.current;
            switch (e.type)
            {
                case EventType.DragPerform:
                    {
                        //Log.Debug("DragPerform");
                        bool use = false;
                        //如果拖动的是特殊的manager~不允许执行
                        foreach (var obj in DragAndDrop.objectReferences)
                        {
                            if (!(obj is GameObject go)) continue;
                            if (CommonFun.GameObjectIsRootOrContainIBaseManager(go))
                            {
                                //Log.Debug(obj.GetType().Name + " , " + obj.name);
                                use = true;
                            }
                        }
                        if (use) e.Use();
                        break;
                    }
                case EventType.DragUpdated:
                    {
                        //Log.Debug("DragUpdated");
                        bool use = false;
                        //如果拖动的是特殊的manager~不允许执行
                        foreach (var obj in DragAndDrop.objectReferences)
                        {
                            if (!(obj is GameObject go)) continue;
                            if (CommonFun.GameObjectIsRootOrContainIBaseManager(go))
                            {
                                //Log.Debug(obj.GetType().Name + " , " + obj.name);
                                use = true;
                            }
                        }
                        if (use) e.Use();
                        break;
                    }
                case EventType.DragExited:
                    {
                        //Log.Debug("DragExited");
                        //e.Use();
                        break;
                    }
            }
        }

        /// <summary>
        /// 设置层级窗口选中游戏对象展开和折叠
        /// </summary>
        /// <param name="expand"></param>
        public static void SetHierarchySelectionExpanded(bool expand)
        {
            Selection.gameObjects.Foreach(go => SetHierarchyExpanded(go, expand));
        }

        /// <summary>
        /// 设置层级窗口选中T类型游戏对象展开和折叠
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="expand">展开或折叠</param>
        public static void SetHierarchySelectionExpanded<T>(bool expand) where T : Component
        {
            Selection.gameObjects.Foreach(go => SetHierarchyExpanded(go, expand));
        }

        /// <summary>
        /// 设置层级窗口所有T类型游戏对象展开和折叠
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="expand">展开或折叠</param>
        /// <param name="includeInactive">包含不活跃对象</param>
        public static void SetHierarchyExpanded<T>(bool expand, bool includeInactive = true) where T : Component
        {
            GetAllGameObject<T>(includeInactive).Foreach(go => SetHierarchyExpanded(go, expand));
        }

        /// <summary>
        /// 获取所有机构游戏对象
        /// </summary>
        /// <param name="includeInactive">包含不活跃对象</param>
        /// <param name="sortChildFirst">子节点靠前，父对象在链表尾部</param>
        /// <returns>游戏对象集</returns>
        public static IEnumerable<GameObject> GetAllGameObject<T>(bool includeInactive = true, bool sortChildFirst = true) where T : Component
        {
            var all = CommonFun.GetComponentsInChildren(typeof(T), includeInactive).ToList().ConvertAll(c => c.gameObject);

            var list = all.Distinct().ToList();
            if (sortChildFirst) list.Sort((x, y) => x.transform.IsChildOf(y.transform) ? -1 : 0);
            return list;
        }

        /// <summary>
        /// 使用反射方法展开和折叠层级窗口中的游戏对象
        /// 注意：折叠时，必须先折叠子对象，父对象才能折叠。
        /// </summary>
        /// <param name="gameObject">游戏对象</param>
        /// <param name="expand">展开或折叠</param>
        public static void SetHierarchyExpanded(GameObject gameObject, bool expand)
        {
            //var type = typeof(EditorWindow).Assembly.GetType("UnityEditor.SceneHierarchyWindow");
            //var methodInfo = type.GetMethod("SetExpandedRecursive");

            //EditorApplication.ExecuteMenuItem("Window/General/Hierarchy");
            //var window = EditorWindow.focusedWindow;
            //methodInfo.Invoke(window, new object[] { gameObject.GetInstanceID(), expand });

            var window = SceneHierarchyWindow.s_LastInteractedHierarchy;
            if (window)
            {
                window.SetExpandedRecursive(gameObject.GetInstanceID(), expand);
                window.editorWindow.Repaint();
            }
        }

        #endregion

        /// <summary>
        /// 获取对象在磁盘上的全路径
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetFullPath(this UnityEngine.Object obj)
        {
            if (!obj) return null;
            var assetPath = AssetDatabase.GetAssetPath(obj);
            if (string.IsNullOrEmpty(assetPath)) return null;
            return UICommonFun.ToFullPath(assetPath);
        }

        /// <summary>
        /// 获取Mono脚本
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static MonoScript GetMonoScript(this Type type)
        {
            if (type == null) return default;
            while (type.DeclaringType != null)//处理嵌套类情况
            {
                type = type.DeclaringType;
            }
            return Resources.FindObjectsOfTypeAll<MonoScript>().FirstOrDefault(s => s.GetClass() == type);
        }

        /// <summary>
        /// 打开Mono脚本
        /// </summary>
        /// <param name="script"></param>
        /// <returns></returns>
        public static MonoScript OpenMonoScript(this MonoScript script)
        {
            if (script) AssetDatabase.OpenAsset(script);
            return script;
        }

        /// <summary>
        /// 打开Mono脚本
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static MonoScript OpenMonoScript(this Type type) => OpenMonoScript(GetMonoScript(type));

        /// <summary>
        /// 打开Mono脚本
        /// </summary>
        /// <param name="scriptableObject"></param>
        /// <returns></returns>
        public static MonoScript OpenMonoScript(this ScriptableObject scriptableObject) => OpenMonoScript(scriptableObject ? MonoScript.FromScriptableObject(scriptableObject) : default);

        /// <summary>
        /// 打开Mono脚本
        /// </summary>
        /// <param name="behaviour"></param>
        /// <returns></returns>
        public static MonoScript OpenMonoScript(this MonoBehaviour behaviour) => OpenMonoScript(behaviour ? MonoScript.FromMonoBehaviour(behaviour) : default);

        /// <summary>
        /// 打开Mono脚本
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        public static MonoScript OpenMonoScript(this MemberInfo memberInfo)
        {
            if (memberInfo is Type type)
            {
                return OpenMonoScript(type);
            }
            else if (memberInfo is MethodInfo methodInfo)
            {
                return OpenMonoScript(methodInfo.ReflectedType ?? methodInfo.DeclaringType);
            }
            return default;
        }

        /// <summary>
        /// 获取检查器Mono脚本
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static MonoScript GetInspectorMonoScript(this UnityEngine.Object obj) => obj ? MonoScript.FromScriptableObject(BaseInspector.GetEditor(obj)) : default;

        /// <summary>
        /// 获取检查器Mono脚本
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static MonoScript GetInspectorMonoScript(this Type type) => GetMonoScript(BaseInspector.GetEditorType(type));

        /// <summary>
        /// 打开检查器Mono脚本
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static MonoScript OpenInspectorMonoScript(this UnityEngine.Object obj) => OpenMonoScript(GetInspectorMonoScript(obj));

        /// <summary>
        /// 打开检查器Mono脚本
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static MonoScript OpenInspectorMonoScript(this Type type) => OpenMonoScript(GetInspectorMonoScript(type));

        private static void OnBeforeDrawItem_CategoryList(IItem item, Rect rect)
        {
            var btnRect = new Rect(rect.x, rect.y, rect.height, rect.height);
            if (GUI.Button(btnRect, ""))
            {
                OpenMonoScript(item.memberInfo);
            }
        }

        /// <summary>
        /// 绘制菜单
        /// </summary>
        /// <param name="selectText"></param>
        /// <param name="texts"></param>
        /// <returns></returns>
        public static void DrawMenu(string selectText, string[] texts, Action<string> onMenuItemClicked)
        {
            var tmpSelectText = selectText;
            var menu = new GenericMenu();
            foreach (var text in texts)
            {
                var tempText = text;//做换存量
                menu.AddItem(new GUIContent(tempText), tempText == selectText, () => onMenuItemClicked?.Invoke(tempText));
            }
            menu.ShowAsContext();
        }

        /// <summary>
        /// 绘制菜单
        /// </summary>
        /// <param name="selectText"></param>
        /// <param name="texts"></param>
        /// <param name="onMenuItemClicked"></param>
        public static void DrawMenu(string selectText, string[] texts, Action<int, string> onMenuItemClicked)
        {
            var tmpSelectText = selectText;
            var menu = new GenericMenu();
            int i = 0;
            foreach (var text in texts)
            {
                var tempText = text;//做换存量
                var tempIndex = i;
                menu.AddItem(new GUIContent(tempText), tempText == selectText, () => onMenuItemClicked?.Invoke(tempIndex, tempText));
                i++;
            }
            menu.ShowAsContext();
        }

        /// <summary>
        /// 显示所有变量字符串弹出式菜单
        /// </summary>
        /// <param name="varString"></param>
        /// <param name="onMenuItemClicked"></param>
        [LanguageTuple("Exception drawing variable string [{0}]: {1}", "绘制变量字符串[{0}]时异常：{1}")]
        private static void DrawAllVarStringPopupMenu(string varString, Action<string> onMenuItemClicked)
        {
            CommonFun.FocusControl();
            var scriptManager = ScriptManager.instance;
            if (!scriptManager) return;

            var selectText = varString;
            var extensionHierarchyKey = "";
            if (VarStringAnalysisResult.TryParse(varString, out var result))
            {
                if (result.extensionHierarchyKey != null)
                {
                    selectText = result.varScope.TrLabel().text + "/" + HierarchyVarHelper.RemoveExtensionHierarchyKey(result.varHierarchyString);
                    extensionHierarchyKey = result.extensionHierarchyKey.formatName;
                }
                else
                {
                    selectText = result.varScope.TrLabel().text + "/" + result.varHierarchyString;
                }
            }

            var varStrings = new List<string>();

            foreach (var varScope in EnumCache<EVarScope>.Array)
            {
                if (scriptManager.varContext.TryGetVarCollection(varScope, out var varCollection) && varCollection != null)
                {
                    var start = varCollection.varScope.TrLabel().text + "/";
                    foreach (var kv in varCollection.varDictionary)
                    {
                        if (kv is DictionaryEntry dictionaryEntry && dictionaryEntry.Value is IVariable variable)
                        {
                            variable?.hierarchyVar?.Foreach((parent, index, key, current) =>
                            {
                                try
                                {
                                    if (parent == null)
                                    {
                                        varStrings.Add(start + current.varHierarchyString.Replace("/", ""));
                                    }
                                    else
                                    {
                                        varStrings.Add(start + current.varHierarchyString);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Log.ExceptionFormat("Exception drawing variable string [{0}]: {1}".Tr(typeof(EditorHelper)), (current?.varString ?? ""), ex);
                                }
                            });
                        }
                    }
                }
            }

            DrawMenu(selectText, varStrings.ToArray(), newSelectText =>
            {
                onMenuItemClicked?.Invoke(HierarchyVarHelper.AddOrUpdateExtensionHierarchyKey(newSelectText.Substring(newSelectText.IndexOf("/") + 1), extensionHierarchyKey));
            });
        }

        /// <summary>
        /// 显示当前变量字符串对应变量作用域的弹出式菜单
        /// </summary>
        /// <param name="varString"></param>
        /// <param name="onMenuItemClicked"></param>
        [LanguageTuple("Exception drawing variable string [{0}]: {1}", "绘制变量字符串[{0}]时异常：{1}")]
        private static void DrawVarStringPopupMenu(string varString, Action<string> onMenuItemClicked)
        {
            CommonFun.FocusControl();
            var scriptManager = ScriptManager.instance;
            if (!scriptManager) return;

            var selectText = varString;
            var extensionHierarchyKey = "";
            var varScope = EVarScope.Global;
            if (VarStringAnalysisResult.TryParse(varString, out var result))
            {
                varScope = result.varScope;
                if (result.extensionHierarchyKey != null)
                {
                    selectText = HierarchyVarHelper.RemoveExtensionHierarchyKey(result.varHierarchyString);
                    extensionHierarchyKey = result.extensionHierarchyKey.formatName;
                }
                else
                {
                    selectText = result.varHierarchyString;
                }
            }

            var varStrings = new List<string>();

            if (scriptManager.varContext.TryGetVarCollection(varScope, out var varCollection) && varCollection != null)
            {
                foreach (var kv in varCollection.varDictionary)
                {
                    if (kv is DictionaryEntry dictionaryEntry && dictionaryEntry.Value is IVariable variable)
                    {
                        variable?.hierarchyVar?.Foreach((parent, index, key, current) =>
                        {
                            try
                            {
                                if (parent == null)
                                {
                                    varStrings.Add(current.varHierarchyString.Replace("/", ""));
                                }
                                else
                                {
                                    varStrings.Add(current.varHierarchyString);
                                }
                            }
                            catch (Exception ex)
                            {
                                Log.ExceptionFormat("Exception drawing variable string [{0}]: {1}".Tr(typeof(EditorHelper)), (current?.varString ?? ""), ex);
                            }
                        });

                    }
                }
            }

            DrawMenu(selectText, varStrings.ToArray(), newSelectText =>
            {
                onMenuItemClicked?.Invoke(HierarchyVarHelper.AddOrUpdateExtensionHierarchyKey(newSelectText, extensionHierarchyKey));
            });
        }

        private static void DrawHierarchyKeyExtensionPopupMenu(EHierarchyKeyMode hierarchyKeyMode, string key, string varString, Action<string> onMenuItemClicked)
        {
            CommonFun.FocusControl();
            if (!HierarchyKeyExtensionHelper.TryGetHierarchyKeyExtensionDataList(hierarchyKeyMode, out var dataList)) return;

            DrawMenu(HierarchyKeyExtensionHelper.GetDisplayKey(key), dataList.displayArray, newSelectText =>
            {
                onMenuItemClicked?.Invoke(HierarchyVarHelper.AddOrUpdateExtensionHierarchyKey(varString, dataList.displayDictioanry[newSelectText].Item1));
            });
        }

        private static void DrawBothHierarchyKeyExtensionPopupMenu(string key, string varString, Action<string> onMenuItemClicked)
        {
            DrawHierarchyKeyExtensionPopupMenu(EHierarchyKeyMode.Both, key, varString, onMenuItemClicked);
        }

        private static void DrawSetHierarchyKeyExtensionPopupMenu(string key, string varString, Action<string> onMenuItemClicked)
        {
            DrawHierarchyKeyExtensionPopupMenu(EHierarchyKeyMode.Set, key, varString, onMenuItemClicked);
        }

        private static void DrawGetHierarchyKeyExtensionPopupMenu(string key, string varString, Action<string> onMenuItemClicked)
        {
            DrawHierarchyKeyExtensionPopupMenu(EHierarchyKeyMode.Get, key, varString, onMenuItemClicked);
        }

        private static string GetDefaultVarString(EVarScope varScope) => ScriptOption.VarFlag + "_" + varScope.ToVarScopeString();

        private static bool NeedFirstVarString(string varString, VarStringAnalysisResult varStringAnalysisResult, EVarScope varScope)
        {
            if (string.IsNullOrEmpty(varString)) return true;
            if(varStringAnalysisResult != null && varStringAnalysisResult.varName == "_")
            {
                var scriptManager = ScriptManager.instance;
                if (scriptManager && !scriptManager.TryGetHierarchyVar(GetDefaultVarString(varScope), out _, out _)) return true;
            }
            return false;
        }

        private static string GetFirstVarString(EVarScope varScope)
        {
            var scriptManager = ScriptManager.instance;
            if (varScope == EVarScope.Invalid) varScope = EVarScope.Global;
            if (scriptManager && scriptManager.varContext.TryGetVarCollection(varScope, out var varCollection) && varCollection != null)
            {
                foreach (var kv in varCollection.varDictionary)
                {
                    if (kv is DictionaryEntry dictionaryEntry && dictionaryEntry.Value is IVariable variable)
                    {
                        return ScriptOption.VarFlag + variable.name + varScope.ToVarScopeString();
                    }
                }
            }
            return GetDefaultVarString(varScope);
        }

        /// <summary>
        /// 绘制变量字符串弹出式菜单
        /// </summary>
        /// <param name="varString"></param>
        /// <param name="onMenuItemClicked"></param>
        /// <param name="options"></param>
        public static string DrawVarStringPopup(string varString, Action<string> onMenuItemClicked, params GUILayoutOption[] options)
        {
            EditorGUI.indentLevel = 0;
            var varScope = VarStringAnalysisResult.TryParse(varString, out var result) ? result.varScope : EVarScope.Global;
            var varScopeNew = (EVarScope)UICommonFun.EnumPopup(GUIContent.none, varScope, EditorObjectHelper.MiniPopup, UICommonOption.Width80);
            if (varScopeNew != varScope)
            {
                if (NeedFirstVarString(varString, result, varScopeNew))
                {
                    varString = GetFirstVarString(varScopeNew);
                }
                else if (result != null)
                {
                    varString = result.GetFormatVarString(false, EVarHierarchyDelimiter.Dot, false, true, true) + varScopeNew.ToVarScopeString();
                }
            }

            if (GUILayout.Button(CommonFun.TempContent(varString, varString), EditorObjectHelper.MiniPopup, options))
            {
                DrawVarStringPopupMenu(varString, onMenuItemClicked);
            }
            return varString;
        }

        /// <summary>
        /// 绘制获取与设置层级键扩展弹出式菜单
        /// </summary>
        /// <param name="varString"></param>
        /// <param name="onMenuItemClicked"></param>
        /// <param name="options"></param>
        public static void DrawBothHierarchyKeyExtensionPopup(string varString, Action<string> onMenuItemClicked, params GUILayoutOption[] options)
        {
            VarStringAnalysisResult.TryParse(varString, out var result);
            var key = result?.extensionHierarchyKey?.key ?? "";

            if (!GUILayout.Button(CommonFun.TempContent(key, key), EditorObjectHelper.MiniPopup, options)) return;

            DrawBothHierarchyKeyExtensionPopupMenu(key, varString, onMenuItemClicked);
        }

        /// <summary>
        /// 绘制获取层级键扩展弹出式菜单
        /// </summary>
        /// <param name="varString"></param>
        /// <param name="onMenuItemClicked"></param>
        /// <param name="options"></param>
        public static void DrawGetHierarchyKeyExtensionPopup(string varString, Action<string> onMenuItemClicked, params GUILayoutOption[] options)
        {
            VarStringAnalysisResult.TryParse(varString, out var result);
            var key = result?.extensionHierarchyKey?.key ?? "";

            if (!GUILayout.Button(CommonFun.TempContent(key, key), EditorObjectHelper.MiniPopup, options)) return;

            DrawGetHierarchyKeyExtensionPopupMenu(key, varString, onMenuItemClicked);
        }

        /// <summary>
        /// 绘制设置层级键扩展弹出式菜单
        /// </summary>
        /// <param name="varString"></param>
        /// <param name="onMenuItemClicked"></param>
        /// <param name="options"></param>
        public static void DrawSetHierarchyKeyExtensionPopup(string varString, Action<string> onMenuItemClicked, params GUILayoutOption[] options)
        {
            VarStringAnalysisResult.TryParse(varString, out var result);
            var key = result?.extensionHierarchyKey?.key ?? "";

            if (!GUILayout.Button(CommonFun.TempContent(key, key), EditorObjectHelper.MiniPopup, options)) return;

            DrawSetHierarchyKeyExtensionPopupMenu(key, varString, onMenuItemClicked);
        }

        /// <summary>
        /// 绘制变量字符串弹出式菜单
        /// </summary>
        /// <param name="position"></param>
        /// <param name="varString"></param>
        /// <param name="onMenuItemClicked"></param>
        public static string DrawVarStringPopup(Rect position, string varString, Action<string> onMenuItemClicked)
        {
            var varScope = VarStringAnalysisResult.TryParse(varString, out var result) ? result.varScope : EVarScope.Global;
            var varScopeNew = (EVarScope)UICommonFun.EnumPopup(new Rect(position.x, position.y, 80, position.height), GUIContent.none, varScope, EditorObjectHelper.MiniPopup);
            if (varScopeNew != varScope)
            {
                if (NeedFirstVarString(varString, result, varScopeNew))
                {
                    varString = GetFirstVarString(varScopeNew);
                }
                else if (result != null)
                {
                    varString = result.GetFormatVarString(false, EVarHierarchyDelimiter.Dot, false, true, true) + varScopeNew.ToVarScopeString();
                }
            }

            if (GUI.Button(new Rect(position.x + 80, position.y, position.width - 80, position.height), CommonFun.TempContent(varString, varString), EditorObjectHelper.MiniPopup))
            {
                DrawVarStringPopupMenu(varString, onMenuItemClicked);
            }
            return varString;
        }

        /// <summary>
        /// 绘制获取与设置层级键扩展弹出式菜单
        /// </summary>
        /// <param name="position"></param>
        /// <param name="varString"></param>
        /// <param name="onMenuItemClicked"></param>
        public static void DrawBothHierarchyKeyExtensionPopup(Rect position, string varString, Action<string> onMenuItemClicked)
        {
            VarStringAnalysisResult.TryParse(varString, out var result);
            var key = result?.extensionHierarchyKey?.key ?? "";

            if (!GUI.Button(position, CommonFun.TempContent(key, key), EditorObjectHelper.MiniPopup)) return;

            DrawBothHierarchyKeyExtensionPopupMenu(key, varString, onMenuItemClicked);
        }

        /// <summary>
        /// 绘制获取层级键扩展弹出式菜单
        /// </summary>
        /// <param name="position"></param>
        /// <param name="varString"></param>
        /// <param name="onMenuItemClicked"></param>
        public static void DrawGetHierarchyKeyExtensionPopup(Rect position, string varString, Action<string> onMenuItemClicked)
        {
            VarStringAnalysisResult.TryParse(varString, out var result);
            var key = result?.extensionHierarchyKey?.key ?? "";

            if (!GUI.Button(position, CommonFun.TempContent(key, key), EditorObjectHelper.MiniPopup)) return;

            DrawGetHierarchyKeyExtensionPopupMenu(key, varString, onMenuItemClicked);
        }

        /// <summary>
        /// 绘制设置层级键扩展弹出式菜单
        /// </summary>
        /// <param name="position"></param>
        /// <param name="varString"></param>
        /// <param name="onMenuItemClicked"></param>
        public static void DrawSetHierarchyKeyExtensionPopup(Rect position, string varString, Action<string> onMenuItemClicked)
        {
            VarStringAnalysisResult.TryParse(varString, out var result);

            var key = result?.extensionHierarchyKey?.key ?? "";
            if (!GUI.Button(position, CommonFun.TempContent(key, key), EditorObjectHelper.MiniPopup)) return;

            DrawSetHierarchyKeyExtensionPopupMenu(key, varString, onMenuItemClicked);
        }

        /// <summary>
        /// 选择场景中所有类型组件
        /// </summary>
        /// <param name="mb"></param>
        public static void SelectTypeComponentsInScene(Type componentType)
        {
            Selection.objects = CommonFun.GetComponentsInChildren(componentType, true).Cast(c => c.gameObject).ToArray();
        }

        /// <summary>
        /// 搜索场景中所有类型组件
        /// </summary>
        /// <param name="componentType"></param>
        /// <param name="searchMode"></param>
        /// <param name="setAll"></param>
        /// <param name="delayed"></param>
        public static void SearchTypeComponentsInScene(Type componentType)
        {
            SceneHierarchyWindow_Extension.SetSearchFilter(String.Format("t:{0}", componentType.Name), SearchMode.All);
        }

        /// <summary>
        /// 执行菜单项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="menuItemPath"></param>
        /// <returns>返回执行菜单项后，新创建的指定类型的组件对象</returns>
        public static T ExecuteMenuItem<T>(string menuItemPath) where T : MonoBehaviour
        {
            var mbs = UnityEngine.Object.FindObjectsOfType<T>();
            EditorApplication.ExecuteMenuItem(menuItemPath);
            var newMBs = UnityEngine.Object.FindObjectsOfType<T>();
            return newMBs.FirstOrDefault(r => !mbs.Contains(r));
        }

        /// <summary>
        /// 查找指定类型的所有资产
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public static IEnumerable<T> FindAllAssetsOfType<T>() where T : UnityEngine.Object
        {
            return AssetDatabase.FindAssets("t:" + typeof(T).Name).Cast(p => AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(p)));
        }

        /// <summary>
        /// 查找所有可应用到目标对象的预设
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static IEnumerable<Preset> FindAllPresetsOfType(UnityEngine.Object target) => FindAllPresetsOfType(new PresetType(target));

        /// <summary>
        /// 查找所有的预设
        /// </summary>
        /// <param name="presetType"></param>
        /// <returns></returns>
        public static IEnumerable<Preset> FindAllPresetsOfType(PresetType presetType) => FindAllAssetsOfType<Preset>().Where(preset => preset.GetPresetType() == presetType);
    }
}
