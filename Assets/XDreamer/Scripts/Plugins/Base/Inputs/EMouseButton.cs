using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Attributes;

namespace XCSJ.Extension.Base.Inputs
{
    /// <summary>
    /// 鼠标按键
    /// </summary>
    [Name("鼠标按键")]
    public enum EMouseButton
    {
        /// <summary>
        /// 总是成立：不检测鼠标按键事件，即总是认为成立；
        /// </summary>
        [Name("总是成立")]
        [Tip("不检测鼠标按键事件，即总是认为成立；")]
        Always = -3,

        /// <summary>
        /// 无：无任何鼠标按键事件事件发生时才成立；
        /// </summary>
        [Name("无")]
        [Tip("无任何鼠标按键事件事件发生时才成立；")]
        None = -2,

        /// <summary>
        /// 任意：有任意鼠标按键事件发生时才成立;
        /// </summary>
        [Name("任意")]
        [Tip("有任意鼠标按键事件发生时才成立;")]
        Any = -1,

        /// <summary>
        /// 左键：左键鼠标按键事件发生时才成立；即第1个（基础）鼠标按钮；
        /// </summary>
        [Name("左键")]
        [Tip("左键鼠标按键事件发生时才成立；即第1个（基础）鼠标按钮；")]
        Left = 0,

        /// <summary>
        /// 右键：右键鼠标按键事件发生时才成立；即第2个鼠标按钮；
        /// </summary>
        [Name("右键")]
        [Tip("右键鼠标按键事件发生时才成立；即第2个鼠标按钮；")]
        Right,

        /// <summary>
        /// 中键：中键鼠标按键事件发生时才成立；即第3个鼠标按钮；
        /// </summary>
        [Name("中键")]
        [Tip("中键鼠标按键事件发生时才成立；即第3个鼠标按钮；")]
        Middle,

        /// <summary>
        /// 鼠标按键3：鼠标按键3事件发生时才成立；即附加的第4个鼠标按钮；
        /// </summary>
        [Name("鼠标按键3")]
        [Tip("鼠标按键3事件发生时才成立；即附加的第4个鼠标按钮；")]
        MouseButton3,

        /// <summary>
        /// 鼠标按键4：鼠标按键4事件发生时才成立；即附加的第5个鼠标按钮；
        /// </summary>
        [Name("鼠标按键4")]
        [Tip("鼠标按键4事件发生时才成立；即附加的第5个鼠标按钮；")]
        MouseButton4,

        /// <summary>
        /// 鼠标按键5：鼠标按键5事件发生时才成立；即附加的第6个鼠标按钮；
        /// </summary>
        [Name("鼠标按键5")]
        [Tip("鼠标按键5事件发生时才成立；即附加的第6个鼠标按钮；")]
        MouseButton5,

        /// <summary>
        /// 鼠标按键6：鼠标按键6事件发生时才成立；即附加的第7个鼠标按钮；
        /// </summary>
        [Name("鼠标按键6")]
        [Tip("鼠标按键6事件发生时才成立；即附加的第7个鼠标按钮；")]
        MouseButton6,
    }

    /// <summary>
    /// 鼠标按键扩展
    /// </summary>
    public static class MouseButtonExtension
    {
        /// <summary>
        /// 获取鼠标按钮是否在按下保持中
        /// </summary>
        /// <param name="mouseButton"></param>
        /// <returns></returns>
        public static bool GetMouseButton(this EMouseButton mouseButton, IInput input)
        {
            return GetMouseButtonInput(mouseButton, mb => input.GetMouseButton(mb));
        }

        /// <summary>
        /// 获取任意鼠标按钮是否在按下保持中
        /// </summary>
        /// <param name="mouseButtons"></param>
        /// <returns></returns>
        public static bool GetAnyMouseButton(this IEnumerable<EMouseButton> mouseButtons, IInput input)
        {
            return mouseButtons.Any(mb => mb.GetMouseButton(input));
        }

