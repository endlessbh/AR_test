using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Caches;
using XCSJ.Collections;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.CNScripts;
using XCSJ.EditorExtension.Base;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.CNScripts;
using XCSJ.Scripts;

namespace XCSJ.EditorExtension.CNScripts.ScriptParamDrawers
{
    /// <summary>
    /// 标准化字符串 脚本参数绘制器
    /// </summary>
    [ScriptParamType(EParamType.StandardString)]
    public class StandardString_ScriptParamDrawer : StringScriptParamDrawer
    {
        /// <summary>
        /// 当绘制值
        /// </summary>
        public override void OnDrawValue()
        {
            //base.OnDrawValue();
            EditorGUI.indentLevel = 2;
            paramObject = VariableHelper.Format(EditorGUILayout.TextField(VariableHelper.Format(paramObject as string)));
        }
    }

    /// <summary>
    /// 字符串 脚本参数绘制器
    /// </summary>
    [ScriptParamType(EParamType.String)]
    public class String_ScriptParamDrawer : StringScriptParamDrawer
    {
        /// <summary>
        /// 当绘制值
        /// </summary>
        public override void OnDrawValue()
        {
            //base.OnDrawValue();
            EditorGUI.indentLevel = 2;
            paramObject = EditorGUILayout.TextArea(paramObject as string);
        }
    }

    /// <summary>
    /// 全局变量名类型 脚本参数绘制器
    /// </summary>
    [ScriptParamType(EParamType.GlobalVariableName)]
    public class GlobalVariableName_ScriptParamDrawer : StringScriptParamDrawer
    {
        /// <summary>
        /// 当绘制值
        /// </summary>
        public override void OnDrawValue()
        {
            //base.OnDrawValue();
            EditorGUI.indentLevel = 2;
            paramObject = UICommonFun.Popup(true, paramObject as string, scriptManager.varCollection.GetVarNames());
        }
    }

    /// <summary>
    /// 数组 脚本参数绘制器
    /// </summary>
    [ScriptParamType(EParamType.Array)]
    public class Array_ScriptParamDrawer : StringScriptParamDrawer
    {
        /// <summary>
        /// 当绘制值
        /// </summary>
        public override void OnDrawValue()
        {
            //base.OnDrawValue();
            EditorGUI.indentLevel = 2;
            var names = scriptManager.varCollection.varDictionary.WhereCast(kv => kv.Value.hierarchyVar.varType == EVarType.Array, kv => kv.Value.name).ToArray();
            paramObject = UICommonFun.Popup(true, paramObject as string, names);
        }
    }

    /// <summary>
    /// 字典 脚本参数绘制器
    /// </summary>
    [ScriptParamType(EParamType.Dictionary)]
    public class Dictionary_ScriptParamDrawer : StringScriptParamDrawer
    {
        /// <summary>
        /// 当绘制值
        /// </summary>
        public override void OnDrawValue()
        {
            //base.OnDrawValue();
            EditorGUI.indentLevel = 2;
            var names = scriptManager.varCollection.varDictionary.WhereCast(kv => kv.Value.hierarchyVar.varType == EVarType.Dictionary, kv => kv.Value.name).ToArray();
            paramObject = UICommonFun.Popup(true, paramObject as string, names);
        }
    }

    /// <summary>
    /// 变量 脚本参数绘制器
    /// </summary>
    [ScriptParamType(EParamType.Variable)]
    public class Variable_ScriptParamDrawer : StringScriptParamDrawer
    {
        /// <summary>
        /// 当绘制值
        /// </summary>
        public override void OnDrawValue()
        {
            //base.OnDrawValue();
            EditorGUI.indentLevel = 2;
            paramObject = VariableHelper.Format(EditorGUILayout.TextField(paramObject as string));
        }
    }

    /// <summary>
    /// unity资源对象类型 脚本参数绘制器
    /// </summary>
    [ScriptParamType(EParamType.UnityAssetObjectType)]
    public class UnityAssetObjectType_ScriptParamDrawer : EnumScriptParamDrawer<EUnityAssetObjectType> { }

    /// <summary>
    /// unity资源对象 脚本参数绘制器
    /// </summary>
    [ScriptParamType(EParamType.UnityAssetObject)]
    public class UnityAssetObject_ScriptParamDrawer : UnityObjectScriptParamDrawer<UnityEngine.Object>
    {
        Type initLimitType = null;

        Type _limitType;

        Type limitType
        {
            get => _limitType;
            set
            {
                _limitType = value;
                unityAssetObjectType = UnityAssetObjectManager.GetUnityAssetObjectType(value);
                if (paramTypeObject && !_limitType.IsAssignableFrom(paramTypeObject.GetType()))
                {
                    paramTypeObject = null;
                }
            }
        }

        EUnityAssetObjectType unityAssetObjectType = EUnityAssetObjectType.Object;
        string assetName = "";

        /// <summary>
        /// 当初始化
        /// </summary>
        public override void OnInit()
        {
            base.OnInit();

            var unityAssetObjectManager = UnityAssetObjectManager.instance;
            if (unityAssetObjectManager.GetUnityAssetObjectType(param.limitType?.Name) is Type tmpType)
            {
                initLimitType = tmpType;
                limitType = initLimitType;
            }
            else
            {
                limitType = UnityAssetObjectManager.GetUnityAssetObjectType(unityAssetObjectType);
            }
        }

        public override void OnParamStringChanged(string paramString)
        {
            base.OnParamStringChanged(paramString);
            var paramTypeObject = this.paramTypeObject;
            if (paramTypeObject)
            {
                assetName = paramTypeObject.name;
                ScriptViewerWindow.CallHasObjectUsed(paramTypeObject);
                if (initLimitType == null)
                {
                    limitType = paramTypeObject.GetType();
                }
                else
                {
                    limitType = initLimitType;
                }
            }
            else
            {
                limitType = UnityAssetObjectManager.GetUnityAssetObjectType(unityAssetObjectType);
            }
        }

