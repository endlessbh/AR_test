using XCSJ.Attributes;
using XCSJ.PluginRepairman.Base;
using XCSJ.PluginRepairman.States;
using XCSJ.PluginTools.SelectionUtils;

namespace XCSJ.PluginRepairman.UI
{
    /// <summary>
    /// 零件按钮界面
    /// </summary>
    [Name("零件按钮界面")]
    public class GUIPartButton : GUIItemButton
    {
        protected Part part => item as Part;

        protected override void Update()
        {
            base.Update();

            if (part && part.assembleState == EAssembleState.Assembled)
            {
                gameObject.SetActive(false);
            }
        }

        protected override void OnButtonClick()
        {
            if (part && part.go)
            {
                LimitedSelection.SetSelected(part.go);
            }
        }
    }
}
