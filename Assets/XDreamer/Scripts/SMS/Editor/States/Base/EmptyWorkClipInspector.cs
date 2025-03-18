using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using XCSJ.Attributes;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.EditorSMS.States.Base
{
    /// <summary>
    /// 空工作剪辑检查器
    /// </summary>
    [Name("空工作剪辑检查器")]
    [CustomEditor(typeof(EmptyWorkClip))]
    public class EmptyWorkClipInspector : WorkClipInspector<EmptyWorkClip>
    {
    }
}
