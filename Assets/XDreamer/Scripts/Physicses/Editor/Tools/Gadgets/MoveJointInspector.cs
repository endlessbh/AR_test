using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorExtension.Base;
using XCSJ.EditorExtension.Base.Interactions.Tools;
using XCSJ.PluginPhysicses.Tools.Gadgets;

namespace XCSJ.EditorPhysicses.Tools.Gadgets
{
    /// <summary>
    /// 平移机关检查器
    /// </summary>
    [Name("平移机关检查器")]
    [CustomEditor(typeof(MoveJoint))]
    [CanEditMultipleObjects]
    public class MoveJointInspector : InteractorInspector<MoveJoint>
    {
        private void OnSceneGUI()
        {
            var lineColor = SceneGraphOption.weakInstance.lineColor;
            var smallSphereRadius = SceneGraphOption.weakInstance.smallSphereRadius;

            var direction = targetObject.worldMoveAxis;
            var position = targetObject.grabbable.targetTransform.position;
            var orginPoint = Application.isPlaying ? targetObject.orgin.position : (position + targetObject._initOffset * direction);

            var minPoint = orginPoint + direction * targetObject.minValue;
            var maxPoint = orginPoint + direction * targetObject.maxValue;

            // 绘制轴线
            SceneGraphHelper.DrawColor(lineColor, () => Handles.DrawLine(minPoint, maxPoint));

            ClearPoints();

            // 使用关节时，绘制最小和最大值
            if (targetObject._useJoint)
            {
                AddPoint(minPoint, targetObject.minValue.ToString(), Color.cyan);
                AddPoint(maxPoint, targetObject.maxValue.ToString(), Color.cyan);
            }
            else // 不使用关节时绘制刻度值
            {
                for (int i = 0; i <= targetObject._stepCount; i++)
                {
                    AddPoint(minPoint, i.ToString(), Color.cyan);
                    minPoint += targetObject.stepSize * direction;
                }
            }

            AddPoint(position, "当前点", Color.green);

            AddPoint(orginPoint, "原点", Color.red);

            DrawPoint();
        }

        private List<(Vector3, string, Color)> pointlist = new List<(Vector3, string, Color)>();

        private void AddPoint(Vector3 point, string text, Color color)
        {
            for (int i = 0; i < pointlist.Count; i++)
            {
                var p = pointlist[i];
                // 两个点重合
                if (Mathf.Approximately((p.Item1 - point).sqrMagnitude, 0))
                {
                    p.Item2 += " " + text;
                    p.Item3 = color;

                    pointlist.RemoveAt(i);
                    pointlist.Insert(i, p);
                    return;
                }
            }
            pointlist.Add((point, text, color));
        }

        private void DrawPoint()
        {
            for (int i = 0; i < pointlist.Count; i++)
            {
                var p = pointlist[i];
                SceneGraphHelper.DrawSphere(p.Item1, p.Item2, p.Item3);
            }
        }

        private void ClearPoints()
        {
            pointlist.Clear();
        }
    }
}
