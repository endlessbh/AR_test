using UnityEngine.UI;
using XCSJ.Algorithms;

namespace XCSJ.Extension.Base.XUnityEngine.XUI
{
    public class MaskableGraphic_LinkType<T> : Graphic_LinkType<T>
         where T : MaskableGraphic_LinkType<T>
    {
        public MaskableGraphic_LinkType(MaskableGraphic obj) : base(obj) { }
        public MaskableGraphic_LinkType(object obj) : base(obj) { }
        protected MaskableGraphic_LinkType() { }
    }

    [LinkType(typeof(MaskableGraphic))]
    public class MaskableGraphic_LinkType : MaskableGraphic_LinkType<MaskableGraphic_LinkType>
    {
        public MaskableGraphic_LinkType(MaskableGraphic obj) : base(obj) { }
        public MaskableGraphic_LinkType(object obj) : base(obj) { }
        protected MaskableGraphic_LinkType() { }
    }
}
