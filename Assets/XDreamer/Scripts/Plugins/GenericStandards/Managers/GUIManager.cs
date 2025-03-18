using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.Scripts;

namespace XCSJ.Extension.GenericStandards.Managers
{
    /// <summary>
    /// GUI管理器
    /// </summary>
    public static class GUIManager
    {
        private static Dictionary<int, GUIWindowInfo> windowDicionary = new Dictionary<int, GUIWindowInfo>();

        private static List<TextBuddle> textBuddles = new List<TextBuddle>();

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static void Init()
        {
            windowDicionary.Clear();
            textBuddles.Clear();
        }

        /// <summary>
        /// 销毁
        /// </summary>
        public static void Release()
        {
            windowDicionary.Clear();
            textBuddles.Clear();
        }

        /// <summary>
        /// 调用中文脚本绘制GUI窗口的函数
        /// </summary>
        /// <param name="windowID"></param>
        public static void GUIWindowFunction(int windowID)
        {
            GUIWindowInfo info;
            if (windowDicionary.TryGetValue(windowID, out info) && info != null)
            {
                ScriptManager.CallUDF(info.funName, windowID.ToString(), info.varCollectionHost);
                if (info.dragWindow) GUI.DragWindow();
            }
        }

        /// <summary>
        /// 创建或更新窗口
        /// </summary>
        /// <param name="windowID"></param>
        /// <param name="funName"></param>
        /// <param name="dragWindow"></param>
        /// <param name="varCollectionHost"></param>
        public static void CreateOrUpdateWindow(int windowID, string funName, bool dragWindow, IVarCollectionHost varCollectionHost)
        {
            GUIWindowInfo info;
            if (windowDicionary.TryGetValue(windowID, out info) && info != null)
            {
                info.Init(windowID, funName, dragWindow, varCollectionHost);
            }
            else
            {
                info = new GUIWindowInfo(windowID, funName, dragWindow, varCollectionHost);
                windowDicionary[windowID] = info;
            }
        }

        /// <summary>
        /// 绘制文字泡
        /// </summary>
        public static void OnGUI()
        {
            for (int i = 0; i < textBuddles.Count; ++i)
            {
                var tb = textBuddles[i];
                if (tb == null || !tb.OnGUI())
                {
                    //生命周期到了那么移除！
                    textBuddles.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// 创建文字泡
        /// </summary>
        /// <param name="text"></param>
        /// <param name="pos"></param>
        /// <param name="type"></param>
        /// <param name="lifeTime"></param>
        /// <param name="textAnchor"></param>
        /// <returns></returns>
        public static TextBuddle CreateTextBuddle(string text, Vector3 pos, ECoordinateType type, float lifeTime, TextAnchor textAnchor)
        {
            TextBuddle tb = new TextBuddle();
            tb.text = text;
            tb.position = pos;
            tb.coordinateType = type;
            tb.lifeTime = lifeTime;
            tb.textAnchor = textAnchor;
            textBuddles.Add(tb);
            return tb;
        }
    }

    /// <summary>
    /// GUI窗口信息
    /// </summary>
    internal class GUIWindowInfo
    {
        public int windowID;

        /// <summary>
        /// 函数名称
        /// </summary>
        public string funName;

        /// <summary>
        /// 是否可拖拽窗口
        /// </summary>
        public bool dragWindow;

        /// <summary>
        /// 变量处理
        /// </summary>
        public IVarCollectionHost varCollectionHost;

        public GUIWindowInfo(int windowID, string funName, bool dragWindow, IVarCollectionHost varCollectionHost)
        {
            Init(windowID, funName, dragWindow, varCollectionHost);
        }

        public void Init(int windowID, string funName, bool dragWindow, IVarCollectionHost varCollectionHost)
        {
            this.windowID = windowID;
            this.funName = funName;
            this.dragWindow = dragWindow;
            this.varCollectionHost = varCollectionHost;
        }
    }

    /// <summary>
    /// 文字泡
    /// </summary>
    [System.Serializable]
    [Name("文字泡")]
    public class TextBuddle
    {
        /// <summary>
        /// 默认风格
        /// </summary>
        public static GUIStyle DefaultStyle => GUI.skin.box;

        /// <summary>
        /// 文本
        /// </summary>
        [TextArea]
        public string text = "";

        /// <summary>
        /// 最小限制
        /// </summary>
        public Vector2 minLimit = new Vector2(50, 22);

        /// <summary>
        /// 是否死亡，即生命时间已耗尽
        /// </summary>
        public bool isDead { get { return lifeTime <= 0; } }

        /// <summary>
        /// 生命时间
        /// </summary>
        public float lifeTime = 1;

        /// <summary>
        /// 位置
        /// </summary>
        public Vector3 position = new Vector3(Screen.width / 2, Screen.height / 2, 0);

        /// <summary>
        /// 位置的坐标类型
        /// </summary>
        public ECoordinateType coordinateType = ECoordinateType.GUI;

        /// <summary>
        /// 标签
        /// </summary>
        public object tag { get; set; } = null;

        /// <summary>
        /// 文本锚点
        /// </summary>
        public TextAnchor textAnchor = TextAnchor.MiddleCenter;

        /// <summary>
        /// 绘制GUI前回调
        /// </summary>
        public event Action<TextBuddle> onPreGUI;

        /// <summary>
        /// 绘制GUI时回调，如果返回值为True，则不调用默认绘制；
        /// </summary>
        public event Func<TextBuddle, bool> onGUI;

        /// <summary>
        /// 死亡时回调，即生命时间已耗尽时回调
        /// </summary>
        public event Action<TextBuddle> onDead;

        /// <summary>
        /// 构造函数
        /// </summary>
        public TextBuddle()
        {
            //
        }

        /// <summary>
        /// 如果在声明周期内渲染那么返回True，否则返回false；
        /// </summary>
        /// <returns>返回True，表示成功绘制；返回False，表示绘制失败（即生命时间已耗尽）</returns>
        public bool OnGUI()
        {
            onPreGUI?.Invoke(this);

            if (isDead)
            {
                onDead?.Invoke(this);
                return false;
            }
            lifeTime -= Time.deltaTime;

            if (onGUI != null && onGUI.Any(this)) return true;

            DefaultGUI();

            return true;
        }

        /// <summary>
        /// 默认GUI的绘制
        /// </summary>
        public void DefaultGUI()
        {
            Vector3 pos = CommonFun.ConvertToGUIPoint(position, coordinateType);
            Vector2 size = DefaultStyle.CalcScreenSize(DefaultStyle.CalcSize(new GUIContent(text)));
            float w = Mathf.Max(minLimit.x, size.x);
            float h = Mathf.Max(minLimit.y, size.y);
            Rect rect = new Rect(pos.x - w / 2, pos.y - h, w, h);
            DefaultStyle.alignment = textAnchor;
            if (lifeTime < 1)
            {
                Color backgroundColor = GUI.backgroundColor;
                Color bc = backgroundColor;
                bc.a = lifeTime;
                GUI.backgroundColor = bc;
                GUI.Box(rect, text, DefaultStyle);
                GUI.backgroundColor = backgroundColor;
            }
            else
            {
                GUI.Box(rect, text, DefaultStyle);
            }
        }
    }
}
