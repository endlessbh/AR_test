using System;
using UnityEngine;
using UnityEngine.Rendering;
using XCSJ.Extension.Base.Dataflows.DataBinders;

namespace XCSJ.Extension.Base.Dataflows.Binders.XUnityEngine.XRenderSettings
{
    /// <summary>
    /// 渲染设置绑定器
    /// </summary>
    [DataBinder(typeof(RenderSettings))]
    public class RenderSettings_Binder : DataBinder<RenderSettings>
    {
        /// <summary>
        /// 成员名
        /// </summary>
        public override string memberName { get => ""; set { } }

        /// <summary>
        /// 尝试获取成员值
        /// </summary>
        /// <param name="type"></param>
        /// <param name="obj"></param>
        /// <param name="memberName"></param>
        /// <param name="value"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public override bool TryGetMemberValue(Type type, object obj, string memberName, out object value, object[] index = null)
        {
            switch (memberName)
            {
                case nameof(RenderSettings.skybox):
                    {
                        value = RenderSettings.skybox;
                        return true;
                    }
                case nameof(RenderSettings.haloStrength):
                    {
                        value = RenderSettings.haloStrength;
                        return true;
                    }
                case nameof(RenderSettings.defaultReflectionResolution):
                    {
                        value = RenderSettings.defaultReflectionResolution;
                        return true;
                    }
                case nameof(RenderSettings.defaultReflectionMode):
                    {
                        value = RenderSettings.defaultReflectionMode;
                        return true;
                    }
                case nameof(RenderSettings.reflectionBounces):
                    {
                        value = RenderSettings.reflectionBounces;
                        return true;
                    }
                case nameof(RenderSettings.reflectionIntensity):
                    {
                        value = RenderSettings.reflectionIntensity;
                        return true;
                    }
                case nameof(RenderSettings.customReflection):
                    {
                        value = RenderSettings.customReflection;
                        return true;
                    }
                case nameof(RenderSettings.sun):
                    {
                        value = RenderSettings.sun;
                        return true;
                    }
                case nameof(RenderSettings.subtractiveShadowColor):
                    {
                        value = RenderSettings.subtractiveShadowColor;
                        return true;
                    }
                case nameof(RenderSettings.flareStrength):
                    {
                        value = RenderSettings.flareStrength;
                        return true;
                    }
                case nameof(RenderSettings.ambientLight):
                    {
                        value = RenderSettings.ambientLight;
                        return true;
                    }
                case nameof(RenderSettings.ambientGroundColor):
                    {
                        value = RenderSettings.ambientGroundColor;
                        return true;
                    }
                case nameof(RenderSettings.ambientEquatorColor):
                    {
                        value = RenderSettings.ambientEquatorColor;
                        return true;
                    }
                case nameof(RenderSettings.ambientSkyColor):
                    {
                        value = RenderSettings.ambientSkyColor;
                        return true;
                    }
                case nameof(RenderSettings.ambientMode):
                    {
                        value = RenderSettings.ambientMode;
                        return true;
                    }
                case nameof(RenderSettings.fogDensity):
                    {
                        value = RenderSettings.fogDensity;
                        return true;
                    }
                case nameof(RenderSettings.fogColor):
                    {
                        value = RenderSettings.fogColor;
                        return true;
                    }
                case nameof(RenderSettings.fogMode):
                    {
                        value = RenderSettings.fogMode;
                        return true;
                    }
                case nameof(RenderSettings.fogEndDistance):
                    {
                        value = RenderSettings.fogEndDistance;
                        return true;
                    }
                case nameof(RenderSettings.fogStartDistance):
                    {
                        value = RenderSettings.fogStartDistance;
                        return true;
                    }
                case nameof(RenderSettings.fog):
                    {
                        value = RenderSettings.fog;
                        return true;
                    }
                case nameof(RenderSettings.ambientIntensity):
                    {
                        value = RenderSettings.ambientIntensity;
                        return true;
                    }
                case nameof(RenderSettings.flareFadeSpeed):
                    {
                        value = RenderSettings.flareFadeSpeed;
                        return true;
                    }
            }
            value = default;
            return false;
        }

