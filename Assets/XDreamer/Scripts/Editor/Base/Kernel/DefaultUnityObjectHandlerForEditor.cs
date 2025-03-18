using System;
using UnityEditor;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginCommonUtils.Base.Kernel;

namespace XCSJ.EditorExtension.Base.Kernel
{
    /// <summary>
    /// 用于编辑器的默认Unity对象处理器；支持撤销操作；
    /// </summary>
    public class DefaultUnityObjectHandlerForEditor : InstanceClass<DefaultUnityObjectHandlerForEditor>, IUnityObjectHandler
    {
        /// <summary>
        /// 注册创建对象撤销
        /// </summary>
        /// <param name="objectToUndo"></param>
        public void RegisterCreatedObjectUndo(UnityEngine.Object objectToUndo)
        {
            UndoHelper.RegisterCreatedObjectUndo(objectToUndo);
        }

        /// <summary>
        /// 创建游戏对象
        /// </summary>
        /// <param name="name">游戏对象的名称：会调用重命名<see cref="Rename(UnityEngine.Object, string)"/></param>
        /// <returns>已创建的游戏对象</returns>
        public GameObject CreateGameObject(string name)
        {
            var go = UnityObjectHandlerEvent.CallCreateObject(null, () =>
            {
                var createdObject = new GameObject("");
                UndoHelper.RegisterCreatedObjectUndo(createdObject);
                return createdObject;
            });
            Rename(go, name);
            return go;
        }

        /// <summary>
        /// 创建游戏对象
        /// </summary>
        /// <param name="primitiveType">基础类型</param>
        /// <returns>已创建的游戏对象</returns>
        public GameObject CreateGameObject(PrimitiveType primitiveType)
        {
            var go = UnityObjectHandlerEvent.CallCreateObject(null, () =>
            {
                var createdObject = GameObject.CreatePrimitive(primitiveType);
                UndoHelper.RegisterCreatedObjectUndo(createdObject);
                return createdObject;
            });
            return go;
        }

        /// <summary>
        /// 标识当前是否正在添加组件的过程中...
        /// </summary>
        private bool inAddComponent = false;

        /// <summary>
        /// 添加组件:在执行<see cref="Undo.AddComponent(GameObject, Type)"/>并自动调用组件的Rest()重置函数时，在内部不可以再执行<see cref="Undo"/>的任何操作，否则会报出警告错误；
        /// </summary>
        /// <param name="gameObject">添加组件的游戏对象</param>
        /// <param name="componentType">待创建组件类型</param>
        /// <returns>已添加的组件对象</returns>
        public Component AddComponent(GameObject gameObject, Type componentType)
        {
            if (!gameObject || componentType == null || !typeof(Component).IsAssignableFrom(componentType)) return default;

            return UnityObjectHandlerEvent.CallCreateObject(componentType, gameObject, () =>
            {
                var inAddComponenting = inAddComponent;
                try
                {
                    if (inAddComponenting)//在添加组件过程中又在添加组件
                    {
                        return gameObject.AddComponent(componentType);
                    }
                    else
                    {
                        inAddComponent = true;
                        return Undo.AddComponent(gameObject, componentType);
                    }
                }
                finally
                {
                    if(!inAddComponenting)
                    {
                        inAddComponent = false;
                    }
                }
            });
        }

        /// <summary>
        /// 创建脚本对象
        /// </summary>
        /// <param name="scriptableObjectType">将要创建的脚本对象类型</param>
        /// <returns>已创建脚本对象</returns>
        public ScriptableObject CreateScriptableObject(Type scriptableObjectType)
        {
            if (scriptableObjectType == null || !typeof(ScriptableObject).IsAssignableFrom(scriptableObjectType)) return default;
            return UnityObjectHandlerEvent.CallCreateObject(scriptableObjectType, null, () =>
            {
                var createdObject = ScriptableObject.CreateInstance(scriptableObjectType);
                UndoHelper.RegisterCreatedObjectUndo(createdObject);
                return createdObject;
            });
        }

        /// <summary>
        /// 克隆对象
        /// </summary>
        /// <param name="obj">将要克隆的对象</param>
        /// <returns>返回新克隆的对象</returns>
        public UnityEngine.Object CloneObject(UnityEngine.Object obj)
        {
            if (!obj) return default;
            return UnityObjectHandlerEvent.CallCloneObject(obj, () =>
            {
                var clonedObject = UnityEngine.Object.Instantiate(obj);
                UndoHelper.RegisterCreatedObjectUndo(clonedObject);
                return clonedObject;
            });
        }

        /// <summary>
        /// 销毁对象
        /// </summary>
        /// <param name="obj">将要销毁的对象</param>
        public void DestoryObject(UnityEngine.Object obj)
        {
            if (!obj) return;
            UnityObjectHandlerEvent.CallDestroyObject(obj.GetType(), obj, () =>
            {
                UndoHelper.DestroyObjectWithRegisterObjectReferenceInScene(obj);
            });
        }

        /// <summary>
        /// 重命名
        /// </summary>
        /// <param name="obj">将要重命名的对象</param>
        /// <param name="name">新名称</param>
        public void Rename(UnityEngine.Object obj, string name)
        {
            if (!obj) return;

            UnityObjectHandlerEvent.CallRename(obj, obj.name, name, () =>
            {              
                UndoHelper.RecordRename(obj);
                obj.name = name;
            });
        }

        /// <summary>
        /// 设置父级变换
        /// </summary>
        /// <param name="transform">将要设置父级变换的变换对象</param>
        /// <param name="newParent">新父级</param>
        public void SetTransformParent(Transform transform, Transform newParent)
        {
            if (!transform) return;
            UnityObjectHandlerEvent.CallSetTransformParent(transform, transform.parent, newParent, () =>
            {
                UndoHelper.RecordSetTransformParent(transform, newParent);
            });
        }

        /// <summary>
        /// 修改属性
        /// </summary>
        /// <param name="obj">将要修改属性的对象</param>
        /// <param name="name">将要修改的属性名称（或是用户对本次属性修改动作的自定义名称，此情况多用于多个属性的修改）</param>
        /// <param name="action">修改函数</param>
        public void ModifyProperty(UnityEngine.Object obj, string name, Action action)
        {
            if (!obj || action == null) return;
            UnityObjectHandlerEvent.CallModifyProperty(obj, name, () =>
            {
                if (!inAddComponent) UndoHelper.RegisterCompleteObjectUndo(obj);
                action();
            });
        }

        /// <summary>
        /// 修改属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">将要修改属性的对象</param>
        /// <param name="name">将要修改的属性名称</param>
        /// <param name="property">将要修改的属性</param>
        /// <param name="newValue">新的属性值</param>
        public void ModifyProperty<T>(UnityEngine.Object obj, string name, ref T property, T newValue)
        {
            if (!obj) return;
            UnityObjectHandlerEvent.CallModifyProperty(obj, name, ref property, newValue, () =>
            {
                if (!inAddComponent) UndoHelper.RegisterCompleteObjectUndo(obj);
            });
        }
    }
}
