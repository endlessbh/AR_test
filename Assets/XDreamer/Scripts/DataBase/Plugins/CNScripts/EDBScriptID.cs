using XCSJ.Attributes;
using XCSJ.PluginDataBase.Tools;
using XCSJ.Scripts;

namespace XCSJ.PluginDataBase.CNScripts
{
    /// <summary>
    /// 数据库脚本ID
    /// </summary>
    [Name("数据库脚本ID")]
    public enum EDBScriptID
    {
        /// <summary>
        /// 开始
        /// </summary>
        _Begin = IDRange.Begin,

        #region 数据库-目录
        /// <summary>
        /// 数据库
        /// </summary>
        [ScriptName("数据库", nameof(DB), EGrammarType.Category)]
        #endregion
        DB,

        /// <summary>
        /// 显示数据库列表窗口
        /// </summary>
        [ScriptName("显示数据库列表窗口")]
        [ScriptDescription("显示结果集窗口；回调函数的输入参数信息包括数据库索引、行索引、列索引、值 信息，各信息之间以‘,’分割；")]
        [ScriptParams(1, EParamType.Bool, "可见性")]
        [ScriptParams(2, EParamType.UserDefineFun, "当记录点击时回调函数")]
        [ScriptParams(3, EParamType.UserDefineFun, "当字段点击时回调函数")]
        ShowDBListWindow,

        /// <summary>
        /// 获取数据库列表窗口信息
        /// </summary>
        [ScriptName("获取数据库列表窗口信息")]
        [ScriptParams(1, EParamType.Combo, "信息", "可见性", "位置尺寸", "缩放值")]
        GetDBListWindowInfo,

        /// <summary>
        /// 设置数据库列表窗口信息
        /// </summary>
        [ScriptName("设置数据库列表窗口信息")]
        [ScriptParams(1, EParamType.Rect, "位置尺寸")]
        SetDBListWindowRect,

        /// <summary>
        /// 设置数据库列表窗口缩放值
        /// </summary>
        [ScriptName("设置数据库列表窗口缩放值")]
        [ScriptParams(1, EParamType.FloatSlider, "缩放值", 0f, 10f, defaultObject = 1f)]
        SetDBListWindowScale,

        /// <summary>
        /// 执行非查询SQL
        /// </summary>
        [ScriptName("执行非查询SQL", nameof(ExecuteNonQuerySQL))]
        [ScriptReturn("成功返回  #True，失败返回 #False")]
        [ScriptParams(0, EParamType.IntSlider, "数据库索引:", 1, 5)]
        [ScriptParams(1, EParamType.String, "Create/Drop/Insert/Delete/Update语句:")]
        [ScriptParams(10, EParamType.UserDefineFun, "执行后回调函数(携带参数为影响记录的数目):")]
        ExecuteNonQuerySQL,

        /// <summary>
        /// 执行查询SQL
        /// </summary>
        [ScriptName("执行查询SQL", nameof(ExecuteQuerySQL))]
        [ScriptDescription("成功执行的结果存放在结果集中，具体信息可使用结果集的相关脚本进行信息获；")]
        [ScriptReturn("成功返回  #True，失败返回 #False")]
        [ScriptParams(0, EParamType.IntSlider, "数据库索引:", 1, 5)]
        [ScriptParams(1, EParamType.String, "Select语句:")]
        [ScriptParams(10, EParamType.UserDefineFun, "执行后回调函数(携带参数为结果集记录数目):")]
        ExecuteQuerySQL,

        /// <summary>
        /// 执行条件查询
        /// </summary>
        [ScriptName("执行条件查询")]
        [ScriptDescription("成功执行的结果存放在结果集中，具体信息可使用结果集的相关脚本进行信息获；")]
        [ScriptReturn("成功返回 #True，失败返回 #False")]
        [ScriptParams(0, EParamType.IntSlider, "数据库索引:", 1, 5)]
        [ScriptParams(1, EParamType.String, "数据库表名:")]
        [ScriptParams(2, EParamType.String, "字段名(如不填写使用*替换，多个字段使用<c>即,分割):")]
        [ScriptParams(3, EParamType.String, "条件字段:")]
        [ScriptParams(4, EParamType.Combo, "匹配条件:", "=", "<>", ">", ">=", "<", "<=", "like", "between", "in", "not in", " ")]
        [ScriptParams(5, EParamType.String, "条件值:")]
        [ScriptParams(6, EParamType.Bool2, "条件值是文本值(如果是，会对条件值添加单引号):")]
        [ScriptParams(10, EParamType.UserDefineFun, "执行后回调函数(携带参数为结果集记录数目):")]
        ExecuteConditionQuery,

        /// <summary>
        /// 显示结果集窗口
        /// </summary>
        [ScriptName("显示结果集窗口")]
        [ScriptDescription("显示结果集窗口；回调函数的输入参数信息包括数据库索引、行索引、列索引、值、窗口模式信息，各信息之间以‘,’分割；")]
        [ScriptParams(0, EParamType.IntSlider, "数据库索引:", 1, 5)]
        [ScriptParams(1, ResultSetWindowMode_ScriptParam.ScriptParamType, "窗口模式")]
        [ScriptParams(2, EParamType.Bool, "可见性")]
        [ScriptParams(3, EParamType.UserDefineFun, "当记录点击时回调函数")]
        [ScriptParams(4, EParamType.UserDefineFun, "当字段点击时回调函数")]
        ShowResultSetWindow,