        /// <summary>
        /// 当绘制值
        /// </summary>
        public override void OnDrawValue()
        {
            //base.OnDrawValue();

            EditorGUI.indentLevel = 2;
            var unityAssetObjectManager = UnityAssetObjectManager.instance;
            try
            {
                EditorGUILayout.BeginHorizontal();

                //无初始类型限定，则绘制类型选择
                if (initLimitType == null)
                {
                    var unityAssetObjectTypeNew = (EUnityAssetObjectType)UICommonFun.EnumPopup(unityAssetObjectType);
                    if (unityAssetObjectTypeNew != unityAssetObjectType)
                    {
                        limitType = UnityAssetObjectManager.GetUnityAssetObjectType(unityAssetObjectTypeNew);
                    }
                }

                //绘制对象选择
                var paramTypeObjectNew = EditorGUILayout.ObjectField(paramTypeObject, limitType, true);
                TrySetNewUnityAssetObject(paramTypeObjectNew);

                //从缓冲区直接选择
                var newName = UICommonFun.Popup(assetName, unityAssetObjectManager.GetAssetNames(limitType), false);
                TrySetNewUnityAssetObjectName(newName);

                //名称控件
                newName = assetName = EditorGUILayout.TextField(assetName);
                TrySetNewUnityAssetObjectName(newName);
            }
            catch// (Exception ex)
            {
                //throw ex;
            }
            finally
            {
                EditorGUILayout.EndHorizontal();
            }
        }

        private void TrySetNewUnityAssetObject(UnityEngine.Object newUnityAssetObject)
        {
            if (newUnityAssetObject != paramTypeObject)
            {
                paramTypeObject = newUnityAssetObject;
                if (newUnityAssetObject)
                {
                    assetName = newUnityAssetObject.name;
                    if (limitType != null && !limitType.IsAssignableFrom(newUnityAssetObject.GetType()))
                    {
                        paramTypeObject = null;
                    }
                    else
                    {
                        ScriptViewerWindow.CallHasObjectUsed(paramTypeObject);
                    }
                }
                else
                {
                    assetName = "";
                }
            }
        }

        private void TrySetNewUnityAssetObjectName(string newUnityAssetObjectName)
        {
            if (newUnityAssetObjectName != assetName)
            {
                assetName = newUnityAssetObjectName;
                paramTypeObject = UnityAssetObjectManager.instance.GetUnityAssetObject(limitType?.Name, assetName);
                if (paramTypeObject)
                {
                    ScriptViewerWindow.CallHasObjectUsed(paramTypeObject);
                }
            }
        }
    }

    /// <summary>
    /// 文件路径 脚本参数绘制器
    /// </summary>
    [ScriptParamType(EParamType.File)]
    [ScriptParamType(EParamType.SaveFile)]
    [ScriptParamType(EParamType.OpenFolder)]
    [ScriptParamType(EParamType.SaveFolder)]
    public class FilePath_ScriptParamDrawer : StringScriptParamDrawer
    {
        /// <summary>
        /// 当绘制值
        /// </summary>
        public override void OnDrawValue()
        {
            //base.OnDrawValue();
            EditorGUI.indentLevel = 2;
            string path = paramObject as string;
            if (string.IsNullOrEmpty(path))
            {
                path = Application.streamingAssetsPath;
            }
            EditorGUILayout.BeginHorizontal();
            paramObject = EditorGUILayout.TextField(path);
            if (GUILayout.Button("路径", GUILayout.Width(60)))
            {
                string retpath = "";
                switch (paramTypeEnum)
                {
                    case EParamType.File:
                        {
                            retpath = EditorUtility.OpenFilePanelWithFilters("打开文件", System.IO.Path.GetDirectoryName(path.Trim()), UICommonFun.GetFileFilters());
                            break;
                        }
                    case EParamType.SaveFile:
                        {
                            retpath = EditorUtility.SaveFilePanel("保存文件", System.IO.Path.GetDirectoryName(path.Trim()), "", "*");
                            break;
                        }
                    case EParamType.OpenFolder:
                        {
                            retpath = EditorUtility.OpenFolderPanel("打开文件夹", System.IO.Path.GetDirectoryName(path.Trim()), "");
                            break;
                        }
                    case EParamType.SaveFolder:
                        {
                            retpath = EditorUtility.SaveFolderPanel("保存文件夹", System.IO.Path.GetDirectoryName(path.Trim()), "");
                            break;
                        }
                }
                CommonFun.FocusControl();
                paramObject = retpath;
            }
            EditorGUILayout.EndHorizontal();
        }
    }

    /// <summary>
    /// 游戏对象 脚本参数绘制器
    /// </summary>
    [ScriptParamType(EParamType.GameObject)]
    public class GameObject_ScriptParamDrawer : UnityObjectScriptParamDrawer<GameObject>
    {
        /// <summary>
        /// 当绘制值
        /// </summary>
        public override void OnDrawValue()
        {
            //base.OnDrawValue();
            try
            {
                EditorGUI.indentLevel = 2;
                GameObject go = paramObject as GameObject;
                if (go && param.limitType != null && ComponentManager.ContainsType(param.limitType))
                {
                    if (!go.GetComponent(param.limitType)) go = null;
                }
                paramObject = EditorGUILayout.ObjectField(go, typeof(GameObject), true) as GameObject;
            }
            catch (ExitGUIException) { }
        }
    }

    /// <summary>
    /// 组件类型 脚本参数绘制器
    /// </summary>
    [ScriptParamType(EParamType.ComponentType)]
    public class ComponentType_ScriptParamDrawer : ScriptParamDrawer<Type>
    {
        string[] componentTypeNames;
        int typeIndex = 0;

