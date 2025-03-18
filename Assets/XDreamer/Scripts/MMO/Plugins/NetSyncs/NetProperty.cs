using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;
using XCSJ.Tools;
using XCSJ.PluginTools;
using XCSJ.PluginCommonUtils.Tools;

namespace XCSJ.PluginMMO.NetSyncs
{
    /// <summary>
    /// 网络属性
    /// </summary>
    [XCSJ.Attributes.Icon(EIcon.Property)]
    [DisallowMultipleComponent]
    [Name("网络属性")]
    [Tool(MMOHelper.CategoryName, MMOHelper.ToolPurpose, rootType = typeof(MMOManager))]
    public class NetProperty : NetMB
    {
        static NetProperty()
        {
            Converter.instance.RegisterClass<List<Property>>();
        }

        /// <summary>
        /// 属性列表:属性列表中的属性信息在发生数据修改时会进行网络同步
        /// </summary>
        [SyncVar]
        [Name("属性列表")]
        [Tip("属性列表中的属性信息在发生数据修改时会进行网络同步", "The attribute information in the attribute list will be synchronized when data is modified")]
        public List<Property> propertys = new List<Property>();

        /// <summary>
        /// 当有新属性值时回调
        /// </summary>
        public static event Action<NetProperty, Property> onNewProperty;

        /// <summary>
        /// 当属性值变化时回调
        /// </summary>
        public static event Action<NetProperty, Property, string> onValueChanged;

        /// <summary>
        /// 设置属性；无则添加有则覆盖
        /// </summary>
        /// <param name="name">不可为空字符串或null</param>
        /// <param name="value"></param>
        /// <param name="mustSet">强制设置；默认情况下如果新值与旧值相同，不执行设置；当本参数为True时，不论新值与旧值是否相同都会触发修改机制执行网络同步</param>
        /// <returns></returns>
        public Property SetProperty(string name, string value, bool mustSet = false)
        {
            if (string.IsNullOrEmpty(name)) return null;
            var property = GetProperty(name);
            if (property == null)
            {
                property = new Property() { name = name };
                this.XModifyProperty(() =>
                {
                    propertys.Add(property);
                });
                MarkDirty();

                onNewProperty?.Invoke(this, property);
            }
            if (property.value != value || mustSet)
            {
                var oldValue = property.value;
                this.XModifyProperty(() =>
                {
                    property.value = value;
                });
                MarkDirty();

                onValueChanged?.Invoke(this, property, oldValue);
            }            
            return property;
        }

        /// <summary>
        /// 获取属性
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Property GetProperty(string name) => propertys.FirstOrDefault(p => p.name == name);

        /// <summary>
        /// 定时检测修改时回调；可用于检测检测同步变量（即被SyncVarAttribute修饰的变量）或数据是否反生变化；
        /// </summary>
        /// <returns></returns>
        protected override bool OnTimedCheckChange()
        {
            //return base.OnTimedCheckChange();
            return false;
        }
    }

    /// <summary>
    /// 属性
    /// </summary>
    [Serializable]
    [Name("属性")]
    public class Property
    {
        /// <summary>
        /// 名称
        /// </summary>
        [Name("名称")]
        public string name;

        /// <summary>
        /// 值
        /// </summary>
        [Name("值")]
        public string value;
    }
}
