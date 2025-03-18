using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginCommonUtils.Runtime;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;
using XCSJ.PluginSMS.States.GameObjects;

namespace XCSJ.PluginSMS.States.Selections
{
    /// <summary>
    /// 自动选中运动对象：自动选中运动对象组件是动态选中运动对象的执行体。状态进入后，首先会判断某个游戏对象是否处于运动中，如发生运动，则将游戏对象设置为选中状态，游戏对象停止运动后，将游戏对象设置非选中状态，状态退出之后功能失效。
    /// </summary>
    [ComponentMenu("选择集/"+ Title, typeof(SMSManager))]
    [Name(Title, nameof(AutoSelectMoveGameObject))]
    [Tip("自动选中运动对象组件是动态选中运动对象的执行体。状态进入后，首先会判断某个游戏对象是否处于运动中，如发生运动，则将游戏对象设置为选中状态，游戏对象停止运动后，将游戏对象设置非选中状态，状态退出之后功能失效。", "The auto select moving object component is an actuator that dynamically selects moving objects. After entering the state, it will first judge whether a game object is in motion. If motion occurs, set the game object to the selected state. After the game object stops moving, set the game object to the non selected state. After the state exits, the function fails.")]
    [XCSJ.Attributes.Icon(index = 33666)]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(GameObjectSet))]
    public class AutoSelectMoveGameObject : StateComponent<AutoSelectMoveGameObject>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "自动选中运动对象";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib("选择集", typeof(SMSManager))]
        [StateComponentMenu("选择集/"+ Title, typeof(SMSManager))]
        [Name(Title, nameof(AutoSelectMoveGameObject))]
        [Tip("自动选中运动对象组件是动态选中运动对象的执行体。状态进入后，首先会判断某个游戏对象是否处于运动中，如发生运动，则将游戏对象设置为选中状态，游戏对象停止运动后，将游戏对象设置非选中状态，状态退出之后功能失效。", "The auto select moving object component is an actuator that dynamically selects moving objects. After entering the state, it will first judge whether a game object is in motion. If motion occurs, set the game object to the selected state. After the game object stops moving, set the game object to the non selected state. After the state exits, the function fails.")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        [Name("包含子对象")]
        public bool includeChildren = true;

        [Name("退出时取消选中游戏对象集")]
        public bool unselectedGameObjectSetWhenExit = true;

        private List<Transform> transforms = new List<Transform>();

        public override void OnEntry(StateData data)
        {
            base.OnEntry(data);

            InitTransform();
        }

        public override void OnUpdate(StateData data)
        {
            base.OnUpdate(data);

            CheckTransformChanged();
        }

        public override void OnExit(StateData data)
        {
            UnSelectOnExit();

            base.OnExit(data);
        }

        private void InitTransform()
        {
            transforms.Clear();
            var gameObjectSet = GetComponent<GameObjectSet>();
            if (gameObjectSet)
            {
                gameObjectSet.objects.ForEach(go =>
                {
                    if (includeChildren)
                    {
                        transforms.AddRange(go.GetComponentsInChildren<Transform>());
                    }
                    else
                    {
                        transforms.Add(go.transform);
                    }
                });
            }
            transforms = transforms.Distinct().ToList();
            transforms.ForEach(t => t.hasChanged = false);
        }

        private void CheckTransformChanged()
        {
            transforms.ForEach(t =>
            {
                if (t.hasChanged)
                {
                    Selection.AddWithDistinct(t.gameObject);
                    t.hasChanged = false;
                }
                else
                {
                    Selection.Remove(t.gameObject);
                }
            });
        }

        private void UnSelectOnExit()
        {
            if (unselectedGameObjectSetWhenExit)
            {
                transforms.ForEach(t => { if (t) Selection.Remove(t.gameObject); });
            }
        }
    }
}
