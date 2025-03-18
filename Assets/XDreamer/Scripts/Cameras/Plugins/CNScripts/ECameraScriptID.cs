using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginsCameras.Base;
using XCSJ.Scripts;
using IDRange = XCSJ.PluginCamera.IDRange;

namespace XCSJ.PluginsCameras.CNScripts
{
    /// <summary>
    /// 相机脚本ID枚举
    /// </summary>
    [Name("相机脚本ID")]
    public enum ECameraScriptID
    {
        /// <summary>
        /// 开始
        /// </summary>
        _Begin = IDRange.Begin,

        #region 新版相机

        #region 新版相机-目录
        /// <summary>
        /// 相机
        /// </summary>
        [ScriptName("相机", nameof(Cameras), EGrammarType.Category)]
        [ScriptDescription("相机操作的目录")]
        #endregion
        Cameras,

        #region 切换相机控制器

        /// <summary>
        /// 切换相机控制器:从当前切换相机控制器切换至指定的目标相机控制器
        /// </summary>
        [ScriptName("切换相机控制器", nameof(SwitchCameraController))]
        [ScriptDescription("从当前切换相机控制器切换至指定的目标相机控制器")]
        [ScriptReturn("返回值为空", "Return null")]
        [ScriptParams(1, EParamType.GameObject, "目标相机控制器对象:", typeof(BaseCameraMainController))]
        [ScriptParams(2, EParamType.FloatSlider, "过渡时间:", 0f, 10f)]
        [ScriptParams(3, EParamType.UserDefineFun, "回调函数:")]
        [ScriptParams(4, EParamType.Bool, "强制切换:")]
        [ScriptParams(5, EParamType.Combo, "切换规则:", "使用目标相机控制器对象", "上一个相机", "下一个相机")]
        #endregion
        SwitchCameraController,

        #region 切换相机控制器

        /// <summary>
        /// 切换相机控制器(按名称):从当前切换相机控制器切换至指定名称的目标相机控制器
        /// </summary>
        [ScriptName("切换相机控制器(按名称)", nameof(SwitchCameraControllerByName))]
        [ScriptDescription("从当前切换相机控制器切换至指定名称的目标相机控制器")]
        [ScriptReturn("返回值为空", "Return null")]
        [ScriptParams(1, CameraControllerName_ScriptParam.ScriptParamType, "目标相机控制器对象名称:", typeof(BaseCameraMainController))]
        [ScriptParams(2, EParamType.FloatSlider, "过渡时间:", 0f, 10f)]
        [ScriptParams(3, EParamType.UserDefineFun, "回调函数:")]
        [ScriptParams(4, EParamType.Bool, "强制切换:")]
        [ScriptParams(5, EParamType.Combo, "切换规则:", "使用目标相机控制器对象名称", "上一个相机", "下一个相机")]
        #endregion
        SwitchCameraControllerByName,

        /// <summary>
        /// 设置相机控制器主目标
        /// </summary>
        [ScriptName("设置相机控制器主目标", nameof(SetCameraControllerMainTarget))]
        [ScriptDescription("设置相机控制器主目标；如相机控制器查找规则为‘指定’时，使用‘相机控制器’参数作为处理对象；如相机控制器查找规则为‘当前’时，使用当前相机控制器作为处理对象")]
        [ScriptReturn("成功返回#True；失败返回#False；")]
        [ScriptParams(1, EParamType.GameObject, "相机控制器:", typeof(BaseCameraMainController))]
        [ScriptParams(2, EParamType.GameObject, "主目标:", typeof(Transform))]
        [ScriptParams(10, EParamType.Combo, "相机控制器查找规则:", "指定", "当前")]
        SetCameraControllerMainTarget,

        /// <summary>
        /// 设置相机控制器速度系数
        /// </summary>
        [ScriptName("设置相机控制器速度系数", nameof(SetCameraControllerSpeedCoefficient))]
        [ScriptDescription("设置相机控制器速度系数；如相机控制器查找规则为‘指定’时，使用‘相机控制器’参数作为处理对象；如相机控制器查找规则为‘当前’时，使用当前相机控制器作为处理对象")]
        [ScriptReturn("成功返回#True；失败返回#False；")]
        [ScriptParams(1, EParamType.GameObject, "相机控制器:", typeof(BaseCameraMainController))]
        [ScriptParams(2, EParamType.Vector3, "速度系数:")]
        [ScriptParams(3, EParamType.Combo, "速度系数类型:", "移动", "旋转")]
        [ScriptParams(10, EParamType.Combo, "相机控制器查找规则:", "指定", "当前")]
        SetCameraControllerSpeedCoefficient,

        /// <summary>
        /// 设置相机控制器阻尼系数
        /// </summary>
        [ScriptName("设置相机控制器阻尼系数", nameof(SetCameraControllerDampingCoefficient))]
        [ScriptDescription("设置相机控制器阻尼系数；如相机控制器查找规则为‘指定’时，使用‘相机控制器’参数作为处理对象；如相机控制器查找规则为‘当前’时，使用当前相机控制器作为处理对象")]
        [ScriptReturn("成功返回#True；失败返回#False；")]
        [ScriptParams(1, EParamType.GameObject, "相机控制器:", typeof(BaseCameraMainController))]
        [ScriptParams(2, EParamType.FloatSlider, "阻尼系数:", 0f, CameraHelperExtension.MaxDampingCoefficient)]
        [ScriptParams(3, EParamType.Combo, "阻尼系数类型:", "移动", "旋转")]
        [ScriptParams(10, EParamType.Combo, "相机控制器查找规则:", "指定", "当前")]
        SetCameraControllerDampingCoefficient,

        /// <summary>
        /// 恢复相机控制器状态
        /// </summary>
        [ScriptName("恢复相机控制器状态", nameof(RecoverCameraControllerState))]
        [ScriptDescription("恢复相机控制器状态；如相机控制器查找规则为‘指定’时，使用‘相机控制器’参数作为处理对象；如相机控制器查找规则为‘当前’时，使用当前相机控制器作为处理对象")]
        [ScriptReturn("成功返回#True；失败返回#False；")]
        [ScriptParams(1, EParamType.GameObject, "相机控制器:", typeof(BaseCameraMainController))]
        [ScriptParams(2, EParamType.Combo, "状态类型:", "原始状态", "上一次状态")]
        [ScriptParams(10, EParamType.Combo, "相机控制器查找规则:", "指定", "当前")]
        RecoverCameraControllerState,

        #endregion

        /// <summary>
        /// 当前已使用的脚本最大ID
        /// </summary>
        MaxCurrent,

        /// <summary>
        /// 结束
        /// </summary>
        _End = IDRange.End,
    }

    /// <summary>
    /// 相机控制器名称脚本参数
    /// </summary>
    [ScriptParamType(ScriptParamType)]
    public class CameraControllerName_ScriptParam : StringScriptParam
    {
        /// <summary>
        /// 脚本参数类型
        /// </summary>
        public const int ScriptParamType = IDRange.Begin;
    }
}

