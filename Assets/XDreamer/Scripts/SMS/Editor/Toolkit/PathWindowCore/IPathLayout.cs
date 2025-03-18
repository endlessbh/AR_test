using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Interfaces;
using XCSJ.PluginSMS.States.Motions;

namespace XCSJ.EditorSMS.Toolkit.PathWindowCore
{
    /// <summary>
    /// 路径布局接口
    /// </summary>
    public interface IPathLayout : IName 
    {
        void OnGUI(PathInfo path);

        void Layout(PathInfo path);
    }
}

