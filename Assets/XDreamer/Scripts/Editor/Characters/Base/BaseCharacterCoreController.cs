using UnityEditor;
using XCSJ.Attributes;
using XCSJ.EditorExtension.Base.Controllers;
using XCSJ.Extension.Characters.Base;

namespace XCSJ.EditorExtension.Characters.Base
{
    /// <summary>
    /// 基础角色核心控制器检查器
    /// </summary>
    [CustomEditor(typeof(BaseCharacterCoreController), true)]
    [Name("基础角色核心控制器检查器")]
    public class BaseCharacterCoreControllerInspector : BaseCharacterCoreControllerInspector<BaseCharacterCoreController>
    {
    }

    /// <summary>
    /// 基础角色核心控制器检查器泛型类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseCharacterCoreControllerInspector<T> : BaseCharacterSubControllerInspector<T>
       where T : BaseCharacterCoreController
    {
    }
}
