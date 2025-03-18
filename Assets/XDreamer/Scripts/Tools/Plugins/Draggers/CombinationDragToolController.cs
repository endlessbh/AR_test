using XCSJ.Attributes;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginTools.Draggers.TRSHandles;
using XCSJ.PluginTools.Effects;
using XCSJ.PluginTools.Inputs;
using XCSJ.PluginTools.SelectionUtils;

namespace XCSJ.PluginTools.Draggers
{
    /// <summary>
    /// 一键拖拽工具控制器
    /// </summary>
    [Name("一键拖拽工具控制器")]
    public sealed class CombinationDragToolController : Interactor
    {
        /// <summary>
        /// 鼠标输入
        /// </summary>
        [Name("鼠标输入")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public MouseInput _mouseInput;

        /// <summary>
        /// 鼠标输入
        /// </summary>
        public MouseInput mouseInput => this.XGetComponentInChildrenOrGlobal<MouseInput>(ref _mouseInput);

        /// <summary>
    /// 悬停器
    /// </summary>
        [Group("悬停设置", textEN = "Hover Settings")]
        [Name("悬停器")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Hover _hover;

        /// <summary>
        /// 悬停特效
        /// </summary>
        [Name("悬停特效")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public EffectController _hoverVisual;

        /// <summary>
        /// 选择集修改器
        /// </summary>
        [Group("选择设置", textEN = "Select Settings")]
        [Name("选择集修改器")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public SelectionModify _selectionModify;

        /// <summary>
        /// 选择集可视化工具
        /// </summary>
        [Name("选择集特效")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public EffectController _selectionVisual;

        /// <summary>
        /// 射线拖拽摆放工具
        /// </summary>
        [Group("拽摆设置", textEN = "Drag Settings")]
        [Name("射线拖拽摆放工具")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public DragByRay _dragByRay;

        /// <summary>
        /// 相机视图平面拖拽
        /// </summary>
        [Name("相机视图平面拖拽")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public DragByCameraView _dragByCameraView;

        /// <summary>
        /// 平移旋转缩放工具
        /// </summary>
        [Name("平移旋转缩放工具")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public TRSHandle _trsHandle;

        /// <summary>
        /// 刚体抓取器
        /// </summary>
        [Name("刚体抓取器")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public RigidbodyGrabber _rigidbodyGrabber;

        public void DisableAllDragTool()
        {
            if (_dragByRay)
            {
                _dragByRay.gameObject.XSetActive(false);
            }
            if (_dragByCameraView)
            {
                _dragByCameraView.gameObject.XSetActive(false);
            }
            if (_trsHandle)
            {
                _trsHandle.gameObject.XSetActive(false);
            }
            if (_rigidbodyGrabber)
            {
                _rigidbodyGrabber.gameObject.XSetActive(false);
            }
        }

        /// <summary>
        /// 设置TRS工具类型
        /// </summary>
        /// <param name="trsToolType"></param>
        public void SetTRSHandle(ETRSToolType trsToolType)
        {
            if (_trsHandle)
            {
                _trsHandle.trsToolType = trsToolType;
            }
        }

        /// <summary>
        /// 工具类型
        /// </summary>
        public enum EToolType
        {
            [Name("悬停器")]
            Hover,

            [Name("选择集修改器")]
            SelectionModify,

            [Name("射线拖拽摆放工具")]
            DragByRay,

            [Name("相机视图平面拖拽")]
            DragByCameraView,

            [Name("平移工具")]
            TRSHandle_Move,

            [Name("旋转工具")]
            TRSHandle_Rotation,

            [Name("缩放工具")]
            TRSHandle_Scale,

            [Name("手交互器")]
            Hand,

            [Name("拖拽记录器")]
            DragRecorder,

            [Name("XR工具")]
            XR
        }

        /// <summary>
        /// 获取工具激活状态
        /// </summary>
        /// <param name="toolType"></param>
        /// <returns></returns>
        public bool GetToolActive(EToolType toolType)
        {
            switch (toolType)
            {
                case EToolType.Hover: return _hover && _hover.gameObject.activeSelf;
                case EToolType.SelectionModify: return _selectionModify && _selectionModify.gameObject.activeSelf;
                case EToolType.DragByRay: return _dragByRay && _dragByRay.gameObject.activeSelf;
                case EToolType.DragByCameraView: return _dragByCameraView && _dragByCameraView.gameObject.activeSelf;
                case EToolType.TRSHandle_Move: return _trsHandle && _trsHandle.gameObject.activeSelf && _trsHandle.trsToolType == ETRSToolType.Position;
                case EToolType.TRSHandle_Rotation: return _trsHandle && _trsHandle.gameObject.activeSelf && _trsHandle.trsToolType == ETRSToolType.Rotate;
                case EToolType.TRSHandle_Scale: return _trsHandle && _trsHandle.gameObject.activeSelf && _trsHandle.trsToolType == ETRSToolType.Scale;
                case EToolType.Hand: return _rigidbodyGrabber && _rigidbodyGrabber.gameObject.activeSelf;
                case EToolType.DragRecorder: return _dragRecorderFlag;
                case EToolType.XR: return _XRTool;
            }
            return false;
        }

        /// <summary>
        /// 工具类型
        /// </summary>
        /// <param name="toolType"></param>
        /// <param name="active"></param>
        public void SetToolActive(EToolType toolType, bool active)
        {
            switch (toolType)
            {
                case EToolType.Hover:
                    {
                        _hover.gameObject.XSetActive(active);
                        break;
                    }
                case EToolType.SelectionModify:
                    {
                        _selectionModify.gameObject.XSetActive(active);
                        break;
                    }
                case EToolType.DragByRay:
                    {
                        if (active)
                        {
                            DisableAllDragTool();
                        }
                        _dragByRay.gameObject.XSetActive(active);
                        break;
                    }
                case EToolType.DragByCameraView:
                    {
                        if (active)
                        {
                            DisableAllDragTool();
                        }
                        _dragByCameraView.gameObject.XSetActive(active);
                        break;
                    }
                case EToolType.TRSHandle_Move:
                    {
                        if (active)
                        {
                            if (!_trsHandle.gameObject.activeSelf)
                            {
                                DisableAllDragTool();
                            }

                            _trsHandle.XModifyProperty(() => _trsHandle.trsToolType = ETRSToolType.Position);
                        }
                        _trsHandle.gameObject.XSetActive(active);
                        break;
                    }
                case EToolType.TRSHandle_Rotation:
                    {
                        if (active)
                        {
                            if (!_trsHandle.gameObject.activeSelf)
                            {
                                DisableAllDragTool();
                            }

                            _trsHandle.XModifyProperty(() => _trsHandle.trsToolType = ETRSToolType.Rotate);
                        }
                        _trsHandle.gameObject.XSetActive(active);
                        break;
                    }
                case EToolType.TRSHandle_Scale:
                    {
                        if (active)
                        {
                            if (!_trsHandle.gameObject.activeSelf)
                            {
                                DisableAllDragTool();
                            }

                            _trsHandle.XModifyProperty(() => _trsHandle.trsToolType = ETRSToolType.Scale);
                        }
                        _trsHandle.gameObject.XSetActive(active);
                        break;
                    }
                case EToolType.Hand:
                    {
                        if (active)
                        {
                            DisableAllDragTool();
                        }
                        _rigidbodyGrabber.gameObject.XSetActive(active);
                        break;
                    }
                case EToolType.DragRecorder:
                    {
                        this.XModifyProperty(() => _dragRecorderFlag = active);
                        break;
                    }
                case EToolType.XR:
                    {
                        this.XModifyProperty(() => _XRTool = active);
                        break;
                    }
            }
        }

        /// <summary>
        /// 激活悬停器
        /// </summary>
        public bool hoverActive { get => GetToolActive(EToolType.Hover); set => SetToolActive(EToolType.Hover, value); }

        /// <summary>
        /// 激活选择集修改器
        /// </summary>
        public bool selectionModifyActive { get => GetToolActive(EToolType.SelectionModify); set => SetToolActive(EToolType.SelectionModify, value); }

        /// <summary>
        /// 拖拽记录器
        /// </summary>
        [Name("拖拽记录器")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public DragRecorder _dragRecorder = null;

        /// <summary>
        /// 拖拽记录器标记量
        /// </summary>
        [Name("拖拽记录器标记量")]
        [HideInSuperInspector]
        public bool _dragRecorderFlag = false;

        #region XR

        /// <summary>
        /// XR工具
        /// </summary>
        [Name("XR工具")]
        [HideInSuperInspector]
        public bool _XRTool = false;

        /// <summary>
        /// 模拟鼠标输入控制器
        /// </summary>
        [Name("模拟鼠标输入控制器")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public AnalogMouseInputController _analogMouseInputController = null;

        /// <summary>
        /// 模拟鼠标输入控制器
        /// </summary>
        public AnalogMouseInputController analogMouseInputController => this.XGetComponentInChildrenOrGlobal<AnalogMouseInputController>(ref _analogMouseInputController);

        #endregion
    }
}
