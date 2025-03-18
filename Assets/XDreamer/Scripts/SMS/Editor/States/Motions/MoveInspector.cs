using System.Text;
using UnityEditor;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.States.Motions;

namespace XCSJ.EditorSMS.States.Motions
{
    /// <summary>
    /// 移动检查器
    /// </summary>
    [Name("移动检查器")]
    [CustomEditor(typeof(Move))]
    public class MoveInspector : TransformMotionInspector<Move>
    {
        public override StringBuilder GetHelpInfo()
        {
            var info = base.GetHelpInfo();
            try
            {
                var pathLength = MathU.PathLength(workClip.offsetValues);
                var realFullPathLength = MathU.PathLength(workClip.GetFullMovePath());
                var standardMoveSpeed = realFullPathLength / workClip.timeLength;
                var moveSpeed = standardMoveSpeed * workClip.parent.speed;
                return info.AppendFormat("\n移动路径长度:\t{0}\n完整移动路径长度:\t{1}\n标准移动速度:\t{2}\n移动速度:\t\t{3}", pathLength, realFullPathLength, standardMoveSpeed, moveSpeed);
            }
            catch
            {
                return info;
            }
        }
    }
}
