using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Inputs;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginsCameras;
using XCSJ.PluginXGUI;

namespace XCSJ.PluginTools.Effects
{
    /// <summary>
    /// 变光标可视化效果：修改鼠标光标
    /// </summary>
    [Name("变光标")]
    [Tip("修改鼠标光标（当前效果仅在电脑端普通3D模式连接鼠标下有效）")]
    [XCSJ.Attributes.Icon(EIcon.Mouse)]
    public class ChangeMouseCursor : BaseEffect
    {
        public enum ECursorType
        {
            [Name("贴图")]
            Texture2D,

            [Name("UGUI图像")]
            ImageOfUGUI,
        }

        /// <summary>
        /// 光标类型
        /// </summary>
        [Name("光标类型")]
        [EnumPopup]
        public ECursorType _cursorType = ECursorType.Texture2D;

        /// <summary>
        /// 图像
        /// </summary>
        [Name("图像")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [HideInSuperInspector(nameof(_cursorType), EValidityCheckType.NotEqual, ECursorType.ImageOfUGUI)]
        public Image _image;

        private RectTransform imageRectTransform = null;

        /// <summary>
        /// 唤醒
        /// </summary>
        protected void Awake()
        {
            if (_image)
            {
                imageRectTransform = _image.GetComponent<RectTransform>();
                imageRectTransform.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 设置光标
        /// </summary>
        /// <param name="interactData"></param>
        /// <param name="gameObject"></param>
        public override void EnableEffect(InteractData interactData, GameObject gameObject)
        {
            switch (_cursorType)
            {
                case ECursorType.Texture2D:
                    {
                        var texture = GetInteractPropertyValueByRule(gameObject)?.texture;
                        if (texture)
                        {
                            Cursor.SetCursor(texture.ToTexture2D(), Vector2.zero, CursorMode.Auto);
                        }
                        break;
                    }
                case ECursorType.ImageOfUGUI:
                    {
                        if (imageRectTransform)
                        {
                            if (_image)
                            {
                                var texture = GetInteractPropertyValueByRule(gameObject)?.texture;
                                if (texture)
                                {
                                    _image.sprite = texture.ToTexture2D().ToSprite();
                                }
                            }
                            imageRectTransform.gameObject.SetActive(true);
                        }
                        break;
                    }
            }
        }

        /// <summary>
        /// 特效工作中
        /// </summary>
        /// <param name="interactData"></param>
        /// <param name="gameObject"></param>
        public override void EffectWorking(InteractData interactData, GameObject gameObject)
        {
            base.EffectWorking(interactData, gameObject);

            switch (_cursorType)
            {
                case ECursorType.ImageOfUGUI:
                    {
                        if (imageRectTransform && imageRectTransform.gameObject.activeInHierarchy)
                        {
                            var cam = CameraHelperExtension.currentCamera;
                            if (!cam) return;
                            imageRectTransform.anchorMax = imageRectTransform.anchorMin = cam.ScreenToViewportPoint(XInput.mousePosition);
                        }
                        break;
                    }
            }
        }

        /// <summary>
        /// 还原为默认光标
        /// </summary>
        /// <param name="interactData"></param>
        /// <param name="gameObject"></param>
        public override void DisableEffect(InteractData interactData, GameObject gameObject)
        {
            switch (_cursorType)
            {
                case ECursorType.Texture2D:
                    {
                        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                        break;
                    }
                case ECursorType.ImageOfUGUI:
                    {
                        if (imageRectTransform)
                        {
                            imageRectTransform.gameObject.SetActive(false);
                        }
                        break;
                    }
            }
        }

    }
}
