using System;
using System.Text;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension.Base;
using XCSJ.EditorExtension.Base.XUnityEditor;
using XCSJ.EditorSMS.Inspectors;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Extension.Base.Tweens;
using XCSJ.Languages;
using XCSJ.Maths;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.EditorSMS.States.Base
{
    /// <summary>
    /// 工作剪辑检查器
    /// </summary>
    [CustomEditor(typeof(WorkClip), true)]
    [Serializable]
    [Name("工作剪辑检查器")]
    public class WorkClipInspector : WorkClipInspector<WorkClip> { }

    /// <summary>
    /// 工作剪辑检查器泛型类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class WorkClipInspector<T> : StateComponentInspector<T> where T : WorkClip
    {
        /// <summary>
        /// 工作剪辑对象
        /// </summary>
        public T workClip => target as T;

        private bool workClipValidity = true;

        private Color guiColor;

        private const float ButtonSize = 32;

        private Vector2 lockRatioButtonSize = new Vector2(ButtonSize, ButtonSize);

        #region 锁定时间与百分比的比例关系

        void SetTimeRangeLimitMaxTimeLength(double limitMaxTimeLength)
        {
            workClip.workRange.timeRange._limitMaxTimeLength = limitMaxTimeLength;
        }

        /// <summary>
        /// 绘制锁定比例按钮
        /// </summary>
        private void DrawLockRatioButton(SerializedProperty workRangeSP)
        {
            var lockRatioOfWorkRangeSP = workRangeSP.serializedObject.FindProperty(nameof(WorkClip.lockRatioOfWorkRange));
            var lockRatio = lockRatioOfWorkRangeSP.boolValue;
            var lockRatioNew = UICommonFun.ButtonToggle(TrLabelByTarget(nameof(WorkClip.lockRatioOfWorkRange), ENameTip.EmptyTextWhenHasImage), lockRatio, GUI.skin.button, GUILayout.Width(lockRatioButtonSize.x + 2), GUILayout.Height(lockRatioButtonSize.y + 8));
            if (lockRatioNew != lockRatio)
            {
                lockRatioOfWorkRangeSP.boolValue = lockRatioNew;
                if (lockRatioNew)//仅在设为锁定的那一刻执行一次
                {
                    workRangeSP.serializedObject.FindProperty(nameof(WorkClip.syncTL)).boolValue = false;
                    workRangeSP.serializedObject.FindProperty(nameof(WorkClip.ttlOfLockRatio)).doubleValue = workClip.totalTimeLength;
                }
            }
        }

        #endregion

        #region 同步TL

        /// <summary>
        /// 标识是否有同步时长按钮
        /// </summary>
        /// <returns></returns>
        protected virtual bool HasSyncTLButton() => false;

        /// <summary>
        /// 获取同步时长按钮内容
        /// </summary>
        /// <returns></returns>
        protected virtual GUIContent GetSyncTLButtonContent() => TrLabelByTarget(nameof(WorkClip.syncTL), ENameTip.EmptyTextWhenHasImage);

        /// <summary>
        /// 获取预期的时长
        /// </summary>
        /// <returns></returns>
        protected virtual double? GetExpectedTL() => default;

        /// <summary>
        /// 绘制同步时长按钮
        /// </summary>
        /// <param name="workRangeSP"></param>
        private void DrawSyncTLButton(SerializedProperty workRangeSP)
        {
            if (!HasSyncTLButton()) return;

            var syncTLSP = workRangeSP.serializedObject.FindProperty(nameof(WorkClip.syncTL));
            var syncTL = syncTLSP.boolValue;
            var syncTLNew = UICommonFun.ButtonToggle(GetSyncTLButtonContent(), syncTL, GUI.skin.button, GUILayout.Width(lockRatioButtonSize.x + 2), GUILayout.Height(lockRatioButtonSize.y + 8));
            if (syncTLNew != syncTL)
            {
                syncTLSP.boolValue = syncTLNew;
            }
            if (syncTLNew)//只要同步就一直执行
            {
                var tl = GetExpectedTL();
                if (tl.HasValue)
                {
                    var timeRangeSP = workRangeSP.FindPropertyRelative(nameof(WorkRange.timeRange)).FindPropertyRelative(nameof(TimeRange.timeRange));
                    var xSP = timeRangeSP.FindPropertyRelative(nameof(V2D.x));
                    var ySP = timeRangeSP.FindPropertyRelative(nameof(V2D.y));

                    //更新时长
                    ySP.doubleValue = xSP.doubleValue + Math.Max(0, tl.Value);

                    //同步更新百分比
                    if (workClip.lockRatioOfWorkRange && !MathX.ApproximatelyZero(workClip.ttlOfLockRatio))
                    {
                        //由时间更新百分比
                        var x = xSP.doubleValue / workClip.ttlOfLockRatio;
                        var y = ySP.doubleValue / workClip.ttlOfLockRatio;

                        var percentRangeSP = workRangeSP.FindPropertyRelative(nameof(WorkRange.percentRange)).FindPropertyRelative(nameof(PercentRange.percentRange));
                        percentRangeSP.FindPropertyRelative(nameof(V2D.x)).doubleValue = MathX.Clamp01(x);
                        percentRangeSP.FindPropertyRelative(nameof(V2D.y)).doubleValue = MathX.Clamp01(y);
                    }
                }
                workRangeSP.serializedObject.FindProperty(nameof(WorkClip.lockRatioOfWorkRange)).boolValue = false;
                workRangeSP.serializedObject.FindProperty(nameof(WorkClip.ttlOfLockRatio)).doubleValue = workClip.totalTimeLength;
            }
        }

        #endregion

        #region 同步OTL

        /// <summary>
        /// 获取同步单次时长按钮内容
        /// </summary>
        /// <returns></returns>
        protected virtual GUIContent GetSyncOTLButtonContent() => TrLabelByTarget(nameof(WorkClip.syncOTL));

        /// <summary>
        /// 获取预期的单次时长
        /// </summary>
        /// <returns></returns>
        protected virtual double? GetExpectedOTL() => workClip.timeLength;

        /// <summary>
        /// 绘制同步单次时长按钮
        /// </summary>
        /// <param name="onceTimeLengthSP"></param>
        private void DrawSyncOTLButton(SerializedProperty onceTimeLengthSP)
        {
            var syncOTLSP = onceTimeLengthSP.serializedObject.FindProperty(nameof(WorkClip.syncOTL));
            var syncOTL = syncOTLSP.boolValue;
            var syncOTLNew = UICommonFun.ButtonToggle(GetSyncOTLButtonContent(), syncOTL, EditorStyles.miniButtonRight, GUILayout.Width(60));
            if (syncOTLNew != syncOTL)
            {
                syncOTLSP.boolValue = syncOTLNew;
            }
            if (syncOTLNew)//只要同步就一直执行
            {
                var otl = GetExpectedOTL();
                if (otl.HasValue)
                {
                    onceTimeLengthSP.doubleValue = Math.Max(0, otl.Value);
                }
            }
        }

        #endregion

        #region 工作曲线

        /// <summary>
        /// 工作曲线范围
        /// </summary>
        public virtual Rect workCurveRange { get; } = new Rect(0, 0, 1, 1);

        /// <summary>
        /// 工作曲线颜色
        /// </summary>
        public virtual Color workCurveColor { get; } = Color.green;

        /// <summary>
        /// 曲线库按钮尺寸
        /// </summary>
        public Vector2 curveLibButtonSize = new Vector2(ButtonSize, ButtonSize);

        /// <summary>
        /// 上下镜像曲线
        /// </summary>
        [Name("上下镜像曲线")]
        [XCSJ.Attributes.Icon(EIcon.UpDownMirror)]
        public bool upDownMirrorCurve;

        /// <summary>
        /// 左右镜像曲线
        /// </summary>
        [Name("左右镜像曲线")]
        [XCSJ.Attributes.Icon(EIcon.LeftRightMirror)]
        public bool leftRightMirrorCurve;

        /// <summary>
        /// XDreamer曲线库
        /// </summary>
        [Name(Product.Name + "曲线库")]
        [Tip("点击打开[" + Product.Name + "曲线库]", "Click Open [" + Product.Name + " Curve Library]")]
        [XCSJ.Attributes.Icon(EIcon.Curve)]
        public bool XDreamerCurveLib;

        /// <summary>
        /// 百分比
        /// </summary>
        [Name("百分比")]
        [Tip("根据工作曲线通过输入百分比值(即横坐标=百分比,百分比范围值[0,1])计算得出纵坐标值;本值仅用于界面显示,不存储也不影响逻辑执行;", "According to the working curve, the ordinate value is calculated by entering the percentage value (i.e. abscissa = percentage, percentage range value [0,1]); This value is only used for interface display and does not store nor affect logic execution;")]
        public double percent = 0;

        /// <summary>
        /// 时间
        /// </summary>
        [Name("时间")]
        [Tip("根据工作曲线通过输入时间值(即横坐标=时间值/单次时长,时间值范围[0,单次时长])计算得出纵坐标值;本值仅用于界面显示,,不存储也不影响逻辑执行;", "According to the working curve, the ordinate value is calculated by inputting the time value (i.e. abscissa = time value / single time length, time value range [0, single time length]); This value is only used for interface display and does not store nor affect logic execution;")]
        public double time = 0;

        /// <summary>
        /// 标题宽度
        /// </summary>
        public const float titleWidth = 100;

        /// <summary>
        /// 锁定百分比与时间的比例关系
        /// </summary>
        [Name("锁定\n比例")]
        [XCSJ.Attributes.Icon(EIcon.Lock)]
        [Tip("锁定百分比与时间的比例关系,根据锁定时当前状态组件单次时长,对二者进行等比例同步调整;即其中一横坐标修改，另一横坐标数据将同步进行等比例的数据修改;", "The proportional relationship between locking percentage and time, and the two are adjusted synchronously in equal proportion according to the single time length of the current state component at the time of locking; That is, when one abscissa is modified, the other abscissa data will be modified synchronously in equal proportion;")]
        public bool lockPercentTimeRatio = true;

        /// <summary>
        /// 当绘制工作曲线时
        /// </summary>
        /// <param name="memberProperty"></param>
        /// <returns></returns>
        [LanguageTuple("Calculation Type", "计算类型")]
        [LanguageTuple("X-axis", "横坐标(x)")]
        [LanguageTuple("Percentage of time progress, range [0,1]", "时间进度百分比,范围[0,1]")]
        [LanguageTuple("Y-axis", "纵坐标(y)")]
        [LanguageTuple("Logical progress percentage, theoretical range [0,1]", "逻辑进度百分比,理论范围[0,1]")]
        [LanguageTuple("Operation", "操作")]
        protected virtual bool OnDrawWorkCurve(SerializedProperty memberProperty)
        {
            try
            {
                #region AnimationCurve绘制

                EditorGUILayout.BeginHorizontal();

                EditorGUI.BeginChangeCheck();
                var curve = EditorGUILayout.CurveField(TrLabelByTarget(nameof(WorkClip.workCurve)), memberProperty.animationCurveValue, workCurveColor, workCurveRange, GUILayout.Height(curveLibButtonSize.y));
                if (EditorGUI.EndChangeCheck())
                {
                    curve.preWrapMode = WrapMode.Clamp;
                    curve.postWrapMode = WrapMode.Clamp;
                    memberProperty.animationCurveValue = curve;
                }
                //上下镜像曲线
                if (GUILayout.Button(TrLabel(nameof(upDownMirrorCurve), ENameTip.EmptyTextWhenHasImage), GUILayout.Width(curveLibButtonSize.x), GUILayout.Height(curveLibButtonSize.y)))
                {
                    curve = curve.UpDownMirrorCurve(0.5f);
                    curve.preWrapMode = WrapMode.Clamp;
                    curve.postWrapMode = WrapMode.Clamp;
                    memberProperty.animationCurveValue = curve;
                }

                //左右镜像曲线
                if (GUILayout.Button(TrLabel(nameof(leftRightMirrorCurve), ENameTip.EmptyTextWhenHasImage), GUILayout.Width(curveLibButtonSize.x), GUILayout.Height(curveLibButtonSize.y)))
                {
                    curve = curve.LeftRightMirrorCurve(0.5f);
                    curve.preWrapMode = WrapMode.Clamp;
                    curve.postWrapMode = WrapMode.Clamp;
                    memberProperty.animationCurveValue = curve;
                }

                //曲线库选择
                EditorGUI.BeginChangeCheck();
                if (GUILayout.Button(TrLabel(nameof(XDreamerCurveLib), ENameTip.EmptyTextWhenHasImage), GUILayout.Width(curveLibButtonSize.x), GUILayout.Height(curveLibButtonSize.y)))
                {
                    CurvePresetLibraryHelper.ShowXDreamer();
                    CurveEditorWindow.ShowCurveEditorWindow(curve, memberProperty, workCurveRange, workCurveColor);
                }
                if (EditorGUI.EndChangeCheck())
                {
                    memberProperty.animationCurveValue.preWrapMode = WrapMode.Clamp;
                    memberProperty.animationCurveValue.postWrapMode = WrapMode.Clamp;
                    curve = memberProperty.animationCurveValue;
                }

                EditorGUILayout.EndHorizontal();

                #endregion

                #region 计算器

                #region 标题

                EditorGUILayout.BeginHorizontal(GUI.skin.box);
                EditorGUILayout.LabelField(Tr("Calculation Type"), GUILayout.Width(titleWidth));
                EditorGUILayout.LabelField(CommonFun.TempContent(Tr("X-axis"), Tr("Percentage of time progress, range [0,1]")));
                EditorGUILayout.LabelField(CommonFun.TempContent(Tr("Y-axis"), Tr("Logical progress percentage, theoretical range [0,1]")), GUILayout.Width(titleWidth));
                EditorGUILayout.LabelField(Tr("Operation"), GUILayout.Width(lockRatioButtonSize.x));
                EditorGUILayout.EndHorizontal();

                #endregion

                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.BeginVertical();

                var otl = workClip.onceTimeLength;

                #region percent

                UICommonFun.BeginHorizontal(true);
                EditorGUILayout.LabelField(TrLabel(nameof(percent)), GUILayout.Width(titleWidth));
                EditorGUI.BeginChangeCheck();
                percent = EditorGUILayout.Slider((float)percent, 0, 1);
                if(EditorGUI.EndChangeCheck() && lockPercentTimeRatio)
                {
                    time = otl * percent;
                }
                EditorGUILayout.SelectableLabel(curve.Evaluate((float)percent).ToString(), GUILayout.Width(titleWidth), GUILayout.Height(lockRatioButtonSize.y / 2));
                UICommonFun.EndHorizontal();

                #endregion

                #region time

                UICommonFun.BeginHorizontal(false);
                EditorGUILayout.LabelField(TrLabel(nameof(time)), GUILayout.Width(titleWidth));               
                EditorGUI.BeginChangeCheck();
                time = EditorGUILayout.Slider((float)time, 0, (float)otl);
                if (EditorGUI.EndChangeCheck() && lockPercentTimeRatio)
                {
                    percent = MathX.Scale(time, otl);
                }
                EditorGUILayout.SelectableLabel(curve.Evaluate((float)MathX.Scale(time, otl)).ToString(), GUILayout.Width(titleWidth), GUILayout.Height(lockRatioButtonSize.y / 2));
                UICommonFun.EndHorizontal();

                #endregion

                EditorGUILayout.EndVertical();

                lockPercentTimeRatio = UICommonFun.ButtonToggle(CommonFun.NameTip(this, nameof(lockPercentTimeRatio), ENameTip.EmptyTextWhenHasImage), lockPercentTimeRatio, GUI.skin.button, GUILayout.Width(lockRatioButtonSize.x + 2), GUILayout.Height(lockRatioButtonSize.y + 8));

                EditorGUILayout.EndHorizontal();

                #endregion
            }
            catch (ExitGUIException)
            {
                //忽略本异常
            }
            return false;
        }

        #endregion

        /// <summary>
        /// 当绘制成员时回调
        /// </summary>
        /// <param name="serializedProperty"></param>
        protected override void OnDrawMember(SerializedProperty serializedProperty, PropertyData propertyData)
        {
            switch (serializedProperty.name)
            {
                case nameof(WorkClip.workRange):
                    {
                        EditorGUILayout.BeginHorizontal();
                        if (!(workClipValidity = WorkClip.WorkClipValidity(workClip)))
                        {
                            guiColor = GUI.backgroundColor;
                            GUI.backgroundColor = XDreamerBaseOption.weakInstance.errorColor;
                        }

                        base.OnDrawMember(serializedProperty, propertyData);

                        if (!workClipValidity)
                        {
                            GUI.backgroundColor = guiColor;
                        }

                        //绘制同步时长按钮
                        DrawSyncTLButton(serializedProperty);

                        //绘制锁定比例按钮
                        DrawLockRatioButton(serializedProperty);

                        EditorGUILayout.EndHorizontal();
                        return;
                    }
                case nameof(WorkRange.percentRange):
                    {
                        EditorGUI.BeginChangeCheck();
                        base.OnDrawMember(serializedProperty, propertyData);
                        if (EditorGUI.EndChangeCheck() && workClip.lockRatioOfWorkRange && !MathX.ApproximatelyZero(workClip.ttlOfLockRatio))
                        {
                            var rangeSP = serializedProperty.FindPropertyRelative(nameof(PercentRange.percentRange));
                            var xSP = rangeSP.FindPropertyRelative(nameof(V2D.x));
                            var ySP = rangeSP.FindPropertyRelative(nameof(V2D.y));

                            //由百分比更新时间
                            var x = workClip.ttlOfLockRatio * xSP.doubleValue;
                            var y = workClip.ttlOfLockRatio * ySP.doubleValue;

                            var timeRangeSP = serializedProperty.serializedObject.FindProperty(nameof(WorkClip.workRange)).FindPropertyRelative(nameof(WorkRange.timeRange)).FindPropertyRelative(nameof(TimeRange.timeRange));
                            timeRangeSP.FindPropertyRelative(nameof(V2D.x)).doubleValue = x;
                            timeRangeSP.FindPropertyRelative(nameof(V2D.y)).doubleValue = y;
                        }
                        return;
                    }
                case nameof(WorkRange.timeRange):
                    {
                        //更新时间区间限定的最大时长
                        if (workClip.lockRatioOfWorkRange)
                        {
                            SetTimeRangeLimitMaxTimeLength(workClip.ttlOfLockRatio);
                        }
                        else if (HasSyncTLButton() && workClip.syncTL)
                        {
                            SetTimeRangeLimitMaxTimeLength(workClip.totalTimeLength);
                        }
                        else
                        {
                            SetTimeRangeLimitMaxTimeLength(TimeRange.DefaultMaxTimeLength);
                        }

                        EditorGUI.BeginChangeCheck();
                        base.OnDrawMember(serializedProperty, propertyData);
                        if (EditorGUI.EndChangeCheck())
                        {
                            var rangeSP = serializedProperty.FindPropertyRelative(nameof(TimeRange.timeRange));
                            var xSP = rangeSP.FindPropertyRelative(nameof(V2D.x));
                            var ySP = rangeSP.FindPropertyRelative(nameof(V2D.y));

                            if (workClip.lockRatioOfWorkRange && !MathX.ApproximatelyZero(workClip.ttlOfLockRatio))
                            {
                                //由时间更新百分比
                                var x = xSP.doubleValue / workClip.ttlOfLockRatio;
                                var y = ySP.doubleValue / workClip.ttlOfLockRatio;

                                var percentRangeSP = serializedProperty.serializedObject.FindProperty(nameof(WorkClip.workRange)).FindPropertyRelative(nameof(WorkRange.percentRange)).FindPropertyRelative(nameof(PercentRange.percentRange));
                                percentRangeSP.FindPropertyRelative(nameof(V2D.x)).doubleValue = MathX.Clamp01(x);
                                percentRangeSP.FindPropertyRelative(nameof(V2D.y)).doubleValue = MathX.Clamp01(y);
                            }

                            if (workClip.syncOTL)
                            {
                                //更新单次时长
                                serializedProperty.serializedObject.FindProperty(nameof(WorkClip._onceTimeLength)).doubleValue = ySP.doubleValue - xSP.doubleValue;
                            }
                        }
                        return;
                    }
                case nameof(WorkClip.percentOnEntry):
                case nameof(WorkClip.percentOnExit):
                case nameof(WorkClip.leastLoopCount):
                case nameof(WorkClip.percentOnAfterWorkRange):
                    {
                        var value = (float)serializedProperty.doubleValue;
                        EditorGUI.BeginChangeCheck();
                        value = EditorGUILayout.Slider(TrLabelByTarget(serializedProperty.name), value, 0, (float)workClip.loopCount);
                        if (EditorGUI.EndChangeCheck())
                        {
                            serializedProperty.doubleValue = value;
                        }
                        return;
                    }
                case nameof(WorkClip.workCurve):
                    {
                        OnDrawWorkCurve(serializedProperty);
                        return;
                    }
                case nameof(WorkClip._onceTimeLength):
                    {
                        EditorGUILayout.BeginHorizontal();
                        base.OnDrawMember(serializedProperty, propertyData);
                        DrawSyncOTLButton(serializedProperty);
                        EditorGUILayout.EndHorizontal();
                        return;
                    }
            }
            base.OnDrawMember(serializedProperty, propertyData);
        }

        /// <summary>
        /// 获取辅助信息
        /// </summary>
        /// <returns></returns>
        [LanguageTuple("\nLoop Information:", "\n循环信息:")]
        [LanguageTuple("\n\tOnce Time Length:\t{0}", "\n\t单次时长:\t{0}")]
        [LanguageTuple("\n\tOnce % Length:\t{0}", "\n\t单次%长:\t{0}")]
        [LanguageTuple("\n\tLoop Count:\t{0}", "\n\t循环次数:\t{0}")]
        public override StringBuilder GetHelpInfo()
        {
            var info = base.GetHelpInfo();
            var workClip = this.workClip;
            if (workClip.loop)
            {
                info.Append(Tr("\nLoop Information:"));
                info.AppendFormat(Tr("\n\tOnce Time Length:\t{0}"), workClip.onceTimeLength);
                info.AppendFormat(Tr("\n\tOnce % Length:\t{0}"), workClip.oncePercentLength);
                info.AppendFormat(Tr("\n\tLoop Count:\t{0}"), workClip.loopCount);
            }
            return info;
        }

        /// <summary>
        /// 获取运行时辅助信息
        /// </summary>
        /// <returns></returns>
        [LanguageTuple("\nLoop Runtime Information:", "\n循环运行时信息:")]
        [LanguageTuple("\n\tTotal Progress(Number of cycles):\t{0}", "\n\t总进度(已循环次数):\t{0}")]
        [LanguageTuple("\n\tProgress[0,1]:\t\t{0}", "\n\t进度[0,1]:\t\t{0}")]
        [LanguageTuple("\n\tWork Curve Progress:\t{0}", "\n\t工作曲线进度:\t{0}")]
        public override StringBuilder GetRuntimeHelpInfo()
        {
            var info = base.GetRuntimeHelpInfo();
            var workClip = this.workClip;
            if (workClip.loop)
            {
                info.Append(Tr("\nLoop Runtime Information:"));
                info.AppendFormat(Tr("\n\tTotal Progress(Number of cycles):\t{0}"), workClip.percent.percent);
                info.AppendFormat(Tr("\n\tProgress[0,1]:\t\t{0}"), workClip.percent.percent01);
                info.AppendFormat(Tr("\n\tWork Curve Progress:\t{0}"), workClip.percent.percentOfWorkCurve);
            }
            return info;
        }
    }
}
