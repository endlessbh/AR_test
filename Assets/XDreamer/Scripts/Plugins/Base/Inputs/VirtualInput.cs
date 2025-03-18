using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;

namespace XCSJ.Extension.Base.Inputs
{
    /// <summary>
    /// 虚拟输入：模拟各种输入事件；可更新；属于公用的虚拟输入；与<see cref="EInput.VirtualInput"/>对应的真实输入对象一致；
    /// </summary>
    [Name("虚拟输入")]
    [Tip("模拟各种输入事件", "Simulate various input events")]
    public class VirtualInput : VirtualInput<VirtualInput>
    {
        /// <summary>
        /// 输入
        /// </summary>
        public override EInput input => EInput.VirtualInput;

        /// <summary>
        /// 名称
        /// </summary>
        public override string name { get => nameof(VirtualInput); set { } }
    }

    /// <summary>
    /// 相机输入:模拟相机控制所需的各种输入事件；可更新；属于相机专用的虚拟输入；与<see cref="EInput.CameraInput"/>对应的真实输入对象一致；
    /// </summary>
    [Name("相机输入")]
    [Tip("模拟相机控制所需的各种输入事件；可更新；属于相机专用的虚拟输入；", "Simulate various input events required for camera control; renewable; Virtual input dedicated to camera;")]
    public class CameraInput : VirtualInput<CameraInput>
    {
        /// <summary>
        /// 输入
        /// </summary>
        public override EInput input => EInput.CameraInput;

        /// <summary>
        /// 名称
        /// </summary>
        public override string name { get => nameof(CameraInput); set { } }
    }

    /// <summary>
    /// 角色输入:模拟角色控制所需的各种输入事件；可更新；属于角色专用的虚拟输入；与<see cref="EInput.CharacterInput"/>对应的真实输入对象一致；
    /// </summary>
    [Name("角色输入")]
    [Tip("模拟角色控制所需的各种输入事件；可更新；属于角色专用的虚拟输入；", "Simulate various input events required by role control; renewable; Virtual input dedicated to the role;")]
    public class CharacterInput : VirtualInput<CharacterInput>
    {
        /// <summary>
        /// 输入
        /// </summary>
        public override EInput input => EInput.CharacterInput;

        /// <summary>
        /// 名称
        /// </summary>
        public override string name { get => nameof(CharacterInput); set { } }
    }

    /// <summary>
    /// 车辆输入:模拟车辆控制所需的各种输入事件；可更新；属于车辆专用的虚拟输入；与<see cref="EInput.VehicleInput"/>对应的真实输入对象一致；
    /// </summary>
    [Name("车辆输入")]
    [Tip("模拟车辆控制所需的各种输入事件；可更新；属于车辆专用的虚拟输入；", "Simulate various input events required for vehicle control; renewable; Virtual input dedicated to vehicle;")]
    public class VehicleInput : VirtualInput<VehicleInput>
    {
        /// <summary>
        /// 输入
        /// </summary>
        public override EInput input => EInput.VehicleInput;

        /// <summary>
        /// 名称
        /// </summary>
        public override string name { get => nameof(VehicleInput); set { } }
    }

    /// <summary>
    /// 虚拟输入控制雷系
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class VirtualInput<T> : InstanceClass<T>, IInput
        where T : VirtualInput<T>, new()
    {
        #region 轴Axis

        protected Dictionary<string, VirtualAxis> virtualAxes = new Dictionary<string, VirtualAxis>();

        public VirtualAxis AddAxis(string name)
        {
            if (!virtualAxes.TryGetValue(name, out VirtualAxis axis) || axis == null)
            {
                virtualAxes[name] = axis = new VirtualAxis(name);
            }
            return axis;
        }

        public bool TryGetAxis(string name, out VirtualAxis axis) => virtualAxes.TryGetValue(name, out axis);

        public bool RemoveAxis(string name) => virtualAxes.Remove(name);

        public void UpdateAxis(string name, float value)
        {
            if (AddAxis(name) is VirtualAxis virtualAxis)
            {
                virtualAxis.value = value;
            }
        }

        #endregion

        #region 按钮Button

        protected Dictionary<string, VirtualButton> virtualButtons = new Dictionary<string, VirtualButton>();

        public VirtualButton AddButton(string name)
        {
            if (!virtualButtons.TryGetValue(name, out VirtualButton button) || button == null)
            {
                virtualButtons[name] = button = new VirtualButton(name);
            }
            return button;
        }

        public bool TryGetButton(string name, out VirtualButton button) => virtualButtons.TryGetValue(name, out button);

        public bool RemoveButton(string name) => virtualButtons.Remove(name);

        public void UpdateButton(string name, bool downOrUp)
        {
            if (AddButton(name) is VirtualButton virtualButton)
            {
                virtualButton.Update(downOrUp);
            }
        }

        #endregion

        #region IInput接口

        /// <summary>
        /// 输入
        /// </summary>
        public abstract EInput input { get; }

        /// <summary>
        /// 名称
        /// </summary>
        public abstract string name { get; set; }

        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            foreach (var kv in virtualAxes)
            {
                kv.Value.Reset();
            }
            foreach (var kv in virtualButtons)
            {
                kv.Value.Reset();
            }
        }

