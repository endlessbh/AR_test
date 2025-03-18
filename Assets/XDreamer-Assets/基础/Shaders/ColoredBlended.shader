Shader "XDreamer/Colored Blended"
{
	SubShader
	{
		Pass {
	   BindChannels { Bind "color",color }
	   Blend SrcAlpha OneMinusSrcAlpha
	   ZWrite Off Cull Off Fog { Mode Off }
		}
	}
}