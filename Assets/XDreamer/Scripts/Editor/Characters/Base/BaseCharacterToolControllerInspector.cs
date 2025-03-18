using UnityEditor;
using XCSJ.Attributes;
using XCSJ.EditorExtension.Base.Controllers;
using XCSJ.Extension.Characters.Base;

namespace XCSJ.EditorExtension.Characters.Base
{

    /// <summary>
    /// 基础角色工具控制器检查器
    /// </summary>
    [Name("基础角色工具控制器检查器")]
    [CustomEditor(typeof(BaseCharacterToolController), true)]
    public class BaseCharacterToolControllerInspector : BaseCharacterToolControllerInspector<BaseCharacterToolController>
    {
    }

    /// <summary>
    /// 基础角色工具控制器检查器泛型类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseCharacterToolControllerInspector<T> : BaseCharacterSubControllerInspector<T>
       where T : BaseCharacterToolController
    {
    }
}
