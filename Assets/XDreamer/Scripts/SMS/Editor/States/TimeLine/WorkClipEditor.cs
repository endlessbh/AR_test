using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Base.Kernel;
using XCSJ.Extension.Base.Attributes;
using XCSJ.Maths;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.Kernel;

namespace XCSJ.EditorSMS.States.TimeLine
{
    [Flags]
    [Name("锁定")]
    public enum ELock
    {
        [Name("无")]
        [EnumFieldName("无")]
        None = 0,

        [Name("开始时间")]
        [EnumFieldName("开始时间")]
        BeginTime = 1 << 0,

        [Name("开始%")]
        [EnumFieldName("开始%")]
        BeginPercent = 1 << 1,

        [Name("结束%")]
        [EnumFieldName("结束%")]
        EndPercent = 1 << 2,

        [Name("结束时间")]
        [EnumFieldName("结束时间")]
        EndTime = 1 << 3,

        [Name("时长")]
        [EnumFieldName("时长")]
        TimeLength = 1 << 4,

        [Name("总时长")]
        [EnumFieldName("总时长")]
        TotalTimeLength = 1 << 5,
    }

    public static class ELockWorkClipHelper
    {
        public static bool HasLock(this ELock eLock, ELock lockCheck)
        {
            return (eLock & lockCheck) != ELock.None;
        }

        public static bool IsLock(this ELock eLock, ELock lockCheck)
        {
            return (eLock & lockCheck) == lockCheck;
        }

        public static bool IsLock(this ELock eLock, int lockIndex)
        {
            return ((int)eLock & (1 << lockIndex)) == lockIndex;
        }

        public static ELock Lock(this ELock eLock, ELock lockCheck)
        {
            return eLock = lockCheck;
        }

        public static ELock LockOr(this ELock eLock, ELock lockCheck)
        {
            return eLock |= lockCheck;
        }

        public static ELock UnLock(this ELock eLock, ELock lockCheck)
        {
            return eLock &= ~lockCheck;
        }

        public static ELock SwitchLock(this ELock eLock, ELock lockCheck)
        {
            return eLock = eLock ^ lockCheck;
        }

        public static ELock SwitchLock(ref ELock eLock, ELock lockCheck)
        {
            return eLock.SwitchLock(lockCheck);
        }

        public static int ValidCount(this ELock eLock)
        {
            return BitCount((int)eLock);
        }

        public static int BitCount(this int num) => MathX.BitCount(num);

        public static ELock BeginIgnore(ref ELock eLock, ELock lockCheck)
        {
            try
            {
                return eLock.IsLock(lockCheck) ? lockCheck : ELock.None;
            }
            finally
            {
                eLock = eLock.UnLock(lockCheck);
            }
        }

        public static void EndIgnore(ref ELock eLock, ELock lockCheck)
        {
            eLock = eLock.LockOr(lockCheck);
        }
    }


    public class WorkClipEditor
    {
        private static WorkClipEditorOption _option = null;
        public static WorkClipEditorOption workClipEditorOption
        {
            get
            {
                if (_option == null)
                {
                    OnOptionModified(SMSExtensionOption.weakInstance);
                    SMSExtensionOption.onModified += OnOptionModified;
                }
                return _option;
            }
        }
        private static void OnOptionModified(SMSExtensionOption option)
        {
            _option = option.workClipEditorOption;
            NameTitleWidth = _option.nameTitleWidth;
            TitleWidth = _option.titleWidth;
        }

        public static float NameTitleWidth =100;

        public static float TitleWidth = 60;

        public static Color rowColor = new Color(0.8f, 0.8f, 0.8f, 1f);

        public static Texture2D lockIcon { get; private set; } = null;

        public static Texture2D unlockIcon { get; private set; } = null;

        public static XGUIStyle helpBoxStyle { get; } = new XGUIStyle("HelpBox");

        public static XGUIStyle boxStyle { get; } = new XGUIStyle("box");

        public static void RecordClips(List<IWorkClip> clips)
        {
            UndoHelper.RegisterCompleteObjectUndo(clips.ConvertAll(c => (UnityEngine.Object)c).ToArray());
        }

