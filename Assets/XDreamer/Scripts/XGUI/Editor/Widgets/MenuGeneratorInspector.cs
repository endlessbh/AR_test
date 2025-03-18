using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Interactions;
using XCSJ.EditorExtension.Base.Attributes;
using XCSJ.EditorExtension.Base.Interactions.Tools;
using XCSJ.PluginXGUI.Widgets;

namespace XCSJ.EditorXGUI.Widgets
{
    /// <summary>
    /// 菜单生成器检查器
    /// </summary>
    [Name("菜单生成器检查器")]
    [CustomEditor(typeof(MenuGenerator), true)]
    public class MenuGeneratorInspector : InteractorInspector<MenuGenerator>
    {

    }
}
