using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Attributes;

namespace XCSJ.Extension.Base.Inputs
{
    /// <summary>
    /// 输入名称枚举：Unity中定义的输入名称；
    /// </summary>
    [Name("输入名称")]
    public enum EInputName
    {
        /// <summary>
        /// 无:不使用任何输入
        /// </summary>
        [Name("无")]
        [Tip("不使用任何输入")]
        [Abbreviation("")]
        None,

        /// <summary>
        /// 横向:通常对应键盘AD、键盘光标的左右
        /// </summary>
        [Name("横向")]
        [Tip("通常对应键盘AD、键盘光标的左右")]
        [Abbreviation(nameof(Horizontal))]
        Horizontal,

        /// <summary>
        /// 纵向:通常对应键盘WS、键盘光标的上下
        /// </summary>
        [Name("纵向")]
        [Tip("通常对应键盘WS、键盘光标的上下")]
        [Abbreviation(nameof(Vertical))]
        Vertical,

        /// <summary>
        /// 开火1:通常对应鼠标左键
        /// </summary>
        [Name("开火1")]
        [Tip("通常对应鼠标左键")]
        [Abbreviation(nameof(Fire1))]
        Fire1,

        /// <summary>
        /// 开火2:通常对应鼠标右键
        /// </summary>
        [Name("开火2")]
        [Tip("通常对应鼠标右键")]
        [Abbreviation(nameof(Fire2))]
        Fire2,

        /// <summary>
        /// 开火3:通常对应鼠标中键
        /// </summary>
        [Name("开火3")]
        [Tip("通常对应鼠标中键")]
        [Abbreviation(nameof(Fire3))]
        Fire3,

        /// <summary>
        /// 跳跃:通常对应键盘空格
        /// </summary>
        [Name("跳跃")]
        [Tip("通常对应键盘空格")]
        [Abbreviation(nameof(Jump))]
        Jump,

        /// <summary>
        /// 鼠标X:通常对应鼠标横向X的移动量
        /// </summary>
        [Name("鼠标X")]
        [Tip("通常对应鼠标横向X的移动量")]
        [Abbreviation("Mouse X")]
        Mouse_X,

        /// <summary>
        /// 鼠标Y:通常对应鼠标纵向Y的移动量
        /// </summary>
        [Name("鼠标Y")]
        [Tip("通常对应鼠标纵向Y的移动量")]
        [Abbreviation("Mouse Y")]
        Mouse_Y,

        /// <summary>
        /// 鼠标Y:通常对应鼠标滚轮的滚动量
        /// </summary>
        [Name("鼠标Y")]
        [Tip("通常对应鼠标滚轮的滚动量")]
        [Abbreviation("Mouse ScrollWheel")]
        Mouse_ScrollWheel,

        /// <summary>
        /// 提交:通常对应键盘回车键
        /// </summary>
        [Name("提交")]
        [Tip("通常对应键盘回车键")]
        [Abbreviation(nameof(Submit))]
        Submit,

        /// <summary>
        /// 取消:通常对应键盘ESC键
        /// </summary>
        [Name("取消")]
        [Tip("通常对应键盘ESC键")]
        [Abbreviation(nameof(Cancel))]
        Cancel,
    }

    /// <summary>
    /// 输入名称扩展
    /// </summary>
    public static class InputNameExtension
    {
        /// <summary>
        /// 获取输入名
        /// </summary>
        /// <param name="inputName"></param>
        /// <returns></returns>
        public static string GetInputName(this EInputName inputName) => AbbreviationAttribute.GetAbbreviation(inputName);
    }
}
