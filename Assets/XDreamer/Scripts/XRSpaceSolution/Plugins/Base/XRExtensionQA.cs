using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Caches;
using XCSJ.Collections;
using XCSJ.ComponentModel;
using XCSJ.DataBase;
using XCSJ.Extension.Base.Maths;
using XCSJ.Extension.Base.Units;
using XCSJ.Helper;
using XCSJ.Interfaces;
using XCSJ.LitJson;
using XCSJ.Maths;
using XCSJ.PluginCommonUtils.Safety.XR;
using XCSJ.PluginStereoView.Base;
using XCSJ.PluginXBox.Base;

#if UNITY_2018_1_OR_NEWER
using UnityEngine;
#endif

namespace XCSJ.PluginXRSpaceSolution.Base
{
    /// <summary>
    /// XR扩展问题
    /// </summary>
    public abstract class XRExtensionQ : XRQuestion { }

    /// <summary>
    /// XR扩展答案
    /// </summary>
    public abstract class XRExtensionA : XRAnswer { }

    /// <summary>
    /// 重置配置答案
    /// </summary>
    [Import]
    public class ResetConfigA : XRExtensionA
    {
        /// <summary>
        /// 重置配置
        /// </summary>
        public EResetConfig resetConfig { get; set; } = EResetConfig.Pose;

        /// <summary>
        /// 动作名
        /// </summary>
        public string actionName { get; set; } = "";
    }

    /// <summary>
    /// 重置配置
    /// </summary>
    public enum EResetConfig
    {
        /// <summary>
        /// 姿态
        /// </summary>
        Pose,
    }

    /// <summary>
    /// XR空间配置答案:空间显示与跟踪交互
    /// </summary>
    [Import]
    public class XRSpaceConfigA : XRExtensionA
    {
        #region 空间配置

        internal bool isDirty { get; set; } = false;

        /// <summary>
        /// 标记脏
        /// </summary>
        public void MarkDirty() => isDirty = true;

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public XRSpaceConfigA Clone() => FromJson<XRSpaceConfigA>(this.ToJson());

        /// <summary>
        /// 当反序列化之后回调
        /// </summary>
        /// <param name="serializeContext">序列化上下文</param>
        public override void OnAfterDeserialize(ISerializeContext serializeContext)
        {
            base.OnAfterDeserialize(serializeContext);
            cameras.ForEach(i => i.SetConfig(this));
            screens.ForEach(i => i.SetConfig(this));
            actions.ForEach(i => i.SetConfig(this));
        }

        /// <summary>
        /// 构建内置配置
        /// </summary>
        public void CreateBuildinConfig()
        {
            var screen = AddScreen("屏幕前", "前");
            screen.screenPose.position = new V3F(0, 1, 2);

            AddCamera("相机前", "屏幕前");

            CreateBuildinActions();
        }

        /// <summary>
        /// 构建内置动作列表
        /// </summary>
        public void CreateBuildinActions()
        {
            foreach (var a in EnumCache<EActionName>.Array)
            {
                AddAction(NameAttribute.ValueName<NameAttribute>(EnumFieldInfoCache.GetFieldInfo(a)));
            }
        }

        #endregion

        #region 空间偏移

        /// <summary>
        /// 空间偏移
        /// </summary>
        [Browsable(false)]
        public Pose spaceOffset { get; set; } = new Pose();

        [Category("空间偏移")]
        [DisplayName("位置")]
        [Description("XR交互空间的子级对象；用于统一管理XR交互空间中客观静态存在的对象；即其位置与旋转信息均相对于XR交互空间对象进行处理；通常可以理解为XR实验室中的地面的空间原点；")]
        [Json(false)]
        public V3F spaceOffset_Position { get => spaceOffset.position; set => spaceOffset.position = value; }

        [Category("空间偏移")]
        [DisplayName("旋转")]
        [Description("XR交互空间的子级对象；用于统一管理XR交互空间中客观静态存在的对象；即其位置与旋转信息均相对于XR交互空间对象进行处理；通常可以理解为XR实验室中的地面的空间原点；")]
        [Json(false)]
        public V3F spaceOffset_Rotation { get => spaceOffset.rotation; set => spaceOffset.rotation = value; }

        #endregion

        #region 屏幕组偏移

        /// <summary>
        /// 屏幕组偏移
        /// </summary>
        [Browsable(false)]
        public Pose screenGroupOffset { get; set; } = new Pose();

        [Category("屏幕组偏移")]
        [DisplayName("位置")]
        [Description("空间偏移的子级对象；用于统一管理XR交互空间中客观静态存在的所有虚拟屏幕对象；即其位置与旋转信息均相对于空间偏移对象进行处理；通常可以理解为XR实验室中的地面上摆放的多个屏幕设备的组合体的空间原点；")]
        [Json(false)]
        public V3F screenGroupOffset_Position { get => screenGroupOffset.position; set => screenGroupOffset.position = value; }

        [Category("屏幕组偏移")]
        [DisplayName("旋转")]
        [Description("空间偏移的子级对象；用于统一管理XR交互空间中客观静态存在的所有虚拟屏幕对象；即其位置与旋转信息均相对于空间偏移对象进行处理；通常可以理解为XR实验室中的地面上摆放的多个屏幕设备的组合体的空间原点；")]
        [Json(false)]
        public V3F screenGroupOffset_Rotation { get => screenGroupOffset.rotation; set => screenGroupOffset.rotation = value; }

        #endregion

        #region 屏幕列表

        /// <summary>
        /// 屏幕列表
        /// </summary>
        [Browsable(false)]
        public List<ScreenInfo> screens { get; set; } = new List<ScreenInfo>();

        /// <summary>
        /// 获取屏幕
        /// </summary>
        /// <param name="screenName"></param>
        /// <returns></returns>
        public ScreenInfo GetScreen(string screenName) => screens.FirstOrDefault(i => i.name == screenName);

        /// <summary>
        /// 添加屏幕：存在同名的则不执行添加，并返回该同名的对象
        /// </summary>
        /// <param name="screenName"></param>
        /// <returns></returns>
        public ScreenInfo AddScreen(string screenName, string screenDisplayName)
        {
            var screenInfo = GetScreen(screenName);
            if (screenInfo != null) return screenInfo;

            screenInfo = new ScreenInfo() { name = screenName, displayName = screenDisplayName };
            screenInfo.SetConfig(this);
            screens.Add(screenInfo);
            return screenInfo;
        }

        #endregion

        #region 相机配置

        [Category("相机配置")]
        [DisplayName("相机配置")]
        [Description("XR交互空间内所有使用的Unity相机属性参数的全局配置；")]
        [Browsable(false)]
        public UnityCameraConfig unityCameraConfig { get; set; } = new UnityCameraConfig();

        [Category("相机配置")]
        [DisplayName("裁剪面-近")]
        [Description("近剪裁平面与摄影机的距离，单位为Unity世界单位，默认为米；XR交互空间内所有使用的Unity相机属性参数的全局配置；")]
        [Json(false)]
        public float nearClipPlane { get => unityCameraConfig.nearClipPlane; set => unityCameraConfig.nearClipPlane = value; }

        [Category("相机配置")]
        [DisplayName("裁剪面-远")]
        [Description("远剪裁平面与摄影机的距离，单位为Unity世界单位，默认为米；XR交互空间内所有使用的Unity相机属性参数的全局配置；")]
        [Json(false)]
        public float farClipPlane { get => unityCameraConfig.farClipPlane; set => unityCameraConfig.farClipPlane = value; }

        [Category("相机配置")]
        [DisplayName("渲染路径")]
        [Description("如果可能的话，应该使用的渲染路径；XR交互空间内所有使用的Unity相机属性参数的全局配置；")]
        [Json(false)]
        //[TypeConverter(typeof(EnumStringConverter))]
        public ERenderingPath renderingPath { get => unityCameraConfig.renderingPath; set => unityCameraConfig.renderingPath = value; }

        [Category("相机配置")]
        [DisplayName("允许HDR")]
        [Description("高动态范围渲染；XR交互空间内所有使用的Unity相机属性参数的全局配置；")]
        [Json(false)]
        [TypeConverter(typeof(BoolStringConverter))]
        public bool allowHDR { get => unityCameraConfig.allowHDR; set => unityCameraConfig.allowHDR = value; }

        [Category("相机配置")]
        [DisplayName("允许MSAA")]
        [Description("MSAA渲染；XR交互空间内所有使用的Unity相机属性参数的全局配置；")]
        [Json(false)]
        [TypeConverter(typeof(BoolStringConverter))]
        public bool allowMSAA { get => unityCameraConfig.allowMSAA; set => unityCameraConfig.allowMSAA = value; }

