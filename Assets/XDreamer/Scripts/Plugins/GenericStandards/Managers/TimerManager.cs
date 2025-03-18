using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.PluginCommonUtils;
using XCSJ.Scripts;

namespace XCSJ.Extension.GenericStandards.Managers
{
    /// <summary>
    /// 定时器管理器
    /// </summary>
    public static class TimerManager
    {
        /// <summary>
        /// 定时器字典
        /// </summary>
        public static Dictionary<string, TimerData> timerDictionary { get; } = new Dictionary<string, TimerData>();

        /// <summary>
        /// 初始化
        /// </summary>
        public static void Init() => Kill();

        /// <summary>
        /// 销毁
        /// </summary>
        public static void Release() => Kill();

        /// <summary>
        /// 删除
        /// </summary>
        public static void Kill()
        {
            var list = timerDictionary.Values.ToList();
            timerDictionary.Clear();
            foreach (var timer in list)
            {
                timer?.Stop();
            }
        }

        /// <summary>
        /// 暂停
        /// </summary>
        public static void Pause()
        {
            foreach (var kv in timerDictionary)
            {
                kv.Value.pause = true;
            }
        }

        /// <summary>
        /// 恢复
        /// </summary>
        public static void Resume()
        {
            foreach (var kv in timerDictionary)
            {
                kv.Value.pause = false;
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="timerName"></param>
        /// <returns></returns>
        public static bool Kill(string timerName)
        {
            if (string.IsNullOrEmpty(timerName)) return false;
            if (timerDictionary.TryGetValue(timerName, out var timer))
            {
                timer.Stop();
                return true;
            }
            return false;
        }

        /// <summary>
        /// 暂停
        /// </summary>
        /// <param name="timerName"></param>
        /// <returns></returns>
        public static bool Pause(string timerName)
        {
            if (string.IsNullOrEmpty(timerName)) return false;
            if (timerDictionary.TryGetValue(timerName, out var timer))
            {
                timer.pause = true;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 恢复
        /// </summary>
        /// <param name="timerName"></param>
        /// <returns></returns>
        public static bool Resume(string timerName)
        {
            if (string.IsNullOrEmpty(timerName)) return false;
            if (timerDictionary.TryGetValue(timerName, out var timer))
            {
                timer.pause = false;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 创建定时器
        /// </summary>
        /// <param name="timerName"></param>
        /// <param name="asyncMono"></param>
        /// <param name="functionName"></param>
        /// <param name="param"></param>
        /// <param name="waitMillisecondTime"></param>
        /// <param name="localVarCollectionHost"></param>
        /// <param name="runCount"></param>
        /// <returns></returns>
        internal static bool CreateTimer(string timerName, MonoBehaviour asyncMono, string functionName, string param, int waitMillisecondTime, IVarCollectionHost localVarCollectionHost, int runCount)
        {
            if (string.IsNullOrEmpty(timerName) || !asyncMono || !Application.isPlaying) return false;
            if (timerDictionary.TryGetValue(timerName, out _)) return false;
            var timer = new TimerData(timerName, asyncMono, functionName, param, waitMillisecondTime, localVarCollectionHost, runCount);
            timerDictionary[timerName] = timer;
            timer.Start();
            return true;
        }
    }

    /// <summary>
    /// 定时器数据
    /// </summary>
    public class TimerData
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string name { get; private set; }

        /// <summary>
        /// 异步对象
        /// </summary>
        public MonoBehaviour asyncMono { get; private set; }

        /// <summary>
        /// 函数名
        /// </summary>
        public string functionName { get; private set; }

        /// <summary>
        /// 参数
        /// </summary>
        public string param { get; private set; }

        /// <summary>
        /// 等待秒数
        /// </summary>
        public float waitSeconds { get; private set; }

        /// <summary>
        /// 本地变量处理
        /// </summary>
        public IVarCollectionHost localVarCollectionHost { get; private set; }

        /// <summary>
        /// 运行次数
        /// </summary>
        public int runCount { get; private set; }

        /// <summary>
        /// 已运行次数
        /// </summary>
        public int counted { get; private set; } = 0;

        /// <summary>
        /// 暂停
        /// </summary>
        public bool pause { get; internal set; } = false;

        private Coroutine coroutine;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="name"></param>
        /// <param name="asyncMono"></param>
        /// <param name="functionName"></param>
        /// <param name="param"></param>
        /// <param name="waitMillisecondTime"></param>
        /// <param name="localVarCollectionHost"></param>
        /// <param name="runCount"></param>
        internal TimerData(string name, MonoBehaviour asyncMono, string functionName, string param, int waitMillisecondTime, IVarCollectionHost localVarCollectionHost, int runCount)
        {
            this.name = name;
            this.asyncMono = asyncMono;
            this.functionName = functionName;
            this.param = param;
            this.waitSeconds = Mathf.Max(waitMillisecondTime / 1000f, 0.001f);
            this.localVarCollectionHost = localVarCollectionHost;
            this.runCount = runCount;
        }

        /// <summary>
        /// 启动
        /// </summary>
        public void Start()
        {
            coroutine = asyncMono.StartCoroutine(Loop());
        }

        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {
            if (coroutine != null)
            {
                asyncMono.StopCoroutine(coroutine);
            }
            OnStop();
        }

        private void OnStop()
        {
            coroutine = null;
            TimerManager.timerDictionary.Remove(name);
        }

        private IEnumerator Loop()
        {
            while (runCount == -1 || counted < runCount)
            {
                //等待指定时间
                yield return new WaitForSeconds(waitSeconds);

                //是否暂停
                while (pause) yield return null;

                //调用函数
                ScriptManager.CallUDF(functionName, param, localVarCollectionHost);

                //计数累加
                counted++;
            }
            OnStop();
            yield break;
        }
    }
}
