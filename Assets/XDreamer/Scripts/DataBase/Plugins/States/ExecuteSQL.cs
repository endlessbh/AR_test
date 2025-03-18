using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.DataBase;
using XCSJ.Extension.Base.Attributes;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginDataBase.Base;
using XCSJ.PluginDataBase.Tools;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Base;
using XCSJ.Scripts;

namespace XCSJ.PluginDataBase.States
{
    /// <summary>
    /// 执行SQL
    /// </summary>
    [Name(Title)]
    [Tip("执行SQL并等待执行完成", "Execute SQL and wait for execution to complete")]
    [ComponentMenu(DBHelper.Title + "/" + Title, typeof(DBManager))]
    [DisallowMultipleComponent]
    [XCSJ.Attributes.Icon(EIcon.Event)]
    [Owner(typeof(DBManager))]
    public class ExecuteSQL : Trigger<ExecuteSQL>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "执行SQL";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(DBHelper.Title, typeof(DBManager))]
        [StateComponentMenu(DBHelper.Title + "/" + Title, typeof(DBManager))]
        [Name(Title, nameof(ExecuteSQL))]
        [Tip("执行SQL并等待执行完成", "Execute SQL and wait for execution to complete")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 数据库
        /// </summary>
        [Name("数据库")]
        [ComponentPopup]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public DBMB _dbMB;

        /// <summary>
        /// 数据库
        /// </summary>
        public DBMB dbMB => this.XGetComponentInGlobal(ref _dbMB);

        /// <summary>
        /// 修改结果集缓存
        /// </summary>
        [Name("修改结果集缓存")]
        [Tip("修改数据库组件中结果集缓存", "Modify the result set cache in the database component")]
        public bool _modifyResultSetCache = true;

        /// <summary>
        /// Sql执行模式
        /// </summary>
        [Name("Sql执行模式")]
        [EnumPopup]
        public ESqlExecuteMode _sqlExecuteMode = ESqlExecuteMode.Query;

        /// <summary>
        /// 非查询SQL:非查询SQL语句
        /// </summary>
        [Name("非查询SQL")]
        [Tip("非查询SQL语句", "Non query SQL statement")]
        [HideInSuperInspector(nameof(_sqlExecuteMode), EValidityCheckType.NotEqual, ESqlExecuteMode.NonQuery)]
        [OnlyMemberElements]
        public StringPropertyValue _nonQuerySql = new StringPropertyValue();

        /// <summary>
        /// 查询SQL:查询SQL语句
        /// </summary>
        [Name("查询SQL")]
        [Tip("查询SQL语句", "Query SQL Statement")]
        [HideInSuperInspector(nameof(_sqlExecuteMode), EValidityCheckType.NotEqual, ESqlExecuteMode.Query)]
        [OnlyMemberElements]
        public StringPropertyValue _querySql = new StringPropertyValue();

        /// <summary>
        /// 条件查询:条件查询
        /// </summary>
        [Name("条件查询")]
        [Tip("条件查询", "Condition query")]
        [HideInSuperInspector(nameof(_sqlExecuteMode), EValidityCheckType.NotEqual, ESqlExecuteMode.ConditionQuery)]
        [OnlyMemberElements]
        public ConditionQueryData _conditionQueryData = new ConditionQueryData();

        /// <summary>
        /// 非查询键:键值非查询使用的键名
        /// </summary>
        [Name("非查询键")]
        [Tip("键值非查询使用的键名", "Key name used in key value non query")]
        [HideInSuperInspector(nameof(_sqlExecuteMode), EValidityCheckType.NotEqual, ESqlExecuteMode.KVNonQuery)]
        public StringPropertyValue _nonQueryKey = new StringPropertyValue();

        /// <summary>
        /// 非查询值参数列表:键值非查询使用的值参数列表
        /// </summary>
        [Name("非查询值参数列表")]
        [Tip("键值非查询使用的值参数列表", "List of value parameters used in key value non query")]
        [HideInSuperInspector(nameof(_sqlExecuteMode), EValidityCheckType.NotEqual, ESqlExecuteMode.KVNonQuery)]
        public List<StringPropertyValue> _nonQueryValueParams = new List<StringPropertyValue>();

        /// <summary>
        /// 查询键:键值查询使用的键名
        /// </summary>
        [Name("查询键")]
        [Tip("键值查询使用的键名", "Key name used in key value query")]
        [HideInSuperInspector(nameof(_sqlExecuteMode), EValidityCheckType.NotEqual, ESqlExecuteMode.KVQuery)]
        public StringPropertyValue _queryKey = new StringPropertyValue();

        /// <summary>
        /// 查询值参数列表:键值查询使用的值参数列表
        /// </summary>
        [Name("查询值参数列表")]
        [Tip("键值查询使用的值参数列表", "List of value parameters used in key value query")]
        [HideInSuperInspector(nameof(_sqlExecuteMode), EValidityCheckType.NotEqual, ESqlExecuteMode.KVQuery)]
        public List<StringPropertyValue> _queryValueParams = new List<StringPropertyValue>();

        /// <summary>
        /// 结果集变量字符串列表
        /// </summary>
        [Name("结果集变量字符串列表")]
        public List<ResultSetVarString> _resultSetVarStringList = new List<ResultSetVarString>();

        /// <summary>
        /// 结果集变量字符串
        /// </summary>
        [Serializable]
        public class ResultSetVarString
        {
            /// <summary>
            /// 结果集信息
            /// </summary>
            [Name("结果集信息")]
            [EnumPopup]
            public EResultSetInfo _resultSetInfo = EResultSetInfo.OnlyRecord;

            /// <summary>
            /// 结果集变量
            /// </summary>
            [Name("结果集变量")]
            [VarString(EVarStringHierarchyKeyMode.Set)]
            public string _resultSetVarString = "";
        }

        /// <summary>
        /// 结果集信息
        /// </summary>
        [Name("结果集信息")]
        public enum EResultSetInfo
        {
            /// <summary>
            /// 完整结果集
            /// </summary>
            [Name("完整结果集")]
            FullResultSet,

            /// <summary>
            /// 仅记录
            /// </summary>
            [Name("仅记录")]
            OnlyRecord,

            /// <summary>
            /// 仅字段
            /// </summary>
            [Name("仅字段")]
            OnlyField,

            /// <summary>
            /// 记录数
            /// </summary>
            [Name("记录数")]
            RecordCount,

            /// <summary>
            /// 字段数
            /// </summary>
            [Name("字段数")]
            FieldCount,

            /// <summary>
            /// SQL
            /// </summary>
            [Name("SQL")]
            SQL,

            /// <summary>
            /// 结果值
            /// </summary>
            [Name("结果值")]
            ResultValue,

            /// <summary>
            /// 错误信息
            /// </summary>
            [Name("错误信息")]
            ErrorInfo,
        }

        private void SetRS(ResultSet resultSet)
        {
            var script = ScriptManager.instance;
            if (script)
            {
                foreach(var v in _resultSetVarStringList)
                {
                    switch (v._resultSetInfo)
                    {
                        case EResultSetInfo.FullResultSet:
                            {
                                script.TrySetOrAddSetHierarchyVarValue(v._resultSetVarString, resultSet?.ToJson() ?? "");
                                break;
                            }
                        case EResultSetInfo.OnlyRecord:
                            {
                                script.TrySetOrAddSetHierarchyVarValue(v._resultSetVarString, resultSet?.recordSet?.ToJsonLite());
                                break;
                            }
                        case EResultSetInfo.OnlyField:
                            {
                                script.TrySetOrAddSetHierarchyVarValue(v._resultSetVarString, resultSet?.fieldSet?.ToJsonLite() ?? "");
                                break;
                            }
                        case EResultSetInfo.RecordCount:
                            {
                                script.TrySetOrAddSetHierarchyVarValue(v._resultSetVarString, resultSet?.recordSet?.records?.Count.ToString() ?? "");
                                break;
                            }
                        case EResultSetInfo.FieldCount:
                            {
                                script.TrySetOrAddSetHierarchyVarValue(v._resultSetVarString, resultSet?.fieldSet?.fields?.Count.ToString() ?? "");
                                break;
                            }
                        case EResultSetInfo.SQL:
                            {
                                script.TrySetOrAddSetHierarchyVarValue(v._resultSetVarString, resultSet?.sql?.sql ?? "");
                                break;
                            }
                        case EResultSetInfo.ResultValue:
                            {
                                script.TrySetOrAddSetHierarchyVarValue(v._resultSetVarString, resultSet?.result.ToString() ?? "");
                                break;
                            }
                        case EResultSetInfo.ErrorInfo:
                            {
                                script.TrySetOrAddSetHierarchyVarValue(v._resultSetVarString, resultSet?.error??"");
                                break;
                            }
                    }
                }
            }
        }

        /// <summary>
        /// 转友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString()
        {
            switch (_sqlExecuteMode)
            {
                case ESqlExecuteMode.NonQuery: return _nonQuerySql.ToFriendlyString();
                case ESqlExecuteMode.Query: return _querySql.ToFriendlyString();
                case ESqlExecuteMode.ConditionQuery: return _conditionQueryData.ToFriendlyString();
                case ESqlExecuteMode.KVNonQuery: return _nonQueryKey.ToFriendlyString();
                case ESqlExecuteMode.KVQuery: return _queryKey.ToFriendlyString();
            }
            return base.ToFriendlyString();
        }

        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity()
        {
            if (!_dbMB) return false;
            switch (_sqlExecuteMode)
            {
                case ESqlExecuteMode.NonQuery: return _nonQuerySql.DataValidity();
                case ESqlExecuteMode.Query: return _querySql.DataValidity();
                case ESqlExecuteMode.ConditionQuery: return _conditionQueryData.DataValidity();
                case ESqlExecuteMode.KVNonQuery: return _nonQueryKey.DataValidity();
                case ESqlExecuteMode.KVQuery: return _queryKey.DataValidity();
            }
            return base.DataValidity();
        }

        /// <summary>
        /// 进入
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnEntry(StateData stateData)
        {
            base.OnEntry(stateData);
            var dbMB = this.dbMB;
            if (!dbMB) return;
            switch (_sqlExecuteMode)
            {
                case ESqlExecuteMode.NonQuery:
                    {
                        dbMB.TryExecuteNonQuery(_nonQuerySql.GetValue(), (ir, r) =>
                        {
                            finished = true;
                        });
                        break;
                    }
                case ESqlExecuteMode.Query:
                    {
                        dbMB.TryExecuteQuery(_querySql.GetValue(), (ir, rs) =>
                        {
                            finished = true;
                            SetRS(rs);
                        }, _modifyResultSetCache);
                        break;
                    }
                case ESqlExecuteMode.ConditionQuery:
                    {
                        dbMB.TryExecuteQuery(_conditionQueryData.GetSql(), (ir, rs) =>
                        {
                            finished = true;
                            SetRS(rs);
                        }, _modifyResultSetCache);
                        break;
                    }
                case ESqlExecuteMode.KVNonQuery:
                    {
                        dbMB.TryExecuteKVNonQuery(_nonQueryKey.GetValue(), _nonQueryValueParams.ToList(pv => pv.GetValue()), (ir, r) =>
                        {
                            finished = true;
                        });
                        break;
                    }
                case ESqlExecuteMode.KVQuery:
                    {
                        dbMB.TryExecuteKVQuery(_queryKey.GetValue(), _queryValueParams.ToList(pv => pv.GetValue()), (ir, rs) =>
                        {
                            finished = true;
                            SetRS(rs);
                        }, _modifyResultSetCache);
                        break;
                    }
            }
        }

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            if (dbMB) { }
        }
    }
}
