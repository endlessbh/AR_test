using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginVehicleDrive.Base
{
    /// <summary>
    /// 车辆组件基类
    /// </summary>
    [RequireManager(typeof(VehicleDriveManger))]
    [Owner(typeof(VehicleDriveManger))]
    public abstract class BaseVehicle : InteractProvider
    {
    }
}
