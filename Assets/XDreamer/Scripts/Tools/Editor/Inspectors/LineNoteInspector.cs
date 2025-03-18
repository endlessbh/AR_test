using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Base.Kernel;
using XCSJ.EditorXGUI;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginTools.Effects;
using XCSJ.PluginTools.LineNotes;

namespace XCSJ.EditorTools.Inspectors
{
    /// <summary>
    /// 批注检查器
    /// </summary>
    [Name("批注检查器")]
    [CustomEditor(typeof(LineNote), true)]
    [CanEditMultipleObjects]
    public class LineNoteInspector : MBInspector<LineNote>
    {
        /// <summary>
        /// 当绘制成员
        /// </summary>
        /// <param name="serializedProperty"></param>
        [LanguageTuple("When the rendering mode is equal to the line renderer, the line style width must be greater than 0", "渲染模式等于线渲染器时线样式宽度必须大于0")]
        protected override void OnDrawMember(SerializedProperty serializedProperty, PropertyData propertyData)
        {
            switch (serializedProperty.name)
            {
                case nameof(LineNote._RenderMode):
                    {
                        base.OnDrawMember(serializedProperty, propertyData);
                        if (targetObject._RenderMode == ERenderMode.LineRenderer)
                        {
                            if (targetObject.lineStyle && targetObject.lineStyle.width == 0)
                            {
                                UICommonFun.RichHelpBox("When the rendering mode is equal to the line renderer, the line style width must be greater than 0".Tr(this.GetType()), MessageType.Error);
                            }
                        }
                        return;
                    }
                case nameof(LineNote.target):
                    {
                        EditorGUILayout.BeginHorizontal();
                        base.OnDrawMember(serializedProperty, propertyData);
                        if (GUILayout.Button(UICommonOption.Insert, UICommonOption.WH24x16))
                        {
                            CreateNoteGameObjectAndFocus(CommonFun.Name(typeof(LineNote), nameof(LineNote.target)), serializedProperty, targetObject.gameObject);
                        }
                        EditorGUILayout.EndHorizontal();
                        break;
                    }
                case nameof(LineNote.lineStyle):
                    {
                        EditorGUILayout.BeginHorizontal();
                        base.OnDrawMember(serializedProperty, propertyData);
                        EditorGUI.BeginDisabledGroup(targetObject.lineStyle);
                        if (GUILayout.Button(UICommonOption.Insert, UICommonOption.WH24x16))
                        {
                            serializedProperty.objectReferenceValue = targetObject.gameObject.AddComponent<LineStyle>();
                        }
                        EditorGUI.EndDisabledGroup();
                        EditorGUILayout.EndHorizontal();
                        return;
                    }
            }
            base.OnDrawMember(serializedProperty, propertyData);
        }

        protected static GameObject CreateNoteGameObjectAndFocus(string name, SerializedProperty memberProperty, GameObject parent)
        {
            var go = new GameObject(name);
            if (go)
            {
                memberProperty.objectReferenceValue = go;
                UndoHelper.RegisterCreatedObjectUndo(go);
                if (parent) UndoHelper.RecordSetTransformParent(go.transform, parent.transform);
                EditorGUIUtility.PingObject(go);
            }
            return go;
        }
    }

    public static class UGUILineHelper
    {
        private const string notePanelName = "批注集";

        public static RectTransform CreateButtonNote()
        {
            GameObject parent = null;
            var canvas = EditorXGUIHelper.FindOrCreateRootCanvas();
            var item = canvas.transform.Find(notePanelName);
            if (item)
            {
                parent = item.gameObject;
            }
            else
            {
                parent = EditorHandler.Create(notePanelName, canvas.transform);
            }

            if (parent && XCSJ.EditorXGUI.ToolsMenu.CreateUIWithStyle<Button>() is Button btn && btn)
            {
                var btnGO = btn.gameObject;
                btnGO.XSetParent(parent);
                btnGO.XModifyProperty(() => GameObjectUtility.EnsureUniqueNameForSibling(btnGO)); 
                btnGO.GetComponent<RectTransform>().XCenterHV();
                return btn.GetComponent<RectTransform>();
            }
            return null;
        }

    }

}
