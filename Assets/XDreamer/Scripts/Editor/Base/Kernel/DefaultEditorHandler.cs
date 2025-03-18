using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Scripts;
using XCSJ.Attributes;
using XCSJ.Caches;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Base.CategoryViews;
using XCSJ.EditorCommonUtils.Base.Kernel;
using XCSJ.EditorExtension.Base.ProjectSettings;
using XCSJ.EditorExtension.Base.Tools;
using XCSJ.EditorExtension.Base.XUnityEditor;
using XCSJ.EditorExtension.Base.XUnityEditorInternal;
using XCSJ.EditorExtension.XAssets;
using XCSJ.EditorTools;
using XCSJ.Extension.Base.Kernel;
using XCSJ.Extension.Base.XUnityEngine;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;
using XCSJ.Tools;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.Extension.Base.Attributes;

namespace XCSJ.EditorExtension.Base.Kernel
{
    /// <summary>
    /// 默认编辑器处理器
    /// </summary>
    public class DefaultEditorHandler : InstanceClass<DefaultEditorHandler>, IEditorHandler
    {
        public void Init()
        {
            Preferences.onOptionModified += OnOptionModify;
            UICommonFun.DelayCall(() =>
            {
                OnOptionModify(XDreamerIconOption.weakInstance);
            });

            DefaultPluginsHandler.instance.onEditInspectorScript += OnEditInspectorScript;
            DefaultPluginsHandler.instance.onSelectTypeComponentsInScene += SelectTypeComponentsInScene;
            DefaultPluginsHandler.instance.onSearchTypeComponentsInScene += SearchTypeComponentsInScene;

            DefaultPluginsHandler.instance.onNeedDelayCall += OnNeedDelayCall;
            DefaultPluginsHandler.instance.onGetPropertyNameInInspector += OnGetPropertyNameInInspector;

            XDreamerEvents.onProjectAnyAssetsOrOptionChangedInEditor += OnProjectAnyAssetsOrOptionChangedInEditor;
        }

        private void OnProjectAnyAssetsOrOptionChangedInEditor()
        {
            inputNames = null;
        }

        private void OnOptionModify(Option option)
        {
            if (option is XDreamerIconOption iconOption)
            {
                skinIconMarker = iconOption.GetSkinIconMarker();
            }
        }

        public void ClearConsole() => LogEntries.Clear();

        /// <summary>
        /// 打开Mono脚本
        /// </summary>
        /// <param name="target"></param>
        public void OpenMonoScript(object target)
        {
            if (target == null) return;

            if (target is MonoScript monoScript)
            {
                EditorHelper.OpenMonoScript(monoScript);
            }
            else if (target is ScriptableObject scriptableObject)
            {
                EditorHelper.OpenMonoScript(scriptableObject);
            }
            else if (target is MonoBehaviour behaviour)
            {
                EditorHelper.OpenMonoScript(behaviour);
            }
            else if (target is MemberInfo memberInfo)
            {
                EditorHelper.OpenMonoScript(memberInfo);
            }
        }

        /// <summary>
        /// 在预制件舞台上
        /// </summary>
        public bool InPrefabStage(GameObject gameObject)
        {
            if (!gameObject) return false;

#if UNITY_2021_3_OR_NEWER
            return UnityEditor.SceneManagement.PrefabStageUtility.GetPrefabStage(gameObject) != null;
#else
            return UnityEditor.Experimental.SceneManagement.PrefabStageUtility.GetPrefabStage(gameObject) != null;
#endif
        }

        public EditorWindow OpenInspectorWindow()
        {
            var window = EditorWindow.GetWindow(InspectorWindow.Type);
            if (window)
            {
                window.Show();
                window.Focus();
            }
            return window;
        }

        public bool lockInspectorWindow
        {
            get
            {
                return ActiveEditorTracker.sharedTracker.isLocked;
            }
            set
            {
                ActiveEditorTracker.sharedTracker.isLocked = value;
                //ActiveEditorTracker.sharedTracker.ForceRebuild();
            }
        }

        public void SetScriptExecutionOrder(MonoBehaviour behaviour, int order)
        {
            if (!behaviour) return;
            var monoScript = MonoScript.FromMonoBehaviour(behaviour);
            if (!monoScript) return;
            //限定顺序的范围，并且是在值变更的时候才重新设置值,如果直接设置会导致Unity重新编译，编译后又触发设置，出现了死循环的递归操作！！
            order = Mathf.Clamp(order, -32000, 32000);
            if (order != MonoImporter.GetExecutionOrder(monoScript))
            {
                MonoImporter.SetExecutionOrder(monoScript, order);
            }
        }

#region 打开首选项窗口PreferencesWindow

        private const string PreferencesNamePrefix = nameof(Preferences) + "/";

        public EditorWindow OpenPreferencesWindow(string name = "")
        {
            try
            {
                return _OpenPreferencesWindow(name);
            }
            finally
            {
#if !UNITY_2018_3_OR_NEWER
                //延时打开对应的配置项！
                //原因：第一次打开时，自定义的配置还未生成，对应的项不存在；延时后，对应项已经生成，则可以成功打开！
                if (!string.IsNullOrEmpty(name))
                {
                    EditorApplicationExtension.DelayCall(0.02f, name, obj => _OpenPreferencesWindow(obj as string));
                }
#endif
            }
        }

