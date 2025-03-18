using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using XCSJ.Caches;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension.CNScripts.Windows;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.Extension.CNScripts.Base;
using XCSJ.Helper;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.CNScripts;
using XCSJ.Scripts;

namespace XCSJ.EditorExtension.CNScripts
{
    /// <summary>
    /// 编辑器脚本组手
    /// </summary>
    public class EditorScriptHelper
    {
        /// <summary>
        /// UGUI菜单
        /// </summary>
        public const string UGUIMenu = XDreamerMenu.ScriptEvent + "UGUI/";

        /// <summary>
        /// 输入菜单
        /// </summary>
        public const string InputMenu = XDreamerMenu.ScriptEvent + "输入/";

        /// <summary>
        /// 初始化
        /// </summary>
        public static void Init() { }
    }

    #region 变量集合

    /// <summary>
    /// 基础变量集合属性绘制器
    /// </summary>
    [LanguageFileOutput]
    public abstract class BaseVarCollectionPropertyDrawer : PropertyDrawer
    {
        private ReorderableList reorderableList;

        /// <summary>
        /// 获取可排序列表
        /// </summary>
        /// <param name="variablesSP"></param>
        /// <returns></returns>
        public ReorderableList GetReorderableList(SerializedProperty variablesSP)
        {
            if (reorderableList == null)
            {
                reorderableList = new ReorderableList(variablesSP.serializedObject, null, true, false, false, false);
            }
            reorderableList.serializedProperty = variablesSP;
            return reorderableList;
        }

        /// <summary>
        /// 获取属性高度
        /// </summary>
        /// <param name="property"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => 0f;

        /// <summary>
        /// 绘制GUI
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!OnDrawHead(position, property, label)) return;

