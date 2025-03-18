using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine.Rendering;
using XCSJ.Algorithms;
using XCSJ.Helper;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    [LinkType(typeof(HandleUtility))]
    public class HandleUtility_LinkType : LinkType<HandleUtility_LinkType>
    {
        #region ApplyWireMaterial

        public static XMethodInfo ApplyWireMaterial_MethodInfo { get; } = new XMethodInfo(Type, nameof(ApplyWireMaterial), new Type[] { }, TypeHelper.StaticNotPublic);

        public static void ApplyWireMaterial()
        {
            ApplyWireMaterial_MethodInfo.Invoke(null, null);
        }

        #endregion
    }
}
