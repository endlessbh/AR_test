using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCSJ.Algorithms;
using XCSJ.Collections;
using XCSJ.Helper;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    [LinkType("UnityEditor.PackageUtility")]
    public class PackageUtility : LinkType<PackageUtility>
    {
        #region BuildExportPackageItemsList

        public static XMethodInfo BuildExportPackageItemsList_MethodInfo { get; } = new XMethodInfo(Type, nameof(BuildExportPackageItemsList), TypeHelper.DefaultLookup);

        public static ExportPackageItem[] BuildExportPackageItemsList(string[] guids, bool dependencies)
        {
            var source = BuildExportPackageItemsList_MethodInfo.Invoke(null, new object[] { guids, dependencies }) as IEnumerable;
            return source?.Cast(s => new ExportPackageItem(s)).ToArray();
        }

        #endregion

        #region ExportPackage

        public static XMethodInfo ExportPackage_MethodInfo { get; } = new XMethodInfo(Type, nameof(ExportPackage), TypeHelper.DefaultLookup);

        public static void ExportPackage(string[] guids, string fileName)
        {
            ExportPackage_MethodInfo.Invoke(null, new object[] { guids, fileName });
        }

        #endregion
    }
}
