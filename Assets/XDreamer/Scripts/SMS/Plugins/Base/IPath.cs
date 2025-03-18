using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Interfaces;

namespace XCSJ.PluginSMS.Base
{
    /// <summary>
    /// 路径接口
    /// </summary>
    public interface IPath : IName
    {
        /// <summary>
        /// 路径的关键控制点;均为世界坐标；
        /// </summary>
        List<Vector3> keyPoints { get; set; }

        /// <summary>
        /// 路径中被控制对象的变换
        /// </summary>
        List<Transform> transforms { get; }

        /// <summary>
        /// 向路径中被控制对象的变换列表中增加变换
        /// </summary>
        /// <param name="transform"></param>
        void AddTransform(Transform transform);

        /// <summary>
        /// 视图规则
        /// </summary>
        EViewRule viewRule { get; set; }
    }

    [Name("视图规则")]
    public enum EViewRule
    {
        [Name("无")]
        None = 0,

        [Name("跟随移动路径")]
        [Tip("游戏对象的观察点跟随移动路径变化，类似3DsMax中Follow属性", "The observation point of the game object changes with the moving path, similar to the follow attribute in 3dsmax")]
        MovePath,

        [Name("视图路径")]
        [Tip("游戏对象的观察点跟随视图路径变化，类似3DsMax中Look at用法", "The observation point of the game object changes with the view path, similar to the look at usage in 3dsmax")]
        ViewPath,
    }
}
