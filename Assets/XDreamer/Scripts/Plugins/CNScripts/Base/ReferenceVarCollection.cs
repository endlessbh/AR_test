using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Caches;
using XCSJ.Collections;
using XCSJ.Extension.Base.Attributes;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.Extension.Base.Dataflows.Binders;
using XCSJ.Extension.Base.Dataflows.DataBinders;
using XCSJ.Helper;
using XCSJ.LitJson;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.CNScripts;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.Scripts;

namespace XCSJ.Extension.CNScripts.Base
{
    /// <summary>
    /// 引用变量集合
    /// </summary>
    [Name(Title)]
    [Tip("用于引用当前场景中某些特定对象的变量,可直接获取或设置引用对象公有字段、属性、空参数方法(void类型返回空字符串)的值；与全局变量的作用域完全相同；", "Variables used to reference certain specific objects in the current scene. You can directly obtain or set the values of the public fields, attributes, and null parameter methods of the reference object (void type returns an empty string); The scope is the same as that of global variables;")]
    [Tool(CNScriptHelper.Title, nameof(ScriptManager))]
    [RequireComponent(typeof(ScriptManager))]
    [DisallowMultipleComponent]
    [ExecuteInEditMode]
    public class ReferenceVarCollection : BaseVarCollection, ITypeBinderGetter
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "引用变量集合";

