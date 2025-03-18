using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Extension.Interactions.Base;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginXGUI;
using XCSJ.PluginXGUI.Base;
using XCSJ.PluginXGUI.ViewControllers;
using XCSJ.PluginXGUI.Views.ScrollViews;
using static XCSJ.PluginTools.Inputs.MouseInput;

namespace XCSJ.PluginXGUI.Widgets
{
    /// <summary>
    /// 菜单项视图
    /// </summary>
    [Name("菜单项视图")]
    public class MenuItemView : UIItem<MenuItemView>
    {
        /// <summary>
        /// 菜单名称
        /// </summary>
        [Name("菜单项视图")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Text _menuName;

        /// <summary>
        /// 菜单按钮
        /// </summary>
        [Name("菜单按钮")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Button _menuButton;

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            _menuButton.onClick.AddListener(OnClick);
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            _menuButton.onClick.RemoveListener(OnClick);
        }

        /// <summary>
        /// 设置菜单数据
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="cmd"></param>
        public void SetData(PopupMenu parent, InteractData interactData)
        {
            this.parent = parent;
            menuData = interactData;

            _menuName.text = interactData != null ? interactData.cmdName : "";
        }

        /// <summary>
        /// 重置数据
        /// </summary>
        public void ResetData()
        {
            parent = null;
            menuData = null;
        }

        private PopupMenu parent; // 菜单项对应父级控制器

        private InteractData menuData { get; set; }// 菜单数据

        public static event Action<MenuItemView, PopupMenu, string, InteractData> onMenuClick;

        /// <summary>
        /// 点击
        /// </summary>
        private void OnClick()
        {
            Debug.Log("菜单项："+ _menuName.text);
            //Debug.Log("交互器：" + menuData.interactor.name + ",可交互对象：" + menuData.interactable.name);

            // 执行交互
            menuData.TryInteract();

            // 菜单点击回调
            onMenuClick?.Invoke(this, parent, _menuName.text, menuData);

            parent.Hidden();
        }
    }
}