using System;
using System.Linq;
using UnityEditor;
using XCSJ.Algorithms;
using XCSJ.Helper;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    [LinkType(typeof(MonoImporter))]
    public class MonoImporter_LinkType : LinkType<MonoImporter_LinkType>
    {
        #region CopyMonoScriptIconToImporters

        public static XMethodInfo CopyMonoScriptIconToImporters_MethodInfo { get; } = new XMethodInfo(Type, nameof(CopyMonoScriptIconToImporters), TypeHelper.StaticNotPublic);

        public static void CopyMonoScriptIconToImporters(MonoScript script)
        {
            CopyMonoScriptIconToImporters_MethodInfo.Invoke(null, new object[] { script });
        }

        #endregion

        public static MonoScript GetMonoScript(Type type)
        {
            if (type == null) return null;
            return MonoImporter.GetAllRuntimeMonoScripts().FirstOrDefault(ms => ms.GetClass() == type);
        }
    }
}
