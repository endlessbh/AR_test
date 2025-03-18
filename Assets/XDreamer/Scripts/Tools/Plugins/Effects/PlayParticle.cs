using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;

namespace XCSJ.PluginTools.Effects
{
    /// <summary>
    /// 播放粒子
    /// </summary>
    [Name("播放粒子")]
    [XCSJ.Attributes.Icon(EIcon.Mono)]
    public class PlayParticle : BaseEffect
    {
        /// <summary>
        /// 粒子模版
        /// </summary>
        [Name("粒子模版")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public ParticleSystem _particleTmplate = null;

        /// <summary>
        /// 最大粒子数
        /// </summary>
        [Name("最大粒子数")]
        [Min(1)]
        public int _maxParticleCount = 5;

        private List<ParticleSystem> cloneParticles = new List<ParticleSystem>();

        private const string ParticleRootName = "碰撞粒子组";

        private void Create(Transform transform)
        {
            if (!_particleTmplate) return;
            if (cloneParticles.Count > 0) return;

            var particleRoot = UnityObjectHelper.CreateGameObject(ParticleRootName);
            particleRoot.XSetParent(transform);

            for (int i = 0; i < _maxParticleCount; i++)
            {
                var go = UnityObjectHelper.XCloneObject(_particleTmplate.gameObject);
                go.XSetParent(particleRoot.transform);
                go.SetActive(true);
                var ps = go.GetComponent<ParticleSystem>();
                var em = ps.emission;
                em.enabled = false;

                cloneParticles.Add(ps);
            }
        }

        private void Play(Vector3 position, Quaternion? quaternion = null)
        {
            foreach (var ps in cloneParticles)
            {
                if (ps.isPlaying) continue;

                ps.transform.position = position;
                if (quaternion.HasValue) ps.transform.rotation = quaternion.Value;
                var em = ps.emission;
                em.enabled = true;
                ps.Play();
            }
        }

        private void Play(Transform parent, Vector3 position, Quaternion? quaternion = null)
        {
            Create(parent);

            Play(position, quaternion);
        }

        /// <summary>
        /// 启用特效
        /// </summary>
        /// <param name="interactData"></param>
        /// <param name="gameObject"></param>
        public override void EnableEffect(InteractData interactData, GameObject gameObject)
        {
            if (gameObject && interactData.TryGetPosition(out var position, true))
            {
                Play(gameObject.transform, position);
            }
        }

        /// <summary>
        /// 禁用特效
        /// </summary>
        /// <param name="interactData"></param>
        /// <param name="gameObject"></param>
        public override void DisableEffect(InteractData interactData, GameObject gameObject)
        {
        }
    }
}