        /// <summary>
        /// 当初始化
        /// </summary>
        public override void OnInit()
        {
            base.OnInit();
            componentTypeNames = ComponentManager.GetNames();
            if (param.defaultObject is Type type)
            {
                paramTypeObject = type;
            }
            else if (param.defaultObject is string typeFullName)
            {
                paramTypeObject = ComponentManager.GetType(ComponentManager.TypeFullNameToKey(typeFullName));
            }
            typeIndex = componentTypeNames.IndexOf(ComponentManager.TypeToKey(paramTypeObject));
        }

        /// <summary>
        /// 当参数字符串变更后
        /// </summary>
        /// <param name="paramString"></param>
        public override void OnParamStringChanged(string paramString)
        {
            base.OnParamStringChanged(paramString);
            typeIndex = ComponentManager.GetIndex(paramTypeObject);
        }

        /// <summary>
        /// 当绘制值
        /// </summary>
        public override void OnDrawValue()
        {
            //base.OnDrawValue();
            try
            {
                EditorGUI.indentLevel = 2;
                var newIndex = EditorGUILayout.Popup(typeIndex, componentTypeNames);
                if (newIndex != typeIndex)
                {
                    typeIndex = newIndex;
                    paramTypeObject = ComponentManager.GetType(componentTypeNames[newIndex]);
                }
            }
            catch (ExitGUIException) { }
        }
    }

    /// <summary>
    /// 游戏对象组件 脚本参数绘制器
    /// </summary>
    [ScriptParamType(EParamType.GameObjectComponent)]
    public class GameObjectComponent_ScriptParamDrawer : UnityObjectScriptParamDrawer<Component>
    {
        GameObject gameObject;
        Type initLimitType;
        Type limitType;

        /// <summary>
        /// 当初始化
        /// </summary>
        public override void OnInit()
        {
            base.OnInit();
            if (param.limitType != null && ComponentManager.ContainsType(param.limitType))
            {
                initLimitType = param.limitType;
                limitType = initLimitType;
            }
            else
            {
                limitType = typeof(Transform);
            }
        }

        /// <summary>
        /// 当参数字符串变更后
        /// </summary>
        /// <param name="paramString"></param>
        public override void OnParamStringChanged(string paramString)
        {
            base.OnParamStringChanged(paramString);
            var component = paramTypeObject;
            if (component)
            {
                gameObject = component.gameObject;
                limitType = component.GetType();
            }
            else
            {
                gameObject = default;
                limitType = limitType ?? typeof(Transform);
            }
        }

        /// <summary>
        /// 当绘制值
        /// </summary>
        public override void OnDrawValue()
        {
            //base.OnDrawValue();
            try
            {
                EditorGUI.indentLevel = 2;
                EditorGUILayout.BeginHorizontal();

                if (initLimitType != null)
                {
                    paramTypeObject = EditorGUILayout.ObjectField(paramTypeObject, initLimitType, true) as Component;
                }
                else
                {
                    //绘制游戏对象选择
                    var gameObjectNew = EditorGUILayout.ObjectField(gameObject, typeof(GameObject), true) as GameObject;
                    if (gameObjectNew != gameObject)
                    {
                        limitType = paramTypeObject ? paramTypeObject.GetType() : typeof(Transform);
                        gameObject = gameObjectNew;
                        paramTypeObject = gameObject ? gameObject.GetComponent(limitType) : default;
                    }

                    //绘制游戏对象上组件类型选择
                    if (gameObject)
                    {
                        var array = gameObject.GetComponents(typeof(Component)).Cast(c => ComponentManager.TypeToKey(c.GetType())).Distinct().ToArray();
                        var index = Array.IndexOf(array, ComponentManager.TypeToKey(limitType));
                        var indexNew = EditorGUILayout.Popup(index, array);
                        if (indexNew != index)
                        {
                            paramTypeObject = gameObject.GetComponent(ComponentManager.GetType(array[indexNew]));
                            limitType = paramTypeObject ? paramTypeObject.GetType() : typeof(Transform);
                        }
                    }
                    else
                    {
                        EditorGUILayout.Popup(-1, Empty<string>.Array);
                    }
                }
            }
            catch /*(Exception ex)*/
            {
                //throw ex;
            }
            finally
            {
                EditorGUILayout.EndHorizontal();
            }
        }
    }

    /// <summary>
    /// 组件游戏对象 脚本参数绘制器
    /// </summary>
    [ScriptParamType(EParamType.ComponentGameObject)]
    public class ComponentGameObject_ScriptParamDrawer : UnityObjectScriptParamDrawer<Component>
    {
        string[] componentTypeNames;
        Type initLimitType;
        Type limitType;

        /// <summary>
        /// 当初始化
        /// </summary>
        public override void OnInit()
        {
            base.OnInit();
            componentTypeNames = ComponentManager.GetNames();
            if (param.limitType != null && ComponentManager.ContainsType(param.limitType))
            {
                initLimitType = param.limitType;
                limitType = initLimitType;
            }
            else
            {
                limitType = typeof(Transform);
            }
        }

        /// <summary>
        /// 当参数字符串变更后
        /// </summary>
        /// <param name="paramString"></param>
        public override void OnParamStringChanged(string paramString)
        {
            base.OnParamStringChanged(paramString);
            var component = paramTypeObject;
            if (component)
            {
                limitType = component.GetType();
            }
            else
            {
                limitType = limitType ?? typeof(Transform);
            }
        }

