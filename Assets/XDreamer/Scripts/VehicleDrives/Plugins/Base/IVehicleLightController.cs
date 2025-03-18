using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCSJ.Attributes;

namespace XCSJ.PluginVehicleDrive.Base
{
    /// <summary>
    /// 车灯接口
    /// </summary>
    public interface IVehicleLightController
    {
        /// <summary>
        /// 近光灯
        /// </summary>
        bool lowBeamHeadLightsOn { get; set; }

        /// <summary>
        /// 远光灯
        /// </summary>
        bool highBeamHeadLightsOn { get; set; }

        /// <summary>
        /// 雾灯
        /// </summary>
        bool fogLightOn { get; set; }

        /// <summary>
        /// 刹车灯
        /// </summary>
        bool brakeLightOn { get; }

        /// <summary>
        /// 倒车灯
        /// </summary>
        bool reverseLightOn { get; }

        /// <summary>
        /// 指示灯
        /// </summary>
        EIndicatorLight indicatorLight { get; set; }

        /// <summary>
        /// 获取灯光开关
        /// </summary>
        /// <param name="lightType"></param>
        bool GetLightState(ELightType lightType);

        /// <summary>
        /// 设置灯光开关
        /// </summary>
        /// <param name="lightType"></param>
        /// <param name="isOn"></param>
        void SetLightState(ELightType lightType, bool isOn);

        /// <summary>
        /// 切换灯光开关
        /// </summary>
        /// <param name="lightType"></param>
        void SwitchLightState(ELightType lightType);
    }

    /// <summary>
    /// 灯光类型
    /// </summary>
    [Name("灯光类型")]
    public enum ELightType
    {
        /// <summary>
        /// 近光
        /// </summary>
        [Name("近光")]
        LowBeamHead,//, 50f, 90f

        /// <summary>
        /// 远光
        /// </summary>
        [Name("远光")]
        HighBeamHead,//, 200f, 45f

        /// <summary>
        /// 刹车
        /// </summary>
        [Name("刹车")]
        Brake,

        /// <summary>
        /// 倒车
        /// </summary>
        [Name("倒车")]
        Reverse,

        /// <summary>
        /// 左转
        /// </summary>
        [Name("左转")]
        LeftIndicator,

        /// <summary>
        /// 右转
        /// </summary>
        [Name("右转")]
        RightIndicator,

        /// <summary>
        /// 雾灯
        /// </summary>
        [Name("雾灯")]
        Fog,

        /// <summary>
        /// 双闪
        /// </summary>
        [Name("双闪")]
        AllIndicator,
    };

    /// <summary>
    /// 指示灯状态
    /// </summary>
    [Name("指示灯状态")]
    public enum EIndicatorLight
    {
        /// <summary>
        /// 关闭
        /// </summary>
        Off,

        /// <summary>
        /// 右转
        /// </summary>
        Right,

        /// <summary>
        /// 左转
        /// </summary>
        Left,

        /// <summary>
        /// 双闪
        /// </summary>
        All
    }

}
