using UnityEngine;
using System.Collections.Generic;

namespace XCSJ.Extension.Base.Maths
{
    /// <summary>
    /// 四叉树：用于平面分割为4个区域并快速查找对象
    /// 1.默认构造的四叉树节点都是叶子节点
    /// 2.对象只存放在叶子节点上, 如果对象横跨于不同区域上，会重复存储（不重复存储，会导致碰撞检测失败）
    /// 3.每个叶子节点可以存放的对象上限是maxNodeCount<see cref="nodeCapacity"/> 
    /// 4.当叶子节点存放的对象数量超过 maxNodeCount<see cref="nodeCapacity"/>，则分离生成子树（4个区域象限）， 并把自身存储的所有对象存放到子树中，并清除自身所有保存的对象引用
    /// 5.使用树深度对四叉树进行分裂控制：主要是防止在横跨在不同区域的对象数量超过了最大节点树时子树无限进入分裂递归状态。这时候叶子节点数量会大于maxNodeCount；
    /// </summary>
    /// <typeparam name="T">模版</typeparam>
    public class QuadTree<T>
    {
        /// <summary>
        /// 矩形区域
        /// </summary>
        public Rect rect { get; private set; }

        /// <summary>
        /// 当前层级最大叶子节点数:插入节点数量超过当前层级最大节点数，则划分为子层级
        /// </summary>
        public int nodeCapacity { get; private set; }

        /// <summary>
        /// 树深度
        /// </summary>
        public int deep { get; private set; }

        /// <summary>
        /// 最大深度
        /// </summary>
        public static int maxDeep = 10;

        /// <summary>
        /// 叶子节点列表
        /// </summary>
        private List<(Rect, T)> nodes;

        /// <summary>
        /// 子级四叉树：顺序为左下、左上、右上、右下
        /// </summary>
        private QuadTree<T>[] children = null;

        /// <summary>
        /// 存在子级四叉树
        /// </summary>
        private bool hasChildren => children != null;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="nodeCapacity"></param>
        /// <param name="deep"></param>
        public QuadTree(Rect rect, int nodeCapacity = 100, int deep = 0)
        {
            this.rect = rect;

            this.deep = deep;

            this.nodeCapacity = nodeCapacity;

            nodes = new List<(Rect, T)>(nodeCapacity);// 事先分配连续内存
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        public bool Add(Rect rect, T node) => Add((rect, node));

        /// <summary>
        /// 添加节点
        /// </summary>
        /// <param name="node"></param>
        public bool Add((Rect, T) node)
        {
            if (!rect.Overlaps(node.Item1)) return false;

            if (hasChildren)
            {
                AddNodeToChildren(node);
            }
            else
            {
                // 小于最大深度 和 节点数量超过当前节点数量
                if (deep < maxDeep && nodes.Count == nodeCapacity)
                {
                    if (!hasChildren)
                    {
                        Split();
                    }

                    //将节点移动到其子对象上
                    foreach (var n in nodes)
                    {
                        AddNodeToChildren(n);
                    }
                    nodes.Clear();

                    // 将新节点插入到子对象中
                    AddNodeToChildren(node);
                }
                else
                {
                    nodes.Add(node);
                }
            }
            return true;
        }

        private void AddNodeToChildren((Rect, T) node)
        {
            for (int i = 0; i < 4; i++)
            {
                children[i].Add(node);
            }
        }

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        public bool Remove(Rect rect, T node) => Remove((rect, node));

        /// <summary>
        /// 移除节点
        /// </summary>
        /// <param name="node"></param>
        public bool Remove((Rect, T) node)
        {
            if (!rect.Overlaps(node.Item1)) return false;
            
            if (hasChildren)
            {
                var rs = false;
                for (int i = 0; i < 4; i++)
                {
                    if (children[i].Remove(node))
                    {
                        rs = true;
                    }
                }
                return rs;
            }
            else
            {
                var index = nodes.FindIndex(l => l.Item2.Equals(node.Item2));
                var rs = index >= 0;
                if (rs)
                {
                    nodes.RemoveAt(index);
                }
                return rs;
            }
        }

        /// <summary>
        /// 查找区域内所有节点
        /// </summary>
        /// <param name="area"></param>
        /// <returns></returns>
        public List<(Rect, T)> Find(Rect area)
        {
            var resultNodes = new List<(Rect, T)>();
            if (!rect.Overlaps(area)) return resultNodes;

            if (hasChildren)
            {
                for (int i = 0; i < 4; i++)
                {
                    resultNodes.AddRange(children[i].Find(area));
                }
            }
            else
            {
                foreach (var n in nodes)
                {
                    if (area.Overlaps(n.Item1))
                    {
                        resultNodes.Add(n);
                    }
                }
            }
            return resultNodes;
        }
        
        /// <summary>
        /// 清空四叉树
        /// </summary>
        public void Clear()
        {
            nodes.Clear();

            if (hasChildren)
            {
                for (int i = 0; i < 4; i++)
                {
                    children[i].Clear();
                }
                children = null;
            }
        }

        /// <summary>
        /// 绘制
        /// </summary>
        public void DrawGizmos()
        {
            var org = Gizmos.color;
            Gizmos.color = Color.red;

            var lt = new Vector3(rect.x, 0, rect.y);// 左下
            var rt = new Vector3(rect.x + rect.width, 0, rect.y);// 右下
            var ld = new Vector3(rect.x, 0, rect.y + rect.height);// 左上
            var rd = new Vector3(rect.x + rect.width, 0, rect.y + rect.height);//右上

            Gizmos.DrawLine(lt, ld);
            Gizmos.DrawLine(lt, rt);
            Gizmos.DrawLine(rt, rd);
            Gizmos.DrawLine(ld, rd);

            Gizmos.color = org;

            if (hasChildren)
            {
                for (int i = 0; i < children.Length; i++)
                {
                    children[i].DrawGizmos();
                }
            }
        }

        /// <summary>
        /// 分裂子树
        /// </summary>
        private void Split()
        {
            var leftDown = rect.position;
            var subSize = rect.size / 2;
            var w = new Vector2(subSize.x, 0);
            var h = new Vector2(0, subSize.y);

            children = new QuadTree<T>[4];
            var childDeep = deep + 1;
            children[0] = new QuadTree<T>(new Rect(leftDown, subSize), nodeCapacity, childDeep);
            children[1] = new QuadTree<T>(new Rect(leftDown + h, subSize), nodeCapacity, childDeep);
            children[2] = new QuadTree<T>(new Rect(leftDown + h + w, subSize), nodeCapacity, childDeep);
            children[3] = new QuadTree<T>(new Rect(leftDown + w, subSize), nodeCapacity, childDeep);
        }
    }
}
