using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using XCSJ.PluginCommonUtils;

namespace XCSJ.Extension.Base.Tweens
{
    /// <summary>
    /// 动画曲线助手类
    /// </summary>
    public static class AnimationCurveHelper
    {
        /// <summary>
        /// 线性
        /// </summary>
        /// <returns></returns>
        public static AnimationCurve Linear()
        {
            AnimationCurve curve = new AnimationCurve();
            curve.AddKey(new Keyframe(0f, 0f, 1f, 1f));
            curve.AddKey(new Keyframe(1f, 1f, 1f, 1f));
            return curve;
        }

        /// <summary>
        /// 弹性
        /// </summary>
        /// <returns></returns>
        public static AnimationCurve Spring()
        {
            AnimationCurve curve = new AnimationCurve();
            curve.AddKey(new Keyframe(0.000f, 0.00f, 3.51f, 3.51f));
            curve.AddKey(new Keyframe(0.367f, 0.88f, 1.79f, 1.79f));
            curve.AddKey(new Keyframe(0.615f, 1.08f, -0.28f, -0.28f));
            curve.AddKey(new Keyframe(0.795f, 0.97f, 0.03f, 0.03f));
            curve.AddKey(new Keyframe(0.901f, 1.01f, 0.20f, 0.20f));
            curve.AddKey(new Keyframe(1.000f, 1.00f, 0.00f, 0.00f));
            return curve;
        }

        /// <summary>
        /// 二次方缓入
        /// </summary>
        /// <returns></returns>
        public static AnimationCurve EaseInQuad()
        {
            AnimationCurve curve = new AnimationCurve();
            curve.AddKey(new Keyframe(0f, 0f, 0f, 0f));
            curve.AddKey(new Keyframe(1f, 1f, 2f, 2f));
            return curve;
        }

        /// <summary>
        /// 二次方缓出
        /// </summary>
        /// <returns></returns>
        public static AnimationCurve EaseOutQuad()
        {
            AnimationCurve curve = new AnimationCurve();
            curve.AddKey(new Keyframe(0f, 0f, 2f, 2f));
            curve.AddKey(new Keyframe(1f, 1f, 0f, 0f));
            return curve;
        }

        /// <summary>
        /// 二次方缓入缓出
        /// </summary>
        /// <returns></returns>
        public static AnimationCurve EaseInOutQuad()
        {
            AnimationCurve curve = new AnimationCurve();
            curve.AddKey(new Keyframe(0f, 0f, 0f, 0f));
            curve.AddKey(new Keyframe(0.5f, 0.5f, 2f, 2f));
            curve.AddKey(new Keyframe(1f, 1f, 0f, 0f));
            return curve;
        }

        /// <summary>
        /// 三次方缓入
        /// </summary>
        /// <returns></returns>
        public static AnimationCurve EaseInCubic()
        {
            AnimationCurve curve = new AnimationCurve();
            curve.AddKey(new Keyframe(0f, 0f, 0f, 0f));
            curve.AddKey(new Keyframe(1f, 1f, 3f, 3f));
            return curve;
        }

        /// <summary>
        /// 三次方缓出
        /// </summary>
        /// <returns></returns>
        public static AnimationCurve EaseOutCubic()
        {
            AnimationCurve curve = new AnimationCurve();
            curve.AddKey(new Keyframe(0f, 0f, 3f, 3f));
            curve.AddKey(new Keyframe(1f, 1f, 0f, 0f));
            return curve;
        }

        /// <summary>
        /// 三次方缓入缓出
        /// </summary>
        /// <returns></returns>
        public static AnimationCurve EaseInOutCubic()
        {
            AnimationCurve curve = new AnimationCurve();
            curve.AddKey(new Keyframe(0f, 0f, 0f, 0f));
            curve.AddKey(new Keyframe(0.5f, 0.5f, 3f, 3f));
            curve.AddKey(new Keyframe(1f, 1f, 0f, 0f));
            return curve;
        }

