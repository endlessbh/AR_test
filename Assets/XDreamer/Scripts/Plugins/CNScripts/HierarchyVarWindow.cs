using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.LitJson;
using XCSJ.PluginCommonUtils;
using XCSJ.Scripts;

namespace XCSJ.Extension.CNScripts
{
    /// <summary>
    /// 层级变量窗口
    /// </summary>
    [Name("层级变量窗口")]
    public class HierarchyVarWindow : BaseGUIWindow
    {
        private HierarchyVar _dictionary = null;

        /// <summary>
        /// 字典
        /// </summary>
        public HierarchyVar dictionary
        {
            get { return _dictionary; }
            set
            {
                _dictionary = value;
                if (_dictionary != null)
                {
                    _title = _dictionary.name;
                }
                else
                {
                    _title = "";
                }
            }
        }

        /// <summary>
        /// 项数目
        /// </summary>
        public int itemCount { get; private set; }

        /// <summary>
        /// 获取滚动视图高度
        /// </summary>
        /// <returns></returns>
        protected override float GetScrollViewHeight() => itemCount * SingleLineHeight;

        /// <summary>
        /// 绘制内容
        /// </summary>
        /// <param name="scrollRect"></param>
        /// <param name="scrollViewRect"></param>
        protected override void OnDrawContent(Rect scrollRect, Rect scrollViewRect)
        {
            int count = 0;
            GUIWindowWithJsonData(dictionary, 0, ref count, scrollViewRect);
            itemCount = count;
        }

        private static void GUIWindowWithJsonData(HierarchyVar json, int indent, ref int y, Rect viewRect)
        {
            if (json == null) return;
            int indentWidth = 8 * indent;//计算本行缩进量
            float w = viewRect.width - indentWidth;//计算本行的数据显示宽度
            float leftWidth = 0.4f * w;//本行左侧名称控件宽度
            float leftX = viewRect.x + indentWidth;//本行左侧名称控件X坐标
            float rightX = leftX + leftWidth;//本行左侧值控件X坐标
            float rightWidth = w - leftWidth;//本行左侧值控件宽度
                                             //左侧名称控件位置与尺寸 -- 纵坐标先不考虑
            Rect lrect = new Rect(leftX, 0, leftWidth, SingleLineHeight);
            //右侧名称控件位置与尺寸 -- 纵坐标先不考虑
            Rect rrect = new Rect(rightX, 0, rightWidth, SingleLineHeight);

            if (json.IsArray)
            {
                for (int i = 0; i < json.Count; ++i)
                {
                    lrect.y = rrect.y = viewRect.y + y++ * SingleLineHeight;
                    GUI.Box(lrect, i.ToString());
                    GUI.Label(rrect, " 数组索引");
                    GUIWindowWithJsonData(json[i], indent + 1, ref y, viewRect);
                }
            }
            else if (json.objectValue != null)
            {
                foreach (var kv in json.objectValue)
                {
                    lrect.y = rrect.y = viewRect.y + y++ * SingleLineHeight;
                    switch (kv.Value.GetJsonType())
                    {
                        case JsonType.Object:
                        case JsonType.Array:
                            {
                                GUI.Box(lrect, kv.Key);
                                //GUI.Label(rrect, kv.Value.GetJsonType().ToString());
                                //开始子对象或数组的渲染
                                GUIWindowWithJsonData(kv.Value, indent + 1, ref y, viewRect);
                                break;
                            }
                        default:
                            {
                                GUI.Box(lrect, kv.Key);
                                GUI.Box(rrect, kv.Value.ToString());
                                break;
                            }
                    }
                }
            }
        }
    }
}
