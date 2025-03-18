using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCamera.Cameras;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginsCameras.Base;

namespace XCSJ.PluginsCameras.Controllers
{
    /// <summary>
    /// 相机管理器提供者
    /// </summary>
    [Name("相机管理器提供者")]
    public class CameraManagerProvider : BaseCameraManagerProvider
    {
        /// <summary>
        /// 最小切换持续时间:如果切换时间低于本值时，将做直接切换；如果切换时间大于等于本值时，将做补间切换；
        /// </summary>
        [Name("最小切换持续时间")]
        [Tip("如果切换时间低于本值时，将做直接切换；如果切换时间大于等于本值时，将做补间切换；", "If the switching time is lower than this value, it will be switched directly; If the switching time is greater than or equal to this value, make-up switching will be performed;")]
        [Range(0.001f, 1)]
        public float _minSwitchDuration = 0.01f;       

        #region 初始相机控制器

        /// <summary>
        /// 初始相机控制器:程序启动时默认的相机控制器
        /// </summary>
        [Name("初始相机控制器")]
        [Tip("程序启动时默认的相机控制器", "Default camera controller at program startup")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public BaseCameraMainController _initCameraController;

        /// <summary>
        /// 初始相机控制器
        /// </summary>
        public BaseCameraMainController initCameraController
        {
            get => _initCameraController;
            set
            {
                this.XModifyProperty(() => _initCameraController = value);
            }
        }

        /// <summary>
        /// 初始相机控制器规则
        /// </summary>
        [Name("初始相机控制器规则")]
        public enum EInitCameraControllerRule
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None,

            /// <summary>
            /// 仅初始相机控制器激活
            /// </summary>
            [Name("仅初始相机控制器激活")]
            OnlyInitActive,
        }

        /// <summary>
        /// 初始相机控制器规则
        /// </summary>
        [Name("初始相机控制器规则")]
        [EnumPopup]
        public EInitCameraControllerRule _initCameraControllerRule = EInitCameraControllerRule.OnlyInitActive;

        #endregion

        #region 切换相机控制器

        private BaseCameraMainController _currentCameraController;

        /// <summary>
        /// 当前相机控制器
        /// </summary>
        public BaseCameraMainController currentCameraController
        {
            get => _currentCameraController;
            private set
            {
                CameraControllerEvent.CallOnWillSwitchToLast(_currentCameraController);
                lastCameraController = _currentCameraController;

                _currentCameraController = value;

                CameraControllerEvent.CallOnSwitchedToCurrent(_currentCameraController);
            }
        }

        /// <summary>
        /// 上一个相机控制器
        /// </summary>
        public BaseCameraMainController lastCameraController { get; private set; }

        /// <summary>
        /// 标识是否正在处于相机切换中
        /// </summary>
        public bool inSwitch => inSwitchCounter > 0;

        /// <summary>
        /// 切换态计数器
        /// </summary>
        private volatile int inSwitchCounter = 0;

        /// <summary>
        /// 下一个相机控制器：即将切换（或正在切换）到的相机控制器；如果已经切换完成（或未处于正在切换中）本对象为空对象；
        /// </summary>
        public BaseCameraMainController nextCameraController { get; private set; }

        /// <summary>
        /// 切换相机时的补间对象
        /// </summary>
        public object switchTweener { get; private set; }

        /// <summary>
        /// 切换相机控制器
        /// </summary>
        /// <param name="targetCameraController">期望切换到的目标相机控制器</param>
        /// <param name="duration"></param>
        /// <param name="onCompleted"></param>
        /// <param name="mustSwitch"></param>
        /// <returns></returns>
        public bool SwitchCameraController(GameObject targetCameraController, float duration = 0, Action onCompleted = null, bool mustSwitch = false)
        {
            if (!targetCameraController) return false;
            return SwitchCameraController(targetCameraController.GetComponent<BaseCameraMainController>(), duration, onCompleted, mustSwitch);
        }

