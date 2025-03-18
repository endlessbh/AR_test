using UnityEngine;
using XCSJ.Interfaces;
using XCSJ.PluginCommonUtils;

namespace XCSJ.Extension.Base.Inputs
{
    /// <summary>
    /// 输入接口
    /// </summary>
    public interface IInput : IName, IReset
    {
        #region 获取

        /// <summary>
        /// 输入
        /// </summary>
        EInput input { get; }

        /// <summary>
        /// 任意按键
        /// </summary>
        bool anyKey { get; }

        /// <summary>
        /// 任意按键按下
        /// </summary>
        bool anyKeyDown { get; }

        /// <summary>
        /// 按键是否触发，只要当前键按下状态未改变，一直为true
        /// </summary>
        /// <param name="buttonName"></param>
        /// <returns></returns>
        bool GetButton(string buttonName);

        /// <summary>
        /// 按键是否按下
        /// </summary>
        /// <param name="buttonName"></param>
        /// <returns></returns>
        bool GetButtonDown(string buttonName);

        /// <summary>
        /// 按键是否弹起
        /// </summary>
        /// <param name="buttonName"></param>
        /// <returns></returns>
        bool GetButtonUp(string buttonName);

        /// <summary>
        /// 获取滑动轴的值
        /// </summary>
        /// <param name="axisName"></param>
        /// <returns></returns>
        float GetAxis(string axisName);

        /// <summary>
        /// 获取滑动轴的值,不连续的值
        /// </summary>
        /// <param name="axisName"></param>
        /// <returns></returns>
        float GetAxisRaw(string axisName);

        /// <summary>
        /// 按键是否有操作
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool GetKey(string keyName);

        /// <summary>
        /// 按键是否有操作
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool GetKey(KeyCode key);

        /// <summary>
        /// 按键是否按下
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool GetKeyDown(string keyName);

        /// <summary>
        /// 按键是否按下
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool GetKeyDown(KeyCode key);

        /// <summary>
        /// 按键是否弹起
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool GetKeyUp(string keyName);

        /// <summary>
        /// 按键是否弹起
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool GetKeyUp(KeyCode key);

        /// <summary>
        /// 检测鼠标按键是否有操作
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        bool GetMouseButton(int button);

        /// <summary>
        /// 鼠标按键是否按下
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        bool GetMouseButtonDown(int button);

        /// <summary>
        /// 鼠标按键是否弹起
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        bool GetMouseButtonUp(int button);

        #endregion

        #region 更新

        /// <summary>
        /// 更新按钮
        /// </summary>
        /// <param name="name"></param>
        /// <param name="downOrUp"></param>
        /// <returns></returns>
        void UpdateButton(string name, bool downOrUp);

        /// <summary>
        /// 更新轴
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        void UpdateAxis(string name, float value);

        #endregion
    }

    /// <summary>
    /// 模拟输入更新器接口
    /// </summary>
    public interface IAnalogInputUpdater
    {
        /// <summary>
        /// 更新轴
        /// </summary>
        /// <param name="input"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        void UpdateAxis(IInput input, string name, float value);

        /// <summary>
        /// 更新按钮
        /// </summary>
        /// <param name="input"></param>
        /// <param name="name"></param>
        /// <param name="downOrUp"></param>
        void UpdateButton(IInput input, string name, bool downOrUp);
    }
}
