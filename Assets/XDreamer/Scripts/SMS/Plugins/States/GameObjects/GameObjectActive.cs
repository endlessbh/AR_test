using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.Scripts;

namespace XCSJ.PluginSMS.States.GameObjects
{
    /// <summary>
    /// 游戏对象激活：游戏对象激活组件是控制游戏对象激活或非激活的执行体。随着状态生命周期发生的事件（进入和退出），激活和非激活设置的游戏对象，组件激活后即刻切换为完成态。
    /// </summary>
    [ComponentMenu(SMSHelperExtension.CommonUseCategoryName + "/" + Title, typeof(SMSManager))]
    [Name(Title, nameof(GameObjectActive))]
    [Tip("游戏对象激活组件是控制游戏对象激活或非激活的执行体。随着状态生命周期发生的事件（进入和退出），激活和非激活设置的游戏对象，组件激活后即刻切换为完成态。", "The game object activation component is an actuator that controls the activation or deactivation of the game object. With the events (entry and exit) occurring in the state life cycle, the game object with active and inactive settings will switch to the completed state immediately after the component is activated.")]
    [XCSJ.Attributes.Icon(EIcon.GameObjectActive)]
    [RequireComponent(typeof(GameObjectSet))]
    public class GameObjectActive : StateComponent<GameObjectActive>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "游戏对象激活";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(SMSHelperExtension.CommonUseCategoryName, typeof(SMSManager))]
        [StateLib("游戏对象", typeof(SMSManager))]
        [StateComponentMenu(SMSHelperExtension.CommonUseCategoryName + "/" + Title, typeof(SMSManager))]
        [StateComponentMenu("游戏对象/" + Title, typeof(SMSManager))]
        [Name(Title, nameof(GameObjectActive))]
        [Tip("游戏对象激活组件是控制游戏对象激活或非激活的执行体。随着状态生命周期发生的事件（进入和退出），激活和非激活设置的游戏对象，组件激活后即刻切换为完成态。", "The game object activation component is an actuator that controls the activation or deactivation of the game object. With the events (entry and exit) occurring in the state life cycle, the game object with active and inactive settings will switch to the completed state immediately after the component is activated.")]
        [XCSJ.Attributes.Icon(EIcon.GameObjectActive)]
        public static State CreateGameObjectSet(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 初始化激活
        /// </summary>
        [Name("初始化激活")]
        [EnumPopup]
        public EBool initActive = EBool.None;

        /// <summary>
        /// 进入激活
        /// </summary>
        [Name("进入激活")]
        [EnumPopup]
        public EBool entryActive = EBool.Yes;

        /// <summary>
        /// 退出激活
        /// </summary>
        [Name("退出激活")]
        [EnumPopup]
        public EBool exitActive = EBool.None;

        /// <summary>
        /// 同层级其他对象操作
        /// </summary>
        [Group("同层级其他对象操作", textEN = "Operation of other objects at the same level")]
        [Name("同层级其他对象初始化激活")]
        [EnumPopup]
        public EBool _sameHierarchyOtherObjectInitActive = EBool.None;

        /// <summary>
        /// 同层级对象进入激活
        /// </summary>
        [Name("同层级其他对象进入激活")]
        [EnumPopup]
        public EBool _sameHierarchyOtherObjectEntryActive = EBool.None;

        /// <summary>
        /// 同层级对象退出激活
        /// </summary>
        [Name("同层级其他对象退出激活")]
        [EnumPopup]
        public EBool _sameHierarchyOtherObjectExitActive = EBool.None;

        /// <summary>
        /// 游戏对象集
        /// </summary>
        public GameObjectSet gameObjectSet => GetComponent<GameObjectSet>();

        private GameObjectSet _gameObjectSet;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override bool Init(StateData data)
        {
            _gameObjectSet = gameObjectSet;

            SetGameObjectsActive(initActive);
            SetSameHierarchyGameObjectsActive(_sameHierarchyOtherObjectInitActive);
            return base.Init(data);
        }

        /// <summary>
        /// 进入
        /// </summary>
        /// <param name="data"></param>
        public override void OnEntry(StateData data)
        {
            base.OnEntry(data);

            SetGameObjectsActive(entryActive);
            SetSameHierarchyGameObjectsActive(_sameHierarchyOtherObjectEntryActive);
        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="data"></param>
        public override void OnExit(StateData data)
        {
            base.OnExit(data);

            SetGameObjectsActive(exitActive);
            SetSameHierarchyGameObjectsActive(_sameHierarchyOtherObjectExitActive);
        }

        /// <summary>
        /// 判定完成状态函数
        /// </summary>
        /// <returns></returns>
        public override bool Finished() => true;

        private void SetGameObjectsActive(EBool active)
        {
            try
            {
                if (!_gameObjectSet || active == EBool.None) return;

                _gameObjectSet.objects.ForEach(go =>
                {
                    go.XSetActive(active);

                });
            }
            catch { }
        }

        private void SetSameHierarchyGameObjectsActive(EBool active)
        {
            try
            {
                if (!_gameObjectSet || active == EBool.None) return;

                _gameObjectSet.objects.ForEach(go =>
                {
                    CommonFun.GetChildGameObjects(go.transform.parent).ForEach(child =>
                    {
                        if (child != go)
                        {
                            child.XSetActive(active);
                        }
                    });
                });
            }
            catch { }
        }
    }
}
