using UnityEngine;
using System.Collections;

namespace XCSJ.CommonUtils.PluginHighlightingSystem.Internal
{
	// Shader property ID cached constants
	[UnityEngine.Internal.ExcludeFromDocs]
	public class ShaderPropertyID
	{
		// Common
		static public readonly int _MainTex = Shader.PropertyToID("_MainTex");
		static public readonly int _Cutoff = Shader.PropertyToID("_Cutoff");

		static public readonly int _HighlightingIntensity = Shader.PropertyToID("_HighlightingIntensity");
		static public readonly int _HighlightingCull = Shader.PropertyToID("_HighlightingCull");
		static public readonly int _HighlightingColor = Shader.PropertyToID("_HighlightingColor");
		static public readonly int _HighlightingBlurOffset = Shader.PropertyToID("_HighlightingBlurOffset");
		static public readonly int _HighlightingFillAlpha = Shader.PropertyToID("_HighlightingFillAlpha");

		static public readonly int _HighlightingBuffer = Shader.PropertyToID("_HighlightingBuffer");
		static public readonly int _HighlightingBlur1 = Shader.PropertyToID("_HighlightingBlur1");
		static public readonly int _HighlightingBlur2 = Shader.PropertyToID("_HighlightingBlur2");
	}
}