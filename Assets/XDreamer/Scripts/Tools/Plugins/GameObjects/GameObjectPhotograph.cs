using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginsCameras;
using XCSJ.PluginsCameras.UI;
using XCSJ.PluginXGUI;

namespace XCSJ.PluginTools.GameObjects
{
    /// <summary>
    /// 游戏对象拍照：动态产生游戏对象图像
    /// </summary>
    [Name("游戏对象拍照")]
    [Tool("游戏对象", rootType = typeof(ToolsManager))]
    [Tip("用于产生游戏对象图像的工具", "Tools for generating game object images")]
    [XCSJ.Attributes.Icon(EIcon.Camera)]
    [RequireManager(typeof(ToolsManager))]
    [Owner(typeof(ToolsManager))]
    public class GameObjectPhotograph : InteractProvider
    {
        /// <summary>
        /// 游戏对象
        /// </summary>
        [Name("游戏对象")]
        [OnlyMemberElements]
        public GameObjectPropertyValue _gameObjectPropertyValue = new GameObjectPropertyValue();

        /// <summary>
        /// 渲染贴图相机
        /// </summary>
        [Name("渲染贴图相机")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public RenderTextureCamera _renderTextureCamera;

        /// <summary>
        /// 渲染相机
        /// </summary>
        public Camera renderCamera => _renderTextureCamera ? _renderTextureCamera.renderCamera : CameraHelperExtension.currentCamera;

        /// <summary>
        /// 图像尺寸
        /// </summary>
        [Name("图像尺寸")]
        public Vector2Int _size = new Vector2Int(DefaultTextureWidth, DefaultTextureHeight);

        /// <summary>
        /// 缺省渲染图宽度
        /// </summary>
        public const int DefaultTextureWidth = 256;

        /// <summary>
        /// 缺省渲染图高度
        /// </summary>
        public const int DefaultTextureHeight = 256;

        /// <summary>
        /// 产生图像应用对象类型
        /// </summary>
        [Name("产生图像应用对象类型")]
        [EnumPopup]
        public EApplyObjectType _applyObjectType = EApplyObjectType.Image;

        /// <summary>
        /// 应用对象类型
        /// </summary>
        [Name("应用对象类型")]
        public enum EApplyObjectType
        {
            [Name("无")]
            None,

            [Name("图像")]
            Image,

            [Name("内存图像")]
            RawImage,

            [Name("材质")]
            GameObjectMaterial,
        }

        /// <summary>
        /// 图像
        /// </summary>
        [Name("图像")]
        [HideInSuperInspector(nameof(_applyObjectType), EValidityCheckType.NotEqual, EApplyObjectType.Image)]
        [ValidityCheck(EValidityCheckType.ElementCountGreater, 0)]
        public List<Image> _images;

        /// <summary>
        /// 内存图像
        /// </summary>
        [Name("内存图像")]
        [HideInSuperInspector(nameof(_applyObjectType), EValidityCheckType.NotEqual, EApplyObjectType.RawImage)]
        [ValidityCheck(EValidityCheckType.ElementCountGreater, 0)]
        public List<RawImage> _rawImages;

        /// <summary>
        /// 游戏对象
        /// </summary>
        [Name("游戏对象")]
        [HideInSuperInspector(nameof(_applyObjectType), EValidityCheckType.NotEqual, EApplyObjectType.GameObjectMaterial)]
        [ValidityCheck(EValidityCheckType.ElementCountGreater, 0)]
        public List<GameObject> _gameObjects;

        /// <summary>
        /// 包含游戏对象子对象
        /// </summary>
        [Name("包含游戏对象子对象")]
        [HideInSuperInspector(nameof(_applyObjectType), EValidityCheckType.NotEqual, EApplyObjectType.GameObjectMaterial)]
        public bool _includeGameObjectChildren = true;

        /// <summary>
        /// 材质名称过滤器 ：当对象有多个材质的时候，可使用过滤规则精确定位需要修改的材质颜色
        /// </summary>
        [Name("材质名称过滤器")]
        [Tip("当对象有多个材质的时候，可使用过滤规则精确定位需要修改的材质颜色", "When an object has multiple materials, you can use filtering rules to accurately locate the material color that needs to be modified")]
        [HideInSuperInspector(nameof(_applyObjectType), EValidityCheckType.NotEqual, EApplyObjectType.GameObjectMaterial)]
        public XStringComparer _materialNameComparer = new XStringComparer();

        protected override void OnEnable()
        {
            base.OnEnable();

           CreateImage();
        }

        public void CreateImage()
        {
            if (!renderCamera) return;

            var go = _gameObjectPropertyValue.GetValue();
            if (!go) return;

            var texture = GameObjectViewInfoMB.GetTexture(renderCamera, _size, go);
            if (!texture) return;

            switch (_applyObjectType)
            {
                case EApplyObjectType.Image:
                    {
                        foreach (var item in _images)
                        {
                            if (item)
                            {
                                item.sprite = XGUIHelper.ToSprite(texture);
                            }
                        }
                        break;
                    }
                case EApplyObjectType.RawImage:
                    {
                        foreach (var item in _rawImages)
                        {
                            if (item)
                            {
                                item.texture = texture;
                            }
                        }
                        break;
                    }
                case EApplyObjectType.GameObjectMaterial:
                    {
                        foreach (var item in _gameObjects)
                        {
                            if (!item) continue;

                            var renderers = _includeGameObjectChildren ? go.GetComponentsInChildren<Renderer>() : go.GetComponents<Renderer>();
                            foreach (var r in renderers)
                            {
                                foreach (var m in r.materials)
                                {
                                    if (m && _materialNameComparer.IsMatch(m.name))
                                    {
                                        m.mainTexture = texture;
                                    }
                                }
                            }
                        }
                        break;
                    }
            }
        }
    }
}
