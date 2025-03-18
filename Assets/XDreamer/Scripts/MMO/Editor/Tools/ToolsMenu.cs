using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorTools;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginMMO;
using XCSJ.PluginMMO.NetSyncs;
using XCSJ.PluginTimelines.Tools;

namespace XCSJ.EditorMMO.Tools
{
    /// <summary>
    /// 工具库MMO菜单
    /// </summary>
    [LanguageFileOutput]
    public static class ToolsMenu
    {
        /// <summary>
        /// 创建网络玩家
        /// </summary>
        /// <param name="gameObject"></param>
        private static void CreateNetPlayer(GameObject gameObject)
        {
            //网络标识组件
            gameObject.XAddComponent<NetIdentity>();

            //网络玩家组件
            var netPlayer = gameObject.XAddComponent<NetPlayer>();
            netPlayer.XModifyProperty(() =>
            {
                netPlayer.intervalTime = 0.05f;
            });

            //网络玩家HUD组件
            var netPlayerHUD = gameObject.XAddComponent<NetPlayerHUD>();
            netPlayerHUD.XModifyProperty(() => {
                netPlayerHUD.offset = new Vector3(0, 2.5f, 0);
                netPlayerHUD.chatInfoOffset = new Vector3(0, 3, 0);
            });

            //网络变换组件
            var netTransform = gameObject.XAddComponent<NetTransform>();
            netTransform.XModifyProperty(() =>
            {
                netTransform.syncMode = NetTransform.ESyncMode.Rigidbody;
            });
        }

        /// <summary>
        /// 空玩家:创建不带任何角色信息的网络玩家游戏对象
        /// </summary>
        /// <param name="toolContext"></param>
        [Name("空玩家")]
        [Tip("创建不带任何角色信息的网络玩家游戏对象", "Create an online player game object without any character information")]
        [XCSJ.Attributes.Icon(EIcon.WalkCamera)]
        [Tool(MMOHelperExtension.MMOCharacterCategoryName, rootType = typeof(MMOManager), index = -1, groupRule = EToolGroupRule.Create)]
        [RequireManager(typeof(MMOManager))]
        [Manual(typeof(NetPlayer))]
        public static void EmptyPlayer(ToolContext toolContext)
        {
            var player = UnityObjectHelper.CreateGameObject("空玩家");

            //创建网络玩家
            CreateNetPlayer(player);

            EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, player);
        }

        /// <summary>
        /// 空角色:创建带角色信息但不带角色模型与角色动画器的网络玩家游戏对象
        /// </summary>
        /// <param name="toolContext"></param>
        [Name("空角色")]
        [Tip("创建带角色信息但不带角色模型与角色动画器的网络玩家游戏对象", "Create an online player game object with character information but without character model and character animator")]
        [XCSJ.Attributes.Icon(EIcon.WalkCamera)]
        [Tool(MMOHelperExtension.MMOCharacterCategoryName, rootType = typeof(MMOManager), index = 0, groupRule = EToolGroupRule.Create)]
        [RequireManager(typeof(MMOManager))]
        [Manual(typeof(NetPlayer))]
        public static void EmptyCharacter(ToolContext toolContext)
        {
            var character = EditorExtension.Characters.Tools.ToolsMenu.EmptyCharacter(toolContext);

            //创建网络玩家
            CreateNetPlayer(character);
        }

        /// <summary>
        /// 胶囊小黄人角色:创建带角色信息且带胶囊小黄人角色模型与角色动画器的网络玩家游戏对象
        /// </summary>
        /// <param name="toolContext"></param>
        [Name("胶囊小黄人角色")]
        [Tip("创建带角色信息且带胶囊小黄人角色模型与角色动画器的网络玩家游戏对象", "Create an online player game object with character information and capsule small yellow man character model and character animator")]
        [XCSJ.Attributes.Icon(EIcon.WalkCamera)]
        [Tool(MMOHelperExtension.MMOCharacterCategoryName, rootType = typeof(MMOManager), index = 1, groupRule = EToolGroupRule.Create)]
        [RequireManager(typeof(MMOManager))]
        [Manual(typeof(NetPlayer))]
        public static void DummyCharacter(ToolContext toolContext)
        {
            var character = EditorExtension.Characters.Tools.ToolsMenu.DummyCharacter(toolContext);

            //创建网络玩家
            CreateNetPlayer(character);
        }

        /// <summary>
        /// Ethan角色:创建带角色信息且带Ethan角色模型与角色动画器的网络玩家游戏对象
        /// </summary>
        /// <param name="toolContext"></param>
        [Name("Ethan角色")]
        [Tip("创建带角色信息且带Ethan角色模型与角色动画器的网络玩家游戏对象", "Create an online player game object with character information and Ethan character model and character animator")]
        [XCSJ.Attributes.Icon(EIcon.WalkCamera)]
        [Tool(MMOHelperExtension.MMOCharacterCategoryName, rootType = typeof(MMOManager), index = 2, groupRule = EToolGroupRule.Create)]
        [RequireManager(typeof(MMOManager))]
        [Manual(typeof(NetPlayer))]
        public static void EhtanCharacter(ToolContext toolContext)
        {
            var character = EditorExtension.Characters.Tools.ToolsMenu.EthanCharacter(toolContext);

            //创建网络玩家
            CreateNetPlayer(character);
        }
    }
}
