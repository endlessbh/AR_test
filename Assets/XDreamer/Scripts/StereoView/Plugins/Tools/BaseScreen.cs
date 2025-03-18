using UnityEngine;
using System.Collections;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.Extension.Base.Dataflows.Base;
using System;
using XCSJ.PluginTools.Renderers;
using XCSJ.PluginTools;
using XCSJ.Maths;
using XCSJ.Extension.Interactions.Tools;

namespace XCSJ.PluginStereoView.Tools
{
    /// <summary>
    /// 基础屏幕
    /// </summary>
    [DisallowMultipleComponent]
    [RequireManager(typeof(StereoViewManager))]
    public abstract class BaseScreen : InteractProvider, IGizmoRendererValue
    {
        /// <summary>
        /// 当屏幕变化时回调：包括位置屏幕的位置、
        /// </summary>
        public static event Action<BaseScreen> onScreenChanged;

        /// <summary>
        /// 放置递归死循环的标记量
        /// </summary>
        private bool inCallback = false;

        /// <summary>
        /// 调用屏幕变化
        /// </summary>
        public void CallScreenChanged()
        {
            if (inCallback) return;
            try
            {
                //Debug.Log("回调: " + name);
                inCallback = true;
                onScreenChanged?.Invoke(this);
            }
            finally { inCallback = false; }
        }

        /// <summary>
        /// 屏幕尺寸：X为宽，Y为高,Z为厚度；单位：米；
        /// </summary>
        public abstract Vector3 screenSize { get; set; }

        /// <summary>
        /// 获取锚点的本地坐标
        /// </summary>
        /// <param name="screenAnchor"></param>
        /// <param name="anchorOffset"></param>
        /// <returns></returns>
        public virtual Vector3 GetAnchorLocalPosition(ERectAnchor screenAnchor) => screenAnchor.GetAnchorOffset(screenSize);

        /// <summary>
        /// 获取锚点位置，世界坐标系
        /// </summary>
        /// <param name="screenAnchor"></param>
        /// <param name="anchorOffset">锚点的偏移量;本值在做本地空间类型计算时仅考虑旋转;</param>
        /// <param name="spaceType">锚点的偏移量的空间类型</param>
        /// <returns></returns>
        public virtual Vector3 GetAnchorPosition(ERectAnchor screenAnchor, Vector3 anchorOffset = default, ESpaceType spaceType = ESpaceType.Local)
        {
            var transform = this.transform;

            if (spaceType == ESpaceType.Local)
            {
                var offset = Matrix4x4.TRS(Vector3.zero, transform.rotation, Vector3.one).MultiplyPoint(anchorOffset);
                return transform.TransformPoint(GetAnchorLocalPosition(screenAnchor)) + offset;
            }
            return transform.TransformPoint(GetAnchorLocalPosition(screenAnchor)) + anchorOffset;
        }

        /// <summary>
        /// 屏幕包围盒
        /// </summary>
        public abstract Bounds screenBounds { get; }

        Vector3 IGizmoRendererValue.value => screenSize;

        /// <summary>
        /// 界面数据变化时调用
        /// </summary>
        public virtual void OnValidate() => CallScreenChanged();
    }
}
