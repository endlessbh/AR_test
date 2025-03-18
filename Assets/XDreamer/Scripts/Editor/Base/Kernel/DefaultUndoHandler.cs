using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Base.Kernel;
using XCSJ.PluginCommonUtils;
using XCSJ.Algorithms;
using XCSJ.EditorExtension.Base.XUnityEditor;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.Extension.GenericStandards;
using XCSJ.Collections;

namespace XCSJ.EditorExtension.Base.Kernel
{
    /// <summary>
    /// 重做助手
    /// </summary>
    public class DefaultUndoHandler : InstanceClass<DefaultUndoHandler>, IUndoHandler
    {
        #region 支持撤销操作标记量的处理

        /// <summary>
        /// 当前是否不支持撤销的标记量
        /// </summary>
        private static bool unsupport = false;

        /// <summary>
        /// 初始化
        /// </summary>
        public static void Init()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;

            EditorSceneManager.sceneOpening += OnSceneOpening;
            EditorSceneManager.sceneOpened += OnSceneOpened;
            EditorSceneManager.sceneClosing += OnSceneClosing;
            EditorSceneManager.sceneClosed += OnSceneClosed;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange playModeStateChanged)
        {
            switch (playModeStateChanged)
            {
                case PlayModeStateChange.EnteredEditMode:
                case PlayModeStateChange.EnteredPlayMode:
                    {
                        unsupport = false;
                        break;
                    }
                case PlayModeStateChange.ExitingEditMode:
                case PlayModeStateChange.ExitingPlayMode:
                    {
                        unsupport = true;
                        break;
                    }
                default: break;
            }
        }

        /// <summary>
        /// 场景打开
        /// </summary>
        /// <param name="path"></param>
        /// <param name="mode"></param>
        private static void OnSceneOpening(string path, OpenSceneMode mode)
        {
            unsupport = true;
        }

        /// <summary>
        /// 场景打开
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="mode"></param>
        private static void OnSceneOpened(UnityEngine.SceneManagement.Scene scene, OpenSceneMode mode)
        {
            unsupport = false;
        }

