using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginMMO.NetSyncs;
using XCSJ.PluginXGUI.Base;

namespace XCSJ.PluginMMO.XGUI.NetChats
{
    /// <summary>
    /// 聊天信息项
    /// </summary>
    [Name("聊天信息项")]
    [RequireManager(typeof(MMOManager))]
    public class ChatInfoItem : View
    {
        [Name("背景")]
        public Image _background;

        [Name("名字")]
        public Text _nameText;

        [Name("消息文本")]
        public Text _msgText;

        public Graphic[] graphics
        {
            get
            {
                return new Graphic[1] { _background };
            }
        }

        public string userID { get; private set; }

        public ChatInfo chatInfo 
        { 
            get => _chatInfo;
            set
            {
                _chatInfo = value;
                msg = _chatInfo.text;
                userName = chatInfo.userName;
                userID = _chatInfo.userGuid;
            }
        }
        private ChatInfo _chatInfo;

        /// <summary>
        /// 用户ID
        /// </summary>
        public string msg
        {
            get => _msgText ? _msgText.text : "";
            set
            {
                if (_msgText)
                {
                    _msgText.text = value;
                }
            }
        }

        /// <summary>
        /// 用户名
        /// </summary>
        public string userName
        {
            get => _nameText ? _nameText.text : "";
            set
            {
                if (_nameText)
                {
                    _nameText.text = value;
                }
            }
        }

        public void ResetData()
        {
            userName = "";
            msg = "";
        }
    }
}
