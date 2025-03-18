using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using XCSJ.Algorithms;

namespace XCSJ.EditorExtension.Base.XUnityEditor.PackageManager.UI
{
#if UNITY_2020_1_OR_NEWER

    [LinkType("UnityEditor.PackageManager.UI.PackageManagerProjectSettings")]
    public class PackageManagerProjectSettings : ScriptableSingleton_T_LinkType<PackageManagerProjectSettings>
    {
        public PackageManagerProjectSettings(ScriptableObject obj) : base(obj) { }
        public PackageManagerProjectSettings(object obj) : base(obj) { }
        public PackageManagerProjectSettings() : base() { }

        #region enablePreviewPackages

        public static XPropertyInfo enablePreviewPackages_XPropertyInfo { get; } = GetXPropertyInfo(nameof(enablePreviewPackages));

        public bool enablePreviewPackages
        {
            get
            {
                return (bool)enablePreviewPackages_XPropertyInfo?.GetValue(obj);
            }
            set
            {
                enablePreviewPackages_XPropertyInfo?.SetValue(obj, value);
            }
        }

        #endregion

        #region enablePackageDependencies

        public static XPropertyInfo enablePackageDependencies_XPropertyInfo { get; } = GetXPropertyInfo(nameof(enablePackageDependencies));

        public bool enablePackageDependencies
        {
            get
            {
                return (bool)enablePackageDependencies_XPropertyInfo?.GetValue(obj);
            }
            set
            {
                enablePackageDependencies_XPropertyInfo?.SetValue(obj, value);
            }
        }

        #endregion
    }

#endif
}
