using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.Extension.Base.Inputs;
using XCSJ.PluginCommonUtils.Tools;

#if XDREAMER_XR_INTERACTION_TOOLKIT
using UnityEngine.SpatialTracking;
using UnityEngine.XR.Interaction.Toolkit.Inputs;
#endif

namespace XCSJ.PluginXXR.Interaction.Toolkit.Tools.LocomotionProviders
{
	/// <summary>
	/// 旋转IO通过键码:通过键码模拟运动提供者旋转的输入输出
	/// </summary>
	[Name("旋转IO通过键码")]
	[Tip("通过键码模拟运动提供者旋转的输入输出", "Simulate the input and output of the rotation of the motion provider through the key code")]
	[Tool(XRITHelper.IO, nameof(AnalogLocomotionProvider))]
	[XCSJ.Attributes.Icon(EIcon.Keyboard)]
	public class RotateIOByKeyCode : BaseRotateIO
	{
		/// <summary>
		/// 正向设置
		/// </summary>
		[Name("正向设置")]
		public KeyCodesSetting _settingOfForward = new KeyCodesSetting();

		/// <summary>
		/// 反向设置
		/// </summary>
		[Name("反向设置")]
		public KeyCodesSetting _settingOfBack = new KeyCodesSetting();

		/// <summary>
		/// 左转设置
		/// </summary>
		[Name("左转设置")]
		public KeyCodesSetting _settingOfLeft = new KeyCodesSetting();

		/// <summary>
		/// 右转设置
		/// </summary>
		[Name("右转设置")]
		public KeyCodesSetting _settingOfRight = new KeyCodesSetting();

		/// <summary>
		/// 读取输入量
		/// </summary>
		/// <returns></returns>
		protected override Vector2 ReadInput()
		{
			var input = Vector2.zero;
			if (_settingOfForward.IsPressed()) input.y += 1;
			if (_settingOfBack.IsPressed()) input.y -= 1;
			if (_settingOfLeft.IsPressed()) input.x -= 1;
			if (_settingOfRight.IsPressed()) input.x += 1;
			return input;
		}

		/// <summary>
		/// 重置
		/// </summary>
		public override void Reset()
		{
			base.Reset();
			this.XModifyProperty(() =>
			{
				_settingOfForward._keyCode = KeyCode.UpArrow;
				_settingOfBack._keyCode = KeyCode.DownArrow;
				_settingOfLeft._keyCode = KeyCode.LeftArrow;
				_settingOfRight._keyCode = KeyCode.RightArrow;
			});
		}
	}

	/// <summary>
	/// 基础旋转IO
	/// </summary>
	public abstract class BaseRotateIO : BaseAnalogLocomotionProviderIO
	{
		[Name("旋转速度")]
		[Tooltip("顺时针旋转的速度；单位：度/秒；")]
		public float _rotateSpeed = 60f;

		/// <summary>
		/// 更新输入输出
		/// </summary>
		/// <param name="analogLocomotionProvider"></param>
		public override void UpdateIO(AnalogLocomotionProvider analogLocomotionProvider)
		{
			Vector2 input = this.ReadInput();
			float turnAmount = this.GetTurnAmount(input);
			this.TurnRig(analogLocomotionProvider, turnAmount);
		}

		/// <summary>
		/// 读取输入量
		/// </summary>
		/// <returns></returns>
		protected abstract Vector2 ReadInput();

		/// <summary>
		/// 获取转向量
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		protected virtual float GetTurnAmount(Vector2 input)
		{
#if XDREAMER_XR_INTERACTION_TOOLKIT

			if (input == Vector2.zero)
			{
				return 0f;
			}
			Cardinal cardinal = CardinalUtility.GetNearestCardinal(input);
			Cardinal cardinal2 = cardinal;
			if ((uint)cardinal2 > 1u)
			{
				if ((uint)(cardinal2 - 2) <= 1u)
				{
					return input.magnitude * (Mathf.Sign(input.x) * this._rotateSpeed * Time.deltaTime);
				}
			}
#endif
			return 0f;
		}

		/// <summary>
		/// 转向装备
		/// </summary>
		/// <param name="analogLocomotionProvider"></param>
		/// <param name="turnAmount"></param>
		protected void TurnRig(AnalogLocomotionProvider analogLocomotionProvider, float turnAmount)
		{
#if XDREAMER_XR_INTERACTION_TOOLKIT
			if (!Mathf.Approximately(turnAmount, 0f) && analogLocomotionProvider.TryBeginLocomotion())
			{
#if XDREAMER_XR_INTERACTION_TOOLKIT_2_0_0_OR_NEWER
				var xrOrigin = analogLocomotionProvider.system.xrOrigin;
				if (xrOrigin)
				{
					xrOrigin.RotateAroundCameraUsingOriginUp(turnAmount);
				}
#else
				var xrRig = analogLocomotionProvider.system.xrRig;
				if (xrRig)
				{
					xrRig.RotateAroundCameraUsingRigUp(turnAmount);
				}
#endif
				analogLocomotionProvider.TryEndLocomotion();
			}
#endif
			}
	}
}
