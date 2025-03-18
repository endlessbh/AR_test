using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginSMS.States.Base;
using XCSJ.PluginTools;
using XCSJ.PluginXGUI.Base;
using XCSJ.Scripts;

namespace XCSJ.PluginXGUI.Views.Toggles
{
    /// <summary>
    /// Toggle切换处理器
    /// </summary>
    [Name("Toggle切换处理器")]
    [XCSJ.Attributes.Icon(EIcon.Toggle)]
    [Tip("通过Toggle切换处理对应事件", "Toggle game object activation")]
    [Tool(XGUICategory.Component, nameof(XGUIManager))]
    [RequireManager(typeof(XGUIManager), typeof(ToolsManager))]
    public class ToggleSwitchProcessor : View
    {
        /// <summary>
        /// 切换
        /// </summary>
        [Name("切换")]
        [Tip("如当前参数无效，会从当前组件所在游戏对象上查找当前参数类型的组件", "If the current parameter is invalid, it will find the component of the current parameter type from the game object where the current component is located")]
        [ComponentPopup]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Toggle _toggle = null;

        public class ToggleTrigger
        {
            /// <summary>
            /// toggle触发类型
            /// </summary>
            [Name("toggle触发类型")]
            [EnumPopup]
            public EToggleTriggerType _toggleTriggerType = EToggleTriggerType.SwitchOn;

            /// <summary>
            /// 是否触发
            /// </summary>
            /// <param name="isSwitch"></param>
            /// <param name="isToggleOn"></param>
            /// <returns></returns>
            public bool IsTrigger(bool isSwitch, bool isToggleOn)
            {
                switch (_toggleTriggerType)
                {
                    case EToggleTriggerType.On: return isToggleOn;
                    case EToggleTriggerType.Off: return !isToggleOn;
                    case EToggleTriggerType.Switch: return isSwitch;
                    case EToggleTriggerType.SwitchOn: return isSwitch && isToggleOn;
                    case EToggleTriggerType.SwitchOff: return isSwitch && !isToggleOn;
                }
                return false;
            }
        }

        #region 游戏对象激活

        [Serializable]
        public class ToggleActiveGameObject : ToggleTrigger
        {
            /// <summary>
            /// 激活操作
            /// </summary>
            [Name("激活操作")]
            [EnumPopup]
            public EBool _activeOperation = EBool.None;

            /// <summary>
            /// 游戏对象列表
            /// </summary>
            [Name("游戏对象列表")]
            [ValidityCheck(EValidityCheckType.ElementCountGreater, 0)]
            public List<GameObject> _gameObjects = new List<GameObject>();

            /// <summary>
            /// 构造函数
            /// </summary>
            public ToggleActiveGameObject() { }

            /// <summary>
            /// 激活游戏对象
            /// </summary>
            public void ActiveGameObject()
            {
                foreach (var go in _gameObjects)
                {
                    go.XSetActive(_activeOperation);
                }
            }
        }

        /// <summary>
        /// 触发激活游戏对象列表
        /// </summary>
        [Name("触发激活游戏对象列表")]
        public List<ToggleActiveGameObject> _toggleActiveGameObjects = new List<ToggleActiveGameObject>();

        #endregion

        #region 交互执行

        [Serializable]
        public class ToggleExecuteInteract : ToggleTrigger
        {
            /// <summary>
            /// 交互信息
            /// </summary>
            [Name("交互信息")]
            public InteractInfo _interactInfo = new InteractInfo();

            /// <summary>
            /// 执行交互
            /// </summary>
            public void ExecuteInteract() => _interactInfo.TryInteract(out _);
        }

        /// <summary>
        /// 交互执行列表
        /// </summary>
        [Name("交互执行列表")]
        public List<ToggleExecuteInteract> _executeInteracts = new List<ToggleExecuteInteract>();

        #endregion

        private Toggle toggleOflistened = null;

        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            _toggle = GetComponent<Toggle>();
        }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            if (_toggle)
            {
                toggleOflistened = _toggle;

                toggleOflistened.onValueChanged.AddListener(OnValueChanged);
            }
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            if (toggleOflistened)
            {
                toggleOflistened.onValueChanged.RemoveListener(OnValueChanged);
            }
        }

        protected void Update()
        {
            if (!toggleOflistened) return;

            ActiveGameObject(false, toggleOflistened.isOn);
            ExecuteInteract(false, toggleOflistened.isOn);
        }

        /// <summary>
        /// 切换值变更事件
        /// </summary>
        /// <param name="value"></param>
        protected void OnValueChanged(bool value)
        {
            ActiveGameObject(true, value);
            ExecuteInteract(true, value);
        }

        private void ActiveGameObject(bool isSwitch, bool isToggleOn)
        {
            foreach (var item in _toggleActiveGameObjects)
            {
                if (item.IsTrigger(isSwitch, isToggleOn))
                {
                    item.ActiveGameObject();
                }
            }
        }

        private void ExecuteInteract(bool isSwitch, bool isToggleOn)
        {
            foreach (var item in _executeInteracts)
            {
                if (item.IsTrigger(isSwitch, isToggleOn))
                {
                    item.ExecuteInteract();
                }
            }
        }
    }
}
