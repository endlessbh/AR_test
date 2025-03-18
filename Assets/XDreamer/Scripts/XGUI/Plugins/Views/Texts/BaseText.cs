using System;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginTools;
using XCSJ.PluginXGUI.Base;

namespace XCSJ.PluginXGUI.Views.Texts
{
    /// <summary>
    /// Text基类
    /// </summary>
    public abstract class BaseText : View
    {
        /// <summary>
        /// 文本:期望的文本对象；如不设置，会从当前组件所在的游戏对象上查找本参数对应类型的组件；
        /// </summary>
        [Name("文本")]
        [Tip("期望的文本对象；如不设置，会从当前组件所在的游戏对象上查找本参数对应类型的组件；", "The desired text object; If not set, the corresponding type of component of this parameter will be found from the game object where the current component is located;")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ComponentPopup]
        public Text text;

        /// <summary>
        /// 重置
        /// </summary>
        public void Reset() => DefaultFindText();

        /// <summary>
        /// 启用
        /// </summary>
        protected virtual void Awake() => DefaultFindText();

        private void DefaultFindText()
        {
            if (!text)
            {
                text = GetComponent<Text>();
            }
        }
    }
}
