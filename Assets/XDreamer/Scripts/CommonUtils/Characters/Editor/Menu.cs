using System;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.CommonUtils.PluginCharacters;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorTools;
using XCSJ.Extension;
using XCSJ.Extension.Characters;
using XCSJ.Maths;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginTools;
using XCSJ.Tools;

namespace XCSJ.CommonUtils.EditorCharacters
{
    /// <summary>
    /// 菜单
    /// </summary>
    public class Menu
    {
        /// <summary>
        /// 初始化
        /// </summary>
        [InitializeOnLoadMethod]
        public static void Init()
        {
            CharacterHelper.getPhysicMaterial = GetPhysicMaterial;
        }

        /// <summary>
        /// 名称
        /// </summary>
        public const string Name = "角色控制";

        /// <summary>
        /// 路径
        /// </summary>
        public const string Path = "GameObject/" + Product.Name + "/" + Name + "/";

        /// <summary>
        /// 物理材质名：默认为无摩擦的物理材质
        /// </summary>
        public const string PhysicMaterialName = CharacterHelper.PhysicMaterialName;

        /// <summary>
        /// 物理材质名带扩展名
        /// </summary>
        public const string PhysicMaterialNameWithExt = PhysicMaterialName + ".physicMaterial";

        /// <summary>
        /// 物理材质文件夹
        /// </summary>
        public static string PhysicMaterialFolder => UICommonFun.GetAssetsPath(EFolder._Assets) + "/基础/PhysicMaterials/";

        /// <summary>
        /// 获取物理材质
        /// </summary>
        /// <returns></returns>
        public static PhysicMaterial GetPhysicMaterial() => UICommonFun.LoadFromAssets<PhysicMaterial>(PhysicMaterialFolder + PhysicMaterialNameWithExt);

        [XCSJ.Attributes.Icon(EIcon.Authentication)]
        //[Tool(Name, needCreateGroup = false, index = 0)]
        [Name("角色")]
        [RequireManager(typeof(GenericStandardScriptManager))]
        public static GameObject CreateCharacter(ToolContext toolContext)
        {
            // Instance Game object with required character's components
            var gameObject = new GameObject("Character", typeof(BaseCharacterController));

            // Initialize rigidbody
            var rb = gameObject.GetComponent<Rigidbody>();

            rb.useGravity = false;
            rb.isKinematic = false;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.freezeRotation = true;

            // Initialize its collider, attempts to load supplied frictionless material
            var capsuleCollider = gameObject.GetComponent<CapsuleCollider>();

            capsuleCollider.center = new Vector3(0f, 1f, 0f);
            capsuleCollider.radius = 0.5f;
            capsuleCollider.height = 2.0f;
            capsuleCollider.material = GetPhysicMaterial();

            var physicMaterial = capsuleCollider.sharedMaterial;
            if (physicMaterial == null)
            {
                // if not founded, instantiate one and logs a warning message

                physicMaterial = CharacterHelper.CreatePhysicMaterial();

                capsuleCollider.material = physicMaterial;

                Debug.LogWarning(
                    string.Format(
                        "CharacterMovement: No 'PhysicMaterial' found for '{0}' CapsuleCollider, a frictionless one has been created and assigned.\n You should add a Frictionless 'PhysicMaterial' to game object '{0}'.",
                        gameObject.name));
            }

            // Focus the newly created character

            Selection.activeGameObject = gameObject;
            SceneView.FrameLastActiveSceneView();

            return gameObject;
        }

        [XCSJ.Attributes.Icon(EIcon.Authentication)]
        //[Tool(Name, needCreateGroup = false, index = 1)]
        [Name("代理")]
        [RequireManager(typeof(GenericStandardScriptManager))]
        public static GameObject CreateAgent(ToolContext toolContext)
        {
            // Instance Game object with required character's components

            var gameObject = new GameObject("Agent", typeof(BaseAgentController));

            // Initialize rigidbody

            var rb = gameObject.GetComponent<Rigidbody>();

            rb.useGravity = false;
            rb.isKinematic = false;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.freezeRotation = true;

            // Initialize its collider, attempts to load supplied frictionless material

            var capsuleCollider = gameObject.GetComponent<CapsuleCollider>();

            capsuleCollider.center = new Vector3(0f, 1f, 0f);
            capsuleCollider.radius = 0.5f;
            capsuleCollider.height = 2.0f;
            capsuleCollider.material = GetPhysicMaterial();

            var physicMaterial = capsuleCollider.sharedMaterial;
            if (physicMaterial == null)
            {
                // if not founded, instantiate one and logs a warning message

                physicMaterial = CharacterHelper.CreatePhysicMaterial();

                capsuleCollider.material = physicMaterial;

                Debug.LogWarning(
                    string.Format(
                        "CharacterMovement: No 'PhysicMaterial' found for '{0}' CapsuleCollider, a frictionless one has been created and assigned.\n You should add a Frictionless 'PhysicMaterial' to game object '{0}'.",
                        gameObject.name));
            }

            // Focus the newly created character

            Selection.activeGameObject = gameObject;
            SceneView.FrameLastActiveSceneView();

            return gameObject;
        }

