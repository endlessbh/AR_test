using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginRepairman.Machines;

namespace XCSJ.PluginRepairman.UI
{

    public abstract class GUIItem : BaseRepairmanGUI
    {
        protected IItem item = null;

        protected GUIItemList guiItemList = null;

        [Name("名称")]
        public Text itemName = null;

        protected void Reset() => FindComponents();

        protected virtual void Awake() => FindComponents();

        protected virtual void FindComponents()
        {
            if (itemName == null)
            {
                itemName = GetComponentInChildren<Text>();
            }            
        }

        public virtual void SetData(IItem item, GUIItemList guiItemList)
        {
            this.item = item;

            this.guiItemList = guiItemList;

            if (itemName)
            {
                itemName.text = this.item.showName;
            }

            SetIcon(item.texture2D);
        }

        protected abstract void SetIcon(Texture2D texture2D);
    }

    /// <summary>
    /// 基础拆装界面
    /// </summary>
    [RequireManager(typeof(RepairmanManager))]
    public abstract class BaseRepairmanGUI : InteractProvider { }
}
