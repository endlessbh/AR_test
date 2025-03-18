using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginTools;
using XCSJ.Attributes;

namespace XCSJ.CommonUtils.PluginHighlightingSystem.Internal
{
	[Name("高亮模式")]
	public enum HighlighterMode : int
	{
		Disabled = -1, 
		Default = 0, 
		Overlay = 1, 
		Occluder = 2, 
	}

	/// <summary>
	/// 高亮核心
	/// </summary>
	[Name("高亮核心")]
	[DisallowMultipleComponent]
	public class HighlighterCore : BaseHighlighterMB
	{
		[UnityEngine.Internal.ExcludeFromDocs]
		private class RendererData
		{
			public Renderer renderer;
			public List<int> submeshIndices = new List<int>();
		}

		// Constants (don't touch this!)
		#region Constants
		// 
		[UnityEngine.Internal.ExcludeFromDocs]
		public const string keywordOverlay = "HIGHLIGHTING_OVERLAY";

		// Occlusion color
		private readonly Color occluderColor = new Color(0f, 0f, 0f, 0f);

		// Highlighting modes rendered in that order
		static private readonly HighlighterMode[] renderingOrder = new HighlighterMode[]
		{
			HighlighterMode.Default, 
			HighlighterMode.Overlay, 
			HighlighterMode.Occluder
		};

		// Pool chunk size for sRendererDataPool expansion
		private const int poolChunkSize = 4;
		#endregion

		#region Static Fields
		// Static collections to prevent unnecessary memory allocations for grabbing and filtering renderer components
		static private readonly List<Renderer> sRenderers = new List<Renderer>(4);
		static private readonly Stack<RendererData> sRendererDataPool = new Stack<RendererData>();
		static private readonly List<RendererData> sRendererData = new List<RendererData>(4);
		static private readonly List<int> sSubmeshIndices = new List<int>(4);
		static private readonly List<HighlighterCore> sHighlighters = new List<HighlighterCore>(); // Collection of all enabled highlighters
		static private ReadOnlyCollection<HighlighterCore> sHighlightersReadonly;
		#endregion

		#region Public Accessors
		static public ReadOnlyCollection<HighlighterCore> highlighters
		{
			get
			{
				if (sHighlightersReadonly == null)
				{
					sHighlightersReadonly = sHighlighters.AsReadOnly();
				}

				return sHighlightersReadonly;
			}
		}
		#endregion

		#region Public Fields
		/// <summary>
		/// Defines how this highlighter instance will be rendered. 
		/// </summary>
		public HighlighterMode mode = HighlighterMode.Default;

		/// <summary>
		/// Force-render this Highlighter. No culling is performed in this case (neither frustum nor occlusion culling) and renderers from all LOD levels will be always rendered. 
		/// Please be considerate in enabling this mode, or you may experience performance degradation. 
		/// </summary>
		public bool forceRender;

		/// <summary>
		/// Highlighting color. 
		/// </summary>
		public Color color = Color.white;
		#endregion

		#region Private Fields
		// Cached transform component reference
		private Transform tr;

		// Cached Renderers
		private List<HighlighterRenderer> highlightableRenderers = new List<HighlighterRenderer>();

		// Renderers dirty flag
		private bool isDirty = true;

		// Cached highlighting mode
		private bool cachedOverlay = false;

		// Cached highlighting color
		private Color cachedColor = Color.clear;

		// Opaque shader cached reference
		static private Shader _opaqueShader;
		[UnityEngine.Internal.ExcludeFromDocs]
		static public Shader opaqueShader
		{
			get
			{
				if (_opaqueShader == null)
				{
					_opaqueShader = Shader.Find("XDreamer/Highlighted/Opaque");
				}
				return _opaqueShader;
			}
		}

		// Transparent shader cached reference
		static private Shader _transparentShader;
		[UnityEngine.Internal.ExcludeFromDocs]
		static public Shader transparentShader
		{
			get
			{
				if (_transparentShader == null)
				{
					_transparentShader = Shader.Find("XDreamer/Highlighted/Transparent");
				}
				return _transparentShader;
			}
		}

		// Shared (for this component) replacement material for opaque geometry highlighting
		private Material _opaqueMaterial;
		private Material opaqueMaterial
		{
			get
			{
				if (_opaqueMaterial == null)
				{
					_opaqueMaterial = new Material(opaqueShader);

					// Make sure that shader will have proper default values
					_opaqueMaterial.SetKeyword(keywordOverlay, cachedOverlay);
					_opaqueMaterial.SetColor(ShaderPropertyID._HighlightingColor, cachedColor);
				}
				return _opaqueMaterial;
			}
		}
		#endregion

		#region Renderers Filtration
		public delegate bool RendererFilter(Renderer renderer, List<int> submeshIndices);

