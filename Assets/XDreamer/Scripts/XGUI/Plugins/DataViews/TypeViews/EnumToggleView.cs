using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Caches;
using XCSJ.Collections;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Base;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginXGUI.Base;
using XCSJ.PluginXGUI.DataViews.Base;

namespace XCSJ.PluginXGUI.DataViews.TypeViews
{
    /// <summary>
    /// 枚举切换数据视图
    /// </summary>
    [Name("枚举切换数据视图")]
    [DataViewAttribute(typeof(Enum))]
#if UNITY_EDITOR && XDREAMER_EDITION_DEVELOPER
    //[Tool(XGUICategory.DataTypeView, rootType = typeof(XGUIManager))]
#endif
    public class EnumToggleView : BaseModelViewCache
    {
        /// <summary>
        /// 切换组，用于将切换进行互斥
        /// </summary>
        [Name("切换组")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public ToggleGroup _toggleGroup;

        /// <summary>
        /// 使用切换规则
        /// </summary>
        public enum EUseToggleRule
        {
            [Name("列表")]
            List,

            [Name("模版")]
            Template,
        }

        /// <summary>
        /// 使用切换规则
        /// </summary>
        [Name("使用切换规则")]
        [EnumPopup]
        public EUseToggleRule _useToggleRule = EUseToggleRule.List;

        /// <summary>
        /// 切换列表
        /// </summary>
        [Name("切换列表")]
        [Array]
        [HideInSuperInspector(nameof(_useToggleRule), EValidityCheckType.NotEqual, EUseToggleRule.List)]
        public List<Toggle> _toggleList = new List<Toggle>();

        /// <summary>
        /// 切换模版
        /// </summary>
        [Name("切换模版")]
        [HideInSuperInspector(nameof(_useToggleRule), EValidityCheckType.NotEqual, EUseToggleRule.Template)]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Toggle _toggleTemplate;

        /// <summary>
        /// 切换枚举映射图 参数1=切换对象，参数2.1=枚举对象，参数2.2=枚举对象在被模型设置之后的值，主要用于视图处理Flags特性枚举的缓存值
        /// </summary>
        private Dictionary<Toggle, EnumBoolInfo> toggleEnumMap = new Dictionary<Toggle, EnumBoolInfo>();

        private class EnumBoolInfo
        {
            public Enum enumObj;
            public bool toggleValue;
        }

        private Type enumType = null;

        /// <summary>
        /// 枚举类型数据
        /// </summary>
        private EnumTypeData enumTypeData => EnumTypeData.GetEnumTypeData(enumType);

        /// <summary>
        /// 视图值类型
        /// </summary>
        public override Type viewValueType => modelValueType;

        /// <summary>
        /// 视图值
        /// </summary>
        public override object viewValue 
        { 
            get
            {
                if (!valid) return default;

                if (enumTypeData.hasFlagsAttribute)
                {
                    int mask = 0;
                    foreach (var item in toggleEnumMap)
                    {
                        if (item.Key.isOn)
                        {
                            mask |= enumTypeData.ToEnumFlagsIntValue(item.Value.enumObj);
                        }
                    }
                    return mask;
                }
                else
                {
                    foreach (var item in toggleEnumMap)
                    {
                        if (item.Key.isOn)
                        {
                            return item.Value.enumObj;
                        }
                    }
                }
                return 0;
            }
            set
            {
                if (!valid) return;
                if (value == null || !value.GetType().IsEnum) return;

                if (enumTypeData.hasFlagsAttribute)
                {
                    var valueMask = enumTypeData.ToEnumFlagsIntValue(value as Enum);
                    foreach (var item in toggleEnumMap)
                    {
                        var mask = enumTypeData.ToEnumFlagsIntValue(item.Value.enumObj);
                        var isOn = (mask & valueMask) == mask; 
                        item.Key.isOn = isOn;
                        item.Value.toggleValue = isOn;
                    }
                }
                else
                {
                    foreach (var item in toggleEnumMap)
                    {
                        if (item.Value.enumObj.Equals(value))
                        {
                            item.Key.isOn = true;
                            break;
                        }
                    }
                }
            }
        }

        private EUseToggleRule useToggleRuleOnEnable = EUseToggleRule.List;

        /// <summary>
        /// 启用：创建Toggle Enum 映射图
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            useToggleRuleOnEnable = _useToggleRule;
            enumType = modelValueType;
            CreateToggleEnumMap();
        }

        /// <summary>
        /// 禁用：销毁Toggle Enum 映射图
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            DestoryToggleEnumMap();
        }

        private bool valid => enumType != null && enumType.IsEnum;

        private WorkObjectPool<Toggle> togglePool
        {
            get
            {
                if (_togglePool == null && useToggleRuleOnEnable == EUseToggleRule.Template && _toggleTemplate)
                {
                    _togglePool = new WorkObjectPool<Toggle>();
                    _toggleTemplate.gameObject.SetActive(false);

                    _togglePool.Init(() =>
                        {
                            if (_toggleTemplate)
                            {
                                var newGO = _toggleTemplate.gameObject.XCloneAndSetParent(_toggleTemplate.transform.parent);
                                return newGO.GetComponent<Toggle>();
                            }
                            return null;
                        },
                        item =>
                        {
                            item.gameObject.SetActive(true);
                            item.transform.SetAsLastSibling();
                        },
                        item =>
                        {
                            item.gameObject.SetActive(false);
                        },
                        item => item);
                }
                return _togglePool;
            }
        }
        private WorkObjectPool<Toggle> _togglePool = null;

