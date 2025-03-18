using System.Collections.Generic;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Extension.Base.RichTexts;

namespace XCSJ.EditorTools.Windows.RichTexts
{
    public class RichTextTool : TICache<RichTextTool, string, RichTextTool.Info>
    {
        #region Cache机制

        public class Info
        {
            public string guid = "";
            public bool editMode = false;
            public RichTextStyle richTextStyle = new RichTextStyle();

            public void CheckEditMode()
            {
                if (!richTextStyle.active)
                {
                    richTextStyle.active = true;
                    editMode = false;
                }
            }
        }

        protected override KeyValuePair<bool, Info> CreateValue(string key1)
        {
            return new KeyValuePair<bool, Info>(true, new Info() { guid = key1 });
        }

        public static Info Get(string text, string guid)
        {
            if (string.IsNullOrEmpty(guid)) return Default<Info>.Instance;
            if (!Cache.TryGetValue(guid, out Info info))
            {
                Cache[guid] = info = new Info() { guid = guid };
                info.richTextStyle.Set(text);
            }
            return info;
        }

        #endregion

        public static void SelectableLabel(Element element, bool editMode, GUIStyle style = null)
        {
            SelectableLabel(ref element.value, element.guid, editMode, style);
        }
        public static void SelectableLabel(ref string text, string guid, bool editMode, GUIStyle style = null)
        {
            if (editMode)
            {
                var info = Get(text, guid);
                if (EditorRichText.Button(text, style))
                {
                    info.editMode = !info.editMode;
                }
                if (info.editMode)
                {
                    text = EditorRichText.TextAreaHandle(text, info.richTextStyle);
                    info.CheckEditMode();
                }
            }
            else
            {
                EditorRichText.SelectableLabel(text, style);
            }
        }

        public static void Label(Content content, bool editMode, GUIStyle style = null)
        {
            if (editMode)
            {
                SelectableLabel(content.name, editMode, style);
                SelectableLabel(content.tip, editMode);
            }
            else
            {
                EditorRichText.Label(content.content, style);
            }
        }

        public static bool Button(Element element, bool editMode, GUIStyle style = null)
        {
            return Button(ref element.value, element.guid, editMode, style);
        }
        public static bool Button(ref string text, string guid, bool editMode, GUIStyle style = null)
        {
            if (editMode)
            {
                var info = Get(text, guid);
                if (EditorRichText.Button(text, style))
                {
                    info.editMode = !info.editMode;
                }
                if (info.editMode)
                {
                    text = EditorRichText.TextAreaHandle(text, info.richTextStyle);
                    info.CheckEditMode();
                }
                return false;
            }
            else
            {
                return EditorRichText.Button(text, style);
            }
        }
    }
}
