using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginCommonUtils;

namespace XCSJ.EditorTools.Windows.Layouts
{
    /// <summary>
    /// 网格布局
    /// </summary>
    [Name("网格布局")]
    public class GridLayoutWindow : ITransformLayoutWindow
    {
        /// <summary>
        /// 展开
        /// </summary>
        [Name("展开")]
        public bool expanded { get; set; } = true;

        /// <summary>
        /// 中心
        /// </summary>
        [Name("中心")]
        public Vector3 center = Vector3.zero;

        /// <summary>
        /// 尺寸
        /// </summary>
        [Name("尺寸")]
        public Vector3 size = new Vector3(3, 3, 3);

        /// <summary>
        /// 单元格尺寸
        /// </summary>
        [Name("单元格尺寸")]
        public Vector3 cellSize = Vector3.one;

        /// <summary>
        /// 间隙
        /// </summary>
        [Name("间隙")]
        public Vector3 space = Vector3.zero;

        /// <summary>
        /// 约束规则
        /// </summary>
        [Name("约束规则")]
        [EnumPopup]
        public EConstraintRule constraintRule = EConstraintRule.Flexible;

        /// <summary>
        /// 约束规则
        /// </summary>
        [Name("约束规则")]
        public enum EConstraintRule
        {
            [Name("自由数量")]
            [Tip("对象分别沿着X,Y,Z轴正方向递增摆放", "The objects are placed incrementally along the positive directions of X, y and Z axes respectively")]
            Flexible,

            [Name("X轴上对象数量最大值")]
            FixedXAxisCount,

            [Name("Y轴上对象数量最大值")]
            FixedYAxisCount,

            [Name("Z轴上对象数量最大值")]
            FixedZAxisCount,

            [Name("XY轴上对象数量最大值")]
            FixedXYAxisCount,

            [Name("XZ轴上对象数量最大值")]
            FixedXZAxisCount,

            [Name("YZ轴上对象数量最大值")]
            FixedYZAxisCount,
        }

        /// <summary>
        /// X轴数量最大值
        /// </summary>
        [Name("X轴数量最大值")]
        public int xMaxCount = 1;

        /// <summary>
        /// Y轴数量最大值
        /// </summary>
        [Name("Y轴数量最大值")]
        public int yMaxCount = 1;

        /// <summary>
        /// Z轴数量最大值
        /// </summary>
        [Name("Z轴数量最大值")]
        public int zMaxCount = 1;

        /// <summary>
        /// 布局
        /// </summary>
        [Name("布局")]
        [Tip("网格布局", "Grid layout")]
        [XCSJ.Attributes.Icon(EIcon.Layout)]
        public XGUIContent layoutContent { get; } = new XGUIContent(typeof(GridLayoutWindow), nameof(layoutContent));

        /// <summary>
        /// 布局算法
        /// </summary>
        /// <param name="list"></param>
        /// <param name="standards"></param>
        /// <returns></returns>
        bool ILayoutWindow<Transform>.OnGUI(List<Transform> transforms, params Transform[] standards)
        {
            bool ret = false;

            Draw(() =>
            {
                Layout(transforms);
                ret = true;
            }, null, standards);

            return ret;
        }

