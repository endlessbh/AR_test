using UnityEditor;
using XCSJ.Attributes;
using XCSJ.PluginSMS.States.Motions;

namespace XCSJ.EditorSMS.States.Motions
{
    /// <summary>
    /// 游游戏对象路径检查器
    /// </summary>
    [Name("游游戏对象路径检查器")]
    [CustomEditor(typeof(GameObjectPath))]
    public class GameObjectPathInspector : PathInspector<GameObjectPath>
    {
    }
}
