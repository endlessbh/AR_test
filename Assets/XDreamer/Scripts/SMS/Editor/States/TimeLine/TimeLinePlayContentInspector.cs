using System.Linq;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.States.TimeLine;

namespace XCSJ.EditorSMS.States.TimeLine
{
    /// <summary>
    /// 时间轴播放内容检查器
    /// </summary>
    [Name("时间轴播放内容检查器")]
    [CustomEditor(typeof(TimeLinePlayContent), true)]
    public class TimeLinePlayContentInspector: StateWorkClipSetInspector
    {
        public TimeLinePlayContent timeLinePlayContent => target as TimeLinePlayContent;

        protected override void OnEnable()
        {
            base.OnEnable();
            FindObject();
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            timeLinePlayer = null;
        }

        private void FindObject()
        {
            timeLinePlayer = Resources.FindObjectsOfTypeAll<TimeLinePlayer>().FirstOrDefault(player => player.playContent == timeLinePlayContent);
        }

        private TimeLinePlayer timeLinePlayer = null;

        public override void OnDrawHelpInfo()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("关联播放器");
            if (GUILayout.Button(CommonFun.NameTip(typeof(TimeLinePlayer))))
            {
                if (timeLinePlayer)
                {
                    EditorSMSHelper.PingObject(timeLinePlayer);
                }
                else
                {
                    timeLinePlayer = TimeLinePlayer.Create(stateComponent.parent.parent).GetComponent<TimeLinePlayer>();
                    if (timeLinePlayer)
                    {
                        timeLinePlayer.playContent = timeLinePlayContent;
                        EditorSMSHelper.PingObject(timeLinePlayer);
                    }
                }
            }
            EditorGUILayout.EndHorizontal();

            base.OnDrawHelpInfo();
        }
    }
}
