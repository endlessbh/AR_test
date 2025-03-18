using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.DataBase;
using XCSJ.Extension.Base.Attributes;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Extension.Base.Net;
using XCSJ.Interfaces;
using XCSJ.Net;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginDataBase.Base;
using XCSJ.PluginTools.Base;
using XCSJ.Scripts;

namespace XCSJ.PluginDataBase.Tools
{
    /// <summary>
    /// 对象关联信息
    /// </summary>
    [Name("对象关联信息")]
    [Tip("对象关联信息", "Object association information")]
    [RequireManager(typeof(DBManager))]
    [Tool(DBHelperExtension.FuncCompoents, nameof(DBManager))]
    [XCSJ.Attributes.Icon(EIcon.Link)]
    [Owner(typeof(DBManager))]
    public class ObjectLinkInfo : TriggerEventMB, IToFriendlyString
    {
        #region 关联

        /// <summary>
        /// 数据库
        /// </summary>
        [Group("关联设置")]
        [Name("数据库")]
        [ComponentPopup]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public DBMB _dbMB;

        /// <summary>
        /// 数据库
        /// </summary>
        public DBMB dbMB => this.XGetComponentInParentOrGlobal(ref _dbMB);

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
        public ESqlExecuteMode _sqlExecuteMode = ESqlExecuteMode.ConditionQuery;

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
        /// 覆盖条件值
        /// </summary>
        [Name("覆盖条件值")]
        public enum EOverrideConditionValue
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None,

            /// <summary>
            /// 名称
            /// </summary>
            [Name("名称")]
            Name,

            /// <summary>
            /// 类型名称
            /// </summary>
            [Name("类型名称")]
            TypeName,

            /// <summary>
            /// 类型全名称
            /// </summary>
            [Name("类型全名称")]
            TypeFullName,

            /// <summary>
            /// 目标名称
            /// </summary>
            [Name("目标名称")]
            TargetName = 100,

            /// <summary>
            /// 目标类型名称
            /// </summary>
            [Name("目标类型名称")]
            TargetTypeName,

            /// <summary>
            /// 目标类型全名称
            /// </summary>
            [Name("目标类型全名称")]
            TargetTypeFullName,
        }

        /// <summary>
        /// 覆盖条件值
        /// </summary>
        [Name("覆盖条件值")]
        [EnumPopup]
        [HideInSuperInspector(nameof(_sqlExecuteMode), EValidityCheckType.NotEqual, ESqlExecuteMode.ConditionQuery)]
        public EOverrideConditionValue _overrideConditionValue = EOverrideConditionValue.None;

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
        /// 获取条件值
        /// </summary>
        /// <returns></returns>
        public string GetConditionValue()
        {
            switch (_overrideConditionValue)
            {
                case EOverrideConditionValue.Name: return name;
                case EOverrideConditionValue.TypeName: return GetType().Name;
                case EOverrideConditionValue.TypeFullName: return GetType().FullName;
                case EOverrideConditionValue.TargetName:
                    {
                        var target = targetObject;
                        if (target) return target.name;
                        break;
                    }
                case EOverrideConditionValue.TargetTypeName:
                    {
                        var target = targetObject;
                        if (target) return target.GetType().Name;
                        break;
                    }
                case EOverrideConditionValue.TargetTypeFullName:
                    {
                        var target = targetObject;
                        if (target) return target.GetType().FullName;
                        break;
                    }
            }
            return null;
        }

        /// <summary>
        /// 转友好字符串
        /// </summary>
        /// <returns></returns>
        public string ToFriendlyString()
        {
            switch (_sqlExecuteMode)
            {
                case ESqlExecuteMode.NonQuery: return _nonQuerySql.ToFriendlyString();
                case ESqlExecuteMode.Query: return _querySql.ToFriendlyString();
                case ESqlExecuteMode.ConditionQuery: return _conditionQueryData.ToFriendlyString(GetConditionValue());
                case ESqlExecuteMode.KVNonQuery: return _nonQueryKey.ToFriendlyString();
                case ESqlExecuteMode.KVQuery: return _queryKey.ToFriendlyString();
            }
            return "";
        }

        /// <summary>
        /// 获取SQL语句
        /// </summary>
        /// <returns></returns>
        public string GetSql()
        {
            switch (_sqlExecuteMode)
            {
                case ESqlExecuteMode.NonQuery: return _nonQuerySql.GetValue();
                case ESqlExecuteMode.Query: return _querySql.GetValue();
                case ESqlExecuteMode.ConditionQuery: return _conditionQueryData.GetSql(GetConditionValue());
                case ESqlExecuteMode.KVNonQuery: return _nonQueryKey.GetValue();
                case ESqlExecuteMode.KVQuery: return _queryKey.GetValue();
            }
            return "";
        }

