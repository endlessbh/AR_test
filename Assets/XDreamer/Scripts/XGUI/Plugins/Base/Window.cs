using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using XCSJ.Attributes;
using UnityEngine;

namespace XCSJ.PluginXGUI.Base
{
    /// <summary>
    /// 窗口 ： 不能嵌套
    /// </summary>
    [Name("窗口")]
    [DisallowMultipleComponent]
    public class Window : SubWindow
    {
        
    }
}
