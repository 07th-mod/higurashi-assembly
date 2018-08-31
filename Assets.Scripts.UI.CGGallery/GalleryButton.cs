using Assets.Scripts.Core;
using Assets.Scripts.Core.Buriko;
using UnityEngine;

namespace Assets.Scripts.UI.CGGallery
{
	public class GalleryButton : MonoBehaviour
	{
		private GameSystem gameSystem;

		private float time;

		private int cgslot;

		private void OnClick()
		{
			if (gameSystem == null)
			{
				gameSystem = GameSystem.Instance;
			}
			if (gameSystem.GameState == GameState.CGGallery && !(time > 0f) && UICamera.currentTouchID == -1)
			{
				gameSystem.PopStateStack();
				BurikoMemory.Instance.SetFlag("LOCALWORK_NO_RESULT", cgslot);
				time = 1f;
			}
		}

		private void OnHover(bool hover)
		{
			if (gameSystem == null)
			{
				gameSystem = GameSystem.Instance;
			}
			if (gameSystem.GameState == GameState.CGGallery)
			{
			}
		}

		public void Prepare(GalleryManager manager, int slot)
		{
			cgslot = slot;
		}

		private void Start()
		{
		}

		private void Update()
		{
			if (time > 0f)
			{
				time -= Time.deltaTime;
			}
		}
	}
}
