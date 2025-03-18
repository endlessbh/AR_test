using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.DataBase;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.Scripts;

namespace XCSJ.PluginDataBase.Tools
{
    /// <summary>
    /// 结果集窗口通过IMGUI
    /// </summary>
    [Name("结果集窗口通过IMGUI")]
    [Tip("通过IMGUI方式在运行时绘制结果集信息", "Draw result set information at runtime through IMGUI")]
    [RequireManager(typeof(DBManager))]
    [RequireComponent(typeof(DBMB))]
    [Tool(DBHelperExtension.FuncCompoents, nameof(DBMB))]
    [XCSJ.Attributes.Icon(EIcon.Window)]
    [Owner(typeof(DBManager))]
    public class ResultSetWindowByIMGUI : InteractProvider
    {
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
        /// 结果集窗口横向型
        /// </summary>
        [Name("结果集窗口横向型")]
        public ResultSetWindowHorizontal _resultSetWindowHorizontal = new ResultSetWindowHorizontal();

        /// <summary>
        /// 结果集窗口纵向型
        /// </summary>
        [Name("结果集窗口纵向型")]
        public ResultSetWindowVertical _resultSetWindowVertical = new ResultSetWindowVertical();

        /// <summary>
        /// 结果集窗口键值型
        /// </summary>
        [Name("结果集窗口键值型")]
        public ResultSetWindowKV _resultSetWindowKV = new ResultSetWindowKV();

        /// <summary>
        /// 获取结果集窗口
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        public ResultSetWindow GetResultSetWindow(EResultSetWindowMode mode)
        {
            switch (mode)
            {
                case EResultSetWindowMode.Horizontal: return _resultSetWindowHorizontal;
                case EResultSetWindowMode.Vertical: return _resultSetWindowVertical;
                case EResultSetWindowMode.KV: return _resultSetWindowKV;
                default: return null;
            }
        }

        /// <summary>
        /// 设置结果集
        /// </summary>
        /// <param name="resultSet"></param>
        public void SetResultSet(ResultSet resultSet)
        {
            _resultSetWindowHorizontal.resultSet = resultSet;
            _resultSetWindowVertical.resultSet = resultSet;
            _resultSetWindowKV.resultSet = resultSet;
        }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            var dbAliases = this.dbAliasData;
            _resultSetWindowHorizontal.dbAliases = dbAliases;
            _resultSetWindowVertical.dbAliases = dbAliases;
            _resultSetWindowKV.dbAliases = dbAliases;
        }

        /// <summary>
        /// 绘制GUI
        /// </summary>
        public virtual void OnGUI()
        {
            _resultSetWindowHorizontal.OnGUI();
            _resultSetWindowVertical.OnGUI();
            _resultSetWindowKV.OnGUI();
        }

