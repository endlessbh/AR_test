using UnityEngine;
using System.Collections;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.Extension.Base.Dataflows.Base;
using System;
using XCSJ.PluginTools.Renderers;
using System.Collections.Generic;
using XCSJ.Caches;
using XCSJ.Collections;
using XCSJ.Maths;

namespace XCSJ.PluginStereoView.Tools
{
    /// <summary>
    /// 虚拟屏幕:现实世界屏幕在虚拟世界中的虚拟对象
    /// </summary>
    [Name(Title)]
    [Tip("现实世界屏幕在虚拟世界中的虚拟对象", "Virtual object of real world screen in virtual world")]
    [DisallowMultipleComponent]
    [ExecuteInEditMode]
    public class VirtualScreen : BaseScreen
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "虚拟屏幕";

        /// <summary>
        /// 屏幕尺寸规则
        /// </summary>
        [Name("屏幕尺寸规则")]
        public enum EScreenSizeRule
        {
            /// <summary>
            /// 值:屏幕的物理尺寸；单位：米；
            /// </summary>
            [Name("值")]
            [Tip("屏幕的物理尺寸；单位：米；", "Physical size of the screen; Unit: meter;")]
            Value,

            /// <summary>
            /// 本地缩放XY对应宽高:当前组件所在游戏对象的本地缩放X值对应屏幕的宽度，本地缩放Y值对应宽屏幕的高度；单位：米；
            /// </summary>
            [Name("本地缩放XY对应宽高")]
            [Tip("当前组件所在游戏对象的本地缩放X值对应屏幕的宽度，本地缩放Y值对应宽屏幕的高度；单位：米；", "The local zoom X value of the game object where the current component is located corresponds to the width of the screen, and the local zoom Y value corresponds to the height of the wide screen; Unit: meter;")]
            LocalScaleXY_WH,

            /// <summary>
            /// 本地缩放ZY对应宽高:当前组件所在游戏对象的本地缩放Z值对应屏幕的宽度，本地缩放Y值对应宽屏幕的高度；单位：米；
            /// </summary>
            [Name("本地缩放ZY对应宽高")]
            [Tip("当前组件所在游戏对象的本地缩放Z值对应屏幕的宽度，本地缩放Y值对应宽屏幕的高度；单位：米；", "The local zoom Z value of the game object where the current component is located corresponds to the width of the screen, and the local zoom Y value corresponds to the height of the wide screen; Unit: meter;")]
            LocalScaleZY_WH,

            /// <summary>
            /// 本地缩放XZ对应宽高:当前组件所在游戏对象的本地缩放X值对应屏幕的宽度，本地缩放Z值对应宽屏幕的高度；单位：米；
            /// </summary>
            [Name("本地缩放XZ对应宽高")]
            [Tip("当前组件所在游戏对象的本地缩放X值对应屏幕的宽度，本地缩放Z值对应宽屏幕的高度；单位：米；", "The local zoom X value of the game object where the current component is located corresponds to the width of the screen, and the local zoom Z value corresponds to the height of the wide screen; Unit: meter;")]
            LocalScaleXZ_WH,
        }

        /// <summary>
        /// 屏幕尺寸规则
        /// </summary>
        [Name("屏幕尺寸规则")]
        [EnumPopup]
        public EScreenSizeRule _screenSizeRule = EScreenSizeRule.LocalScaleXY_WH;

        /// <summary>
        /// 屏幕尺寸：X为宽，Y为高，Z为厚度；单位：米；
        /// </summary>
        [Name("屏幕尺寸")]
        [Tip("X为宽，Y为高,Z为厚度；单位：米；", "X is width, y is height and Z is thickness; Unit: meter;")]
        [HideInSuperInspector(nameof(_screenSizeRule), EValidityCheckType.NotEqual, EScreenSizeRule.Value)]
        public Vector3 _screenSize = new Vector3(4, 2, 0.01f);