        /// <summary>
        /// 四次方缓入
        /// </summary>
        /// <returns></returns>
        public static AnimationCurve EaseInQuart()
        {
            AnimationCurve curve = new AnimationCurve();
            curve.AddKey(new Keyframe(0f, 0f, 0f, 0f));
            curve.AddKey(new Keyframe(0.5f, 0.064f, 0.5f, 0.5f));
            curve.AddKey(new Keyframe(1f, 1f, 4f, 4f));
            return curve;
        }

        /// <summary>
        /// 四次方缓出
        /// </summary>
        /// <returns></returns>
        public static AnimationCurve EaseOutQuart()
        {
            AnimationCurve curve = new AnimationCurve();
            curve.AddKey(new Keyframe(0f, 0f, 4f, 4f));
            curve.AddKey(new Keyframe(0.5f, 0.936f, 0.5f, 0.5f));
            curve.AddKey(new Keyframe(1f, 1f, 0f, 0f));
            return curve;
        }

        /// <summary>
        /// 四次方缓入缓出
        /// </summary>
        /// <returns></returns>
        public static AnimationCurve EaseInOutQuart()
        {
            AnimationCurve curve = new AnimationCurve();
            curve.AddKey(new Keyframe(0f, 0f, 0f, 0f));
            curve.AddKey(new Keyframe(0.25f, 0.032f, 0.5f, 0.5f));
            curve.AddKey(new Keyframe(0.5f, 0.5f, 4f, 4f));
            curve.AddKey(new Keyframe(0.75f, 0.968f, 0.5f, 0.5f));
            curve.AddKey(new Keyframe(1f, 1f, 0f, 0f));
            return curve;
        }

        /// <summary>
        /// 五次方缓入
        /// </summary>
        /// <returns></returns>
        public static AnimationCurve EaseInQuint()
        {
            AnimationCurve curve = new AnimationCurve();
            curve.AddKey(new Keyframe(0f, 0f, 0f, 0f));
            curve.AddKey(new Keyframe(0.2f, 0f, 0.033f, 0.033f));
            curve.AddKey(new Keyframe(0.6f, 0.077f, 0.65f, 0.65f));
            curve.AddKey(new Keyframe(1f, 1f, 5f, 5f));
            return curve;
        }

        /// <summary>
        /// 五次方缓出
        /// </summary>
        /// <returns></returns>
        public static AnimationCurve EaseOutQuint()
        {
            AnimationCurve curve = new AnimationCurve();
            curve.AddKey(new Keyframe(0f, 0f, 5f, 5f));
            curve.AddKey(new Keyframe(0.4f, 0.92f, 0.65f, 0.65f));
            curve.AddKey(new Keyframe(0.8f, 1f, 0.033f, 0.033f));
            curve.AddKey(new Keyframe(1f, 1f, 0f, 0f));
            return curve;
        }

        /// <summary>
        /// 五次方缓入缓出
        /// </summary>
        /// <returns></returns>
        public static AnimationCurve EaseInOutQuint()
        {
            AnimationCurve curve = new AnimationCurve();
            curve.AddKey(new Keyframe(0f, 0f, 0f, 0f));
            curve.AddKey(new Keyframe(0.1f, 0f, 0f, 0f));
            curve.AddKey(new Keyframe(0.3f, 0.04f, 0.65f, 0.65f));
            curve.AddKey(new Keyframe(0.5f, 0.5f, 5f, 5f));
            curve.AddKey(new Keyframe(0.7f, 0.96f, 0.65f, 0.65f));
            curve.AddKey(new Keyframe(0.9f, 1f, 0f, 0f));
            curve.AddKey(new Keyframe(1f, 1f, 0f, 0f));
            return curve;
        }

