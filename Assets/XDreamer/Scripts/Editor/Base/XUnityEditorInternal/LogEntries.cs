using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using XCSJ.Algorithms;
using XCSJ.Extension.Base.Kernel;

namespace XCSJ.EditorExtension.Base.XUnityEditorInternal
{
    [LinkType(EditorHelper.UnityEditorInternalPrefix + nameof(LogEntries))]
    public class LogEntries : LinkType<LogEntries>
    {
        #region Clear

        public static XMethodInfo Clear_MethodInfo { get; } = new XMethodInfo(Type, nameof(Clear), BindingFlags.Static | BindingFlags.Public);

        public static void Clear()=> Clear_MethodInfo.Invoke(null, null);

        #endregion
    }
}