		static private RendererFilter _globalRendererFilter = null;
		static public RendererFilter globalRendererFilter
		{
			get { return _globalRendererFilter; }
			set
			{
				if (_globalRendererFilter != value)
				{
					_globalRendererFilter = value;

					for (int i = sHighlighters.Count - 1; i >= 0; i--)
					{
						var highlighter = sHighlighters[i];

						// Cleanup null highlighters for extra safety
						if (highlighter == null)
						{
							sHighlighters.RemoveAt(i);
							continue;
						}

						// Reinitialize all Highlighters without explicitly assigned rendererFilters
						if (highlighter.rendererFilter == null)
						{
							highlighter.SetDirty();
						}
					}
				}
			}
		}

		private RendererFilter _rendererFilter = null;
		public RendererFilter rendererFilter
		{
			get { return _rendererFilter; }
			set
			{
				if (_rendererFilter != value)
				{
					_rendererFilter = value;

					SetDirty();
				}
			}
		}

		protected virtual RendererFilter rendererFilterToUse
		{
			get
			{
				if (_rendererFilter != null)
				{
					return _rendererFilter;
				}
				else if (_globalRendererFilter != null)
				{
					return _globalRendererFilter;
				}
				else
				{
					return DefaultRendererFilter;
				}
			}
		}

		// 
		static public bool DefaultRendererFilter(Renderer renderer, List<int> submeshIndices)
		{
			// Do not highlight this renderer if it has HighlighterBlocker in parent
			if (renderer.GetComponentInParent<HighlighterBlocker>() != null) { return false; }

			bool pass = false;

			if (renderer is MeshRenderer) { pass = true; }
			else if (renderer is SkinnedMeshRenderer) { pass = true; }
			else if (renderer is SpriteRenderer) { pass = true; }
			else if (renderer is ParticleSystemRenderer) { pass = true; }

			if (pass)
			{
				// Highlight all submeshes
				submeshIndices.Add(-1);
			}

			return pass;
		}
		#endregion

		#region MonoBehaviour
		// Override and use AwakeSafe instead of this method
		private void Awake()
		{
			tr = GetComponent<Transform>();
			AwakeSafe();
		}

		// Override and use OnEnableSafe instead of this method
		protected override void OnEnable()
		{
			base.OnEnable();
			if (!sHighlighters.Contains(this)) { sHighlighters.Add(this); }
			OnEnableSafe();
		}

		// Override and use OnDisableSafe instead of this method
		protected override void OnDisable()
		{
			base.OnDisable();
			sHighlighters.Remove(this);
			ClearRenderers();
			isDirty = true;
			OnDisableSafe();
		}

		// 
		private void OnDestroy()
		{
			// Unity never garbage-collects unreferenced materials, so it is our responsibility to destroy them
			if (_opaqueMaterial != null)
			{
				Destroy(_opaqueMaterial);
			}
			OnDestroySafe();
		}
		#endregion

		#region MonoBehaviour Overrides
		// 
		protected virtual void AwakeSafe() { }

		// 
		protected virtual void OnEnableSafe() { }

		// 
		protected virtual void OnDisableSafe() { }

		// 
		protected virtual void OnDestroySafe() { }
		#endregion

		#region Public Methods
		/// <summary>
		/// Reinitialize renderers. 
		/// Call this method if your highlighted object has changed it's materials, renderers or child objects.
		/// Can be called multiple times per update - renderers reinitialization will occur only once.
		/// </summary>
		public void SetDirty()
		{
			isDirty = true;
		}
		#endregion

		#region Protected Methods
		// 
		protected virtual void UpdateHighlighting()
		{
			// Update highlighting mode and color here. 
		}
		#endregion

		#region Private Methods
		// Clear cached renderers
		private void ClearRenderers()
		{
			for (int i = highlightableRenderers.Count - 1; i >= 0; i--)
			{
				HighlighterRenderer renderer = highlightableRenderers[i];
				renderer.isAlive = false;
			}
			highlightableRenderers.Clear();
		}

		// This method defines how renderers are initialized
		private void UpdateRenderers()
		{
			if (isDirty)
			{
				isDirty = false;

				ClearRenderers();

				// Find all renderers which should be highlighted with this HighlighterCore component instance
				GrabRenderers(tr);

				// Cache found renderers
				for (int i = 0, imax = sRendererData.Count; i < imax; i++)
				{
					RendererData rendererData = sRendererData[i];
					GameObject rg = rendererData.renderer.gameObject;

					HighlighterRenderer renderer = rg.GetComponent<HighlighterRenderer>();
					if (renderer == null) { renderer = rg.AddComponent<HighlighterRenderer>(); }
					renderer.isAlive = true;

					renderer.Initialize(opaqueMaterial, transparentShader, rendererData.submeshIndices);
					renderer.SetOverlay(cachedOverlay);
					renderer.SetColor(cachedColor);
					highlightableRenderers.Add(renderer);
				}

				// Release RendererData instances
				for (int i = 0; i < sRendererData.Count; i++)
				{
					ReleaseRendererDataInstance(sRendererData[i]);
				}
				sRendererData.Clear();
			}
		}

