using System;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Helper;

namespace XCSJ.Extension.Base.XUnityEngine
{
    [LinkType("UnityEngine.GUIClip")]
    public class GUIClip : LinkType<GUIClip>
    {
        #region Unclip

        public static XMethodInfo Unclip_Rect_MethodInfo { get; } = new XMethodInfo(Type, nameof(Unclip), new Type[] { typeof(Rect) }, TypeHelper.DefaultLookup);

        public static Rect Unclip(Rect rect)
        {
            return (Rect)Unclip_Rect_MethodInfo.Invoke(null, new object[] { rect });
        }

        public static XMethodInfo Unclip_Vector2_MethodInfo { get; } = new XMethodInfo(Type, nameof(Unclip), new Type[] { typeof(Vector2) }, TypeHelper.DefaultLookup);

        public static Rect Unclip(Vector2 pos)
        {
            return (Rect)Unclip_Vector2_MethodInfo.Invoke(null, new object[] { pos });
        }

        #endregion
    }
}
