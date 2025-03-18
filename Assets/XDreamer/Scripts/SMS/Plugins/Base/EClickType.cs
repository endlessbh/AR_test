using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.Kernel;
using XCSJ.Scripts;

namespace XCSJ.PluginSMS.Base
{
    /// <summary>
    /// 点击类型
    /// </summary>
    [Name("点击类型")]
    public enum EClickType
    {
        /// <summary>
        /// 按下并弹起
        /// </summary>
        [Name("按下并弹起")]
        DownAndUp = 0,

        /// <summary>
        /// 按下
        /// </summary>
        [Name("按下")]
        Down,

        /// <summary>
        /// 弹起
        /// </summary>
        [Name("弹起")]
        Up,
    }
}
