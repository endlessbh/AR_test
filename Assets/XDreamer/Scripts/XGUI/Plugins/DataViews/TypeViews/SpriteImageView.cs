using System;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginXGUI.Base;
using XCSJ.PluginXGUI.DataViews.Base;

namespace XCSJ.PluginXGUI.DataViews.TypeViews
{
    /// <summary>
    /// 精灵图像数据视图
    /// </summary>
    [Name("精灵图像数据视图")]
    [DataViewAttribute(typeof(Sprite))]
#if UNITY_EDITOR && XDREAMER_EDITION_DEVELOPER
    //[Tool(XGUICategory.DataTypeView, rootType = typeof(XGUIManager))]
#endif
    public sealed class SpriteImageView : BaseModelViewCache
    {
        [Name("图像")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Image _spriteView;

        public override Type viewValueType => typeof(Sprite);

        public override object viewValue { get => _spriteView.sprite; set => _spriteView.sprite = (Sprite)value; }
    }
}
