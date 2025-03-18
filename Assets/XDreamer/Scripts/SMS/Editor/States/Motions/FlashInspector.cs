using System.Text;
using XCSJ.PluginSMS.States.Motions;

namespace XCSJ.EditorSMS.States.Motions
{
    public class FlashInspector<T> : MotionInspector<T> where T : Motion<T>, IFlash
    {
        public override StringBuilder GetHelpInfo()
        {
            var info = base.GetHelpInfo();
            info.AppendFormat("\n闪烁次数:\t{0}", stateComponent.flashCount);
            return info;
        }
    }
}
