using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension.Base.XUnityEditor;
using XCSJ.EditorExtension.EditorWindows;
using XCSJ.LitJson;
using XCSJ.PluginCommonUtils;
using XCSJ.Scripts;
using XCSJ.Tools;

namespace XCSJ.EditorExtension.CNScripts.Windows
{
    /// <summary>
    /// 层级变量编辑器
    /// </summary>
    [Name(Title)]
    [XCSJ.Attributes.Icon(EIcon.Variable)]
    public class HierarchyVarEditorWindow : XEditorWindowWithScrollView<HierarchyVarEditorWindow>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "层级变量编辑器";

        /// <summary>
        /// 打开
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="variable"></param>
        /// <param name="variableSerializedProperty"></param>
        public static void Open(UnityEngine.Object obj, IVariable variable, SerializedProperty variableSerializedProperty)
        {
            OpenAndFocus();
            instance.Set(obj, variable, variableSerializedProperty);
        }

        private static void CloseNow() => Open(default, default, default);

        private void Set(UnityEngine.Object obj, IVariable variable, SerializedProperty variableSerializedProperty)
        {
            if (!obj)
            {
                CloseIfNeed();
                return;
            }

            HierarchyVarEditorWindow.variable = variable;
            HierarchyVarEditorWindow.varHost = obj;
            HierarchyVarEditorWindow.variableSerializedProperty = variableSerializedProperty?.Copy();
            variablePropertyPath = variableSerializedProperty?.propertyPath ?? "";

            varValueJsonString = hierarchyVar.ToJson(true);
        }

        private static UnityEngine.Object varHost;
        private static IVariable variable;
        /// <summary>
        /// 变量序列化属性
        /// </summary>
        private static SerializedProperty variableSerializedProperty { get; set; }
        private static string variablePropertyPath { get; set; }

        /// <summary>
        /// 在编辑中
        /// </summary>
        /// <param name="variableSP"></param>
        /// <returns></returns>
        public static bool InEditing(SerializedProperty variableSP)
        {
            if (variableSerializedProperty == null || variableSP == null) return false;
            if (inUndo)
            {
                inUndo = false;
                if (new SerializedObject(varHost).FindProperty(variablePropertyPath) == null)
                {
                    CloseNow();
                    return false;
                }
            }
            return SerializedProperty.EqualContents(variableSP, variableSerializedProperty);
        }

        /// <summary>
        /// 在编辑中
        /// </summary>
        /// <param name="variable"></param>
        /// <returns></returns>
        public static bool InEditing(Variable variable)
        {
            if (HierarchyVarEditorWindow.variable == null || variable == null) return false;
            if (inUndo)
            {
                inUndo = false;
            }
            return HierarchyVarEditorWindow.variable == variable;
        }

        private IHierarchyVar hierarchyVar => variable?.hierarchyVar;
        private string varValueJsonString;

        private EDisplayMode _displayMode = EDisplayMode.List;

