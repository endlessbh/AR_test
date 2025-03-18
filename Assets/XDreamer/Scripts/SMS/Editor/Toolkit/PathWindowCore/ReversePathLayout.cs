using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.States.Base;
using XCSJ.PluginSMS.States.Motions;

namespace XCSJ.EditorSMS.Toolkit.PathWindowCore
{
    /// <summary>
    /// 正弦曲线布局
    /// </summary>
    public class ReversePathLayout //: IPathLayout
    {
        public string name { get; set; } = "正弦曲线";

        public void Layout(PathInfo path)
        {
            //path.SetToLastPoint();
            //path.offsetValues.Reverse();
        }
    }
}
