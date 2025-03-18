using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using XCSJ.Algorithms;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    [LinkType(typeof(Selection))]
    public class Selection_LinkType : LinkType<Selection_LinkType>
    {
        #region assetGUIDsDeepSelection

        public static XPropertyInfo assetGUIDsDeepSelection_XPropertyInfo { get; } = GetXPropertyInfo(nameof(assetGUIDsDeepSelection));

        public static string[] assetGUIDsDeepSelection
        {
            get => assetGUIDsDeepSelection_XPropertyInfo?.GetValue(null) as string[];
        }

        #endregion
    }
}
