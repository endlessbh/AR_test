using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.DataBase;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;

namespace XCSJ.PluginDataBase.Tools
{
    /// <summary>
    /// 数据库别名数据
    /// </summary>
    [Name("数据库别名数据")]
    [Tip("数据库别名数据", "Database alias data")]
    [RequireManager(typeof(DBManager))]
    [Tool(DBHelperExtension.FuncCompoents, nameof(DBManager), nameof(DBMB))]
    [XCSJ.Attributes.Icon(EIcon.Data)]
    [Owner(typeof(DBManager))]
    public class DBAliasData : InteractProvider, IDBAliases
    {
        /// <summary>
        /// 数据库别名
        /// </summary>
        [Name("数据库别名")]
        [OnlyMemberElements]
        public DBAliases _dbAliases = new DBAliases();

        /// <summary>
        /// 获取数据库别名
        /// </summary>
        /// <returns></returns>
        public DBAlias GetDBAlias(string dbName) => _dbAliases.GetDBAlias(dbName) ;

        /// <summary>
        /// 获取表别名
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public TableAlias GetTableAlias(string dbName, string tableName) => _dbAliases.GetDBAlias(dbName)?.GetTableAlias(tableName) ;

        /// <summary>
        /// 获取字段别名
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public FieldAlias GetFieldAlias(string dbName, string tableName, string fieldName) => _dbAliases.GetDBAlias(dbName)?.GetTableAlias(tableName)?.GetFieldAlias(fieldName) ;

        /// <summary>
        /// 获取字段别名
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public FieldAlias GetFieldAlias(string fieldName) => GetFieldAlias("", "", fieldName);
    }

    /// <summary>
    /// 数据库别名集类
    /// </summary>
    [Serializable]
    [Name("数据库别名集")]
    public class DBAliases
    {
        /// <summary>
        /// 数据库别名列表
        /// </summary>
        [Name("数据库别名列表")]
        public List<DBAlias> _dbAliases = new List<DBAlias>();

        /// <summary>
        /// 获取表别名
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public DBAlias GetDBAlias(string dbName) => _dbAliases.FirstOrDefault(a => a._name == dbName);

        /// <summary>
        /// 获取表别名
        /// </summary>
        /// <param name="dbName"></param>
        /// <returns></returns>
        public string GetDBAliasName(string dbName) => _dbAliases.FirstOrDefault(a => a._name == dbName)?._alias ?? dbName;
    }

    /// <summary>
    /// 数据库别名
    /// </summary>
    [Serializable]
    [Name("数据库别名")]
    public class DBAlias : AliasData
    {
        /// <summary>
        /// 表别名列表
        /// </summary>
        [Name("表别名列表")]
        public List<TableAlias> _tableAliases = new List<TableAlias>();

        /// <summary>
        /// 获取表别名
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public TableAlias GetTableAlias(string tableName) => _tableAliases.FirstOrDefault(a => a._name == tableName);

        /// <summary>
        /// 获取表别名
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public string GetTableAliasName(string tableName) => _tableAliases.FirstOrDefault(a => a._name == tableName)?._alias ?? tableName;
    }

    /// <summary>
    /// 表别名
    /// </summary>
    [Serializable]
    [Name("表别名")]
    public class TableAlias : AliasData, ITableAlias
    {
        /// <summary>
        /// 字段别名列表
        /// </summary>
        [Name("字段别名列表")]
        public List<FieldAlias> _fieldAliases = new List<FieldAlias>();

        /// <summary>
        /// 获取字段别名
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public FieldAlias GetFieldAlias(string fieldName) => _fieldAliases.FirstOrDefault(a => a._name == fieldName);

        /// <summary>
        /// 获取字段别名
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public string GetFieldAliasName(string fieldName) => _fieldAliases.FirstOrDefault(a => a._name == fieldName)?._alias ?? fieldName;
    }

    /// <summary>
    /// 字段别名
    /// </summary>
    [Serializable]
    [Name("字段别名")]
    public class FieldAlias : AliasData { }

    /// <summary>
    /// 别名数据
    /// </summary>
    [Serializable]
    [Name("别名数据")]
    public class AliasData
    {
        /// <summary>
        /// 名称:原始名称
        /// </summary>
        [Name("名称")]
        [Tip("原始名称", "Original name")]
        public string _name;

        /// <summary>
        /// 别名:别名名称
        /// </summary>
        [Name("别名")]
        [Tip("别名名称", "Alias name")]
        public string _alias;
    }

    /// <summary>
    /// 数据库别名列表接口
    /// </summary>
    public interface IDBAliases: ITableAlias
    {
        /// <summary>
        /// 获取数据库别名
        /// </summary>
        /// <param name="dbName"></param>
        /// <returns></returns>
        DBAlias GetDBAlias(string dbName);

        /// <summary>
        /// 获取表别名
        /// </summary>
        /// <param name="dbName"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        TableAlias GetTableAlias(string dbName, string tableName);

        /// <summary>
        /// 获取字段别名
        /// </summary>
        /// <param name="dbName"></param>
        /// <param name="tableName"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        FieldAlias GetFieldAlias(string dbName, string tableName, string fieldName);
    }

    /// <summary>
    /// 表别名接口
    /// </summary>
    public interface ITableAlias
    {
        /// <summary>
        /// 获取字段别名
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        FieldAlias GetFieldAlias(string fieldName);
    }
}
