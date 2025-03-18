using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Inputs;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Runtime;

namespace XCSJ.PluginTools.Others
{
    /// <summary>
    /// 应用程序退出
    /// </summary>
    [Name("应用程序退出")] 
    [XCSJ.Attributes.Icon(EIcon.Close)]
    [RequireManager(typeof(ToolsManager))]
    public class ApplicationQuit : InteractProvider
    {
        /// <summary>
        /// 退出应用程序键码:按下对应键码后会退出当前应用程序；必须选择集为空时才触发退出操作;
        /// </summary>
        [Name("退出应用程序键码")]
        [Tip("按下对应键码后会退出当前应用程序；必须选择集为空时才触发退出操作;", "Pressing the corresponding key code will exit the current application; The exit operation is triggered only when the selection set is empty;")]
        public KeyCode exitApplicationKeyCode = KeyCode.Escape;

        /// <summary>
        /// 退出应用程序计数:连续按下退出应用程序键码指定次数后才执行退出当前应用程序的操作；
        /// </summary>
        [Name("退出应用程序计数")]
        [Tip("连续按下退出应用程序键码指定次数后才执行退出当前应用程序的操作；", "Press the exit application key code continuously for a specified number of times before exiting the current application;")]
        [Range(1, 5)]
        [HideInSuperInspector("exitApplicationKeyCode", EValidityCheckType.Equal, KeyCode.None)]
        public int exitApplicationCount = 2;

#if UNITY_STANDALONE || UNITY_EDITOR
        private int _currentExitApplicationCount = 0;
#endif

        /// <summary>
        /// 更新
        /// </summary>
        public void Update()
        {
#if UNITY_STANDALONE || UNITY_EDITOR
            //退出应用程序
            if (XInput.GetKeyDown(exitApplicationKeyCode))
            {
                if (!Selection.selection && ++_currentExitApplicationCount >= exitApplicationCount)
                {
                    Application.Quit();
                    return;
                }
            }
            else if (XInput.anyKeyDown)
            {
                _currentExitApplicationCount = 0;
            }
#endif
        }
    }
}