        /// <summary>
        /// 正弦缓入
        /// </summary>
        /// <returns></returns>
        public static AnimationCurve EaseInSine()
        {
            AnimationCurve curve = new AnimationCurve();
            curve.AddKey(new Keyframe(0f, 0f, 0f, 0f));
            curve.AddKey(new Keyframe(0.5f, 0.292f, 1.11f, 1.11f));
            curve.AddKey(new Keyframe(1f, 1f, 1.56f, 1.56f));
            return curve;
        }

        /// <summary>
        /// 正弦缓出
        /// </summary>
        /// <returns></returns>
        public static AnimationCurve EaseOutSine()
        {
            AnimationCurve curve = new AnimationCurve();
            curve.AddKey(new Keyframe(0f, 0f, 1.56f, 1.56f));
            curve.AddKey(new Keyframe(0.5f, 0.708f, 1.11f, 1.11f));
            curve.AddKey(new Keyframe(1f, 1f, 0f, 0f));
            return curve;
        }

        /// <summary>
        /// 正弦缓入缓入
        /// </summary>
        /// <returns></returns>
        public static AnimationCurve EaseInOutSine()
        {
            AnimationCurve curve = new AnimationCurve();
            curve.AddKey(new Keyframe(0f, 0f, 0f, 0f));
            curve.AddKey(new Keyframe(0.25f, 0.145f, 1.1f, 1.1f));
            curve.AddKey(new Keyframe(0.5f, 0.5f, 1.6f, 1.6f));
            curve.AddKey(new Keyframe(0.75f, 0.853f, 1.1f, 1.1f));
            curve.AddKey(new Keyframe(1f, 1f, 0f, 0f));
            return curve;
        }

        /// <summary>
        /// 指数缓入
        /// </summary>
        /// <returns></returns>
        public static AnimationCurve EaseInExpo()
        {
            AnimationCurve curve = new AnimationCurve();
            curve.AddKey(new Keyframe(0f, 0f, 0.031f, 0.031f));
            curve.AddKey(new Keyframe(0.5f, 0.031f, 0.214f, 0.214f));
            curve.AddKey(new Keyframe(0.8f, 0.249f, 1.682f, 1.682f));
            curve.AddKey(new Keyframe(1f, 1f, 6.8f, 6.8f));
            return curve;
        }

        /// <summary>
        /// 指数缓出
        /// </summary>
        /// <returns></returns>
        public static AnimationCurve EaseOutExpo()
        {
            AnimationCurve curve = new AnimationCurve();
            curve.AddKey(new Keyframe(0f, 0f, 6.8f, 6.8f));
            curve.AddKey(new Keyframe(0.2f, 0.751f, 1.682f, 1.682f));
            curve.AddKey(new Keyframe(0.5f, 0.969f, 0.214f, 0.214f));
            curve.AddKey(new Keyframe(1f, 1f, 0.031f, 0.031f));
            return curve;
        }

        /// <summary>
        /// 指数缓入缓入
        /// </summary>
        /// <returns></returns>
        public static AnimationCurve EaseInOutExpo()
        {
            AnimationCurve curve = new AnimationCurve();
            curve.AddKey(new Keyframe(0f, 0f, 0f, 0f));
            curve.AddKey(new Keyframe(0.25f, 0.015f, 0.181f, 0.181f));
            curve.AddKey(new Keyframe(0.4f, 0.125f, 1.58f, 1.58f));
            curve.AddKey(new Keyframe(0.5f, 0.5f, 6.8f, 6.8f));
            curve.AddKey(new Keyframe(0.6f, 0.873f, 1.682f, 1.682f));
            curve.AddKey(new Keyframe(0.75f, 0.982f, 0.21f, 0.21f));
            curve.AddKey(new Keyframe(1f, 1f, 0f, 0f));
            return curve;
        }

