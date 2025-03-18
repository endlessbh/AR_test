using UnityEditor;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginTimelines.CNScripts;

namespace XCSJ.EditorTimelines.CNScripts
{
    /// <summary>
    /// 视频脚本事件检查器
    /// </summary>
    [Name("视频脚本事件检查器")]
    [CustomEditor(typeof(VideoScriptEvent), true)]
    public class VideoScriptEventInspector : BaseScriptEventInspector<VideoScriptEvent, EVideoScriptEventType, VideoScriptEventFunction, VideoScriptEventFunctionCollection>
    {
        
    }
}