        /// <summary>
        /// 屏幕尺寸：X为宽，Y为高,Z为厚度；单位：米；
        /// </summary>
        public override Vector3 screenSize
        {
            set
            {
                switch (_screenSizeRule)
                {
                    case EScreenSizeRule.LocalScaleXY_WH:
                        {
                            transform.XSetLocalScale(new Vector3(value.x, value.y, value.z));
                            break;
                        }
                    case EScreenSizeRule.LocalScaleZY_WH:
                        {
                            transform.XSetLocalScale(new Vector3(value.z, value.y, value.x));
                            break;
                        }
                    case EScreenSizeRule.LocalScaleXZ_WH:
                        {
                            transform.XSetLocalScale(new Vector3(value.x, value.z, value.y));
                            break;
                        }
                    default:
                        {
                            this.XModifyProperty(ref _screenSize, value);
                            break;
                        }
                }
                CallScreenChanged();
            }
            get
            {
                switch (_screenSizeRule)
                {
                    case EScreenSizeRule.LocalScaleXY_WH:
                        {
                            var scale = this.transform.localScale;
                            return new Vector3(scale.x, scale.y, scale.z);
                        }
                    case EScreenSizeRule.LocalScaleZY_WH:
                        {
                            var scale = this.transform.localScale;
                            return new Vector3(scale.z, scale.y, scale.x);
                        }
                    case EScreenSizeRule.LocalScaleXZ_WH:
                        {
                            var scale = this.transform.localScale;
                            return new Vector3(scale.x, scale.z, scale.y);
                        }
                    default: return _screenSize;
                }
            }
        }

        /// <summary>
        /// 屏幕包围盒
        /// </summary>
        public override Bounds screenBounds
        {
            get
            {
                var bounds = new Bounds(transform.position, Vector3.zero);
                bounds.Encapsulate(GetAnchorPosition(ERectAnchor.TopLeft));
                bounds.Encapsulate(GetAnchorPosition(ERectAnchor.TopRight));
                bounds.Encapsulate(GetAnchorPosition(ERectAnchor.BottomLeft));
                bounds.Encapsulate(GetAnchorPosition(ERectAnchor.BottomRight));
                return bounds;
            }
        }

        private Transform thisTransform;

        /// <summary>
        /// 启用
        /// </summary>
        public void Start()
        {
            thisTransform = this.transform;
        }

