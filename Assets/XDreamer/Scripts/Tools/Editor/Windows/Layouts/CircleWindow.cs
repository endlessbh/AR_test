using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginCommonUtils;

namespace XCSJ.EditorTools.Windows.Layouts
{
    [Name("圆形")]
    public class CircleWindow : IRectTransformLayoutWindow, ITransformLayoutWindow
    {
        [Name("展开")]
        public bool expanded { get; set; } = true;

        [Name("面向量")]
        public Vector3 planeNormal = Vector3.up;

        [Name("中心")]
        public Vector3 center = new Vector3();

        [Name("半径")]
        public float r = 160;

        [Name("起始角度")]
        public float beginAngle = 0f;

        [Name("方向")]
        public Circle.EDirection direction = Circle.EDirection.None;

        [Name("布局")]
        [Tip("圆形布局", "Circular layout")]
        [XCSJ.Attributes.Icon(index = 36233)]
        public XGUIContent CircleLayout { get; } = new XGUIContent(typeof(CircleWindow), nameof(CircleLayout));

        private void ShareDraw(Action onLayoutButtonClicked, Action onAfterDrawLayoutButton, params Transform[] standards)
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

            r = EditorGUILayout.FloatField(CommonFun.NameTooltip(this, nameof(r)), r);

            beginAngle = EditorGUILayout.Slider(CommonFun.NameTooltip(this, nameof(beginAngle)), beginAngle, 0, 360);

            direction = (Circle.EDirection)UICommonFun.EnumPopup(CommonFun.NameTooltip(this, nameof(direction)), direction);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("操作");
            if (GUILayout.Button(CircleLayout, ToolEditorWindowOption.weakInstance.defaultButtonSizeOption))
            {
                onLayoutButtonClicked?.Invoke();
            }
            onAfterDrawLayoutButton?.Invoke();
            EditorGUILayout.EndHorizontal();
        }

        bool ILayoutWindow<RectTransform>.OnGUI(List<RectTransform> list, params RectTransform[] standards)
        {
            bool ret = false;

            ShareDraw(() =>
            {
                Circle.Layout(list, center, r, beginAngle, direction);
                ret = true;
            }, null, standards as Transform[]);

            return ret;
        }

        bool ILayoutWindow<Transform>.OnGUI(List<Transform> list, params Transform[] standards)
        {
            bool ret = false;

            planeNormal = EditorGUILayout.Vector3Field(CommonFun.NameTooltip(this, nameof(planeNormal)), planeNormal);

            ShareDraw(() =>
            {
                Circle.Layout(planeNormal, list, center, r, beginAngle, direction);
                ret = true;
            }, null, standards as Transform[]);

            return ret;
        }
    }
}
