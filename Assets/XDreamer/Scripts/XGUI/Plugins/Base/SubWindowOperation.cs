using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginTools;
using XCSJ.PluginXGUI.Base;
using XCSJ.Tools;

namespace XCSJ.PluginXGUI.Views.Toggles
{
    /// <summary>
    /// XGUI用于响应开关状态的基类
    /// </summary>
    [Name("子窗口操作")]
    public class SubWindowOperation : SubWindowEventListener
    {
        /// <summary>
        /// XGUI 事件类型
        /// </summary>
        [Name("属性名称")]
        public enum EPropertyName
        {
            /// <summary>
            /// 可拖拽
            /// </summary>
            [Name("可拖拽")]
            CanDrag,

            /// <summary>
            /// 展开
            /// </summary>
            [Name("展开")]
            Expand,

            /// <summary>
            /// 全屏
            /// </summary>
            [Name("全屏")]
            FullScreen,

            /// <summary>
            /// 窗口最大化
            /// </summary>
            [Name("窗口最大化")]
            MaxSize,

            /// <summary>
            /// 打开窗口
            /// </summary>
            [Name("打开窗口")]
            Open,

            /// <summary>
            /// 关闭窗口
            /// </summary>
            [Name("关闭窗口")]
            Close,
        }

        /// <summary>
        /// XGUI事件
        /// </summary>
        [Name("XGUI事件")]
        [EnumPopup]
        public EPropertyName _propertyName = EPropertyName.CanDrag;

        /// <summary>
        /// 可拖拽切换
        /// </summary>
        [Name("可拖拽切换")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.CanDrag)]
        [Readonly(EEditorMode.Runtime)]
        public Toggle _canDragToggle = null;

        /// <summary>
        /// 展开切换
        /// </summary>
        [Name("展开切换")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Expand)]
        [Readonly(EEditorMode.Runtime)]
        public Toggle _expandToggle = null;

        /// <summary>
        /// 全屏切换
        /// </summary>
        [Name("全屏切换")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.FullScreen)]
        [Readonly(EEditorMode.Runtime)]
        public Toggle _fullScreenToggle = null;

        /// <summary>
        /// 最大化切换
        /// </summary>
        [Name("最大化切换")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.MaxSize)]
        [Readonly(EEditorMode.Runtime)]
        public Toggle _maxSizeToggle = null;

        /// <summary>
        /// 打开窗口按钮
        /// </summary>
        [Name("打开窗口按钮")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Open)]
        [Readonly(EEditorMode.Runtime)]
        public Button _openButton = null;

        /// <summary>
        /// 关闭窗口按钮
        /// </summary>
        [Name("关闭窗口按钮")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Close)]
        [Readonly(EEditorMode.Runtime)]
        public Button _closeButton = null;

        /// <summary>
        /// 唤醒
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            if(!_canDragToggle) _canDragToggle = GetComponent<Toggle>();
            if (!_expandToggle) _expandToggle = GetComponent<Toggle>();
            if (!_fullScreenToggle) _fullScreenToggle = GetComponent<Toggle>();
            if (!_maxSizeToggle) _maxSizeToggle = GetComponent<Toggle>();
            if (!_openButton) _openButton = GetComponent<Button>();
            if (!_closeButton) _closeButton = GetComponent<Button>();
        }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            switch (_propertyName)
            {
                case EPropertyName.CanDrag:
                    {
                        if (_canDragToggle) _canDragToggle.onValueChanged.AddListener(OnCanDragChanged);
                        break;
                    }
                case EPropertyName.Expand:
                    {
                        if (_expandToggle) _expandToggle.onValueChanged.AddListener(OnExpandChanged);
                        break;
                    }
                case EPropertyName.FullScreen:
                    {
                        if (_fullScreenToggle) _fullScreenToggle.onValueChanged.AddListener(OnFullScreenChanged);
                        break;
                    }
                case EPropertyName.MaxSize:
                    {
                        if (_maxSizeToggle) _maxSizeToggle.onValueChanged.AddListener(OnMaxSizeChanged);
                        break;
                    }
                case EPropertyName.Open:
                    {
                        if (_openButton) _openButton.onClick.AddListener(OnOpenWindowButton);
                        break;
                    }
                case EPropertyName.Close:
                    {
                        if (_closeButton) _closeButton.onClick.AddListener(OnCloseWindowButton);
                        break;
                    }
            }
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            switch (_propertyName)
            {
                case EPropertyName.CanDrag:
                    {
                        if (_canDragToggle) _canDragToggle.onValueChanged.RemoveListener(OnCanDragChanged);
                        break;
                    }
                case EPropertyName.Expand:
                    {
                        if (_expandToggle) _expandToggle.onValueChanged.RemoveListener(OnExpandChanged);
                        break;
                    }
                case EPropertyName.FullScreen:
                    {
                        if (_fullScreenToggle) _fullScreenToggle.onValueChanged.RemoveListener(OnFullScreenChanged);
                        break;
                    }
                case EPropertyName.MaxSize:
                    {
                        if (_maxSizeToggle) _maxSizeToggle.onValueChanged.RemoveListener(OnMaxSizeChanged);
                        break;
                    }
                case EPropertyName.Open:
                    {
                        if (_openButton) _openButton.onClick.RemoveListener(OnOpenWindowButton);
                        break;
                    }
                case EPropertyName.Close:
                    {
                        if (_closeButton) _closeButton.onClick.RemoveListener(OnCloseWindowButton);
                        break;
                    }
            }
        }

        private void OnCanDragChanged(bool isOn)
        {
            subWindow.canDrag = isOn;
        }

        private void OnExpandChanged(bool isOn)
        {
            subWindow.expand = isOn;
        }

        private void OnFullScreenChanged(bool isOn)
        {
            subWindow.fullScreen = isOn;
        }

        private void OnMaxSizeChanged(bool isOn)
        {
            subWindow.maxSize = isOn;
        }

        private void OnOpenWindowButton()
        {
            if (subWindow)
            {
                subWindow.Open();
            }
        }

        private void OnCloseWindowButton()
        {
            if (subWindow)
            {
                subWindow.Close();
            }
        }

        /// <summary>
        /// 子窗口事件
        /// </summary>
        /// <param name="windowEventArgs"></param>
        protected override void OnSubWindowEvent(WindowEventArgs windowEventArgs)
        {
            switch (_propertyName)
            {
                case EPropertyName.CanDrag:
                    {
                        if(_canDragToggle) _canDragToggle.isOn = windowEventArgs.canDrag;
                        break;
                    }
                case EPropertyName.Expand:
                    {
                        if(_expandToggle) _expandToggle.isOn = windowEventArgs.expand;
                        break;
                    }
                case EPropertyName.FullScreen:
                    {
                        if (_fullScreenToggle) _fullScreenToggle.isOn = windowEventArgs.fullScreen;
                        break;
                    }
                case EPropertyName.MaxSize:
                    {
                        if (_maxSizeToggle) _maxSizeToggle.isOn = windowEventArgs.maxSize;
                        break;
                    }
            }
        }

    }
}
