using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginTools.Items;
using XCSJ.PluginTools.PropertyDatas;
using XCSJ.PluginXGUI.Base;

namespace XCSJ.PluginXGUI.Windows.Tables
{
    /// <summary>
    /// 表数据提供器
    /// </summary>
    [Name("表数据提供器")]
    [XCSJ.Attributes.Icon(EIcon.Property)]
    [Tool(XGUICategory.Data, nameof(XGUIManager))]
    public sealed class TableDataProvider : InteractTagProvider
    {
        /// <summary>
        /// 组件表数据模型
        /// </summary>
        [Name("组件表数据模型")]
        public ComponentTableData_Model _componentTableData_Model = new ComponentTableData_Model();

        /// <summary>
        /// 添加模型到表中
        /// </summary>
        public bool needAddModel { get; set; } = true;

        /// <summary>
        /// 表名
        /// </summary>
        public string tableName
        {
            get => _tagProperty.GetFirstValue(Table.TableName);
            set => _tagProperty.SetFirstValue(Table.TableName, value);
        }

        /// <summary>
        /// 重置命令
        /// </summary>
        public void Reset()
        {
            var table = ComponentCache.GetComponent<Table>();
            if (table != null) 
            {
                _tagProperty._tagPropertyDatas.Add(new TagPropertyData(Table.TableName, table._tagProperty.firstValue));
            }
            else
            {
                _tagProperty._tagPropertyDatas.Add(new TagPropertyData(Table.TableName));
            }

            _componentTableData_Model._title = name;
            _componentTableData_Model._unityObject = GetComponent<Grabbable>();
        }

        /// <summary>
        /// 开始
        /// </summary>
        private void Start()
        {
            if (needAddModel)
            {
                needAddModel = false;

                CommonFun.DelayCall(() => AddModel());
            }
        }

        private void AddModel()
        {
            if (!_componentTableData_Model.valid) return;

            foreach (var table in ComponentCache.GetComponents<Table>(false))
            {
                if (table.tableName == tableName)
                {
                    table.AddData(_componentTableData_Model);
                }
            }
        }

        private void RemoveModel() 
        {
            if (!_componentTableData_Model.valid) return;

            foreach (var table in ComponentCache.GetComponents<Table>(false))
            {
                if (table.tableName == tableName)
                {
                    table.RemoveData(_componentTableData_Model);
                }
            }
        }
    }
}
