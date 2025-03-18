using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginTools.GameObjects;
using XCSJ.PluginTools.Notes.Dimensionings;
using XCSJ.PluginTools.Draggers;
using XCSJ.PluginXGUI.Windows.ColorPickers;
using XCSJ.PluginXGUI.Windows.MiniMaps;
using XCSJ.Scripts;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils.Interactions;

namespace XCSJ.PluginTools.CNScripts
{
    /// <summary>
    /// 脚本ID
    /// </summary>
    [Name("脚本ID")]
    public enum EScriptID
    {
        /// <summary>
        /// 结束
        /// </summary>
        _Begin = IDRange.Begin,

        #region 工具库-目录
        /// <summary>
        /// 工具库
        /// </summary>
        [ScriptName("工具库", nameof(ToolsExtension), EGrammarType.Category)]
        [ScriptDescription("工具库的相关脚本目录；")]
        #endregion
        ToolsExtension,

        #region 跟踪陀螺仪
        /// <summary>
        /// 跟踪陀螺仪
        /// </summary>
        [ScriptName("跟踪陀螺仪", nameof(TrackGyro))]
        [ScriptDescription("跟踪陀螺仪;")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.Bool, "是否跟踪:")]
        #endregion 跟踪陀螺仪
        TrackGyro,

