using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.ComponentModel;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginPhysicses.CNScripts;
using XCSJ.PluginPhysicses.Tools.Collisions;
using XCSJ.Scripts;

namespace XCSJ.PluginPhysicses
{
    /// <summary>
    /// 物理系统:用于模拟各种物理现象的工具;包括射击、碰撞、按钮、摇杆、抽屉和门开关;
    /// </summary>
    [Serializable]
    [DisallowMultipleComponent]
    [ComponentKit(EKit.Advanced)]
    [ComponentOption(EComponentOption.Optional)]
    [Name(PhysicsManager.Title)]
    [Tip("用于模拟各种物理现象的工具;包括射击、碰撞、按钮、摇杆、抽屉和门开关;", "Tools for simulating various physical phenomena; Including shooting, collision, button, rocker, drawer and door switch;")]
    [Guid("5BCAEB62-DD6E-4764-A94D-68C2DA2D969B")]
    [Version("23.428")]
    public sealed class PhysicsManager : BaseManager<PhysicsManager>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "物理系统";

        [Name("克隆对象生命时长")]
        public float _cloneLifeTime = 100;

        /// <summary>
        /// 获取脚本
        /// </summary>
        /// <returns></returns>
        public override List<Script> GetScripts() => Script.GetScriptsOfEnum<EScriptID>(this);

        /// <summary>
        /// 执行脚本
        /// </summary>
        /// <param name="id"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public override ReturnValue ExecuteScript(int id, ScriptParamList param)
        {
            switch ((EScriptID)id)
            {
                case EScriptID.SetRigidbody:
                    {
                        var rb = param[1] as Rigidbody;
                        if (!rb) break;

                        rb.useGravity = CommonFun.BoolChange(rb.useGravity, (EBool)param[2]);

                        return ReturnValue.Yes;
                    }
                case EScriptID.SetJoint:
                    {
                        var joint = param[1] as Joint;
                        if (!joint) break;

                        joint.connectedBody = param[2] as Rigidbody;

                        return ReturnValue.Yes;
                    }
                case EScriptID.MeshDamagerOperation:
                    {
                        var meshDamager = param[1] as MeshDamager;
                        if (!meshDamager) break;

                        switch (param[2] as string)
                        {
                            case "开始修复":
                                {
                                    meshDamager.BeginRepair((float)param[3]);
                                    return ReturnValue.Yes;
                                }
                            case "停止修复":
                                {
                                    meshDamager.StopRepair();
                                    return ReturnValue.Yes;
                                }
                        }
                        break;
                    }
            }
            return ReturnValue.No;
        }

        /// <summary>
        /// 物理克隆对象管理
        /// </summary>
        public Dictionary<GameObject, float> cloneGameObjectLifeTimeMap = new Dictionary<GameObject, float>();
        private List<GameObject> willDestoryGameObjects = new List<GameObject>();

        /// <summary>
        /// 克隆对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="component"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public T Clone<T>(T component, Transform parent = null) where T : Component
        {
            return Clone<T>(component, _cloneLifeTime);
        }

        /// <summary>
        /// 克隆对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="component"></param>
        /// <param name="lifeTime"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public T Clone<T>(T component, float lifeTime, Transform parent = null) where T : Component
        {
            var newGO = UnityEngine.Object.Instantiate(component.gameObject);
            newGO.transform.XSetTransformParent(parent);
            if (!parent)
            {
                newGO.transform.XSetLocalScale(component.gameObject.transform.lossyScale);// 新对象的父级为根对象
            }
            newGO.SetActive(true);
            var newComponent = newGO.GetComponent<T>();
            AddCloneObject(newGO, lifeTime);
            return newComponent;
        }

        /// <summary>
        /// 克隆对象
        /// </summary>
        /// <param name="go"></param>
        public void AddCloneObject(GameObject go)
        {
            AddCloneObject(go, _cloneLifeTime);
        }

        /// <summary>
        /// 添加游戏对象
        /// </summary>
        /// <param name="go"></param>
        /// <param name="lifeTime"></param>
        public void AddCloneObject(GameObject go, float lifeTime)
        {
            if (!cloneGameObjectLifeTimeMap.ContainsKey(go))
            {
                cloneGameObjectLifeTimeMap.Add(go, lifeTime);
                if (lifeTime > 0)
                {
                    willDestoryGameObjects.Add(go);
                    DelayAction.Start(lifeTime, go, DestoryCloneObject, nameof(DestoryCloneObject));
                }
            }
        }

        /// <summary>
        /// 移除克隆刚体对象
        /// </summary>
        /// <param name="rigidbody"></param>
        public void Remove(Rigidbody rigidbody)
        {
            if (rigidbody)
            {
                RemoveCloneObject(rigidbody.gameObject);
            }
        }

        /// <summary>
        /// 移除克隆游戏对象
        /// </summary>
        /// <param name="gameObject"></param>
        public void RemoveCloneObject(GameObject gameObject)
        {
            if (!gameObject) return;
            try
            {
                cloneGameObjectLifeTimeMap.Remove(gameObject);
                willDestoryGameObjects.Remove(gameObject);
                Destroy(gameObject);
            }
            catch (System.Exception ex)
            {
                Debug.Log(ex.ToString());
            }
        }

        /// <summary>
        /// 清空所有克隆对象
        /// </summary>
        public void ClearCloneObject()
        {
            var list = new List<GameObject>();
            list.AddRange(cloneGameObjectLifeTimeMap.Keys);
            foreach (var go in list)
            {
                RemoveCloneObject(go);
            }
        }

        private void DestoryCloneObject(object cloneObject)
        {
            var go = cloneObject as GameObject;
            if (go && willDestoryGameObjects.Remove(go))
            {
                Destroy(go.gameObject);
            }
        }
    }
}
