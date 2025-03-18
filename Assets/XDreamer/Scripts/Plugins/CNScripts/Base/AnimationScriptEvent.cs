using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.CNScripts;
using XCSJ.Scripts;

namespace XCSJ.Extension.CNScripts.Base
{
    /// <summary>
    /// 动画脚本事件类型
    /// </summary>
    [Name("动画脚本事件类型")]
    public enum EAnimationScriptEventType
    {
        [Name("布尔型切换事件")]
        BoolChange = 0,

        [Name("浮点数型切换事件")]
        FloatChange,

        [Name("启动时执行")]
        Start,
    }

    /// <summary>
    /// 动画脚本事件函数
    /// </summary>
    [Name("动画脚本事件函数")]
    [Serializable]
    public class AnimationScriptEventFunction : EnumFunction<EAnimationScriptEventType> { }

    /// <summary>
    /// 动画脚本事件函数集合
    /// </summary>
    [Name("动画脚本事件函数集合")]
    [Serializable]
    public class AnimationScriptEventFunctionCollection : EnumFunctionCollection<EAnimationScriptEventType, AnimationScriptEventFunction> { }

    /// <summary>
    /// 动画脚本事件
    /// </summary>
    [Serializable]
    [Name(Title)]
    [DisallowMultipleComponent]
    [AddComponentMenu(CNScriptHelper.CNScriptMenu + Title)]
    public class AnimationScriptEvent : BaseScriptEvent<EAnimationScriptEventType, AnimationScriptEventFunction, AnimationScriptEventFunctionCollection>
    {
        public const string Title = "动画脚本事件";

        [Name("布尔标识")]
        public bool boolFlag;

        public bool _boolFlag { get; private set; }

        [Name("浮点数标识")]
        public float floatFlag;

        public float _floatFlag { get; private set; }

        protected override void Start()
        {
            base.Start();
            _boolFlag = boolFlag;
            _floatFlag = floatFlag;
            ExecuteScriptEvent(EAnimationScriptEventType.Start);
        }

        protected virtual void Update()
        {
            if (_boolFlag != boolFlag)
            {
                //Debug.Log(name + " , b :  " + boolFlag.ToString());
                _boolFlag = boolFlag;
                ExecuteScriptEvent(EAnimationScriptEventType.BoolChange, ScriptOption.ReturnValueFlag + boolFlag.ToString());
            }

            if (!Mathf.Approximately(_floatFlag, floatFlag))
            {
                //Debug.Log(name + " , b :  " + floatFlag.ToString());
                _floatFlag = floatFlag;
                ExecuteScriptEvent(EAnimationScriptEventType.FloatChange, floatFlag.ToString());
            }
        }

    }
}
