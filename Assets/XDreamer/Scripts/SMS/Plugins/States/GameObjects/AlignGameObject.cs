using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.PluginSMS.States.GameObjects
{
    /// <summary>
    /// 对齐游戏对象坐标:平移置游戏对象前方组件是用于将一个游戏对象移动至另外一个游戏对象坐标系下某个位置的执行体。执行操作完成后组件切换为完成态。
    /// </summary>
    [ComponentMenu("游戏对象/" + Title, typeof(SMSManager))]
    [Name(Title, nameof(AlignGameObject))]
    [Tip("平移置游戏对象前方组件是用于将一个游戏对象移动至另外一个游戏对象坐标系下某个位置的执行体。执行操作完成后组件切换为完成态。", "The pan and place in front of the game object component is used to move one game object to a certain position under the coordinate system of another game object. After the operation is completed, the component switches to the completed state.")]
    [XCSJ.Attributes.Icon(EIcon.Position)]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(GameObjectSet))]
    public class AlignGameObject : LifecycleExecutor<AlignGameObject>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "对齐游戏对象坐标";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib("游戏对象", typeof(SMSManager))]
        [StateComponentMenu("游戏对象/" + Title, typeof(SMSManager), categoryIndex = IndexAttribute.DefaultIndex + 1)]
        [Name(Title, nameof(AlignGameObject))]
        [Tip("平移置游戏对象前方组件是用于将一个游戏对象移动至另外一个游戏对象坐标系下某个位置的执行体。执行操作完成后组件切换为完成态。", "The pan and place in front of the game object component is used to move one game object to a certain position under the coordinate system of another game object. After the operation is completed, the component switches to the completed state.")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        [Name("对齐对象")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public GameObject gameObject = null;

        [Name("基于游戏对象坐标系的偏移量")]
        [Tip("向量X/Y/Z是基于游戏对象的前、右和上向量的偏移量", "The vector X / Y / Z is an offset based on the front, right, and upper vectors of the GameObject")]
        public Vector3 offsetOnGameObjectTransform = Vector3.zero;

        [Name("对齐旋转量")]
        public bool alignRotation = true;

        public override void Execute(StateData data, EExecuteMode executeMode)
        {
            if (gameObject)
            {
                var gameObjectSet = GetComponent<GameObjectSet>();
                if (gameObjectSet)
                {
                    var transform = gameObject.transform;
                    var offset = transform.position + transform.forward * offsetOnGameObjectTransform.z + transform.right * offsetOnGameObjectTransform.x + transform.up * offsetOnGameObjectTransform.y;
                    gameObjectSet.objects.ForEach(go =>
                    {
                        go.transform.position = offset;
                        if (alignRotation)
                        {
                            go.transform.rotation = gameObject.transform.rotation;
                        }
                    });
                }
            }
        }

        public override bool DataValidity()
        {
            return base.DataValidity() && gameObject;
        }

        public override string ToFriendlyString()
        {
            return gameObject? gameObject.name:"";
        }
    }
}
