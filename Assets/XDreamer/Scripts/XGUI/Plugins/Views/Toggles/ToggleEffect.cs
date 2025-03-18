using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXGUI.Base;

namespace XCSJ.PluginXGUI.Views.Toggles
{
    [Name("切换效果")]
    [RequireComponent(typeof(Toggle))]
    public class ToggleEffect : View
    {
        /// <summary>
        /// 切换效果
        /// </summary>
        [Name("切换效果")]
        public enum EToggleEffect
        {
            [Name("默认")]
            Default,

            [Name("透明度")]
            [Tip("开时前景透明度为1，背景透明度为0；关时前景透明度为0，背景透明度为1", "When on, the foreground transparency is 1 and the background transparency is 0; When off, the foreground transparency is 0 and the background transparency is 1")]
            Alpha,
        }

        [Name("切换效果")]
        [EnumPopup]
        public EToggleEffect _toggleEffect = EToggleEffect.Alpha;

        /// <summary>
        /// 切换
        /// </summary>
        protected Toggle toggle = null;

        /// <summary>
        /// 唤醒
        /// </summary>
        protected void Awake()
        {
            toggle = GetComponent<Toggle>();

            ApllyToggleEffect();
        }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            toggle.onValueChanged.AddListener(OnValueChanged);
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            toggle.onValueChanged.RemoveListener(OnValueChanged);
        }

        /// <summary>
        /// 切换值变化
        /// </summary>
        /// <param name="isOn"></param>
        protected virtual void OnValueChanged(bool isOn)
        {
            ApllyToggleEffect();
        }

        /// <summary>
        /// 应用Toggle效果
        /// </summary>
        private void ApllyToggleEffect()
        {
            switch (_toggleEffect)
            {
                case EToggleEffect.Default:
                    break;
                case EToggleEffect.Alpha:
                    {
                        // 前景图
                        if (toggle.graphic)
                        {
                            var fColor = toggle.graphic.color;
                            fColor.a = toggle.isOn ? 1 : 0;
                            toggle.graphic.color = fColor;
                        }

                        // 背景图
                        if (toggle.targetGraphic)
                        {
                            var bColor = toggle.targetGraphic.color;
                            bColor.a = toggle.isOn ? 0 : 1;
                            toggle.targetGraphic.color = bColor;
                        }
                        break;
                    }
            }
        }
    }
}
