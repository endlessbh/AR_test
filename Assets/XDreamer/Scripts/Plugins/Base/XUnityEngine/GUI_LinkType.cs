using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Helper;

namespace XCSJ.Extension.Base.XUnityEngine
{
    [LinkType(typeof(GUI))]
    public class GUI_LinkType : LinkType<GUI_LinkType>
    {
        #region tooltipRect

        public static XPropertyInfo tooltipRect_PropertyInfo { get; } = new XPropertyInfo(Type, nameof(tooltipRect), TypeHelper.StaticNotPublic);

        public static Rect tooltipRect
        {
            get => (Rect)tooltipRect_PropertyInfo.GetValue(null);
            set => tooltipRect_PropertyInfo.SetValue(null, value);
        }

        #endregion
    }
}
