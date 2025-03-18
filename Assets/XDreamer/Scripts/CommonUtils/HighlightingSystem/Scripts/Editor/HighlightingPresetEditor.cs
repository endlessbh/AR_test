using UnityEditor;
using UnityEngine;
using XCSJ.CommonUtils.PluginHighlightingSystem.Internal;

namespace XCSJ.CommonUtils.EditorHighlightingSystem
{
	[CustomPropertyDrawer(typeof(HighlightingPreset), true)]
	public class HighlightingPresetEditor : PropertyDrawer
	{
		#region Static Fields and Constants
		static public readonly GUIContent labelFillAlpha = new GUIContent("Fill Alpha", "Inner fill alpha value.");
		static public readonly GUIContent labelDownsampling = new GUIContent("Downsampling:", "Downsampling factor.");
		static public readonly GUIContent labelIterations = new GUIContent("Iterations:", "Blur iterations. Number of blur iterations performed. Larger number means more blur.");
		static public readonly GUIContent labelBlurMinSpread = new GUIContent("Min Spread:", "Blur Min Spread. Lower values give better looking blur, but require more iterations to get large blurs. Pixel offset for each blur iteration is calculated as 'Min Spread + Spread * Iteration Index'. Usually 'Min Spread + Spread' value is between 0.5 and 1.0.");
		static public readonly GUIContent labelBlurSpread = new GUIContent("Spread:", "Blur Spread. Lower values give better looking blur, but require more iterations to get large blurs. Pixel offset for each blur iteration is calculated as 'Min Spread + Spread * Iteration Index'. Usually 'Min Spread + Spread' value is between 0.5 and 1.0.");
		static public readonly GUIContent labelBlurIntensity = new GUIContent("Intensity:", "Highlighting Intensity. Internally defines the value by which highlighting buffer alpha channel will be multiplied after each blur iteration.");
		static public readonly GUIContent labelBlurDirections = new GUIContent("Blur Directions:", "Blur directions.");

		static public readonly GUIContent[] downsampleOptions = new GUIContent[] { new GUIContent("None"), new GUIContent("Half"), new GUIContent("Quarter") };
		static public readonly int[] downsampleGet = new int[] { -1, 0, 1, -1, 2 };		// maps downsampleFactor to the downsampleOptions index
		static public readonly int[] downsampleSet = new int[] {     1, 2,     4 };		// maps downsampleOptions index to the downsampleFactor
		static public readonly GUIContent[] blurDirections = new GUIContent[] { new GUIContent("Diagonal"), new GUIContent("Straight"), new GUIContent("All") };

		private const float rowSpace = 2f;
		#endregion

		#region Private Fields
		private Rect[] rects = new Rect[8];
		#endregion

		#region PropertyDrawer
		// 
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			int l = rects.Length;
			return 16f * l + rowSpace * (l - 1);
		}

		// 
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);

			// Draw label
			position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

			int indent = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;

			// Calculate rects
			HighlightingRendererEditor.GetRowRects(position, rowSpace, rects);

			// Find properties
			SerializedProperty propertyName = property.FindPropertyRelative("_name");
			SerializedProperty propertyFillAlpha = property.FindPropertyRelative("_fillAlpha");
			SerializedProperty propertyDownsampleFactor = property.FindPropertyRelative("_downsampleFactor");
			SerializedProperty propertyIterations = property.FindPropertyRelative("_iterations");
			SerializedProperty propertyBlurMinSpread = property.FindPropertyRelative("_blurMinSpread");
			SerializedProperty propertyBlurSpread = property.FindPropertyRelative("_blurSpread");
			SerializedProperty propertyBlurIntensty = property.FindPropertyRelative("_blurIntensity");
			SerializedProperty propertyBlurDirections = property.FindPropertyRelative("_blurDirections");

			// Draw properties
			int index = 0;
			propertyName.stringValue = EditorGUI.TextField(rects[index], propertyName.stringValue); index++;
			propertyFillAlpha.floatValue = EditorGUI.Slider(rects[index], labelFillAlpha, propertyFillAlpha.floatValue, 0f, 1f); index++;
			propertyDownsampleFactor.intValue = downsampleSet[EditorGUI.Popup(rects[index], labelDownsampling, downsampleGet[propertyDownsampleFactor.intValue], downsampleOptions)]; index++;
			propertyIterations.intValue = Mathf.Clamp(EditorGUI.IntField(rects[index], labelIterations, propertyIterations.intValue), 0, 50); index++;
			propertyBlurMinSpread.floatValue = EditorGUI.Slider(rects[index], labelBlurMinSpread, propertyBlurMinSpread.floatValue, 0f, 3f); index++;
			propertyBlurSpread.floatValue = EditorGUI.Slider(rects[index], labelBlurSpread, propertyBlurSpread.floatValue, 0f, 3f); index++;
			propertyBlurIntensty.floatValue = EditorGUI.Slider(rects[index], labelBlurIntensity, propertyBlurIntensty.floatValue, 0f, 1f); index++;
			propertyBlurDirections.enumValueIndex = (int)EditorGUI.Popup(rects[index], labelBlurDirections, propertyBlurDirections.enumValueIndex, blurDirections); index++;

			EditorGUI.indentLevel = indent;

			EditorGUI.EndProperty();
		}
		#endregion
	}
}