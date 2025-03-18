using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Interactions;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.Extension.Base.Tweens;
using XCSJ.PluginCommonUtils;
using System.Text;
using XCSJ.Helper;
using XCSJ.Interfaces;
using System;

namespace XCSJ.EditorExtension.Base.Interactions.Tools
{
    /// <summary>
    /// 工作剪辑交互器检查器
    /// </summary>
    [CustomEditor(typeof(WorkClipInteractor), true)]
    public class WorkClipInteractorInspector : InteractorInspector<WorkClipInteractor> { }

    /// <summary>
    /// 工作剪辑交互器检查器
    /// </summary>
    public class WorkClipInteractorInspector<T> : InteractorInspector<T> where T : WorkClipInteractor
    {
        protected override void OnDrawMember(SerializedProperty serializedProperty, PropertyData propertyData)
        {
            switch (serializedProperty.name)
            {
                case nameof(WorkClipInteractor._workCurve):
                    {
                        OnDrawWorkCurve(serializedProperty);
                        return;
                    }
            }
            base.OnDrawMember(serializedProperty, propertyData);
        }

        private Color workCurveColor { get; } = Color.green;

        private Rect workCurveRange { get; } = new Rect(0, 0, 1, 1);

        /// <summary>
        /// 曲线库按钮尺寸
        /// </summary>
        private Vector2 curveLibButtonSize = new Vector2(32, 32);

        /// <summary>
        /// XDreamer曲线库
        /// </summary>
        [Name(Product.Name + "曲线库")]
        [Tip("点击打开[" + Product.Name + "曲线库]", "Click Open [" + Product.Name + " Curve Library]")]
        [XCSJ.Attributes.Icon(EIcon.Curve)]
        public bool XDreamerCurveLib;

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
        /// 绘制工作曲线
        /// </summary>
        /// <param name="memberProperty"></param>
        /// <returns></returns>
        protected virtual bool OnDrawWorkCurve(SerializedProperty memberProperty)
        {
            try
            {
                #region AnimationCurve绘制

                EditorGUILayout.BeginHorizontal();

                EditorGUI.BeginChangeCheck();
                var curve = EditorGUILayout.CurveField(TrLabelByTarget(nameof(WorkClipInteractor._workCurve)), memberProperty.animationCurveValue, workCurveColor, workCurveRange, GUILayout.Height(curveLibButtonSize.y));
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
                //EditorGUI.BeginChangeCheck();
                //if (GUILayout.Button(TrLabel(nameof(XDreamerCurveLib), ENameTip.EmptyTextWhenHasImage), GUILayout.Width(curveLibButtonSize.x), GUILayout.Height(curveLibButtonSize.y)))
                //{
                //    CurvePresetLibraryHelper.ShowXDreamer();
                //    CurveEditorWindow.ShowCurveEditorWindow(curve, memberProperty, workCurveRange, workCurveColor);
                //}
                //if (EditorGUI.EndChangeCheck())
                //{
                //    memberProperty.animationCurveValue.preWrapMode = WrapMode.Clamp;
                //    memberProperty.animationCurveValue.postWrapMode = WrapMode.Clamp;
                //    curve = memberProperty.animationCurveValue;
                //}

                EditorGUILayout.EndHorizontal();

                #endregion
            }
            catch (ExitGUIException)
            {
                //忽略本异常
            }
            return false;
        }
    }

    /// <summary>
    /// 可播放内容检查器
    /// </summary>
    public class PlayableContentInspector<T> : WorkClipInteractorInspector<T> where T : PlayableContent 
    {
        /// <summary>
        /// 显示帮助信息
        /// </summary>
        protected override bool displayHelpInfo => true;

        /// <summary>
        /// 生成帮助信息
        /// </summary>
        /// <returns></returns>
        public override StringBuilder GetHelpInfo()
        {
            var sb = base.GetHelpInfo();

            sb.AppendFormat("播放状态:\t{0}", EnumHelper.GetEnumString(targetObject.playerState, EEnumStringType.NameAttributeCN));
            sb.AppendFormat("\n播放进度:\t{0}%", targetObject.percent*100);
            return sb;
        }
    }

    /// <summary>
    /// 可播放内容检查器
    /// </summary>

    [CustomEditor(typeof(PlayableContent), true)] 
    public class PlayableContentInspector : PlayableContentInspector<PlayableContent> 
    {

    }
}
