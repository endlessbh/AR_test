using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginTools.Inputs;
using XCSJ.PluginXGUI.Base;
using XCSJ.PluginXGUI.Windows.ColorPickers;

namespace XCSJ.PluginXGUI.Widgets
{
    /// <summary>
    /// XGUI通用资产
    /// </summary>
    [Name("XGUI通用资产")]
    [Tip("用于管理和加载常用的XGUI资源", "Used to manage and load commonly used xgui resources")]
    [RequireComponent(typeof(XGUIManager))]
    [RequireManager(typeof(XGUIManager))]
    [ExecuteInEditMode]
    public class XGUICommonAssets : InteractProvider
    {
        /// <summary>
        /// 对话框
        /// </summary>
        [Name("对话框")]
        public DialogBox _dialogBox;

        /// <summary>
        /// 日志窗口
        /// </summary>
        [Name("日志窗口")]
        public LogViewController _logViewController;

        /// <summary>
        /// 提示弹出框
        /// </summary>
        [Name("提示弹出框")]
        public TipPopup _tipPopup;

        /// <summary>
        /// 调色板
        /// </summary>
        [Name("调色板")]
        public ColorPicker _colorPicker;

        /// <summary>
        /// 弹出菜单
        /// </summary>
        [Name("弹出菜单")]
        public MenuGenerator _menuGenerator;

        private void OnValidate()
        {
            UpdateGlobalVar();
        }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            UpdateGlobalVar();
        }

        private void UpdateGlobalVar()
        {
            XGUIHelper.dialogBox = _dialogBox;
            XGUIHelper.logViewController = _logViewController;
            XGUIHelper.tipPopup = _tipPopup;
            XGUIHelper.colorPicker = _colorPicker;
            XGUIHelper.globalMenuGenerator = _menuGenerator;
        }

        /// <summary>
        /// 获取指定类型的资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetAsset<T>() where T : MB
        {
            if (typeof(T) == typeof(DialogBox))
            {
                return _dialogBox as T;
            }
            else if (typeof(T) == typeof(LogViewController))
            {
                return _logViewController as T;
            }
            else if (typeof(T) == typeof(TipPopup))
            {
                return _tipPopup as T;
            }
            else if (typeof(T) == typeof(ColorPicker))
            {
                return _colorPicker as T;
            }
            else if (typeof(T) == typeof(MenuGenerator))
            {
                return _menuGenerator as T;
            }

            return default;
        }

        public void SetAsset(MonoBehaviour component)
        {
            var targetType = component.GetType();

            if (targetType == typeof(DialogBox))
            {
                _dialogBox = component as DialogBox;
            }
            else if (targetType == typeof(LogViewController))
            {
                _logViewController = component as LogViewController;
            }
            else if (targetType == typeof(TipPopup))
            {
                _tipPopup = component as TipPopup;
            }
            else if (targetType == typeof(ColorPicker))
            {
                _colorPicker = component as ColorPicker;
            }
            else if (targetType == typeof(MenuGenerator))
            {
                _menuGenerator = component as MenuGenerator;
            }

            UpdateGlobalVar();
        }

        public void SetAsset<T>(GameObject go) where T : MB
        {
            if (!go) return;

            var c = go.GetComponent<T>();
            if (!c) return;

            SetAsset(c);
        }

        /// <summary>
        /// 设置
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="go"></param>
        public static void SetAssetIfEmpty<T>(GameObject go) where T : MB
        {
            if (!go) return;

            var t = go.GetComponent<T>();
            if (!t) return;

            var asset = XGUIManager.instance.XGetOrAddComponent<XGUICommonAssets>();

            var target = asset.GetAsset<T>();
            if (!target)
            {
                asset.SetAsset(t);
            }
        }
    }

    /// <summary>
    /// 资产源
    /// </summary>
    public enum EAssetSource
    {
        [Name("通用资产")]
        CommonAssets,

        [Name("自定义")]
        Custom,
    }

    public class XGUIAsset<T> where T : MB
    {
        [Name("资产来源")]
        [EnumPopup]
        public EAssetSource _assetSource = EAssetSource.CommonAssets;

        [HideInSuperInspector(nameof(_assetSource), EValidityCheckType.NotEqual, EAssetSource.Custom)]
        public T _view;

        private XGUICommonAssets XGUICommonAssets = null;

        public T view
        {
            get
            {
                switch (_assetSource)
                {
                    case EAssetSource.CommonAssets:
                        {
                            if (!XGUICommonAssets && XGUIManager.instance)
                            {
                                XGUICommonAssets = XGUIManager.instance.GetComponent<XGUICommonAssets>();
                            }

                            if (XGUICommonAssets)
                            {
                                return XGUICommonAssets.GetAsset<T>();
                            }
                            break;
                        }
                    case EAssetSource.Custom: return _view;
                }
                return null;
            }
        }
    }

    [Serializable]
    public class DialogBoxAsset : XGUIAsset<DialogBox> { }

    [Serializable]
    public class LogViewControllerAsset : XGUIAsset<LogViewController> { }

    [Serializable]
    public class TipPopupAsset : XGUIAsset<TipPopup> { }

    [Serializable]
    public class ColorPickerAsset : XGUIAsset<ColorPicker> { }

    [Serializable]
    public class PopupMenuAsset : XGUIAsset<MenuGenerator> { }
}
