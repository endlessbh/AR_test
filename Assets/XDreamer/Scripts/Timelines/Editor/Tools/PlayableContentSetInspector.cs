using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension.Base.Interactions.Tools;
using XCSJ.EditorSMS.States.TimeLine;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.Maths;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginTimelines.Tools;
using static XCSJ.PluginTimelines.Tools.PlayableContentSet;

namespace XCSJ.EditorTimelines.Tools
{
    /// <summary>
    /// 可播放内容集合检查器
    /// </summary>
    [CustomEditor(typeof(PlayableContentSet), true)]
    public class PlayableContentSetInspector : PlayableContentInspector<PlayableContentSet>
    {
        /// <summary>
        /// 绘制成员
        /// </summary>
        /// <param name="serializedProperty"></param>
        /// <param name="propertyData"></param>
        protected override void OnDrawMember(SerializedProperty serializedProperty, PropertyData propertyData)
        {
            switch (serializedProperty.name)
            {
                case nameof(PlayableContentSet._playableContentDatas):
                    {
                        targetObject.XModifyProperty(() => DrawPlayableContentDatas());
                        return;
                    }
            }
            base.OnDrawMember(serializedProperty, propertyData);
        }

        private void DrawPlayableContentDatas()
        {
            expand = UICommonFun.Foldout(expand, CommonFun.NameTip(typeof(PlayableContentSet), nameof(PlayableContentSet._playableContentDatas)), true, null, () =>
            {
                EditorGUI.BeginChangeCheck();
                var newCount = EditorGUILayout.DelayedIntField(targetObject._playableContentDatas.Count, UICommonOption.Width64);
                newCount = Mathf.Max(newCount, 0);
                if (EditorGUI.EndChangeCheck())
                {
                    var offsetCount = newCount - targetObject._playableContentDatas.Count;
                    if (offsetCount > 0)
                    {
                        for (int i = 0; i < offsetCount; i++)
                        {
                            targetObject._playableContentDatas.Add(new PlayableContentData());
                        }
                    }
                    else
                    {
                        for (int i = 0; i < -offsetCount; i++)
                        {
                            targetObject._playableContentDatas.RemoveAt(targetObject._playableContentDatas.Count - 1);
                        }
                    }
                }

                if (GUILayout.Button(CommonFun.NameTip(EIcon.Add), EditorStyles.miniButtonLeft, UICommonOption.WH24x16))
                {
                    targetObject._playableContentDatas.Add(new PlayableContentData());
                }

                if (GUILayout.Button(CommonFun.NameTip(EIcon.Delete), EditorStyles.miniButtonRight, UICommonOption.WH24x16))
                {
                    var count = targetObject._playableContentDatas.Count;
                    if (count > 0)
                    {
                        targetObject._playableContentDatas.RemoveAt(count - 1);
                    }
                }
            });
            if (expand)
            {
                CommonFun.BeginLayout();
                {
                    DrawTitle();
                    DrawContent();
                }
                CommonFun.EndLayout();
            }
        }

        private static bool expand = true;

        private const float IndexWidth = 20;

        private const float NameTitleWidth = 120;

        private const float TimeWidth = 50;

        private const float PercentWidth = 50;

        private static XGUIStyle helpBoxStyle { get; } = new XGUIStyle("HelpBox");

        private static XGUIStyle boxStyle { get; } = new XGUIStyle("box");

