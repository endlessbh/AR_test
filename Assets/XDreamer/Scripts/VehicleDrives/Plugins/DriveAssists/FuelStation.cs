using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginVehicleDrive.Base;
using XCSJ.PluginVehicleDrive.Controllers;

namespace XCSJ.PluginVehicleDrive.DriveAssists
{
    /// <summary>
    /// 加油站：当车辆驶入组件所在的触发型碰撞器时，可自动给车辆增加燃料
    /// </summary>
    [Name("加油站")]
    [Tip("当车辆驶入组件所在的触发型碰撞器时，可自动给车辆增加燃料", "When the vehicle drives into the triggered impactor where the assembly is located, it can automatically add fuel to the vehicle")]
    [Tool(VehicleDriveHelper.CategoryComponentName, rootType = typeof(VehicleDriveManger))]
    [RequireManager(typeof(VehicleDriveManger))]
    public class FuelStation : BaseVehicle
    {
        /// <summary>
        /// 加油速度
        /// </summary>
        [Name("每秒钟加油量")]
        [Tip("每秒钟加油量", "Refueling volume per second")]
        [Min(0)]
        public float _fillSpeed = 1f;

        /// <summary>
        /// 启动加油的间隔时间
        /// </summary>
        [Name("启动加油的间隔时间")]
        [Tip("等待当前时间量后开始加油", "Start refueling after waiting for the current amount of time")]
        [Min(0)]
        public float _delayTimeToBeginFillFuel = 3f;

        /// <summary>
        /// 延迟加油对象
        /// </summary>
        private class DelayFillFuel
        {
            public float timeCounter = 0;

            public IFuel enterFuel = null;

            public DelayFillFuel(IFuel enterFuel)
            {
                this.enterFuel = enterFuel;
            }
        }

        private List<DelayFillFuel> delayFillFuels = new List<DelayFillFuel>();

        private List<IFuel> fillFuels = new List<IFuel>();

        /// <summary>
        /// 更新
        /// </summary>
        protected void Update()
        {
            // 延迟加油
            for (int i = delayFillFuels.Count-1; i >=0; i--)
            {
                var df = delayFillFuels[i];
                df.timeCounter += Time.deltaTime;
                if (df.timeCounter > _delayTimeToBeginFillFuel)
                {
                    fillFuels.Add(df.enterFuel);
                    delayFillFuels.RemoveAt(i);
                }
            }

            // 加油中……
            foreach (var f in fillFuels)
            {
                f.Add(_fillSpeed * Time.deltaTime);
            }
        }

        /// <summary>
        /// 碰撞进入
        /// </summary>
        /// <param name="collider">与当前对象发生碰撞的对象</param>
        protected void OnTriggerEnter(Collider collider)
        {
            var fuel = FindFuel(collider.gameObject);
            if (fuel!=null)
            {
                delayFillFuels.Add(new DelayFillFuel(fuel));
            }
        }

        /// <summary>
        /// 碰撞退出
        /// </summary>
        /// <param name="collider">与当前对象发生碰撞的对象</param>
        protected virtual void OnTriggerExit(Collider collider)
        {
            var fuel = FindFuel(collider.gameObject);
            if (fuel!=null)
            {
                delayFillFuels.RemoveAll(item => item.enterFuel == fuel);
                fillFuels.Remove(fuel);
            }
        }

        private IFuel FindFuel(GameObject go)
        {
            var vc = go.GetComponentInChildren<VehicleDriver>();
            if (!vc)
            {
                vc = go.GetComponentInParent<VehicleDriver>();
            }
            return vc ? vc.fuel : null;
        }
    }
}
