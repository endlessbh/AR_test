using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension.Base.Attributes;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.Extension.Base.Maths;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginMechanicalMotion.Tools;
using static XCSJ.PluginMechanicalMotion.Tools.MotionTransfer;
using static XCSJ.PluginMechanicalMotion.Tools.MotionTransfer.TransferData;

namespace XCSJ.EditorMechanicalMotion.Tools
{
    /// <summary>
    /// 运动转换数据绘制器
    /// </summary>
    [CustomPropertyDrawer(typeof(TransferData))]
    public class TransferDataDrawer : PropertyDrawerAsArrayElement<TransferDataDrawer.Data>
    {
        /// <summary>
        /// 数据项
        /// </summary>
        public class Data : ArrayElementData
        {
            public SerializedProperty transferDataTypeSP;

            public SerializedProperty inMechanismSP;
            public SerializedProperty inCustomRotationMechanismRadiusSP;
            public SerializedProperty inRotationMechanismRadiusSP;

            public SerializedProperty outMechanismSP;
            public SerializedProperty outCustomRotationMechanismRadiusSP;
            public SerializedProperty outRotationMechanismRadiusSP;

            public SerializedProperty computeTypeSP;
            public SerializedProperty multiplyValueSP;
            public SerializedProperty addValueSP;

            /// <summary>
            /// 显示
            /// </summary>
            public bool display = true;

            /// <summary>
            /// 初始化序列属性
            /// </summary>
            /// <param name="property"></param>
            public override void Init(SerializedProperty property)
            {
                base.Init(property);

                transferDataTypeSP = property.FindPropertyRelative(nameof(TransferData._transferDataType));

                inMechanismSP = property.FindPropertyRelative(nameof(TransferData._inMechanism));
                inCustomRotationMechanismRadiusSP = property.FindPropertyRelative(nameof(TransferData._inCustomRotationMechanismRadius));
                inRotationMechanismRadiusSP = property.FindPropertyRelative(nameof(TransferData._inRotationMechanismRadius));

                outMechanismSP = property.FindPropertyRelative(nameof(TransferData._outMechanism));
                outCustomRotationMechanismRadiusSP = property.FindPropertyRelative(nameof(TransferData._outCustomRotationMechanismRadius));
                outRotationMechanismRadiusSP = property.FindPropertyRelative(nameof(TransferData._outRotationMechanismRadius));

                computeTypeSP = property.FindPropertyRelative(nameof(TransferData._computeType));
                multiplyValueSP = property.FindPropertyRelative(nameof(TransferData._multiplyValue));
                addValueSP = property.FindPropertyRelative(nameof(TransferData._addValue));
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
            int rowCount = 1;// 包含标题, 
            if (data.display)
            {
                rowCount += 1;// 转换类型

                var transferDataType = (ETransferDataType)data.transferDataTypeSP.intValue;
                if (transferDataType != ETransferDataType.None)
                {
                    rowCount += 3;// 输入机构、输出机构、计算类型

                    if (data.inMechanismSP.objectReferenceValue is RotationMechanism inRM && inRM)
                    {
                        ++rowCount;
                        if (data.inCustomRotationMechanismRadiusSP.boolValue)
                        {
                            ++rowCount;
                        }
                    }

                    if (data.outMechanismSP.objectReferenceValue is RotationMechanism outRM && outRM)
                    {
                        ++rowCount;
                        if (data.outCustomRotationMechanismRadiusSP.boolValue)
                        {
                            ++rowCount;
                        }
                    }

                    var computeType = (EComputeType)data.computeTypeSP.intValue;
                    if (computeType != EComputeType.None)//乘或除
                    {
                        ++rowCount;
                    }
                    switch (computeType)
                    {
                        case EComputeType.Multiply:
                        case EComputeType.Add:
                            {
                                ++rowCount;
                                break;
                            }
                        case EComputeType.MultiplyAndAdd:
                        case EComputeType.AddAndMultiply:
                            {
                                rowCount += 2;
                                break;
                            }
                    }
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
            var rect = new Rect(position.x, position.y, position.width, 18);

            GUI.Label(rect, "", XGUIStyleLib.Get(EGUIStyle.Box));
            data.display = GUI.Toggle(rect, data.display, label, EditorStyles.foldout);
            if (!data.display) return;

            // 匹配规则
            rect.xMin += 18;

            // 运动数据变换类型
            rect = DrawProperty(rect, data.transferDataTypeSP);

            var transferDataType = (ETransferDataType)data.transferDataTypeSP.intValue;
            if (transferDataType != ETransferDataType.None)
            {
                // 输入机构
                rect = DrawProperty(rect, data.inMechanismSP);

                if (data.inMechanismSP.objectReferenceValue is RotationMechanism inRM && inRM)
                {
                    rect = DrawProperty(rect, data.inCustomRotationMechanismRadiusSP);
                    if (data.inCustomRotationMechanismRadiusSP.boolValue)
                    {
                        rect = DrawProperty(rect, data.inRotationMechanismRadiusSP);
                    }
                }

                // 输出机构
                rect = DrawProperty(rect, data.outMechanismSP);
                if (data.outMechanismSP.objectReferenceValue is RotationMechanism outRM && outRM)
                {
                    rect = DrawProperty(rect, data.outCustomRotationMechanismRadiusSP);
                    if (data.outCustomRotationMechanismRadiusSP.boolValue)
                    {
                        rect = DrawProperty(rect, data.outRotationMechanismRadiusSP);
                    }
                }
                rect = DrawProperty(rect, data.computeTypeSP);

                // 计算方式
                var computeType = (EComputeType)data.computeTypeSP.intValue;
                switch (computeType)
                {
                    case EComputeType.Multiply: rect = DrawProperty(rect, data.multiplyValueSP); break;
                    case EComputeType.Add: rect = DrawProperty(rect, data.addValueSP); break;
                    case EComputeType.MultiplyAndAdd:
                    case EComputeType.AddAndMultiply:
                        {
                            rect = DrawProperty(rect, data.multiplyValueSP); 
                            rect = DrawProperty(rect, data.addValueSP);
                            break;
                        }
                }
            }
        }

        private Rect DrawProperty(Rect rect, SerializedProperty property)
        {
            rect.y += EditorGUIUtility.singleLineHeight + 2;
            EditorGUI.PropertyField(rect, property, PropertyData.GetPropertyData(property).trLabel);
            return rect;
        }

    }
}