        private void CreateToggleEnumMap()
        {
            if (!valid) return;

            var enumTypeData = this.enumTypeData;

            switch (useToggleRuleOnEnable)
            {
                case EUseToggleRule.List:
                    {
                        BindToggleList(_toggleList);
                        break;
                    }
                case EUseToggleRule.Template:
                    {
                        if (!_toggleTemplate)
                        {
                            Debug.LogErrorFormat("{0}:切换模版无效，无法有效初始化切换组！", CommonFun.GameObjectComponentToString(this));
                            return;
                        }
                        var toggles = new List<Toggle>();
                        var count = enumTypeData.displayNames.Length;
                        while (count > 0)
                        {
                            --count;
                            toggles.Add(togglePool.Alloc());
                        }
                        BindToggleList(toggles);
                        break;
                    }
            }
            SetToggleGroup(!enumTypeData.hasFlagsAttribute);
        }

        private void BindToggleList(List<Toggle> toggles)
        {
            var enumDataLength = enumTypeData.displayNames.Length;
            if (toggles.Count != enumDataLength)
            {
                Debug.LogErrorFormat("{0}：切换列表数量需等于{1}", CommonFun.GameObjectComponentToString(this), enumDataLength);
                return;
            }

            for (int i = 0; i < enumDataLength; i++)
            {
                var toggle = toggles[i];
                if (toggle)
                {
                    if (toggleEnumMap.ContainsKey(toggle))
                    {
                        Debug.LogErrorFormat("{0}：在切换列表中不能重复!", CommonFun.GameObjectComponentToString(toggle));
                    }
                    else
                    {
                        var text = toggle.GetComponentInChildren<Text>();
                        if (text)
                        {
                            text.text = enumTypeData.displayNames[i];// 使用枚举值设置文本名称
                        }
                        toggle.onValueChanged.AddListener(OnToggleChanged);


                        toggleEnumMap.Add(toggle, new EnumBoolInfo() { enumObj = enumTypeData.GetEnumByIndex(i), toggleValue = toggle.isOn });
                    }
                }
                else
                {
                    Debug.LogErrorFormat("{0}：切换列表第{1}个不能为空对象!", CommonFun.GameObjectComponentToString(this), i+1);
                }
            }
        }

        private void SetToggleGroup(bool useGroup)
        {
            foreach (var item in toggleEnumMap)
            {
                if (item.Key) item.Key.group = useGroup ? _toggleGroup : null;
            }
        }

        private void DestoryToggleEnumMap()
        {
            foreach (var item in toggleEnumMap)
            {
                if (item.Key) item.Key.onValueChanged.RemoveListener(OnToggleChanged);
            }
            toggleEnumMap.Clear();

            if (useToggleRuleOnEnable == EUseToggleRule.Template)
            {
                togglePool.Clear();
            }
        }

        private void OnToggleChanged(bool isOn)
        {
            if (inModelToView || disableViewToModel) return;

            //有Flags特性枚举时：找出由开启至关闭的Toggle对象，将其他与之关联位的Toggle进行关闭操作
            if (enumTypeData.hasFlagsAttribute && !isOn)
            {
                TurnFlagsToggleOff();                
            }

            ViewToModelIfCanAndTrigger();

            //有Flags特性枚举时：在视图更新模型之后，再使用模型数据强制刷新一次视图，激活位操作匹配的对象
            if (enumTypeData.hasFlagsAttribute && isOn)
            {
                ModelToViewIfCan();
            }
        }

        /// <summary>
        /// 找出由开启至关闭的Toggle对象，将其他与之关联位的Toggle进行关闭操作
        /// </summary>
        private void TurnFlagsToggleOff()
        {
            try
            {
                disableViewToModel = true;

                // 找出由开启至关闭的Toggle对象
                int currentMask = 0;
                Enum currentEnum = null;
                foreach (var item in toggleEnumMap)
                {
                    if (item.Key.isOn != item.Value.toggleValue)
                    {
                        currentEnum = item.Value.enumObj;
                        currentMask = enumTypeData.ToEnumFlagsIntValue(item.Value.enumObj);
                        break;
                    }
                }
                // 对关联位的Toggle进行关闭操作
                foreach (var item in toggleEnumMap)
                {
                    if (item.Value.enumObj == currentEnum)
                    {
                        continue;
                    }
                    var mask = enumTypeData.ToEnumFlagsIntValue(item.Value.enumObj);

                    if (((mask & currentMask) == mask) || ((mask & currentMask) == currentMask))
                    {
                        item.Key.isOn = false;
                        item.Value.toggleValue = false;
                    }
                }
            }
            finally
            {
                disableViewToModel = false;
            }
        }

        private bool disableViewToModel = false;
    }
}
