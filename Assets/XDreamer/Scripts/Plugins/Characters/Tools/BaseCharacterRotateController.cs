using UnityEngine;
using XCSJ.Attributes;
using XCSJ.CommonUtils.PluginCharacters;
using XCSJ.Extension.Base.Attributes;
using XCSJ.Extension.Characters.Base;
using XCSJ.Tools;
using XCSJ.PluginTools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.Extension.Base.Algorithms;

namespace XCSJ.Extension.Characters.Tools
{
    /// <summary>
    /// 基础角色旋转控制器
    /// </summary>
    public abstract class BaseCharacterRotateController : BaseCharacterToolController
    {
        /// <summary>
        /// 速度信息
        /// </summary>
        [Name("速度信息")]
        public UpdateVector3RuntimePlatformInfo _speedInfo = new UpdateVector3RuntimePlatformInfo();

        /// <summary>
        /// 本帧的实时速度；已经过时间缩放的速度值；
        /// </summary>
        public Vector3 speedRealtime => _speedInfo.valueRealtime;

        /// <summary>
        /// 旋转模式
        /// </summary>
        [Name("旋转模式")]
        [EnumPopup]
        public ERotateMode _rotateMode = ERotateMode.Self_WorldY;

        /// <summary>
        /// 角度偏移量
        /// </summary>
        [Name("角度偏移量")]
        [Readonly]
        public Vector3 _offset = Vector3.zero;

        /// <summary>
        /// 旋转：使用<see cref="_offset"/>与<see cref="_rotateMode"/>执行旋转逻辑，之后将<see cref="_offset"/>恢复为默认值；
        /// </summary>
        public virtual void Rotate()
        {
            mainController.Rotate(_offset, (int)_rotateMode);
            _offset = Vector3.zero;
        }

        /// <summary>
        /// 唤醒
        /// </summary>
        public override void Awake()
        {
            base.Awake();
            _speedInfo.Init();
        }

        /// <summary>
        /// 更新：使用间隔时间与基础速度更新本帧的实时速度信息
        /// </summary>
        protected virtual void Update()
        {
            //更新速度信息
            _speedInfo.Update(Time.deltaTime, Vector3.one);
        }

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();

            _speedInfo.Reset(new Vector3(0, 540, 0));
        }
    }
}
