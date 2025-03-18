using UnityEditor;
using UnityEngine;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension.Base.Attributes;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.Extension.Base.Maths;
using XCSJ.PluginCommonUtils;

namespace XCSJ.EditorExtension.Base.Maths
{
    /// <summary>
    /// 三维向量数据绘制器
    /// </summary>
    [CustomPropertyDrawer(typeof(Vector3Data))]
    public class Vector3DataDrawer : PropertyDrawerAsArrayElement<Vector3DataDrawer.Data>
    {
        /// <summary>
        /// 数据项
        /// </summary>
        public class Data : ArrayElementData
        {
            public SerializedProperty dataTypeSP;
            public SerializedProperty transformSP;
            public SerializedProperty vector3SP;

            public bool display = true;

            public override void Init(SerializedProperty property)
            {
                base.Init(property);

                dataTypeSP = property.FindPropertyRelative(nameof(Vector3Data._dataType));
                transformSP = property.FindPropertyRelative(nameof(Vector3Data._transform));
                vector3SP = property.FindPropertyRelative(nameof(Vector3Data._vector3));
            }
        }

        /// <summary>
        /// 获取对象绘制高度
        /// </summary>
        /// <param name="property"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var data = cache.GetData(property);
            int rowCount = 1;// 包含标题
            if (data.display)
            {
                ++rowCount;// 数据类型
                var dataType = (EVector3DataType)data.dataTypeSP.intValue;
                switch (dataType)
                {
                    case EVector3DataType.Vector3:
                    case EVector3DataType.TransformPosition:
                    case EVector3DataType.TransformRotation:
                    case EVector3DataType.TransformLocalPosition:
                    case EVector3DataType.TransformLocalRotation:
                    case EVector3DataType.TransformLocalScale:
                    case EVector3DataType.TransformUp:
                    case EVector3DataType.TransformDown:
                    case EVector3DataType.TransformForward:
                    case EVector3DataType.TransformBack:
                    case EVector3DataType.TransformLeft:
                    case EVector3DataType.TransformRight: ++rowCount; break;
                    case EVector3DataType.TransformPoint:
                    case EVector3DataType.TransformDirection: rowCount += 2; break;
                }
            }
            return base.GetPropertyHeight(property, label) * rowCount + 6;
        }


        /// <summary>
        /// 绘制
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var data = cache.GetData(property);

            // 标题
            var rect = new Rect(position.x, position.y+2, position.width, 18);

            GUI.Label(rect, "", XGUIStyleLib.Get(EGUIStyle.Box));
            data.display = GUI.Toggle(rect, data.display, label, EditorStyles.foldout);
            if (!data.display) return;

            // 匹配规则
            rect.xMin += 18;

            // 数据类型
            rect = DrawProperty(rect, data.dataTypeSP);

            var dataType = (EVector3DataType)data.dataTypeSP.intValue;

            switch (dataType)
            {
                case EVector3DataType.Vector3: rect = DrawProperty(rect, data.vector3SP); break;
                case EVector3DataType.TransformPosition:
                case EVector3DataType.TransformRotation:
                case EVector3DataType.TransformLocalPosition:
                case EVector3DataType.TransformLocalRotation:
                case EVector3DataType.TransformLocalScale:
                case EVector3DataType.TransformUp:
                case EVector3DataType.TransformDown:
                case EVector3DataType.TransformForward:
                case EVector3DataType.TransformBack:
                case EVector3DataType.TransformLeft:
                case EVector3DataType.TransformRight:
                    {
                        rect = DrawTransformSP(rect, data.transformSP);
                        break;
                    }
                case EVector3DataType.TransformPoint:
                case EVector3DataType.TransformDirection:
                    {
                        rect = DrawTransformSP(rect, data.transformSP);
                        rect = DrawProperty(rect, data.vector3SP);
                        break;
                    }
            }
        }

        private Rect DrawTransformSP(Rect rect, SerializedProperty property)
        {
            var transformSP = property.FindPropertyRelative(nameof(TransformPropertyValue._transfrom));
            var valid = transformSP.objectReferenceValue;
            var orgColor = GUI.color;
            if (!valid)
            {
                GUI.color = Color.red;
            }
            rect = DrawProperty(rect, property);
            GUI.color = orgColor;

            return rect;
        }

        private Rect DrawProperty(Rect rect, SerializedProperty property)
        {
            rect.y += EditorGUIUtility.singleLineHeight + 2;
            EditorGUI.PropertyField(rect, property, PropertyData.GetPropertyData(property).trLabel);
            return rect;
        }

    }
}
