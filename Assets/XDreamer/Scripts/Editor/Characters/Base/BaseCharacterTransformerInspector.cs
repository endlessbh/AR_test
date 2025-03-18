using UnityEditor;
using XCSJ.Attributes;
using XCSJ.Extension.Characters.Base;

namespace XCSJ.EditorExtension.Characters.Base
{
    /// <summary>
    /// 基础角色变换器检查器
    /// </summary>
    [Name("基础角色变换器检查器")]
    [CustomEditor(typeof(BaseCharacterTransformer), true)]
    public class BaseCharacterTransformerInspector : BaseCharacterTransformerInspector<BaseCharacterTransformer>
    {
    }

    /// <summary>
    /// 基础角色变换器检查器泛型类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseCharacterTransformerInspector<T> : BaseCharacterCoreControllerInspector<T>
       where T : BaseCharacterTransformer
    {
    }
}