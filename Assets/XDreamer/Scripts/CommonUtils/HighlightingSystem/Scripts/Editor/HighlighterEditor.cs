using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using XCSJ.CommonUtils.PluginHighlightingSystem;

namespace XCSJ.CommonUtils.EditorHighlightingSystem
{
	[CustomEditor(typeof(XCSJ.CommonUtils.PluginHighlightingSystem.Highlighter)), CanEditMultipleObjects]
	public class HighlighterEditor : Editor
	{
		private const string kShowTweenKey = "HighlightingSystem.Highlighter.ShowTween";
		private const string kShowConstantKey = "HighlightingSystem.Highlighter.ShowConstant";
		private const string kShowFilterKey = "HighlightingSystem.Highlighter.ShowFilter";

		static private readonly GUIContent labelOverlay = new GUIContent("Overlay");
		static private readonly GUIContent labelOccluder = new GUIContent("Occluder");
		static private readonly GUIContent labelColor = new GUIContent("Color (Current)");
		static private readonly GUIContent labelForceRender = new GUIContent("Force Render");

		static private readonly GUIContent labelTween = new GUIContent("Tween");
		static private readonly GUIContent labelGradient = new GUIContent("Gradient");
		static private readonly GUIContent labelDuration = new GUIContent("Duration");
		static private readonly GUIContent labelUseUnscaledTime = new GUIContent("Use Unscaled Time");
		static private readonly GUIContent labelDelay = new GUIContent("Delay");
		static private readonly GUIContent labelLoopMode = new GUIContent("Loop Mode");
		static private readonly GUIContent labelEasing = new GUIContent("Easing");
		static private readonly GUIContent labelReverse = new GUIContent("Reverse");
		static private readonly GUIContent labelRepeatCount = new GUIContent("Repeat Count");

		static private readonly GUIContent labelConstant = new GUIContent("Constant");
		static private readonly GUIContent labelConstantColor = new GUIContent("Constant Color");
		static private readonly GUIContent labelFadeInTime = new GUIContent("Fade In Time");
		static private readonly GUIContent labelFadeOutTime = new GUIContent("Fade Out Time");

		static private readonly GUIContent labelFilterMode = new GUIContent("Mode");
		static private readonly GUIContent labelFilterList = new GUIContent("Transform Filtering List");

		static private readonly GUIContent labelGroupTween = new GUIContent("Tween");
		static private readonly GUIContent labelGroupConstant = new GUIContent("Constant");
		static private readonly GUIContent labelGroupFilter = new GUIContent("Filter");

		private Dictionary<string, SerializedProperty> fieldCache = new Dictionary<string, SerializedProperty>();
		private XCSJ.CommonUtils.PluginHighlightingSystem.Highlighter highlighter;
		private SerializedProperty propertyFilterList;

		private ReorderableList listFilter;

		private bool showTween;
		private bool showConstant;
		private bool showFilter;

		// 
		void OnEnable()
		{
			highlighter = target as XCSJ.CommonUtils.PluginHighlightingSystem.Highlighter;

			propertyFilterList = serializedObject.FindProperty("_filterList");

			showTween = EditorPrefs.GetBool(kShowTweenKey, false);
			showConstant = EditorPrefs.GetBool(kShowConstantKey, false);
			showFilter = EditorPrefs.GetBool(kShowFilterKey, false);

			listFilter = new ReorderableList(serializedObject, propertyFilterList, true, true, true, true);
			listFilter.drawHeaderCallback = new ReorderableList.HeaderCallbackDelegate(FilterListDrawHeader);
			listFilter.onAddCallback = new ReorderableList.AddCallbackDelegate(FilterListAdd);
			listFilter.onRemoveCallback = new ReorderableList.RemoveCallbackDelegate(FilterListRemove);
			listFilter.drawElementCallback = new ReorderableList.ElementCallbackDelegate(FilterListDrawElement);
			listFilter.elementHeight = EditorGUIUtility.singleLineHeight + 2f;
		}

		// 
		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			EditorGUI.BeginChangeCheck();

			DoGeneralGUI();
			DoHighlighterGUI();

			if (EditorGUI.EndChangeCheck())
			{
				serializedObject.ApplyModifiedProperties();
			}
		}

		// 
		private void DoGeneralGUI()
		{
			// General
			EditorGUILayout.PropertyField(FindField("_overlay"), labelOverlay);
			EditorGUILayout.PropertyField(FindField("_occluder"), labelOccluder);
			EditorGUILayout.PropertyField(FindField("forceRender"), labelForceRender);

			// Current color (readonly, since Highlighter component overrides it's value every frame)
			using (new EditorGUI.DisabledScope(true))
			{
				EditorGUILayout.PropertyField(FindField("color"), labelColor);
			}
		}

