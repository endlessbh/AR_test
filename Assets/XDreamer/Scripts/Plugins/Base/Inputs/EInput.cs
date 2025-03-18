using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Attributes;

namespace XCSJ.Extension.Base.Inputs
{
    /// <summary>
    /// 输入枚举
    /// </summary>
    [Name("输入")]
    [Flags]
    public enum EInput
    {
        /// <summary>
        /// X输入：使用<see cref="Inputs.XInput"/>做处理；包含所有的输入类型；
        /// </summary>
        [Name("X输入")]
        [Tip("使用XInput做处理；包含所有的输入类型；")]
        [EnumFieldName("X输入")]
        XInput = -1,

        /// <summary>
        /// 标准输入：使用<see cref="Inputs.StandaloneInput"/>做处理；不可更新；直接映射<see cref="Input"/>类获取信息；
        /// </summary>
        [Name("标准输入")]
        [Tip("使用StandaloneInput做处理；")]
        [EnumFieldName("标准输入")]
        StandaloneInput = 1 << 0,

        /// <summary>
        /// 虚拟输入：使用<see cref="Inputs.VirtualInput"/>做处理；可更新；属于公用的虚拟输入；
        /// </summary>
        [Name("虚拟输入")]
        [Tip("使用VirtualInput做处理；模拟各种输入事件；可更新；")]
        [EnumFieldName("虚拟输入")]
        VirtualInput = 1 << 1,

        /// <summary>
        /// 相机输入：使用<see cref="Inputs.CameraInput"/>做处理；可更新；属于相机专用的虚拟输入；
        /// </summary>
        [Name("相机输入")]
        [Tip("使用CameraInput做处理；模拟相机控制所需的各种输入事件；可更新；属于相机专用的虚拟输入；")]
        [EnumFieldName("相机输入")]
        CameraInput = 1 << 2,

        /// <summary>
        /// 角色输入：使用<see cref="Inputs.CharacterInput"/>做处理；可更新；属于角色专用的虚拟输入；
        /// </summary>
        [Name("角色输入")]
        [Tip("使用CharacterInput做处理；模拟角色控制所需的各种输入事件；可更新；属于角色专用的虚拟输入；")]
        [EnumFieldName("角色输入")]
        CharacterInput = 1 << 3,

        /// <summary>
        /// 车辆输入：使用<see cref="Inputs.VehicleInput"/>做处理；可更新；属于车辆专用的虚拟输入；
        /// </summary>
        [Name("车辆输入")]
        [Tip("使用VehicleInput做处理；模拟车辆控制所需的各种输入事件；可更新；属于车辆专用的虚拟输入；")]
        [EnumFieldName("车辆输入")]
        VehicleInput = 1 << 4,
    }

    /// <summary>
    /// 输入扩展
    /// </summary>
    public static class InputExtension
    {
        /// <summary>
        /// 获取单例输入列表:添加新的虚拟输入类型后，必须同步更新本函数；
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static IEnumerable<IInput> GetInstanceInputs(this EInput input)
        {
            var list = new List<IInput>();

            if ((input & EInput.StandaloneInput) != 0) list.Add(StandaloneInput.instance);
            if ((input & EInput.VirtualInput) != 0) list.Add(VirtualInput.instance);
            if ((input & EInput.CameraInput) != 0) list.Add(CameraInput.instance);
            if ((input & EInput.CharacterInput) != 0) list.Add(CharacterInput.instance);
            if ((input & EInput.VehicleInput) != 0) list.Add(VehicleInput.instance);

            return list;
        }

        /// <summary>
        /// 获取输入
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static IInput GetInput(this EInput input) => InputCache.GetCacheValue(input, default);

        /// <summary>
        /// 更新轴
        /// </summary>
        /// <param name="inputs"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public static void UpdateAxis(this IEnumerable<EInput> inputs, string name, float value)
        {
            if (inputs == null) return;
            foreach (var i in inputs)
            {
                UpdateAxis(i, name, value);
            }
        }

        /// <summary>
        /// 更新轴
        /// </summary>
        /// <param name="input"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public static void UpdateAxis(this EInput input, string name, float value)
        {
            GetInput(input)?.UpdateAxis(name, value);
        }

        /// <summary>
        /// 更新按钮
        /// </summary>
        /// <param name="inputs"></param>
        /// <param name="name"></param>
        /// <param name="downOrUp"></param>
        public static void UpdateButton(this IEnumerable<EInput> inputs, string name, bool downOrUp)
        {
            if (inputs == null) return;
            foreach (var i in inputs)
            {
                UpdateButton(i, name, downOrUp);
            }
        }

        /// <summary>
        /// 更新按钮
        /// </summary>
        /// <param name="input"></param>
        /// <param name="name"></param>
        /// <param name="downOrUp"></param>
        public static void UpdateButton(this EInput input, string name, bool downOrUp)
        {
            GetInput(input)?.UpdateButton(name, downOrUp);
        }
    }
}
