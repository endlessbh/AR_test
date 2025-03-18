using XCSJ.EditorSMS.States.Base;
using XCSJ.PluginSMS.States.Motions;

namespace XCSJ.EditorSMS.States.Motions
{
    //[CustomEditor(typeof(Motion<>), true)]
    public class MotionInspector<T> : WorkClipInspector<T> where T : Motion<T>
    {

    }
}