        [Category("相机配置")]
        [DisplayName("允许动态分辨率")]
        [Description("动态分辨率缩放；XR交互空间内所有使用的Unity相机属性参数的全局配置；")]
        [Json(false)]
        [TypeConverter(typeof(BoolStringConverter))]
        public bool allowDynamicResolution { get => unityCameraConfig.allowDynamicResolution; set => unityCameraConfig.allowDynamicResolution = value; }

        [Category("相机配置")]
        [DisplayName("立体分离")]
        [Description("虚拟眼睛之间的距离；单位为米；使用此选项可以查询或设置当前的眼睛间距；请注意，大多数VR设备都提供该值，在这种情况下，设置该值将没有任何效果；XR交互空间内所有使用的Unity相机属性参数的全局配置；")]
        [Json(false)]
        public float stereoSeparation { get => unityCameraConfig.stereoSeparation; set => unityCameraConfig.stereoSeparation = value; }

        [Category("相机配置")]
        [DisplayName("立体融合")]
        [Description("到虚拟眼睛汇聚点的距离；单位为米；XR交互空间内所有使用的Unity相机属性参数的全局配置；")]
        [Json(false)]
        public float stereoConvergence { get => unityCameraConfig.stereoConvergence; set => unityCameraConfig.stereoConvergence = value; }


        #endregion

        #region 相机控制

        [Category("相机控制")]
        [Description("XR交互空间内相机控制器属性参数的全局配置；")]
        [Browsable(false)]
        public UnityCameraControl unityCameraControl { get; set; } = new UnityCameraControl();

        [Category("相机控制")]
        [DisplayName("启用屏幕相机关联")]
        [Description("用于整体控制是否启用相机与用户自定义屏幕的关联；多用于在多屏幕（不同角度）、有姿态跟踪（即动作捕捉）设备情况下的立体显示控制；影响【相机透视】组件的【更新模式】参数；为是时，使用【自定义虚拟屏幕】做计算；为否时，使用【Unity虚拟屏幕】做计算；")]
        [TypeConverter(typeof(BoolStringConverter))]
        [Json(false)]
        public bool enableScreenCameraLink { get => unityCameraControl.enableScreenCameraLink; set => unityCameraControl.enableScreenCameraLink = value; }

        [Category("相机控制")]
        [DisplayName("左右眼矩阵模式")]
        [Description("立体渲染启用时本参数有效，用于基于当前相机的左右眼透视矩阵与视图矩阵的不同计算方法；影响【相机透视】组件的【左右眼矩阵模式】参数；")]
        [TypeConverter(typeof(EnumStringConverter))]
        [Json(false)]
        public ELREyeMatrixMode LREyeMatrixMode { get => unityCameraControl.LREyeMatrixMode; set => unityCameraControl.LREyeMatrixMode = value; }

        [Category("相机控制")]
        [DisplayName("相机变换处理规则")]
        [Description("当配置更新时，相机变换的处理规则；")]
        [TypeConverter(typeof(EnumStringConverter))]
        [Json(false)]
        public ECameraTransformHandleRule cameraTransformHandleRule { get => unityCameraControl.cameraTransformHandleRule; set => unityCameraControl.cameraTransformHandleRule = value; }

        [Category("相机控制")]
        [DisplayName("允许直接变换控制")]
        [Description("允许不同输入方式（包括XRIS、ART、OptiTrack、ZVR、XR头盔设备等）直接控制相机的变换信息（T位置、R旋转、S缩放）")]
        [TypeConverter(typeof(BoolStringConverter))]
        [Json(false)]
        public bool allowDirectTransformControl { get => unityCameraControl.allowDirectTransformControl; set => unityCameraControl.allowDirectTransformControl = value; }

        [Category("相机控制")]
        [DisplayName("允许移动控制")]
        [Description("允许通过鼠标、键盘、触摸、手柄等输入方式控制相机的移动")]
        [TypeConverter(typeof(BoolStringConverter))]
        [Json(false)]
        public bool allowMoveControl { get => unityCameraControl.allowMoveControl; set => unityCameraControl.allowMoveControl = value; }

        [Category("相机控制")]
        [DisplayName("允许旋转控制")]
        [Description("允许通过鼠标、键盘、触摸、手柄等输入方式控制相机的旋转")]
        [TypeConverter(typeof(BoolStringConverter))]
        [Json(false)]
        public bool allowRotateControl { get => unityCameraControl.allowRotateControl; set => unityCameraControl.allowRotateControl = value; }

        [Category("相机控制")]
        [DisplayName("允许屏幕边界控制")]
        [Description("允许鼠标在屏幕边界时控制相机的移动；在参数【允许移动控制】启用时，本参数设置才可生效；")]
        [TypeConverter(typeof(BoolStringConverter))]
        [Json(false)]
        public bool allowScreenBoundaryControl { get => unityCameraControl.allowScreenBoundaryControl; set => unityCameraControl.allowScreenBoundaryControl = value; }

        #endregion

        #region 相机偏移

        /// <summary>
        /// 相机偏移
        /// </summary>
        [Browsable(false)]
        public Pose cameraOffset { get; set; } = new Pose();

        [Category("相机偏移")]
        [DisplayName("位置")]
        [Description("XR交互空间的子级对象；用于统一管理XR交互空间中需要跟踪交互的所有对象；即其位置与旋转信息均相对于XR交互空间对象进行处理；通常可以理解为XR实验室中人物穿戴的头盔、左右手控制器等设备组合体的空间原点；")]
        [Json(false)]
        public V3F cameraOffset_Position { get => cameraOffset.position; set => cameraOffset.position = value; }

        [Category("相机偏移")]
        [DisplayName("旋转")]
        [Description("XR交互空间的子级对象；用于统一管理XR交互空间中需要跟踪交互的所有对象；即其位置与旋转信息均相对于XR交互空间对象进行处理；通常可以理解为XR实验室中人物穿戴的头盔、左右手控制器等设备组合体的空间原点；")]
        [Json(false)]
        public V3F cameraOffset_Rotation { get => cameraOffset.rotation; set => cameraOffset.rotation = value; }

        #endregion

        #region 相机列表

        /// <summary>
        /// 相机列表
        /// </summary>
        [Browsable(false)]
        public List<CameraInfo> cameras { get; set; } = new List<CameraInfo>();

        /// <summary>
        /// 获取相机
        /// </summary>
        /// <param name="screenName"></param>
        /// <returns></returns>
        public CameraInfo GetCamera(string cameraName) => cameras.FirstOrDefault(i => i.name == cameraName);

        /// <summary>
        /// 添加相机：存在同名的则不执行添加，并返回该同名的对象
        /// </summary>
        public CameraInfo AddCamera(string cameraName, string screenName)
        {
            var cameraInfo = GetCamera(cameraName);
            if (cameraInfo != null) return cameraInfo;

            cameraInfo = new CameraInfo() { name = cameraName, screen = screenName };
            cameraInfo.SetConfig(this);
            cameras.Add(cameraInfo);
            return cameraInfo;
        }

        #endregion

        #region 动作列表

        /// <summary>
        /// 动作列表
        /// </summary>
        [Browsable(false)]
        public List<ActionInfo> actions { get; set; } = new List<ActionInfo>();

        public ActionInfo GetActionInfo(string actionName) => actions.FirstOrDefault(i => i.name == actionName);

        /// <summary>
        /// 添加动作：存在同名的则不执行添加，并返回该同名的对象
        /// </summary>
        /// <param name="actionName"></param>
        /// <returns></returns>
        public ActionInfo AddAction(string actionName)
        {
            var actionInfo = GetActionInfo(actionName);
            if (actionInfo != null) return actionInfo;

            actionInfo = new ActionInfo() { name = actionName };
            actionInfo.SetConfig(this);
            actions.Add(actionInfo);
            return actionInfo;
        }

        #endregion
    }

    #region 基础信息

    /// <summary>
    /// 基础信息
    /// </summary>
    [Import]
    public class BaseInfo
    {
        /// <summary>
        /// 配置
        /// </summary>
        internal XRSpaceConfigA config { get; private set; }

        /// <summary>
        /// 设置配置
        /// </summary>
        /// <param name="config"></param>
        public virtual void SetConfig(XRSpaceConfigA config) => this.config = config;
    }

    #endregion

    #region Unity相机控制

    /// <summary>
    /// Unity相机控制
    /// </summary>
    [Import]
    public class UnityCameraControl
    {
        /// <summary>
        /// 启用屏幕相机关联
        /// </summary>
        public bool enableScreenCameraLink { get; set; } = true;

