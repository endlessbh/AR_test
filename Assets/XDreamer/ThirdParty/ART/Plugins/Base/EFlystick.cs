using System;
using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Caches;
using XCSJ.Interfaces;
using XCSJ.PluginART.Tools;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginART.Base
{
    /// <summary>
    /// Flystick枚举
    /// </summary>
    [Name("Flystick")]
    public enum EFlystick
    {
        /// <summary>
        /// 无
        /// </summary>
        [Name("无")]
        None,

        /// <summary>
        /// Flystick1
        /// </summary>
        [Name("Flystick1")]
        Flystick1,

        /// <summary>
        /// Flystick2
        /// </summary>
        [Name("Flystick2")]
        Flystick2,

        /// <summary>
        /// Flystick3
        /// </summary>
        [Name("Flystick3")]
        Flystick3,
    }

    /// <summary>
    /// Flystick1开关
    /// </summary>
    [Name("Flystick1开关")]
    public enum EFlystick1Switchs
    {
        /// <summary>
        /// 任意
        /// </summary>
        [Name("任意")]
        Any = -1,

        /// <summary>
        /// 无
        /// </summary>
        [Name("无")]
        None,

        /// <summary>
        /// 前开关(红色):可称开关或按钮
        /// </summary>
        [Name("前开关(红色)")]
        [Tip("可称开关或按钮", "It can be called switch or button")]
        [ID6df(0)]
        [ID6df2(0)]
        [Code6df(0x01)]
        [Code6df2(0x01)]
        FrontSwitch,

        /// <summary>
        /// 右侧背面开关(红色):可称开关或按钮
        /// </summary>
        [Name("右侧背面开关(红色)")]
        [Tip("可称开关或按钮", "It can be called switch or button")]
        [ID6df(1)]
        [ID6df2(1)]
        [Code6df(0x02)]
        [Code6df2(0x02)]
        RightSwitchOnBackside,

        /// <summary>
        /// 背面中间开关(红色):可称开关或按钮
        /// </summary>
        [Name("背面中间开关(红色)")]
        [Tip("可称开关或按钮", "It can be called switch or button")]
        [ID6df(2)]
        [ID6df2(2)]
        [Code6df(0x04)]
        [Code6df2(0x04)]
        CenterSwitchOnBackside,

        /// <summary>
        /// 左侧背面开关(红色):可称开关或按钮
        /// </summary>
        [Name("左侧背面开关(红色)")]
        [Tip("可称开关或按钮", "It can be called switch or button")]
        [ID6df(3)]
        [ID6df2(3)]
        [Code6df(0x08)]
        [Code6df2(0x08)]
        LeftSwitchOnBackside,

        /// <summary>
        /// 帽子开关(黑色)左:第一控制器，模拟控制数值[0.0, 1.0]
        /// </summary>
        [Name("帽子开关(黑色)左")]
        [Tip("第一控制器，模拟控制数值[0.0, 1.0]", "First controller, analog control value [0.0, 1.0]")]
        [ID6df(4)]
        [ID6df2(4)]
        [Code6df(0x20)]
        [Code6df2(0x20)]
        HatSwitchLeft,

        /// <summary>
        /// 帽子开关(黑色)右:第一控制器，模拟控制数值[-1.0, 0.0]
        /// </summary>
        [Name("帽子开关(黑色)右")]
        [Tip("第一控制器，模拟控制数值[-1.0, 0.0]", "First controller, analog control value [- 1.0, 0.0]")]
        [ID6df(5)]
        [ID6df2(5)]
        [Code6df(0x80)]
        [Code6df2(0x80)]
        HatSwitchRight,

        /// <summary>
        /// 帽子开关(黑色)上:第二控制器，模拟控制数值[0.0, 1.0]
        /// </summary>
        [Name("帽子开关(黑色)上")]
        [Tip("第二控制器，模拟控制数值[0.0, 1.0]", "Second controller, analog control value [0.0, 1.0]")]
        [ID6df(6)]
        [ID6df2(6)]
        [Code6df(0x40)]
        [Code6df2(0x40)]
        HatSwitchUp,

        /// <summary>
        /// 帽子开关(黑色)下:第二控制器，模拟控制数值[-1.0, 0.0]
        /// </summary>
        [Name("帽子开关(黑色)下")]
        [Tip("第二控制器，模拟控制数值[-1.0, 0.0]", "Second controller, analog control value [- 1.0, 0.0]")]
        [ID6df(7)]
        [ID6df2(7)]
        [Code6df(0x10)]
        [Code6df2(0x10)]
        HatSwitchDown,
    }

    /// <summary>
    /// Flystick2开关
    /// </summary>
    [Name("Flystick2开关")]
    public enum EFlystick2Switchs
    {
        /// <summary>
        /// 任意
        /// </summary>
        [Name("任意")]
        Any = -1,

        /// <summary>
        /// 无
        /// </summary>
        [Name("无")]
        None,

        /// <summary>
        /// 前开关(黄色):可称开关或按钮
        /// </summary>
        [Name("前开关(黄色)")]
        [Tip("可称开关或按钮")]
        [ID6df(0)]
        [ID6df2(0)]
        [Code6df(0x01)]
        [Code6df2(0x01)]
        FrontSwitch,

        /// <summary>
        /// 背面外面右侧开关(蓝色):可称开关或按钮
        /// </summary>
        [Name("背面外面右侧开关(蓝色)")]
        [Tip("可称开关或按钮")]
        [ID6df(1)]
        [ID6df2(1)]
        [Code6df(0x02)]
        [Code6df2(0x02)]
        OuterRightSwitchOnBackside,

        /// <summary>
        /// 背面内部右侧开关(蓝色):可称开关或按钮
        /// </summary>
        [Name("背面内部右侧开关(蓝色)")]
        [Tip("可称开关或按钮")]
        [ID6df(2)]
        [ID6df2(2)]
        [Code6df(0x04)]
        [Code6df2(0x04)]
        InnerRightSwitchOnBackside,

        /// <summary>
        /// 背面内部左侧开关(蓝色):可称开关或按钮
        /// </summary>
        [Name("背面内部左侧开关(蓝色)")]
        [Tip("可称开关或按钮")]
        [ID6df(3)]
        [ID6df2(3)]
        [Code6df(0x08)]
        [Code6df2(0x08)]
        InnerLeftSwitchOnBackside,

        /// <summary>
        /// 背面外面左侧开关(蓝色):可称开关或按钮
        /// </summary>
        [Name("背面外面左侧开关(蓝色)")]
        [Tip("可称开关或按钮")]
        [ID6df(4)]
        [ID6df2(4)]
        [Code6df()]
        [Code6df2(0x10)]
        OuterLeftSwitchOnBackside,

        /// <summary>
        /// 摇杆上的按钮(黄色):可称开关或按钮
        /// </summary>
        [Name("摇杆上的按钮(黄色)")]
        [Tip("可称开关或按钮")]
        [ID6df(5)]
        [ID6df2(5)]
        [Code6df()]
        [Code6df2(0x20)]
        SwitchOnJoystick,

        /// <summary>
        /// 摇杆左(黄色):第一控制器，模拟控制数值[0.0, 1.0]
        /// </summary>
        [Name("摇杆左(黄色)")]
        [Tip("第一控制器，模拟控制数值[0.0, 1.0]")]
        [ID6df(0, buttonOrJoystick = false, joystickValueRangeLeft = 0, joystickValueRangeRight = 1)]
        [ID6df2(0, buttonOrJoystick = false, joystickValueRangeLeft = 0, joystickValueRangeRight = 1)]
        [Code6df(0x20)]
        [Code6df2()]
        JoystickLeft,

        /// <summary>
        /// 摇杆右(黄色):第一控制器，模拟控制数值[-1.0, 0.0]
        /// </summary>
        [Name("摇杆右(黄色)")]
        [Tip("第一控制器，模拟控制数值[-1.0, 0.0]")]
        [ID6df(0, buttonOrJoystick = false, joystickValueRangeLeft = -1, joystickValueRangeRight = 0)]
        [ID6df2(0, buttonOrJoystick = false, joystickValueRangeLeft = -1, joystickValueRangeRight = 0)]
        [Code6df(0x80)]
        [Code6df2()]
        JoystickRight,

        /// <summary>
        /// 摇杆上(黄色):第二控制器，模拟控制数值[0.0, 1.0]
        /// </summary>
        [Name("摇杆上(黄色)")]
        [Tip("第二控制器，模拟控制数值[0.0, 1.0]")]
        [ID6df(1, buttonOrJoystick = false, joystickValueRangeLeft = 0, joystickValueRangeRight = 1)]
        [ID6df2(1, buttonOrJoystick = false, joystickValueRangeLeft = 0, joystickValueRangeRight = 1)]
        [Code6df(0x40)]
        [Code6df2()]
        JoystickUp,

        /// <summary>
        /// 摇杆下(黄色):第二控制器，模拟控制数值[-1.0, 0.0]
        /// </summary>
        [Name("摇杆下(黄色)")]
        [Tip("第二控制器，模拟控制数值[-1.0, 0.0]")]
        [ID6df(1, buttonOrJoystick = false, joystickValueRangeLeft = -1, joystickValueRangeRight = 0)]
        [ID6df2(1, buttonOrJoystick = false, joystickValueRangeLeft = -1, joystickValueRangeRight = 0)]
        [Code6df(0x10)]
        [Code6df2()]
        JoystickDown,
    }

    /// <summary>
    /// Flystick3开关
    /// </summary>
    [Name("Flystick3开关")]
    public enum EFlystick3Switchs
    {
        /// <summary>
        /// 任意
        /// </summary>
        [Name("任意")]
        Any = -1,

        /// <summary>
        /// 无
        /// </summary>
        [Name("无")]
        None,

        /// <summary>
        /// 上方开关(黄色):可称开关或按钮
        /// </summary>
        [Name("上方开关(黄色)")]
        [Tip("可称开关或按钮")]
        [ID6df(0)]
        [ID6df2(0)]
        [Code6df(0x01)]
        [Code6df2()]
        BottomSwitch,

        /// <summary>
        /// 上部右边开关(蓝色):可称开关或按钮
        /// </summary>
        [Name("上部右边开关(蓝色)")]
        [Tip("可称开关或按钮")]
        [ID6df(1)]
        [ID6df2(1)]
        [Code6df(0x02)]
        [Code6df2()]
        TopRightSwitch,

        /// <summary>
        /// 上部中间开关(蓝色):可称开关或按钮
        /// </summary>
        [Name("上部中间开关(蓝色)")]
        [Tip("可称开关或按钮")]
        [ID6df(2)]
        [ID6df2(2)]
        [Code6df(0x04)]
        [Code6df2()]
        TopMiddleSwitch,

        /// <summary>
        /// 上部左边开关(蓝色):可称开关或按钮
        /// </summary>
        [Name("上部左边开关(蓝色)")]
        [Tip("可称开关或按钮")]
        [ID6df(3)]
        [ID6df2(3)]
        [Code6df(0x10)]
        [Code6df2()]
        TopLeftSwitch,

        /// <summary>
        /// 摇杆左(黄色):第一控制器，模拟控制数值[0.0, 1.0]
        /// </summary>
        [Name("摇杆左(黄色)")]
        [Tip("第一控制器，模拟控制数值[0.0, 1.0]")]
        [ID6df(0, buttonOrJoystick = false, joystickValueRangeLeft = 0, joystickValueRangeRight = 1)]
        [ID6df2(0, buttonOrJoystick = false, joystickValueRangeLeft = 0, joystickValueRangeRight = 1)]
        [Code6df(0x20)]
        [Code6df2()]
        JoystickLeft,

        /// <summary>
        /// 摇杆右(黄色):第一控制器，模拟控制数值[-1.0, 0.0]
        /// </summary>
        [Name("摇杆右(黄色)")]
        [Tip("第一控制器，模拟控制数值[-1.0, 0.0]")]
        [ID6df(0, buttonOrJoystick = false, joystickValueRangeLeft = -1, joystickValueRangeRight = 0)]
        [ID6df2(0, buttonOrJoystick = false, joystickValueRangeLeft = -1, joystickValueRangeRight = 0)]
        [Code6df(0x80)]
        [Code6df2()]
        JoystickRight,

        /// <summary>
        /// 摇杆上(黄色):第二控制器，模拟控制数值[0.0, 1.0]
        /// </summary>
        [Name("摇杆上(黄色)")]
        [Tip("第二控制器，模拟控制数值[0.0, 1.0]")]
        [ID6df(1, buttonOrJoystick = false, joystickValueRangeLeft = 0, joystickValueRangeRight = 1)]
        [ID6df2(1, buttonOrJoystick = false, joystickValueRangeLeft = 0, joystickValueRangeRight = 1)]
        [Code6df(0x40)]
        [Code6df2()]
        JoystickUp,

        /// <summary>
        /// 摇杆下(黄色):第二控制器，模拟控制数值[-1.0, 0.0]
        /// </summary>
        [Name("摇杆下(黄色)")]
        [Tip("第二控制器，模拟控制数值[-1.0, 0.0]")]
        [ID6df(1, buttonOrJoystick = false, joystickValueRangeLeft = -1, joystickValueRangeRight = 0)]
        [ID6df2(1, buttonOrJoystick = false, joystickValueRangeLeft = -1, joystickValueRangeRight = 0)]
        [Code6df(0x10)]
        [Code6df2()]
        JoystickDown,
    }

    /// <summary>
    /// 值特性
    /// </summary>
    public class ValueAttribute : Attribute
    {
        /// <summary>
        /// 值
        /// </summary>
        public int value { get; private set; }

        /// <summary>
        /// 按钮或摇杆
        /// </summary>
        public bool buttonOrJoystick { get; set; } = true;

        /// <summary>
        /// 摇杆值范围左
        /// </summary>
        public float joystickValueRangeLeft { get; set; } = 0;

        /// <summary>
        /// 摇杆值范围左
        /// </summary>
        public float joystickValueRangeRight { get; set; } = 1;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="id"></param>
        public ValueAttribute(int id)
        {
            this.value = id;
        }
    }

    /// <summary>
    /// ID 6df 特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class ID6dfAttribute : ValueAttribute
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="id"></param>
        public ID6dfAttribute(int id = FlystickHelper.InvalidValue) : base(id) { }
    }

    /// <summary>
    /// 代码 6df 特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class Code6dfAttribute : ValueAttribute
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="id"></param>
        public Code6dfAttribute(int id = FlystickHelper.InvalidValue) : base(id) { }
    }

    /// <summary>
    /// ID 6df2 特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class ID6df2Attribute : ValueAttribute
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="id"></param>
        public ID6df2Attribute(int id = FlystickHelper.InvalidValue) : base(id) { }
    }

    /// <summary>
    /// 代码 6df2 特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class Code6df2Attribute : ValueAttribute
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="id"></param>
        public Code6df2Attribute(int id = FlystickHelper.InvalidValue) : base(id) { }
    }

    /// <summary>
    /// Flystick辅助类
    /// </summary>
    public static class FlystickHelper
    {
        /// <summary>
        /// 无效值
        /// </summary>
        public const int InvalidValue = -1;

        /// <summary>
        /// 尝试获取值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="e"></param>
        /// <returns></returns>
        public static bool TryGetValue<T>(Enum e, out int value) where T : ValueAttribute
        {
            value = AttributeCache<T>.GetOfField(e) is T attribute ? attribute.value : InvalidValue;
            return value != InvalidValue;
        }

        /// <summary>
        /// 尝试获取ID
        /// </summary>
        /// <param name="flystick1Switchs"></param>
        /// <returns></returns>
        public static bool TryGetID_6df2(this EFlystick1Switchs flystick1Switchs, out int value) => TryGetValue<ID6df2Attribute>(flystick1Switchs, out value);

        /// <summary>
        /// 尝试获取ID
        /// </summary>
        /// <param name="flystick2Switchs"></param>
        /// <returns></returns>
        public static bool TryGetID_6df2(this EFlystick2Switchs flystick2Switchs, out int value) => TryGetValue<ID6df2Attribute>(flystick2Switchs, out value);

        /// <summary>
        /// 尝试获取ID
        /// </summary>
        /// <param name="flystick3Switchs"></param>
        /// <returns></returns>
        public static bool TryGetID_6df2(this EFlystick3Switchs flystick3Switchs, out int value) => TryGetValue<ID6df2Attribute>(flystick3Switchs, out value);

        /// <summary>
        /// 尝试获取代码
        /// </summary>
        /// <param name="flystick1Switchs"></param>
        /// <returns></returns>
        public static bool TryGetCode_6df2(this EFlystick1Switchs flystick1Switchs, out int value) => TryGetValue<Code6df2Attribute>(flystick1Switchs, out value);

        /// <summary>
        /// 尝试获取代码
        /// </summary>
        /// <param name="flystick2Switchs"></param>
        /// <returns></returns>
        public static bool TryGetCode_6df2(this EFlystick2Switchs flystick2Switchs, out int value) => TryGetValue<Code6df2Attribute>(flystick2Switchs, out value);

        /// <summary>
        /// 尝试获取代码
        /// </summary>
        /// <param name="flystick3Switchs"></param>
        /// <returns></returns>
        public static bool TryGetCode_6df2(this EFlystick3Switchs flystick3Switchs, out int value) => TryGetValue<Code6df2Attribute>(flystick3Switchs, out value);

        private static bool IsPressedInternal(Enum flystickButton, ARTStreamClient streamClient, int flystickID, float deadZone, out int pressedFlystickID, out int pressedFlystickSwitchs)
        {
            pressedFlystickID = -1;
            pressedFlystickSwitchs = -1;
            if (!streamClient) return false;

            switch (flystickID)
            {
                case -1://任意Flystick编号的事件
                    {
                        var flystickIDs = streamClient.GetAllFlyStickStateIDs();
                        int a = -1;
                        int b = -1;

                        if (flystickIDs.Any(id => IsPressedInternal(flystickButton, streamClient, id, deadZone, out a, out b)))
                        {
                            pressedFlystickID = a;
                            pressedFlystickSwitchs = b;
                            return true;
                        }
                        break;
                    }
                default:
                    {
                        if (streamClient.GetLatestFlyStickState(flystickID) is ARTFlyStickState data)
                        {
                            switch (flystickButton.GetHashCode())
                            {
                                case -1://任意Flystick按钮的事件
                                    {
                                        int a = -1;
                                        int b = -1;

                                        if (EnumCache.GetValues(flystickButton).Any(e =>
                                        {
                                            return e.GetHashCode() > 0 && IsPressedInternal(e, streamClient, flystickID, deadZone, out a, out b);
                                        }))
                                        {
                                            pressedFlystickID = a;
                                            pressedFlystickSwitchs = b;
                                            return true;
                                        }
                                        break;
                                    }
                                case 0: break;//无-不处理
                                default:
                                    {
                                        if (data.TryGetValue(flystickButton, out var value, deadZone))
                                        {
                                            pressedFlystickID = flystickID;
                                            pressedFlystickSwitchs = flystickButton.GetHashCode();
                                            return true;
                                        }
                                        break;
                                    }
                            }
                        }
                        break;
                    }
            }//end switch flystickID
            return false;
        }

        /// <summary>
        /// Flystick1按钮是否被按压中
        /// </summary>
        /// <param name="flystick1Switchs">Flystick1按钮</param>
        /// <param name="streamClient">ART流客户端</param>
        /// <param name="flystickID">从0开始的Flystick有效编号；如果值为-1标识任意Flystick输入设备；</param>
        /// <param name="deadZone">死区：对于摇杆的值超过本死区值时，认为事件成立；</param>
        /// <returns></returns>
        public static bool IsPressed1(this EFlystick1Switchs flystick1Switchs, ARTStreamClient streamClient, int flystickID, float deadZone)
        {
            return IsPressed(flystick1Switchs, streamClient, flystickID, deadZone, out _, out _);
        }

        /// <summary>
        /// Flystick1按钮是否被按压中
        /// </summary>
        /// <param name="flystick1Switchs">Flystick1按钮</param>
        /// <param name="streamClient">ART流客户端</param>
        /// <param name="flystickID">从0开始的Flystick有效编号；如果值为-1标识任意Flystick输入设备；</param>
        /// <param name="deadZone">死区：对于摇杆的值超过本死区值时，认为事件成立；</param>
        /// <param name="pressedFlystickID">被按压的Flystick编号</param>
        /// <param name="pressedFlystickSwitchs">被按压的Flystick按钮</param>
        /// <returns></returns>
        public static bool IsPressed(this EFlystick1Switchs flystick1Switchs, ARTStreamClient streamClient, int flystickID, float deadZone, out int pressedFlystickID, out int pressedFlystickSwitchs)
        {
            return IsPressedInternal(flystick1Switchs, streamClient, flystickID, deadZone, out pressedFlystickID, out pressedFlystickSwitchs);
        }

        /// <summary>
        /// Flystick2按钮是否被按压中
        /// </summary>
        /// <param name="flystick2Switchs">Flystick3按钮</param>
        /// <param name="streamClient">ART流客户端</param>
        /// <param name="flystickID">从0开始的Flystick有效编号；如果值为-1标识任意Flystick输入设备；</param>
        /// <param name="deadZone">死区：对于摇杆的值超过本死区值时，认为事件成立；</param>
        /// <returns></returns>
        public static bool IsPressed(this EFlystick2Switchs flystick2Switchs, ARTStreamClient streamClient, int flystickID, float deadZone)
        {
            return IsPressed(flystick2Switchs, streamClient, flystickID, deadZone, out _, out _);
        }

        /// <summary>
        /// Flystick2按钮是否被按压中
        /// </summary>
        /// <param name="flystick2Switchs">Flystick2按钮</param>
        /// <param name="streamClient">ART流客户端</param>
        /// <param name="flystickID">从0开始的Flystick有效编号；如果值为-1标识任意Flystick输入设备；</param>
        /// <param name="deadZone">死区：对于摇杆的值超过本死区值时，认为事件成立；</param>
        /// <param name="pressedFlystickID">被按压的Flystick编号</param>
        /// <param name="pressedFlystickSwitchs">被按压的Flystick按钮</param>
        /// <returns></returns>
        public static bool IsPressed(this EFlystick2Switchs flystick2Switchs, ARTStreamClient streamClient, int flystickID, float deadZone, out int pressedFlystickID, out int pressedFlystickSwitchs)
        {
            return IsPressedInternal(flystick2Switchs, streamClient, flystickID, deadZone, out pressedFlystickID, out pressedFlystickSwitchs);
        }

        /// <summary>
        /// Flystick3按钮是否被按压中
        /// </summary>
        /// <param name="flystick3Switchs">Flystick3按钮</param>
        /// <param name="streamClient">ART流客户端</param>
        /// <param name="flystickID">从0开始的Flystick有效编号；如果值为-1标识任意Flystick输入设备；</param>
        /// <param name="deadZone">死区：对于摇杆的值超过本死区值时，认为事件成立；</param>
        /// <returns></returns>
        public static bool IsPressed(this EFlystick3Switchs flystick3Switchs, ARTStreamClient streamClient, int flystickID, float deadZone)
        {
            return IsPressed(flystick3Switchs, streamClient, flystickID, deadZone, out _, out _);
        }

        /// <summary>
        /// Flystick1按钮是否被按压中
        /// </summary>
        /// <param name="flystick3Switchs">Flystick1按钮</param>
        /// <param name="streamClient">ART流客户端</param>
        /// <param name="flystickID">从0开始的Flystick有效编号；如果值为-1标识任意Flystick输入设备；</param>
        /// <param name="deadZone">死区：对于摇杆的值超过本死区值时，认为事件成立；</param>
        /// <param name="pressedFlystickID">被按压的Flystick编号</param>
        /// <param name="pressedFlystickSwitchs">被按压的Flystick按钮</param>
        /// <returns></returns>
        public static bool IsPressed(this EFlystick3Switchs flystick3Switchs, ARTStreamClient streamClient, int flystickID, float deadZone, out int pressedFlystickID, out int pressedFlystickSwitchs)
        {
            return IsPressedInternal(flystick3Switchs, streamClient, flystickID, deadZone, out pressedFlystickID, out pressedFlystickSwitchs);
        }

        /// <summary>
        /// 获取Flystick按钮枚举
        /// </summary>
        /// <param name="flystick"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Enum GetFlystickSwitchs(this EFlystick flystick, int id)
        {
            switch (flystick)
            {
                case EFlystick.Flystick1: return (EFlystick1Switchs)id;
                case EFlystick.Flystick3: return (EFlystick3Switchs)id;
                case EFlystick.Flystick2:
                default: return (EFlystick2Switchs)id;
            }
        }
    }

    /// <summary>
    /// Flystick按钮
    /// </summary>
    [Serializable]
    [Name("Flystick按钮")]
    public class FlystickButton : IToFriendlyString
    {
        /// <summary>
        /// Flystick编号：从0开始的Flystick编号；如果值为-1标识任意Flystick输入设备；
        /// </summary>
        [Name("Flystick编号")]
        [Tip("从0开始的Flystick有效编号；如果值为-1标识任意Flystick输入设备；")]
        [Range(-1, 4)]
        public int _flysitckID = 0;

        /// <summary>
        /// Flystick手柄
        /// </summary>
        [Name("Flystick手柄")]
        [EnumPopup]
        public EFlystick _flystick = EFlystick.Flystick2;

        /// <summary>
        /// Flystick1按钮
        /// </summary>
        [Name("Flystick1按钮")]
        [EnumPopup]
        [HideInSuperInspector(nameof(_flystick), EValidityCheckType.NotEqual, EFlystick.Flystick1)]
        public EFlystick1Switchs _flystick1Switchs = EFlystick1Switchs.FrontSwitch;

        /// <summary>
        /// Flystick2按钮
        /// </summary>
        [Name("Flystick2按钮")]
        [EnumPopup]
        [HideInSuperInspector(nameof(_flystick), EValidityCheckType.NotEqual, EFlystick.Flystick2)]
        public EFlystick2Switchs _flystick2Switchs = EFlystick2Switchs.FrontSwitch;

        /// <summary>
        /// Flystick3按钮
        /// </summary>
        [Name("Flystick3按钮")]
        [EnumPopup]
        [HideInSuperInspector(nameof(_flystick), EValidityCheckType.NotEqual, EFlystick.Flystick3)]
        public EFlystick3Switchs _flystick3Switchs = EFlystick3Switchs.BottomSwitch;

        /// <summary>
        /// 死区：对于摇杆的值超过本死区值时，认为事件成立；
        /// </summary>
        [Name("死区")]
        [Tip("对于摇杆的值超过本死区值时，认为事件成立；")]
        [Range(-1, 1)]
        public float _deadZone = 0.5f;

        /// <summary>
        /// Flystick按钮是否被按压中
        /// </summary>
        /// <param name="streamClient"></param>
        /// <returns></returns>
        public bool IsPressed(ARTStreamClient streamClient) => IsPressed(streamClient, out _, out _);

        /// <summary>
        /// Flystick按钮是否被按压中
        /// </summary>
        /// <param name="streamClient"></param>
        /// <param name="pressedFlystickID">被按压的Flystick编号</param>
        /// <param name="pressedFlystickSwitchs">被按压的Flystick按钮</param>
        /// <returns></returns>
        public bool IsPressed(ARTStreamClient streamClient, out int pressedFlystickID, out int pressedFlystickSwitchs)
        {
            pressedFlystickID = -1;
            pressedFlystickSwitchs = -1;
            if (!streamClient) return false;
            switch (_flystick)
            {
                case EFlystick.Flystick1:
                    {
                        return _flystick1Switchs.IsPressed(streamClient, _flysitckID, _deadZone, out pressedFlystickID, out pressedFlystickSwitchs);
                    }
                case EFlystick.Flystick2:
                    {
                        return _flystick2Switchs.IsPressed(streamClient, _flysitckID, _deadZone, out pressedFlystickID, out pressedFlystickSwitchs);
                    }
                case EFlystick.Flystick3:
                    {
                        return _flystick3Switchs.IsPressed(streamClient, _flysitckID, _deadZone, out pressedFlystickID, out pressedFlystickSwitchs);
                    }
            }
            return false;
        }

        /// <summary>
        /// 转友好字符串
        /// </summary>
        /// <returns></returns>
        public string ToFriendlyString()
        {
            switch (_flystick)
            {
                case EFlystick.Flystick1: return CommonFun.Name(_flystick1Switchs);
                case EFlystick.Flystick2: return CommonFun.Name(_flystick2Switchs);
                case EFlystick.Flystick3: return CommonFun.Name(_flystick3Switchs);
            }
            return CommonFun.Name(_flystick);
        }
    }
}
