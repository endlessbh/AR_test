using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXGUI.Base;

namespace XCSJ.PluginMMO.XGUI
{
    /// <summary>
    /// 将网络信息设置到Text
    /// </summary>
    [Name("网络信息文本")]
    [RequireManager(typeof(MMOManager))]
    public class NetInfoText : View
    {
        /// <summary>
        /// 网络信息类型
        /// </summary>
        [Name("网络信息类型")]
        [EnumPopup]
        public ENetInfoType _netInfoType = ENetInfoType.None;

        /// <summary>
        /// 文本
        /// </summary>
        [Name("文本")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Text _text = null;

        /// <summary>
        /// 信息更新规则
        /// </summary>
        [Name("信息更新规则")]
        [EnumPopup]
        public EUpdateRule _updateRule = EUpdateRule.Once;

        /// <summary>
        /// 更新时间 : 每隔一段时间更新信息
        /// </summary>
        [Name("更新时间")]
        [Tip("每隔一段时间更新信息", "Update information at regular intervals")]
        [Range(0.5f, 10)]
        [HideInSuperInspector(nameof(_updateRule), EValidityCheckType.NotEqual, EUpdateRule.Loop)]
        public float _updateTime = 1f;

        private float _timeCounter = 0;

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            if (!_text)
            {
                _text = GetComponent<Text>();
            }

            switch (_updateRule)
            {
                case EUpdateRule.Once: UpdateInfo(); break;
                case EUpdateRule.Loop: _timeCounter = _updateTime; break;
                default: break;
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        protected void Update()
        {
            switch (_updateRule)
            {
                case EUpdateRule.Once: break;
                case EUpdateRule.Loop:
                    {
                        _timeCounter += Time.deltaTime;
                        if (_timeCounter > _updateTime)
                        {
                            _timeCounter = 0;
                            UpdateInfo();
                        }
                        break;
                    }
                default: break;
            }
        }

        private void UpdateInfo()
        {
            if (_text)
            {
                _text.text = GetInfo();
            }
        }

        private string GetInfo()
        {
            var mmo = MMOManager.instance;
            if (!mmo) return "";

            switch (_netInfoType)
            {
                case ENetInfoType.None: return "";
                case ENetInfoType.AppName: return mmo.roomInfo.appName;
                case ENetInfoType.AppGuid: return mmo.roomInfo.appGuid;
                case ENetInfoType.AppVersion: return mmo.roomInfo.appVersion;
                case ENetInfoType.RoomGuid: return mmo.roomInfo.roomGuid;
                case ENetInfoType.UserGuid: return LocalCache.userGuid;
                case ENetInfoType.UserName:
                    {
                        if (LocalCache.players.TryGetValue(LocalCache.userGuid, out LocalCache.PlayerInfo playerInfo))
                        {
                            return playerInfo.name;
                        }
                        return "";
                    }
                case ENetInfoType.PlayerNickName:
                    {
                        if (LocalCache.players.TryGetValue(LocalCache.userGuid, out LocalCache.PlayerInfo playerInfo))
                        {
                            return playerInfo.nickName;
                        }
                        return "";
                    }
                case ENetInfoType.TokenGuid: return mmo.tokenGuid;
                case ENetInfoType.RoomName: return mmo.roomInfo.name;
                case ENetInfoType.NeedPwd: return mmo.roomInfo.pwd?"是":"否";
                case ENetInfoType.UserCount: return mmo.roomInfo.userCount.ToString();
                case ENetInfoType.MaxLimitUserCount: return mmo.roomInfo.limitCount.ToString();
                default: return "";
            }
        }

        /// <summary>
        /// 更新规则
        /// </summary>
        [Name("更新规则")]
        public enum EUpdateRule
        {
            [Name("仅一次")]
            Once,

            [Name("循环")]
            Loop,
        }

        /// <summary>
        /// 网络信息类型
        /// </summary>
        [Name("网络信息类型")]
        public enum ENetInfoType
        {
            [Name("无")]
            None,

            [Name("App名")]
            AppName,

            [Name("App编号")]
            AppGuid,

            [Name("用户编号")]
            UserGuid,

            [Name("用户名")]
            UserName,

            [Name("往家昵称")]
            PlayerNickName,

            [Name("会话ID")]
            TokenGuid,

            [Name("App版本")]
            AppVersion,

            [Name("房间编号")]
            RoomGuid,

            [Name("房间名")]
            RoomName,

            [Name("是否需要密码")]
            NeedPwd,

            [Name("在线人数")]
            UserCount,

            [Name("总人数")]
            MaxLimitUserCount,
        }
    }
}
