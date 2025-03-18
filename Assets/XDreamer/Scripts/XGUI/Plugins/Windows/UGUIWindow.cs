using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginXGUI.Base;

namespace XCSJ.PluginXGUI.Windows
{
    /// <summary>
    /// UGUI窗口
    /// </summary>
    [Name("UGUI窗口")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    public class UGUIWindow : Window
    {

    }
}
