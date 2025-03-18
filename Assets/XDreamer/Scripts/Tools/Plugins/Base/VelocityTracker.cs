using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;

namespace XCSJ.PluginTools.Base
{
    /// <summary>
    /// 速度追踪器：用于追踪一段时间内，抓取器的速度，并实现扔速度的计算
    /// </summary>
    [Serializable]
    public class VelocityTracker
    {
        /// <summary>
        /// 追踪时间长度（单位为秒）
        /// </summary>
        [Name("追踪时间长度")]
        public float _trackLength = 1F;

        /// <summary>
        /// 追踪延迟时间
        /// </summary>
        [Name("追踪延迟时间")]
        public float _trackDelay = 0.02F;

        /// <summary>
        /// 速度倍数曲线：最终速度=计算速度*当前曲线
        /// </summary>
        [Name("速度倍数曲线")] 
        public AnimationCurve _velocityMultiplierCurve = new AnimationCurve( new Keyframe(0.0F, 1.0F, 0, 0), new Keyframe(3.0F, 1.5F, 0, 0));

        /// <summary>
        /// 乘积系数
        /// </summary>
        [Name("乘积系数")]
        public float _coefficient = 1f;

        private struct VelocitySample
        {
            public float fixedTime;
            public Vector3 position;
            public Quaternion rotation;

            public VelocitySample(Vector3 position, Quaternion rotation, float time)
            {
                this.position = position;
                this.rotation = rotation;
                this.fixedTime = Time.fixedTime;
            }

            public static VelocitySample Interpolate(VelocitySample a, VelocitySample b, float time)
            {
                float alpha = Mathf.Clamp01(Mathf.InverseLerp(a.fixedTime, b.fixedTime, time));

                return new VelocitySample(Vector3.Lerp(a.position, b.position, alpha),
                                          Quaternion.Slerp(a.rotation, b.rotation, alpha),
                                          time);
            }
        }

        private Queue<VelocitySample> _velocityQueue = new Queue<VelocitySample>(64);

        /// <summary>
        /// 记录对象速度
        /// </summary>
        public void Record(Vector3 position, Quaternion rotation)
        {
            _velocityQueue.Enqueue(new VelocitySample(position, rotation, Time.fixedTime));

            while (true)
            {
                VelocitySample vs = _velocityQueue.Peek();

                // 移除超时数据
                var overtime = vs.fixedTime + _trackLength + _trackDelay;
                if (Time.fixedTime > overtime)
                {
                    _velocityQueue.Dequeue();
                }
                else
                {
                    break;
                }
            }
        }

        /// <summary>
        /// 获取速度量
        /// </summary>
        public bool TryGetVelocity(out Vector3 velocity, out Vector3 angularVelocity)
        {
            if (_velocityQueue.Count < 2)
            {
                Debug.Log("无效数据");
                velocity = Vector3.zero;
                angularVelocity = Vector3.zero;
                return false;
            }

            float trackEnd = Time.fixedTime - _trackDelay;
            float trackStart = trackEnd - _trackLength;

            VelocitySample start0, start1;
            VelocitySample end0, end1;
            VelocitySample s0, s1;

            s0 = s1 = start0 = start1 = end0 = end1 = _velocityQueue.Dequeue();

            while (_velocityQueue.Count != 0)
            {
                s0 = s1;
                s1 = _velocityQueue.Dequeue();

                if (s0.fixedTime < trackStart && s1.fixedTime >= trackStart)
                {
                    start0 = s0;
                    start1 = s1;
                }

                if (s0.fixedTime < trackEnd && s1.fixedTime >= trackEnd)
                {
                    end0 = s0;
                    end1 = s1;

                    _velocityQueue.Clear();
                    break;
                }
            }

            VelocitySample start = VelocitySample.Interpolate(start0, start1, trackStart);
            VelocitySample end = VelocitySample.Interpolate(end0, end1, trackEnd);

            velocity = (end.position - start.position) / _trackLength;
            angularVelocity = (end.position - start.position) / _trackLength;

            velocity *= _velocityMultiplierCurve.Evaluate(velocity.magnitude) * _coefficient;

            return true;
        }
    }
}
