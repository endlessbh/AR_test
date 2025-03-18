using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;

namespace XCSJ.PluginOptiTrack.Tools
{
    /// <summary>
    /// 刚体变换通过OptiTrack:通过与Motive软件进行数据流通信，控制目标变换的姿态（位置与旋转）
    /// </summary> 
    [Name("刚体变换通过OptiTrack")]
    [Tip("通过与Motive软件进行数据流通信，控制目标变换的姿态（位置与旋转）", "Through data flow communication with Motive software, the attitude (position and rotation) of the target is controlled")]
    [Tool(OptiTrackHelper.Title, rootType = typeof(OptiTrackManager))]
    [DisallowMultipleComponent]
    public class RigidBodyTransformByOptiTrack : BaseTransformByOptiTrack
    {
        /// <summary>
        /// 目标变换:用于控制的目标变换
        /// </summary>
        [Name("目标变换")]
        [Tip("用于控制的目标变换", "Target transformation for control")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Transform _targetTransform;

        /// <summary>
        /// 目标变换
        /// </summary>
        public override Transform targetTransform
        {
            get
            {
                if (!_targetTransform)
                {
                    _targetTransform = this.transform;
                }
                return _targetTransform;
            }
        }

        /// <summary>
        /// 刚体ID:用于与Motive软件进行数据流通信的刚体ID
        /// </summary>
        [Name("刚体ID")]
        [Tip("用于与Motive软件进行数据流通信的刚体ID", "Rigid body ID for data flow communication with Motive software")]
        public int _rigidBodyID;

        /// <summary>
        /// 刚体ID
        /// </summary>
        public override int rigidBodyID
        {
            get => _rigidBodyID;
            set => this.XModifyProperty(ref _rigidBodyID, value);
        }

        /// <summary>
        /// 空间类型
        /// </summary>
        [Name("空间类型")]
        [EnumPopup]
        public ESpaceType _spaceType = ESpaceType.Local;

        /// <summary>
        /// 空间类型
        /// </summary>
        public override ESpaceType spaceType => _spaceType;

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            if (targetTransform) { }
        }
    }
}
