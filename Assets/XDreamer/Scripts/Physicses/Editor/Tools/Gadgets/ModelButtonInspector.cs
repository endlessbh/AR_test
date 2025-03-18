using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorExtension.Base;
using XCSJ.EditorExtension.Base.Interactions.Tools;
using XCSJ.PluginPhysicses.Tools.Gadgets;

namespace XCSJ.EditorPhysicses.Tools.Gadgets
{
    /// <summary>
    /// 模型按钮检查器
    /// </summary>
    [Name("模型按钮检查器")]
    [CustomEditor(typeof(ModelButton))]
    [CanEditMultipleObjects]
    public class ModelButtonInspector : InteractableVirtualInspector<ModelButton>
    {
        private void OnSceneGUI()
        {
            var direction = -targetObject.transform.up;
            var orginPoint = targetObject.transform.position;
            var minPoint = orginPoint + direction * targetObject._downOffsetLength;
            var threshHoldPoint = orginPoint + direction * targetObject._threshHold;

            // 绘制轴线
            SceneGraphHelper.DrawColor(SceneGraphOption.weakInstance.lineColor, () => Handles.DrawLine(orginPoint, minPoint));

            SceneGraphHelper.DrawSphere(minPoint, targetObject._downOffsetLength.ToString(), Color.cyan);
            SceneGraphHelper.DrawSphere(threshHoldPoint, targetObject._threshHold.ToString(), Color.red);
        }
    }
}