        public static void DrawTotalTimeLength(UnityEngine.Object obj, List<IWorkClip> clips, ref ELock lockValue, ref double totalTimeLength, Action onAfterTTLHorizontal = null)
        {
            EditorGUILayout.BeginHorizontal();
            {
                // 总长度
                DrawTotalTimeLengthInternal(obj, clips, ref lockValue, ref totalTimeLength);
                onAfterTTLHorizontal?.Invoke();
            }
            EditorGUILayout.EndHorizontal();

            if (Event.current.type == EventType.Repaint)
            {
                UpdateData(obj, clips, totalTimeLength);
            }
        }

        public static void DrawTitle(List<IWorkClip> clips, ref ELock lockValue, ref double totalTimeLength, Action onAfterTitle = null)
        {
            if (!lockIcon) lockIcon = EditorIconHelper.GetIconInLib(EIcon.Lock);
            if (!unlockIcon) unlockIcon = EditorIconHelper.GetIconInLib(EIcon.Unlock);

            // 时间片段列表
            DrawClipTitle(ref lockValue, onAfterTitle);
        }

        public static void Draw(UnityEngine.Object obj, List<IWorkClip> clips, ref ELock lockValue, ref double totalTimeLength,
            Action OnAfterTotalTimeFun = null, Action OnAfterTitleFun = null)
        {
            DrawTotalTimeLength(obj, clips, ref lockValue, ref totalTimeLength, OnAfterTotalTimeFun);

            try
            {
                EditorGUILayout.BeginHorizontal(boxStyle);

                DrawTitle(clips, ref lockValue, ref totalTimeLength, OnAfterTitleFun);
            }
            finally
            {
                EditorGUILayout.EndHorizontal();
            }

            Draw(obj, clips, lockValue, totalTimeLength);
        }

        #region 总时长

        private static void DrawTotalTimeLengthInternal(UnityEngine.Object obj, List<IWorkClip> clips, ref ELock lockValue, ref double totalTimeLength)
        {
            //var lockTTL = lockValue.IsLock(ELock.TotalTimeLength);
            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginDisabledGroup(lockValue.IsLock(ELock.TotalTimeLength));
            EditorGUI.BeginChangeCheck();
            var newTTL = EditorGUILayout.DelayedDoubleField("总时长", totalTimeLength);
            var modifyTTL = EditorGUI.EndChangeCheck();
            EditorGUI.EndDisabledGroup();
            DrawTitleButton(ref lockValue, ELock.TotalTimeLength);
            EditorGUILayout.EndHorizontal();
            if (lockValue.IsLock(ELock.TotalTimeLength)) return;

            if (newTTL < 0) newTTL = 0;

            if (lockValue.IsLock(ELock.BeginTime) && GetMaxBeginTime(clips) > newTTL)
            {
                //EditorCommon.DisplayPromptDialog("开始时间已锁定，总时长不能比最大开始时间短！");
                return;
            }

            if (lockValue.IsLock(ELock.EndTime) && GetMaxEndTime(clips) > newTTL)
            {
                //EditorCommon.DisplayPromptDialog("结束时间已锁定，总时长不能比最大结束时间短！");
                return;
            }

            if (lockValue.IsLock(ELock.TimeLength) && GetMaxTimeLength(clips) > newTTL)
            {
                //EditorCommon.DisplayPromptDialog("时长已锁定，总时长不能比最大时长短！");
                return;
            }

            if (modifyTTL)
            {
                RecordClips(clips);

                totalTimeLength = newTTL;
                foreach (var clip in clips)
                {
                    SetTotalTimeLength(obj, lockValue, totalTimeLength, clip);
                }
            }
        }

        public static double GetMaxTotalTimeLength(IEnumerable<IWorkClip> clips)
        {
            double value = 0;
            foreach (var clip in clips)
            {
                value = Math.Max(value, clip.totalTimeLength);
            }
            return value;
        }

        public static double GetMaxBeginTime(IEnumerable<IWorkClip> clips)
        {
            double value = 0;
            foreach (var clip in clips)
            {
                value = Math.Max(value, clip.beginTime);
            }
            return value;
        }

        public static double GetMaxEndTime(IEnumerable<IWorkClip> clips)
        {
            double value = 0;
            foreach (var clip in clips)
            {
                value = Math.Max(value, clip.endTime);
            }
            return value;
        }

        public static double GetMaxTimeLength(IEnumerable<IWorkClip> clips)
        {
            double value = 0;
            foreach (var clip in clips)
            {
                value = Math.Max(value, clip.timeLength);
            }
            return value;
        }

        #endregion

        #region 标题

