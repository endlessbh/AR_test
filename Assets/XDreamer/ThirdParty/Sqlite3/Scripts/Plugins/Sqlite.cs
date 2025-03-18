using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using XCSJ.DataBase;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginDataBase;
#if XDREAMER_SQLITE
using Mono.Data.Sqlite;
#endif

namespace XCSJ.PluginSqlite
{
    /// <summary>
    /// SQLite数据类
    /// </summary>
#if XDREAMER_SQLITE
    public class Sqlite : BaseSqlite<SqliteConnection, SqliteCommand, SqliteDataReader>
#else
    public class Sqlite : BaseDB
#endif
    {
        /// <summary>
        /// 构造
        /// </summary>
        public Sqlite() : base() { }

#if XDREAMER_SQLITE

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="sqliteFileFullPath"></param>
        public Sqlite(string sqliteFileFullPath) : base(sqliteFileFullPath) { }

        /// <summary>
        /// 尝试获取连接字符串
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public override bool TryGetConnectionString(out string connectionString)
        {
#if UNITY_ANDROID
            connectionString = string.Format("URI = file:{0}", sqliteFileFullPath);
#else
            connectionString = string.Format("Data Source = {0}", sqliteFileFullPath);
#endif
            return true;
        }

#else

        /// <summary>
        /// 获取连接设置
        /// </summary>
        public override ConnectSettings connectSettings => new BaseSqliteConnectSettings(nameof(Sqlite));

        /// <summary>
        /// 关闭
        /// </summary>
        public override void Close() { }

        /// <summary>
        /// 连接
        /// </summary>
        /// <returns></returns>
        public override bool Connect() => false;

        /// <summary>
        /// 执行非查询
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public override Result ExecuteNonQuery(Sql sql) => new Result(sql, DataBase.DBHelper.ErrorCode, "执行" + nameof(ExecuteNonQuery) + "失败！原因是" + nameof(Sqlite) + "所需依赖库缺失！");

        /// <summary>
        /// 执行非查询
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public override ResultSet ExecuteQuery(Sql sql) => new ResultSet(this, sql, DataBase.DBHelper.ErrorCode, "执行" + nameof(ExecuteQuery) + "失败！原因是" + nameof(Sqlite) + "所需依赖库缺失！");

        /// <summary>
        /// 获取表名列表
        /// </summary>
        /// <returns></returns>
        public override List<string> GetTables() => new List<string>();

        /// <summary>
        /// 获取表结构
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public override TableStructure GetTableStructure(string tableName) => new TableStructure();

        /// <summary>
        /// 获取是否已连接
        /// </summary>
        /// <returns></returns>
        public override bool IsConnected() => false;

#endif
    }
}
