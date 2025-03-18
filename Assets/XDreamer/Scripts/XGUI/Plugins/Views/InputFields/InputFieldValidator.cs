using System;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXGUI.Base;

namespace XCSJ.PluginXGUI.Views.InputFields
{
    /// <summary>
    /// 输入框校验器
    /// </summary>
    [DisallowMultipleComponent]
    [Name("输入框校验器")]
    public class InputFieldValidator : View
    {
        /// <summary>
        /// 输入框
        /// </summary>
        [Name("输入框")]
        [ComponentPopup]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public InputField _inputField = null;

        public string inputFieldValue
        {
            get
            {
                return _inputField ? _inputField.text : "";
            }
            set
            {
                if (_inputField)
                {
                    _inputField.text = value;
                }
            }
        }

        /// <summary>
        /// 提示：输入非法时，提示信息输出到当前对象上
        /// </summary>
        [Name("提示")]
        [Tip("输入非法时，提示信息输出到当前对象上", "If the input is illegal, the prompt information will be output to the current object")]
        [ComponentPopup]
        public Text _tip = null;

        /// <summary>
        /// 检查模式
        /// </summary>
        [Name("检查模式")]
        [HideInSuperInspector(nameof(_inputField), EValidityCheckType.NullOrEmpty)]
        [EnumPopup]
        public ECheckMode _checkMode = ECheckMode.ValueChanged;

        /// <summary>
        /// 检查模式
        /// </summary>
        [Name("检查类型")]
        [HideInSuperInspector(nameof(_inputField), EValidityCheckType.NullOrEmpty)]
        [EnumPopup]
        public ECheckType _checkType = ECheckType.None;

        /// <summary>
        /// 浮点数范围
        /// </summary>
        [Name("数字范围")]
        [HideInSuperInspector(nameof(_checkType), EValidityCheckType.NotEqual, ECheckType.IntegerNumber, nameof(_checkType), EValidityCheckType.NotEqual, ECheckType.DecimalNumber)]
        public Vector2 _numberRange = new Vector2(0, 1000);

        /// <summary>
        /// 值变更事件 : 参数1表示检查是否有效，第二个参数是值
        /// </summary>
        public event Action<bool, string> onValueChanged = null;

        private void OnValidate()
        {
            if (_numberRange.x > _numberRange.y) _numberRange.x = _numberRange.y;
        }

        /// <summary>
        /// 唤醒
        /// </summary>
        protected void Awake()
        {
            if (!_inputField)
            {
                _inputField = GetComponent<InputField>();
            }
            if (!_tip)
            {
                _tip = GetComponent<Text>();
            }
        }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            if(_inputField) 
            {
                _inputField.onValueChanged.AddListener(OnValueChanged); 
                _inputField.onEndEdit.AddListener(OnEndEdit);
            }
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            if (_inputField)
            {
            	_inputField.onValueChanged.RemoveListener(OnValueChanged);
                _inputField.onEndEdit.RemoveListener(OnEndEdit);
            }
        }

        /// <summary>
        /// 值变更事件回调
        /// </summary>
        /// <param name="value"></param>
        protected void OnValueChanged(string value)
        {
            if (_checkMode == ECheckMode.ValueChanged)
            {
                CheckAndTip(value);
            }
        }

        /// <summary>
        /// 值提交事件回调 ：键盘回车或者输入框失去焦点时触发提交事件
        /// </summary>
        /// <param name="value"></param>
        protected void OnEndEdit(string value)
        {
            if (_checkMode == ECheckMode.EndEdit)
            {
                CheckAndTip(value);
            }
        }

        public void CheckAndTip()
        {
            if (_inputField)
            {
                CheckAndTip(_inputField.text);
            }
        }

        /// <summary>
        /// 检查输入数据值
        /// 错误：设置错误提示信息
        /// 正确：设置为缺省正确提示
        /// </summary>
        /// <param name="value"></param>
        protected void CheckAndTip(string value)
        {
            var rs = Check(value, out string msg);

            if (_tip)
            {
                _tip.text = msg;
            }
        }

        /// <summary>
        /// 检查并纠正输入数值
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="msg">提示信息</param>
        /// <returns></returns>
        protected bool Check(string value, out string msg)
        {
            bool rs = true;
            msg = "";
            switch (_checkType)
            {
                case ECheckType.None: break;
                case ECheckType.IntegerNumber:
                    {
                        if (rs = int.TryParse(value, out int i))
                        {
                            value = Mathf.Clamp(i, (int)_numberRange.x, (int)_numberRange.y).ToString();
                        }
                        else
                        {
                            msg = value + ":不是有效的整数";
                        }
                        break;
                    }
                case ECheckType.DecimalNumber:
                    {
                        if (rs = float.TryParse(value, out float f))
                        {
                            if (f < _numberRange.x || f > _numberRange.y)
                            {
                                value = Mathf.Clamp(f, _numberRange.x, _numberRange.y).ToString();
                            }
                        }
                        else
                        {
                            msg = value + ":不是有效的浮点数";
                        }
                        break;
                    }
                case ECheckType.NotNull:
                    {
                        if (rs = string.IsNullOrEmpty(value))
                        {
                            msg = "输入不能为空";
                        }
                        rs = !rs;
                        break;
                    }
                default:
                    break;
            }
            onValueChanged?.Invoke(rs, value);
            return rs;
        }

        /// <summary>
        /// 检查模式
        /// </summary>
        [Name("检查模式")]
        public enum ECheckMode
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None,

            /// <summary>
            /// 值变化时
            /// </summary>
            [Name("值变化时")]
            ValueChanged,

            /// <summary>
            /// 提交时
            /// </summary>
            [Name("结束编辑时")]
            EndEdit,
        }

        /// <summary>
        /// 检测类型
        /// </summary>
        [Name("检测类型")]
        public enum ECheckType
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None = 1 << 0,

            /// <summary>
            /// 整数
            /// </summary>
            [Name("整数")]
            IntegerNumber = 1 << 1,

            /// <summary>
            /// 浮点数
            /// </summary>
            [Name("浮点数")]
            DecimalNumber = 1 << 2,

            /// <summary>
            /// 非空
            /// </summary>
            [Name("非空")]
            NotNull = 1 << 3,
        }
    }
}