        private static void DrawTitleButton(ref ELock lockValue, ELock lockCheck)
        {
            var content = CommonFun.NameTooltip(lockCheck);
            content.image = lockValue.IsLock(lockCheck) ? lockIcon : unlockIcon;
            if (GUILayout.Button(content, helpBoxStyle, GUILayout.Width(TitleWidth), GUILayout.Height(20)))
            {
                SwitchLocks(ref lockValue, lockCheck);
            }
        }

        private static void DrawClipTitle(ref ELock lockValue, Action onAfterTitle = null)
        {
            var option = workClipEditorOption;

            EditorGUILayout.LabelField("名称", GUILayout.Width(NameTitleWidth));

            if (option.beginTime) DrawTitleButton(ref lockValue, ELock.BeginTime);
            if (option.beginPercent) DrawTitleButton(ref lockValue, ELock.BeginPercent);

            //GUILayout.FlexibleSpace();
            if (option.slider) GUILayout.Label("", GUILayout.MinWidth(20));            

            if (option.endPercent) DrawTitleButton(ref lockValue, ELock.EndPercent);
            if (option.endTime) DrawTitleButton(ref lockValue, ELock.EndTime);
            if (option.timeLength) DrawTitleButton(ref lockValue, ELock.TimeLength);

            onAfterTitle?.Invoke();

            if (!option.slider) GUILayout.FlexibleSpace();
        }

        private static void SwitchLocks(ref ELock lockValue, ELock eLock)
        {
            lockValue = lockValue.SwitchLock(eLock);

            if (lockValue.IsLock(eLock))
            {
                switch (eLock)
                {
                    case ELock.BeginTime:
                        {
                            lockValue = lockValue.UnLock(ELock.BeginPercent);
                            break;
                        }
                    case ELock.BeginPercent:
                        {
                            lockValue = lockValue.UnLock(ELock.BeginTime);
                            break;
                        }
                    case ELock.EndPercent:
                        {
                            lockValue = lockValue.UnLock(ELock.EndTime);
                            break;
                        }
                    case ELock.EndTime:
                        {
                            lockValue = lockValue.UnLock(ELock.EndPercent);
                            break;
                        }
                }
            }

            var tmpLock = ELockWorkClipHelper.BeginIgnore(ref lockValue, ELock.TotalTimeLength);
            if (lockValue.ValidCount() > 2)
            {
                lockValue = lockValue.SwitchLock(eLock);
            }
            ELockWorkClipHelper.EndIgnore(ref lockValue, tmpLock);
        }

        #endregion

        #region 工作剪辑列表

        private static GUIContent IndexNameTitle(int index, IWorkClip workClip)
        {
            string text = string.Format("{0}.{1}", (index + 1).ToString(), workClip.name);
            string tooltip = string.Format("{0}\n类型:{1}\n", workClip.name, workClip.GetType());
            return new GUIContent(text, tooltip);
        }

        private static void Draw(UnityEngine.Object obj, List<IWorkClip> workClips, ELock lockValue, double totalTimeLength)
        {
            for (int i = 0; i < workClips.Count; ++i)
            {
                var workClip = workClips[i];

                try
                {
                    UICommonFun.BeginHorizontal(i % 2 == 1);
                    Draw(obj, i, workClip, lockValue, totalTimeLength);
                }
                finally
                {
                    UICommonFun.EndHorizontal();
                }
            }
        }

