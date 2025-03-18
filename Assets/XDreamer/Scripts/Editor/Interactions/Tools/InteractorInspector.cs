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
    /// �����������
    /// </summary>
    [CustomEditor(typeof(Interactor), true)]
    public class InteractorInspector : InteractorInspector<Interactor>
    {
    }


    /// <summary>
    /// �����������
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class InteractorInspector<T> : InteractUsageObjectInspector<T> where T : Interactor
    {
    }

    /// <summary>
    /// ���������������
    /// </summary>
    [CustomPropertyDrawer(typeof(InteractorInput))]
    public class InteractorInputDrawer : PropertyDrawerAsArrayElement<InteractorInputDrawer.Data>
    {
        /// <summary>
        /// ������
        /// </summary>
        public class Data : InteractComparerData
        {
            /// <summary>
            /// ƥ�䴦�����
            /// </summary>
            public SerializedProperty matchHandleRuleSP;

            /// <summary>
            /// �滻����
            /// </summary>
            public SerializedProperty replaceCmdSP;

            /// <summary>
            /// ��ʼ��
            /// </summary>
            /// <param name="property"></param>
            public override void Init(SerializedProperty property)
            {
                base.Init(property);

                matchHandleRuleSP = property.FindPropertyRelative(nameof(InteractorInput._matchHandleRule));
                replaceCmdSP = property.FindPropertyRelative(nameof(InteractorInput._repalceCmdName));
            }

            /// <summary>
            /// ��ȡ����
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
            /// ����GUI
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

                    // �滻����
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
        /// ��ȡ������Ƹ߶�
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
        /// ����
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
