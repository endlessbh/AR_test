using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Interfaces;

namespace XCSJ.Extension.Base.XUnityEngine
{
    public interface IUnityEngine_Object : ILinkType_Name
    {
        Object unityEngineObject { get; }
        HideFlags hideFlags { get; set; }
    }

    public class UnityEngine_Object<T> : LinkType_Name<T>, IUnityEngine_Object
        where T : UnityEngine_Object<T>
    {
        public Object unityEngineObject => obj as Object;

        public override string name
        {
            get => unityEngineObject ? unityEngineObject.name : "";
            set
            {
                if (unityEngineObject)
                {
                    unityEngineObject.name = value;
                }
            }
        }

        public HideFlags hideFlags
        {
            get => unityEngineObject ? unityEngineObject.hideFlags : HideFlags.None;
            set
            {
                if(unityEngineObject)
                {
                    unityEngineObject.hideFlags = value;
                }
            }
        }

        public UnityEngine_Object(Object obj) : base(obj) { }
        public UnityEngine_Object(object obj) : base(obj) { }
        protected UnityEngine_Object() { }

        public override int GetHashCode()
        {
            return unityEngineObject ? unityEngineObject.GetHashCode() : base.GetHashCode();
        }

        public override bool Equals(object other)
        {
            var _this = other as UnityEngine_Object<T>;
            if (_this == null && other != null && !(other is UnityEngine_Object<T>))
            {
                return false;
            }
            return Compare(this, _this);
        }

        public static bool Compare(UnityEngine_Object<T> x, UnityEngine_Object<T> y)
        {
            bool lflag = (object)x == null;
            bool rflag = (object)y == null;
            if (lflag && rflag) return true;
            if (lflag || rflag) return false;
            return x.unityEngineObject == y.unityEngineObject;
        }

        public static implicit operator bool(UnityEngine_Object<T> exists)
        {
            return !Compare(exists, null);
        }

        public static bool operator ==(UnityEngine_Object<T> x, UnityEngine_Object<T> y)
        {
            return Compare(x, y);
        }

        public static bool operator !=(UnityEngine_Object<T> x, UnityEngine_Object<T> y)
        {
            return !Compare(x, y);
        }
    }

    [LinkType(typeof(Object))]
    public class UnityEngine_Object : UnityEngine_Object<UnityEngine_Object>
    {
        public UnityEngine_Object(Object obj) : base(obj) { }
        public UnityEngine_Object(object obj) : base(obj) { }
        protected UnityEngine_Object() { }
    }
}