        /// <summary>
        /// 左右眼矩阵模式
        /// </summary>
        public ELREyeMatrixMode LREyeMatrixMode { get; set; } = ELREyeMatrixMode.None;

        /// <summary>
        /// 相机相机变换处理规则
        /// </summary>
        public ECameraTransformHandleRule cameraTransformHandleRule { get; set; } =  ECameraTransformHandleRule.None;

        /// <summary>
        /// 允许直接变换控制
        /// </summary>
        public bool allowDirectTransformControl { get; set; } = true;

        /// <summary>
        /// 允许移动控制
        /// </summary>
        public bool allowMoveControl { get; set; } = true;

        /// <summary>
        /// 允许旋转控制
        /// </summary>
        public bool allowRotateControl { get; set; } = true;

        /// <summary>
        /// 允许屏幕边界控制
        /// </summary>
        public bool allowScreenBoundaryControl { get; set; } = false;
    }

    /// <summary>
    /// 相机相机变换处理规则
    /// </summary>
    public enum ECameraTransformHandleRule
    {
        /// <summary>
        /// 无
        /// </summary>
        [Name("无")]
        [Tip("即不对相机的变换做任何额外处理")]
        None,

        /// <summary>
        /// 重置
        /// </summary>
        [Name("重置")]
        [Tip("将相机的变换信息重置到缺省状态，即相对父级位置、旋转均为0，缩放全为1的初始状态；")]
        Reset,

        /// <summary>
        /// 重置到启动
        /// </summary>
        [Name("重置到启动")]
        [Tip("将相机的变换信息重置到程序启动时记录的状态")]
        ResetToStart,
    }

    #endregion

    #region Unity相机配置

    /// <summary>
    /// Unity相机配置
    /// </summary>
    [Import]
    public class UnityCameraConfig
    {
        public float nearClipPlane { get; set; } = 0.01f;

        public float farClipPlane { get; set; } = 1000;

        public ERenderingPath renderingPath { get; set; } = ERenderingPath.Forward;

        public bool allowHDR { get; set; } = false;

        public bool allowMSAA { get; set; } = false;

        public bool allowDynamicResolution { get; set; } = false;

        public float stereoSeparation { get; set; } = 0.022f;

        public float stereoConvergence { get; set; } = 10;


#if UNITY_2019_4_OR_NEWER

        /// <summary>
        /// 设置相机配置
        /// </summary>
        /// <param name="camera"></param>
        public void SetCameraConfig(Camera camera)
        {
            camera.nearClipPlane = nearClipPlane;
            camera.farClipPlane = farClipPlane;
            camera.renderingPath = (RenderingPath)(int)renderingPath;
            camera.allowHDR = allowHDR;
            camera.allowMSAA = allowMSAA;
            camera.allowDynamicResolution = allowDynamicResolution;
            camera.stereoSeparation = stereoSeparation;
            camera.stereoConvergence = stereoConvergence;
        }

#endif

    }

    //
    // 摘要:
    //     Enum used to specify how the sensor gate (sensor frame) defined by Camera.sensorSize
    //     fits into the resolution gate (render frame).
    public enum EGateFitMode
    {
        //
        // 摘要:
        //     Stretch the sensor gate to fit exactly into the resolution gate.
        None = 0,
        //
        // 摘要:
        //     Fit the resolution gate vertically within the sensor gate.
        Vertical = 1,
        //
        // 摘要:
        //     Fit the resolution gate horizontally within the sensor gate.
        Horizontal = 2,
        //
        // 摘要:
        //     Automatically selects a horizontal or vertical fit so that the sensor gate fits
        //     completely inside the resolution gate.
        Fill = 3,
        //
        // 摘要:
        //     Automatically selects a horizontal or vertical fit so that the render frame fits
        //     completely inside the resolution gate.
        Overscan = 4
    }

    public enum ERenderingPath
    {
        //unityCameraConfig.farClipPlane;
        // 摘要:
        //     Use Player Settings.
        UsePlayerSettings = -1,
        //
        // 摘要:
        //     Vertex Lit.
        VertexLit = 0,
        //
        // 摘要:
        //     Forward Rendering.
        Forward = 1,
        //
        // 摘要:
        //     Deferred Lighting (Legacy).
        DeferredLighting = 2,
        //
        // 摘要:
        //     Deferred Shading.
        DeferredShading = 3
    }

    #endregion

    #region 相机

    /// <summary>
    /// 相机立体目标眼罩
    /// </summary>
    public enum ECameraStereoTargetEyeMask
    {
        /// <summary>
        /// 不要将任何一只眼睛渲染到HMD。
        /// </summary>
        [Name("无")]
        None = 0,

        /// <summary>
        /// 仅将左眼渲染到HMD。
        /// </summary>
        [Name("左眼")]
        Left = 1,

        /// <summary>
        /// 仅将右眼渲染到HMD
        /// </summary>
        [Name("右眼")]
        Right = 2,

        /// <summary>
        /// 将双眼渲染到HMD
        /// </summary>
        [Name("双眼")]
        Both = 3
    }

    /// <summary>
    /// 相机目标显示
    /// </summary>
    public enum ECameraTargetDisplay
    {
        /// <summary>
        /// 显示1
        /// </summary>
        Display1,

        /// <summary>
        /// 显示2
        /// </summary>
        Display2,

        /// <summary>
        /// 显示3
        /// </summary>
        Display3,

        /// <summary>
        /// 显示4
        /// </summary>
        Display4,

        /// <summary>
        /// 显示5
        /// </summary>
        Display5,

        /// <summary>
        /// 显示6
        /// </summary>
        Display6,

        /// <summary>
        /// 显示7
        /// </summary>
        Display7,

        /// <summary>
        /// 显示8
        /// </summary>
        Display8,
    }

    /// <summary>
    /// 相机信息
    /// </summary>
    [Import]
    public class CameraInfo : BaseInfo, ICustomEnumStringConverter
    {
        #region 信息

        /// <summary>
        /// 名称
        /// </summary>
        [Category("基础信息")]
        [DisplayName("名称")]
        [Description("当前相机的名称信息，会同步为Unity内标识对应的相机游戏对象的名称；")]
        [Field(index = 1)]
        public string name { get; set; } = "";

        /// <summary>
        /// 
        /// </summary>
        [Category("基础信息")]
        [DisplayName("视口矩形")]
        [Description("基于Unity视口矩形坐标系：左下角为(0,0)原点，向右为X轴正方向，向上为Y轴正方向；XY与宽度高度区间值默认为[0，1]；")]
        [Field(index = 2)]
        [Json(exportString = true)]
        [ColumnHeader(150)]
        public RectF viewportRect { get => _viewportRect; set => _viewportRect = value; }

        protected RectF _viewportRect = new RectF(0, 0, 1, 1);

        /// <summary>
        /// 屏幕
        /// </summary>
        [Category("基础信息")]
        [DisplayName("屏幕")]
        [Description("当前相机进行投影计算时使用的屏幕名称；")]
        [Field(index = 3)]
        [TypeConverter(typeof(CustomEnumStringConverter))]
        public string screen { get; set; } = "";

        /// <summary>
        /// 相机立体目标眼罩
        /// </summary>
        [Category("基础信息")]
        [DisplayName("相机立体目标眼罩")]
        [Description("定义“摄影机”渲染到VR显示器的哪只眼睛；")]
        [Field(index = 6)]
        [TypeConverter(typeof(EnumStringConverter))]
        public ECameraStereoTargetEyeMask stereoTargetEye { get; set; } = ECameraStereoTargetEyeMask.Both;

        /// <summary>
        /// 相机目标显示
        /// </summary>
        [Category("基础信息")]
        [DisplayName("相机目标显示")]
        [Description("设置此相机的目标显示,即该设置相机渲染到指定的显示中；支持的最大显示（例如显示器）数量为 8；")]
        [Field(index = 7)]
        [TypeConverter(typeof(EnumStringConverter))]
        public ECameraTargetDisplay targetDisplay { get; set; } =  ECameraTargetDisplay.Display1;

        #endregion

        #region 相机变换

        /// <summary>
        /// 相机偏移
        /// </summary>
        [Browsable(false)]
        [Field(ignore = true)]
        public Pose cameraOffset { get; set; } = new Pose();

        [Category("相机变换")]
        [DisplayName("位置")]
        [Description("相对头盔类设备（初始时可认为是相机偏移对象）的位置偏移信息；以米为单位；使用Unity内左手坐标系,X右Y上Z前；X轴偏移量为左右水平距离差；Y轴偏移量为垂直距离差；Z轴偏移量为前后水平距离差；")]
        [Field(index = 4)]
        [Json(false)]
        public V3F cameraOffset_Position { get => cameraOffset.position; set => cameraOffset.position = value; }

