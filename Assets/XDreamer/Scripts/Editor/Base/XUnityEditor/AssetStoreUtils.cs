using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Helper;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    [LinkType("UnityEditor.AssetStoreUtils")]
    public class AssetStoreUtils : LinkType<AssetStoreUtils>
    {
        #region GetLoaderPath

        public static XMethodInfo GetLoaderPath_MethodInfo { get; } = new XMethodInfo(Type, nameof(GetLoaderPath), TypeHelper.DefaultLookup);

        public static string GetLoaderPath()
        {
            return (string)GetLoaderPath_MethodInfo.Invoke(null, Empty<object>.Array);
        }

        #endregion
    }
}
