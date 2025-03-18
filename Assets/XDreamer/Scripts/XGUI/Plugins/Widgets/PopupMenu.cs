using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Extension.Interactions.Base;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginsCameras;
using XCSJ.PluginTools.Inputs;
using XCSJ.PluginTools.Notes.Tips;
using XCSJ.PluginXGUI.Views.ScrollViews;
using XCSJ.PluginXGUI.Windows;

namespace XCSJ.PluginXGUI.Widgets
{
    /// <summary>
    /// 弹出菜单
    /// </summary>
    [Name("弹出菜单")]
    public class PopupMenu : UIContainer<MenuItemView>
    {
        public static event Action<PopupMenu, bool> onShow;

        /// <summary>
        /// 菜单内容域:用于动态控制菜单高度
        /// </summary>
        [Name("菜单内容域")]
        [Tip("用于动态控制菜单高度", "For dynamic control of menu height")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public RectTransform _menuContentArea;

        /// <summary>
        /// 菜单内容域扩展尺寸
        /// </summary>
        [Name("菜单内容域扩展尺寸")]
        public Vector2 _menuExtendSize = Vector2.zero;

        /// <summary>
        /// 弹出位置
        /// </summary>
        [Name("弹出位置")]
        public PopupPosition _popupPosition = new PopupPosition();

        private UGUIWindow _uguiWindow = null;

        protected virtual void Awake()
        {
            _uguiWindow = GetComponent<UGUIWindow>();
        }

        /// <summary>
        /// 显示菜单位置
        /// </summary>
        /// <param name="position"></param>
        /// <param name="interactDatas"></param>
        public void Show(string title, Vector3 position, params InteractData[] interactDatas)
        {
            // 设置游戏对象激活，并设定画布组可交互
            gameObject.XSetActive(true);

            rectTransform.position = position;

            // 设置窗口标题
            if (_uguiWindow)
            {
                _uguiWindow.titleText = title;
            }

            // 创建菜单项
            pool.Clear();

            foreach (var item in interactDatas)
            {
                var menuItemView = pool.Alloc();
                if (menuItemView)
                {
                    menuItemView.SetData(this, item);
                }
            }

            // 动态设定父级高度
            if (_menuContentArea)
            {
                var size = _menuContentArea.sizeDelta;
                size.y = _template.rectTransform.sizeDelta.y * itemCount;
                _menuContentArea.sizeDelta = size + _menuExtendSize;
            }

            onShow?.Invoke(this, true);
        }

        /// <summary>
        /// 显示菜单
        /// </summary>
        /// <param name="actions"></param>
        public void Show(string title, params InteractData[] interactDatas)
        {
            var pos = rectTransform.position;
            if (_popupPosition.TryGetPosition(interactDatas.FirstOrDefault(), out var value))
            {
                pos = value;
            }
            Show(title, pos, interactDatas);
        }

        /// <summary>
        /// 隐藏菜单
        /// </summary>
        public void Hidden()
        {
            pool.Clear();

            gameObject.XSetActive(false);

            onShow?.Invoke(this, false);
        }

        /// <summary>
        /// 创建项回调
        /// </summary>
        /// <param name="item"></param>
        protected override void OnNewItem(MenuItemView item)
        {
            item.transform.SetAsLastSibling();
        }

        /// <summary>
        /// 分配项回调
        /// </summary>
        /// <param name="item"></param>
        protected override void OnAllocItem(MenuItemView item)
        {
            item.transform.SetAsLastSibling();
        }

        /// <summary>
        /// 释放回调
        /// </summary>
        /// <param name="item"></param>
        protected override void OnFreeItem(MenuItemView item)
        {
            base.OnFreeItem(item);

            item.ResetData();
        }
    }
}
