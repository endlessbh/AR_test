using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginsCameras.Controllers;
using XCSJ.PluginsCameras.Tools.Base;
using XCSJ.Tools;
using XCSJ.PluginCommonUtils.Tools;

namespace XCSJ.PluginsCameras.Tools.Controllers
{
    /// <summary>
    /// 速度调节器通过键码
    /// </summary>
    [Name("速度调节器通过键码")]
    [Tool(CameraHelperExtension.ControllersCategoryName, nameof(CameraController))]
    [XCSJ.Attributes.Icon(EIcon.Speed)]
    public class SpeedRegulatorByKeyCode : BaseSpeedRegulator
    {
        /// <summary>
        /// 增加键码列表
        /// </summary>
        [Name("增加键码列表")]
        [ValidityCheck(EValidityCheckType.NotNullOrEmpty)]
        public List<KeyCode> _increaseKeyCodes = new List<KeyCode>();

        /// <summary>
        /// 减少键码列表
        /// </summary>
        [Name("减少键码列表")]
        [ValidityCheck(EValidityCheckType.NotNullOrEmpty)]
        public List<KeyCode> _decreaseKeyCodes = new List<KeyCode>();

        /// <summary>
        /// 更新
        /// </summary>
        protected virtual void Update()
        {
            int flag = 0;

            if (_increaseKeyCodes.Count > 0 && _increaseKeyCodes.All(keyCode => Input.GetKey(keyCode)))
            {
                flag++;
            }
            if (_decreaseKeyCodes.Count > 0 && _decreaseKeyCodes.All(keyCode => Input.GetKey(keyCode)))
            {
                flag--;
            }

            if (flag == 0) return;

            UpdateSpeed(flag > 0);
        }

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();

            _increaseKeyCodes.Add(KeyCode.Equals);

            _decreaseKeyCodes.Add(KeyCode.Minus);
        }
    }
}
