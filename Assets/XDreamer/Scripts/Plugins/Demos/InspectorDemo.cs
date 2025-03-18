using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Attributes;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;

namespace XCSJ.Extension.Demos
{
    /// <summary>
    /// 检查器案例
    /// </summary>
    [Name("检查器案例")]
    [RequireManager(typeof(ExtensionExampleManager))]
    public class InspectorDemo : InteractProvider
    {
        #region XDreamer中继承自PropertyAttribute的特性

        [Group("XDreamer中继承自PropertyAttribute的特性", textEN = "Attributes Inherited From PropertyAttribute In XDreamer", tooltip = "针对具体修饰的字段，至多有一个特性可生效", tooltipEN = "For specific modified fields, at most one property can take effect", defaultIsExpanded = false)]
        [Name("变量名")]
        [Tip(nameof(GlobalVariableAttribute) + "绘制", nameof(GlobalVariableAttribute) + " draw")]
        [GlobalVariable]
        public string _variableName;

        [Name("变量字符串")]
        [Tip(nameof(VarStringAttribute) + "绘制", nameof(VarStringAttribute) + " draw")]
        [VarString]
        public string _varString;

        [Name("变量字符串（获取)")]
        [Tip(nameof(VarStringAttribute) + "绘制", nameof(VarStringAttribute) + " draw")]
        [VarString(EVarStringHierarchyKeyMode.Get)]
        public string _varStringGet;

        [Name("变量字符串（设置）")]
        [Tip(nameof(VarStringAttribute) + "绘制", nameof(VarStringAttribute) + " draw")]
        [VarString(EVarStringHierarchyKeyMode.Set)]
        public string _varStringSet;

        [Name("用户定义函数")]
        [Tip(nameof(UserDefineFunAttribute) + "绘制", nameof(UserDefineFunAttribute) + " draw")]
        [UserDefineFun]
        public string _userDefineFun;

        [Name("输入")]
        [Tip(nameof(InputAttribute) + "绘制", nameof(InputAttribute) + " draw")]
        [Input]
        public string _input;

        [Name("限定范围")]
        [Tip(nameof(LimitRangeAttribute) + "绘制[-2,2]", nameof(LimitRangeAttribute) + " draw [-2,2]")]
        [LimitRange(-2, 2)]
        public Vector2 _limitRange;

        [Name("限定范围整型")]
        [Tip(nameof(LimitRangeIntAttribute) + "绘制[-2,2]", nameof(LimitRangeIntAttribute) + " draw [-2,2]")]
        [LimitRangeInt(-2, 2)]
        public Vector2Int _limitRangeInt;

        [Name("密码文本")]
        [Tip(nameof(PasswordTextAttribute) + "绘制", nameof(PasswordTextAttribute) + " draw")]
        [PasswordText]
        public string _passwordText = nameof(PasswordTextAttribute);

        [Name("组件弹出")]
        [Tip(nameof(ComponentPopupAttribute) + "绘制", nameof(ComponentPopupAttribute) + " draw")]
        [ComponentPopup]
        public Component _componentPopup;

        [Name("相机弹出")]
        [Tip(nameof(ComponentPopupAttribute) + "绘制", nameof(ComponentPopupAttribute) + " draw")]
        [ComponentPopup(typeof(Camera))]
        public Component _cameraPopup;

        [Name("相机游戏对象弹出")]
        [Tip(nameof(GameObjectPopupAttribute) + "绘制", nameof(GameObjectPopupAttribute) + " draw")]
        [GameObjectPopup(typeof(Camera))]
        public GameObject _cameraGameObjectPopup;

        [Name("纹理预览")]
        [Tip(nameof(TexturePreviewAttribute) + "绘制", nameof(TexturePreviewAttribute) + " draw")]
        [TexturePreview]
        public Texture2D _texturePreview;

        [Name("枚举弹出")]
        [Tip(nameof(EnumPopupAttribute) + "绘制", nameof(EnumPopupAttribute) + " draw")]
        [EnumPopup]
        [EndGroup(true)]
        public ETestEnum _enumPopup = ETestEnum.Zero;

        /// <summary>
        /// 测试枚举
        /// </summary>
        [Name("测试枚举")]
        public enum ETestEnum
        {
            [Name("零")]
            [EnumFieldName("零")]
            Zero,

