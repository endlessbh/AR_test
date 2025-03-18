using UnityEditor;
using XCSJ.Attributes;
using XCSJ.PluginSMS.States.Motions;

namespace XCSJ.EditorSMS.States.Motions
{
    /// <summary>
    /// 游戏对象激活区间检查器
    /// </summary>
    [Name("游戏对象激活区间检查器")]
    [CustomEditor(typeof(GameObjectActiveRange))]
    public class GameObjectActiveRangeInspector : RangeHandleInspector<GameObjectActiveRange>
    {
    }
}
