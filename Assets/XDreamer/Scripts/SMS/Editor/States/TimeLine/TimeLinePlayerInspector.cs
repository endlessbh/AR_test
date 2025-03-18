using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Caches;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorSMS.Inspectors;
using XCSJ.EditorTools;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS;
using XCSJ.PluginSMS.States.TimeLine;
using XCSJ.PluginTimelines.Tools;

namespace XCSJ.EditorSMS.States.TimeLine
{
    /// <summary>
    /// 时间轴播放器检查器
    /// </summary>
    [Name("时间轴播放器检查器")]
    [CustomEditor(typeof(TimeLinePlayer))]
    public class TimeLinePlayerInspector : StateComponentInspector<TimeLinePlayer>
    {
        protected TimeLinePlayer player => target as TimeLinePlayer;

        private SerializedProperty useContentTimeLength;
        
        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            if (target == null) return;
            useContentTimeLength = serializedObject.FindProperty(nameof(TimeLinePlayer.useContentTimeLength));
            EditorApplication.hierarchyChanged += FindObject;
            FindObject();
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            _playerController = null;
            EditorApplication.hierarchyChanged += FindObject;
        }

        /// <summary>
        /// 当绘制成员
        /// </summary>
        /// <param name="serializedProperty"></param>
        protected override void OnDrawMember(SerializedProperty serializedProperty, PropertyData propertyData)
        {
            switch (serializedProperty.name)
            {
                case nameof(TimeLinePlayer.useContentTimeLength):
                    {
                        return;
                    }
                case nameof(TimeLinePlayer.playContent):
                    {
                        EditorGUILayout.BeginHorizontal();
                        base.OnDrawMember(serializedProperty, propertyData);
                        EditorGUI.BeginDisabledGroup(player.playContent);
                        if (GUILayout.Button(CommonFun.NameTip(EIcon.Add), EditorStyles.miniButtonRight, UICommonOption.WH24x16))
                        {
                            serializedProperty.objectReferenceValue = TimeLinePlayContent.CreateTimeLinePlayContent(player).GetComponent<TimeLinePlayContent>();
                        }
                        EditorGUI.EndDisabledGroup();

                        EditorGUILayout.EndHorizontal();
                        return;
                    }
                case nameof(TimeLinePlayer.duration):
                    {
                        EditorGUILayout.BeginHorizontal();
                        base.OnDrawMember(serializedProperty, propertyData);
                        if (useContentTimeLength != null)
                        {
                            if ((useContentTimeLength.boolValue = UICommonFun.ButtonToggle(CommonFun.NameTooltip(typeof(TimeLinePlayer), nameof(TimeLinePlayer.useContentTimeLength)), useContentTimeLength.boolValue, EditorStyles.miniButtonRight, GUILayout.Width(60))) && player.playContent)
                            {
                                //player.UseContentTimeLength();
                                serializedProperty.doubleValue = player.playContent.GetTimeLength();
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                        return;
                    }
            }
            base.OnDrawMember(serializedProperty, propertyData);
        }

        private void FindObject()
        {
            _playerController = CommonFun.GetComponentsInChildren<PlayerController>(true).FirstOrDefault();

        }

        private PlayerController _playerController = null;

        /// <summary>
        /// 当绘制检查器GUI
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("界面");
            if (GUILayout.Button(CommonFun.NameTip(MethodInfoCache.Get(typeof(XCSJ.EditorTimelines.ToolsMenu), nameof(XCSJ.EditorTimelines.ToolsMenu.CreateTimeLinePlayer), TypeHelper.DefaultLookup))))
            {
                if (_playerController)
                {
                    EditorGUIUtility.PingObject(_playerController.gameObject);
                }
                else
                {
                    var go = XCSJ.EditorTimelines.ToolsMenu.CreateTimeLinePlayer(ToolContext.Get(typeof(XCSJ.EditorTimelines.ToolsMenu), nameof(XCSJ.EditorTimelines.ToolsMenu.CreateTimeLinePlayer)));
                    _playerController = FindObjectOfType<PlayerController>();
                    if (_playerController)
                    {
                        
                        EditorGUIUtility.PingObject(_playerController.gameObject);
                    }
                }

            }
            EditorGUILayout.EndHorizontal();

            DrawPlayerController();
        }

        public override StringBuilder GetHelpInfo()
        {
            var info = base.GetHelpInfo();
            if (player.playContent)
            {
                info.AppendFormat("播放内容:\t{0}", player.playContent.GetNamePath());
            }
            else
            {
                info.Append("<color=red>播放内容无效</color>");
            }
            return info;
        }

        public override StringBuilder GetRuntimeHelpInfo()
        {
            var info = base.GetHelpInfo();
            if (player.playContent)
            {
                info.AppendFormat("播放状态:\t{0}", player.playerState);
                info.AppendFormat("\n当前进度:\t{0}", player.percent);
                info.AppendFormat("\n当前时间:\t{0}", player.time);
            }
            return info;
        }

        [Name("播放器控制器")]
        private bool playerController = true;

        private void DrawPlayerController()
        {
            if (!(playerController = UICommonFun.Foldout(playerController, CommonFun.NameTip(GetType(), nameof(playerController))))) return;

            CommonFun.BeginLayout();

            EditorGUI.BeginDisabledGroup(!Application.isPlaying || !player.parent.isActive);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("控制", UICommonOption.Width48, UICommonOption.Height32);
            if (GUILayout.Button(CommonFun.NameTip(EIcon.Play), GUI.skin.button, UICommonOption.Width32, UICommonOption.Height32))
            {
                player.PlayOrResume();
            }
            if (GUILayout.Button(CommonFun.NameTip(EIcon.Pause), GUI.skin.button, UICommonOption.Width32, UICommonOption.Height32))
            {
                player.Pause();
            }
            if (GUILayout.Button(CommonFun.NameTip(EIcon.Stop), GUI.skin.button, UICommonOption.Width32, UICommonOption.Height32))
            {
                player.Stop();
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("进度", UICommonOption.Width48);
            EditorGUI.BeginChangeCheck();
            var percent = EditorGUILayout.Slider((float)player.percent, 0, 1);
            if (EditorGUI.EndChangeCheck())
            {
                player.SetPercent(percent);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("时间", UICommonOption.Width48);
            EditorGUI.BeginChangeCheck();
            var time = EditorGUILayout.Slider((float)player.time, 0, (float)player.duration);
            if (EditorGUI.EndChangeCheck())
            {
                player.SetTime(time);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUI.EndDisabledGroup();

            CommonFun.EndLayout();
        }
    }
}
