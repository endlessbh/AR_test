using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginTools.Notes.Dimensionings
{
    /// <summary>
    /// 尺寸标注
    /// </summary>
    [Name("尺寸标注")]
    [RequireManager(typeof(ToolsManager))]
    public abstract class Dimensioning : InteractProvider
    {
        /// <summary>
        /// 起点尺寸界线
        /// </summary>
        public abstract MeshLine beginExtensionLine { get; }

        /// <summary>
        /// 终点尺寸界线
        /// </summary>
        public abstract MeshLine endExtensionLine { get; }

        /// <summary>
        /// 尺寸线
        /// </summary>
        public abstract MeshLine dimensionLine { get; }

        /// <summary>
        /// 尺寸数字
        /// </summary>
        public abstract SizeDigital sizeDigital { get; }

        /// <summary>
        /// 更新
        /// </summary>
        protected abstract void Update();

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            beginExtensionLine?.OnEnable(this);
            endExtensionLine?.OnEnable(this);
            dimensionLine?.OnEnable(this);
            sizeDigital?.OnEnable(this);
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            beginExtensionLine?.OnDisable(this);
            endExtensionLine?.OnDisable(this);
            dimensionLine?.OnDisable(this);
            sizeDigital?.OnDisable(this);
        }

        /// <summary>
        /// 重置
        /// </summary>
        public virtual void Reset()
        {
            Create();
        }

        /// <summary>
        /// 验证
        /// </summary>
        public virtual void OnValidate()
        {
            if (Application.isPlaying) return;
            Update();
        }

        /// <summary>
        /// 创建
        /// </summary>
        public virtual void Create()
        {
            beginExtensionLine?.Create(this, transform, nameof(beginExtensionLine));
            endExtensionLine?.Create(this, transform, nameof(endExtensionLine));
            dimensionLine?.Create(this, transform, nameof(dimensionLine));
            sizeDigital?.Create(this, transform);
        }
    }
}
