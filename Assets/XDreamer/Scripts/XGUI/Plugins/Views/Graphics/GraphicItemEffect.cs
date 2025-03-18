using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXGUI.Base;

namespace XCSJ.PluginXGUI.Views.Graphics
{
    /// <summary>
    /// 图形项效果
    /// 主要用于，图形项列表
    /// </summary>
    [Name("图形项效果")]
    public class GraphicItemEffect : View
    {
        /// <summary>
        /// 项效果类型
        /// </summary>
        [Name("项效果类型")]
        [EnumPopup]
        public EItemEffect _itemEffect = EItemEffect.None;

        #region 变色效果

        [Name("选中图形颜色")]
        [HideInSuperInspector(nameof(_itemEffect), EValidityCheckType.NotEqual, EItemEffect.ChangeColor)]
        public Color selectedColor = Color.green;

        [Name("未选中图形颜色")]
        [HideInSuperInspector(nameof(_itemEffect), EValidityCheckType.NotEqual, EItemEffect.ChangeColor)]
        public Color normalColor = Color.black;       

        protected void SelectedColor(bool selected, params Graphic[] graphics)
        {
            switch (_itemEffect)
            {
                case EItemEffect.None: break;
                case EItemEffect.ChangeColor:
                    {
                        foreach (var item in graphics)
                        {
                            if (item)
                            {
                                item.color = selected ? selectedColor : normalColor;
                            }
                        }
                        break;
                    }
                default: break;
            }
        } 

        #endregion
    }

    /// <summary>
    /// 项效果类型
    /// </summary>
    [Name("项效果类型")]
    public enum EItemEffect
    {
        [Name("无")]
        None,

        [Name("改变颜色")]
        ChangeColor,
    }

}
