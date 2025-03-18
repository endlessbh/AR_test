using XCSJ.Attributes;

namespace XCSJ.PluginXXR.Interaction.Toolkit.Tools.LocomotionProviders
{
    /// <summary>
    /// 移动IO通过键码:通过键码模拟运动提供者移动的输入输出
    /// </summary>
    [Name("移动IO通过键码")]
    [Tip("通过键码模拟运动提供者移动的输入输出", "Simulate the input and output of the movement of the motion provider through the key code")]
    //[Tool(XRITHelper.MoveIO, nameof(AnalogLocomotionProvider))]
    [XCSJ.Attributes.Icon(EIcon.Keyboard)]
    public class MoveIOByKeyCode : BaseAnalogLocomotionProviderIO
    {
        /// <summary>
        /// 更新输入输出
        /// </summary>
        /// <param name="analogLocomotionProvider"></param>
        public override void UpdateIO(AnalogLocomotionProvider analogLocomotionProvider)
        {
        }
    }
}
