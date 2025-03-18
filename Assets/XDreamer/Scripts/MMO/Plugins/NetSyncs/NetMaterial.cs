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
    [XCSJ.Attributes.Icon(EIcon.Material)]
    [DisallowMultipleComponent]
    [Name("网络材质")]
    [Tool(MMOHelper.CategoryName, MMOHelper.ToolPurpose, rootType = typeof(MMOManager))]
    public class NetMaterial : NetMB, IAwake
    {
        [Name("渲染器")]
        public Renderer _renderer;

        [SyncVar]
        [Readonly]
        [Name("材质名")]
        public string materialName;

        [Readonly]
        [Name("之前材质名")]
        public string prevMaterialName;

        [Readonly]
        [Name("原始材质")]
        public Material originalMaterial;

        public void Awake()
        {
            if(!_renderer)
            {
                _renderer = GetComponent<Renderer>();
                if (_renderer)
                {
                    // 记录以前的材质
                    UnityAssetObjectManager.instance.Add(_renderer.material);
                }
            }
        }

        public override void OnSyncEnable()
        {
            base.OnSyncEnable();

            if (_renderer)
            {
                originalMaterial = _renderer.material;
                if (originalMaterial)
                {
                    prevMaterialName = materialName = originalMaterial.name;
                }
            }
        }

        public override void OnSyncDisable()
        {
            base.OnSyncDisable();
            if (_renderer)
            {
                _renderer.material = originalMaterial;
            }
        }

        protected override bool OnTimedCheckChange()
        {
            if (_renderer && _renderer.material)
            {
                materialName = _renderer.material.name;
            }
            return materialName != prevMaterialName;
        }

        protected override void OnSyncVarChanged()
        {
            base.OnSyncVarChanged();
            if (_renderer)
            {
                if (!SetRenderMaterial(materialName))
                {
                    int length = materialName.LastIndexOf(" (Instance)");
                    if (length >= 0)
                    {
                        SetRenderMaterial(materialName.Substring(0, length));
                    }
                }

                prevMaterialName = materialName;
            }
        }

        private bool SetRenderMaterial(string matName)
        {
            Material newMat = UnityAssetObjectManager.instance.GetUnityAssetObject<Material>(matName);
            if (newMat)
            {
                _renderer.material = newMat;
                return true;
            }
            return false;
        }
    }
}
