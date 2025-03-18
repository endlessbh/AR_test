using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginTools.LineNotes
{
    /// <summary>
    /// 线样式
    /// </summary>
    [Name("线样式")]
    public class LineStyle : InteractProvider
    {
        [Name("宽度")]
        [Range(0, 100)]
        [Tip("=0为系统细线", "=0 is the system thin line")]
        public float width = 0;

        [Name("颜色")]
        public Color color = Color.green;

        [Name("材质")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Material mat;

        [Name("遮挡")]
        public bool occlusion = true;
    }
}
