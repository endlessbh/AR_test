using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.DataBase;
using XCSJ.Extension.Base.Attributes;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginDataBase.Tools;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Base;
using XCSJ.Scripts;

namespace XCSJ.PluginDataBase.States
{
    /// <summary>
    /// 数据库列表窗口事件：捕获数据库列表窗口的各种事件
    /// </summary>
    [Name(Title)]
    [Tip("捕获数据库列表窗口的各种事件", "Capture various events in the database list window")]
    [ComponentMenu(DBHelper.Title + "/" + Title, typeof(DBManager))]
    [DisallowMultipleComponent]
    [XCSJ.Attributes.Icon(EIcon.Event)]
    [Owner(typeof(DBManager))]
    public class DBListWindowEvent : Trigger<DBListWindowEvent>, ISerializationCallbackReceiver
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "数据库列表窗口事件";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(DBHelper.Title, typeof(DBManager))]
        [StateComponentMenu(DBHelper.Title + "/" + Title, typeof(DBManager))]
        [Name(Title, nameof(DBListWindowEvent))]
        [Tip("捕获数据库列表窗口的各种事件", "Capture various events in the database list window")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 数据库列表窗口
        /// </summary>
        [Name("数据库列表窗口")]
        [ComponentPopup]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public DBListWindowByIMGUI _dbListWindow;

        /// <summary>
        /// 数据库列表窗口
        /// </summary>
        public DBListWindowByIMGUI dbListWindow => this.XGetComponentInGlobal(ref _dbListWindow);

        private DBListWindowByIMGUI dbListWindowTmp;

        /// <summary>
        /// 窗口事件
        /// </summary>
        [Name("窗口事件")]
        public enum EWindowEvent
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None,

            /// <summary>
            /// 字段被点击
            /// </summary>
            [Name("字段被点击")]
            FieldClicked,

            /// <summary>
            /// 记录被点击
            /// </summary>
            [Name("记录被点击")]
            RecordClicked,
        }

        /// <summary>
        /// 窗口事件
        /// </summary>
        [Name("窗口事件")]
        [EnumPopup]
        public EWindowEvent _windowEvent = EWindowEvent.FieldClicked;

        /// <summary>
        /// 数据库索引变量
        /// </summary>
        [Name("数据库索引变量")]
        [VarString]
        public string _dbIndexVariableName = "";

        /// <summary>
        /// 记录索引变量
        /// </summary>
        [Name("记录索引变量")]
        [VarString]
        public string _recordIndexVariableName = "";

        /// <summary>
        /// 字段索引变量
        /// </summary>
        [Name("字段索引变量")]
        [VarString]
        public string _fieldIndexVariableName = "";

        /// <summary>
        /// 字段名变量
        /// </summary>
        [Name("字段名变量")]
        [VarString]
        public string _fieldNameVariableName = "";

        /// <summary>
        /// 字段显示名名变量
        /// </summary>
        [Name("字段显示名名变量")]
        [VarString]
        public string _fieldDisplayNameVariableName = "";

        /// <summary>
        /// 字段值变量
        /// </summary>
        [Name("字段值变量")]
        [VarString]
        public string _fieldValueVariableName = "";

        #region ISerializationCallbackReceiver

        void ISerializationCallbackReceiver.OnBeforeSerialize() { }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            CommonFun.VarNameToVarString(ref _dbIndexVariableName);
            CommonFun.VarNameToVarString(ref _recordIndexVariableName);
            CommonFun.VarNameToVarString(ref _fieldIndexVariableName);
            CommonFun.VarNameToVarString(ref _fieldNameVariableName);
            CommonFun.VarNameToVarString(ref _fieldDisplayNameVariableName);
            CommonFun.VarNameToVarString(ref _fieldValueVariableName);
        }

        #endregion

        /// <summary>
        /// 当进入
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnEntry(StateData stateData)
        {
            base.OnEntry(stateData);
            dbListWindowTmp = this.dbListWindow;
            if (dbListWindowTmp)
            {
                dbListWindowTmp._dbListWindow.onFieldClicked += OnFieldClicked;
                dbListWindowTmp._dbListWindow.onRecordClicked += OnRecordClicked;
            }
        }

        /// <summary>
        /// 当退出
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnExit(StateData stateData)
        {
            base.OnExit(stateData);
            if (dbListWindowTmp)
            {
                dbListWindowTmp._dbListWindow.onFieldClicked -= OnFieldClicked;
                dbListWindowTmp._dbListWindow.onRecordClicked -= OnRecordClicked;
            }
        }

        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity()
        {
            if (!_dbListWindow) return false;
            return base.DataValidity();
        }

        /// <summary>
        /// 转友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString()
        {
            return CommonFun.Name(_windowEvent);
            //return base.ToFriendlyString();
        }

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            if (dbListWindow) { }
        }

        private void OnFieldClicked(ResultSetClickedEventArgs eventArgs)
        {
            if (_windowEvent != EWindowEvent.FieldClicked) return;
            try
            {
                finished = true;
                SetVariable(eventArgs);
            }
            catch (Exception ex)
            {
                Log.Exception(nameof(OnFieldClicked) + "执行输出时异常:" + ex);
            }
        }

        private void OnRecordClicked(ResultSetClickedEventArgs eventArgs)
        {
            if (_windowEvent != EWindowEvent.RecordClicked) return;
            try
            {
                finished = true;
                SetVariable(eventArgs);
            }
            catch (Exception ex)
            {
                Log.Exception(nameof(OnRecordClicked) + "执行输出时异常:" + ex);
            }
        }

        private void SetVariable(ResultSetClickedEventArgs eventArgs)
        {
            var manager = ScriptManager.instance;
            if (!manager) return;
            manager.TrySetOrAddSetHierarchyVarValue(_dbIndexVariableName, DBManager.instance.GetDBCNScriptIndex(eventArgs.db).ToString());
            manager.TrySetOrAddSetHierarchyVarValue(_recordIndexVariableName, (eventArgs.recordIndex + 1).ToString());
            manager.TrySetOrAddSetHierarchyVarValue(_fieldIndexVariableName, (eventArgs.fieldIndex + 1).ToString());
            manager.TrySetOrAddSetHierarchyVarValue(_fieldNameVariableName, eventArgs.fieldName);
            manager.TrySetOrAddSetHierarchyVarValue(_fieldDisplayNameVariableName, eventArgs.fieldDispalyName);
            manager.TrySetOrAddSetHierarchyVarValue(_fieldValueVariableName, eventArgs.fieldValue?.Value<string>() ?? "");
        }
    }
}
