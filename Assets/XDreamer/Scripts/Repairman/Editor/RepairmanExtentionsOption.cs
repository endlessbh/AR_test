using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.LitJson;

namespace XCSJ.EditorRepairman
{
    [XDreamerPreferences]
    [Name("拆装修理扩展")]
    [Import]
    public class RepairmanExtentionsOption : XDreamerOption<RepairmanExtentionsOption>
    {
        [Name("零件列表尺寸")]
        [Json(exportString = true)]
        public Vector2 partListSize = new Vector2(100, 200);

        [Name("零件项尺寸")]
        [Json(exportString = true)]
        public Vector2 partItemSize = new Vector2(80, 30);

        [Name("工具包尺寸")]
        [Json(exportString = true)]
        public Vector2 toolBagSize = new Vector2(200, 100);

        [Name("工具项尺寸")]
        [Json(exportString = true)]
        public Vector2 toolItemSize = new Vector2(60, 60);

        [Name("项间距")]
        [Json(exportString = true)]
        public Vector2 CellSpaceSize = new Vector2(1, 1);
    }
}