        [XCSJ.Attributes.Icon(EIcon.Authentication)]
        //[Tool(Name, needCreateGroup = false, index = 2)]
        [Name("第一人称")]
        [RequireManager(typeof(GenericStandardScriptManager))]
        public static GameObject CreateFirstPerson(ToolContext toolContext)
        {
            // Instance Game object with required character's components

            var gameObject = new GameObject("FirstPerson", typeof(BaseFirstPersonController), typeof(ObsoleteMouseLook));

            // Initialize rigidbody

            var rb = gameObject.GetComponent<Rigidbody>();

            rb.useGravity = false;
            rb.isKinematic = false;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.freezeRotation = true;

            // Initialize its collider, attempts to load supplied frictionless material

            var capsuleCollider = gameObject.GetComponent<CapsuleCollider>();

            capsuleCollider.center = new Vector3(0f, 1f, 0f);
            capsuleCollider.radius = 0.5f;
            capsuleCollider.height = 2.0f;
            capsuleCollider.material = GetPhysicMaterial();

            var physicMaterial = capsuleCollider.sharedMaterial;
            if (physicMaterial == null)
            {
                // if not founded, instantiate one and logs a warning message

                physicMaterial = CharacterHelper.CreatePhysicMaterial();

                capsuleCollider.material = physicMaterial;

                Debug.LogWarning(
                    string.Format(
                        "CharacterMovement: No 'PhysicMaterial' found for '{0}' CapsuleCollider, a frictionless one has been created and assigned.\n You should add a Frictionless 'PhysicMaterial' to game object '{0}'.",
                        gameObject.name));
            }

            //增加眼睛
            var eye = new GameObject("Eye");
            eye.transform.SetParent(gameObject.transform);
            eye.transform.localPosition = new Vector3(0f, 1.6f, 0.0f);

            //眼睛增加渲染用的相机
            var o = new GameObject("Camera", typeof(Camera), typeof(AudioListener), typeof(FlareLayer));
            o.tag = "MainCamera";
            o.transform.SetParent(eye.transform);
            o.transform.localPosition = new Vector3(0, 0, 0);

            // Focus the newly created character

            Selection.activeGameObject = gameObject;
            SceneView.FrameLastActiveSceneView();

            return gameObject;
        }

        [XCSJ.Attributes.Icon(EIcon.Authentication)]
        //[Tool(Name, needCreateGroup = false, index = 3)]
        [Name("自定义角色")]
        [RequireManager(typeof(GenericStandardScriptManager))]
        public static GameObject CreateCustomCharacter(ToolContext toolContext)
        {
            // Instance Game object with required character's components
            var gameObject = new GameObject("CustomCharacter", typeof(CustomCharacterController));

            // Initialize rigidbody
            var rb = gameObject.GetComponent<Rigidbody>();

            rb.useGravity = false;
            rb.isKinematic = false;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.freezeRotation = true;

            // Initialize its collider, attempts to load supplied frictionless material
            var capsuleCollider = gameObject.GetComponent<CapsuleCollider>();

            capsuleCollider.center = new Vector3(0f, 0.8f, 0f);
            capsuleCollider.radius = 0.33f;
            capsuleCollider.height = 1.6f;
            capsuleCollider.material = GetPhysicMaterial();

            var physicMaterial = capsuleCollider.sharedMaterial;
            if (physicMaterial == null)
            {
                // if not founded, instantiate one and logs a warning message

                physicMaterial = CharacterHelper.CreatePhysicMaterial();

                capsuleCollider.material = physicMaterial;

                Debug.LogWarning(
                    string.Format(
                        "CharacterMovement: No 'PhysicMaterial' found for '{0}' CapsuleCollider, a frictionless one has been created and assigned.\n You should add a Frictionless 'PhysicMaterial' to game object '{0}'.",
                        gameObject.name));
            }

            // Focus the newly created character

            Selection.activeGameObject = gameObject;
            SceneView.FrameLastActiveSceneView();

            return gameObject;
        }

