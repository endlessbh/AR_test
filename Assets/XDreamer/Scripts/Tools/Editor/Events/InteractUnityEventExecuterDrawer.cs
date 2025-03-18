using UnityEditor;
using UnityEngine;
using XCSJ.EditorExtension.Base;
using XCSJ.EditorExtension.Base.Attributes;
using XCSJ.EditorExtension.Base.Interactions.Tools;
using XCSJ.Extension.Interactions.Base;
using static XCSJ.PluginTools.Events.InteractEventHandle;
using static XCSJ.PluginTools.Inputs.MouseInput;

namespace XCSJ.EditorTools.Inputs
{
    /// <summary>
    /// 交互Unity事件数据绘制器
    /// </summary>
    [CustomPropertyDrawer(typeof(InteractUnityEventExecuter))]
    public class InteractUnityEventExecuterDrawer : PropertyDrawerAsArrayElement<InteractUnityEventExecuterDrawer.Data>
    {
        /// <summary>
        /// 数据项
        /// </summary>
        public class Data : InteractComparerData
        {
            private SerializedProperty interactableUnityEventSP;
            private SerializedProperty interactableUnityEventPersistentCallsSP;// unity事件调用数组

            /// <summary>
            /// 初始化
            /// </summary>
            /// <param name="property"></param>
            public override void Init(SerializedProperty property)
            {
                base.Init(property);

                interactableUnityEventSP = property.FindPropertyRelative(nameof(InteractUnityEventExecuter._interactUnityEvent));
                interactableUnityEventPersistentCallsSP = interactableUnityEventSP.FindPropertyRelative("m_PersistentCalls.m_Calls");
            }

            /// <summary>
            /// 获取属性高度
            /// </summary>
            /// <returns></returns>
            public float GetPropertyHeight()
            {
                var standardHeight = EditorGUIUtility.singleLineHeight + 2;
                float unityEventHeight = 0;
                if (display)
                {
                    var eventCount = interactableUnityEventPersistentCallsSP.arraySize;
                    unityEventHeight = 3 * standardHeight + 2 * (eventCount == 0 ? 1 : eventCount) * (standardHeight+4);
                }
                return standardHeight * base.GetRowCount() + unityEventHeight;
            }

            /// <summary>
            /// 绘制UI
            /// </summary>
            /// <param name="rect"></param>
            /// <returns></returns>
            public override Rect OnGUI(Rect rect, GUIContent label)
            {
                rect = base.OnGUI(rect, label);
                if (display)
                {
                    rect = PropertyDrawerHelper.DrawProperty(rect, interactableUnityEventSP);
                }
                return rect;
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
            return cache.GetData(property).GetPropertyHeight();
        }

        /// <summary>
        /// 绘制
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            cache.GetData(property).OnGUI(rect, label);
        }
    }
}