        /// <summary>
        /// 圆形缓入
        /// </summary>
        /// <returns></returns>
        public static AnimationCurve EaseInCirc()
        {
            AnimationCurve curve = new AnimationCurve();
            curve.AddKey(new Keyframe(0f, 0f, 0.04f, 0.04f));
            curve.AddKey(new Keyframe(0.6f, 0.2f, 0.76f, 0.76f));
            curve.AddKey(new Keyframe(0.9f, 0.562f, 1.92f, 1.92f));
            curve.AddKey(new Keyframe(0.975f, 0.78f, 4.2f, 4.2f));
            curve.AddKey(new Keyframe(1f, 1f, 17.3f, 17.3f));
            return curve;
        }

        /// <summary>
        /// 圆形缓出
        /// </summary>
        /// <returns></returns>
        public static AnimationCurve EaseOutCirc()
        {
            AnimationCurve curve = new AnimationCurve();
            curve.AddKey(new Keyframe(0f, 0f, 17.3f, 17.3f));
            curve.AddKey(new Keyframe(0.025f, 0.22f, 4.2f, 4.2f));
            curve.AddKey(new Keyframe(0.1f, 0.438f, 1.92f, 1.92f));
            curve.AddKey(new Keyframe(0.4f, 0.8f, 0.76f, 0.76f));
            curve.AddKey(new Keyframe(1f, 1f, 0.04f, 0.04f));
            return curve;
        }

        /// <summary>
        /// 圆形缓入缓入
        /// </summary>
        /// <returns></returns>
        public static AnimationCurve EaseInOutCirc()
        {
            AnimationCurve curve = new AnimationCurve();
            curve.AddKey(new Keyframe(0f, 0f, 0f, 0f));
            curve.AddKey(new Keyframe(0.3f, 0.098f, 0.75f, 0.75f));
            curve.AddKey(new Keyframe(0.45f, 0.281f, 1.96f, 1.96f));
            curve.AddKey(new Keyframe(0.4875f, 0.392f, 4.4f, 4.4f));
            curve.AddKey(new Keyframe(0.5f, 0.5f, 8.14f, 8.14f));
            curve.AddKey(new Keyframe(0.5125f, 0.607f, 4.3f, 4.3f));
            curve.AddKey(new Keyframe(0.55f, 0.717f, 1.94f, 1.94f));
            curve.AddKey(new Keyframe(0.7f, 0.899f, 0.74f, 0.74f));
            curve.AddKey(new Keyframe(1f, 1f, 0f, 0f));
            return curve;
        }

        /// <summary>
        /// 反弹缓入
        /// </summary>
        /// <returns></returns>
        public static AnimationCurve EaseInBounce()
        {
            AnimationCurve curve = new AnimationCurve();
            curve.AddKey(new Keyframe(0f, 0f, 0.715f, 0.715f));
            curve.AddKey(new Keyframe(0.091f, 0f, -0.677f, 1.365f));
            curve.AddKey(new Keyframe(0.272f, 0f, -1.453f, 2.716f));
            curve.AddKey(new Keyframe(0.636f, 0f, -2.775f, 5.517f));
            curve.AddKey(new Keyframe(1f, 1f, -0.0023f, -0.0023f));
            return curve;
        }

        /// <summary>
        /// 反弹缓出
        /// </summary>
        /// <returns></returns>
        public static AnimationCurve EaseOutBounce()
        {
            AnimationCurve curve = new AnimationCurve();
            curve.AddKey(new Keyframe(0f, 0f, -0.042f, -0.042f));
            curve.AddKey(new Keyframe(0.364f, 1f, 5.414f, -2.758f));
            curve.AddKey(new Keyframe(0.727f, 1f, 2.773f, -1.295f));
            curve.AddKey(new Keyframe(0.909f, 1f, 1.435f, -0.675f));
            curve.AddKey(new Keyframe(1f, 1f, 0.735f, 0.735f));
            return curve;
        }