        /// <summary>
        /// 获取结果集窗口信息
        /// </summary>
        [ScriptName("获取结果集窗口信息")]
        [ScriptParams(0, EParamType.IntSlider, "数据库索引:", 1, 5)]
        [ScriptParams(1, ResultSetWindowMode_ScriptParam.ScriptParamType, "窗口模式")]
        [ScriptParams(2, EParamType.Combo, "信息", "可见性", "位置尺寸", "字段宽度", "缩放值")]
        GetResultSetWindowInfo,

        /// <summary>
        /// 设置结果集窗口位置尺寸
        /// </summary>
        [ScriptName("设置结果集窗口位置尺寸")]
        [ScriptParams(0, EParamType.IntSlider, "数据库索引:", 1, 5)]
        [ScriptParams(1, ResultSetWindowMode_ScriptParam.ScriptParamType, "窗口模式")]
        [ScriptParams(2, EParamType.Rect, "位置尺寸")]
        SetResultSetWindowRect,

        /// <summary>
        /// 设置结果集窗口位置缩放值
        /// </summary>
        [ScriptName("设置结果集窗口位置缩放值")]
        [ScriptParams(0, EParamType.IntSlider, "数据库索引:", 1, 5)]
        [ScriptParams(1, ResultSetWindowMode_ScriptParam.ScriptParamType, "窗口模式")]
        [ScriptParams(2, EParamType.FloatSlider, "缩放值", 0f, 10f, defaultObject = 1f)]
        SetResultSetWindowScale,

        /// <summary>
        /// 设置结果集窗口字段宽度
        /// </summary>
        [ScriptName("设置结果集窗口字段宽度")]
        [ScriptParams(0, EParamType.IntSlider, "数据库索引:", 1, 5)]
        [ScriptParams(1, ResultSetWindowMode_ScriptParam.ScriptParamType, "窗口模式")]
        [ScriptParams(2, EParamType.FloatSlider, "字段宽度", -1f, 1920f, defaultObject = ResultSetWindow.DefaultFieldWidth)]
        SetResultSetWindowFieldWidth,

        /// <summary>
        /// 获取结果集信息
        /// </summary>
        [ScriptName("获取结果集信息")]
        [ScriptReturn("成功返回 具体的请求信息，失败返回 #False")]
        [ScriptParams(0, EParamType.IntSlider, "数据库索引:", 1, 5)]
        [ScriptParams(1, EParamType.Combo, "信息类型:", "记录数目", "字段数目", "SQL语句", "结果值", "错误信息", "是否有效")]
        GetResultSetInfo,

        /// <summary>
        /// 获取结果集字段名
        /// </summary>
        [ScriptName("获取结果集字段名")]
        [ScriptReturn("成功返回 具体的请求信息，失败返回 #False")]
        [ScriptParams(0, EParamType.IntSlider, "数据库索引:", 1, 5)]
        [ScriptParams(1, EParamType.IntSlider, "字段索引:", 1, 32)]
        GetResultSetFieldName,

        /// <summary>
        /// 获取结果集字段值
        /// </summary>
        [ScriptName("获取结果集字段值")]
        [ScriptReturn("成功返回 具体的请求信息，失败返回 #False")]
        [ScriptParams(0, EParamType.IntSlider, "数据库索引:", 1, 5)]
        [ScriptParams(1, EParamType.IntSlider, "记录索引:", 1, 999)]
        [ScriptParams(2, EParamType.IntSlider, "字段索引:", 1, 32)]
        GetResultSetFieldValue,

        /// <summary>
        /// 获取结果集字段值(按字段名)
        /// </summary>
        [ScriptName("获取结果集字段值(按字段名)")]
        [ScriptReturn("成功返回 具体的请求信息，失败返回 #False")]
        [ScriptParams(0, EParamType.IntSlider, "数据库索引:", 1, 5)]
        [ScriptParams(1, EParamType.IntSlider, "记录索引:", 1, 999)]
        [ScriptParams(2, EParamType.String, "字段名:")]
        GetResultSetFieldValueByFieldName,

        /// <summary>
        /// 结果集转字典
        /// </summary>
        [ScriptName("结果集转字典")]
        [ScriptParams(0, EParamType.IntSlider, "数据库索引:", 1, 5)]
        [ScriptParams(1, EParamType.Combo, "转换方式:", "完整结果集", "仅记录", "仅字段")]
        [ScriptParams(2, EParamType.Dictionary, "字典名:")]
        ResultSetToDictionary,

        /// <summary>
        /// 结果集转字符串
        /// </summary>
        [ScriptName("结果集转字符串")]
        [ScriptParams(0, EParamType.IntSlider, "数据库索引:", 1, 5)]
        [ScriptParams(1, EParamType.Combo, "转换方式:", "完整结果集", "仅记录", "仅字段")]
        ResultSetToString,

        /// <summary>
        /// 当前已使用的脚本最大ID
        /// </summary>
        MaxCurrent,

        /// <summary>
        /// 借宿
        /// </summary>
        _End = IDRange.End,
    }
}

