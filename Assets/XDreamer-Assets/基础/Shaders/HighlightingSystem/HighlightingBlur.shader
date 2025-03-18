Shader "XDreamer/Highlighted/Blur"
{
	Properties
	{
		[HideInInspector] _MainTex ("", 2D) = "" {}
		[HideInInspector] _HighlightingIntensity ("", Range (0.25,0.5)) = 0.3
	}
	
	SubShader
	{
		Pass
		{
			ZTest Always
			Cull Off
			ZWrite Off
			Lighting Off
			Fog { Mode Off }
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma target 2.0
			#pragma multi_compile DIAGONAL_DIRECTIONS STRAIGHT_DIRECTIONS ALL_DIRECTIONS
			
			#include "UnityCG.cginc"
			
			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			uniform float4 _MainTex_TexelSize;
			
			uniform float _HighlightingBlurOffset;
			uniform half _HighlightingIntensity;

			struct vs_input
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct ps_input
			{
				float4 pos : SV_POSITION;

				#if defined(ALL_DIRECTIONS)
				float4 uv0 : TEXCOORD0;
				float4 uv1 : TEXCOORD1;
				float4 uv2 : TEXCOORD2;
				float4 uv3 : TEXCOORD3;
				#else
				float4 uv0 : TEXCOORD0;
				float4 uv1 : TEXCOORD1;
				#endif
			};
			
			ps_input vert(vs_input v)
			{
				ps_input o;
				o.pos = UnityObjectToClipPos(v.vertex);

				float2 uv = UnityStereoScreenSpaceUVAdjust(v.texcoord, _MainTex_ST);
				float2 offs = _HighlightingBlurOffset * _MainTex_TexelSize.xy;

				#if defined(ALL_DIRECTIONS)

				// Diagonal
				o.uv0.x = uv.x - offs.x;
				o.uv0.y = uv.y - offs.y;
				
				o.uv0.z = uv.x + offs.x;
				o.uv0.w = uv.y - offs.y;
				
				o.uv1.x = uv.x + offs.x;
				o.uv1.y = uv.y + offs.y;
				
				o.uv1.z = uv.x - offs.x;
				o.uv1.w = uv.y + offs.y;

				// Straight
				o.uv2.x = uv.x - offs.x;
				o.uv2.y = uv.y;
				
				o.uv2.z = uv.x + offs.x;
				o.uv2.w = uv.y;
				
				o.uv3.x = uv.x;
				o.uv3.y = uv.y - offs.y;
				
				o.uv3.z = uv.x;
				o.uv3.w = uv.y + offs.y;

				#elif defined(STRAIGHT_DIRECTIONS)

				// Straight
				o.uv0.x = uv.x - offs.x;
				o.uv0.y = uv.y;
				
				o.uv0.z = uv.x + offs.x;
				o.uv0.w = uv.y;
				
				o.uv1.x = uv.x;
				o.uv1.y = uv.y - offs.y;
				
				o.uv1.z = uv.x;
				o.uv1.w = uv.y + offs.y;

				#else 

				// Diagonal
				o.uv0.x = uv.x - offs.x;
				o.uv0.y = uv.y - offs.y;
				
				o.uv0.z = uv.x + offs.x;
				o.uv0.w = uv.y - offs.y;
				
				o.uv1.x = uv.x + offs.x;
				o.uv1.y = uv.y + offs.y;
				
				o.uv1.z = uv.x - offs.x;
				o.uv1.w = uv.y + offs.y;

				#endif

				return o;
			}
			
			half4 frag(ps_input i) : SV_Target
			{
				half4 color1 = tex2D(_MainTex, i.uv0.xy);
				fixed4 color2;

				// For straight or diagonal directions
				color2 = tex2D(_MainTex, i.uv0.zw);
				color1.rgb = max(color1.rgb, color2.rgb);
				color1.a += color2.a;

				color2 = tex2D(_MainTex, i.uv1.xy);
				color1.rgb = max(color1.rgb, color2.rgb);
				color1.a += color2.a;

				color2 = tex2D(_MainTex, i.uv1.zw);
				color1.rgb = max(color1.rgb, color2.rgb);
				color1.a += color2.a;

				// For all directions
				#if defined(ALL_DIRECTIONS)
				color2 = tex2D(_MainTex, i.uv2.xy);
				color1.rgb = max(color1.rgb, color2.rgb);
				color1.a += color2.a;

				color2 = tex2D(_MainTex, i.uv2.zw);
				color1.rgb = max(color1.rgb, color2.rgb);
				color1.a += color2.a;

				color2 = tex2D(_MainTex, i.uv3.xy);
				color1.rgb = max(color1.rgb, color2.rgb);
				color1.a += color2.a;

				color2 = tex2D(_MainTex, i.uv3.zw);
				color1.rgb = max(color1.rgb, color2.rgb);
				color1.a += color2.a;
				#endif
				
				color1.a *= _HighlightingIntensity;
				return color1;
			}
			ENDCG
		}
	}
	
	Fallback off
}