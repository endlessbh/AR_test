using UnityEngine;
using System;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using System.Collections.Generic;
using XCSJ.PluginXGUI.Styles.Base;
using System.Linq;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginXGUI.Base;
using XCSJ.Extension.Interactions.Tools;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace XCSJ.PluginXGUI.Styles.Tools
{
    /// <summary>
    /// 样式链接器
    /// </summary>
    [Name("样式链接器")]
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    [XCSJ.Attributes.Icon(EIcon.Color)]
    [Tool(XGUICategory.Component, rootType = typeof(XGUIManager))]
    [RequireManager(typeof(XGUIManager))]
    [Owner(typeof(XGUIManager))]
    public class StyleLinker : InteractProvider, IComponentCollection
    {
        /// <summary>
        /// 样式规则
        /// </summary>
        [Name("样式规则")]
        public enum EStyleRule
        {
            [Name("默认")]
            Default,

            [Name("自定义")]
            Custom,

            [Name("父类")]
            Parent,
        }

        /// <summary>
        /// 样式规则
        /// </summary>
        [Name("样式规则")]
        [EnumPopup]
        public EStyleRule _styleRule = EStyleRule.Default;

        /// <summary>
        /// 样式名称
        /// </summary>
        [Name("样式名称")]
        [HideInSuperInspector(nameof(_styleRule), EValidityCheckType.NotEqual, EStyleRule.Custom)]
        public string _customStyleName;

        /// <summary>
        /// 样式元素链接规则
        /// </summary>
        [Name("样式元素链接规则")]
        [EnumPopup]
        public EStyleElementLinkRule _elementLinkRule = EStyleElementLinkRule.CollectionFirstThenBase;

        [Name("样式元素链接规则")]
        public enum EStyleElementLinkRule
        {
            [Name("基础")]
            Base,

            [Name("组合")]
            Collection,

            [Name("组合优先其次基础")]
            CollectionFirstThenBase,
        }

        [Name("组合元素名称")]
        [HideInSuperInspector(nameof(_elementLinkRule), EValidityCheckType.Equal, EStyleElementLinkRule.Base)]
        public string _styleElementCollectionName = "";

        /// <summary>
        /// 组件样式列表
        /// </summary>
        [Name("组件样式列表")]
        [ArrayElement(EArrayElementHandleRule.None)]
        public List<ComponentStyle> _componentStyles = new List<ComponentStyle>();

        /// <summary>
        /// 样式
        /// </summary>
        public XStyle style
        {
            get
            {
                switch (_styleRule)
                {
                    case EStyleRule.Default: return XStyleCache.defaultStyle;
                    case EStyleRule.Custom: return XStyleCache.GetStyle(_customStyleName);
                    case EStyleRule.Parent: return parentLinker && parentLinker.enabled ? parentLinker.style : null;
                    default: return null;
                }
            }
        }

        /// <summary>
        /// 父级对象连接器：往父级上层查找
        /// </summary>
        private StyleLinker parentLinker
        {
            get
            {
                if (!_parentLinker)
                {
                    var current = transform.parent;
                    while (current)
                    {
                        _parentLinker = current.GetComponent<StyleLinker>();
                        if (_parentLinker)
                        {
                            break;
                        }
                        else
                        {
                            current = current.parent;
                        }
                    }
                }
                return _parentLinker;
            }
        }
        private StyleLinker _parentLinker;

        /// <summary>
        /// 通过样式元素链接规则，获取到样式集合
        /// </summary>
        public StyleElementCollection styleElementCollection
        {
            get 
            {
                switch (_elementLinkRule)
                {
                    case EStyleElementLinkRule.Base: return style;
                    case EStyleElementLinkRule.Collection:
                        {
                            if (style)
                            {
                                return style.GetElement<StyleElementCollection>(_styleElementCollectionName);
                            }
                            break;
                        }
                    case EStyleElementLinkRule.CollectionFirstThenBase:
                        {
                            if (style)
                            {
                                var sec = style.GetElement<StyleElementCollection>(_styleElementCollectionName);
                                return sec ? sec : style;
                            }
                            break;
                        }
                }
                return null;
            }
        }

        /// <summary>
        /// 组件集合接口
        /// </summary>
        public Component[] components => GetComponents<Component>();

        /// <summary>
        /// 重置
        /// </summary>
        protected void Reset() => InitComponentStyle(this);

        /// <summary>
        /// 重置数据
        /// </summary>
        protected void OnValidate()
        {
            if (enabled) SetData();
        }

        /// <summary>
        /// 初始化组件样式
        /// </summary>
        /// <param name="componentCollection"></param>
        private void InitComponentStyle(IComponentCollection componentCollection)
        {
            _componentStyles.Clear();
            foreach (var c in componentCollection.components)
            {
                if (StyleHelper.ExistLinkStyleElement(c))
                {
                    _componentStyles.Add(new ComponentStyle(c));
                }
            }
        }

        /// <summary>
        /// 更新组件样式关联名称
        /// </summary>
        /// <param name="styleElementCollection"></param>
        public void UpdateComponentStyleName(StyleElementCollection styleElementCollection)
        {
            _componentStyles.ForEach(cs => cs.SetElementName(styleElementCollection));
        }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            XStyleCache.onUpdateStyle += OnStyleChanged;

            SetData();
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            XStyleCache.onUpdateStyle -= OnStyleChanged;
        }

        private void SetData()
        {
            if (!style) return;

            if (string.IsNullOrEmpty(_styleElementCollectionName))
            {
                _styleElementCollectionName = style.GetElementNames(typeof(StyleElementCollection)).FirstOrDefault();
            }
            this.XModifyProperty(() =>
            {
                UpdateComponentStyleName(styleElementCollection);
            });

            TryStyleToObject();
        }

        /// <summary>
        /// 响应样式变化回调
        /// </summary>
        private void OnStyleChanged() => TryStyleToObject();

        /// <summary>
        /// 应用样式修改
        /// </summary>
        /// <returns></returns>
        public bool TryStyleToObject() => UpdateData(true);

        /// <summary>
        /// 从对象中提取样式，并设置至样式中
        /// </summary>
        public void TryObjectToStyle() => UpdateData(false);

        /// <summary>
        /// 使用组件样式列表设置数据
        /// </summary>
        /// <param name="action"></param>
        private bool UpdateData(bool isStyleToObject)
        {
            var isUpdate = false;
            var _currentStyle = styleElementCollection; // 缓存
            if (_currentStyle)
            {
                foreach (var cs in _componentStyles)
                {
                    isUpdate = cs.UpdateData(_currentStyle, isStyleToObject);
                }

#if UNITY_EDITOR
                // Unity编辑器非运行态，需标记场景修改，刷新Scene或游戏视图窗口
                if (isUpdate && !Application.isPlaying)
                {
                    UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
                }
#endif
            }
            return isUpdate;
        }
    }
}
