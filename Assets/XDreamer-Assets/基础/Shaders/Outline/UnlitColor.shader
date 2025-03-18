Shader "XDreamer/Outline/UnlitColor"
{
    Properties{
        [Enum(UnityEngine.Rendering.CompareFunction)] _ZTest("Z test", Int) = 8
    }
    SubShader
    {
        Pass
        {
            ZTest[_ZTest]
            ZWrite Off
            Fog { Mode off }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma fragmentoption ARB_precision_hint_fastest
            #include "UnityCG.cginc"                

            float4 vert(float4 v : POSITION) : SV_Position
            {
                return UnityObjectToClipPos(v);
            }            
            fixed4 frag(float4 i:SV_Position) : SV_Target
            {
                return 1;
            }
            ENDCG
        }
    }
}
