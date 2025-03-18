using System;
using UnityEditor;
using XCSJ.Algorithms;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    [LinkType(typeof(MonoScript))]
    public class MonoScript_LinkType : LinkType<MonoScript_LinkType>
    {
        public static MonoScript PingObject(Type type)
        {
            var monoScript = MonoImporter_LinkType.GetMonoScript(type);
            if (monoScript == null) return null;
            EditorGUIUtility.PingObject(monoScript);
            return monoScript;
        }
    }
}
