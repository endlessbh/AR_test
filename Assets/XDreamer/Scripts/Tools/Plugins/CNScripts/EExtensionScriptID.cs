using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension;
using XCSJ.PluginTools.Gif;
using XCSJ.PluginTools.Points;
using XCSJ.Scripts;

namespace XCSJ.PluginTools.CNScripts
{
    /// <summary>
    /// 工具扩展脚本ID
    /// </summary>
    public enum EExtensionScriptID
    {
        /// <summary>
        /// 开始
        /// </summary>
        _Begin = ExtensionIDRange.Begin,

        #region 工具库扩展-目录
        /// <summary>
        /// 工具库扩展
        /// </summary>
        [ScriptName("工具库扩展", nameof(ToolsExtensions), EGrammarType.Category)]
        [ScriptDescription("工具库扩展的相关脚本目录；")]
        #endregion
        ToolsExtensions,

        #region 线段创建器操作
        /// <summary>
        /// 线段创建器操作
        /// </summary>
        [ScriptName("线段创建器操作", nameof(SegmentCreaterOperation))]
        [ScriptDescription("线段创建器开始拾取、记录和结束拾取操作", "The segment creator starts picking, records, and ends picking operations")]
        [ScriptReturn("成功返回 #True; 失败返回 #False;", "Successfully return #True; Failure returns #False;")]
        [ScriptParams(1, EParamType.GameObjectComponent, "线段创建器", typeof(SegmentCreater))]
        [ScriptParams(2, EParamType.Combo, "操作:", "开始拾取", "记录", "结束拾取")]
        #endregion
        SegmentCreaterOperation,

        #region 替换游戏对象主纹理为Gif纹理
        /// <summary>
        /// 替换游戏对象主纹理为Gif纹理
        /// </summary>
        [ScriptName("替换游戏对象主纹理为Gif纹理", nameof(ReplaceGameObjectMainTextureToGifTexture))]
        [ScriptDescription("替换游戏对象主纹理为Gif纹理；")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObject, "游戏对象:")]
        [ScriptParams(2, EParamType.UnityAssetObject, "文本资源:", typeof(TextAsset))]
        #endregion
        ReplaceGameObjectMainTextureToGifTexture,

        #region 控制游戏对象的Gif纹理
        /// <summary>
        /// 控制游戏对象的Gif纹理
        /// </summary>
        [ScriptName("控制游戏对象的Gif纹理", nameof(ControlGameObjectGifTexture))]
        [ScriptDescription("控制游戏对象的Gif纹理；需要对应的游戏对象有Gif纹理组件；智能判断的顺序即对应组合中纹理类型的顺序；")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObject, "游戏对象:")]
        [ScriptParams(2, EParamType.Combo, "控制:", "播放", "停止", "暂停", "继续", "播放或继续", defaultObject = "播放或继续")]
        #endregion
        ControlGameObjectGifTexture,

        /// <summary>
        /// 当前已使用的脚本最大ID
        /// </summary>
        MaxCurrent
    }

    /// <summary>
    /// 工具扩展的ID区间
    /// </summary>
    [Name("扩展脚本ID")]
    public static class ExtensionIDRange
    {
        /// <summary>
        /// 开始35840
        /// </summary>
        public const int Begin = (int)EExtensionID._0x18;

        /// <summary>
        /// 结束36351
        /// </summary>
        public const int End = (int)EExtensionID._0x1c - 1;

        public const int Fragment = 0x40;//64

        public const int Common = Begin + Fragment * 0;//35840
        public const int MonoBehaviour = Begin + Fragment * 1;//35904
        public const int StateLib = Begin + Fragment * 2;//35968
        public const int Tools = Begin + Fragment * 3;//36032
        public const int Editor = Begin + Fragment * 6;//36224
    }
}

