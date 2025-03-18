using System;
using UnityEngine;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Extension.Characters.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginsCameras;
using XCSJ.PluginsCameras.Controllers;
using XCSJ.PluginsCameras.Tools.Base;
using XCSJ.PluginsCameras.Tools.Controllers;
using XCSJ.PluginTools.Renderers;
using XCSJ.Scripts;

namespace XCSJ.Extension.Characters
{
    /// <summary>
    /// 角色助手类
    /// </summary>
    public static class CharacterHelper
    {
        /// <summary>
        /// 工具库中角色目录名称
        /// </summary>
        public const string CategoryName = "角色";

        /// <summary>
        /// 工具库中角色控制目录名称
        /// </summary>
        public const string ControllersCategoryName = "角色组件";

        /// <summary>
        /// 工具库中角色控制目录名称
        /// </summary>
        public const string ControllersCategoryName_Move = "角色组件-移动";

        /// <summary>
        /// 工具库中角色控制目录名称
        /// </summary>
        public const string ControllersCategoryName_Rotate = "角色组件-旋转";

        /// <summary>
        /// 物理材质名：默认为无摩擦的物理材质
        /// </summary>
        public const string PhysicMaterialName = "Frictionless";

        /// <summary>
        /// 获取物理材质委托
        /// </summary>
        public static Func<PhysicMaterial> getPhysicMaterial;

        /// <summary>
        /// 获取物理材质
        /// </summary>
        /// <returns></returns>
        public static PhysicMaterial GetPhysicMaterial() => getPhysicMaterial?.Invoke();

        /// <summary>
        /// 创建物理材质
        /// </summary>
        /// <returns></returns>
        public static PhysicMaterial CreatePhysicMaterial()
        {
            return new PhysicMaterial(PhysicMaterialName)
            {
                dynamicFriction = 0.0f,
                staticFriction = 0.0f,
                bounciness = 0.0f,
                frictionCombine = PhysicMaterialCombine.Multiply,
                bounceCombine = PhysicMaterialCombine.Multiply
            };
        }

        /// <summary>
        /// 创建角色控制器
        /// </summary>
        /// <param name="name"></param>
        /// <param name="multiGameObjectMode"></param>
        /// <returns></returns>
        public static XCharacterController CreateCharacterController(string name, bool multiGameObjectMode = true)
        {
            //创建游戏对象
            var cameraControllerGO = UnityObjectHelper.CreateGameObject(name);

            //添加角色控制器
            var characterController = cameraControllerGO.XAddComponent<XCharacterController>();

            //初始化刚体
            var rb = cameraControllerGO.GetComponent<Rigidbody>();
            rb.XModifyProperty(() =>
            {
                rb.useGravity = false;
                rb.isKinematic = false;
                rb.interpolation = RigidbodyInterpolation.Interpolate;
                rb.freezeRotation = true;

            });

            //初始化胶囊碰撞体，并尝试加载已提供的无摩擦物理材质
            var capsuleCollider = cameraControllerGO.GetComponent<CapsuleCollider>();
            capsuleCollider.XModifyProperty(() =>
            {
                capsuleCollider.center = new Vector3(0f, 1f, 0f);
                capsuleCollider.radius = 0.5f;
                capsuleCollider.height = 2.0f;
                capsuleCollider.material = GetPhysicMaterial();
            });
            var physicMaterial = capsuleCollider.sharedMaterial;
            if (physicMaterial == null)
            {
                //未找到有效的物理材质，创建一个新的，并弹出提示信息；
                physicMaterial = CreatePhysicMaterial();
                capsuleCollider.material = physicMaterial;

                Debug.LogWarningFormat("角色运动CharacterMovement: 未找到适合胶囊碰撞体[(0)]的有效物理材质, 已创建一个新的无摩擦物理材质并赋值给该碰撞体.\n你应该添添加一个无摩擦物理材质到胶囊碰撞体[(0)]", CommonFun.GameObjectToString(cameraControllerGO));
            }

            if (multiGameObjectMode)
            {
                //添加角色相机控制器
                characterController.XCreateChild<CharacterCameraController>();

                //添加角色变换器
                var characterTransformer = characterController.characterTransformer;
                characterController.characterTransformer.XAddComponent<CharacterMoveByInput>();
                characterController.characterTransformer.XAddComponent<CharacterMoveSpeedByInput>();
                characterController.characterTransformer.XAddComponent<CharacterRotateByInput>();
                characterController.characterTransformer.XAddComponent<CharacterRotateByTouch>();
            }
            else
            {
                //添加各种控制器组件
                characterController.XAddComponent<CharacterCameraController>();
                characterController.XAddComponent<CharacterTransformer>();

                //添加移动与旋转控制组件
                characterController.XAddComponent<CharacterMoveByInput>();
                characterController.XAddComponent<CharacterMoveSpeedByInput>();
                characterController.XAddComponent<CharacterRotateByInput>();
            }

            //重置角色控制器
            characterController.Reset();

            return characterController;
        }

        /// <summary>
        /// 创建带相机控制器的角色控制器
        /// </summary>
        /// <param name="name"></param>
        /// <param name="multiGameObjectMode"></param>
        /// <returns></returns>
        public static XCharacterController CreateCharacterControllerWithCameraController(string name, bool multiGameObjectMode = true)
        {
            //创建角色控制器
            var characterController = CreateCharacterController(name, multiGameObjectMode);

            //创建相机控制器
            var cameraController = CameraHelperExtension.CreateCameraControllerForCharacter("角色相机");
            var cameraControllerEvent = cameraController.XGetOrAddComponent<CameraControllerEvent>();
            cameraControllerEvent.Add(ECameraControllerEvent.OnSwitchedToCurrent, characterController.gameObject, true);
            cameraControllerEvent.Add(ECameraControllerEvent.OnWillSwitchToLast, characterController.gameObject, false);

            //创建相机拥有者
            characterController.XAddComponent<CameraOwner>();

            //修改相机控制器与角色控制器
            cameraController.gameObject.XSetParent(characterController.characterCameraController.gameObject);
            cameraController.cameraTargetController.mainTarget = characterController.characterTransform;
            cameraController.transform.XSetLocalPosition(new Vector3(0, 3, -3));

            //重置角色控制器
            characterController.characterCameraController.Reset();

            return characterController;
        }

        /// <summary>
        /// 创建空角色
        /// </summary>
        /// <returns></returns>
        public static XCharacterController CreateEmptyCharacter(string name = "空角色")
        {
            //创建带相机控制器的角色控制器
            var characterController = CreateCharacterControllerWithCameraController(name);

            //角色模型
            var characterModel = characterController.XCreateChild<Transform>("角色模型");

            //添加角色虚拟眼睛游戏对象
            var eye = UnityObjectHelper.CreateGameObject("Eye");
            eye.XSetParent(characterModel);
            eye.transform.XSetLocalPosition(new Vector3(0, 1.4f, 0));
            var renderer = eye.XAddComponent<GizmoRenderer>();
            renderer.XModifyProperty(() => {
                renderer.sizeValue = new Vector3(0.8f, 0.2f, 1f);
                renderer.alwayShowGizmos = false;
            });

            //纠正角色相机的目标对象
            characterController.characterCameraController.cameraMainController.cameraTargetController.mainTarget = eye.transform;

            return characterController;
        }
    }
}
