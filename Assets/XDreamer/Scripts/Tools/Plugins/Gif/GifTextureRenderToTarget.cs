using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginTools.Gif
{
    /// <summary>
    /// Gif纹理熏染到目标
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    [DisallowMultipleComponent]
    public abstract class GifTextureRenderToTarget<T> : GifTextureUpdater where T : Component
    {
        /// <summary>
        /// 目标对象列表
        /// </summary>
        [Name("目标对象列表")]
        [Tip("Gif纹理渲染时的目标组件对象;默认为当前游戏对象的可支持Gif纹理渲染的组件对象；", "Target component object in GIF texture rendering; The default is the component object of the current game object that can support GIF texture rendering;")]
        public List<T> _targetObjects = new List<T>();

        /// <summary>
        /// 初始化
        /// </summary>
        protected void Awake()
        {
            _targetObjects.RemoveAll(obj => !obj);
            _targetObjects.Distinct();
        }

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            var targetObject = this.GetComponent<T>();
            if(targetObject)
            {
                _targetObjects.Add(targetObject);
            }
        }
    }
}
