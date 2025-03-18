using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.LitJson;

namespace XCSJ.EditorTools.Windows.Layouts
{
    [XDreamerPreferences]
    [Name("工具包窗口-布局窗口")]
    [Import]
    public class LayoutOption : XDreamerOption<LayoutOption>
    {
        [Name("标准颜色1")]
        [Json(exportString = true)]
        public Color standardColor1 = Color.red;

        [Name("标准颜色2")]
        [Json(exportString = true)]
        public Color standardColor2 = Color.green;
    }
}
