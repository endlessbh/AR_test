using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Helper;

namespace XCSJ.Extension.Base.XUnityEngine
{
    public interface IScriptableObject_LinkType : IUnityEngine_Object
    {
        ScriptableObject scriptableObject { get; }
    }

    public class ScriptableObject_LinkType<T> : UnityEngine_Object<T>, IScriptableObject_LinkType
    where T : ScriptableObject_LinkType<T>
    {
        public ScriptableObject scriptableObject => obj as ScriptableObject;

        public ScriptableObject_LinkType(ScriptableObject obj) : base(obj) { }
        public ScriptableObject_LinkType(object obj) : base(obj) { }
        public ScriptableObject_LinkType() { }

        public static T CreateInstance()
        {
            var obj = TypeHelper.CreateInstance<T>();
            if (((object)obj) == null) return default(T);
            obj.obj = ScriptableObject.CreateInstance(Type);
            return obj;
        }
    }

    [LinkType(typeof(ScriptableObject))]
    public class ScriptableObject_LinkType : ScriptableObject_LinkType<ScriptableObject_LinkType>
    {
        public ScriptableObject_LinkType(ScriptableObject obj) : base(obj) { }
        public ScriptableObject_LinkType(object obj) : base(obj) { }
        public ScriptableObject_LinkType() { }
    }
}
