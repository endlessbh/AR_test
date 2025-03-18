using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Recorders;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginTools.GameObjects;
using XCSJ.PluginXGUI.Base;

namespace XCSJ.PluginXGUI.Windows.Tables
{
    /// <summary>
    /// 表单元格
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    [Name("表单元格")]
    public class TableData : View, IPointerDownHandler, IDragHandler, IPointerUpHandler, IScrollHandler
    {
        /// <summary>
        /// 表数据
        /// </summary>
        public virtual TableData_Model model
        {
            get => _model;
            set
            {
                if (_model != value)
                {
                    _model = value;
                    UpdateData(_model);
                }
            }
        }
        protected TableData_Model _model;

        [Group("基础设置", textEN = "Base Settings")]
        /// <summary>
        /// 标题
        /// </summary>
        [Name("标题")]
        [Readonly(EEditorMode.Runtime)]
        public Text _titleText = null;

        /// <summary>
        /// 描述
        /// </summary>
        [Name("描述")]
        [Readonly(EEditorMode.Runtime)]
        public Text _descriptionText = null;

        /// <summary>
        /// 图片
        /// </summary>
        [Name("图片")]
        [Readonly(EEditorMode.Runtime)]
        public Image _image = null;

        /// <summary>
        /// 数量文本
        /// </summary>
        [Name("数量文本")]
        public Text _count = null;

        /// <summary>
        /// 交互按钮
        /// </summary>
        [Name("交互按钮")]
        [Readonly(EEditorMode.Runtime)]
        public Button _button = null;

        [Group("效果设置", textEN = "Effect Settings")]
        /// <summary>
        /// 背景图像
        /// </summary>
        [Name("背景边框")]
        [Readonly(EEditorMode.Runtime)]
        public Image _border = null;

        /// <summary>
        /// 可用性效果
        /// </summary>
        [Name("可用性效果")]
        public enum EInteractableEffect
        {
            [Name("透明度")]
            Transparent,
        }

        /// <summary>
        /// 可用性效果
        /// </summary>
        [Name("可交互性效果")]
        [EnumPopup]
        public EInteractableEffect _interactableEffect = EInteractableEffect.Transparent;

        /// <summary>
        /// 透明度
        /// </summary>
        [Name("透明度")]
        [HideInSuperInspector(nameof(_interactableEffect), EValidityCheckType.NotEqual, EInteractableEffect.Transparent)]
        public float _alpha = 0.5f;

        private GraphicRecorder _graphicRecorder = new GraphicRecorder();

        private GraphicRecorder _borderGraphicRecorder = new GraphicRecorder();

        internal Table table {get; set;}

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            TableData_Model.onUpdate += UpdateData;

            if (_button)
            {
                _button.onClick.AddListener(OnClick);
            }

            _graphicRecorder.Recover();
            _graphicRecorder.Clear();
            _graphicRecorder.Record(GetComponentsInChildren<Graphic>());

            _borderGraphicRecorder.Recover();
            _borderGraphicRecorder.Clear();
            _borderGraphicRecorder.Record(_border);

            if (!_parentScrollRect) _parentScrollRect = GetComponentInParent<ScrollRect>();
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            if (_button)
            {
                _button.onClick.RemoveListener(OnClick);
            }

            _graphicRecorder.Recover();
            _borderGraphicRecorder.Recover();

            TableData_Model.onUpdate -= UpdateData;
        }

        /// <summary>
        /// 启用
        /// </summary>
        protected void Start()
        {
            UpdateData(); // 主动刷新数据
        }

        /// <summary>
        /// 更新模型
        /// </summary>
        public void UpdateData() => UpdateData(model);

