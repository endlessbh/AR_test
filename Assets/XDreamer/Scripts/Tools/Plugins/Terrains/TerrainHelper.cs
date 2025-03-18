using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Maths;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginTools.Terrains
{
    /// <summary>
    /// 地形通用函数操作
    /// </summary>
    public static class TerrainHelper
    {
        /// <summary>
        /// 忽略地形旋转，获取世界坐标点在地形上有效投影点的高度
        /// </summary>
        /// <param name="terrain"></param>
        /// <param name="worldPoint"></param>
        /// <param name="worldTerrainHeight"></param>
        /// <returns></returns>
        public static bool TryGetTerrainHeightIgnoreRotation(this Terrain terrain, Vector3 worldPoint, out float worldTerrainHeight)
        {
            if (!terrain)
            {
                worldTerrainHeight = default;
                return false;
            }

            var pointInTerrain = worldPoint - terrain.GetPosition();
            var size = terrain.terrainData.size;
            worldTerrainHeight = terrain.terrainData.GetInterpolatedHeight(Mathf.Clamp01(pointInTerrain.x / size.x), Mathf.Clamp01(pointInTerrain.z / size.z));
            return true;
        }

        /// <summary>
        /// 忽略地形旋转，获取世界坐标点在地形上有效投影点的坐标
        /// </summary>
        /// <param name="terrain"></param>
        /// <param name="worldPoint"></param>
        /// <param name="worldTerrainPoint"></param>
        /// <returns></returns>
        public static bool TryGetTerrainPositionIgnoreRotation(this Terrain terrain, Vector3 worldPoint, out Vector3 worldTerrainPoint)
        {
            if (TryGetTerrainHeightIgnoreRotation(terrain, worldPoint, out var h))
            {
                worldTerrainPoint = new Vector3(worldPoint.x, h, worldPoint.z);
                return true;
            }
            worldTerrainPoint = default;
            return false;
        }

        /// <summary>
        /// 添加新的地形层
        /// </summary>
        /// <param name="terrain"></param>
        /// <param name="inputLayer"></param>
        public static bool AddTerrainLayer(this Terrain terrain, TerrainLayer inputLayer)
        {
            if (!terrain || inputLayer == null) return false;

            var terrainData = terrain.terrainData;
            var layers = terrainData.terrainLayers;
            for (var idx = 0; idx < layers.Length; ++idx)
            {
                if (layers[idx] == inputLayer) return false;
            }

            int newIndex = layers.Length;
            var newarray = new TerrainLayer[newIndex + 1];
            Array.Copy(layers, 0, newarray, 0, newIndex);
            newarray[newIndex] = inputLayer;
            terrainData.terrainLayers = newarray;

            return true;
        }

        /// <summary>
        /// 移除地形层
        /// </summary>
        /// <param name="terrain"></param>
        /// <param name="index"></param>
        public static bool RemoveTerrainLayer(this Terrain terrain, int index)
        {
            var terrainData = terrain.terrainData;
            int width = terrainData.alphamapWidth;
            int height = terrainData.alphamapHeight;
            float[,,] alphamap = terrainData.GetAlphamaps(0, 0, width, height);
            int alphaCount = alphamap.GetLength(2);

            int newAlphaCount = alphaCount - 1;
            float[,,] newalphamap = new float[height, width, newAlphaCount];

            // 移动层
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    for (int a = 0; a < index; ++a)
                    {
                        newalphamap[y, x, a] = alphamap[y, x, a];
                    }
                    for (int a = index + 1; a < alphaCount; ++a)
                    {
                        newalphamap[y, x, a - 1] = alphamap[y, x, a];
                    }
                }
            }

            // 重置权值
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    float sum = 0.0F;
                    for (int a = 0; a < newAlphaCount; ++a)
                        sum += newalphamap[y, x, a];
                    if (sum >= 0.01)
                    {
                        float multiplier = 1.0F / sum;
                        for (int a = 0; a < newAlphaCount; ++a)
                            newalphamap[y, x, a] *= multiplier;
                    }
                    else
                    {
                        // in case all weights sum to pretty much zero (e.g.
                        // removing splat that had 100% weight), assign
                        // everything to 1st splat texture (just like
                        // initial terrain).
                        for (int a = 0; a < newAlphaCount; ++a)
                            newalphamap[y, x, a] = (a == 0) ? 1.0f : 0.0f;
                    }
                }
            }

            // 移除地形层
            var layers = terrainData.terrainLayers;
            var newLayers = new TerrainLayer[layers.Length - 1];
            for (int a = 0; a < index; ++a)
            {
                newLayers[a] = layers[a];
            }
            for (int a = index + 1; a < alphaCount; ++a)
            {
                newLayers[a - 1] = layers[a];
            }
            terrainData.terrainLayers = newLayers;

            terrainData.SetAlphamaps(0, 0, newalphamap);

            return true;
        }

        /// <summary>
        /// 替换地形层
        /// </summary>
        /// <param name="terrain"></param>
        /// <param name="index"></param>
        /// <param name="inputLayer"></param>
        public static bool ReplaceTerrainLayer(Terrain terrain, int index, TerrainLayer inputLayer)
        {
            if (!terrain) return false;

            if (inputLayer == null)
            {
                RemoveTerrainLayer(terrain, index);
                return true;
            }

            var layers = terrain.terrainData.terrainLayers;
            // 层级有效
            if (index < 0 || index > layers.Length) return false;

            // 已经使用
            for (var idx = 0; idx < layers.Length; ++idx)
            {
                if (layers[idx] == inputLayer) return false;
            }

            layers[index] = inputLayer;
            terrain.terrainData.terrainLayers = layers;
            return true;
        }

        #region 设置地形层透明度

        /// <summary>
        /// 地形区域形状
        /// </summary>
        [Name("地形区域形状")]
        public enum EAreaShape
        {
            [Name("矩形")]
            Rect,

            [Name("圆形")]
            Circle,
        }

        /// <summary>
        /// 设置地形层透明度:通过求解游戏对象的包围盒转换为对应地形区域范围进行设置
        /// </summary>
        /// <param name="terrain"></param>
        /// <param name="layerIndex">地形层索引</param>
        /// <param name="alpha">透明度值（0，1）</param>
        /// <param name="gameObject"></param>
        /// <param name="areaShape"></param>
        public static void SetLayerAlpha(this Terrain terrain, int layerIndex, float alpha, GameObject gameObject, EAreaShape areaShape = EAreaShape.Rect)
        {
            if (terrain.TryCreateAlphaArray(layerIndex, alpha, out float[] alphas))
            {
                SetLayerAlphas(terrain, alphas, gameObject, areaShape);
            }
        }

        /// <summary>
        /// 设置地形层透明度:通过求解游戏对象的包围盒转换为对应地形区域范围进行设置
        /// </summary>
        /// <param name="terrain"></param>
        /// <param name="layerIndex">地形层索引</param>
        /// <param name="alphaValue">透明度值（0，1）</param>
        /// <param name="gameObject"></param>
        /// <param name="areaShape"></param>
        public static void SetLayerAlpha(this Terrain terrain, int layerIndex, float alpha, RectInt area, EAreaShape areaShape = EAreaShape.Rect)
        {
            if (terrain.TryCreateAlphaArray(layerIndex, alpha, out float[] alphas))
            {
                SetLayerAlphas(terrain, new List<RectAlphaLayerInfo>() { new RectAlphaLayerInfo(area, alphas) }, areaShape);
            }
        }

        private static bool TryCreateAlphaArray(this Terrain terrain, int layerIndex, float alpha, out float[] alphaArray)
        {
            int layerCount = terrain.terrainData.terrainLayers.Length;
            alphaArray = new float[layerCount];
            if (layerIndex < layerCount)
            {
                for (int i = 0; i < layerCount; i++)
                {
                    alphaArray[i] = (i == layerIndex) ? alpha : -1;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// 设置地形层透明度:通过求解游戏对象的包围盒转换为对应地形区域范围进行设置
        /// </summary>
        /// <param name="terrain"></param>
        /// <param name="alphas">透明度数组：当透明度小于0时，表示不改变对应层的透明度</param>
        /// <param name="gameObject"></param>
        /// <param name="areaShape"></param>
        public static void SetLayerAlphas(this Terrain terrain, float[] alphas, GameObject gameObject, EAreaShape areaShape = EAreaShape.Rect)
        {
            if (CommonFun.GetBounds(out Bounds bounds, gameObject, true, false, false))
            {
                SetLayerAlphas(terrain, alphas, bounds, areaShape);
            }
        }

        /// <summary>
        /// 设置地形层透明度:通过包围盒转换为对应地形区域范围进行设置
        /// </summary>
        /// <param name="terrain"></param>
        /// <param name="alphas">透明度数组：当透明度小于0时，表示不改变对应层的透明度，只使用透明度数组中索引小于地形层数的数值</param>
        /// <param name="bounds"></param>
        /// <param name="areaShape"></param>
        public static void SetLayerAlphas(this Terrain terrain, float[] alphas, Bounds bounds, EAreaShape areaShape = EAreaShape.Rect)
        {
            SetLayerAlphas(terrain, new List<RectAlphaLayerInfo>() { new RectAlphaLayerInfo(BoundsToTerrainAlphamapRect(terrain, bounds), alphas) }, areaShape);
        }

        /// <summary>
        /// 地形层区域透明度信息
        /// </summary>
        public class RectAlphaLayerInfo
        {
            /// <summary>
            /// 包围盒
            /// </summary>
            public RectInt rect;

            /// <summary>
            /// 透明度数组
            /// </summary>
            public float[] alphas;

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="rect"></param>
            public RectAlphaLayerInfo(RectInt rect)
            {
                this.rect = rect;
                Compute();
            }

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="rect"></param>
            /// <param name="alphaArrayLength"></param>
            public RectAlphaLayerInfo(RectInt rect, int alphaArrayLength) : this(rect)
            {
                alphas = new float[alphaArrayLength];
            }

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="bounds"></param>
            /// <param name="alphas"></param>
            public RectAlphaLayerInfo(RectInt rect, float[] alphas) : this(rect)
            {
                this.alphas = (float[])alphas.Clone();
            }

            /// <summary>
            /// 计算内切圆
            /// </summary>
            private void Compute()
            {
                var center = new Vector2Int(rect.x + (int)(rect.width / 2f), rect.y + (int)(rect.height / 2f));
                var circleRaduis = Mathf.Min(rect.size.x, rect.size.y)/2;
                var circleRaduisSqr = circleRaduis * circleRaduis;

                // 预计算圆形坐标
                var xMin = center.x - circleRaduis;
                var xMax = center.x + circleRaduis;
                var yMin = center.y - circleRaduis;
                var yMax = center.y + circleRaduis;
                for (int x = xMin; x < xMax; x++)
                {
                    for (int y = yMin; y < yMax; y++)
                    {
                        if ((new Vector2(x - center.x, y - center.y)).sqrMagnitude < circleRaduisSqr)
                        {
                            circleXY.Add((x, y));
                        }
                    }
                }
            }

            public List<(int, int)> circleXY = new List<(int, int)>();
        }

        /// <summary>
        /// 包围盒转地形矩形
        /// </summary>
        /// <param name="terrain"></param>
        /// <param name="bounds"></param>
        /// <returns></returns>
        public static RectInt BoundsToTerrainAlphamapRect(this Terrain terrain, Bounds bounds)
        {
            var aw = terrain.terrainData.alphamapWidth;
            var ah = terrain.terrainData.alphamapHeight;
            var toTerrainLocalScale = new Vector3(aw / terrain.terrainData.size.x, 0, ah / terrain.terrainData.size.z);

            var min = bounds.min - terrain.transform.position;
            min.Scale(toTerrainLocalScale);

            var maxW = aw - 1;
            var maxH = ah - 1;
            min.x = Mathf.Clamp(min.x, 0, maxW);
            min.z = Mathf.Clamp(min.z, 0, maxH);

            var max = bounds.max - terrain.transform.position;
            max.Scale(toTerrainLocalScale);

            max.x = Mathf.Clamp(max.x, 0, maxW);
            max.z = Mathf.Clamp(max.z, 0, maxH);

            var size = max - min;

            return new RectInt((int)min.x, (int)min.z, (int)size.x, (int)size.z);
        }

        /// <summary>
        /// 混合计算设置地形层透明度
        /// </summary>
        /// <param name="terrain">地形</param>
        /// <param name="rectAlphaInfoList">包围盒信息列表，只使用透明度数组中索引小于地形层数的数值</param>
        /// <param name="areaShape">区域形状</param>
        /// <param name="GetAlphaExtensionFun">外围计算透明度值算法：参数1=层索引；参数2=原透明度，参数3=将要设定的透明度，返回值=最后透明度</param>
        public static void SetLayerAlphas(this Terrain terrain, IEnumerable<RectAlphaLayerInfo> rectAlphaInfoList, EAreaShape areaShape = EAreaShape.Rect, Func<int, float, float, float> GetAlphaExtensionFun = null)
        {
            if (rectAlphaInfoList == null) return;

            if (GetAlphaExtensionFun == null)
            {
                GetAlphaExtensionFun = (index, oldValue, newValue) => newValue;
            }

            var layerLength = terrain.terrainData.terrainLayers.Length;

            var alphamap = terrain.terrainData.GetAlphamaps(0, 0, terrain.terrainData.alphamapWidth, terrain.terrainData.alphamapHeight);

            // 将矩形alpha组合为一个完整区域
            foreach (var item in rectAlphaInfoList)
            {
                var rect = item.rect;
                var center = rect.center;
                var size = rect.size;

                Func<int, int, bool> valueFun = (x, y) => true;
                switch (areaShape)
                {
                    case EAreaShape.Circle:
                        {
                            size.x = size.y = Mathf.Min(size.x, size.y);
                            rect.size = size;
                            var radius = size.x / 2;
                            var squareRadius = radius * radius;
                            valueFun = (x, y) => new Vector2(x - center.x, y - center.y).sqrMagnitude < squareRadius;
                            break;
                        }
                }

                var minAlphaArrayLength = Mathf.Min(item.alphas.Length, layerLength);

                for (int x = rect.min.x; x < rect.max.x; x++)
                {
                    for (int y = rect.min.y; y < rect.max.y; y++)
                    {
                        if (valueFun.Invoke(x, y))
                        {
                            for (int j = 0; j < minAlphaArrayLength; j++)
                            {
                                var a = item.alphas[j];
                                if (a >= 0)
                                {
                                    alphamap[y, x, j] = GetAlphaExtensionFun.Invoke(j, alphamap[y, x, j], a);
                                }
                            }
                        }
                    }
                }
            }

            terrain.terrainData.SetAlphamaps(0, 0, alphamap);
        }

        #endregion

        /// <summary>
        /// 根据位置获取地面索引
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public static int GetIndex(Vector3 position)
        {
            InitTerrainData();
            int index = -1;
            var TerrainCord = ConvertToSplatMapCoordinate(position);

            for (int k = 0; k < numTextures; k++)
            {
                if (0 < splatmapData[(int)TerrainCord.z, (int)TerrainCord.x, k]) index = k;
            }
            return index;
        }

        private static bool init = false;
        private static float[,,] splatmapData;
        private static float numTextures;

        /// <summary>
        /// 获取地形数据
        /// </summary>
        private static void InitTerrainData()
        {
            if (!init)
            {
                init = true;

                if (!Terrain.activeTerrain) return;

                var td = Terrain.activeTerrain.terrainData;
                var width = td.alphamapWidth;
                var height = td.alphamapHeight;

                splatmapData = td.GetAlphamaps(0, 0, width, height);
                numTextures = splatmapData.Length / (height * height);
            }
        }

        /// <summary>
        /// 将位置转为SplatMap坐标系
        /// </summary>
        /// <returns>SplatMap坐标系</returns>
        /// <param name="playerPos">玩家坐标</param>
        private static Vector3 ConvertToSplatMapCoordinate(Vector3 playerPos)
        {
            var ter = Terrain.activeTerrain;
            var terPosition = ter.transform.position;
            var vecRet = new Vector3();
            float tmp = (playerPos.x - terPosition.x) / ter.terrainData.size.x;
            vecRet.x = tmp * ter.terrainData.alphamapWidth;
            vecRet.z = tmp * ter.terrainData.alphamapHeight;
            return vecRet;
        }
    }

}
