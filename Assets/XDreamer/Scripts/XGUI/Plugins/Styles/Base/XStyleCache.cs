using System;
using System.Collections.Generic;
using System.Linq;
using XCSJ.Collections;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginXGUI.Styles.Base
{
    /// <summary>
    /// 样式缓存器:样式核心信息管理器，运行态和编辑器都使用它获取样式信息
    /// </summary>
    public static class XStyleCache
    {
        /// <summary>
        /// 样式集合
        /// </summary>
        public static HashSet<XStyle> styles { get; private set; } = new HashSet<XStyle>();

        /// <summary>
        /// 默认样式
        /// </summary>
        public static XStyle defaultStyle
        {
            get => _defaulttyle;
            set
            {
                if (_defaulttyle != value)
                {
                    _defaulttyle = value;
                    Register(_defaulttyle);
                    onDefaultStyleChanged?.Invoke(_defaulttyle);
                    UpdateStyle();
                }
            }
        }
        private static XStyle _defaulttyle;

        /// <summary>
        /// 设置默认样式
        /// </summary>
        /// <param name="name"></param>
        public static void SetDefaultStyle(string name)
        {
            if (defaultStyle && defaultStyle.name == name)
            {
                return;
            }
            defaultStyle = GetStyle(name);
        }

        /// <summary>
        /// 默认样式修改回调
        /// </summary>
        public static event Action<XStyle> onDefaultStyleChanged;

        /// <summary>
        /// 样式变化事件
        /// </summary>
        public static event Action onUpdateStyle;

        /// <summary>
        /// 样式更新
        /// </summary>
        public static void UpdateStyle()
        {
            onUpdateStyle?.Invoke();
        }

        /// <summary>
        /// 清理
        /// </summary>
        public static void Clear() => styles.Clear();

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="register"></param>
        public static void Register(XStyle register)
        {
            if (register && styles.Add(register))
            {
                UpdateStyle();
            }
        }

        /// <summary>
        /// 批量注册
        /// </summary>
        /// <param name="registers"></param>
        public static void Register(IEnumerable<XStyle> registers)
        {
            if (registers == null) return;

            registers.Foreach(s =>
            {
                if (s)
                {
                    styles.Add(s);
                }
            });

            // 最后进行统一刷新
            UpdateStyle();
        }

        /// <summary>
        /// 批量注销
        /// </summary>
        /// <param name="registers"></param>
        public static void Unregister(IEnumerable<XStyle> registers)
        {
            registers.Foreach(s => Unregister(s));
        }

        /// <summary>
        /// 注销
        /// </summary>
        /// <param name="register"></param>
        public static void Unregister(XStyle register)
        {
            if (register && styles.Remove(register))
            {
                UpdateStyle();
            }
        }

        /// <summary>
        /// 获取样式
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static XStyle GetStyle(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                foreach (var s in styles)
                {
                    if (s && s.name == name)
                    {
                        return s;
                    }
                }
            }
            return null;
        }


        /// <summary>
        /// 获取样式名称列表
        /// </summary>
        /// <returns></returns>
        public static string[] GetNames() => styles.WhereCast(s => s, s => s.name).ToArray();
    }
}
