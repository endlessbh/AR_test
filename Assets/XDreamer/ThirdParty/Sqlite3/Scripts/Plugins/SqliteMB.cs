using UnityEngine;
using System.Collections;
using XCSJ.DataBase;
using XCSJ.PluginDataBase;
using System;
using XCSJ.PluginCommonUtils;
using XCSJ.Helper;
using System.IO;
using XCSJ.Attributes;

namespace XCSJ.PluginSqlite
{
    /// <summary>
    /// SQLite数据库：SQLite本地数据库,可在多个平台使用;仅可用于发布单独APP时使用；
    /// </summary>
    [Serializable]
    [DisallowMultipleComponent]
    [Name("SQLite数据库")]
    [Tip("SQLite本地数据库,可在多个平台使用;仅可用于发布单独APP时使用", "SQLite local database, which can be used on multiple platforms; It can only be used when issuing a separate app")]
#if UNITY_STANDALONE || UNITY_IOS || UNITY_ANDROID

#elif UNITY_EDITOR
    [Obsolete("SQLite数据库不支持当前的运行时环境(但可在编辑器环境中使用)，请使用其他类型的数据库替代")]
#else
    [Obsolete("SQLite数据库不支持当前的运行时环境，请使用其他类型的数据库替代", true)]
#endif
    public class SqliteMB : DBMB
    {
        /// <summary>
        /// 数据库
        /// </summary>
        public override BaseDB db => sqlite;

        /// <summary>
        /// 数据库名称
        /// </summary>
        public override string dbName
        {
            get
            {
                var name = base.dbName;
                if (string.IsNullOrEmpty(name))
                {
                    return dbInfo.GetResourceName();
                }
                return name;
            }
        }

        /// <summary>
        /// 数据库信息:数据库文件拷贝到可读写目录后与原始文件名同名
        /// </summary>
        [Name("数据库信息")]
        [Tip("数据库文件拷贝到可读写目录后与原始文件名同名", "After the database file is copied to the read-write directory, it has the same name as the original file name")]
        public ResourceFileInfo dbInfo = new ResourceFileInfo();

        /// <summary>
        /// SQLite数据库对象
        /// </summary>
        private Sqlite sqlite = new Sqlite();

        /// <summary>
        /// 在连接中
        /// </summary>
        [Name("在连接中")]
        [Readonly]
        public bool _inConnecting = false;

        /// <summary>
        /// 在连接中
        /// </summary>
        public override bool inConnecting => _inConnecting;

        /// <summary>
        /// 连接数据库
        /// </summary>
        /// <returns></returns>
        protected override bool ConnectDB()
        {
            try
            {
                if (sqlite.IsConnected()) return true;

                if (_inConnecting) return true;
                _inConnecting = true;

                //处理数据库
                sqlite.name = this.name;

#if XDREAMER_SQLITE
                sqlite.Connect(dbInfo.GetResourcePath());
#else
                sqlite.Connect();
#endif
                return sqlite.IsConnected();
            }
            finally
            {
                _inConnecting = false;
            }
        }

        /// <summary>
        /// 关闭数据库
        /// </summary>
        protected override void CloseDB()
        {
            sqlite.Close();
        }

        /// <summary>
        /// 销毁
        /// </summary>
        public void OnDestroy()
        {
            CloseDB();
        }
    }
}
