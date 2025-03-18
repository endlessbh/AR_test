Shader "XDreamer/Highlighted/Composite"
{
	Properties
	{
		[HideInInspector] _MainTex ("", 2D) = "" {}
		[HideInInspector] _HighlightingBuffer ("", 2D) = "" {}
	}
	
	SubShader
	{
		Pass
		{
			Lighting Off
			Fog { Mode off }
			ZWrite Off
			ZTest Always
			Cull Off
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma target 2.0
			#include "UnityCG.cginc"

			struct vs_input
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct ps_input
			{
				float4 pos : SV_POSITION;
				half2 uv0 : TEXCOORD0;
				half2 uv1 : TEXCOORD1;
			};

			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			uniform float4 _MainTex_TexelSize;
			uniform sampler2D _HighlightingBuffer;

			ps_input vert(vs_input v)
			{
				ps_input o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv0 = UnityStereoScreenSpaceUVAdjust(v.texcoord, _MainTex_ST);
				o.uv1 = o.uv0;

				#if UNITY_UV_STARTS_AT_TOP
				if (_MainTex_TexelSize.y < 0)
				{
					o.uv1.y = 1-o.uv1.y;
				}
				#endif

				return o;
			}

			fixed4 frag(ps_input i) : SV_Target
			{
				fixed4 c1 = tex2D(_MainTex, i.uv0);
				fixed4 c2 = tex2D(_HighlightingBuffer, i.uv1);
				c1.rgb = lerp(c1.rgb, c2.rgb, c2.a);
				return c1;
			}
			ENDCG
		}
	}
	FallBack Off
}