        /// <summary>
        /// 更新
        /// </summary>
        public void Update()
        {
            if (thisTransform.hasChanged)
            {
                thisTransform.hasChanged = false;
                //Debug.Log("变化: " + name);
                CallScreenChanged();
            }
        }

        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            screenSize = new Vector3(4, 2, 0.01f);
        }

        /// <summary>
        /// 界面数据变化时调用
        /// </summary>
        public override void OnValidate()
        {
            var screenSize = this.screenSize;
            screenSize.x = Mathf.Clamp(screenSize.x, 0.01f, 10f);
            screenSize.y = Mathf.Clamp(screenSize.y, 0.01f, 10f);
            this.screenSize = screenSize;
            base.OnValidate();
        }

        /// <summary>
        /// 获取锚点的本地坐标
        /// </summary>
        /// <param name="screenAnchor"></param>
        /// <returns></returns>
        public override Vector3 GetAnchorLocalPosition(ERectAnchor screenAnchor)
        {
            switch (_screenSizeRule)
            {
                case EScreenSizeRule.LocalScaleXY_WH:
                    {
                        return screenAnchor.GetAnchorOffset(Vector2.one);
                    }
                case EScreenSizeRule.LocalScaleZY_WH:
                    {
                        var offset = screenAnchor.GetAnchorOffset(Vector2.one);
                        return new Vector3(0, offset.y, offset.x);
                    }
                case EScreenSizeRule.LocalScaleXZ_WH:
                    {
                        var offset = screenAnchor.GetAnchorOffset(Vector2.one);
                        return new Vector3(offset.x, 0, offset.y);
                    }
                case EScreenSizeRule.Value:
                default:
                    {
                        return screenAnchor.GetAnchorOffset(screenSize);
                    }
            }
        }

        private static VirtualScreen SetScreen(GameObject screenGO, Transform parent = null)
        {
            var screen = screenGO.XGetOrAddComponent<VirtualScreen>();

            //渲染器
            var renderer = screenGO.XGetOrAddComponent<GizmoRenderer>();
            renderer.XModifyProperty(() =>
            {
                //renderer._sizeRule = GizmoRenderer.ESizeRule.ComponentValue;
                renderer._component = screen;
                renderer.gizmosColor = new Color(0, 1, 0, 0.5f);
                renderer._drawLabel = GizmoRenderer.EDrawLabel.Text;
                if (string.IsNullOrEmpty(renderer._text)) renderer._text = screenGO.name;
            });

            var screenTransform = screenGO.transform;

            //设置父级
            if (parent) screenTransform.XSetTransformParent(parent);

            //重置其本地位置与旋转
            screenTransform.XSetLocalPosition(Vector3.zero);
            screenTransform.XSetLocalRotation(Quaternion.identity);

            return screen;
        }

        /// <summary>
        /// 创建屏幕
        /// </summary>
        /// <returns></returns>
        public static VirtualScreen CreateScreen(string name = null, Transform parent = null)
        {
            var screenGO = UnityObjectHelper.CreateGameObject(string.IsNullOrEmpty(name) ? VirtualScreen.Title : name);
            return SetScreen(screenGO, parent);
        }

        /// <summary>
        /// 克隆或创建屏幕
        /// </summary>
        /// <param name="cloneFromScreen"></param>
        /// <param name="name"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static VirtualScreen CloneOrCreateScreen(BaseScreen cloneFromScreen, string name = null, Transform parent = null)
        {
            if (!cloneFromScreen) return CreateScreen(name, parent);
            var screenGO = UnityObjectHelper.XCloneObject(cloneFromScreen.gameObject);

            //设置父级
            var tmpParent = cloneFromScreen.transform.parent;
            if (tmpParent) screenGO.transform.XSetTransformParent(tmpParent);

            //设置名称
            screenGO.XSetName(string.IsNullOrEmpty(name) ? VirtualScreen.Title : name);

            return SetScreen(screenGO, parent);
        }

        /// <summary>
        /// 布局屏幕
        /// </summary>
        /// <param name="screens"></param>
        /// <param name="screenLayoutMode"></param>
        public static void LayoutScreen(List<VirtualScreen> screens, EScreenLayoutMode screenLayoutMode)
        {
            if (screens == null || screens.Count < screenLayoutMode.GetScreenCount()) return;
            switch (screenLayoutMode)
            {
                case EScreenLayoutMode.F:
                    {
                        //前-基准屏幕
                        var standardScreen = screens[0];
                        standardScreen.XSetName("屏幕前");
                        standardScreen.XGetOrAddComponent<GizmoRenderer>().text = "前";
                        break;
                    }
                case EScreenLayoutMode.FD_90:
                    {
                        //前-基准屏幕
                        var standardScreen = screens[0];
                        standardScreen.XSetName("屏幕前");
                        standardScreen.XGetOrAddComponent<GizmoRenderer>().text = "前";

                        //下
                        var screen1 = screens[1];
                        screen1.XSetName("屏幕下");
                        screen1.XGetOrAddComponent<GizmoRenderer>().text = "下";
                        screen1.AnchorLinkToScreen(ERectAnchor.Top, new Vector3(90, 0, 0), standardScreen, ERectAnchor.Bottom);
                        break;
                    }
                case EScreenLayoutMode.LFR_45:
                    {
                        //前-基准屏幕
                        var standardScreen = screens[1];
                        standardScreen.XSetName("屏幕前");
                        standardScreen.XGetOrAddComponent<GizmoRenderer>().text = "前";

                        //左
                        var screen0 = screens[0];
                        screen0.XSetName("屏幕左");
                        screen0.XGetOrAddComponent<GizmoRenderer>().text = "左";
                        screen0.AnchorLinkToScreen(ERectAnchor.Right, new Vector3(0, -45, 0), standardScreen, ERectAnchor.Left);

                        //右
                        var screen2 = screens[2];
                        screen2.XSetName("屏幕右");
                        screen2.XGetOrAddComponent<GizmoRenderer>().text = "右";
                        screen2.AnchorLinkToScreen(ERectAnchor.Left, new Vector3(0, 45, 0), standardScreen, ERectAnchor.Right);
                        break;
                    }
                case EScreenLayoutMode.LFR_90:
                    {
                        //前-基准屏幕
                        var standardScreen = screens[1];
                        standardScreen.XSetName("屏幕前");
                        standardScreen.XGetOrAddComponent<GizmoRenderer>().text = "前";

                        //左
                        var screen0 = screens[0];
                        screen0.XSetName("屏幕左");
                        screen0.XGetOrAddComponent<GizmoRenderer>().text = "左";
                        screen0.AnchorLinkToScreen(ERectAnchor.Right, new Vector3(0, -90, 0), standardScreen, ERectAnchor.Left);

                        //右
                        var screen2 = screens[2];
                        screen2.XSetName("屏幕右");
                        screen2.XGetOrAddComponent<GizmoRenderer>().text = "右";
                        screen2.AnchorLinkToScreen(ERectAnchor.Left, new Vector3(0, 90, 0), standardScreen, ERectAnchor.Right);
                        break;
                    }
                case EScreenLayoutMode.LFRD_Cave:
                    {
                        LayoutScreen(screens, EScreenLayoutMode.LFR_90);

                        //下
                        var screen3 = screens[3];
                        screen3.XSetName("屏幕下");
                        screen3.XGetOrAddComponent<GizmoRenderer>().text = "下";
                        screen3.AnchorLinkToScreen(ERectAnchor.Top, new Vector3(90, 0, 0), screens[1], ERectAnchor.Bottom);
                        break;
                    }
                case EScreenLayoutMode.LFRDU_Cave:
                    {
                        LayoutScreen(screens, EScreenLayoutMode.LFRD_Cave);

                        //上
                        var screen4 = screens[4];
                        screen4.XSetName("屏幕上");
                        screen4.XGetOrAddComponent<GizmoRenderer>().text = "上";
                        screen4.AnchorLinkToScreen(ERectAnchor.Bottom, new Vector3(-90, 0, 0), screens[1], ERectAnchor.Top);
                        break;
                    }
            }
        }

        /// <summary>
        /// 布局相机
        /// </summary>
        /// <param name="cameras"></param>
        /// <param name="screenLayoutMode"></param>
        public static void LayoutCamera(List<Camera> cameras, EScreenLayoutMode screenLayoutMode)
        {
            if (cameras == null || screenLayoutMode == EScreenLayoutMode.None) return;
            cameras.Foreach((i, c) => LayoutCamera(i, c, screenLayoutMode));
        }

        /// <summary>
        /// 布局相机
        /// </summary>
        /// <param name="index"></param>
        /// <param name="camera"></param>
        /// <param name="baseScreen"></param>
        /// <param name="screenLayoutMode"></param>
        public static void LayoutCamera(int index, Camera camera, EScreenLayoutMode screenLayoutMode)
        {
            if (!camera) return;
            switch(screenLayoutMode)
            {
                case EScreenLayoutMode.F:
                    {
                        switch (index)
                        {
                            case 0:
                                {
                                    camera.XSetName("相机前");
                                    //camera.XModifyProperty(() => { camera.rect = new Rect(0.333333f, 0, 0.333333f, 1); });
                                    break;
                                }
                        }
                        break;
                    }
                case EScreenLayoutMode.FD_90:
                    {
                        switch (index)
                        {
                            case 0:
                                {
                                    camera.XSetName("相机前");
                                    camera.XModifyProperty(() => { camera.rect = new Rect(0, 0.5f, 1, 0.5f); });
                                    break;
                                }
                            case 1:
                                {
                                    camera.XSetName("相机下");
                                    camera.XModifyProperty(() => { camera.rect = new Rect(0, 0, 1, 0.5f); });
                                    break;
                                }
                        }
                        break;
                    }
                case EScreenLayoutMode.LFR_45:
                case EScreenLayoutMode.LFR_90:
                    {
                        // L   F   R
                        switch (index)
                        {
                            case 0:
                                {
                                    camera.XSetName("相机左");
                                    camera.XModifyProperty(() => { camera.rect = new Rect(0, 0, 0.333333f, 1); });
                                    break;
                                }
                            case 1:
                                {
                                    camera.XSetName("相机前");
                                    camera.XModifyProperty(() => { camera.rect = new Rect(0.333333f, 0, 0.333333f, 1); });
                                    break;
                                }
                            case 2:
                                {
                                    camera.XSetName("相机右");
                                    camera.XModifyProperty(() => { camera.rect = new Rect(0.666666f, 0, 0.333333f, 1); });
                                    break;
                                }
                        }
                        break;
                    }
                case EScreenLayoutMode.LFRD_Cave:
                    {
                        // L   F   R
                        //     D
                        switch (index)
                        {
                            case 0:
                                {
                                    camera.XSetName("相机左");
                                    camera.XModifyProperty(() => { camera.rect = new Rect(0, 0.5f, 0.333333f, 0.5f); });
                                    break;
                                }
                            case 1:
                                {
                                    camera.XSetName("相机前");
                                    camera.XModifyProperty(() => { camera.rect = new Rect(0.333333f, 0.5f, 0.333333f, 0.5f); });
                                    break;
                                }
                            case 2:
                                {
                                    camera.XSetName("相机右");
                                    camera.XModifyProperty(() => { camera.rect = new Rect(0.666666f, 0.5f, 0.333333f, 0.5f); });
                                    break;
                                }
                            case 3:
                                {
                                    camera.XSetName("相机下");
                                    camera.XModifyProperty(() => { camera.rect = new Rect(0, 0, 1, 0.5f); });
                                    break;
                                }
                        }
                        break;
                    }
                case EScreenLayoutMode.LFRDU_Cave:
                    {
                        //     U
                        // L   F   R
                        //     D
                        switch (index)
                        {
                            case 0:
                                {
                                    camera.XSetName("相机左");
                                    camera.XModifyProperty(() => { camera.rect = new Rect(0, 0.333333f, 0.333333f, 0.333333f); });
                                    break;
                                }
                            case 1:
                                {
                                    camera.XSetName("相机前");
                                    camera.XModifyProperty(() => { camera.rect = new Rect(0.333333f, 0.333333f, 0.333333f, 0.333333f); });
                                    break;
                                }
                            case 2:
                                {
                                    camera.XSetName("相机右");
                                    camera.XModifyProperty(() => { camera.rect = new Rect(0.666666f, 0.333333f, 0.333333f, 0.333333f); });
                                    break;
                                }
                            case 3:
                                {
                                    camera.XSetName("相机上");
                                    camera.XModifyProperty(() => { camera.rect = new Rect(0, 0.666666f, 1, 0.333333f); });
                                    break;
                                }
                            case 4:
                                {
                                    camera.XSetName("相机下");
                                    camera.XModifyProperty(() => { camera.rect = new Rect(0, 0, 1, 0.333333f); });
                                    break;
                                }
                        }
                        break;
                    }
            }
        }
    }

    /// <summary>
    /// 屏幕布局模式:内置的屏幕布局模式；1块屏幕以上可进行有效布局；L左R右U上D下F前B后；
    /// </summary>
    [Name("屏幕布局模式")]
    [Tip("内置的屏幕布局模式；1块屏幕以上可进行有效布局；L左R右U上D下F前B后；", "Built in screen layout mode; More than one screen can be effectively arranged; L left r right u upper D lower f front B rear;")]
    public enum EScreenLayoutMode
    {
        /// <summary>
        /// 无
        /// </summary>
        [Name("无")]
        None,

        /// <summary>
        /// 单通道-前型
        /// </summary>
        [Name("单通道-前型")]
        [ScreenCount(1)]
        F = 100 * 1,

        /// <summary>
        /// 双通道-前
        /// </summary>
        [Name("双通道-前下90度型")]
        [ScreenCount(2)]
        FD_90 = 100 * 2,

        /// <summary>
        /// 三通道-左前右45度型
        /// </summary>
        [Name("三通道-左前右45度型")]
        [ScreenCount(3)]
        LFR_45 = 100 * 3,

        /// <summary>
        /// 三通道-左前右90度型
        /// </summary>
        [Name("三通道-左前右90度型")]
        [ScreenCount(3)]
        LFR_90,

        /// <summary>
        /// 四通道-左前右下Cave型
        /// </summary>
        [Name("四通道-左前右下Cave型")]
        [ScreenCount(4)]
        LFRD_Cave = 100 * 4,

        /// <summary>
        /// 五通道-左前右上下Cave型
        /// </summary>
        [Name("五通道-左前右上下Cave型")]
        [ScreenCount(5)]
        LFRDU_Cave = 100 * 5,
    }

    /// <summary>
    /// 屏幕布局模式属性值
    /// </summary>
    [Serializable]
    [PropertyType(typeof(EScreenLayoutMode))]
    [Name("屏幕布局模式属性值")]
    public class EScreenLayoutModePropertyValue : EnumPropertyValue<EScreenLayoutMode>
    {
    }

    /// <summary>
    /// 屏幕数目特性
    /// </summary>
    public class ScreenCountAttribute : Attribute
    {
        /// <summary>
        /// 数目
        /// </summary>
        public int count { get; set; } = 0;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="count"></param>
        public ScreenCountAttribute(int count)
        {
            this.count = count;
        }

        /// <summary>
        /// 获取屏幕数目
        /// </summary>
        /// <param name="screenLayoutMode"></param>
        /// <returns></returns>
        public static int GetScreenCount(EScreenLayoutMode screenLayoutMode)
        {
            return (AttributeCache<ScreenCountAttribute>.GetOfField(screenLayoutMode) is ScreenCountAttribute screenCountAttribute) ? screenCountAttribute.count : 0;
        }
    }

    /// <summary>
    /// 虚拟屏幕属性值
    /// </summary>
    [Name("虚拟屏幕属性值")]
    [Serializable]
    [PropertyType(typeof(VirtualScreen))]
    public class VirtualScreenPropertyValue : BaseComponentPropertyValue<VirtualScreen> { }
}
