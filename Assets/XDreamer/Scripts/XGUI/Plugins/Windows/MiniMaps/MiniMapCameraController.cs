using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginXGUI.Base;

namespace XCSJ.PluginXGUI.Windows.MiniMaps
{
    /// <summary>
    /// 导航图相机控制器 : 用于控制相机同步玩家的位置和角度，以及控制正交相机看到的范围
    /// </summary>
    [Name("导航图相机控制器")]
    public class MiniMapCameraController : View
    {
        [Name("关联相机")]
        public Camera _camera = null;

        /// <summary>
        /// 关联相机
        /// </summary>
        public Camera linkCamera { get => _camera;}

        /// <summary>
        /// 导航图跟随玩家移动旋转
        /// </summary>
        [Name("导航图跟随玩家移动旋转")] 
        [Tip("导航图会自动跟随玩家移动和旋转", "The player will follow and rotate the camera automatically")]
        public bool autoFollow = true;

        /// <summary>
        /// 缩放
        /// </summary>
        [Name("缩放")] 
        public float _zoom = 1;
        public float zoom
        {
            get => _zoom;
            set
            {
                _zoom = value;
                _zoom = Mathf.Clamp(_zoom, zoomRange.x, zoomRange.y);
                SetCameraOrthographicSizeZoom(_zoom);
            }
        }

        /// <summary>
        /// 缩放范围
        /// </summary>
        [Name("缩放范围")] 
        public Vector2 zoomRange = new Vector2(0.5f, 2f);

        public Transform player { get; set; }

        private float _orgOrthographicSize = 0;

        private void Awake()
        {
            if (!_camera) _camera = GetComponentInChildren<Camera>();
            _orgOrthographicSize = linkCamera.orthographicSize;
        }

        private void Update()
        {
            if (autoFollow && player)
            {
                transform.position = player.position;

                var angle = transform.eulerAngles;
                angle.y = player.eulerAngles.y;
                transform.eulerAngles = angle;
            }
        }

        private void SetCameraOrthographicSizeZoom(float zoom)
        {
            linkCamera.orthographicSize = _orgOrthographicSize * zoom;
        }

        /// <summary>
        /// 设置缩放偏移量
        /// </summary>
        /// <param name="offset"></param>
        public void SetScaleOffset(float offset)
        {
            zoom += offset;
        }
    }
}
