using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Extension.Base.XUnityEngine;
using XCSJ.Helper;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    [LinkType(typeof(Undo))]
    public class Undo_LinkType : LinkType<Undo_LinkType>
    {
        #region GetRecords

        public static XMethodInfo GetRecords_MethodInfo { get; } = new XMethodInfo(Type, nameof(GetRecords), TypeHelper.StaticNotPublic);

        public static void GetRecords(List<string> undoRecords, List<string> redoRecords)
        {
            GetRecords_MethodInfo.Invoke(null, new object[] { undoRecords, redoRecords });
        }

        #endregion

        #region DestroyObjectUndoable

        public static XMethodInfo DestroyObjectUndoable_MethodInfo { get; } = new XMethodInfo(Type, nameof(DestroyObjectUndoable), TypeHelper.StaticNotPublic);

        public static void DestroyObjectUndoable(UnityEngine.Object objectToUndo, string name)
        {
            DestroyObjectUndoable_MethodInfo.Invoke(null, new object[] { objectToUndo, name });
        }

        #endregion
    }
}
