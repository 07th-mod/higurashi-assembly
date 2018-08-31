using Assets.Scripts.Core.Audio;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Core.History
{
	public class HistoryTextButton : MonoBehaviour
	{
		private UITweener tweener;

		private TextMeshPro textMesh;

		private void OnHover(bool isHover)
		{
		}

		public void UpdateAlpha(float a)
		{
			textMesh.color = new Color(1f, 1f, 1f, a);
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

		public void ClearVoice()
		{
		}

		public void RegisterVoice(AudioInfo vfile)
		{
		}

		private void LateUpdate()
		{
		}
	}
}
