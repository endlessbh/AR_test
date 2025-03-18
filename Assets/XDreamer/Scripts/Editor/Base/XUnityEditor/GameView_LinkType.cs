using UnityEditor;
using UnityEngine;
using XCSJ.Algorithms;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    /// <summary>
    /// 类UnityEditor.GameView关联类型
    /// </summary>
    [LinkType("UnityEditor.GameView")]
    public class GameView_LinkType : EditorWindow_LinkType<GameView_LinkType>
    {
        public GameView_LinkType() { }
        public GameView_LinkType(EditorWindow obj) : base(obj) { }
        public GameView_LinkType(object obj) : base(obj) { }

        #region GetMainGameViewTargetSize

        public static XMethodInfo GetMainGameViewTargetSize_MethodInfo { get; } = GetXMethodInfo(nameof(GetMainGameViewTargetSize));

        public static Vector2 GetMainGameViewTargetSize() => (Vector2)GetMainGameViewTargetSize_MethodInfo.Invoke(null, null);

        #endregion
    }
}
