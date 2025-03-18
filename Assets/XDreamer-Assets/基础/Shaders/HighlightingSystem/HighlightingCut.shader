Shader "XDreamer/Highlighted/Cut"
{
	Properties
	{
		[HideInInspector] _MainTex ("", 2D) = "" {}
		[HideInInspector] _HighlightingFillAlpha ("", Range(0.0, 1.0)) = 1.0
	}

	SubShader
	{
		Lighting Off
		Fog { Mode off }
		ZWrite Off
		ZTest Always
		Cull Back

		Pass
		{
			Stencil
			{
				Ref 1
				Comp NotEqual
				Pass Keep
				ZFail Keep
			}

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma target 2.0
			
			#include "UnityCG.cginc"
			
			struct vs_input
			{
				float4 vertex : POSITION;
				half2 texcoord : TEXCOORD0;
			};
			
			struct ps_input
			{
				float4 pos : SV_POSITION;
				half2 uv : TEXCOORD0;
			};

			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			
			ps_input vert(vs_input v)
			{
				ps_input o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = UnityStereoScreenSpaceUVAdjust(v.texcoord, _MainTex_ST);
				return o;
			}
			
			fixed4 frag(ps_input i) : SV_Target
			{
				return tex2D(_MainTex, i.uv);
			}
			ENDCG
		}

		Pass
		{
			Stencil
			{
				Ref 1
				Comp Equal
				Pass Keep
				ZFail Keep
			}
			ColorMask A
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma target 2.0

			#include "UnityCG.cginc"
			
			struct vs_input
			{
				float4 vertex : POSITION;
			};
			
			struct ps_input
			{
				float4 pos : SV_POSITION;
			};

			uniform float _HighlightingFillAlpha;

			ps_input vert(vs_input v)
			{
				ps_input o;
				o.pos = UnityObjectToClipPos(v.vertex);
				return o;
			}
			
			fixed4 frag() : SV_Target
			{
				return fixed4(0, 0, 0, _HighlightingFillAlpha);
			}
			ENDCG
		}
	}
	FallBack Off
}
