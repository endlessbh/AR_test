using UnityEngine;
using XCSJ.Scripts;
using XCSJ.Attributes;
#if XDREAMER_ZXING
#endif
using UnityEngine.UI;
#if UNITY_5_5_OR_NEWER
#endif

namespace XCSJ.Extension.GenericStandards
{
    /// <summary>
    /// ID区间
    /// </summary>
    public static class IDRange
    {
        /// <summary>
        /// 开始37120
        /// </summary>
        public const int Begin = (int)EExtensionID._0x22;

        /// <summary>
        /// 结束37887
        /// </summary>
        public const int End = (int)EExtensionID._0x28 - 1;

        /// <summary>
        /// 片段24
        /// </summary>
        public const int Fragment = 0x18;

        /// <summary>
        /// 通用
        /// </summary>
        public const int Common = Begin + Fragment * 0;

        /// <summary>
        /// Mono行为
        /// </summary>
        public const int MonoBehaviour = Begin + Fragment * 1;

        /// <summary>
        /// 状态库
        /// </summary>
        public const int StateLib = Begin + Fragment * 2;

        /// <summary>
        /// 工具库
        /// </summary>
        public const int Tools = Begin + Fragment * 3;

        /// <summary>
        /// 编辑器
        /// </summary>
        public const int Editor = Begin + Fragment * 4;
    }

    /// <summary>
    /// 消息ID
    /// </summary>
    [Name("消息ID")]
    public enum EMsgID
    {
        /// <summary>
        /// 开始
        /// </summary>
        _Begin = IDRange.Begin,

        /// <summary>
        /// Web请求任务的响应返回,<see cref="XCSJ.Message.Msg"/>中属性<see cref="XCSJ.Message.Msg.Params"/>的参数Key值有：
        /// url    :   请求时传入的url；类型：<see cref="string"/>
        /// ret    :   执行结果；类型：<see cref="bool"/>
        /// type   :   执行消息类型；类型：<see cref="EDataType"/>
        /// fun    :   回调信息的中文脚本函数，类型：<see cref="string"/>
        /// var    :   回传信息的中文脚本全局变量名，类型：<see cref="string"/>
        /// tag    :   执行请求时传入的绑定消息体；类型：<see cref="XCSJ.Algorithms.ParamList"/>
        /// data   :   数据；类型：<see cref="XCSJ.Extension.GenericStandards.Managers.WebDataInfo"/><br />
        /// </summary>
        WebResponseOfWebRequestTask,

        /// <summary>
        /// 结束
        /// </summary>
        _End = IDRange.End,
    }

    /// <summary>
    /// 通用标准脚本ID
    /// </summary>
    [Name("通用标准脚本ID")]
    public enum EGenericStandardScriptID
    {
        /// <summary>
        /// 开始
        /// </summary>
        _Begin = IDRange.Begin,

        #region 通用标准

        #region 通用标准脚本-目录
        /// <summary>
        /// 通用标准脚本
        /// </summary>
        [ScriptName("通用标准脚本", nameof(GenericStandardScript), EGrammarType.Category)]
        [ScriptDescription("通用标准脚本操作的相关脚本目录；本目录下的中文脚本命令会不定期的调整；功能稳定后可能将对应中文脚本命令转移到功能集合所在的DLL管理器中；")]
        #endregion
        GenericStandardScript,

        #region 设置Animator播放速度
        /// <summary>
        /// 跟踪陀螺仪
        /// </summary>
        [ScriptName("设置Animator播放速度", nameof(SetAnimatorSpeed))]
        [ScriptDescription("设置Animator播放速度;")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObjectComponent, "动画对象（限定Animator类型）：", "", typeof(Animator))]
        [ScriptParams(2, EParamType.Float, "速度:", 0f, 100f)]
        #endregion 设置Animator播放速度
        SetAnimatorSpeed,