        public static void Draw(UnityEngine.Object obj, int index, IWorkClip workClip, ELock lockValue, double totalTimeLength)
        {
            // 索引与名称
            EditorGUILayout.LabelField(IndexNameTitle(index, workClip), GUILayout.Width(NameTitleWidth));

            var option = workClipEditorOption;

            if (option.beginTime)// 开始时间
            {                
                try
                {
                    EditorGUI.BeginDisabledGroup(lockValue.IsLock(ELock.BeginTime));
                    {
                        var beginTime = EditorGUILayout.DelayedDoubleField(workClip.beginTime, GUILayout.Width(TitleWidth));
                        var maxBeginTime = lockValue.IsLock(ELock.TimeLength) ? totalTimeLength - workClip.timeLength : workClip.endTime;
                        beginTime = MathX.Clamp(beginTime, 0, maxBeginTime);
                        SetData(obj, ELock.BeginTime, lockValue, workClip.beginTime, beginTime, workClip, totalTimeLength);
                    }
                }
                finally
                {
                    EditorGUI.EndDisabledGroup();
                }
            }

            if (option.beginPercent)// 开始百分比
            {                
                try
                {
                    EditorGUI.BeginDisabledGroup(lockValue.IsLock(ELock.BeginPercent));
                    {
                        var beginPercent = EditorGUILayout.DelayedDoubleField(workClip.beginPercent * 100, GUILayout.Width(TitleWidth)) / 100;
                        beginPercent = MathX.Clamp(beginPercent, 0, workClip.endPercent);
                        SetData(obj, ELock.BeginPercent, lockValue, workClip.beginPercent, beginPercent, workClip, totalTimeLength);
                    }
                }
                finally
                {
                    EditorGUI.EndDisabledGroup();
                }
            }

            if (option.slider)//限定范围的左右滑动条      
            {
                var minValue = (float)workClip.beginPercent;
                var maxValue = (float)workClip.endPercent;
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.MinMaxSlider(ref minValue, ref maxValue, 0, 1);
                if(EditorGUI.EndChangeCheck())
                {
                    if (!lockValue.IsLock(ELock.BeginPercent))
                    {
                        SetData(obj, ELock.BeginPercent, lockValue, workClip.beginPercent, minValue, workClip, totalTimeLength);
                    }
                    if (!lockValue.IsLock(ELock.EndPercent))
                    {
                        SetData(obj, ELock.EndPercent, lockValue, workClip.endPercent, maxValue, workClip, totalTimeLength);
                    }
                }
            }
            else
            {
                GUILayout.FlexibleSpace();
            }

            if (option.endPercent) // 结束百分比
            {               
                try
                {
                    EditorGUI.BeginDisabledGroup(lockValue.IsLock(ELock.EndPercent));
                    {
                        var endPercent = EditorGUILayout.DelayedDoubleField(workClip.endPercent * 100, GUILayout.Width(TitleWidth)) / 100;
                        endPercent = MathX.Clamp(endPercent, workClip.beginPercent, 1);
                        SetData(obj, ELock.EndPercent, lockValue, workClip.endPercent, endPercent, workClip, totalTimeLength);
                    }
                }
                finally
                {
                    EditorGUI.EndDisabledGroup();
                }
            }

            if(option.endTime)// 结束时间
            {                
                try
                {
                    EditorGUI.BeginDisabledGroup(lockValue.IsLock(ELock.EndTime));
                    {
                        var endTime = EditorGUILayout.DelayedDoubleField(workClip.endTime, GUILayout.Width(TitleWidth));
                        var minTime = lockValue.IsLock(ELock.TimeLength) ? workClip.timeLength : workClip.beginTime;
                        endTime = MathX.Clamp(endTime, minTime, totalTimeLength);
                        SetData(obj, ELock.EndTime, lockValue, workClip.endTime, endTime, workClip, totalTimeLength);
                    }
                }
                finally
                {
                    EditorGUI.EndDisabledGroup();
                }
            }

            if (option.timeLength) // 时间长度
            {               
                try
                {
                    EditorGUI.BeginDisabledGroup(lockValue.IsLock(ELock.TimeLength));
                    {
                        var timeLength = EditorGUILayout.DelayedDoubleField(workClip.timeLength, GUILayout.Width(TitleWidth));
                        timeLength = MathX.Clamp(timeLength, 0, totalTimeLength);
                        SetData(obj, ELock.TimeLength, lockValue, workClip.timeLength, timeLength, workClip, totalTimeLength);
                    }
                }
                finally
                {
                    EditorGUI.EndDisabledGroup();
                }
            }
        }