        /// <summary>
        /// 尝试设置成员值
        /// </summary>
        /// <param name="type"></param>
        /// <param name="obj"></param>
        /// <param name="memberName"></param>
        /// <param name="value"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public override bool TrySetMemberValue(Type type, object obj, string memberName, object value, object[] index = null)
        {
            switch (memberName)
            {
                case nameof(RenderSettings.skybox):
                    {
                        if (TryConvertTo(value, out Material outValue))
                        {
                            RenderSettings.skybox = outValue;
                            return true;
                        }
                        break;
                    }
                case nameof(RenderSettings.haloStrength):
                    {
                        if (TryConvertTo(value, out float outValue))
                        {
                            RenderSettings.haloStrength = outValue;
                            return true;
                        }
                        break;
                    }
                case nameof(RenderSettings.defaultReflectionResolution):
                    {
                        if (TryConvertTo(value, out int outValue))
                        {
                            RenderSettings.defaultReflectionResolution = outValue;
                            return true;
                        }
                        break;
                    }
                case nameof(RenderSettings.defaultReflectionMode):
                    {
                        if (TryConvertTo(value, out DefaultReflectionMode outValue))
                        {
                            RenderSettings.defaultReflectionMode = outValue;
                            return true;
                        }
                        break;
                    }
                case nameof(RenderSettings.reflectionBounces):
                    {
                        if (TryConvertTo(value, out int outValue))
                        {
                            RenderSettings.reflectionBounces = outValue;
                            return true;
                        }
                        break;
                    }
                case nameof(RenderSettings.reflectionIntensity):
                    {
                        if (TryConvertTo(value, out float outValue))
                        {
                            RenderSettings.reflectionIntensity = outValue;
                            return true;
                        }
                        break;
                    }
                case nameof(RenderSettings.customReflection):
                    {
                        if (TryConvertTo(value, out Cubemap outValue))
                        {
                            RenderSettings.customReflection = outValue;
                            return true;
                        }
                        break;
                    }
                case nameof(RenderSettings.sun):
                    {
                        if (TryConvertTo(value, out Light outValue))
                        {
                            RenderSettings.sun = outValue;
                            return true;
                        }
                        break;
                    }
                case nameof(RenderSettings.subtractiveShadowColor):
                    {
                        if (TryConvertTo(value, out Color outValue))
                        {
                            RenderSettings.subtractiveShadowColor = outValue;
                            return true;
                        }
                        break;
                    }
                case nameof(RenderSettings.flareStrength):
                    {
                        if (TryConvertTo(value, out float outValue))
                        {
                            RenderSettings.flareStrength = outValue;
                            return true;
                        }
                        break;
                    }
                case nameof(RenderSettings.ambientLight):
                    {
                        if (TryConvertTo(value, out Color outValue))
                        {
                            RenderSettings.ambientLight = outValue;
                            return true;
                        }
                        break;
                    }
                case nameof(RenderSettings.ambientGroundColor):
                    {
                        if (TryConvertTo(value, out Color outValue))
                        {
                            RenderSettings.ambientGroundColor = outValue;
                            return true;
                        }
                        break;
                    }
                case nameof(RenderSettings.ambientSkyColor):
                    {
                        if (TryConvertTo(value, out Color outValue))
                        {
                            RenderSettings.ambientSkyColor = outValue;
                            return true;
                        }
                        break;
                    }
                case nameof(RenderSettings.ambientMode):
                    {
                        if (TryConvertTo(value, out AmbientMode outValue))
                        {
                            RenderSettings.ambientMode = outValue;
                            return true;
                        }
                        break;
                    }
                case nameof(RenderSettings.fogDensity):
                    {
                        if (TryConvertTo(value, out float outValue))
                        {
                            RenderSettings.fogDensity = outValue;
                            return true;
                        }
                        break;
                    }
                case nameof(RenderSettings.fogColor):
                    {
                        if (TryConvertTo(value, out Color outValue))
                        {
                            RenderSettings.fogColor = outValue;
                            return true;
                        }
                        break;
                    }
                case nameof(RenderSettings.fogMode):
                    {
                        if (TryConvertTo(value, out FogMode outValue))
                        {
                            RenderSettings.fogMode = outValue;
                            return true;
                        }
                        break;
                    }
                case nameof(RenderSettings.fogEndDistance):
                    {
                        if (TryConvertTo(value, out float outValue))
                        {
                            RenderSettings.fogEndDistance = outValue;
                            return true;
                        }
                        break;
                    }
                case nameof(RenderSettings.fogStartDistance):
                    {
                        if (TryConvertTo(value, out float outValue))
                        {
                            RenderSettings.fogStartDistance = outValue;
                            return true;
                        }
                        break;
                    }
                case nameof(RenderSettings.fog):
                    {
                        if (TryConvertTo(value, out bool outValue))
                        {
                            RenderSettings.fog = outValue;
                            return true;
                        }
                        break;
                    }
                case nameof(RenderSettings.ambientIntensity):
                    {
                        if (TryConvertTo(value, out float outValue))
                        {
                            RenderSettings.ambientIntensity = outValue;
                            return true;
                        }
                        break;
                    }
                case nameof(RenderSettings.flareFadeSpeed):
                    {
                        if (TryConvertTo(value, out float outValue))
                        {
                            RenderSettings.flareFadeSpeed = outValue;
                            return true;
                        }
                        break;
                    }
            }
            return false;
        }
    }
}