        [XCSJ.Attributes.Icon(EIcon.Authentication)]
        //[Tool(Name, needCreateGroup = false, index = 4)]
        [Name("自定义代理")]
        [RequireManager(typeof(GenericStandardScriptManager))]
        public static GameObject CreateCustomAgent(ToolContext toolContext)
        {
            // Instance Game object with required character's components

            var gameObject = new GameObject("CustomAgent", typeof(CustomAgentController));

            // Initialize rigidbody

            var rb = gameObject.GetComponent<Rigidbody>();

            rb.useGravity = false;
            rb.isKinematic = false;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.freezeRotation = true;

            // Initialize its collider, attempts to load supplied frictionless material

            var capsuleCollider = gameObject.GetComponent<CapsuleCollider>();

            capsuleCollider.center = new Vector3(0f, 0.8f, 0f);
            capsuleCollider.radius = 0.33f;
            capsuleCollider.height = 1.6f;
            capsuleCollider.material = GetPhysicMaterial();

            var physicMaterial = capsuleCollider.sharedMaterial;
            if (physicMaterial == null)
            {
                // if not founded, instantiate one and logs a warning message

                physicMaterial = CharacterHelper.CreatePhysicMaterial();

                capsuleCollider.material = physicMaterial;

                Debug.LogWarning(
                    string.Format(
                        "CharacterMovement: No 'PhysicMaterial' found for '{0}' CapsuleCollider, a frictionless one has been created and assigned.\n You should add a Frictionless 'PhysicMaterial' to game object '{0}'.",
                        gameObject.name));
            }

            // Focus the newly created character

            Selection.activeGameObject = gameObject;
            SceneView.FrameLastActiveSceneView();

            return gameObject;
        }

        [XCSJ.Attributes.Icon(EIcon.Authentication)]
        //[Tool(Name, needCreateGroup = false, index = 2)]
        [Name("自定义第一人称")]
        [RequireManager(typeof(GenericStandardScriptManager))]
        public static GameObject CreateCustomFirstPerson(ToolContext toolContext)
        {
            // Instance Game object with required character's components

            var gameObject = new GameObject("CustomFirstPerson", typeof(CustomFirstPersonController), typeof(ObsoleteMouseLook));

            // Initialize rigidbody

            var rb = gameObject.GetComponent<Rigidbody>();

            rb.useGravity = false;
            rb.isKinematic = false;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.freezeRotation = true;

            // Initialize its collider, attempts to load supplied frictionless material

            var capsuleCollider = gameObject.GetComponent<CapsuleCollider>();

            capsuleCollider.center = new Vector3(0f, 0.8f, 0f);
            capsuleCollider.radius = 0.33f;
            capsuleCollider.height = 1.6f;
            capsuleCollider.material = GetPhysicMaterial();

            var physicMaterial = capsuleCollider.sharedMaterial;
            if (physicMaterial == null)
            {
                // if not founded, instantiate one and logs a warning message

                physicMaterial = CharacterHelper.CreatePhysicMaterial();

                capsuleCollider.material = physicMaterial;

                Debug.LogWarning(
                    string.Format(
                        "CharacterMovement: No 'PhysicMaterial' found for '{0}' CapsuleCollider, a frictionless one has been created and assigned.\n You should add a Frictionless 'PhysicMaterial' to game object '{0}'.",
                        gameObject.name));
            }

            //增加眼睛
            var eye = new GameObject("Eye");
            eye.transform.SetParent(gameObject.transform);
            eye.transform.localPosition = new Vector3(0f, 1.4f, 0.0f);

            //眼睛增加渲染用的相机
            var o = new GameObject("Camera", typeof(Camera), typeof(AudioListener), typeof(FlareLayer));
            o.tag = "MainCamera";
            o.transform.SetParent(eye.transform);
            o.transform.localPosition = new Vector3(0f, 0f, 0f);

            // Focus the newly created character

            Selection.activeGameObject = gameObject;
            SceneView.FrameLastActiveSceneView();

            return gameObject;
        }
    }
}