        public static bool Draw(UnityEngine.Object obj, Rect rect, int index, IWorkClip workClip, ELock lockValue, double totalTimeLength, float afterHorizontalSpaceWidth, Func<Rect, bool> onAfterHorizontal)
        {
            float spaceWidth = 4;
            float rowHeight = 16;
            var titleCount = 0;

            // 序号
            var tmpRect = rect;
            tmpRect.height = rowHeight;
            tmpRect.width = NameTitleWidth;
            EditorGUI.LabelField(tmpRect, IndexNameTitle(index, workClip));

            var option = workClipEditorOption;

            if (option.beginTime)// 开始时间
            {
                titleCount++;
                try
                {
                    EditorGUI.BeginDisabledGroup(lockValue.IsLock(ELock.BeginTime));
                    {
                        tmpRect.x += tmpRect.width + spaceWidth;
                        tmpRect.width = TitleWidth;
                        var beginTime = EditorGUI.DelayedDoubleField(tmpRect, workClip.beginTime);
                        beginTime = MathX.Clamp(beginTime, 0, workClip.endTime);
                        SetData(obj, ELock.BeginTime, lockValue, workClip.beginTime, beginTime, workClip, totalTimeLength);
                    }
                }
                finally
                {
                    EditorGUI.EndDisabledGroup();
                }
            }

            if (option.beginPercent)// 开始百分比
            {
                titleCount++;
                try
                {
                    EditorGUI.BeginDisabledGroup(lockValue.IsLock(ELock.BeginPercent));
                    {
                        tmpRect.x += tmpRect.width + spaceWidth;
                        tmpRect.width = TitleWidth;
                        var beginPercent = EditorGUI.DelayedDoubleField(tmpRect, workClip.beginPercent * 100) / 100;
                        beginPercent = MathX.Clamp(beginPercent, 0, workClip.endPercent);
                        SetData(obj, ELock.BeginPercent, lockValue, workClip.beginPercent, beginPercent, workClip, totalTimeLength);
                    }
                }
                finally
                {
                    EditorGUI.EndDisabledGroup();
                }
            }

            if (option.slider) //限定范围的左右滑动条
            {
                #region 计算弹性区域的宽度等信息

                if (option.endPercent) titleCount++;
                if (option.endTime) titleCount++;
                if (option.timeLength) titleCount++;
                tmpRect.x += tmpRect.width + spaceWidth;
                tmpRect.width = rect.width - NameTitleWidth - TitleWidth * titleCount - (titleCount + 1) * spaceWidth - afterHorizontalSpaceWidth;
                if (tmpRect.width < 20) tmpRect.width = 20;

                #endregion

                float minValue = (float)workClip.beginPercent;
                float maxValue = (float)workClip.endPercent;
                EditorGUI.BeginChangeCheck();
                EditorGUI.MinMaxSlider(tmpRect, ref minValue, ref maxValue, 0, 1);
                if (EditorGUI.EndChangeCheck())
                {
                    minValue = Mathf.Clamp(minValue, 0, 1);
                    maxValue = Mathf.Clamp(maxValue, 0, 1);
                    if (!lockValue.IsLock(ELock.BeginPercent))
                    {
                        SetData(obj, ELock.BeginPercent, lockValue, workClip.beginPercent, minValue, workClip, totalTimeLength);
                    }
                    if (!lockValue.IsLock(ELock.EndPercent))
                    {
                        SetData(obj, ELock.EndPercent, lockValue, workClip.endPercent, maxValue, workClip, totalTimeLength);
                    }
                }                
            }
            else
            {
                //GUI.Label(tmpRect, "");
            }

            if (option.endPercent)// 结束百分比
            {
                titleCount++;
                try
                {
                    EditorGUI.BeginDisabledGroup(lockValue.IsLock(ELock.EndPercent));
                    {
                        tmpRect.x += tmpRect.width + spaceWidth;
                        tmpRect.width = TitleWidth;
                        var endPercent = EditorGUI.DelayedDoubleField(tmpRect, workClip.endPercent * 100) / 100;
                        endPercent = MathX.Clamp(endPercent, workClip.beginPercent, 1);
                        SetData(obj, ELock.EndPercent, lockValue, workClip.endPercent, endPercent, workClip, totalTimeLength);
                    }
                }
                finally
                {
                    EditorGUI.EndDisabledGroup();
                }
            }

            if (option.endTime)// 结束时间
            {
                titleCount++;
                try
                {
                    EditorGUI.BeginDisabledGroup(lockValue.IsLock(ELock.EndTime));
                    {
                        tmpRect.x += tmpRect.width + spaceWidth;
                        tmpRect.width = TitleWidth;
                        var endTime = EditorGUI.DelayedDoubleField(tmpRect, workClip.endTime);
                        endTime = MathX.Clamp(endTime, workClip.beginTime, totalTimeLength);
                        SetData(obj, ELock.EndTime, lockValue, workClip.endTime, endTime, workClip, totalTimeLength);
                    }
                }
                finally
                {
                    EditorGUI.EndDisabledGroup();
                }
            }

            if (option.timeLength)// 时间长度
            {
                titleCount++;
                try
                {
                    EditorGUI.BeginDisabledGroup(lockValue.IsLock(ELock.TimeLength));
                    {
                        tmpRect.x += tmpRect.width + spaceWidth;
                        tmpRect.width = TitleWidth;
                        var timeLength = EditorGUI.DelayedDoubleField(tmpRect, workClip.timeLength);
                        timeLength = MathX.Clamp(timeLength, 0, totalTimeLength);
                        SetData(obj, ELock.TimeLength, lockValue, workClip.timeLength, timeLength, workClip, totalTimeLength);
                    }
                }
                finally
                {
                    EditorGUI.EndDisabledGroup();
                }
            }

            tmpRect.x += tmpRect.width + spaceWidth;
            if (onAfterHorizontal != null)
            {
                return onAfterHorizontal(tmpRect);
            }
            return false;
        }

