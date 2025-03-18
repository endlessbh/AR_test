Shader "XDreamer/Highlighted/Transparent"
{
	Properties
	{
		[HideInInspector] _MainTex ("", 2D) = "" {}
		[HideInInspector] _HighlightingColor ("", Color) = (1, 1, 1, 1)
		[HideInInspector] _Cutoff ("", Float) = 0.5
		[HideInInspector] _HighlightingCull ("", Int) = 2		// UnityEngine.Rendering.CullMode.Back
	}
	
	SubShader
	{
		Lighting Off
		Fog { Mode Off }
		ZWrite Off		// Manual depth test
		ZTest Always	// Manual depth test
		Cull [_HighlightingCull]	// For rendering both sides of the Sprite

		Pass
		{
			Stencil
			{
				Ref 1
				Comp Always
				Pass Replace
				ZFail Keep
			}
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma target 2.0
			#pragma multi_compile __ HIGHLIGHTING_OVERLAY
			#include "UnityCG.cginc"

			uniform fixed4 _HighlightingColor;
			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			uniform fixed _Cutoff;

			#ifndef HIGHLIGHTING_OVERLAY
			UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);
			#endif

			struct vs_input
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				fixed4 color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct ps_input
			{
				float4 pos : SV_POSITION;
				float2 texcoord : TEXCOORD0;
				fixed alpha : TEXCOORD1;

				#ifndef HIGHLIGHTING_OVERLAY
				float4 screen : TEXCOORD2;
				#endif
			};

			ps_input vert(vs_input v)
			{
				ps_input o;

				UNITY_SETUP_INSTANCE_ID(v);
				o.pos = UnityObjectToClipPos(v.vertex);

				#ifndef HIGHLIGHTING_OVERLAY
				o.screen = ComputeScreenPos(o.pos);
				COMPUTE_EYEDEPTH(o.screen.z);
				#endif

				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.alpha = v.color.a;
				return o;
			}

			fixed4 frag(ps_input i) : SV_Target
			{
				fixed a = tex2D(_MainTex, i.texcoord).a;
				clip(a - _Cutoff);

				#ifndef HIGHLIGHTING_OVERLAY
				float z = SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.screen));
				float perspZ = LinearEyeDepth(z);	// LinearEyeDepth automatically handles UNITY_REVERSED_Z case
				#if defined(UNITY_REVERSED_Z)
				z = 1 - z;
				#endif
				float orthoZ = _ProjectionParams.y + z * (_ProjectionParams.z - _ProjectionParams.y);	// near + z * (far - near)
				float sceneZ = lerp(perspZ, orthoZ, unity_OrthoParams.w);
				clip(sceneZ - i.screen.z + 0.01);
				#endif

				fixed4 c = _HighlightingColor;
				c.a *= i.alpha;

				return c;
			}
			ENDCG
		}
	}
	
	Fallback Off
}