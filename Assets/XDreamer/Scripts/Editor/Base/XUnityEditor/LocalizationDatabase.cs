using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Helper;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    [LinkType("UnityEditor.LocalizationDatabase")]
    public class LocalizationDatabase : LinkType<LocalizationDatabase>
    {
        #region currentEditorLanguage

        public static XPropertyInfo currentEditorLanguage_PropertyInfo { get; } = GetXPropertyInfo(nameof(currentEditorLanguage));

        public static SystemLanguage currentEditorLanguage
        {
            get => currentEditorLanguage_PropertyInfo.GetStaticValue(null, SystemLanguage.English);
            set => currentEditorLanguage_PropertyInfo.SetStaticValue(value);
        }

        #endregion

        #region GetDefaultEditorLanguage

        public static XMethodInfo GetDefaultEditorLanguage_MethodInfo { get; } = GetXMethodInfo(nameof(GetDefaultEditorLanguage));

        public static SystemLanguage GetDefaultEditorLanguage() => GetDefaultEditorLanguage_MethodInfo.InvokeStaticEmpty<SystemLanguage>();

        #endregion

        #region GetAvailableEditorLanguages

        public static XMethodInfo GetAvailableEditorLanguages_MethodInfo { get; } = GetXMethodInfo(nameof(GetAvailableEditorLanguages));

        public static SystemLanguage[] GetAvailableEditorLanguages() => GetAvailableEditorLanguages_MethodInfo.InvokeStaticEmpty<SystemLanguage[]>();

        #endregion

        #region GetLocalizationResourceFolder

        public static XMethodInfo GetLocalizationResourceFolder_MethodInfo { get; } = GetXMethodInfo(nameof(GetLocalizationResourceFolder));

        public static string GetLocalizationResourceFolder() => GetLocalizationResourceFolder_MethodInfo.InvokeStaticEmpty<string>();

        #endregion

        #region GetContextGroupName

        public static XMethodInfo GetContextGroupName_MethodInfo { get; } = GetXMethodInfo(nameof(GetContextGroupName));

        public static string GetContextGroupName() => GetContextGroupName_MethodInfo.InvokeStaticEmpty<string>();

        #endregion
    }
}
