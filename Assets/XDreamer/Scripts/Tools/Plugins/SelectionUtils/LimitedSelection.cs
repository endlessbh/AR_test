using UnityEngine;
using XCSJ.PluginCommonUtils.Runtime;

namespace XCSJ.PluginTools.SelectionUtils
{
    /// <summary>
    /// 限定选择集
    /// </summary>
    public class LimitedSelection
    {
        /// <summary>
        /// 最大数目
        /// </summary>
        public static int _maxCount = 1;

        /// <summary>
        /// 最大数目
        /// </summary>
        public static int maxCount
        {
            get { return _maxCount; }
            set { _maxCount = Mathf.Clamp(value, 1, 10000); }
        }

        /// <summary>
        /// 设置游戏对象被选择
        /// </summary>
        /// <param name="go"></param>
        public static void SetSelected(GameObject go)
        {
            if (maxCount == 1)
            {
                Selection.selection = go;
            }
            else
            {
                Selection.AddOrRemove(go);
                if (Selection.selections.Length > maxCount)
                {
                    Selection.Remove(Selection.selections[0]);
                }
            }
        }
    }
}
