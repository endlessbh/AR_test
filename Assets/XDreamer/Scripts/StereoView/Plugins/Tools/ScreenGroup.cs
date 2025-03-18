using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginStereoView.Tools
{
    /// <summary>
    /// 
    /// </summary>
    [Name(Title)]
    [Tip("现实世界多个屏幕在虚拟世界中组合构建的虚拟屏幕组", "A virtual screen group constructed by combining multiple screens in the real world in the virtual world")]
    [DisallowMultipleComponent]
    public class ScreenGroup : BaseScreen
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "屏幕组";

        /// <summary>
        /// 屏幕尺寸：X为宽，Y为高,Z为厚度；单位：米；
        /// </summary>
        public override Vector3 screenSize { get => screenBounds.size; set { } }

        /// <summary>
        /// 屏幕包围盒
        /// </summary>
        public override Bounds screenBounds
        {
            get
            {
                var hasValue = false;
                Bounds bounds = default;
                foreach (var s in GetComponentsInChildren<BaseScreen>())
                {
                    if (s == this) continue;
                    if (hasValue)
                    {
                        bounds.Encapsulate(s.screenBounds);
                    }
                    else
                    {
                        hasValue = true;
                        bounds = s.screenBounds;
                    }
                }
                return bounds;
            }
        }

        /// <summary>
        /// 屏幕组下的所有子一级屏幕：包括激活与非激活的
        /// </summary>
        public BaseScreen[] screens => GetComponentsInChildren<BaseScreen>(true).Where(bs => bs.transform.parent == transform).ToArray();

        /// <summary>
        /// 选中时绘制Gizmos
        /// </summary>
        public void OnDrawGizmosSelected()
        {
            var bounds = screenBounds;
            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }
    }
}
