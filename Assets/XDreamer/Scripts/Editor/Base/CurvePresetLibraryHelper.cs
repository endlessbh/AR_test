using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using XCSJ.Collections;
using XCSJ.Caches;
using XCSJ.EditorExtension.Base.XUnityEditor;
using XCSJ.Extension.Base.Tweens;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;

namespace XCSJ.EditorExtension.Base
{
    /// <summary>
    /// 曲线预设库组手
    /// </summary>
    public static class CurvePresetLibraryHelper
    {
        public const CurveLibraryType DefaultCurveLibraryType = CurveLibraryType.NormalizedZeroToOne;

        public static CurvePresetLibrary curvePresetLibrary => curvePresetLibraryEditor.GetCurrentLib();

        public static PresetLibraryEditor_CurvePresetLibrary curvePresetLibraryEditor => CurveEditorWindow.instance.curvePresetsValid.m_CurveLibraryEditor;

        public static List<string> GetAvailableFilesWithExtensionOnTheHDD(PresetFileLocation fileLocation, CurveLibraryType curveLibraryType = DefaultCurveLibraryType)
        {
            return PresetLibraryLocations.GetAvailableFilesWithExtensionOnTheHDD(fileLocation, GetExtension(curveLibraryType));
        }

        public static string GetExtension(CurveLibraryType curveLibraryType = DefaultCurveLibraryType) => CurvePresetsContentsForPopupWindow.GetExtension(curveLibraryType);

        public static void CreateNewPreset(AnimationCurve animationCurve, string presetName) => CreateNewPreset(curvePresetLibraryEditor, animationCurve, presetName);

        public static void CreateNewPreset(PresetLibraryEditor_CurvePresetLibrary curvePresetLibraryEditor, AnimationCurve animationCurve, string presetName)
        {
            curvePresetLibraryEditor?.CreateNewPreset(animationCurve, presetName);
        }

        /// <summary>
        /// 创建的新的库
        /// </summary>
        /// <param name="curvePresetLibraryName"></param>
        /// <param name="presetFileLocation"></param>
        /// <returns>如果执行失败,返回错误信息；如果执行成功返回null</returns>
        public static string CreateNewLibrary(string curvePresetLibraryName, PresetFileLocation presetFileLocation = PresetFileLocation.PreferencesFolder, CurveLibraryType curveLibraryType = DefaultCurveLibraryType)
        {
            return CurveEditorWindow.instance.CreateNewLibrary(curvePresetLibraryName, presetFileLocation, curveLibraryType);
        }

        public static string GetCurvePresetLibraryFileFullPath(string curvePresetLibraryName, PresetFileLocation presetFileLocation = PresetFileLocation.PreferencesFolder, CurveLibraryType curveLibraryType = DefaultCurveLibraryType)
        {
            var ext = GetExtension(curveLibraryType);
            var list = GetAvailableFilesWithExtensionOnTheHDD(presetFileLocation);
            return list.FirstOrDefault(s => Path.GetFileName(s) == (curvePresetLibraryName + "." + ext));
        }

        public static bool Exist(string curvePresetLibraryName, PresetFileLocation presetFileLocation = PresetFileLocation.PreferencesFolder, CurveLibraryType curveLibraryType = DefaultCurveLibraryType)
        {
            var fullPath = GetCurvePresetLibraryFileFullPath(curvePresetLibraryName, presetFileLocation, curveLibraryType);
            return !string.IsNullOrEmpty(fullPath);
        }

        public static void Show(string curvePresetLibraryName, PresetFileLocation presetFileLocation = PresetFileLocation.PreferencesFolder, CurveLibraryType curveLibraryType = DefaultCurveLibraryType)
        {
            var fullPath = GetCurvePresetLibraryFileFullPath(curvePresetLibraryName, presetFileLocation, curveLibraryType);
            if (string.IsNullOrEmpty(fullPath)) return;
            curvePresetLibraryEditor.LibraryModeChange(fullPath);
        }

        public static void ShowAndInitIfNotExist(string curvePresetLibraryName, PresetFileLocation presetFileLocation = PresetFileLocation.PreferencesFolder, CurveLibraryType curveLibraryType = DefaultCurveLibraryType, Action<PresetLibraryEditor_CurvePresetLibrary> init = null, bool alwaysInit = false)
        {
            if (!Exist(curvePresetLibraryName, presetFileLocation, curveLibraryType) || alwaysInit)
            {
                CreateNewLibrary(curvePresetLibraryName, presetFileLocation, curveLibraryType);
                init?.Invoke(curvePresetLibraryEditor);
            }
            Show(curvePresetLibraryName);
        }

        /// <summary>
        /// 显示XDremaer曲线库
        /// </summary>
        public static void ShowXDreamer()
        {
            ShowAndInitIfNotExist(Product.Name, PresetFileLocation.PreferencesFolder, DefaultCurveLibraryType,
                editor =>
                {
                    var lib = editor.GetCurrentLib();
                    List<string> names = new List<string>();
                    for (int i = 0; i < lib.Count(); i++)
                    {
                        names.Add(lib.GetName(i));
                    }
                    foreach (var easeType in EnumCache<EEaseType>.Array)
                    {
                        var presetName = AnimationCurveHelper.PresetName(easeType);
                        if (names.Contains(presetName)) continue;
                        CreateNewPreset(editor, AnimationCurveHelper.Create(easeType), presetName);
                    }
                }, true);
        }
    }
}
