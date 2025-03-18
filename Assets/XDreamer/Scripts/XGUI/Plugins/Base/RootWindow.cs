using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using XCSJ.Attributes;

namespace XCSJ.PluginXGUI.Base
{
    /// <summary>
    /// 根窗口：依赖画布，可管理子级窗口
    /// </summary>
    [Name("根窗口")]
    [Tip("依赖画布，可管理子级窗口", "Depending on the canvas, you can manage child windows")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Canvas))]
    public class RootWindow : Window
    {
        
    }
}
