using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Runtime;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginXGUI.Base;
using XCSJ.PluginXGUI.DataViews.Base;

namespace XCSJ.PluginXGUI.DataViews.GroupViews
{
    /// <summary>
    /// 游戏对象视图
    /// </summary>
    [Name("游戏对象视图")]
    [DataViewAttribute(typeof(GameObject))]
#if UNITY_EDITOR && XDREAMER_EDITION_DEVELOPER
    //[Tool(XGUICategory.DataGroupView, rootType = typeof(XGUIManager))]
#endif
    public class GameObjectView : GroupView
    {
        public enum EGameObjectLinkRule
        {
            [Name("无")]
            None,

            [Name("选择集")]
            Selection,
        }

        [Name("游戏对象关联规则")]
        public EGameObjectLinkRule _gameObjectLinkRule = EGameObjectLinkRule.Selection;

        /// <summary>
        /// 关联游戏对象
        /// </summary>
        [Name("关联游戏对象")]
        [HideInSuperInspector(nameof(_gameObjectLinkRule), EValidityCheckType.NotEqual, EGameObjectLinkRule.None)]
        public GameObject _gameObject;

        /// <summary>
        /// 关联游戏对象
        /// </summary>
        public GameObject linkGameObject
        {
            get
            {
                switch (_gameObjectLinkRule)
                {
                    case EGameObjectLinkRule.None:
                        {
                            return _gameObject;
                        }
                    case EGameObjectLinkRule.Selection:
                        {
                            return Selection.selection;
                        }
                }
                return null;
            }
        }

        public override Type viewValueType => typeof(GameObject);

        public override object viewValue { get => _gameObject; set => _gameObject = (GameObject)value; }

        /// <summary>
        /// 隐藏无子数据视图的视图组件
        /// </summary>
        [Name("隐藏无子数据视图的视图组件")]
        public bool _hiddenComponentWhenNoChildView = true;

        private void OnValidate()
        {
            HiddenComponentViews();
        }

        /// <summary>
        /// 启用：绑定选择事件
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            CreateComponentViews();
            Selection.selectionChanged += OnSelectionChanged;
        }

        /// <summary>
        /// 启用：解除选择事件绑定
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            Selection.selectionChanged -= OnSelectionChanged;
        }

        private void BindGameObject(GameObject go)
        {
            if (!go) return;

            BindModel(transform);
        }

        /// <summary>
        /// 开始
        /// </summary>
        protected override void Start()
        {
            CreateComponentViews();
        }

        private List<ComponentView> componentViews = new List<ComponentView>();

        private void OnSelectionChanged(GameObject[] oldSelections, bool isUndoOrRedo)
        {
            if (_gameObjectLinkRule!= EGameObjectLinkRule.Selection) return;

            CreateComponentViews();
        }

        /// <summary>
        /// 为游戏对象上的组件创建视图集
        /// </summary>
        public void CreateComponentViews()
        {
            foreach (var v in componentViews)
            {
                DestroyImmediate(v.gameObject);
            }
            componentViews.Clear();

            var go = linkGameObject;
            if (!go) return;
            SetLabelText(go.name);

            foreach (Component component in go.GetComponents<Component>())
            {
                if (DataViewHelper.CreateDataView(component) is ComponentView componentView && componentView)
                {
                    componentView.XSetName(component.GetType().Name);
                    componentView.transform.XSetTransformParent(_parent ? _parent : transform);
                    componentView.BindModel(component);

                    componentViews.Add(componentView);
                }
            }

            HiddenComponentViews();
        }

        private void HiddenComponentViews()
        {
            foreach (var item in componentViews)
            {
                if (_hiddenComponentWhenNoChildView)
                {
                    if (item.GetComponentsInChildren<BaseModelView>().Length <= 1)
                    {
                        item.gameObject.SetActive(false);
                    }
                    else
                    {
                        item.gameObject.XSetActive(true);
                    }
                }
                else
                {
                    item.gameObject.XSetActive(true);
                }
            }
        }
    }
}
