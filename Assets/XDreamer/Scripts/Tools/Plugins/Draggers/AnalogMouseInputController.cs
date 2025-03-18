using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Extension.Base.Inputs;
using XCSJ.Extension.Base.Recorders;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginTools.Base;
using XCSJ.PluginTools.Inputs;
using XCSJ.PluginTools.Items;
using XCSJ.PluginTools.Notes;
using XCSJ.PluginTools.PropertyDatas;
using XCSJ.PluginXGUI.Views.Inputs;
using XCSJ.Scripts;

namespace XCSJ.PluginTools.Draggers
{
    /// <summary>
    /// 模拟鼠标输入控制器
    /// </summary>
    [Name("模拟鼠标输入控制器")]
    public class AnalogMouseInputController : Interactor, IAnalogInputUpdater
    {
        /// <summary>
        /// 鼠标输入组件提供器
        /// </summary>
        [Name("鼠标输入组件提供器")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public MouseInputComponentProvider _mouseInputComponentProvider = new MouseInputComponentProvider();

        /// <summary>
        /// 射线变换
        /// </summary>
        [Name("射线变换")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Transform _rayTransform = null;

        /// <summary>
        /// 射线变换
        /// </summary>
        public Transform rayTransform
        {
            get => _rayTransform ? _rayTransform : transform;
            set => _rayTransform = value;
        }

        /// <summary>
        /// 模拟鼠标输入
        /// </summary>
        [Name("模拟鼠标输入")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public AnalogMouseInput _analogMouseInput = null;

        /// <summary>
        /// 模拟鼠标输入
        /// </summary>
        public AnalogMouseInput analogMouseInput => this.XGetComponentInChildrenOrGlobal<AnalogMouseInput>(ref _analogMouseInput);

        /// <summary>
        /// 标准鼠标输入
        /// </summary>
        [Name("标准鼠标输入")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public MouseInput _standardMouseInput = null;

        /// <summary>
        /// 启用禁用模拟鼠标输入中的标准鼠标驱动
        /// </summary>
        public bool enableStandardMouseInput
        {
            get => _standardMouseInput ? _standardMouseInput.gameObject.activeSelf : false;
            set
            {
                if (_standardMouseInput)
                {
                    _standardMouseInput.gameObject.SetActive(value);
                }
            }
        }

        private EXRMouseInputMode _XRMouseInputMode = EXRMouseInputMode.Analog;

        /// <summary>
        /// XR鼠标输入模式
        /// </summary>
        public EXRMouseInputMode XRMouseInputMode { get => _XRMouseInputMode; set => SetXRMouseInput(value); }

        /// <summary>
        /// 激活和非激活模拟鼠标输入控制器
        /// </summary>
        public bool active 
        { 
            get => gameObject.activeSelf; 
            set => gameObject.SetActive(value); 
        }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            RecordRayTransform();

            mouseInputRecorder.Record(_mouseInputComponentProvider.GetComponents());
            SetXRMouseInput(XRMouseInputMode);
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            RecoverRayTransform();
            transformRecorder.Clear();

            mouseInputRecorder.Recover();
            mouseInputRecorder.Clear();
        }

        /// <summary>
        /// 更新
        /// </summary>
        protected void Update()
        {
            if (rayTransform)
            {
                rayTransform.Rotate(0, horizontalInputValue, 0, Space.World);
                rayTransform.Rotate(verticalInputValue, 0, 0);
            }
        }

        private TransformRecorder transformRecorder = new TransformRecorder();

        private MouseInputRecorder mouseInputRecorder = new MouseInputRecorder();

        private void SetXRMouseInput(EXRMouseInputMode XRMouseInputMode)
        {
            _XRMouseInputMode = XRMouseInputMode;
            switch (XRMouseInputMode)
            {
                case EXRMouseInputMode.Init:
                    {
                        mouseInputRecorder?.Recover();
                        break;
                    }
                case EXRMouseInputMode.None:
                    {
                        foreach (var input in _mouseInputComponentProvider.GetComponents())
                        {
                            if (input)
                            {
                                input._cmds._cmds.ForEach(m => m._inputMode = 0);
                            }
                        }
                        break;
                    }
                case EXRMouseInputMode.Standard:
                    {
                        foreach (var input in _mouseInputComponentProvider.GetComponents())
                        {
                            if (input)
                            {
                                input._cmds._cmds.ForEach(m => m._inputMode = RayCmd.EInputMode.Standard);
                            }
                        }
                        break;
                    }
                case EXRMouseInputMode.Analog:
                    {
                        foreach (var input in _mouseInputComponentProvider.GetComponents())
                        {
                            if (input)
                            {
                                input._cmds._cmds.ForEach(m => m._inputMode = RayCmd.EInputMode.Analog);
                            }
                        }
                        break;
                    }
                case EXRMouseInputMode.Standard_And_Analog:
                    {
                        foreach (var input in _mouseInputComponentProvider.GetComponents())
                        {
                            if (input)
                            {
                                input._cmds._cmds.ForEach(m => m._inputMode = RayCmd.EInputMode.Standard | RayCmd.EInputMode.Analog);
                            }
                        }
                        break;
                    }
            }
        }

        /// <summary>
        /// 记录射线对象位置
        /// </summary>
        public void RecordRayTransform()
        {
            if (rayTransform)
            {
                transformRecorder.Clear();
                transformRecorder.Record(rayTransform);
            }
        }

        /// <summary>
        /// 还原射线对象位置
        /// </summary>
        public void RecoverRayTransform() => transformRecorder._records.Foreach(r => r.Recover(Space.Self));

        /// <summary>
        /// 射线变换横向转动输入
        /// </summary>
        [Name("射线变换横向转动输入")]
        [Input]
        public string _horizontalInput = "Horizontal";

        /// <summary>
        /// 射线变换纵向转动输入
        /// </summary>
        [Name("射线变换纵向转动输入")]
        [Input]
        public string _verticalInput = "Vertical";

        private float horizontalInputValue = 0;

        private float verticalInputValue = 0;

        /// <summary>
        /// 更新轴
        /// </summary>
        /// <param name="input"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void UpdateAxis(IInput input, string name, float value)
        {
            if (_horizontalInput == name)
            {
                horizontalInputValue = value;
            }

            if (_verticalInput == name)
            {
                verticalInputValue = value;
            }
        }

        /// <summary>
        /// 更新按钮
        /// </summary>
        /// <param name="input"></param>
        /// <param name="name"></param>
        /// <param name="downOrUp"></param>
        public void UpdateButton(IInput input, string name, bool downOrUp) { }
    }

    /// <summary>
    /// XR 鼠标输入
    /// </summary>
    public enum EXRMouseInputMode
    {
        /// <summary>
        /// 初始态
        /// </summary>
        [Name("初始态")]
        Init = -1,

        /// <summary>
        /// 无
        /// </summary>
        [Name("无")]
        None = 0,

        /// <summary>
        /// 标准
        /// </summary>
        [Name("标准")]
        Standard,

        /// <summary>
        /// 模拟
        /// </summary>
        [Name("模拟")]
        Analog,

        /// <summary>
        /// 标准和模拟
        /// </summary>
        [Name("标准和模拟")]
        Standard_And_Analog,
    }
}
