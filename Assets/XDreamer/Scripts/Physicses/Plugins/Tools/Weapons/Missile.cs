using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginPhysicses.Tools.Destructions;
using XCSJ.PluginTools;

namespace XCSJ.PluginPhysicses.Tools.Weapons
{
    /// <summary>
    /// 发射物
    /// </summary>
    [Name("发射物")]
    [RequireComponent(typeof(Rigidbody))]
    [Tool(PhysicsManager.Title, rootType = typeof(PhysicsManager), index = InteractionCategory.InteractableIndex)]
    public class Missile : Breaker
    {
        /// <summary>
        /// 开火
        /// </summary>
        /// <param name="hitPower"></param>
        public virtual void OnFire(float hitPower)
        {
            ownRigidbody.AddForce(hitPower * transform.forward, ForceMode.Impulse);
        }
    }
}
