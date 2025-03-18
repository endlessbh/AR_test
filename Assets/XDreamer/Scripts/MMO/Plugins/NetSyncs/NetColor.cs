using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;
using XCSJ.Tools;
using XCSJ.PluginTools;
using XCSJ.PluginCommonUtils.Tools;

namespace XCSJ.PluginMMO.NetSyncs
{
    [XCSJ.Attributes.Icon(EIcon.Color)]
    [DisallowMultipleComponent]
    [Name("网络颜色")]
    [Tool(MMOHelper.CategoryName, MMOHelper.ToolPurpose, rootType = typeof(MMOManager))]
    public sealed class NetColor : NetMB, IAwake
    {
        [Name("渲染器")]
        [Readonly(EEditorMode.Runtime)]
        public Renderer _renderer;

        [SyncVar]
        [Readonly]
        [Name("颜色")]
        public List<Color> colors = new List<Color>();

        [Readonly]
        [Name("之前颜色")]
        public List<Color> prevColors = new List<Color>();

        [Readonly]
        [Name("原始颜色")]
        public List<Color> originalColors = new List<Color>();

        public void Awake()
        {
            if (!_renderer)
            {
                _renderer = GetComponent<Renderer>();
            }
        }

        public override void OnSyncEnable()
        {
            base.OnSyncEnable();

            GetRendererColor();
            originalColors.AddRange(colors);
            prevColors.AddRange(colors);
        }

        public override void OnSyncDisable()
        {
            base.OnSyncDisable();

            SetRendererColor(originalColors);
        }

        protected override bool OnTimedCheckChange()
        {
            GetRendererColor();

            for (int i = 0; i < colors.Count; i++)
            {
                if (i < prevColors.Count)
                {
                    if (colors[i] != prevColors[i])
                    {
                        prevColors.Clear();
                        prevColors.AddRange(colors);
                        return true;
                    }
                }
            }
            return false;
        }

        protected override void OnSyncVarChanged()
        {
            base.OnSyncVarChanged();

            SetRendererColor(colors);
        }

        private void GetRendererColor()
        {
            if (_renderer)
            {
                colors.Clear();
                foreach (var m in _renderer.materials)
                {
                    colors.Add(m ? m.color : Color.clear);
                }
            }
        }

        private void SetRendererColor(List<Color> colorList)
        {
            if (_renderer)
            {
                for (int i = 0; i < _renderer.materials.Length; i++)
                {
                    var m = _renderer.materials[i];
                    if (m && i < colorList.Count)
                    {
                        m.color = colorList[i];
                    }
                }
            }
        }
    }
}
