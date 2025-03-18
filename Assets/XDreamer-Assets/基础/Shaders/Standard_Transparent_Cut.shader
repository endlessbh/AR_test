// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "XDreamer/Standard Transparent with Cut"
{
    /*Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _MainTex("Albedo", 2D) = "white" {}

        _Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5

        _Glossiness("Smoothness", Range(0.0, 1.0)) = 0.5
        _GlossMapScale("Smoothness Scale", Range(0.0, 1.0)) = 1.0
        [Enum(Metallic Alpha,0,Albedo Alpha,1)] _SmoothnessTextureChannel ("Smoothness texture channel", Float) = 0

        [Gamma] _Metallic("Metallic", Range(0.0, 1.0)) = 0.0
        _MetallicGlossMap("Metallic", 2D) = "white" {}

        [ToggleOff] _SpecularHighlights("Specular Highlights", Float) = 1.0
        [ToggleOff] _GlossyReflections("Glossy Reflections", Float) = 1.0

        _BumpScale("Scale", Float) = 1.0
        _BumpMap("Normal Map", 2D) = "bump" {}

        _Parallax ("Height Scale", Range (0.005, 0.08)) = 0.02
        _ParallaxMap ("Height Map", 2D) = "black" {}

        _OcclusionStrength("Strength", Range(0.0, 1.0)) = 1.0
        _OcclusionMap("Occlusion", 2D) = "white" {}

        _EmissionColor("Color", Color) = (0,0,0)
        _EmissionMap("Emission", 2D) = "white" {}

        _DetailMask("Detail Mask", 2D) = "white" {}

        _DetailAlbedoMap("Detail Albedo x2", 2D) = "grey" {}
        _DetailNormalMapScale("Scale", Float) = 1.0
        _DetailNormalMap("Normal Map", 2D) = "bump" {}

        [Enum(UV0,0,UV1,1)] _UVSec ("UV Set for secondary textures", Float) = 0


        // Blending state
        [HideInInspector] _Mode ("__mode", Float) = 0.0
        [HideInInspector] _SrcBlend ("__src", Float) = 1.0
        [HideInInspector] _DstBlend ("__dst", Float) = 0.0
        [HideInInspector] _ZWrite ("__zw", Float) = 1.0
    }

    CGINCLUDE
        #define UNITY_SETUP_BRDF_INPUT MetallicSetup
    ENDCG*/

	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
	_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
	}



	SubShader
    {
			Tags{ "RenderType" = "Transparent" "Queue" = "Transparent" }
			Blend SrcAlpha OneMinusSrcAlpha
			Cull Off

			Pass { ColorMask 0 }

			CGPROGRAM
			// Physically based Standard lighting model
#pragma surface surf Standard alpha:fade

			// Use shader model 3.0 target, to get nicer looking lighting
#pragma target 3.0

		uniform float4 _planeEquation;
		uniform int _cuttingOn;
		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
			float3 worldPos;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		fixed4 _ColorRed;

		

		void surf(Input IN, inout SurfaceOutputStandard o) {

			fixed4 red = fixed4(1, 0, 0, 1);

			if (_cuttingOn)
				clip(dot(IN.worldPos, _planeEquation.xyz) - _planeEquation.w);
			// Albedo comes from a texture tinted by color

			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;

			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;

			if (_cuttingOn) {
				//clip(dot(IN.worldPos, _planeEquation.xyz) - _planeEquation.w);
				if (dot(IN.worldPos, _planeEquation.xyz) - _planeEquation.w < 0.004) {
					o.Albedo = red.rgb;
					o.Emission = red.rgb;
				}
			}

		}
		ENDCG


    //FallBack "VertexLit"
    //CustomEditor "StandardShaderGUI"


		//Clip all shadows from object 
		Pass{
		Name "Caster"
		Tags{ "LightMode" = "ShadowCaster" }
		Offset 1, 1

		Fog{ Mode Off }
		ZWrite On ZTest LEqual Cull Off

		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma multi_compile_shadowcaster
#include "UnityCG.cginc"

		struct v2f {
		V2F_SHADOW_CASTER;
		float3 wpos : TEXCOORD1;
		float2  uv : TEXCOORD2;
	};

	struct Input {
		float3 worldPos;
	};

	uniform float4 _MainTex_ST;
	uniform float4 _planeEquation;
	uniform int _cuttingOn;
	float yBound;
	float xBound;
	v2f vert(appdata_base v)
	{
		v2f o;
		TRANSFER_SHADOW_CASTER(o)
			o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
		o.pos = UnityObjectToClipPos(v.vertex);
		float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
		o.wpos = worldPos;
		return o;
	}

	uniform sampler2D _MainTex;
	uniform fixed _Cutoff;
	uniform fixed4 _Color;

	float4 frag(v2f i) : SV_TARGET
	{
		if (_cuttingOn) {
			clip(dot(i.wpos, _planeEquation.xyz) - _planeEquation.w);
		}

	SHADOW_CASTER_FRAGMENT(i)
	}
		ENDCG

	}
	}
	FallBack "Diffuse"
}