        #region 设置Shader
        /// <summary>
        /// 设置Shader
        /// </summary>
        [ScriptName("设置Shader", nameof(SetShader))]
        [ScriptDescription("设置游戏对象下所有Render的Shader材质;")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObject, "游戏对象:")]
        [ScriptParams(2, EParamType.String, "shader路径:")]
        [ScriptParams(3, EParamType.Bool, "包含子对象:", defaultObject = EBool.Yes)]
        #endregion 设置Shader
        SetShader,

        #region 获取模型层ID
        /// <summary>
        /// 获取模型层ID
        /// </summary>
        [ScriptName("获取模型层ID", nameof(GetModelLayerID))]
        [ScriptDescription("获取当前模型层的数字ID;")]
        [ScriptReturn("成功返回 模型层数字ID ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObject, "游戏对象:")]
        #endregion 获取模型层ID
        GetModelLayerID,

        #region 获取模型层名称
        /// <summary>
        /// 获取模型层名称
        /// </summary>
        [ScriptName("获取模型层名称", nameof(GetModelLayerName))]
        [ScriptDescription("获取当前模型层的名称;")]
        [ScriptReturn("成功返回 模型层数字ID ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObject, "游戏对象:")]
        #endregion 获取模型层名称
        GetModelLayerName,

        #region 设置模型层
        /// <summary>
        /// 设置模型层ID
        /// </summary>
        [ScriptName("设置模型层ID", nameof(SetModelLayerID))]
        [ScriptDescription("设置当前模型层的ID;")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObject, "游戏对象:")]
        [ScriptParams(2, EParamType.IntSlider, "层ID:", 0, 31)]
        [ScriptParams(3, EParamType.Bool2, "包含子对象:", defaultObject = EBool2.Yes)]
        #endregion 设置模型层
        SetModelLayerID,

        #region 设置模型层
        /// <summary>
        /// 设置模型层ID
        /// </summary>
        [ScriptName("设置模型层(通过名称)", nameof(SetModelLayerName))]
        [ScriptDescription("设置当前模型层的名称;")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObject, "游戏对象:")]
        [ScriptParams(2, EParamType.String, "层名称:")]
        [ScriptParams(3, EParamType.Bool2, "包含子对象:", defaultObject = EBool2.Yes)]
        #endregion 设置模型层
        SetModelLayerName,

        [ScriptName("获取Profiler信息", nameof(GetProfilerInfo))]
        [ScriptParams(1, EParamType.Combo, "信息:", "MonoHeapSize", "MonoUsedSize", "TotalAllocatedMemory", "TotalReservedMemory", "TotalUnusedReservedMemory", "usedHeapSize", "supported", "enabled")]
        GetProfilerInfo,

        [ScriptName("设置Profiler可用性", nameof(SetProfilerEnabled))]
        [ScriptParams(1, EParamType.Bool, "是否可用:")]
        SetProfilerEnabled,

        [ScriptName("设置图形颜色", nameof(SetGraphicColor))]
        [ScriptDescription("设置图形颜色;所有可设置颜色的UGUI都属于图形对象")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObjectComponent, "图形对象:", typeof(Graphic))]
        [ScriptParams(2, EParamType.Color, "颜色:", defaultObject = "255/255/255/255")]
        [ScriptParams(3, EParamType.Bool2, "包含子图形对象:", defaultObject = false)]
        SetGraphicColor,

        #endregion

        #region 游戏对象扩展

        [ScriptName("获取游戏对象之间朝向夹角", nameof(GetAngleWithTwoGameObjectForward))]
        [ScriptDescription("获取游戏对象之间朝向夹角")]
        [ScriptReturn("成功返回 夹角 ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObject, "游戏对象1:")]
        [ScriptParams(2, EParamType.GameObject, "游戏对象2:")]
        GetAngleWithTwoGameObjectForward,

        #endregion

        #region 应用程序

        #region 应用程序-目录
        /// <summary>
        /// 应用程序-目录
        /// </summary>
        [ScriptName("应用程序", nameof(Application), EGrammarType.Category)]
        #endregion
        Application,

        #region 窗口全屏切换
        /// <summary>
        /// 窗口全屏切换
        /// </summary>
        [ScriptName("窗口全屏切换", nameof(FullWindowChange))]
        [ScriptDescription("窗口全屏切换；")]
        [ScriptReturn("成功返回 #True  ;失败返回 #False;")]
        [ScriptParams(1, EParamType.Combo, "状态:", "开启", "关闭", "切换")]
        #endregion
        FullWindowChange,

        #region 关闭程序
        /// <summary>
        /// 关闭程序
        /// </summary>
        [ScriptName("关闭程序", nameof(CloseApplication))]
        [ScriptDescription("关闭程序；")]
        [ScriptReturn("成功返回 #True  ;失败返回 #False;")]
        #endregion
        CloseApplication,

        #region 取消关闭程序
        /// <summary>
        /// 取消关闭程序
        /// </summary>
        [ScriptName("取消关闭程序", nameof(CancelQuitApplication))]
        [ScriptDescription("取消关闭程序；在脚本MonoBehaviour事件的“程序退出时 执行”中可调用，取消关闭程序！")]
        [ScriptReturn("成功返回 #True  ;失败返回 #False;")]
        #endregion
        CancelQuitApplication,

        #region 获取应用程序信息
        /// <summary>
        /// 获取应用程序信息
        /// </summary>
        [ScriptName("获取应用程序信息", nameof(GetApplicationInfo))]
        [ScriptDescription("获取应用程序信息；")]
        [ScriptReturn("成功返回 具体信息  ;失败返回 #False;")]
        [ScriptParams(1, EParamType.Combo, "信息类型：", "绝对网址", "公司名", "数据路径", "关卡数目", "关卡索引", "关卡名称", "持续数据路径", "运行平台", "产品名称", "流资源路径", "系统语言", "临时缓存路径", "unity版本", "版本")]
        #endregion
        GetApplicationInfo,

        #region 设置应用程序信息
        /// <summary>
        /// 设置应用程序信息
        /// </summary>
        [ScriptName("设置应用程序信息", nameof(SetApplicationInfo))]
        [ScriptDescription("设置应用程序信息；")]
        [ScriptReturn("成功返回 #True  ;失败返回 #False;")]
        [ScriptParams(1, EParamType.Combo, "信息类型：", "目标帧速率", "后台运行")]
        [ScriptParams(2, EParamType.String, "参数：")]
        #endregion
        SetApplicationInfo,

        #region 应用程序外部调用
        /// <summary>
        /// 应用程序外部调用
        /// </summary>
        [ScriptName("应用程序外部调用", nameof(ApplicationExternalCall))]
        [ScriptDescription("应用程序外部调用；对应Application.ExternalCall;可适用于Unity5.6之前版本；之后版本不再生效;")]
        [ScriptReturn("成功返回 #True  ;失败返回 #False;")]
        [ScriptParams(1, EParamType.String, "JS函数名：")]
        [ScriptParams(2, EParamType.String, "参数1：")]
        #endregion
        ApplicationExternalCall,

        #region 应用程序外部运行
        /// <summary>
        /// 应用程序外部运行
        /// </summary>
        [ScriptName("应用程序外部运行", nameof(ApplicationExternalEval))]
        [ScriptDescription("应用程序外部运行；对应Application.ExternalEval;可适用于Unity5.6之前版本；之后版本不再生效;")]
        [ScriptReturn("成功返回 #True  ;失败返回 #False;")]
        [ScriptParams(1, EParamType.String, "JS函数名：")]
        #endregion
        ApplicationExternalEval,

        #region 恢复窗口
        /// <summary>
        /// 最小化窗口
        /// </summary>
        [ScriptName("恢复窗口", nameof(RecoverWindow))]
        [ScriptDescription("恢复窗口；恢复窗口为刚启动时的窗口尺寸；")]
        [ScriptReturn("成功返回 #True  ;失败返回 #False;")]
        #endregion
        RecoverWindow,

        #region 设置窗口尺寸
        /// <summary>
        /// 设置窗口尺寸
        /// </summary>
        [ScriptName("设置窗口尺寸", nameof(SetWindowSize))]
        [ScriptDescription("设置窗口尺寸；如当前为全屏模式，执行本脚本后会切换为窗口模式；")]
        [ScriptReturn("成功返回 #True  ;失败返回 #False;")]
        [ScriptParams(1, EParamType.IntSlider, "窗口宽度:", 0, 1920)]
        [ScriptParams(2, EParamType.IntSlider, "窗口高度:", 0, 1080)]
        #endregion
        SetWindowSize,

        #region 获取窗口尺寸
        /// <summary>
        /// 获取窗口尺寸
        /// </summary>
        [ScriptName("获取窗口尺寸", nameof(GetWindowSize))]
        [ScriptDescription("获取窗口尺寸；")]
        [ScriptReturn("成功返回 #True  ;失败返回 #False;")]
        [ScriptParams(1, EParamType.Combo, "尺寸类型：", "窗口宽度", "窗口高度")]
        #endregion
        GetWindowSize,

        #region 打开网页
        /// <summary>
        /// 打开网页
        /// </summary>
        [ScriptName("打开网页", nameof(OpenURL))]
        [ScriptDescription("打开网页；")]
        [ScriptReturn("成功返回 #True  ;失败返回 #False;")]
        [ScriptParams(1, EParamType.File, "网址URL:")]
        #endregion
        OpenURL,

        #region 启动可执行程序
        /// <summary>
        /// 启动可执行程序
        /// </summary>
        [ScriptName("启动可执行程序", nameof(StartExe))]
        [ScriptDescription("启动可执行程序；Window平台为启动EXE执行程序；其他平台功能暂不实现；")]
        [ScriptReturn("成功返回 #True  ;失败返回 #False;")]
        [ScriptParams(1, EParamType.File, "可执行程序路径(本地路径):")]
        [ScriptParams(2, EParamType.String, "执行参数:")]
        #endregion
        StartExe,

        #region 启动文件
        /// <summary>
        /// 启动文件
        /// </summary>
        [ScriptName("启动文件", nameof(StartFile))]
        [ScriptDescription("启动文件；")]
        [ScriptReturn("成功返回 #True  ;失败返回 #False;")]
        [ScriptParams(1, EParamType.File, "文件路径（本地路径）:")]
        #endregion
        StartFile,

        #region 屏幕截图导出图片
        /// <summary>
        /// 屏幕截图导出图片
        /// </summary>
        [ScriptName("屏幕截图导出图片", nameof(ScreenshotExportToImage))]
        [ScriptDescription("屏幕截图导出图片；每帧只能使用一次；在web中无效；自动保存在工程根目录下；")]
        [ScriptReturn("成功返回 #True  ;失败返回 #False;")]
        [ScriptParams(1, EParamType.String, "文件名称:")]
        [ScriptParams(2, EParamType.IntSlider, "放大系数(当前屏幕分辨率倍数):", 0, 16)]
        [ScriptParams(3, EParamType.Combo, "文件名称变化规则（文件后缀使用文件名称参数中后缀）:", "文件名称", "文件名称与系统时间", "系统时间", "文件名称与递增编号", "递增编号")]
        [ScriptParams(4, EParamType.String, "系统时间格式(文件名称变化规则使用系统时间时本参数生效):", defaultObject = "yyyyMMdd-HHmmss-fff")]
        #endregion
        ScreenshotExportToImage,

        #region 如果当前运行平台
        /// <summary>
        /// 如果当前运行平台
        /// </summary>
        [ScriptName("如果当前运行平台", nameof(IfCurrentRuntimePlatform), EGrammarType.If)]
        [ScriptDescription("如果当前运行平台；")]
        [ScriptReturn("成功返回 #True  ;失败返回 #False;")]
        [ScriptParams(1, EParamType.Combo, "判断:", "是", "不是")]
        [ScriptParams(2, EParamType.RuntimePlatform, "运行平台类型:")]
        #endregion
        IfCurrentRuntimePlatform,

        #region 否则如果当前运行平台
        /// <summary>
        /// 否则如果当前运行平台
        /// </summary>
        [ScriptName("否则如果当前运行平台", nameof(ElseIfCurrentRuntimePlatform), EGrammarType.ElseIf)]
        [ScriptDescription("否则如果当前运行平台；")]
        [ScriptReturn("成功返回 #True  ;失败返回 #False;")]
        [ScriptParams(1, EParamType.Combo, "判断:", "是", "不是")]
        [ScriptParams(2, EParamType.RuntimePlatform, "运行平台类型:")]
        #endregion
        ElseIfCurrentRuntimePlatform,

        #region 请求用户授权
        /// <summary>
        /// 请求用户授权
        /// </summary>
        [ScriptName("请求用户授权", nameof(RequestUserAuthorization))]
        [ScriptDescription("请求用户授权；")]
        [ScriptReturn("成功返回 #True  ;失败返回 #False;")]
        [ScriptParams(1, EParamType.Combo, "用户授权类型:", "相机", "麦克风")]
        [ScriptParams(2, EParamType.UserDefineFun, "回调函数（传入参数标识用户是否授权）:")]
        #endregion
        RequestUserAuthorization,

        #region 检查用户授权
        /// <summary>
        /// 检查用户授权
        /// </summary>
        [ScriptName("检查用户授权", nameof(CheckUserAuthorization))]
        [ScriptDescription("检查用户授权；")]
        [ScriptReturn("成功返回 #True  ;失败返回 #False;")]
        [ScriptParams(1, EParamType.Combo, "用户授权类型:", "相机", "麦克风")]
        #endregion
        CheckUserAuthorization,

        #region 获取命令行参数
        /// <summary>
        /// 获取命令行参数
        /// </summary>
        [ScriptName("获取命令行参数", nameof(GetCommandLineArgs))]
        [ScriptDescription("获取命令行参数；在winodow中会将命令行参数以拆分后数组的形式存储；")]
        [ScriptReturn("成功返回 命令行  ;失败返回 #False;")]
        [ScriptParams(1, EParamType.Array, "期望存储命令行参数的数组名:")]
        #endregion
        GetCommandLineArgs,

        #endregion

        #region 系统

        #region 系统-目录
        /// <summary>
        /// 系统-目录
        /// </summary>
        [ScriptName("系统", nameof(System), EGrammarType.Category)]
        #endregion
        System,

        /// <summary>
        /// 获取环境信息
        /// </summary>
        [ScriptName("获取环境信息", nameof(GetEnvironmentInfo))]
        [ScriptDescription("获取当前系统环境的各种信息；")]
        [ScriptReturn("成功返回 对应信息  ;失败返回 #False;")]
        [ScriptParams(1, EParamType.Combo, "信息类型:", "退出码", "是64为进程", "当前管理线程ID", "当前目录", "命令行", "机器名", "新行", "OS版本", "处理器数", "堆栈跟踪", "系统目录", "系统页尺寸", "滴答数", "用户域名", "用户交互", "用户名", "版本", "工作集", "关机已启动", "是64位操作系统")]
        GetEnvironmentInfo,

        /// <summary>
        /// 获取DNS信息
        /// </summary>
        [ScriptName("获取DNS信息", nameof(GetDNSInfo))]
        [ScriptReturn("成功返回 对应信息  ;失败返回 #False;")]
        [ScriptParams(1, EParamType.Combo, "信息类型:", "主机名", "主机IP地址", "主机IPV4地址", "主机IPV6地址")]
        GetDNSInfo,

        #endregion

        #region 时间

        #region 时间-目录
        /// <summary>
        /// 时间-目录
        /// </summary>
        [ScriptName("时间", nameof(TimeCategory), EGrammarType.Category)]
        #endregion
        TimeCategory,

        #region 获取当前系统时间
        /// <summary>
        /// 获取当前时间
        /// </summary>
        [ScriptName("获取当前系统时间", nameof(GetCurrentSystemTime))]
        [ScriptDescription("获取当前系统时间；格式说明（以 2015-06-08 18:13:25为例）：Y或y June,2015；yy 15；yyy 2015；yyyy 2015；yyyyy 02015；M或m June 8；MM 06；MMM Jun；MMMM June；d 6/8/2015；dd 08；ddd Mon；dddd Monday；D Monday,June 08, 2015；HH或H_ 18；hh 06(12小时制的18点)；mm 13；s 2015-06-08T18:13:25；ss 25；y 年，M 月，d 日，h 时(12小时制)，H 时(24小时制)，m 分，s 秒；默认输出格式为：MM/dd/yyyy hh:mm:ss AM/PM 即 6/8/2015 6:13:25 PM；其中_表示空格；")]
        [ScriptReturn("成功返回 时间字符串  ;失败返回 #False;")]
        [ScriptParams(1, EParamType.Combo, "输出格式:", "G", "g", "D", "d", "F", "f", "T", "t", "U", "u", "M", "m", "R", "r", "Y", "y", "O", "o", "s", "yyyyMMddHHmmss", "yyyy-MM-dd HH:mm:ss", "yyyyMMdd", "yyyy-MM-dd", "HHmmss", "HH:mm:ss", "yyyy", "MM", "dd", "HH", "mm", "ss", "yyyy/MM/dd HH:mm:ss:ffff")]
        [ScriptParams(2, EParamType.String, "自定义格式(如本项不空，则忽略已定义的输出格式):")]
        #endregion
        GetCurrentSystemTime,

        #region 获取启动时间
        /// <summary>
        /// 获取启动时间
        /// </summary>
        [ScriptName("获取启动时间", nameof(GetStartTime))]
        [ScriptDescription("获取启动时间；格式说明（以 2015-06-08 18:13:25为例）：Y或y June,2015；yy 15；yyy 2015；yyyy 2015；yyyyy 02015；M或m June 8；MM 06；MMM Jun；MMMM June；d 6/8/2015；dd 08；ddd Mon；dddd Monday；D Monday,June 08, 2015；HH或H_ 18；hh 06(12小时制的18点)；mm 13；s 2015-06-08T18:13:25；ss 25；y 年，M 月，d 日，h 时(12小时制)，H 时(24小时制)，m 分，s 秒；默认输出格式为：MM/dd/yyyy hh:mm:ss AM/PM 即 6/8/2015 6:13:25 PM；其中_表示空格；")]
        [ScriptReturn("成功返回 场景启动的时间 ;失败返回 #False;")]
        [ScriptParams(1, EParamType.Combo, "输出格式:", "G", "g", "D", "d", "F", "f", "T", "t", "U", "u", "M", "m", "R", "r", "Y", "y", "O", "o", "s", "yyyyMMddHHmmss", "yyyy-MM-dd HH:mm:ss", "yyyyMMdd", "yyyy-MM-dd", "HHmmss", "HH:mm:ss", "yyyy", "MM", "dd", "HH", "mm", "ss", "yyyy/MM/dd HH:mm:ss:ffff")]
        [ScriptParams(2, EParamType.String, "自定义格式(如本项不空，则忽略已定义的输出格式):")]
        #endregion
        GetStartTime,

        #region 获取Unity时间信息
        /// <summary>
        /// 获取Unity时间信息
        /// </summary>
        [ScriptName("获取Unity时间信息", nameof(GetUnityTimeInfo))]
        [ScriptDescription("获取Unity时间信息;对应UnityEngine.Time对象；")]
        [ScriptReturn("成功返回 获取Unity时间信息 ;失败返回 #False;")]
        [ScriptParams(1, EParamType.Combo, "信息类型:", nameof(Time.captureFramerate), nameof(Time.deltaTime), nameof(Time.fixedDeltaTime), nameof(Time.fixedTime), nameof(Time.frameCount), nameof(Time.maximumDeltaTime), nameof(Time.realtimeSinceStartup), nameof(Time.renderedFrameCount), nameof(Time.smoothDeltaTime), nameof(Time.time), nameof(Time.timeScale), nameof(Time.timeSinceLevelLoad), nameof(Time.unscaledDeltaTime), nameof(Time.unscaledTime), "即时FPS", "平均FPS(全局)", "平均FPS(当前)")]
        #endregion
        GetUnityTimeInfo,

        #region 数字转时分秒格式
        /// <summary>
        /// 获取演示播放时长
        /// </summary>
        [ScriptName("数字转时分秒格式", nameof(ConvertSecondToHMS))]
        [ScriptDescription("将单位为秒的整数转为 时:分:秒格式（00:00:00）;输入不能超过24小时;")]
        [ScriptReturn("成功返回时分秒格式字符串 ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.IntSlider, "秒数:", 0, 86399)] // 86399 = 3600*24-1
        [ScriptParams(2, EParamType.Combo, "格式类型", "时:分:秒", "分:秒")]
        [ScriptParams(3, EParamType.Bool2, "不足十位数用0填充", defaultObject = EBool2.Yes)]
        #endregion
        ConvertSecondToHMS,

        [ScriptName("转换秒为时间格式", nameof(ConvertSecondsToTimeFormat))]
        [ScriptDescription("将单位为秒的数转为时间格式;")]
        [ScriptReturn("成功返回时间格式字符串 ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.Double, "秒:")]
        [ScriptParams(2, EParamType.Combo, "输出格式:", "c", "g", "G", @"hh\:mm\:ss", @"h\:m\:s", @"hh\:mm\:ss\.fff", @"d\.hh\:mm\:ss\.fff", "%d", "hh", "%h", "mm", "%m", "ss", "%s", "%f", "ff", "fff")]
        [ScriptParams(3, EParamType.String, "自定义格式(如本项不空，则忽略已定义的输出格式):")]
        ConvertSecondsToTimeFormat,


        [ScriptName("转换毫秒为时间格式", nameof(ConvertMillisecondsToTimeFormat))]
        [ScriptDescription("将单位为毫秒的数转为时间格式;")]
        [ScriptReturn("成功返回时间格式字符串 ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.Double, "毫秒:")]
        [ScriptParams(2, EParamType.Combo, "输出格式:", "c", "g", "G", @"hh\:mm\:ss", @"h\:m\:s", @"hh\:mm\:ss\.fff", @"d\.hh\:mm\:ss\.fff", "%d", "hh", "%h", "mm", "%m", "ss", "%s", "%f", "ff", "fff")]
        [ScriptParams(3, EParamType.String, "自定义格式(如本项不空，则忽略已定义的输出格式):")]
        ConvertMillisecondsToTimeFormat,

        #region 创建定时器
        /// <summary>
        /// 创建定时器
        /// </summary>
        [ScriptName("创建定时器", nameof(CreateTimer))]
        [ScriptDescription("创建定时器；自定义函数存在，且处于激活状态才可以被执行；会先等待指定的延时时间才调用对应函数；启动本命令后会一直循环调用直到满足循环次数或程序结束，或者用户调用删除定时器的命令；设置定时器后会直接进入执行状态；多次创建同名的定时器以首次创建的为准；定时器在执行完指定次数后会自动删除创建的定时器；")]
        [ScriptReturn("成功调用返回 #True ；定时器名称无效、自定义函数函数不存在、执行次数为0，则返回 #False ；")]
        [ScriptParams(0, EParamType.StandardString, "定时器名称：")]
        [ScriptParams(1, EParamType.UserDefineFun, "自定义函数名：")]
        [ScriptParams(2, EParamType.String, "执行时携带的信息:")]
        [ScriptParams(3, EParamType.IntSlider, "延时时间（单位： 毫秒）:", 0, 9999999)]
        [ScriptParams(4, EParamType.IntSlider, "执行次数（-1 表示无限执行，0不执行）:", -1, 9999999, defaultObject = 1)]
        #endregion
        CreateTimer,

        #region 控制定时器
        /// <summary>
        /// 控制定时器
        /// </summary>
        [ScriptName("控制定时器", nameof(ControlTimer))]
        [ScriptDescription("控制定时器；")]
        [ScriptReturn("成功调用返回 #True ;定时器不存在返回 #False ；")]
        [ScriptParams(0, EParamType.StandardString, "定时器名称：")]
        [ScriptParams(1, EParamType.Combo, "操作命令：", "删除", "暂停", "继续", "全部删除", "全部暂停", "全部继续")]
        #endregion
        ControlTimer,

        #endregion

        #region 选择集扩展

        #region 选择集扩展-目录
        /// <summary>
        /// 选择集扩展
        /// </summary>
        [ScriptName("选择集扩展", nameof(SelectionExtension), EGrammarType.Category)]
        #endregion
        SelectionExtension,

        #region 设置选择集可选最大个数
        [ScriptName("设置选择集可选最大个数", nameof(SetSelectionLimitedMaxCount))]
        [ScriptReturn("成功返回 具体信息, 失败返回 #False")]
        [ScriptParams(1, EParamType.IntSlider, "个数:", 1, 1000)]
        #endregion
        SetSelectionLimitedMaxCount,

        #region 获取选择集对象数量
        [ScriptName("获取选择集对象数量", nameof(GetSelectionCount))]
        [ScriptReturn("成功返回 对象数量, 失败返回 #False")]
        #endregion
        GetSelectionCount,

        #region 选择集操作
        [ScriptName("选择集操作", nameof(SelectionOperation))]
        [ScriptReturn("成功返回 操作第一个对象路径(克隆时为克隆后对象、删除时为删除对象), 失败返回 #False")]
        [ScriptParams(1, EParamType.Combo, "操作类型:", "克隆", "删除")]
        #endregion
        SelectionOperation,

        #endregion

        #region 其它

        /// <summary>
        /// 其它-目录
        /// </summary>
        [ScriptName("其它", nameof(Other), EGrammarType.Category)]
        Other,

        #region 解码网络摄像纹理二维码
        /// <summary>
        /// 解码网络摄像纹理二维码
        /// </summary>
        [ScriptName("解码网络摄像纹理二维码", nameof(DecodeWebCamTextureQRCode))]
        [ScriptDescription("解码网络摄像纹理二维码；用于解析网络摄像纹理画面中的二维码信息；")]
        [ScriptReturn("成功返回 #True ，并可以使用传入的变量名；失败返回 #False")]
        [ScriptParams(1, EParamType.String, "网络摄像设备名称:")]
        [ScriptParams(2, EParamType.GlobalVariableName, "成功解码后字符串信息的存储变量名（变量无则添加有则修改）:")]
        #endregion，
        DecodeWebCamTextureQRCode,

        #region 解码2D纹理二维码
        /// <summary>
        /// 解码2D纹理二维码
        /// </summary>
        [ScriptName("解码2D纹理二维码", nameof(DecodeTexture2DQRCode))]
        [ScriptDescription("解码2D纹理二维码；用于解析2D纹理画面中的二维码信息；")]
        [ScriptReturn("成功返回 #True ，并可以使用传入的变量名；失败返回 #False")]
#pragma warning disable CS0618 // 类型或成员已过时
        [ScriptParams(1, EParamType.UnityAssetObject, "2D纹理:", typeof(Texture2D))]
#pragma warning restore CS0618 // 类型或成员已过时
        [ScriptParams(2, EParamType.GlobalVariableName, "成功解码后字符串信息的存储变量名（变量无则添加有则修改）:")]
        #endregion，
        DecodeTexture2DQRCode,

        #region 设置光标贴图
        /// <summary>
        /// 设置光标贴图
        /// </summary>
        [ScriptName("设置光标贴图", nameof(SetCursorTexture))]
        [ScriptDescription("设置光标贴图；光标贴图类型必须为Cursor；2D纹理为空时设置为缺省光标；")]
        [ScriptReturn("成功返回 #True；失败返回 #False")]
#pragma warning disable CS0618 // 类型或成员已过时
        [ScriptParams(1, EParamType.UnityAssetObject, "2D纹理:", typeof(Texture2D))]
#pragma warning restore CS0618 // 类型或成员已过时
        [ScriptParams(2, EParamType.Vector2, "偏移量:")]
        #endregion，
        SetCursorTexture,

        #region 获取风区属性
        /// <summary>
        /// 获取风区属性
        /// </summary>
        [ScriptName("获取风区属性", nameof(GetWindZoneProperty))]
        [ScriptDescription("获取风区属性；")]
        [ScriptReturn("成功返回 风区对应属性值；失败返回 #False")]
        [ScriptParams(1, EParamType.GameObjectComponent, "风区:", typeof(WindZone))]
        [ScriptParams(2, EParamType.Combo, "类型:", "模式", "半径", "主风力", "湍流", "脉冲强度", "脉冲频率")]
        #endregion，
        GetWindZoneProperty,

        #region 设置风区属性
        /// <summary>
        /// 设置风区属性
        /// </summary>
        [ScriptName("设置风区属性", nameof(SetWindZoneProperty))]
        [ScriptDescription("设置风区属性；")]
        [ScriptReturn("成功返回 #True；失败返回 #False")]
        [ScriptParams(1, EParamType.GameObjectComponent, "风区:", typeof(WindZone))]
        [ScriptParams(2, EParamType.Combo, "类型:", "模式", "半径", "主风力", "湍流", "脉冲强度", "脉冲频率")]
        [ScriptParams(3, EParamType.Combo, "模式:", nameof(WindZoneMode.Directional), nameof(WindZoneMode.Spherical))]
        [ScriptParams(4, EParamType.FloatSlider, "半径:", 0, 10000f, defaultObject = 20)]
        [ScriptParams(5, EParamType.FloatSlider, "主风力:", 0, 1000f, defaultObject = 1)]
        [ScriptParams(6, EParamType.FloatSlider, "湍流:", 0, 1000f, defaultObject = 1)]
        [ScriptParams(7, EParamType.FloatSlider, "脉冲强度:", 0, 1000f, defaultObject = 0.5f)]
        [ScriptParams(8, EParamType.FloatSlider, "脉冲频率:", 0, 100f, defaultObject = 0.01f)]
        #endregion，
        SetWindZoneProperty,

        #endregion

        #region 粒子系统

        /// <summary>
        /// 粒子系统-目录
        /// </summary>
        [ScriptName("粒子系统", nameof(ParticleSystemOperation), EGrammarType.Category)]
        [ScriptDescription("粒子系统目录")]
        ParticleSystemOperation,

        #region 粒子系统控制
        /// <summary>
        /// 粒子系统控制
        /// </summary>
        [ScriptName("粒子系统控制", nameof(ControlParticleSystem))]
        [ScriptDescription("对粒子系统进行播放、暂停、停止和重置等播放控制；")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObjectComponent, "粒子系统:", typeof(ParticleSystem))]
        [ScriptParams(2, EParamType.Combo, "操作类型:", "播放", "暂停", "停止", "清空", "重置", defaultObject = "播放")]
        #endregion
        ControlParticleSystem,

        #region 粒子系统批量控制
        /// <summary>
        /// 粒子系统批量控制
        /// </summary>
        [ScriptName("粒子系统批量控制", nameof(ControlParticleSystemBatch))]
        [ScriptDescription("对游戏对象下的粒子系统批量的进行播放、暂停、停止和重置等进行控制；")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObject, "游戏对象:")]
        [ScriptParams(2, EParamType.Combo, "操作类型:", "播放", "暂停", "停止", "清空", "重置", defaultObject = "播放")]
        #endregion
        ControlParticleSystemBatch,

        #region 获取粒子系统信息
        /// <summary>
        /// 获取粒子系统信息
        /// </summary>
        [ScriptName("获取粒子系统信息", nameof(GetParticleSystemInfo))]
        [ScriptDescription("获取粒子系统信息；")]
        [ScriptReturn("成功返回 具体信息; 失败返回 #False")]
        [ScriptParams(1, EParamType.GameObjectComponent, "粒子系统:", typeof(ParticleSystem))]
        [ScriptParams(2, EParamType.Combo, "信息类型:", "是否循环", "是否发射", "是否播放", "是否暂停", "是否停止", defaultObject = "是否发射")]
        #endregion
        GetParticleSystemInfo,

        #endregion

        #region UGUI

        #region UGUI-目录
        /// <summary>
        /// UGUI
        /// </summary>
        [ScriptName(nameof(UGUI), nameof(UGUI), EGrammarType.Category)]
        [ScriptDescription("UGUI目录;")]
        #endregion
        UGUI,

        #region 设置UGUI游戏对象位置
        /// <summary>
        /// 设置UGUI游戏对象位置
        /// </summary>
        [ScriptName("设置UGUI游戏对象位置", nameof(UGUISetGameObjectPosition))]
        [ScriptDescription("设置UGUI游戏对象位置；屏幕坐标位置")]
        [ScriptReturn("成功返回  #True ;失败返回 #False;")]
        [ScriptParams(1, EParamType.GameObjectComponent, "UGUI组件对象:", typeof(RectTransform))]
        [ScriptParams(2, EParamType.Vector3, "屏幕坐标:")]
        [ScriptParams(3, EParamType.Combo, "坐标类型:", "本地坐标", "世界坐标")]
        #endregion
        UGUISetGameObjectPosition,

        #region 获取UGUI游戏对象位置
        /// <summary>
        /// 获取UGUI游戏对象位置
        /// </summary>
        [ScriptName("获取UGUI游戏对象位置", nameof(UGUIGetGameObjectPosition))]
        [ScriptDescription("获取UGUI游戏对象位置；屏幕坐标位置")]
        [ScriptReturn("成功返回  UGUI游戏对象位置 ;失败返回 #False;")]
        [ScriptParams(1, EParamType.GameObjectComponent, "UGUI组件对象:", typeof(RectTransform))]
        [ScriptParams(2, EParamType.Combo, "坐标类型:", "本地坐标", "世界坐标")]
        #endregion
        UGUIGetGameObjectPosition,

        #region 设置UGUI-Scrollbar数据值
        /// <summary>
        /// 设置UGUI-Scrollbar数据值
        /// </summary>
        [ScriptName("设置UGUI-Scrollbar数据值", nameof(UGUISetScrollbarValue))]
        [ScriptDescription("设置UGUI-Scrollbar数据值；数据值范围:[0 ,1] ;")]
        [ScriptReturn("成功返回 #True  ;失败返回 #False;")]
        [ScriptParams(1, EParamType.GameObjectComponent, "UGUI-Scrollbar组件对象:", typeof(Scrollbar))]
        [ScriptParams(2, EParamType.FloatSlider, "数据值范围([0 ,1]):", 0f, 1f)]
        #endregion
        UGUISetScrollbarValue,

        #region 获取UGUI-Scrollbar数据值
        /// <summary>
        /// 获取UGUI-Scrollbar数据值
        /// </summary>
        [ScriptName("获取UGUI-Scrollbar数据值", nameof(UGUIGetScrollbarValue))]
        [ScriptDescription("获取UGUI-Scrollbar数据值；数据值范围:[0 ,1] ;")]
        [ScriptReturn("成功返回 [0 ,1] 的数据值  ;失败返回 #False;")]
        [ScriptParams(1, EParamType.GameObjectComponent, "UGUI-Scrollbar组件对象:", "", typeof(Scrollbar))]
        #endregion
        UGUIGetScrollbarValue,

        #region 设置UGUI-Slider数据值
        /// <summary>
        /// 设置UGUI-Slider数据值
        /// </summary>
        [ScriptName("设置UGUI-Slider数据值", nameof(UGUISetSliderValue))]
        [ScriptDescription("设置UGUI-Slider数据值；数据值范围:[0 ,1] ;")]
        [ScriptReturn("成功返回 #True  ;失败返回 #False;")]
        [ScriptParams(1, EParamType.GameObjectComponent, "UGUI-Slider组件对象:", "", typeof(Slider))]
        [ScriptParams(2, EParamType.FloatSlider, "数据值范围([0 ,1]):", 0f, 1f)]
        #endregion
        UGUISetSliderValue,

        #region 获取UGUI-Slider数据值
        /// <summary>
        /// 获取UGUI-Slider数据值
        /// </summary>
        [ScriptName("获取UGUI-Slider数据值", nameof(UGUIGetSliderValue))]
        [ScriptDescription("获取UGUI-Slider数据值；数据值范围:[0 ,1] ;")]
        [ScriptReturn("成功返回 [0 ,1] 的数据值  ;失败返回 #False;")]
        [ScriptParams(1, EParamType.GameObjectComponent, "UGUI-Slider组件对象:", "", typeof(Slider))]
        #endregion
        UGUIGetSliderValue,

        #region 设置UGUI-Text文字
        /// <summary>
        /// 设置UGUI-Text文字
        /// </summary>
        [ScriptName("设置UGUI-Text文字", nameof(UGUISetTextValue))]
        [ScriptDescription("设置UGUI-Text文字内容")]
        [ScriptReturn("成功返回 #True  ;失败返回 #False;")]
        [ScriptParams(1, EParamType.GameObjectComponent, "UGUI-Text组件对象:", "", typeof(UnityEngine.UI.Text))]
        [ScriptParams(2, EParamType.String, "文本:")]
        #endregion
        UGUISetTextValue,

        #region 获取UGUI-Text文字
        /// <summary>
        /// 获取UGUI-Text文字
        /// </summary>
        [ScriptName("获取UGUI-Text文字", nameof(UGUIGetTextValue))]
        [ScriptDescription("获取UGUI-Text文字内容")]
        [ScriptReturn("成功返回Text文字内容  ;失败返回 #False;")]
        [ScriptParams(1, EParamType.GameObjectComponent, "UGUI-Text组件对象:", "", typeof(UnityEngine.UI.Text))]
        #endregion
        UGUIGetTextValue,

        #region 设置UGUI-InputField文本
        /// <summary>
        /// 设置UGUI-InputField文本
        /// </summary>
        [ScriptName("设置UGUI-InputField文本", nameof(UGUISetInputFieldText))]
        [ScriptDescription("设置UGUI-InputField文本内容")]
        [ScriptReturn("成功返回 #True ;失败返回 #False;")]
        [ScriptParams(1, EParamType.GameObjectComponent, "UGUI-InputField组件对象:", "", typeof(InputField))]
        [ScriptParams(2, EParamType.String, "文本:")]
        #endregion
        UGUISetInputFieldText,

        #region 获取UGUI-InputField文本
        /// <summary>
        /// 获取UGUI-InputField文本
        /// </summary>
        [ScriptName("获取UGUI-InputField文本", nameof(UGUIGetInputFieldText))]
        [ScriptDescription("获取UGUI-InputField文本内容")]
        [ScriptReturn("成功返回InputField文本内容  ;失败返回 #False;")]
        [ScriptParams(1, EParamType.GameObjectComponent, "UGUI-InputField组件对象:", "", typeof(InputField))]
        #endregion
        UGUIGetInputFieldText,

        #region 设置UGUI-Toggle状态值
        /// <summary>
        /// 设置UGUI-Toggle状态值
        /// </summary>
        [ScriptName("设置UGUI-Toggle状态值", nameof(UGUISetToggleOn))]
        [ScriptDescription("设置Toggle状态值")]
        [ScriptReturn("成功返回 #True ;失败返回 #False;")]
        [ScriptParams(1, EParamType.GameObjectComponent, "UGUI-Toggle组件:", typeof(UnityEngine.UI.Toggle))]
        [ScriptParams(2, EParamType.Bool, "是否选中:")]
        #endregion
        UGUISetToggleOn,

        #region 获取UGUI-Toggle状态值
        /// <summary>
        /// 获取UGUI-Toggle状态值
        /// </summary>
        [ScriptName("获取UGUI-Toggle状态值", nameof(UGUIGetToggleOn))]
        [ScriptDescription("获取Toggle状态值")]
        [ScriptReturn("成功返回 #True ;失败返回 #False;")]
        [ScriptParams(1, EParamType.GameObjectComponent, "UGUI-Toggle组件:", typeof(UnityEngine.UI.Toggle))]
        #endregion
        UGUIGetToggleOn,

        #region 获取UGUI控件交互性
        /// <summary>
        /// 设置UGUI控件交互性
        /// </summary>
        [ScriptName("设置UGUI控件交互性", nameof(UGUISetControlInteractable))]
        [ScriptDescription("设置UGUI控件交互性")]
        [ScriptReturn("成功返回 #True ;失败返回 #False;")]
        [ScriptParams(1, EParamType.GameObjectComponent, "UGUI可交互组件:", typeof(UnityEngine.UI.Selectable))]
        [ScriptParams(2, EParamType.Bool, "是否可交互:")]
        #endregion
        UGUISetControlInteractable,

        #region 获取UGUI控件交互性
        /// <summary>
        /// 获取UGUI控件交互性
        /// </summary>
        [ScriptName("获取UGUI控件交互性", nameof(UGUIGetControlInteractable))]
        [ScriptDescription("获取UGUI控件交互性")]
        [ScriptReturn("成功返回 #True ;失败返回 #False;")]
        [ScriptParams(1, EParamType.GameObjectComponent, "UGUI可交互组件:", typeof(UnityEngine.UI.Selectable))]
        #endregion
        UGUIGetControlInteractable,

        #region 清除UGUI-Dropdown选项
        /// <summary>
        /// 清除UGUI-Dropdown选项
        /// </summary>
        [ScriptName("清除UGUI-Dropdown选项", nameof(UGUIClearDropdownOptions))]
        [ScriptDescription("清除UGUI-Dropdown选项")]
        [ScriptReturn("成功返回 #True ;失败返回 #False;")]
        [ScriptParams(1, EParamType.GameObjectComponent, "UGUI-Dropdown组件对象:", typeof(UnityEngine.UI.Dropdown))]
        #endregion
        UGUIClearDropdownOptions,

        #region 设置UGUI-Dropdown选项
        /// <summary>
        /// 设置下拉列表选项
        /// </summary>
        [ScriptName("设置UGUI-Dropdown选项", nameof(UGUISetDropdownValue))]
        [ScriptDescription("设置UGUI-Dropdown选中项")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObjectComponent, "下拉列表:", typeof(Dropdown))]
        [ScriptParams(2, EParamType.IntSlider, "选项[1, 选项数目]:", 1, 99)]
        #endregion
        UGUISetDropdownValue,

        #region 设置UGUI控件以及子控件交互性
        /// <summary>
        /// 设置UGUI控件以及子控件交互性
        /// </summary>
        [ScriptName("设置UGUI控件以及子控件交互性", nameof(UGUISetInteractableIncludeChildren))]
        [ScriptDescription("设置一个根节点，递归设置某个类型的子节点UGUI控件的交互性")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObjectComponent, "UGUI根节点:", typeof(RectTransform))]
        [ScriptParams(2, EParamType.Combo, "UGUI控件类型：", "Selectable", "Button", "Toggle", "Scrollbar", "Slider")]
        [ScriptParams(3, EParamType.Bool, "是否可交互：")] //,defaultObject =EBool.Yes
        #endregion
        UGUISetInteractableIncludeChildren,

        [ScriptName("如果指针指向UGUI游戏对象", nameof(UGUIIfIsPointerOverGameObject), EGrammarType.If)]
        [ScriptDescription("判断当前指针(包括鼠标，触摸点等)是否在UGUI游戏对象上")]
        [ScriptReturn("成功返回 #True ;失败返回 #False;")]
        UGUIIfIsPointerOverGameObject,

        [ScriptName("否则如果指针指向UGUI游戏对象", nameof(UGUIElseIfIsPointerOverGameObject), EGrammarType.ElseIf)]
        [ScriptDescription("判断当前指针(包括鼠标，触摸点等)是否在UGUI游戏对象上")]
        [ScriptReturn("成功返回 #True ;失败返回 #False;")]
        UGUIElseIfIsPointerOverGameObject,

        [ScriptName("获取指针是否指向UGUI游戏对象", nameof(UGUIGetIsPointerOverGameObject))]
        [ScriptDescription("判断当前指针(包括鼠标，触摸点等)是否在UGUI游戏对象上")]
        [ScriptReturn("如果指针指向UGUI游戏对象则返回 #True ;否则返回 #False;")]
        UGUIGetIsPointerOverGameObject,

        #region 设置UGUI-Button点击
        /// <summary>
        /// 设置UGUI-Button点击
        /// </summary>
        [ScriptName("设置UGUI-Button点击", nameof(UGUISetClickButton))]
        [ScriptDescription("设置UGUI-Button点击")]
        [ScriptReturn("成功返回 #True ;失败返回 #False;")]
        [ScriptParams(1, EParamType.GameObjectComponent, "UGUI-Button组件:", typeof(UnityEngine.UI.Button))]
        #endregion
        UGUISetClickButton,

        #endregion

        #region Unity游戏对象

        #region Unity游戏对象-目录
        /// <summary>
        /// Unity游戏对象
        /// </summary>
        [ScriptName("Unity游戏对象", nameof(UnityGameObject), EGrammarType.Category)]
        [ScriptDescription("Unity游戏对象脚本的目录;")]
        #endregion
        UnityGameObject,

        #region 游戏对象查找
        /// <summary>
        /// 游戏对象查找
        /// </summary>
        [ScriptName("游戏对象查找", nameof(GameObjectFind))]
        [ScriptDescription("游戏对象查找；")]
        [ScriptReturn("成功找到返回 游戏对象形式的字符串 ;失败返回 #False;")]
        [ScriptParams(1, EParamType.GameObject, "游戏对象：")]
        #endregion
        GameObjectFind,

        #region 游戏对象递归调用自定义函数
        /// <summary>
        /// 游戏对象递归调用自定义函数
        /// </summary>
        [ScriptName("游戏对象递归调用自定义函数", nameof(GameObjectRecursionCallUserDefineFun))]
        [ScriptDescription("游戏对象递归调用自定义函数；会将传入的游戏对象递归（包含全部子级游戏对象）遍历直接调用指定的自定义函数；会将游戏对象信息以参数的方式传入到自定义函数中；注意，如果传入的游戏对象子节点过多或自定以函数逻辑复杂，执行效率会受影响；")]
        [ScriptReturn("成功返回 #True ;失败返回 #False;")]
        [ScriptParams(1, EParamType.GameObject, "游戏对象：")]
        [ScriptParams(2, EParamType.UserDefineFun, "自定义函数（参数为对应的游戏对象信息）：")]
        [ScriptParams(3, EParamType.Bool, "是否包括未激活的游戏对象：")]
        #endregion
        GameObjectRecursionCallUserDefineFun,

        #region 游戏对象成员调用自定义函数
        /// <summary>
        /// 游戏对象成员调用自定义函数
        /// </summary>
        [ScriptName("游戏对象成员调用自定义函数", nameof(GameObjectChildrenCallUserDefineFun))]
        [ScriptDescription("游戏对象成员调用自定义函数；会将传入的游戏对象所有成员子节点（不包括子节点的子节点）遍历并直接调用指定的自定义函数；会将游戏对象信息以参数的方式传入到自定义函数中；注意，如果传入的游戏对象子节点过多或自定以函数逻辑复杂，执行效率会受影响；")]
        [ScriptReturn("成功返回 #True ;失败返回 #False;")]
        [ScriptParams(1, EParamType.GameObject, "游戏对象：")]
        [ScriptParams(2, EParamType.UserDefineFun, "自定义函数（参数为对应的游戏对象信息）：")]
        #endregion
        GameObjectChildrenCallUserDefineFun,

        #region 获取游戏对象成员数目
        /// <summary>
        /// 获取游戏对象成员数目
        /// </summary>
        [ScriptName("获取游戏对象成员数目", nameof(GetGameObjectChildrenCount))]
        [ScriptReturn("成功返回 游戏对象成员数目 ;失败返回 #False;")]
        [ScriptParams(1, EParamType.GameObject, "游戏对象：")]
        #endregion
        GetGameObjectChildrenCount,

        #region 获取游戏对象成员(通过索引)
        /// <summary>
        /// 获取游戏对象成员(通过索引)
        /// </summary>
        [ScriptName("获取游戏对象成员(通过索引)", nameof(GetGameObjectChildrenWithIndex))]
        [ScriptReturn("成功返回 游戏对象成员数目 ;失败返回 #False;")]
        [ScriptParams(1, EParamType.GameObject, "游戏对象：")]
        [ScriptParams(2, EParamType.IntSlider, "索引[1,获取游戏对象成员数目]：", 1, 999, defaultObject = 1)]
        #endregion
        GetGameObjectChildrenWithIndex,

        #region 游戏对象显示隐藏
        /// <summary>
        /// 游戏对象显示隐藏
        /// </summary>
        [ScriptName("游戏对象显示隐藏", nameof(GameObjectShowHide))]
        [ScriptDescription("游戏对象显示隐藏；通过设置游戏对象的Renderer组件是否启用，控制游戏对象显示隐藏；")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObject, "游戏对象:")]
        [ScriptParams(2, EParamType.Combo, "显示隐藏:", "隐藏", "显示", "切换")]
        #endregion
        GameObjectShowHide,

        #region 游戏对象激活
        /// <summary>
        /// 游戏对象激活
        /// </summary>
        [ScriptName("游戏对象激活", nameof(GameObjectActive))]
        [ScriptDescription("游戏对象激活；通过设置游戏对象自身是否激活，控制游戏对象是否起作用；")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObject, "游戏对象:")]
        [ScriptParams(2, EParamType.Combo, "是否激活:", "是", "否", "切换")]
        #endregion
        GameObjectActive,

        #region 设置游戏对象激活
        /// <summary>
        /// 设置游戏对象激活
        /// </summary>
        [ScriptName("设置游戏对象激活", nameof(SetGameObjectActive))]
        [ScriptDescription("设置游戏对象激活；通过设置游戏对象自身是否激活，控制游戏对象是否起作用；")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObject, "游戏对象:")]
        [ScriptParams(2, EParamType.Combo, "是否激活:", "是", "否", "切换")]
        #endregion
        SetGameObjectActive,

        [ScriptName("游戏对象激活闪烁", nameof(GameObjectActiveFlash))]
        [ScriptDescription("游戏对象激活闪烁；通过设置游戏对象自身是否激活，控制游戏对象是否起作用以实现闪烁效果；激活闪烁结束后恢复游戏对象激活状态；")]
        [ScriptParams(1, EParamType.GameObject, "游戏对象:")]
        [ScriptParams(2, EParamType.Float, "频率:", defaultObject = 2f)]
        [ScriptParams(3, EParamType.Float, "时长（单位：秒）:", defaultObject = 3f)]
        GameObjectActiveFlash,

        #region 子级游戏对象激活
        [ScriptName("子级游戏对象激活", nameof(SubLevelGameObjectActive))]
        [ScriptDescription("子级游戏对象激活；通过设置子级游戏对象自身是否激活，控制子级游戏对象是否起作用；")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObject, "游戏对象:")]
        [ScriptParams(2, EParamType.String, "子级游戏对象:")]
        [ScriptParams(3, EParamType.Combo, "是否激活:", "是", "否", "切换")]
        #endregion
        SubLevelGameObjectActive,

        #region 设置子级游戏对象激活
        [ScriptName("设置子级游戏对象激活", nameof(SetSubLevelGameObjectActive))]
        [ScriptDescription("设置子级游戏对象激活；通过设置子级游戏对象自身是否激活，控制子级游戏对象是否起作用；")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObject, "游戏对象:")]
        [ScriptParams(2, EParamType.String, "子级游戏对象:")]
        [ScriptParams(3, EParamType.Combo, "是否激活:", "是", "否", "切换")]
        #endregion
        SetSubLevelGameObjectActive,

        #region 父级游戏对象激活
        [ScriptName("父级游戏对象激活", nameof(ParentLevelGameObjectActive))]
        [ScriptDescription("父级游戏对象激活；通过设置父级游戏对象自身是否激活，控制父级游戏对象是否起作用；")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObject, "游戏对象:")]
        [ScriptParams(2, EParamType.String, "父级游戏对象名(对象的父节点的同级游戏对象名,不填写默认为控制当前游戏对象的直系父游戏对象):")]
        [ScriptParams(3, EParamType.Bool, "是否激活:")]
        #endregion
        ParentLevelGameObjectActive,

        #region 设置父级游戏对象激活
        [ScriptName("设置父级游戏对象激活", nameof(SetParentLevelGameObjectActive))]
        [ScriptDescription("设置父级游戏对象激活；通过设置父级游戏对象自身是否激活，控制父级游戏对象是否起作用；")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObject, "游戏对象:")]
        [ScriptParams(2, EParamType.String, "父级游戏对象名(对象的父节点的同级游戏对象名,不填写默认为控制当前游戏对象的直系父游戏对象):")]
        [ScriptParams(3, EParamType.Bool, "是否激活:")]
        #endregion
        SetParentLevelGameObjectActive,

        #region 同级游戏对象激活
        [ScriptName("同级游戏对象激活", nameof(SameLevelGameObjectActive))]
        [ScriptDescription("同级游戏对象激活；通过设同级游戏对象自身是否激活，控制同级游戏对象是否起作用；")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObject, "游戏对象:")]
        [ScriptParams(2, EParamType.String, "同级游戏对象名(游戏对象的同级节点游戏对象名，不填写默认为控制当前游戏对象):")]
        [ScriptParams(3, EParamType.Bool, "是否激活:")]
        #endregion
        SameLevelGameObjectActive,

        #region 设置同级游戏对象激活
        [ScriptName("设置同级游戏对象激活", nameof(SetSameLevelGameObjectActive))]
        [ScriptDescription("设置同级游戏对象激活；通过设同级游戏对象自身是否激活，控制同级游戏对象是否起作用；")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObject, "游戏对象:")]
        [ScriptParams(2, EParamType.String, "同级游戏对象名(游戏对象的同级节点游戏对象名，不填写默认为控制当前游戏对象):")]
        [ScriptParams(3, EParamType.Bool, "是否激活:")]
        #endregion
        SetSameLevelGameObjectActive,

        #region 获取游戏对象名称
        /// <summary>
        /// 获取游戏对象名称
        /// </summary>
        [ScriptName("获取游戏对象名称", nameof(GetGameObjectName))]
        [ScriptDescription("获取游戏对象名称；即去除游戏对象的路径信息后，只保留游戏对象的名称；")]
        [ScriptReturn("成功返回 游戏对象名称 ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObject, "游戏对象:")]
        #endregion
        GetGameObjectName,

        #region 游戏对象改名
        /// <summary>
        /// 游戏对象改名
        /// </summary>
        [ScriptName("游戏对象改名", nameof(GameObjectRename))]
        [ScriptDescription("游戏对象改名；修改游戏对象的名称；")]
        [ScriptReturn("成功返回 游戏对象名称路径（即修改后的游戏对象描述字符串）; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObject, "游戏对象:")]
        [ScriptParams(2, EParamType.StandardString, "新名:")]
        #endregion
        GameObjectRename,

        #region 设置游戏对象名称
        /// <summary>
        /// 设置游戏对象名称
        /// </summary>
        [ScriptName("设置游戏对象名称", nameof(SetGameObjectName))]
        [ScriptDescription("设置游戏对象名称；修改游戏对象的名称；")]
        [ScriptReturn("成功返回 游戏对象名称路径（即修改后的游戏对象描述字符串）; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObject, "游戏对象:")]
        [ScriptParams(2, EParamType.StandardString, "新名:")]
        #endregion
        SetGameObjectName,

        #region 游戏对象克隆
        /// <summary>
        /// 游戏对象克隆
        /// </summary>
        [ScriptName("游戏对象克隆", nameof(GameObjectClone))]
        [ScriptDescription("游戏对象克隆；会在待克隆游戏对象位置克隆一个包括子级游戏对象以及完备组件的新游戏对象；")]
        [ScriptReturn("成功返回克隆后的 游戏对象名称路径（即修改后的游戏对象描述字符串） ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObject, "游戏对象:")]
        [ScriptParams(2, EParamType.StandardString, "克隆后的游戏对象名:")]
        [ScriptParams(3, EParamType.GameObject, "克隆后的父级游戏对象(如果为空，则克隆体与原游戏对象父级游戏对象相同):")]
        #endregion
        GameObjectClone,

        #region 游戏对象删除
        /// <summary>
        /// 游戏对象删除
        /// </summary>
        [ScriptName("游戏对象删除", nameof(GameObjectRemove))]
        [ScriptDescription("游戏对象删除；会将游戏对象本身以及所属的子级游戏对象全部删除掉；")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObject, "游戏对象:")]
        #endregion
        GameObjectRemove,

        #region 删除游戏对象子节点
        /// <summary>
        /// 删除游戏对象子节点
        /// </summary>
        [ScriptName("删除游戏对象子节点", nameof(GameObjectRemoveChild))]
        [ScriptDescription("删除游戏对象子节点；会将游戏对象所属的子级游戏对象全部删除掉，游戏对象本身不删除；")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObject, "游戏对象:")]
        [ScriptParams(2, EParamType.UserDefineFun, "删除完成后的执行函数:")]
        [ScriptParams(3, EParamType.String, "回调时携带的信息:")]
        #endregion
        GameObjectRemoveChild,

        #region 获取父级游戏对象名称
        /// <summary>
        /// 获取父级游戏对象名称
        /// </summary>
        [ScriptName("获取父级游戏对象名称", nameof(GetGameObjectParentName))]
        [ScriptDescription("获取父级游戏对象名称；不包括父级游戏对象的路径信息，仅返回该游戏对象的名称；")]
        [ScriptReturn("成功返回 父级游戏对象名称名称 ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObject, "游戏对象:")]
        #endregion
        GetGameObjectParentName,

        #region 获取游戏对象路径名称
        /// <summary>
        /// 获取游戏对象路径名称
        /// </summary>
        [ScriptName("获取游戏对象路径名称", nameof(GetGameObjectPath))]
        [ScriptDescription("获取游戏对象路径名称")]
        [ScriptReturn("成功返回 游戏对象名称路径 ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObject, "游戏对象:")]
        #endregion
        GetGameObjectPath,

        #region 游戏对象是否存在子节点
        /// <summary>
        /// 游戏对象是否存在子节点
        /// </summary>
        [ScriptName("游戏对象是否存在子节点", nameof(GameObjectExistChild))]
        [ScriptDescription("游戏对象是否存在子节点")]
        [ScriptReturn("是返回 #True ; 否返回 #False ;")]
        [ScriptParams(1, EParamType.GameObject, "游戏对象:")]
        #endregion
        GameObjectExistChild,

        #region 游戏对象平移
        /// <summary>
        /// 游戏对象平移
        /// </summary>
        [ScriptName("游戏对象平移", nameof(GameObjectOffset))]
        [ScriptDescription("游戏对象平移", "GameObjectOffset")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObject, "游戏对象:")]
        [ScriptParams(2, EParamType.Combo, "坐标系:", "世界坐标系", "自身坐标系")]
        [ScriptParams(3, EParamType.Vector3, "偏移量(X/Y/Z):")]
        #endregion
        GameObjectOffset,

        #region 游戏对象旋转
        /// <summary>
        /// 游戏对象旋转
        /// </summary>
        [ScriptName("游戏对象旋转", nameof(GameObjectRotate))]
        [ScriptDescription("游戏对象旋转")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObject, "游戏对象:")]
        [ScriptParams(2, EParamType.Combo, "坐标系:", "世界坐标系", "自身坐标系")]
        [ScriptParams(3, EParamType.Vector3, "旋转量(X/Y/Z):")]
        #endregion
        GameObjectRotate,

        #region 获取游戏对象位置
        /// <summary>
        /// 获取游戏对象位置
        /// </summary>
        [ScriptName("获取游戏对象位置", nameof(GetGameObjectPosition))]
        [ScriptDescription("获取游戏对象位置；")]
        [ScriptReturn("成功返回 #True  ;失败返回 #False;")]
        [ScriptParams(1, EParamType.GameObject, "游戏对象:")]
        [ScriptParams(2, EParamType.Combo, "坐标系:", "世界坐标系", "自身坐标系")]
        #endregion
        GetGameObjectPosition,

        #region 游戏对象位置
        /// <summary>
        /// 游戏对象位置
        /// </summary>
        [ScriptName("游戏对象位置", nameof(GameObjectPosition))]
        [ScriptDescription("游戏对象位置")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObject, "游戏对象:")]
        [ScriptParams(2, EParamType.Combo, "坐标系:", "世界坐标系", "自身坐标系")]
        [ScriptParams(3, EParamType.Vector3, "位置(X/Y/Z):")]
        #endregion
        GameObjectPosition,

        #region 设置游戏对象位置
        /// <summary>
        /// 设置游戏对象位置
        /// </summary>
        [ScriptName("设置游戏对象位置", nameof(SetGameObjectPosition))]
        [ScriptDescription("设置游戏对象位置")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObject, "游戏对象:")]
        [ScriptParams(2, EParamType.Combo, "坐标系:", "世界坐标系", "自身坐标系")]
        [ScriptParams(3, EParamType.Vector3, "位置(X/Y/Z):")]
        #endregion
        SetGameObjectPosition,

        #region 获取游戏对象欧拉角度
        /// <summary>
        /// 获取游戏对象欧拉角度
        /// </summary>
        [ScriptName("获取游戏对象欧拉角度", nameof(GetGameObjectEularAngle))]
        [ScriptDescription("获取游戏对象欧拉角度;返回指定游戏对象的欧拉角（即游戏对象的transform.eulerAngles）**注意非Local欧拉角** ")]
        [ScriptReturn("成功返回 游戏对象欧拉角度 ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObject, "游戏对象:")]
        [ScriptParams(3, EParamType.Combo, "坐标系:", "世界坐标系", "自身坐标系")]
        #endregion
        GetGameObjectEularAngle,

        #region 设置游戏对象欧拉角度
        /// <summary>
        /// 设置游戏对象欧拉角度
        /// </summary>
        [ScriptName("设置游戏对象欧拉角度", nameof(SetGameObjectEularAngle))]
        [ScriptDescription("设置游戏对象欧拉角度;通过设置模型的欧拉角（即游戏对象的transform.eulerAngles）完成设置")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObject, "游戏对象:")]
        [ScriptParams(2, EParamType.Vector3, "欧拉角(X/Y/Z):")]
        [ScriptParams(3, EParamType.Combo, "坐标系:", "世界坐标系", "自身坐标系")]
        #endregion
        SetGameObjectEularAngle,

        #region 游戏对象朝向
        /// <summary>
        /// 游戏对象朝向
        /// </summary>
        [ScriptName("游戏对象朝向", nameof(GameObjectLookAt))]
        [ScriptDescription("游戏对象朝向；旋转游戏对象使z轴指向目标方向，即注视目标点；")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObject, "游戏对象:")]
        [ScriptParams(2, EParamType.Vector3, "朝向的坐标(X/Y/Z):")]
        [ScriptParams(3, EParamType.Vector3, "游戏对象朝向的世界坐标上方向(X/Y/Z):", defaultObject = "0/1/0")]
        #endregion
        GameObjectLookAt,

        #region 设置游戏对象朝向
        /// <summary>
        /// 设置游戏对象朝向
        /// </summary>
        [ScriptName("设置游戏对象朝向", nameof(SetGameObjectLookAt))]
        [ScriptDescription("设置游戏对象朝向；旋转游戏对象使z轴指向目标方向，即注视目标点；")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObject, "游戏对象:")]
        [ScriptParams(2, EParamType.Vector3, "朝向的坐标(X/Y/Z):")]
        [ScriptParams(3, EParamType.Vector3, "游戏对象朝向的世界坐标上方向(X/Y/Z):", defaultObject = "0/1/0")]
        #endregion
        SetGameObjectLookAt,

        #region 获取游戏对象缩放值
        /// <summary>
        /// 获取游戏对象缩放值
        /// </summary>
        [ScriptName("获取游戏对象缩放值", nameof(GetGameObjectScale))]
        [ScriptDescription("获取游戏对象缩放值；")]
        [ScriptReturn("成功返回 游戏对象本地缩放系数(X/Y/Z) ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObject, "游戏对象:")]
        #endregion
        GetGameObjectScale,

        #region 设置游戏对象缩放
        /// <summary>
        /// 设置游戏对象缩放
        /// </summary>
        [ScriptName("设置游戏对象缩放", nameof(SetGameObjectScale))]
        [ScriptDescription("设置游戏对象缩放；")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObject, "游戏对象:")]
        [ScriptParams(2, EParamType.Vector3, "缩放系数(X/Y/Z):", defaultObject = "1/1/1")]
        #endregion
        SetGameObjectScale,

        #region 游戏对象朝向游戏对象
        /// <summary>
        /// 游戏对象朝向游戏对象
        /// </summary>
        [ScriptName("游戏对象朝向游戏对象", nameof(GameObjectLookAtGameObject))]
        [ScriptDescription("游戏对象朝向游戏对象；旋转游戏对象使z轴指向目标游戏对象，即注视目标游戏对象；")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObject, "游戏对象:")]
        [ScriptParams(2, EParamType.GameObject, "朝向的目标游戏对象:")]
        [ScriptParams(3, EParamType.Vector3, "游戏对象朝向的世界坐标上方向(X/Y/Z):", defaultObject = "0/1/0")]
        #endregion
        GameObjectLookAtGameObject,

        #region 游戏对象重合
        [ScriptName("游戏对象重合", nameof(GameObjectOverlap))]
        [ScriptDescription("游戏对象重合；将 待重合的游戏对象 与 目标对象 的世界坐标、旋转信息设置为相等；")]
        [ScriptReturn("成功返回 #True  ;失败返回 #False;")]
        [ScriptParams(1, EParamType.GameObject, "待重合的游戏对象:")]
        [ScriptParams(2, EParamType.GameObject, "目标对象:")]
        #endregion
        GameObjectOverlap,

        #region 设置游戏对象重合
        [ScriptName("设置游戏对象重合", nameof(SetGameObjectOverlap))]
        [ScriptDescription("设置游戏对象重合；将 待重合的游戏对象 与 目标对象 的世界坐标、旋转信息设置为相等；")]
        [ScriptReturn("成功返回 #True  ;失败返回 #False;")]
        [ScriptParams(1, EParamType.GameObject, "待重合的游戏对象:")]
        [ScriptParams(2, EParamType.GameObject, "目标对象:")]
        #endregion
        SetGameObjectOverlap,

        #region 设置游戏对象颜色
        /// <summary>
        /// 设置游戏对象颜色
        /// </summary>
        [ScriptName("设置游戏对象颜色", nameof(SetGameObjectColor))]
        [ScriptDescription("设置游戏对象材质的颜色，如果设置了包含子游戏对象，则同时修改子游戏对象的颜色")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObject, "游戏对象:")]
        [ScriptParams(2, EParamType.Color, "颜色:", defaultObject = "255/255/255/255")]
        [ScriptParams(3, EParamType.Bool2, "是否包含子级游戏对象:", defaultObject = EBool2.Yes)]
        #endregion
        SetGameObjectColor,

        #region 设置游戏对象父节点
        /// <summary>
        /// 设置游戏对象父节点
        /// </summary>
        [ScriptName("设置游戏对象父节点", nameof(SetGameObjectParent))]
        [ScriptDescription("设置游戏对象的父节点，即修改游戏对象的层级关系；如果输入的 父游戏对象 为 当前游戏对象 的子级（或子级的子级）游戏对象时，那么不调整层级关系，仍然返回当前游戏对象的名称路径；")]
        [ScriptReturn("成功返回 游戏对象名称路径（即修改后的游戏对象描述字符串） ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObject, "当前游戏对象(即期望修改后的子级游戏对象):")]
        [ScriptParams(2, EParamType.GameObject, "父级游戏对象(如果为空，则将当前游戏对象设置为根节点游戏对象):")]
        #endregion
        SetGameObjectParent,

        #region 获取游戏对象场景树层级索引
        /// <summary>
        /// 获取游戏对象场景树层级索引
        /// </summary>
        [ScriptName("获取游戏对象场景树层级索引", nameof(GetGameObjectSiblingIndex))]
        [ScriptDescription("获取游戏对象场景树层级索引；")]
        [ScriptReturn("成功返回 游戏对象场景树层级索引编号 ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObject, "游戏对象:")]
        #endregion
        GetGameObjectSiblingIndex,

        #region 设置游戏对象场景树层级索引
        /// <summary>
        /// 设置游戏对象场景树层级索引
        /// </summary>
        [ScriptName("设置游戏对象场景树层级索引", nameof(SetGameObjectSiblingIndex))]
        [ScriptDescription("设置游戏对象场景树层级索引；")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObject, "游戏对象:")]
        [ScriptParams(2, EParamType.Combo, "类型:", "编号", "第一个", "最后一个")]
        [ScriptParams(3, EParamType.IntSlider, "编号:", 0, 10000)]
        #endregion
        SetGameObjectSiblingIndex,

        #region 获取游戏对象层索引
        /// <summary>
        /// 获取游戏对象层索引
        /// </summary>
        [ScriptName("获取游戏对象层索引", nameof(GetGameObjectLayerID))]
        [ScriptDescription("获取游戏对象层索引;")]
        [ScriptReturn("成功返回 游戏对象层数字ID ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObject, "游戏对象:")]
        #endregion
        GetGameObjectLayerID,

        #region 设置游戏对象层索引
        /// <summary>
        /// 设置游戏对象层索引
        /// </summary>
        [ScriptName("设置游戏对象层索引", nameof(SetGameObjectLayerID))]
        [ScriptDescription("设置游戏对象层索引;")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObject, "游戏对象:")]
        [ScriptParams(2, EParamType.IntSlider, "层ID:", 0, 31)]
        [ScriptParams(3, EParamType.Bool2, "包含子对象:", defaultObject = EBool2.Yes)]
        #endregion 
        SetGameObjectLayerID,

        #region 获取游戏对象层名称
        /// <summary>
        /// 获取游戏对象层名称
        /// </summary>
        [ScriptName("获取游戏对象层名称", nameof(GetGameObjectLayerName))]
        [ScriptDescription("获取游戏对象层名称;")]
        [ScriptReturn("成功返回 游戏对象层名称 ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObject, "游戏对象:")]
        #endregion 
        GetGameObjectLayerName,

        #region 设置游戏对象层名称
        /// <summary>
        /// 设置游戏对象层名称
        /// </summary>
        [ScriptName("设置游戏对象层名称", nameof(SetGameObjectLayerName))]
        [ScriptDescription("设置游戏对象层名称;")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObject, "游戏对象:")]
        [ScriptParams(2, EParamType.String, "层名称:")]
        [ScriptParams(3, EParamType.Bool2, "包含子对象:", defaultObject = EBool2.Yes)]
        #endregion
        SetGameObjectLayerName,

        #region 获取游戏对象标签名称
        /// <summary>
        /// 获取游戏对象标签名称
        /// </summary>
        [ScriptName("获取游戏对象标签名称", nameof(GetGameObjectTagName))]
        [ScriptDescription("获取游戏对象标签名称;")]
        [ScriptReturn("成功返回 游戏对象标签名称 ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObject, "游戏对象:")]
        #endregion 
        GetGameObjectTagName,

        #region 设置游戏对象标签名称
        /// <summary>
        /// 设置游戏对象标签名称
        /// </summary>
        [ScriptName("设置游戏对象标签名称", nameof(SetGameObjectTagName))]
        [ScriptDescription("设置游戏对象标签名称;")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObject, "游戏对象:")]
        [ScriptParams(2, EParamType.String, "层名称:")]
        [ScriptParams(3, EParamType.Bool2, "包含子对象:", defaultObject = EBool2.Yes)]
        #endregion
        SetGameObjectTagName,

        #endregion

        #region Unity组件

        #region Unity组件-目录
        /// <summary>
        /// Unity组件
        /// </summary>
        [ScriptName("Unity组件", nameof(UnityComponent), EGrammarType.Category)]
        [ScriptDescription("Unity组件操作的目录;")]
        #endregion
        UnityComponent,

        #region 组件添加
        /// <summary>
        /// 添加组件
        /// </summary>
        [ScriptName("组件添加", nameof(ComponentAdd))]
        [ScriptDescription("组件添加；如果组件已经存在 不允许重复添加；")]
        [ScriptReturn("成功返回 游戏对象组件形式的字符串 ;失败返回 #False;")]
        [ScriptParams(1, EParamType.GameObject, "游戏对象：")]
        [ScriptParams(2, EParamType.ComponentType, "组件类型：")]
        #endregion
        ComponentAdd,

        #region 组件删除
        /// <summary>
        /// 组件删除
        /// </summary>
        [ScriptName("组件删除", nameof(ComponentRemove))]
        [ScriptDescription("组件删除；")]
        [ScriptReturn("成功返回 #True ;失败返回 #False;")]
        [ScriptParams(1, EParamType.GameObjectComponent, "游戏对象组件：")]
        [ScriptParams(2, EParamType.UserDefineFun, "删除完成后的回调函数：")]
        [ScriptParams(3, EParamType.String, "额外参数：")]
        #endregion
        ComponentRemove,

        #region 组件查找
        /// <summary>
        /// 组件查找
        /// </summary>
        [ScriptName("组件查找", nameof(ComponentFind))]
        [ScriptDescription("组件查找；获取指定游戏对象上某类型的组件对象；")]
        [ScriptReturn("成功返回 游戏对象组件形式的字符串 ;失败返回 #False;")]
        [ScriptParams(1, EParamType.GameObject, "游戏对象：")]
        [ScriptParams(2, EParamType.ComponentType, "组件类型：")]
        #endregion
        ComponentFind,

        #region 组件激活
        /// <summary>
        /// 组件激活
        /// </summary>
        [ScriptName("组件激活", nameof(ComponentActive))]
        [ScriptDescription("组件激活；")]
        [ScriptReturn("成功返回 #True ;失败返回 #False;")]
        [ScriptParams(1, EParamType.GameObjectComponent, "游戏对象组件：")]
        [ScriptParams(2, EParamType.Bool, "是否激活:")]
        #endregion
        ComponentActive,

        #region 组件递归激活
        /// <summary>
        /// 组件递归激活
        /// </summary>
        [ScriptName("组件递归激活", nameof(ComponentRecursionActive))]
        [ScriptDescription("组件递归激活；会将传入的游戏对象以及子级游戏对象的所有指定类型的组件全部进行对应的激活操作；")]
        [ScriptReturn("成功返回 #True ;失败返回 #False;")]
        [ScriptParams(1, EParamType.GameObject, "游戏对象：")]
        [ScriptParams(2, EParamType.ComponentType, "组件类型：")]
        [ScriptParams(3, EParamType.Bool, "是否激活:")]
        [ScriptParams(4, EParamType.Bool, "是否包括未激活的游戏对象：")]
        #endregion
        ComponentRecursionActive,

        #region 组件递归调用自定义函数
        /// <summary>
        /// 组件递归调用自定义函数
        /// </summary>
        [ScriptName("组件递归调用自定义函数", nameof(ComponentRecursionCallUserDefineFun))]
        [ScriptDescription("组件递归调用自定义函数；会将传入的游戏对象以及子级游戏对象的所有指定类型的组件，然后直接调用指定的自定义函数；会将组件信息以参数的方式传入到自定义函数中；注意，如果传入的游戏对象子节点过多或自定以函数逻辑复杂，执行效率会受影响；")]
        [ScriptReturn("成功返回 #True ;失败返回 #False;")]
        [ScriptParams(1, EParamType.GameObject, "游戏对象：")]
        [ScriptParams(2, EParamType.ComponentType, "组件类型：")]
        [ScriptParams(3, EParamType.UserDefineFun, "自定义函数（参数为对应的组件信息）：")]
        [ScriptParams(4, EParamType.Bool, "是否包括未激活的游戏对象：")]
        #endregion
        ComponentRecursionCallUserDefineFun,

        #region 获取组件的成员变量值
        /// <summary>
        /// 获取组件的成员变量值
        /// </summary>
        [ScriptName("获取组件的成员变量值", nameof(ComponentMemberGetValue))]
        [ScriptDescription("获取组件的成员变量值；反射机制完成；**谨慎使用**")]
        [ScriptReturn("成功返回 #True ;失败返回 #False;")]
        [ScriptParams(1, EParamType.GameObjectComponent, "游戏对象组件：")]
        [ScriptParams(2, EParamType.StandardString, "成员变量名：")]
        #endregion
        ComponentMemberGetValue,

        #region 设置组件的成员变量值
        /// <summary>
        /// 设置组件的成员变量值
        /// </summary>
        [ScriptName("设置组件的成员变量值", nameof(ComponentMemberSetValue))]
        [ScriptDescription("设置组件的成员变量值；反射机制完成；成员变量值类型如果为空字符串时，会尝试使用成员变量名对应的类型自动尝试，但执行效率低；")]
        [ScriptReturn("成功返回 #True ;失败返回 #False;")]
        [ScriptParams(1, EParamType.GameObjectComponent, "游戏对象组件：")]
        [ScriptParams(2, EParamType.StandardString, "成员变量名：")]
        [ScriptParams(3, EParamType.String, "成员变量新值：")]
        [ScriptParams(4, EParamType.Combo, "成员变量值类型:", "", "string", "bool", "int", "long", "float", "double", "Vector2Int", "Vector3Int", "Vector2", "Vector3", "Vector4", "Rect", "Bounds", "Color", "GameObject", "GameObjectComponent")]
        #endregion
        ComponentMemberSetValue,

        [ScriptName("获取组件信息", nameof(GetComponentInfo))]
        [ScriptDescription("获取组件信息；")]
        [ScriptReturn("成功返回 具体信息 ;失败返回 #False;")]
        [ScriptParams(1, EParamType.GameObjectComponent, "游戏对象组件：")]
        [ScriptParams(2, EParamType.Combo, "信息：", "组件路径名称", "组件类型", "游戏对象路径名称", "游戏对象名称", "父级游戏对象路径名称", "父级游戏对象名称", "可用性")]
        GetComponentInfo,

        #endregion

        #region Unity资源对象

        #region Unity资源对象-目录
        /// <summary>
        /// Unity资源对象-目录
        /// </summary>
        [ScriptName("Unity资源对象", nameof(UnityAssetObject), EGrammarType.Category)]
        [ScriptDescription("Unity资源对象脚本目录；")]
        #endregion
        UnityAssetObject,

        #region 加载Unity资源对象
        /// <summary>
        /// 加载Unity资源对象
        /// </summary>
        [ScriptName("加载Unity资源对象", nameof(LoadUnityAssetObject))]
        [ScriptDescription("加载Unity资源对象；如果期望的资源对象类型存在期望的资源对象名称的资源对象，则不再加载文件；如资源对象为空，则尝试从本地文件路径下加载资源；会根据传入的资源文件为AssetBundle压缩文件或对应类型的实体文件，程序底层会根据加载情况自动识别，之后根据期望类型进行加载；推荐使用AssetBundle压缩文件，因为可以加载全部类型的Unity资源对象；使用对应类型的实体文件仅可以加载纹理、声音资源；")]
        [ScriptReturn("成功返回 #True ，不管加载结果如何都会执行回调函数; 文件无法找到时返回 #False ，并且不在执行回调;")]
        [ScriptParams(0, EParamType.UnityAssetObjectType, "期望的资源对象类型(*必选*):")]
        [ScriptParams(1, EParamType.String, "期望的资源对象名称(如为空，跳过存在性检测阶段而直接进行加载):")]
        [ScriptParams(2, EParamType.File, "存储资源对象的本地磁盘资源文件路径或http开头的网络路径:")]
        [ScriptParams(4, EParamType.String, "AssetBundle文件中资源路径名:")]
        [ScriptParams(5, EParamType.GlobalVariableName, "加载资源文件后存储资源对象信息的变量:")]
        [ScriptParams(6, EParamType.UserDefineFun, "资源加载完成后的回调函数:")]
        [ScriptParams(7, EParamType.String, "回调函数的额外信息(如果文件加载失败本信息会被替换为 #False):")]
        [ScriptParams(8, EParamType.Combo, "路径加载规则(网络路径时,忽略本项；如本参数无效，默认为智能加载模式):", "绝对路径加载", "智能加载")]
        #endregion
        LoadUnityAssetObject,

        #region 销毁Unity资源对象
        /// <summary>
        /// 销毁Unity资源对象
        /// </summary>
        [ScriptName("销毁Unity资源对象", nameof(DestroyUnityAssetObject))]
        [ScriptDescription("销毁Unity资源对象；将资源对象从场景中销毁；")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
#pragma warning disable CS0618 // 类型或成员已过时
        [ScriptParams(1, EParamType.UnityAssetObject, "资源对象信息:")]
#pragma warning restore CS0618 // 类型或成员已过时
        [ScriptParams(2, EParamType.Combo, "销毁规则:", "智能销毁", "立即销毁")]
        #endregion
        DestroyUnityAssetObject,

        #region 获取文本资源对象文本信息
        /// <summary>
        /// 获取文本资源对象文本信息
        /// </summary>
        [ScriptName("获取文本资源对象文本信息", nameof(GetTextAssetObjectTextInfo))]
        [ScriptDescription("获取文本资源对象的文本信息；")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
#pragma warning disable CS0618 // 类型或成员已过时
        [ScriptParams(1, EParamType.UnityAssetObject, "文本资源对象信息:", typeof(TextAsset))]
#pragma warning restore CS0618 // 类型或成员已过时
        [ScriptParams(2, EParamType.GlobalVariableName, "存储文本资源对象文本信息的变量:")]
        #endregion
        GetTextAssetObjectTextInfo,

        #region 替换游戏对象主纹理
        /// <summary>
        /// 替换游戏对象主纹理
        /// </summary>
        [ScriptName("替换游戏对象主纹理", nameof(ReplaceGameObjectMainTexture))]
        [ScriptDescription("替换游戏对象主纹理；仅支持替换主纹理，传入的纹理参数可以是 Texture 类的子类对象，如2D纹理、渲染纹理等；")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObject, "游戏对象:")]
#pragma warning disable CS0618 // 类型或成员已过时
        [ScriptParams(2, EParamType.UnityAssetObject, "纹理:", typeof(Texture))]
#pragma warning restore CS0618 // 类型或成员已过时
        #endregion
        ReplaceGameObjectMainTexture,

        #region 修改游戏对象材质
        /// <summary>
        /// 修改游戏对象材质
        /// </summary>
        [ScriptName("修改游戏对象材质", nameof(ModifyGameObjectMaterial))]
        [ScriptDescription("修改游戏对象材质；传入的材质参数可以转化为 Material 类对象")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObject, "游戏对象:")]
#pragma warning disable CS0618 // 类型或成员已过时
        [ScriptParams(2, EParamType.UnityAssetObject, "材质:", typeof(Material))]
#pragma warning restore CS0618 // 类型或成员已过时
        [ScriptParams(3, EParamType.IntSlider, "材质索引(填入-1时，将该游戏对象的所有材质均修改为新的材质):", -1, 5, defaultObject = 0)]
        #endregion
        ModifyGameObjectMaterial,

        #region 修改游戏对象材质主纹理
        /// <summary>
        /// 修改游戏对象材质主纹理
        /// </summary>
        [ScriptName("修改游戏对象材质主纹理", nameof(ModifyGameObjectMaterialMainTexture))]
        [ScriptDescription("修改游戏对象材质主纹理；仅支持修改主纹理，传入的纹理参数可以是 Texture 类的子类对象，如2D纹理、渲染纹理等；")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObject, "游戏对象:")]
#pragma warning disable CS0618 // 类型或成员已过时
        [ScriptParams(2, EParamType.UnityAssetObject, "纹理:", typeof(Texture))]
#pragma warning restore CS0618 // 类型或成员已过时
        [ScriptParams(3, EParamType.IntSlider, "材质索引(填入-1时，将该游戏对象的所有材质的纹理均修改为新的纹理):", -1, 5, defaultObject = 0)]
        #endregion
        ModifyGameObjectMaterialMainTexture,

        #region 修改游戏对象材质颜色
        /// <summary>
        /// 修改游戏对象材质颜色
        /// </summary>
        [ScriptName("修改游戏对象材质颜色", nameof(ModifyGameObjectMaterialColor))]
        [ScriptDescription("修改游戏对象材质颜色；")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObject, "游戏对象:")]
        [ScriptParams(2, EParamType.Color, "颜色:")]
        [ScriptParams(3, EParamType.IntSlider, "材质索引(填入-1时，将该游戏对象的所有材质的颜色均修改为新的颜色):", -1, 5, defaultObject = 0)]
        #endregion
        ModifyGameObjectMaterialColor,

        #region 修改游戏对象共享材质
        /// <summary>
        /// 修改游戏对象共享材质
        /// </summary>
        [ScriptName("修改游戏对象共享材质", nameof(ModifyGameObjectSharedMaterial))]
        [ScriptDescription("修改游戏对象共享材质；传入的材质参数可以转化为 Material 类对象;在编辑器中谨慎使用！会修改工程中磁盘文件信息，推荐发布后做测试；")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObject, "游戏对象:")]
#pragma warning disable CS0618 // 类型或成员已过时
        [ScriptParams(2, EParamType.UnityAssetObject, "共享材质:", typeof(Material))]
#pragma warning restore CS0618 // 类型或成员已过时
        [ScriptParams(3, EParamType.IntSlider, "材质索引(填入-1时，将该游戏对象的所有共享材质均修改为新的材质):", -1, 5, defaultObject = 0)]
        #endregion
        ModifyGameObjectSharedMaterial,

        #region 修改游戏对象共享材质主纹理
        /// <summary>
        /// 修改游戏对象共享材质主纹理
        /// </summary>
        [ScriptName("修改游戏对象共享材质主纹理", nameof(ModifyGameObjectSharedMaterialMainTexture))]
        [ScriptDescription("修改游戏对象共享材质主纹理；仅支持修改主纹理，传入的纹理参数可以是 Texture 类的子类对象，如2D纹理、渲染纹理等；在编辑器中谨慎使用！会修改工程中磁盘文件信息，推荐发布后做测试；")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObject, "游戏对象:")]
#pragma warning disable CS0618 // 类型或成员已过时
        [ScriptParams(2, EParamType.UnityAssetObject, "纹理:", typeof(Texture))]
#pragma warning restore CS0618 // 类型或成员已过时
        [ScriptParams(3, EParamType.IntSlider, "材质索引(填入-1时，将该游戏对象的所有共享材质的纹理均修改为新的纹理):", -1, 5, defaultObject = 0)]
        #endregion
        ModifyGameObjectSharedMaterialMainTexture,

        #region 修改游戏对象共享材质颜色
        /// <summary>
        /// 修改游戏对象共享材质颜色
        /// </summary>
        [ScriptName("修改游戏对象共享材质颜色", nameof(ModifyGameObjectSharedMaterialColor))]
        [ScriptDescription("修改游戏对象共享材质颜色；在编辑器中谨慎使用！会修改工程中磁盘文件信息，推荐发布后做测试；")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObject, "游戏对象:")]
        [ScriptParams(2, EParamType.Color, "颜色:")]
        [ScriptParams(3, EParamType.IntSlider, "材质索引(填入-1时，将该游戏对象的所有共享材质的颜色均修改为新的颜色):", -1, 5, defaultObject = 0)]
        #endregion
        ModifyGameObjectSharedMaterialColor,

        #endregion

        #region GUI

        #region GUI-目录
        /// <summary>
        /// GUI-目录
        /// </summary>
        [ScriptName(nameof(GUI), nameof(GUI), EGrammarType.Category)]
        #endregion
        GUI,

        #region 创建标签
        /// <summary>
        /// 创建标签
        /// </summary>
        [ScriptName("创建标签", nameof(GUILabel))]
        [ScriptDescription("创建标签;本脚本命令仅能在'脚本MonoBehaviour事件'的‘渲染GUI时 执行’脚本事件中使用，即对应MonoBehaviour的OnGUI函数;")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.String, "标签内容:")]
        [ScriptParams(2, EParamType.Rect, "标签位置(Left/Top/Width/Height):")]
        #endregion
        GUILabel,

        #region 创建按钮
        /// <summary>
        /// 创建按钮
        /// </summary>
        [ScriptName("创建按钮", nameof(GUIButton))]
        [ScriptDescription("创建按钮;本脚本命令仅能在'脚本MonoBehaviour事件'的‘渲染GUI时 执行’脚本事件中使用，即对应MonoBehaviour的OnGUI函数;")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.String, "按钮内容:")]
        [ScriptParams(2, EParamType.Rect, "按钮位置(Left/Top/Width/Height):")]
        [ScriptParams(3, EParamType.UserDefineFun, "点击时回调的自定义函数:")]
        [ScriptParams(4, EParamType.String, "传入额外参数信息:")]
        [ScriptParams(5, EParamType.IntSlider, "字体大小:", 0, 999)]
        #endregion
        GUIButton,

        #region 绘制纹理
        /// <summary>
        /// 绘制纹理
        /// </summary>
        [ScriptName("绘制纹理", nameof(GUIDrawTexture))]
        [ScriptDescription("绘制纹理;本脚本命令仅能在'脚本MonoBehaviour事件'的‘渲染GUI时 执行’脚本事件中使用，即对应MonoBehaviour的OnGUI函数;源图片的长宽比，如果为0，则使用图像的长宽比。通过\"宽/高\"获得所需的长宽比，这允许源图像的宽高比被调整而不影响像素宽度和高度")]
        [ScriptReturn("成功返回 #True，失败返回 #False")]
#pragma warning disable CS0618 // 类型或成员已过时
        [ScriptParams(1, EParamType.UnityAssetObject, "纹理名称:", typeof(Texture))]
#pragma warning restore CS0618 // 类型或成员已过时
        [ScriptParams(2, EParamType.Rect, "坐标位置与尺寸信息:")]
        [ScriptParams(3, EParamType.Combo, "缩放模式:", "StretchToFill", "ScaleAndCrop", "ScaleToFit")]
        [ScriptParams(4, EParamType.Combo, "是否通道混合图片显示:", "是", "否")]
        [ScriptParams(5, EParamType.FloatSlider, "源图片的长宽比（如果为0，则使用图像的长宽比）:", 0f, 10f)]
        #endregion，
        GUIDrawTexture,

        #region 绘制纹理(纹理尺寸)
        /// <summary>
        /// 绘制纹理(纹理尺寸)
        /// </summary>
        [ScriptName("绘制纹理(纹理尺寸)", nameof(GUIDrawTextureWithTextureSize))]
        [ScriptDescription("绘制纹理(纹理尺寸);本脚本命令仅能在'脚本MonoBehaviour事件'的‘渲染GUI时 执行’脚本事件中使用，即对应MonoBehaviour的OnGUI函数;源图片的长宽比，如果为0，则使用图像的长宽比。通过\"宽/高\"获得所需的长宽比，这允许源图像的宽高比被调整而不影响像素宽度和高度;会根据网络摄像设备的尺寸值设定渲染结果的尺寸值；")]
        [ScriptReturn("成功返回 #True，失败返回 #False")]
#pragma warning disable CS0618 // 类型或成员已过时
        [ScriptParams(1, EParamType.UnityAssetObject, "纹理名称:", typeof(Texture))]
#pragma warning restore CS0618 // 类型或成员已过时
        [ScriptParams(2, EParamType.Vector2, "坐标位置:")]
        [ScriptParams(3, EParamType.Combo, "缩放模式:", "StretchToFill", "ScaleAndCrop", "ScaleToFit")]
        [ScriptParams(4, EParamType.Combo, "是否通道混合图片显示:", "是", "否")]
        [ScriptParams(5, EParamType.FloatSlider, "源图片的长宽比（如果为0，则使用图像的长宽比）:", 0f, 10f)]
        #endregion，
        GUIDrawTextureWithTextureSize,

        #region 创建窗口
        /// <summary>
        /// 创建窗口
        /// </summary>
        [ScriptName("创建窗口", nameof(GUIWindow))]
        [ScriptDescription("创建窗口;本脚本命令仅能在'脚本MonoBehaviour事件'的‘渲染GUI时 执行’脚本事件中使用，即对应MonoBehaviour的OnGUI函数;注意：如果期望窗口可以拖拽那么需要使用使用本脚本语句的返回值填充新的坐标位置与尺寸信息；")]
        [ScriptReturn("成功返回窗口新的坐标位置与尺寸信息 ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.IntSlider, "窗口编号:", 0, 1000)]
        [ScriptParams(2, EParamType.Rect, "坐标位置与尺寸信息(Left/Top/Width/Height):")]
        [ScriptParams(3, EParamType.UserDefineFun, "渲染函数:")]
        [ScriptParams(4, EParamType.String, "标题名:")]
        [ScriptParams(5, EParamType.String, "标题提示:")]
#pragma warning disable CS0618 // 类型或成员已过时
        [ScriptParams(6, EParamType.UnityAssetObject, "标题图片:", typeof(Texture))]
        [ScriptParams(7, EParamType.UnityAssetObject, "GUI风格-界面皮肤(如为空，使用当前风格):", typeof(GUISkin))]
#pragma warning restore CS0618 // 类型或成员已过时
        [ScriptParams(8, EParamType.Bool, "是否允许拖拽:")]
        #endregion
        GUIWindow,

        #region 文字泡
        /// <summary>
        /// 文字泡
        /// </summary>
        [ScriptName("文字泡", nameof(GUITextBuddle))]
        [ScriptDescription("文字泡；文字泡基于Unity的GUI系统进行渲染(即文字泡在MonoBehaviour的OnGUI函数中执行渲染),以二维方式进行渲染;本;可在任意脚本命令可执行环境下调用并执行本脚本命令;")]
        [ScriptReturn("成功返回#True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.String, "文字内容(支持富文本):")]
        [ScriptParams(2, EParamType.Vector3, "位置坐标:")]
        [ScriptParams(3, EParamType.CoordinateType, "坐标类型:")]
        [ScriptParams(4, EParamType.Float, "显示时间(单位:秒):", defaultObject = 5f)]
        [ScriptParams(5, EParamType.TextAnchor, "文字对其方式:", defaultObject = TextAnchor.MiddleLeft)]
        #endregion
        GUITextBuddle,

        #endregion

        #region 场景资源

        #region 场景资源-目录
        /// <summary>
        /// 场景资源<br />
        /// </summary>
        [ScriptName("场景资源", nameof(SceneAssets), EGrammarType.Category)]
        [ScriptDescription("场景资源管理操作 脚本的目录")]
        #endregion
        SceneAssets,

        #region 加载主场景
        [ScriptName("加载主场景", nameof(LoadMainScene))]
        [ScriptDescription("索引为0、程序启动时的默认场景称之为主场景;本脚本仅单纯的加载主场景，不对当前场景做移除或卸载等操作；")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.Combo, "操作:", "同步加载", "异步加载")]
        #endregion
        LoadMainScene,

        #region 加载场景(通过编译索引)
        /// <summary>
        /// 加载场景(通过编译索引)
        /// </summary>
        [ScriptName("加载场景(通过编译索引)", nameof(LoadSceneByBuildIndex))]
        [ScriptDescription("本脚本仅可用于加载在编辑器Build Settings设置中的场景；")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.IntSlider, "场景编译索引[1, 编译设置中场景数目]:", 1, 20)]
        [ScriptParams(2, EParamType.Combo, "操作:", "同步加载", "异步加载")]
        [ScriptParams(3, EParamType.Combo, "加载时:", "清空加载", "累加加载")]
        [ScriptParams(4, EParamType.UserDefineFun, "异步操作失败完后的回调函数:")]
        [ScriptParams(5, EParamType.String, "回调时携带的信息:")]
        #endregion
        LoadSceneByBuildIndex,

        #region 加载场景(通过名称)
        /// <summary>
        /// 加载场景(通过名称)
        /// </summary>
        [ScriptName("加载场景(通过名称)", nameof(LoadSceneByName))]
        [ScriptDescription("本脚本异步执行后，调用本脚本的场景被销毁，新场景被加载！场景必须在内存中!")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.String, "场景名称(编译设置中的场景名称 或 unity3d文件打包时unity场景文件名称):")]
        [ScriptParams(2, EParamType.Combo, "操作:", "同步加载", "异步加载")]
        [ScriptParams(3, EParamType.Combo, "加载时:", "清空加载", "累加加载")]
        [ScriptParams(4, EParamType.UserDefineFun, "异步操作失败完后的回调函数:")]
        [ScriptParams(5, EParamType.String, "回调时携带的信息:")]
        [ScriptParams(6, EParamType.Bool2, "检测场景有效性:", defaultObject = EBool2.No)]
        #endregion
        LoadSceneByName,

        #region 导入并加载场景
        /// <summary>
        /// 导入并加载场景
        /// </summary>
        [ScriptName("导入并加载场景", nameof(ImprotAndLoadSceneByName))]
        [ScriptDescription("本脚本成功执行后，调用本脚本的场景被销毁，新场景被加载！！会执行先导入内存，再加载到前台渲染；")]
        [ScriptReturn("成功返回 #True ; 重复导入、加载、场景重名会失败返回 #False ;")]
        [ScriptParams(1, EParamType.File, "unity3d文件全路径(本地路径或http开头的网络路径):")]
        [ScriptParams(2, EParamType.String, "场景名称(unity3d文件打包时的场景名称):")]
        [ScriptParams(3, EParamType.Combo, "操作:", "同步加载", "异步加载")]
        [ScriptParams(4, EParamType.Combo, "加载时:", "清空加载", "累加加载")]
        [ScriptParams(5, EParamType.UserDefineFun, "异步操作失败完后的回调函数:")]
        [ScriptParams(6, EParamType.String, "回调时携带的信息(如果异步导入失败，会被替换为#False;异步加载失败，值不变):")]
        #endregion
        ImprotAndLoadSceneByName,

        #region 导入场景
        /// <summary>
        /// 导入场景
        /// </summary>
        [ScriptName("导入场景", nameof(ImportSceneToMemory))]
        [ScriptDescription("导入场景，会将场景导入内存中;；本脚本执行成功后才会执行回调；")]
        [ScriptReturn("成功返回 #True ; 重复导入或场景重名会失败返回 #False ;")]
        [ScriptParams(1, EParamType.File, "unity3d文件全路径(本地路径或http开头的网络路径):")]
        [ScriptParams(2, EParamType.String, "场景名称(unity3d文件打包时的场景名称):")]
        [ScriptParams(3, EParamType.Combo, "操作:", "同步导入", "异步导入")]
        [ScriptParams(4, EParamType.UserDefineFun, "异步导入完后的回调函数:")]
        [ScriptParams(5, EParamType.String, "回调时携带的信息(如果异步导入失败，会被替换为#False):")]
        #endregion
        ImportSceneToMemory,

        #region 获取异步导入并加载场景的进度
        /// <summary>
        /// 获取场景异步加载进度
        /// </summary>
        [ScriptName("获取异步导入并加载场景的进度", nameof(GetAsyncImportAndLoadSceneProgress))]
        [ScriptDescription("获取异步导入并加载场景进度；在触发 导入并加载场景 的脚本命令后，后可获取有效值！")]
        [ScriptReturn("成功返回 范围[0,1]的浮点数进度值([0,0.5]表示导入进度,[0.5,1]表示加载进度); 失败或未调用 获取异步导入并加载场景进度 返回 #False ;")]
        #endregion
        GetAsyncImportAndLoadSceneProgress,

        #region 获取异步导入场景进度
        /// <summary>
        /// 获取异步导入场景进度
        /// </summary>
        [ScriptName("获取异步导入场景进度", nameof(GetAsyncImportSceneProgress))]
        [ScriptDescription("获取异步导入场景进度；在触发 导入场景 的异步调用脚本命令后，后可获取有效值！")]
        [ScriptReturn("成功返回 范围[0,1]的浮点数进度值 ; 失败或未调用场景加载返回 #False ;")]
        #endregion
        GetAsyncImportSceneProgress,

        #region 获取异步加载场景进度
        /// <summary>
        /// 获取异步加载场景进度
        /// </summary>
        [ScriptName("获取异步加载场景进度", nameof(GetAsyncLoadSceneProgress))]
        [ScriptDescription("获取异步加载场景进度；在触发 加载场景 的异步调用脚本命令后，后可获取有效值！")]
        [ScriptReturn("成功返回 范围[0,1]的浮点数进度值 ; 失败或未调用场景加载返回 #False ;")]
        #endregion
        GetAsyncLoadSceneProgress,

        #region 获取场景信息
        /// <summary>
        /// 获取场景信息
        /// </summary>
        [ScriptName("获取场景信息", nameof(GetSceneInfo))]
        [ScriptDescription("获取场景信息；")]
        [ScriptReturn("成功返回 具体信息 ; 失败或未调用场景加载返回 #False ;")]
        [ScriptParams(1, EParamType.Combo, "信息:", "场景数目", "编译设置中场景数目", "当前场景打包索引", "当前场景资源名", "当前场景是否被加载", "当前场景是否被修改", "当前场景路径", "当前场景根游戏对象数目")]
        #endregion
        GetSceneInfo,

        #region 获取场景资源信息
        /// <summary>
        /// 获取场景资源信息
        /// </summary>
        [ScriptName("获取场景资源信息", nameof(GetSceneAssetsInfo))]
        [ScriptDescription("获取场景资源信息；不包括默认的场景信息；所有动态加载的场景基础信息；")]
        [ScriptReturn("成功返回 具体信息 ; 失败或未调用场景加载返回 #False ;")]
        [ScriptParams(1, EParamType.Combo, "信息:", "场景数目", "场景资源名(通过索引)")]
        [ScriptParams(2, EParamType.IntSlider, "索引[1,场景数目]:", 1, 10)]
        #endregion
        GetSceneAssetsInfo,

        #region 卸载全部子场景
        /// <summary>
        /// 卸载全部子场景
        /// </summary>
        [ScriptName("卸载全部子场景", nameof(UnloadAllSubScene))]
        [ScriptDescription("只会卸载 '导入场景' 类型的脚本命令添加的子场景；当选择 '除主场景外全部卸载' 操作类型时，如果当前场景不是主场景(索引为0、程序启动时的默认场景)，那么不执行任何卸载操作；")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.Combo, "操作类型:", "除主场景与当前场景外全部卸载", "除主场景外全部卸载")]
        #endregion
        UnloadAllSubScene,

        #region 卸载子场景
        /// <summary>
        /// 卸载子场景
        /// </summary>
        [ScriptName("卸载子场景", nameof(UnloadSubScene))]
        [ScriptDescription("如果当前场景是要卸载的场景，不做任何处理；默认只会卸载 导入场景 类型的脚本命令添加的子场景；无法卸载主场景(索引为0、程序启动时的默认场景);")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.String, "场景名:")]
        #endregion
        UnloadSubScene,

        [ScriptName("卸载未使用的资产并GC收集", nameof(UnloadUnusedAssetAndGCCollect))]
        UnloadUnusedAssetAndGCCollect,

        #endregion

        #region Web请求

        #region Web请求-目录
        /// <summary>
        /// Web请求-目录
        /// </summary>
        [ScriptName("Web请求", nameof(WebRequest), EGrammarType.Category)]
        [ScriptDescription("Web请求操作脚本的目录；")]
        #endregion
        WebRequest,

        #region 发送Web请求
        /// <summary>
        /// 发送Web请求
        /// </summary>
        [ScriptName("发送Web请求", nameof(SendWebRequest))]
        [ScriptDescription("发送Web请求；只可以请求普通Web数据请求，结果是明文字符串类型的；可以同时开启多个 发送Web请求 操作；如果变量不存在，则定义该变量；")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.String, "URL:")]
        [ScriptParams(2, EParamType.UserDefineFun, "请求响应后的回调函数:")]
        [ScriptParams(3, EParamType.GlobalVariableName, "用于存储 请求响应后文本信息 的变量(* 注意不要加$ *):")]
        [ScriptParams(4, EParamType.String, "回调函数的额外信息(如果请求失败本信息会被替换为 #False):")]
        #endregion
        SendWebRequest,

        #region 获取Web请求数目
        /// <summary>
        /// 获取Web请求数目
        /// </summary>
        [ScriptName("获取Web请求数目", nameof(GetSendWebRequestCount))]
        [ScriptDescription("获取Web请求数目；获取当前正处于 Web请求 数目；返回的数量仅在本次执行逻辑内有效；数目仅包括调用脚本“发送Web请求”请求数目；")]
        [ScriptReturn("成功返回 正在Web请求的数目 ; 失败返回 #False ;")]
        #endregion
        GetSendWebRequestCount,

        #region 获取Web请求进度
        /// <summary>
        /// 获取Web请求进度
        /// </summary>
        [ScriptName("获取Web请求进度", nameof(GetWebRequestProgress))]
        [ScriptDescription("获取Web请求进度；")]
        [ScriptReturn("成功返回 [0,1] 的浮点数进度值；如果请求已经返回或是没有下载返回 -1;")]
        [ScriptParams(1, EParamType.String, "URL:")]
        #endregion
        GetWebRequestProgress,

        #region Web文件下载
        /// <summary>
        /// Web文件下载
        /// </summary>
        [ScriptName("Web文件下载", nameof(WebDownloadFile))]
        [ScriptDescription("Web文件下载；文件直接存储在制定的临时缓冲目录中；可以同时开启多个 Web文件下载 操作；如果变量不存在，则定义该变量；")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.String, "URL:")]
        [ScriptParams(2, EParamType.UserDefineFun, "文件下载完成后的回调函数:")]
        [ScriptParams(3, EParamType.GlobalVariableName, "变量(文件存储的本地磁盘路径(*注意不要加$*)):")]
        [ScriptParams(4, EParamType.String, "回调函数的额外信息(如果文件下载失败本信息会被替换为 #False):")]
        #endregion
        WebDownloadFile,

        #region 获取Web文件下载数目
        /// <summary>
        /// 获取Web文件下载数目
        /// </summary>
        [ScriptName("获取Web文件下载数目", nameof(GetWebDownloadFileCount))]
        [ScriptDescription("获取Web文件下载数目；获取当前正处于请求列表中的下载中文件数目；返回的数量仅在本次执行逻辑内有效；数目仅包括调用脚本“Web文件下载”的请求数目；")]
        [ScriptReturn("成功返回 下载中文件的数目 ; 失败返回 #False ;")]
        #endregion
        GetWebDownloadFileCount,

        #region 获取Web文件下载进度
        /// <summary>
        /// 获取Web文件下载进度
        /// </summary>
        [ScriptName("获取Web文件下载进度", nameof(GetWebDownloadFileProgress))]
        [ScriptDescription("获取Web文件下载进度；")]
        [ScriptReturn("成功返回 [0,1] 的浮点数进度值；如果文件url已经下载完成或是没有下载返回 -1;")]
        [ScriptParams(1, EParamType.String, "URL:")]
        #endregion
        GetWebDownloadFileProgress,

        #region 检查网络连接
        /// <summary>
        /// 检查网络连接
        /// </summary>
        [ScriptName("检查网络连接", nameof(CheckNetworkConnection))]
        [ScriptDescription("检查网络连接；")]
        [ScriptReturn("可以连接互联网则返回 #True ; 否则返回 #False ;")]
        #endregion
        CheckNetworkConnection,

        #region 获取网络可达性信息
        /// <summary>
        /// 获取网络可达性信息
        /// </summary>
        [ScriptName("获取网络可达性信息", nameof(GetInternetReachabilityInfo))]
        [ScriptDescription("获取网络可达性信息；")]
        [ScriptReturn("返回网络可达性的描述字符串")]
        #endregion
        GetInternetReachabilityInfo,

        #endregion

        #region 声音

        #region 声音-目录
        /// <summary>
        /// 鼠标与触摸操作
        /// </summary>
        [ScriptName("声音", nameof(Audio), EGrammarType.Category)]
        [ScriptDescription("声音操作脚本的目录;")]
        #endregion
        Audio,

        #region 声音操作-播放AudioSource音源
        /// <summary>
        /// 播放音源
        /// </summary>
        [ScriptName("播放音源", nameof(AudioSoundPlay))]
        [ScriptDescription("控制AudioSource的播放")]
        [ScriptReturn("成功返回 #True  ;失败返回 #False;")]
        [ScriptParams(1, EParamType.GameObjectComponent, "音源对象:(限定AudioSource类型)", "", typeof(UnityEngine.AudioSource))]
        [ScriptParams(2, EParamType.Combo, "播放控制:", "播放", "暂停", "停止")]
        #endregion
        AudioSoundPlay,

        [ScriptName("设置音源时间", nameof(SetAudioSoundTime))]
        [ScriptParams(1, EParamType.GameObjectComponent, "音源对象:(限定AudioSource类型)", "", typeof(UnityEngine.AudioSource))]
        [ScriptParams(2, EParamType.FloatSlider, "时间(单位: 秒):", 0f, 3600f)]
        SetAudioSoundTime,

        [ScriptName("设置音源进度", nameof(SetAudioSoundProgress))]
        [ScriptParams(1, EParamType.GameObjectComponent, "音源对象:(限定AudioSource类型)", "", typeof(UnityEngine.AudioSource))]
        [ScriptParams(2, EParamType.FloatSlider, "进度:", 0f, 1f)]
        SetAudioSoundProgress,

        #region 声音操作-设置音源

        /// <summary>
        /// 设置音源
        /// </summary>
        [ScriptName("设置音源", nameof(SetAudioSound))]
        [ScriptDescription("控制AudioSource的音频源")]
        [ScriptReturn("成功返回 #True  ;失败返回 #False;")]
        [ScriptParams(1, EParamType.GameObjectComponent, "音源对象:(限定AudioSource类型)", "", typeof(UnityEngine.AudioSource))]
