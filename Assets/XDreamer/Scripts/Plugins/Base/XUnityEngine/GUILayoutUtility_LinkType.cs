using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Helper;

namespace XCSJ.Extension.Base.XUnityEngine
{
    /// <summary>
    /// 类<see cref="GUILayoutUtility"/>的关联类
    /// </summary>
    [LinkType(typeof(GUILayoutUtility))]
    public class GUILayoutUtility_LinkType : GUILayoutUtility_LinkType<GUILayoutUtility_LinkType>
    {
    }

    /// <summary>
    /// 类<see cref="GUILayoutUtility"/>的泛型关联类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GUILayoutUtility_LinkType<T> : LinkType<T> where T : GUILayoutUtility_LinkType<T>
    {
        #region topLevel

        public static XPropertyInfo topLevel_FieldInfo { get; } = GetXPropertyInfo(nameof(topLevel));

        public static GUILayoutGroup topLevel => new GUILayoutGroup(topLevel_FieldInfo.GetValue(null));

        #endregion
    }
}
