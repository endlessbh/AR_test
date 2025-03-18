using UnityEditor;
using XCSJ.Algorithms;
using XCSJ.Helper;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    [LinkType(typeof(EditorUtility))]
    public class EditorUtility_LinkType : LinkType<EditorUtility_LinkType>
    {
        #region ForceReloadInspectors

        public static XMethodInfo ForceReloadInspectors_MethodInfo { get; } = new XMethodInfo(Type, nameof(ForceReloadInspectors), TypeHelper.StaticNotPublic);

        public static void ForceReloadInspectors()
        {
            ForceReloadInspectors_MethodInfo.Invoke(null, null);
        }

        #endregion
    }
};