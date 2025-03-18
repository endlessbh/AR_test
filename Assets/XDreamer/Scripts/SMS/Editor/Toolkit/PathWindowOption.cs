using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.LitJson;

namespace XCSJ.EditorSMS.Toolkit
{
    [XDreamerPreferences]
    [Name("路径编辑器")]
    [Import]
    public class PathWindowOption : XDreamerOption<PathWindowOption>
    {
        [Name("关键点尺寸系数")]
        public float keyPointSizeValue = 0.01f;

        [Name("仅记录当前状态机下的移动状态组件")]
        [Tip("勾时，仅记录当前状态机下的移动状态组件;不勾时，记录当前状态机下(包含子状态机中)所有的移动状态组件;", "When checked, only the moving components in the current state machine are recorded; If unchecked, record all moving state components in the current state machine (including sub state machines);")]
        public bool onlyRecordMoveInCurrentStateMachine = true;

        [Name("当停止录制时自动导出")]
        public bool autoExportWhenStopRecord = true;

        [Name("路径线颜色")]
        [Json(exportString = true)]
        public Color pathLineColor = Color.green;

        [Name("路径关键点盒体颜色")]
        [Json(exportString = true)]
        public Color pathKeyPointBoxColor = Color.magenta;

        [Name("路径名称位置偏移量")]
        [Json(exportString = true)]
        public Vector3 namePositionOffset = new Vector3(0,1,0);

        [Name("路径文字颜色")]
        [Json(exportString = true)]
        public Color labelColor = Color.white;

        [Name("路径文字尺寸")]
        public int labelFontSize = 20;

        [Name("虚拟对象尺寸")]
        [Tip("虚拟对象尺寸(虚拟对象:当路径编辑没有游戏对象时，使用一个虚拟体代替编辑对象)", "Dummy size (dummy: when there is no game object in the path editing, use a dummy instead of the editing object)")]
        public float virtualObjectSize = 0.5f;

        [Name("覆盖渲染Gizmos")]
        public bool overrideDrawGizmos = true;

        [Name("默认路径类型")]
        public string defaultPathType = nameof(XCSJ.PluginSMS.States.Motions.Move);
    }
}