            [Name("一")]
            [EnumFieldName("一")]
            One,

            [Name("二")]
            [EnumFieldName("二")]
            Tow,

            [Name("三")]
            [EnumFieldName("三")]
            Three,
        }

        #endregion

        #region XDreamer中继承自PropertyAttribute的特性-数组(列表)专用

        [Group("XDreamer中继承自PropertyAttribute的特性-数组(列表)专用", textEN= "Attributes Inherited From PropertyAttribute In XDreamer - Array (List) Specific", tooltip = "针对具体修饰的数组(列表)的字段，针对每个元素做处理的特性；可以与其他多个XDreamer专有特性叠加使用并生效;",tooltipEN = "For the fields of the array (list) modified specifically, the characteristics of processing for each element; It can be superimposed with other XDreamer proprietary features and take effect;", defaultIsExpanded = false)]
        [Name("数组")]
        [Tip(nameof(ArrayElementAttribute) + "绘制", nameof(ArrayElementAttribute) + " draw")]
        [ArrayElement]
        public Rect[] _array = new Rect[5];

        [Name("数组(可插入)")]
        [Tip(nameof(ArrayElementAttribute) + "绘制", nameof(ArrayElementAttribute) + " draw")]
        [ArrayElement(EArrayElementHandleRule.CanInsert | EArrayElementHandleRule.DisplaySize)]
        public Color[] _arrayCanInsert = new Color[5];

        [Name("数组(可删除)")]
        [Tip(nameof(ArrayElementAttribute) + "绘制", nameof(ArrayElementAttribute) + " draw")]
        [ArrayElement(EArrayElementHandleRule.CanDelete | EArrayElementHandleRule.DisplaySize)]
        public Bounds[] _arrayCanDelete = new Bounds[5];

        [Name("数组(不可调整大小)")]
        [Tip(nameof(ArrayElementAttribute) + "绘制", nameof(ArrayElementAttribute) + " draw")]
        [ArrayElement(EArrayElementHandleRule.DisplaySize)]
        public int[] _arrayNoResize = new int[5];

        [Name("数组(无特性)")]
        [Tip("无" + nameof(ArrayElementAttribute) + "绘制", "Drawing without " + nameof(ArrayElementAttribute))]
        public Color[] _arrayWitioutAttribute = new Color[5];

        [Name("案例节点数组")]
        [Tip(nameof(ArrayElementAttribute) + "绘制", nameof(ArrayElementAttribute) + " draw")]
        [ArrayElement]
        public DemoNode[] _demoNodeArray = new DemoNode[2];

        [Name("案例节点数组(仅成员元素)")]
        [Tip(nameof(ArrayElementAttribute) + "绘制", nameof(ArrayElementAttribute) + " draw")]
        [OnlyMemberElements]
        [ArrayElement]
        public DemoNode[] _demoNodeArrayOnlyMemberElements = new DemoNode[2];

        [Name("案例节点数组(无特性)")]
        [Tip("无" + nameof(ArrayElementAttribute) + "绘制", "Drawing without " + nameof(ArrayElementAttribute))]
        public DemoNode[] _demoNodeArraWitioutAttribute = new DemoNode[2];

        #endregion

        #region XDreamer专有特性

        [Group("XDreamer专有特性", textEN = "XDreamer Proprietary Attributes", tooltip = "针对具体修饰的字段，允许多个特性叠加使用并生效", tooltipEN = "For specific modified fields, multiple features are allowed to be superimposed and take effect", defaultIsExpanded = false)]
        [Name("整形数组(仅成员元素)")]
        [Tip(nameof(OnlyMemberElementsAttribute) + "绘制", nameof(OnlyMemberElementsAttribute) + " draw")]
        [OnlyMemberElements]
        public int[] _intArrayOnlyMemberElements;

        [Name("只读")]
        [Tip(nameof(ReadonlyAttribute) + "绘制", nameof(ReadonlyAttribute) + " draw")]
        [Readonly]
        public string _readonly = nameof(ReadonlyAttribute);

        [Name("在检查器中隐藏")]
        [Tip(nameof(HideInSuperInspectorAttribute) + "绘制", nameof(HideInSuperInspectorAttribute) + " draw")]
        [HideInSuperInspector]
        public string _hideInSuperInspector = nameof(HideInSuperInspectorAttribute);

