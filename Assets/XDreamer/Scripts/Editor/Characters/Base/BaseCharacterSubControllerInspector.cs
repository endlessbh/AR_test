using UnityEditor;
using XCSJ.Attributes;
using XCSJ.EditorExtension.Base.Controllers;
using XCSJ.Extension.Characters.Base;

namespace XCSJ.EditorExtension.Characters.Base
{
    /// <summary>
    /// 基础角色子控制器检查器
    /// </summary>
    [CustomEditor(typeof(BaseCharacterSubController), true)]
    [Name("基础角色子控制器检查器")]
    public class BaseCharacterSubControllerInspector : BaseCharacterSubControllerInspector<BaseCharacterSubController>
    {
    }

    /// <summary>
    /// 基础角色子控制器检查器泛型类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseCharacterSubControllerInspector<T> : BaseSubControllerInspector<T>
       where T : BaseCharacterSubController
    {
    }
}
