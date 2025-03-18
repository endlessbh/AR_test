using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using UnityEngine.XR;
using System.Collections;
using System.Collections.Generic;
using XCSJ.PluginCommonUtils;
using XCSJ.Attributes;
#if UNITY_5_5_OR_NEWER
using UnityEngine.Profiling;
#endif

namespace XCSJ.CommonUtils.PluginHighlightingSystem.Internal
{
	[Name("模糊方向")]
	public enum BlurDirections : int
	{
		Diagonal, 
		Straight, 
		All
	}

	[Name("抗锯齿")]
	public enum AntiAliasing : int
	{
		QualitySettings, 
		Disabled, 
		MSAA2x, 
		MSAA4x, 
		MSAA8x, 
	}

	[Name("高亮基础")]
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Camera))]
	public class HighlightingBase : BaseHighlighterMB
	{
		#region Static Fields and Constants
		static protected readonly Color colorClear = new Color(0f, 0f, 0f, 0f);
		static protected readonly string renderBufferName = "HighlightingSystem";
		static protected readonly Matrix4x4 identityMatrix = Matrix4x4.identity;
		static protected readonly string keywordStraightDirections = "STRAIGHT_DIRECTIONS";
		static protected readonly string keywordAllDirections = "ALL_DIRECTIONS";
		static protected readonly string profileHighlightingSystem = "HighlightingSystem";
		protected const CameraEvent queue = CameraEvent.BeforeImageEffectsOpaque;

		static protected Camera currentCamera;
		static protected HashSet<HighlighterRenderer> visibleRenderers = new HashSet<HighlighterRenderer>();
		#endregion
		
		#region Accessors
		// True if supported on this platform
		public bool isSupported
		{
			get
			{
				return CheckSupported(false);
			}
		}

		public float fillAlpha
		{
			get { return _fillAlpha; }
			set
			{
				value = Mathf.Clamp01(value);
				if (_fillAlpha != value)
				{
					if (Application.isPlaying)
					{
						cutMaterial.SetFloat(ShaderPropertyID._HighlightingFillAlpha, value);
					}

					_fillAlpha = value;
				}
			}
		}

		// Highlighting buffer size downsample factor
		public int downsampleFactor
		{
			get { return _downsampleFactor; }
			set
			{
				if (_downsampleFactor != value)
				{
					// Is power of two check
					if ((value != 0) && ((value & (value - 1)) == 0))
					{
						_downsampleFactor = value;
					}
					else
					{
						Debug.LogWarning("HighlightingSystem : Prevented attempt to set incorrect downsample factor value.");
					}
				}
			}
		}
		
		// Blur iterations
		public int iterations
		{
			get { return _iterations; }
			set
			{
				if (_iterations != value)
				{
					_iterations = value;
				}
			}
		}
		
		// Blur minimal spread
		public float blurMinSpread
		{
			get { return _blurMinSpread; }
			set
			{
				if (_blurMinSpread != value)
				{
					_blurMinSpread = value;
				}
			}
		}
		
		// Blur spread per iteration
		public float blurSpread
		{
			get { return _blurSpread; }
			set
			{
				if (_blurSpread != value)
				{
					_blurSpread = value;
				}
			}
		}
		
		// Blurring intensity for the blur material
		public float blurIntensity
		{
			get { return _blurIntensity; }
			set
			{
				if (_blurIntensity != value)
				{
					_blurIntensity = value;
					if (Application.isPlaying)
					{
						blurMaterial.SetFloat(ShaderPropertyID._HighlightingIntensity, _blurIntensity);
					}
				}
			}
		}

		public BlurDirections blurDirections
		{
			get { return _blurDirections; }
			set
			{
				if (_blurDirections != value)
				{
					_blurDirections = value;
					if (Application.isPlaying)
					{
						blurMaterial.SetKeyword(keywordStraightDirections, _blurDirections == BlurDirections.Straight);
						blurMaterial.SetKeyword(keywordAllDirections, _blurDirections == BlurDirections.All);
					}
				}
			}
		}

		// Blitter component reference (optional)
		public HighlightingBlitter blitter
		{
			get { return _blitter; }
			set
			{
				if (_blitter != value)
				{
					if (_blitter != null)
					{
						_blitter.Unregister(this);
					}
					_blitter = value;
					if (_blitter != null)
					{
						_blitter.Register(this);
					}
				}
			}
		}

		public AntiAliasing antiAliasing
		{
			get { return _antiAliasing; }
			set
			{
				if (_antiAliasing != value)
				{
					_antiAliasing = value;
				}
			}
		}
		#endregion
		
		#region Protected Fields
		protected CommandBuffer renderBuffer;

		protected RenderTextureDescriptor cachedDescriptor;

		[SerializeField]
		protected float _fillAlpha = 0f;

		[FormerlySerializedAs("downsampleFactor")]
		[SerializeField]
		protected int _downsampleFactor = 4;

		[FormerlySerializedAs("iterations")]
		[SerializeField]
		protected int _iterations = 2;

		[FormerlySerializedAs("blurMinSpread")]
		[SerializeField]
		protected float _blurMinSpread = 0.65f;

		[FormerlySerializedAs("blurSpread")]
		[SerializeField]
		protected float _blurSpread = 0.25f;

		[SerializeField]
		protected float _blurIntensity = 0.3f;

		[SerializeField]
		protected BlurDirections _blurDirections = BlurDirections.Diagonal;

		[SerializeField]
		protected HighlightingBlitter _blitter;

		[SerializeField]
		protected AntiAliasing _antiAliasing = AntiAliasing.QualitySettings;

		// RenderTargetidentifier for the highlightingBuffer RenderTexture
		protected RenderTargetIdentifier highlightingBufferID;
		protected RenderTargetIdentifier blur1ID;
		protected RenderTargetIdentifier blur2ID;

		// RenderTexture with highlighting buffer
		protected RenderTexture highlightingBuffer = null;

		// Camera reference
		protected Camera cam = null;

		// Material parameters
		protected const int BLUR = 0;
		protected const int CUT = 1;
		protected const int COMP = 2;
		static protected readonly string[] shaderPaths = new string[]
		{
			"XDreamer/Highlighted/Blur",
            "XDreamer/Highlighted/Cut",
            "XDreamer/Highlighted/Composite", 
		};
		static protected Shader[] shaders;
		static protected Material[] materials;

		// Dynamic materials
		protected Material blurMaterial;
		protected Material cutMaterial;
		protected Material compMaterial;

		static protected bool initialized = false;
		#endregion

		#region MonoBehaviour
		// 
		protected override void OnEnable()
		{
			base.OnEnable();
			Initialize();

			if (!CheckSupported(true))
			{
				enabled = false;
				Debug.LogError("HighlightingSystem : Highlighting System has been disabled due to unsupported Unity features on the current platform!");
				return;
			}

			blur1ID = new RenderTargetIdentifier(ShaderPropertyID._HighlightingBlur1);
			blur2ID = new RenderTargetIdentifier(ShaderPropertyID._HighlightingBlur2);

			blurMaterial = new Material(materials[BLUR]);
			cutMaterial = new Material(materials[CUT]);
			compMaterial = new Material(materials[COMP]);
			
			// Set initial material properties
			blurMaterial.SetKeyword(keywordStraightDirections, _blurDirections == BlurDirections.Straight);
			blurMaterial.SetKeyword(keywordAllDirections, _blurDirections == BlurDirections.All);
			blurMaterial.SetFloat(ShaderPropertyID._HighlightingIntensity, _blurIntensity);
			cutMaterial.SetFloat(ShaderPropertyID._HighlightingFillAlpha, _fillAlpha);

			renderBuffer = new CommandBuffer();
			renderBuffer.name = renderBufferName;

			cam = GetComponent<Camera>();

			cam.depthTextureMode |= DepthTextureMode.Depth;

			cam.AddCommandBuffer(queue, renderBuffer);

			if (_blitter != null)
			{
				_blitter.Register(this);
			}

			EndOfFrame.AddListener(OnEndOfFrame);
		}

		/// <summary>
		/// 禁用
		/// </summary>
		protected override void OnDisable()
		{
			base.OnDisable();
			if (renderBuffer != null)
			{
				cam.RemoveCommandBuffer(queue, renderBuffer);
				renderBuffer = null;
			}

			if (highlightingBuffer != null && highlightingBuffer.IsCreated())
			{
				highlightingBuffer.Release();
				highlightingBuffer = null;
			}

			if (_blitter != null)
			{
				_blitter.Unregister(this);
			}

			EndOfFrame.RemoveListener(OnEndOfFrame);
		}

		// 
		protected virtual void OnPreCull()
		{
			currentCamera = cam;
			visibleRenderers.Clear();
		}

		// 
		protected virtual void OnPreRender()
		{
			Profiler.BeginSample("HighlightingSystem.OnPreRender");

			var descriptor = GetDescriptor();

			if (highlightingBuffer == null || !Equals(cachedDescriptor, descriptor))
			{
				if (highlightingBuffer != null)
				{
					if (highlightingBuffer.IsCreated())
					{
						highlightingBuffer.Release();
					}
					highlightingBuffer = null;
				}

				cachedDescriptor = descriptor;

				highlightingBuffer = new RenderTexture(cachedDescriptor);
				highlightingBuffer.filterMode = FilterMode.Point;
				highlightingBuffer.wrapMode = TextureWrapMode.Clamp;

				if (!highlightingBuffer.Create())
				{
					Debug.LogError("HighlightingSystem : UpdateHighlightingBuffer() : Failed to create highlightingBuffer RenderTexture!");
				}
				
				highlightingBufferID = new RenderTargetIdentifier(highlightingBuffer);
				compMaterial.SetTexture(ShaderPropertyID._HighlightingBuffer, highlightingBuffer);
			}

			RebuildCommandBuffer();
			Profiler.EndSample();
		}

		// Do not remove! 
		// Having this method in this script is necessary to support multiple cameras with different clear flags even in case custom blitter is being used. 
		// Also, CommandBuffer is bound to CameraEvent.BeforeImageEffectsOpaque event, 
		// so Unity will spam 'depthSurface == NULL || rcolorZero->backBuffer == depthSurface->backBuffer' error even if MSAA is enabled
		protected virtual void OnRenderImage(RenderTexture src, RenderTexture dst)
		{
			Profiler.BeginSample("HighlightingSystem.OnRenderImage");
			if (blitter == null)
			{
				Blit(src, dst);
			}
			else
			{
				Graphics.Blit(src, dst);
			}
			Profiler.EndSample();
		}

		// 
		protected virtual void OnEndOfFrame()
		{
			currentCamera = null;
			visibleRenderers.Clear();
		}
		#endregion

		#region Internal
		// 
		[UnityEngine.Internal.ExcludeFromDocs]
		static public void SetVisible(HighlighterRenderer renderer)
		{
			// Another camera may intercept rendering and send it's own OnWillRenderObject events (i.e. water rendering). 
			// Also, VR Camera with Multi Pass Stereo Rendering Method renders twice per frame (once for each eye), 
			// but OnWillRenderObject is called once so we have to check. 
			if (Camera.current != currentCamera) { return; }

			// Add to the list of renderers visible for the current Highlighting renderer
			visibleRenderers.Add(renderer);
		}

		// 
		[UnityEngine.Internal.ExcludeFromDocs]
		static public bool GetVisible(HighlighterRenderer renderer)
		{
			return visibleRenderers.Contains(renderer);
		}

		// 
		static protected void Initialize()
		{
			if (initialized) { return; }

			// Initialize shaders and materials
			int l = shaderPaths.Length;
			shaders = new Shader[l];
			materials = new Material[l];
			for (int i = 0; i < l; i++)
			{
				Shader shader = Shader.Find(shaderPaths[i]);
				shaders[i] = shader;
				
				Material material = new Material(shader);
				materials[i] = material;
			}

			initialized = true;
		}

		// 
		protected virtual RenderTextureDescriptor GetDescriptor()
		{
			RenderTextureDescriptor descriptor;

			// RTT
			var targetTexture = cam.targetTexture;
			if (targetTexture != null)
			{
				descriptor = targetTexture.descriptor;
			}
			// VR
			else if (XRSettings.enabled)
			{
				descriptor = XRSettings.eyeTextureDesc;
			}
			// Normal
			else 
			{
				descriptor = new RenderTextureDescriptor(cam.pixelWidth, cam.pixelHeight, RenderTextureFormat.ARGB32, 24);
			}

			// Overrides
			descriptor.colorFormat = RenderTextureFormat.ARGB32;
			descriptor.sRGB = QualitySettings.activeColorSpace == ColorSpace.Linear;
			descriptor.useMipMap = false;
			descriptor.msaaSamples = GetAA(targetTexture);

			return descriptor;
		}

		// 
		protected virtual bool Equals(RenderTextureDescriptor x, RenderTextureDescriptor y)
		{
			return x.width == y.width && x.height == y.height && x.msaaSamples == y.msaaSamples;    // TODO compare all fields?
		}

		// 
		protected virtual int GetAA(RenderTexture targetTexture)
		{
			int aa = 1;
			switch (_antiAliasing)
			{
				case AntiAliasing.QualitySettings:
					// Set aa value to 1 in case camera is in DeferredLighting or DeferredShading Rendering Path
					if (cam.actualRenderingPath == RenderingPath.DeferredLighting || cam.actualRenderingPath == RenderingPath.DeferredShading)
					{
						aa = 1;
					}
					else
					{
						if (targetTexture == null)
						{
							aa = QualitySettings.antiAliasing;
							if (aa == 0) { aa = 1; }
						}
						else
						{
							aa = targetTexture.antiAliasing;
						}
					}
					break;
				case AntiAliasing.Disabled:
					aa = 1;
					break;
				case AntiAliasing.MSAA2x:
					aa = 2;
					break;
				case AntiAliasing.MSAA4x:
					aa = 4;
					break;
				case AntiAliasing.MSAA8x:
					aa = 8;
					break;
			}
			return aa;
		}

		// 
		protected virtual bool CheckSupported(bool verbose)
		{
			bool supported = true;

#if UNITY_2019_1_OR_NEWER
#else
            // Image Effects supported?
            if (!SystemInfo.supportsImageEffects)
			{
				if (verbose) { Debug.LogError("HighlightingSystem : Image effects is not supported on this platform!"); }
				supported = false;
			}
#endif

            // Required Render Texture Format supported?
            if (!SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGB32))
			{
				if (verbose) { Debug.LogError("HighlightingSystem : RenderTextureFormat.ARGB32 is not supported on this platform!"); }
				supported = false;
			}

			// HighlightingOpaque shader supported?
			if (!HighlighterCore.opaqueShader.isSupported)
			{
				if (verbose) { Debug.LogError("HighlightingSystem : HighlightingOpaque shader is not supported on this platform!"); }
				supported = false;
			}
			
			// HighlightingTransparent shader supported?
			if (!HighlighterCore.transparentShader.isSupported)
			{
				if (verbose) { Debug.LogError("HighlightingSystem : HighlightingTransparent shader is not supported on this platform!"); }
				supported = false;
			}

			// Highlighting shaders supported?
			for (int i = 0; i < shaders.Length; i++)
			{
				Shader shader = shaders[i];
				if (!shader.isSupported)
				{
					if (verbose) { Debug.LogError("HighlightingSystem : Shader '" + shader.name + "' is not supported on this platform!"); }
					supported = false;
				}
			}

			return supported;
		}

		// 
		protected virtual void RebuildCommandBuffer()
		{
			renderBuffer.Clear();

			renderBuffer.BeginSample(profileHighlightingSystem);

			// Prepare and clear render target
			renderBuffer.SetRenderTarget(highlightingBufferID);
			renderBuffer.ClearRenderTarget(true, true, colorClear);

			// Fill buffer with highlighters rendering commands
			HighlighterCore.FillBuffer(renderBuffer);

			RenderTextureDescriptor desc = cachedDescriptor;
			desc.width = highlightingBuffer.width / _downsampleFactor;
			desc.height = highlightingBuffer.height / _downsampleFactor;
			desc.depthBufferBits = 0;

			// Create two buffers for blurring the image
			renderBuffer.GetTemporaryRT(ShaderPropertyID._HighlightingBlur1, desc, FilterMode.Bilinear);
			renderBuffer.GetTemporaryRT(ShaderPropertyID._HighlightingBlur2, desc, FilterMode.Bilinear);

			renderBuffer.Blit(highlightingBufferID, blur1ID);

			// Blur the small texture
			bool oddEven = true;
			for (int i = 0; i < _iterations; i++)
			{
				float off = _blurMinSpread + _blurSpread * i;
				renderBuffer.SetGlobalFloat(ShaderPropertyID._HighlightingBlurOffset, off);
				
				if (oddEven)
				{
					renderBuffer.Blit(blur1ID, blur2ID, blurMaterial);
				}
				else
				{
					renderBuffer.Blit(blur2ID, blur1ID, blurMaterial);
				}
				
				oddEven = !oddEven;
			}

			// Upscale blurred texture and cut stencil from it
			renderBuffer.Blit(oddEven ? blur1ID : blur2ID, highlightingBufferID, cutMaterial);

			// Cleanup
			renderBuffer.ReleaseTemporaryRT(ShaderPropertyID._HighlightingBlur1);
			renderBuffer.ReleaseTemporaryRT(ShaderPropertyID._HighlightingBlur2);

			renderBuffer.EndSample(profileHighlightingSystem);
		}

		// Blit highlighting result to the destination RenderTexture
		public virtual void Blit(RenderTexture src, RenderTexture dst)
		{
			Graphics.Blit(src, dst, compMaterial);
		}
#endregion
	}
}