        private EditorWindow _OpenPreferencesWindow(string name)
        {
            EditorWindow window = null;
            try
            {
#if UNITY_2018_3_OR_NEWER
                if (!string.IsNullOrEmpty(name) && !name.StartsWith(PreferencesNamePrefix))
                {
                    name = PreferencesNamePrefix + name;
                }
                window = SettingsService.OpenUserPreferences(name);
#else
                PreferencesWindow_LinkType.ShowPreferencesWindow();

                window = EditorWindow.GetWindow(PreferencesWindow_LinkType.Type);
                var referencesWindow = new PreferencesWindow_LinkType(window);

                if (string.IsNullOrEmpty(name)) return window;
                if (name.StartsWith(PreferencesNamePrefix)) name = name.Substring(PreferencesNamePrefix.Length);
                if (string.IsNullOrEmpty(name)) return window;

                var sections = referencesWindow.m_Sections;
                for (int i = 0; i < sections.Count; i++)
                {
                    if (sections[i].content.text == name)
                    {
                        referencesWindow.m_SelectedSectionIndex = i;
                        break;
                    }
                }
#endif
            }
            catch { }
            finally
            {
                if (window)
                {
                    window.Show();
                    window.Focus();
                }
            }
            return window;
        }

#endregion

#region 打开项目设置窗口

        private const string ProjectSettingsNamePrefix = "Project/";

        /// <summary>
        /// 打开项目设置窗口
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public EditorWindow OpenProjectSettingsWindow(string name = "")
        {
            try
            {
                return _OpenProjectSettingsWindow(name);
            }
            finally
            {
#if !UNITY_2018_3_OR_NEWER
                //延时打开对应的配置项！
                //原因：第一次打开时，自定义的配置还未生成，对应的项不存在；延时后，对应项已经生成，则可以成功打开！
                if (!string.IsNullOrEmpty(name))
                {
                    EditorApplicationExtension.DelayCall(0.02f, name, obj => _OpenProjectSettingsWindow(obj as string));
                }
#endif
            }
        }


        private EditorWindow _OpenProjectSettingsWindow(string name)
        {
            EditorWindow window = null;
            try
            {
#if UNITY_2018_3_OR_NEWER
                if (!string.IsNullOrEmpty(name) && !name.StartsWith(ProjectSettingsNamePrefix))
                {
                    name = ProjectSettingsNamePrefix + name;
                }
                window = SettingsService.OpenProjectSettings(name);
#else
                //
#endif
            }
            catch { }
            finally
            {
                if (window)
                {
                    window.Show();
                    window.Focus();
                }
            }
            return window;
        }

#endregion

        public bool CompileMacroExists(string macro)
        {
            return Macro.Defined(macro);
        }

        public bool GUIIsExiting()
        {
            return GUIUtility_LinkType.guiIsExiting;
        }

#region GetPathOfSkinMarker

        private string _skinIconMarker = "";
        public string skinIconMarker
        {
            get => string.IsNullOrEmpty(_skinIconMarker) ? (_skinIconMarker = XDreamerIconOption.weakInstance.GetSkinIconMarker()) : _skinIconMarker;
            set => _skinIconMarker = value;
        }

        public string GetPathOfSkinMarker(string path)
        {
            if (string.IsNullOrEmpty(path)) return string.Empty;
            var i = path.LastIndexOf('.');
            if (i == -1) return path;

            var skinIconMarker = this.skinIconMarker;
            if (string.IsNullOrEmpty(skinIconMarker)) return path;

            var dotMarker = "." + skinIconMarker;
            var markerExt = dotMarker + path.Substring(i);

            if (path.EndsWith(markerExt))
            {
                return path;
            }
            else
            {
                return path.Insert(i, dotMarker);
            }
        }

#endregion

#region GetIconInLib

        public Texture2D GetIconInLib(string name, Vector2Int size, ESkinRule skinRule = ESkinRule.AutoSkin)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }
            return GetIconFromIconOption(XDreamerIconOption.IconPathCache.Get(GetIconKeyName(name, size)), skinRule);
        }

        public static string GetIconKeyName(string name, Vector2Int size)
        {
            if (size.x <= 0 || size.y <= 0 || size == EditorIconHelper.DefaultIconSize) return name;

            return string.Format("{0}__{1}x{2}", name, size.x, size.y);
        }

        public Texture2D GetIconFromIconOption(string icon, ESkinRule skinRule)
        {
            if (string.IsNullOrEmpty(icon)) return null;

            switch (skinRule)
            {
                case ESkinRule.AutoSkin:
                    {
                        if (_GetIconFromIconOption(GetPathOfSkinMarker(icon)) is Texture2D tex0 && tex0)
                        {
                            return tex0;
                        }
                        break;
                    }
                case ESkinRule.OnlySkin:
                    {
                        if (_GetIconFromIconOption(GetPathOfSkinMarker(icon)) is Texture2D tex0 && tex0)
                        {
                            return tex0;
                        }
                        return null;
                    }
            }

            return _GetIconFromIconOption(icon);
        }

        private static Texture2D _GetIconFromIconOption(string icon)
        {
            string iconPath = XDreamerIconOption.IconPathCache.Get(icon);
            if (string.IsNullOrEmpty(iconPath))
            {
                iconPath = XDreamerIconOption.ToAssetsPath(icon);
            }
            return UICommonFun.LoadFromAssets<Texture2D>(iconPath);
        }

