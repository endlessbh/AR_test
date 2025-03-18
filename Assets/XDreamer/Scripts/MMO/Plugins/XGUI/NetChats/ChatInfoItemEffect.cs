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
    public class ChatInfoItemEffect : GraphicItemEffect
    {
        public ChatInfoItemList _chatInfoItemList = null;

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            ChatInfoItemList.onAddChatInfoItem += OnAddChatInfoItem;
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            ChatInfoItemList.onAddChatInfoItem -= OnAddChatInfoItem;
        }

        protected void OnAddChatInfoItem(ChatInfoItemList chatInfoItemList, ChatInfoItem chatInfoItem)
        {
            if (chatInfoItemList== _chatInfoItemList)
            {
                SelectedColor(chatInfoItem.userID == LocalCache.userGuid, chatInfoItem.graphics);
            }
        }

    }
}
