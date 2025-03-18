using System;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXGUI.Styles.Base;

namespace XCSJ.PluginXGUI.Styles.Elements
{

    /// <summary>
    /// 输入框样式
    /// </summary>
    [StyleLink(typeof(InputField), typeof(EParamsSetting))]
    [Name("输入框")]
    public class InputFieldStyleElement : SelectableStyleElement
    {
        /// <summary>
        /// 样式参数设置
        /// </summary>
        [Flags]
        [Name("样式参数设置")]
        private enum EParamsSetting
        {
            #region 基类参数

            [Name("过渡")]
            [EnumFieldName("过渡")]
            Transition = 1 << 0,

            [Name("颜色过渡")]
            [EnumFieldName("颜色过渡")]
            ColorBlock = 1 << 1,

            [Name("精灵状态")]
            [EnumFieldName("材质")]
            SpriteState = 1 << 2,

            [Name("动画触发器")]
            [EnumFieldName("动画触发器")]
            AnimationTriggers = 1 << 3,

            #endregion

            [Name("字符数量限定")]
            [EnumFieldName("字符数量限定")]
            CharacterLimit = 1 << 4,

            [Name("内容类型")]
            [EnumFieldName("内容类型")]
            ContentType = 1 << 5,

            [Name("行类型")]
            [EnumFieldName("行类型")]
            LineType = 1 << 6,

            [Name("光标配置")]
            [EnumFieldName("光标配置")]
            CaretConfig = 1 << 7,

            [Name("选中颜色")]
            [EnumFieldName("选中颜色")]
            SelectionColor = 1 << 8,

            [Name("移动端隐藏输入键盘")]
            [EnumFieldName("移动端隐藏输入键盘")]
            HideMobileInput = 1 << 9,

            [Name("只读")]
            [EnumFieldName("只读")]
            ReadOnly = 1 << 10,
        }

        /// <summary>
        /// 字符数量限定
        /// </summary>
        [Name("字符数量限定")]
        public int _characterLimit = 0;

        /// <summary>
        /// 内容类型
        /// </summary>
        [Name("内容类型")]
        public InputField.ContentType _contentType = InputField.ContentType.Standard;

        /// <summary>
        /// 行类型
        /// </summary>
        [Name("行类型")]
        public InputField.LineType _lineType = InputField.LineType.SingleLine;

        /// <summary>
        /// 光标闪烁频率
        /// </summary>
        [Name("光标闪烁频率")]
        [Range(0, 1)]
        public float _caretBlinkRate = 0.85f;

        /// <summary>
        /// 光标宽度
        /// </summary>
        [Name("光标宽度")]
        [Min(0)]
        public int _caretWidth = 1;

        /// <summary>
        /// 自定义光标颜色
        /// </summary>
        [Name("自定义光标颜色")]
        public bool _customCaretColor = false;

        /// <summary>
        /// 光标颜色
        /// </summary>
        [Name("光标颜色")]
        [HideInSuperInspector(nameof(_customCaretColor), EValidityCheckType.False)]
        public Color _caretColor = Color.black;

        /// <summary>
        /// 选中颜色
        /// </summary>
        [Name("选中颜色")]
        public Color _selectionColor = Color.black;

        /// <summary>
        /// 移动端隐藏输入键盘
        /// </summary>
        [Name("移动端隐藏输入键盘")]
        public bool _hideMobileInput = false;

        /// <summary>
        /// 只读
        /// </summary>
        [Name("只读")]
        public bool _readOnly = false;

        /// <summary>
        /// 样式应用对象
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="paramsMask"></param>
        /// <returns></returns>
        protected override bool OnStyleToObject(UnityEngine.Object obj, int paramsMask)
        {
            var rs1 = base.OnStyleToObject(obj, paramsMask);
            var rs2 = StyleHelper.ModifyObjectPropertyWithMask(obj as InputField, typeof(EParamsSetting), paramsMask, (inputField, paramsSetting) =>
            {
                switch (paramsSetting)
                {
                    case EParamsSetting.CharacterLimit: inputField.characterLimit = _characterLimit; break;
                    case EParamsSetting.ContentType: inputField.contentType = _contentType; break;
                    case EParamsSetting.LineType: inputField.lineType = _lineType; break;
                    case EParamsSetting.CaretConfig:
                        {
                            inputField.caretBlinkRate = _caretBlinkRate;
                            inputField.caretWidth = _caretWidth;
                            inputField.customCaretColor = _customCaretColor;
                            inputField.caretColor = _caretColor;
                            break;
                        }
                    case EParamsSetting.SelectionColor: inputField.selectionColor = _selectionColor; break;
                    case EParamsSetting.HideMobileInput: inputField.shouldHideMobileInput = _hideMobileInput; break;
                    case EParamsSetting.ReadOnly: inputField.readOnly = _readOnly; break;
                }
            });

            return rs1 || rs2;
        }

        /// <summary>
        /// 从对象中提取样式
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="paramsMask"></param>
        /// <returns></returns>
        protected override bool OnObjectToStyle(UnityEngine.Object obj, int paramsMask)
        {
            var rs1 = base.OnObjectToStyle(obj, paramsMask);
            var rs2 = StyleHelper.ModifyObjectPropertyWithMask(obj as InputField, typeof(EParamsSetting), paramsMask, (inputField, paramsSetting) =>
            {
                switch (paramsSetting)
                {
                    case EParamsSetting.CharacterLimit: _characterLimit = inputField.characterLimit; break;
                    case EParamsSetting.ContentType: _contentType = inputField.contentType; break;
                    case EParamsSetting.LineType: _lineType = inputField.lineType; break;
                    case EParamsSetting.CaretConfig:
                        {
                            _caretBlinkRate = inputField.caretBlinkRate;
                            _caretWidth = inputField.caretWidth;
                            _customCaretColor = inputField.customCaretColor;
                            _caretColor = inputField.caretColor;
                            break;
                        }
                    case EParamsSetting.SelectionColor: _selectionColor = inputField.selectionColor; break;
                    case EParamsSetting.HideMobileInput: _hideMobileInput = inputField.shouldHideMobileInput; break;
                    case EParamsSetting.ReadOnly: _readOnly = inputField.readOnly; break;
                }
            });
            return rs1 || rs2;
        }
    }
}
