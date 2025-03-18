using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXGUI.Views.Graphics;

namespace XCSJ.PluginMMO.XGUI.NetChats
{
    /// <summary>
    /// 用户信息项效果
    /// </summary>
    [Name("用户信息项效果")]
    [RequireManager(typeof(MMOManager))]
    public class UserInfoItemEffect : GraphicItemEffect
    {
        [Name("用户信息列表")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public UserInfoList _userInfoList = null;

        protected override void OnEnable()
        {
            base.OnEnable();
            UserInfoList.onAddUserInfoItem += OnAddUserInfoItem;
            UserInfoList.onUpdateUserNameItem += OnUpdateUserNameItem;
            UserInfoList.onRemoveUserInfoItem += OnRemoveUserInfoItem;
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            UserInfoList.onAddUserInfoItem -= OnAddUserInfoItem;
            UserInfoList.onUpdateUserNameItem -= OnUpdateUserNameItem;
            UserInfoList.onRemoveUserInfoItem -= OnRemoveUserInfoItem;
        }

        protected void OnAddUserInfoItem(UserInfoList userInfoList, UserInfoItem userInfoItem)
        {
            OnUpdateUserNameItem(userInfoList, userInfoItem);
        }

        protected void OnUpdateUserNameItem(UserInfoList userInfoList, UserInfoItem userInfoItem)
        {
            if (_userInfoList == userInfoList)
            {
                SelectedColor(userInfoItem.userID == LocalCache.userGuid, userInfoItem.graphics);
            }
        }

        protected void OnRemoveUserInfoItem(UserInfoList userInfoList, UserInfoItem userInfoItem)
        {
            if (_userInfoList == userInfoList)
            {
                SelectedColor(false, userInfoItem.graphics);
            }
        }
    }
}
