using System;
using UnityEngine.UI;
using XCSJ.Algorithms;
using XCSJ.Helper;

namespace XCSJ.Extension.Base.XUnityEngine.XUI
{
    [LinkType(typeof(Slider))]
    public class Slider_LinkType : Selectable_LinkType<Slider_LinkType>
    {
        public Slider_LinkType(Slider obj) : base(obj) { }
        public Slider_LinkType(object obj) : base(obj) { }
        protected Slider_LinkType() { }

        #region UpdateVisuals

        public static XMethodInfo UpdateVisuals_MethodInfo { get; } = new XMethodInfo(Type, nameof(UpdateVisuals), TypeHelper.InstanceNotPublic);

        public void UpdateVisuals() => UpdateVisuals_MethodInfo.Invoke(obj, null);

        #endregion

        #region Set

        public static XMethodInfo Set_Float_MethodInfo { get; } = new XMethodInfo(Type, nameof(Set), new Type[] { typeof(float) }, TypeHelper.InstanceNotPublic);

        public void Set(float input) => Set_Float_MethodInfo.Invoke(obj, new object[] { input });

        public static XMethodInfo Set_Float_Bool_MethodInfo { get; } = new XMethodInfo(Type, nameof(Set), new Type[] { typeof(float), typeof(bool) }, TypeHelper.InstanceNotPublic);

        public void Set(float input, bool sendCallback) => Set_Float_Bool_MethodInfo.Invoke(obj, new object[] { input, sendCallback });

        #endregion
    }
}
