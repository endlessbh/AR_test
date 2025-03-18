using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using XCSJ.Attributes;
using XCSJ.PluginSMS.States.Motions;

namespace XCSJ.EditorSMS.States.Motions
{
    /// <summary>
    /// 透明度检查器
    /// </summary>
    [Name("透明度检查器")]
    [CustomEditor(typeof(Alpha))]
    public class AlphaInspector : MotionInspector<Alpha>
    {
    }
}
