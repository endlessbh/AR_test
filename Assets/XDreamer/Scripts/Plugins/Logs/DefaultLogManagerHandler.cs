using System.Collections.Generic;
using System.Text;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.CNScripts;
using XCSJ.Scripts;

namespace XCSJ.Extension.Logs
{
    /// <summary>
    /// 默认日志管理器处理器
    /// </summary>
    public class DefaultLogManagerHandler : InstanceClass<DefaultLogManagerHandler>, PluginCommonUtils.Base.Kernel.ILogHandler
    {
        static DefaultLogManagerHandler()
        {
            Log.onLog += LogHandle;
            XDreamer.onAwake += OnAwake;
            XDreamer.onDestroy += OnDestroy;
        }

        private static void OnAwake()
        {
            //在编辑态不做处理
#if UNITY_EDITOR
            if (!Application.isPlaying) return;
#endif
            //Application.logMessageReceived += LogViewerWindow.OnLogCallback;
            Application.logMessageReceivedThreaded -= LogViewerWindow.OnLogCallbackThreaded;
            Application.logMessageReceivedThreaded += LogViewerWindow.OnLogCallbackThreaded;
        }

        private static void OnDestroy()
        {
            //Application.logMessageReceived -= LogViewerWindow.OnLogCallback;
            Application.logMessageReceivedThreaded -= LogViewerWindow.OnLogCallbackThreaded;
        }

        /// <summary>
        /// 获取脚本
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        public List<Script> GetScripts(LogManager manager) => Script.GetScriptsOfEnum<ELogScriptID>(manager);

        /// <summary>
        /// 执行脚本
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="id"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public ReturnValue ExecuteScript(LogManager manager, int id, ScriptParamList param)
        {
            switch ((ELogScriptID)id)
            {
                case ELogScriptID.OutputUnityLog:
                    {
                        string info = string.Format("{0}{1}{2}{3}{4}{5}{6}", param[2], param[3], param[4], param[5], param[6], param[7], param[8]);
                        switch (param[1] as string)
                        {
                            case "调试": Log.Output(info, ELogLevel.Debug); break;
                            case "警告": Log.Output(info, ELogLevel.Warning); break;
                            case "错误": Log.Output(info, ELogLevel.Error); break;
                            default: Log.Output(info, ELogLevel.Info); break;
                        }
                        return ReturnValue.True(info);
                    }
                case ELogScriptID.ShowLogInfoWindow:
                    {
                        LogViewerWindow.SetVisable((EBool)param[1]);
                        return ReturnValue.Yes;
                    }
            }
            return ReturnValue.No;
        }

        /// <summary>
        /// 日志处理
        /// </summary>
        /// <param name="logInfo"></param>
        private static void LogHandle(ILogInfo logInfo)
        {
            LogHandle(logInfo, LogManager.instance ? LogManager.instance.logInfoOption : LogInfoOption.instance);
        }

        /// <summary>
        /// 日志处理
        /// </summary>
        /// <param name="logInfo"></param>
        /// <param name="option"></param>
        public static void LogHandle(ILogInfo logInfo, LogInfoOption option)
        {
            if (logInfo.level < option.outputLevel) return;

            StringBuilder sb = new StringBuilder();
            if (option.withIndex)
            {
                sb.AppendFormat("[{0}]", logInfo.index);
            }
            if (option.withType)
            {
                sb.AppendFormat("[{0}]", logInfo.level);
            }
            if (option.withDateTime)
            {
                sb.AppendFormat("[{0}]", logInfo.dateTime.ToString(option.withoutDate ? DateTimeHelper.TimeFormatLong : DateTimeHelper.FormatLong));
            }
            if (option.withFrameCount)
            {
                if (option.showRemainderOfFrameCountBy1000)
                {
                    sb.AppendFormat("[{0:000}]", frameCount % 1000);
                }
                else
                {
                    sb.AppendFormat("[{0}]", frameCount);
                }
            }
            sb.AppendFormat(": {0}", logInfo.info);
            var logString = sb.ToString();
            switch (logInfo.level)
            {
                case ELogLevel.Debug:
                    {
                        Debug.Log(logString);
                        break;
                    }
                case ELogLevel.Info:
                    {
                        Debug.Log(logString);
                        break;
                    }
                case ELogLevel.Warning:
                    {
                        Debug.LogWarning(logString);
                        break;
                    }
                case ELogLevel.Error:
                case ELogLevel.Exception:
                case ELogLevel.Fatal:
                default:
                    {
                        Debug.LogError(logString);
                        break;
                    }
            }
        }

        private static volatile int _frameCount = 0;

        /// <summary>
        /// 帧数目
        /// </summary>
        public static int frameCount
        {
            get
            {
                try
                {
                    return _frameCount = Time.frameCount;
                }
                catch { return _frameCount; }
            }
        }
    }

    /// <summary>
    /// 日志脚本ID
    /// </summary>
    [Name("日志脚本ID")]
    public enum ELogScriptID
    {
        /// <summary>
        /// 开始
        /// </summary>
        _Begin = EScriptID.MaxCurrent + 1,

        #region 日志-目录
        /// <summary>
        /// 日志
        /// </summary>
        [ScriptName("日志", nameof(Log), EGrammarType.Category)]
        #endregion
        Log,

        #region 输出日志信息
        /// <summary>
        /// 输出日志信息
        /// 1：string
        /// </summary>
        [ScriptName("输出日志信息", nameof(OutputUnityLog))]
        [ScriptDescription("输出日志信息；在编辑器状态下输出到控制台；功能即为Unity 中类 Debug 的调试信息输出功能；运行状态时可将信息输出到专用的日志窗口中！其中级别为‘信息’的是普通信息级别；")]
        [ScriptReturn("成功返回 日志信息 的字符串 ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.Combo, "信息级别:", "调试", "信息", "警告", "错误", defaultObject = "信息")]
        [ScriptParams(2, EParamType.String, "信息1:")]
        [ScriptParams(3, EParamType.String, "信息2:")]
        [ScriptParams(4, EParamType.String, "信息3:")]
        [ScriptParams(5, EParamType.String, "信息4:")]
        [ScriptParams(6, EParamType.String, "信息5:")]
        [ScriptParams(7, EParamType.String, "信息6:")]
        [ScriptParams(8, EParamType.String, "信息7:")]
        #endregion
        OutputUnityLog,

        /// <summary>
        /// 显示日志信息窗口
        /// </summary>
        [ScriptName("显示日志信息窗口", nameof(ShowLogInfoWindow))]
        [ScriptParams(1, EParamType.Bool, "是否显示日志信息输出窗口:", defaultObject = EBool.No)]
        ShowLogInfoWindow,

        /// <summary>
        /// 结束
        /// </summary>
        _End,
    }
}
