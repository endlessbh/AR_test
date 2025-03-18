using System;
using System.Linq;
using UnityEditor.PackageManager;
using XCSJ.Algorithms;
using XCSJ.Caches;

namespace XCSJ.EditorExtension.Base.XUnityEditor.PackageManager
{
    /// <summary>
    /// 包信息关联类
    /// </summary>
    [LinkType(typeof(PackageInfo))]
    public class PackageInfo_LinkType : LinkType<PackageInfo_LinkType>
    {
#if UNITY_2019_4_OR_NEWER

        #region GetAll

        /// <summary>
        /// 获取所有的包信息
        /// </summary>
        public static XMethodInfo GetAll_XMethodInfo { get; } = GetXMethodInfo(nameof(GetAll));

        /// <summary>
        /// 获取所有的包信息
        /// </summary>
        /// <returns></returns>
        public static PackageInfo[] GetAll()
        {
            return GetAll_XMethodInfo.Invoke(null, Empty<object>.Array) as PackageInfo[];
        }

        #endregion

        /// <summary>
        /// 通过包名称获取包信息
        /// </summary>
        /// <param name="packageNameOrDisplayName"></param>
        /// <returns></returns>
        public static PackageInfo GetPackageInfoByPackageName(string packageNameOrDisplayName)
        {
            return GetAll()?.FirstOrDefault(info => info.name == packageNameOrDisplayName || info.displayName == packageNameOrDisplayName);
        }

        /// <summary>
        /// 通过类型名称或获取包信息
        /// </summary>
        /// <param name="typeNameOrFullName"></param>
        /// <returns></returns>
        public static PackageInfo GetPackageInfoByType(string typeNameOrFullName)
        {
            return GetPackageInfoByType(TypeCache.Get(typeNameOrFullName));
        }

        /// <summary>
        /// 通过类型获取包信息
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static PackageInfo GetPackageInfoByType(Type type)
        {
            return null;
            //if (type == null) return default;
            //return PackageInfo.FindForAssembly(type.Assembly);
        }

#endif
    }
}
