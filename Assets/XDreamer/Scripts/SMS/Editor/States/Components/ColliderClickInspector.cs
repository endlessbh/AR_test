using System;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorSMS.Inspectors;
using XCSJ.PluginSMS.States.Components;

namespace XCSJ.EditorSMS.States.Components
{
    /// <summary>
    /// 碰撞体点击检查器
    /// </summary>
    [Name("碰撞体点击检查器")]
    [CustomEditor(typeof(ColliderClick))]
    public class ColliderClickInspector : StateComponentInspector<ColliderClick>
    {
        /// <summary>
        /// 当绘制检查器GUI
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var targetGO = stateComponent.go;
            if (targetGO)
            {
                if (!targetGO.GetComponent<Collider>() && !targetGO.GetComponentInChildren<MeshRenderer>())
                {
                    UICommonFun.RichHelpBox("当前游戏对象没有碰撞体与网格渲染器，将自动添加立方碰撞体", MessageType.Warning);
                }
            }
        }
    }
}
