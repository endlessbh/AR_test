using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Dataflows.Binders;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils.Runtime;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginTools;
using XCSJ.PluginXGUI.Base;
using XCSJ.PluginXGUI.DataViews.Base;

namespace XCSJ.PluginXGUI.DataViews.GroupViews
{
    /// <summary>
    /// 变换视图
    /// </summary>
    [Name("变换视图")]
    [DataViewAttribute(typeof(Transform))]
#if UNITY_EDITOR && XDREAMER_EDITION_DEVELOPER
    //[Tool(XGUICategory.DataGroupView, rootType = typeof(XGUIManager))]
#endif
    public class TransformView : ComponentView
    {
        #region 监听选择集和Transform数据发生变化

        public enum ETransformLinkRule
        {
            [Name("无")]
            None,

            [Name("选择集")]
            Selection,
        }

        [Name("变换关联规则")]
        public ETransformLinkRule _transformLinkRule = ETransformLinkRule.Selection;

        /// <summary>
        /// 启用：绑定选择事件
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            switch (_transformLinkRule)
            {
                case ETransformLinkRule.None:
                    {
                        if (_component)
                        {
                            BindTransform(_component.transform);
                        }
                        break;
                    }
                case ETransformLinkRule.Selection:
                    {
                        if (Selection.selection)
                        {
                            BindTransform(Selection.selection.transform);
                        }
                        break;
                    }
            }

            Selection.selectionChanged += OnSelectionChanged;

            ToolsManager.onTransformHasChanged += OnTransformHasChanged;
        }

        /// <summary>
        /// 启用：解除选择事件绑定
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            Selection.selectionChanged -= OnSelectionChanged;

            ToolsManager.onTransformHasChanged -= OnTransformHasChanged;
        }

        [LanguageTuple("Unselect GameObject", "未选中游戏对象")]
        private void OnSelectionChanged(GameObject[] oldSelections, bool isUndoOrRedo)
        {
            if (_transformLinkRule != ETransformLinkRule.Selection) return;

            if (Selection.selection)
            {
                BindTransform(Selection.selection.transform);
            }
        }

        private void BindTransform(Transform transform)
        {
            if (!transform) return;

            BindModel(transform);
        }

        /// <summary>
        /// 标题
        /// </summary>
        public override string modelDataLabelText => Selection.selection? base.modelDataLabelText: LanguageHelper.Tr("Unselect GameObject", this);

        private void OnTransformHasChanged(Transform transform)
        {
            if (transform == modelTarget)
            {
                ModelDataChanged();
            }
        }

        #endregion

        private const string Position = nameof(Transform.position);
        private const string LocalPosition = nameof(Transform.localPosition);
        private const string EulerAngle = nameof(Transform.eulerAngles);
        private const string LocalEulerAngle = nameof(Transform.localEulerAngles);
        private const string Scale = nameof(Transform.localScale);

        /// <summary>
        /// 世界位置视图
        /// </summary>
        [Name("世界位置视图")]
        public BaseModelView _positionView = null;

        /// <summary>
        /// 本地位置视图
        /// </summary>
        [Name("本地位置视图")]
        public BaseModelView _localPositionView = null;

        /// <summary>
        /// 世界欧拉角视图
        /// </summary>
        [Name("世界欧拉角视图")]
        public BaseModelView _eulerAnglesView = null;

        /// <summary>
        /// 本地欧拉角视图
        /// </summary>
        [Name("本地欧拉角视图")]
        public BaseModelView _localEulerAnglesView = null;

        /// <summary>
        /// 缩放视图
        /// </summary>
        [Name("缩放视图")]
        public BaseModelView _scaleView = null;

        /// <summary>
        /// 创建视图集
        /// </summary>
        /// <param name="data"></param>
        [LanguageTuple(Position, "世界位置")]
        [LanguageTuple(LocalPosition, "本地位置")]
        [LanguageTuple(EulerAngle, "世界欧拉角")]
        [LanguageTuple(LocalEulerAngle, "本地欧拉角")]
        [LanguageTuple(Scale, "缩放")]
        protected override void CreateViewsInternal(UnityEngine.Object data)
        {
            var parent = _parent ? _parent : transform;

            // 世界位置
            if (!_positionView)
            {
                _positionView = DataViewHelper.CreateDataView<Vector3>(data, EBindType.Property, Position, parent, Position.Tr(typeof(TransformView)));
            }
            _positionView._modelToViewDataUpdateRule = EDataUpdateRule.Trigger;

            // 本地位置
            if (!_localPositionView)
            {
                _localPositionView = DataViewHelper.CreateDataView<Vector3>(data, EBindType.Property, LocalPosition, parent, LocalPosition.Tr(typeof(TransformView)));
            }
            _localPositionView._modelToViewDataUpdateRule = EDataUpdateRule.Trigger;

            //世界欧拉角
            if (!_eulerAnglesView)
            {
                _eulerAnglesView = DataViewHelper.CreateDataView<Vector3>(data, EBindType.Property, EulerAngle, parent, EulerAngle.Tr(typeof(TransformView)));
            }
            _eulerAnglesView._modelToViewDataUpdateRule = EDataUpdateRule.Trigger;

            //本地欧拉角
            if (!_localEulerAnglesView)
            {
                _localEulerAnglesView = DataViewHelper.CreateDataView<Vector3>(data, EBindType.Property, LocalEulerAngle, parent, LocalEulerAngle.Tr(typeof(TransformView)));
            }
            _localEulerAnglesView._modelToViewDataUpdateRule = EDataUpdateRule.Trigger;

            // 缩放
            if (!_scaleView)
            {
                _scaleView = DataViewHelper.CreateDataView<Vector3>(data, EBindType.Property, Scale, parent, Scale.Tr(typeof(TransformView)));
            }
            _scaleView._modelToViewDataUpdateRule = EDataUpdateRule.Trigger;
        }

        private void ModelDataChanged()
        {
            if (_positionView)
            {
                _positionView.ModelToViewIfCan();
            }

            if (_localPositionView)
            {
                _localPositionView.ModelToViewIfCan();
            }

            if (_eulerAnglesView)
            {
                _eulerAnglesView.ModelToViewIfCan();
            }

            if (_localEulerAnglesView)
            {
                _localEulerAnglesView.ModelToViewIfCan();
            }

            if (_scaleView)
            {
                _scaleView.ModelToViewIfCan();
            }
        } 

    }
}
