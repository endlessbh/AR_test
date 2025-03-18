
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginPhysicses.Tools.Weapons;
using XCSJ.PluginXGUI.Base;

namespace XCSJ.PluginPhysicses.UI
{
    /// <summary>
    /// 射击视图：关联射击器的属性界面视图
    /// </summary>
    public class ShooterView : View
    {
        /// <summary>
        /// 射击器
        /// </summary>
        [Name("射击器")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Shooter _shooter;

        /// <summary>
        /// 弹药容量文本
        /// </summary>
        [Name("弹药容量文本")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Text _ammoCapacityText;

        /// <summary>
        /// 剩余弹药文本
        /// </summary>
        [Name("剩余弹药文本")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Text _ammoRemainderText;

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            if (!_shooter || !_ammoCapacityText || !_ammoRemainderText)
            {
                enabled = false;
                return;
            }
        }
    }
}