#pragma warning disable CS0618 // 类型或成员已过时
        [ScriptParams(2, EParamType.UnityAssetObject, "音频文件:", typeof(AudioClip))]
#pragma warning restore CS0618 // 类型或成员已过时
        #endregion
        SetAudioSound,

        #region 声音操作-设置音源
        /// <summary>
        /// 设置音源
        /// </summary>
        [ScriptName("设置音源循环", nameof(SetAudioSoundLoop))]
        [ScriptDescription("设置AudioSource的循环类型")]
        [ScriptReturn("成功返回 #True  ;失败返回 #False;")]
        [ScriptParams(1, EParamType.GameObjectComponent, "音源对象:(限定AudioSource类型)", typeof(UnityEngine.AudioSource))]
        [ScriptParams(2, EParamType.Combo, "是否循环:", "是", "否")]
        #endregion
        SetAudioSoundLoop,

        [ScriptName("设置音源音量", nameof(SetAudioSourceVolume))]
        [ScriptParams(1, EParamType.GameObjectComponent, "音源对象:(限定AudioSource类型)", typeof(UnityEngine.AudioSource))]
        [ScriptParams(2, EParamType.FloatSlider, "音量", 0f, 1f, defaultObject = 1f)]
        SetAudioSourceVolume,

        [ScriptName("获取音源信息", nameof(GetAudioSourceInfo))]
        [ScriptParams(1, EParamType.GameObjectComponent, "音源对象:(限定AudioSource类型)", typeof(UnityEngine.AudioSource))]
        [ScriptParams(2, EParamType.Combo, "信息类型", "播放状态", "时长", "时间", "进度", "游戏对象", "循环", "音量")]
        GetAudioSourceInfo,

        #endregion

        #region 动画系统

        #region 动画系统(Legacy)-目录
        /// <summary>
        /// 动画系统(Legacy)
        /// </summary>
        [ScriptName("动画系统(Legacy)", nameof(LegacyAnimationSystem), EGrammarType.Category)]
        [ScriptDescription("Legacy动画系统操作脚本的目录;")]
        #endregion
        LegacyAnimationSystem,

        #region 设置Animation循环模式
        /// <summary>
        /// 设置Animation循环模式
        /// </summary>
        [ScriptName("设置Animation循环模式", nameof(SetAnimationWarpMode))]
        [ScriptDescription("设置Animation循环模式;Default,默认；Once，单向播放一次；Loop，单向循环；PingPong，钟摆往复；ClampForever，播放动画并定位到结尾到帧；")]
        [ScriptReturn("成功返回 #True  ;失败返回 #False;")]
        [ScriptParams(1, EParamType.GameObjectComponent, "动画组件对象(限定Animation类型):", typeof(UnityEngine.Animation))]
        [ScriptParams(2, EParamType.Combo, "循环模式：", "Default", "Once", "Loop", "PingPong", "ClampForever")]
        #endregion
        SetAnimationWarpMode,

        #region 获取Animation循环模式
        /// <summary>
        /// 获取Animation循环模式
        /// </summary>
        [ScriptName("获取Animation循环模式", nameof(GetAnimationWarpMode))]
        [ScriptDescription("获取Animation循环模式")]
        [ScriptReturn("成功返回 Animation循环模式 ;失败返回 #False;")]
        [ScriptParams(1, EParamType.GameObjectComponent, "动画组件对象(限定Animation类型):", typeof(UnityEngine.Animation))]
        #endregion
        GetAnimationWarpMode,

        #region Animation播放
        /// <summary>
        ///  Animation播放
        /// </summary>
        [ScriptName("Animation播放", nameof(AnimationPlay))]
        [ScriptDescription("Animation播放")]
        [ScriptReturn("成功返回 #True  ;失败返回 #False;")]
        [ScriptParams(1, EParamType.GameObjectComponent, "动画组件对象(限定Animation类型):", typeof(UnityEngine.Animation))]
        [ScriptParams(2, EParamType.String, "动画名（如不填写，播放默认动画）：")]
        [ScriptParams(3, EParamType.Combo, "播放模式：", "停止同层动画", "停止全部动画")]
        #endregion
        AnimationPlay,

        #region Animation播放队列
        /// <summary>
        ///  Animation播放队列
        /// </summary>
        [ScriptName("Animation播放队列", nameof(AnimationPlayQueued))]
        [ScriptDescription("Animation播放队列；在前一个动画播放完成之后直接播放下一个动画")]
        [ScriptReturn("成功返回 #True  ;失败返回 #False;")]
        [ScriptParams(1, EParamType.GameObjectComponent, "动画组件对象(限定Animation类型):", typeof(UnityEngine.Animation))]
        [ScriptParams(2, EParamType.String, "动画名（如不填写，播放默认动画）：")]
        [ScriptParams(3, EParamType.Combo, "播放模式：", "停止同层动画", "停止全部动画")]
        [ScriptParams(4, EParamType.Combo, "队列模式：", "完成其他动画后开始播放", "立即开始播放")]
        #endregion
        AnimationPlayQueued,

        #region Animation停止
        /// <summary>
        /// Animation停止
        /// </summary>
        [ScriptName("Animation停止", nameof(AnimationStop))]
        [ScriptDescription("Animation停止")]
        [ScriptReturn("成功返回 #True  ;失败返回 #False;")]
        [ScriptParams(1, EParamType.GameObjectComponent, "动画组件对象(限定Animation类型):", typeof(UnityEngine.Animation))]
        [ScriptParams(2, EParamType.String, "动画名（如不填写，停止所有当前Animation正在播放的动画）：")]
        #endregion
        AnimationStop,

        #region Animation倒播
        /// <summary>
        /// Animation倒播
        /// </summary>
        [ScriptName("Animation倒播", nameof(AnimationRewind))]
        [ScriptDescription("Animation倒播")]
        [ScriptReturn("成功返回 #True  ;失败返回 #False;")]
        [ScriptParams(1, EParamType.GameObjectComponent, "动画组件对象(限定Animation类型):", typeof(UnityEngine.Animation))]
        [ScriptParams(2, EParamType.String, "动画名（如不填写，倒回所有动画）：")]
        #endregion
        AnimationRewind,

        #region Animation是否播放中
        /// <summary>
        /// 获取Animation播放状态
        /// </summary>
        [ScriptName("Animation是否播放中", nameof(AnimationIsPlaying))]
        [ScriptDescription("Animation是否播放中，即获取Animation的当前播放状态")]
        [ScriptReturn("成功返回 #True  ;失败返回 #False;")]
        [ScriptParams(1, EParamType.GameObjectComponent, "动画组件对象(限定Animation类型):", typeof(UnityEngine.Animation))]
        [ScriptParams(2, EParamType.String, "动画名（如不填写，返回是否在播放任何动画）：")]
        #endregion
        AnimationIsPlaying,

        #region Animation混合
        /// <summary>
        /// Animation混合
        /// </summary>
        [ScriptName("Animation混合", nameof(AnimationBlend))]
        [ScriptDescription("在接下来的几秒内混合指定名称的动画直到目标权重")]
        [ScriptReturn("成功返回 #True  ;失败返回 #False;")]
        [ScriptParams(1, EParamType.GameObjectComponent, "动画组件对象(限定Animation类型):", typeof(UnityEngine.Animation))]
        [ScriptParams(2, EParamType.String, "动画名（如不填写，返回是否在播放任何动画）：")]
        [ScriptParams(3, EParamType.FloatSlider, "目标权重：", 0f, 1f)]
        [ScriptParams(4, EParamType.FloatSlider, "褪色长度（推荐 0.3）：", 0f, 10f, defaultObject = 0.3f)]
        #endregion
        AnimationBlend,

        #region Animation淡入淡出
        /// <summary>
        /// Animation淡入淡出
        /// </summary>
        [ScriptName("Animation淡入淡出", nameof(AnimationCrossFade))]
        [ScriptDescription("在一定时间内淡入指定名称的动画并且淡出其他动画")]
        [ScriptReturn("成功返回 #True  ;失败返回 #False;")]
        [ScriptParams(1, EParamType.GameObjectComponent, "动画组件对象(限定Animation类型):", typeof(UnityEngine.Animation))]
        [ScriptParams(2, EParamType.String, "动画名（如不填写，返回是否在播放任何动画）：")]
        [ScriptParams(3, EParamType.FloatSlider, "褪色长度（推荐 0.3）：", 0f, 10f, defaultObject = 0.3f)]
        [ScriptParams(4, EParamType.Combo, "播放模式：", "停止同层动画", "停止全部动画")]
        #endregion
        AnimationCrossFade,

        #region Animation淡入淡出队列
        /// <summary>
        /// Animation淡入淡出队列
        /// </summary>
        [ScriptName("Animation淡入淡出队列", nameof(AnimationCrossFadeQueued))]
        [ScriptDescription("在一定时间内淡入指定名称的动画并且淡出其他动画；在前一个动画播放完成之后淡入淡出下一个动画；")]
        [ScriptReturn("成功返回 #True  ;失败返回 #False;")]
        [ScriptParams(1, EParamType.GameObjectComponent, "动画组件对象(限定Animation类型):", typeof(UnityEngine.Animation))]
        [ScriptParams(2, EParamType.String, "动画名（如不填写，返回是否在播放任何动画）：")]
        [ScriptParams(3, EParamType.FloatSlider, "褪色长度（推荐 0.3）：", 0f, 10f, defaultObject = 0.3f)]
        [ScriptParams(4, EParamType.Combo, "播放模式：", "停止同层动画", "停止全部动画")]
        [ScriptParams(5, EParamType.Combo, "队列模式：", "完成其他动画后开始播放", "立即开始播放")]
        #endregion
        AnimationCrossFadeQueued,

        #region 动画系统(Mecanim)-目录
        /// <summary>
        /// 动画系统(Mecanim)
        /// </summary>
        [ScriptName("动画系统(Mecanim)", nameof(MecanimAnimatiSystem), EGrammarType.Category)]
        [ScriptDescription("Mecanim动画系统操作脚本的目录;")]
        #endregion
        MecanimAnimatiSystem,

        #region 动画操作-设置Animator参数值
        /// <summary>
        /// 设置unity动画参数值
        /// </summary>
        [ScriptName("设置Animator参数值", nameof(SetAnimatorParamValue))]
        [ScriptDescription("设置Animator动画参数值;对于布尔型，传入值大于0为真，小于等于0为假;整数时退一取整，即取不大于传入值的最大整数值；对于触发器类型，传入值大于0会设置触发器为触发状态（在动画系统使用一次该触发器后会自动变为非触发状态），小于等于0时会将触发器重置（即非触发状态）；")]
        [ScriptReturn("成功返回 #True  ;失败返回 #False;")]
        [ScriptParams(1, EParamType.GameObjectComponent, "动画对象(限定Animator类型):", "", typeof(UnityEngine.Animator))]
        [ScriptParams(2, EParamType.String, "参数名:")]
        [ScriptParams(3, EParamType.Combo, "参数类型:", "布尔型", "浮点数", "整数", "触发器")]
        [ScriptParams(4, EParamType.FloatSlider, "值:", -1000f, 1000f)]
        #endregion
        SetAnimatorParamValue,

        #region 动画操作-获取Animator参数值
        /// <summary>
        /// 获取unity动画参数值
        /// </summary>
        [ScriptName("获取Animator参数值", nameof(GetAnimatorParamValue))]
        [ScriptDescription("设置Animator动画参数值;对于布尔型，传入值大于0为真，小于等于0为假;整数时退一取整，即取不大于传入值的最大整数值；")]
        [ScriptReturn("成功返回 #True  ;失败返回 #False;")]
        [ScriptParams(1, EParamType.GameObjectComponent, "动画对象(限定Animator类型):", "", typeof(UnityEngine.Animator))]
        [ScriptParams(2, EParamType.String, "参数名:")]
        [ScriptParams(3, EParamType.Combo, "参数类型:", "布尔型", "浮点数", "整数")]
        #endregion
        GetAnimatorParamValue,

        #region Animator播放
        /// <summary>
        ///  Animator播放
        /// </summary>
        [ScriptName("Animator播放", nameof(AnimatorPlay))]
        [ScriptDescription("Animator播放;如果层索引为-1，则会在所有层中(优先当前层？)查找第一个符合状态名的状态；如果标准时间小于0，则设置为负无穷大；")]
        [ScriptReturn("成功返回 #True  ;失败返回 #False;")]
        [ScriptParams(1, EParamType.GameObjectComponent, "动画组件对象(限定Animator类型):", typeof(UnityEngine.Animator))]
        [ScriptParams(2, EParamType.String, "状态名：")]
        [ScriptParams(3, EParamType.IntSlider, "层索引[-1, 组件对应控制器的最大层数)：", -1, 100, defaultObject = -1)]
        [ScriptParams(4, EParamType.FloatSlider, "标准时间[0,1]：", -1f, 1f)]
        #endregion
        AnimatorPlay,

        #region Animator播放(固定时间)
        /// <summary>
        ///  Animator播放(固定时间)
        /// </summary>
        [ScriptName("Animator播放(固定时间)", nameof(AnimatorPlayInFixedTime))]
        [ScriptDescription("Animator播放;如果层索引为-1，则会在所有层中(优先当前层？)查找第一个符合状态名的状态；如果固定时间小于0，则设置为负无穷大；")]
        [ScriptReturn("成功返回 #True  ;失败返回 #False;")]
        [ScriptParams(1, EParamType.GameObjectComponent, "动画组件对象(限定Animator类型):", typeof(UnityEngine.Animator))]
        [ScriptParams(2, EParamType.String, "状态名：")]
        [ScriptParams(3, EParamType.IntSlider, "层索引[-1, 组件对应控制器的最大层数)：", -1, 100, defaultObject = -1)]
        [ScriptParams(4, EParamType.FloatSlider, "固定时间[0,1]：", -1f, 1f)]
        #endregion
        AnimatorPlayInFixedTime,

        #region Animator淡入淡出
        /// <summary>
        ///  Animator淡入淡出
        /// </summary>
        [ScriptName("Animator淡入淡出", nameof(AnimatorCrossFade))]
        [ScriptDescription("Animator淡入淡出;如果层索引为-1，则会在所有层中(优先当前层？)查找第一个符合状态名的状态；如果标准时间小于0，则设置为负无穷大；")]
        [ScriptReturn("成功返回 #True  ;失败返回 #False;")]
        [ScriptParams(1, EParamType.GameObjectComponent, "动画组件对象(限定Animator类型):", typeof(UnityEngine.Animator))]
        [ScriptParams(2, EParamType.String, "状态名：")]
        [ScriptParams(3, EParamType.IntSlider, "层索引[-1, 组件对应控制器的最大层数)：", -1, 100, defaultObject = -1)]
        [ScriptParams(4, EParamType.FloatSlider, "标准时间[0,1]：", -1f, 1f)]
        [ScriptParams(5, EParamType.FloatSlider, "过渡的持续时间[0,1]：", 0f, 1f)]
        #endregion
        AnimatorCrossFade,

        #region Animator淡入淡出(固定时间)
        /// <summary>
        ///  Animator淡入淡出(固定时间)
        /// </summary>
        [ScriptName("Animator淡入淡出(固定时间)", nameof(AnimatorCrossFadeInFixedTime))]
        [ScriptDescription("Animator淡入淡出(固定时间);如果层索引为-1，则会在所有层中(优先当前层？)查找第一个符合状态名的状态；如果固定时间小于0，则设置为0；")]
        [ScriptReturn("成功返回 #True  ;失败返回 #False;")]
        [ScriptParams(1, EParamType.GameObjectComponent, "动画组件对象(限定Animator类型):", typeof(UnityEngine.Animator))]
        [ScriptParams(2, EParamType.String, "状态名：")]
        [ScriptParams(3, EParamType.IntSlider, "层索引[-1, 组件对应控制器的最大层数)：", -1, 100, defaultObject = -1)]
        [ScriptParams(4, EParamType.FloatSlider, "固定时间[0,1]：", 0f, 1f)]
        [ScriptParams(5, EParamType.FloatSlider, "过渡的持续时间[0,1]：", 0f, 1f)]
        #endregion
        AnimatorCrossFadeInFixedTime,

        #endregion

        #region 二维向量

        #region 二维向量-目录
        /// <summary>
        /// 二维向量操作
        /// </summary>
        [ScriptName("二维向量", nameof(Vector2), EGrammarType.Category)]
        [ScriptDescription("二维向量操作脚本的目录;")]
        #endregion
        Vector2,

        #region 二维向量计算
        /// <summary>
        /// 二维向量计算
        /// </summary>
        [ScriptName("二维向量计算", nameof(Vector2Calculate))]
        [ScriptDescription("二维向量计算")]
        [ScriptReturn("成功返回 计算结果  ;")]
        [ScriptParams(1, EParamType.Vector2, "二维向量:")]
        [ScriptParams(2, EParamType.Combo, "运算符:", "加", "减")]
        [ScriptParams(3, EParamType.Vector2, "二维向量:")]
        #endregion
        Vector2Calculate,

        #region 构造二维向量
        /// <summary>
        /// 构造二维向量
        /// </summary>
        [ScriptName("构造二维向量", nameof(Vector2Create))]
        [ScriptDescription("构造二维向量;二维向量的 X,Y值使用 / 分割")]
        [ScriptReturn("成功返回 二维向量 ;失败返回 #False;")]
        [ScriptParams(1, EParamType.Vector2, "二维向量：")]
        #endregion
        Vector2Create,

        #region 修改二维向量
        /// <summary>
        /// 修改二维向量
        /// </summary>
        [ScriptName("修改二维向量", nameof(Vector2Modify))]
        [ScriptDescription("修改二维向量;二维向量的 X,Y值使用新值替代，并返回修改后的值；")]
        [ScriptReturn("成功返回 二维向量 ;失败返回 #False;")]
        [ScriptParams(1, EParamType.Vector2, "二维向量：")]
        [ScriptParams(2, EParamType.Combo, "待修改的类型：", "x", "y")]
        [ScriptParams(3, EParamType.Float, "新值：")]
        #endregion
        Vector2Modify,

        #region 二维向量合成
        /// <summary>
        /// 二维向量合成
        /// </summary>
        [ScriptName("二维向量合成", nameof(Vector2Merge))]
        [ScriptDescription("二维向量合成;二维向量的 X,Y值使用 / 分割")]
        [ScriptReturn("成功返回 二维向量 ;失败返回 #False;")]
        [ScriptParams(1, EParamType.FloatSlider, "X:", -99999999f, 99999999f)]
        [ScriptParams(2, EParamType.FloatSlider, "Y:", -99999999f, 99999999f)]
        #endregion
        Vector2Merge,

        #region 二维向量拆分
        /// <summary>
        /// 二维向量拆分
        /// </summary>
        [ScriptName("二维向量拆分", nameof(Vector2Split))]
        [ScriptDescription("拆分二维向量;二维向量的 X,Y值使用 / 分割")]
        [ScriptReturn("成功返回 #True ;失败返回 #False;")]
        [ScriptParams(1, EParamType.Vector2, "二维向量：")]
        [ScriptParams(2, EParamType.GlobalVariableName, "二维向量拆分后存储 X 值的变量名：")]
        [ScriptParams(3, EParamType.GlobalVariableName, "二维向量拆分后存储 Y 值的变量名：")]
        #endregion
        Vector2Split,

        #region 二维向量长度
        /// <summary>
        /// 二维向量长度
        /// </summary>
        [ScriptName("二维向量长度", nameof(Vector2Magnitude))]
        [ScriptDescription("获取二维向量长度;")]
        [ScriptReturn("成功返回 二维向量长度 ;失败返回 #False;")]
        [ScriptParams(1, EParamType.Vector2, "二维向量：")]
        #endregion
        Vector2Magnitude,

        #region 获取二维向量长度的平方
        /// <summary>
        /// 获取二维向量长度的平方
        /// </summary>
        [ScriptName("获取二维向量长度的平方", nameof(Vector2SqrMagnitude))]
        [ScriptDescription("获取二维向量长度的平方;")]
        [ScriptReturn("成功返回 获取二维向量长度的平方 ;失败返回 #False;")]
        [ScriptParams(1, EParamType.Vector2, "二维向量：")]
        #endregion
        Vector2SqrMagnitude,

        #region 单位化二维向量
        /// <summary>
        /// 单位化二维向量
        /// </summary>
        [ScriptName("单位化二维向量", nameof(Vector2Normalized))]
        [ScriptDescription("单位化二维向量;")]
        [ScriptReturn("成功返回 单位化后的二维向量 ;失败返回 #False;")]
        [ScriptParams(1, EParamType.Vector2, "二维向量：")]
        #endregion
        Vector2Normalized,

        #region 二维向量角度
        /// <summary>
        /// 二维向量角度
        /// </summary>
        [ScriptName("二维向量角度", nameof(Vector2Angle))]
        [ScriptDescription("获取二维向量 From 与 二维向量 To 之间的一个角度;")]
        [ScriptReturn("成功返回 单位化后的二维向量 ;失败返回 #False;")]
        [ScriptParams(1, EParamType.Vector2, "二维向量 From：")]
        [ScriptParams(2, EParamType.Vector2, "二维向量 To：")]
        #endregion
        Vector2Angle,

        #region 二维向量限制长度
        /// <summary>
        /// 二维向量限制长度
        /// </summary>
        [ScriptName("二维向量限制长度", nameof(Vector2ClampMagnitude))]
        [ScriptDescription("二维向量限制长度;返回向量的长度，最大不超过maxLength所指示的长度；也就是说，把向量限制在一个特定的长度；")]
        [ScriptReturn("成功返回 限制长度后的二维向量 ;失败返回 #False;")]
        [ScriptParams(1, EParamType.Vector2, "二维向量：")]
        [ScriptParams(2, EParamType.FloatSlider, "最大长度：", 0f, 999999f)]
        #endregion
        Vector2ClampMagnitude,

        #region 二维向量距离
        /// <summary>
        /// 二维向量距离
        /// </summary>
        [ScriptName("二维向量距离", nameof(Vector2Distance))]
        [ScriptDescription("获取两个二维向量之间的距离；")]
        [ScriptReturn("成功返回 距离值 ;失败返回 #False;")]
        [ScriptParams(1, EParamType.Vector2, "二维向量 A：")]
        [ScriptParams(2, EParamType.Vector2, "二维向量 B：")]
        #endregion
        Vector2Distance,

        #region 二维向量点乘积
        /// <summary>
        /// 二维向量点乘积
        /// </summary>
        [ScriptName("二维向量点乘积", nameof(Vector2Dot))]
        [ScriptDescription("获取两个二维向量的点乘积；")]
        [ScriptReturn("成功返回 点乘积值 ;失败返回 #False;")]
        [ScriptParams(1, EParamType.Vector2, "二维向量 A：")]
        [ScriptParams(2, EParamType.Vector2, "二维向量 B：")]
        #endregion
        Vector2Dot,

        #region 二维向量线性插值
        /// <summary>
        /// 二维向量线性插值
        /// </summary>
        [ScriptName("二维向量线性插值", nameof(Vector2Lerp))]
        [ScriptDescription("获取两个二维向量的线性插值；")]
        [ScriptReturn("成功返回 点乘积值 ;失败返回 #False;")]
        [ScriptParams(1, EParamType.Vector2, "二维向量 A：")]
        [ScriptParams(2, EParamType.Vector2, "二维向量 B：")]
        [ScriptParams(3, EParamType.FloatSlider, "线性插值比例值：", 0f, 1f)]
        #endregion
        Vector2Lerp,

        #region 二维向量非限制线性插值
        /// <summary>
        /// 二维向量非限制线性插值
        /// </summary>
        [ScriptName("二维向量非限制线性插值", nameof(Vector2LerpUnclamped))]
        [ScriptDescription("二维向量非限制线性插值；线性插值比例值 = 0 ，则返回 A；线性插值比例值 = 1 ，则返回 B")]
        [ScriptReturn("成功返回 点乘积值 ;失败返回 #False;")]
        [ScriptParams(1, EParamType.Vector2, "二维向量 A：")]
        [ScriptParams(2, EParamType.Vector2, "二维向量 B：")]
        [ScriptParams(3, EParamType.FloatSlider, "线性插值比例值：", -999999f, 999999f)]
        #endregion
        Vector2LerpUnclamped,

        #region 获取二维向量并集
        /// <summary>
        /// 获取二维向量并集
        /// </summary>
        [ScriptName("获取二维向量并集", nameof(Vector2Max))]
        [ScriptDescription("获取二维向量并集；由两个向量的最大组件组成的向量；")]
        [ScriptReturn("成功返回 并集二维向量 ;失败返回 #False;")]
        [ScriptParams(1, EParamType.Vector2, "二维向量 A：")]
        [ScriptParams(2, EParamType.Vector2, "二维向量 B：")]
        #endregion
        Vector2Max,

        #region 获取二维向量交集
        /// <summary>
        /// 获取二维向量交集
        /// </summary>
        [ScriptName("获取二维向量交集", nameof(Vector2Min))]
        [ScriptDescription("获取二维向量交集；两个向量的最小组件组成的向量；")]
        [ScriptReturn("成功返回 交集二维向量 ;失败返回 #False;")]
        [ScriptParams(1, EParamType.Vector2, "二维向量 A：")]
        [ScriptParams(2, EParamType.Vector2, "二维向量 B：")]
        #endregion
        Vector2Min,

        #region 二维向量移向
        /// <summary>
        /// 二维向量移向
        /// </summary>
        [ScriptName("二维向量移向", nameof(Vector2MoveTowards))]
        [ScriptDescription("二维向量移向；最大移动偏移量为 如果为正值，当前地点移向目标，如果是负值当前地点将远离目标；")]
        [ScriptReturn("成功返回 移向后的二维向量 ;失败返回 #False;")]
        [ScriptParams(1, EParamType.Vector2, "当前向量：")]
        [ScriptParams(2, EParamType.Vector2, "目标向量：")]
        [ScriptParams(3, EParamType.FloatSlider, "最大移动偏移量：", -999999f, 999999f)]
        #endregion
        Vector2MoveTowards,

        #region 二维向量反射
        /// <summary>
        /// 二维向量反射
        /// </summary>
        [ScriptName("二维向量反射", nameof(Vector2Reflect))]
        [ScriptDescription("二维向量反射；V对于法线N的反射；")]
        [ScriptReturn("成功返回 反射计算后的二维向量 ;失败返回 #False;")]
        [ScriptParams(1, EParamType.Vector2, "向量V：")]
        [ScriptParams(2, EParamType.Vector2, "法线N向量：")]
        #endregion
        Vector2Reflect,

        #region 二维向量缩放
        /// <summary>
        /// 二维向量缩放
        /// </summary>
        [ScriptName("二维向量缩放", nameof(Vector2Scale))]
        [ScriptDescription("二维向量缩放；两个向量组件对应X,Y相乘；")]
        [ScriptReturn("成功返回 缩放后的二维向量 ;失败返回 #False;")]
        [ScriptParams(1, EParamType.Vector2, "当前向量：")]
        [ScriptParams(2, EParamType.Vector2, "缩放系数向量：")]
        #endregion
        Vector2Scale,

        #region 二维向量平滑阻尼
        /// <summary>
        /// 二维向量平滑阻尼
        /// </summary>
        [ScriptName("二维向量平滑阻尼", nameof(Vector2SmoothDamp))]
        [ScriptDescription("二维向量平滑阻尼；随着时间的推移逐渐改变一个向量朝着期望的目标；阻尼函数是平滑的，不会过量；")]
        [ScriptReturn("成功返回 二维向量移向后的新二维向量 ;失败返回 #False;")]
        [ScriptParams(1, EParamType.Vector2, "当前位置向量：")]
        [ScriptParams(2, EParamType.Vector2, "目标位置向量：")]
        [ScriptParams(3, EParamType.Vector2, "当前速度:")]
        [ScriptParams(4, EParamType.FloatSlider, "期望花费的时间(单位 秒)：", 0f, 999999f)]
        [ScriptParams(5, EParamType.FloatSlider, "最大速度限制(输入0 代表 Mathf.Infinity ，即不限制速度)：", 0f, 999999f)]
        [ScriptParams(6, EParamType.FloatSlider, "函数调用的流逝时间(输入0 代表 Time.deltaTime，单位为 秒)：", 0f, 99f)]
        [ScriptParams(7, EParamType.GlobalVariableName, "执行完成计算后，用于存储新速度值的变量名：")]
        #endregion
        Vector2SmoothDamp,

        #endregion

        #region 三维向量

        #region 三维向量-目录
        /// <summary>
        /// 三维向量
        /// </summary>
        [ScriptName("三维向量", nameof(Vector3), EGrammarType.Category)]
        [ScriptDescription("三维向量操作脚本的目录;")]
        #endregion
        Vector3,

        #region 三维向量计算
        /// <summary>
        /// 三维向量计算
        /// </summary>
        [ScriptName("三维向量计算", nameof(Vector3Calculate))]
        [ScriptDescription("三维向量计算")]
        [ScriptReturn("成功返回 计算结果  ;")]
        [ScriptParams(1, EParamType.Vector3, "三维向量:")]
        [ScriptParams(2, EParamType.Combo, "运算符:", "加", "减")]
        [ScriptParams(3, EParamType.Vector3, "三维向量:")]
        #endregion
        Vector3Calculate,

        #region 构造三维向量
        /// <summary>
        /// 构造三维向量
        /// </summary>
        [ScriptName("构造三维向量", nameof(Vector3Create))]
        [ScriptDescription("构造三维向量;三维向量的 X,Y,Z值使用 / 分割")]
        [ScriptReturn("成功返回 三维向量 ;失败返回 #False;")]
        [ScriptParams(1, EParamType.Vector3, "三维向量:")]
        #endregion
        Vector3Create,

        #region 修改三维向量
        /// <summary>
        /// 修改三维向量
        /// </summary>
        [ScriptName("修改三维向量", nameof(Vector3Modify))]
        [ScriptDescription("修改三维向量;三维向量的 X,Y,Z值使用新值替代，并返回修改后的值；")]
        [ScriptReturn("成功返回 三维向量 ;失败返回 #False;")]
        [ScriptParams(1, EParamType.Vector3, "三维向量：")]
        [ScriptParams(2, EParamType.Combo, "待修改的类型：", "x", "y", "z")]
        [ScriptParams(3, EParamType.Float, "新值：")]
        #endregion
        Vector3Modify,

        #region 三维向量合成
        /// <summary>
        /// 三维向量合成
        /// </summary>
        [ScriptName("三维向量合成", nameof(Vector3Merge))]
        [ScriptDescription("合成三维向量;三维向量的 X,Y,Z值使用 / 分割")]
        [ScriptReturn("成功返回 三维向量 ;失败返回 #False;")]
        [ScriptParams(1, EParamType.FloatSlider, "X:", -99999999f, 99999999f)]
        [ScriptParams(2, EParamType.FloatSlider, "Y:", -99999999f, 99999999f)]
        [ScriptParams(3, EParamType.FloatSlider, "Z:", -99999999f, 99999999f)]
        #endregion
        Vector3Merge,

        #region 三维向量拆分
        /// <summary>
        /// 三维向量拆分
        /// </summary>
        [ScriptName("三维向量拆分", nameof(Vector3Split))]
        [ScriptDescription("拆分三维向量;三维向量的 X,Y,Z值使用 / 分割")]
        [ScriptReturn("成功返回 #True ;失败返回 #False;")]
        [ScriptParams(1, EParamType.Vector3, "三维向量：")]
        [ScriptParams(2, EParamType.GlobalVariableName, "三维向量拆分后存储 X 值的变量名：")]
        [ScriptParams(3, EParamType.GlobalVariableName, "三维向量拆分后存储 Y 值的变量名：")]
        [ScriptParams(4, EParamType.GlobalVariableName, "三维向量拆分后存储 Z 值的变量名：")]
        #endregion
        Vector3Split,

        #region 三维向量长度
        /// <summary>
        /// 三维向量长度
        /// </summary>
        [ScriptName("三维向量长度", nameof(Vector3Magnitude))]
        [ScriptDescription("获取三维向量长度;")]
        [ScriptReturn("成功返回 三维向量长度 ;失败返回 #False;")]
        [ScriptParams(1, EParamType.Vector3, "三维向量：")]
        #endregion
        Vector3Magnitude,

        #region 获取三维向量长度的平方
        /// <summary>
        /// 获取三维向量长度的平方
        /// </summary>
        [ScriptName("获取三维向量长度的平方", nameof(Vector3SqrMagnitude))]
        [ScriptDescription("获取三维向量长度的平方;")]
        [ScriptReturn("成功返回 获取三维向量长度的平方 ;失败返回 #False;")]
        [ScriptParams(1, EParamType.Vector3, "三维向量：")]
        #endregion
        Vector3SqrMagnitude,

        #region 单位化三维向量
        /// <summary>
        /// 单位化三维向量
        /// </summary>
        [ScriptName("单位化三维向量", nameof(Vector3Normalized))]
        [ScriptDescription("单位化三维向量;")]
        [ScriptReturn("成功返回 单位化后的三维向量 ;失败返回 #False;")]
        [ScriptParams(1, EParamType.Vector3, "三维向量：")]
        #endregion
        Vector3Normalized,

        #region 三维向量角度
        /// <summary>
        /// 三维向量角度
        /// </summary>
        [ScriptName("三维向量角度", nameof(Vector3Angle))]
        [ScriptDescription("获取三维向量 From 与 三维向量 To 之间的一个角度;")]
        [ScriptReturn("成功返回 单位化后的三维向量 ;失败返回 #False;")]
        [ScriptParams(1, EParamType.Vector3, "三维向量 From：")]
        [ScriptParams(2, EParamType.Vector3, "三维向量 To：")]
        #endregion
        Vector3Angle,

        #region 三维向量限制长度
        /// <summary>
        /// 三维向量限制长度
        /// </summary>
        [ScriptName("三维向量限制长度", nameof(Vector3ClampMagnitude))]
        [ScriptDescription("三维向量限制长度;返回向量的长度，最大不超过maxLength所指示的长度；也就是说，把向量限制在一个特定的长度；")]
        [ScriptReturn("成功返回 限制长度后的三维向量 ;失败返回 #False;")]
        [ScriptParams(1, EParamType.Vector3, "三维向量：")]
        [ScriptParams(2, EParamType.FloatSlider, "最大长度：", 0f, 999999f)]
        #endregion
        Vector3ClampMagnitude,

        #region 三维向量交叉乘积
        /// <summary>
        /// 三维向量交叉乘积
        /// </summary>
        [ScriptName("三维向量交叉乘积", nameof(Vector3Cross))]
        [ScriptDescription("三维向量交叉乘积;计算两个向量的交叉乘积；即 A X B")]
        [ScriptReturn("成功返回 三维向量交叉乘积的三维向量 ;失败返回 #False;")]
        [ScriptParams(1, EParamType.Vector3, "三维向量 A：")]
        [ScriptParams(2, EParamType.Vector3, "三维向量 B：")]
        #endregion
        Vector3Cross,

        #region 三维向量距离
        /// <summary>
        /// 三维向量距离
        /// </summary>
        [ScriptName("三维向量距离", nameof(Vector3Distance))]
        [ScriptDescription("获取两个三维向量之间的距离；")]
        [ScriptReturn("成功返回 距离值 ;失败返回 #False;")]
        [ScriptParams(1, EParamType.Vector3, "三维向量 A：")]
        [ScriptParams(2, EParamType.Vector3, "三维向量 B：")]
        #endregion
        Vector3Distance,

        #region 三维向量点乘积
        /// <summary>
        /// 三维向量点乘积
        /// </summary>
        [ScriptName("三维向量点乘积", nameof(Vector3Dot))]
        [ScriptDescription("获取两个三维向量的点乘积；")]
        [ScriptReturn("成功返回 点乘积值 ;失败返回 #False;")]
        [ScriptParams(1, EParamType.Vector3, "三维向量 A：")]
        [ScriptParams(2, EParamType.Vector3, "三维向量 B：")]
        #endregion
        Vector3Dot,

        #region 三维向量线性插值
        /// <summary>
        /// 三维向量线性插值
        /// </summary>
        [ScriptName("三维向量线性插值", nameof(Vector3Lerp))]
        [ScriptDescription("获取两个三维向量的线性插值；")]
        [ScriptReturn("成功返回 点乘积值 ;失败返回 #False;")]
        [ScriptParams(1, EParamType.Vector3, "三维向量 A：")]
        [ScriptParams(2, EParamType.Vector3, "三维向量 B：")]
        [ScriptParams(3, EParamType.FloatSlider, "线性插值比例值：", 0f, 1f)]
        #endregion
        Vector3Lerp,

        #region 三维向量非限制线性插值
        /// <summary>
        /// 三维向量非限制线性插值
        /// </summary>
        [ScriptName("三维向量非限制线性插值", nameof(Vector3LerpUnclamped))]
        [ScriptDescription("三维向量非限制线性插值；线性插值比例值 = 0 ，则返回 A；线性插值比例值 = 1 ，则返回 B")]
        [ScriptReturn("成功返回 点乘积值 ;失败返回 #False;")]
        [ScriptParams(1, EParamType.Vector3, "三维向量 A：")]
        [ScriptParams(2, EParamType.Vector3, "三维向量 B：")]
        [ScriptParams(3, EParamType.FloatSlider, "线性插值比例值：", -999999f, 999999f)]
        #endregion
        Vector3LerpUnclamped,

        #region 获取三维向量并集
        /// <summary>
        /// 获取三维向量并集
        /// </summary>
        [ScriptName("获取三维向量并集", nameof(Vector3Max))]
        [ScriptDescription("获取三维向量并集；由两个向量的最大组件组成的向量；")]
        [ScriptReturn("成功返回 并集三维向量 ;失败返回 #False;")]
        [ScriptParams(1, EParamType.Vector3, "三维向量 A：")]
        [ScriptParams(2, EParamType.Vector3, "三维向量 B：")]
        #endregion
        Vector3Max,

        #region 获取三维向量交集
        /// <summary>
        /// 获取三维向量交集
        /// </summary>
        [ScriptName("获取三维向量交集", nameof(Vector3Min))]
        [ScriptDescription("获取三维向量交集；两个向量的最小组件组成的向量；")]
        [ScriptReturn("成功返回 交集三维向量 ;失败返回 #False;")]
        [ScriptParams(1, EParamType.Vector3, "三维向量 A：")]
        [ScriptParams(2, EParamType.Vector3, "三维向量 B：")]
        #endregion
        Vector3Min,

        #region 三维向量移向
        /// <summary>
        /// 三维向量移向
        /// </summary>
        [ScriptName("三维向量移向", nameof(Vector3MoveTowards))]
        [ScriptDescription("三维向量移向；最大移动偏移量为 如果为正值，当前地点移向目标，如果是负值当前地点将远离目标；")]
        [ScriptReturn("成功返回 移向后的三维向量 ;失败返回 #False;")]
        [ScriptParams(1, EParamType.Vector3, "当前向量：")]
        [ScriptParams(2, EParamType.Vector3, "目标向量：")]
        [ScriptParams(3, EParamType.FloatSlider, "最大移动偏移量：", -999999f, 999999f)]
        #endregion
        Vector3MoveTowards,

        #region 三维向量投影
        /// <summary>
        /// 三维向量投影
        /// </summary>
        [ScriptName("三维向量投影", nameof(Vector3Project))]
        [ScriptDescription("三维向量投影;")]
        [ScriptReturn("成功返回 三维向量投影计算后的三维向量 ;失败返回 #False;")]
        [ScriptParams(1, EParamType.Vector3, "三维向量 A：")]
        [ScriptParams(2, EParamType.Vector3, "法向量 N：")]
        #endregion
        Vector3Project,

        #region 三维向量投影(平面)
        /// <summary>
        /// 三维向量投影(平面)
        /// </summary>
        [ScriptName("三维向量投影(平面)", nameof(Vector3ProjectOnPlane))]
        [ScriptDescription("三维向量投影(平面);")]
        [ScriptReturn("成功返回 三维向量平面投影计算后的三维向量 ;失败返回 #False;")]
        [ScriptParams(1, EParamType.Vector3, "三维向量 A：")]
        [ScriptParams(2, EParamType.Vector3, "平面法向量 N：")]
        #endregion
        Vector3ProjectOnPlane,

        #region 三维向量反射
        /// <summary>
        /// 三维向量反射
        /// </summary>
        [ScriptName("三维向量反射", nameof(Vector3Reflect))]
        [ScriptDescription("三维向量反射；V对于法线N的反射；")]
        [ScriptReturn("成功返回 反射计算后的三维向量 ;失败返回 #False;")]
        [ScriptParams(1, EParamType.Vector3, "向量V：")]
        [ScriptParams(2, EParamType.Vector3, "法线N向量：")]
        #endregion
        Vector3Reflect,

        #region 三维向量转向
        /// <summary>
        /// 三维向量转向
        /// </summary>
        [ScriptName("三维向量转向", nameof(Vector3RotateTowards))]
        [ScriptDescription("三维向量转向；最大移动偏移量为 如果为正值，当前地点移向目标，如果是负值当前地点将远离目标；")]
        [ScriptReturn("成功返回 移向后的三维向量 ;失败返回 #False;")]
        [ScriptParams(1, EParamType.Vector3, "当前向量：")]
        [ScriptParams(2, EParamType.Vector3, "目标向量：")]
        [ScriptParams(3, EParamType.FloatSlider, "最大角度偏移量：", -999999f, 999999f)]
        [ScriptParams(4, EParamType.FloatSlider, "最大距离偏移量：", -999999f, 999999f)]
        #endregion
        Vector3RotateTowards,

        #region 三维向量缩放
        /// <summary>
        /// 三维向量缩放
        /// </summary>
        [ScriptName("三维向量缩放", nameof(Vector3Scale))]
        [ScriptDescription("三维向量缩放；两个向量组件对应X,Y相乘；")]
        [ScriptReturn("成功返回 缩放后的三维向量 ;失败返回 #False;")]
        [ScriptParams(1, EParamType.Vector3, "当前向量：")]
        [ScriptParams(2, EParamType.Vector3, "缩放系数向量：")]
        #endregion
        Vector3Scale,

        #region 三维向量球形插值
        /// <summary>
        /// 三维向量球形插值
        /// </summary>
        [ScriptName("三维向量球形插值", nameof(Vector3Slerp))]
        [ScriptDescription("获取两个三维向量的球形插值；")]
        [ScriptReturn("成功返回 点乘积值 ;失败返回 #False;")]
        [ScriptParams(1, EParamType.Vector3, "三维向量 A：")]
        [ScriptParams(2, EParamType.Vector3, "三维向量 B：")]
        [ScriptParams(3, EParamType.FloatSlider, "球形插值比例值：", 0f, 1f)]
        #endregion
        Vector3Slerp,

        #region 三维向量非限制球形插值
        /// <summary>
        /// 三维向量非限制球形插值
        /// </summary>
        [ScriptName("三维向量非限制球形插值", nameof(Vector3SlerpUnclamped))]
        [ScriptDescription("三维向量非限制球形插值；线性插值比例值 = 0 ，则返回 A；线性插值比例值 = 1 ，则返回 B")]
        [ScriptReturn("成功返回 点乘积值 ;失败返回 #False;")]
        [ScriptParams(1, EParamType.Vector3, "三维向量 A：")]
        [ScriptParams(2, EParamType.Vector3, "三维向量 B：")]
        [ScriptParams(3, EParamType.FloatSlider, "球形插值比例值：", -999999f, 999999f)]
        #endregion
        Vector3SlerpUnclamped,

        #region 三维向量平滑阻尼
        /// <summary>
        /// 三维向量平滑阻尼
        /// </summary>
        [ScriptName("三维向量平滑阻尼", nameof(Vector3SmoothDamp))]
        [ScriptDescription("三维向量平滑阻尼；随着时间的推移逐渐改变一个向量朝着期望的目标；阻尼函数是平滑的，不会过量；")]
        [ScriptReturn("成功返回 三维向量移向后的新三维向量 ;失败返回 #False;")]
        [ScriptParams(1, EParamType.Vector3, "当前位置向量：")]
        [ScriptParams(2, EParamType.Vector3, "目标位置向量：")]
        [ScriptParams(3, EParamType.Vector3, "当前速度:")]
        [ScriptParams(4, EParamType.FloatSlider, "期望花费的时间(单位 秒)：", 0f, 999999f)]
        [ScriptParams(5, EParamType.FloatSlider, "最大速度限制(输入0 代表 Mathf.Infinity ，即不限制速度)：", 0f, 999999f)]
        [ScriptParams(6, EParamType.FloatSlider, "函数调用的流逝时间(输入0 代表 Time.deltaTime，单位为 秒)：", 0f, 99f)]
        [ScriptParams(7, EParamType.GlobalVariableName, "执行完成计算后，用于存储新速度值的变量名：")]
        #endregion
        Vector3SmoothDamp,

        #endregion

        #region 输入

        #region 输入-目录
        /// <summary>
        /// 输入
        /// </summary>
        [ScriptName("输入", nameof(Input), EGrammarType.Category)]
        [ScriptDescription("输入操作脚本的目录")]
        #endregion
        Input,

        #region 获取鼠标位置
        /// <summary>
        /// 获取鼠标位置
        /// </summary>
        [ScriptName("获取鼠标位置", nameof(GetMousePos))]
        [ScriptDescription("获取鼠标屏幕坐标位置；只能在Script Mono Behaviour Event的\"更新时 执行\"事件中使用；")]
        [ScriptReturn("成功返回 #True  ;失败返回 #False;")]
        #endregion
        GetMousePos,

        #region 屏幕转世界位置
        /// <summary>
        /// 屏幕转世界位置
        /// </summary>
        [ScriptName("屏幕转世界位置", nameof(ScreenToWorldPoint))]
        [ScriptDescription("屏幕转世界位置；从屏幕空间到世界空间的变化位置，屏幕空间以像素定义。屏幕的左下为(0,0)；右上是(pixelWidth,pixelHeight)，Z的位置是以世界单位衡量的到相机的距离；只能在Script Mono Behaviour Event的\"更新时 执行\"事件中使用；")]
        [ScriptReturn("成功返回 #True  ;失败返回 #False;")]
        [ScriptParams(1, EParamType.Vector3, "屏幕坐标与距离:")]
        #endregion
        ScreenToWorldPoint,

        #region 获取输入-按键状态
        /// <summary>
        /// 获取输入-按键状态
        /// </summary>
        [ScriptName("获取输入-按键状态", nameof(InputGetKey))]
        [ScriptDescription("获取输入-按键状态；获取键盘的按键输入信息，检测某个按键是否被按下的状态；推荐在Mono事件脚本的 更新时执行 函数中使用；")]
        [ScriptReturn("成功返回 #True;失败返回 #False;")]
        [ScriptParams(1, EParamType.KeyCode, "按键:")]
        [ScriptParams(2, EParamType.Combo, "按键状态:", "按键按下", "按键按下中", "按键弹起")]
        #endregion
        InputGetKey,

        #region 如果输入-按键
        /// <summary>
        /// 如果输入-按键
        /// </summary>
        [ScriptName("如果输入-按键", nameof(IfInputGetKey), EGrammarType.If)]
        [ScriptDescription("如果输入-按键；判断键盘的按键输入信息，检测某个按键是否被按下的状态；推荐在Mono事件脚本的 更新时执行 函数中使用；判断开始标记；")]
        [ScriptReturn("成功返回 #True;失败返回 #False;")]
        [ScriptParams(1, EParamType.KeyCode, "按键:")]
        [ScriptParams(2, EParamType.Combo, "按键状态:", "按键按下", "按键按下中", "按键弹起")]
        [ScriptParams(3, EParamType.Combo, "判断条件:", "有效", "无效")]
        #endregion
        IfInputGetKey,

        #region 否则如果输入-按键
        /// <summary>
        /// 否则如果输入-按键
        /// </summary>
        [ScriptName("否则如果输入-按键", nameof(ElseIfInputGetKey), EGrammarType.ElseIf)]
        [ScriptDescription("否则如果输入-按键；判断键盘的按键输入信息，检测某个按键是否被按下的状态；推荐在Mono事件脚本的 更新时执行 函数中使用；判断开始标记；")]
        [ScriptReturn("成功返回 #True;失败返回 #False;")]
        [ScriptParams(1, EParamType.KeyCode, "按键:")]
        [ScriptParams(2, EParamType.Combo, "按键状态:", "按键按下", "按键按下中", "按键弹起")]
        [ScriptParams(3, EParamType.Combo, "判断条件:", "有效", "无效")]
        #endregion
        ElseIfInputGetKey,

        #region 获取输入-鼠标按钮状态
        /// <summary>
        /// 获取输入-鼠标按钮状态
        /// </summary>
        [ScriptName("获取输入-鼠标按钮状态", nameof(InputGetMouseButton))]
        [ScriptDescription("获取输入-鼠标按钮状态；获取按钮是否被按下的状态；；推荐在Mono事件脚本的 更新时执行 函数中使用；")]
        [ScriptReturn("成功返回 #True;失败返回 #False;")]
        [ScriptParams(1, EParamType.MouseButton, "鼠标按钮:")]
        [ScriptParams(2, EParamType.Combo, "鼠标按钮状态:", "按钮按下", "按钮按下中", "按钮键弹起")]
        #endregion
        InputGetMouseButton,

        #region 如果输入-鼠标按钮
        /// <summary>
        /// 如果输入-鼠标按钮
        /// </summary>
        [ScriptName("如果输入-鼠标按钮", nameof(IfInputGetMouseButton), EGrammarType.If)]
        [ScriptDescription("如果输入-鼠标按钮；判断按钮是否被按下的状态；推荐在Mono事件脚本的 更新时执行 函数中使用；判断开始标记；")]
        [ScriptReturn("成功返回 #True;失败返回 #False;")]
        [ScriptParams(1, EParamType.MouseButton, "鼠标按钮:")]
        [ScriptParams(2, EParamType.Combo, "鼠标按钮状态:", "按钮按下", "按钮按下中", "按钮键弹起")]
        [ScriptParams(3, EParamType.Combo, "判断条件:", "有效", "无效")]
        #endregion
        IfInputGetMouseButton,

        #region 否则如果输入-鼠标按钮
        /// <summary>
        /// 否则如果输入-鼠标按钮
        /// </summary>
        [ScriptName("否则如果输入-鼠标按钮", nameof(ElseIfInputGetMouseButton), EGrammarType.If)]
        [ScriptDescription("否则如果输入-鼠标按钮；判断按钮是否被按下的状态；推荐在Mono事件脚本的 更新时执行 函数中使用；判断开始标记；")]
        [ScriptReturn("成功返回 #True;失败返回 #False;")]
        [ScriptParams(1, EParamType.MouseButton, "鼠标按钮:")]
        [ScriptParams(2, EParamType.Combo, "鼠标按钮状态:", "按钮按下", "按钮按下中", "按钮键弹起")]
        [ScriptParams(3, EParamType.Combo, "判断条件:", "有效", "无效")]
        #endregion
        ElseIfInputGetMouseButton,

        #region 获取点击点游戏对象
        /// <summary>
        /// 获取点击点游戏对象
        /// </summary>
        [ScriptName("获取点击点游戏对象", nameof(GetClickPointGameObject))]
        [ScriptDescription("获取点击点游戏对象；使用射线检测的第一个可视的游戏对象(必须带碰撞体组件)；射线检测不做距离限制；推荐在Script Mono Behaviour Event的\"更新时 执行\"事件中使用；")]
        [ScriptReturn("成功返回 点击点的游戏对象的描述字符串； 失败返回 #False;")]
        #endregion
        GetClickPointGameObject,

        #region 获取点击点三维坐标
        /// <summary>
        /// 获取点击点三维坐标
        /// </summary>
        [ScriptName("获取点击点三维坐标", nameof(GetClickPoint3DPos))]
        [ScriptDescription("获取点击点三维坐标；使用射线检测，射线与游戏对象(必须带碰撞体组件)的可视交点的三维坐标；射线检测不做距离限制；推荐在Script Mono Behaviour Event的\"更新时 执行\"事件中使用；")]
        [ScriptReturn("成功返回 点击点三维坐标 ;失败返回 #False;")]
        #endregion
        GetClickPoint3DPos,

        #region 获取点击信息
        /// <summary>
        /// 获取点击信息
        /// </summary>
        [ScriptName("获取点击信息", nameof(GetClickInfo))]
        [ScriptDescription("获取点击信息；鼠标左键点击或触摸时可以检测到；使用主相机做射线检测;使用射线检测，在当前可视距离内查找到的各种信息；推荐在Script Mono Behaviour Event的\"更新时 执行\"事件中使用；")]
        [ScriptReturn("成功返回 点击点三维坐标  ;失败返回 #False;")]
        [ScriptParams(1, EParamType.Combo, "信息类型:", "屏幕坐标", "世界坐标", "射线起点坐标", "射线方向", "三维坐标(限定距离)", "游戏对象", "三维坐标(与游戏对象的碰撞点)", "交点三角面法向量", "交点三角面城重心坐标", "起点到碰撞点的距离", "碰撞点的UV纹理坐标", "碰撞点的第二个UV纹理坐标", "碰撞器组件", "碰撞的刚体组件", "碰撞的Transform组件")]
        [ScriptParams(2, EParamType.FloatSlider, "距离(射线检测)：", 0f, 9999f)]
        #endregion
        GetClickInfo,

        #region 获取屏幕坐标点信息
        /// <summary>
        /// 获取屏幕坐标点信息
        /// </summary>
        [ScriptName("获取屏幕坐标点信息", nameof(GetScreenPointInfo))]
        [ScriptDescription("获取屏幕坐标点信息；使用主相机做射线检测;使用射线检测，在当前可视距离内查找到的各种信息；推荐在Script Mono Behaviour Event的\"更新时 执行\"事件中使用；")]
        [ScriptReturn("成功返回 点击点三维坐标  ;失败返回 #False;")]
        [ScriptParams(1, EParamType.Vector3, "屏幕坐标点信息(Z 坐标默认设置设置为 0 即可):")]
        [ScriptParams(2, EParamType.Combo, "信息类型:", "屏幕坐标", "世界坐标", "射线起点坐标", "射线方向", "三维坐标(限定距离)", "游戏对象", "三维坐标(与游戏对象的碰撞点)", "交点三角面法向量", "交点三角面城重心坐标", "起点到碰撞点的距离", "碰撞点的UV纹理坐标", "碰撞点的第二个UV纹理坐标", "碰撞器组件", "碰撞的刚体组件", "碰撞的Transform组件")]
        [ScriptParams(3, EParamType.FloatSlider, "距离(射线检测)：", 0f, 9999f)]
        #endregion
        GetScreenPointInfo,

        #endregion

        #region 文件

        #region 文件-目录
        /// <summary>
        /// 文件
        /// </summary>
        [ScriptName("文件", nameof(File), EGrammarType.Category)]
        [ScriptDescription("文件路径操作脚本的目录")]
        #endregion
        File,

        #region 文件删除
        /// <summary>
        /// 文件删除
        /// </summary>
        [ScriptName("文件删除", nameof(FileDelete))]
        [ScriptDescription("文件删除")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.File, "文件路径(本地路径):")]
        #endregion
        FileDelete,

        #region 文件夹删除
        /// <summary>
        /// 文件夹删除
        /// </summary>
        [ScriptName("文件夹删除", nameof(FolderDelete))]
        [ScriptDescription("文件夹删除")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.OpenFolder, "文件夹路径:")]
        [ScriptParams(2, EParamType.Combo, "操作:", "删除空文件夹", "删除文件夹以及文件夹内的全部子文件夹与文件")]
        #endregion
        FolderDelete,

        #region 获取文件路径信息
        /// <summary>
        /// 获取文件路径信息
        /// </summary>
        [ScriptName("获取文件路径信息", nameof(GetFilePathInfo))]
        [ScriptDescription("获取文件路径信息；")]
        [ScriptReturn("成功返回 对应的请求信息 ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.File, "文件路径:")]
        [ScriptParams(2, EParamType.Combo, "操作:", "路径", "文件名", "文件扩展名", "文件名(含扩展名)", "绝对路径", "是否存在")]
        #endregion
        GetFilePathInfo,

        #region 获取文件文本
        /// <summary>
        /// 获取文件文本
        /// </summary>
        [ScriptName("获取文件文本", nameof(GetFileText))]
        [ScriptDescription("获取文件文本；文件路径可以为本地路径或以http开头的网络路径；")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.File, "文件路径(本地路径或http开头的网络路径):")]
        [ScriptParams(2, EParamType.GlobalVariableName, "存储文本信息的变量:")]
        [ScriptParams(3, EParamType.UserDefineFun, "文本信息加载文成后的回调函数:")]
        [ScriptParams(4, EParamType.String, "回调函数的额外信息(如果请求失败本信息会被替换为 #False):")]
        #endregion
        GetFileText,

        #region 设置文件文本
        /// <summary>
        /// 设置文件文本
        /// </summary>
        [ScriptName("设置文件文本", nameof(SetFileText))]
        [ScriptDescription("设置文件文本；文件默认以UTF-8模式输出；文件路径仅可以为本地路径，且该路径必须有写权限；新建，如文件不存在则新建并写入，如文件已经存在则不写入；覆盖，如文件不存在则不写入，如文件存在则覆盖写入；新建或覆盖，有则覆盖，无则新建并写入；")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.File, "文件路径(本地路径):")]
        [ScriptParams(2, EParamType.String, "文本:")]
        [ScriptParams(3, EParamType.Combo, "文件生成规则", "新建或覆盖", "新建", "覆盖", defaultObject = "新建或覆盖")]
        [ScriptParams(4, EParamType.Bool2, "携带UTF-8 BOM前缀", defaultObject = EBool2.No)]
        #endregion
        SetFileText,

        #region 文件查找
        /// <summary>
        /// 文件查找
        /// </summary>
        [ScriptName("文件查找", nameof(FileSearch))]
        [ScriptDescription("文件查找；仅返回第一个查找到的有效文件路径；")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.File, "文件路径（本地路径）:")]
        [ScriptParams(2, EParamType.GlobalVariableName, "存储文件路径的变量:")]
        [ScriptParams(3, EParamType.Combo, "文件查找规则:", "绝对路径", "根据文件名智能查找", "通配符查找")]
        #endregion
        FileSearch,

        #region 获取路径基础信息
        /// <summary>
        /// 获取路径基础信息
        /// </summary>
        [ScriptName("获取路径基础信息", nameof(GetPathBaseInfo))]
        [ScriptDescription("获取路径基础信息；ALT目录分隔符,提供平台特定的替换字符，该替换字符用于在反映分层文件系统组织的路径字符串中分隔目录级别，如‘/’;目录分隔符,提供平台特定的字符，该字符用于在反映分层文件系统组织的路径字符串中分隔目录级别，如‘\\’;路径分隔符,用于在环境变量中分隔路径字符串的平台特定的分隔符，如‘;’;卷分隔符,提供平台特定的卷分隔符，如‘:’;")]
        [ScriptReturn("成功返回 对应的请求信息 ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.Combo, "信息类型:", "ALT目录分隔符", "目录分隔符", "路径分隔符", "卷分隔符")]
        #endregion
        GetPathBaseInfo,

        #endregion

        #region 网络摄像设备

        #region 网络摄像设备-目录
        /// <summary>
        /// 网络摄像设备
        /// </summary>
        [ScriptName("网络摄像设备", nameof(WebCamDevice), EGrammarType.Category)]
        [ScriptDescription("网络摄像设备操作脚本的目录")]
        #endregion
        WebCamDevice,

        #region 获取网络摄像设备数目
        /// <summary>
        /// 获取网络摄像设备数目
        /// </summary>
        [ScriptName("获取网络摄像设备数目", nameof(GetWebCamDeviceCount))]
        [ScriptDescription("获取当前设备上摄像头的设备数目")]
        [ScriptReturn("成功返回设备数目 ，无用户授权或设备上无有效摄像头则返回 #False")]
        #endregion
        GetWebCamDeviceCount,

        #region 获取网络摄像设备名称
        /// <summary>
        /// 获取网络摄像设备名称
        /// </summary>
        [ScriptName("获取网络摄像设备名称", nameof(GetWebCamDeviceName))]
        [ScriptDescription("获取当前设备上某个摄像头的设备描述名称;执行效率很低，切勿在循环操作的函数中频繁调用；")]
        [ScriptReturn("成功返回设备名称，无用户授权或不存在该摄像设备返回 #False")]
        [ScriptParams(1, EParamType.IntSlider, "网络摄像设备索引:", 1, 10, defaultObject = 1)]
        #endregion
        GetWebCamDeviceName,

        #region 创建网络摄像纹理
        /// <summary>
        /// 创建网络摄像纹理
        /// </summary>
        [ScriptName("创建网络摄像纹理", nameof(CreateWebCamTexture))]
        [ScriptDescription("根据网络摄像设备名称创建网络摄像纹理，摄像头拍摄的画面会实时更新到该纹理上；必须经过了用户授权；")]
        [ScriptReturn("成功返回 #True，失败返回 #False")]
        [ScriptParams(1, EParamType.String, "网络摄像设备名称:")]
        [ScriptParams(2, EParamType.IntSlider, "请求的画面宽度:", 1, 1920, defaultObject = 640)]
        [ScriptParams(3, EParamType.IntSlider, "请求的画面高度:", 1, 1080, defaultObject = 480)]
        [ScriptParams(4, EParamType.IntSlider, "请求的画面FPS:", 1, 120, defaultObject = 24)]
        #endregion
        CreateWebCamTexture,

        #region 获取网络摄像纹理信息
        /// <summary>
        /// 获取网络摄像纹理信息
        /// </summary>
        [ScriptName("获取网络摄像纹理信息", nameof(GetWebCamTextureInfo))]
        [ScriptDescription("获取网络摄像纹理的一些基础信息")]
        [ScriptReturn("成功返回 对应的请求信息，失败返回 #False")]
        [ScriptParams(1, EParamType.String, "网络摄像设备名称:")]
        [ScriptParams(2, EParamType.Combo, "信息类型：", "名称", "设备名", "本帧画面缓冲区是否更新", "是否播放中", "帧速率FPS", "画面宽度", "画面高度", "视频旋转角度", "视频垂直镜像")]
        #endregion
        GetWebCamTextureInfo,

        #region 控制网络摄像纹理
        /// <summary>
        /// 控制网络摄像纹理
        /// </summary>
        [ScriptName("控制网络摄像纹理", nameof(ControlWebCamTexture))]
        [ScriptDescription("控制网络摄像纹理")]
        [ScriptReturn("成功返回 #True，失败返回 #False")]
        [ScriptParams(1, EParamType.String, "网络摄像设备名称:")]
        [ScriptParams(2, EParamType.Combo, "操作：", "播放", "暂停", "停止", "销毁")]
        #endregion，
        ControlWebCamTexture,

        #region 控制全部网络摄像纹理
        /// <summary>
        /// 控制全部网络摄像纹理
        /// </summary>
        [ScriptName("控制全部网络摄像纹理", nameof(ControlAllWebCamTexture))]
        [ScriptDescription("控制全部网络摄像纹理")]
        [ScriptReturn("成功返回 #True，失败返回 #False")]
        [ScriptParams(1, EParamType.Combo, "操作：", "播放", "暂停", "停止", "销毁")]
        #endregion，
        ControlAllWebCamTexture,

        #region 绘制网络摄像纹理
        /// <summary>
        /// 绘制网络摄像纹理
        /// </summary>
        [ScriptName("绘制网络摄像纹理", nameof(DrawWebCamTexture))]
        [ScriptDescription("绘制网络摄像纹理;只能在脚本MonoBehaviour事件中调用;源图片的长宽比，如果为0，则使用图像的长宽比。通过\"宽/高\"获得所需的长宽比，这允许源图像的宽高比被调整而不影响像素宽度和高度")]
        [ScriptReturn("成功返回 #True，失败返回 #False")]
        [ScriptParams(1, EParamType.String, "网络摄像设备名称:")]
        [ScriptParams(2, EParamType.Rect, "坐标位置与尺寸信息:")]
        [ScriptParams(3, EParamType.Combo, "缩放模式:", "StretchToFill", "ScaleAndCrop", "ScaleToFit")]
        [ScriptParams(4, EParamType.Combo, "是否通道混合图片显示:", "是", "否")]
        [ScriptParams(5, EParamType.FloatSlider, "源图片的长宽比（如果为0，则使用图像的长宽比）:", 0f, 10f)]
        #endregion，
        DrawWebCamTexture,

        #region 绘制网络摄像纹理(自动尺寸)
        /// <summary>
        /// 绘制网络摄像纹理
        /// </summary>
        [ScriptName("绘制网络摄像纹理(自动尺寸)", nameof(DrawWebCamTextureWithAutoSize))]
        [ScriptDescription("绘制网络摄像纹理(自动尺寸);只能在脚本MonoBehaviour事件中调用;源图片的长宽比，如果为0，则使用图像的长宽比。通过\"宽/高\"获得所需的长宽比，这允许源图像的宽高比被调整而不影响像素宽度和高度;会根据网络摄像设备的尺寸值设定渲染结果的尺寸值；")]
        [ScriptReturn("成功返回 #True，失败返回 #False")]
        [ScriptParams(1, EParamType.String, "网络摄像设备名称:")]
        [ScriptParams(2, EParamType.Vector2, "坐标位置:")]
        [ScriptParams(3, EParamType.Combo, "缩放模式:", "StretchToFill", "ScaleAndCrop", "ScaleToFit")]
        [ScriptParams(4, EParamType.Combo, "是否通道混合图片显示:", "是", "否")]
        [ScriptParams(5, EParamType.FloatSlider, "源图片的长宽比（如果为0，则使用图像的长宽比）:", 0f, 10f)]
        #endregion，
        DrawWebCamTextureWithAutoSize,

        #endregion

        #region 数组

        #region 数组-目录
        /// <summary>
        /// 数组
        /// </summary>
        [ScriptName("数组", nameof(Array), EGrammarType.Category)]
        #endregion
        Array,

        #region 添加数组
        /// <summary>
        /// 添加数组
        /// </summary>
        [ScriptName("添加数组", nameof(AddArray))]
        [ScriptDescription("添加数组；")]
        [ScriptReturn("成功返回 #True  ;失败返回 #False;")]
        [ScriptParams(1, EParamType.Array, "数组名:")]
        #endregion
        AddArray,

        #region 删除数组
        /// <summary>
        /// 删除数组
        /// </summary>
        [ScriptName("删除数组", nameof(RemoveArray))]
        [ScriptDescription("删除数组；")]
        [ScriptReturn("成功返回 #True  ;失败返回 #False;")]
        [ScriptParams(1, EParamType.Array, "数组名:")]
        #endregion
        RemoveArray,

        #region 获取数组长度
        /// <summary>
        /// 获取数组长度
        /// </summary>
        [ScriptName("获取数组长度", nameof(GetArrayItemCount))]
        [ScriptDescription("获取数组长度；")]
        [ScriptReturn("成功返回 #True  ;失败返回 #False;")]
        [ScriptParams(1, EParamType.Array, "数组名:")]
        #endregion
        GetArrayItemCount,

        #region 设置数组长度
        /// <summary>
        /// 设置数组长度
        /// </summary>
        [ScriptName("设置数组长度", nameof(SetArrayItemCount))]
        [ScriptDescription("设置数组长度；末尾操作，多删除，少补空字符串；")]
        [ScriptReturn("成功返回 #True  ;失败返回 #False;")]
        [ScriptParams(1, EParamType.Array, "数组名:")]
        [ScriptParams(2, EParamType.IntSlider, "数组元素个数[0,1000]:", 0, 1000)]
        #endregion
        SetArrayItemCount,

        #region 添加数组数据
        /// <summary>
        /// 添加数组数据
        /// </summary>
        [ScriptName("添加数组数据", nameof(AddArrayItem))]
        [ScriptDescription("添加数组数据；")]
        [ScriptReturn("成功返回 #True  ;失败返回 #False;")]
        [ScriptParams(1, EParamType.Array, "数组名:")]
        [ScriptParams(2, EParamType.String, "数据：")]
        [ScriptParams(3, EParamType.Combo, "操作:", "数组末尾添加", "数据存在则不添加")]
        #endregion
        AddArrayItem,

        #region 删除数组数据(按数据值)
        /// <summary>
        /// 删除数组数据(按数据值)
        /// </summary>
        [ScriptName("删除数组数据(按数据值)", nameof(RemoveArrayItemByValue))]
        [ScriptDescription("删除数组数据(按数据值)；")]
        [ScriptReturn("成功返回 #True  ;失败返回 #False;")]
        [ScriptParams(1, EParamType.Array, "数组名:")]
        [ScriptParams(2, EParamType.String, "数据值：")]
        [ScriptParams(3, EParamType.Combo, "操作：", "根据数据值删除所有重复数据", "根据数据值删除正向第一个查找到的数据", "根据数据值删除反向第一个查找到的数据")]
        #endregion
        RemoveArrayItemByValue,

        #region 删除数组数据(按索引)
        /// <summary>
        /// 删除数组数据(按索引)
        /// </summary>
        [ScriptName("删除数组数据(按索引)", nameof(RemoveArrayItemByIndex))]
        [ScriptDescription("删除数组数据(按索引)；")]
        [ScriptReturn("成功返回 #True  ;失败返回 #False;")]
        [ScriptParams(1, EParamType.Array, "数组名:")]
        [ScriptParams(2, EParamType.IntSlider, "索引[1,数组长度]：", 1, 1000)]
        [ScriptParams(3, EParamType.Combo, "操作:", "根据索引删除数组中的值", "删除数组中第一个数据(忽略索引值)", "删除数组中最后一个数据(忽略索引值)")]
        #endregion
        RemoveArrayItemByIndex,

        #region 清空数组数据
        /// <summary>
        /// 清空数组数据
        /// </summary>
        [ScriptName("清空数组数据", nameof(ClearArrayItem))]
        [ScriptDescription("清空数组数据；")]
        [ScriptReturn("成功返回 #True  ;失败返回 #False;")]
        [ScriptParams(1, EParamType.Array, "数组名:")]
        #endregion
        ClearArrayItem,

        #region 获取数组数据
        /// <summary>
        /// 获取数组数据
        /// </summary>
        [ScriptName("获取数组数据", nameof(GetArrayItemByIndex))]
        [ScriptDescription("获取数组数据；")]
        [ScriptReturn("成功返回 对应的数据信息  ;失败返回 #False;")]
        [ScriptParams(1, EParamType.Array, "数组名:")]
        [ScriptParams(2, EParamType.IntSlider, "索引[1,数组长度]：", 1, 1000)]
        #endregion
        GetArrayItemByIndex,

        #region 设置数组数据
        /// <summary>
        /// 设置数组数据
        /// </summary>
        [ScriptName("设置数组数据", nameof(SetArrayItem))]
        [ScriptDescription("设置数组数据；")]
        [ScriptReturn("成功返回 #True  ;失败返回 #False;")]
        [ScriptParams(1, EParamType.Array, "数组名:")]
        [ScriptParams(2, EParamType.IntSlider, "索引[1,数组长度]：", 1, 1000)]
        [ScriptParams(3, EParamType.String, "数据值：")]
        #endregion
        SetArrayItem,

        #region 检查数组数据
        /// <summary>
        /// 检查数组数据
        /// </summary>
        [ScriptName("检查数组数据", nameof(CheckArrayItem))]
        [ScriptDescription("检查数组数据；检查数据是否存在于数组数据值中；")]
        [ScriptReturn("成功返回 #True  ;失败返回 #False;")]
        [ScriptParams(1, EParamType.Array, "数组名:")]
        [ScriptParams(2, EParamType.String, "数据值：")]
        #endregion
        CheckArrayItem,

        #region 数组基本操作
        /// <summary>
        /// 数组基本操作
        /// </summary>
        [ScriptName("数组基本操作", nameof(ArrayBaseOperation))]
        [ScriptDescription("数组基本操作；")]
        [ScriptReturn("成功返回 #True  ;失败返回 #False;")]
        [ScriptParams(1, EParamType.Array, "数组名:")]
        [ScriptParams(2, EParamType.Combo, "操作:", "去除重复值", "排序(升序)", "排序(降序)", "转置(前后顺序反转)")]
        #endregion
        ArrayBaseOperation,

        #region 字符串转化为数组
        /// <summary>
        /// 字符串转化为数组
        /// </summary>
        [ScriptName("字符串转化为数组", nameof(ConvertStringToArray))]
        [ScriptDescription("字符串转化为数组；对应数组名，数组存在则追加，数组不存在则新建数组后添加；")]
        [ScriptReturn("成功返回 #True  ;失败返回 #False;")]
        [ScriptParams(1, EParamType.String, "字符串:")]
        [ScriptParams(2, EParamType.Array, "数组名:")]
        [ScriptParams(3, EParamType.String, "字符串分割符:")]
        [ScriptParams(4, EParamType.Combo, "操作:", "直接追加", "去除空字符后追加", "清空数组数据后直接追加", "请空数组数据并去除空字符后追加")]
        #endregion
        ConvertStringToArray,

        #region 数组转化为字符串
        /// <summary>
        /// 数组转化为字符串
        /// </summary>
        [ScriptName("数组转化为字符串", nameof(ConvertArrayToString))]
        [ScriptDescription("数组转化为字符串；")]
        [ScriptReturn("成功返回 #True  ;失败返回 #False;")]
        [ScriptParams(1, EParamType.Array, "数组名:")]
        [ScriptParams(2, EParamType.String, "字符串分割符:")]
        #endregion
        ConvertArrayToString,

        #region 数组合并
        /// <summary>
        /// 数组合并
        /// </summary>
        [ScriptName("数组合并", nameof(ArrayMerge))]
        [ScriptDescription("数组合并；在A、B为同一个数组情况下:如C是A或B,则C数组中数据出现3份;如C不是A或B，无则添加新数组，有则原数组追加数据。在A、B为两个数组情况下:如C是A或B,则直接修改数组A或B中的值,只单纯追加另一个数组的数据,即数据出现2份；如C不是A或B，无则添加新数组，有则原数组追加数据。默认先添加A后添加B;")]
        [ScriptReturn("成功返回 #True  ;失败返回 #False;")]
        [ScriptParams(1, EParamType.Array, "数组名A:")]
        [ScriptParams(2, EParamType.Array, "数组名B:")]
        [ScriptParams(3, EParamType.Array, "合并后数组名C:")]
        #endregion
        ArrayMerge,

        #region 数组拷贝
        /// <summary>
        /// 数组拷贝
        /// </summary>
        [ScriptName("数组拷贝", nameof(ArrayClone))]
        [ScriptDescription("数组拷贝；数组名B为A则失败；数组名B已经存在，会将B中原数据清空，后再克隆数据；数组B不存在，则创建B数组后克隆数据；")]
        [ScriptReturn("成功返回 #True  ;失败返回 #False;")]
        [ScriptParams(1, EParamType.Array, "数组名A:")]
        [ScriptParams(2, EParamType.Array, "拷贝到数组名B:")]
        #endregion
        ArrayClone,

        #region 数组排序
        /// <summary>
        /// 数组排序
        /// </summary>
        [ScriptName("数组排序", nameof(ArraySort))]
        [ScriptDescription("数组排序；会直接修改原数组的信息；")]
        [ScriptReturn("成功返回 #True  ;失败返回 #False;")]
        [ScriptParams(1, EParamType.Array, "数组名:")]
        [ScriptParams(2, EParamType.Combo, "比较规则:", "字符串比较", "转化整形数值后比较", "转化浮点数值后比较", "自然比较")]
        [ScriptParams(3, EParamType.Combo, "排序规则:", "升序", "降序")]
        [ScriptParams(4, EParamType.Combo, "部分数组元素无法转化为数值时(使用字符串进行比较)，字符串与数值的前后顺序:", "数值前字符串后", "字符串前数值后")]
        #endregion
        ArraySort,

        #region 数组循环开始
        /// <summary>
        /// 数组循环开始，遍历数组中的元素
        /// </summary>
        [ScriptName("数组循环开始", nameof(WhileArray), EGrammarType.Loop)]
        [ScriptDescription("数组循环开始；如果数组不存在、变量名无效等会返回失败；索引变量与值变量在全局有效，但是每次执行循环都会修改该值,变量无则添加，有则覆盖值；变量名前可不加'$'；循环开始标记;")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.Array, "数组名:")]
        [ScriptParams(2, EParamType.GlobalVariableName, "索引变量(*注意不要加$*)：")]
        [ScriptParams(3, EParamType.GlobalVariableName, "值变量(*注意不要加$*)：")]
        #endregion
        WhileArray,

        #endregion

        #region 字典

        #region 字典-目录
        /// <summary>
        /// 字典
        /// </summary>
        [ScriptName("字典", nameof(Dictionary), EGrammarType.Category)]
        #endregion
        Dictionary,

        #region 添加字典
        /// <summary>
        /// 添加字典
        /// </summary>
        [ScriptName("添加字典", nameof(AddDictionary))]
        [ScriptDescription("添加字典；")]
        [ScriptReturn("成功返回 #True  ;失败返回 #False;")]
        [ScriptParams(1, EParamType.Dictionary, "字典名:")]
        #endregion
        AddDictionary,

        #region 删除字典
        /// <summary>
        /// 删除字典
        /// </summary>
        [ScriptName("删除字典", nameof(RemoveDictionary))]
        [ScriptDescription("删除字典；")]
        [ScriptReturn("成功返回 #True  ;失败返回 #False;")]
        [ScriptParams(1, EParamType.Dictionary, "字典名:")]
        #endregion
        RemoveDictionary,

        #region 更新字典数据
        /// <summary>
        /// 更新字典数据
        /// </summary>
        [ScriptName("更新字典数据", nameof(UpdateDictionaryItem))]
        [ScriptDescription("更新字典数据；更新多层级的数据时，键名为格式如: grand/parent/child/[1]/name ；有则更新，无则添加；如果中间层级缺少会自动补充中间层子级字典对象；数据值类型为子级字典对象时，会忽略子级字典对象；字符(整型.浮点型.真假)；子级字典对象(从字典中查找并填充)；子级数组对象(从数组中查找并填充)；")]
        [ScriptReturn("成功返回 #True  ;失败返回 #False;")]
        [ScriptParams(1, EParamType.Dictionary, "字典名:")]
        [ScriptParams(2, EParamType.String, "键名(多个层级间使用/分割):")]
        [ScriptParams(3, EParamType.String, "数据值(均以字符串形式存储):")]
        [ScriptParams(4, EParamType.Combo, "数据值类型:", "字符", "子级字典对象", "子级数组对象", "字典字符串", "数组字符串")]
        [ScriptParams(5, EParamType.Combo, "操作：", "键名中间层缺少自动补齐", "键名中间层缺少不补齐")]
        #endregion
        UpdateDictionaryItem,

        #region 获取字典数据
        /// <summary>        
        /// 获取字典数据
        /// </summary>
        [ScriptName("获取字典数据", nameof(GetDictionaryItem))]
        [ScriptDescription("获取字典数据；获取多层级的数据时，键名为格式如: grand/parent/child ")]
        [ScriptReturn("成功返回 获取的数据  ;失败返回 #False;")]
        [ScriptParams(1, EParamType.Dictionary, "字典名:")]
        [ScriptParams(2, EParamType.String, "键名(多个层级间使用/分割):")]
        #endregion
        GetDictionaryItem,

        #region 获取字典数据类型
        /// <summary>
        /// 获取字典数据类型
        /// </summary>
        [ScriptName("获取字典数据类型", nameof(GetDictionaryItemType))]
        [ScriptDescription("获取字典数据类型；数据值类型:字符；子级字典对象；子级数组对象；")]
        [ScriptReturn("成功返回 数据值类型  ;失败返回 #False;")]
        [ScriptParams(1, EParamType.Dictionary, "字典名:")]
        [ScriptParams(2, EParamType.String, "键名(多个层级间使用/分割):")]
        #endregion
        GetDictionaryItemType,

        #region 获取字典中子级数组对象数据数目
        /// <summary>
        /// 获取字典中子级数组对象数据数目
        /// </summary>
        [ScriptName("获取字典中子级数组对象数据数目", nameof(GetDictionaryItemCountWhenArray))]
        [ScriptDescription("获取字典中子级数组对象数据数目；数据值类型:子集数组对象时返回数组的数目；非数组返回失败；")]
        [ScriptReturn("成功返回 数目  ;失败返回 #False;")]
        [ScriptParams(1, EParamType.Dictionary, "字典名:")]
        [ScriptParams(2, EParamType.String, "键名(多个层级间使用/分割):")]
        #endregion
        GetDictionaryItemCountWhenArray,

        #region 获取字典中子级字典数据数目
        /// <summary>
        /// 获取字典中子级字典数据数目
        /// </summary>
        [ScriptName("获取字典中子级字典数据数目", nameof(GetDictionaryItemCountWhenObject))]
        [ScriptDescription("获取字典中子级字典数据数目；数据值类型:子集字典对象时返回数组的数目；非字典返回失败；")]
        [ScriptReturn("成功返回 数目  ;失败返回 #False;")]
        [ScriptParams(1, EParamType.Dictionary, "字典名:")]
        [ScriptParams(2, EParamType.String, "键名(多个层级间使用/分割):")]
        #endregion
        GetDictionaryItemCountWhenObject,

        #region 获取字典中子级字典数据(按索引)
        /// <summary>
        /// 获取字典中子级字典键名(按索引)
        /// </summary>
        [ScriptName("获取字典中子级字典键名(按索引)", nameof(GetDictionaryItemKeyWhenObjectByIndex))]
        [ScriptDescription("获取字典中子级字典键名(按索引)；数据值类型:子集字典对象时进行键名获取；")]
        [ScriptReturn("成功返回 键名  ;失败返回 #False;")]
        [ScriptParams(1, EParamType.Dictionary, "字典名:")]
        [ScriptParams(2, EParamType.String, "键名(多个层级间使用/分割):")]
        [ScriptParams(3, EParamType.IntSlider, "索引(范围[1,获取字典中子级数组对象数据数目]):", 1, 9999)]
        #endregion
        GetDictionaryItemKeyWhenObjectByIndex,

        #region 字符串转化为字典
        /// <summary>
        /// 字符串转化为字典
        /// </summary>
        [ScriptName("字符串转化为字典", nameof(ConvertStringToDictionary))]
        [ScriptDescription("字符串转化为字典；字符串格式必须是json格式；字典名，无则添加，有则覆盖；")]
        [ScriptReturn("成功返回 #True  ;失败返回 #False;")]
        [ScriptParams(1, EParamType.Dictionary, "字典名:")]
        [ScriptParams(2, EParamType.String, "字符串:")]
        #endregion
        ConvertStringToDictionary,

        #region 字典转化为字符串
        /// <summary>
        /// 字典转化为字符串
        /// </summary>
        [ScriptName("字典转化为字符串", nameof(ConvertDictionaryToString))]
        [ScriptDescription("字典转化为字符串；JSON格式 的输出使用默认的函数直接输出，但是经过测试貌似litJson有bug，在修改某些值后(肯定已经成功修改)对应的信息在输出字符串时还是原来的值！不清楚具体原因！所以对应的有 JSON格式(修复) 按照 JSON规范遍历输出字符串，经测试基本修复了原BUG;")]
        [ScriptReturn("成功返回 字符串  ;失败返回 #False;")]
        [ScriptParams(1, EParamType.Dictionary, "字典名:")]
        //[ScriptParams(2, EParamType.Combo, "类型:", "JSON格式", "JSON格式(修复)", "XML格式")]
        #endregion
        ConvertDictionaryToString,

        /// <summary>
        /// 显示字典窗口
        /// </summary>
        [ScriptName("显示字典窗口", nameof(ShowDictionaryWindow))]
        [ScriptDescription("显示字典窗口；是否强制使用新设定的窗口位置尺寸，如选择“否No”则使用记录的上一次窗口的坐标尺寸信息填填充窗口位置信息；")]
        [ScriptReturn("成功返回#True息 ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.Dictionary, "字典名:")]
        [ScriptParams(2, EParamType.Rect, "窗口位置尺寸:", defaultObject = "0/0/300/400")]
        [ScriptParams(3, EParamType.Bool, "是否强制使用新设定的窗口位置尺寸:", defaultObject = EBool.No)]
        [ScriptParams(4, EParamType.Bool, "可见性:", defaultObject = EBool.Yes)]
        ShowDictionaryWindow,

        /// <summary>
        /// 获取字典窗口信息
        /// </summary>
        [ScriptName("获取字典窗口信息", nameof(GetDictionaryWindowInfo))]
        [ScriptDescription("获取字典窗口信息")]
        [ScriptReturn("成功返回期望获取的信息 ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.Combo, "信息", "可见性", "位置尺寸")]
        GetDictionaryWindowInfo,

        #region 字典循环开始
        /// <summary>
        /// 字典循环开始，遍历数组中的元素
        /// </summary>
        [ScriptName("字典循环开始", nameof(WhileDictionary), EGrammarType.Loop)]
        [ScriptDescription("字典循环开始；如果字典不存在、键名、变量名无效等会返回失败；键名变量与值变量在全局有效，但是每次执行循环都会修改该值;对字典根遍历时，操作键名为空即可；变量无则添加，有则覆盖值；变量名前可不加'$'；循环开始标记;")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.Dictionary, "字典名:")]
        [ScriptParams(2, EParamType.String, "操作键名:")]
        [ScriptParams(3, EParamType.GlobalVariableName, "键名变量(*注意不要加$*)：")]
        [ScriptParams(4, EParamType.GlobalVariableName, "值变量(*注意不要加$*)：")]
        #endregion
        WhileDictionary,

        #endregion

        /// <summary>
        /// 当前已使用的脚本最大ID
        /// </summary>
        MaxCurrent,

        /// <summary>
        /// 结束
        /// </summary>
        _End = IDRange.End,
    }

}