        [Category("相机变换")]
        [DisplayName("旋转")]
        [Description("相机相对头盔类设备（初始时可认为是相机偏移对象）的旋转欧拉角度信息；以度为单位；使用Unity内左手坐标系,X右Y上Z前；")]
        [Field(index = 5)]
        [Json(false)]
        public V3F cameraOffset_Rotation { get => cameraOffset.rotation; set => cameraOffset.rotation = value; }

        #endregion

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public CameraInfo Clone() => new CameraInfo().CopyDataFrom(this);

        public CameraInfo CopyDataFrom(CameraInfo cameraInfo)
        {
            if (cameraInfo != null)
            {
                this.SetConfig(cameraInfo.config);

                this.name = cameraInfo.name;
                this.viewportRect = cameraInfo.viewportRect;
                this.screen = cameraInfo.screen;
                this.stereoTargetEye = cameraInfo.stereoTargetEye;
                this.targetDisplay = cameraInfo.targetDisplay;
                this.cameraOffset = cameraInfo.cameraOffset.Clone();
            }
            return this;
        }

        public string[] GetCustomEnumStrings(ITypeDescriptorContext context)
        {
            switch (context.PropertyDescriptor.Name)
            {
                case nameof(screen):
                    {
                        if (config != null)
                        {
                            return config.screens.Cast(s => s.name).ToArray();
                        }
                        break;
                    }
            }
            return Empty<string>.Array;
        }
    }

    #endregion

    #region 屏幕

    /// <summary>
    /// 屏幕信息
    /// </summary>
    [Import]
    public class ScreenInfo : BaseInfo, ICustomEnumStringConverter
    {
        #region 信息

        /// <summary>
        /// 名称
        /// </summary>
        [Category("基础信息")]
        [DisplayName("名称")]
        [Description("当前屏幕的名称信息，会同步为Unity内标识对应的屏幕游戏对象的名称；")]
        [Field(index = 1)]
        public string name { get; set; } = "";

        /// <summary>
        /// 显示名
        /// </summary>
        [Category("基础信息")]
        [DisplayName("显示名")]
        [Description("当前屏幕的显示名信息，会同步为Unity内标识对应的屏幕对象的提示信息名称；")]
        [Field(index = 2)]
        public string displayName { get; set; } = "";

        /// <summary>
        /// 屏幕尺寸
        /// </summary>
        [Category("基础信息")]
        [DisplayName("屏幕尺寸")]
        [Description("可理解为当前屏幕的物理客观尺寸信息，即表示屏幕立方体的宽高厚信息；以米为单位；使用Unity内左手坐标系,X右Y上Z前；X轴长度为当前屏幕的物理客观尺寸的宽度信息；Y轴长度为当前屏幕的物理客观尺寸的高度信息；Z轴长度为当前屏幕的物理客观尺寸的厚度信息，计算相机与屏幕的投影关系时，本值不参与运算，即使用默认值即可；")]
        [Field(index = 3)]
        [Json(exportString = true)]
        public V3F screenSize { get => _screenSize; set => _screenSize = value; }

        protected V3F _screenSize = new V3F(4, 2, 0.01f);

        /// <summary>
        /// 屏幕姿态模式
        /// </summary>
        [Category("基础信息")]
        [DisplayName("屏幕姿态模式")]
        [Description("用于标识屏幕的姿态如何定位")]
        [ColumnHeader(100)]
        [Field(index = 4)]
        [TypeConverter(typeof(EnumStringConverter))]
        public EScreenPoseMode screenPoseMode { get; set; } = EScreenPoseMode.ScreenPose;

        [Category("基础信息")]
        [DisplayName("屏幕姿态信息")]
        [Description("屏幕姿态定位的结果信息")]
        [ColumnHeader(200)]
        [Field(index = 5)]
        [Json(false)]
        public string screen
        {
            get
            {
                switch (screenPoseMode)
                {
                    case EScreenPoseMode.ScreenPose: return position.ToString();
                    case EScreenPoseMode.AnchorLink: return screenAnchorLinkInfo.ToString();
                    default: return "<无效的屏幕姿态模式>";
                }
            }
        }

        [Category("基础信息")]
        [DisplayName("有效性")]
        [Description("当前屏幕是否有效的屏幕，包括检查屏幕的名称、尺寸、位置、旋转、锚点关联等信息是否有效；")]
        [Field(index = 10)]
        [Json(false)]
        [TypeConverter(typeof(BoolStringConverter))]
        public bool valid
        {
            get
            {
                //检查屏幕名称
                if (string.IsNullOrEmpty(name)) return false;
                //检查屏幕尺寸
                if (screenSize.x <= 0 || screenSize.y <= 0 || screenSize.z <= 0) return false;
                switch (screenPoseMode)
                {
                    case EScreenPoseMode.ScreenPose: return true;
                    case EScreenPoseMode.AnchorLink: return !string.IsNullOrEmpty(standardScreen) && standardScreen != name && screenAnchorLinkInfo.standardScreenAnchor != ERectAnchor.None && screenAnchorLinkInfo.screenAnchor != ERectAnchor.None;
                }
                return false;
            }
        }

        #endregion

        #region 屏幕位置

        /// <summary>
        /// 屏幕姿态
        /// </summary>
        [Browsable(false)]
        [Field(ignore = true)]
        public Pose screenPose { get; set; } = new Pose();

        /// <summary>
        /// 屏幕位置
        /// </summary>
        [Category("屏幕姿态-屏幕位置")]
        [DisplayName("位置")]
        [Description("相对屏幕组的屏幕位置；以米为单位；以米为单位；使用Unity内左手坐标系,X右Y上Z前；X轴偏移量为屏幕中心到屏幕组对象的左右水平距离差；Y轴偏移量为屏幕中心到屏幕组对象的垂直距离差；Z轴偏移量为屏幕中心到屏幕组对象的前后水平距离差；")]
        [Field(ignore = true)]
        [Json(false)]
        public V3F position { get => screenPose.position; set => screenPose.position = value; }

        [Category("屏幕姿态-屏幕位置")]
        [DisplayName("地面到屏幕下边沿距离")]
        [Description("空间偏移对象到的屏幕下边沿垂直距离差,即当前屏幕位置Y轴偏移量的修正;地面0高度以上为正，地面以下为负值;当前屏幕的【屏幕姿态模式】为【屏幕姿态】时本参数有效；以米为单位；使用Unity内左手坐标系,X右Y上Z前；")]
        [Field(ignore = true)]
        [Json(false)]
        public float screenDownToSpaceOffsetDistance
        {
            get
            {
                switch (screenPoseMode)
                {
                    case EScreenPoseMode.ScreenPose:
                        {
                            if (config != null)
                            {
                                return screenPose.position.y - screenSize.y / 2 + config.screenGroupOffset.position.y;
                            }
                            break;
                        }
                }
                return 0;
            }
            set
            {
                switch (screenPoseMode)
                {
                    case EScreenPoseMode.ScreenPose:
                        {
                            if (config != null)
                            {
                                screenPose._position.y = screenSize.y / 2 - config.screenGroupOffset.position.y + value;
                            }
                            break;
                        }
                }
            }
        }

        [Category("屏幕姿态-屏幕位置")]
        [DisplayName("屏幕组到屏幕下边沿距离")]
        [Description("屏幕组对象到屏幕下边沿的垂直距离差,即当前屏幕位置Y轴偏移量的修正;当前屏幕的【屏幕姿态模式】为【屏幕姿态】时本参数有效；以米为单位；使用Unity内左手坐标系,X右Y上Z前；")]
        [Field(ignore = true)]
        [Json(false)]
        public float screenDownToScreenGroupDistance
        {
            get
            {
                switch (screenPoseMode)
                {
                    case EScreenPoseMode.ScreenPose:
                        {
                            return screenPose.position.y - screenSize.y / 2;
                        }
                }
                return 0;
            }
            set
            {
                switch (screenPoseMode)
                {
                    case EScreenPoseMode.ScreenPose:
                        {
                            screenPose._position.y = screenSize.y / 2 + value;
                            break;
                        }
                }
            }
        }

        #endregion

        #region 屏幕旋转

        /// <summary>
        /// 屏幕旋转
        /// </summary>
        [Category("屏幕姿态-屏幕旋转")]
        [DisplayName("旋转")]
        [Description("相对屏幕组的屏幕旋转欧拉角度,以度为单位；使用Unity内左手坐标系,X右Y上Z前；X为屏幕中心相对屏幕组对象的X轴旋转量；Y为屏幕中心相对屏幕组对象的Y轴旋转量；Z为屏幕中心相对屏幕组对象的Z轴旋转量；")]
        [Field(ignore = true)]
        [Json(false)]
        public V3F rotation { get => screenPose.rotation; set => screenPose.rotation = value; }

