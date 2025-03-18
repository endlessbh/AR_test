using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXGUI.Base;
using static XCSJ.PluginMMO.LocalCache;

namespace XCSJ.PluginMMO.XGUI.NetChats
{
    /// <summary>
    /// 用户信息项
    /// </summary>
    [Name("用户信息项")]
    [RequireManager(typeof(MMOManager))]
    public class UserInfoItem : View
    {
        [Name("用户名称")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Text _nameText;

        [Name("用户ID")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Text _IDText;

        public Graphic[] graphics
        {
            get
            {
                return new Graphic[2] { _nameText, _IDText };
            }
        }

        /// <summary>
        /// 玩家信息
        /// </summary>
        public PlayerInfo playerInfo
        {
            get => _playerInfo;
            set
            {
                _playerInfo = value;
                userID = _playerInfo.guid;
                userName = _playerInfo.nickName ?? _playerInfo.name;
            }
        }
        private PlayerInfo _playerInfo;

        /// <summary>
        /// 用户ID
        /// </summary>
        public string userID
        {
            get => _IDText ? _IDText.text : "";
            set
            {
                if (_IDText)
                {
                    _IDText.text = value;
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
            userID = "";
        }

        public void SetNameTextColor(Color color)
        {
            if (_nameText)
            {
                _nameText.color = color;
            }
        }

        public void SetIDTextColor(Color color)
        {
            if (_IDText)
            {
                _IDText.color = color;
            }
        }
    }
}
