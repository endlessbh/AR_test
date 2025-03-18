using UnityEngine;
using XCSJ.Algorithms;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    /// <summary>
    /// UnityEditor.PlayModeView关联类
    /// </summary>
    [LinkType("UnityEditor.PlayModeView")]
    public class PlayModeView_LinkType:EditorWindow_LinkType<PlayModeView_LinkType>
    {
        #region GetMainPlayModeViewTargetSize

        public static XMethodInfo GetMainPlayModeViewTargetSize_MethodInfo { get; } = GetXMethodInfo(nameof(GetMainPlayModeViewTargetSize));
        public static Vector2 GetMainPlayModeViewTargetSize()
        {
            return GetMainPlayModeViewTargetSize_MethodInfo.InvokeStaticEmpty<Vector2>();
        }
        #endregion 
    }
}
