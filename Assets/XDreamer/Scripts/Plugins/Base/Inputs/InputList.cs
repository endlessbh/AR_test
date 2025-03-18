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
    /// 输入链表用于管理所有注册的输入
    /// </summary>
    public class InputList : IInput
    {
        #region 输入链表

        private List<IInput> _inputs = new List<IInput>();

        /// <summary>
        /// 输入列表
        /// </summary>
        public IInput[] inputs => _inputs.ToArray();

        /// <summary>
        /// 构造
        /// </summary>
        public InputList() { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="input"></param>
        public InputList(EInput input)
        {
            Register(input);
            _name = nameof(InputList) + ":" + _inputs.ToString(i => i.name);
        }

        /// <summary>
        /// 注册输入
        /// </summary>
        /// <param name="input"></param>
        public void Register(EInput input)
        {
            Register(input.GetInstanceInputs());
        }

        /// <summary>
        /// 注册输入
        /// </summary>
        /// <param name="input"></param>
        public void Register(params IInput[] inputs)
        {
            if (inputs == null) return;
            _inputs.AddRangeWithDistinct(inputs);
        }

        /// <summary>
        /// 注册输入
        /// </summary>
        /// <param name="input"></param>
        public void Register(IEnumerable<IInput> inputs)
        {
            if (inputs == null) return;
            _inputs.AddRangeWithDistinct(inputs);
        }

        /// <summary>
        /// 取消注册输入
        /// </summary>
        /// <param name="input"></param>
        public void Unregister(IInput input)
        {
            if (input == null) return;
            _inputs.Remove(input);
        }

        #endregion

        #region IInput

        /// <summary>
        /// 输入
        /// </summary>
        public EInput input
        {
            get
            {
                var result = default(EInput);
                _inputs.ForEach(i => result |= i.input);
                return result;
            }
        }

        private string _name = nameof(InputList);

        /// <summary>
        /// 名称
        /// </summary>
        public string name { get => _name; set { } }

        /// <summary>
        /// 任意按键
        /// </summary>
        public bool anyKey => _inputs.Any(i => i.anyKey);

        /// <summary>
        /// 任意按键按下
        /// </summary>
        public bool anyKeyDown => _inputs.Any(i => i.anyKeyDown);

        /// <summary>
        /// 按键是否触发，只要当前键按下状态未改变，一直为true
        /// </summary>
        /// <param name="buttonName"></param>
        /// <returns></returns>
        public bool GetButton(string buttonName) => _inputs.Any(i => i.GetButton(buttonName));

        /// <summary>
        /// 按键是否按下
        /// </summary>
        /// <param name="buttonName"></param>
        /// <returns></returns>
        public bool GetButtonDown(string buttonName) => _inputs.Any(i => i.GetButtonDown(buttonName));

        /// <summary>
        /// 按键是否弹起
        /// </summary>
        /// <param name="buttonName"></param>
        /// <returns></returns>
        public bool GetButtonUp(string buttonName) => _inputs.Any(i => i.GetButtonUp(buttonName));

        /// <summary>
        /// 获取滑动轴的值
        /// </summary>
        /// <param name="axisName"></param>
        /// <returns></returns>
        public float GetAxis(string axisName) => _inputs.Sum(i => i.GetAxis(axisName));

        /// <summary>
        /// 获取滑动轴的值,不连续的值
        /// </summary>
        /// <param name="axisName"></param>
        /// <returns></returns>
        public float GetAxisRaw(string axisName) =>_inputs.Sum(i => i.GetAxisRaw(axisName));

        /// <summary>
        /// 按键是否有操作
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool GetKey(string keyName) => _inputs.Any(i => i.GetKey(keyName));

        /// <summary>
        /// 按键是否有操作
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool GetKey(KeyCode key) => _inputs.Any(i => i.GetKey(key));

        /// <summary>
        /// 按键是否按下
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool GetKeyDown(string keyName) => _inputs.Any(i => i.GetKeyDown(keyName));

        /// <summary>
        /// 按键是否按下
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool GetKeyDown(KeyCode key) => _inputs.Any(i => i.GetKeyDown(key));

        /// <summary>
        /// 按键是否弹起
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool GetKeyUp(string keyName) => _inputs.Any(i => i.GetKeyUp(keyName));

        /// <summary>
        /// 按键是否弹起
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool GetKeyUp(KeyCode key) => _inputs.Any(i => i.GetKeyUp(key));

        /// <summary>
        /// 检测鼠标按键是否有操作
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        public bool GetMouseButton(int button) => _inputs.Any(i => i.GetMouseButton(button));

        /// <summary>
        /// 鼠标按键是否按下
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        public bool GetMouseButtonDown(int button) => _inputs.Any(i => i.GetMouseButtonDown(button));

        /// <summary>
        /// 鼠标按键是否弹起
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        public bool GetMouseButtonUp(int button) => _inputs.Any(i => i.GetMouseButtonUp(button));

        /// <summary>
        /// 更新按钮
        /// </summary>
        /// <param name="name"></param>
        /// <param name="downOrUp"></param>
        public void UpdateButton(string name, bool downOrUp)
        {
            foreach(var i in _inputs)
            {
                i.UpdateButton(name, downOrUp);
            }
        }

        /// <summary>
        /// 更新轴
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void UpdateAxis(string name, float value)
        {
            foreach (var i in _inputs)
            {
                i.UpdateAxis(name, value);
            }
        }

        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            foreach (var i in _inputs)
            {
                i.Reset();
            }
        }

        #endregion
    }
}