        #endregion

        #region 触发

        /// <summary>
        /// 当触发关联时
        /// </summary>
        protected override void OnTriggerEvent()
        {
            var dbMB = this.dbMB;
            if (!dbMB) return;

            //Debug.Log("OnTriggerEvent: " + name);
            switch (_sqlExecuteMode)
            {
                case ESqlExecuteMode.NonQuery:
                    {
                        if (!dbMB.TryExecuteNonQuery(_nonQuerySql.GetValue(), OnHandle))
                        {
                            OnHandle(InvokeResult.Fail, default(Result));
                        }
                        break;
                    }
                case ESqlExecuteMode.Query:
                    {
                        if (!dbMB.TryExecuteQuery(_querySql.GetValue(), OnHandle, _modifyResultSetCache))
                        {
                            OnHandle(InvokeResult.Fail, default(ResultSet));
                        }
                        break;
                    }
                case ESqlExecuteMode.ConditionQuery:
                    {
                        if (!dbMB.TryExecuteQuery(_conditionQueryData.GetSql(GetConditionValue()), OnHandle, _modifyResultSetCache))
                        {
                            OnHandle(InvokeResult.Fail, default(ResultSet));
                        }
                        break;
                    }
                case ESqlExecuteMode.KVNonQuery:
                    {
                        if (!dbMB.TryExecuteKVNonQuery(_nonQueryKey.GetValue(), _nonQueryValueParams.ToList(pv => pv.GetValue()), OnHandle))
                        {
                            OnHandle(InvokeResult.Fail, default(Result));
                        }
                        break;
                    }
                case ESqlExecuteMode.KVQuery:
                    {
                        if (!dbMB.TryExecuteKVQuery(_queryKey.GetValue(), _queryValueParams.ToList(pv => pv.GetValue()), OnHandle, _modifyResultSetCache))
                        {
                            OnHandle(InvokeResult.Fail, default(ResultSet));
                        }
                        break;
                    }
            }
        }

        #endregion

        #region 处理-成功数据的处理

        /// <summary>
        /// 成功时处理规则
        /// </summary>
        [Flags]
        [Name("成功时处理规则")]
        public enum EHadleRuleOnSuccess
        {
            /// <summary>
            /// 输出日志
            /// </summary>
            [Name("输出日志")]
            [EnumFieldName("输出日志")]
            OutputLog = 1 << 0,

            /// <summary>
            /// 显示结果集窗口
            /// </summary>
            [Name("显示结果集窗口")]
            [EnumFieldName("显示结果集窗口")]
            ShowResultSetWindow = 1 << 1,
        }

        /// <summary>
        /// 成功时处理规则
        /// </summary>
        [Group("处理设置")]
        [Name("成功时处理规则")]
        [EnumPopup]
        public EHadleRuleOnSuccess _hadleRuleOnSuccess = EHadleRuleOnSuccess.ShowResultSetWindow;

        /// <summary>
        /// 失败时处理规则
        /// </summary>
        [Flags]
        [Name("失败时处理规则")]
        public enum EHadleRuleOnFail
        {
            /// <summary>
            /// 输出日志
            /// </summary>
            [Name("输出日志")]
            [EnumFieldName("输出日志")]
            OutputLog = 1 << 0,
        }

        /// <summary>
        /// 失败时处理规则
        /// </summary>
        [Name("失败时处理规则")]
        [EnumPopup]
        public EHadleRuleOnFail _hadleRuleOnFail = EHadleRuleOnFail.OutputLog;

        /// <summary>
        /// 结果集窗口
        /// </summary>
        [Name("结果集窗口")]
        [ComponentPopup]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public ResultSetWindowByIMGUI _resultSetWindow;

        /// <summary>
        /// 结果集窗口
        /// </summary>
        public ResultSetWindowByIMGUI resultSetWindow
        {
            get
            {
                if (!_resultSetWindow)
                {
                    var dbMB = this.dbMB;
                    if (dbMB)//从数据库组件上查找
                    {
                        var window = dbMB.GetComponent<ResultSetWindowByIMGUI>();
                        if (window)
                        {
                            this.XModifyProperty(ref _resultSetWindow, window);
                        }
                    }
                    if (!_resultSetWindow)//全局查找
                    {
                        this.XGetComponentInParentOrGlobal(ref _resultSetWindow);
                    }
                    if (!_resultSetWindow)//新创建
                    {
                        var window = this.XGetOrAddComponent<ResultSetWindowByIMGUI>();
                        this.XModifyProperty(ref _resultSetWindow, window);
                    }
                }
                return _resultSetWindow;
            }
        }

