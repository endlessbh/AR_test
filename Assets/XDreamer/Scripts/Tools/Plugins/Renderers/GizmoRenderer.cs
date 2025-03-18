using System;
using UnityEngine;
using UnityEngine.Serialization;
using XCSJ.Attributes;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;
using XCSJ.Tools;
using XCSJ.PluginCommonUtils.Tools;

namespace XCSJ.PluginTools.Renderers
{
    /// <summary>
    /// Gizmo渲染器:本组件仅在编辑器中生效
    /// </summary>
    [Tool("渲染器", rootType = typeof(ToolsManager))]
    [Name("Gizmo渲染器")]
    [Tip("本组件仅在编辑器中生效", "This component only works in the editor")]
    [DisallowMultipleComponent]
    public class GizmoRenderer : ToolRenderer
    {
        /// <summary>
        /// Gizmo形状类型
        /// </summary>
        [Name("Gizmo形状类型")]
        [EnumPopup]
        public EGizmoShapeType gizmoShapeType = EGizmoShapeType.Cube;

        /// <summary>
        /// 尺寸规则
        /// </summary>
        [Name("尺寸规则")]
        [EnumPopup]
        public ESizeRule _sizeRule = ESizeRule.TransformLocalScale;

        /// <summary>
        /// 尺寸规则
        /// </summary>
        [Name("尺寸规则")]
        public enum ESizeRule
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None,

            /// <summary>
            /// 变换本地缩放
            /// </summary>
            [Name("变换本地缩放")]
            TransformLocalScale,

            /// <summary>
            /// 值
            /// </summary>
            [Name("值")]
            Value,

            /// <summary>
            /// 组件值
            /// </summary>
            [Name("组件值")]
            ComponentValue,
        }

        /// <summary>
        /// 尺寸值
        /// </summary>
        [Name("尺寸值")]
        [HideInSuperInspector(nameof(_sizeRule), EValidityCheckType.NotEqual, ESizeRule.Value)]
        public Vector3 sizeValue = Vector3.one;

        /// <summary>
        /// 组件
        /// </summary>
        [Name("组件")]
        [HideInSuperInspector(nameof(_sizeRule), EValidityCheckType.NotEqual, ESizeRule.ComponentValue)]
        [ComponentPopup(typeof(IGizmoRendererValue))]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public MonoBehaviour _component;

        /// <summary>
        /// Gizmos颜色
        /// </summary>
        [Name("Gizmos颜色")]
        public Color gizmosColor = Color.white;

        /// <summary>
        /// 总是显示Gizmos:游戏对象未选中是否执行绘制Gizmos
        /// </summary>
        [Name("总是显示Gizmos")]
        [Tip("游戏对象未选中是否执行绘制Gizmos", "The game object is not selected. Do you want to execute drawing gizmos")]
        public bool alwayShowGizmos = true;

        /// <summary>
        /// 绘制标签枚举
        /// </summary>
        [Name("绘制标签")]
        public enum EDrawLabel
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None,

            /// <summary>
            /// 名称
            /// </summary>
            [Name("名称")]
            Name,