        #region 跟踪陀螺仪(指定游戏对象)
        /// <summary>
        /// 跟踪陀螺仪
        /// </summary>
        [ScriptName("跟踪陀螺仪(指定游戏对象)", nameof(TrackGyroWithGameObject))]
        [ScriptDescription("跟踪陀螺仪(指定游戏对象);")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObject, "游戏对象:")]
        [ScriptParams(2, EParamType.Bool, "是否跟踪:")]
        #endregion 跟踪陀螺仪
        TrackGyroWithGameObject,

        #region GL线框渲染器控制
        [ScriptName("GL线框渲染器控制", nameof(GLWireFrameRendererControl))]
        [ScriptDescription("GL线框渲染器控制")]
        [ScriptReturn("成功返回 #True ;失败返回 #False;")]
        [ScriptParams(1, EParamType.Combo, "操作:", "启动", "停止")]
        #endregion
        GLWireFrameRendererControl,

        #region 标注

        #region 开启标注点拾取
        /// <summary>
        /// 开始标注点拾取
        /// </summary>
        [ScriptName("开启标注点拾取", nameof(BeginNotePointPicker))]
        [ScriptDescription("通过鼠标点击场景，拾取标注的起点、终点或者中心等坐标")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObjectComponent, "点击点拾取器", typeof(ClickPointPicker))]
        #endregion
        BeginNotePointPicker,

        #region 结束标注点拾取
        /// <summary>
        /// 开始标注点拾取
        /// </summary>
        [ScriptName("结束标注点拾取", nameof(EndNotePointPicker))]
        [ScriptDescription("结束标注点拾取")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObjectComponent, "点击点拾取器", typeof(ClickPointPicker))]
        #endregion
        EndNotePointPicker,

        #endregion

        #region 颜色拾取器

        #region 获取调色板颜色
        /// <summary>
        /// 获取调色板颜色
        /// </summary>
        [ScriptName("获取调色板颜色", nameof(GetColorPickerColor))]
        [ScriptDescription("获取调色板颜色")]
        [ScriptReturn("成功返回 颜色值 ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObjectComponent, "调色板", typeof(ColorPicker))]
        #endregion
        GetColorPickerColor,

        #region 设置调色板颜色
        /// <summary>
        /// 设置调色板颜色
        /// </summary>
        [ScriptName("设置调色板颜色", nameof(SetColorPickerColor))]
        [ScriptDescription("设置调色板颜色")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObjectComponent, "调色板", typeof(ColorPicker))]
        [ScriptParams(2, EParamType.Color, "颜色")]
        #endregion
        SetColorPickerColor,

        #region 同步渲染器颜色至调色板
        /// <summary>
        /// 同步渲染器颜色至颜色拾取器
        /// </summary>
        [ScriptName("同步渲染器颜色至调色板", nameof(SetRendererToColorPicker))]
        [ScriptDescription("同步渲染器颜色至调色板")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObjectComponent, "游戏对象", typeof(Renderer))]
        [ScriptParams(2, EParamType.GameObjectComponent, "调色板", typeof(ColorPicker))]
        #endregion
        SetRendererToColorPicker,

        #region 设置颜色绑定器模式
        /// <summary>
        /// 设置颜色绑定器模式
        /// </summary>
        [ScriptName("设置颜色绑定器模式", nameof(SetColorPickerBinderMode))]
        [ScriptDescription("设置颜色绑定器模式")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObjectComponent, "颜色绑定器", typeof(ColorPickerBinder))]
        [ScriptParams(2, EParamType.Combo, "类型:", "游戏对象列表", "选择集")]
        #endregion
        SetColorPickerBinderMode,

        #region 同步颜色绑定器颜色至颜色拾取器
        /// <summary>
        /// 同步颜色绑定器颜色至颜色拾取器
        /// </summary>
        [ScriptName("同步颜色绑定器颜色至调色板", nameof(SetColorPickerBinderToColorPicker))]
        [ScriptDescription("同步颜色绑定器颜色至调色板")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObjectComponent, "调色板", typeof(ColorPickerBinder))]
        #endregion
        SetColorPickerBinderToColorPicker,

        #region 颜色绑定器游戏对象操作
        /// <summary>
        /// 颜色绑定器游戏对象操作
        /// </summary>
        [ScriptName("颜色绑定器游戏对象操作", nameof(ColorPickerBinderGameObjectOperation))]
        [ScriptDescription("颜色绑定器游戏对象操作")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObjectComponent, "调色板", typeof(ColorPickerBinder))]
        [ScriptParams(2, EParamType.Combo, "类型:", "添加绑定游戏对象", "移除绑定游戏对象", "清空所有绑定游戏对象")]
        [ScriptParams(3, EParamType.GameObject, "游戏对象")]
        #endregion
        ColorPickerBinderGameObjectOperation,

        #endregion

        #region 导航图

        #region 导航项操作
        /// <summary>
        /// 导航项操作
        /// </summary>
        [ScriptName("导航项操作", nameof(MiniMapItemOperation))]
        [ScriptDescription("导航项操作")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObjectComponent, "导航图", typeof(MiniMap))]
        [ScriptParams(2, EParamType.Combo, "类型:", "添加", "移除")]
        [ScriptParams(3, EParamType.GameObject, "游戏对象")]
        [ScriptParams(4, EParamType.GameObjectComponent, "关联UGUI", typeof(RectTransform))]
        #endregion
        MiniMapItemOperation,

        #region 设置导航图传送规则
        /// <summary>
        /// 设置导航图传送规则
        /// </summary>
        [ScriptName("设置导航图传送规则", nameof(SetMiniMapTeleportRule))]
        [ScriptDescription("设置导航图传送规则")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObjectComponent, "导航图", typeof(MiniMapTeleport))]
        [ScriptParams(2, EParamType.Combo, "传送规则:", "无", "传送需射线撞击")]
        #endregion
        SetMiniMapTeleportRule,


        #region 设置导航图传送高度偏移量
        /// <summary>
        /// 设置导航图传送高度偏移量
        /// </summary>
        [ScriptName("设置导航图传送高度偏移量", nameof(SetMiniMapTeleportHeightOffset))]
        [ScriptDescription("设置导航图传送高度偏移量")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObjectComponent, "导航图", typeof(MiniMapTeleport))]
        [ScriptParams(2, EParamType.Float, "高度偏移量:")]
        #endregion
        SetMiniMapTeleportHeightOffset,

        #endregion

        #region 交互

        #region 交互-目录
        /// <summary>
        /// 交互
        /// </summary>
        [ScriptName("交互", nameof(Interaction), EGrammarType.Category)]
        #endregion
        Interaction,

        #region 执行交互
        /// <summary>
        /// 执行交互
        /// </summary>
        [ScriptName("执行交互", nameof(ExcuteInteract))]
        [ScriptDescription("执行交互", "Try Interact")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObjectComponent, "交互器", typeof(InteractObject))]
        [ScriptParams(2, EParamType.String, "命令")]
        [ScriptParams(3, EParamType.GameObjectComponent, "可交互对象", typeof(InteractableVirtual))]
        #endregion
        ExcuteInteract,

        #region 设置可交互实体选择
        /// <summary>
        /// 设置可交互实体选择
        /// </summary>
        [ScriptName("设置可交互实体选择", nameof(SetInteractableEntitySelected))]
        [ScriptDescription("设置可交互实体选择", "Set InteractableEntity Selected")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObjectComponent, "可交互实体", typeof(InteractableEntity))]
        [ScriptParams(2, EParamType.Bool, "选择")]
        #endregion
        SetInteractableEntitySelected,

        #region 设置可交互实体激活
        /// <summary>
        /// 设置可交互实体激活
        /// </summary>
        [ScriptName("设置可交互实体激活", nameof(SetInteractableEntityActived))]
        [ScriptDescription("设置可交互实体激活", "Set InteractableEntity Actived")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObjectComponent, "可交互实体", typeof(InteractableEntity))]
        [ScriptParams(2, EParamType.Bool, "激活")]
        #endregion
        SetInteractableEntityActived,

        #endregion

        /// <summary>
        /// 当前已使用的脚本最大ID
        /// </summary>
        MaxCurrent
    }
}