        #endregion

        #region 更新数据

        /// <summary>
        /// 根据百分比，更新数据信息
        /// </summary>
        /// <param name="clips"></param>
        /// <param name="totalTimeLength"></param>
        private static void UpdateData(UnityEngine.Object obj, List<IWorkClip> clips, double totalTimeLength)
        {
            WorkClipRecorder recorder = new WorkClipRecorder(obj);

            clips.Foreach(c =>
            {
                recorder.Record(c, totalTimeLength);
                recorder.KeepPercent();
                recorder.Recover(c);
            });
        }

        /// <summary>
        /// 总时长变化之后，计算时间片参数
        /// </summary>
        /// <param name="newTotalTimeLength">新的总时长</param>
        /// <param name="workClip">时间片</param>
        private static void SetTotalTimeLength(UnityEngine.Object obj, ELock lockValue, double newTotalTimeLength, IWorkClip workClip)
        {
            WorkClipRecorder recorder = new WorkClipRecorder(obj, workClip, newTotalTimeLength);
            {
                switch (lockValue.ValidCount())
                {
                    case 0:
                        {
                            recorder.KeepPercent();
                            break;
                        }
                    case 1:
                        {
                            if (lockValue.IsLock(ELock.BeginTime))
                            {
                                recorder.KeepBeginTime();
                            }
                            else if (lockValue.IsLock(ELock.BeginPercent) || lockValue.IsLock(ELock.EndPercent))
                            {
                                recorder.KeepPercent();
                            }
                            else if (lockValue.IsLock(ELock.EndTime))
                            {
                                recorder.KeepEndTime();
                            }
                            else if (lockValue.IsLock(ELock.TimeLength))
                            {
                                recorder.KeepTime();
                            }
                            break;
                        }
                    case 2:
                        {
                            if (lockValue.IsLock(ELock.BeginTime))
                            {
                                if (lockValue.IsLock(ELock.EndPercent))
                                {
                                    recorder.KeepBeginTime();
                                }
                                else if (lockValue.IsLock(ELock.EndTime))
                                {
                                    recorder.KeepTime();
                                }
                                else if (lockValue.IsLock(ELock.TimeLength))
                                {
                                    recorder.KeepTime();
                                }
                            }
                            else if (lockValue.IsLock(ELock.BeginPercent))
                            {
                                if (lockValue.IsLock(ELock.EndPercent))
                                {
                                    recorder.KeepPercent();
                                }
                                else if (lockValue.IsLock(ELock.EndTime))
                                {
                                    recorder.KeepEndTime();
                                }
                                else if (lockValue.IsLock(ELock.TimeLength))
                                {
                                    recorder.KeepTimeLengthAndBeginPercent();
                                }
                            }
                            else if (lockValue.IsLock(ELock.EndPercent) && lockValue.IsLock(ELock.TimeLength))
                            {
                                recorder.KeepTimeLengthAndEndPercent();
                            }
                            else if (lockValue.IsLock(ELock.EndTime) && lockValue.IsLock(ELock.TimeLength))
                            {
                                recorder.KeepTime();
                            }
                            break;
                        }
                }
            }
            recorder.Recover(workClip);
        }