        /// <summary>
        /// 反弹缓入缓出
        /// </summary>
        /// <returns></returns>
        public static AnimationCurve EaseInOutBounce()
        {
            AnimationCurve curve = new AnimationCurve();
            curve.AddKey(new Keyframe(0f, 0f, 0.682f, 0.682f));
            curve.AddKey(new Keyframe(0.046f, 0f, -0.732f, 1.316f));
            curve.AddKey(new Keyframe(0.136f, 0f, -1.568f, 2.608f));
            curve.AddKey(new Keyframe(0.317f, 0f, -2.908f, 5.346f));
            curve.AddKey(new Keyframe(0.5f, 0.5f, -0.061f, 0.007f));
            curve.AddKey(new Keyframe(0.682f, 1f, 5.463f, -2.861f));
            curve.AddKey(new Keyframe(0.864f, 1f, 2.633f, -1.258f));
            curve.AddKey(new Keyframe(0.955f, 1f, 1.488f, -0.634f));
            curve.AddKey(new Keyframe(1f, 1f, 0.804f, 0.804f));
            return curve;
        }

        /// <summary>
        /// 回归缓入
        /// </summary>
        /// <returns></returns>
        public static AnimationCurve EaseInBack()
        {
            AnimationCurve curve = new AnimationCurve();
            curve.AddKey(new Keyframe(0.00f, 0.00f, 0.00f, 0.00f));
            curve.AddKey(new Keyframe(1.00f, 1.00f, 4.71f, 4.71f));
            return curve;
        }

        /// <summary>
        /// 回归缓出
        /// </summary>
        /// <returns></returns>
        public static AnimationCurve EaseOutBack()
        {
            AnimationCurve curve = new AnimationCurve();
            curve.AddKey(new Keyframe(0.00f, 0.00f, 4.71f, 4.71f));
            curve.AddKey(new Keyframe(1.00f, 1.00f, 0.00f, 0.00f));
            return curve;
        }

        /// <summary>
        /// 回归缓入缓出
        /// </summary>
        /// <returns></returns>
        public static AnimationCurve EaseInOutBack()
        {
            AnimationCurve curve = new AnimationCurve();
            curve.AddKey(new Keyframe(0.00f, 0.00f, 0.00f, 0.00f));
            curve.AddKey(new Keyframe(0.50f, 0.50f, 5.61f, 5.61f));
            curve.AddKey(new Keyframe(1.00f, 1.00f, 0.00f, 0.00f));
            return curve;
        }

        /// <summary>
        /// 弹性缓入
        /// </summary>
        /// <returns></returns>
        public static AnimationCurve EaseInElastic()
        {
            AnimationCurve curve = new AnimationCurve();
            curve.AddKey(new Keyframe(0.00f, 0.00f, 0.00f, 0.00f));
            curve.AddKey(new Keyframe(0.15f, 0.00f, -0.04f, -0.04f));
            curve.AddKey(new Keyframe(0.30f, -0.005f, 0.04f, 0.04f));
            curve.AddKey(new Keyframe(0.42f, 0.02f, -0.07f, -0.07f));
            curve.AddKey(new Keyframe(0.58f, -0.04f, 0.15f, 0.15f));
            curve.AddKey(new Keyframe(0.72f, 0.13f, 0.20f, 0.20f));
            curve.AddKey(new Keyframe(0.80f, -0.13f, -5.33f, -5.33f));
            curve.AddKey(new Keyframe(0.868f, -0.375f, 0.14f, 0.14f));
            curve.AddKey(new Keyframe(0.92f, -0.05f, 11.32f, 11.32f));
            curve.AddKey(new Keyframe(1.00f, 1.00f, 7.50f, 7.50f));
            return curve;
        }

