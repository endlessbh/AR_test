using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXGUI.Base;

namespace XCSJ.PluginMMO.XGUI.NetChats
{
    /// <summary>
    /// 聊天发送器
    /// </summary>
    [DisallowMultipleComponent]
    [Name("聊天发送器", "Net Sender")]
    [RequireManager(typeof(MMOManager))]
    public class ChatSender : View
    {
        [Name("网络聊天窗口")]
        public NetChatWindow _netChatWindow;

        [Name("文本输入框")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [Readonly(EEditorMode.Runtime)]
        public InputField _intputField;

        [Name("发送按钮")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [Readonly(EEditorMode.Runtime)]
        public Button _sendButton;

        [Name("发送完消息清除文本")]
        public bool clearTextWhenMsgSended = true;

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            if (_sendButton)
            {
                _sendButton.onClick.AddListener(OnClick);
            }

            if (_intputField)
            {
                _intputField.onValueChanged.AddListener(OnInputFieldValueChanged);
                _intputField.onEndEdit.AddListener(OnInputFieldEndEdit);
                OnInputFieldValueChanged(_intputField.text);
            }
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            if (_sendButton)
            {
                _sendButton.onClick.RemoveListener(OnClick);
            }

            if (_intputField)
            {
                _intputField.onValueChanged.RemoveListener(OnInputFieldValueChanged);
                _intputField.onEndEdit.RemoveListener(OnInputFieldEndEdit);
            }
        }

        private void OnInputFieldValueChanged(string value)
        {
            if (_sendButton)
            {
                _sendButton.interactable = !string.IsNullOrEmpty(value);
            }
        }

        private void OnInputFieldEndEdit(string value) => SendText(value);

        private void OnClick() => SendText(_intputField ? _intputField.text : "");

        private void SendText(string msg)
        {
            if (string.IsNullOrEmpty(msg)) return;

            if (_netChatWindow && _netChatWindow.netChat)
            {
                _netChatWindow.netChat.Send(msg);

                if (clearTextWhenMsgSended && _intputField)
                {
                    _intputField.text = "";
                }
            }
        }
    }
}