        [Name("真时隐藏")]
        [Tip("当隐藏为真时隐藏", "Hide when hide is true")]
        public bool _hideWhenTrue = false;

        [Name("如果真时隐藏")]
        [Tip(nameof(HideInSuperInspectorAttribute) + "绘制", nameof(HideInSuperInspectorAttribute) + " draw")]
        [HideInSuperInspector(nameof(_hideWhenTrue), EValidityCheckType.True)]
        public string _hideIfHideTrue = nameof(HideInSuperInspectorAttribute);

        [Name("小于0时隐藏")]
        [Tip("当小于0隐藏", "Hide when less than 0")]
        public int _hideWhenLess_0 = 1;

        [Name("如果小于0隐藏")]
        [Tip(nameof(HideInSuperInspectorAttribute) + "绘制", nameof(HideInSuperInspectorAttribute) + " draw")]
        [HideInSuperInspector(nameof(_hideWhenLess_0), EValidityCheckType.Less, 0)]
        public int _hideIfLess_0;

        [Name("如果小于0且隐藏为真时隐藏")]
        [Tip("当小于0且隐藏为真时隐藏", "Hide when less than 0 and hide is true")]
        [HideInSuperInspector(nameof(_hideWhenLess_0), EValidityCheckType.Less | EValidityCheckType.And, 0, nameof(_hideWhenTrue), EValidityCheckType.True)]
        public int _hideIfLess_0_AndHideTrue;

        [Name("如果小于0或隐藏为真时隐藏")]
        [Tip("当小于0或隐藏为真时隐藏", "Hide when less than 0 or hide is true")]
        [HideInSuperInspector(nameof(_hideWhenLess_0), EValidityCheckType.Less | EValidityCheckType.Or, 0, nameof(_hideWhenTrue), EValidityCheckType.True)]
        public int _hideIfLess_0_OrHideTrue;

        [Name("字符串不空")]
        [Tip(nameof(ValidityCheckAttribute) + "绘制", nameof(ValidityCheckAttribute) + " draw")]
        [ValidityCheck(EValidityCheckType.NotNullOrEmpty)]
        public string _stringNotEmpty;

        [Name("游戏对象不空")]
        [Tip(nameof(ValidityCheckAttribute) + "绘制", nameof(ValidityCheckAttribute) + " draw")]
        [ValidityCheck(EValidityCheckType.NotNullOrEmpty)]
        public GameObject _gameObjectNotNull;

        [Name("值不为0")]
        [Tip(nameof(ValidityCheckAttribute) + "绘制", nameof(ValidityCheckAttribute) + " draw")]
        [ValidityCheck(EValidityCheckType.NotZero, invalidExplanation = "当前值不可为0", invalidExplanationEN = "The current value cannot be 0")]
        public int _valueNotZero;

        [Name("值大于等于1且小于5")]
        [Tip("要求值大于等于1且小于5", "The required value is greater than or equal to 1 and less than 5")]
        [ValidityCheck(EValidityCheckType.GreaterEqual | EValidityCheckType.And, 1, nameof(_valueGreaterEqual1AndLess5), EValidityCheckType.Less, 5, invalidExplanation = "当前值范围 [1,5)", invalidExplanationEN = "Current value range [1,5)")]
        public int _valueGreaterEqual1AndLess5;

        #endregion
    }

    /// <summary>
    /// 案例节点
    /// </summary>
    [Serializable]
    [Name("案例节点")]
    [LanguageFileOutput]
    public class DemoNode
    {
        /// <summary>
        /// 值
        /// </summary>
        [Name("值")]
        public int _value;

        /// <summary>
        /// 文本
        /// </summary>
        [Name("文本")]
        public string _text;

        /// <summary>
        /// 案例子节点
        /// </summary>
        [Name("案例子节点")]
        public DemoSubNode _demoSubNode = new DemoSubNode();
    }

    /// <summary>
    /// 案例子节点
    /// </summary>
    [Name("案例子节点")]
    [Serializable]
    [LanguageFileOutput]
    public class DemoSubNode
    {
        /// <summary>
        /// 值
        /// </summary>
        [Name("值")]
        public int _value;

        /// <summary>
        /// 文本
        /// </summary>
        [Name("文本")]
        public string _text;
    }
}
