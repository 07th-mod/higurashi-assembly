using Assets.Scripts.Core.AssetManagement;
using UnityEngine;

namespace Assets.Scripts.Core.TextWindow
{
	[RequireComponent(typeof(UITexture))]
	public class FaceController : MonoBehaviour
	{
		private UITexture uiTexture;

		public void UpdateFaceAlpha(float a)
		{
			uiTexture.alpha = a;
		}

		public void ShowFace()
		{
			iTween.Stop(base.gameObject);
			uiTexture.alpha = 1f;
		}

		public void HideFace()
		{
			uiTexture.alpha = 0f;
			uiTexture.mainTexture = null;
			uiTexture.enabled = false;
		}

		public void FadeFace(float time, bool isBlocking)
		{
			iTween.ValueTo(base.gameObject, iTween.Hash("from", uiTexture.alpha, "to", 0, "time", time / 1000f, "onupdate", "UpdateFaceAlpha", "oncomplete", "HideFace"));
			if (isBlocking)
			{
				GameSystem.Instance.AddWait(new Wait(time / 1000f, WaitTypes.WaitForMove, HideFace));
			}
		}

		public void SetFaceTexture(string texName, float time, bool isBlocking)
		{
			uiTexture.enabled = true;
			uiTexture.alpha = 0f;
			uiTexture.mainTexture = AssetManager.Instance.LoadTexture(texName);
			iTween.ValueTo(base.gameObject, iTween.Hash("from", 0f, "to", 1f, "time", time / 1000f, "onupdate", "UpdateFaceAlpha", "oncomplete", "ShowFace"));
			if (isBlocking)
			{
				GameSystem.Instance.AddWait(new Wait(time / 1000f, WaitTypes.WaitForMove, ShowFace));
			}
		}

		private void Start()
		{
			uiTexture = GetComponent<UITexture>();
		}

		private void Update()
		{
		}
	}
}
