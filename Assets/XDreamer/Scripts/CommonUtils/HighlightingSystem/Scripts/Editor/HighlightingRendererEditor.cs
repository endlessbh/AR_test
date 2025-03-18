using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Reflection;
using XCSJ.CommonUtils.PluginHighlightingSystem;
using XCSJ.CommonUtils.PluginHighlightingSystem.Internal;

namespace XCSJ.CommonUtils.EditorHighlightingSystem
{
	[CustomEditor(typeof(HighlightingRenderer), true)]
	public class HighlightingRendererEditor : Editor
	{
		#region Static Fields and Constants
		static protected readonly GUIContent labelButtonRemove = new GUIContent("Remove Preset");
		static protected readonly GUIContent labelAntiAliasing = new GUIContent("Anti Aliasing");
		static protected readonly GUIContent[] antiAliasingOptions = new GUIContent[] { new GUIContent("Use Value From Quality Settings"), new GUIContent("Disabled"), new GUIContent("2x Multi Sampling"), new GUIContent("4x Multi Sampling"), new GUIContent("8x Multi Sampling") };
		#endregion

		#region Protected Fields
		protected HighlightingRenderer hr;
		protected SavePresetWindow window;
		protected MethodInfo boldFontMethodInfo;
		protected Rect[] buttonRects = new Rect[2];
		protected GUIContent[] presetNames;
		#endregion

		#region Editor
		// 
		protected virtual void OnEnable()
		{
			hr = target as HighlightingRenderer;
		}

