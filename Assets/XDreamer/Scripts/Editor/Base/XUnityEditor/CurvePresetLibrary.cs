using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using XCSJ.Algorithms;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    /// <summary>
    /// 与UnityEditor.CurvePresetLibrary类关联
    /// </summary>
    [LinkType(EditorHelper.UnityEditorPrefix + nameof(CurvePresetLibrary))]
    public class CurvePresetLibrary : PresetLibrary<CurvePresetLibrary>
    {
        public CurvePresetLibrary() { }
        public CurvePresetLibrary(object obj) : base(obj) { }

        public AnimationCurve GetCurve(int index) => GetPreset(index) as AnimationCurve;

        #region m_Presets

        public static XFieldInfo m_PresetsName_FieldInfo { get; } = new XFieldInfo(Type, nameof(m_Presets), BindingFlags.NonPublic | BindingFlags.Instance);

        public List<CurvePreset> m_Presets
        {
            get
            {
                List<CurvePreset> curvePresets = new List<CurvePreset>();
                if (m_PresetsName_FieldInfo.GetValue(obj) is IList curvePresetList)
                {
                    foreach (var curvePreset in curvePresetList)
                    {
                        curvePresets.Add(new CurvePreset(curvePreset));
                    }
                }
                return curvePresets;
            }
        }

        #endregion

        public CurvePreset GetCurvePreset(int index) => m_Presets[index];

        /// <summary>
        /// 与UnityEditor.CurvePresetLibrary+CurvePreset类对应
        /// </summary>
        [LinkType(EditorHelper.UnityEditorPrefix + nameof(CurvePresetLibrary) + "+" + nameof(CurvePreset))]
        public class CurvePreset : LinkType_Name<CurvePreset>
        {
            #region curve

            public static XPropertyInfo curve_PropertyInfo { get; } = new XPropertyInfo(Type, nameof(curve));

            public AnimationCurve curve
            {
                get => (AnimationCurve)curve_PropertyInfo.GetValue(obj);
                set => curve_PropertyInfo.SetValue(obj, value);
            }

            #endregion

            public static XConstructorInfo CurvePreset_AnimationCurve_String_ConstructorInfo { get; } = new XConstructorInfo(Type, new Type[] { typeof(AnimationCurve), typeof(string) });

            public CurvePreset(AnimationCurve preset, string presetName)
            {
                CurvePreset_AnimationCurve_String_ConstructorInfo.obj?.Invoke(new object[] { preset, presetName });
            }

            public CurvePreset(object obj) : base(obj) { }
        }
    }
}
