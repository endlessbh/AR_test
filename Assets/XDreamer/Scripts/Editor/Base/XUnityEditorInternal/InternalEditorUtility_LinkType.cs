using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEditorInternal;
using XCSJ.Algorithms;
using XCSJ.Extension.Base.Kernel;

namespace XCSJ.EditorExtension.Base.XUnityEditorInternal
{
    [LinkType(typeof(InternalEditorUtility))]
    public class InternalEditorUtility_LinkType : LinkType<InternalEditorUtility_LinkType>
    {
        #region IsScriptOrAssembly

        public static XMethodInfo IsScriptOrAssembly_XPropertyInfo { get; } = GetXMethodInfo(nameof(IsScriptOrAssembly));

        public static bool IsScriptOrAssembly(string filename)
        {
            return (bool)IsScriptOrAssembly_XPropertyInfo?.Invoke(null, new object[] { filename });
        }

        #endregion

        #region GetAllScriptGUIDs

        public static XMethodInfo GetAllScriptGUIDs_XPropertyInfo { get; } = GetXMethodInfo(nameof(GetAllScriptGUIDs));

        public static IEnumerable<string> GetAllScriptGUIDs()
        {
            return GetAllScriptGUIDs_XPropertyInfo?.Invoke(null, Empty<object>.Array) as IEnumerable<string>;
        }

        #endregion
    }
}
