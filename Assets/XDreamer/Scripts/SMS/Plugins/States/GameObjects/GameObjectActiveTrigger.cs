using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;
using XCSJ.Scripts;

namespace XCSJ.PluginSMS.States.GameObjects
{
    /// <summary>
    /// 游戏对象激活触发器：用于监测游戏对象激活状态的触发器
    /// </summary>
    [ComponentMenu("游戏对象" + "/" + Title, typeof(SMSManager))]
    [Name(Title, nameof(GameObjectActiveTrigger))]
    [Tip("用于监测游戏对象激活状态的触发器", "Trigger for monitoring activation status of game object")]
    [XCSJ.Attributes.Icon(EIcon.GameObjectActive)]
    public class GameObjectActiveTrigger : ToggleTrigger<GameObjectActiveTrigger>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "游戏对象激活触发器";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib("游戏对象", typeof(SMSManager))]
        [StateComponentMenu("游戏对象/" + Title, typeof(SMSManager), categoryIndex = IndexAttribute.DefaultIndex + 1)]
        [Name(Title, nameof(GameObjectActiveTrigger))]
        [Tip("用于监测游戏对象激活状态的触发器", "Trigger for monitoring activation status of game object")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State CreateGameObjectActiveTrigger(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 游戏对象
        /// </summary>
        [Name("游戏对象")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public GameObject _gameObject = null;

        /// <summary>
        /// 切换状态
        /// </summary>
        protected override bool toggleState
        {
            get
            {
                if (_gameObject)
                {
                    return _gameObject.activeSelf;
                }
                return false;
            }
            set
            {
                if (_gameObject)
                {
                    _gameObject.SetActive(value);
                }
            }
        }

        private bool stateOnEntry = false;

        /// <summary>
        /// 进入
        /// </summary>
        /// <param name="data"></param>
        public override void OnEntry(StateData data)
        {
            base.OnEntry(data);

            stateOnEntry = toggleState;
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnUpdate(StateData stateData)
        {
            base.OnUpdate(stateData);

            switch (triggerType)
            {
                case EToggleTriggerType.Switch:
                    {
                        finished = stateOnEntry != toggleState;
                        break;
                    }
                case EToggleTriggerType.SwitchOn:
                    {
                        finished = (stateOnEntry != toggleState) && toggleState;
                        break;
                    }
                case EToggleTriggerType.SwitchOff:
                    {
                        finished = (stateOnEntry != toggleState) && !toggleState;
                        break;
                    }
            }
        }

        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity()
        {
            return _gameObject;
        }

        /// <summary>
        /// 友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString()
        {
            return _gameObject? _gameObject.name: base.ToFriendlyString() + CommonFun.Name(triggerType);
        }
    }
}
