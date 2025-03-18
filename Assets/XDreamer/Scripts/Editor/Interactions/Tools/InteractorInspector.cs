using UnityEditor;
using UnityEngine;
using XCSJ.EditorCommonUtils.Interactions;
using XCSJ.EditorExtension.Base.Attributes;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginTimelines.UI;
using static XCSJ.Extension.Interactions.Tools.InteractorInput;

namespace XCSJ.EditorExtension.Base.Interactions.Tools
{

    /// <summary>
    /// 交互器检查器
    /// </summary>
    [CustomEditor(typeof(Interactor), true)]
    public class InteractorInspector : InteractorInspector<Interactor>
    {
    }


    /// <summary>
    /// 交互器检查器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class InteractorInspector<T> : InteractUsageObjectInspector<T> where T : Interactor
    {
    }

    /// <summary>
    /// 交互器输入绘制器
    /// </summary>
    [CustomPropertyDrawer(typeof(InteractorInput))]
    public class InteractorInputDrawer : PropertyDrawerAsArrayElement<InteractorInputDrawer.Data>
    {
        /// <summary>
        /// 数据项
        /// </summary>
        public class Data : InteractComparerData
        {
            /// <summary>
            /// 匹配处理规则
            /// </summary>
            public SerializedProperty matchHandleRuleSP;

            /// <summary>
            /// 替换命令
            /// </summary>
            public SerializedProperty replaceCmdSP;

            /// <summary>
            /// 初始化
            /// </summary>
            /// <param name="property"></param>
            public override void Init(SerializedProperty property)
            {
                base.Init(property);

                matchHandleRuleSP = property.FindPropertyRelative(nameof(InteractorInput._matchHandleRule));
                replaceCmdSP = property.FindPropertyRelative(nameof(InteractorInput._repalceCmdName));
            }

            /// <summary>
            /// 获取行数
            /// </summary>
            /// <returns></returns>
            public override int GetRowCount()
            {
                var count = base.GetRowCount();
                if (display)
                {
                    ++count;
                    if ((EMatchHandleRule)matchHandleRuleSP.intValue == EMatchHandleRule.ReplaceCmd)
                    {
                        ++count;
                    }
                }
                return count;
            }

            /// <summary>
            /// 绘制GUI
            /// </summary>
            /// <param name="inRect"></param>
            /// <param name="label"></param>
            /// <returns></returns>
            public override Rect OnGUI(Rect inRect, GUIContent label)
            {
                var rect = base.OnGUI(inRect, label);

                if (display)
                {
                    rect = PropertyDrawerHelper.DrawProperty(rect, matchHandleRuleSP);

                    // 替换命令
                    if ((EMatchHandleRule)matchHandleRuleSP.intValue == EMatchHandleRule.ReplaceCmd)
                    {
                        rect.y += rect.height;
                        var interactor = matchHandleRuleSP.serializedObject.targetObject as InteractObject;
                        DrawInteractorCmd(rect, replaceCmdSP, interactor ? interactor.cmds : emptyCmds);
                    }
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
            var data = cache.GetData(property);
            if (data == null) return 6;
            return (base.GetPropertyHeight(property, label) +2) * data.GetRowCount();
        }

        /// <summary>
        /// 绘制
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            cache.GetData(property)?.OnGUI(position, label);
        }
    }
}
