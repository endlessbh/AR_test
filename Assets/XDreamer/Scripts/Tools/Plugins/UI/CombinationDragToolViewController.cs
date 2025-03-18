using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginTools.Draggers;
using XCSJ.PluginTools.Draggers.TRSHandles;
using XCSJ.PluginXGUI.ViewControllers;
using XCSJ.PluginXGUI.Views.Inputs;
using static XCSJ.PluginTools.Draggers.CombinationDragToolController;
using static XCSJ.PluginTools.Draggers.DragByRay;
using static XCSJ.PluginTools.Draggers.RigidbodyGrabber;

namespace XCSJ.PluginTools.UI
{
    /// <summary>
    /// 一键拖拽工具视图控制器
    /// </summary>
    [Name("一键拖拽工具视图控制器")]
    public sealed class CombinationDragToolViewController : BaseViewController
    {
        /// <summary>
        /// 一键拖拽工具控制器
        /// </summary>
        [Name("一键拖拽工具控制器")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public CombinationDragToolController _combinationDragToolController;

        /// <summary>
        /// 一键拖拽工具控制器
        /// </summary>
        public CombinationDragToolController combinationDragToolController => this.XGetComponentInParentOrGlobal(ref _combinationDragToolController);

        #region 全局

        private bool GetToolActive(EToolType toolType)
        {
            return _combinationDragToolController && _combinationDragToolController.GetToolActive(toolType);
        }

        private void SetToolActive(EToolType toolType, bool active)
        {
            if (_combinationDragToolController)
            {
                _combinationDragToolController.SetToolActive(toolType, active);
            }
        }

        private void SetToolActiveAndViewActive(EToolType toolType, bool toolActive)
        {
            SetToolActive(toolType, toolActive);

            // 只要点击了UI对应的Toggle就显示对象
            SetToolViewActive(toolType, true);
        }

        private void DisableAllView()
        {
            SetHoverSelectView(false);
            SetDragByRayView(false);
            SetDragByCameraViewView(false);

            SetTRSHandleMoveView(false);
            SetTRSHandleRotationView(false);
            SetTRSHandleScaleView(false);

            SetRigidbodyGrabberView(false);

            SetOtherToolView(false);
            SetXRToolView(false);
        }

        /// <summary>
        /// 一键拖拽工具类型变化
        /// </summary>
        /// <param name="toolType"></param>
        public void OnCombinationDragToolControllerChanged(EToolType toolType) => SetToolViewActive(toolType);

        private void SetToolViewActive(EToolType toolType, bool active)
        {
            DisableAllView();

            switch (toolType)
            {
                case EToolType.Hover:
                case EToolType.SelectionModify:
                    {
                        SetHoverSelectView(active);
                        break;
                    }
                case EToolType.DragByRay:
                    {
                        SetDragByRayView(active);
                        break;
                    }
                case EToolType.DragByCameraView:
                    {
                        SetDragByCameraViewView(active);
                        break;
                    }
                case EToolType.TRSHandle_Move:
                    {
                        SetTRSHandleMoveView(active);
                        break;
                    }
                case EToolType.TRSHandle_Rotation:
                    {
                        SetTRSHandleRotationView(active);
                        break;
                    }
                case EToolType.TRSHandle_Scale:
                    {
                        SetTRSHandleScaleView(active);
                        break;
                    }
                case EToolType.Hand:
                    {
                        SetRigidbodyGrabberView(active);
                        break;
                    }
                case EToolType.DragRecorder:
                    {
                        SetOtherToolView(active);
                        break;
                    }
                case EToolType.XR:
                    {
                        SetXRToolView(active);
                        break;
                    }
            }
        }

        private void SetToolViewActive(EToolType toolType)
        {
            DisableAllView();

            switch (toolType)
            {
                case EToolType.Hover:
                case EToolType.SelectionModify:
                    {
                        SetHoverSelectView(clickSelectTool);
                        break;
                    }
                case EToolType.DragByRay:
                    {
                        SetDragByRayView(dragByRay);
                        break;
                    }
                case EToolType.DragByCameraView:
                    {
                        SetDragByCameraViewView(dragByCameraView);
                        break;
                    }
                case EToolType.TRSHandle_Move:
                    {
                        SetTRSHandleMoveView(trsHandleMove);
                        break;
                    }
                case EToolType.TRSHandle_Rotation:
                    {
                        SetTRSHandleRotationView(trsHandleRotation);
                        break;
                    }
                case EToolType.TRSHandle_Scale:
                    {
                        SetTRSHandleScaleView(trsHandleScale);
                        break;
                    }
                case EToolType.Hand:
                    {
                        SetRigidbodyGrabberView(rigidbodyGrabber);
                        break;
                    }
                case EToolType.DragRecorder:
                    {
                        SetOtherToolView(otherTool);
                        break;
                    }
                case EToolType.XR:
                    {
                        SetXRToolView(XRTool);
                        break;
                    }
            }
        }

        #endregion

        #region 悬停选择工具

        public bool clickSelectTool
        {
            get => GetToolActive(EToolType.Hover) && GetToolActive(EToolType.SelectionModify);
            set
            {
                // 只要点击了UI对应的Toggle就显示对象
                SetToolViewActive(EToolType.Hover, true);
            }
        }

        private void SetHoverSelectView(bool active)
        {
            if (_clickSelectToolView)
            {
                _clickSelectToolView.XSetActive(active);
            }
        }

        [Group("视图列表", textEN = "View List")]
        [Name("悬停选择工具视图")]
        public GameObject _clickSelectToolView;

        #endregion

        #region 射线拖拽工具

        public bool dragByRay { get => GetToolActive(EToolType.DragByRay); set => SetToolActiveAndViewActive(EToolType.DragByRay, value); }

        private void SetDragByRayView(bool active)
        {
            if (_dragByRayView)
            {
                _dragByRayView.XSetActive(active);
            }
        }

        [Name("射线拖拽工具视图")]
        public GameObject _dragByRayView;

        #endregion

        #region 相机视图平面拖拽工具

        public bool dragByCameraView { get => GetToolActive(EToolType.DragByCameraView); set => SetToolActiveAndViewActive(EToolType.DragByCameraView, value); }

        [Name("相机视图平面拖拽工具视图")]
        public GameObject _dragByCameraViewView;

        private void SetDragByCameraViewView(bool active)
        {
            if (_dragByCameraViewView)
            {
                _dragByCameraViewView.XSetActive(active);
            }
        }

        #endregion

        #region 三轴平移拖拽工具

        public bool trsHandleMove { get => GetToolActive(EToolType.TRSHandle_Move); set => SetToolActiveAndViewActive(EToolType.TRSHandle_Move, value); }

        [Name("三轴平移拖拽工具视图")]
        public GameObject _trsHandleMoveView;

        private void SetTRSHandleMoveView(bool active)
        {
            if (_trsHandleMoveView)
            {
                _trsHandleMoveView.XSetActive(active);
            }
        }

        public bool trsHandleRotation { get => GetToolActive(EToolType.TRSHandle_Rotation); set => SetToolActiveAndViewActive(EToolType.TRSHandle_Rotation, value); }

        [Name("三轴旋转拖拽工具视图")]
        public GameObject _trsHandleRotationView;

        private void SetTRSHandleRotationView(bool active)
        {
            if (_trsHandleRotationView)
            {
                _trsHandleRotationView.XSetActive(active);
            }
        }

        public bool trsHandleScale { get => GetToolActive(EToolType.TRSHandle_Scale); set => SetToolActiveAndViewActive(EToolType.TRSHandle_Scale, value); }

        [Name("三轴缩放拖拽工具视图")]
        public GameObject _trsHandleScaleView;

        private void SetTRSHandleScaleView(bool active)
        {
            if (_trsHandleScaleView)
            {
                _trsHandleScaleView.XSetActive(active);
            }
        }

        #endregion

        #region 刚体抓取器

        /// <summary>
        /// 刚体抓取器
        /// </summary>
        public bool rigidbodyGrabber { get => GetToolActive(EToolType.Hand); set => SetToolActiveAndViewActive(EToolType.Hand, value); }

        private void SetRigidbodyGrabberView(bool active)
        {
            if (_rigidbodyGrabberView)
            {
                _rigidbodyGrabberView.XSetActive(active);
            }
        }

        [Name("刚体抓取器视图")]
        public GameObject _rigidbodyGrabberView;

        #endregion

        #region 其他工具设置

        public bool otherTool { get => GetToolActive(EToolType.DragRecorder); set => SetToolActiveAndViewActive(EToolType.DragRecorder, value); }

        /// <summary>
        /// 其他工具视图
        /// </summary>
        [Name("其他工具视图")]
        public GameObject _otherToolView;

        private void SetOtherToolView(bool active)
        {
            if (_otherToolView)
            {
                _otherToolView.XSetActive(active);
            }
        }

        #endregion

        #region XR工具

        public bool XRTool { get => GetToolActive(EToolType.XR); set => SetToolActiveAndViewActive(EToolType.XR, value); }

        /// <summary>
        /// XR工具视图
        /// </summary>
        [Name("XR工具视图")]
        public GameObject _XRToolView;

        private void SetXRToolView(bool active)
        {
            if (_XRToolView)
            {
                _XRToolView.XSetActive(active);
            }
        }

        #endregion
    }
}
