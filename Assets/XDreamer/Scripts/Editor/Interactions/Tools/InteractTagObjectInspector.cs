using System.Linq;
using UnityEditor;
using UnityEngine;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Interactions;
using XCSJ.EditorExtension.Base.Attributes;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginTools.PropertyDatas;

namespace XCSJ.EditorExtension.Base.Interactions.Tools
{
    /// <summary>
    /// 交互标签对象检查器
    /// </summary>
    [CustomEditor(typeof(InteractTagObject), true)]
    [CanEditMultipleObjects]
    public class InteractTagObjectInspector : InteractTagObjectInspector<InteractTagObject>
    {

    }

    /// <summary>
    /// 交互标签对象检查器模板
    /// </summary>
    public class InteractTagObjectInspector<T> : InteractObjectInspector<T> where T : InteractTagObject
    {

    }

    /// <summary>
    /// 基础交互属性数据绘制器
    /// </summary>
    [CustomPropertyDrawer(typeof(BaseInteractPropertyData), true)]
    public class BaseInteractPropertyDataDrawer : PropertyDrawerAsArrayElement<BaseInteractPropertyDataDrawer.Data>
    {
        /// <summary>
        /// 数据项
        /// </summary>
        public class Data : ArrayElementData
        {
            #region 序列化属性

            /// <summary>
            /// 关键字序列化属性
            /// </summary>
            public SerializedProperty keySP;

            /// <summary>
            /// 关键字值类型序列化属性
            /// </summary>
            public SerializedProperty keyValueTypeSP;

            /// <summary>
            /// 关键字值值序列化属性
            /// </summary>
            public SerializedProperty keyValueValueSP;

            /// <summary>
            /// 值序列化属性
            /// </summary>
            public SerializedProperty valueSP;

            #endregion

            /// <summary>
            /// 显示
            /// </summary>
            public bool display = true;

            /// <summary>
            /// 初始化
            /// </summary>
            /// <param name="property"></param>
            public override void Init(SerializedProperty property)
            {
                base.Init(property);

                keySP = property.FindPropertyRelative(nameof(InteractPropertyData._key));
                keyValueTypeSP = keySP.FindPropertyRelative(nameof(StringPropertyValue._propertyValueType));
                keyValueValueSP = keySP.FindPropertyRelative(nameof(StringPropertyValue._value));

                // 空值时设定初值
                if (string.IsNullOrEmpty(keyValueValueSP.stringValue))
                {
                    keyValueValueSP.stringValue = PropertyKeyCache.propertyKeys.FirstOrDefault();
                }

                valueSP = property.FindPropertyRelative(nameof(InteractPropertyData._value));
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
            return (base.GetPropertyHeight(property, label) + 2) * (cache.GetData(property).display ? 3 : 1);
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
            label = data.isArrayElement ? data.indexContent : label;

            // 标题
            var rect = new Rect(position.x, position.y, position.width, 18);
            GUI.Label(rect, "", XGUIStyleLib.Get(EGUIStyle.Box));
            data.display = GUI.Toggle(rect, data.display, label, EditorStyles.foldout);
            if (!data.display) return;

            // 匹配规则
            rect.xMin += 18;

            if (data.keyValueTypeSP.intValue == 0)// 值类型
            {
                var tmp = rect;
                tmp.width -= 100;
                tmp = PropertyDrawerHelper.DrawProperty(tmp, data.keySP);

                tmp.x += tmp.width;
                tmp.width = 100;
                data.keyValueValueSP.stringValue = UICommonFun.Popup(tmp, data.keyValueValueSP.stringValue, PropertyKeyCache.propertyKeys);

                rect.y += EditorGUIUtility.singleLineHeight + 2;
            }
            else
            {
                rect = PropertyDrawerHelper.DrawProperty(rect, data.keySP, "");
            }
            rect = PropertyDrawerHelper.DrawProperty(rect, data.valueSP, "");

        }
    }
}
