using Assets.Scripts.Core;
using Assets.Scripts.Core.AssetManagement;
using Assets.Scripts.Core.Audio;
using Assets.Scripts.Core.Buriko;
using Assets.Scripts.Core.State;
using Assets.Scripts.UI.Prompt;
using System.IO;
using UnityEngine;

namespace Assets.Scripts.UI
{
	public class SlideBoxButton : MonoBehaviour
	{
		public SlideButtonType Type;

		private float cooldown;

		private void OnClick()
		{
			if (UICamera.currentTouchID >= -1 && !(cooldown > 0f))
			{
				SlideButtonType type = Type;
				switch (type)
				{
				case SlideButtonType.Log:
					GameSystem.Instance.SwitchToHistoryScreen();
					break;
				case SlideButtonType.Auto:
					GameSystem.Instance.IsAuto = true;
					break;
				case SlideButtonType.Skip:
					GameSystem.Instance.IsSkipping = true;
					break;
				case SlideButtonType.Menu:
					GameSystem.Instance.SwitchToRightClickMenu();
					break;
				case SlideButtonType.QuickSave:
					if (!GameSystem.Instance.CanSave)
					{
						AudioController.Instance.PlaySystemSound("sysse04.ogg", 1);
						return;
					}
					GameSystem.Instance.ScriptSystem.SaveQuickSave();
					break;
				case SlideButtonType.QuickLoad:
				{
					if (!GameSystem.Instance.CanSave || !GameSystem.Instance.CanLoad)
					{
						AudioController.Instance.PlaySystemSound("sysse04.ogg", 1);
						return;
					}
					SaveEntry d = BurikoScriptSystem.Instance.GetQSaveInfo();
					if (d == null)
					{
						return;
					}
					StateDialogPrompt prompt = new StateDialogPrompt(PromptType.DialogLoad, delegate
					{
						GameSystem.Instance.ScriptSystem.LoadQuickSave();
					}, null);
					GameSystem.Instance.PushStateObject(prompt);
					GameSystem.Instance.RegisterAction(delegate
					{
						PromptController promptController = prompt.GetPromptController();
						if (!(promptController == null))
						{
							string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(d.Path);
							Texture2D image = AssetManager.Instance.LoadScreenshot(fileNameWithoutExtension + ".png");
							Debug.Log(promptController);
							Debug.Log(d);
							promptController.SetScreenshotDetails(image, d.Time.ToString("ddd MMM dd, yyyy h:mm tt"), d.Text, d.TextJp);
						}
					});
					GameSystem.Instance.ExecuteActions();
					break;
				}
				}
				AudioController.Instance.PlaySystemSound("wa_038.ogg", 1);
				cooldown = 0.5f;
			}
		}

		private void OnHover(bool ishover)
		{
			if (!(cooldown > 0f) && GameSystem.Instance.MessageBoxVisible)
			{
			}
		}

		private void Update()
		{
			if (cooldown > 0f)
			{
				cooldown -= Time.deltaTime;
			}
		}
	}
}
