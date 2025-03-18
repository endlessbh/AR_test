using System.Text;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorSMS.States.Motions;
using XCSJ.Languages;
using XCSJ.PluginSMS.Base;
using XCSJ.PluginSMS.States.MultiMedia;

namespace XCSJ.EditorSMS.States.MultiMedia
{
    /// <summary>
    /// 路径动画检查器
    /// </summary>
    [Name("路径动画检查器")]
    [CustomEditor(typeof(PathAnimation))]
    public class PathAnimationInspector : PathInspector<PathAnimation>
    {
        [LanguageTuple("\nMove Perfect Spacing :", "\n移动完美间距:")]
        [LanguageTuple("\n\tPercentage :\t{0}", "\n\t百分比:\t{0}")]
        [LanguageTuple("\n\tTime :\t{0}", "\n\t时间:\t{0}")]
        [LanguageTuple("\n\tDistance :\t{0}", "\n\t距离:\t{0}")]
        [LanguageTuple("\nView Perfect Spacing :", "\n视图完美间距:")]
        public override StringBuilder GetHelpInfo()
        {
            var info = base.GetHelpInfo();

            var count = workClip.transforms.Count;

            var movePath = workClip.GetFullMovePath();
            info.Append(Tr("\nMove Perfect Spacing :"));
            info.AppendFormat(Tr("\n\tPercentage :\t{0}"), workClip.GetPrettySpaceValue(count, PathAnimation.ESpaceType.Perent, movePath));
            info.AppendFormat(Tr("\n\tTime :\t{0}"), workClip.GetPrettySpaceValue(count, PathAnimation.ESpaceType.Time, movePath));
            info.AppendFormat(Tr("\n\tDistance :\t{0}"), workClip.GetPrettySpaceValue(count, PathAnimation.ESpaceType.Distance, movePath));

            Vector3[] viewPath = null;
            switch (workClip.viewRule)
            {
                case EViewRule.MovePath:
                    {
                        viewPath = movePath;
                        break;
                    }
                case EViewRule.ViewPath:
                    {
                        viewPath = workClip.GetFullViewPath();
                        break;
                    }
                default: return info;
            }
            info.Append(Tr("\nView Perfect Spacing :"));
            info.AppendFormat(Tr("\n\tPercentage :\t{0}"), workClip.GetPrettySpaceValue(count, PathAnimation.ESpaceType.Perent, viewPath));
            info.AppendFormat(Tr("\n\tTime :\t{0}"), workClip.GetPrettySpaceValue(count, PathAnimation.ESpaceType.Time, viewPath));
            info.AppendFormat(Tr("\n\tDistance :\t{0}"), workClip.GetPrettySpaceValue(count, PathAnimation.ESpaceType.Distance, viewPath));

            return info;
        }
    }
}
