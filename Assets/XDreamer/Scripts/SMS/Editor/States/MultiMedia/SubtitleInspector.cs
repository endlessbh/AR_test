using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension.Base.Attributes;
using XCSJ.EditorSMS.States.Base;
using XCSJ.EditorXGUI;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.States.MultiMedia;

namespace XCSJ.EditorSMS.States.MultiMedia
{
    /// <summary>
    /// 字幕检查器
    /// </summary>
    [Name("字幕检查器")]
    [CustomEditor(typeof(Subtitle))]
    public class SubtitleInspector : WorkClipInspector<Subtitle>
    {
        /// <summary>
        /// 当绘制成员
        /// </summary>
        /// <param name="serializedProperty"></param>
        protected override void OnDrawMember(SerializedProperty serializedProperty, PropertyData propertyData)
        {
            switch (serializedProperty.name)
            {
                case nameof(Subtitle.subtitleClips):
                    {
                        UICommonFun.EnumButton<ESubtitleSortRule>(sr => SortssSubtitle(sr, serializedProperty), true, true, null, null, null, null, ENameTip.Image, GUILayout.ExpandWidth(true), GUILayout.Height(20));
                        break;
                    }
                case nameof(Subtitle.subtitleText):
                    {
                        EditorGUILayout.BeginHorizontal();
                        base.OnDrawMember(serializedProperty, propertyData);
                        EditorXGUIHelper.DrawCreateButton(stateComponent.subtitleText, () =>
                        {
                            ToolsMenu.CreateUIInCanvas(() =>
                            {
                                var text = ToolsMenu.CreateUIWithStyle<Text>();
                                serializedProperty.objectReferenceValue = text;
                                return text.gameObject;
                            });
                        });
                        EditorGUILayout.EndHorizontal();
                        return;
                    }
            }
            base.OnDrawMember(serializedProperty, propertyData);
        }

        void SortssSubtitle(ESubtitleSortRule eSubtitleSortRule, SerializedProperty memberProperty)
        {
            CommonFun.FocusControl();
            switch (eSubtitleSortRule)
            {
                case ESubtitleSortRule.Time:
                    {
                        SerializedObjectHelper.ArrayElementSort(memberProperty, (x, y) => stateComponent.subtitleClips[x.index].CompareTo(stateComponent.subtitleClips[y.index]));
                        break;
                    }
                case ESubtitleSortRule.Reverse:
                    {
                        SerializedObjectHelper.ArrayElementReverse(memberProperty);
                        break;
                    }
            }
        }
    }

    [Name("排序规则")]
    [Tip("对象列表内的元素进行排序", "Sort the elements in the object list")]
    public enum ESubtitleSortRule
    {
        /// <summary>
        /// 时间
        /// </summary>
        [Name("时间")]
        [XCSJ.Attributes.Icon(EIcon.NameAscendingOrder)]
        Time,

        /// <summary>
        /// 逆序
        /// </summary>
        [Name("逆序")]
        [XCSJ.Attributes.Icon(EIcon.ReverseOrder)]
        Reverse,
    }

    /// <summary>
    /// 字幕剪辑绘制器
    /// </summary>
    [CustomPropertyDrawer(typeof(SubtitleClip))]
    public class SubtitleClipDrawer : PropertyDrawerAsArrayElement<SubtitleClipDrawer.Data>
    {
        /// <summary>
        /// 数据
        /// </summary>
        public class Data : ArrayElementData
        {
            public SerializedProperty timeSP;
            public SerializedProperty textSP;

            /// <summary>
            /// 初始化
            /// </summary>
            /// <param name="property"></param>
            public override void Init(SerializedProperty property)
            {
                base.Init(property);
                timeSP = property.FindPropertyRelative(nameof(SubtitleClip.time));
                textSP = property.FindPropertyRelative(nameof(SubtitleClip.text));

                if (index == 0)
                {
                    var reorderableList = BaseInspector.GetDrawingInspector(property).targetPropertyCache.GetPropertyData(propertyData.arrayPropertyPath)?.reorderableList;
                    if (reorderableList != null)
                    {
                        reorderableList.headerHeight = 20;
                        reorderableList.drawHeaderCallback = (Rect rect) =>
                        {
                            EditorGUI.LabelField(new Rect(rect.x + 16, rect.y, LabelWidth, rect.height), "NO.");
                            EditorGUI.LabelField(new Rect(rect.x + 16 + LabelWidth, rect.y, TimeWidth, rect.height), "时间");
                            EditorGUI.LabelField(new Rect(rect.x + 16 + LabelWidth + TimeWidth, rect.y, rect.width - TimeWidth - LabelWidth, rect.height), "字幕");
                        };
                    }
                }
            }
        }

        internal const float LabelWidth = 48;
        internal const float TimeWidth = 72;
        private const float TextX = LabelWidth + TimeWidth;

        /// <summary>
        /// 绘制GUI
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //base.OnGUI(position, property, label);
            var data = cache.GetData(property);
            if (data.index >= 0)
            {
                label = EditorGUI.BeginProperty(position, data.indexContent, property);
                EditorGUI.PrefixLabel(new Rect(position.x, position.y, LabelWidth, position.height), label);
            }
            else
            {
                label = EditorGUI.BeginProperty(position, label, property);
                EditorGUI.PrefixLabel(new Rect(position.x, position.y, LabelWidth, position.height), label);
            }

            EditorGUI.PropertyField(new Rect(position.x + LabelWidth, position.y, TimeWidth, position.height), data.timeSP, GUIContent.none);
            EditorGUI.PropertyField(new Rect(position.x + TextX, position.y, position.width - TextX, position.height), data.textSP, GUIContent.none);

            EditorGUI.EndProperty();
        }
    }
}
