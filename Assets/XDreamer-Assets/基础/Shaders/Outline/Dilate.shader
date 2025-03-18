Shader "XDreamer/Outline/Dilate" {
	Properties
	{
		[HideInInspector] _MainTex("Texture", 2D) = "white" {}		
	}
	CGINCLUDE	
	#include "UnityCG.cginc"
	
	struct v2f {
		float4 pos : POSITION;
		float2 uv : TEXCOORD0;
		float4 uv01 : TEXCOORD1;
		float4 uv23 : TEXCOORD2;		
	};
	
	float4 _OutlineOffsets;		
	sampler2D _MainTex;
	
	v2f vert (appdata_img v) {
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv.xy = v.texcoord.xy;
		o.uv01 =  v.texcoord.xyxy + _OutlineOffsets.xyxy * float4(1,1, -1,-1);
		o.uv23 =  v.texcoord.xyxy + _OutlineOffsets.xyxy * float4(1,1, -1,-1) * 2.0;
		return o;
	}
	
	half4 frag (v2f i) : COLOR {
		half4 color = float4 (0,0,0,0);

		half4 c1 = tex2D (_MainTex, i.uv);
		half4 c2 = tex2D (_MainTex, i.uv01.xy);
		half4 c3 = tex2D (_MainTex, i.uv01.zw);
		half4 c4 = tex2D (_MainTex, i.uv23.xy);
		half4 c5 = tex2D (_MainTex, i.uv23.zw);
		c1 = max(c1, c2);
		c3 = max(c3, c4);
		c5 = max(c1, c5);
		return max(c3, c5);
	}

	ENDCG
	Subshader {
		Pass {
		ZTest Always Cull Off ZWrite Off
		CGPROGRAM
		#pragma fragmentoption ARB_precision_hint_fastest
		#pragma vertex vert
		#pragma fragment frag
		ENDCG
		}
	}
} 