        /// <summary>
        /// 当绘制值
        /// </summary>
        public override void OnDrawValue()
        {
            //base.OnDrawValue();
            try
            {
                EditorGUI.indentLevel = 2;
                EditorGUILayout.BeginHorizontal();
                if (initLimitType != null)
                {
                    paramTypeObject = EditorGUILayout.ObjectField(paramTypeObject, initLimitType, true) as Component;
                }
                else
                {
                    //绘制组件类型选择
                    var key = ComponentManager.TypeToKey(limitType);
                    var index = componentTypeNames.IndexOf(key);
                    var indexNew = EditorGUILayout.Popup(index, componentTypeNames);
                    if(indexNew != index)
                    {
                        limitType = ComponentManager.GetType(componentTypeNames[indexNew]);
                        if (paramTypeObject)
                        {
                            paramTypeObject = paramTypeObject.GetComponent(limitType);
                        }
                    }

                    //绘制组件选择
                    paramTypeObject = EditorGUILayout.ObjectField(paramTypeObject, limitType, true) as Component;
                }
            }
            catch (ExitGUIException) { }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                EditorGUILayout.EndHorizontal();
            }
        }
    }

    /// <summary>
    /// 游戏对象脚本事件 脚本参数绘制器
    /// </summary>
    [ScriptParamType(EParamType.GameObjectSciptEvent)]
    public class GameObjectSciptEvent_ScriptParamDrawer : UnityObjectScriptParamDrawer<Component>
    {
        string[] eventNames;
        Type initLimitType;
        Type limitType;

        /// <summary>
        /// 当初始化
        /// </summary>
        public override void OnInit()
        {
            base.OnInit();
            eventNames = ScriptEventManager.GetEventNames();
            if (param.limitType != null && ScriptEventManager.ContainsType(param.limitType))
            {
                initLimitType = param.limitType;
                limitType = initLimitType;
            }
            else
            {
                limitType = ScriptEventManager.GetScriptEventType(eventNames[0]);
            }
        }

        /// <summary>
        /// 当参数字符串变更后
        /// </summary>
        /// <param name="paramString"></param>
        public override void OnParamStringChanged(string paramString)
        {
            base.OnParamStringChanged(paramString);
            var component = paramTypeObject;
            if (component)
            {
                limitType = component.GetType();
            }
            else
            {
                limitType = limitType ?? ScriptEventManager.GetScriptEventType(eventNames[0]);
            }
        }

        /// <summary>
        /// 当绘制值
        /// </summary>
        public override void OnDrawValue()
        {
            //base.OnDrawValue();
            try
            {
                EditorGUI.indentLevel = 2;
                EditorGUILayout.BeginHorizontal();

                if (initLimitType != null)
                {
                    paramTypeObject = EditorGUILayout.ObjectField(paramTypeObject, initLimitType, true) as Component;
                }
                else
                {
                    //绘制脚本类型事件选择
                    var index = eventNames.IndexOf(ScriptEventManager.TypeToKey(limitType));
                    int indexNew = EditorGUILayout.Popup(index, eventNames);
                    if (indexNew != index)
                    {
                        limitType = ScriptEventManager.GetScriptEventType(eventNames[indexNew]);
                    }

                    //绘制组件对象选择
                    paramTypeObject = EditorGUILayout.ObjectField(paramTypeObject, limitType, true) as Component;
                }
            }
            catch //(Exception ex)
            {
               // throw ex;
            }
            finally
            {
                EditorGUILayout.EndHorizontal();
            }
        }
    }

    /// <summary>
    /// 游戏对象脚本事件函数 脚本参数绘制器
    /// </summary>
    [ScriptParamType(EParamType.GameObjectSciptEventFunction)]
    public class GameObjectSciptEventFunction_ScriptParamDrawer : ScriptParamDrawer<Function>
    {
        Component component;
        string[] eventNames;
        Type initLimitType;
        Type limitType;

        string funcName = "";

        /// <summary>
        /// 当初始化
        /// </summary>
        public override void OnInit()
        {
            base.OnInit();
            eventNames = ScriptEventManager.GetEventNames();
            if (param.limitType != null && ScriptEventManager.ContainsType(param.limitType))
            {
                initLimitType = param.limitType;
                limitType = initLimitType;
            }
            else
            {
                limitType = ScriptEventManager.GetScriptEventType(eventNames[0]);
            }

            if (param.defaultObject is string str && !str.Contains("."))
            {
                funcName = str;
            }
        }

        /// <summary>
        /// 当参数字符串变更后
        /// </summary>
        /// <param name="paramString"></param>
        public override void OnParamStringChanged(string paramString)
        {
            base.OnParamStringChanged(paramString);
            var function = paramTypeObject;
            if (function != null)
            {
                component = function.funcCollectionHost as Component;
                limitType = component ? component.GetType() : limitType;
                funcName = function.GetName();
            }
            else
            {
                component = default;
                limitType = limitType ?? ScriptEventManager.GetScriptEventType(eventNames[0]);
            }
        }

        /// <summary>
        /// 当绘制值
        /// </summary>
        public override void OnDrawValue()
        {
            //base.OnDrawValue();
            try
            {
                EditorGUI.indentLevel = 2;
                EditorGUILayout.BeginHorizontal();

                if (initLimitType != null)
                {
                    component = EditorGUILayout.ObjectField(component, initLimitType, true) as Component;
                }
                else
                {
                    //绘制脚本类型事件选择
                    var index = eventNames.IndexOf(ScriptEventManager.TypeToKey(limitType));
                    int indexNew = EditorGUILayout.Popup(index, eventNames);
                    if (indexNew != index)
                    {
                        limitType = ScriptEventManager.GetScriptEventType(eventNames[indexNew]);
                    }

                    //绘制组件对象选择
                    component = EditorGUILayout.ObjectField(component, limitType, true) as Component;
                }

                //绘制脚本事件函数选择
                var scriptEvent = component ? component as IScriptEvent : default;
                if (scriptEvent != null)
                {
                    var names = scriptEvent.funcCollection.GetFunctionNames();
                    var index = names.IndexOf(funcName);
                    var indexNew = EditorGUILayout.Popup(index, names);
                    if (indexNew != index)
                    {
                        funcName = names[indexNew];
                    }
                    paramTypeObject = scriptEvent.funcCollection.GetFunction(funcName);
                }
                else
                {
                    EditorGUILayout.Popup(-1, Empty<string>.Array);
                    paramTypeObject = default;
                }
            }
            catch// (Exception ex)
            {
                //throw ex;
            }
            finally
            {
                EditorGUILayout.EndHorizontal();
            }
        }
    }

    /// <summary>
    /// 游戏对象脚本事件变量 脚本参数绘制器
    /// </summary>
    [ScriptParamType(EParamType.GameObjectSciptEventVariable)]
    public class GameObjectSciptEventVariable_ScriptParamDrawer : ScriptParamDrawer<GameObjectScriptEventVariableData>
    {
        Component component { get => paramTypeObject.component; set => paramTypeObject.component = value; }
        string varName { get => paramTypeObject.varName; set => paramTypeObject.varName = VariableHelper.Format(value); }

        string[] eventNames;
        Type initLimitType;
        Type limitType;

        /// <summary>
        /// 默认参数对象
        /// </summary>
        /// <returns></returns>
        public override object DefaultParamObject() => new GameObjectScriptEventVariableData();

        /// <summary>
        /// 当初始化
        /// </summary>
        public override void OnInit()
        {
            base.OnInit();
            eventNames = ScriptEventManager.GetEventNames();
            if (param.limitType != null && ScriptEventManager.ContainsType(param.limitType))
            {
                initLimitType = param.limitType;
                limitType = initLimitType;
            }
            else
            {
                limitType = ScriptEventManager.GetScriptEventType(eventNames[0]);
            }

            if (param.defaultObject is string str && !string.IsNullOrEmpty(str))
            {
                if (scriptParam.StringToParamObject(str) is GameObjectScriptEventVariableData data)
                {
                    paramTypeObject = data;
                }
                else if (!str.Contains("."))
                {
                    paramTypeObject.varName = str;
                }
            }
        }

        /// <summary>
        /// 当参数字符串变更后
        /// </summary>
        /// <param name="paramString"></param>
        public override void OnParamStringChanged(string paramString)
        {
            base.OnParamStringChanged(paramString);
            var data = paramTypeObject;
            if (data != null)
            {
                limitType = component ? component.GetType() : limitType;
            }
            else
            {
                limitType = limitType ?? ScriptEventManager.GetScriptEventType(eventNames[0]);
                paramTypeObject = new GameObjectScriptEventVariableData();
            }
        }

        /// <summary>
        /// 当绘制值
        /// </summary>
        public override void OnDrawValue()
        {
            //base.OnDrawValue();
            try
            {
                EditorGUI.indentLevel = 2;
                EditorGUILayout.BeginHorizontal();

                if (initLimitType != null)
                {
                    component = EditorGUILayout.ObjectField(component, initLimitType, true) as Component;
                }
                else
                {
                    //绘制脚本类型事件选择
                    var index = eventNames.IndexOf(ScriptEventManager.TypeToKey(limitType));
                    int indexNew = EditorGUILayout.Popup(index, eventNames);
                    if (indexNew != index)
                    {
                        limitType = ScriptEventManager.GetScriptEventType(eventNames[indexNew]);
                    }

                    //绘制组件对象选择
                    component = EditorGUILayout.ObjectField(component, limitType, true) as Component;
                }

                //绘制脚本事件变量选择
                var scriptEvent = component ? component as IScriptEvent : default;
                if (scriptEvent != null)
                {
                    var names = scriptEvent.varCollection.GetVarNames();
                    var index = names.IndexOf(varName);
                    var indexNew = EditorGUILayout.Popup(index, names);
                    if (indexNew != index)
                    {
                        varName = names[indexNew];
                    }
                }
                else
                {
                    EditorGUILayout.Popup(-1, Empty<string>.Array);
                }

                //绘制脚本事件变量文本框
                varName = EditorGUILayout.TextField(varName);
            }
            catch// (Exception ex)
            {
                //throw ex;
            }
            finally
            {
                EditorGUILayout.EndHorizontal();
            }
        }
    }

    /// <summary>
    /// 颜色 脚本参数绘制器
    /// </summary>
    [ScriptParamType(EParamType.Color)]
    public class Color_ScriptParamDrawer : ScriptParamDrawer<Color>
    {
        /// <summary>
        /// 当绘制值
        /// </summary>
        public override void OnDrawValue()
        {
            //base.OnDrawValue();
            EditorGUI.indentLevel = 2;
            try
            {
                paramObject = EditorGUILayout.ColorField((Color)(paramObject));
            }
            //catch (ExitGUIException) { }
            catch// (Exception colorEx)
            {
                //EditorGUILayout.ColorField 始终抛出 UnityEngine.ExitGUIException 异常~
                //查询得知，这个异常是无害的~所以将异常不处理了~
                //网址：http://answers.unity3d.com/questions/385235/editorguilayoutcolorfield-inside-guilayoutwindow-c.html
                //Log.Debug("脚本编辑器窗口 Color Exception: " + colorEx.ToString());
                //throw colorEx;
            }
        }
    }

    /// <summary>
    /// 包围盒 脚本参数绘制器
    /// </summary>
    [ScriptParamType(EParamType.Bounds)]
    public class Bounds_ScriptParamDrawer : ScriptParamDrawer<Bounds>
    {
        /// <summary>
        /// 当绘制值
        /// </summary>
        public override void OnDrawValue()
        {
            //base.OnDrawValue();
            //EditorGUI.indentLevel = 2;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("", GUILayout.Width(10));
            paramObject = EditorGUILayout.BoundsField((Bounds)(paramObject));
            EditorGUILayout.EndHorizontal();
        }
    }

    /// <summary>
    /// 整型包围盒 脚本参数绘制器
    /// </summary>
    [ScriptParamType(EParamType.BoundsInt)]
    public class BoundsInt_ScriptParamDrawer : ScriptParamDrawer<BoundsInt>
    {
        /// <summary>
        /// 当绘制值
        /// </summary>
        public override void OnDrawValue()
        {
            //base.OnDrawValue();
            //EditorGUI.indentLevel = 2;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("", GUILayout.Width(10));
            paramObject = EditorGUILayout.BoundsIntField((BoundsInt)(paramObject));
            EditorGUILayout.EndHorizontal();
        }
    }

    /// <summary>
    /// 脚本事件类型 脚本参数绘制器
    /// </summary>
    [ScriptParamType(EParamType.ScriptEventType)]
    public class ScriptEvent_ScriptParamDrawer : StringScriptParamDrawer
    {
        string[] eventNames;

        /// <summary>
        /// 当初始化
        /// </summary>
        public override void OnInit()
        {
            base.OnInit();
            eventNames = ScriptEventManager.GetEventNames();
            if (param.defaultObject is int i)
            {
                paramTypeObject = eventNames.ElementAtOrDefault(i) ?? "";
            }
            else if (param.defaultObject is Type type && ScriptEventManager.ContainsType(type))
            {
                paramTypeObject = ScriptEventManager.TypeToKey(type);
            }
            else if (param.limitType != null && ScriptEventManager.ContainsType(param.limitType))
            {
                paramTypeObject = ScriptEventManager.TypeToKey(param.limitType);
            }
        }

        /// <summary>
        /// 当绘制值
        /// </summary>
        public override void OnDrawValue()
        {
            //base.OnDrawValue();
            EditorGUI.indentLevel = 2;
            paramTypeObject = UICommonFun.Popup(paramTypeObject, eventNames);
        }
    }

    /// <summary>
    /// 用户自定义函数 脚本参数绘制器
    /// </summary>
    [ScriptParamType(EParamType.UserDefineFun)]
    public class UserDefineFun_ScriptParamDrawer : StringScriptParamDrawer
    {
        /// <summary>
        /// 当初始化
        /// </summary>
        public override void OnInit()
        {
            base.OnInit();
            if(param.defaultObject is int i && scriptManager)
            {
                paramObject = scriptManager.funcCollection.GetFunctionNames().ElementAtOrDefault(i) ?? i.ToString();
            }
        }

        /// <summary>
        /// 当绘制值
        /// </summary>
        public override void OnDrawValue()
        {
            //base.OnDrawValue();
            EditorGUI.indentLevel = 2;
            var names = scriptManager ? scriptManager.funcCollection.GetFunctionNames() : Empty<string>.Array;
            paramObject = UICommonFun.Popup(paramObject as string, names);
        }
    }

    /// <summary>
    /// 矩形 脚本参数绘制器
    /// </summary>
    [ScriptParamType(EParamType.Rect)]
    public class Rect_ScriptParamDrawer : ScriptParamDrawer<Rect>
    {
        /// <summary>
        /// 当绘制值
        /// </summary>
        public override void OnDrawValue()
        {
            //base.OnDrawValue();
            //EditorGUI.indentLevel = 2;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("", GUILayout.Width(10));
            paramObject = EditorGUILayout.RectField((Rect)paramObject);
            EditorGUILayout.EndHorizontal();
        }
    }

    /// <summary>
    /// 整型矩形 脚本参数绘制器
    /// </summary>
    [ScriptParamType(EParamType.RectInt)]
    public class RectInt_ScriptParamDrawer : ScriptParamDrawer<RectInt>
    {
        /// <summary>
        /// 当绘制值
        /// </summary>
        public override void OnDrawValue()
        {
            //base.OnDrawValue();
            EditorGUI.indentLevel = 2;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("", GUILayout.Width(25));
            paramObject = EditorGUILayout.RectIntField((RectInt)paramObject);
            EditorGUILayout.EndHorizontal();
        }
    }

    /// <summary>
    /// 二维向量 脚本参数绘制器
    /// </summary>
    [ScriptParamType(EParamType.Vector2)]
    public class Vector2_ScriptParamDrawer : ScriptParamDrawer<Vector2>
    {
        /// <summary>
        /// 当绘制值
        /// </summary>
        public override void OnDrawValue()
        {
            //base.OnDrawValue();
            //EditorGUI.indentLevel = 2;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("", GUILayout.Width(15));
            paramObject = EditorGUILayout.Vector2Field("", (Vector2)paramObject, GUILayout.Height(16));
            EditorGUILayout.EndHorizontal();
        }
    }

    /// <summary>
    /// 整型二维向量 脚本参数绘制器
    /// </summary>
    [ScriptParamType(EParamType.Vector2Int)]
    public class Vector2Int_ScriptParamDrawer : ScriptParamDrawer<Vector2Int>
    {
        /// <summary>
        /// 当绘制值
        /// </summary>
        public override void OnDrawValue()
        {
            //base.OnDrawValue();
            //EditorGUI.indentLevel = 2;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("", GUILayout.Width(15));
            paramObject = EditorGUILayout.Vector2IntField("", (Vector2Int)paramObject, GUILayout.Height(16));
            EditorGUILayout.EndHorizontal();
        }
    }

    /// <summary>
    /// 最大最小滑动条 脚本参数绘制器
    /// </summary>
    [ScriptParamType(EParamType.MinMaxSlider)]
    public class MinMaxSlider_ScriptParamDrawer : ScriptParamDrawer<Vector2>
    {
        /// <summary>
        /// 当绘制值
        /// </summary>
        public override void OnDrawValue()
        {
            //base.OnDrawValue();
            EditorGUI.indentLevel = 2;
            float[] comboInfo = param.GetLimitArray<float>();
            float leftValue = ScriptOption.scriptParamFloatLeft;
            float rightValue = ScriptOption.scriptParamFloatRight;
            if (comboInfo != null && comboInfo.Length >= 2)
            {
                leftValue = comboInfo[0];
                rightValue = comboInfo[1];
            }
            Vector2 v2 = (Vector2)paramObject;
            UICommonFun.MinMaxSliderLayout(ref v2.x, ref v2.y, leftValue, rightValue, 100);
            paramObject = v2;
        }
    }

    /// <summary>
    /// 三维向量 脚本参数绘制器
    /// </summary>
    [ScriptParamType(EParamType.Vector3)]
    public class Vector3_ScriptParamDrawer : ScriptParamDrawer<Vector3>
    {
        /// <summary>
        /// 当绘制值
        /// </summary>
        public override void OnDrawValue()
        {
            //base.OnDrawValue();
            //EditorGUI.indentLevel = 2;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("", GUILayout.Width(15));
            paramObject = EditorGUILayout.Vector3Field("", (Vector3)paramObject, GUILayout.Height(0));
            EditorGUILayout.EndHorizontal();
        }
    }

    /// <summary>
    /// 整型三维向量 脚本参数绘制器
    /// </summary>
    [ScriptParamType(EParamType.Vector3Int)]
    public class Vector3Int_ScriptParamDrawer : ScriptParamDrawer<Vector3Int>
    {
        /// <summary>
        /// 当绘制值
        /// </summary>
        public override void OnDrawValue()
        {
            //base.OnDrawValue();
            //EditorGUI.indentLevel = 2;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("", GUILayout.Width(15));
            paramObject = EditorGUILayout.Vector3IntField("", (Vector3Int)paramObject, GUILayout.Height(0));
            EditorGUILayout.EndHorizontal();
        }
    }

    /// <summary>
    /// 四维向量 脚本参数绘制器
    /// </summary>
    [ScriptParamType(EParamType.Vector4)]
    public class Vector4_ScriptParamDrawer : ScriptParamDrawer<Vector4>
    {
        /// <summary>
        /// 当绘制值
        /// </summary>
        public override void OnDrawValue()
        {
            //base.OnDrawValue();
            //EditorGUI.indentLevel = 2;
            string[] comboInfo = param.GetLimitArray<string>();//param.tag as string[];
            string remark = CommonFun.ArrayToString<string>(comboInfo);
            if (string.IsNullOrEmpty(remark)) remark = "****";
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("", GUILayout.Width(25));
            paramObject = EditorGUILayout.Vector4Field(remark, (Vector4)paramObject, GUILayout.Height(2));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Separator(); EditorGUILayout.Separator(); EditorGUILayout.Separator();
        }
    }

    /// <summary>
    /// 整型 脚本参数绘制器
    /// </summary>
    [ScriptParamType(EParamType.Int)]
    public class Int_ScriptParamDrawer : ScriptParamDrawer<int>
    {
        /// <summary>
        /// 当绘制值
        /// </summary>
        public override void OnDrawValue()
        {
            //base.OnDrawValue();
            EditorGUI.indentLevel = 2;
            paramObject = EditorGUILayout.IntField((int)paramObject);
        }
    }

    /// <summary>
    /// 整型滑动条 脚本参数绘制器
    /// </summary>
    [ScriptParamType(EParamType.IntSlider)]
    public class IntSlider_ScriptParamDrawer : ScriptParamDrawer<int>
    {
        /// <summary>
        /// 当绘制值
        /// </summary>
        public override void OnDrawValue()
        {
            //base.OnDrawValue();
            EditorGUI.indentLevel = 2;
            int[] comboInfo = param.GetLimitArray<int>();//param.tag as int[];
            int leftValue = ScriptOption.scriptParamIntLeft;
            int rightValue = ScriptOption.scriptParamIntRight;
            if (comboInfo != null && comboInfo.Length >= 2)
            {
                leftValue = comboInfo[0];
                rightValue = comboInfo[1];
            }
            paramObject = EditorGUILayout.IntSlider((int)paramObject, leftValue, rightValue);
        }
    }

    /// <summary>
    /// 整型弹框 脚本参数绘制器
    /// </summary>
    [ScriptParamType(EParamType.IntPopup)]
    public class IntPopup_ScriptParamDrawer : ScriptParamDrawer<int>
    {
        /// <summary>
        /// 当绘制值
        /// </summary>
        public override void OnDrawValue()
        {
            //base.OnDrawValue();
            EditorGUI.indentLevel = 2;
            string[] nameArray = param.GetLimitArray<string>();
            int[] valueArray = param.GetLimitArray<int>();
            if (nameArray.Length < valueArray.Length)
            {
                List<string> nameList = new List<string>(nameArray);
                for (int i = nameArray.Length; i < valueArray.Length; ++i)
                {
                    nameList.Add(valueArray[i].ToString());
                }
                paramObject = EditorGUILayout.IntPopup((int)paramObject, nameList.ToArray(), valueArray);
            }
            else if (nameArray.Length > valueArray.Length)
            {
                List<string> nameList = new List<string>(nameArray);
                for (int i = 0; i < valueArray.Length; ++i)
                {
                    nameList.Add(nameArray[i]);
                }
                paramObject = EditorGUILayout.IntPopup((int)paramObject, nameArray, valueArray);
            }
            else
            {
                paramObject = EditorGUILayout.IntPopup((int)paramObject, nameArray, valueArray);
            }
        }
    }

    /// <summary>
    /// 长整型 脚本参数绘制器
    /// </summary>
    [ScriptParamType(EParamType.Long)]
    public class Long_ScriptParamDrawer : ScriptParamDrawer<long>
    {
        /// <summary>
        /// 当绘制值
        /// </summary>
        public override void OnDrawValue()
        {
            //base.OnDrawValue();
            EditorGUI.indentLevel = 2;
            paramObject = EditorGUILayout.LongField((long)paramObject);
        }
    }

    /// <summary>
    /// 双精度浮点数 脚本参数绘制器
    /// </summary>
    [ScriptParamType(EParamType.Double)]
    public class Double_ScriptParamDrawer : ScriptParamDrawer<double>
    {
        /// <summary>
        /// 当绘制值
        /// </summary>
        public override void OnDrawValue()
        {
            //base.OnDrawValue();
            EditorGUI.indentLevel = 2;
            paramObject = EditorGUILayout.DoubleField((double)paramObject);
        }
    }

    /// <summary>
    /// 单精度浮点数 脚本参数绘制器
    /// </summary>
    [ScriptParamType(EParamType.Float)]
    public class Float_ScriptParamDrawer : ScriptParamDrawer<float>
    {
        /// <summary>
        /// 当绘制值
        /// </summary>
        public override void OnDrawValue()
        {
            //base.OnDrawValue();
            EditorGUI.indentLevel = 2;
            paramObject = EditorGUILayout.FloatField((float)paramObject);
        }
    }

    /// <summary>
    /// 浮点数滑动条 脚本参数绘制器
    /// </summary>
    [ScriptParamType(EParamType.FloatSlider)]
    public class FloatSlider_ScriptParamDrawer : ScriptParamDrawer<float>
    {
        /// <summary>
        /// 当绘制值
        /// </summary>
        public override void OnDrawValue()
        {
            //base.OnDrawValue();
            EditorGUI.indentLevel = 2;
            float[] comboInfo = param.GetLimitArray<float>();//param.tag as float[];
            float leftValue = ScriptOption.scriptParamFloatLeft;
            float rightValue = ScriptOption.scriptParamFloatRight;
            if (comboInfo != null && comboInfo.Length >= 2)
            {
                leftValue = comboInfo[0];
                rightValue = comboInfo[1];
            }
            paramObject = EditorGUILayout.Slider((float)paramObject, leftValue, rightValue);
        }
    }

    /// <summary>
    /// 组合 脚本参数绘制器
    /// </summary>
    [ScriptParamType(EParamType.Combo)]
    public class Combo_ScriptParamDrawer : ScriptParamDrawer
    {
        /// <summary>
        /// 当绘制值
        /// </summary>
        public override void OnDrawValue()
        {
            //base.OnDrawValue();
            EditorGUI.indentLevel = 2;
            string[] comboInfos = param.GetLimitArray<string>();//param.tag as string[];
            if (comboInfos == null)
            {
                throw new InvalidOperationException("脚本(Combo):[" + scriptStringInfo.script.name[XCSJ.Languages.Language.languageType] + "]的第[" + paramIndex + "]参数:[" + param.name[XCSJ.Languages.Language.languageType] + "]无效！");
            }
            char[] chArray = new char[] { ScriptOption.ScriptParamDelimiterChar, ScriptOption.ScriptParamDelimiterCharCN, ScriptOption.WrapLineChar };
            foreach (var comboInfo in comboInfos)
            {
                //检测非法字符
                if (comboInfo.IndexOfAny(chArray) >= 0)
                {
                    throw new InvalidOperationException("脚本(Combo):[" + scriptStringInfo.script.name[XCSJ.Languages.Language.languageType] + "]参数: [" + param.name.GetAllLanguageText() + "] 的每个Combo字符串中不可以出现 " + CommonFun.ArrayToString<char>(chArray) + " 等字符！");
                }
            }
            int index = Array.IndexOf(comboInfos, paramObject as string);
            index = EditorGUILayout.Popup(index, comboInfos);
            if (index < 0 || index >= comboInfos.Length)
            {
                paramObject = param.defaultObject ?? comboInfos.FirstOrDefault() ?? "";
                GUI.changed = true;
            }
            else paramObject = comboInfos[index];
        }
    }

    /// <summary>
    /// 键码 脚本参数绘制器
    /// </summary>
    [ScriptParamType(EParamType.KeyCode)]
    public class KeyCode_ScriptParamDrawer : EnumScriptParamDrawer<KeyCode> { }

    /// <summary>
    /// 布尔 脚本参数绘制器
    /// </summary>    
    [ScriptParamType(EParamType.Bool)]
    public class Bool_ScriptParamDrawer : EnumScriptParamDrawer<EBool>
    {
        /// <summary>
        /// 默认参数对象
        /// </summary>
        /// <returns></returns>
        public override object DefaultParamObject() => EBool.No;

        /// <summary>
        /// 当初始化
        /// </summary>
        public override void OnInit()
        {
            base.OnInit();
            if (param.defaultObject is bool boolValue)
            {
                paramObject = boolValue ? EBool.Yes : EBool.No;
            }
        }
    }

    /// <summary>
    /// 布尔2 脚本参数绘制器
    /// </summary>
    [ScriptParamType(EParamType.Bool2)]
    public class Bool2_ScriptParamDrawer : EnumScriptParamDrawer<EBool2>
    {
        /// <summary>
        /// 默认参数对象
        /// </summary>
        /// <returns></returns>
        public override object DefaultParamObject() => EBool2.No;

        /// <summary>
        /// 当初始化
        /// </summary>
        public override void OnInit()
        {
            base.OnInit();
            if (param.defaultObject is bool boolValue)
            {
                paramObject = boolValue ? EBool2.Yes : EBool2.No;
            }
        }
    }

    /// <summary>
    /// 坐标轴类型 脚本参数绘制器
    /// </summary>
    [ScriptParamType(EParamType.CoordinateType)]
    public class CoordinateType_ScriptParamDrawer : EnumScriptParamDrawer<ECoordinateType> { }

    /// <summary>
    /// 文本锚点 脚本参数绘制器
    /// </summary>
    [ScriptParamType(EParamType.TextAnchor)]
    public class TextAnchor_ScriptParamDrawer : EnumScriptParamDrawer<TextAnchor>
    {
        /// <summary>
        /// 默认参数对象
        /// </summary>
        /// <returns></returns>
        public override object DefaultParamObject() => TextAnchor.MiddleCenter;
    }

    /// <summary>
    /// 运行时平台 脚本参数绘制器
    /// </summary>
    [ScriptParamType(EParamType.RuntimePlatform)]
    public class RuntimePlatform_ScriptParamDrawer : EnumScriptParamDrawer<RuntimePlatform> { }

    /// <summary>
    /// 鼠标按钮 脚本参数绘制器
    /// </summary>
    [ScriptParamType(EParamType.MouseButton)]
    public class MouseButton_ScriptParamDrawer : EnumScriptParamDrawer<EMouseButtonType> { }
}
