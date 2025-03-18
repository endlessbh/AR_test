using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Caches;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Base;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginXGUI.Base;
using XCSJ.PluginXGUI.DataViews.Base;

namespace XCSJ.PluginXGUI.DataViews.TypeViews
{
    /// <summary>
    /// 枚举下拉框数据视图
    /// </summary>
    [Name("枚举下拉框数据视图")]
    [DataViewAttribute(typeof(Enum))]
#if UNITY_EDITOR && XDREAMER_EDITION_DEVELOPER
    //[Tool(XGUICategory.DataTypeView, rootType = typeof(XGUIManager))]
#endif
    public sealed class EnumDropdownView : BaseModelViewCache
    {
        /// <summary>
        /// 下拉框
        /// </summary>
        [Name("下拉框")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Dropdown _enumDropdownView;

        private Type enumType = null;
        private EnumTypeData enumTypeData => EnumTypeData.GetEnumTypeData(enumType);

        private const string NothingFlags = "Nothing";

        public override Type viewValueType => enumType;

        public override object viewValue 
        { 
            get
            {
                var enumTypeData = this.enumTypeData;
                if (enumTypeData == null) return null;
                if (enumTypeData.hasFlagsAttribute)
                {
                    // Nothings
                    if (_enumDropdownView.value == 0)
                    {
                        return 0;
                    }
                    else// 使用文本进行反向赋值
                    {
                        return EnumValueCache.Get(enumType, _enumDropdownView.options[_enumDropdownView.value].text, EEnumStringType.Default);
                    }
                }
                else
                {
                    // 处理非flags，使用值进行处理，目前仅局限从0开始连续枚举值的对象
                    return enumTypeData.GetEnumByIndex(_enumDropdownView.value);
                }
            }
            set
            {
                var enumTypeData = this.enumTypeData;
                if (enumTypeData.hasFlagsAttribute)
                {
                    if ((int)value == 0)
                    {
                        _enumDropdownView.value = 0;
                    }
                    else
                    {
                        var option = value.ToString();
                        var index = _enumDropdownView.options.FindIndex(op => op.text == option);
                        if (index >= 0)
                        {
                            _enumDropdownView.value = index;
                        }
                    }
                }
                else
                {
                    _enumDropdownView.value = enumTypeData.GetIndex((Enum)value);
                }
            }
        }

        /// <summary>
        /// 启用：绑定UI事件
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            enumType = modelValueType;
            InitDropdownOptionData();
            _enumDropdownView.onValueChanged.AddListener(OnValueChanged);
        }

        /// <summary>
        /// 禁用：解除UI事件绑定
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            _enumDropdownView.onValueChanged.RemoveListener(OnValueChanged);
        }

        private void ResetData()
        {
            enumType = null;
        }

        private void OnValueChanged(int index)
        {
            if (inModelToView) return;

            ViewToModelIfCanAndTrigger();
        }

        private void InitDropdownOptionData()
        {
            var etd = enumTypeData;
            if (etd == null) return;
            _enumDropdownView.options.Clear();

            if (etd.hasFlagsAttribute)
            {
                int count = etd.displayNames.Length;

                if (count == 0) return;

                var textList = new List<string>();
                for (int loopCount = 1; loopCount <= count; loopCount++)
                {
                    for (int firstIndex = 0; firstIndex < count; firstIndex++)
                    {
                        var firstStr = etd.displayNames[firstIndex];
                        if (loopCount == 1)// 第一次循环
                        {
                            textList.Add(firstStr);
                        }
                        else// 第二次及以上循环
                        {
                            for (int secondIndex = firstIndex + 1; secondIndex < count; secondIndex++)
                            {
                                if ((secondIndex + loopCount - 2) < count)
                                {
                                    var str = firstStr;
                                    for (int j = 0; j <= loopCount - 2; j++)
                                    {
                                        str += ", ";
                                        str += etd.displayNames[secondIndex + j];// flags枚举默认使用逗号+空格分隔成多个叠加枚举位
                                    }
                                    textList.Add(str);
                                }
                            }
                        }
                    }
                }

                _enumDropdownView.options.Add(new Dropdown.OptionData(NothingFlags));
                foreach (var text in textList)
                {
                    _enumDropdownView.options.Add(new Dropdown.OptionData(text));
                }
            }
            else
            {
                foreach (var item in etd.displayNames)
                {
                    _enumDropdownView.options.Add(new Dropdown.OptionData(item.ToString()));
                }
            }

            _enumDropdownView.RefreshShownValue();
        }
    }
}