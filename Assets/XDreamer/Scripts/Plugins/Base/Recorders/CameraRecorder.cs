using UnityEngine;
using XCSJ.PluginCommonUtils;

namespace XCSJ.Extension.Base.Recorders
{
    /// <summary>
    /// 相机属性记录器
    /// </summary>
    public class CameraRecorder : Recorder<Camera, CameraRecorder.Info>
    {
        /// <summary>
        /// 记录信息类
        /// </summary>
        public class Info : ISingleRecord<Camera>
        {
            private Camera camera;

            private bool enabled;
            private float nearClipPlane;
            private float farClipPlane;
            private float fieldOfView;
            private bool orthographic;
            private RenderTexture targetTexture;
            private Rect pixelRect;

            /// <summary>
            /// 记录
            /// </summary>
            /// <param name="camera"></param>
            public void Record(Camera camera)
            {
                this.camera = camera;
                enabled = camera.enabled;
                nearClipPlane = camera.nearClipPlane;
                farClipPlane = camera.farClipPlane;
                fieldOfView = camera.fieldOfView;
                orthographic = camera.orthographic;
                targetTexture = camera.targetTexture;
                pixelRect = camera.pixelRect;
            }

            /// <summary>
            /// 恢复
            /// </summary>
            public void Recover()
            {
                camera.enabled = enabled;
                camera.nearClipPlane = nearClipPlane;
                camera.farClipPlane = farClipPlane;
                camera.fieldOfView = fieldOfView;
                camera.orthographic = orthographic;
                camera.targetTexture = targetTexture;
                camera.pixelRect = pixelRect;
            }
        }
    }
}
