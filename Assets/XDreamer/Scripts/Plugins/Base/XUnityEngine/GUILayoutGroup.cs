using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Helper;

namespace XCSJ.Extension.Base.XUnityEngine
{
    /// <summary>
    /// 类UnityEngine.GUILayoutGroup的关联类
    /// </summary>
    [LinkType("UnityEngine.GUILayoutGroup")]
    public class GUILayoutGroup : GUILayoutGroup<GUILayoutGroup>
    {
        public GUILayoutGroup(object obj) : base(obj) { }

        protected GUILayoutGroup() { }
    }

    /// <summary>
    /// 类UnityEngine.GUILayoutGroup的泛型关联类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GUILayoutGroup<T> : GUILayoutEntry<T> where T : GUILayoutGroup<T>
    {
        public GUILayoutGroup(object obj) : base(obj) { }

        protected GUILayoutGroup() { }

        #region isVertical

        public static XFieldInfo isVertical_FieldInfo { get; } = GetXFieldInfo(nameof(isVertical));

        public bool isVertical => (bool)isVertical_FieldInfo.GetValue(obj);

        #endregion
    }
}