        private void Draw(Action layout, Action onAfterDrawLayoutButton, params Transform[] standards)
        {
            EditorGUILayout.BeginHorizontal();
            center = EditorGUILayout.Vector3Field(CommonFun.NameTooltip(this, nameof(center)), center);
            if (GUILayout.Button(CommonFun.TempContent("父级中心", "以 标准(矩形)变换1 的父级(矩形)变换为基准"), GUILayout.Width(80)) && standards != null && standards.Length > 1)
            {
                var pt = standards[0].parent as Transform;
                if (pt)
                {
                    center = pt.position;
                }
            }
            EditorGUILayout.EndHorizontal();

            size = EditorGUILayout.Vector3Field(CommonFun.NameTooltip(this, nameof(size)), size);

            cellSize = EditorGUILayout.Vector3Field(CommonFun.NameTooltip(this, nameof(cellSize)), cellSize);

            space = EditorGUILayout.Vector3Field(CommonFun.NameTooltip(this, nameof(space)), space);

            constraintRule = (EConstraintRule)UICommonFun.EnumPopup(CommonFun.NameTooltip(this, nameof(constraintRule)), constraintRule);

            switch (constraintRule)
            {
                case EConstraintRule.FixedXAxisCount:
                    {
                        xMaxCount = EditorGUILayout.IntField(CommonFun.NameTooltip(this, nameof(xMaxCount)), xMaxCount);
                        break;
                    }
                case EConstraintRule.FixedYAxisCount:
                    {
                        yMaxCount = EditorGUILayout.IntField(CommonFun.NameTooltip(this, nameof(yMaxCount)), yMaxCount);
                        break;
                    }
                case EConstraintRule.FixedZAxisCount:
                    {
                        zMaxCount = EditorGUILayout.IntField(CommonFun.NameTooltip(this, nameof(zMaxCount)), zMaxCount);
                        break;
                    }
                case EConstraintRule.FixedXYAxisCount:
                    {
                        xMaxCount = EditorGUILayout.IntField(CommonFun.NameTooltip(this, nameof(xMaxCount)), xMaxCount);
                        yMaxCount = EditorGUILayout.IntField(CommonFun.NameTooltip(this, nameof(yMaxCount)), yMaxCount);
                        break;
                    }
                case EConstraintRule.FixedXZAxisCount:
                    {
                        xMaxCount = EditorGUILayout.IntField(CommonFun.NameTooltip(this, nameof(xMaxCount)), xMaxCount);
                        zMaxCount = EditorGUILayout.IntField(CommonFun.NameTooltip(this, nameof(zMaxCount)), zMaxCount);
                        break;
                    }
                case EConstraintRule.FixedYZAxisCount:
                    {
                        yMaxCount = EditorGUILayout.IntField(CommonFun.NameTooltip(this, nameof(yMaxCount)), yMaxCount);
                        zMaxCount = EditorGUILayout.IntField(CommonFun.NameTooltip(this, nameof(zMaxCount)), zMaxCount);
                        break;
                    }
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("操作");
            if (GUILayout.Button(layoutContent, ToolEditorWindowOption.weakInstance.defaultButtonSizeOption))
            {
                layout?.Invoke();
            }
            onAfterDrawLayoutButton?.Invoke();
            EditorGUILayout.EndHorizontal();
        }

        private void Layout(List<Transform> transforms)
        {
            var layoutCellSize = cellSize + space;
            var maxCount = new Vector3Int((int)(size.x / layoutCellSize.x), (int)(size.y / layoutCellSize.y), (int)(size.z / layoutCellSize.z));

            switch (constraintRule)
            {
                case EConstraintRule.FixedXAxisCount:
                    {
                        maxCount.x = Math.Min(xMaxCount, maxCount.x);
                        break;
                    }
                case EConstraintRule.FixedYAxisCount:
                    {
                        maxCount.y = Math.Min(xMaxCount, maxCount.y);
                        break;
                    }
                case EConstraintRule.FixedZAxisCount:
                    {
                        maxCount.z = Math.Min(xMaxCount, maxCount.z);
                        break;
                    }
                case EConstraintRule.FixedXYAxisCount:
                    {
                        maxCount.x = Math.Min(xMaxCount, maxCount.x);
                        maxCount.y = Math.Min(xMaxCount, maxCount.y);
                        break;
                    }
                case EConstraintRule.FixedXZAxisCount:
                    {
                        maxCount.x = Math.Min(xMaxCount, maxCount.x);
                        maxCount.z = Math.Min(xMaxCount, maxCount.z);
                        break;
                    }
                case EConstraintRule.FixedYZAxisCount:
                    {
                        maxCount.y = Math.Min(xMaxCount, maxCount.y);
                        maxCount.z = Math.Min(xMaxCount, maxCount.z);
                        break;
                    }
            }

            var beginPos = center - size / 2 + layoutCellSize/2;
            var faceCount = maxCount.x * maxCount.z;
            for (int i = 0; i < transforms.Count; i++)
            {
                int xNumber = i % maxCount.x;
                int zNumber = i % faceCount / maxCount.z;
                int yNumber = i / faceCount;

                var newPosition = beginPos + new Vector3(xNumber * layoutCellSize.x, yNumber * layoutCellSize.y, zNumber * layoutCellSize.z);
                var t = transforms[i];

                t.XSetPosition(newPosition);
            }

        }
    }
}