            try
            {
                var variablesSP = property.FindPropertyRelative(nameof(VarCollection._variables));
                //var variablesData = PropertyData.GetPropertyData(variablesSP);

                CommonFun.BeginLayout(true, false);

                //当绘制添加变量
                OnDrawAddVariable(property, variablesSP);

                //获取可排序列表
                var reorderableList = this.GetReorderableList(variablesSP);

                //元素高度
                reorderableList.elementHeightCallback=(int index) => GetDrawVariablesElementHeight(index, reorderableList);

                //当绘制变量列表头部
                reorderableList.drawHeaderCallback = (Rect rect) => OnDrawVariablesHead(rect, reorderableList);

                //当绘制变量列表元素
                reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => OnDrawVariablesElement(rect, index, isActive, isFocused, reorderableList);

                //执行布局列表
                reorderableList.DoLayoutList();
            }
            finally
            {
                CommonFun.EndLayout();
            }
        }

        /// <summary>
        /// 当绘制头部
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
        /// <returns>如果展开返回True；折叠返回False；</returns>
        protected bool OnDrawHead(Rect position, SerializedProperty property, GUIContent label)
        {
            var isExpanded = property.isExpanded;
            var isExpandedNew = UICommonFun.Foldout(isExpanded, label, () => OnDrawHead_After(property));
            if (isExpandedNew != isExpanded)
            {
                property.isExpanded = isExpandedNew;
            }
            return isExpandedNew;
        }

        /// <summary>
        /// 当绘制头部之后
        /// </summary>
        /// <param name="varCollectionSP"></param>
        protected virtual void OnDrawHead_After(SerializedProperty varCollectionSP)
        {
            if (GUILayout.Button(UICommonOption.Delete, EditorStyles.miniButtonRight, UICommonOption.WH24x16))
            {
                varCollectionSP.FindPropertyRelative(nameof(VarCollection._variables)).arraySize = 0;
                RefreshVarDictionary(varCollectionSP);
            }
        }

        /// <summary>
        /// 当绘制添加变量
        /// </summary>
        /// <param name="property"></param>
        /// <param name="variablesSP"></param>
        protected void OnDrawAddVariable(SerializedProperty property, SerializedProperty variablesSP)
        {
            var varNameSP = property.FindPropertyRelative(nameof(VarCollection._varName));
            var varNameData = PropertyData.GetPropertyData(varNameSP);

            var varName = varNameSP.stringValue;

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(varNameData.trLabel, GUILayout.Width(60));
            var varNameNew = EditorGUILayout.TextField(varName);
            if (varNameNew != varName)
            {
                varNameNew = VariableHelper.Format(varNameNew);
                if (varNameNew != varName)
                {
                    varNameSP.stringValue = varNameNew;
                }
            }
            if (GUILayout.Button(UICommonOption.Insert, UICommonOption.WH24x16) && !string.IsNullOrEmpty(varNameNew))
            {
                CommonFun.FocusControl();
                AddVariable(variablesSP, varNameNew, out _);
            }
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 添加变量
        /// </summary>
        /// <param name="variablesSP"></param>
        /// <param name="varName"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        protected virtual SerializedProperty AddVariable(SerializedProperty variablesSP, string varName, out int index)
        {
            //已经存在同名变量
            var variableSP = variablesSP.GetArrayElement(out index, sp => sp.FindPropertyRelative(nameof(Variable.name)).stringValue == varName);
            if (variableSP != null) return variableSP;

            //增加新的变量
            variableSP = variablesSP.AddArrayElement(out index);
            variableSP.FindPropertyRelative(nameof(Variable.name)).stringValue = varName;
            return variableSP;
        }

        /// <summary>
        /// 变量标记脏
        /// </summary>
        /// <param name="variableSP"></param>
        /// <returns></returns>
        protected IVariable VariableMarkDirty(SerializedProperty variableSP)
        {
            if (variableSP?.GetSerializedPropertyValue() is IVariable variable)
            {
                OnVariableMarkDirty(variableSP, variable);
                return variable;
            }
            return default;
        }

        /// <summary>
        /// 当变量标记为脏时
        /// </summary>
        /// <param name="variableSP"></param>
        /// <param name="variable"></param>
        protected virtual void OnVariableMarkDirty(SerializedProperty variableSP,IVariable variable)
        {
            UICommonFun.DelayCall(variable.MarkDirty, nameof(BaseVarCollectionPropertyDrawer) + "." + nameof(OnVariableMarkDirty));
        }

        /// <summary>
        /// 刷新变量字典
        /// </summary>
        protected void RefreshVarDictionary(SerializedProperty varCollectionOrVariableSP)
        {
            var varCollectionHost = varCollectionOrVariableSP.serializedObject.targetObject as IVarCollectionHost;
            if (varCollectionHost == null) return;

            UICommonFun.DelayCall(() =>
            {
                varCollectionHost.varCollection.RefreshVarDictionary();
            });
        }

        /// <summary>
        /// 当绘制变量列表头部
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="reorderableList"></param>
        protected abstract void OnDrawVariablesHead(Rect rect, ReorderableList reorderableList);

        /// <summary>
        /// 当绘制变量列表元素
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="index"></param>
        /// <param name="isActive"></param>
        /// <param name="isFocused"></param>
        /// <param name="reorderableList"></param>
        protected abstract void OnDrawVariablesElement(Rect rect, int index, bool isActive, bool isFocused, ReorderableList reorderableList);

        /// <summary>
        /// 获取绘制变量列表元素高度
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="reorderableList"></param>
        protected virtual float GetDrawVariablesElementHeight(int index, ReorderableList reorderableList) => reorderableList.elementHeight;
    }

    /// <summary>
    /// 变量集合属性绘制器
    /// </summary>
    [CustomPropertyDrawer(typeof(VarCollection), true)]
    public class VarCollectionPropertyDrawer : BaseVarCollectionPropertyDrawer
    {
        /// <summary>
        /// 当变量标记为脏时:检查变量类型
        /// </summary>
        /// <param name="variableSP"></param>
        /// <param name="variable"></param>
        protected override void OnVariableMarkDirty(SerializedProperty variableSP, IVariable variable)
        {
            if (variable is CustomVariable customVariable)
            {
                var varType = customVariable._varType;
                UICommonFun.DelayCall(() =>
                {
                    variable.MarkDirty();
                    var newVarType = variable.hierarchyVar.varType;
                    if (newVarType != varType)
                    {
                        if (newVarType == EVarType.Invalid) newVarType = EVarType.String;
                        ((UnityEngine.Object)variable.varCollectionHost).XModifyProperty(() => { customVariable._varType = newVarType; });
                    }
                }, nameof(VarCollectionPropertyDrawer) + "." + nameof(OnVariableMarkDirty));
            }
            else
            {
                base.OnVariableMarkDirty(variableSP, variable);
            }
        }

        /// <summary>
        /// 当绘制变量列表头部
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="reorderableList"></param>
        [LanguageTuple("Variable Name", "变量名")]
        [LanguageTuple("Type", "类型")]
        [LanguageTuple("Variable Value", "变量值")]
        [LanguageTuple("Operation", "操作")]
        protected override void OnDrawVariablesHead(Rect rect, ReorderableList reorderableList)
        {
            var tmpRect = new Rect(rect.x, rect.y, 45, rect.height);
            EditorGUI.SelectableLabel(tmpRect, "NO.");

            tmpRect.x = tmpRect.xMax;
            tmpRect.width = rect.width * 2 / 5 - 70;
            EditorGUI.SelectableLabel(tmpRect, "Variable Name".Tr(this));

            tmpRect.x = tmpRect.xMax;
            tmpRect.width = 30;
            EditorGUI.SelectableLabel(tmpRect, "Type".Tr(this));

            tmpRect.x = tmpRect.xMax + 5;
            tmpRect.width = rect.width * 3 / 5 - 20 * 2 - 10;
            EditorGUI.SelectableLabel(tmpRect, "Variable Value".Tr(this));

            tmpRect.x = tmpRect.xMax;
            tmpRect.width = 40;
            EditorGUI.SelectableLabel(tmpRect, "Operation".Tr(this));

            //调整标记量
            hasInEditing = false;
        }

        bool hasInEditing = false;

        /// <summary>
        /// 当绘制变量列表元素
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="index"></param>
        /// <param name="isActive"></param>
        /// <param name="isFocused"></param>
        /// <param name="reorderableList"></param>
        protected override void OnDrawVariablesElement(Rect rect, int index, bool isActive, bool isFocused, ReorderableList reorderableList)
        {
            var variablesSP = reorderableList.serializedProperty;
            if (variablesSP == null || index >= variablesSP.arraySize) return;

            var element = variablesSP.GetArrayElementAtIndex(index);
            bool inEditing = !hasInEditing && HierarchyVarEditorWindow.InEditing(element);

            rect.y += 2;
            rect.height = EditorGUIUtility.singleLineHeight + 2;

            //NO.
            var tmpRect = new Rect(rect.x, rect.y, 30, rect.height);
            EditorGUI.SelectableLabel(tmpRect, (index + 1).ToString());

            //名称
            var nameSP = element.FindPropertyRelative(nameof(CustomVariable.name));
            var name = nameSP.stringValue;
            tmpRect.x = tmpRect.xMax;
            tmpRect.width = rect.width * 2 / 5 - 50;
            EditorGUI.SelectableLabel(tmpRect, name);

            //类型
            var typeSP = element.FindPropertyRelative(nameof(CustomVariable._varType));
            tmpRect.x = tmpRect.xMax;
            tmpRect.width = 20;
            EditorGUI.LabelField(tmpRect, ((EVarType)typeSP.intValue).ToGUIContent());

            //值-字符串
            var valueSP = element.FindPropertyRelative(nameof(CustomVariable.value));
            tmpRect.x = tmpRect.xMax;
            tmpRect.width = rect.width * 3 / 5 - 20 * 2;
            EditorGUI.BeginChangeCheck();
            EditorGUI.DelayedTextField(tmpRect, valueSP, GUIContent.none);
            if (EditorGUI.EndChangeCheck())
            {
                VariableMarkDirty(element);
            }

            //编辑
            tmpRect.x = tmpRect.xMax;
            tmpRect.width = 20;
            if (inEditing)
            {
                hasInEditing = true;
                if (!GUI.Toggle(tmpRect, true, UICommonOption.Script, EditorStyles.miniButtonMid))
                {
                    HierarchyVarEditorWindow.Open(default, default, default);
                }
            }
            else if (GUI.Button(tmpRect, UICommonOption.Script, EditorStyles.miniButtonMid))
            {
                CommonFun.FocusControl();
                HierarchyVarEditorWindow.Open(variablesSP.serializedObject.targetObject, element.GetSerializedPropertyValue() as Variable, element);
            }

            //删除
            EditorGUI.BeginDisabledGroup(VariableHelper.IsSystemVariable(name));
            tmpRect.x = tmpRect.xMax;
            tmpRect.width = 20;
            if (GUI.Button(tmpRect, UICommonOption.Delete, EditorStyles.miniButtonRight))
            {
                CommonFun.FocusControl();
                variablesSP.DeleteArrayElementAtIndex(index);
                RefreshVarDictionary(variablesSP);
            }

            EditorGUI.EndDisabledGroup();
        }
    }

    /// <summary>
    /// 全局变量集合属性绘制器
    /// </summary>
    [CustomPropertyDrawer(typeof(GlobalVarCollection), true)]
    public class GlobalVarCollectionPropertyDrawer : VarCollectionPropertyDrawer
    {
        /// <summary>
        /// 当绘制头部之后
        /// </summary>
        /// <param name="varCollectionSP"></param>
        protected override void OnDrawHead_After(SerializedProperty varCollectionSP)
        {
            if (GUILayout.Button(CommonFun.NameTooltip(typeof(EnumEditor)), EditorStyles.miniButtonLeft, UICommonOption.Width80))
            {
                EnumEditor.OpenAndFocus();
            }
            base.OnDrawHead_After(varCollectionSP);
        }
    }

    /// <summary>
    /// 静态变量字典型变量集合属性绘制器
    /// </summary>
    [CustomPropertyDrawer(typeof(StaticVarDictionary_VarCollection), true)]
    public class StaticVarDictionary_VarCollectionPropertyDrawer : VarCollectionPropertyDrawer
    {
        bool expand = true;

        ReorderableList dictionaryReorderableList;

        GUIContent labelDictionary = new GUIContent();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
        [LanguageTuple("(Runtime Dictionary)", "(运行时字典)")]
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            base.OnGUI(position, property, label);

            if (!property.TryGetSerializedPropertyValue(out var obj) || !(obj is StaticVarDictionary_VarCollection data)) return;
            var list = data.varDictionary.ToList();

            #region 折页标题栏

            labelDictionary.text = label.text + "(Runtime Dictionary)".Tr(this);
            labelDictionary.tooltip = label.tooltip;
            expand = UICommonFun.Foldout(expand, labelDictionary, () =>
            {
                if (GUILayout.Button(UICommonOption.Delete, EditorStyles.miniButtonRight, UICommonOption.WH24x16))
                {
                    data.varDictionary.Clear();
                    data.RefreshVarDictionary();
                }
            });
            if (!expand) return;

            #endregion

            try
            {
                CommonFun.BeginLayout(true, false);

                #region 变量列表

                if (this.dictionaryReorderableList == null)
                {
                    this.dictionaryReorderableList = new ReorderableList(list, null, false, false, false, false);
                }
                this.dictionaryReorderableList.list = list;

                #region drawElementCallback
                //绘制头部
                dictionaryReorderableList.drawHeaderCallback = (Rect rect) =>
                {
                    var tmpRect = new Rect(rect.x, rect.y, 45, rect.height);
                    EditorGUI.SelectableLabel(tmpRect, "NO.");

                    tmpRect.x = tmpRect.xMax;
                    tmpRect.width = rect.width * 2 / 5 - 70;
                    EditorGUI.SelectableLabel(tmpRect, "Variable Name".Tr(this));

                    tmpRect.x = tmpRect.xMax;
                    tmpRect.width = 30;
                    EditorGUI.SelectableLabel(tmpRect, "Type".Tr(this));

                    tmpRect.x = tmpRect.xMax + 5;
                    tmpRect.width = rect.width * 3 / 5 - 20 * 2 - 10;
                    EditorGUI.SelectableLabel(tmpRect, "Variable Value".Tr(this));

                    tmpRect.x = tmpRect.xMax;
                    tmpRect.width = 40;
                    EditorGUI.SelectableLabel(tmpRect, "Operation".Tr(this));
                };
                //绘制元素
                bool hasInEditing = false;
                dictionaryReorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    if (index >= list.Count) return;
                    var element = list[index];
                    bool inEditing = !hasInEditing && HierarchyVarEditorWindow.InEditing(element.Value);

                    rect.y += 2;
                    rect.height = EditorGUIUtility.singleLineHeight + 2;

                    //NO.
                    var tmpRect = new Rect(rect.x, rect.y, 30, rect.height);
                    EditorGUI.SelectableLabel(tmpRect, (index + 1).ToString());

                    //名称
                    var name = element.Key;
                    tmpRect.x = tmpRect.xMax;
                    tmpRect.width = rect.width * 2 / 5 - 50;
                    EditorGUI.SelectableLabel(tmpRect, name);

                    //类型
                    var typeSP = element.Value._varType;
                    tmpRect.x = tmpRect.xMax;
                    tmpRect.width = 20;
                    EditorGUI.LabelField(tmpRect, typeSP.ToGUIContent());

                    //值-字符串
                    var valueSP = element.Value.value;
                    tmpRect.x = tmpRect.xMax;
                    tmpRect.width = rect.width * 3 / 5 - 20 * 2;
                    EditorGUI.BeginChangeCheck();
                    var value = EditorGUI.DelayedTextField(tmpRect, GUIContent.none, valueSP);
                    if (EditorGUI.EndChangeCheck())
                    {
                        element.Value.value = value;
                        element.Value.MarkDirty();                        
                    }

                    //编辑
                    tmpRect.x = tmpRect.xMax;
                    tmpRect.width = 20;
                    if (inEditing)
                    {
                        hasInEditing = true;
                        if (!GUI.Toggle(tmpRect, true, UICommonOption.Script, EditorStyles.miniButtonMid))
                        {
                            HierarchyVarEditorWindow.Open(default, default, default);
                        }
                    }
                    else if (GUI.Button(tmpRect, UICommonOption.Script, EditorStyles.miniButtonMid))
                    {
                        CommonFun.FocusControl();
                        HierarchyVarEditorWindow.Open(property.serializedObject.targetObject, element.Value, default);
                    }

                    //删除
                    EditorGUI.BeginDisabledGroup(VariableHelper.IsSystemVariable(name));
                    tmpRect.x = tmpRect.xMax;
                    tmpRect.width = 20;
                    if (GUI.Button(tmpRect, UICommonOption.Delete, EditorStyles.miniButtonRight))
                    {
                        CommonFun.FocusControl();
                        UICommonFun.DelayCall(() =>
                        {
                            data.varDictionary.Remove(element.Key);
                            data.RefreshVarDictionary();
                        });
                    }

                    EditorGUI.EndDisabledGroup();
                };

                #endregion

                dictionaryReorderableList.DoLayoutList();

                #endregion
            }
            finally
            {
                CommonFun.EndLayout();
            }
        }
    }

    /// <summary>
    /// 引用变量集合数据绘制器
    /// </summary>
    [CustomPropertyDrawer(typeof(ReferenceVarCollectionData), true)]
    public class ReferenceVarCollectionPropertyDrawer : BaseVarCollectionPropertyDrawer
    {
        /// <summary>
        /// 添加变量
        /// </summary>
        /// <param name="variablesSP"></param>
        /// <param name="varName"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        protected override SerializedProperty AddVariable(SerializedProperty variablesSP, string varName, out int index)
        {
            var variableSP = base.AddVariable(variablesSP, varName, out index);
            if (variablesSP.arraySize == 1)
            {
                variableSP.FindPropertyRelative(nameof(ReferenceVariable._value)).FindPropertyRelative(nameof(ReferenceVariable._value._argumentType)).intValue = (int)EArgumentType.UnityObject;
            }
            return variableSP;
        }

        /// <summary>
        /// 获取绘制变量列表元素高度
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="reorderableList"></param>
        protected override float GetDrawVariablesElementHeight(int index, ReorderableList reorderableList)
        {
            var variablesSP = reorderableList.serializedProperty;
            if (variablesSP == null || index >= variablesSP.arraySize) return base.GetDrawVariablesElementHeight(index, reorderableList);

            var element = variablesSP.GetArrayElementAtIndex(index);
            var valueSP = element.FindPropertyRelative(nameof(ReferenceVariable._value));

            return EditorGUI.GetPropertyHeight(valueSP, GUIContent.none, true) + 2;
        }

        /// <summary>
        /// 当绘制变量列表元素
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="index"></param>
        /// <param name="isActive"></param>
        /// <param name="isFocused"></param>
        /// <param name="reorderableList"></param>
        protected override void OnDrawVariablesElement(Rect rect, int index, bool isActive, bool isFocused, ReorderableList reorderableList)
        {
            var variablesSP = reorderableList.serializedProperty;
            if (variablesSP == null || index >= variablesSP.arraySize) return;

            var element = variablesSP.GetArrayElementAtIndex(index);

            rect.y += 2;
            rect.height = EditorGUIUtility.singleLineHeight + 2;

            //NO.
            var tmpRect = new Rect(rect.x, rect.y, 30, rect.height);
            EditorGUI.SelectableLabel(tmpRect, (index + 1).ToString());

            //名称
            var nameSP = element.FindPropertyRelative(nameof(ReferenceVariable.name));
            var name = nameSP.stringValue;
            tmpRect.x = tmpRect.xMax;
            tmpRect.width = rect.width * 2 / 5 - 110;
            EditorGUI.SelectableLabel(tmpRect, name);

            //类型
            //var typeSP = element.FindPropertyRelative(nameof(AliasVar._varType));
            //tmpRect.x = tmpRect.xMax;
            //tmpRect.width = 80;
            //EditorGUI.LabelField(tmpRect, "");

            //值
            var valueSP = element.FindPropertyRelative(nameof(ReferenceVariable._value));
            tmpRect.x = tmpRect.xMax;
            tmpRect.width = rect.width * 3 / 5 - 40 + 80;
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(tmpRect, valueSP, GUIContent.none, true);
            if (EditorGUI.EndChangeCheck())
            {
                VariableMarkDirty(element);
            }

            //删除
            EditorGUI.BeginDisabledGroup(VariableHelper.IsSystemVariable(name));
            tmpRect.x = tmpRect.xMax + 20;
            tmpRect.width = 20;
            if (GUI.Button(tmpRect, UICommonOption.Delete, EditorStyles.miniButtonRight))
            {
                CommonFun.FocusControl();
                variablesSP.DeleteArrayElementAtIndex(index);
                RefreshVarDictionary(variablesSP);
            }

            EditorGUI.EndDisabledGroup();
        }

        /// <summary>
        /// 当绘制变量列表头部
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="reorderableList"></param>
        protected override void OnDrawVariablesHead(Rect rect, ReorderableList reorderableList)
        {
            var tmpRect = new Rect(rect.x, rect.y, 45, rect.height);
            EditorGUI.SelectableLabel(tmpRect, "NO.");

            tmpRect.x = tmpRect.xMax;
            tmpRect.width = rect.width * 2 / 5 - 70;
            EditorGUI.SelectableLabel(tmpRect, "Variable Name".Tr(this));

            tmpRect.x = tmpRect.xMax;
            tmpRect.width = 30;
            EditorGUI.SelectableLabel(tmpRect, "Type".Tr(this));

            tmpRect.x = tmpRect.xMax + 5;
            tmpRect.width = rect.width * 3 / 5 - 20 * 2 - 10;
            EditorGUI.SelectableLabel(tmpRect, "Variable Value".Tr(this));

            tmpRect.x = tmpRect.xMax;
            tmpRect.width = 40;
            EditorGUI.SelectableLabel(tmpRect, "Operation".Tr(this));
        }
    }

    #endregion

    #region 函数

    /// <summary>
    /// 函数属性绘制器
    /// </summary>
    [CustomPropertyDrawer(typeof(CustomFunction), true)]
    [LanguageFileOutput]
    public class CustomFunctionPropertyDrawer : PropertyDrawer
    {
        /// <summary>
        /// 获取属性高度
        /// </summary>
        /// <param name="property"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => 0f;

        /// <summary>
        /// 绘制GUI
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) => DrawFunction(property);

        /// <summary>
        /// 获取函数名称内容
        /// </summary>
        /// <param name="functionSP"></param>
        /// <returns></returns>
        public virtual GUIContent GetFunctionNameContent(SerializedProperty functionSP) => CommonFun.TempContent(functionSP.FindPropertyRelative(nameof(CustomFunction.name)).stringValue);

        /// <summary>
        /// 绘制函数
        /// </summary>
        /// <param name="functionSP"></param>
        protected void DrawFunction(SerializedProperty functionSP)
        {
            #region 脚本列表的基础信息

            var displayModeSP = functionSP.FindPropertyRelative(nameof(CustomFunction._displayMode));
            var displayMode = (EDisplayMode)displayModeSP.intValue;

            var expandSP = functionSP.FindPropertyRelative(nameof(CustomFunction.Expand));
            var activeSP = functionSP.FindPropertyRelative(nameof(CustomFunction.Active));

            var toggleValue = activeSP.boolValue;

            EditorGUI.BeginChangeCheck();
            var expand = UICommonFun.Foldout(expandSP.boolValue, GetFunctionIcon(functionSP), ref toggleValue, GetFunctionNameContent(functionSP), () =>
            {
                //清除 按钮
                {
                    EditorGUI.BeginDisabledGroup(UICommonFun.hasScriptStringEditing);
                    if (GUILayout.Button(UICommonOption.Clear, EditorStyles.miniButtonMid, UICommonOption.WH24x16))
                    {
                        var ssl = functionSP.FindPropertyRelative(nameof(CustomFunction.ScriptStringList));
                        if (ssl != null)
                        {
                            ScriptViewerWindow.SetCurrentScriptString();
                            ssl.ClearArray();
                            ssl.arraySize = 1;
                        }
                    }
                    EditorGUI.EndDisabledGroup();
                }

                //执行 按钮
                {
                    if (GUILayout.Button(UICommonOption.Run, EditorStyles.miniButtonMid, UICommonOption.WH24x16))
                    {
                        UICommonFun.RunFunction(functionSP.GetSerializedPropertyValue() as Function, functionSP.serializedObject.targetObject as IVarCollectionHost);
                    }
                }

                //删除 按钮
                {
                    EditorGUI.BeginDisabledGroup(UICommonFun.hasScriptStringEditing);
                    if (GUILayout.Button(UICommonOption.Delete, EditorStyles.miniButtonRight, UICommonOption.WH24x16))
                    {
                        OnFunctionDisable(functionSP);
                    }
                    EditorGUI.EndDisabledGroup();
                }

                //脚本编辑视图风格
                {
                    EditorGUI.BeginChangeCheck();
                    var selectList = UICommonFun.ButtonToggle(EDisplayMode.List.TrLabel(ENameTip.EmptyTextWhenHasImage), displayMode == EDisplayMode.List, EditorStyles.miniButtonLeft, UICommonOption.WH24x16);
                    var selectText = UICommonFun.ButtonToggle(EDisplayMode.Text.TrLabel(ENameTip.EmptyTextWhenHasImage), displayMode == EDisplayMode.Text, EditorStyles.miniButtonRight, UICommonOption.WH24x16);
                    if (EditorGUI.EndChangeCheck())
                    {
                        switch (displayMode)
                        {
                            case EDisplayMode.List:
                                {
                                    if (selectText) displayMode = EDisplayMode.Text;
                                    break;
                                }
                            case EDisplayMode.Text:
                                {
                                    if (selectList) displayMode = EDisplayMode.List;
                                    break;
                                }
                            default:
                                {
                                    displayMode = EDisplayMode.List;
                                    break;
                                }
                        }
                        displayModeSP.intValue = (int)displayMode;
                        UICommonFun.CancleScriptStringEdit();
                    }
                }
            }, true);
            if (EditorGUI.EndChangeCheck())
            {
                expandSP.boolValue = expand;
                activeSP.boolValue = toggleValue;
                CommonFun.FocusControl();
            }
            if (!expand) return;

            #endregion

            #region 脚本字符串列表

            var ScriptStringListSP = functionSP.FindPropertyRelative(nameof(CustomFunction.ScriptStringList));
            CommonFun.BeginLayout(true, false);
            switch (displayMode)
            {
                case EDisplayMode.Text:
                    {
                        DrawFunctionText(ScriptStringListSP);
                        break;
                    }
                default:
                    {
                        DrawFunctionList(ScriptStringListSP);
                        break;
                    }
            }
            CommonFun.EndLayout();

            #endregion
        }

        private Texture2D functionIcon;

        /// <summary>
        /// 获取函数图标
        /// </summary>
        /// <param name="functionSP"></param>
        /// <returns></returns>
        public virtual Texture2D GetFunctionIcon(SerializedProperty functionSP) => functionIcon ? functionIcon : (functionIcon = UICommonFun.GetTexture2D(functionSP.serializedObject.targetObject.GetType()));

        /// <summary>
        /// 当函数禁用时
        /// </summary>
        /// <param name="functionSP"></param>
        protected void OnFunctionDisable(SerializedProperty functionSP)
        {
            functionSP.FindPropertyRelative(nameof(Function.Enable)).boolValue = false;
            functionSP.DeleteArrayElementCommand();
        }

        /// <summary>
        /// 可记录列表字典
        /// </summary>
        protected Dictionary<string, ReorderableList> reorderableLists = new Dictionary<string, ReorderableList>();

        /// <summary>
        /// 绘制错误
        /// </summary>
        /// <param name="errorMatch"></param>
        [LanguageTuple("Error: logical start / end flags do not match in pairs, Please check line {0}!", "错误: 逻辑开始/结束标志未成对匹配，请检查第{0}行！")]
        protected void DrawError(List<int> errorLines)
        {
            if (errorLines.Count > 0)
            {
                //EditorGUILayout.Separator();
                EditorGUILayout.SelectableLabel(string.Format("Error: logical start / end flags do not match in pairs, Please check line {0}!".Tr(this), errorLines[0] + 1), UICommonOption.labelRedBG, GUILayout.Height(16), GUILayout.ExpandWidth(true));
            }
        }

        /// <summary>
        /// 绘制函数列表
        /// </summary>
        /// <param name="ScriptStringListSP"></param>
        [LanguageTuple("Script String", "脚本字符串")]
        [LanguageTuple("Operation", "操作")]
        protected void DrawFunctionList(SerializedProperty ScriptStringListSP)
        {
            var arraySize = ScriptStringListSP.arraySize;
            //确保至少有一个元素
            if (arraySize == 0) ScriptStringListSP.arraySize = arraySize = 1;

            if (!reorderableLists.TryGetValue(ScriptStringListSP.propertyPath, out var reorderableList))
            {
                reorderableLists[ScriptStringListSP.propertyPath] = reorderableList = new ReorderableList(ScriptStringListSP.serializedObject, null, true, false, false, false);
            }
            reorderableList.serializedProperty = ScriptStringListSP;

            int indent = 0;
            List<int> errorLines = new List<int>();

            //绘制头部
            reorderableList.drawHeaderCallback = (Rect rect) =>
            {
                var tmpRect = new Rect(rect.x, rect.y, 35, rect.height);
                EditorGUI.SelectableLabel(tmpRect, "NO.");

                tmpRect.x = tmpRect.xMax;
                tmpRect.width = rect.width - 135;
                EditorGUI.SelectableLabel(tmpRect, "Script String".Tr(this));

                tmpRect.x = tmpRect.xMax;
                tmpRect.width = 100;
                EditorGUI.SelectableLabel(tmpRect, "Operation".Tr(this));
            };
            bool hasInEditing = false;            
            //绘制元素
            reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                if (index >= arraySize) return;
                var element = ScriptStringListSP.GetArrayElementAtIndex(index);
                var scriptStringSP = element?.FindPropertyRelative(nameof(ScriptString.scriptString));
                if (scriptStringSP == null) return;

                var grammarType = GetGrammarTypeByScriptString(scriptStringSP.stringValue);
                var isNote = grammarType == EGrammarType.Note;

                //计算缩进
                var currentIndent = ScriptHelper.GetIndent(grammarType, ref indent);
                if (currentIndent < 0 || indent < 0)
                {
                    errorLines.Add(index);
                    currentIndent = 0;
                }

                rect.y += 2;
                rect.height = EditorGUIUtility.singleLineHeight + 2;

                float bw = 100;
                float rw = 0;

                //编号
                rw += 20;
                bool inEditing = !hasInEditing;
                try
                {
                    inEditing &= SerializedProperty.EqualContents(ScriptViewerWindow.currentScriptStringSerializedProperty, element);
                }
                catch
                {
                    ScriptViewerWindow.SetCurrentScriptString();
                }

                if (inEditing)
                {
                    hasInEditing = true;
                    EditorGUI.LabelField(new Rect(rect.x, rect.y, 20, rect.height), CommonFun.TempContent((index + 1).ToString()), UICommonOption.labelSelectedStyle);
                }
                else
                {
                    EditorGUI.LabelField(new Rect(rect.x, rect.y, 20, rect.height), CommonFun.TempContent((index + 1).ToString()));
                }

                //缩进
                int w = 12 * currentIndent;
                EditorGUI.LabelField(new Rect(rect.x, rect.y, w, rect.height), " ");
                rw += w;

                //脚本字符串
                EditorGUI.BeginDisabledGroup(inEditing);
                EditorGUI.BeginChangeCheck();
                EditorGUI.PropertyField(new Rect(rect.x + rw, rect.y, rect.width - rw - bw, rect.height), scriptStringSP, GUIContent.none);
                if (EditorGUI.EndChangeCheck())
                {
                    ScriptStringMarkDirty(element);
                }
                EditorGUI.EndDisabledGroup();

                //开始检测按钮点击
                EditorGUI.BeginChangeCheck();

                //编辑
                {
                    rw = rect.width - bw;
                    if (inEditing)
                    {
                        bool ret = GUI.Toggle(new Rect(rect.x + rw, rect.y, 20, rect.height), true, UICommonOption.Script, EditorStyles.miniButtonMid);
                        if (!ret) ScriptViewerWindow.SetCurrentScriptString();
                    }
                    else if (GUI.Button(new Rect(rect.x + rw, rect.y, 20, rect.height), UICommonOption.Script, EditorStyles.miniButtonMid))
                    {
                        ScriptViewerWindow.SetCurrentScriptString(element, element.serializedObject.targetObject as IVarCollectionHost);
                    }
                }

                //执行
                {
                    rw += 20f;
                    if (GUI.Button(new Rect(rect.x + rw, rect.y, 20, rect.height), UICommonOption.Run, EditorStyles.miniButtonMid))
                    {
                        UICommonFun.RunScriptCmd(element.GetSerializedPropertyValue() as ScriptString, element.serializedObject.targetObject as IVarCollectionHost);
                    }
                }

                //注释
                {
                    rw += 20f;
                    EditorGUI.BeginDisabledGroup(inEditing);
                    if (UICommonFun.ButtonToggle(new Rect(rect.x + rw, rect.y, 20, rect.height), UICommonOption.noteContent, isNote))
                    {
                        if (!isNote && !inEditing && ScriptManager.instance)//添加注释--需要保证脚本管理器有效
                        {
                            scriptStringSP.stringValue = "/," + scriptStringSP.stringValue;
                            ScriptStringMarkDirty(element);
                        }
                    }
                    else
                    {
                        if (isNote && !inEditing)//移除注释
                        {
                            var scriptString = scriptStringSP.stringValue;
                            var first = scriptString.IndexOf(',');
                            if (first > 0)
                            {
                                scriptStringSP.stringValue = scriptString.Substring(first + 1);
                                ScriptStringMarkDirty(element);
                            }
                        }
                    }
                    EditorGUI.EndDisabledGroup();
                }

                //插入脚本
                {
                    EditorGUI.BeginChangeCheck();
                    rw += 20f;
                    if (GUI.Button(new Rect(rect.x + rw, rect.y, 20, rect.height), UICommonOption.Insert, EditorStyles.miniButtonMid))
                    {
                        ScriptStringListSP.InsertArrayElementAtIndex(index);
                    }
                    if (EditorGUI.EndChangeCheck())
                    {
                        UICommonFun.CancleScriptStringEdit();
                    }
                }

                //移除脚本
                {
                    rw += 20f;
                    if (GUI.Button(new Rect(rect.x + rw, rect.y, 20, rect.height), UICommonOption.Delete, EditorStyles.miniButtonRight))
                    {
                        arraySize--;
                        if (inEditing)//要删除的家伙是正在被编辑的
                        {
                            UICommonFun.CancleScriptStringEdit();
                        }
                        ScriptStringListSP.DeleteArrayElementAtIndex(index);
                    }
                }

                //检测到任意按钮点击时，取消控件聚焦
                if (EditorGUI.EndChangeCheck())
                {
                    CommonFun.FocusControl();
                }
            };

            //处理变更
            reorderableList.onChangedCallback = (ReorderableList list) =>
            {
                UICommonFun.CancleScriptStringEdit();
            };

            reorderableList.DoLayoutList();

            DrawError(errorLines);
        }

        /// <summary>
        /// 分隔符
        /// </summary>
        protected readonly char[] separator = new[] { '\n' };

        private StringBuilder stringBuilder = new StringBuilder();

        /// <summary>
        /// 绘制函数文本
        /// </summary>
        /// <param name="ScriptStringListSP"></param>
        protected void DrawFunctionText(SerializedProperty ScriptStringListSP)
        {
            var arraySize = ScriptStringListSP.arraySize;
            //确保至少有一个元素
            if (arraySize == 0) ScriptStringListSP.arraySize = arraySize = 1;

            int indent = 0;
            List<int> errorLines = new List<int>();
            stringBuilder.Clear();
            for (int i = 0; i < arraySize; ++i)
            {
                var scriptStringSP = ScriptStringListSP.GetArrayElementAtIndex(i)?.FindPropertyRelative(nameof(ScriptString.scriptString));
                if (scriptStringSP != null)
                {
                    //计算缩进
                    var currentIndent = ScriptHelper.GetIndent(GetGrammarTypeByScriptString(scriptStringSP.stringValue), ref indent);
                    if (currentIndent < 0 || indent < 0)
                    {
                        errorLines.Add(i);
                        currentIndent = 0;
                    }
                    stringBuilder.Append(ScriptOption.AutoAlignChar, currentIndent * 4);
                    stringBuilder.AppendLine(scriptStringSP.stringValue);
                }
            }

            var text = stringBuilder.ToString();
            var textNew = EditorGUILayout.TextArea(text, GUILayout.ExpandHeight(true));
            if (textNew != text)
            {
                var stringArray = textNew.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                if (stringArray.Length != arraySize)
                {
                    ScriptStringListSP.arraySize = arraySize = stringArray.Length;
                }
                for (int i = 0; i < arraySize; ++i)
                {
                    var scriptStringSP = ScriptStringListSP.GetArrayElementAtIndex(i)?.FindPropertyRelative(nameof(ScriptString.scriptString));
                    if (scriptStringSP != null)
                    {
                        scriptStringSP.stringValue = stringArray[i].Trim();
                    }
                }
                ScriptStringListMarkDirty(ScriptStringListSP);
            }

            DrawError(errorLines);
        }

        /// <summary>
        /// 脚本字符串列表<see cref="ScriptList.ScriptStringList"/>标记脏
        /// </summary>
        /// <param name="scriptStringListSP"></param>
        protected void ScriptStringListMarkDirty(SerializedProperty scriptStringListSP)
        {
            var list = scriptStringListSP.GetSerializedPropertyValue() as List<ScriptString>;
            if (list != null)
            {
                UICommonFun.DelayCall(() => list.ForEach(e => e.MarkDirty()), nameof(FuncCollectionPropertyDrawer) + "." + nameof(ScriptStringListMarkDirty));
            }
        }

        /// <summary>
        /// 脚本字符串<see cref="ScriptString"/>标记脏
        /// </summary>
        /// <param name="scriptStringSP"></param>
        /// <returns></returns>
        protected ScriptString ScriptStringMarkDirty(SerializedProperty scriptStringSP)
        {
            var scriptString = scriptStringSP.GetSerializedPropertyValue() as ScriptString;
            if (scriptString != null)
            {
                UICommonFun.DelayCall(scriptString.MarkDirty, nameof(FuncCollectionPropertyDrawer) + "." + nameof(ScriptStringMarkDirty));
            }
            return scriptString;
        }

        /// <summary>
        /// 通过脚本字符串获取语法类型
        /// </summary>
        /// <param name="scriptString"></param>
        /// <returns></returns>
        protected EGrammarType GetGrammarTypeByScriptString(string scriptString)
        {
            try
            {
                return ScriptManager.instance.GetGrammarTypeByScriptString(scriptString);
            }
            catch
            {
                return EGrammarType.Common;
            }
        }
    }

    #endregion

    #region 函数集合

    /// <summary>
    /// 函数集合属性绘制器
    /// </summary>
    public abstract class FuncCollectionPropertyDrawer : CustomFunctionPropertyDrawer
    {
        /// <summary>
        /// 获取属性高度
        /// </summary>
        /// <param name="property"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => 0f;

        /// <summary>
        /// 绘制GUI
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            #region 折页标题栏

            var isExpanded = property.isExpanded;
            var isExpandedNew = UICommonFun.Foldout(isExpanded, label, () =>
            {
                if (GUILayout.Button(UICommonOption.Delete, EditorStyles.miniButtonRight, UICommonOption.WH24x16))
                {
                    property.FindPropertyRelative(nameof(CustomFunctionCollection._functions)).arraySize = 0;
                }
            });
            if (isExpandedNew != isExpanded)
            {
                property.isExpanded = isExpandedNew;
            }
            if (!isExpandedNew) return;

            #endregion

            try
            {
                var functionsSP = property.FindPropertyRelative(nameof(CustomFunctionCollection._functions));
                CommonFun.BeginLayout(true, false);
                OnBeforeDrawFunctions(property, functionsSP);
                OnDrawFunctions(property, functionsSP);
                OnAfterDrawFunctions(property, functionsSP);
            }
            finally
            {
                CommonFun.EndLayout();
            }
        }

        /// <summary>
        /// 当绘制函数列表之前
        /// </summary>
        /// <param name="functionsSP"></param>
        protected virtual void OnBeforeDrawFunctions(SerializedProperty funcCollectionSP, SerializedProperty functionsSP) { }

        /// <summary>
        /// 当绘制函数列表
        /// </summary>
        /// <param name="functionsSP"></param>
        protected void OnDrawFunctions(SerializedProperty funcCollectionSP, SerializedProperty functionsSP)
        {
            for (int i = 0; i < functionsSP.arraySize; i++)
            {
                var functionSP = functionsSP.GetArrayElementAtIndex(i);
                if (functionSP == null) continue;

                OnDrawFunction(functionSP);
            }
        }

        /// <summary>
        /// 当绘制函数列表之后
        /// </summary>
        /// <param name="funcCollectionSP"></param>
        /// <param name="functionsSP"></param>
        protected virtual void OnAfterDrawFunctions(SerializedProperty funcCollectionSP, SerializedProperty functionsSP) { }

        /// <summary>
        /// 当绘制函数
        /// </summary>
        /// <param name="functionSP"></param>
        protected virtual void OnDrawFunction(SerializedProperty functionSP) => DrawFunction(functionSP);

        /// <summary>
        /// 绘制函数名与添加
        /// </summary>
        /// <param name="funcCollectionSP"></param>
        /// <param name="functionsSP"></param>
        /// <param name="relativePropertyPath"></param>
        /// <param name="onAdd"></param>
        protected void DrawFunctionNameAndAdd(SerializedProperty funcCollectionSP, SerializedProperty functionsSP, string relativePropertyPath, Action<string> onAdd)
        {
            var functionNameSP = funcCollectionSP.FindPropertyRelative(relativePropertyPath);
            var functionNameData = PropertyData.GetPropertyData(functionNameSP);
            var functionName = functionNameSP.stringValue;

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(functionNameData.trLabel, GUILayout.Width(60));
            var functionNameNew = EditorGUILayout.TextField(functionName);
            if (functionNameNew != functionName)
            {
                functionNameNew = VariableHelper.Format(functionNameNew);
                if (functionNameNew != functionName)
                {
                    functionNameSP.stringValue = functionNameNew;
                }
            }
            if (GUILayout.Button(UICommonOption.Insert, UICommonOption.WH24x16) && !string.IsNullOrEmpty(functionNameNew))
            {
                onAdd?.Invoke(functionNameNew);
            }
            EditorGUILayout.EndHorizontal();
        }
    }

    /// <summary>
    /// 自定义函数集合属性绘制器
    /// </summary>
    [CustomPropertyDrawer(typeof(CustomFunctionCollection), true)]
    public class CustomFunctionCollectionPropertyDrawer : FuncCollectionPropertyDrawer
    {
        /// <summary>
        /// 绘制GUI
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            base.OnGUI(position, property, label);

            //运行时自定义函数列表
            if (!Application.isPlaying) return;
            var funcCollection = property.GetSerializedPropertyValue() as CustomFunctionCollection;
            DrawRTFunctions(funcCollection);
        }

        /// <summary>
        /// 当绘制函数
        /// </summary>
        /// <param name="functionSP"></param>
        protected override void OnDrawFunction(SerializedProperty functionSP)
        {
            if (ScriptOption.IsDefaultCustomFunction(functionSP.FindPropertyRelative(nameof(Function.name)).stringValue)) return;
            base.OnDrawFunction(functionSP);
        }

        /// <summary>
        /// 函数运行时展开
        /// </summary>
        public bool functionRTExpand = false;

        private void DrawRTFunctions(CustomFunctionCollection funcCollection)
        {
            functionRTExpand = UICommonFun.Foldout(functionRTExpand, "运行时自定义函数列表(只读)");
            if (!functionRTExpand) return;

            CommonFun.BeginLayout(true, false);
            foreach (var kv in funcCollection.funcDictionary)
            {
                if (ScriptOption.IsDefaultCustomFunction(kv.Key)) continue;

                var name = kv.Key;
                var function = kv.Value;
                function.Expand = UICommonFun.Foldout(function.Expand, new GUIContent(name), () =>
                {
                    function.Active = EditorGUILayout.ToggleLeft(typeof(Function).TrLabel(nameof(Function.Active)), function.Active, UICommonOption.Height18, UICommonOption.Width60);

                    //执行 按钮
                    if (GUILayout.Button(UICommonOption.Run, EditorStyles.miniButtonRight, UICommonOption.WH24x16))
                    {
                        if (UICommonFun.TryEnsureScriptManagerWork())
                        {
                            ScriptManager.Instance().ExecuteScripts(function);
                        }
                    }
                });
                if (function.Expand)
                {
                    CommonFun.BeginLayout();
                    foreach (ScriptString s in function.ScriptStringList)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.TextField(s.scriptString, UICommonOption.Height18);
                        if (GUILayout.Button(UICommonOption.Run, EditorStyles.miniButtonRight, UICommonOption.WH24x16))
                        {
                            if (UICommonFun.TryEnsureScriptManagerWork())
                            {
                                ScriptManager.Instance().ExecuteScript(s.scriptString);
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    CommonFun.EndLayout();
                }
            }
            CommonFun.EndLayout();
        }

        /// <summary>
        /// 当绘制函数列表之前
        /// </summary>
        /// <param name="funcCollectionSP"></param>
        /// <param name="functionsSP"></param>
        protected override void OnBeforeDrawFunctions(SerializedProperty funcCollectionSP, SerializedProperty functionsSP)
        {
            //base.OnBeforeDrawFunctions(functionsSP);
            DrawFunctionNameAndAdd(funcCollectionSP, functionsSP, nameof(CustomFunctionCollection._functionName), functionNameNew => AddFunction(functionsSP, functionNameNew, out _));
        }

        /// <summary>
        /// 添加函数
        /// </summary>
        /// <param name="functionsSP"></param>
        /// <param name="functionName"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        protected SerializedProperty AddFunction(SerializedProperty functionsSP, string functionName, out int index)
        {
            //已经存在同名函数
            var functionSP = functionsSP.GetArrayElement(out index, sp => sp.FindPropertyRelative(nameof(Function.name)).stringValue == functionName);
            if (functionSP != null) return functionSP;

            //增加新的函数
            functionSP = functionsSP.AddArrayElement(out index);
            functionSP.FindPropertyRelative(nameof(Function.name)).stringValue = functionName;
            functionSP.FindPropertyRelative(nameof(Function.Enable)).boolValue = true;
            functionSP.FindPropertyRelative(nameof(Function.Active)).boolValue = true;
            return functionSP;
        }
    }

    /// <summary>
    /// 枚举函数集合属性绘制器
    /// </summary>
    [CustomPropertyDrawer(typeof(EnumFunctionCollection<,>), true)]
    public class EnumFuncCollectionPropertyDrawer : FuncCollectionPropertyDrawer
    {
        public Type enumType { get; private set; }

        Enum GetEnumValue(int enumValueInt) => EnumValueCache.Get(enumType, enumValueInt.ToString(), EEnumStringType.UnderlyingType);

        /// <summary>
        /// 当绘制函数列表之前
        /// </summary>
        /// <param name="funcCollectionSP"></param>
        /// <param name="functionsSP"></param>
        protected override void OnBeforeDrawFunctions(SerializedProperty funcCollectionSP, SerializedProperty functionsSP)
        {
            //base.OnBeforeDrawFunctions(functionsSP);
            var enumValueSP = funcCollectionSP.FindPropertyRelative(nameof(UserDefineScriptEventFunctionCollection._enumValue));
            var enumValueData = PropertyData.GetPropertyData(enumValueSP);
            if (enumType == null) enumType = enumValueData.fieldInfo.FieldType;

            var enumValueInt = enumValueSP.intValue;

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(enumValueData.trLabel, GUILayout.Width(60));
            EditorGUI.BeginChangeCheck();
            var enumValueNew = UICommonFun.EnumPopup(GetEnumValue(enumValueInt), GUILayout.Height(20));
            if (EditorGUI.EndChangeCheck())
            {
                enumValueSP.intValue = (int)(object)enumValueNew;
            }
            if (GUILayout.Button(UICommonOption.Insert, UICommonOption.WH24x16))
            {
                AddFunction(functionsSP, (int)(object)enumValueNew, out _);
            }
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 添加函数
        /// </summary>
        /// <param name="functionsSP"></param>
        /// <param name="enumValueInt"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        protected SerializedProperty AddFunction(SerializedProperty functionsSP, int enumValueInt, out int index)
        {
            //已经存在同名函数
            var functionSP = functionsSP.GetArrayElement(out index, sp => sp.FindPropertyRelative(nameof(UserDefineScriptEventFunction.type)).intValue == enumValueInt);
            if (functionSP == null)
            {
                //增加新的函数
                functionSP = functionsSP.AddArrayElement(out index);
            } 

            functionSP.FindPropertyRelative(nameof(Function.name)).stringValue = MultiLanguageHelper.TrLabel(GetEnumValue(enumValueInt))?.text ?? enumValueInt.ToString();
            functionSP.FindPropertyRelative(nameof(EnumFunction<EBool>.type)).intValue = enumValueInt;
            functionSP.FindPropertyRelative(nameof(Function.Enable)).boolValue = true;
            functionSP.FindPropertyRelative(nameof(Function.Active)).boolValue = true;

            //排序
            functionsSP.ArrayElementSort((x, y) => x.serializedProperty.FindPropertyRelative(nameof(UserDefineScriptEventFunction.type)).intValue - y.serializedProperty.FindPropertyRelative(nameof(UserDefineScriptEventFunction.type)).intValue);
            return functionSP;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="functionSP"></param>
        /// <returns></returns>
        public override GUIContent GetFunctionNameContent(SerializedProperty functionSP) => GetEnumValue(functionSP.FindPropertyRelative(nameof(EnumFunction<EBool>.type)).intValue).TrLabel();

        /// <summary>
        /// 当绘制函数
        /// </summary>
        /// <param name="functionSP"></param>
        protected override void OnDrawFunction(SerializedProperty functionSP)
        {
            //如果禁用，则不绘制
            if (!functionSP.FindPropertyRelative(nameof(Function.Enable)).boolValue) return;
            base.OnDrawFunction(functionSP);
        }

        Dictionary<int, Texture2D> functionIcons = new Dictionary<int, Texture2D>();

        /// <summary>
        /// 获取函数图标
        /// </summary>
        /// <param name="functionSP"></param>
        /// <returns></returns>
        public override Texture2D GetFunctionIcon(SerializedProperty functionSP)
        {
            var enumValueSP = functionSP.FindPropertyRelative(nameof(UserDefineScriptEventFunction.type));
            if (functionIcons.TryGetValue(enumValueSP.intValue, out var icon) && icon)
            {
                return icon;
            }

            var enumValueData = PropertyData.GetPropertyData(enumValueSP);
            var enumValue = EnumValueCache.Get(enumValueData.fieldInfo.FieldType, enumValueSP.intValue.ToString(), EEnumStringType.UnderlyingType);
            var fieldInfo = EnumFieldInfoCache.GetFieldInfo(enumValue);

            if (AttributeCache<XCSJ.Attributes.IconAttribute>.Exist(fieldInfo))
            {
                icon = IconHelper.GetIconInLib(fieldInfo);
            }
            if (!icon)
            {
                icon = base.GetFunctionIcon(functionSP);
            }
            functionIcons[enumValueSP.intValue] = icon;
            return icon;
        }
    }

    #endregion
}
