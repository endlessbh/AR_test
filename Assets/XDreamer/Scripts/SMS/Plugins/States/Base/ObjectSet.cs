using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.Kernel;
using UnityEngine.Serialization;
using XCSJ.Extension.Base;
using XCSJ.Extension.Base.Helpers;
using XCSJ.Extension.Base.Attributes;

namespace XCSJ.PluginSMS.States.Base
{
    /// <summary>
    /// 对象集
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TObject"></typeparam>
    public abstract class ObjectSet<T, TObject> : StateComponent<T>, IObjectSet<TObject>
        where T : ObjectSet<T, TObject>
        where TObject : UnityEngine.Object
    {
        [Readonly(EEditorMode.Runtime)]
        [Name("包含自身")]
        [Tip("运行初始化时，会从当前状态所在游戏对象上查找符合要求的对象；", "When running initialization, it will find qualified objects from the game object in the current state;")]
        public bool includeSelf = false;

        /// <summary>
        /// 对象集
        /// </summary>
        [Name("对象集")]
        [SerializeField]
        //[Readonly(EEditorMode.Runtime)]
        [FormerlySerializedAs(nameof(IObjectSet.objects))]
        [ObjectPopup]
        public List<TObject> _objects = new List<TObject>();

        /// <summary>
        /// 对象列表
        /// </summary>
        public virtual List<TObject> objects => _objects;

        List<Object> IObjectSet.objects => _objects.ToList(o => (Object)o);

        public override bool Init(StateData data)
        {
            //移除无效项
            RemoveInvalidObject();
            return base.Init(data);
        }

        /// <summary>
        /// 去重方式添加对象；支持在Unity编辑器中执行撤销与重做；
        /// </summary>
        /// <param name="obj"></param>
        public void Add(TObject obj)
        {
            if (ObjectHelper.ObjectIsNull(obj)) return;
            this.XModifyProperty(() =>
            {
                _objects.AddWithDistinct(obj);
            });
        }

        public bool Remove(TObject obj) => _objects.Remove(obj);

        public bool Contains(TObject obj) => _objects.Contains(obj);

        public void DistinctObjects()
        {
            _objects = objects.Distinct().ToList();
        }

        public void RemoveInvalidObject()
        {
            _objects.RemoveAll(ObjectHelper.ObjectIsNull);
        }

        public override bool Finished() => true;

        public override bool DataValidity()
        {
            return includeSelf || (objects.Count > 0 && objects.All(obj => !ObjectHelper.ObjectIsNull(obj)));
        }

        public override string ToFriendlyString()
        {
            return (objects.Count == 1 && objects[0]) ? objects[0].name : objects.Count + "个";
        }
    }

    public interface IObjectSet<T> : IObjectSet
    {
        new List<T> objects { get; }
    }

    public interface IObjectSet
    {
        List<UnityEngine.Object> objects { get; }
    }
}
