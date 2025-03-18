using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.Tools;

namespace XCSJ.PluginTools.Others
{
    /// <summary>
    /// 跟踪陀螺仪:将当前游戏对象的局部旋转与陀螺仪绑定并进行持续性的跟踪
    /// </summary>
    [Name("跟踪陀螺仪")]
    [Tip("将当前游戏对象的局部旋转与陀螺仪绑定并进行持续性的跟踪", "Bind the local rotation of the current game object to the gyroscope and track it continuously")]
    public class TrackGyro : InteractProvider
    {
        /// <summary>
        /// 跟踪X
        /// </summary>
        [Name("跟踪X")]
        public bool trackX = false;

        /// <summary>
        /// 跟踪Y
        /// </summary>
        [Name("跟踪Y")]
        public bool trackY = false;

        /// <summary>
        /// 跟踪Z
        /// </summary>
        [Name("跟踪Z")]
        public bool trackZ = false;
  
        private Vector3 lastRotation;

        private float xRotation = 0.0f;

        private float yRotation = 0.0f;

        private float zRotation = 0.0f;

        /// <summary>
        /// 启动
        /// </summary>
        public void Start()
        {
#if UNITY_ANDROID || UNITY_IPHONE
            Vector3 angles = transform.eulerAngles;

            xRotation = angles.y;
            yRotation = angles.x;
            zRotation = angles.z;

            //设置设备陀螺仪的开启/关闭状态，使用陀螺仪功能必须设置为 true  
            Input.gyro.enabled = true;

            //初始角度
            lastRotation = Input.gyro.attitude.eulerAngles;
#endif
        }

        /// <summary>
        /// 更新
        /// </summary>
        public void Update()
        {
#if UNITY_ANDROID || UNITY_IPHONE
            //获取最新的陀螺仪旋转角
            var rot = Input.gyro.attitude.eulerAngles;
            var newrot = rot - lastRotation;

            //计算旋转角的改变值
            xRotation = xRotation - newrot.y;
            yRotation = yRotation + newrot.x;
            zRotation = zRotation + newrot.z;
            lastRotation = rot;
#endif
        }

        /// <summary>
        /// 稍后更新
        /// </summary>
        public void LateUpdate()
        {
            Vector3 eura = Vector3.zero;

            //根据当前设置，修改旋转角，保证镜头随陀螺仪旋转
            if (trackX) eura.x = yRotation;
            if (trackY) eura.y = xRotation;
            if (trackZ) eura.z = zRotation;

            var rotation = Quaternion.Euler(eura);

            //修改当前的旋转角
            transform.localRotation = rotation;
        }

        /// <summary>
        /// 设置可跟踪的
        /// </summary>
        /// <param name="track"></param>
        public void SetTracked(bool track)
        {
#if UNITY_ANDROID || UNITY_IPHONE
            if (track)
            {
                lastRotation = Input.gyro.attitude.eulerAngles;
            }
#endif
            this.enabled = track;
        }
    }
}