		// 
		public override void OnInspectorGUI()
		{
			#if UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8 || UNITY_BLACKBERRY
			if (!PlayerSettings.use32BitDisplayBuffer)
			{
			EditorGUILayout.HelpBox("Highlighting System requires 32-bit display buffer. Set the 'Use 32-bit Display Buffer' checkbox under the 'Resolution and Presentation' section of Player Settings.", MessageType.Error);
			}
			#endif

			EditorGUILayout.Space();

			// HighlightingBlitter field
			if (hr.blitter == null)
			{
				EditorGUILayout.HelpBox("Use order of this component (relatively to other Image Effects on this camera) to control the point at which highlighting will be applied to the framebuffer (click on a little gear icon to the right and choose Move Up / Move Down) or assign HighlightingBlitter component from another camera.", MessageType.Info);
			}
			else if (hr.GetComponent<Camera>() == hr.blitter.GetComponent<Camera>())
			{
				EditorGUILayout.HelpBox("Assigned HighlightingBlitter component exists on the same camera. This is not really necessary in most situations and affects rendering performance! Please make sure this is intended.", MessageType.Warning);
			}
			hr.blitter = EditorGUILayout.ObjectField("Blitter (Optional)", hr.blitter, typeof(HighlightingBlitter), true) as HighlightingBlitter;

			hr.antiAliasing = (AntiAliasing)EditorGUILayout.Popup(labelAntiAliasing, (int)hr.antiAliasing, antiAliasingOptions);

			InitializePresetNames();
			int presetIndex = IdentifyCurrentPreset();

			EditorGUILayout.Space();

			// Preset fields start
			EditorGUILayout.LabelField("Preset:", EditorStyles.boldLabel);

			// Preset selection popup
			int oldIndex = presetIndex;
			int newIndex = EditorGUILayout.Popup(oldIndex, presetNames);
			if (oldIndex != newIndex)
			{
				hr.LoadPreset(presetNames[newIndex].text);
				presetIndex = newIndex;
			}

			HighlightingPreset currentPreset = new HighlightingPreset();
			if (presetIndex >= 0)
			{
				hr.GetPreset(presetNames[presetIndex].text, out currentPreset);
			}

			EditorGUI.BeginChangeCheck();

			// Fill Alpha
			SetBoldDefaultFont(presetIndex < 0 || hr.fillAlpha != currentPreset.fillAlpha);
			hr.fillAlpha = EditorGUILayout.Slider(HighlightingPresetEditor.labelFillAlpha, hr.fillAlpha, 0f, 1f);

			// Downsample factor
			SetBoldDefaultFont(presetIndex < 0 || hr.downsampleFactor != currentPreset.downsampleFactor);
			hr.downsampleFactor = HighlightingPresetEditor.downsampleSet[EditorGUILayout.Popup(HighlightingPresetEditor.labelDownsampling, HighlightingPresetEditor.downsampleGet[hr.downsampleFactor], HighlightingPresetEditor.downsampleOptions)];

			// Iterations
			SetBoldDefaultFont(presetIndex < 0 || hr.iterations != currentPreset.iterations);
			hr.iterations = Mathf.Clamp(EditorGUILayout.IntField(HighlightingPresetEditor.labelIterations, hr.iterations), 0, 50);

			// Blur min spread
			SetBoldDefaultFont(presetIndex < 0 || hr.blurMinSpread != currentPreset.blurMinSpread);
			hr.blurMinSpread = EditorGUILayout.Slider(HighlightingPresetEditor.labelBlurMinSpread, hr.blurMinSpread, 0f, 3f);

			// Blur spread
			SetBoldDefaultFont(presetIndex < 0 || hr.blurSpread != currentPreset.blurSpread);
			hr.blurSpread = EditorGUILayout.Slider(HighlightingPresetEditor.labelBlurSpread, hr.blurSpread, 0f, 3f);

			// Blur intensity
			SetBoldDefaultFont(presetIndex < 0 || hr.blurIntensity != currentPreset.blurIntensity);
			hr.blurIntensity = EditorGUILayout.Slider(HighlightingPresetEditor.labelBlurIntensity, hr.blurIntensity, 0f, 1f);

			// Blur straight directions
			SetBoldDefaultFont(presetIndex < 0 || hr.blurDirections != currentPreset.blurDirections);
			hr.blurDirections = (BlurDirections)EditorGUILayout.Popup(HighlightingPresetEditor.labelBlurDirections, (int)hr.blurDirections, HighlightingPresetEditor.blurDirections);

			SetBoldDefaultFont(false);

			if (EditorGUI.EndChangeCheck())
			{
				EditorUtility.SetDirty(hr);
			}

			// Define button rects
			Rect position = GUILayoutUtility.GetRect(0f, 16f, GUILayout.ExpandWidth(true));
			HighlightingRendererEditor.GetColumnRects(position, 2f, buttonRects);

			// Save preset button

			using (new EditorGUI.DisabledScope(Application.isPlaying || 
				(hr.downsampleFactor == currentPreset.downsampleFactor && 
				hr.iterations == currentPreset.iterations && 
				hr.blurMinSpread == currentPreset.blurMinSpread && 
				hr.blurSpread == currentPreset.blurSpread && 
				hr.blurIntensity == currentPreset.blurIntensity && 
				hr.blurDirections == currentPreset.blurDirections)))
			{
				if (GUI.Button(buttonRects[0], "Save Preset"))
				{
					window = SavePresetWindow.Init(presetIndex < 0 ? "My Preset" : presetNames[presetIndex].text, SavePresetAs);
				}
			}

			// Remove preset button
			using (new EditorGUI.DisabledScope(Application.isPlaying || presetIndex < 0))
			{
				if (GUI.Button(buttonRects[1], labelButtonRemove) && presetIndex >= 0)
				{
					string presetName = presetNames[presetIndex].text;
					if (EditorUtility.DisplayDialog("Removing Preset", "Are you sure you want to remove Preset '" + presetName + "'?", "Yes", "No"))
					{
						hr.RemovePreset(presetName);
						EditorUtility.SetDirty(hr);
						ReadOnlyCollection<HighlightingPreset> presets = hr.presets;
						if (presets.Count > 0)
						{
							presetIndex = 0;
							hr.ApplyPreset(presets[presetIndex]);
						}
						InitializePresetNames();
					}
				}
			}
			
			EditorGUILayout.Space();
		}
		#endregion

		#region Protected Methods
		// 
		protected virtual void InitializePresetNames()
		{
			ReadOnlyCollection<HighlightingPreset> presets = hr.presets;
			int presetCount = presets.Count;
			if (presetNames == null || presetNames.Length != presetCount)
			{
				presetNames = new GUIContent[presetCount];
			}

			for (int i = 0; i < presetCount; i++)
			{
				HighlightingPreset preset = presets[i];
				GUIContent presetName = presetNames[i];
				if (presetName == null)
				{
					presetName = new GUIContent();
					presetNames[i] = presetName;
				}
				presetName.text = preset.name;
			}
		}

		// Returns current preset index
		protected virtual int IdentifyCurrentPreset()
		{
			ReadOnlyCollection<HighlightingPreset> presets = hr.presets;
			for (int i = presets.Count - 1; i >= 0; i--)
			{
				HighlightingPreset p = presets[i];
				if (hr.downsampleFactor == p.downsampleFactor && 
					hr.iterations == p.iterations && 
					hr.blurMinSpread == p.blurMinSpread && 
					hr.blurSpread == p.blurSpread && 
					hr.blurIntensity == p.blurIntensity && 
					hr.blurDirections == p.blurDirections)
				{
					return i;
				}
			}
			return -1;
		}