		// Recursively follows hierarchy of objects from t, searches for Renderers and adds them to the list. 
		// Breaks if another Highlighter component found
		private void GrabRenderers(Transform t)
		{
			GameObject g = t.gameObject;

			// Find all Renderers on the current GameObject g, filter them and add to the sRendererData list
			g.GetComponents<Renderer>(sRenderers);
			for (int i = 0, imax = sRenderers.Count; i < imax; i++)
			{
				Renderer renderer = sRenderers[i];
				if (rendererFilterToUse(renderer, sSubmeshIndices))
				{
					RendererData rendererData = GetRendererDataInstance();
					rendererData.renderer = renderer;
					List<int> submeshIndices = rendererData.submeshIndices;
					submeshIndices.Clear();
					submeshIndices.AddRange(sSubmeshIndices);
					sRendererData.Add(rendererData);
				}
				sSubmeshIndices.Clear();
			}
			sRenderers.Clear();

			// Return if transform t doesn't have any children
			int childCount = t.childCount;
			if (childCount == 0) { return; }

			// Recursively cache renderers on all child GameObjects
			for (int i = 0; i < childCount; i++)
			{
				Transform childTransform = t.GetChild(i);

				// Do not cache Renderers of this childTransform in case it has it's own Highlighter component
				HighlighterCore h = childTransform.GetComponent<HighlighterCore>();
				if (h != null) { continue; }

				GrabRenderers(childTransform);
			}
		}

		// 
		private void FillBufferInternal(CommandBuffer buffer)
		{
			// Set cachedOverlay if changed
			bool overlayToUse = mode == HighlighterMode.Overlay || mode == HighlighterMode.Occluder;
			if (cachedOverlay != overlayToUse)
			{
				cachedOverlay = overlayToUse;

				opaqueMaterial.SetKeyword(keywordOverlay, cachedOverlay);
				for (int i = 0; i < highlightableRenderers.Count; i++) 
				{
					highlightableRenderers[i].SetOverlay(cachedOverlay);
				}
			}

			// Set cachedColor if changed
			Color colorToUse = mode != HighlighterMode.Occluder ? color : occluderColor;
			if (cachedColor != colorToUse)
			{
				cachedColor = colorToUse;

				// Apply color
				opaqueMaterial.SetColor(ShaderPropertyID._HighlightingColor, cachedColor);
				for (int i = 0; i < highlightableRenderers.Count; i++)
				{
					highlightableRenderers[i].SetColor(cachedColor);
				}
			}

			// Fill CommandBuffer with this highlighter rendering commands
			for (int i = highlightableRenderers.Count - 1; i >= 0; i--)
			{
				// To avoid null-reference exceptions when cached renderer has been removed but SetDirty() wasn't been called
				HighlighterRenderer renderer = highlightableRenderers[i];
				if (renderer == null)
				{
					highlightableRenderers.RemoveAt(i);
				}
				else if (!renderer.IsValid())
				{
					highlightableRenderers.RemoveAt(i);
					renderer.isAlive = false;
				}
				else 
				{
					// Check if renderer is visible for the currently rendering camera. 
					// (Last camera called OnWillRenderObject on HighlighterRenderer instance is the same camera for which we're currently filling CommandBuffer)
					if (HighlightingBase.GetVisible(renderer) || forceRender)
					{
						renderer.FillBuffer(buffer);
					}
				}
			}
		}
		#endregion

		#region Static Methods
		// 
		static private void ExpandRendererDataPool(int count)
		{
			for (int i = 0; i < count; i++)
			{
				RendererData instance = new RendererData();
				sRendererDataPool.Push(instance);
			}
		}

		// 
		static private RendererData GetRendererDataInstance()
		{
			RendererData instance;

			if (sRendererDataPool.Count == 0)
			{
				ExpandRendererDataPool(poolChunkSize);
			}

			instance = sRendererDataPool.Pop();

			return instance;
		}

		// 
		static private void ReleaseRendererDataInstance(RendererData instance)
		{
			if (instance == null || sRendererDataPool.Contains(instance)) { return; }

			instance.renderer = null;
			instance.submeshIndices.Clear();

			sRendererDataPool.Push(instance);
		}

		// Fill CommandBuffer with highlighters rendering commands
		[UnityEngine.Internal.ExcludeFromDocs]
		static public void FillBuffer(CommandBuffer buffer)
		{
			// Update all highlighters
			for (int i = sHighlighters.Count - 1; i >= 0; i--)
			{
				var highlighter = sHighlighters[i];

				// Cleanup null highlighters for extra safety
				if (highlighter == null)
				{
					sHighlighters.RemoveAt(i);
					continue;
				}

				// Update mode and color
				highlighter.UpdateHighlighting();

				// Cleanup again in case Highlighter has been destroyed in UpdateHighlighting() call
				if (highlighter == null)
				{
					sHighlighters.RemoveAt(i);
					continue;
				}

				// Update Renderer's if dirty
				highlighter.UpdateRenderers();
			}

			// Fill CommandBuffer with highlighters rendering commands
			for (int i = 0; i < renderingOrder.Length; i++)
			{
				HighlighterMode renderMode = renderingOrder[i];

				for (int j = sHighlighters.Count - 1; j >= 0; j--)
				{
					var highlighter = sHighlighters[j];
					if (highlighter.mode == renderMode)
					{
						highlighter.FillBufferInternal(buffer);
					}
				}
			}
		}
		#endregion
	}
}