        /// <summary>
        /// 更新数据
        /// </summary>
        public void UpdateData(TableData_Model model)
        {
            if (this.model == null || this.model != model) return;

            // 设置透明度
            if (model.interactable)
            {
                _graphicRecorder.Recover();
            }
            else
            {
                foreach (var item in _graphicRecorder.records)
                {
                    item.SetAlpha(_alpha);
                }
            }

            // 选择
            if (model.selected)
            {
                _borderGraphicRecorder._records.ForEach(r => r.SetColor(table._selectedColor));
            }
            else
            {
                _borderGraphicRecorder.Recover();
            }

            // 标题
            if (_titleText)
            {
                _titleText.text = model.title;
            }

            // 描述
            if (_descriptionText)
            {
                _descriptionText.text = model.description;
            }

            // 图片
            if (_image)
            {
                _image.sprite = model.texture2D.ToSprite();
            }

            // 按钮交互性
            if (_button)
            {
                _button.interactable = model.interactable;
                //_button.interactable = !model.valid ? false : model.interactable;// 无效时，禁用可交互性
            }

            // 数量
            if (_count)
            {
                _count.text = model.count.ToString();
            }
        }

        /// <summary>
        /// 父对象滚动矩形
        /// </summary>
        private ScrollRect _parentScrollRect = null;

        /// <summary>
        /// 响应滚轮滚动
        /// </summary>
        /// <param name="eventData"></param>
        public void OnScroll(PointerEventData eventData)
        {
            if (_parentScrollRect)
            {
                _parentScrollRect.OnScroll(eventData);
            }
        }

        /// <summary>
        /// 点击
        /// </summary>
        private void OnClick()
        {
            Interact(ETableDataCmd.Click);
        }

        /// <summary>
        /// 指针按下
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnPointerDown(PointerEventData pointerEventData)
        {
            BeginUI();

            if (_parentScrollRect)
            {
                _parentScrollRect.OnBeginDrag(pointerEventData);
            }

            Interact(ETableDataCmd.DragStart, pointerEventData);
        }

        /// <summary>
        /// 拖拽进行中
        /// </summary>
        /// <param name="pointerEventData"></param>
        public virtual void OnDrag(PointerEventData pointerEventData)
        {
            if (_parentScrollRect && _parentScrollRect.enabled)
            {
                _parentScrollRect.OnDrag(pointerEventData);

                // 求与垂直的夹角，判断拖动方向，防止水平与垂直方向同时响应导致的拖动时整个界面都会动
                float angle = Vector2.Angle(pointerEventData.delta, Vector2.up);

                // 视图项正在水平运动时禁用垂直运行
                if (angle > 45f && angle < 135f) _parentScrollRect.enabled = false;
            }

            Interact(ETableDataCmd.Draging, pointerEventData);
        }

        /// <summary>
        /// 指针弹起
        /// </summary>
        /// <param name="pointerEventData"></param>
        public virtual void OnPointerUp(PointerEventData pointerEventData)
        {
            EndUI();

            if (_parentScrollRect)
            {
                _parentScrollRect.enabled = true;
                _parentScrollRect.OnEndDrag(pointerEventData);
            }

            Interact(ETableDataCmd.DragEnd, pointerEventData);
        }

        private bool isBeginUI = false;

        private void BeginUI()
        {
            if (!isBeginUI)
            {
                isBeginUI = true;
                CommonFun.BeginOnUI();
            }
        }

        private void EndUI()
        {
            if (isBeginUI)
            {
                isBeginUI = false;
                CommonFun.EndOnUI();
            }
        }

        private void Interact(ETableDataCmd cmd, PointerEventData pointerEventData = null)
        {
            // 模型无效时停止交互
            if (!model.valid)
            {
                UpdateData();
            }
            else if (table && model.count > 0)
            {
                InteractableEntity interactableEntity = null;
                if (model is IGameObjectGetter gameObjectGetter)
                {
                    interactableEntity = gameObjectGetter.gameObject.GetComponent<InteractableEntity>();
                }
                table.TryInteract(new TableInteractData(pointerEventData, table, this, model, cmd, interactableEntity), out _);
            }
        }
    }
}
