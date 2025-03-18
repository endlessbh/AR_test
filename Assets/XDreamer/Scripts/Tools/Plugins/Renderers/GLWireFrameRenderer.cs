using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCamera;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginsCameras;
using XCSJ.PluginsCameras.Controllers;
using XCSJ.Tools;
using XCSJ.PluginCommonUtils.Tools;

namespace XCSJ.PluginTools.Renderers
{
    /// <summary>
    /// GL线框渲染器
    /// </summary>
    [RequireComponent(typeof(Camera))]
    [DisallowMultipleComponent]
    [XCSJ.Attributes.Icon(EIcon.WireFrame)]
    [Name(Title)]
    [RequireManager(typeof(CameraManager))]
    [Tip("移动终端(OpenGL-ES)可能无法支持该线框模式。(经测试iOS可支持，安卓不支持)", "The wireframe mode may not be supported by the mobile terminal (opengl-es). (tested, IOS can support, Android does not support)")]
    [Tool(CameraHelperExtension.ControllersCategoryName, nameof(CameraEntityController))]
    public class GLWireFrameRenderer : ToolRenderer
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "GL线框渲染器";

        private Camera _camera = null;

        private CameraClearFlags defaultClearFlags;

        /// <summary>
        /// 唤醒初始化
        /// </summary>
        protected void Awake()
        {
            _camera = GetComponent<Camera>();

            defaultClearFlags = _camera.clearFlags;
        }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            _camera.clearFlags = CameraClearFlags.SolidColor;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _camera.clearFlags = defaultClearFlags;
        }

        protected void OnPreRender()
        {
            GL.wireframe = true;
        }

        void OnPostRender()
        {
            GL.wireframe = false;
        }
    }
}
