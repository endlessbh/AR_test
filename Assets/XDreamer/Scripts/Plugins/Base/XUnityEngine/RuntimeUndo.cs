using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Helper;

namespace XCSJ.Extension.Base.XUnityEngine
{
    [LinkType(PlguinsHelper.UnityEngine_Prefix + nameof(RuntimeUndo))]
    public class RuntimeUndo : LinkType<RuntimeUndo>
    {
        #region SetTransformParent

        public static XMethodInfo SetTransformParent_MethodInfo { get; } = new XMethodInfo(Type, nameof(SetTransformParent), TypeHelper.StaticNotPublic);

        public static void SetTransformParent(Transform transform, Transform newParent, string name)
        {
            SetTransformParent_MethodInfo?.Invoke(null, new object[] { transform, newParent, name });
        }

        #endregion

        #region RecordObject

        public static XMethodInfo RecordObject_MethodInfo { get; } = new XMethodInfo(Type, nameof(RecordObject), TypeHelper.StaticNotPublic);

        public static void RecordObject(UnityEngine.Object objectToUndo, string name)
        {
            RecordObject_MethodInfo?.Invoke(null, new object[] { objectToUndo, name });
        }

        #endregion

        #region RecordObjects

        public static XMethodInfo RecordObjects_MethodInfo { get; } = new XMethodInfo(Type, nameof(RecordObjects), TypeHelper.StaticNotPublic);

        public static void RecordObjects(UnityEngine.Object[] objectsToUndo, string name)
        {
            RecordObjects_MethodInfo?.Invoke(null, new object[] { objectsToUndo, name });
        }

        #endregion
    }
}
