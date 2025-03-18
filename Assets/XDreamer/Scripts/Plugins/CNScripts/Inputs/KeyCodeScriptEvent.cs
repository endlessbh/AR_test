using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.CNScripts;

namespace XCSJ.Extension.CNScripts.Inputs
{
    /// <summary>
    /// 键码脚本事件函数
    /// </summary>
    [Name("键码脚本事件函数")]
    [Serializable]
    public class KeyCodeScriptEventFunction : EnumFunction<KeyCode> { }

    /// <summary>
    /// 键码脚本事件函数集合
    /// </summary>
    [Name("键码脚本事件函数集合")]
    [Serializable]
    public class KeyCodeScriptEventFunctionCollection : EnumFunctionCollection<KeyCode, KeyCodeScriptEventFunction> { }

    /// <summary>
    /// 键码脚本事件
    /// </summary>
    [Serializable]
    [Name(Title)]
    [DisallowMultipleComponent]
    [AddComponentMenu(CNScriptHelper.InputMenu + Title)]
    public class KeyCodeScriptEvent : BaseScriptEvent<KeyCode, KeyCodeScriptEventFunction, KeyCodeScriptEventFunctionCollection>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "键码脚本事件";

        /// <summary>
        /// 更新
        /// </summary>
        protected virtual void Update()
        {
            //float begin = Time.realtimeSinceStartup; DateTime dtb = DateTime.Now;            
            //foreach (var e in Enum.GetValues(typeof(KeyCode)))
            if (Input.anyKey || Input.anyKeyDown)
            {
                foreach (var kv in _funcCollection.enumFunctionDictionary)
                {
                    KeyCode kc = kv.Key;
                    if (Input.GetKeyDown(kc))
                    {
                        ExecuteFunction(kv.Value, "按键按下");
                    }
                    else if (Input.GetKey(kc))
                    {
                        ExecuteFunction(kv.Value, "按键按下中");
                    }
                    else if (Input.GetKeyUp(kc))//按键弹起动作与按键按下中动作不可能同一frame内操作，所以加个else~
                    {
                        ExecuteFunction(kv.Value, "按键弹起");
                    }
                }
            }
            //Debug.Log("Update 耗时:" + (Time.realtimeSinceStartup - begin).ToString() + " , DateTime: " + (DateTime.Now - dtb).TotalMilliseconds);
            //base.Update();
        }
    }
}
