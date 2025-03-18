using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using XCSJ.Algorithms;

namespace XCSJ.EditorExtension.Base.XUnityEditor.PackageManager.UI
{
    [LinkType("UnityEditor.PackageManager.UI.PackageManagerWindow")]
    public class PackageManagerWindow : EditorWindow_LinkType<PackageManagerWindow>
    {
        #region OpenPackageManager

        /// <summary>
        /// 打开包管理器，在Unity2019.4之后才有效
        /// </summary>
        public static XMethodInfo OpenPackageManager_XMethodInfo { get; } = GetXMethodInfo(nameof(OpenPackageManager));

        public static void OpenPackageManager(string packageNameOrDisplayName)
        {
#if UNITY_2019_4_OR_NEWER
            OpenPackageManager_XMethodInfo?.Invoke(null, new object[] { packageNameOrDisplayName });
#else
            OpneWindow(packageNameOrDisplayName);
#endif
        }

        #endregion

        /// <summary>
        /// 打开包管理器窗口
        /// </summary>
        /// <param name="packageNameOrDisplayName"></param>
        public static void OpneWindow(string packageNameOrDisplayName)
        {
#if UNITY_2019_4_OR_NEWER
            UnityEditor.PackageManager.UI.Window.Open(packageNameOrDisplayName);
#else
            EditorApplication.ExecuteMenuItem("Window/Package Manager");
#endif
        }
    }
}
