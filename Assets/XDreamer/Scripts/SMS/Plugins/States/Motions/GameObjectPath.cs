using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;
using XCSJ.PluginSMS.States.GameObjects;

namespace XCSJ.PluginSMS.States.Motions
{
    /// <summary>
    /// 游戏对象路径：游戏对象路径组件是游戏对象的路径动画。在给定的时间内，游戏对象沿着世界绝对路径移动，移动完成后，组件切换为完成态。
    /// </summary>
    [ComponentMenu("动作/"+ Title, typeof(SMSManager))]
    [Name(Title, nameof(GameObjectPath))]
    [Tip("游戏对象路径组件是游戏对象的路径动画。在给定的时间内，游戏对象沿着世界绝对路径移动，移动完成后，组件切换为完成态。", "The game object path component is the path animation of the game object. Within a given time, the game object moves along the absolute path of the world. After the movement is completed, the component switches to the completed state.")]
    [XCSJ.Attributes.Icon(index = 33622)]
    [RequireComponent(typeof(GameObjectSet))]
    public class GameObjectPath: Path<GameObjectPath>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "游戏对象路径";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib("动作", typeof(SMSManager))]
        [StateComponentMenu("动作/" + Title, typeof(SMSManager))]
        [Name(Title, nameof(GameObjectPath))]
        [Tip("游戏对象路径组件是游戏对象的路径动画。在给定的时间内，游戏对象沿着世界绝对路径移动，移动完成后，组件切换为完成态。", "The game object path component is the path animation of the game object. Within a given time, the game object moves along the absolute path of the world. After the movement is completed, the component switches to the completed state.")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        #region IPath

        public GameObjectSet gameObjectSet => GetComponent<GameObjectSet>(true);

        public override List<Transform> transforms => gameObjectSet.objects.ToList(go => go.transform);

        public override void AddTransform(Transform transform)
        {
            if (transform) gameObjectSet.Add(transform.gameObject);
        }

        #endregion
    }
}
