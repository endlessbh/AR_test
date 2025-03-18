using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Characters;
using XCSJ.Extension.Characters.Tools;
using XCSJ.PluginCamera;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginTools.Renderers;
using XCSJ.Tools;
using XCSJ.PluginTools;
using XCSJ.EditorTools;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.Languages;
using XCSJ.PluginMMO.NetSyncs;

namespace XCSJ.EditorExtension.Characters.Tools
{
    /// <summary>
    /// 工具库角色菜单
    /// </summary>
    [LanguageFileOutput]
    public static class ToolsMenu
    {
        /// <summary>
        /// 空角色
        /// </summary>
        /// <param name="toolContext"></param>
        [Name("空角色")]
        [XCSJ.Attributes.Icon(EIcon.WalkCamera)]
        [Tool(CharacterHelper.CategoryName, index = 0, groupRule = EToolGroupRule.None)]
        [RequireManager(typeof(CameraManager))]
        [Manual(typeof(XCharacterController))]
        public static GameObject EmptyCharacter(ToolContext toolContext)
        {
            var characterController = CharacterHelper.CreateEmptyCharacter("空角色");
            EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, characterController.gameObject);
            return characterController.gameObject;
        }

        /// <summary>
        /// 胶囊小黄人角色
        /// </summary>
        /// <param name="toolContext"></param>
        [Name("胶囊小黄人角色")]
        [XCSJ.Attributes.Icon(EIcon.WalkCamera)]
        [Tool(CharacterHelper.CategoryName, index = 1, groupRule = EToolGroupRule.None)]
        [RequireManager(typeof(CameraManager))]
        [Manual(typeof(XCharacterController))]
        public static GameObject DummyCharacter(ToolContext toolContext)
        {
            //创建带相机控制器的角色控制器
            var characterController = CharacterHelper.CreateCharacterControllerWithCameraController("胶囊小黄人角色");

            //角色模型
            var characterModel = characterController.XCreateChild<Transform>("角色模型");

            //加载角色并设置层级关系
            var character = EditorCharacterHelper.LoadDummy();
            character.XSetParent(characterModel);
            character.transform.XResetLocalPRS();

            //查找角色虚拟眼睛游戏对象
            var eye = CommonFun.GetChildGameObject(character, "Cube");

            //纠正角色相机的目标对象
            characterController.characterCameraController.cameraMainController.cameraTargetController.mainTarget = eye ? eye.transform : character.transform;

            //根据角色调整胶囊碰撞体
            var capsuleCollider = characterController.GetComponent<CapsuleCollider>();
            capsuleCollider.XModifyProperty(() =>
            {
                capsuleCollider.center = new Vector3(0f, 1f, 0f);
                capsuleCollider.radius = 0.5f;
                capsuleCollider.height = 2.0f;
            });

            EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, characterController.gameObject);
            return characterController.gameObject;
        }

        /// <summary>
        /// Ethan角色
        /// </summary>
        /// <param name="toolContext"></param>
        [Name("Ethan角色")]
        [XCSJ.Attributes.Icon(EIcon.WalkCamera)]
        [Tool(CharacterHelper.CategoryName, index = 2, groupRule = EToolGroupRule.None)]
        [RequireManager(typeof(CameraManager))]
        [Manual(typeof(XCharacterController))]
        public static GameObject EthanCharacter(ToolContext toolContext)
        {
            //创建带相机控制器的角色控制器
            var characterController = CharacterHelper.CreateCharacterControllerWithCameraController("Ethan角色");

            //角色模型
            var characterModel = characterController.XCreateChild<Transform>("角色模型");

            //加载角色并设置层级关系
            var character = EditorCharacterHelper.LoadEthan();
            character.XSetParent(characterModel);
            character.transform.XResetLocalPRS();

            //添加角色虚拟眼睛游戏对象
            var eye = UnityObjectHelper.CreateGameObject("Eye");
            eye.XSetParent(characterModel);
            eye.transform.XSetLocalPosition(new Vector3(0.0002910048f, 1.406925f, -0.003441393f));
            var renderer = eye.XAddComponent<GizmoRenderer>();
            renderer.XModifyProperty(() => {
                renderer.sizeValue = new Vector3(0.15f, 0.08f, 0.12f);
                renderer.alwayShowGizmos = false;
            });

            //纠正角色相机的目标对象
            characterController.characterCameraController.cameraMainController.cameraTargetController.mainTarget = eye.transform;

            //添加角色动画控制器
            var characterAnimatorController = character.XAddComponent<CharacterAnimatorController>();
            character.XAddComponent<CharacterAnimatorForwardController>();

            //根据角色调整胶囊碰撞体
            var capsuleCollider = characterController.GetComponent<CapsuleCollider>();
            capsuleCollider.XModifyProperty(() =>
            {
                capsuleCollider.center = new Vector3(0f, 0.8f, 0f);
                capsuleCollider.radius = 0.33f;
                capsuleCollider.height = 1.6f;
            });

            EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, characterController.gameObject);
            return characterController.gameObject;
        }
    }
}
