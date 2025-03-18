using UnityEngine;
using System.Collections;
using XCSJ.Extension.Demos;
using XCSJ.EditorCommonUtils;
using UnityEditor;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Menus;
using XCSJ.Attributes;

namespace XCSJ.EditorExtension.Demos
{
    public class XMenuAttribute : BaseMenuAttribute
    {
        public XMenuAttribute(string itemName) : base(itemName) { }
    }

    /// <summary>
    /// 菜单案例检查器
    /// </summary>
    [Name("菜单案例检查器")]
    [CustomEditor(typeof(MenuDemo))]
    public class MenuDemoInspector : MBInspector<MenuDemo>
    {
        [XMenu("A")]
        [Menu("M1", "A")]
        public static void FunA()
        {
            Debug.Log("FunA()");
        }

        [XMenu("A", isValid = true)]
        [Menu("M1", "A", isValid = true)]
        public static bool ValidFunA()
        {
            Debug.Log("ValidFunA()");
            return true;
        }

        [XMenu("B")]        
        public static void FunB()
        {
            Debug.Log("FunB()");
        }

        [XMenu("B", isValid = true)]
        public static bool ValidFunB()
        {
            Debug.Log("ValidFunB()");
            return false;
        }

        [XMenu("C/C1", separatorType = ESeparatorType.TopUp)]
        [XMenu("C/C2", separatorType = ESeparatorType.TopDown)]
        public static void FunC()
        {
            Debug.Log("FunC()");
        }

        [XMenu("D", userData = "D of XMenu")]
        [XMenu("D/D1", userData = "D1 of XMenu")]
        [XMenu("D/D2", userData = "D2 of XMenu")]
        [Menu("M2", "D", userData = "D of M2 Menu")]
        [Menu("M2", "D/D1", userData = "D1 of M2 Menu")]
        [Menu("M2", "D/D2", userData = "D2 of M2 Menu")]
        public static void FunC(object obj)
        {
            Debug.Log("FunD(): " + obj);
        }

        /// <summary>
        /// 当绘制检查器GUI
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("XMenuAttribute"))
            {
                MenuHelper.DrawMenu<XMenuAttribute>(m =>
                {
                    m.AddMenuItem("T1", () => Debug.Log("T1 of XMenu"));
                    m.AddMenuItem("T2", () => Debug.Log("T2 of XMenu"));
                });
            }

            if (GUILayout.Button("M1 of MenuAttribute"))
            {
                MenuHelper.DrawMenu("M1", m =>
                {
                    m.AddMenuItem("T1", () => Debug.Log("T1 of M1 Menu"));
                    m.AddMenuItem("T2", () => Debug.Log("T2 of M1 Menu"));
                });
            }

            if (GUILayout.Button("M2 of MenuAttribute"))
            {
                MenuHelper.DrawMenu("M2", m =>
                {
                    m.AddMenuItem("T1", () => Debug.Log("T1 of M2 Menu"));
                    m.AddMenuItem("T2", () => Debug.Log("T2 of M2 Menu"));
                });
            }
        }
    }
}
