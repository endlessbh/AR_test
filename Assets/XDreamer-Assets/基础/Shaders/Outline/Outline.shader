Shader "XDreamer/Outline"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {} 
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            sampler2D _ObjectsTex;                   
            sampler2D _BlurredTex; 
            float4 _OutlineColor;
            float _Softness;
            float _Size;
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 screen = tex2D(_MainTex, i.uv);
                fixed4 objblur = tex2D(_BlurredTex, i.uv);
                fixed4 objects = tex2D(_ObjectsTex, i.uv);                 
                fixed4 outline = saturate(objblur - objects);
                outline = outline.rgbr;
                outline.rgb *= _OutlineColor.rgb;
                outline *= _OutlineColor.a;
                return screen* (1 - outline.a) + outline;
            }
            ENDCG
        }
    }
}
