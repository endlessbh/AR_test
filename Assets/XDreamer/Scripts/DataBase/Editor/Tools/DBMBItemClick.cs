using System;
using UnityEditor;
using XCSJ.EditorCommonUtils.Base.CategoryViews;
using XCSJ.EditorTools;
using XCSJ.PluginDataBase;

namespace XCSJ.EditorDataBase.Tools
{
    /// <summary>
    /// 数据库组件项点击
    /// </summary>
    [ToolItemClick(typeof(DBMB), true)]
    public class DBMBItemClick : IItemClick
    {
        /// <summary>
        /// 能否点击
        /// </summary>
        /// <param name="toolItem"></param>
        /// <returns></returns>
        public bool CanClick(IItem toolItem) => true;

        /// <summary>
        /// 点击时
        /// </summary>
        /// <param name="toolItem"></param>
        public void OnClick(IItem toolItem)
        {
            if (!DBManager.instance) return;
            var dBMB = DBManager.instance.AddDB(toolItem.memberInfo as Type);
            if (dBMB)
            {
                EditorGUIUtility.PingObject(dBMB.gameObject);
            }
        }
    }
}
