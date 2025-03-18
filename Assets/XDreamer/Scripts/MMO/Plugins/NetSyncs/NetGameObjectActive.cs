using System;
using System.Collections.Generic;
using UnityEngine;
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
    [XCSJ.Attributes.Icon(EIcon.GameObjectActive)]
    [DisallowMultipleComponent]
    [Name("网络游戏对象激活")]
    [Tool(MMOHelper.CategoryName, MMOHelper.ToolPurpose, rootType = typeof(MMOManager))]
    public class NetGameObjectActive : NetMB, IAwake
    {
        [Name("游戏对象")]
        public GameObject go;

        [SyncVar]
        [Readonly]
        [Name("激活状态")]
        public bool activeState = false;

        [Readonly]
        [Name("上一次激活状态")]
        public bool lastActiveState = false;

        [Readonly]
        [Name("原始激活状态")]
        public bool originalActiveState = false;

        public void Awake()
        {
            if (!go)
            {
                go = gameObject;
            }
        }

        public override void OnSyncEnable()
        {
            base.OnSyncEnable();
            originalActiveState = lastActiveState = activeState = go.activeSelf;
        }

        public override void OnSyncDisable()
        {
            base.OnSyncDisable();
            go.SetActive(originalActiveState);
        }

        protected override bool OnTimedCheckChange()
        {
            activeState = go.activeSelf;
            return activeState != lastActiveState;
        }

        protected override void OnSyncVarChanged()
        {
            base.OnSyncVarChanged();

            go.SetActive(activeState);
            lastActiveState = activeState;
        }
    }
}