		// 
		protected virtual bool SavePresetAs(string name)
		{
			name = name.Trim();

			window = null;

			if (string.IsNullOrEmpty(name))
			{
				EditorUtility.DisplayDialog("Unable to save Preset", "Please specify valid Preset name.", "Close");
				return false;
			}

			// Preset with this name exists?
			HighlightingPreset preset;
			if (hr.GetPreset(name, out preset))
			{
				// Overwrite?
				if (!EditorUtility.DisplayDialog("Overwriting Preset", "Preset '" + name + "' already exists. Overwrite?", "Yes", "No"))
				{
					return false;
				}
			}

			// Add or overwrite preset
			preset.name = name;
			preset.downsampleFactor = hr.downsampleFactor;
			preset.iterations = hr.iterations;
			preset.blurMinSpread = hr.blurMinSpread;
			preset.blurSpread = hr.blurSpread;
			preset.blurIntensity = hr.blurIntensity;
			preset.blurDirections = hr.blurDirections;

			hr.AddPreset(preset, true);
			EditorUtility.SetDirty(hr);
			InitializePresetNames();
			IdentifyCurrentPreset();

			return true;
		}

		// 
		protected void SetBoldDefaultFont(bool value)
		{
			if (boldFontMethodInfo == null)
			{
				boldFontMethodInfo = typeof(EditorGUIUtility).GetMethod("SetBoldDefaultFont", BindingFlags.Static | BindingFlags.NonPublic);
			}
			boldFontMethodInfo.Invoke(null, new[] { value as object });
		}
		#endregion

		#region Helpers
		// 
		static public void GetRowRects(Rect rect, float space, Rect[] rects)
		{
			int l = rects.Length;
			for (int i = 0; i < l; i++)
			{
				float h = (rect.height - space * (l - 1)) / l;
				if (h < 0f) { h = 0f; }

				Rect r = rects[i];

				r.x = rect.x;
				r.y = rect.y + i * (h + space);
				r.width = rect.width;
				r.height = h;

				rects[i] = r;
			}
		}

		// 
		static public void GetColumnRects(Rect rect, float space, Rect[] rects)
		{
			int l = rects.Length;
			for (int i = 0; i < l; i++)
			{
				float w = (rect.width - space * (l - 1)) / l;
				if (w < 0f) { w = 0f; }

				Rect r = rects[i];

				r.x = rect.x + i * (w + space);
				r.y = rect.y;
				r.width = w;
				r.height = rect.height;

				rects[i] = r;
			}
		}
		#endregion
		
	}

	#region SavePresetWindow
	public class SavePresetWindow : EditorWindow
	{
		static private readonly string presetTextFieldName = "PresetTextFieldName";
		public delegate bool InputResult(string input);
		private event InputResult callback;
		private string presetName;

		// 
		public static SavePresetWindow Init(string name, InputResult callback)
		{
			Rect rect = new Rect(Screen.width * 0.5f, Screen.height * 0.5f, 300f, 60f);
			SavePresetWindow window = GetWindowWithRect<SavePresetWindow>(rect, true, "Specify Preset Name", true);
			window.callback = callback;
			window.presetName = name;
			return window;
		}

		// 
		void OnGUI()
		{
			GUI.SetNextControlName(presetTextFieldName);
			presetName = EditorGUILayout.TextField("Preset Name", presetName);

			EditorGUI.FocusTextInControl(presetTextFieldName);

			bool pressed = GUILayout.Button("Save Preset", GUILayout.ExpandHeight(true));
			Event e = Event.current;
			bool submitted = e.type == EventType.KeyUp && GUI.GetNameOfFocusedControl() == presetTextFieldName && (e.keyCode == KeyCode.Return || e.keyCode == KeyCode.KeypadEnter);

			if (pressed || submitted)
			{
				OnSavePreset();
				GUIUtility.ExitGUI();
			}
		}

		// 
		void OnLostFocus() { Quit(); }
		void OnSelectionChange() { Quit(); }
		void OnProjectChange() { Quit(); }

		// 
		private void OnSavePreset()
		{
			if (callback(presetName))
			{
				Close();
			}
		}

		// 
		private void Quit()
		{
			Close();
		}
	}
	#endregion
}