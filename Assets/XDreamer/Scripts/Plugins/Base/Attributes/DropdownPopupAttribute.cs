using System;
using UnityEngine;

namespace XCSJ.Extension.Base.Attributes
{
    /// <summary>
    /// 下拉式弹出菜单特性,需要与<see cref="IDropdownPopupAttribute"/>接口组合使用；
    /// </summary>
    public class DropdownPopupAttribute : PropertyAttribute
    {
        /// <summary>
        /// 目标用途
        /// </summary>
        public string purpose { get; private set; }

        /// <summary>
        /// 弹出式菜单宽度
        /// </summary>
        public float width = 80;

        /// <summary>
        /// 有无字段
        /// </summary>
        public bool hasField = true;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="purpose"></param>
        /// <param name="hasText"></param>
        public DropdownPopupAttribute(string purpose)
        {
            this.purpose = purpose ?? throw new ArgumentNullException(nameof(purpose));
        }
    }

    /// <summary>
    /// 下拉式弹出菜单特性接口,需要与<see cref="DropdownPopupAttribute"/>特性（或其子类特性）组合使用；
    /// </summary>
    public interface IDropdownPopupAttribute
    {
        /// <summary>
        /// 尝试获取选项文本列表；
        /// </summary>
        /// <param name="purpose">目标用途</param>
        /// <param name="propertyPath">属性路径</param>
        /// <param name="options">选项文本列表；如果期望下拉式弹出菜单出现层级，需要数组元素中有'/'</param>
        /// <returns></returns>
        bool TryGetOptions(string purpose, string propertyPath, out string[] options);

        /// <summary>
        /// 尝试获取文本选项：通过属性值转为文本选项
        /// </summary>
        /// <param name="purpose">目标用途</param>
        /// <param name="propertyPath">属性路径</param>
        /// <param name="propertyValue">属性值</param>
        /// <param name="option">选项文本</param>
        /// <returns></returns>
        bool TryGetOption(string purpose, string propertyPath, string[] options, object propertyValue, out string option);

        /// <summary>
        /// 尝试获取属性值：通过文本选项转为属性值
        /// </summary>
        /// <param name="purpose">目标用途</param>
        /// <param name="propertyPath">属性路径</param>
        /// <param name="option">选项文本</param>
        /// <param name="propertyValue">属性值</param>
        /// <returns></returns>
        bool TryGetPropertyValue(string purpose, string propertyPath, string[] options, string option, out object propertyValue);
    }
}
