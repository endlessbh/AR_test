using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginTools.Terrains;

namespace XCSJ.PluginPhysicses.Tools.Grounds
{
    /// <summary>
    /// 地面材质配置
    /// </summary>
    [Name("地面材质配置")]
    [Tool(PhysicsManager.Title, rootType = typeof(PhysicsManager))]
    [XCSJ.Attributes.Icon(EIcon.Material)]
    [DisallowMultipleComponent]
    [RequireManager(typeof(PhysicsManager))]
    public class GroundMaterialConfig : BasePhysicsMB
    {
        /// <summary>
        /// 地面材质摩擦力列表
        /// </summary>
        [Name("地面材质摩擦力列表")]
        public List<GroundMaterialFrictions> _groundMaterialFrictions = new List<GroundMaterialFrictions>();

        /// <summary>
        /// 地形材质列表
        /// </summary>
        [Name("地形材质列表")]
        public TerrainFrictions[] _terrainFrictions;

        /// <summary>
        /// 唤醒
        /// </summary>
        protected void Awake()
        {
            foreach (var gmf in _groundMaterialFrictions)
            {
                var ps = gmf.groundParticles.GetComponentsInChildren<ParticleSystem>();
                foreach (var p in ps)
                {
                    var emission = p.emission;
                    emission.enabled = false;
                }
            }
        }

        /// <summary>
        /// 获取当前地面材质摩擦力
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public GroundMaterialFrictions GetCurrentGroundMaterialFrictions(int index)
        {
            index = Mathf.Clamp(index, 0, _groundMaterialFrictions.Count - 1);
            return _groundMaterialFrictions[index];
        }

        /// <summary>
        /// 获取地面材质索引
        /// </summary>
        /// <returns>索引</returns>
        public int GetGroundMaterialIndex(WheelHit wheelHit, Vector3 wheelPosition)
        {
            int ret = -1;

            for (int i = 0; i < _groundMaterialFrictions.Count; i++)
            {
                if (wheelHit.collider.sharedMaterial == _groundMaterialFrictions[i].groundMaterial)
                {
                    ret = i;
                }
            }

            // 在地面材质中没找到，则使用地形数据
            if (ret < 0)
            {
                var tf = _terrainFrictions;
                for (int i = 0; i < tf.Length; i++)
                {
                    if (wheelHit.collider.sharedMaterial == tf[i].groundMaterial)
                    {
                        var index = TerrainHelper.GetIndex(wheelPosition);
                        if (index >= 0)
                        {
                            ret = tf[i].splatmapIndexes[index].index;
                        }
                    }
                }
            }
            return Mathf.Clamp(ret, 0, int.MaxValue);
        }
    }

    /// <summary>
    /// 地面材质
    /// </summary>
    [System.Serializable]
    public class GroundMaterialFrictions
    {
        /// <summary>
        /// 地面材质
        /// </summary>
        [Name("地面材质")]
        public PhysicMaterial groundMaterial;

        /// <summary>
        /// 前向刚度
        /// </summary>
        [Name("前向刚度")]
        public float forwardStiffness = 1f;

        /// <summary>
        /// 侧向刚度
        /// </summary>
        [Name("侧向刚度")]
        public float sidewaysStiffness = 1f;

        /// <summary>
        /// 滑动值
        /// </summary>
        [Name("滑动值")]
        public float slip = .25f;

        /// <summary>
        /// 阻尼
        /// </summary>
        [Name("阻尼")]
        public float damp = 1f;

        /// <summary>
        /// 音频
        /// </summary>
        [Name("音频")]
        public AudioClip groundSound;

        /// <summary>
        /// 音量
        /// </summary>
        [Name("音量")]
        [Range(0f, 1f)]
        public float volume = 1f;

        /// <summary>
        /// 划痕
        /// </summary>
        [Name("划痕")]
        public Skidmark skidmark;

        /// <summary>
        /// 地面粒子
        /// </summary>
        [Name("地面粒子")]
        public GameObject groundParticles;
    }

    /// <summary>
    /// 地形摩擦力
    /// </summary>
    [System.Serializable]
    [Name("地形摩擦力")]
    public class TerrainFrictions
    {
        /// <summary>
        /// 地面物理材质
        /// </summary>
        public PhysicMaterial groundMaterial;

        /// <summary>
        /// 泼溅贴图索引
        /// </summary>
        [Name("泼溅贴图索引")]
        [System.Serializable]
        public class SplatmapIndexes
        {
            public int index = 0;
            public PhysicMaterial groundMaterial;
        }

        public SplatmapIndexes[] splatmapIndexes;
    }
}