        /// <summary>
        /// 切换相机控制器
        /// </summary>
        /// <param name="targetCameraController">期望切换到的目标相机控制器</param>
        /// <param name="duration"></param>
        /// <param name="onCompleted"></param>
        /// <param name="mustSwitch"></param>
        /// <returns></returns>
        public bool SwitchCameraController(BaseCameraMainController targetCameraController, float duration = 0, Action onCompleted = null, bool mustSwitch = false)
        {
            //期望切换到的相机控制器无效
            if (!targetCameraController) return false;

            //缓存当前相机控制器
            var currentCameraController = this.currentCameraController;
            if (currentCameraController == targetCameraController) return false;

            //时间过短，立即切换，不再补间
            var directSwitch = duration < _minSwitchDuration;

            if (inSwitch)//正在切换中，当前相机控制器大概率是有效的
            {
                //不强制切换，那么直接返回
                if (!mustSwitch) return false;

                //检查是否支持强制切换操作
                if (currentCameraController && !currentCameraController.cameraTransformer.OnWillBeginMustSwitch(targetCameraController, directSwitch))
                {
                    //不支持强制切换，那么直接返回
                    return false;
                }

                //继续执行相机切换操作
            }
            if (!currentCameraController)//当前相机控制器无效
            {
                //将补间时间设置为0，后续逻辑将执行立即切换
                duration = 0;
                directSwitch = true;
            }

            //切换态计数器增加
            inSwitchCounter++;

            //设置下一个相机
            nextCameraController = targetCameraController;

            //回调将要切换相机控制器事件
            CameraControllerEvent.CallOnBeginSwitch(currentCameraController, nextCameraController);

            if (directSwitch)//直接切换
            {
                OnCompleted(onCompleted);
            }
            else//补间切换，当前相机控制器肯定是有效的
            {
                //开始切换补间
                currentCameraController.cameraTransformer.OnBeginSwitchTween(targetCameraController, duration, () => { OnCompleted(onCompleted); });
            }

            return true;
        }

        private void OnCompleted(Action onCompleted)
        {
            var last = currentCameraController;
            try
            {
                //回调将要结束切换事件
                CameraControllerEvent.CallOnWillEndSwitch(last, nextCameraController);

                //结束切换补间
                if(last) last.cameraTransformer.OnEndSwitchTween(nextCameraController);

                //信息更新
                currentCameraController = nextCameraController;
                nextCameraController = null;

                //事件回调
                onCompleted?.Invoke();
            }
            finally
            {
                //切换态计数器减少
                inSwitchCounter--;
            }

            //回调已切换相机控制器事件
            CameraControllerEvent.CallOnEndSwitch(last, currentCameraController);
        }

        #endregion

        #region MB方法

        /// <summary>
        /// 启动
        /// </summary>
        public void Start()
        {
            //启动时将所有相机控制器事件型组件对象进行事件注册
            foreach(var c in ComponentCache.Get(typeof(ICameraControllerEvent), true).components)
            {
                CameraControllerEvent.Register(c as ICameraControllerEvent);
            }

            switch (_initCameraControllerRule)
            {
                case EInitCameraControllerRule.OnlyInitActive:
                    {
                        foreach (var c in ComponentCache.Get(typeof(BaseCameraMainController), false).components)
                        {
                            var cc = c as BaseCameraMainController;
                            if (cc && cc != initCameraController)
                            {
                                SwitchCameraController(cc);
                            }
                        }
                        SwitchCameraController(initCameraController);
                        break;
                    }
            }

            //捕获事件
            CameraControllerEvent.onEnabled += OnEnabled;
        }

        /// <summary>
        /// 销毁:将所有注册的相机事件直接清空
        /// </summary>
        public virtual void OnDestroy()
        {
            CameraControllerEvent.onEnabled -= OnEnabled;
            CameraControllerEvent.Clear();
        }

        private void OnEnabled(BaseCameraMainController cameraController)
        {
            if (cameraController == currentCameraController) return;
            if (currentCameraController && currentCameraController.isActiveAndEnabled) return;

            //用户直接通过游戏对象激活方式激活相机控制器时，才可能触此情况
            currentCameraController = cameraController;
        }

        #endregion
    }
}
