using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.Scripts;

namespace XCSJ.Extension.Logs
{
    /// <summary>
    /// 日志查看器窗口
    /// </summary>
    [Name("日志查看器窗口")]
    public class LogViewerWindow : BaseGUIWindow
    {
        #region 日志数据

        List<LogInfo> logs = new List<LogInfo>();

        public int logCount = 0;

        private void AddLog(LogInfo logInfo)
        {
            logs.Add(logInfo);
            logCount++;
        }

        /// <summary>
        /// 清空日志信息
        /// </summary>
        public void Clear()
        {
            logs.Clear();
            logCount = 0;
        }

        public void AddLog(string condition, string stackTrace, LogType type)
        {
            AddLog(new LogInfo(condition, stackTrace, type));
        }

        /// <summary>
        /// 当日志回调时
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="stackTrace"></param>
        /// <param name="type"></param>
        public static void OnLogCallback(string condition, string stackTrace, LogType type)
        {
            instance.AddLog(condition, stackTrace, type);
        }

        List<LogInfo> logsThreaded = new List<LogInfo>();

        /// <summary>
        /// 当多线程日志回调时
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="stackTrace"></param>
        /// <param name="type"></param>
        public static void OnLogCallbackThreaded(string condition, string stackTrace, LogType type)
        {
            var logInfo = new LogInfo(condition, stackTrace, type);
            lock (instance.logsThreaded)
            {
                instance.logsThreaded.Add(logInfo);
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        public void Update()
        {
            if (logsThreaded.Count > 0)
            {
                lock (logsThreaded)
                {
                    logsThreaded.ForEach(l => AddLog(l));
                    logsThreaded.Clear();
                }
            }
        }

        #endregion

        /// <summary>
        /// 设置可见性
        /// </summary>
        /// <param name="visable"></param>
        public static void SetVisable(EBool visable)
        {
            instance.visable = CommonFun.BoolChange(instance.visable, visable);
            if (instance.visable && LogManager.instance)
            {
                LogManager.instance.XGetOrAddComponent<LogViewerDrawer>().enabled = true;
            }
        }

        /// <summary>
        /// 左边框高度
        /// </summary>
        public int leftMargin = 12;

        private GUIStyle m_BoxStyle = null;

        /// <summary>
        /// BOX样式
        /// </summary>
        public GUIStyle boxStyle
        {
            get
            {
                if (m_BoxStyle == null)
                {
                    m_BoxStyle = new GUIStyle(GUI.skin.box);
                    m_BoxStyle.alignment = TextAnchor.UpperLeft;
                }
                return m_BoxStyle;
            }
        }

        /// <summary>
        /// 视图高度
        /// </summary>
        public float viewHeight = 0;

        /// <summary>
        /// 单例
        /// </summary>
        public readonly static LogViewerWindow instance = new LogViewerWindow();

        /// <summary>
        /// 构造
        /// </summary>
        public LogViewerWindow()
        {
            _title = "日志信息";
            rect = new Rect(rect.x, rect.y, rect.width * 2.5f, rect.height);
        }

        /// <summary>
        /// 绘制标题
        /// </summary>
        /// <returns></returns>
        protected override bool OnDrawTitle()
        {
            Rect r = new Rect(alignRect.x + alignRect.width, alignRect.y, alignRect.width, alignRect.height);
            if (GUI.Button(r, CommonFun.TempContent("C", "Clear")))
            {
                Clear();
            }
            r.x += r.width;
            if (GUI.Button(r, CommonFun.TempContent("F", "Fold")))
            {
                foreach (var log in logs)
                {
                    log.selected = false;
                }
            }
            return base.OnDrawTitle();
        }

        /// <summary>
        /// 绘制内容
        /// </summary>
        /// <param name="scrollRect"></param>
        /// <param name="scrollViewRect"></param>
        protected override void OnDrawContent(Rect scrollRect, Rect scrollViewRect)
        {
            Rect r = new Rect(scrollViewRect.x, scrollViewRect.y, 0, 0);
            var noWidth = 32;
            var contentX = scrollViewRect.x + noWidth;
            var contentWidth = scrollViewRect.width - noWidth;
            Color c = GUI.contentColor;
            for (int i = logs.Count - 1; i >= 0; --i)//逆序输出所有日志
            {
                LogInfo log = logs[i];
                GUI.contentColor = GetLogContenColor(log.logType);

                //编号
                r.x = scrollViewRect.x;
                r.width = noWidth;
                r.y += r.height;
                r.height = SingleLineHeight;
                log.selected = GUI.Toggle(r, log.selected, (i + 1).ToString(), boxStyle);

                //内容
                r.x = contentX;
                r.width = contentWidth;
                log.selected = GUI.Toggle(r, log.selected, log.condition, boxStyle);

                if (log.selected)
                {
                    r.x += leftMargin;
                    r.width -= leftMargin;
                    r.y += r.height;
                    GUIContent condition = new GUIContent(log.condition);
                    //r.height = boxStyle.CalcScreenSize(boxStyle.CalcSize(condition)).y;
                    r.height = boxStyle.CalcSize(condition).y;
                    //r.height = log.conditionLines * boxStyle.lineHeight + boxStyle.border.top + boxStyle.border.bottom;
                    if (GUI.Toggle(r, false, condition, boxStyle)) CommonFun.CopyTextToClipboardForPC(log.condition);

                    r.y += r.height;
                    GUIContent stackTrace = new GUIContent(log.stackTrace);
                    //r.height = boxStyle.CalcScreenSize(boxStyle.CalcSize(stackTrace)).y;
                    r.height = boxStyle.CalcSize(stackTrace).y;
                    //r.height = log.stackTraceLines * boxStyle.lineHeight + boxStyle.border.top + boxStyle.border.bottom;
                    if (GUI.Toggle(r, false, stackTrace, boxStyle)) CommonFun.CopyTextToClipboardForPC(log.stackTrace);
                }
            }
            GUI.contentColor = c;
            viewHeight = r.y + r.height;
        }

        private Color GetLogContenColor(LogType logType)
        {
            switch (logType)
            {
                case LogType.Assert: return Color.gray;
                case LogType.Error: return Color.magenta;
                case LogType.Exception: return Color.red;
                case LogType.Warning: return Color.yellow;
                default: return GUI.contentColor;
            }
        }

        /// <summary>
        /// 获取滚动视图高度
        /// </summary>
        /// <returns></returns>
        protected override float GetScrollViewHeight()
        {
            return viewHeight;
        }

        class LogInfo
        {
            public bool selected { get; set; }

            public string condition { get; private set; }

            public string stackTrace { get; private set; }

            public LogType logType { get; private set; }

            public int conditionLines { get; private set; }

            public int stackTraceLines { get; set; }

            public LogInfo(string condition, string stackTrace, LogType logType)
            {
                selected = false;
                this.condition = condition;
                this.stackTrace = stackTrace;
                this.logType = logType;
                conditionLines = this.condition.Split('\n').Length;
                stackTraceLines = this.stackTrace.Split('\n').Length;
            }

            public override string ToString() => condition + "\n" + stackTrace;
        }
    }
}
