using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.Extension;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginTools.SelectionUtils;

namespace XCSJ.EditorExtension
{
    /// <summary>
    /// 通用标准脚本检查器
    /// </summary>
    [Name("通用标准脚本检查器")]
    [CustomEditor(typeof(GenericStandardScriptManager))]
    public class GenericStandardScriptManagerInspector : BaseManagerInspector<GenericStandardScriptManager>
    {
        /// <summary>
        /// 初始化
        /// </summary>
        [InitializeOnLoadMethod]
        public static void Init()
        {
            Manager.onReset += InitManager;
            ScriptViewerWindow.onHasObjectUsed += OnHasObjectUsed;
        }

        private static void InitManager(Manager manager)
        {
            if (manager is GenericStandardScriptManager m && m)
            {
                if (!m._commonMaterial)
                {
                    m._commonMaterial = UICommonFun.LoadFromAssets<Material>(XDreamerBaseOption.weakInstance.commonMaterialPath);
                }
            }
        }

        private static void OnHasObjectUsed(UnityEngine.Object obj)
        {
            var manager = GenericStandardScriptManager.instance;
            if (!manager) return;
            manager.Add(obj);
        }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            if (!target) return;
            base.OnEnable();
            InitManager(manager);
            manager.AddToUnityAssetObjectManager();
        }

        /// <summary>
        /// 当绘制脚本
        /// </summary>
        /// <param name="serializedProperty"></param>
        protected override void OnDrawScript(SerializedProperty serializedProperty)
        {
            base.OnDrawScript(serializedProperty);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(typeof(ScriptViewerWindow).TrLabel()))
            {
                ScriptViewerWindow.OpenAndFocus();
            };
            if (GUILayout.Button(typeof(ScriptEditorWindow).TrLabel()))
            {
                ScriptEditorWindow.OpenAndFocus();
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}