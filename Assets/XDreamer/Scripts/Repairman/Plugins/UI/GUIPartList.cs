using System.Linq;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginRepairman.States;
using XCSJ.PluginSMS;
using XCSJ.PluginSMS.States;

namespace XCSJ.PluginRepairman.UI
{
    [Name("零件列表界面")]
    public class GUIPartList : GUIItemList 
    {
        [Name("设备")]
        [StateComponentPopup(typeof(Device))]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Device device;

        [Name("只显示零件")]
        [Tip("不显示设备根节点和模块", "Do not display device root nodes and modules")]
        public bool onlyDisplayPart = true;

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            CreatePartList();
        }

        public void CreatePartList()
        {
            if (!device)
            {
                device = SMSHelper.GetStateComponents<Device>().FirstOrDefault();
            }

            if (device)
            {
                ClearItemList();
                var itemList = device.GetChildrenItems().ToList();
                if (onlyDisplayPart)
                {
                    itemList = itemList.Where(i => i is Part).ToList();
                }
                CreateItemList(itemList);
            }
        }
    }
}
