using System.Collections.Generic;
using System.Linq;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginMechanicalMotion.Tools;
using XCSJ.PluginXGUI.ViewControllers;

namespace XCSJ.PluginMechanicalMotion.UI
{
    /// <summary>
    /// 平面机构视图控制器
    /// </summary>
    [Name("平面机构视图控制器")]
    [XCSJ.Attributes.Icon(nameof(MoveMechanism))]
    //[Tool(MechanicalMotionManager.Title, rootType = typeof(MechanicalMotionManager))]
    [RequireManager(typeof(MechanicalMotionManager))]
    public sealed class PlaneMechanismViewController : BaseViewController
    {
        /// <summary>
        /// 平面机构
        /// </summary>
        [Name("平面机构")]
        public List<PlaneMechanism> _planeMechanisms = new List<PlaneMechanism>();

        /// <summary>
        /// 速度属性
        /// </summary>
        public double velocity
        {
            get
            {
                var pm = _planeMechanisms.Find(m => m);
                if (pm)
                {
                    return pm.velocity;
                }
                return 0;
            }
            set
            {
                foreach (var item in _planeMechanisms)
                {
                    if(item) item.velocity = value;
                }
            }
        }

        /// <summary>
        /// 设置速度量为0
        /// </summary>
        public void SetVelocityZero() => velocity = 0;

        /// <summary>
        /// 重置平面机构初始数据
        /// </summary>
        public void ResetInitValue()
        {
            foreach (var item in _planeMechanisms)
            {
                item.ResetInit();
            }
        }
    }
}
