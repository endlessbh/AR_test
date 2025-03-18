using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Extension.Base.XUnityEngine;
using XCSJ.Helper;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    public class ScriptableSingleton_T_LinkType<T> : ScriptableObject_LinkType<T> where T : ScriptableSingleton_T_LinkType<T>, new()
    {
        public ScriptableSingleton_T_LinkType(ScriptableObject obj) : base(obj) { }
        public ScriptableSingleton_T_LinkType(object obj) : base(obj) { }
        public ScriptableSingleton_T_LinkType() : base() { }

        #region instance

        public static XPropertyInfo instance_XPropertyInfo { get; } = GetXPropertyInfo(nameof(instance));

        public static T instance
        {
            get
            {
                var _this = new T();
                _this.obj = instance_XPropertyInfo?.GetValue(null);
                return _this;
            }
        }

        #endregion
    }


}
