using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Kernel;
using XCSJ.Helper;
using XCSJ.LitJson;
using XCSJ.PluginCommonUtils;
using XCSJ.Tools;
using XCSJ.PluginTools;
using XCSJ.PluginCommonUtils.Tools;

namespace XCSJ.PluginMMO.NetSyncs
{
    [XCSJ.Attributes.Icon(EIcon.UIEvent)]
    [DisallowMultipleComponent]
    [Name("网络UGUI对象")]
    [Tool(MMOHelper.CategoryName, MMOHelper.ToolPurpose, rootType = typeof(MMOManager))]
    public class NetUGUI : NetMB
    {
        [Name("UGUI类型")]
        [EnumPopup]
        public EUGUIType uguiType = EUGUIType.Toggle;

        #region Toggle

        [Name("Toggle切换")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [HideInSuperInspector(nameof(uguiType), EValidityCheckType.NotEqual, EUGUIType.Toggle)]
        public Toggle _toggle;

        [SyncVar]
        [Readonly]
        [Name("Toggle状态")]
        public bool isOn = false;

        [Readonly]
        [Name("上一次Toggle状态")]
        public bool lastIsOn = false;

        [Readonly]
        [Name("原始Toggle状态")]
        public bool originalIsOn = false;

        #endregion

        #region DropDown

        [Name("DropDown下拉列表")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [HideInSuperInspector(nameof(uguiType), EValidityCheckType.NotEqual, EUGUIType.DropDown)]
        public Dropdown _dropDown = null;

        [SyncVar]
        [Readonly]
        [Name("Toggle状态")]
        public int index = 0;

        [Readonly]
        [Name("上一次Toggle状态")]
        public int lastIndex = 0;

        [Readonly]
        [Name("原始Toggle状态")]
        public int originalIndex = 0;

        #endregion

        public override void OnSyncEnable()
        {
            base.OnSyncEnable();

            if (!dataValid) return;

            switch (uguiType)
            {
                case EUGUIType.Toggle: originalIsOn = lastIsOn = isOn = _toggle.isOn; break;
                case EUGUIType.DropDown: originalIndex = lastIndex = index = _dropDown.value; break;
                default: break;
            }
        }

        public override void OnSyncDisable()
        {
            base.OnSyncDisable();

            if (!dataValid) return;

            switch (uguiType)
            {
                case EUGUIType.Toggle: _toggle.isOn = originalIsOn; break;
                case EUGUIType.DropDown: _dropDown.value = originalIndex; break;
                default: break;
            }
        }

        protected override bool OnTimedCheckChange()
        {
            if (!dataValid) return false;

            switch (uguiType)
            {
                case EUGUIType.Toggle:
                    {
                        isOn = _toggle.isOn;
                        return isOn != lastIsOn;
                    }
                case EUGUIType.DropDown:
                    {
                        index = _dropDown.value;
                        return index != lastIndex;
                    }
                default: return false;
            }
        }

        protected override void OnSyncVarChanged()
        {
            base.OnSyncVarChanged();

            if (!dataValid) return;

            switch (uguiType)
            {
                case EUGUIType.Toggle:
                    {
                        _toggle.isOn = isOn;
                        lastIsOn = isOn;
                        break;
                    }
                case EUGUIType.DropDown:
                    {
                        _dropDown.value = index;
                        lastIndex = index;
                        break;
                    }
                default:
                    break;
            }
        }

        private bool dataValid
        {
            get
            {
                switch (uguiType)
                {
                    case EUGUIType.Toggle: return _toggle;
                    case EUGUIType.DropDown: return _dropDown;
                    default:return false;
                }
            }
        }

        /// <summary>
        /// UGUI类型
        /// </summary>
        [Name("UGUI类型")]
        public enum EUGUIType
        {
            [Name("切换")]
            Toggle,

            [Name("下拉框")]
            DropDown,
        }
    }
}

