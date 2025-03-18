using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginPhysicses.Tools.Weapons
{
    /// <summary>
    /// 发射物容器
    /// </summary>
    [Name("发射物容器")]
    public class MissileContainer : InteractableVirtual
    {
        /// <summary>
        /// 发射物原型
        /// </summary>
        [Name("发射物原型")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Missile _prototypeMissile;

        /// <summary>
        /// 发射物容量
        /// </summary>
        [Name("发射物容量")]
        [Min(0)]
        public int _capacity = 10;

        /// <summary>
        /// 发射物剩余数量
        /// </summary>
        [Name("发射物剩余数量")]
        [Min(0)]
        [Readonly(EEditorMode.Runtime)]
        public int _remainder = 10;

        /// <summary>
        /// 当前发射物数量
        /// </summary>
        public int currentCount
        {
            get => _remainder;
            set => _remainder = value;
        }

        /// <summary>
        /// 获取发射物
        /// </summary>
        /// <returns></returns>
        public Missile GetMissile()
        {
            if (currentCount > 0 && _prototypeMissile)
            {
                --currentCount;

                return PhysicsManager.instance.Clone<Missile>(_prototypeMissile);
            }
            return null;
        }
    }
}
