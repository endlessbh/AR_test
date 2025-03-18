using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Interactions;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;

namespace XCSJ.EditorExtension.Base.Interactions.Tools
{
    /// <summary>
    /// 交互用途对象检查器
    /// </summary>
    [CustomEditor(typeof(InteractUsageObject), true)]
    [CanEditMultipleObjects]
    public class InteractUsageObjectInspector : InteractUsageObjectInspector<InteractUsageObject>
    {

    }

    /// <summary>
    /// 交互用途对象检查器模板
    /// </summary>
    public class InteractUsageObjectInspector<T> : InteractTagObjectInspector<T> where T : InteractUsageObject
    {
        /// <summary>
        /// 用途列表
        /// </summary>
        [Name("用途列表")]
        [Tip("当前对象的使用者列表", "List of users for the current object")]
        public bool _display = true;

        /// <summary>
        /// 当检查器绘制
        /// </summary>
        [LanguageTuple("Key Word","关键字")]
        [LanguageTuple("User", "使用者")]
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (!Application.isPlaying) return;

            // 折叠
            _display = UICommonFun.Foldout(_display, CommonFun.NameTip(GetType(), nameof(_display)));
            if (!_display) return;

            CommonFun.BeginLayout();
            {                
                // 标题
                EditorGUILayout.BeginHorizontal(GUI.skin.box);
                {
                    GUILayout.Label("Key Word".Tr(), UICommonOption.Width200);
                    GUILayout.Label("User".Tr());
                }
                EditorGUILayout.EndHorizontal();

                // 列表
                EditorGUILayout.BeginVertical(GUI.skin.box);
                {
                    foreach (var item in targetObject.usage.usageMap)
                    {
                        EditorGUILayout.BeginHorizontal();

                        // 关键字
                        EditorGUILayout.LabelField(item.Key, UICommonOption.Width200);

                        // 对象列表
                        if (item.Value.userCount == 0)
                        {
                            EditorGUILayout.LabelField("");
                        }
                        else
                        {
                            EditorGUILayout.BeginVertical();
                            {
                                foreach (var user in item.Value.users)
                                {
                                    EditorGUILayout.ObjectField(user, user ? user.GetType() : typeof(InteractUsageObject), true);
                                }
                            }
                            EditorGUILayout.EndVertical();
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }
                EditorGUILayout.EndVertical();
            }
            CommonFun.EndLayout();
        }
    }
}
