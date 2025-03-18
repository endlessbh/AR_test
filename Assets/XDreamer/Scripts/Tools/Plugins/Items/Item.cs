using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils.Interactions;

namespace XCSJ.PluginTools.Items
{
    public class Item : InteractableVirtual
    {
        [Readonly]
        public string _id;

        public string _itemName;

        public string _showDescription;

        public Sprite _icon;

        public virtual string id { get => _id; set => _id = value; }

        public virtual string itemName { get => _itemName; set => _itemName = value; }

        public ItemCollection _container;

        public virtual ItemCollection container { get => _container; set => _container = value; }

        public override List<string> cmds => throw new NotImplementedException();

        public virtual void Reset()
        {
            if (string.IsNullOrEmpty(_id))
            {
                _id = System.Guid.NewGuid().ToString();
            }
        }

        public void Remove()
        {
            if (container)
            {
                container.Remove(this);
            }
        }

        public override List<string> GetWorkCmds(InteractData interactData) => new List<string>();

        public override bool CanInteractAsInteractable(InteractData interactData) => true;

        protected override EInteractResult OnInteractAsInteractable(InteractData interactData) => EInteractResult.Finished;
    }

    [Serializable]
    public class ItemInfo
    {
        [Name("物品名称")]
        public string _name;

        [Name("数量")]
        [Min(0)]
        public int _amount;

        [Name("物品坐标参考")]
        public Transform _pose;

        public ItemInfo(string name, int amount, Transform pose)
        {
            this._name = name;
            this._amount = amount;
            this._pose = pose;
        }

        public void SetPose(Transform transform)
        {
            if (_pose)
            {
                transform.position = _pose.position;
                transform.rotation = _pose.rotation;
            }
        }
    }

}
