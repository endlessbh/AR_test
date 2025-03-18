using UnityEditor;
using XCSJ.Attributes;
using XCSJ.PluginSMS.States.Motions;

namespace XCSJ.EditorSMS.States.Motions
{
    /// <summary>
    /// 游戏对象闪烁检查器
    /// </summary>
    [Name("游戏对象闪烁检查器")]
    [CustomEditor(typeof(GameObjectFlash))]
    public class GameObjectFlashInspector : FlashInspector<GameObjectFlash>
    {
    }
}
