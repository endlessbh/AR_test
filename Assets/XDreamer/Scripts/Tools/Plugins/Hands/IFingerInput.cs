using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginTools.Hands
{
    /// <summary>
    /// 手指弯曲度输入接口
    /// </summary>
    public interface IFingerInput 
    {
        /// <summary>
        /// 手指弯曲度
        /// </summary>
        float[] bends { get; }
    }

    /// <summary>
    /// 高级手指输入
    /// </summary>
    public interface IComplexFingerInput : IFingerInput
    {

    }
}
