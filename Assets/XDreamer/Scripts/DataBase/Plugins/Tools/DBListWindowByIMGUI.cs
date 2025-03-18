using System;
using System.Collections.Generic;
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
    /// 数据库列表窗口通过IMGUI
    /// </summary>
    [Name("数据库列表窗口通过IMGUI")]
    [Tip("通过IMGUI方式在运行时绘制数据库列表信息", "Drawing database list information at runtime through imgui")]
    [RequireManager(typeof(DBManager))]
    //[RequireComponent(typeof(DBManager))]
    [Tool(DBHelperExtension.FuncCompoents, nameof(DBManager))]
    [XCSJ.Attributes.Icon(EIcon.Data)]
    [Owner(typeof(DBManager))]
    public class DBListWindowByIMGUI : InteractProvider
    {
        /// <summary>
        /// 数据库
        /// </summary>
        [Name("数据库")]
        [ComponentPopup]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public DBManager _dbManager;

        /// <summary>
        /// 数据库
        /// </summary>
        public DBManager dbManager => this.XGetComponentInParentOrGlobal(ref _dbManager);

        /// <summary>
        /// 数据库别名数据
        /// </summary>
        [Name("数据库别名数据")]
        [ComponentPopup]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public DBAliasData _dbAliasData;

        /// <summary>
        /// 数据库别名数据
        /// </summary>
        public DBAliasData dbAliasData => this.XGetComponentInParentOrGlobal(ref _dbAliasData);

        /// <summary>
        /// 数据库列表窗口
        /// </summary>
        [Name("数据库列表窗口")]
        public DBListWindow _dbListWindow = new DBListWindow();

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            _dbListWindow.dbManager = dbManager;
            _dbListWindow.dbAliases = dbAliasData;
        }

        /// <summary>
        /// 绘制GUI
        /// </summary>
        private void OnGUI()
        {
            _dbListWindow.OnGUI();
        }

        /// <summary>
        /// 重置
        /// </summary>
        private void Reset()
        {
            if (dbManager) { }
            if (dbAliasData) { }
            _dbListWindow.size = new Vector2(800, 400);
            _dbListWindow._minSize = new Vector2(600, 300);
        }
    }

    /// <summary>
    /// 数据库列表窗口
    /// </summary>
    [Serializable]
    [Name("数据库列表窗口")]
    public class DBListWindow : BaseGUIWindow
    {
        /// <summary>
        /// 数据库管理器
        /// </summary>
        public DBManager dbManager { get; internal set; }

        /// <summary>
        /// 数组库别名
        /// </summary>
        public IDBAliases dbAliases { get; internal set; } = null;

        /// <summary>
        /// 当绘制内容
        /// </summary>
        /// <param name="scrollRect"></param>
        /// <param name="scrollViewRect"></param>
        protected override void OnDrawContent(Rect scrollRect, Rect scrollViewRect)
        {
            DrawDBInfo(dbManager);
        }

        /// <summary>
        /// 左侧滚动位置
        /// </summary>
        [Name("左侧滚动位置")]
        public Vector2 _leftScrollPosition = new Vector2();

        /// <summary>
        /// 右侧滚动位置
        /// </summary>
        [Name("右侧滚动位置")]
        public Vector2 _rightScrollPosition = new Vector2();

        /// <summary>
        /// 选中数据库颜色
        /// </summary>
        [Name("选中数据库颜色")]
        public Color _selectedDBColor = Color.yellow;

        /// <summary>
        /// 选中表颜色
        /// </summary>
        [Name("选中表颜色")]
        public Color _selectedTableColor = Color.yellow;

        /// <summary>
        /// 连接数据库按钮宽度
        /// </summary>
        [Name("连接数据库按钮宽度")]
        public float _connectDBButtonWidth = 18;

        /// <summary>
        /// 刷新数据库按钮宽度
        /// </summary>
        [Name("刷新数据库按钮宽度")]
        public float _updateDBButtonWidth = 18;

        private Dictionary<DBMB, DBInfo> dbInfos = new Dictionary<DBMB, DBInfo>();

        /// <summary>
        /// 当记录点击后:记录显示的项被点击，参数依次为：被点击的结果集、被点击的记录、记录索引、字段索引、标签对象
        /// </summary>

        public Action<ResultSetClickedEventArgs> onRecordClicked = null;

        /// <summary>
        /// 当字段点击后:字段名显示的项被点击，参数依次为：被点击的结果集、记录索引、字段索引、标签对象
        /// </summary>

        public Action<ResultSetClickedEventArgs> onFieldClicked = null;

        /// <summary>
        /// 字段宽度
        /// </summary>

        public Dictionary<int, float> fieldWidths = new Dictionary<int, float>();

        class DBInfo
        {
            public DBMB db = null;

            public bool expand = false;

            public string selectedTableName = "";
            public ITableAlias tableAlias;

            public ResultSet resultSet;

            public int Count => 1 + db.db.tables.count;
        }

        DBInfo selectedDBInfo = null;

        /// <summary>
        /// 绘制数据库信息
        /// </summary>
        /// <param name="dbManager"></param>
        public void DrawDBInfo(DBManager dbManager)
        {
            if (dbManager == null) return;

            GUILayout.BeginHorizontal();

            #region Left

            GUILayout.BeginVertical(GUI.skin.box, GUILayout.Width(200), GUILayout.ExpandHeight(true));

            GUILayout.Label("数据库列表", GUILayout.Height(0));

            _leftScrollPosition = GUILayout.BeginScrollView(_leftScrollPosition, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

            {
                for (int i = 0; i < dbManager.dbs.Count; ++i)
                {
                    var dbMB = dbManager.dbs[i];
                    DBInfo dbInfo;
                    if (!dbInfos.TryGetValue(dbMB, out dbInfo) || dbInfo == null)
                    {
                        dbInfo = new DBInfo()
                        {
                            db = dbMB,
                        };
                        dbInfos[dbMB] = dbInfo;
                    }
                    var db = dbMB.db;
                    var isConnected = db.IsConnected();
                    var isCurrentDB = selectedDBInfo == dbInfo;

                    var dbName = dbMB.dbDisplayName;
                    var dbAlias = dbAliases?.GetDBAlias(dbName);
                    var dbNameDisplay = dbAlias?._alias ?? dbName;

                    GUILayout.BeginHorizontal();
                    var dbTitle = isCurrentDB ? string.Format("{0}.<color=#{3}>{1} ({2})</color>", (i + 1), dbNameDisplay, isConnected ? "Y" : "N", ColorUtility.ToHtmlStringRGBA(_selectedDBColor)) : string.Format("{0}.{1} ({2})", (i + 1), dbNameDisplay, isConnected ? "Y" : "N");
                    if (GUILayout.Button(dbTitle, CommonGUIStyle.middleLeftBoxRichText))
                    {
                        dbInfo.expand = !dbInfo.expand;
                        if (dbInfo.expand && db.tables.count == 0)
                        {
                            db.TryGetTablesWithTableStructure();
                            isConnected = false;
                        }
                    }
                    if (GUILayout.Button("R", CommonGUIStyle.middleLeftBox, GUILayout.Width(_connectDBButtonWidth)) && !isConnected)
                    {
                        dbMB.enabled = false;
                        dbMB.enabled = true;
                    }
                    if (GUILayout.Button("U", CommonGUIStyle.middleLeftBox, GUILayout.Width(_updateDBButtonWidth)))
                    {
                        db.tables.Clear();
                        db.TryGetTablesWithTableStructure();
                        isConnected = false;
                    }
                    GUILayout.EndHorizontal();

                    if (dbInfo.expand && isConnected)//必须保证数据库处于连接状态
                    {
                        var tables = db.tables.weakItems;
                        if (tables.Count > 0)
                        {
                            CommonFun.BeginLayout(true, false);
                            foreach (var table in tables)
                            {
                                var tableName = table.name;
                                var tableAlias = dbAlias?.GetTableAlias(tableName);
                                var tableNameDisplay = tableAlias?._alias ?? tableName;

                                var tableTitle = isCurrentDB && dbInfo.selectedTableName == tableName ? string.Format("<color=#{1}>{0}</color>", tableNameDisplay, ColorUtility.ToHtmlStringRGBA(_selectedTableColor)) : tableNameDisplay;
                                if (GUILayout.Button(tableTitle, CommonGUIStyle.middleLeftBoxRichText))
                                {
                                    dbInfo.selectedTableName = tableName;
                                    dbInfo.tableAlias = tableAlias;
                                    if (!db.TryExecuteQuery(string.Format("select * from {0}", dbInfo.selectedTableName), (ir, rs) => {
                                        CommonFun.DelayCall(() => dbInfo.resultSet = rs);
                                    }))
                                    {
                                        dbMB.OnError(null);
                                    }
                                    selectedDBInfo = dbInfo;
                                }
                            }
                            CommonFun.EndLayout();
                        }
                    }
                }
            }

            GUILayout.EndScrollView();

            GUILayout.EndVertical();

            #endregion

            #region Right

            GUILayout.BeginVertical(GUI.skin.box, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

            if (selectedDBInfo != null)
            {
                _rightScrollPosition = ResultSetWindowHorizontal.DrawResultSet(_rightScrollPosition, selectedDBInfo.resultSet, onRecordClicked, onFieldClicked, ResultSetWindow.DefaultFieldWidth, fieldWidths, selectedDBInfo.tableAlias);
            }

            GUILayout.EndVertical();

            #endregion

            GUILayout.EndHorizontal();
        }
    }

    /// <summary>
    /// 结果集被点击事件参数
    /// </summary>
    public class ResultSetClickedEventArgs : EventArgs
    {
        /// <summary>
        /// 数据库
        /// </summary>
        public IBaseDB db => resultSet?.db;

        /// <summary>
        /// 结果集
        /// </summary>
        public ResultSet resultSet { get; private set; }

        /// <summary>
        /// 记录
        /// </summary>
        public Record record { get; private set; }

        /// <summary>
        /// 记录索引
        /// </summary>
        public int recordIndex { get; private set; }

        /// <summary>
        /// 字段索引
        /// </summary>
        public int fieldIndex { get; private set; }

        private string _fieldName;

        /// <summary>
        /// 字段名
        /// </summary>
        public string fieldName
        {
            get
            {
                if (string.IsNullOrEmpty(_fieldName))
                {
                    if (fieldIndex >= 0)
                    {
                        if (resultSet != null)
                        {
                            return _fieldName = resultSet.fieldSet[fieldIndex]?.name;
                        }
                        if (record != null)
                        {
                            return _fieldName = record.fieldSet[fieldIndex]?.name;
                        }
                    }
                }
                return _fieldName;
            }
            private set => _fieldName = value;
        }

        private string _fieldDispalyName;

        /// <summary>
        /// 字段显示名
        /// </summary>
        public string fieldDispalyName
        {
            get
            {
                if (string.IsNullOrEmpty(_fieldDispalyName))
                {
                    _fieldDispalyName = fieldName;
                }
                return _fieldDispalyName;
            }
            set => _fieldDispalyName = value;
        }

        /// <summary>
        /// 字段值
        /// </summary>
        public FieldValue fieldValue { get; private set; }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="resultSet"></param>
        /// <param name="recordIndex"></param>
        /// <param name="fieldIndex"></param>
        public ResultSetClickedEventArgs(ResultSet resultSet, int recordIndex, int fieldIndex)
        {
            this.resultSet = resultSet;
            this.recordIndex = recordIndex;
            this.fieldIndex = fieldIndex;
        }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="resultSet"></param>
        /// <param name="recordIndex"></param>
        /// <param name="fieldIndex"></param>
        /// <param name="fieldName"></param>
        /// <param name="fieldDispalyName"></param>
        public ResultSetClickedEventArgs(ResultSet resultSet, int recordIndex, int fieldIndex, string fieldName, string fieldDispalyName) : this(resultSet, recordIndex, fieldIndex)
        {
            this.fieldName = fieldName;
            this.fieldDispalyName = fieldDispalyName;
        }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="resultSet"></param>
        /// <param name="record"></param>
        /// <param name="recordIndex"></param>
        /// <param name="fieldIndex"></param>
        /// <param name="fieldValue"></param>
        public ResultSetClickedEventArgs(ResultSet resultSet, Record record, int recordIndex, int fieldIndex, FieldValue fieldValue) : this(resultSet, recordIndex, fieldIndex)
        {
            this.record = record;
            this.fieldValue = fieldValue;
        }

        /// <summary>
        /// 构造 
        /// </summary>
        /// <param name="resultSet"></param>
        /// <param name="record"></param>
        /// <param name="recordIndex"></param>
        /// <param name="fieldIndex"></param>
        /// <param name="fieldValue"></param>
        /// <param name="fieldName"></param>
        /// <param name="fieldDispalyName"></param>
        public ResultSetClickedEventArgs(ResultSet resultSet, Record record, int recordIndex, int fieldIndex, FieldValue fieldValue, string fieldName, string fieldDispalyName) : this(resultSet, recordIndex, fieldIndex, fieldName, fieldDispalyName)
        {
            this.record = record;
            this.fieldValue = fieldValue;
        }
    }
}