        /// <summary>
        /// 获取所有鼠标按钮是否在按下保持中
        /// </summary>
        /// <param name="mouseButtons"></param>
        /// <returns></returns>
        public static bool GetAllMouseButton(this IEnumerable<EMouseButton> mouseButtons, IInput input)
        {
            return mouseButtons.All(mb => mb.GetMouseButton(input));
        }

        /// <summary>
        /// 获取任意鼠标按钮是否在按下
        /// </summary>
        /// <param name="mouseButtons"></param>
        /// <returns></returns>
        public static bool GetAnyMouseButtonDown(this IEnumerable<EMouseButton> mouseButtons, IInput input)
        {
            return mouseButtons.Any(mb => mb.GetMouseButtonDown(input));
        }

        /// <summary>
        /// 获取鼠标按钮是否在按下
        /// </summary>
        /// <param name="mouseButton"></param>
        /// <returns></returns>
        public static bool GetMouseButtonDown(this EMouseButton mouseButton, IInput input)
        {
            return GetMouseButtonInput(mouseButton, mb => input.GetMouseButtonDown(mb));
        }

        /// <summary>
        /// 获取任意鼠标按钮是否在弹起
        /// </summary>
        /// <param name="mouseButtons"></param>
        /// <returns></returns>
        public static bool GetAnyMouseButtonUp(this IEnumerable<EMouseButton> mouseButtons, IInput input)
        {
            return mouseButtons.Any(mb => mb.GetMouseButtonUp(input));
        }

        /// <summary>
        /// 获取鼠标按钮是否在弹起
        /// </summary>
        /// <param name="mouseButton"></param>
        /// <returns></returns>
        public static bool GetMouseButtonUp(this EMouseButton mouseButton, IInput input)
        {
            return GetMouseButtonInput(mouseButton, mb => input.GetMouseButtonUp(mb));
        }

        /// <summary>
        /// 鼠标按钮数目
        /// </summary>
        private const int MouseButtonCount = 7;

        public static bool GetMouseButtonInput(this EMouseButton mouseButton, Func<int, bool> func)
        {
            switch (mouseButton)
            {
                case EMouseButton.Any:
                    {
                        for (int i = 0; i < MouseButtonCount; i++)
                        {
                            if (func(i)) return true;
                        }
                    }
                    break;
                case EMouseButton.None: return !GetMouseButtonInput(EMouseButton.Any, func);
                case EMouseButton.Always: return true;
                default: return func((int)mouseButton);
            }
            return false;
        }

        /// <summary>
        /// 获取键码列表
        /// </summary>
        /// <param name="mouseButton"></param>
        /// <returns></returns>
        public static IEnumerable<KeyCode> GetKeyCodes(this EMouseButton mouseButton)
        {
            var list = new List<KeyCode>();
            switch (mouseButton)
            {
                case EMouseButton.Any:
                    {
                        for (int i = 0; i < MouseButtonCount; i++)
                        {
                            list.Add(MouseButtonToKeyCode(i));
                        }
                    }
                    break;
                case EMouseButton.None:
                case EMouseButton.Always:
                    {
                        break;
                    }
                default:
                    {
                        list.Add(MouseButtonToKeyCode((int)mouseButton));
                        break;
                    }
            }
            return list;
        }

        /// <summary>
        /// 将鼠标按钮索引[0,6]转为对应的键码<see cref="KeyCode"/>
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        public static KeyCode MouseButtonToKeyCode(this int button)
        {
            switch (button)
            {
                case 0: return KeyCode.Mouse0;
                case 1: return KeyCode.Mouse1;
                case 2: return KeyCode.Mouse2;
                case 3: return KeyCode.Mouse3;
                case 4: return KeyCode.Mouse4;
                case 5: return KeyCode.Mouse5;
                case 6: return KeyCode.Mouse6;
                default: return KeyCode.None;
            }
        }
    }
}
