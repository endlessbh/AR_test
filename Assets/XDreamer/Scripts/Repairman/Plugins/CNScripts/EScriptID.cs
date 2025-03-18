using XCSJ.Attributes;
using XCSJ.PluginRepairman.UI;
using XCSJ.Scripts;

namespace XCSJ.PluginRepairman.CNScripts
{
    /// <summary>
    /// 脚本ID
    /// </summary>
    [Name("脚本ID")]
    public enum EScriptID
    {
        /// <summary>
        /// 开始
        /// </summary>
        _Begin = IDRange.Begin,

        #region 拆装-目录
        /// <summary>
        /// 拆装-目录
        /// </summary>
        [ScriptName("拆装", nameof(PluginRepairman), EGrammarType.Category)]
        [ScriptDescription("拆装")]
        #endregion
        PluginRepairman,

        [ScriptName("设置物品拾取最大数量", nameof(SetItemSelectedMaxCount))]
        [ScriptDescription("设置物品拾取最大数量")]
        [ScriptReturn("成功返回 #True ;失败返回 #False;")]
        [ScriptParams(1, EParamType.IntSlider, "拾取数量:", 0, 100)]
        SetItemSelectedMaxCount,

        [ScriptName("清除选择工具", nameof(ClearSelectedTool))]
        [ScriptDescription("清除选择工具")]
        [ScriptReturn("成功返回 #True ;失败返回 #False;")]
        ClearSelectedTool,

        #region 设置设备零件状态
        [ScriptName("设置设备零件状态", nameof(SetDevicePartState))]
        [ScriptDescription("设置设备零件状态")]
        [ScriptReturn("成功返回 #True ;失败返回 #False;")]
        [ScriptParams(1, Device_ScriptParam.Device, "设备:")]
        [ScriptParams(2, PartState_ScriptParam.ScriptPartStateType, "零件状态:")]
        #endregion
        SetDevicePartState,

        #region 创建零件列表
        [ScriptName("创建零件列表", nameof(CreatePartList))]
        [ScriptDescription("创建零件列表")]
        [ScriptReturn("成功返回 #True ;失败返回 #False;")]
        [ScriptParams(1, EParamType.GameObjectComponent, "零件列表界面:", typeof(GUIPartList))]
        #endregion
        CreatePartList,

        #region 获取修理步骤可点击
        [ScriptName("获取修理步骤可点击", nameof(GetRepairStepClickEnable))]
        [ScriptDescription("获取修理步骤能否点击")]
        [ScriptReturn("返回修理步骤能否点击")]
        #endregion
        GetRepairStepClickEnable,

        #region 设置修理步骤可点击
        [ScriptName("设置修理步骤可点击", nameof(SetRepairStepClickEnable))]
        [ScriptDescription("设置修理步骤能否点击")]
        [ScriptReturn("成功返回 #True ;失败返回 #False;")]
        [ScriptParams(1, EParamType.Bool2, "可点击:")]
        #endregion
        SetRepairStepClickEnable,

        #region 获取修理步骤零件和工具自动选中
        [ScriptName("获取修理步骤零件和工具自动选中", nameof(GetRepairStepAutoSelectPartAndTool))]
        [ScriptDescription("返回修理步骤零件和工具是否自动选中。")]
        [ScriptReturn("成功返回 #True ;失败返回 #False;")]
        #endregion
        GetRepairStepAutoSelectPartAndTool,

        #region 设置修理步骤零件和工具自动选中
        [ScriptName("设置修理步骤零件和工具自动选中", nameof(SetRepairStepAutoSelectPartAndTool))]
        [ScriptDescription("当播放器播放修理步骤的时候，步骤对应的零件和工具会被自动选中。")]
        [ScriptReturn("成功返回 #True ;失败返回 #False;")]
        [ScriptParams(1, EParamType.Bool2, "自动选中:")]
        #endregion
        SetRepairStepAutoSelectPartAndTool,


        #region 拆装教学-目录
        /// <summary>
        /// 拆装教学
        /// </summary>
        [ScriptName("拆装教学", nameof(RepairmanTeaching), EGrammarType.Category)]
        [ScriptDescription("拆装教学的相关脚本目录；")]
        #endregion
        RepairmanTeaching,

        #region 拆装学习操作
        [ScriptName("拆装学习操作", nameof(OperateStudy))]
        [ScriptDescription("学习操作")]
        [ScriptReturn("成功返回 #True ;失败返回 #False;")]
        [ScriptParams(1, RepairStudy_ScriptParam.RepairStudy, "拆装学习组件")]
        [ScriptParams(2, EParamType.Combo, "操作:", "提示")]
        #endregion
        OperateStudy,

        #region 拆装考试操作
        [ScriptName("拆装考试操作", nameof(OperateExam))]
        [ScriptDescription("考试操作")]
        [ScriptReturn("成功返回 #True ;失败返回 #False;")]
        [ScriptParams(1, RepairExam_ScriptParam.RepairExam, "拆装考试组件")]
        [ScriptParams(2, EParamType.Combo, "操作:", "回答问题", "跳过")]
        #endregion
        OperateExam,

        /// <summary>
        /// 结束
        /// </summary>
        _End = IDRange.End,
    }
}

