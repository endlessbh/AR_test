using UnityEditor;
using UnityEngine;
using XCSJ.Algorithms;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    /// <summary>
    /// 类<see cref="Handles"/>关联类型
    /// </summary>
    [LinkType(typeof(Handles))]
    public class Handles_LinkType : LinkType<Handles_LinkType>
    {
        #region EmitGUIGeometryForCamera

        public static XMethodInfo EmitGUIGeometryForCamera_MethodInfo { get; } = GetXMethodInfo(nameof(EmitGUIGeometryForCamera));

        public static void EmitGUIGeometryForCamera(Camera source, Camera dest)
        {
            EmitGUIGeometryForCamera_MethodInfo.Invoke(null, new object[] { source, dest });
        }

        #endregion
    }
}
