using System.Text;
using UnityEditor;
using XCSJ.Attributes;
using XCSJ.EditorSMS.States.Base;
using XCSJ.PluginSMS.States.Dataflows.DataModel;

namespace XCSJ.EditorSMS.States.Dataflows.DataModel
{
    /// <summary>
    /// 网络数据检查器
    /// </summary>
    [Name("网络数据检查器")]
    [CustomEditor(typeof(WebData))]
    public class WebDataInspector : TriggerInspector<WebData>
    {
        /// <summary>
        /// 显示辅助信息
        /// </summary>
        protected override bool displayHelpInfo => true;

        /// <summary>
        /// 获取辅助信息
        /// </summary>
        /// <returns></returns>
        public override StringBuilder GetHelpInfo()
        {
            var info = base.GetHelpInfo();
            info.AppendFormat("URL:\t{0}\n", stateComponent.url);
            info.Append(@"网络数据由URI指向的网络数据运营商提供!
对网络数据的准确性、安全性、完整性等XDreamer官方均不做担保!
XDreamer仅提供网络数据获取的方法与途径!
因网络数据运营商数据接口调整导致的组件不可用,XDreamer官方将会尽快提出解决方法!
使用者在使用网络数据过程中请遵守网络数据运营商的规范或守则!
如使用者与网络数据运营商产生任何纠纷时，XDreamer官方不参与任何纠纷也不承担责任!");
            return info;
        }
    }
}
