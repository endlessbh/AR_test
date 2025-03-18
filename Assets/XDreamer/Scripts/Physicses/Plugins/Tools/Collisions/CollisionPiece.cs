using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginTools.Effects;
using XCSJ.PluginTools.Items;

namespace XCSJ.PluginPhysicses.Tools.Collisions
{
    /// <summary>
    /// 碰撞粉碎
    /// </summary>
    [Name("碰撞粉碎")]
    [XCSJ.Attributes.Icon(EIcon.Mono)]
    [RequireManager(typeof(PhysicsManager))]
    public class CollisionPiece : BaseEffect
    {
        [Name("原型碎块")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public GameObject _prototypePieces;

        public GameObject _clonePieces { get; private set; }
        
        /// <summary>
        /// 启用特效
        /// </summary>
        /// <param name="visualData"></param>
        /// <param name="gameObject"></param>
        public override void EnableEffect(InteractData visualData, GameObject gameObject)
        {
            if (visualData.parent is ColliderInteractData colliderInteractData)
            {
                if (colliderInteractData.collision != null)
                {
                    Play(transform, colliderInteractData.collision.impulse.magnitude);
                }
            }
        }

        /// <summary>
        /// 禁用特效
        /// </summary>
        /// <param name="visualData"></param>
        /// <param name="gameObject"></param>
        public override void DisableEffect(InteractData visualData, GameObject gameObject)
        {

        }

        /// <summary>
        /// 碰撞进入
        /// </summary>
        /// <param name="collision"></param>
        private void Play(Transform transfrom, float force)
        {
            if (!_prototypePieces) return;

            if (!_clonePieces)
            {
                _clonePieces = UnityObjectHelper.XCloneObject(_prototypePieces.gameObject);
                _clonePieces.transform.position = transfrom.position;
                _clonePieces.transform.rotation = transfrom.rotation;
                _clonePieces.gameObject.SetActive(true);
            }

            foreach (var rigidBody in _clonePieces.GetComponentsInChildren<Rigidbody>())
            {
                var v = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f));
                rigidBody.AddForce(v * force, ForceMode.VelocityChange);
            }
        }
    }
}
