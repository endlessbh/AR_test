using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Caches;
using XCSJ.Collections;

namespace XCSJ.Extension.Base.Inputs
{
    /// <summary>
    /// 输入：包含所有的输入类型；与<see cref="EInput.XInput"/>对应的真实输入对象一致；
    /// </summary>
    public static class XInput
    {
        /// <summary>
        /// 静态构造函数
        /// </summary>
        static XInput()
        {
            input = EInput.XInput.GetInput();
        }

        /// <summary>
        /// 输入对象；管理所有有效输入的输入器
        /// </summary>
        public static IInput input { get; private set; }

        /// <summary>
        /// 鼠标位置
        /// </summary>
        public static Vector3 mousePosition => Input.mousePosition;

        /// <summary>
        /// 任意按键按下
        /// </summary>
        public static bool anyKeyDown => Input.anyKeyDown;

        /// <summary>
        /// 触摸数目
        /// </summary>
        public static int touchCount => Input.touchCount;

        #region IInput接口同名函数

        /// <summary>
        /// 按键是否触发，只要当前键按下状态未改变，一直为true
        /// </summary>
        /// <param name="buttonName"></param>
        /// <returns></returns>
        public static bool GetButton(string buttonName) => input.GetButton(buttonName);

        /// <summary>
        /// 按键是否按下
        /// </summary>
        /// <param name="buttonName"></param>
        /// <returns></returns>
        public static bool GetButtonDown(string buttonName) => input.GetButtonDown(buttonName);

        /// <summary>
        /// 按键是否弹起
        /// </summary>
        /// <param name="buttonName"></param>
        /// <returns></returns>
        public static bool GetButtonUp(string buttonName) => input.GetButtonUp(buttonName);

        /// <summary>
        /// 获取滑动轴的值
        /// </summary>
        /// <param name="axisName"></param>
        /// <returns></returns>
        public static float GetAxis(string axisName) => input.GetAxis(axisName);

        /// <summary>
        /// 获取滑动轴的值,不连续的值
        /// </summary>
        /// <param name="axisName"></param>
        /// <returns></returns>
        public static float GetAxisRaw(string axisName) => input.GetAxisRaw(axisName);

        /// <summary>
        /// 按键是否有操作
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool GetKey(string keyName) => input.GetKey(keyName);

        /// <summary>
        /// 按键是否有操作
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool GetKey(KeyCode key) => input.GetKey(key);

        /// <summary>
        /// 按键是否按下
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool GetKeyDown(string keyName) => input.GetKeyDown(keyName);

        /// <summary>
        /// 按键是否按下
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool GetKeyDown(KeyCode key) => input.GetKeyDown(key);

        /// <summary>
        /// 按键是否弹起
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool GetKeyUp(string keyName) => input.GetKeyUp(keyName);

        /// <summary>
        /// 按键是否弹起
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool GetKeyUp(KeyCode key) => input.GetKeyUp(key);

        /// <summary>
        /// 检测鼠标按键是否有操作
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        public static bool GetMouseButton(int button) => input.GetMouseButton(button);

        /// <summary>
        /// 鼠标按键是否按下
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        public static bool GetMouseButtonDown(int button) => input.GetMouseButtonDown(button);

        /// <summary>
        /// 鼠标按键是否弹起
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        public static bool GetMouseButtonUp(int button) => input.GetMouseButtonUp(button);

        /// <summary>
        /// 更新按钮
        /// </summary>
        /// <param name="name"></param>
        /// <param name="downOrUp"></param>
        public static void UpdateButton(string name, bool downOrUp) => input.UpdateButton(name, downOrUp);

        /// <summary>
        /// 更新轴
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public static void UpdateAxis(string name, float value) => input.UpdateAxis(name, value);

        #endregion
    }
}
