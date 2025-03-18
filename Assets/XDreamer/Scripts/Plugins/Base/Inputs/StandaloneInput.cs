using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;

namespace XCSJ.Extension.Base.Inputs
{
    /// <summary>
    /// 标准输入：直接映射<see cref="Input"/>类
    /// </summary>
    [Name("标准输入")]
    [Tip("直接映射UnityEngine.Input类", "Directly map unityengine Input class")]
    public sealed class StandaloneInput : InstanceClass<StandaloneInput>, IInput
    {
        /// <summary>
        /// 虚拟输入
        /// </summary>
        public string name { get =>nameof(StandaloneInput); set { } }

        /// <summary>
        /// 重置
        /// </summary>
        public void Reset() { }

        #region 获取

        /// <summary>
        /// 输入
        /// </summary>
        public EInput input => EInput.StandaloneInput;

        /// <summary>
        /// 任意按键
        /// </summary>
        public bool anyKey => Input.anyKey;

        /// <summary>
        /// 任意按键按下
        /// </summary>
        public bool anyKeyDown => Input.anyKeyDown;

        /// <summary>
        /// 按键是否触发，只要当前键按下状态未改变，一直为true
        /// </summary>
        /// <param name="buttonName"></param>
        /// <returns></returns>
        public bool GetButton(string buttonName) => Input.GetButton(buttonName);

        /// <summary>
        /// 按键是否按下
        /// </summary>
        /// <param name="buttonName"></param>
        /// <returns></returns>
        public bool GetButtonDown(string buttonName) => Input.GetButtonDown(buttonName);

        /// <summary>
        /// 按键是否弹起
        /// </summary>
        /// <param name="buttonName"></param>
        /// <returns></returns>
        public bool GetButtonUp(string buttonName) => Input.GetButtonUp(buttonName);

        /// <summary>
        /// 获取滑动轴的值
        /// </summary>
        /// <param name="axisName"></param>
        /// <returns></returns>
        public float GetAxis(string axisName) => Input.GetAxis(axisName);

        /// <summary>
        /// 获取滑动轴的值,不连续的值
        /// </summary>
        /// <param name="axisName"></param>
        /// <returns></returns>
        public float GetAxisRaw(string axisName) => Input.GetAxisRaw(axisName);

        /// <summary>
        /// 按键是否有操作
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool GetKey(string keyName) => Input.GetKey(keyName);

        /// <summary>
        /// 按键是否有操作
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool GetKey(KeyCode key) => Input.GetKey(key);

        /// <summary>
        /// 按键是否按下
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool GetKeyDown(string keyName) => Input.GetKeyDown(keyName);

        /// <summary>
        /// 按键是否按下
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool GetKeyDown(KeyCode key) => Input.GetKeyDown(key);

        /// <summary>
        /// 按键是否弹起
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool GetKeyUp(string keyName) => Input.GetKeyUp(keyName);

        /// <summary>
        /// 按键是否弹起
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool GetKeyUp(KeyCode key) => Input.GetKeyUp(key);

        /// <summary>
        /// 检测鼠标按键是否有操作
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        public bool GetMouseButton(int button) => Input.GetMouseButton(button);

        /// <summary>
        /// 鼠标按键是否按下
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        public bool GetMouseButtonDown(int button) => Input.GetMouseButtonDown(button);

        /// <summary>
        /// 鼠标按键是否弹起
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        public bool GetMouseButtonUp(int button) => Input.GetMouseButtonUp(button);

        #endregion

        #region 更新

        /// <summary>
        /// 更新按钮
        /// </summary>
        /// <param name="name"></param>
        /// <param name="downOrUp"></param>
        /// <returns></returns>
        public void UpdateButton(string name, bool downOrUp) { }

        /// <summary>
        /// 更新轴
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public void UpdateAxis(string name, float value) { }

        #endregion
    }
}
