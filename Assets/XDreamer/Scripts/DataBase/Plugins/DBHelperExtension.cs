using XCSJ.DataBase;
using XCSJ.Products;

namespace XCSJ.PluginDataBase
{
    /// <summary>
    /// 数据库辅助扩展类
    /// </summary>
    public static class DBHelperExtension
    {
        /// <summary>
        /// 功能组件
        /// </summary>
        public const string FuncCompoents = DBHelper.Title + "-功能组件";

        /// <summary>
        /// WWW过时无效解释
        /// </summary>
        public const string WWWObsoleteInvalidExplanation = "因Unity的WWW类被标记过时,所以WWW类型网络数据库不再推荐使用!可使用UnityWebRequest类型的Web网络数据库替代!";

        /// <summary>
        /// WS路径
        /// </summary>
        public const string WSPath = ProductServer.DataBaseServer.WSPath;

        /// <summary>
        /// 获取数据库索引
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="baseDB"></param>
        /// <returns></returns>
        public static int GetDBIndex(this DBManager manager, IBaseDB baseDB)
        {
            if (!manager) return -1;
            return manager.dbs.FindIndex(db => db.db == baseDB);
        }

        /// <summary>
        /// 获取数据库中文脚本索引
        /// </summary>
        /// <returns></returns>
        public static int GetDBCNScriptIndex(this DBManager manager, IBaseDB baseDB) => GetDBIndex(manager, baseDB) + 1;
    }
}
