using UnityEngine;
using UnityEditor;
using XCSJ.CommonUtils.PluginHighlightingSystem;

namespace XCSJ.CommonUtils.EditorHighlightingSystem
{
	[CustomEditor(typeof(HighlightingBlitter), true)]
	public class HighlightingBlitterEditor : Editor
	{
		// 
		public override void OnInspectorGUI()
		{
			EditorGUILayout.HelpBox("Use order of this component (relatively to other Image Effects on this camera) to control the point at which highlighting will be applied to the framebuffer (click on a little gear icon to the right and choose Move Up / Move Down).", MessageType.Info);
		}
	}
}