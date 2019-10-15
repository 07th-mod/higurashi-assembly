using System.Collections.Generic;
using Assets.Scripts.Core.Audio;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Core.History
{
	public class HistoryTextButton : MonoBehaviour
	{
		private UITweener tweener;

		private TextMeshPro textMesh;

		private List<List<AudioInfo>> voices;

		private bool isHovered = false;

		/// <summary>
		/// Used by HistoryWindow to block audio playback when closing
		/// Solves the issue where right click plays audio in addition to closing the history window
		/// There's probably a better way to do this, but this works
		/// Note: The solution relies on HistoryWindow's Leave getting called before the button's OnClick.  If the patch isn't working, that might be the cause.
		/// </summary>
		private bool isClosing = false;

		public TextMeshPro GetTextMesh()
		{
			if (textMesh == null)
			{
				textMesh = GetComponent<TextMeshPro>();
			}
			return textMesh;
		}

		private void UpdateColorForHover()
		{
			if (isHovered && voices != null && voices.Count > 0)
			{
				UpdateColor(new Color(0.2f, 0.7f, 1f));
			}
			else
			{
				UpdateColor(new Color(1f, 1f, 1f));
			}
		}

		private void OnHover(bool isHover)
		{
			isHovered = isHover;
			UpdateColorForHover();
		}

		private void OnClick()
		{
			if (!isClosing && voices != null && voices.Count > 0)
			{
				GameSystem.Instance.AudioController.PlayVoices(voices);
			}
		}

		public void SetIsClosing(bool isClosing)
		{
			this.isClosing = isClosing;
		}

		public void UpdateAlpha(float a)
		{
			Color color = textMesh.color;
			color.a = a;
			textMesh.color = color;
		}

		public void UpdateColor(Color color)
		{
			color.a = GetTextMesh().color.a;
			GetTextMesh().color = color;
		}

		public void FadeIn(float t)
		{
			textMesh = GetComponent<TextMeshPro>();
			UpdateAlpha(0f);
			iTween.ValueTo(base.gameObject, iTween.Hash("from", 0, "to", 1, "time", t, "onupdate", "UpdateAlpha", "oncomplete", "UpdateAlpha", "oncompleteparams", 1));
		}

		public void FadeOut(float t)
		{
			UpdateAlpha(1f);
			iTween.ValueTo(base.gameObject, iTween.Hash("from", 1, "to", 0, "time", t, "onupdate", "UpdateAlpha", "oncomplete", "UpdateAlpha", "oncompleteparams", 0));
		}

		public void RegisterVoices(List<List<AudioInfo>> voices)
		{
			this.voices = voices;
			UpdateColorForHover();
		}

		private void LateUpdate()
		{
		}
	}
}
