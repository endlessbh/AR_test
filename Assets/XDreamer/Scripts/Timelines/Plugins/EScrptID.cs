using UnityEngine.Video;
using XCSJ.Attributes;
using XCSJ.Extension;
using XCSJ.Scripts;

namespace XCSJ.PluginTimelines
{
    /// <summary>
    /// ID区间
    /// </summary>
    public static class IDRange
    {
        /// <summary>
        /// 开始34304
        /// </summary>
        public const int Begin = (int)EExtensionID._0xc;

        /// <summary>
        /// 结束34431
        /// </summary>
        public const int End = (int)EExtensionID._0xd - 1;

        /// <summary>
        /// 片段
        /// </summary>
        public const int Fragment = 0x18;//24

        /// <summary>
        /// 通用
        /// </summary>
        public const int Common = Begin + Fragment * 0;//34304

        /// <summary>
        /// Mono行为
        /// </summary>
        public const int MonoBehaviour = Begin + Fragment * 1;//34432

        /// <summary>
        /// 状态库
        /// </summary>
        public const int StateLib = Begin + Fragment * 2;//34560

        /// <summary>
        /// 工具
        /// </summary>
        public const int Tools = Begin + Fragment * 3;//34688

        /// <summary>
        /// 编辑器
        /// </summary>
        public const int Editor = Begin + Fragment * 4;//34816
    }

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

        #region 视频

        #region 视频-目录
        /// <summary>
        /// 视频
        /// </summary>
        [ScriptName("视频", nameof(Video), EGrammarType.Category)]
        #endregion
        Video,

        #region 视频控制
        /// <summary>
        /// 视频播放
        /// </summary>
        [ScriptName("视频控制", nameof(HandleVideo))]
        [ScriptDescription("视频控制;")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObjectComponent, "播放器对象（限定Video Player类型）：", "", typeof(VideoPlayer))]
        [ScriptParams(2, EParamType.Combo, "操作方式:", "播放", "暂停", "停止")]
        #endregion 视频控制
        HandleVideo,

        #region 视频速度
        /// <summary>
        /// 视频速度
        /// </summary>
        [ScriptName("设置视频速度", nameof(SetVideoSpeed))]
        [ScriptDescription("设置视频速度;")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObjectComponent, "播放器对象（限定Video Player类型）：", "", typeof(VideoPlayer))]
        [ScriptParams(2, EParamType.Float, "速度:", 0f, 10f)]
        #endregion 视频控制
        SetVideoSpeed,

        #region 设置视频进度
        /// <summary>
        /// 设置视频进度
        /// </summary>
        [ScriptName("设置视频进度", nameof(SetVideoProgress))]
        [ScriptDescription("设置视频进度;")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObjectComponent, "播放器对象（限定Video Player类型）：", "", typeof(VideoPlayer))]
        [ScriptParams(2, EParamType.Float, "进度:", 0f, 10f)]
        #endregion 视频速度
        SetVideoProgress,

        #region 获取视频进度
        /// <summary>
        /// 获取视频速度
        /// </summary>
        [ScriptName("获取视频进度", nameof(GetVideoProgress))]
        [ScriptDescription("获取视频进度;")]
        [ScriptReturn("成功返回 视频当前进度 ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObjectComponent, "播放器对象（限定Video Player类型）：", "", typeof(VideoPlayer))]
        #endregion 获取视频速度
        GetVideoProgress,

        #region 获取视频时长
        /// <summary>
        /// 获取视频速度
        /// </summary>
        [ScriptName("获取视频时长", nameof(GetVideoLength))]
        [ScriptDescription("获取视频时长;")]
        [ScriptReturn("成功返回 视频当前时长 ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObjectComponent, "播放器对象（限定Video Player类型）：", "", typeof(VideoPlayer))]
        #endregion 获取视频时长
        GetVideoLength,

        #region 获取视频播放时长
        /// <summary>
        /// 获取视频速度
        /// </summary>
        [ScriptName("获取视频播放时长", nameof(GetVideoPlayingTime))]
        [ScriptDescription("获取视频播放时长;")]
        [ScriptReturn("成功返回 视频当前播放时长 ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObjectComponent, "播放器对象（限定Video Player类型）：", "", typeof(VideoPlayer))]
        #endregion 获取视频播放时长
        GetVideoPlayingTime,

        #endregion

        /// <summary>
        /// 当前已使用的脚本最大ID
        /// </summary>
        MaxCurrent,
    }
}