        /// <summary>
        /// 结果集窗口模式
        /// </summary>
        [Name("结果集窗口模式")]
        [EnumPopup]
        public EResultSetWindowMode _resultSetWindowMode = EResultSetWindowMode.KV;

        private void OnHandle(InvokeResult invokeResult, Result result)
        {
            //Debug.Log("OnHandle: " + name);
            if (invokeResult)
            {
                if ((_hadleRuleOnSuccess & EHadleRuleOnSuccess.OutputLog) == EHadleRuleOnSuccess.OutputLog)
                {
                    Log.DebugFormat("对象关联信息[{0}]对目标对象[{1}]执行非查询[{2}]成功:{3}",
                        CommonFun.GameObjectToString(gameObject),
                        CommonFun.ObjectToString(targetObject),
                        result?.sql,
                        result?.result);
                }
            }
            else
            {
                if ((_hadleRuleOnFail & EHadleRuleOnFail.OutputLog) == EHadleRuleOnFail.OutputLog)
                {
                    Log.ErrorFormat("对象关联信息[{0}]对目标对象[{1}]执行非查询[{2}]失败:{3}\n{4}",
                        CommonFun.GameObjectToString(gameObject),
                        CommonFun.ObjectToString(targetObject),
                        result?.sql,
                        result?.error,
                        invokeResult.error);
                }
            }
        }

        private void OnHandle(InvokeResult invokeResult, ResultSet resultSet)
        {
            //Debug.Log(invokeResult.success);
            if (invokeResult)
            {
                if ((_hadleRuleOnSuccess & EHadleRuleOnSuccess.OutputLog) == EHadleRuleOnSuccess.OutputLog)
                {
                    Log.DebugFormat("对象关联信息[{0}]对目标对象[{1}]执行查询[{2}]成功:{3},记录数:{4},字段数:{5} ",
                        CommonFun.GameObjectToString(gameObject),
                        CommonFun.ObjectToString(targetObject),
                        resultSet?.sql, 
                        resultSet?.result, 
                        resultSet?.recordsAffected, 
                        resultSet?.fieldCount);
                }
                if ((_hadleRuleOnSuccess & EHadleRuleOnSuccess.ShowResultSetWindow) == EHadleRuleOnSuccess.ShowResultSetWindow)
                {
                    var resultSetWindow = this.resultSetWindow;
                    if (resultSetWindow)
                    {
                        resultSetWindow.XSetEnable(true);
                        var window = resultSetWindow.GetResultSetWindow(_resultSetWindowMode);
                        window.visable = true;
                        window.resultSet = resultSet;
                    }
                }
            }
            else
            {
                if ((_hadleRuleOnFail & EHadleRuleOnFail.OutputLog) == EHadleRuleOnFail.OutputLog)
                {
                    Log.ErrorFormat("对象关联信息[{0}]对目标对象[{1}]执行查询[{2}]失败:{3}\n{4}",
                        CommonFun.GameObjectToString(gameObject),
                        CommonFun.ObjectToString(targetObject),
                        resultSet?.sql, 
                        resultSet?.error, 
                        invokeResult.error);
                }
            }
        }

        #endregion

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            if (dbMB) { }
            if (resultSetWindow) { }
        }
    }

    /// <summary>
    /// Sql执行模式
    /// </summary>
    [Name("Sql执行模式")]
    public enum ESqlExecuteMode
    {
        /// <summary>
        /// 无
        /// </summary>
        [Name("无")]
        None,

        /// <summary>
        /// 非查询
        /// </summary>
        [Name("非查询")]
        NonQuery,

        /// <summary>
        /// 查询
        /// </summary>
        [Name("查询")]
        Query,

        /// <summary>
        /// 条件查询
        /// </summary>
        [Name("条件查询")]
        ConditionQuery,

        /// <summary>
        /// 键值非查询
        /// </summary>
        [Name("键值非查询")]
        KVNonQuery,

        /// <summary>
        /// 键值查询
        /// </summary>
        [Name("键值查询")]
        KVQuery,
    }
}