        /// <summary>
        /// 场景关闭中
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="removingScene"></param>
        private static void OnSceneClosing(UnityEngine.SceneManagement.Scene scene, bool removingScene)
        {
            unsupport = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scene"></param>
        private static void OnSceneClosed(UnityEngine.SceneManagement.Scene scene)
        {
            unsupport = false;
        }

        #endregion

        #region 撤销命令的名称定义与处理

        /// <summary>
        /// 重命名
        /// </summary>
        public const string NameOfRename = "重命名";

        /// <summary>
        /// 创建对象
        /// </summary>
        public const string NameOfCreateObject = "创建对象";

        /// <summary>
        /// 销毁对象
        /// </summary>
        public const string NameOfDestroyObjectImmediate = "销毁对象";

        /// <summary>
        /// 将游戏对象移入场景
        /// </summary>
        public const string MoveGameObjectToSceneName = "将游戏对象移入场景";

        /// <summary>
        /// 记录对象
        /// </summary>
        public const string RecordObjectName = "记录对象";

        /// <summary>
        /// 注册完整对象
        /// </summary>
        public const string NameOfRegisterCompleteObject = "注册完整对象";

        /// <summary>
        /// 注册完整对象层级
        /// </summary>
        public const string RegisterFullObjectHierarchyName = "注册完整层级";

        /// <summary>
        /// 设置父变换
        /// </summary>
        public const string SetTransformParentName = "设置父变换";

        /// <summary>
        /// 创建from to字符串
        /// </summary>
        /// <param name="name"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        private static string GenerateFromToRecordName(string name, string from, string to)
        {
            return GenerateRecordName(name, ObjectLeft + from + Arrow + to + ObjectRight);
        }

        private static string GenerateRecordName(string name, params UnityEngine.Object[] objectsToUndo)
        {
            return GenerateRecordName(name, OjbectsToString(objectsToUndo));
        }

        private static string GenerateRecordName(string name, string description)
        {
            return GenerateGroupDescription() + Product.Name + name + NameSuffix + description;
        }

        private static string GenerateGroupDescription()
        {
            return "[" + Undo.GetCurrentGroup().ToString() + "]";
        }

        /// <summary>
        /// 创建对象字符串
        /// </summary>
        /// <param name="objectsToUndo"></param>
        /// <returns></returns>
        private static string OjbectsToString(UnityEngine.Object[] objectsToUndo)
        {
            var sb = new StringBuilder(ObjectLeft);
            var separator = "";
            for (int i = 0; i < objectsToUndo.Length; i++)
            {
                var obj = objectsToUndo[i];
                if (obj)
                {
                    sb.AppendFormat("{0}{1}", separator, obj.name);
                    separator = ObjectSeparator;
                }
            }
            sb.Append(ObjectRight);
            return sb.ToString();
        }

        private const string NameSuffix = ":";
        private const string ObjectSeparator = ",";
        private const string Arrow = "=>";
        private const string ObjectLeft = "{";
        private const string ObjectRight = "}";

        #endregion

        #region 撤销命令的处理函数

        /// <summary>
        /// 记录重命名
        /// </summary>
        /// <param name="objectsToUndo"></param>
        public void RecordRename(params UnityEngine.Object[] objectsToUndo)
        {
            if (unsupport || inPatch) return;
            if (objectsToUndo == null || objectsToUndo.Length == 0 || objectsToUndo.Any(o => !o)) return;

            Undo.RegisterCompleteObjectUndo(objectsToUndo, GenerateRecordName(NameOfRename, objectsToUndo));
        }

        /// <summary>
        /// 注册创建对象对象
        /// </summary>
        /// <param name="objectToUndo"></param>
        public void RegisterCreatedObjectUndo(UnityEngine.Object objectToUndo)
        {
            if (unsupport || inPatch) return;
            if (objectToUndo)
            {
                Undo.RegisterCreatedObjectUndo(objectToUndo, GenerateRecordName(NameOfCreateObject, objectToUndo.name));
            }
        }

        /// <summary>
        /// 销毁对象
        /// </summary>
        /// <param name="objectToUndo"></param>
        public void DestroyObjectImmediate(UnityEngine.Object objectToUndo)
        {
            if (unsupport || inPatch) return;
            if(objectToUndo is Model model)
            {
                DestroyModelWithRegisterObjectReferenceInScene(model);
            }
            else
            {
                DestroyObjectImmediateInternal(objectToUndo);
            }
        }

        /// <summary>
        /// 内部立即删除对象不做任何检测
        /// </summary>
        /// <param name="objectToUndo"></param>
        private void DestroyObjectImmediateInternal(UnityEngine.Object objectToUndo)
        {
            if (objectToUndo)
            {
                Undo_LinkType.DestroyObjectUndoable(objectToUndo, GenerateRecordName(NameOfDestroyObjectImmediate, objectToUndo.name));
            }
        }

        /// <summary>
        /// 注册完整对象
        /// </summary>
        /// <param name="objectsToUndo"></param>
        public void RegisterCompleteObjectUndo(params UnityEngine.Object[] objectsToUndo)
        {
            if (unsupport || inPatch) return;
            if (objectsToUndo == null || objectsToUndo.Length == 0 || objectsToUndo.Any(o => !o)) return;

            Undo.RegisterCompleteObjectUndo(objectsToUndo, GenerateRecordName(NameOfRegisterCompleteObject, objectsToUndo));
        }


        /// <summary>
        /// 设置父变换
        /// </summary>
        /// <param name="child"></param>
        /// <param name="parent"></param>
        public void RecordSetTransformParent(Transform child, Transform parent)
        {
            if (unsupport || inPatch) return;
            if (!child) return;

            Undo.SetTransformParent(child, parent, GenerateFromToRecordName(SetTransformParentName, child.name, parent ? parent.name : ""));
        }

        /// <summary>
        /// 查找当前激活场景中所有引用objectToUndo的对象引用，将所有的对象引用做完整注册；
        /// </summary>
        /// <param name="objectToUndo"></param>
        public void RegisterObjectReferenceInScene(UnityEngine.Object objectToUndo)
        {
            if (unsupport || inPatch) return;
            if (!objectToUndo) return;
            RegisterCompleteObjectUndo(objectToUndo);
            foreach (var item in ReferenceInfo.FindReferenceInScene(objectToUndo))
            {
                if (item.serializedProperty != null)
                {
                    RegisterCompleteObjectUndo(item.targetObject);
                }
            }
        }

        /// <summary>
        /// 查找当前激活场景中所有引用objectToUndo的对象引用，将所有的对象引用做完整注册，之后删除objectToUndo对象；
        /// </summary>
        /// <param name="objectToUndo"></param>
        public void DestroyObjectWithRegisterObjectReferenceInScene(UnityEngine.Object objectToUndo)
        {
            if (unsupport || inPatch) return;
            if (!objectToUndo) return;
            if(objectToUndo is Model model)
            {
                DestroyModelWithRegisterObjectReferenceInScene(model);
            }
            else
            {
                RegisterObjectReferenceInScene(objectToUndo);
                DestroyObjectImmediateInternal(objectToUndo);
            }            
        }

        #endregion

        #region 批量删除Model

        /// <summary>
        /// 批量查找当前激活场景中所有引用modelsToUndo元素的对象引用，将所有的对象引用做完整注册，之后依次删除modelsToUndo元素；
        /// </summary>
        /// <param name="modelsToUndo"></param>
        public void DestroyModelsWithRegisterObjectReferenceInScene(IEnumerable<Model> modelsToUndo)
        {
            if (modelsToUndo != null && BeginPatchDestroyModels())
            {
                try
                {
                    //查找引用
                    foreach (var item in FindModelsReferenceInScene(modelsToUndo))
                    {
                        if (item.serializedProperty != null)
                        {
                            //记录引用
                            Undo.RegisterCompleteObjectUndo(item.targetObject, item.targetObject.name);
                        }
                    }

                    //执行删除
                    foreach (var m in modelsToUndo)
                    {
                        if (m) m.Delete(false);
                    }
                }
                catch(Exception ex)
                {
                    Debug.LogException(ex);
                }
                finally
                {
                    EndPatchDestroyModels();
                }
            }
        }

        /// <summary>
        /// 是否在批处理状态的标记量
        /// </summary>
        private static bool inPatch = false;

        private bool BeginPatchDestroyModels()
        {
            if (unsupport || inPatch) return false;
            inPatch = true;
            Undo.SetCurrentGroupName("批量删除Model");
            return true;
        }

        private void EndPatchDestroyModels()
        {
            inPatch = false;
        }

        private List<ReferenceInfo> FindModelsReferenceInScene(IEnumerable<Model> modelsToUndo)
        {
            var models = new HashSet<Model>();
            modelsToUndo.Foreach(m =>
            {
                if (m)
                {
                    models.UnionWith(m.GetWillDeleteModels());
                }
            });
            if (models.Count == 0) return new List<ReferenceInfo>();

            return ReferenceInfo.FindReferenceInScene((go, c, o, sp) => BatchFindModelReferenceFunc(go, c, o, sp, models), CheckTypeValid);
        }

        private static bool BatchFindModelReferenceFunc(GameObject gameObject, UnityEngine.Component component, UnityEngine.Object obj, SerializedProperty serializedProperty, HashSet<Model> models)
        {
            if (serializedProperty == null)
            {
                if (obj && obj is Model model && models.Contains(model)) return true;
            }
            else
            {
                //var goName = CommonFun.GameObjectToString(gameObject);
                //var componentName = component ? component.GetType().FullName : "";
                //var objName = obj ? obj.GetType().FullName : "";
                //var serializedPropertyPath = serializedProperty.propertyPath;
                //var serializedPropertyType = serializedProperty.type;
                //Debug.Log(goName + "=>" + componentName + "=>" + objName + "=>" + serializedPropertyPath + "=>" + serializedPropertyType);
                switch (serializedProperty.propertyType)
                {
                    case SerializedPropertyType.ObjectReference:
                        {
                            var tmpObject = serializedProperty.objectReferenceValue;
                            if (tmpObject && tmpObject is Model model && models.Contains(model)) return true;
                            break;
                        }
                }
            }

            return false;
        }

        #endregion

        #region 单个删除Model

        /// <summary>
        /// 查找当前激活场景中所有引用modelToUndo的对象引用，将所有的对象引用做完整注册，之后删除modelToUndo对象；
        /// </summary>
        /// <param name="objectToUndo"></param>
        public void DestroyModelWithRegisterObjectReferenceInScene(Model modelToUndo)
        {
            if (!modelToUndo) return;
            if (inPatch)
            {
                Undo.DestroyObjectImmediate(modelToUndo);
            }
            else
            {
                if (unsupport) return;

                RegisterModelReferenceInScene(modelToUndo);
                DestroyObjectImmediateInternal(modelToUndo);
            }
        }

        /// <summary>
        /// 查找当前激活场景中所有引用modelToUndo的对象引用，将所有的对象引用做完整注册；
        /// </summary>
        /// <param name="modelToUndo"></param>
        public void RegisterModelReferenceInScene(Model modelToUndo)
        {
            if (unsupport || inPatch) return;
            if (!modelToUndo) return;

            Undo.RegisterCompleteObjectUndo(modelToUndo, modelToUndo.name);

            foreach (var item in ReferenceInfo.FindReferenceInScene((go, c, o, sp) => ReferenceInfo.DefaultFindObjectReferenceFunc(go, c, o, sp, modelToUndo), CheckTypeValid))
            {
                if (item.serializedProperty != null)
                {
                    Undo.RegisterCompleteObjectUndo(item.targetObject, item.targetObject.name);
                }
            }
        }

        #endregion

        #region 对Model缓存机制处理

        private static bool CheckTypeValid(UnityEngine.Object obj)
        {
            if (InvalidTypes.Contains(obj.GetType())) return false;

            //通过命名空间做检查，如果无效则加入缓存
            var type = obj.GetType();
            if (InvalidTypeNamespacess.Any(ns => type.FullName.StartsWith(ns)))
            {
                InvalidTypes.Add(type);
                return false;
            }
            return true;
        }

        private readonly static HashSet<string> InvalidTypeNamespacess = new HashSet<string>()
        {
            nameof(UnityEngine) + ".",
        };

        private static HashSet<Type> InvalidTypes = new HashSet<Type>();

#endregion
    }
}
