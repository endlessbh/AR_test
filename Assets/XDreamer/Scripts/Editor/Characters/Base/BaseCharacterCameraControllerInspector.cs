using UnityEditor;
using XCSJ.Attributes;
using XCSJ.Extension.Characters.Base;

namespace XCSJ.EditorExtension.Characters.Base
{
    /// <summary>
    /// 基础角色相机控制器检查器
    /// </summary>
    [CustomEditor(typeof(BaseCharacterCameraController), true)]
    [Name("基础角色相机控制器检查器")]
    public class BaseCharacterCameraControllerInspector : BaseCharacterCameraControllerInspector<BaseCharacterCameraController>
    {
    }

    /// <summary>
    /// 基础角色相机控制器检查器泛型类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseCharacterCameraControllerInspector<T> : BaseCharacterCoreControllerInspector<T>
       where T : BaseCharacterCameraController
    {
    }
}

