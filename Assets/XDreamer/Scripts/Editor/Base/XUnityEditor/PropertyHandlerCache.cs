using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Helper;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    [LinkType("UnityEditor.PropertyHandlerCache")]
    public class PropertyHandlerCache : LinkType<PropertyHandlerCache>
    {
        #region GetPropertyHash

        public static XMethodInfo GetPropertyHash_MethodInfo { get; } = GetXMethodInfo(nameof(GetPropertyHash));

        public static int GetPropertyHash(SerializedProperty property)
        {
            return (int)GetPropertyHash_MethodInfo.Invoke(null, new object[] { property });
        }

        #endregion
    }
}