        /// <summary>
        /// 弹性缓出
        /// </summary>
        /// <returns></returns>
        public static AnimationCurve EaseOutElastic()
        {
            AnimationCurve curve = new AnimationCurve();
            curve.AddKey(new Keyframe(0.000f, 0.00f, 6.56f, 6.56f));
            curve.AddKey(new Keyframe(0.079f, 1.06f, 11.22f, 11.22f));
            curve.AddKey(new Keyframe(0.134f, 1.38f, 0.03f, 0.03f));
            curve.AddKey(new Keyframe(0.204f, 1.10f, -5.24f, -5.24f));
            curve.AddKey(new Keyframe(0.289f, 0.87f, 0.65f, 0.65f));
            curve.AddKey(new Keyframe(0.424f, 1.05f, 0.13f, 0.13f));
            curve.AddKey(new Keyframe(0.589f, 0.98f, 0.12f, 0.12f));
            curve.AddKey(new Keyframe(0.696f, 1.00f, 0.07f, 0.07f));
            curve.AddKey(new Keyframe(0.898f, 1.00f, 0.00f, 0.00f));
            curve.AddKey(new Keyframe(1.000f, 1.00f, 0.00f, 0.00f));
            return curve;
        }

        /// <summary>
        /// 弹性缓入缓出
        /// </summary>
        /// <returns></returns>
        public static AnimationCurve EaseInOutElastic()
        {
            AnimationCurve curve = new AnimationCurve();
            curve.AddKey(new Keyframe(0.000f, 0.00f, 0.00f, 0.00f));
            curve.AddKey(new Keyframe(0.093f, 0.00f, -0.05f, -0.05f));
            curve.AddKey(new Keyframe(0.149f, 0.00f, 0.06f, 0.06f));
            curve.AddKey(new Keyframe(0.210f, 0.01f, -0.04f, -0.04f));
            curve.AddKey(new Keyframe(0.295f, -0.02f, 0.31f, 0.31f));
            curve.AddKey(new Keyframe(0.356f, 0.07f, 0.11f, 0.11f));
            curve.AddKey(new Keyframe(0.400f, -0.06f, -5.12f, -5.12f));
            curve.AddKey(new Keyframe(0.435f, -0.19f, 0.18f, 0.18f));
            curve.AddKey(new Keyframe(0.463f, 0.02f, 12.44f, 12.44f));
            curve.AddKey(new Keyframe(0.500f, 0.50f, 8.33f, 8.33f));
            curve.AddKey(new Keyframe(0.540f, 1.03f, 12.05f, 12.05f));
            curve.AddKey(new Keyframe(0.568f, 1.18f, 0.31f, 0.31f));
            curve.AddKey(new Keyframe(0.604f, 1.04f, -5.03f, -5.03f));
            curve.AddKey(new Keyframe(0.645f, 0.93f, 0.36f, 0.36f));
            curve.AddKey(new Keyframe(0.705f, 1.02f, 0.39f, 0.39f));
            curve.AddKey(new Keyframe(0.786f, 0.99f, -0.04f, -0.04f));
            curve.AddKey(new Keyframe(0.848f, 1.00f, 0.04f, 0.04f));
            curve.AddKey(new Keyframe(0.900f, 1.00f, -0.01f, -0.01f));
            curve.AddKey(new Keyframe(1.000f, 1.00f, 0.00f, 0.00f));
            return curve;
        }

        /// <summary>
        /// 重击
        /// </summary>
        /// <returns></returns>
        public static AnimationCurve Punch()
        {
            AnimationCurve curve = new AnimationCurve();
            for (int i = 0; i <= 100; i++)
            {
                var time = i * 1f / 100;
                curve.AddKey(time, EaseCurveHelper.Punch(1, time));
            }
            return curve;
        }

