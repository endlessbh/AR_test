using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Helper;

namespace XCSJ.Extension.Base.XUnityEngine
{
    [LinkType(typeof(GUIUtility))]
    public class GUIUtility_LinkType : GUIUtility_LinkType<GUIUtility_LinkType> { }

    public class GUIUtility_LinkType<T> : LinkType<T> where T : GUIUtility_LinkType<T>
    {
        #region guiIsExiting

        public static XPropertyInfo guiIsExiting_PropertyInfo { get; } = new XPropertyInfo(Type, nameof(guiIsExiting), TypeHelper.StaticNotPublicHierarchy);

        public static bool guiIsExiting => (bool)guiIsExiting_PropertyInfo.GetValue(null);

        #endregion

        #region GetDefaultSkin

        public static XMethodInfo GetDefaultSkin_MethodInfo { get; } = new XMethodInfo(Type, nameof(GetDefaultSkin), TypeHelper.StaticNotPublicHierarchy);

        public static GUISkin GetDefaultSkin()
        {
            return GetDefaultSkin_MethodInfo.Invoke(null, null) as GUISkin;
        }

        #endregion
    }
}
