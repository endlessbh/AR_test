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
    /// 贴图原始图像数据视图
    /// </summary>
    [Name("贴图原始图像数据视图")]
    [DataViewAttribute(typeof(Texture))]
#if UNITY_EDITOR && XDREAMER_EDITION_DEVELOPER
    //[Tool(XGUICategory.DataTypeView, rootType = typeof(XGUIManager))]
#endif
    public sealed class TextureRawImageView : BaseModelViewCache
    {
        /// <summary>
        /// 原始图像
        /// </summary>
        [Name("原始图像")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public RawImage _textureView;

        public override Type viewValueType => typeof(Texture);

        public override object viewValue { get => _textureView.texture; set => _textureView.texture = (Texture)value; }
    }
}
