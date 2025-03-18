using System;
using System.Linq;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginTools.Base
{
    /// <summary>
    /// 射线碰撞器
    /// </summary>
    [Serializable]
    [Name("射线碰撞器")]
    public class RayHitter : RayGenerater
    {
        /// <summary>
        /// 射线最大距离
        /// </summary>
        [Name("射线最大距离")]
        public float _maxDistance = 1000;

        /// <summary>
        /// 射线层
        /// </summary>
        [Name("射线层")]
        public LayerMask _layerMask = Physics.DefaultRaycastLayers;

        /// <summary>
        /// 触发器拾取规则
        /// </summary>
        [Name("触发器拾取规则")]
        public QueryTriggerInteraction _queryTriggerInteraction = QueryTriggerInteraction.UseGlobal;

        /// <summary>
        /// 射线产生碰撞，获取碰撞体的刚体
        /// </summary>
        /// <returns></returns>
        public Rigidbody PickRigidbody() => TryGetHit(out _, out var hit) ? hit.rigidbody : null;

        /// <summary>
        /// 获取射线碰撞点
        /// </summary>
        /// <param name="raycastHit"></param>
        /// <returns></returns>
        public bool TryGetHit(out RaycastHit raycastHit) => TryGetHit(out _, out raycastHit);

        /// <summary>
        /// 获取射线碰撞点
        /// </summary>
        /// <param name="ray"></param>
        /// <param name="raycastHit"></param>
        /// <returns></returns>
        public bool TryGetHit(out Ray ray, out RaycastHit raycastHit)
        {
            if (TryGetRay(out ray))
            {
                return TryGetHit(ray, out raycastHit);
            }

            ray = default;
            raycastHit = default;
            return false;
        }

        /// <summary>
        /// 获取射线碰撞点
        /// </summary>
        /// <param name="ray"></param>
        /// <param name="raycastHit"></param>
        /// <returns></returns>
        public bool TryGetHit(Ray ray, out RaycastHit raycastHit)
        {
            if (Physics.Raycast(ray, out raycastHit, _maxDistance, _layerMask, _queryTriggerInteraction))
            {
                return true;
            }

            raycastHit = default;
            return false;
        }

        public bool IsHit() => TryGetHit(out _, out _);

        /// <summary>
        /// 获取射线所有碰撞点信息，返回点击对象为升序
        /// </summary>
        /// <returns></returns>
        public RaycastHit[] GetHitAll(out Ray ray)
        {
            if (TryGetRay(out ray))
            {
                GetHitAll(ray, _maxDistance, _layerMask);
            }
            return Empty<RaycastHit>.Array;
        }

        /// <summary>
        /// 获取所有碰撞点
        /// </summary>
        /// <param name="ray"></param>
        /// <param name="maxDistance"></param>
        /// <param name="layerMask"></param>
        /// <returns></returns>
        public static RaycastHit[] GetHitAll(Ray ray, float maxDistance, LayerMask layerMask)
        {
            var hits = Physics.RaycastAll(ray, maxDistance, layerMask);
            if (hits.Length > 0)
            {
                return hits.OrderBy(h => h.distance).ToArray();// 升序排列
            }
            return Empty<RaycastHit>.Array;
        }
    }
}
