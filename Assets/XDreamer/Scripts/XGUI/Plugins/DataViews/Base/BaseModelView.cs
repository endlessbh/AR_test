using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Attributes;
using XCSJ.Extension.Base.Dataflows.Binders;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXGUI.ViewControllers;
using XCSJ.Scripts;
using static XCSJ.PluginXGUI.DataViews.DataViewHelper;

namespace XCSJ.PluginXGUI.DataViews.Base
{
    /// <summary>
    /// 基础数据视图：将视图与数据对象的字段、属性或方法进行绑定
    /// </summary>
    public abstract class BaseModelView : BaseViewController, IDropdownPopupAttribute, ITypeBinderGetter
    {
        #region Unity生命周期方法

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();

            if (this.XGetComponent(ref _modelToViewConverter)) { }
            if (this.XGetComponent(ref _viewToModelConverter)) { }
        }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            if (TryGetValidModelToViewConverter(out var MVConverter))
            {
                validModelToViewConverter = MVConverter;
            }

            if (TryGetValidViewToModelConverter(out var VMConverter))
            {
                validViewToModelConverter = VMConverter;
            }

            if (_modelType == EModelType.ScriptVariable)
            {
                Variable.onValueChanged += OnVariableValueChanged;
            }
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            Variable.onValueChanged -= OnVariableValueChanged;