        #endregion

        #region 锚点关联

        /// <summary>
        /// 屏幕锚点关联信息
        /// </summary>
        [Category("锚点关联")]
        [DisplayName("屏幕锚点关联信息")]
        [Field(ignore = true)]
        [Browsable(false)]
        public ScreenAnchorLinkInfo screenAnchorLinkInfo { get; set; } = new ScreenAnchorLinkInfo();

        #endregion

        #region 锚点关联-标准屏幕

        [Category("锚点关联-标准屏幕")]
        [DisplayName("标准屏幕")]
        [Description("标准屏幕的名称；用于定义当前屏幕以哪个屏幕为标准进行姿态的定位；")]
        [Field(ignore = true)]
        [Json(false)]
        [TypeConverter(typeof(CustomEnumStringConverter))]
        public string standardScreen { get => screenAnchorLinkInfo.standardScreen; set => screenAnchorLinkInfo.standardScreen = value; }

        [Category("锚点关联-标准屏幕")]
        [DisplayName("标准屏幕锚点")]
        [Description("标准屏幕的矩形平面内的特征点作为锚点")]
        [Field(ignore = true)]
        [Json(false)]
        [TypeConverter(typeof(EnumStringConverter))]
        public ERectAnchor standardScreenAnchor { get => screenAnchorLinkInfo.standardScreenAnchor; set => screenAnchorLinkInfo.standardScreenAnchor = value; }

        [Category("锚点关联-标准屏幕")]
        [DisplayName("标准屏幕锚点偏移空间类型")]
        [Field(ignore = true)]
        [Json(false)]
        [TypeConverter(typeof(EnumStringConverter))]
        public EAnchorOffsetSpaceType standardScreenAnchorOffsetSpaceType { get => screenAnchorLinkInfo.standardScreenAnchorOffsetSpaceType; set => screenAnchorLinkInfo.standardScreenAnchorOffsetSpaceType = value; }

        [Category("锚点关联-标准屏幕")]
        [DisplayName("标准屏幕锚点偏移")]
        [Description("标准屏幕锚点三轴偏移值；以米为单位；使用Unity内左手坐标系,X右Y上Z前；X为标准屏幕锚点X轴偏移值；Y为标准屏幕锚点Y轴偏移值；Z为标准屏幕锚点Z轴偏移值；")]
        [Field(ignore = true)]
        [Json(false)]
        public V3F standardScreenAnchorOffset { get => screenAnchorLinkInfo.standardScreenAnchorOffset; set => screenAnchorLinkInfo.standardScreenAnchorOffset = value; }

        #endregion

        #region 锚点关联-当前屏幕

        [Category("锚点关联-当前屏幕")]
        [DisplayName("屏幕锚点")]
        [Description("当前屏幕的矩形平面内的特征点作为锚点")]
        [Field(ignore = true)]
        [Json(false)]
        [TypeConverter(typeof(EnumStringConverter))]
        public ERectAnchor screenAnchor { get => screenAnchorLinkInfo.screenAnchor; set => screenAnchorLinkInfo.screenAnchor = value; }

        [Category("锚点关联-当前屏幕")]
        [DisplayName("屏幕锚点偏移空间类型")]
        [Description("当前屏幕锚点三轴偏移值")]
        [Field(ignore = true)]
        [Json(false)]
        [TypeConverter(typeof(EnumStringConverter))]
        public EAnchorOffsetSpaceType screenAnchorOffsetSpaceType { get => screenAnchorLinkInfo.screenAnchorOffsetSpaceType; set => screenAnchorLinkInfo.screenAnchorOffsetSpaceType = value; }

        [Category("锚点关联-当前屏幕")]
        [DisplayName("屏幕锚点偏移")]
        [Description("当前屏幕锚点三轴偏移值；以米为单位；使用Unity内左手坐标系,X右Y上Z前；X为当前屏幕锚点X轴偏移值；Y为当前屏幕锚点Y轴偏移值；Z为当前屏幕锚点Z轴偏移值；")]
        [Field(ignore = true)]
        [Json(false)]
        public V3F screenAnchorOffset { get => screenAnchorLinkInfo.screenAnchorOffset; set => screenAnchorLinkInfo.screenAnchorOffset = value; }

        [Category("锚点关联-当前屏幕")]
        [DisplayName("屏幕锚点旋转")]
        [Description("当前屏幕锚点三轴旋转值；以度为单位；使用Unity内左手坐标系,X右Y上Z前；X为当前屏幕锚点Z轴旋转值；Y轴当前屏幕锚点Y轴旋转值；Z为当前屏幕锚点Z轴旋转值；")]
        [Field(ignore = true)]
        [Json(false)]
        public V3F linkRotation { get => screenAnchorLinkInfo.linkRotation; set => screenAnchorLinkInfo.linkRotation = value; }

        #endregion

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public ScreenInfo Clone() => new ScreenInfo().CopyDataFrom(this);

        public ScreenInfo CopyDataFrom(ScreenInfo screenInfo)
        {
            if (screenInfo != null)
            {
                this.SetConfig(screenInfo.config);

                this.name = screenInfo.name;
                this.displayName = screenInfo.displayName;
                this.screenSize = screenInfo.screenSize;
                this.screenPoseMode = screenInfo.screenPoseMode;
                this.screenPose = screenInfo.screenPose.Clone();
                this.screenAnchorLinkInfo = screenInfo.screenAnchorLinkInfo.Clone();
            }
            return this;
        }

        public string[] GetCustomEnumStrings(ITypeDescriptorContext context)
        {
            switch (context.PropertyDescriptor.Name)
            {
                case nameof(standardScreen):
                    {
                        if (config != null)
                        {
                            return config.screens.Cast(s => s.name).ToArray();
                        }
                        break;
                    }
            }
            return Empty<string>.Array;
        }
    }

    /// <summary>
    /// 屏幕姿态模式
    /// </summary>
    [Name("屏幕姿态模式")]
    public enum EScreenPoseMode
    {
        /// <summary>
        /// 屏幕姿态
        /// </summary>
        [Name("屏幕姿态")]
        ScreenPose,

        /// <summary>
        /// 锚点关联
        /// </summary>
        [Name("锚点关联")]
        AnchorLink,
    }

    /// <summary>
    /// 场景关联锚点信息
    /// </summary>
    [Import]
    public class ScreenAnchorLinkInfo
    {
        /// <summary>
        /// 标准屏幕
        /// </summary>
        public string standardScreen { get; set; } = "";

        /// <summary>
        /// 标准屏幕锚点
        /// </summary>
        public ERectAnchor standardScreenAnchor { get; set; } = ERectAnchor.Center;

        /// <summary>
        /// 标准屏幕锚点偏移
        /// </summary>
        [Json(exportString = true)]
        public V3F standardScreenAnchorOffset { get => _standardScreenAnchorOffset; set => _standardScreenAnchorOffset = value; }

        internal V3F _standardScreenAnchorOffset = new V3F();

        /// <summary>
        /// 标准屏幕锚点偏移空间类型
        /// </summary>
        public EAnchorOffsetSpaceType standardScreenAnchorOffsetSpaceType { get; set; } = EAnchorOffsetSpaceType.Local;

        /// <summary>
        /// 屏幕锚点
        /// </summary>
        public ERectAnchor screenAnchor { get; set; } = ERectAnchor.Center;

        /// <summary>
        /// 屏幕锚点偏移
        /// </summary>
        [Json(exportString = true)]
        public V3F screenAnchorOffset { get => _screenAnchorOffset; set => _screenAnchorOffset = value; }

        internal V3F _screenAnchorOffset = new V3F();

        /// <summary>
        /// 屏幕锚点偏移空间类型
        /// </summary>
        public EAnchorOffsetSpaceType screenAnchorOffsetSpaceType { get; set; } = EAnchorOffsetSpaceType.Local;

        /// <summary>
        /// 关联旋转
        /// </summary>
        [Json(exportString = true)]
        public V3F linkRotation { get => _linkRotation; set => _linkRotation = value; }

        internal V3F _linkRotation = new V3F();

        /// <summary>
        /// 转字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0}.{1}-->{2}-->当前.{3}", standardScreen, NameCache.Get(standardScreenAnchor), (string)linkRotation, NameCache.Get(screenAnchor));
        }

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public ScreenAnchorLinkInfo Clone() => new ScreenAnchorLinkInfo().CopyDataFrom(this);

