Shader "XDreamer/Control" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
	}
	SubShader {  	
			
			Tags { "Queue" = "Overlay" }
			LOD 100
			ZWrite Off
			Material {  
				    Diffuse [_Color]    
				    Emission [_Emission]  
			}
			
			Pass {
				Cull Front
				Tags { "Queue" = "Transparent" }
				Blend SrcAlpha OneMinusSrcAlpha
				SetTexture [_MainTex]  {
					ConstantColor [_Color]
					Combine Texture * constant
				}
			}

			Pass {
				Cull Back
				Tags { "Queue" = "Transparent" }
				Blend SrcAlpha OneMinusSrcAlpha
				SetTexture [_MainTex]  {
					ConstantColor [_Color]
					Combine Texture * constant
				}
			}
			
        }
	FallBack "Diffuse"
}