        private void DrawTitle()
        {
            EditorGUILayout.BeginHorizontal(boxStyle);
            {
                EditorGUILayout.LabelField("名称", GUILayout.Width(IndexWidth + NameTitleWidth));

                EditorGUILayout.LabelField(CommonFun.NameTooltip(ELock.BeginTime), GUILayout.Width(TimeWidth));

                EditorGUILayout.LabelField(CommonFun.NameTooltip(ELock.BeginPercent), GUILayout.Width(PercentWidth));

                GUILayout.Label("", GUILayout.MinWidth(20), GUILayout.ExpandWidth(true));

                EditorGUILayout.LabelField(CommonFun.NameTooltip(ELock.EndPercent), GUILayout.Width(PercentWidth));

                EditorGUILayout.LabelField(CommonFun.NameTooltip(ELock.EndTime), GUILayout.Width(TimeWidth));

                EditorGUILayout.LabelField(CommonFun.NameTooltip(ELock.TimeLength), GUILayout.Width(TimeWidth));
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawContent()
        {
            for (int i = 0; i < targetObject._playableContentDatas.Count; ++i)
            {
                DrawRow(targetObject._playableContentDatas[i], i);
            }
        }

        private void DrawRow(PlayableContentData playableContentData, int index)
        {
            try
            {
                UICommonFun.BeginHorizontal(index % 2 == 1);

                // 索引
                EditorGUILayout.LabelField(new GUIContent(string.Format("{0}", index)), GUILayout.Width(IndexWidth));

                // 可播放内容对象
                var orgColor = GUI.color;
                if (!playableContentData._playableContent) GUI.color = Color.red;
                playableContentData._playableContent = EditorGUILayout.ObjectField(playableContentData._playableContent, typeof(PlayableContent), true, GUILayout.Width(NameTitleWidth)) as PlayableContent;
                GUI.color = orgColor;

                // 开始时间
                double oldBeginTime = playableContentData.beginPercent * targetObject.timeLength;
                double oldEndTime = playableContentData.endPercent * targetObject.timeLength;

                EditorGUI.BeginChangeCheck();
                var beginTime = EditorGUILayout.DelayedDoubleField(oldBeginTime, GUILayout.Width(TimeWidth));
                beginTime = MathX.Clamp(beginTime, 0, oldEndTime);
                if (EditorGUI.EndChangeCheck())
                {
                    playableContentData.beginPercent = (float)(beginTime / targetObject.timeLength);
                }

                // 开始百分比
                EditorGUI.BeginChangeCheck();
                var beginPercent = EditorGUILayout.DelayedDoubleField(playableContentData.beginPercent * 100, GUILayout.Width(PercentWidth)) / 100;
                beginPercent = MathX.Clamp(beginPercent, 0, playableContentData.endPercent);
                if (EditorGUI.EndChangeCheck())
                {
                    playableContentData.beginPercent = beginPercent;
                }

                // 百分比滑动条
                var minValue = (float)playableContentData.beginPercent;
                var maxValue = (float)playableContentData.endPercent;
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.MinMaxSlider(ref minValue, ref maxValue, 0, 1);
                if (EditorGUI.EndChangeCheck())
                {
                    playableContentData.beginPercent = minValue;
                    playableContentData.endPercent = maxValue;
                }

                // 结束百分比
                EditorGUI.BeginChangeCheck();
                var endPercent = EditorGUILayout.DelayedDoubleField(playableContentData.endPercent * 100, GUILayout.Width(PercentWidth)) / 100;
                endPercent = MathX.Clamp(beginPercent, playableContentData.beginPercent, 1);
                if (EditorGUI.EndChangeCheck())
                {
                    playableContentData.endPercent = endPercent;
                }

                // 结束时间
                EditorGUI.BeginChangeCheck();
                var endTime = EditorGUILayout.DelayedDoubleField(oldEndTime, GUILayout.Width(TimeWidth));
                endTime = MathX.Clamp(endTime, oldBeginTime, targetObject.timeLength);
                if (EditorGUI.EndChangeCheck())
                {
                    playableContentData.endPercent = (float)(endTime / targetObject.timeLength);
                }

                // 时长
                var timeLength = EditorGUILayout.DelayedDoubleField(endTime - beginTime, GUILayout.Width(TimeWidth));
                timeLength = MathX.Clamp(timeLength, 0, targetObject.timeLength);

            }
            finally
            {
                UICommonFun.EndHorizontal();
            }
            
        }
    }
}
