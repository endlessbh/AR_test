using System;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.CNScripts;

namespace XCSJ.Extension.CNScripts.Base
{
    /// <summary>
    /// MonoBehaviour脚本事件类型：仅处理MonoBehaviour中可被重载方法；<br />
    /// **部分脚本事件在跨平台时会失效**使用其他方法替换**<br />
    /// 添加新的事件时只能在末尾追加！！！
    /// </summary>
    [Name("MonoBehaviour脚本事件类型")]
    public enum EMonoBehaviourScriptEventType
    {
        /// <summary>
        /// 启动时
        /// </summary>
        [Name("启动时执行")]
        Start = 0,
        /// <summary>
        /// 更新时
        /// </summary>
        [Name("更新时执行")]
        Update,
        /// <summary>
        /// 变为可用或激活状态时
        /// </summary>
        [Name("变为可用或激活状态时执行")]
        OnEnable,
        /// <summary>
        /// 变为不可用或非激活状态时
        /// </summary>
        [Name("变为不可用或非激活状态时执行")]
        OnDisable,
        /// <summary>
        /// 销毁时
        /// </summary>
        [Name("销毁时执行")]
        OnDestroy,
        /// <summary>
        /// 重置时
        /// </summary>
        [Name("重置时执行")]
        Reset,
        /// <summary>
        /// 渲染GUI时
        /// </summary>
        [Name("渲染GUI时执行")]
        OnGUI,
        /// <summary>
        /// 程序退出时
        /// </summary>
        [Name("程序退出时执行")]
        OnApplicationQuit,
        /// <summary>
        /// 程序获取焦点时
        /// </summary>
        [Name("程序获取焦点时执行")]
        OnApplicationFocus,
        /// <summary>
        /// 鼠标移入时
        /// </summary>
        [Name("鼠标移入时执行")]
        OnMouseEnter,
        /// <summary>
        /// 鼠标悬浮时
        /// </summary>
        [Name("鼠标悬浮时执行")]
        OnMouseOver,
        /// <summary>
        /// 鼠标移出时
        /// </summary>
        [Name("鼠标移出时执行")]
        OnMouseExit,
        /// <summary>
        /// 鼠标点击时
        /// </summary>
        [Name("鼠标点击时执行")]
        OnMouseDown,
        /// <summary>
        /// 鼠标弹起时
        /// </summary>
        [Name("鼠标弹起时执行")]
        OnMouseUp,
        /// <summary>
        /// 鼠标弹起(点击与弹起为同一元素)时
        /// </summary>
        [Name("鼠标弹起(点击与弹起为同一元素)时执行")]
        OnMouseUpAsButton,
        /// <summary>
        /// 鼠标拖拽时
        /// </summary>
        [Name("鼠标拖拽时执行")]
        OnMouseDrag,
        /// <summary>
        /// 进入触发器时
        /// </summary>
        [Name("进入触发器时执行")]
        OnTriggerEnter,
        /// <summary>
        /// 停止触发器时
        /// </summary>
        [Name("停止触发器时执行")]
        OnTriggerExit,
        /// <summary>
        /// 碰撞体接触触发器时
        /// </summary>
        [Name("碰撞体接触触发器时执行")]
        OnTriggerStay,
        /// <summary>
        /// 碰撞体与碰撞体接触时(每帧调用)
        /// </summary>
        [Name("碰撞体与碰撞体接触时(每帧调用)执行")]
        OnCollisionEnter,
        /// <summary>
        /// 碰撞体与碰撞体停止接触时时
        /// </summary>
        [Name("碰撞体与碰撞体停止接触时时执行")]
        OnCollisionExit,
        /// <summary>
        /// 碰撞体与碰撞体停止接触时(每帧调用)
        /// </summary>
        [Name("碰撞体与碰撞体停止接触时(每帧调用)执行")]
        OnCollisionStay,
        /// <summary>
        /// 控制体与碰撞体碰撞时
        /// </summary>
        [Name("控制体与碰撞体碰撞时执行")]
        OnControllerColliderHit,
        /// <summary>
        /// 相机渲染场景前(所有渲染开始前)
        /// </summary>
        [Name("相机渲染场景前(所有渲染开始前)执行")]
        OnPreRender,
        /// <summary>
        /// 相机渲染场景后(所有渲染完成后)
        /// </summary>
        [Name("相机渲染场景后(所有渲染完成后)执行")]
        OnPostRender,
        /// <summary>
        /// 当前对象渲染后
        /// </summary>
        [Name("当前对象渲染后执行")]
        OnRenderObject,
        /// <summary>
        /// 对象可见,相机渲染前
        /// </summary>
        [Name("对象可见且相机渲染前执行")]
        OnWillRenderObject,
    }

    /// <summary>
    /// MonoBehaviour脚本事件函数
    /// </summary>
    [Serializable]
    [Name("MonoBehaviour脚本事件函数")]
    public class MonoBehaviourScriptEventFunction : EnumFunction<EMonoBehaviourScriptEventType> { }

    /// <summary>
    /// MonoBehaviour脚本事件函数集合
    /// </summary>
    [Serializable]
    [Name("MonoBehaviour脚本事件函数集合")]
    public class MonoBehaviourScriptEventFunctionCollection : EnumFunctionCollection<EMonoBehaviourScriptEventType, MonoBehaviourScriptEventFunction> { }
}