        /// <summary>
        /// 创建动画曲线
        /// </summary>
        /// <param name="easeType"></param>
        /// <returns></returns>
        public static AnimationCurve Create(EEaseType easeType)
        {
            switch (easeType)
            {
                case EEaseType.Linear: return Linear();
                case EEaseType.Spring: return Spring();

                case EEaseType.EaseInQuad: return EaseInQuad();
                case EEaseType.EaseOutQuad: return EaseOutQuad();
                case EEaseType.EaseInOutQuad: return EaseInOutQuad();

                case EEaseType.EaseInCubic: return EaseInCubic();
                case EEaseType.EaseOutCubic: return EaseOutCubic();
                case EEaseType.EaseInOutCubic: return EaseInOutCubic();

                case EEaseType.EaseInQuart: return EaseInQuart();
                case EEaseType.EaseOutQuart: return EaseOutQuart();
                case EEaseType.EaseInOutQuart: return EaseInOutQuart();

                case EEaseType.EaseInQuint: return EaseInQuint();
                case EEaseType.EaseOutQuint: return EaseOutQuint();
                case EEaseType.EaseInOutQuint: return EaseInOutQuint();

                case EEaseType.EaseInSine: return EaseInSine();
                case EEaseType.EaseOutSine: return EaseOutSine();
                case EEaseType.EaseInOutSine: return EaseInOutSine();

                case EEaseType.EaseInExpo: return EaseInExpo();
                case EEaseType.EaseOutExpo: return EaseOutExpo();
                case EEaseType.EaseInOutExpo: return EaseInOutExpo();

                case EEaseType.EaseInCirc: return EaseInCirc();
                case EEaseType.EaseOutCirc: return EaseOutCirc();
                case EEaseType.EaseInOutCirc: return EaseInOutCirc();

                case EEaseType.EaseInBounce: return EaseInBounce();
                case EEaseType.EaseOutBounce: return EaseOutBounce();
                case EEaseType.EaseInOutBounce: return EaseInOutBounce();

                case EEaseType.EaseInBack: return EaseInBack();
                case EEaseType.EaseOutBack: return EaseOutBack();
                case EEaseType.EaseInOutBack: return EaseInOutBack();

                case EEaseType.EaseInElastic: return EaseInElastic();
                case EEaseType.EaseOutElastic: return EaseOutElastic();
                case EEaseType.EaseInOutElastic: return EaseInOutElastic();

                case EEaseType.Punch: return Punch();
                default: return Linear();
            }
        }

        /// <summary>
        /// 获取缓动曲线的预设名称
        /// </summary>
        /// <param name="easeType"></param>
        /// <returns></returns>
        public static string PresetName(EEaseType easeType)
        {
            var c = CommonFun.NameTooltip(easeType);
            return string.Format("{0}({1})\n{2}", c.text, easeType.ToString(), c.tooltip);
        }

        /// <summary>
        /// 上下镜像曲线
        /// </summary>
        /// <param name="animationCurve"></param>
        /// <param name="yMirror"></param>
        /// <returns></returns>
        public static AnimationCurve UpDownMirrorCurve(this AnimationCurve animationCurve, float yMirror)
        {
            var keys = animationCurve.keys.ToList();
            for (int i = 0; i < keys.Count; i++)
            {
                var key = keys[i];

                key.value = 2 * yMirror - key.value;

                key.inTangent = -key.inTangent;
                key.outTangent = -key.outTangent;

                keys[i] = key;
            }
            var mirrorCurve = new AnimationCurve(keys.ToArray());
            return mirrorCurve;
        }

        /// <summary>
        /// 左右镜像曲线
        /// </summary>
        /// <param name="animationCurve"></param>
        /// <param name="xMirror"></param>
        /// <returns></returns>
        public static AnimationCurve LeftRightMirrorCurve(this AnimationCurve animationCurve, float xMirror)
        {
            var keys = animationCurve.keys.ToList();
            for (int i = 0; i < keys.Count; i++)
            {
                var key = keys[i];

                key.time = 2 * xMirror - key.time;

                var inTangent = key.inTangent;
                var outTangent = key.outTangent;
                key.inTangent = -outTangent;
                key.outTangent = -inTangent;

                keys[i] = key;
            }
            var mirrorCurve = new AnimationCurve(keys.ToArray());
            return mirrorCurve;
        }
    }
}
