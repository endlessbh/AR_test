using System.Text;
using UnityEditor;
using XCSJ.Attributes;
using XCSJ.EditorXAR.Foundation.Base.Tools;
using XCSJ.PluginXAR.Foundation.Images.Tools;

namespace XCSJ.EditorXAR.Foundation.Images.Tools
{
    /// <summary>
    /// 图像跟踪器检查器
    /// </summary>
    [Name("图像跟踪器检查器")]
    [CustomEditor(typeof(ImageTracker), true)]
    public class ImageTrackerInspector : BaseTrackerInspector<ImageTracker>
    {
        /// <summary>
        /// 标识是否显示辅助信息
        /// </summary>
        protected override bool displayHelpInfo => tracker.trackData.displayHelpInfo;

        /// <summary>
        /// 获取辅助信息
        /// </summary>
        /// <returns></returns>
        public override StringBuilder GetHelpInfo()
        {
            var info = base.GetHelpInfo(); 
            info.AppendLine("<color=red>图像数据中纹理对象要求可读且名称有效！</color>");
            return info;
        }
    }
}
