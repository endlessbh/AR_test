using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;

namespace XCSJ.CommonUtils.PluginHighlightingSystem.Internal
{
	[Name("帧结束")]
	[UnityEngine.Internal.ExcludeFromDocs]
	public class EndOfFrame : BaseHighlighterMB
	{
		[UnityEngine.Internal.ExcludeFromDocs]
		public delegate void OnEndOfFrame();

		#region Static Fields
		static private EndOfFrame _singleton;
		static private EndOfFrame singleton
		{
			get
			{
				if (_singleton == null)
				{
					GameObject go = new GameObject("EndOfFrameHelper");
					go.hideFlags = HideFlags.HideAndDontSave;
					_singleton = go.AddComponent<EndOfFrame>();
				}
				return _singleton;
			}
		}
		#endregion

		#region Private Fields
		private WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
		private Coroutine coroutine;
		private List<OnEndOfFrame> listeners = new List<OnEndOfFrame>();
		#endregion

		#region MonoBehaviour

		/// <summary>
		/// 启用
		/// </summary>
		protected override void OnEnable()
		{
			base.OnEnable();
			coroutine = StartCoroutine(EndOfFrameRoutine());
		}

		/// <summary>
		/// 禁用
		/// </summary>
		protected override void OnDisable()
		{
			base.OnDisable();
			if (coroutine != null)
			{
				StopCoroutine(coroutine);
			}
		}
		#endregion

		#region Public Methods
		// 
		static public void AddListener(OnEndOfFrame listener)
		{
			if (listener == null) { return; }

			singleton.listeners.Add(listener);
		}

		// 
		static public void RemoveListener(OnEndOfFrame listener)
		{
			if (listener == null || _singleton == null) { return; }

			singleton.listeners.Remove(listener);
		}
		#endregion

		#region Private Methods
		// 
		private IEnumerator EndOfFrameRoutine()
		{
			while (true)
			{
				yield return waitForEndOfFrame;

				for (int i = listeners.Count - 1; i >= 0; i--)
				{
					OnEndOfFrame listener = listeners[i];
					if (listener != null)
					{
						listener();
					}
					else
					{
						listeners.RemoveAt(i);
					}
				}
			}
		}
		#endregion
	}
}