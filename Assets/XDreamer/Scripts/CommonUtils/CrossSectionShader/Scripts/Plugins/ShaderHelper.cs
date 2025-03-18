using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Collections;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginTools;

namespace XCSJ.CommonUtils.PluginCrossSectionShader
{
    public class ShaderHelper
    {
        public class GenericThreePlanesBSP
        {
            public const string DefaultShaderName = Product.Name + "/CrossSection/GenericThreePlanesBSP";

            public const string _Plane1Normal = nameof(_Plane1Normal);
            public const string _Plane1Position = nameof(_Plane1Position);

            public const string _Plane2Normal = nameof(_Plane2Normal);
            public const string _Plane2Position = nameof(_Plane2Position);

            public const string _Plane3Normal = nameof(_Plane3Normal);
            public const string _Plane3Position = nameof(_Plane3Position);

            // 检查材质是否有对应的Shader属性
            public static bool Valid(Material material)
            {
                if (!material) return false;

                return material.HasProperty(_Plane1Normal) && material.HasProperty(_Plane1Position)
                    && material.HasProperty(_Plane2Normal) && material.HasProperty(_Plane2Position)
                    && material.HasProperty(_Plane3Normal) && material.HasProperty(_Plane3Position);
            }
        }
    }
}
