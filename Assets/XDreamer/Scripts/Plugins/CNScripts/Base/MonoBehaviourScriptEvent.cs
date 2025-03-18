using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.CNScripts;

namespace XCSJ.Extension.CNScripts.Base
{
    /// <summary>
    /// MonoBehaviour脚本事件
    /// </summary>
    [Serializable]
    [Name(Title)]
    [DisallowMultipleComponent]
    [AddComponentMenu(CNScriptHelper.CNScriptMenu + Title)]
    public sealed class MonoBehaviourScriptEvent : BaseScriptEvent<EMonoBehaviourScriptEventType, MonoBehaviourScriptEventFunction, MonoBehaviourScriptEventFunctionCollection>,
        IOnTrigger,
        IOnCollision
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "MonoBehaviour脚本事件";

        /// <summary>
        /// 启动
        /// </summary>
        protected override void Start()
        {
            base.Start();
            ExecuteScriptEvent(EMonoBehaviourScriptEventType.Start);
        }

        /// <summary>
        /// 更新（Update is called once per frame)
        /// </summary>
        void Update()
        {
            ExecuteScriptEvent(EMonoBehaviourScriptEventType.Update);
        }

        /// <summary>
        /// 启动时
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            ExecuteScriptEvent(EMonoBehaviourScriptEventType.OnEnable);
        }

        /// <summary>
        /// 变为不可用或非激活状态时
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            ExecuteScriptEvent(EMonoBehaviourScriptEventType.OnDisable);
        }

        /// <summary>
        /// 渲染GUI时
        /// </summary>
        void OnGUI()
        {
            ExecuteScriptEvent(EMonoBehaviourScriptEventType.OnGUI);
        }

        /// <summary>
        /// 销毁时
        /// </summary>
        protected override void OnDestroy()
        {
            base.OnDestroy();
            ExecuteScriptEvent(EMonoBehaviourScriptEventType.OnDestroy);
        }

        /// <summary>
        /// 重置时
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            ExecuteScriptEvent(EMonoBehaviourScriptEventType.Reset);
        }

        /// <summary>
        /// 程序退出时
        /// </summary>
        public void OnApplicationQuit()
        {
            ExecuteScriptEvent(EMonoBehaviourScriptEventType.OnApplicationQuit);
        }

        /// <summary>
        /// 程序获取焦点时
        /// </summary>
        public void OnApplicationFocus()
        {
            ExecuteScriptEvent(EMonoBehaviourScriptEventType.OnApplicationFocus);
        }

        /// <summary>
        /// 鼠标移入时
        /// </summary>
        public void OnMouseEnter()
        {
            ExecuteScriptEvent(EMonoBehaviourScriptEventType.OnMouseEnter);
        }

        /// <summary>
        /// 鼠标悬浮时
        /// </summary>
        public void OnMouseOver()
        {
            ExecuteScriptEvent(EMonoBehaviourScriptEventType.OnMouseOver);
        }

        /// <summary>
        /// 鼠标移出时
        /// </summary>
        public void OnMouseExit()
        {
            ExecuteScriptEvent(EMonoBehaviourScriptEventType.OnMouseExit);
        }

        /// <summary>
        /// 鼠标点击时
        /// </summary>
        public void OnMouseDown()
        {
            ExecuteScriptEvent(EMonoBehaviourScriptEventType.OnMouseDown);
        }

        /// <summary>
        /// 鼠标弹起时
        /// </summary>
        public void OnMouseUp()
        {
            ExecuteScriptEvent(EMonoBehaviourScriptEventType.OnMouseUp);
        }

        /// <summary>
        /// 鼠标弹起(点击与弹起为同一元素)时
        /// </summary>
        public void OnMouseUpAsButton()
        {
            ExecuteScriptEvent(EMonoBehaviourScriptEventType.OnMouseUpAsButton);
        }

        /// <summary>
        /// 鼠标拖拽时
        /// </summary>
        public void OnMouseDrag()
        {
            ExecuteScriptEvent(EMonoBehaviourScriptEventType.OnMouseDrag);
        }

        /// <summary>
        /// 进入触发器时
        /// </summary>
        public void OnTriggerEnter(Collider collider)
        {
            ExecuteScriptEvent(EMonoBehaviourScriptEventType.OnTriggerEnter, CommonFun.GameObjectComponentToString(collider));
        }

        /// <summary>
        /// 停止触发器时
        /// </summary>
        public void OnTriggerExit(Collider collider)
        {
            ExecuteScriptEvent(EMonoBehaviourScriptEventType.OnTriggerExit, CommonFun.GameObjectComponentToString(collider));
        }

        /// <summary>
        /// 碰撞体接触触发器时(每帧调用)
        /// </summary>
        public void OnTriggerStay(Collider collider)
        {
            ExecuteScriptEvent(EMonoBehaviourScriptEventType.OnTriggerStay, CommonFun.GameObjectComponentToString(collider));
        }

        /// <summary>
        /// 碰撞体与碰撞体接触时
        /// </summary>
        public void OnCollisionEnter(Collision collision)
        {
            ExecuteScriptEvent(EMonoBehaviourScriptEventType.OnCollisionEnter, CommonFun.GameObjectToString(collision.gameObject));
        }

        /// <summary>
        /// 碰撞体与碰撞体停止接触时时
        /// </summary>
        public void OnCollisionExit(Collision collision)
        {
            ExecuteScriptEvent(EMonoBehaviourScriptEventType.OnCollisionExit, CommonFun.GameObjectToString(collision.gameObject));
        }

        /// <summary>
        /// 碰撞体与碰撞体停止接触时(每帧调用)
        /// </summary>
        public void OnCollisionStay(Collision collision)
        {
            ExecuteScriptEvent(EMonoBehaviourScriptEventType.OnCollisionStay, CommonFun.GameObjectToString(collision.gameObject));
        }

        /// <summary>
        /// 控制体与碰撞体碰撞时
        /// </summary>
        public void OnControllerColliderHit(ControllerColliderHit hit)
        {
            ExecuteScriptEvent(EMonoBehaviourScriptEventType.OnControllerColliderHit, CommonFun.GameObjectToString(hit.gameObject));
        }

        /// <summary>
        /// 相机渲染场景前(所有渲染开始前)
        /// </summary>
        public void OnPreRender()
        {
            ExecuteScriptEvent(EMonoBehaviourScriptEventType.OnPreRender);
        }

        /// <summary>
        /// 相机渲染场景后(所有渲染完成后)
        /// </summary>
        public void OnPostRender()
        {
            ExecuteScriptEvent(EMonoBehaviourScriptEventType.OnPostRender);
        }

        /// <summary>
        /// 当前对象渲染后
        /// </summary>
        public void OnRenderObject()
        {
            ExecuteScriptEvent(EMonoBehaviourScriptEventType.OnRenderObject);
        }

        /// <summary>
        /// 对象可见且相机渲染前
        /// </summary>
        public void OnWillRenderObject()
        {
            ExecuteScriptEvent(EMonoBehaviourScriptEventType.OnWillRenderObject);
        }
    }
}