            validModelToViewConverter = null;
            validViewToModelConverter = null;
        }

        /// <summary>
        /// 启用时尝试将模型数据刷新至视图
        /// </summary>
        protected virtual void Start()
        {
            ModelToViewIfCan();
        }

        private float modelToViewDataUpdateTimeCounter = 0;

        private float viewToModelDataUpdateTimeCounter = 0;

        /// <summary>
        /// 更新函数
        /// </summary>
        protected virtual void Update()
        {
            if (CanModelToView())
            {
                if (_modelToViewDataUpdateRule == EDataUpdateRule.Timing)
                {
                    modelToViewDataUpdateTimeCounter += Time.deltaTime;
                    if (modelToViewDataUpdateTimeCounter >= _modelToViewDataUpdateIntervalTime)
                    {
                        modelToViewDataUpdateTimeCounter = 0;
                        ModelToViewIfValueChanged();
                    }
                }
                else if (_modelToViewDataUpdateRule == EDataUpdateRule.EveryFrame)
                {
                    ModelToViewIfValueChanged();
                }
            }

            if (CanViewToModel())
            {
                if (_viewToModelDataUpdateRule == EDataUpdateRule.Timing)
                {
                    viewToModelDataUpdateTimeCounter += Time.deltaTime;
                    if (viewToModelDataUpdateTimeCounter >= _viewToModelDataUpdateIntervalTime)
                    {
                        viewToModelDataUpdateTimeCounter = 0;
                        ViewToModelIfValueChanged();
                    }
                }
                else if (_viewToModelDataUpdateRule == EDataUpdateRule.EveryFrame)
                {
                    ViewToModelIfValueChanged();
                }
            }
        }

        #endregion

        #region 模型<==>视图

        /// <summary>
        /// 模型到视图数据连接模式
        /// </summary>
        [Name("模型到视图数据连接模式")]
        [EnumPopup]
        public EModelViewDataLinkMode _modelViewDataLinkMode = EModelViewDataLinkMode.Both;

        /// <summary>
        /// 能否将模型数据更新到视图
        /// </summary>
        /// <returns></returns>
        public virtual bool CanModelToView() => (_modelViewDataLinkMode & EModelViewDataLinkMode.ModelToView) == EModelViewDataLinkMode.ModelToView;

        /// <summary>
        /// 能否将视图数据更新到模型
        /// </summary>
        /// <returns></returns>
        public virtual bool CanViewToModel() => (_modelViewDataLinkMode & EModelViewDataLinkMode.ViewToModel) == EModelViewDataLinkMode.ViewToModel;

        #endregion

        #region 模型

        /// <summary>
        /// 模型数据类型
        /// </summary>
        public virtual Type modelValueType
        {
            get
            {
                switch (_modelType)
                {
                    case EModelType.FieldPropertyMethodBinder: return _fieldPropertyMethodBinder.memberType;
                    case EModelType.ScriptVariable: return typeof(string);
                }
                return null;
            }
        }

        /// <summary>
        /// 模型数据值
        /// </summary>
        public virtual object modelValue
        {
            get
            {
                switch (_modelType)
                {
                    case EModelType.FieldPropertyMethodBinder: return _fieldPropertyMethodBinder.memberValue;
                    case EModelType.ScriptVariable:
                        {                            
                            if (_scriptVariable.TryGetHierarchyVarValue(out var value))
                            {
                                return value;
                            }
                            break;
                        }
                }
                return null;
            }
            set
            {
                switch (_modelType)
                {
                    case EModelType.FieldPropertyMethodBinder: _fieldPropertyMethodBinder.memberValue = value; break;
                    case EModelType.ScriptVariable: _scriptVariable.TrySetOrAddSetHierarchyVarValue(value.ToString()); break;
                }
            }
        }

        /// <summary>
        /// 模型类型
        /// </summary>
        [Group("模型", textEN = "Model")]
        [Name("模型类型")]
        [EnumPopup]
        public EModelType _modelType = EModelType.FieldPropertyMethodBinder;

        #region 字段属性方法绑定器

        /// <summary>
        /// 字段、属性或方法绑定器
        /// </summary>
        [Name("字段属性方法绑定器")]
        [HideInSuperInspector(nameof(_modelType), EValidityCheckType.NotEqual, EModelType.FieldPropertyMethodBinder)]
        public FieldPropertyMethodBinder _fieldPropertyMethodBinder;

        /// <summary>
        /// 模型目标
        /// </summary>
        public virtual UnityEngine.Object modelTarget => _fieldPropertyMethodBinder.target;

        /// <summary>
        /// 绑定数据
        /// </summary>
        /// <param name="target"></param>
        /// <param name="title"></param>
        public virtual void BindModel(UnityEngine.Object target)
        {
            _fieldPropertyMethodBinder.target = target;

            if (_viewLabelDataRule == EViewLabelDataRule.ModelInfo)
            {
                SetLabelText(modelDataLabelText);
            }

            ModelToViewIfCan();
        }

        /// <summary>
        /// 绑定数据成员
        /// </summary>
        /// <param name="target"></param>
        /// <param name="bindType"></param>
        /// <param name="memberName"></param>
        public virtual void BindModel(UnityEngine.Object target, EBindType bindType, string memberName)
        {
            _fieldPropertyMethodBinder._bindType = bindType;
            _fieldPropertyMethodBinder.memberName = memberName;

            BindModel(target);
        }

        /// <summary>
        /// 解除绑定
        /// </summary>
        public virtual void UnbindModel()
        {
            _fieldPropertyMethodBinder.target = null;
        }

        #endregion

        /// <summary>
        /// 脚本变量
        /// </summary>
        [Name("脚本变量")]
        [HideInSuperInspector(nameof(_modelType), EValidityCheckType.NotEqual, EModelType.ScriptVariable)]
        [VarString(EVarStringHierarchyKeyMode.Both)]
        public string _scriptVariable;

        /// <summary>
        /// 模型到视图更新规则
        /// </summary>
        [Name("模型到视图更新规则")]
        [Tip("根据不同的视图刷新规则，将模型数据更新到视图", "Update the model data to the view according to different view refresh rules")]
        [EnumPopup]
        public EDataUpdateRule _modelToViewDataUpdateRule = EDataUpdateRule.Timing;

        /// <summary>
        /// 模型到视图更新定时间隔
        /// </summary>
        [Name("模型到视图更新定时间隔")]
        [Range(0, 3)]
        [HideInSuperInspector(nameof(_modelToViewDataUpdateRule), EValidityCheckType.NotEqual, EDataUpdateRule.Timing)]
        public float _modelToViewDataUpdateIntervalTime = 0.3f;

        /// <summary>
        /// 变量变换回调
        /// </summary>
        /// <param name="variable"></param>
        private void OnVariableValueChanged(Variable variable)
        {
            if (variable.name == _scriptVariable)
            {
                ModelToViewIfCanAndTrigger();
            }
        }

        /// <summary>
        /// 模型发生改变
        /// </summary>
        /// <returns></returns>
        public virtual bool ModelHasChanged() => false;

        #endregion

        #region 模型=>视图

        /// <summary>
        /// 将模型数据设置到视图
        /// </summary>
        /// <returns></returns>
        public bool ModelToView() => OnModelToView();

        /// <summary>
        /// 如果模型至视图可连通，则将模型数据设置到视图
        /// </summary>
        /// <returns></returns>
        public bool ModelToViewIfCan() => CanModelToView() && OnModelToView();

        /// <summary>
        /// 如果模型至视图可连通，并且模型到视图数据更新规则为触发器，则将模型数据设置到视图
        /// </summary>
        /// <returns></returns>
        public bool ModelToViewIfCanAndTrigger() => _modelToViewDataUpdateRule == EDataUpdateRule.Trigger && ModelToViewIfCan();

        /// <summary>
        /// 如果模型至视图可连通，并且数据已变化，则将模型数据设置到视图
        /// </summary>
        /// <returns></returns>
        public bool ModelToViewIfCanAndValueChanged() => ModelHasChanged() && ModelToViewIfCan();

        /// <summary>
        /// 如果模型至视图可连通，模型到视图数据更新规则为触发器，并且模型数据已改变，则将模型数据设置到视图
        /// </summary>
        /// <returns></returns>
        public bool ModelToViewIfCanAndTriggerAndValueChanged() => ModelHasChanged() && ModelToViewIfCanAndTrigger();

        /// <summary>
        /// 如果模型数据已变化，则将模型数据设置到视图
        /// </summary>
        /// <returns></returns>
        public bool ModelToViewIfValueChanged() => ModelHasChanged() && OnModelToView();

        /// <summary>
        /// 将模型数据更新至视图
        /// </summary>
        /// <returns></returns>
        protected virtual bool OnModelToView()
        {
            bool result = TryConvertModelToView(modelValue, out var value);
            if (result)
            {
                SetViewValue(value);
            }
            return result;
        }

        /// <summary>
        /// 设置模型值：可破循环
        /// </summary>
        /// <param name="value"></param>
        public void SetModelValue(object value)
        {
            try
            {
                if (inViewToModel) return;
                inViewToModel = true;

                modelValue = value;
            }
            finally
            {
                inViewToModel = false;
            }
        }

        /// <summary>
        /// 模型数据更新中
        /// </summary>
        public bool inViewToModel { get; private set; } = false;

        #region 数据转换器
        public Type modelToViewConverterInterfaceType
        {
            get
            {
                if (modelValueType == null || viewValueType == null 
                    || typeof(MethodInfo).IsAssignableFrom(modelValueType) || typeof(MethodInfo).IsAssignableFrom(viewValueType)) return null;
                return typeof(IConverter<,>).MakeGenericType(modelValueType, viewValueType);
            }
        }

        /// <summary>
        /// 模型到视图数据转换器
        /// </summary>
        [Name("模型到视图数据转换器")]
        [ComponentPopup]
        public BaseDataConverter _modelToViewConverter;

        private BaseDataConverter validModelToViewConverter;

        /// <summary>
        /// 转换模型数据到视图中
        /// </summary>
        /// <param name="modelVale"></param>
        /// <param name="viewValue"></param>
        /// <returns></returns>
        protected bool TryConvertModelToView(object modelVale, out object viewValue)
        {
            if (modelVale != null)
            {
                if (validModelToViewConverter && validModelToViewConverter.TryConvertTo(modelVale, viewValueType, out viewValue))
                {
                    return true;
                }
                if (Converter.instance.TryConvertTo(modelVale, viewValueType, out viewValue))
                {
                    return true;
                }
            }
            viewValue = default;
            return false;
        }

        /// <summary>
        /// 尝试获取有效模型到视图转换器
        /// </summary>
        /// <param name="modelToViewConverter"></param>
        /// <returns></returns>
        public bool TryGetValidModelToViewConverter(out BaseDataConverter modelToViewConverter)
        {
            if (_modelToViewConverter && modelValueType != null)
            {
                if (ConverterCache.Get(modelValueType, viewValueType, _modelToViewConverter.GetType()).canInputToOutput)
                {
                    modelToViewConverter = _modelToViewConverter;
                    return true;
                }
                else
                {
                    var tmp = _modelToViewConverter.GetComponent(modelToViewConverterInterfaceType) as BaseDataConverter;
                    if (tmp)
                    {
                        modelToViewConverter = tmp;
                        return true;
                    }
                }
            }
            modelToViewConverter = default;
            return false;
        }

        #endregion

        #endregion

        #region 标签

        /// <summary>
        /// 视图标签
        /// </summary>
        [Group("视图标签", textEN = "View Label")]
        [Name("视图标签")]
        public Text _viewLabel;

        public enum EViewLabelDataRule
        {
            [Name("无")]
            None,

            [Name("模型信息")]
            ModelInfo,
        }

        [Name("视图标签数据源规则")]
        [EnumPopup]
        public EViewLabelDataRule _viewLabelDataRule = EViewLabelDataRule.ModelInfo;

        /// <summary>
        /// 模型数据标签文本
        /// </summary>
        public virtual string modelDataLabelText
        {
            get
            {
                switch (_modelType)
                {
                    case EModelType.FieldPropertyMethodBinder: return CommonFun.Name(_fieldPropertyMethodBinder.memberInfo);
                    case EModelType.ScriptVariable: return _scriptVariable;
                    default: return "";
                }
            }
        }

        /// <summary>
        /// 获取视图标签文本
        /// </summary>
        /// <returns></returns>
        public virtual string GetLabelText()
        {
            return _viewLabel ? _viewLabel.text : default;
        }

        /// <summary>
        /// 设置视图标签文本
        /// </summary>
        /// <param name="title"></param>
        public virtual void SetLabelText(string text)
        {
            if (_viewLabel)
            {
                _viewLabel.text = text;
            }
        }

        #endregion

        #region 视图

        /// <summary>
        /// 视图数据类型
        /// </summary>
        public abstract Type viewValueType { get; }

        /// <summary>
        /// 视图数据值
        /// </summary>
        public abstract object viewValue { get; set; }

        /// <summary>
        /// 模型数据更新规则
        /// </summary>
        [Group("视图", textEN = "View")]
        [Name("视图到模型数据更新规则")]
        [Tip("根据不同的模型数据更新规则，将视图数据更新到模型", "Update the view data to the model according to different refresh rules")]
        [EnumPopup]
        public EDataUpdateRule _viewToModelDataUpdateRule = EDataUpdateRule.Trigger;

        [Name("视图到模型数据更新定时间隔")]
        [Range(0, 3)]
        [HideInSuperInspector(nameof(_viewToModelDataUpdateRule), EValidityCheckType.NotEqual, EDataUpdateRule.Timing)]
        public float _viewToModelDataUpdateIntervalTime = 0.3f;

        /// <summary>
        /// 视图发生变换
        /// </summary>
        /// <returns></returns>
        public virtual bool ViewHasChanged() => false;

        #endregion

        #region 视图=>模型

        /// <summary>
        /// 将视图数据设置到模型
        /// </summary>
        /// <returns></returns>
        public bool ViewToModel() => OnViewToModel();

        /// <summary>
        /// 将视图数据设置到模型
        /// </summary>
        /// <returns></returns>
        protected virtual bool OnViewToModel()
        {
            bool result = TryConvertViewToModel(viewValue, out var value);
            if (result)
            {
                SetModelValue(value);
            }
            return result;
        }

        /// <summary>
        /// 设置视图值：可破循环
        /// </summary>
        /// <param name="value"></param>
        public void SetViewValue(object value)
        {
            try
            {
                if (inModelToView) return;
                inModelToView = true;

                viewValue = value;
            }
            finally
            {
                inModelToView = false;
            }
        }

        /// <summary>
        /// 视图更新中
        /// </summary>
        public bool inModelToView { get; private set; } = false;

        /// <summary>
        /// 如果视图至模型可连通，将视图数据设置到模型
        /// </summary>
        /// <returns></returns>
        public bool ViewToModelIfCan() => CanViewToModel() && OnViewToModel();

        /// <summary>
        /// 如果视图至模型可连通，并且视图到模型数据更新规则为触发器，将视图数据设置到模型
        /// </summary>
        /// <returns></returns>
        public bool ViewToModelIfCanAndTrigger() => _viewToModelDataUpdateRule == EDataUpdateRule.Trigger && ViewToModelIfCan();

        /// <summary>
        /// 如果视图至模型可连通，并且视图数据已改变，将视图数据设置到模型
        /// </summary>
        /// <returns></returns>
        public bool ViewToModelIfCanAndValueChanged() => ViewHasChanged() && ViewToModelIfCan();

        /// <summary>
        /// 如果视图至模型可连通，并且视图到模型数据更新规则为触发器，并且视图数据已改变，将视图数据设置到模型 
        /// </summary>
        /// <returns></returns>
        public bool ViewToModelIfCanAndTriggerAndValueChanged() => ViewHasChanged() && ViewToModelIfCanAndTrigger();

        /// <summary>
        /// 如果视图数据已改变，将视图数据设置到模型
        /// </summary>
        /// <returns></returns>
        public bool ViewToModelIfValueChanged() => ViewHasChanged() && OnViewToModel();

        #region 数据转换器

        public Type viewToModelConverterInterfaceType
        {
            get
            {
                if (viewValueType == null || modelValueType == null) return null;
                return typeof(IConverter<,>).MakeGenericType(viewValueType, modelValueType);
            }
        }

        /// <summary>
        /// 视图到模型数据转换器
        /// </summary>
        [Name("视图到模型数据转换器")]
        [ComponentPopup]
        public BaseDataConverter _viewToModelConverter;

        private BaseDataConverter validViewToModelConverter;

        /// <summary>
        /// 转换视图数据到模型中
        /// </summary>
        /// <param name="viewData"></param>
        /// <param name="modelData"></param>
        /// <returns></returns>
        protected bool TryConvertViewToModel(object viewData, out object modelData)
        {
            if (viewData != null)
            {
                if (validViewToModelConverter && validViewToModelConverter.TryConvertTo(viewData, modelValueType, out modelData))
                {
                    return true;
                }
                if (Converter.instance.TryConvertTo(viewData, modelValueType, out modelData))
                {
                    return true;
                }
            }
            modelData = default;
            return false;
        }

        /// <summary>
        /// 尝试获取有效视图到模型转换器
        /// </summary>
        /// <param name="viewToModelConverter"></param>
        /// <returns></returns>
        public bool TryGetValidViewToModelConverter(out BaseDataConverter viewToModelConverter)
        {
            if (_viewToModelConverter && modelValueType != null)
            {
                if (ConverterCache.Get(viewValueType, modelValueType, _viewToModelConverter.GetType()).canInputToOutput)
                {
                    viewToModelConverter = _viewToModelConverter;
                    return true;
                }
                else
                {
                    var tmp = _viewToModelConverter.GetComponent(viewToModelConverterInterfaceType) as BaseDataConverter;
                    if (tmp)
                    {
                        viewToModelConverter = tmp;
                        return true;
                    }
                }
            }
            viewToModelConverter = default;
            return false;
        }

        #endregion

        #endregion

        #region ITypeBinderGetter

        /// <summary>
        /// 获取器所有者
        /// </summary>
        public UnityEngine.Object owner => this;

        /// <summary>
        /// 类型绑定器获取器
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ITypeBinder> GetTypeBinders() => new ITypeBinder[] { _fieldPropertyMethodBinder };

        #endregion

        #region IDropdownPopupAttribute

        bool IDropdownPopupAttribute.TryGetOptions(string purpose, string propertyPath, out string[] options)
        {
            switch (purpose)
            {
                case nameof(MemberNamePopupAttribute):
                    {
                        options = FieldPropertyMethodBinder.GetMemberNames(_fieldPropertyMethodBinder.targetType, _fieldPropertyMethodBinder.bindMemberInfoType, _fieldPropertyMethodBinder.bindingFlags, _fieldPropertyMethodBinder.includeBaseType);
                        return true;
                    }
                case nameof(TypeFullNamePopupAttribute):
                    {
                        options = FieldPropertyMethodBinder.GetTypeFullNames(_fieldPropertyMethodBinder.bindMemberInfoType, _fieldPropertyMethodBinder.bindingFlags, _fieldPropertyMethodBinder.includeBaseType);
                        return true;
                    }
            }
            options = default;
            return false;
        }

        bool IDropdownPopupAttribute.TryGetOption(string purpose, string propertyPath, string[] options, object propertyValue, out string option)
        {
            switch (purpose)
            {
                case nameof(MemberNamePopupAttribute):
                    {
                        option = (propertyValue as string) ?? "";
                        return true;
                    }
                case nameof(TypeFullNamePopupAttribute):
                    {
                        option = (propertyValue as string) ?? "";
                        return true;
                    }
            }
            option = default;
            return false;
        }

        bool IDropdownPopupAttribute.TryGetPropertyValue(string purpose, string propertyPath, string[] options, string option, out object propertyValue)
        {
            switch (purpose)
            {
                case nameof(MemberNamePopupAttribute):
                    {
                        propertyValue = option;
                        return true;
                    }
                case nameof(TypeFullNamePopupAttribute):
                    {
                        propertyValue = option;
                        return true;
                    }
            }
            propertyValue = default;
            return false;
        }

        #endregion
    }

    /// <summary>
    /// 最后值比较数据视图
    /// </summary>
    public abstract class BaseModelViewCache : BaseModelView
    {
        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            lastOrgModelDataValue = null;
            lastOrgViewDataValue = null;
        }

        /// <summary>
        /// 最后模型原始数据
        /// </summary>
        protected object lastOrgModelDataValue = null;

        /// <summary>
        /// 模型值
        /// </summary>
        public override object modelValue 
        { 
            get => base.modelValue; 
            set
            {
                base.modelValue = value;
                lastOrgModelDataValue = value;
            }
        }

        /// <summary>
        /// 模型发生改变
        /// </summary>
        /// <returns></returns>
        public override bool ModelHasChanged()
        {
            var dataChanged = false;
            var mdv = modelValue;
            if (mdv != null)
            {
                dataChanged = !mdv.Equals(lastOrgModelDataValue);
                lastOrgModelDataValue = mdv;
            }
            return dataChanged;
        }

        /// <summary>
        /// 将模型数据至视图
        /// </summary>
        /// <returns></returns>
        protected override bool OnModelToView()
        {
            var result = base.OnModelToView();
            if (!result)
            {
                lastOrgViewDataValue = viewValue;
            }
            return result;
        }

        /// <summary>
        /// 最后视图原始数据
        /// </summary>
        protected object lastOrgViewDataValue = null;

        /// <summary>
        /// 视图发生改变
        /// </summary>
        /// <returns></returns>
        public override bool ViewHasChanged()
        {
            var dataChanged = false;
            var vdv = viewValue;
            if (vdv != null)
            {
                dataChanged = !vdv.Equals(lastOrgViewDataValue);
                lastOrgViewDataValue = vdv;
            }
            return dataChanged;
        }
    }

    /// <summary>
    /// 模型类型
    /// </summary>
    public enum EModelType
    {
        [Name("字段属性方法绑定器")]
        FieldPropertyMethodBinder,

        [Name("脚本变量")]
        ScriptVariable,
    }

    /// <summary>
    /// 数据更新规则
    /// </summary>
    public enum EDataUpdateRule
    {
        [Name("无")]
        None,

        [Name("定时")]
        Timing,

        [Name("每帧")]
        EveryFrame,

        [Name("触发")]
        Trigger,
    }

    /// <summary>
    /// 模型视图数据连接模式
    /// </summary>
    [Name("模型视图数据连接模式")]
    public enum EModelViewDataLinkMode
    {
        None = 0,

        /// <summary>
        /// 模型到视图
        /// </summary>
        [Name("模型-->视图")]
        [Tip("模型到视图", "Model to View")]
        ModelToView = 1 << 0,

        /// <summary>
        /// 视图到模型
        /// </summary>
        [Name("模型<--视图")]
        [Tip("视图到模型", "View to Model")]
        ViewToModel = 1 << 1,

        /// <summary>
        /// 同时允许模型到视图和视图到模型
        /// </summary>
        [Name("模型<-->视图")]
        [Tip("同时允许模型到视图和视图到模型", "Allow both model to view and view to model")]
        Both,

    }
}