            /// <summary>
            /// 文本
            /// </summary>
            [Name("文本")]
            Text,
        };

        /// <summary>
        /// 绘制标签
        /// </summary>
        [Name("绘制标签")]
        [EnumPopup]
        public EDrawLabel _drawLabel = EDrawLabel.None;

        /// <summary>
        /// 绘制标签
        /// </summary>
        public EDrawLabel drawLabel
        {
            get => _drawLabel;
            set => this.XModifyProperty(ref _drawLabel, value);
        }

        /// <summary>
        /// 文本
        /// </summary>
        [Name("文本")]
        [HideInSuperInspector(nameof(_drawLabel), EValidityCheckType.NotEqual, EDrawLabel.Text)]
        public string _text = "";

        /// <summary>
        /// 文本
        /// </summary>
        public string text
        {
            get => _text;
            set => this.XModifyProperty(ref _text, value);
        }

        /// <summary>
        /// 绘制标签文本
        /// </summary>
        public string drawLabelText
        {
            get
            {
                switch(_drawLabel)
                {
                    case EDrawLabel.Name: return name;
                    case EDrawLabel.Text: return _text;
                    default:return "";
                }
            }
        }

        /// <summary>
        /// 标签颜色
        /// </summary>
        [Name("标签颜色")]
        [HideInSuperInspector(nameof(_drawLabel), EValidityCheckType.Equal, EDrawLabel.None)]
        public Color _labelColor = Color.black;

        /// <summary>
        /// 字体尺寸
        /// </summary>
        [Name("字体尺寸")]
        [HideInSuperInspector(nameof(_drawLabel), EValidityCheckType.Equal, EDrawLabel.None)]
        public int _fontSize = 24;

        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            gizmoShapeType = EGizmoShapeType.Cube;
            gizmosColor.a = 0.5f;
            sizeValue = Vector3.one;
            alwayShowGizmos = true;
        }

        /// <summary>
        /// 绘制Gizmos
        /// </summary>
        public void OnDrawGizmos()
        {
            if(alwayShowGizmos) DrawGizmos();
        }

        /// <summary>
        /// 选中时绘制Gizmos
        /// </summary>
        public void OnDrawGizmosSelected()
        {
            DrawGizmos();
        }

        private void DrawGizmos()
        {
            switch (_sizeRule)
            {
                case ESizeRule.TransformLocalScale:
                    {
                        var transform = this.transform;
                        DrawGizmos(transform.position, transform.rotation, transform.localScale);
                        break;
                    }
                case ESizeRule.Value:
                    {
                        var transform = this.transform;
                        DrawGizmos(transform.position, transform.rotation, sizeValue);
                        break;
                    }
                case ESizeRule.ComponentValue:
                    {
                        IGizmoRendererValue rendererValue = _component as IGizmoRendererValue;
                        if (rendererValue == null)
                        {
                            if (_component)
                            {
                                rendererValue = _component.GetComponent<IGizmoRendererValue>();
                            }
                            else
                            {
                                rendererValue = this.GetComponent<IGizmoRendererValue>();
                            }
                            if (rendererValue == null) break;
                        }

                        var transform = this.transform;
                        DrawGizmos(transform.position, transform.rotation, rendererValue.value);
                        break;
                    }
            }
        }

        /// <summary>
        /// 尝试获取渲染尺寸
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public bool TryGetRenderSize(out Vector3 size)
        {
            switch (_sizeRule)
            {
                case ESizeRule.TransformLocalScale:
                    {
                        size = transform.localScale;
                        return true;
                    }
                case ESizeRule.Value:
                    {
                        size = sizeValue;
                        return true;
                    }
                case ESizeRule.ComponentValue:
                    {
                        IGizmoRendererValue rendererValue = _component as IGizmoRendererValue;
                        if (rendererValue == null)
                        {
                            if (_component)
                            {
                                rendererValue = _component.GetComponent<IGizmoRendererValue>();
                            }
                            else
                            {
                                rendererValue = this.GetComponent<IGizmoRendererValue>();
                            }
                            if (rendererValue == null) break;
                        }
                        size = rendererValue.value;
                        return true;
                    }
            }
            size = default;
            return false;
        }

        private void DrawGizmos(Vector3 position, Quaternion rotation, Vector3 size)
        {
            var mat = Matrix4x4.TRS(position, rotation, Vector3.one);

            //绘制Gizmos
            var bak = Gizmos.matrix;
            Gizmos.matrix = mat;
            DrawGizmos(gizmoShapeType, Vector3.zero, size, gizmosColor);
            Gizmos.matrix = bak;

#if UNITY_EDITOR

            if (_drawLabel != EDrawLabel.None)
            {
                //绘制文字
                var labelStyle = this.labelStyle.obj;
                labelStyle.fontSize = _fontSize;
                labelStyle.normal.textColor = _labelColor;

                bak = UnityEditor.Handles.matrix;
                UnityEditor.Handles.matrix = mat;
                UnityEditor.Handles.Label(new Vector3(0, 0, size.z), drawLabelText, labelStyle);
                UnityEditor.Handles.matrix = bak;
            }
#endif
        }

        private XGUIStyle labelStyle = new XGUIStyle(nameof(GUI.Label));

        private static void DrawGizmos(EGizmoShapeType type, Vector3 center, Vector3 size, Color color)
        {
            Color c = Gizmos.color;
            Gizmos.color = color;
            switch (type)
            {
                case EGizmoShapeType.Cube:
                    {
                        Gizmos.DrawCube(center, size);
                        break;
                    }
                case EGizmoShapeType.WireCube:
                    {
                        Gizmos.DrawWireCube(center, size);
                        break;
                    }
                case EGizmoShapeType.Sphere:
                    {
                        Gizmos.DrawSphere(center, size.magnitude);
                        break;
                    }
                case EGizmoShapeType.WireSphere:
                    {
                        Gizmos.DrawWireSphere(center, size.magnitude);
                        break;
                    }
            }
            Gizmos.color = c;
        }

        #region 绘制图标 
        // 1、绘制在对象中心
        // 2、绘制在对象顶部（带线和不带线）
        // 3、绘制任意指向位置

        /// <summary>
        /// 在对象中心绘制图标
        /// </summary>
        /// <param name="mb">Unity MonoBehaviour 组件</param>
        /// <param name="icon">图标为空，则自动反射mb对象的Icon属性</param>
        public static void DrawIcon(MonoBehaviour mb, string icon = null)
        {
            DrawIcon(mb.transform, icon, mb.GetType());
        }

        /// <summary>
        /// 在对象中心绘制图标
        /// </summary>
        /// <param name="transform">Unity 变换组件</param>
        /// <param name="icon">图标为空，则自动反射type对象的Icon属性</param>
        /// <param name="type">外部类型</param> 
        public static void DrawIcon(Transform transform, string icon = null, Type type = null)
        {
            DrawIconWithLine(transform, icon, type, false, Color.green, Vector3.zero, 0);
        }

        /// <summary>
        /// 在对象包围盒顶部绘制图标
        /// </summary>
        /// <param name="mb">Unity MonoBehaviour 组件</param>
        /// <param name="icon">图标为空，则自动反射mb对象的Icon属性</param>
        /// <param name="displayLine">是否画线</param>
        public static void DrawIconOnBoundsTop(MonoBehaviour mb, string icon = null, bool displayLine = true)
        {
            DrawIconWithLine(mb.transform, icon, mb.GetType(), displayLine, Color.green, Vector3.up, -1);
        }

        /// <summary>
        /// 在对象包围盒绘制图标
        /// </summary>
        /// <param name="mb">Unity 变换组件</param>
        /// <param name="icon">图标为空，则自动反射type对象的Icon属性</param>
        /// <param name="type">外部类型</param> 
        /// <param name="displayLine">是否画线</param>
        public static void DrawIconOnBoundsTop(Transform transform, string icon = null, Type type = null, bool displayLine = true)
        {
            DrawIconWithLine(transform, icon, type, displayLine, Color.green, Vector3.up, -1);
        }

        public static void DrawIconWithLine(Transform transform, string icon, Type type, bool displayLine, Color lineColor, Vector3 lineDir, float lineLength = -1)
        {
            Vector3 beginPos = transform.position;

            if (lineLength < 0)
            {
                Bounds bounds;
                CommonFun.GetBounds(out bounds, transform.gameObject);
                lineLength = bounds.size.y/2 + 0.2f;
            }
            Vector3 offset = lineDir * lineLength;

            if (displayLine)
            {
                var orgColor = Gizmos.color;
                {
                    Gizmos.color = lineColor;
                    Gizmos.DrawLine(beginPos, beginPos + offset);
                }
                Gizmos.color = orgColor;
            }
            
            // 绘制图标
            if (string.IsNullOrEmpty(icon))
            {
                icon = AttributeHelper.GetAttribute<Attributes.IconAttribute>(type, true)?.icon;
                if (icon.LastIndexOf("/") is int index && index >= 0)
                {
                    icon = icon.Substring(index + 1);
                }
            }
            Gizmos.DrawIcon(transform.position + offset, string.IsNullOrEmpty(icon) ? "Default Icon.tiff" : icon);
        }

        #endregion
    }

    /// <summary>
    /// Gizmo渲染器值接口
    /// </summary>
    public interface IGizmoRendererValue
    {
        /// <summary>
        /// 值
        /// </summary>
        Vector3 value { get; }
    }

    /// <summary>
    /// Gizmo形状类型
    /// </summary>
    [Name("Gizmo形状类型")]
    public enum EGizmoShapeType
    {
        /// <summary>
        /// 立方体
        /// </summary>
        [Name("立方体")]
        Cube = 0,

        /// <summary>
        /// 线立方体
        /// </summary>
        [Name("线立方体")]
        WireCube,

        /// <summary>
        /// 球
        /// </summary>
        [Name("球")]
        Sphere,

        /// <summary>
        /// 线球
        /// </summary>
        [Name("线球")]
        WireSphere,
    }
}
