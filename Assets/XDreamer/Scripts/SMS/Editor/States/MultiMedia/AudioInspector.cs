using System.Text;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorSMS.States.Base;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.States.MultiMedia;

namespace XCSJ.EditorSMS.States.MultiMedia
{
    /// <summary>
    /// 音频检查器
    /// </summary>
    [Name("音频检查器")]
    [CustomEditor(typeof(Audio))]
    public class AudioInspector : WorkClipInspector<Audio>
    {
        #region 同步TL

        /// <summary>
        /// 有同步时长按钮
        /// </summary>
        /// <returns></returns>
        protected override bool HasSyncTLButton() => true;

        /// <summary>
        /// 获取同步时长按钮内容
        /// </summary>
        /// <returns></returns>
        protected override GUIContent GetSyncTLButtonContent()
        {
            var content = base.GetSyncTLButtonContent();
            content.tooltip += string.Format("\n将时长同步为音频时长");
            return content;
        }

        /// <summary>
        /// 获取预期的时长
        /// </summary>
        /// <returns></returns>
        protected override double? GetExpectedTL()
        {
            if (workClip && workClip.audioSource && workClip.audioSource.clip)
            {
                return workClip.audioSource.clip.length;
            }
            return default;
        }

        #endregion

        /// <summary>
        /// 当绘制成员
        /// </summary>
        /// <param name="serializedProperty"></param>
        protected override void OnDrawMember(SerializedProperty serializedProperty, PropertyData propertyData)
        {
            switch (serializedProperty.name)
            {
                case nameof(Audio.audioSource):
                    {
                        EditorGUILayout.BeginHorizontal();
                        base.OnDrawMember(serializedProperty, propertyData);
                        EditorGUI.BeginDisabledGroup(stateComponent.audioSource);
                        if (GUILayout.Button(CommonFun.NameTip(EIcon.Add), EditorStyles.miniButtonRight, UICommonOption.WH24x16))
                        {
                            serializedProperty.objectReferenceValue = CreateAudioSource();
                        }
                        EditorGUI.EndDisabledGroup();
                        EditorGUILayout.EndHorizontal();
                        return;
                    }
            }
            base.OnDrawMember(serializedProperty, propertyData);
        }

        private const string AudioSourceRootName = "音频源目录";
        private const string AudioSourceName = "音频源";

        private AudioSource CreateAudioSource()
        {
            var root = GameObject.Find(AudioSourceRootName);
            if(!root)
            {
                root = new GameObject(AudioSourceRootName);
                UndoHelper.RegisterCreatedObjectUndo(root);
            }

            var go = new GameObject(GameObjectUtility.GetUniqueNameForSibling(root.transform, AudioSourceName));
            UndoHelper.RegisterCreatedObjectUndo(go);
            UndoHelper.RecordSetTransformParent(go.transform, root.transform);

            var audioSource = Undo.AddComponent(go, typeof(AudioSource)) as AudioSource;

            EditorGUIUtility.PingObject(go);

            return audioSource;
        }

        /// <summary>
        /// 获取辅助信息
        /// </summary>
        /// <returns></returns>
        public override StringBuilder GetHelpInfo()
        {
            var info = base.GetHelpInfo();
            var workClip = this.workClip;
            if (!workClip) return info;

            info.Append("\n音频源:\t");
            if (workClip.audioSource)
            {
                info.Append(CommonFun.GameObjectToString(workClip.audioSource.gameObject));
            }
            else
            {
                return info.AppendFormat("<color=red>数据无效</color>");
            }


            info.Append("\n音频剪辑:\t");
            if (workClip.audioSource.clip)
            {
                info.Append(AssetDatabase.GetAssetPath(workClip.audioSource.clip));
            }
            else
            {
                return info.Append("<color=red>数据无效</color>");
            }

            return info.AppendFormat("\n音频时长:\t{0} 秒\n高音:\t{1}", workClip.audioSource.clip.length, workClip.audioSource.pitch);
        }
    }
}