        /// <summary>
        /// 从源拷贝数据
        /// </summary>
        /// <param name="screenAnchorLinkInfo"></param>
        /// <returns></returns>
        public ScreenAnchorLinkInfo CopyDataFrom(ScreenAnchorLinkInfo screenAnchorLinkInfo)
        {
            if (screenAnchorLinkInfo != null)
            {
                this.standardScreen = screenAnchorLinkInfo.standardScreen;
                this.standardScreenAnchor = screenAnchorLinkInfo.standardScreenAnchor;
                this.standardScreenAnchorOffset = screenAnchorLinkInfo.standardScreenAnchorOffset;
                this.standardScreenAnchorOffsetSpaceType = screenAnchorLinkInfo.standardScreenAnchorOffsetSpaceType;

                this.screenAnchor = screenAnchorLinkInfo.screenAnchor;
                this.screenAnchorOffset = screenAnchorLinkInfo.screenAnchorOffset;
                this.screenAnchorOffsetSpaceType = screenAnchorLinkInfo.screenAnchorOffsetSpaceType;

                this.linkRotation = screenAnchorLinkInfo.linkRotation;
            }
            return this;
        }
    }

    /// <summary>
    /// 锚点偏移空间类型
    /// </summary>
    [Name("锚点偏移空间类型")]
    public enum EAnchorOffsetSpaceType
    {
        /// <summary>
        /// 世界
        /// </summary>
        [Name("世界")]
        World = 0,

        /// <summary>
        /// 本地
        /// </summary>
        [Name("本地")]
        Local,
    }

    #endregion

    #region 动作

    /// <summary>
    /// 动作配置类型
    /// </summary>
    [Name("动作配置类型")]
    [ColumnHeader(120)]
    public enum EActionConfigType
    {
        [Name("基础")]
        [Tip("动作的基础信息")]
        Base = 1,

        [Name("跟踪器-姿态")]
        [Tip("基于VRPN记录跟踪器的姿态（位置和方向）；会对跟踪器对应的目标基于本地坐标系进行相同的姿态更新；")]
        Tracker_Pose,

        [Name("按钮-交互")]
        [Tip("用于配置按钮对目标做不同交互的控制操作；")]
        Button_Interact,

        [Name("按钮-交互选择")]
        [Tip("基于VRPN记录按钮的按下与释放事件；可简单理解为对目标执行鼠标左键操作；")]
        Button_InteractSelect,

        [Name("按钮-交互激活")]
        [Tip("基于VRPN记录按钮的按下与释放事件；可简单理解为对目标执行鼠标右键操作；")]
        Button_InteractActivate,

        [Name("按钮-交互UI")]
        [Tip("基于VRPN记录按钮的按下与释放事件；可简单理解为对UI执行鼠标左键操作；")]
        Button_InteractUI,

        [Name("模拟量-XYZ")]
        [Tip("用于配置模拟量对XYZ各轴的控制操作；")]      Analog_XYZ,

        [Name("模拟量-负X")]
        [Tip("基于VRPN记录的模拟量；可简单理解为对坐标系的X值做减操作；")]
        Analog_NX,

        [Name("模拟量-正X")]
        [Tip("基于VRPN记录的模拟量；可简单理解为对坐标系的X值做加操作；")]
        Analog_PX,

        [Name("模拟量-负Y")]
        [Tip("基于VRPN记录的模拟量；可简单理解为对坐标系的Y值做减操作；")]
        Analog_NY,

        [Name("模拟量-正Y")]
        [Tip("基于VRPN记录的模拟量；可简单理解为对坐标系的Y值做加操作；")]
        Analog_PY,

        [Name("模拟量-负Z")]
        [Tip("基于VRPN记录的模拟量；可简单理解为对坐标系的Z值做减操作；")]
        Analog_NZ,

        [Name("模拟量-正Z")]
        [Tip("基于VRPN记录的模拟量；可简单理解为对坐标系的Z值做加操作；")]
        Analog_PZ,
    }

    /// <summary>
    /// 动作配置类型特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ActionConfigTypeAttribute: Attribute
    {
        /// <summary>
        /// 动作配置类型
        /// </summary>
        public EActionConfigType actionConfigType { get; private set; }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="actionConfigType"></param>
        public ActionConfigTypeAttribute(EActionConfigType actionConfigType)
        {
            this.actionConfigType = actionConfigType;
        }
    }

    /// <summary>
    /// 动作信息
    /// </summary>
    public class ActionInfo : BaseInfo
    {
        #region 基础

        /// <summary>
        /// 名称：动作名称
        /// </summary>
        [Category("基础")]
        [DisplayName("名称")]
        [Field(index = 0)]
        [ColumnHeader(100)]
        public string name { get; set; } = "";

        #endregion

        #region 跟踪器

        /// <summary>
        /// 姿态
        /// </summary>
        [Browsable(false)]
        [ColumnHeader(ignore = true)]
        [ActionConfigType(EActionConfigType.Tracker_Pose)]
        public PoseVrpnConfig pose { get; set; } = new PoseVrpnConfig();

        [Category("跟踪器")]
        [DisplayName("姿态")]
        [Field(index = 1)]
        [ColumnHeader(36)]
        [Json(false)]
        [TypeConverter(typeof(BoolStringConverter))]
        public bool poseEnable => pose.enable;

        #endregion

        #region 按钮交互

        /// <summary>
        /// 按钮交互
        /// </summary>
        [Browsable(false)]
        [ColumnHeader(ignore = true)]
        [ActionConfigType(EActionConfigType.Button_Interact)]
        public ButtonInteractConfig buttonInteract { get; set; } = new ButtonInteractConfig();

        #endregion

        #region 按钮

        /// <summary>
        /// 交互选择
        /// </summary>
        [ColumnHeader(ignore = true)]
        [Browsable(false)]
        [ActionConfigType(EActionConfigType.Button_InteractSelect)]
        public ButtonVrpnConfig interactSelect { get; set; } = new ButtonVrpnConfig();

        [Category("按钮")]
        [DisplayName("交互选择")]
        [Field(index = 2)]
        [Json(false)]
        [TypeConverter(typeof(BoolStringConverter))]
        public bool interactSelectEnable => buttonInteract.enable && interactSelect.CanInteract();

        /// <summary>
        /// 交互激活
        /// </summary>
        [ColumnHeader(ignore = true)]
        [Browsable(false)]
        [ActionConfigType(EActionConfigType.Button_InteractActivate)]
        public ButtonVrpnConfig interactActivate { get; set; } = new ButtonVrpnConfig();

        [Category("按钮")]
        [DisplayName("交互激活")]
        [Field(index = 3)]
        [Json(false)]
        [TypeConverter(typeof(BoolStringConverter))]
        public bool interactActivateEnable => buttonInteract.enable && interactActivate.CanInteract();

        /// <summary>
        /// 交互UI
        /// </summary>
        [ColumnHeader(ignore = true)]
        [Browsable(false)]
        [ActionConfigType(EActionConfigType.Button_InteractUI)]
        public ButtonVrpnConfig interactUI { get; set; } = new ButtonVrpnConfig();

        [Category("按钮")]
        [DisplayName("交互UI")]
        [Field(index = 4)]
        [Json(false)]
        [TypeConverter(typeof(BoolStringConverter))]
        public bool interactUIEnable => buttonInteract.enable && interactUI.CanInteract();

        #endregion

        #region 模拟量XYZ

        /// <summary>
        /// 模拟量XYZ
        /// </summary>
        [Browsable(false)]
        [ColumnHeader(ignore = true)]
        [ActionConfigType(EActionConfigType.Analog_XYZ)]
        public AnalogXYZConfig analogXYZ { get; set; } = new AnalogXYZConfig();

        #endregion

        #region 模拟量

        /// <summary>
        /// 负X
        /// </summary>
        [ColumnHeader(ignore = true)]
        [Browsable(false)]
        [ActionConfigType(EActionConfigType.Analog_NX)]
        public AnalogVrpnConfig nx { get; set; } = new AnalogVrpnConfig();

        [Category("模拟量")]
        [DisplayName("负X")]
        [Field(index = 5)]
        [ColumnHeader(32)]
        [Json(false)]
        [TypeConverter(typeof(BoolStringConverter))]
        public bool nxEnable => nx.enable || nx.xboxEnable;

        /// <summary>
        /// 正X
        /// </summary>
        [ColumnHeader(ignore = true)]
        [Browsable(false)]
        [ActionConfigType(EActionConfigType.Analog_PX)]
        public AnalogVrpnConfig px { get; set; } = new AnalogVrpnConfig();

        [Category("模拟量")]
        [DisplayName("正X")]
        [Field(index = 6)]
        [ColumnHeader(32)]
        [Json(false)]
        [TypeConverter(typeof(BoolStringConverter))]
        public bool pxEnable => px.enable || px.xboxEnable;

        /// <summary>
        /// 负Y
        /// </summary>
        [ColumnHeader(ignore = true)]
        [Browsable(false)]
        [ActionConfigType(EActionConfigType.Analog_NY)]
        public AnalogVrpnConfig ny { get; set; } = new AnalogVrpnConfig();

        [Category("模拟量")]
        [DisplayName("负Y")]
        [Field(index = 7)]
        [ColumnHeader(32)]
        [Json(false)]
        [TypeConverter(typeof(BoolStringConverter))]
        public bool nyEnable => ny.enable || ny.xboxEnable;

        /// <summary>
        /// 正Y
        /// </summary>
        [ColumnHeader(ignore = true)]
        [Browsable(false)]
        [ActionConfigType(EActionConfigType.Analog_PY)]
        public AnalogVrpnConfig py { get; set; } = new AnalogVrpnConfig();

        [Category("模拟量")]
        [DisplayName("正Y")]
        [Field(index = 8)]
        [ColumnHeader(32)]
        [Json(false)]
        [TypeConverter(typeof(BoolStringConverter))]
        public bool pyEnable => py.enable || py.xboxEnable;

        /// <summary>
        /// 负Z
        /// </summary>
        [ColumnHeader(ignore = true)]
        [Browsable(false)]
        [ActionConfigType(EActionConfigType.Analog_NZ)]
        public AnalogVrpnConfig nz { get; set; } = new AnalogVrpnConfig();

        [Category("模拟量")]
        [DisplayName("负Z")]
        [Field(index = 9)]
        [ColumnHeader(32)]
        [Json(false)]
        [TypeConverter(typeof(BoolStringConverter))]
        public bool nzEnable => nz.enable || nz.xboxEnable;

        /// <summary>
        /// 正Z
        /// </summary>
        [ColumnHeader(ignore = true)]
        [Browsable(false)]
        [ActionConfigType(EActionConfigType.Analog_PZ)]
        public AnalogVrpnConfig pz { get; set; } = new AnalogVrpnConfig();

        [Category("模拟量")]
        [DisplayName("正Z")]
        [Field(index = 10)]
        [ColumnHeader(32)]
        [Json(false)]
        [TypeConverter(typeof(BoolStringConverter))]
        public bool pzEnable => pz.enable || pz.xboxEnable;

        #endregion

        public ActionInfo()
        {
            SetChildrenConfig();
        }

        public override void SetConfig(XRSpaceConfigA config)
        {
            base.SetConfig(config);
            SetChildrenConfig();
        }

        private void SetChildrenConfig()
        {
            this.pose.config = this;

            this.buttonInteract.config = this;
            this.interactSelect.config = this;
            this.interactActivate.config = this;
            this.interactUI.config = this;

            this.analogXYZ.config = this;
            this.nx.config = this;
            this.px.config = this;
            this.ny.config = this;
            this.py.config = this;
            this.nz.config = this;
            this.pz.config = this;
        }

        public object GetActionConfig(EActionConfigType actionConfigType)
        {
            if (actionConfigType == EActionConfigType.Base) return this;
            var propertyInfo = PropertyInfosCache.Get(this.GetType(), TypeHelper.InstancePublic).FirstOrDefault(pi =>
            {
                return AttributeCache<ActionConfigTypeAttribute>.Get(pi) is ActionConfigTypeAttribute attribute && attribute.actionConfigType == actionConfigType;
            });
            return propertyInfo?.GetValue(this, Empty<object>.Array) ?? this;
        }
    }

    #endregion

    #region 内置动作名

    /// <summary>
    /// 内置动作名
    /// </summary>
    public enum EActionName
    {
        /// <summary>
        /// HMD
        /// </summary>
        [Name("HMD")]
        [Tip("头盔显示器")]
        HMD,

        /// <summary>
        /// 左手
        /// </summary>
        [Name("左手")]
        LeftHand,

        /// <summary>
        /// 右手
        /// </summary>
        [Name("右手")]
        RigthHand,

        /// <summary>
        /// 空间移动
        /// </summary>
        [Name("空间移动")]
        SpaceTranslate,

        /// <summary>
        /// 空间旋转
        /// </summary>
        [Name("空间旋转")]
        SpaceRotate,

        /// <summary>
        /// 主菜单
        /// </summary>
        [Name("主菜单")]
        MainMenu,
    }

    #endregion

    #region VRPN

    /// <summary>
    /// 基础配置
    /// </summary>
    [Import]
    public class BaseConfig
    {
        internal ActionInfo config { get; set; }

        /// <summary>
        /// 启用
        /// </summary>
        [DisplayName("启用")]
        [TypeConverter(typeof(BoolStringConverter))]
        public virtual bool enable { get; set; } = false;
    }

    /// <summary>
    /// VRPN配置
    /// </summary>
    [Import]
    public class VrpnConfig : BaseConfig
    {
        /// <summary>
        /// VRPN启用
        /// </summary>
        [Category("VRPN配置")]
        [DisplayName("VRPN启用")]
        [TypeConverter(typeof(BoolStringConverter))]
        public override bool enable { get => base.enable; set => base.enable = value; }

        /// <summary>
        /// 主机名
        /// </summary>
        [Category("VRPN配置")]
        [DisplayName("主机名")]
        [Description("VRPN服务所在的地址；如期望指明端口可使用IP:Port的形式，如127.0.0.1:3883；VRPN的默认端口3883；")]
        public string hostname { get => _hostname; set { _hostname = value; UpdateAddress(); } } 

        internal string _hostname = "127.0.0.1";

        /// <summary>
        /// 对象名
        /// </summary>
        [Category("VRPN配置")]
        [DisplayName("对象名")]
        [Description("对应VRPN中不同设备设备类型（Tracker/Button/Analog/Dial/ForceDevice等）的具体对象的名称")]
        public string objectName { get => _objectName; set { _objectName = value; UpdateAddress(); } }

        internal string _objectName = "";

        /// <summary>
        /// VRPN通信使用的地址
        /// </summary>
        [Category("VRPN配置")]
        [DisplayName("地址")]
        [Description("VRPN通信使用的地址")]
        [Json(false)]
        public string address => _address;

        internal string _address = "";

        public string UpdateAddress() => _address = objectName + "@" + hostname;

        /// <summary>
        /// 通道
        /// </summary>
        [Category("VRPN配置")]
        [DisplayName("通道")]
        [Description("VRPN通信使用的通道")]
        public int channel { get; set; } = 0;

        /// <summary>
        /// 构造
        /// </summary>
        public VrpnConfig()
        {
            UpdateAddress();
        }
    }

    /// <summary>
    /// 姿态VRPN配置
    /// </summary>
    [Import]
    public class PoseVrpnConfig : VrpnConfig
    {
        /// <summary>
        /// 长度单位
        /// </summary>
        [Category("坐标轴")]
        [DisplayName("长度单位")]
        [TypeConverter(typeof(EnumStringConverter))]
        public ELengthUnits lengtheUnit { get; set; } = ELengthUnits.M;

        /// <summary>
        /// X轴
        /// </summary>
        [Category("坐标轴")]
        [DisplayName("X轴")]
        [TypeConverter(typeof(EnumStringConverter))]
        public EAxisDirection xAxis { get; set; } = EAxisDirection.R;

        /// <summary>
        /// Y轴
        /// </summary>
        [Category("坐标轴")]
        [DisplayName("Y轴")]
        [TypeConverter(typeof(EnumStringConverter))]
        public EAxisDirection yAxis { get; set; } = EAxisDirection.U;

        /// <summary>
        /// Z轴
        /// </summary>
        [Category("坐标轴")]
        [DisplayName("Z轴")]
        [TypeConverter(typeof(EnumStringConverter))]
        public EAxisDirection zAxis { get; set; } = EAxisDirection.F;
    }    

    /// <summary>
    /// 按钮VRPN配置
    /// </summary>
    [Import]
    public class ButtonVrpnConfig : VrpnConfig
    {
        #region XBox

        /// <summary>
        /// XBox配置
        /// </summary>
        [Browsable(false)]
        public XBoxConfig xboxConfig { get; set; } = new XBoxConfig();

        [Category("XBox配置")]
        [DisplayName("XBox启用")]
        [Description("XBox直连在对应控制的计算机设备上时，XBox配置才可生效；")]
        [Json(false)]
        [TypeConverter(typeof(BoolStringConverter))]
        public bool xboxEnable { get => xboxConfig.enable; set => xboxConfig.enable = value; }

        [Category("XBox配置")]
        [DisplayName("轴与按钮")]
        [Description("XBox直连在对应控制的计算机设备上时，XBox配置才可生效；")]
        [Json(false)]
        [TypeConverter(typeof(EnumStringConverter))]
        public EXBoxAxisAndButton axisAndButton_XBox { get => xboxConfig.axisAndButton; set => xboxConfig.axisAndButton = value; }

        [Category("XBox配置")]
        [DisplayName("死区小值")]
        [Description("XBox的值小于本值时，则认为0值；在死区区间内时，如果按钮则为1，如果模拟量则做0到1的线性补间值；XBox直连在对应控制的计算机设备上时，XBox配置才可生效；")]
        [Json(false)]
        public float deadZoneMinValue_XBox { get => xboxConfig.deadZone.x; set => xboxConfig._deadZone.x = value; }

        [Category("XBox配置")]
        [DisplayName("死区大值")]
        [Description("XBox的值大于本值时，则认为1值；在死区区间内时，如果按钮则为1，如果模拟量则做0到1的线性补间值；XBox直连在对应控制的计算机设备上时，XBox配置才可生效；")]
        [Json(false)]
        public float deadZoneMaxValue_XBox { get => xboxConfig.deadZone.y; set => xboxConfig._deadZone.y = value; }

        #endregion

        /// <summary>
        /// 构造
        /// </summary>
        public ButtonVrpnConfig()
        {
            xboxConfig.deadZone = new V2F(0.5f, 1f);
        }

        /// <summary>
        /// 能交互
        /// </summary>
        /// <returns></returns>
        public bool CanInteract() => enable || xboxEnable;
    }

    /// <summary>
    /// 模拟量VRPN配置
    /// </summary>
    [Import]
    public class AnalogVrpnConfig : ButtonVrpnConfig
    {
        /// <summary>
        /// 模拟量配置
        /// </summary>
        [Browsable(false)]
        public AnalogConfig analogConfig { get; set; } = new AnalogConfig();

        [Category("VRPN配置-模拟量")]
        [DisplayName("源最小值")]
        [Description("当前VRPN可以提供的模拟量的最小值")]
        [Json(false)]
        public double srcMinValue { get => analogConfig._srcValue.x; set => analogConfig._srcValue.x = value; }

        [Category("VRPN配置-模拟量")]
        [DisplayName("源最大值")]
        [Description("当前VRPN可以提供的模拟量的最大值")]
        [Json(false)]
        public double srcMaxValue { get => analogConfig._srcValue.y; set => analogConfig._srcValue.y = value; }

        [Category("VRPN配置-模拟量")]
        [DisplayName("死区小值")]
        [Description("VRPN模拟量的值小于本值时，则认为目标最小值；在死区区间内时，对模拟量做目标最小值到目标最大值的线性补间处理；")]
        [Json(false)]
        public double deadZoneMinValue { get => analogConfig._deadZone.x; set => analogConfig._deadZone.x = value; }

        [Category("VRPN配置-模拟量")]
        [DisplayName("死区大值")]
        [Description("VRPN模拟量的值大于本值时，则认为目标最大值；在死区区间内时，对模拟量做目标最小值到目标最大值的线性补间处理；")]
        [Json(false)]
        public double deadZoneMaxValue { get => analogConfig._deadZone.y; set => analogConfig._deadZone.y = value; }

        [Category("VRPN配置-模拟量")]
        [DisplayName("目标最小值")]
        [Description("对VRPN模拟量经过死区矫正后，为后续处理提供的最小值；")]
        [Json(false)]
        public double dstMinValue { get => analogConfig._dstValue.x; set => analogConfig._dstValue.x = value; }

        [Category("VRPN配置-模拟量")]
        [DisplayName("目标最大值")]
        [Description("对VRPN模拟量经过死区矫正后，为后续处理提供的最大值；")]
        [Json(false)]
        public double dstMaxValue { get => analogConfig._dstValue.y; set => analogConfig._dstValue.y = value; }

        /// <summary>
        /// 构造
        /// </summary>
        public AnalogVrpnConfig()
        {
            xboxConfig.deadZone = new V2F(0.1f, 0.9f);
        }

        /// <summary>
        /// 获取归一化的值
        /// </summary>
        /// <param name="srcValue"></param>
        /// <returns>值范围为[0,1]区间</returns>
        public double GetNormalizationValue(double srcValue)
        {
            if (srcValue < deadZoneMinValue) return 0;
            if (srcValue > deadZoneMaxValue) return 1;
            return MathX.Clamp01((srcValue - deadZoneMinValue) / (deadZoneMaxValue - deadZoneMinValue));
        }

        /// <summary>
        /// 获取目标值
        /// </summary>
        /// <param name="srcValue"></param>
        /// <returns></returns>
        public double GetDstValue(double srcValue) => (dstMaxValue - dstMinValue) * GetNormalizationValue(srcValue) + dstMinValue;
    }

    #endregion

    #region 模拟量配置

    /// <summary>
    /// 模拟量配置
    /// </summary>
    [Import]
    public class AnalogConfig
    {
        /// <summary>
        /// 源值
        /// </summary>
        [Json(exportString = true)]
        public V2D srcValue { get => _srcValue; set => _srcValue = value; }

        internal V2D _srcValue = new V2D(0, 1);

        /// <summary>
        /// 死区
        /// </summary>
        [Json(exportString = true)]
        public V2D deadZone { get => _deadZone; set => _deadZone = value; }

        internal V2D _deadZone = new V2D(0, 1);

        /// <summary>
        /// 目标值
        /// </summary>
        [Json(exportString = true)]
        public V2D dstValue { get => _dstValue; set => _dstValue = value; }

        internal V2D _dstValue = new V2D(0, 1);
    }

    #endregion

    #region 按钮交互配置

    /// <summary>
    /// 按钮交互配置
    /// </summary>
    [Import]
    public class ButtonInteractConfig : BaseConfig
    {

    }

    #endregion

    #region 模拟量XYZ配置

    /// <summary>
    /// 模拟量XYZ配置
    /// </summary>
    [Import]
    public class AnalogXYZConfig : BaseConfig
    {
        [ColumnHeader(ignore = true)]
        [Browsable(false)]
        [Json(exportString = true)]
        public V3F speed { get => _speed; set => _speed = value; }

        internal V3F _speed = new V3F(1, 1, 1);

        [Category("速度")]
        [DisplayName("X速度")]
        [Json(false)]
        public float xSpeed { get => _speed.x; set => _speed.x = value; }

        [Category("速度")]
        [DisplayName("Y速度")]
        [Json(false)]
        public float ySpeed { get => _speed.y; set => _speed.y = value; }

        [Category("速度")]
        [DisplayName("Z速度")]
        [Json(false)]
        public float zSpeed { get => _speed.z; set => _speed.z = value; }

        [Category("轴向")]
        [DisplayName("变换TRS")]
        [TypeConverter(typeof(EnumStringConverter))]
        public ETransformTRS transformTRS { get; set; } = ETransformTRS.None;
    }

    #endregion

    #region XBox

    /// <summary>
    /// XBox配置
    /// </summary>
    [Import]
    public class XBoxConfig : BaseConfig
    {
        /// <summary>
        /// 轴与按钮
        /// </summary>
        public EXBoxAxisAndButton axisAndButton { get; set; } = EXBoxAxisAndButton.None;

        /// <summary>
        /// 死区
        /// </summary>
        [Json(exportString = true)]
        public V2F deadZone { get => _deadZone; set => _deadZone = value; }

        internal V2F _deadZone = new V2F();
    }
    

    #endregion

    #region 姿态

    /// <summary>
    /// 姿态
    /// </summary>
    [Import]
    public class Pose
    {
        /// <summary>
        /// 位置
        /// </summary>
        [Json(exportString = true)]
        public V3F position { get => _position; set => _position = value; }

        internal V3F _position = new V3F();

        /// <summary>
        /// 旋转
        /// </summary>
        [Json(exportString = true)]
        public V3F rotation { get => _rotation; set => _rotation = value; }

        internal V3F _rotation = new V3F();

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public Pose Clone() => new Pose().CopyDataFrom(this);

        /// <summary>
        /// 从姿态复制数据
        /// </summary>
        /// <param name="pose"></param>
        /// <returns></returns>
        public Pose CopyDataFrom(Pose pose)
        {
            if (pose != null)
            {
                this.position = pose.position;
                this.rotation = pose.rotation;
            }
            return this;
        }
    }

    /// <summary>
    /// TRS数据
    /// </summary>
    public class TRSData: Pose
    {
        /// <summary>
        /// 位置
        /// </summary>
        [Json(exportString = true)]
        public V3F scale { get => _scale; set => _scale = value; }

        internal V3F _scale = new V3F(1, 1, 1);
    }

    #endregion
}
