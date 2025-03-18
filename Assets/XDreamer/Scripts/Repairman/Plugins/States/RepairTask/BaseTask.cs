using XCSJ.PluginCommonUtils;
using XCSJ.PluginRepairman.Machines;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Show;

namespace XCSJ.PluginRepairman.States.RepairTask
{
    /// <summary>
    /// 抽象任务基类
    /// </summary>
    [RequireManager(typeof(RepairmanManager))]
    [Owner(typeof(RepairmanManager))]
    public abstract class BaseTask : Step, IBaseInfo
    {
        public string showName
        {
            get
            {
                return parent.name;
            }
            set
            {
                parent.name = value;
            }
        }

        string IBaseInfo.description
        {
            get
            {
                return description;
            }
            set
            {
                description = value;
            }
        }

        public override void OnEntry(StateData data)
        {
            base.OnEntry(data);

        }

        public override void OnExit(StateData data)
        {

            base.OnExit(data);
        }

        public abstract void Help();
    }
}
