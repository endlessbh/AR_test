using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginTools;
using XCSJ.PluginXGUI.Base;

namespace XCSJ.PluginXGUI.Windows.Weathers
{
    /// <summary>
    /// 天气数据文本:将天气数据显示到文本中
    /// </summary>
    [Name("天气数据文本")]
    [Tip("将天气数据显示到文本中", "Display weather data into text")]
    [Tool(XGUICategory.Component, nameof(XGUIManager))]
    [XCSJ.Attributes.Icon(EIcon.Text)]
    [RequireManager(typeof(XGUIManager), typeof(ToolsManager))]
    public class WeatherDataText : WeatherDataUI
    {
        /// <summary>
        /// 文本:期望显示天气数据的文本对象；如不设置，会从当前组件所在的游戏对象上查找本参数对应类型的组件；
        /// </summary>
        [Name("文本")]
        [Tip("期望显示天气数据的文本对象；如不设置，会从当前组件所在的游戏对象上查找本参数对应类型的组件；", "A text object expected to display weather data; If not set, the corresponding type of component of this parameter will be found from the game object where the current component is located;")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ComponentPopup]
        public Text text;

        /// <summary>
        /// 启用时
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            if (!text)
            {
                text = GetComponent<Text>();
            }
        }

        /// <summary>
        /// 更新UI
        /// </summary>
        protected override void UpdateUI()
        {
            if (!text) return;
            if (TryGetData(out string data))
            {
                text.text = data;
            }
        }

        /// <summary>
        /// 验证
        /// </summary>
        public void OnValidate()
        {
            if (Application.isPlaying)
            {
                UpdateUI();
            }
        }
    }
}