        private static void SetData(UnityEngine.Object obj, ELock dataType, ELock lockValue, double oldValue, double newValue, IWorkClip workClip, double totalTimeLength)
        {
            if (MathX.Approximately(oldValue, newValue)) return;
            var tmpLock = ELockWorkClipHelper.BeginIgnore(ref lockValue, ELock.TotalTimeLength);
            try
            {
                if (lockValue.ValidCount() >= 2) return;
                switch (dataType)
                {
                    case ELock.BeginTime:
                        {
                            SetBeginTime(obj, lockValue, newValue, workClip, totalTimeLength);
                            break;
                        }
                    case ELock.EndTime:
                        {
                            SetEndTime(obj, lockValue, newValue, workClip, totalTimeLength);
                            break;
                        }
                    case ELock.BeginPercent:
                        {
                            SetBeginPercent(obj, lockValue, newValue, workClip, totalTimeLength);
                            break;
                        }
                    case ELock.EndPercent:
                        {
                            SetEndPercent(obj, lockValue, newValue, workClip, totalTimeLength);
                            break;
                        }
                    case ELock.TimeLength:
                        {
                            SetTimeLength(obj, lockValue, newValue, workClip, totalTimeLength);
                            break;
                        }
                }
            }
            finally
            {
                ELockWorkClipHelper.EndIgnore(ref lockValue, tmpLock);
            }
        }

        private static void SetBeginTime(UnityEngine.Object obj, ELock lockValue, double newBeginTime, IWorkClip workClip, double totalTimeLength)
        {
            // 在总时间不变的情况下，处于锁定状态
            if (lockValue.IsLock(ELock.BeginPercent)) return;

            WorkClipRecorder recorder = new WorkClipRecorder(obj, workClip, totalTimeLength);
            {
                // 未锁定百分比
                recorder.SetBeginTime(newBeginTime);

                // 时长锁定
                if (lockValue.IsLock(ELock.TimeLength))
                {
                    recorder.KeepTimeLengthOnBeginTime();
                }
            }
            recorder.Recover(workClip);
        }

        private static void SetEndTime(UnityEngine.Object obj, ELock lockValue, double newEndTime, IWorkClip workClip, double totalTimeLength)
        {
            // 在总时间不变的情况下，处于锁定状态
            if (lockValue.IsLock(ELock.EndPercent)) return;

            WorkClipRecorder recorder = new WorkClipRecorder(obj, workClip, totalTimeLength);
            {
                recorder.SetEndTime(newEndTime);

                // 时长锁定
                if (lockValue.IsLock(ELock.TimeLength))
                {
                    recorder.KeepTimeLengthOnEndTime();
                }
            }
            recorder.Recover(workClip);
        }

        private static void SetBeginPercent(UnityEngine.Object obj, ELock lockValue, double newBeginPercent, IWorkClip workClip, double totalTimeLength)
        {
            // 在总时间不变的情况下，处于锁定状态
            if (lockValue.IsLock(ELock.BeginTime)) return;

            WorkClipRecorder recorder = new WorkClipRecorder(obj, workClip, totalTimeLength);
            {
                recorder.SetBeginPercent(newBeginPercent);

                // 时长锁定
                if (lockValue.IsLock(ELock.TimeLength))
                {
                    recorder.KeepTimeLengthOnBeginTime();
                }
            }
            recorder.Recover(workClip);
        }

        private static void SetEndPercent(UnityEngine.Object obj, ELock lockValue, double newEndPercent, IWorkClip workClip, double totalTimeLength)
        {
            // 在总时间不变的情况下，处于锁定状态
            if (lockValue.IsLock(ELock.EndTime)) return;

            WorkClipRecorder recorder = new WorkClipRecorder(obj, workClip, totalTimeLength);
            {
                recorder.SetEndPercent(newEndPercent);

                // 时长锁定
                if (lockValue.IsLock(ELock.TimeLength))
                {
                    recorder.KeepTimeLengthOnEndTime();
                }
            }
            recorder.Recover(workClip);
        }

        private static void SetTimeLength(UnityEngine.Object obj, ELock lockValue, double newTimeLength, IWorkClip workClip, double totalTimeLength)
        {
            WorkClipRecorder recorder = new WorkClipRecorder(obj, workClip, totalTimeLength);
            {
                switch (lockValue.ValidCount())
                {
                    case 0:
                        {
                            recorder.SetTimeLength(newTimeLength);
                            break;
                        }
                    case 1:
                        {
                            if (lockValue.IsLock(ELock.BeginTime) || lockValue.IsLock(ELock.BeginPercent))
                            {
                                recorder.OnTimeLengthChangeFixBeginTime(newTimeLength);
                            }
                            if (lockValue.IsLock(ELock.EndPercent) || lockValue.IsLock(ELock.EndTime))
                            {
                                recorder.OnTimeLengthChangeFixEndTime(newTimeLength);
                            }
                            break;
                        }
                }
            }
            recorder.Recover(workClip);
        }

        #endregion
    }
}