#endregion

        public Gradient GetGradient(SerializedProperty serializedProperty)
        {
            return new SerializedProperty_LinkType(serializedProperty).gradientValue;
        }

        public void SetGradient(SerializedProperty serializedProperty, Gradient gradient)
        {
            new SerializedProperty_LinkType(serializedProperty).gradientValue = gradient;
        }

        private string[] inputNames = null;

        public string[] GetInputNames()
        {
            if (inputNames == null || inputNames.Length == 0)
            {
                inputNames = XInputManager.GetAxisNamesDistinct().ToArray();
            }
            return inputNames.ToArray();
        }

        public void DrawArrow(Color color, Vector3 direction, Vector3 center, float arrowSize, float outlineWidth)
        {
            HandlesHelper.DrawArrow(color, direction, center, arrowSize, outlineWidth);
        }

        public GameObject CreatePrefab(string path, GameObject go)
        {
#if UNITY_2018_3_OR_NEWER
            return PrefabUtility.SaveAsPrefabAsset(go, path);
#else
            return PrefabUtility.CreatePrefab(path, go);
#endif
        }

        public bool HasCustomPropetyDrawer(SerializedProperty serializedProperty)
        {
            return ScriptAttributeUtility.HasCustomPropetyDrawer(serializedProperty);
        }

        private void OnEditInspectorScript(UnityEngine.Object obj)
        {
            if (!obj) return;
            var script = MonoScript.FromScriptableObject(BaseInspector.GetEditor(obj));
            if (script) AssetDatabase.OpenAsset(script);
        }

        /// <summary>
        /// 选择场景中所有类型组件
        /// </summary>
        /// <param name="mb"></param>
        private void SelectTypeComponentsInScene(MB mb)
        {
            EditorHelper.SelectTypeComponentsInScene(mb.GetType());
        }

        /// <summary>
        /// 搜索场景中所有类型组件
        /// </summary>
        /// <param name="mb"></param>
        private void SearchTypeComponentsInScene(MB mb)
        {
            EditorHelper.SearchTypeComponentsInScene(mb.GetType());
        }

        private void OnNeedDelayCall(object param, Action<object> action, float delayTime)
        {
            UICommonFun.DelayCall(delayTime, param, action);
        }

        private string[] OnGetPropertyNameInInspector(UnityEngine.Object obj) => UICommonFun.GetPropertyNameInInspector(obj);

        public void DeleteArrayElement(SerializedProperty arraySerializedProperty, SerializedProperty elementSerializedProperty, int index)
        {
            if (UICommonFun.NaturalCompare(Application.unityVersion, "2020.3.20f1c1") >= 0)
            {
                //在此版本后已知问题已修复
            }
            else
            {
                if (elementSerializedProperty != null
                      && elementSerializedProperty.propertyType == SerializedPropertyType.ObjectReference
                      && elementSerializedProperty.objectReferenceValue)
                {
                    //Debug.Log("objectReferenceValue: " + serializedProperty.objectReferenceValue);
                    //这种情况比较特殊，第一次删除会将对象设置为null；为null之后再删除，才会将数组长度做真正的修改！！所以对于非空对象，需要连续2次的删除操作！！
                    arraySerializedProperty.DeleteArrayElementAtIndex(index);
                }
            }
            arraySerializedProperty.DeleteArrayElementAtIndex(index);
        }

        /// <summary>
        /// 绘制变量字符串弹出式菜单
        /// </summary>
        /// <param name="varString"></param>
        /// <param name="onMenuItemClicked"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public string DrawVarStringPopup(string varString, Action<string> onMenuItemClicked, params GUILayoutOption[] options) => EditorHelper.DrawVarStringPopup(varString, onMenuItemClicked, options);

        /// <summary>
        /// 绘制获取与设置层级键扩展弹出式菜单
        /// </summary>
        /// <param name="varString"></param>
        /// <param name="onMenuItemClicked"></param>
        /// <param name="options"></param>
        public void DrawBothHierarchyKeyExtensionPopup(string varString, Action<string> onMenuItemClicked, params GUILayoutOption[] options) => EditorHelper.DrawBothHierarchyKeyExtensionPopup(varString, onMenuItemClicked, options);

        /// <summary>
        /// 绘制获取层级键扩展弹出式菜单
        /// </summary>
        /// <param name="varString"></param>
        /// <param name="onMenuItemClicked"></param>
        /// <param name="options"></param>
        public void DrawGetHierarchyKeyExtensionPopup(string varString, Action<string> onMenuItemClicked, params GUILayoutOption[] options) => EditorHelper.DrawGetHierarchyKeyExtensionPopup(varString, onMenuItemClicked, options);

        /// <summary>
        /// 绘制设置层级键扩展弹出式菜单
        /// </summary>
        /// <param name="varString"></param>
        /// <param name="onMenuItemClicked"></param>
        /// <param name="options"></param>
        public void DrawSetHierarchyKeyExtensionPopup(string varString, Action<string> onMenuItemClicked, params GUILayoutOption[] options) => EditorHelper.DrawSetHierarchyKeyExtensionPopup(varString, onMenuItemClicked, options);

        /// <summary>
        /// 绘制变量字符串弹出式菜单
        /// </summary>
        /// <param name="position"></param>
        /// <param name="varString"></param>
        /// <param name="onMenuItemClicked"></param>
        /// <returns></returns>
        public string DrawVarStringPopup(Rect position, string varString, Action<string> onMenuItemClicked) => EditorHelper.DrawVarStringPopup(position, varString, onMenuItemClicked);

        /// <summary>
        /// 绘制获取与设置层级键扩展弹出式菜单
        /// </summary>
        /// <param name="position"></param>
        /// <param name="varString"></param>
        /// <param name="onMenuItemClicked"></param>
        public void DrawBothHierarchyKeyExtensionPopup(Rect position, string varString, Action<string> onMenuItemClicked) => EditorHelper.DrawBothHierarchyKeyExtensionPopup(position, varString, onMenuItemClicked);

        /// <summary>
        /// 绘制获取层级键扩展弹出式菜单
        /// </summary>
        /// <param name="position"></param>
        /// <param name="varString"></param>
        /// <param name="onMenuItemClicked"></param>
        public void DrawGetHierarchyKeyExtensionPopup(Rect position, string varString, Action<string> onMenuItemClicked) => EditorHelper.DrawGetHierarchyKeyExtensionPopup(position, varString, onMenuItemClicked);

        /// <summary>
        /// 绘制设置层级键扩展弹出式菜单
        /// </summary>
        /// <param name="position"></param>
        /// <param name="varString"></param>
        /// <param name="onMenuItemClicked"></param>
        public void DrawSetHierarchyKeyExtensionPopup(Rect position, string varString, Action<string> onMenuItemClicked) => EditorHelper.DrawSetHierarchyKeyExtensionPopup(position, varString, onMenuItemClicked);

        public CategoryList GetToolCategoryListByPurposes(params string[] purposes) => EditorToolsHelper.GetWithPurposes(purposes);

        public FieldInfo GetFieldInfoFromPropertyPath(Type host, string propertyPath, out Type type)
        {
            return ScriptAttributeUtility.GetFieldInfoFromPropertyPath(host, propertyPath, out type);
        }

        public FieldInfo GetFieldInfoFromProperty(SerializedProperty serializedProperty, out Type type)
        {
            return ScriptAttributeUtility.GetFieldInfoFromProperty(serializedProperty, out type);
        }

        public bool HasPropertyDrawer(SerializedProperty serializedProperty) => ScriptAttributeUtility.GetHandler(serializedProperty).hasPropertyDrawer;

        public Type GetDrawerTypeForType(Type type) => ScriptAttributeUtility.GetDrawerTypeForType(type);

        public SystemLanguage GetUnityEditorLanguage() => LocalizationDatabase.currentEditorLanguage;

        /// <summary>
        /// 是可排序数组
        /// </summary>
        /// <returns></returns>
        public bool IsReorderableArray(PropertyData propertyData)
        {
            if (!propertyData.isArray) return false;
#if UNITY_2020_1_OR_NEWER
            return !AttributeCache<NonReorderableAttribute>.Exist(propertyData.fieldInfo);
#else
            return true;
#endif
        }

        /// <summary>
        /// 在可排序列表中作为数组元素时能否直接绘制属性
        /// </summary>
        /// <param name="serializedProperty"></param>
        /// <param name="propertyData"></param>
        /// <returns></returns>
        public bool CanDirectDrawPropertyAsArrayElementInReorderableList(SerializedProperty serializedProperty, PropertyData propertyData)
        {
            switch (serializedProperty.propertyType)
            {
                case SerializedPropertyType.Vector4:
                case SerializedPropertyType.Quaternion:
                    {
                        return false;
                    }
            }
            return propertyData.directDrawProperty;
        }

        /// <summary>
        /// 绘制通用成员：针对数组（列表）时做特殊处理
        /// </summary>
        /// <param name="inspector"></param>
        /// <param name="serializedProperty"></param>
        /// <param name="propertyData"></param>
        /// <param name="drawHead"></param>
        /// <param name="drawChildren"></param>
        /// <returns></returns>
        public bool DrawGenericMember(BaseInspector inspector, SerializedProperty serializedProperty, PropertyData propertyData)
        {
            if (!propertyData.isReorderableArray) return false;

#if UNITY_2020_1_OR_NEWER

            //数组元素不能直接绘制对应属性，则直接尝试使用Unity默认绘制对应的数组序列化属性；对应数组元素的标签、内置特性等均导致失效；
            if (!propertyData.arrayElement_CanDirectDrawProperty) 
            {
                EditorGUILayout.PropertyField(serializedProperty, inspector.GetTrLabel(serializedProperty, propertyData, null), true);
                return true;
            }
#else
            //数组元素不能直接绘制对应属性，那么后续使用默认绘制
            if (!propertyData.arrayElement_CanDirectDrawProperty) return false;
#endif

            //绘制通用成员头部且不展开，则后续不再绘制
            if (!inspector.CallOnDrawGenericMemberHead(serializedProperty, propertyData)) return true;

            //绘制数组元素-尝试使用可排序列表进行绘制
            try
            {
                CommonFun.BeginLayout(propertyData.needIndent, propertyData.needBoundBox);

                if (propertyData.displayArraySizeNewLine)
                {
                    //绘制数组大小
                    var arraySizePropertyData = propertyData.arrarySizePropertyData;
                    inspector.DrawArraySizeIfAllow(arraySizePropertyData.serializedProperty, arraySizePropertyData);
                }

                var reorderableList = propertyData.reorderableList;
                reorderableList.serializedProperty = serializedProperty;

                reorderableList.elementHeightCallback = (int index) =>
                {
                    if (index >= reorderableList.serializedProperty.arraySize) return 0;
                    var element = reorderableList.serializedProperty.GetArrayElementAtIndex(index);
                    return EditorGUI.GetPropertyHeight(element, true) + EditorGUIUtility.standardVerticalSpacing;
                };
                reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    var element = reorderableList.serializedProperty.GetArrayElementAtIndex(index);
                    var elementPropertyData = propertyData.propertyCache.GetPropertyData(element);

                    EditorGUI.PropertyField(rect, element, inspector.GetTrLabel(element, elementPropertyData, default));
                };
                reorderableList.DoLayoutList();
            }
            finally
            {
                CommonFun.EndLayout();
            }

            return true;
        }

        /// <summary>
        /// 绘制通用成员头部
        /// </summary>
        /// <param name="inspector"></param>
        /// <param name="serializedProperty"></param>
        /// <param name="propertyData"></param>
        /// <param name="label"></param>
        /// <param name="after"></param>
        /// <returns></returns>
        public bool DrawGenericMemberHead(BaseInspector inspector, SerializedProperty serializedProperty, PropertyData propertyData, GUIContent label = null, Action after = null)
        {
#if UNITY_2020_1_OR_NEWER

            var isExpanded = serializedProperty.isExpanded;
            label = inspector.GetTrLabel(serializedProperty, propertyData, label);

            EditorGUILayout.BeginHorizontal(XGUIStyleLib.Get(EGUIStyle.Box));
            var isExpandedNew = GUILayout.Toggle(isExpanded, GUIContent.none, EditorStyles.foldout, UICommonOption.Width8);
            EditorGUILayout.LabelField("");
            var labelRect = GUILayoutUtility.GetLastRect();
            after?.Invoke();
            EditorGUILayout.EndHorizontal();

            label = EditorGUI.BeginProperty(labelRect, label, serializedProperty);
            EditorGUI.PrefixLabel(labelRect, label);

            //绘制数组大小
            if (propertyData.isArray && propertyData.displayArraySizeAfterHead)
            {
                var arraySizePropertyData = propertyData.arrarySizePropertyData;
                var arraySize = serializedProperty.arraySize;

                var x = labelRect.xMax;
                if (propertyData.canDelete)//允许值变小
                {
                    x -= 24;
                    if (GUI.Button(new Rect(x, labelRect.y, 24, 16), UICommonOption.Delete, EditorStyles.miniButtonRight))
                    {
                        serializedProperty.arraySize = Math.Max(0, arraySize - 1);
                        CommonFun.FocusControl();
                    }
                }

                if (propertyData.canInsert)//允许值变大
                {
                    x -= 24;
                    if (GUI.Button(new Rect(x, labelRect.y, 24, 16), UICommonOption.Insert, EditorStyles.miniButtonMid))
                    {
                        serializedProperty.arraySize = arraySize + 1;
                        CommonFun.FocusControl();
                    }
                }

                var arraySizeRect = new Rect(x - 48, labelRect.y, 48, EditorGUIUtility.singleLineHeight);
                EditorGUI.PropertyField(arraySizeRect,arraySizePropertyData.serializedProperty, GUIContent.none);
                EditorGUI.LabelField(arraySizeRect, arraySizePropertyData.TrLabel(ENameTip.OnlyTip));

                var newArraySize = serializedProperty.arraySize;
                if (arraySize != newArraySize)
                {
                    if (!propertyData.canDelete && newArraySize < arraySize)//不允许值变小，却变小了，那么还原
                    {
                        serializedProperty.arraySize = arraySize;
                    }
                    else if (!propertyData.canInsert && newArraySize > arraySize)//不允许值变大，却变大了，那么还原
                    {
                        serializedProperty.arraySize = arraySize;
                    }
                }
            }

            EditorGUI.EndProperty();

            if (isExpandedNew == isExpanded)
            {
                var e = Event.current;
                if (e.type == EventType.MouseUp && e.button == 0 && labelRect.Contains(e.mousePosition))
                {
                    e.Use();
                    isExpandedNew = !isExpanded;
                }
            }

            if (isExpandedNew != isExpanded)
            {
                if (Event.current.alt)
                {
                    serializedProperty.SetExpandedRecurse(isExpandedNew);
                }
                else
                {
                    serializedProperty.isExpanded = isExpandedNew;
                }
            }
            return isExpandedNew;
#else

            EditorGUILayout.BeginHorizontal(XGUIStyleLib.Get(EGUIStyle.Box));
            EditorGUILayout.LabelField("", UICommonOption.Width10);
            EditorGUILayout.PropertyField(serializedProperty, inspector.GetTrLabel(serializedProperty, propertyData, label), false);

            //绘制数组大小
            if (propertyData.isArray && propertyData.displayArraySizeAfterHead)
            {
                var arraySizePropertyData = propertyData.arrarySizePropertyData;

                var arraySize = serializedProperty.arraySize;
                EditorGUILayout.PropertyField(arraySizePropertyData.serializedProperty, GUIContent.none, UICommonOption.Width48);
                EditorGUI.LabelField(GUILayoutUtility.GetLastRect(), arraySizePropertyData.TrLabel(ENameTip.OnlyTip));
                var newArraySize = serializedProperty.arraySize;

                if (propertyData.canInsert)//允许值变大
                {
                    if (GUILayout.Button(UICommonOption.Insert, EditorStyles.miniButtonMid, UICommonOption.WH24x16))
                    {
                        newArraySize = arraySize + 1;
                        CommonFun.FocusControl();
                    }
                }
                else if (newArraySize > arraySize)//不允许值变大，却变大了，那么还原
                {
                    newArraySize = arraySize;
                }
                if (propertyData.canDelete)//允许值变小
                {
                    if (GUILayout.Button(UICommonOption.Delete, EditorStyles.miniButtonRight, UICommonOption.WH24x16))
                    {
                        newArraySize = Math.Max(0, arraySize - 1);
                        CommonFun.FocusControl();
                    }
                }
                else if (newArraySize < arraySize)//不允许值变小，却变小了，那么还原
                {
                    newArraySize = arraySize;
                }

                if (arraySize != newArraySize)
                {
                    if (propertyData.canResize)
                    {
                        serializedProperty.arraySize = newArraySize;
                    }
                    else
                    {
                        serializedProperty.arraySize = arraySize;
                    }
                }
            }

            after?.Invoke();
            EditorGUILayout.EndHorizontal();
            return serializedProperty.isExpanded;
#endif
        }

        /// <summary>
        /// 绘制数组处理
        /// </summary>
        /// <param name="inspector"></param>
        /// <param name="serializedProperty"></param>
        /// <param name="propertyData"></param>
        public void DrawArrayHandle(BaseInspector inspector, SerializedProperty serializedProperty, PropertyData propertyData)
        {
            serializedProperty.DrawArrayHandleRule();
        }

        /// <summary>
        /// 根据SerializedProperty的值类型获取对应的值，并转化为基础对象类型；在获取过程中会发生装箱操作；
        /// </summary>
        /// <param name="serializedProperty">序列化属性</param>
        /// <returns>返回序列化属性的值；</returns>
        public object GetSerializedPropertyValue(SerializedProperty serializedProperty)
        {
            switch (serializedProperty.propertyType)
            {
                case SerializedPropertyType.Generic:
                    {
                        var datas = PropertyData.GetPropertyData(serializedProperty).hierarchyPropertyDatas;
                        object obj = serializedProperty.serializedObject.targetObject;
                        foreach (var data in datas)
                        {
                            if (data.isArrayElement)
                            {
                                if (obj is IList list) obj = list[data.arrayElementIndex];
                                else if (obj is Array array) obj = array.GetValue(data.arrayElementIndex);
                                else
                                {
                                    throw new InvalidCastException("未知数组（列表）类型:" + obj?.GetType());
                                }
                            }
                            else
                            {
                                obj = data.fieldInfo.GetValue(obj);
                            }
                        }
                        return obj;
                    }
                case SerializedPropertyType.Integer: return serializedProperty.longValue;
                case SerializedPropertyType.Boolean: return serializedProperty.boolValue;
                case SerializedPropertyType.Float: return serializedProperty.doubleValue;
                case SerializedPropertyType.String: return serializedProperty.stringValue;
                case SerializedPropertyType.Color: return serializedProperty.colorValue;
                case SerializedPropertyType.ObjectReference: return serializedProperty.objectReferenceValue;
                case SerializedPropertyType.LayerMask: return serializedProperty.intValue;
                case SerializedPropertyType.Enum: return serializedProperty.intValue;
                case SerializedPropertyType.Vector2: return serializedProperty.vector2Value;
                case SerializedPropertyType.Vector3: return serializedProperty.vector3Value;
                case SerializedPropertyType.Vector4: return serializedProperty.vector4Value;
                case SerializedPropertyType.Rect: return serializedProperty.rectValue;
                case SerializedPropertyType.ArraySize: return serializedProperty.intValue;
                case SerializedPropertyType.Character: return serializedProperty.intValue;
                case SerializedPropertyType.AnimationCurve: return serializedProperty.animationCurveValue;
                case SerializedPropertyType.Bounds: return serializedProperty.boundsValue;
                case SerializedPropertyType.Gradient: return EditorHandler.GetGradient(serializedProperty);
                case SerializedPropertyType.Quaternion: return serializedProperty.quaternionValue;
                //case SerializedPropertyType.ExposedReference:
                //case SerializedPropertyType.FixedBufferSize:
                case SerializedPropertyType.Vector2Int: return serializedProperty.vector2IntValue;
                case SerializedPropertyType.Vector3Int: return serializedProperty.vector3IntValue;
                case SerializedPropertyType.RectInt: return serializedProperty.rectIntValue;
                case SerializedPropertyType.BoundsInt: return serializedProperty.boundsIntValue;
                case SerializedPropertyType.ManagedReference: return serializedProperty.managedReferenceFullTypename;
                default:
                    {
                        throw new InvalidOperationException("无法有效处理的序列化属性类型: " + serializedProperty.propertyType.ToString());
                    }
            }
        }

        /// <summary>
        /// 设置序列化属性值
        /// </summary>
        /// <param name="serializedProperty">待处理的序列化属性对象</param>
        /// <param name="value">新的属性值，使用时会通过<see cref="Converter"/>尝试将本参数转化为属性类型对应类型的对象</param>
        public void SetSerializedPropertyValue(SerializedProperty serializedProperty, object value)
        {
            switch (serializedProperty.propertyType)
            {
                //case SerializedPropertyType.Generic: return property.objectReferenceInstanceIDValue
                case SerializedPropertyType.Integer: serializedProperty.longValue = Converter.instance.ConvertTo<long>(value); break;
                case SerializedPropertyType.Boolean: serializedProperty.boolValue = Converter.instance.ConvertTo<bool>(value); break;
                case SerializedPropertyType.Float: serializedProperty.doubleValue = Converter.instance.ConvertTo<double>(value); break;
                case SerializedPropertyType.String: serializedProperty.stringValue = Converter.instance.ConvertTo<string>(value); break;
                case SerializedPropertyType.Color: serializedProperty.colorValue = Converter.instance.ConvertTo<Color>(value); break;
                case SerializedPropertyType.ObjectReference: serializedProperty.objectReferenceValue = Converter.instance.ConvertTo<UnityEngine.Object>(value); break;
                case SerializedPropertyType.LayerMask: serializedProperty.intValue = Converter.instance.ConvertTo<int>(value); break;
                case SerializedPropertyType.Enum: serializedProperty.intValue = Converter.instance.ConvertTo<int>(value); break;
                case SerializedPropertyType.Vector2: serializedProperty.vector2Value = Converter.instance.ConvertTo<Vector2>(value); break;
                case SerializedPropertyType.Vector3: serializedProperty.vector3Value = Converter.instance.ConvertTo<Vector3>(value); break;
                case SerializedPropertyType.Vector4: serializedProperty.vector4Value = Converter.instance.ConvertTo<Vector4>(value); break;
                case SerializedPropertyType.Rect: serializedProperty.rectValue = Converter.instance.ConvertTo<Rect>(value); break;
                case SerializedPropertyType.ArraySize: serializedProperty.intValue = Converter.instance.ConvertTo<int>(value); break;
                case SerializedPropertyType.Character: serializedProperty.intValue = Converter.instance.ConvertTo<int>(value); break;
                case SerializedPropertyType.AnimationCurve: serializedProperty.animationCurveValue = Converter.instance.ConvertTo<AnimationCurve>(value); break;
                case SerializedPropertyType.Bounds: serializedProperty.boundsValue = Converter.instance.ConvertTo<Bounds>(value); break;
                case SerializedPropertyType.Gradient: EditorHandler.SetGradient(serializedProperty, Converter.instance.ConvertTo<Gradient>(value)); break;
                case SerializedPropertyType.Quaternion: serializedProperty.quaternionValue = Converter.instance.ConvertTo<Quaternion>(value); break;
                //case SerializedPropertyType.ExposedReference: 
                //case SerializedPropertyType.FixedBufferSize: 
                case SerializedPropertyType.Vector2Int: serializedProperty.vector2IntValue = Converter.instance.ConvertTo<Vector2Int>(value); break;
                case SerializedPropertyType.Vector3Int: serializedProperty.vector3IntValue = Converter.instance.ConvertTo<Vector3Int>(value); break;
                case SerializedPropertyType.RectInt: serializedProperty.rectIntValue = Converter.instance.ConvertTo<RectInt>(value); break;
                case SerializedPropertyType.BoundsInt: serializedProperty.boundsIntValue = Converter.instance.ConvertTo<BoundsInt>(value); break;
                //case SerializedPropertyType.ManagedReference: 
                default:
                    {
                        throw new InvalidOperationException("无法有效处理的序列化属性类型: " + serializedProperty.propertyType.ToString());
                    }
            }
        }
        class Dictionary { };

        class TSO : SO, IDropdownPopupAttribute, IPropertyPathList
        {
            /// <summary>
            /// 属性路径列表
            /// </summary>
            [Name("属性路径列表")]
            public PropertyPathList _propertyPathList = new PropertyPathList();

            public string varName { get; set; }

            public EVarScope varScope { get; set; }

#region IDropdownPopupAttribute

            public bool TryGetOptions(string purpose, string propertyPath, out string[] options)
            {
                if (varScope == EVarScope.Reference) return ((IDropdownPopupAttribute)_propertyPathList).TryGetOptions(purpose, propertyPath, out options);

                var scriptManager = ScriptManager.instance;
                if (scriptManager
                    && PropertyPathHelper.TryGetLastArrayElementIndex(propertyPath, out var index)
                    && scriptManager.TryGetHierarchyVar(ScriptOption.VarFlag + varName + _propertyPathList.ToPropertyPath(index - 1), out var hv, out _))
                {
                    var varNames = new List<string>();
                    hv.Foreach((parent, i, key, current) =>
                    {
                        if (current.GetDepth() != index + 1) return;
                        varNames.Add(current.name);
                    });
                    options = varNames.ToArray();
                    return true;
                }
                options = default;
                return false;
            }

            public bool TryGetOption(string purpose, string propertyPath, string[] options, object propertyValue, out string option)
            {
                return ((IDropdownPopupAttribute)_propertyPathList).TryGetOption(purpose, propertyPath, options, propertyValue, out option);
            }

            public bool TryGetPropertyValue(string purpose, string propertyPath, string[] options, string option, out object propertyValue)
            {
                return ((IDropdownPopupAttribute)_propertyPathList).TryGetPropertyValue(purpose, propertyPath, options, option, out propertyValue);
            }

#endregion

#region IPropertyPathList

            public bool TryGetPropertyPathValue(out object value)
            {
                return ((IPropertyPathList)_propertyPathList).TryGetPropertyPathValue(out value);
            }

            public bool TryGetPropertyPathValue(int index, out object value)
            {
                if (varScope == EVarScope.Reference) return ((IPropertyPathList)_propertyPathList).TryGetPropertyPathValue(index, out value);

                var scriptManager = ScriptManager.instance;
                if (scriptManager
                    && scriptManager.TryGetHierarchyVar(ScriptOption.VarFlag + varName + _propertyPathList.ToPropertyPath(index), out var hv, out _))
                {
                    value = hv.originalValue;
                    return true;
                }

                value = default;
                return false;
            }

            public bool TryGetPropertyPathValueType(out Type type)
            {
                return ((IPropertyPathList)_propertyPathList).TryGetPropertyPathValueType(out type);
            }

            private Type GetType(object o)
            {
                if (o is IDictionary)
                {
                    return typeof(Dictionary);
                }
                if (o is IList)
                {
                    return typeof(Array);
                }
                return typeof(string);
            }

            public bool TryGetPropertyPathValueType(int index, out Type type)
            {
                if (varScope == EVarScope.Reference) return ((IPropertyPathList)_propertyPathList).TryGetPropertyPathValueType(index, out type);
                if (index < 0)
                {
                    type = GetType(_propertyPathList.propertyInstance);
                    return true;
                }

                var scriptManager = ScriptManager.instance;
                if (scriptManager
                    && scriptManager.TryGetHierarchyVar(ScriptOption.VarFlag + varName + _propertyPathList.ToPropertyPath(index), out var hv, out _))
                {
                    type = GetType(hv.originalValue);
                    return true;
                }

                type = default;
                return false;
            }

            public bool TryGetPropertyValue(out object value)
            {
                return ((IPropertyPathList)_propertyPathList).TryGetPropertyValue(out value);
            }

            public bool TryGetPropertyValueType(out Type type)
            {
                return ((IPropertyPathList)_propertyPathList).TryGetPropertyValueType(out type);
            }

#endregion
        }

        static TSO tso;

        /// <summary>
        /// 绘制变量字符串窗口内容
        /// </summary>
        /// <param name="varStringEditorWindow"></param>
        public void DrawVarStringWindowContent(VarStringEditorWindow varStringEditorWindow)
        {
            var varString = VarStringEditorWindow.varString;
            var scriptManager = ScriptManager.instance;
            if (!VarStringAnalysisResult.TryParse(varString, out var result) || !scriptManager) return;

            var extensionHierarchyKey = "";
            if (result.extensionHierarchyKey != null)
            {
                extensionHierarchyKey = result.extensionHierarchyKey.formatName;

                varString = HierarchyVarHelper.RemoveExtensionHierarchyKey(result.varString);
                VarStringAnalysisResult.TryParse(varString, out result);
            }

            if (!tso) tso = ScriptableObject.CreateInstance<TSO>();
            tso.varName = result.varName;
            tso.varScope = result.varScope;
            var so = new SerializedObject(tso);
            so.UpdateIfRequiredOrScript();
            var sp = so.FindProperty(nameof(TSO._propertyPathList));

            if (VarStringEditorWindow.newVarString)
            {
                var pathSP = sp.FindPropertyRelative(nameof(TSO._propertyPathList._propertyPaths));
                pathSP.arraySize = result.keys.Count;
                var i = 0;
                foreach (var k in result.keys)
                {
                    pathSP.GetArrayElementAtIndex(i++).FindPropertyRelative(nameof(TypeMember._memberName)).stringValue = k.formatName;
                }
            }

            //修改后的回调
            Action delayCall = () =>
            {
                var newVarString = ScriptOption.VarFlag + result.varName + "." + tso._propertyPathList.ToPropertyPath() + result.varScope.ToVarScopeString();
                VarStringEditorWindow.varString = HierarchyVarHelper.AddOrUpdateExtensionHierarchyKey(newVarString, extensionHierarchyKey);
                varStringEditorWindow.CallVarStringChanged();
                varStringEditorWindow.Repaint();
            };

            //撤销重做中
            if (VarStringEditorWindow.inUndo)
            {
                UICommonFun.DelayCall(delayCall);
            }
            //获取根对象的值
            if (scriptManager.TryGetHierarchyVarValue(ScriptOption.VarFlag + result.varName + result.varScope.ToVarScopeString(), out var value))
            {
                tso._propertyPathList.SetInstance(value?.GetType(), tso, value);
            }
            switch (result.varScope)
            {
                case EVarScope.Reference: break;
                default://下拉选择的绘制
                    {
                        var display = result.varString;
                        if (GUILayout.Button(CommonFun.TempContent(display, display), EditorObjectHelper.MiniPopup))
                        {
                            CommonFun.FocusControl();
                            var selectText = result.extensionHierarchyKey != null ? HierarchyVarHelper.RemoveExtensionHierarchyKey(result.varHierarchyString) : result.varHierarchyString;
                            var varStrings = new List<string>();

                            if (scriptManager.varContext.TryGetVarCollection(result.varScope, out var varCollection)
                                && varCollection != null
                                && varCollection.varDictionary.TryGetVariable(result.varName, out var variable))
                            {
                                variable?.hierarchyVar?.Foreach((parent, index, key, current) =>
                                {
                                    if (parent == null)
                                    {
                                        varStrings.Add(current.varHierarchyString.Replace("/", ""));
                                    }
                                    else
                                    {
                                        varStrings.Add(current.varHierarchyString);
                                    }
                                });
                            }

                            var spCopy = sp.Copy();
                            EditorHelper.DrawMenu(selectText, varStrings.ToArray(), newSelectText =>
                            {
                                VarStringAnalysisResult.TryParse(newSelectText, out var result0);
                                var pathSP = spCopy.FindPropertyRelative(nameof(TSO._propertyPathList._propertyPaths));
                                pathSP.arraySize = result0.keys.Count;
                                var i = 0;
                                foreach (var k in result0.keys)
                                {
                                    pathSP.GetArrayElementAtIndex(i++).FindPropertyRelative(nameof(TypeMember._memberName)).stringValue = k.formatName;
                                }

                                UICommonFun.DelayCall(delayCall);
                                //应用修改
                                spCopy.serializedObject.ApplyModifiedProperties();
                            });
                        }
                        break;
                    }
            }

            //属性路径列表的绘制
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(sp, PropertyData.GetPropertyData(sp).trLabel);
            if (EditorGUI.EndChangeCheck())
            {
                UICommonFun.DelayCall(delayCall);
            }

            //应用修改
            so.ApplyModifiedProperties();
        }
    }
}