		// 
		private void DoHighlighterGUI()
		{
			// Tween
			if (Foldout(labelGroupTween, ref showTween, kShowTweenKey))
			{
				using (new EditorGUI.IndentLevelScope())
				{
					EditorGUILayout.PropertyField(FindField("_tween"), labelTween);
					EditorGUILayout.PropertyField(FindField("_tweenGradient"), labelGradient);
					//DoSpectrumButtonGUI();
					EditorGUILayout.PropertyField(FindField("_tweenDuration"), labelDuration);
					EditorGUILayout.PropertyField(FindField("_tweenReverse"), labelReverse);
					EditorGUILayout.PropertyField(FindField("_tweenLoop"), labelLoopMode);
					EditorGUILayout.PropertyField(FindField("_tweenEasing"), labelEasing);
					EditorGUILayout.PropertyField(FindField("_tweenDelay"), labelDelay);
					EditorGUILayout.PropertyField(FindField("_tweenRepeatCount"), labelRepeatCount);
					EditorGUILayout.PropertyField(FindField("_tweenUseUnscaledTime"), labelUseUnscaledTime);
				}
			}

			// Constant
			if (Foldout(labelGroupConstant, ref showConstant, kShowConstantKey))
			{
				using (new EditorGUI.IndentLevelScope())
				{
					EditorGUILayout.PropertyField(FindField("_constant"), labelConstant);
					EditorGUILayout.PropertyField(FindField("_constantColor"), labelConstantColor);
					EditorGUILayout.PropertyField(FindField("_constantFadeInTime"), labelFadeInTime);
					EditorGUILayout.PropertyField(FindField("_constantFadeOutTime"), labelFadeOutTime);
					EditorGUILayout.PropertyField(FindField("_constantEasing"), labelEasing);
					EditorGUILayout.PropertyField(FindField("_constantUseUnscaledTime"), labelUseUnscaledTime);
				}
			}

			if (Foldout(labelGroupFilter, ref showFilter, kShowFilterKey))
			{
				using (new EditorGUI.IndentLevelScope())
				{
					// filterMode
					SerializedProperty fieldFilterMode = FindField("_filterMode");
					EditorGUILayout.PropertyField(fieldFilterMode, labelFilterMode);

					if (!fieldFilterMode.hasMultipleDifferentValues)
					{
						RendererFilterMode filterMode = (RendererFilterMode)fieldFilterMode.enumValueIndex;

						if (highlighter.rendererFilter != null)
						{
							EditorGUILayout.HelpBox("Custom RendererFilter assigned to this Highlighter. Filtering list disabled.", MessageType.Warning);
						}
						else
						{
							switch (filterMode)
							{
								case RendererFilterMode.None:
									EditorGUILayout.HelpBox("All Renderers found in child GameObjects will be highlighted.", MessageType.Info);
									break;
								case RendererFilterMode.Include:
									EditorGUILayout.HelpBox("Renderers only on specified Transforms (and any of their children) will be highlighted.", MessageType.Info);
									break;
								case RendererFilterMode.Exclude:
									EditorGUILayout.HelpBox("Renderers on specified Transforms (and any of their children) will be excluded from highlighting.", MessageType.Info);
									break;
							}
						}
					}

					// filterList
					Rect rect = EditorGUI.IndentedRect(GUILayoutUtility.GetRect(0f, listFilter.GetHeight()));
					listFilter.DoList(rect);
				}
			}
		}

		// 
		private SerializedProperty FindField(string fieldPath)
		{
			SerializedProperty field;
			if (!fieldCache.TryGetValue(fieldPath, out field))
			{
				field = serializedObject.FindProperty(fieldPath);
				fieldCache.Add(fieldPath, field);
			}
			return field;
		}

		// 
		private bool Foldout(GUIContent content, ref bool isExpanded, string key)
		{
			Rect rect = GUILayoutUtility.GetRect(content, EditorStyles.foldout);
			bool expanded = EditorGUI.Foldout(rect, isExpanded, content, true, EditorStyles.foldout);
			if (expanded != isExpanded && !string.IsNullOrEmpty(key))
			{
				isExpanded = expanded;
				EditorPrefs.SetBool(key, isExpanded);
			}
			return isExpanded;
		}

		// 
		private void DoSpectrumButtonGUI()
		{
			// Spectrum
			if (GUILayout.Button("Spectrum"))
			{
				highlighter.tweenGradient = new Gradient()
				{
					colorKeys = new GradientColorKey[]
					{
						new GradientColorKey(new Color(1f, 0f, 0f, 1f), 0f / 6f), 
						new GradientColorKey(new Color(1f, 1f, 0f, 1f), 1f / 6f), 
						new GradientColorKey(new Color(0f, 1f, 0f, 1f), 2f / 6f), 
						new GradientColorKey(new Color(0f, 1f, 1f, 1f), 3f / 6f), 
						new GradientColorKey(new Color(0f, 0f, 1f, 1f), 4f / 6f), 
						new GradientColorKey(new Color(1f, 0f, 1f, 1f), 5f / 6f), 
						new GradientColorKey(new Color(1f, 0f, 0f, 1f), 6f / 6f), 
					}, 
					alphaKeys = new GradientAlphaKey[]
					{
						new GradientAlphaKey(1f, 0f), 
						new GradientAlphaKey(1f, 1f), 
					}
				};
				GUI.changed = true;
			}
		}

		// 
		private void FilterListDrawHeader(Rect rect)
		{
			EditorGUI.LabelField(rect, labelFilterList);
		}

		// 
		private void FilterListAdd(ReorderableList list)
		{
			ReorderableList.defaultBehaviours.DoAddButton(list);
		}

		// 
		private void FilterListRemove(ReorderableList list)
		{
			ReorderableList.defaultBehaviours.DoRemoveButton(list);
		}

		// 
		private void FilterListDrawElement(Rect rect, int index, bool selected, bool focused)
		{
			rect.height -= 2f;
			var propertyElement = propertyFilterList.GetArrayElementAtIndex(index);
			EditorGUI.ObjectField(rect, propertyElement);
			//EditorGUIUtility.ShowObjectPicker;
		}
	}
}