        /// <summary>
        /// 任意按键
        /// </summary>
        public bool anyKey => virtualButtons.Any(kv => kv.Value.isKeeping);

        /// <summary>
        /// 任意按键按下
        /// </summary>
        public bool anyKeyDown => virtualButtons.Any(kv => kv.Value.isDown);

        /// <summary>
        /// 按键是否触发，只要当前键按下状态未改变，一直为true
        /// </summary>
        /// <param name="buttonName"></param>
        /// <returns></returns>
        public bool GetButton(string buttonName)
        {
            if (TryGetButton(buttonName, out var button))
            {
                return button.isKeeping;
            }
            return false;
        }

        /// <summary>
        /// 按键是否按下
        /// </summary>
        /// <param name="buttonName"></param>
        /// <returns></returns>
        public bool GetButtonDown(string buttonName)
        {
            if (TryGetButton(buttonName, out var button))
            {
                return button.isDown;
            }
            return false;
        }

        /// <summary>
        /// 按键是否弹起
        /// </summary>
        /// <param name="buttonName"></param>
        /// <returns></returns>
        public bool GetButtonUp(string buttonName)
        {
            if (TryGetButton(buttonName, out var button))
            {
                return button.isUp;
            }
            return false;
        }

        /// <summary>
        /// 获取滑动轴的值
        /// </summary>
        /// <param name="axisName"></param>
        /// <returns></returns>
        public float GetAxis(string axisName)
        {
            if (TryGetAxis(axisName, out var axis))
            {
                return axis.value;
            }
            return 0;
        }

        /// <summary>
        /// 获取滑动轴的值,不连续的值，与<see cref="GetAxis"/>获取结果一致
        /// </summary>
        /// <param name="axisName"></param>
        /// <returns></returns>
        public float GetAxisRaw(string axisName) => GetAxis(axisName);

        /// <summary>
        /// 按键是否有操作
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool GetKey(string keyName)
        {
            if (TryGetButton(keyName, out var button))
            {
                return button.isKeeping;
            }
            return false;
        }

        /// <summary>
        /// 按键是否有操作
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool GetKey(KeyCode key) => GetKey(key.ToString());

        /// <summary>
        /// 按键是否按下
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool GetKeyDown(string keyName)
        {
            if (TryGetButton(keyName, out var button))
            {
                return button.isDown;
            }
            return false;
        }

        /// <summary>
        /// 按键是否按下
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool GetKeyDown(KeyCode key) => GetKeyDown(key.ToString());

        /// <summary>
        /// 按键是否弹起
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool GetKeyUp(string keyName)
        {
            if (TryGetButton(keyName, out var button))
            {
                return button.isUp;
            }
            return false;
        }

        /// <summary>
        /// 按键是否弹起
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool GetKeyUp(KeyCode key) => GetKeyUp(key.ToString());

        /// <summary>
        /// 检测鼠标按键是否有操作
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        public bool GetMouseButton(int button) => GetKey(button.MouseButtonToKeyCode());

        /// <summary>
        /// 鼠标按键是否按下
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        public bool GetMouseButtonDown(int button) => GetKeyDown(button.MouseButtonToKeyCode());

        /// <summary>
        /// 鼠标按键是否弹起
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        public bool GetMouseButtonUp(int button) => GetKeyUp(button.MouseButtonToKeyCode());

        #endregion
    }

    /// <summary>
    /// 虚拟数据
    /// </summary>
    public abstract class VirtualData
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string name { get; private set; }

        /// <summary>
        /// 标签
        /// </summary>
        public object tag { get; set; }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="name"></param>
        public VirtualData(string name)
        {
            this.name = name;
        }

        /// <summary>
        /// 重置
        /// </summary>
        public abstract void Reset();
    }

    /// <summary>
    /// 虚拟轴
    /// </summary>
    public class VirtualAxis : VirtualData
    {
        /// <summary>
        /// 值
        /// </summary>
        public float value = 0;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="name"></param>
        public VirtualAxis(string name) : base(name) { }

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            value = 0;
        }
    }

    /// <summary>
    /// 虚拟按钮
    /// </summary>
    public class VirtualButton : VirtualData
    {
        private int lastPressed = -10;

        private int lastReleased = -10;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="name"></param>
        public VirtualButton(string name) : base(name) { }

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            lastPressed = -10;
            lastReleased = -10;
            isKeeping = false;
        }

        /// <summary>
        /// 是否按下
        /// </summary>
        public bool isDown => lastPressed + 1 == Time.frameCount;

        /// <summary>
        /// 是否按下保持中
        /// </summary>
        public bool isKeeping { get; private set; } = false;

        /// <summary>
        /// 是否抬起
        /// </summary>
        public bool isUp => lastReleased + 1 == Time.frameCount;

        /// <summary>
        /// 按钮按压时调用，对应Down事件
        /// </summary>
        public void Pressed()
        {
            if (isKeeping) return;

            isKeeping = true;
            lastPressed = Time.frameCount;
        }

        /// <summary>
        /// 按钮释放时调用，对应Up事件
        /// </summary>
        public void Released()
        {
            if (isKeeping)
            {
                isKeeping = false;
                lastReleased = Time.frameCount;
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="pressedOrReleased"></param>
        public void Update(bool pressedOrReleased)
        {
            if (pressedOrReleased) Pressed();
            else Released();
        }
    }
}