        #region Unity方法重写

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            _varCollection.OnEnable();
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            _varCollection.OnDisable();
        }

        private void Awake()
        {
            _varCollection.OnInit();
        }

        private void OnDestroy()
        {
            _varCollection.OnRelease();
        }

        #endregion

        #region IVarCollectionHost

        /// <summary>
        /// 引用变量集合
        /// </summary>
        [Name("引用变量集合")]
        [Tip("", "")]
        public ReferenceVarCollectionData _varCollection = new ReferenceVarCollectionData();

        /// <summary>
        /// 变量集合
        /// </summary>
        public override IVarCollection varCollection => _varCollection;

        #endregion

        #region ISerializationCallbackReceiver

        /// <summary>
        /// 当反序列化之后回调
        /// </summary>
        public override void OnAfterDeserialize()
        {
            base.OnAfterDeserialize();
            _varCollection.SetVarCollectionHost(this);
            _varCollection.ListToDictionary();
        }

        #endregion

        #region

        /// <summary>
        /// 类型绑定器拥有者
        /// </summary>
        public UnityEngine.Object owner => this;

        /// <summary>
        /// 获取类型绑定器
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ITypeBinder> GetTypeBinders()
        {
            return _varCollection._variables.Cast(var => var._value._fieldPropertyMethodBinderValue);
        }

        #endregion
    }

    /// <summary>
    /// 引用变量集合数据
    /// </summary>
    [Serializable]
    [Name("引用变量集合数据")]
    public class ReferenceVarCollectionData : VarCollection<ReferenceVariable, ReferenceVarDictionary>
    {
        /// <summary>
        /// 变量作用域
        /// </summary>
        public override EVarScope varScope => EVarScope.Reference;

        /// <summary>
        /// 变量字典
        /// </summary>
        public override ReferenceVarDictionary varDictionary { get; } = new ReferenceVarDictionary();
    }

    #region 引用变量

    /// <summary>
    /// 引用变量
    /// </summary>
    [Serializable]
    public class ReferenceVariable : Variable<ReferenceVariable, ReferenceHierarchyVar>
    {
        /// <summary>
        /// 值
        /// </summary>
        public Argument _value = new Argument();

        /// <summary>
        /// 获取值
        /// </summary>
        /// <returns></returns>
        public override object GetValue() => _value.GetValue();

        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="value">新变量值</param>
        /// <returns>变量值:如成功设置返回新变量值，否则返回null;</returns>
        public override object SetValue(object value)
        {
            try
            {
                return _value.SetValue(value);
            }
            finally
            {
                MarkDirty();
            }
        }

        /// <summary>
        /// 转层级变量
        /// </summary>
        /// <returns></returns>
        public override ReferenceHierarchyVar ToHierarchyVar() => new ReferenceHierarchyVar(this);
    }

    #endregion

    #region 引用变量字典

    /// <summary>
    /// 引用变量字典
    /// </summary>
    public class ReferenceVarDictionary : VarDictionary<ReferenceVarDictionary, ReferenceVariable>
    {
        /// <summary>
        /// 创建变量
        /// </summary>
        /// <param name="varType"></param>
        /// <param name="varName"></param>
        /// <param name="varValue"></param>
        /// <returns></returns>
        protected override ReferenceVariable CreateVariable(EVarType varType, string varName, object varValue)
        {
            var referenceVariable = new ReferenceVariable
            {
                name = varName,
            };
            referenceVariable.SetValue(varValue);
            referenceVariable.SetVarCollectionHost(varCollectionHost);
            return referenceVariable;
        }
    }

    #endregion

    #region 引用层级变量

    /// <summary>
    /// 引用层级变量
    /// </summary>
    public class ReferenceHierarchyVar : IHierarchyVar
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="parent"></param>
        private ReferenceHierarchyVar(ReferenceHierarchyVar parent)
        {
            this.parent = parent;
            varCollection = parent.varCollection;
        }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="referenceVariable"></param>
        internal ReferenceHierarchyVar(ReferenceVariable referenceVariable)
        {
            _variable = referenceVariable;
            varCollection = referenceVariable.varCollection;
            name = referenceVariable.name;
        }

        /// <summary>
        /// 原始值
        /// </summary>
        [Json(false)]
        public object originalValue
        {
            get
            {
                if (parent == null) return _variable?.GetValue();
                try
                {
                    var parents = this.parents;
                    var result = default(object);
                    for (int i = 0; i < parents.Length; i++)
                    {
                        var referenceHierarchyVar = parents[i] as ReferenceHierarchyVar;
                        if (i == 0)
                        {
                            result = referenceHierarchyVar.originalValue;
                        }
                        else if (result != null && DataBinderHelper.TryGetValue(result.GetType(), result, referenceHierarchyVar.name, out var tmpValue, default))
                        {
                            result = tmpValue;
                        }
                        else
                        {
                            return "";
                        }
                    }
                    return result;
                }
                catch (Exception ex) { Debug.LogException(ex); }
                return "";
            }
        }

        Type _originalValueType;

        /// <summary>
        /// 原始值类型
        /// </summary>
        [Json(false)]
        public Type originalValueType => _originalValueType ?? (_originalValueType = originalValue?.GetType());

        IVariable _variable;

        /// <summary>
        /// 变量
        /// </summary>
        [Json(false)]
        public IVariable variable => this.GetRootVar()?._variable;

        /// <summary>
        /// 引用变量
        /// </summary>
        [Json(false)]
        public ReferenceVariable referenceVariable => variable as ReferenceVariable;

        private void CallHierarchyVarChanged()
        {
            variable?.OnHierarchyVarChanged();
            HierarchyVarEvent.CallChangedEvent(this);
        }

        /// <summary>
        /// 变量集合
        /// </summary>
        protected IVarCollection _varCollection = null;

        /// <summary>
        /// 变量集合
        /// </summary>
        [Json(false)]
        public IVarCollection varCollection
        {
            get => _varCollection;
            set => Foreach((p, i, k, c) => c._varCollection = value);
        }

        /// <summary>
        /// 名称：字段、属性、空形参方法的名称
        /// </summary>
        public string name { get; private set; }

        /// <summary>
        /// 变量作用域
        /// </summary>
        [Json(false)]
        public EVarScope varScope { get; private set; } = EVarScope.Reference;

        /// <summary>
        /// 变量类型
        /// </summary>
        [Json(false)]
        public EVarType varType { get; private set; } = EVarType.String;

        /// <summary>
        /// 变量字符串
        /// </summary>
        protected string _varString;

        /// <summary>
        /// 变量字符串
        /// </summary>
        public string varString => _varString ?? (_varString = this.GetFormatVarString(false, EVarHierarchyDelimiter.Dot));

        /// <summary>
        /// 变量层级字符串
        /// </summary>
        protected string _varHierarchyString;

        /// <summary>
        /// 变量层级字符串
        /// </summary>
        [Json(false)]
        public string varHierarchyString => _varHierarchyString ?? (_varHierarchyString = this.GetFormatVarString(true, EVarHierarchyDelimiter.Slash));

        /// <summary>
        /// 字符串值
        /// </summary>
        public string stringValue => originalValue.ToScriptParamString();

        /// <summary>
        /// 父级
        /// </summary>
        [Json(false)]
        public IHierarchyVar parent { get; private set; }

        private IHierarchyVar[] _parents;

        /// <summary>
        /// 父级列表：由根级到当前级的顺序
        /// </summary>
        public IHierarchyVar[] parents => _parents ?? (_parents = this.GetParents()?.ToArray());

        /// <summary>
        /// 是否有效
        /// </summary>
        public bool isValid => varScope != EVarScope.Invalid;

        /// <summary>
        /// 是数组元素
        /// </summary>
        public bool isArrayElement => false;

        /// <summary>
        /// 遍历:会遍历子级以及子级的子级；
        /// </summary>
        /// <param name="action">动作，参数依次为：父级对象（可能为null）、当前对象在父级对象中的索引、当前对象在父级对象中的键值（父级对象是Json对象时有效）、当前对象(不可能为null)</param>
        void IHierarchyVar.Foreach(Action<IHierarchyVar, int, string, IHierarchyVar> action)
        {
            if (action == null) return;
            Foreach((p, i, key, c) => action(p, i, key, c));
        }

        /// <summary>
        /// 遍历
        /// </summary>
        /// <param name="action">动作，参数依次为：父级对象（可能为null）、当前对象在父级对象中的索引、当前对象在父级对象中的键值（父级对象是Json对象时有效）、当前对象(不可能为null)</param>
        public void Foreach(Action<ReferenceHierarchyVar, int, string, ReferenceHierarchyVar> action)
        {
            if (action == null) return;
            Foreach(null, -1, null, action);
        }

        private void Foreach(ReferenceHierarchyVar parent, int index, string key, Action<ReferenceHierarchyVar, int, string, ReferenceHierarchyVar> action)
        {
            action(parent, index, key, this);
            foreach (var kv in dic)
            {
                kv.Value.Foreach(this, -1, kv.Key, action);
            }
        }

        /// <summary>
        /// 设置无效
        /// </summary>
        public void SetInvalid()
        {
            Foreach((parent, index, key, current) => current?.InternalSetInvalid());
            HierarchyVarEvent.CallInvalidEvent(this);
        }

        private void InternalSetInvalid()
        {
            varScope = EVarScope.Invalid;
            InternalMarkDirty();
        }

        private void InternalMarkDirty()
        {
            _parents = null;
            _varString = null;
            _varHierarchyString = null;
        }

        /// <summary>
        /// 尝试添加子级
        /// </summary>
        /// <param name="varValue"></param>
        /// <param name="programIndex"></param>
        /// <returns></returns>
        public bool TryAddChild(string varValue, out int programIndex)
        {
            programIndex = default;
            return false;
        }

        /// <summary>
        /// 尝试批量添加子级
        /// </summary>
        /// <param name="varValues"></param>
        /// <returns></returns>
        public bool TryAddChildren(IEnumerable<string> varValues)
        {
            return false;
        }

        /// <summary>
        /// 尝试清理全部子级
        /// </summary>
        /// <returns></returns>
        public bool TryClearChildren()
        {
            ClearDictionary();
            return true;
        }

        /// <summary>
        /// 尝试获取子级
        /// </summary>
        /// <param name="index"></param>
        /// <param name="hierarchyIndexOrProgramIndex"></param>
        /// <param name="hierarchyVar"></param>
        /// <returns></returns>
        public bool TryGetChild(int index, bool hierarchyIndexOrProgramIndex, out IHierarchyVar hierarchyVar)
        {
            hierarchyVar = default;
            return false;
        }

        private void ClearDictionary()
        {
            var list = dic.Values.ToList();
            dic.Clear();
            list.ForEach(v => v.SetInvalid());
        }

        Dictionary<string, ReferenceHierarchyVar> dic = new Dictionary<string, ReferenceHierarchyVar>();

        /// <summary>
        /// 尝试获取子级
        /// </summary>
        /// <param name="key"></param>
        /// <param name="hierarchyVar"></param>
        /// <returns></returns>
        public bool TryGetChild(string key, out IHierarchyVar hierarchyVar)
        {
            if (string.IsNullOrEmpty(key))
            {
                hierarchyVar = default;
                return false;
            }
            if (dic.TryGetValue(key, out var hv))
            {
                hierarchyVar = hv;
                return true;
            }
            var v = TypeMemberCache.Get(originalValueType, key);
            if (v != null && v.memberInfo != null)
            {
                var referenceHierarchyVar = new ReferenceHierarchyVar(this)
                {
                    _originalValueType = v.memberValueType,
                    name = key,
                };
                hierarchyVar = referenceHierarchyVar;
                dic[key] = referenceHierarchyVar;
                return true;
            }
            hierarchyVar = default;
            return false;
        }

        /// <summary>
        /// 尝试获取子级
        /// </summary>
        /// <param name="matchFunc"></param>
        /// <param name="hierarchyVar"></param>
        /// <param name="programIndex"></param>
        /// <returns></returns>
        public bool TryGetChild(Func<int, IHierarchyVar, bool> matchFunc, out IHierarchyVar hierarchyVar, out int programIndex)
        {
            hierarchyVar = default;
            programIndex = default;
            return false;
        }

        /// <summary>
        /// 尝试获取子级
        /// </summary>
        /// <param name="matchFunc"></param>
        /// <param name="hierarchyVar"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool TryGetChild(Func<string, IHierarchyVar, bool> matchFunc, out IHierarchyVar hierarchyVar, out string key)
        {
            if (matchFunc != null)
            {
                foreach (var kv in dic)
                {
                    if (matchFunc(kv.Key, kv.Value))
                    {
                        hierarchyVar = kv.Value;
                        key = kv.Key;
                        return true;
                    }
                }
            }
            hierarchyVar = default;
            key = default;
            return false;
        }

        /// <summary>
        /// 尝试获取子级数量
        /// </summary>
        /// <param name="childCount"></param>
        /// <returns></returns>
        public bool TryGetChildCount(out int childCount)
        {
            childCount = dic.Count;
            return true;
        }

        /// <summary>
        /// 尝试获取或添加设置子级
        /// </summary>
        /// <param name="index"></param>
        /// <param name="hierarchyIndexOrProgramIndex"></param>
        /// <param name="value"></param>
        /// <param name="varType"></param>
        /// <param name="hierarchyVar"></param>
        /// <returns></returns>
        public bool TryGetOrAddSetChild(int index, bool hierarchyIndexOrProgramIndex, string value, EVarType varType, out IHierarchyVar hierarchyVar)
        {
            hierarchyVar = default;
            return false;
        }

        /// <summary>
        /// 尝试获取或添加设置子级
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="varType"></param>
        /// <param name="hierarchyVar"></param>
        /// <returns></returns>
        public bool TryGetOrAddSetChild(string key, string value, EVarType varType, out IHierarchyVar hierarchyVar)
        {
            if (string.IsNullOrEmpty(key))
            {
                hierarchyVar = default;
                return false;
            }

            if (TryGetChild(key, out hierarchyVar)) return true;

            hierarchyVar = default;
            return false;
        }

        /// <summary>
        /// 尝试移除子级
        /// </summary>
        /// <param name="key"></param>
        /// <param name="hierarchyVar"></param>
        /// <returns></returns>
        public bool TryRemoveChild(string key, out IHierarchyVar hierarchyVar)
        {
            hierarchyVar = default;
            return false;
        }

        /// <summary>
        /// 尝试移除子级
        /// </summary>
        /// <param name="index"></param>
        /// <param name="hierarchyIndexOrProgramIndex"></param>
        /// <param name="hierarchyVar"></param>
        /// <returns></returns>
        public bool TryRemoveChild(int index, bool hierarchyIndexOrProgramIndex, out IHierarchyVar hierarchyVar)
        {
            hierarchyVar = default;
            return false;
        }

        /// <summary>
        /// 尝试设置子级数量
        /// </summary>
        /// <param name="childCount"></param>
        /// <returns></returns>
        public bool TrySetChildCount(int childCount)
        {
            return false;
        }

        /// <summary>
        /// 尝试设置值
        /// </summary>
        /// <param name="value"></param>
        /// <param name="varType"></param>
        /// <returns></returns>
        public bool TrySetValue(object value, EVarType varType)
        {
            if (parent == null)//对根层级变量赋值
            {
                //能成功转为根原始值类型类型
                if (Converter.instance.TryConvertTo(value, originalValueType, out var rootValue) && rootValue != null)
                {
                    return _variable?.SetValue(rootValue) != null;
                }
                else//无法转为根原始值类型类型
                {
                    TryClearChildren();
                    return _variable?.SetValue(value) != null;
                }
            }

            //对子级的赋值
            if (Converter.instance.TryConvertTo(value, originalValueType, out var o))
            {
                try
                {
                    var parents = this.parents;
                    var result = default(object);
                    var last = parents.Length - 1;
                    for (int i = 0; i < last; i++)
                    {
                        var referenceHierarchyVar = parents[i] as ReferenceHierarchyVar;
                        if (i == 0)
                        {
                            result = referenceHierarchyVar.originalValue;
                        }
                        else if (result != null && DataBinderHelper.TryGetValue(result.GetType(), result, referenceHierarchyVar.name, out var tmpValue, default))
                        {
                            result = tmpValue;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    return result != null && DataBinderHelper.TrySetValue(result, name, o);
                }
                catch (Exception ex) { Debug.LogException(ex); }
            }
            return false;
        }

        /// <summary>
        /// 尝试设置变量类型
        /// </summary>
        /// <param name="varType"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TrySetVarType(EVarType varType, object value) => TrySetValue(value, varType);

        /// <summary>
        /// 更新层级
        /// </summary>
        public void UpdateHierarchy()
        {
            var varScope = this.varScope;
            Foreach((parent, index, key, current) =>
            {
                if (index >= 0)//数组
                {
                    current._varCollection = _varCollection;
                    current.varScope = varScope;
                    //current._hierarchyKey = new HierarchyKey(index, false);
                    //current._name = current._hierarchyKey.formatName;

                    current.parent = parent;
                }
                else if (key != null)//对象
                {
                    current._varCollection = _varCollection;
                    current.varScope = varScope;
                    //current._hierarchyKey = new HierarchyKey(key);
                    //current._name = current._hierarchyKey.formatName;

                    current.parent = parent;
                }
                current.InternalMarkDirty();
            });
            CallHierarchyVarChanged();
        }

        /// <summary>
        /// 转JSON
        /// </summary>
        /// <param name="prettyPrint"></param>
        /// <returns></returns>
        public string ToJson(bool prettyPrint) => JsonHelper.ToJson(this, prettyPrint);
    }

    #endregion
}
