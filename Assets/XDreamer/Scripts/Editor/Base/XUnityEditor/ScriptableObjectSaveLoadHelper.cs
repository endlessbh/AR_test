using System.Reflection;
using XCSJ.Algorithms;
using XCSJ.Extension.Base.XUnityEngine;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    /// <summary>
    /// 与UnityEditor.ScriptableObjectSaveLoadHelper<T>类关联
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TScriptableObject"></typeparam>
    public abstract class ScriptableObjectSaveLoadHelper<T, TScriptableObject> : LinkType<T>
        where T : ScriptableObjectSaveLoadHelper<T, TScriptableObject>
        where TScriptableObject : ScriptableObject_LinkType<TScriptableObject>, new()
    {
        public ScriptableObjectSaveLoadHelper() { }
        public ScriptableObjectSaveLoadHelper(object obj) : base(obj) { }

        #region fileExtensionWithoutDot

        public static XPropertyInfo fileExtensionWithoutDot_PropertyInfo { get; } = new XPropertyInfo(Type, nameof(fileExtensionWithoutDot), BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);

        public string fileExtensionWithoutDot
        {
            get => fileExtensionWithoutDot_PropertyInfo.GetValue(obj) as string;
            set => fileExtensionWithoutDot_PropertyInfo.SetValue(obj, value);
        }

        #endregion
    }
}
