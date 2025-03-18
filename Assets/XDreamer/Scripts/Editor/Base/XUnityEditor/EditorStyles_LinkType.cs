using UnityEditor;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Helper;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    [LinkType(typeof(EditorStyles))]
    public class EditorStyles_LinkType : LinkType<EditorStyles_LinkType>
    {
        #region toggleMixed

        public static XPropertyInfo toggleMixed_XPropertyInfo { get; } = GetXPropertyInfo(nameof(toggleMixed));

        public static GUIStyle toggleMixed
        {
            get
            {
                var s = toggleMixed_XPropertyInfo?.GetValue(null);
                return s as GUIStyle;
            }
        }

        #endregion
    }
}
