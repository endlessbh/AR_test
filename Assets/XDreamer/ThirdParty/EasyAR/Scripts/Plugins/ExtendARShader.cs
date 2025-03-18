using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
#if XDREAMER_EASYAR_3_0_1
using easyar;
#endif

namespace XCSJ.PluginEasyAR
{
    /// <summary>
    /// 扩展AR着色器
    /// </summary>
    [Name("扩展AR着色器")]
#if XDREAMER_EASYAR_3_0_1
    public class ExtendARShader : ARShader
#else
    public class ExtendARShader : BaseEasyARMB
#endif
    {
#if !XDREAMER_EASYAR_3_0_1
        public Shader BGR = null;
        public Shader GRAY = null;
        public Shader YUV_I420_YV12 = null;
        public Shader YUV_NV12 = null;
        public Shader YUV_NV21 = null;
#endif
    }
}
