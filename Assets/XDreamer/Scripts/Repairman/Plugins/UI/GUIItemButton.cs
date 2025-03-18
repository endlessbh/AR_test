using UnityEngine;
using UnityEngine.UI;
using XCSJ.PluginXGUI;
using XCSJ.PluginXGUI.Base;

namespace XCSJ.PluginRepairman.UI
{
    [RequireComponent(typeof(Button))]
    public abstract class GUIItemButton : GUIItem
    {
        private Button button;

        private bool selected;

        protected override void Awake()
        {
            base.Awake();

            button = GetComponent<Button>();
            if (button)
            {
                button.onClick.AddListener(OnButtonClick);
            }

            selected = false;
        }

        protected void OnDestroy()
        {
            if (button)
            {
                button.onClick.RemoveListener(OnButtonClick);
            }
        }

        protected abstract void OnButtonClick();

        protected virtual void Update()
        {
            if (item.selected != selected)
            {
                selected = item.selected;
                button.SetColor(selected ? guiItemList.selectedColor : guiItemList.unselectedColor);
            }
        }

        protected override void SetIcon(Texture2D texture2D)
        {
            if (texture2D)
            {
                button.GetComponent<Image>().sprite = XGUIHelper.ToSprite(item.texture2D);
            }
        }
    }
}