        /// <summary>
        /// 重置
        /// </summary>
        private void Reset()
        {
            if (dbAliasData) { }
        }
    }

    /// <summary>
    /// 结果集窗口模式
    /// </summary>
    [Name("结果集窗口模式")]
    public enum EResultSetWindowMode
    {
        /// <summary>
        /// 横向
        /// </summary>
        [Name("横向")]
        Horizontal = 0,

        /// <summary>
        /// 纵向
        /// </summary>
        [Name("纵向")]
        Vertical,

        /// <summary>
        /// 键值
        /// </summary>
        [Name("键值")]
        KV,
    }

    /// <summary>
    /// 用于结果集窗口模式
    /// </summary>
    [ScriptParamType(ScriptParamType)]
    public class ResultSetWindowMode_ScriptParam : EnumScriptParam<EResultSetWindowMode>
    {
        /// <summary>
        /// 脚本参数类型
        /// </summary>
        public const int ScriptParamType = IDRange.Begin + 1;
    }

    /// <summary>
    /// 结果集窗口
    /// </summary>
    public abstract class ResultSetWindow : BaseGUIWindow
    {
        /// <summary>
        /// 默认字段宽度
        /// </summary>
        public const float DefaultFieldWidth = 80f;

        private float _fieldWidth = DefaultFieldWidth;

        /// <summary>
        /// 字段宽度
        /// </summary>
        public virtual float fieldWidth
        {
            get => _fieldWidth;
            set => _fieldWidth = value;
        }

        /// <summary>
        /// 结果集
        /// </summary>
        public ResultSet resultSet { get; set; } = null;

        /// <summary>
        /// 数组库别名
        /// </summary>
        public IDBAliases dbAliases { get; set; } = null;

        /// <summary>
        /// 记录滚动位置
        /// </summary>
        [Name("记录滚动位置")]
        public Vector2 _recordsScrollPosition = new Vector2();

        /// <summary>
        /// 当记录点击后
        /// </summary>
        public Action<ResultSetClickedEventArgs> onRecordClicked = null;

        /// <summary>
        /// 当字段点击后
        /// </summary>
        public Action<ResultSetClickedEventArgs> onFieldClicked = null;

        /// <summary>
        /// 获取字段宽度
        /// </summary>
        /// <param name="width"></param>
        /// <param name="fieldWidth"></param>
        /// <returns></returns>
        public static float GetFieldWidth(float width, float fieldWidth)
        {
            if (fieldWidth > 0) return fieldWidth;
            fieldWidth = Math.Abs(fieldWidth);

            if (fieldWidth < 1) return fieldWidth * width;

            return fieldWidth;
        }
    }

    /// <summary>
    /// 横向结果集窗口；字段标题在顶部，横向排列；
    /// </summary>
    [Serializable]
    [Name("横向结果集窗口")]
    public class ResultSetWindowHorizontal : ResultSetWindow
    {
        /// <summary>
        /// 字段宽度
        /// </summary>
        public Dictionary<int, float> fieldWidths = new Dictionary<int, float>();

        /// <summary>
        /// 当绘制内容
        /// </summary>
        /// <param name="scrollRect"></param>
        /// <param name="scrollViewRect"></param>
        protected override void OnDrawContent(Rect scrollRect, Rect scrollViewRect)
        {
            _recordsScrollPosition = DrawResultSet(_recordsScrollPosition, resultSet, onRecordClicked, onFieldClicked, fieldWidth, fieldWidths, dbAliases);
        }

        /// <summary>
        /// 绘制结果集
        /// </summary>
        /// <param name="recordsScrollPosition"></param>
        /// <param name="resultSet"></param>
        /// <param name="onRecordClicked"></param>
        /// <param name="onFieldClicked"></param>
        /// <param name="fieldWidth"></param>
        /// <param name="fieldWidths"></param>
        /// <returns></returns>
        public static Vector2 DrawResultSet(Vector2 recordsScrollPosition, ResultSet resultSet, Action<ResultSetClickedEventArgs> onRecordClicked = null, Action<ResultSetClickedEventArgs> onFieldClicked = null, float fieldWidth = DefaultFieldWidth, Dictionary<int, float> fieldWidths = null, ITableAlias tableAlias = null)
        {
            if (resultSet == null) return recordsScrollPosition;
            //Debug.Log(JsonHelper.ToJson(resultSet));

            try
            {
                GUILayout.BeginVertical(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

                #region Head
                if (fieldWidths == null) fieldWidths = new Dictionary<int, float>();
                GUILayout.BeginScrollView(new Vector2(recordsScrollPosition.x, 0), false, false, GUIStyle.none, GUIStyle.none, GUILayout.ExpandWidth(true), GUILayout.Height(32));
                GUILayout.BeginHorizontal();
                for (int i = 0; i < resultSet.fieldSet.fields.Count; i++)
                {
                    if (!fieldWidths.ContainsKey(i)) fieldWidths[i] = fieldWidth;

                    var field = resultSet.fieldSet.fields[i].name;
                    var fieldDisplay = tableAlias?.GetFieldAlias(field)?._alias ?? field;
                    if (GUILayout.Button(CommonFun.TempContent(fieldDisplay, field), GUI.skin.box, GUILayout.Width(fieldWidths[i])))
                    {
                        onFieldClicked?.Invoke(new ResultSetClickedEventArgs(resultSet, -1, i, field, fieldDisplay));
                    }
                }
                GUILayout.EndHorizontal();
                GUILayout.EndScrollView();

                #endregion

                GUILayout.Space(2);
                //GUILayout.Box("", GUILayout.Height(2), GUILayout.ExpandWidth(true));

                #region Body

                try
                {
                    recordsScrollPosition = GUILayout.BeginScrollView(recordsScrollPosition, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

                    for (int i = 0; i < resultSet.recordSet.records.Count; i++)
                    {
                        var record = resultSet.recordSet.records[i];
                        GUILayout.BeginHorizontal();
                        for (int j = 0; j < record.fieldValues.Count; j++)
                        {
                            var fieldValue = record[j];
                            var str = fieldValue.Value<string>();

                            if (!fieldWidths.TryGetValue(j, out var fw)) fw = fieldWidth;

                            if (GUILayout.Button(new GUIContent(str.Substring(0, str.Length > 64 ? 64 : str.Length), str), CommonGUIStyle.middleLeftBox, GUILayout.Width(fw)))
                            {
                                onRecordClicked?.Invoke(new ResultSetClickedEventArgs(resultSet, record, i, j, fieldValue));
                            }
                        }
                        GUILayout.EndHorizontal();
                    }
                }
                finally
                {
                    GUILayout.EndScrollView();
                }

                #endregion
            }
            finally
            {
                GUILayout.EndVertical();
            }

            return recordsScrollPosition;
        }
    }

    /// <summary>
    /// 纵向结果集窗口；字段标题在左侧，纵向排列；
    /// </summary>
    [Name("纵向结果集窗口")]
    [Serializable]
    public class ResultSetWindowVertical : ResultSetWindow
    {
        /// <summary>
        /// 当绘制内容
        /// </summary>
        /// <param name="scrollRect"></param>
        /// <param name="scrollViewRect"></param>
        protected override void OnDrawContent(Rect scrollRect, Rect scrollViewRect)
        {
            _recordsScrollPosition = DrawResultSet(_recordsScrollPosition, scrollViewRect, resultSet, onRecordClicked, onFieldClicked, fieldWidth);
        }

        /// <summary>
        /// 绘制结果集
        /// </summary>
        /// <param name="recordsScrollPosition"></param>
        /// <param name="viewRect"></param>
        /// <param name="resultSet"></param>
        /// <param name="onRecordClicked"></param>
        /// <param name="onFieldClicked"></param>
        /// <param name="fieldWidth"></param>
        /// <returns></returns>
        public static Vector2 DrawResultSet(Vector2 recordsScrollPosition, Rect viewRect, ResultSet resultSet, Action<ResultSetClickedEventArgs> onRecordClicked = null, Action<ResultSetClickedEventArgs> onFieldClicked = null, float fieldWidth = DefaultFieldWidth, ITableAlias tableAlias = null)
        {
            if (resultSet == null) return recordsScrollPosition;
            //Debug.Log(JsonHelper.ToJson(resultSet));

            try
            {
                GUILayout.BeginVertical(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                GUILayout.BeginHorizontal();

                #region Head

                fieldWidth = GetFieldWidth(viewRect.width, fieldWidth);
                var recordWidth = viewRect.width - fieldWidth - 2 - 4;

                GUILayout.BeginVertical();
                GUILayout.BeginScrollView(new Vector2(0, recordsScrollPosition.y), false, false, GUIStyle.none, GUIStyle.none, GUILayout.Width(fieldWidth), GUILayout.ExpandHeight(true));
                for (int i = 0; i < resultSet.fieldSet.fields.Count; i++)
                {
                    var field = resultSet.fieldSet.fields[i].name;
                    var fieldDisplay = tableAlias?.GetFieldAlias(field)?._alias ?? field;
                    if (GUILayout.Button(CommonFun.TempContent(fieldDisplay, field), GUI.skin.box, GUILayout.Width(fieldWidth - 4)))
                    {
                        onFieldClicked?.Invoke(new ResultSetClickedEventArgs(resultSet, -1, i, field, fieldDisplay));
                    }
                }
                GUILayout.EndScrollView();
                GUILayout.EndVertical();

                #endregion

                GUILayout.Space(2);
                //GUILayout.Box("", GUILayout.Width(2), GUILayout.ExpandHeight(true));

                #region Body

                try
                {
                    GUILayout.BeginVertical();
                    recordsScrollPosition = GUILayout.BeginScrollView(recordsScrollPosition, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

                    for (int j = 0; j < resultSet.fieldSet.count; j++)
                    {
                        GUILayout.BeginHorizontal();


                        for (int i = 0; i < resultSet.recordSet.records.Count; i++)
                        {
                            var record = resultSet.recordSet.records[i];

                            var fieldValue = record[j];
                            var str = fieldValue.Value<string>();
                            if (GUILayout.Button(new GUIContent(str.Substring(0, str.Length > 64 ? 64 : str.Length), str), CommonGUIStyle.middleLeftBox, GUILayout.Width(recordWidth)))
                            {
                                onRecordClicked?.Invoke(new ResultSetClickedEventArgs(resultSet, record, i, j, fieldValue));
                            }
                        }

                        GUILayout.EndHorizontal();
                    }
                }
                finally
                {
                    GUILayout.EndScrollView();
                    GUILayout.EndVertical();
                }

                #endregion
            }
            finally
            {
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
            }

            return recordsScrollPosition;
        }
    }

    /// <summary>
    /// KV型结果集窗口
    /// </summary>
    [Serializable]
    [Name("KV型结果集窗口")]
    public class ResultSetWindowKV : ResultSetWindow
    {
        /// <summary>
        /// 当绘制内容
        /// </summary>
        /// <param name="scrollRect"></param>
        /// <param name="scrollViewRect"></param>
        protected override void OnDrawContent(Rect scrollRect, Rect scrollViewRect)
        {
            _recordsScrollPosition = DrawResultSet(_recordsScrollPosition, scrollViewRect, resultSet, onRecordClicked, onFieldClicked, fieldWidth);
        }

        private static void DrawResultSet(float keyWidth, float valueWidth, ResultSet resultSet, Record record, int recordIndex, Action<ResultSetClickedEventArgs> onRecordClicked = null, Action<ResultSetClickedEventArgs> onFieldClicked = null, ITableAlias tableAlias = null)
        {
            for (int i = 0; i < record.fieldValues.Count; i++)
            {
                var field = record.fieldSet.fields[i].name;
                var fieldDisplay = tableAlias?.GetFieldAlias(field)?._alias ?? field;

                GUILayout.BeginHorizontal();
                if (GUILayout.Button(CommonFun.TempContent(fieldDisplay, field), GUI.skin.box, GUILayout.Width(keyWidth)))
                {
                    onFieldClicked?.Invoke(new ResultSetClickedEventArgs(resultSet, recordIndex, i, field, fieldDisplay));
                }

                var fieldValue = record[i];
                var str = fieldValue.Value<string>();
                if (GUILayout.Button(new GUIContent(str.Substring(0, str.Length > 64 ? 64 : str.Length), str), CommonGUIStyle.middleLeftBox, valueWidth > 0 ? GUILayout.Width(valueWidth) : GUILayout.ExpandWidth(true)))
                {
                    onRecordClicked?.Invoke(new ResultSetClickedEventArgs(resultSet, record, recordIndex, i, fieldValue, field, fieldDisplay));
                }
                GUILayout.EndHorizontal();
            }
        }

        /// <summary>
        /// 绘制结果集
        /// </summary>
        /// <param name="recordsScrollPosition"></param>
        /// <param name="viewRect"></param>
        /// <param name="resultSet"></param>
        /// <param name="onRecordClicked"></param>
        /// <param name="onFieldClicked"></param>
        /// <param name="fieldWidth"></param>
        /// <returns></returns>
        public static Vector2 DrawResultSet(Vector2 recordsScrollPosition, Rect viewRect, ResultSet resultSet, Action<ResultSetClickedEventArgs> onRecordClicked = null, Action<ResultSetClickedEventArgs> onFieldClicked = null, float fieldWidth = DefaultFieldWidth, ITableAlias tableAlias = null)
        {
            if (resultSet == null) return recordsScrollPosition;
            //Debug.Log(Helper.JsonHelper.ToJson(resultSet));

            try
            {
                GUILayout.BeginVertical(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                recordsScrollPosition = GUILayout.BeginScrollView(recordsScrollPosition, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

                bool needIndent = resultSet.recordSet.records.Count != 1;
                var indent = 8;

                var width = viewRect.width - indent;
                var keyWidth = GetFieldWidth(width, fieldWidth);
                var valueWidth = width - keyWidth;

                for (int i = 0; i < resultSet.recordSet.records.Count; i++)
                {
                    try
                    {
                        if (needIndent) GUILayout.Button((i + 1).ToString(), GUI.skin.box, GUILayout.ExpandWidth(true));

                        CommonFun.BeginLayout(needIndent, false);

                        var record = resultSet.recordSet.records[i];
                        DrawResultSet(keyWidth, valueWidth, resultSet, record, i, onRecordClicked, onFieldClicked, tableAlias);
                    }
                    finally
                    {
                        CommonFun.EndLayout();
                    }
                }
            }
            finally
            {
                GUILayout.EndScrollView();
                GUILayout.EndVertical();
            }
            return recordsScrollPosition;
        }
    }
}
