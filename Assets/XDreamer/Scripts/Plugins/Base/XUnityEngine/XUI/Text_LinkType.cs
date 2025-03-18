using UnityEngine.UI;
using XCSJ.Algorithms;

namespace XCSJ.Extension.Base.XUnityEngine.XUI
{
    [LinkType(typeof(Text))]
    public class Text_LinkType : MaskableGraphic_LinkType<Text_LinkType>
    {
        public Text text => obj as Text;

        public Text_LinkType(Text obj) : base(obj) { }
        public Text_LinkType(object obj) : base(obj) { }
        protected Text_LinkType() { }
    }
}
