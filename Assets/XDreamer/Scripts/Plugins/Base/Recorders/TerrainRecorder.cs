using System.Collections.Generic;
using UnityEngine;

namespace XCSJ.Extension.Base.Recorders
{
    /// <summary>
    /// 地形记录器：记录了地形的树实例，地形细节和地面贴图层
    /// </summary>
    public class TerrainRecorder : Recorder<Terrain, TerrainRecorder.Info>
    {
        /// <summary>
        /// 信息
        /// </summary>
        public class Info : ISingleRecord<Terrain>
        {
            /// <summary>
            /// 地形对象
            /// </summary>
            public Terrain terrain;

            /// <summary>
            /// 树实例
            /// </summary>
            private TreeInstance[] orgTreeInstances;

            /// <summary>
            /// 地形贴图层
            /// </summary>
            private float[,,] orgAlphaMaps;

            /// <summary>
            /// 地形细节层
            /// </summary>
            private List<int[,]> orgDetails = new List<int[,]>();

            /// <summary>
            /// 记录
            /// </summary>
            /// <param name="terrain"></param>
            public void Record(Terrain terrain)
            {
                if (!terrain) return;

                this.terrain = terrain;
                var terrainData = terrain.terrainData;
                orgTreeInstances = (TreeInstance[])terrainData.treeInstances.Clone();
                orgAlphaMaps = terrainData.GetAlphamaps(0, 0, terrainData.alphamapWidth, terrainData.alphamapHeight);

                for (int i = 0; i < terrainData.detailPrototypes.Length; i++)
                {
                    orgDetails.Add(terrainData.GetDetailLayer(0, 0, terrainData.detailWidth, terrainData.detailHeight, i));
                }
            }

            /// <summary>
            /// 恢复
            /// </summary>
            public void Recover()
            {
                if (!terrain) return;

                var terrainData = terrain.terrainData;
                terrainData.treeInstances = orgTreeInstances;
                terrainData.SetAlphamaps(0, 0, orgAlphaMaps);

                for (int i = 0; i < terrainData.detailPrototypes.Length; i++)
                {
                    terrainData.SetDetailLayer(0, 0, i, orgDetails[i]);
                }
            }
        }
    }

    /// <summary>
    /// 地形树实例记录器
    /// </summary>
    public class TerrainTreeInstanceRecorder : Recorder<Terrain, TerrainTreeInstanceRecorder.Info>
    {
        /// <summary>
        /// 信息
        /// </summary>
        public class Info : ISingleRecord<Terrain>
        {
            /// <summary>
            /// 地形对象
            /// </summary>
            public Terrain terrain;

            /// <summary>
            /// 树实例
            /// </summary>
            private TreeInstance[] orgTreeInstances;

            /// <summary>
            /// 记录
            /// </summary>
            /// <param name="terrain"></param>
            public void Record(Terrain terrain)
            {
                if (!terrain) return;

                this.terrain = terrain;
                orgTreeInstances = (TreeInstance[])terrain.terrainData.treeInstances.Clone();

            }

            /// <summary>
            /// 恢复
            /// </summary>
            public void Recover()
            {
                if (!terrain) return;

                terrain.terrainData.treeInstances = orgTreeInstances;
            }
        }
    }

    /// <summary>
    /// 地形地面贴图层记录器
    /// </summary>
    public class TerrainAlphaMapRecorder : Recorder<Terrain, TerrainAlphaMapRecorder.Info>
    {
        /// <summary>
        /// 信息
        /// </summary>
        public class Info : ISingleRecord<Terrain>
        {
            /// <summary>
            /// 地形对象
            /// </summary>
            public Terrain terrain;

            /// <summary>
            /// 地形贴图层
            /// </summary>
            private float[,,] orgAlphaMaps;

            /// <summary>
            /// 记录
            /// </summary>
            /// <param name="terrain"></param>
            public void Record(Terrain terrain)
            {
                if (!terrain) return;

                this.terrain = terrain;
                var terrainData = terrain.terrainData;
                orgAlphaMaps = terrainData.GetAlphamaps(0, 0, terrainData.alphamapWidth, terrainData.alphamapHeight);
            }

            /// <summary>
            /// 恢复
            /// </summary>
            public void Recover()
            {
                if (!terrain) return;

                terrain.terrainData.SetAlphamaps(0, 0, orgAlphaMaps);
            }
        }
    }

    /// <summary>
    /// 地形细节记录器
    /// </summary>
    public class TerrainDetailRecorder : Recorder<Terrain, TerrainDetailRecorder.Info>
    {
        /// <summary>
        /// 信息
        /// </summary>
        public class Info : ISingleRecord<Terrain>
        {
            /// <summary>
            /// 地形对象
            /// </summary>
            public Terrain terrain;

            /// <summary>
            /// 地形细节层
            /// </summary>
            private List<int[,]> orgDetails = new List<int[,]>();

            /// <summary>
            /// 记录
            /// </summary>
            /// <param name="terrain"></param>
            public void Record(Terrain terrain)
            {
                if (!terrain) return;

                this.terrain = terrain;
                var terrainData = terrain.terrainData;
                
                for (int i = 0; i < terrainData.detailPrototypes.Length; i++)
                {
                    orgDetails.Add(terrainData.GetDetailLayer(0, 0, terrainData.detailWidth, terrainData.detailHeight, i));
                }
            }

            /// <summary>
            /// 恢复
            /// </summary>
            public void Recover()
            {
                if (!terrain) return;

                var terrainData = terrain.terrainData;
                
                for (int i = 0; i < terrainData.detailPrototypes.Length; i++)
                {
                    terrainData.SetDetailLayer(0, 0, i, orgDetails[i]);
                }
            }
        }
    }
}
