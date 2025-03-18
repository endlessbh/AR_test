using System;
using System.Text;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorSMS.NodeKit;
using XCSJ.EditorSMS.States.Base;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.States.MultiMedia;

namespace XCSJ.EditorSMS.States.MultiMedia
{
    /// <summary>
    /// 可播放导引器检查器
    /// </summary>
    [Name("可播放导引器检查器")]
    [CustomEditor(typeof(XPlayableDirector))]
    class XPlayableDirectorInspector : WorkClipInspector<XPlayableDirector>
    {
        #region 同步TL

        /// <summary>
        /// 标识是否有同步时长按钮
        /// </summary>
        /// <returns></returns>
        protected override bool HasSyncTLButton() => true;

        /// <summary>
        /// 获取同步时按钮内容
        /// </summary>
        /// <returns></returns>
        protected override GUIContent GetSyncTLButtonContent()
        {
            var content = base.GetSyncTLButtonContent();
            content.tooltip += string.Format("\n将时长同步为时间轴时长");
            return content;
        }

        /// <summary>
        /// 获取预期的时长
        /// </summary>
        /// <param name="workRangeMemberProperty"></param>
        protected override double? GetExpectedTL()
        {
            var workClip = this.workClip;
            if (workClip && workClip.playableDirector && workClip.playableDirector.playableAsset)
            {
                return workClip.playableDirector.duration;
            }
            return default;
        }

        #endregion

        /// <summary>
        /// 获取辅助信息
        /// </summary>
        /// <returns></returns>
        public override StringBuilder GetHelpInfo()
        {
            var info = base.GetHelpInfo();
            var workClip = this.workClip;
            if (!workClip) return info;

            info.Append("\n可播放导引器:\t");
            if (workClip.playableDirector)
            {
                info.Append(CommonFun.GameObjectToString(workClip.playableDirector.gameObject));
            }
            else
            {
                return info.AppendFormat("<color=red>数据无效</color>");
            }


            info.Append("\n时间轴资产: \t");
            if (workClip.playableDirector.playableAsset)
            {
                info.Append(AssetDatabase.GetAssetPath(workClip.playableDirector.playableAsset));
            }
            else
            {
                return info.Append("<color=red>数据无效</color>");
            }

            return info.AppendFormat("\n时间轴时长: \t{0} 秒", workClip.playableDirector.duration);
        }
    }
}
