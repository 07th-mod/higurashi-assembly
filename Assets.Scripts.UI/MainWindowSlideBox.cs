using Assets.Scripts.Core;
using Assets.Scripts.Core.Buriko;
using UnityEngine;

namespace Assets.Scripts.UI
{
	public class MainWindowSlideBox : MonoBehaviour
	{
		public UIPanel Panel;

		private bool isOpen = true;

		private bool autoHide;

		private bool isoffset;

		private void OnHover(bool hover)
		{
			if (hover && !isOpen && (GameSystem.Instance.GameState == GameState.Normal || GameSystem.Instance.GameState == GameState.ChoiceScreen))
			{
				LeanTween.cancel(Panel.gameObject);
				LeanTween.value(Panel.gameObject, delegate(float f)
				{
					Panel.alpha = f;
				}, Panel.alpha, 1f, 0.3f);
				isOpen = true;
			}
		}

		private void CloseWindow()
		{
			LeanTween.cancel(Panel.gameObject);
			LeanTween.value(Panel.gameObject, delegate(float f)
			{
				Panel.alpha = f;
			}, Panel.alpha, 0f, 0.3f);
			isOpen = false;
		}

		private void Start()
		{
		}

		private void FixedUpdate()
		{
			if (BurikoMemory.Instance != null)
			{
				autoHide = BurikoMemory.Instance.GetGlobalFlag("GHideButtons").BoolValue();
				if (!autoHide && !isOpen)
				{
					LeanTween.cancel(Panel.gameObject);
					LeanTween.value(Panel.gameObject, delegate(float f)
					{
						Panel.alpha = f;
					}, Panel.alpha, 1f, 0.3f);
					isOpen = true;
				}
			}
		}

		private void Update()
		{
			if (autoHide)
			{
				GameObject hoveredObject = UICamera.hoveredObject;
				if (isOpen)
				{
					if (hoveredObject == GameSystem.Instance.SceneController.SceneCameras || (GameSystem.Instance.GameState != GameState.Normal && GameSystem.Instance.GameState != GameState.ChoiceScreen))
					{
						CloseWindow();
					}
					else if (!(hoveredObject == null) && !hoveredObject.transform.IsChildOf(base.gameObject.transform))
					{
						CloseWindow();
					}
				}
			}
		}
	}
}
