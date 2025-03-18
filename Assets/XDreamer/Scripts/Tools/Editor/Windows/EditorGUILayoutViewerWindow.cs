using System;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension;
using XCSJ.EditorExtension.EditorWindows;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.Tools;

namespace XCSJ.EditorTools.Windows
{
    /// <summary>
    /// EditorGUILayout控件查看器
    /// </summary>
    [Name(Title)]
    [XCSJ.Attributes.Icon(EIcon.Material)]
    [XDreamerEditorWindow(nameof(TrHelper.Other), categoryIndex = int.MaxValue)]
    public class EditorGUILayoutViewerWindow : XEditorWindowWithScrollView<EditorGUILayoutViewerWindow>
    {
        public const string Title = "EditorGUILayout控件查看器";

        /// <summary>
        /// 静态主入口函数
        /// </summary>
        [MenuItem(XDreamerEditor.EditorWindowMenu + Title)]
        public static void Init() => OpenAndFocus();

        [Flags]
        [Name("测试类型")]
        public enum ETestType
        {
            E1 = 0,
            E2,
            E3,
        }

        public bool BeginToggleGroup = false;
        public Bounds BoundsField = new Bounds();
        public Color ColorField = new Color();
        public AnimationCurve CurveField = new AnimationCurve();
        public float DelayedFloatField = 0;
        public int DelayedIntField = 0;
        public string DelayedTextField = "";
        public double DoubleField = 0;
        public ETestType EnumMaskField = ETestType.E2;
        public ETestType EnumMaskPopup = ETestType.E2;
        public ETestType EnumPopup = ETestType.E2;
        public float FloatField = 0;
        public bool Foldout = true;
        public bool InspectorTitlebar = true;
        public int IntField = 0;
        public int IntPopup = 0;
        public int IntSlider = 0;
        public float Knob = 0;
        public int LayerField = 0;
        public long LongField = 0;
        public int MaskField = 0;
        public float MinMaxSlider_minValue = -1;
        public float MinMaxSlider_maxValue = 1;
        public UnityEngine.Object ObjectField = null;
        public string PasswordField = "";
        public Rect RectField = new Rect();
        public float Slider = 0;
        public string TagField = "";
        public string TextArea = "TextArea\nTextArea";
        public string TextField = "TextField\nTextField";
        public bool Toggle = true;
        public bool ToggleLeft = true;
        public Vector2 Vector2Field = new Vector2();
        public Vector3 Vector3Field = new Vector3();
        public Vector4 Vector4Field = new Vector4();

        public override void OnGUIWithScrollView()
        {
            try
            {
                EditorGUILayout.BeginVertical();
                {
                    BeginToggleGroup = EditorGUILayout.BeginToggleGroup("BeginToggleGroup", BeginToggleGroup);

                    BoundsField = EditorGUILayout.BoundsField("BoundsField", BoundsField);
                    ColorField = EditorGUILayout.ColorField("ColorField", ColorField);
                    CurveField = EditorGUILayout.CurveField("CurveField", CurveField);
                    DelayedFloatField = EditorGUILayout.DelayedFloatField("DelayedFloatField", DelayedFloatField);
                    DelayedIntField = EditorGUILayout.DelayedIntField("DelayedIntField", DelayedIntField);
                    DelayedTextField = EditorGUILayout.DelayedTextField("DelayedTextField", DelayedTextField);
                    DoubleField = EditorGUILayout.DoubleField("DoubleField", DoubleField);
                    EnumMaskField = (ETestType)EditorGUILayout.EnumFlagsField("EnumMaskField", EnumMaskField);
                    EnumMaskPopup = (ETestType)EditorGUILayout.EnumFlagsField(new GUIContent("EnumMaskPopup"), EnumMaskPopup);
                    EnumPopup = (ETestType)EditorGUILayout.EnumPopup("EnumPopup", EnumPopup);
                    FloatField = EditorGUILayout.FloatField("FloatField", FloatField);
                    Foldout = EditorGUILayout.Foldout(Foldout, "Foldout");
                    EditorGUILayout.GetControlRect(true, 20f);
                    EditorGUILayout.HelpBox("HelpBox", MessageType.Info);
                    //InspectorTitlebar = EditorGUILayout.InspectorTitlebar(InspectorTitlebar, (UnityEngine.Object)null, true);
                    //InspectorTitlebar = EditorGUILayout.InspectorTitlebar(InspectorTitlebar, (UnityEngine.Object)null, false);
                    IntField = EditorGUILayout.IntField("IntField", IntField);
                    IntPopup = EditorGUILayout.IntPopup("IntPopup", IntPopup, new string[] { "数字0", "数字1", "数字2" }, new int[] { 0, 1, 2 });
                    IntSlider = EditorGUILayout.IntSlider("IntSlider", IntSlider, -10, 10);
                    Knob = EditorGUILayout.Knob(new Vector2(40, 40), Knob, -10, 20, "Knob unit", Color.black, Color.red, true);
                    EditorGUILayout.LabelField("LabelField label", "LabelField label2");
                    LayerField = EditorGUILayout.LayerField("LayerField", LayerField);
                    LongField = EditorGUILayout.LongField("LongField", LongField);
                    MaskField = EditorGUILayout.MaskField("MaskField", MaskField, new string[] { "0", "1", "2" });
                    EditorGUILayout.MinMaxSlider(new GUIContent("MinMaxSlider"), ref MinMaxSlider_minValue, ref MinMaxSlider_maxValue, -10f, 10f);
                    ObjectField = EditorGUILayout.ObjectField(new GUIContent("ObjectField"), ObjectField, typeof(GameObject), true);
                    PasswordField = EditorGUILayout.PasswordField(new GUIContent("PasswordField"), PasswordField);
                    EditorGUILayout.PrefixLabel("PrefixLabel");
                    RectField = EditorGUILayout.RectField("RectField", RectField);
                    EditorGUILayout.SelectableLabel("SelectableLabel");
                    Slider = EditorGUILayout.Slider("Slider", Slider, -10, 10);
                    TagField = EditorGUILayout.TagField("TagField", TagField);
                    TextArea = EditorGUILayout.TextArea(TextArea);
                    TextField = EditorGUILayout.TextField("TextField", TextField);
                    Toggle = EditorGUILayout.Toggle("Toggle", Toggle);
                    ToggleLeft = EditorGUILayout.ToggleLeft("ToggleLeft", ToggleLeft);
                    Vector2Field = EditorGUILayout.Vector2Field("Vector2Field", Vector2Field);
                    Vector3Field = EditorGUILayout.Vector3Field("Vector3Field", Vector3Field);
                    Vector4Field = EditorGUILayout.Vector4Field("Vector4Field", Vector4Field);

                    EditorGUILayout.EndToggleGroup();
                }
            }
            catch (Exception ex)
            {
                Log.Exception(this.titleContent + " Exception: " + ex.ToString());
            }
            finally
            {
                EditorGUILayout.EndVertical();
                EditorGUILayout.Separator();
            }
        }
    }
}
