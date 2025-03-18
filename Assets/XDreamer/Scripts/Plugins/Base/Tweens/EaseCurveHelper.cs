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
    /// 缓动曲线助手类
    /// </summary>
    public static class EaseCurveHelper
    {
        /// <summary>
        /// 线性
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float Linear(float start, float end, float value)
        {
            return Mathf.Lerp(start, end, value);
        }

        /// <summary>
        /// 类似于线性，但是可越界
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float Clerp(float start, float end, float value)
        {
            float min = 0.0f;
            float max = 360.0f;
            float half = Mathf.Abs((max - min) / 2.0f);
            float retval = 0.0f;
            float diff = 0.0f;
            if ((end - start) < -half)
            {
                diff = ((max - start) + end) * value;
                retval = start + diff;
            }
            else if ((end - start) > half)
            {
                diff = -((max - end) + start) * value;
                retval = start + diff;
            }
            else retval = start + (end - start) * value;
            return retval;
        }

        /// <summary>
        /// 弹性
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float Spring(float start, float end, float value)
        {
            value = Mathf.Clamp01(value);
            value = (Mathf.Sin(value * Mathf.PI * (0.2f + 2.5f * value * value * value)) * Mathf.Pow(1f - value, 2.2f) + value) * (1f + (1.2f * (1f - value)));
            return start + (end - start) * value;
        }

        /// <summary>
        /// 二次方缓入
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float EaseInQuad(float start, float end, float value)
        {
            end -= start;
            return end * value * value + start;
        }

        /// <summary>
        /// 二次方缓出
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float EaseOutQuad(float start, float end, float value)
        {
            end -= start;
            return -end * value * (value - 2) + start;
        }

        /// <summary>
        /// 二次方缓入缓出
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float EaseInOutQuad(float start, float end, float value)
        {
            value /= .5f;
            end -= start;
            if (value < 1) return end / 2 * value * value + start;
            value--;
            return -end / 2 * (value * (value - 2) - 1) + start;
        }

        /// <summary>
        /// 三次方缓入
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float EaseInCubic(float start, float end, float value)
        {
            end -= start;
            return end * value * value * value + start;
        }

        /// <summary>
        /// 三次方缓出
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float EaseOutCubic(float start, float end, float value)
        {
            value--;
            end -= start;
            return end * (value * value * value + 1) + start;
        }

        /// <summary>
        /// 三次方缓入缓出
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float EaseInOutCubic(float start, float end, float value)
        {
            value /= .5f;
            end -= start;
            if (value < 1) return end / 2 * value * value * value + start;
            value -= 2;
            return end / 2 * (value * value * value + 2) + start;
        }

        /// <summary>
        /// 四次方缓入
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float EaseInQuart(float start, float end, float value)
        {
            end -= start;
            return end * value * value * value * value + start;
        }

        /// <summary>
        /// 四次方缓出
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float EaseOutQuart(float start, float end, float value)
        {
            value--;
            end -= start;
            return -end * (value * value * value * value - 1) + start;
        }

        /// <summary>
        /// 四次方缓入缓出
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float EaseInOutQuart(float start, float end, float value)
        {
            value /= .5f;
            end -= start;
            if (value < 1) return end / 2 * value * value * value * value + start;
            value -= 2;
            return -end / 2 * (value * value * value * value - 2) + start;
        }

        /// <summary>
        /// 五次方缓入
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float EaseInQuint(float start, float end, float value)
        {
            end -= start;
            return end * value * value * value * value * value + start;
        }

        /// <summary>
        /// 五次方缓出
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float EaseOutQuint(float start, float end, float value)
        {
            value--;
            end -= start;
            return end * (value * value * value * value * value + 1) + start;
        }

        /// <summary>
        /// 五次方缓入缓出
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float EaseInOutQuint(float start, float end, float value)
        {
            value /= .5f;
            end -= start;
            if (value < 1) return end / 2 * value * value * value * value * value + start;
            value -= 2;
            return end / 2 * (value * value * value * value * value + 2) + start;
        }

        /// <summary>
        /// 正弦缓入
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float EaseInSine(float start, float end, float value)
        {
            end -= start;
            return -end * Mathf.Cos(value / 1 * (Mathf.PI / 2)) + end + start;
        }

        /// <summary>
        /// 正弦缓出
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float EaseOutSine(float start, float end, float value)
        {
            end -= start;
            return end * Mathf.Sin(value / 1 * (Mathf.PI / 2)) + start;
        }

        /// <summary>
        /// 正弦缓入缓入
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float EaseInOutSine(float start, float end, float value)
        {
            end -= start;
            return -end / 2 * (Mathf.Cos(Mathf.PI * value / 1) - 1) + start;
        }

        /// <summary>
        /// 指数缓入
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float EaseInExpo(float start, float end, float value)
        {
            end -= start;
            return end * Mathf.Pow(2, 10 * (value / 1 - 1)) + start;
        }

        /// <summary>
        /// 指数缓出
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float EaseOutExpo(float start, float end, float value)
        {
            end -= start;
            return end * (-Mathf.Pow(2, -10 * value / 1) + 1) + start;
        }

        /// <summary>
        /// 指数缓入缓入
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float EaseInOutExpo(float start, float end, float value)
        {
            value /= .5f;
            end -= start;
            if (value < 1) return end / 2 * Mathf.Pow(2, 10 * (value - 1)) + start;
            value--;
            return end / 2 * (-Mathf.Pow(2, -10 * value) + 2) + start;
        }

        /// <summary>
        /// 圆形缓入
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float EaseInCirc(float start, float end, float value)
        {
            end -= start;
            return -end * (Mathf.Sqrt(1 - value * value) - 1) + start;
        }

        /// <summary>
        /// 圆形缓出
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float EaseOutCirc(float start, float end, float value)
        {
            value--;
            end -= start;
            return end * Mathf.Sqrt(1 - value * value) + start;
        }

        /// <summary>
        /// 圆形缓入缓入
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float EaseInOutCirc(float start, float end, float value)
        {
            value /= .5f;
            end -= start;
            if (value < 1) return -end / 2 * (Mathf.Sqrt(1 - value * value) - 1) + start;
            value -= 2;
            return end / 2 * (Mathf.Sqrt(1 - value * value) + 1) + start;
        }

        /// <summary>
        /// 反弹缓入
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float EaseInBounce(float start, float end, float value)
        {
            end -= start;
            float d = 1f;
            return end - EaseOutBounce(0, end, d - value) + start;
        }

        /// <summary>
        /// 反弹缓出
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float EaseOutBounce(float start, float end, float value)
        {
            value /= 1f;
            end -= start;
            if (value < (1 / 2.75f))
            {
                return end * (7.5625f * value * value) + start;
            }
            else if (value < (2 / 2.75f))
            {
                value -= (1.5f / 2.75f);
                return end * (7.5625f * (value) * value + .75f) + start;
            }
            else if (value < (2.5 / 2.75))
            {
                value -= (2.25f / 2.75f);
                return end * (7.5625f * (value) * value + .9375f) + start;
            }
            else
            {
                value -= (2.625f / 2.75f);
                return end * (7.5625f * (value) * value + .984375f) + start;
            }
        }

        /// <summary>
        /// 反弹缓入缓出
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float EaseInOutBounce(float start, float end, float value)
        {
            end -= start;
            float d = 1f;
            if (value < d / 2) return EaseInBounce(0, end, value * 2) * 0.5f + start;
            else return EaseOutBounce(0, end, value * 2 - d) * 0.5f + end * 0.5f + start;
        }

        /// <summary>
        /// 回归缓入
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float EaseInBack(float start, float end, float value)
        {
            end -= start;
            value /= 1;
            float s = 1.70158f;
            return end * (value) * value * ((s + 1) * value - s) + start;
        }

        /// <summary>
        /// 回归缓出
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float EaseOutBack(float start, float end, float value)
        {
            float s = 1.70158f;
            end -= start;
            value = (value / 1) - 1;
            return end * ((value) * value * ((s + 1) * value + s) + 1) + start;
        }

        /// <summary>
        /// 回归缓入缓出
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float EaseInOutBack(float start, float end, float value)
        {
            float s = 1.70158f;
            end -= start;
            value /= .5f;
            if ((value) < 1)
            {
                s *= (1.525f);
                return end / 2 * (value * value * (((s) + 1) * value - s)) + start;
            }
            value -= 2;
            s *= (1.525f);
            return end / 2 * ((value) * value * (((s) + 1) * value + s) + 2) + start;
        }

        /// <summary>
        /// 弹性缓入
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float EaseInElastic(float start, float end, float value)
        {
            end -= start;

            float d = 1f;
            float p = d * .3f;
            float s = 0;
            float a = 0;

            if (value == 0) return start;

            if ((value /= d) == 1) return start + end;

            if (a == 0f || a < Mathf.Abs(end))
            {
                a = end;
                s = p / 4;
            }
            else
            {
                s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
            }

            return -(a * Mathf.Pow(2, 10 * (value -= 1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p)) + start;
        }

        /// <summary>
        /// 弹性缓出
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float EaseOutElastic(float start, float end, float value)
        {
            /* GFX47 MOD END */
            //Thank you to rafael.marteleto for fixing this as a port over from Pedro's UnityTween
            end -= start;

            float d = 1f;
            float p = d * .3f;
            float s = 0;
            float a = 0;

            if (value == 0) return start;

            if ((value /= d) == 1) return start + end;

            if (a == 0f || a < Mathf.Abs(end))
            {
                a = end;
                s = p / 4;
            }
            else
            {
                s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
            }

            return (a * Mathf.Pow(2, -10 * value) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p) + end + start);
        }

        /// <summary>
        /// 弹性缓入缓出
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float EaseInOutElastic(float start, float end, float value)
        {
            end -= start;

            float d = 1f;
            float p = d * .3f;
            float s = 0;
            float a = 0;

            if (value == 0) return start;

            if ((value /= d / 2) == 2) return start + end;

            if (a == 0f || a < Mathf.Abs(end))
            {
                a = end;
                s = p / 4;
            }
            else
            {
                s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
            }

            if (value < 1) return -0.5f * (a * Mathf.Pow(2, 10 * (value -= 1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p)) + start;
            return a * Mathf.Pow(2, -10 * (value -= 1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p) * 0.5f + end + start;
        }

        /// <summary>
        /// 重击
        /// </summary>
        /// <param name="amplitude"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float Punch(float amplitude, float value)
        {
            float s = 9;
            if (value == 0)
            {
                return 0;
            }
            if (value == 1)
            {
                return 0;
            }
            float period = 1 * 0.3f;
            s = period / (2 * Mathf.PI) * Mathf.Asin(0);
            return (amplitude * Mathf.Pow(2, -10 * value) * Mathf.Sin((value * 1 - s) * (2 * Mathf.PI) / period));
        }

        /// <summary>
        /// 通过缓动曲线值；即通过缓动曲线的X值获取对应的Y值；
        /// </summary>
        /// <param name="easeType"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float EaseCurveValue(EEaseType easeType, float start, float end, float value)
        {
            switch (easeType)
            {
                case EEaseType.Linear: return Linear(start, end, value);
                case EEaseType.Spring: return Spring(start, end, value);

                case EEaseType.EaseInQuad: return EaseInQuad(start, end, value);
                case EEaseType.EaseOutQuad: return EaseOutQuad(start, end, value);
                case EEaseType.EaseInOutQuad: return EaseInOutQuad(start, end, value);

                case EEaseType.EaseInCubic: return EaseInCubic(start, end, value);
                case EEaseType.EaseOutCubic: return EaseOutCubic(start, end, value);
                case EEaseType.EaseInOutCubic: return EaseInOutCubic(start, end, value);

                case EEaseType.EaseInQuart: return EaseInQuart(start, end, value);
                case EEaseType.EaseOutQuart: return EaseOutQuart(start, end, value);
                case EEaseType.EaseInOutQuart: return EaseInOutQuart(start, end, value);

                case EEaseType.EaseInQuint: return EaseInQuint(start, end, value);
                case EEaseType.EaseOutQuint: return EaseOutQuint(start, end, value);
                case EEaseType.EaseInOutQuint: return EaseInOutQuint(start, end, value);

                case EEaseType.EaseInSine: return EaseInSine(start, end, value);
                case EEaseType.EaseOutSine: return EaseOutSine(start, end, value);
                case EEaseType.EaseInOutSine: return EaseInOutSine(start, end, value);

                case EEaseType.EaseInExpo: return EaseInExpo(start, end, value);
                case EEaseType.EaseOutExpo: return EaseOutExpo(start, end, value);
                case EEaseType.EaseInOutExpo: return EaseInOutExpo(start, end, value);

                case EEaseType.EaseInCirc: return EaseInCirc(start, end, value);
                case EEaseType.EaseOutCirc: return EaseOutCirc(start, end, value);
                case EEaseType.EaseInOutCirc: return EaseInOutCirc(start, end, value);

                case EEaseType.EaseInBounce: return EaseInBounce(start, end, value);
                case EEaseType.EaseOutBounce: return EaseOutBounce(start, end, value);
                case EEaseType.EaseInOutBounce: return EaseInOutBounce(start, end, value);

                case EEaseType.EaseInBack: return EaseInBack(start, end, value);
                case EEaseType.EaseOutBack: return EaseOutBack(start, end, value);
                case EEaseType.EaseInOutBack: return EaseInOutBack(start, end, value);

                case EEaseType.EaseInElastic: return EaseInElastic(start, end, value);
                case EEaseType.EaseOutElastic: return EaseOutElastic(start, end, value);
                case EEaseType.EaseInOutElastic: return EaseInOutElastic(start, end, value);

                case EEaseType.Punch: return Punch(end - start, value);
                default: return Linear(start, end, value);
            }
        }
    }
}
