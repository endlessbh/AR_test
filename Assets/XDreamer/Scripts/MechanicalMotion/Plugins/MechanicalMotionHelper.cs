using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Collections;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginMechanicalMotion.Tools;

namespace XCSJ.PluginMechanicalMotion
{
    /// <summary>
    /// 机械运动助手
    /// </summary>
    public static class MechanicalMotionHelper
    {
        /// <summary>
        /// 字符串转为机构
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Mechanism StringToMechanism(string path) => GetMechanisms(path).FirstOrDefault();

        /// <summary>
        /// 机构转为字符串
        /// </summary>
        /// <param name="mechanism"></param>
        /// <returns></returns>
        public static string MechanismToString(Mechanism mechanism) => mechanism ? mechanism.path : string.Empty;

        /// <summary>
        /// 通过路径获取多个机构：机构路径可重复
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Mechanism[] GetMechanisms(string path)
        {
            if (!path.StartsWith("/")) // 未包含/前缀，则增加前缀
            {
                path = "/" + path;
            }
            return CommonFun.GetComponentsInChildren<Mechanism>(true).Where(m => m.path == path).ToArray();
        }

        /// <summary>
        /// 获取根机构对象集
        /// </summary>
        /// <returns></returns>
        public static Mechanism[] GetRootMechanisms()
        {
            return CommonFun.GetComponentsInChildren<Mechanism>(true).Where(m => !m.parent).ToArray();
        }
    }
}
