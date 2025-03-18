using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Extension.Base.XUnityEngine;
using XCSJ.Helper;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    [LinkType(typeof(AssetDatabase))]
    public class AssetDatabase_LinkType : LinkType<AssetDatabase_LinkType>
    {
        #region assetFolderGUID

        public static XPropertyInfo assetFolderGUID_XPropertyInfo { get; } = GetXPropertyInfo(nameof(assetFolderGUID));

        public static string assetFolderGUID
        {
            get => assetFolderGUID_XPropertyInfo?.GetValue(null) as string;
        }

        #endregion

        #region CollectAllChildren

        public static XMethodInfo CollectAllChildren_XPropertyInfo { get; } = GetXMethodInfo(nameof(CollectAllChildren));

        public static string[] CollectAllChildren(string guid, string[] collection)
        {
            return CollectAllChildren_XPropertyInfo?.Invoke(null, new object[] { guid, collection }) as string[];
        }

        #endregion
    }
}
