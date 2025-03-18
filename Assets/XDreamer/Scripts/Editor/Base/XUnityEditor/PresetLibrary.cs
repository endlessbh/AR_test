using System;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Extension.Base.XUnityEngine;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    /// <summary>
    /// 与UnityEditor.PresetLibrary类对应
    /// </summary>
    public abstract class PresetLibrary<T> : ScriptableObject_LinkType<T> where T : PresetLibrary<T>, new()
    {
        public PresetLibrary() { }
        public PresetLibrary(object obj) : base(obj) { }

        #region Count

        public static XMethodInfo Count_MethodInfo { get; } = new XMethodInfo(Type, nameof(Count));

        public int Count() => (int)Count_MethodInfo.Invoke(obj, null);

        #endregion

        #region GetPreset

        public static XMethodInfo GetPreset_MethodInfo { get; } = new XMethodInfo(Type, nameof(GetPreset));

        public object GetPreset(int index) => Count_MethodInfo.Invoke(obj, new object[] { index });

        #endregion

        #region Add

        public static XMethodInfo Add_MethodInfo { get; } = new XMethodInfo(Type, nameof(Add));

        public void Add(object presetObject, string presetName) => Add_MethodInfo.Invoke(obj, new object[] { presetObject, presetName });

        #endregion

        #region Replace

        public static XMethodInfo Replace_MethodInfo { get; } = new XMethodInfo(Type, nameof(Replace));

        public void Replace(int index, object newPresetObject) => Replace_MethodInfo.Invoke(obj, new object[] { index, newPresetObject });

        #endregion

        #region Remove

        public static XMethodInfo Remove_MethodInfo { get; } = new XMethodInfo(Type,nameof(Remove));

        public void Remove(int index) => Remove_MethodInfo.Invoke(obj, new object[] { index });

        #endregion

        #region Move

        public static XMethodInfo Move_MethodInfo { get; } = new XMethodInfo(Type, nameof(Move));

        public void Move(int index, int destIndex, bool insertAfterDestIndex) => Move_MethodInfo.Invoke(obj, new object[] { index, destIndex, insertAfterDestIndex });

        #endregion

        #region Draw

        public static XMethodInfo Draw_Rect_Int_MethodInfo { get; } = new XMethodInfo(Type, nameof(Draw), new Type[] { typeof(Rect), typeof(int) });

        public void Draw(Rect rect, int index) => Draw_Rect_Int_MethodInfo.Invoke(obj, new object[] { rect, index });

        public static XMethodInfo Draw_Rect_Object_MethodInfo { get; } = new XMethodInfo(Type, nameof(Draw), new Type[] { typeof(Rect), typeof(object) });

        public void Draw(Rect rect, object presetObject) => Draw_Rect_Object_MethodInfo.Invoke(obj, new object[] { rect, presetObject });

        #endregion

        #region GetName

        public static XMethodInfo GetName_MethodInfo { get; } = new XMethodInfo(Type, nameof(GetName));

        public string GetName(int index) => (string)GetName_MethodInfo.Invoke(obj, new object[] { index });

        #endregion

        #region SetName

        public static XMethodInfo SetName_MethodInfo { get; } = new XMethodInfo(Type, nameof(SetName));

        public void SetName(int index, string name) => SetName_MethodInfo.Invoke(obj, new object[] { index, name });

        #endregion
    }
}
