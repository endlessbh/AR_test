using System.Text;
using UnityEditor;
using XCSJ.Attributes;
using XCSJ.EditorSMS.Inspectors;
using XCSJ.Maths;
using XCSJ.PluginSMS;
using XCSJ.PluginSMS.States.Dataflows.DataModel;

namespace XCSJ.EditorSMS.States.Dataflows.DataModel
{
    /// <summary>
    /// 数据恢复检查器
    /// </summary>
    [Name("数据恢复检查器")]
    [CustomEditor(typeof(DataRecovery), true)]
    public class DataRecoveryInspector : StateComponentInspector<DataRecovery>
    {
        /// <summary>
        /// 获取辅助信息
        /// </summary>
        /// <returns></returns>
        public override StringBuilder GetHelpInfo()
        {
            var info = base.GetHelpInfo();
            var dataRecovery = stateComponent;
            var dataRecorder = dataRecovery.dataRecorder;
            if (dataRecorder)
            {
                info.AppendFormat("数据记录器:\t{0}", dataRecorder.GetNamePath());

                var value = (int)(dataRecorder.dataRecordMode & dataRecovery.dataRecoveryMode);
                if (value == 0)
                {
                    info.AppendFormat("\n<color=red>数据恢复模式与数据记录器中数据记录模式没有匹配项</color>");
                }
                else if (value.BitCount() != 1)
                {
                    info.AppendFormat("\n<color=red>数据恢复模式仅可选择单一有效项</color>");
                }
            }
            else
            {
                info.AppendFormat("<color=red>数据记录器无效</color>");
            }
            return info;
        }
    }
}
