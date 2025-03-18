using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginTools.Draggers;
using XCSJ.PluginTools.Items;

namespace XCSJ.PluginTools.Hands
{
    /// <summary>
    /// 手类型
    /// </summary>
    public enum EHandType
    {
        [Name("无")]
        None,

        [Name("左")]
        Left,

        [Name("右")]
        Right,
    }

    /// <summary>
    /// 手交互器
    /// </summary>
    [Name("手")]
    public class Hand : InteractProvider
    {
        /// <summary>
        /// 抓取交互器
        /// </summary>
        [Name("抓取交互器")]
        public RigidbodyGrabber _grabber;

        /// <summary>
        /// 抓取交互器
        /// </summary>
        public RigidbodyGrabber grabber => this.XGetComponentInChildrenOrGlobal(ref _grabber);

        /// <summary>
        /// 手类型
        /// </summary>
        [Name("手类型")]
        public EHandType _handType = EHandType.Left;

        /// <summary>
        /// 手指输入器
        /// </summary>
        public IFingerInput fingerInput { get; set; }

        /// <summary>
        /// 手指
        /// </summary>
        [Name("手指")]
        [ValidityCheck(EValidityCheckType.ElementCountGreater, 0)]
        public Finger[] _fingers;

        /// <summary>
        /// 手指数量
        /// </summary>
        public int fingerCount => _fingers.Length;

        private bool fingerInputValid = false;

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            if (fingerInput == null)
            {
                fingerInput = GetComponent<IFingerInput>();
            }
            if (fingerInput != null)
            {
                fingerInputValid = fingerInput.bends.Length == _fingers.Length;
            }

            //CatchInteractor.onCatchEnter += OnGrabEnter;
            //CatchInteractor.onCatchExit += OnGrabExit;

            //CatchInteractor.onReleaseEnter += OnReleaseEnter;
            //CatchInteractor.onReleaseExit += OnReleaseExit;
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            //CatchInteractor.onCatchEnter -= OnGrabEnter;
            //CatchInteractor.onCatchExit -= OnGrabExit;

            //CatchInteractor.onReleaseEnter -= OnReleaseEnter;
            //CatchInteractor.onReleaseExit -= OnReleaseExit;
        }

        private void UpdateFinger()
        {
            if (fingerInputValid)
            {
                for (int i = 0; i < _fingers.Length; i++)
                {
                    _fingers[i].Bend(fingerInput.bends[i]);
                }
            }
        }

        private bool InvalidGrabInteractor(RigidbodyGrabber catcher) => this.grabber != catcher;

        private void OnGrabEnter(RigidbodyGrabber catcher, Grabbable catchable)
        {
            if (InvalidGrabInteractor(catcher)) return;

            //var grabbable = grabber.grabbedObject;
            //if (grabbable != null)
            //{
            //    if (grabbable.gameObject.TryGetComponent<HandPose>(out var handPose))
            //    {
            //        if (handPose.TryGetOffsetToHand(this, out var holdPositionOffset, out var holdRotationOffset))
            //        {
            //            isSetHoldOffset = true;
            //            grabber.holdPositionOffset = holdPositionOffset;
            //            grabber.holdRotationOffset = holdRotationOffset;
            //            Debug.Log("grabber.holdPositionOffset:"+ grabber.holdPositionOffset + ",grabber.holdRotationOffset"
            //                + grabber.holdRotationOffset);
            //        }
            //    }
            //}
        }

        //private bool isSetHoldOffset = false;
        private Vector3 holdPositionOffset = Vector3.zero;
        private Quaternion holdRotationOffset = Quaternion.identity;

        private void OnGrabExit(RigidbodyGrabber grabInteractor, Grabbable catchable)
        {
            //if (InvalidGrabInteractor(catcher)) return;

            //// 尝试使用被抓对象存在的手指弯曲数据
            //var grabbable = grabber.grabbedObject;
            //if (grabbable != null)
            //{
            //    if (grabbable.gameObject.TryGetComponent<HandPose>(out var handPose))
            //    {
            //        handPose.SetPose(this);
            //        return;
            //    }                
            //}

            //// 使用默认弯曲
            //TryBendFingers();
        }

        private void OnReleaseEnter(RigidbodyGrabber grabInteractor, Grabbable catchable)
        {
            if (InvalidGrabInteractor(grabInteractor)) return;

            ExpandFingers();
        }

        private void OnReleaseExit(RigidbodyGrabber grabInteractor, Grabbable catchable)
        {
            //if (InvalidGrabInteractor(catcher)) return;

            //if (isSetHoldOffset)
            //{
            //    isSetHoldOffset = false;
            //    grabber.holdPositionOffset = holdPositionOffset;
            //    grabber.holdRotationOffset = holdRotationOffset;
            //}
        }

        /// <summary>
        /// 展开手指
        /// </summary>
        protected void ExpandFingers()
        {
            for (int i = 0; i < _fingers.Length; i++)
            {
                _fingers[i].BendMin();
            }
        }

        /// <summary>
        /// 尝试弯曲手指至遇到碰撞
        /// </summary>
        [ContextMenu("尝试弯曲手指")]
        public void TryBendFingers()
        {
            for (int i = 0; i < _fingers.Length; i++)
            {
                _fingers[i].BendWithCollision(10);
            }
        }
    }
}