        /// <summary>
        /// 显示模式
        /// </summary>
        public EDisplayMode displayMode
        {
            get => _displayMode;
            set
            {
                if (_displayMode != value)
                {
                    _displayMode = value;
                    XDreamerExtensionOption.weakInstance.hierarchyVarEditorWindowDisplayMode = value;
                    XDreamerExtensionOption.weakInstance.MarkDirty();
                }
            }
        }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            Undo.undoRedoPerformed += OnUndo;
            OnOptionModified(XDreamerExtensionOption.weakInstance);
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            Undo.undoRedoPerformed -= OnUndo;
        }

        /// <summary>
        /// 当选项修改时
        /// </summary>
        /// <param name="option"></param>
        protected override void OnOptionModified(Option option)
        {
            base.OnOptionModified(option);
            if (option is XDreamerExtensionOption extensionOption)
            {
                displayMode = extensionOption.hierarchyVarEditorWindowDisplayMode;
            }
        }

        private static bool inUndo = false;

        private void OnUndo()
        {
            inUndo = true;
            variable?.MarkDirty();
            varValueJsonString = hierarchyVar.ToJson(true);
            Repaint();
        }

        /// <summary>
        /// 绘制GUI
        /// </summary>
        protected override void OnGUI()
        {
            if (!varHost)
            {
                CloseIfNeed();
                return;
            }
            var hierarchyVar = this.hierarchyVar;
            if (hierarchyVar == null) return;

            EditorGUILayout.BeginHorizontal(GUI.skin.box, UICommonOption.Height16, GUILayout.ExpandWidth(true));

            GUILayout.Label("变量宿主:");
            EditorGUILayout.ObjectField(varHost, varHost.GetType(), true, GUILayout.ExpandWidth(true));

            GUILayout.Label("变量作用域:");
            EditorGUILayout.LabelField(CommonFun.NameTip(hierarchyVar.varScope), UICommonOption.Width60, UICommonOption.Height16);

            //GUILayout.FlexibleSpace();

            GUILayout.Label("变量字符串:");
            EditorGUILayout.SelectableLabel(hierarchyVar.varString, UICommonOption.Height16);

            EditorGUI.BeginChangeCheck();
            var selectList = UICommonFun.ButtonToggle(CommonFun.NameTip(EDisplayMode.List, ENameTip.EmptyTextWhenHasImage), displayMode == EDisplayMode.List, EditorStyles.miniButtonLeft, UICommonOption.WH24x16);
            var selectText = UICommonFun.ButtonToggle(CommonFun.NameTip(EDisplayMode.Text, ENameTip.EmptyTextWhenHasImage), displayMode == EDisplayMode.Text, EditorStyles.miniButtonRight, UICommonOption.WH24x16);
            if (EditorGUI.EndChangeCheck())
            {
                switch (displayMode)
                {
                    case EDisplayMode.List:
                        {
                            if (selectText)
                            {
                                displayMode = EDisplayMode.Text;
                                varValueJsonString = hierarchyVar.ToJson(true);
                            }
                            break;
                        }
                    case EDisplayMode.Text:
                        {
                            if (selectList)
                            {
                                displayMode = EDisplayMode.List;
                            }
                            break;
                        }
                }
            }

            //打开
            if (GUILayout.Button(TrLabel(nameof(hierarchyKeyExtensionDescription), ENameTip.EmptyTextWhenHasImage), EditorStyles.miniButton, UICommonOption.WH24x16))
            {
                HierarchyKeyExtensionViewerEditorWindow.OpenAndFocus();
            }

            EditorGUILayout.EndHorizontal();

            base.OnGUI();
        }

        /// <summary>
        /// 层级键扩展说明
        /// </summary>
        [Name("层级键扩展说明")]
        [Tip("打开层级键扩展查看器,同时关闭当前编辑窗口", "Open the level key extension viewer and close the current editing window")]
        [XCSJ.Attributes.Icon(EIcon.Help)]
        public string hierarchyKeyExtensionDescription = "";

        /// <summary>
        /// 带滚动视图的GUI
        /// </summary>
        public override void OnGUIWithScrollView()
        {
            switch (displayMode)
            {
                case EDisplayMode.List:
                    {
                        Draw(hierarchyVar, 0);
                        break;
                    }
                case EDisplayMode.Text:
                    {
                        EditorGUI.BeginChangeCheck();
                        varValueJsonString = EditorGUILayout.TextArea(varValueJsonString, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                        if (EditorGUI.EndChangeCheck())
                        {
                            varHost.XModifyProperty(() => hierarchyVar.TrySetValue(varValueJsonString, hierarchyVar.varType));
                        }
                        break;
                    }
            }
        }

        /// <summary>
        /// 键值字典
        /// </summary>
        public Dictionary<string, string> keyValues = new Dictionary<string, string>();

        /// <summary>
        /// 键值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string KeyValue(string key)
        {
            if (keyValues.TryGetValue(key ?? "", out var value)) return value;
            keyValues[key ?? ""] = "";
            return "";
        }

        private void Draw(IHierarchyVar hierarchyVar, int index)
        {
            UICommonFun.BeginHorizontal(index);

            //变量名
            EditorGUILayout.PrefixLabel(hierarchyVar.name);

            //类型
            var varType = hierarchyVar.varType;
            EditorGUILayout.LabelField(varType.ToGUIContent(), UICommonOption.Width20);

            //根据不同类型绘制不同的信息
            switch (varType)
            {
                case EVarType.Array:
                    {
                        EditorGUILayout.LabelField("新增值:", UICommonOption.Width60);
                        var addValueText = KeyValue(hierarchyVar.varString);
                        var addValueTextNew = EditorGUILayout.TextField(addValueText);
                        if (addValueTextNew != addValueText)
                        {
                            keyValues[hierarchyVar.varString] = addValueText = addValueTextNew;
                        }
                        if (GUILayout.Button(UICommonOption.Insert, EditorStyles.miniButtonRight, UICommonOption.WH20x16))
                        {
                            CommonFun.FocusControl();
                            varHost.XModifyProperty(() => hierarchyVar.TryAddChild(addValueText, out _));
                        }

                        break;
                    }
                case EVarType.Dictionary:
                    {
                        EditorGUILayout.LabelField("新增键:", UICommonOption.Width60);
                        var addKeyText = KeyValue(hierarchyVar.varString);
                        var addKeyTextNew = EditorGUILayout.TextField(addKeyText);
                        if (addKeyTextNew != addKeyText)
                        {
                            keyValues[hierarchyVar.varString] = addKeyText = VariableHelper.Format(addKeyTextNew);
                        }
                        if (GUILayout.Button(UICommonOption.Insert, EditorStyles.miniButtonRight, UICommonOption.WH20x16))
                        {
                            CommonFun.FocusControl();
                            if (!string.IsNullOrEmpty(addKeyText))
                            {
                                varHost.XModifyProperty(() => hierarchyVar.TryGetOrAddSetChild(addKeyText, "", EVarType.String, out _));
                            }
                        }

                        break;
                    }
                default:
                    {
                        EditorGUI.BeginChangeCheck();
                        var newText = EditorGUILayout.TextField(hierarchyVar.stringValue);
                        if (EditorGUI.EndChangeCheck())
                        {
                            varHost.XModifyProperty(() => hierarchyVar.TrySetValue(newText, EVarType.String));
                        }
                        break;
                    }
            }

            //类型切换
            EditorGUI.BeginChangeCheck();
            var newVarType = (EVarType)UICommonFun.EnumPopup(varType, UICommonOption.Width60);
            if (EditorGUI.EndChangeCheck())
            {
                varHost.XModifyProperty(() =>
                {
                    if (varType == EVarType.Array && newVarType == EVarType.Dictionary)//数组转字典,数组是特殊的字典类型，装完后任然是数组；如果要强制转换，则转换字符串为空字符串即可；
                    {
                        hierarchyVar.TrySetVarType(newVarType, "");
                    }
                    else
                    {
                        hierarchyVar.TrySetVarType(newVarType, hierarchyVar.stringValue);
                    }
                    varType = hierarchyVar.varType;
                });
            }

            //复制
            if (GUILayout.Button(UICommonOption.Copy, EditorStyles.miniButtonMid, UICommonOption.WH20x16))
            {
                CommonFun.CopyTextToClipboardForPC(hierarchyVar.varString);
            }

            //删除
            var parent = hierarchyVar.parent;
            if (parent != null && GUILayout.Button(UICommonOption.Delete, EditorStyles.miniButtonRight, UICommonOption.WH20x16))
            {
                CommonFun.FocusControl();

                var tmpParent = parent;
                var tmpIndex = index;
                UICommonFun.DelayCall(() =>
                {
                    varHost.XModifyProperty(() => tmpParent.TryRemoveChild(tmpIndex, false, out _));
                    Repaint();
                });
            }

            UICommonFun.EndHorizontal();

            switch (varType)
            {
                case EVarType.Array:
                    {
                        int i = 0;
                        CommonFun.BeginLayout();
                        foreach (HierarchyVar hv in (IList)hierarchyVar)
                        {
                            Draw(hv, i++);
                        }
                        CommonFun.EndLayout();

                        break;
                    }
                case EVarType.Dictionary:
                    {
                        int i = 0;
                        CommonFun.BeginLayout();
                        foreach (DictionaryEntry kv in (IDictionary)hierarchyVar)
                        {
                            Draw(kv.Value as HierarchyVar, i++);
                        }
                        CommonFun.EndLayout();
                        break;
                    }
            }
        }

        private void OnLostFocus() => CloseIfNeed();

        private void CloseIfNeed()
        {
            try
            {
                Close();
                varHost = default;
                variable = default;
                variableSerializedProperty = default;
            }
            catch { }
        }
    }
}
