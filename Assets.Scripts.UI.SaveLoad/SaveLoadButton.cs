using Assets.Scripts.Core;
using Assets.Scripts.Core.AssetManagement;
using Assets.Scripts.Core.Audio;
using Assets.Scripts.Core.Buriko;
using Assets.Scripts.Core.State;
using Assets.Scripts.UI.Prompt;
using MOD.Scripts.Core.Localization;
using System.IO;
using UnityEngine;

namespace Assets.Scripts.UI.SaveLoad
{
	public class SaveLoadButton : MonoBehaviour
	{
		private int slot;

		private float time = 0.45f;

		private bool isEnabled = true;

		private GameSystem gameSystem;

		private SaveLoadManager manager;

		private static int needUIRefresh = 0;

		private void OnClick()
		{
			if (gameSystem == null)
			{
				gameSystem = GameSystem.Instance;
			}
			if (gameSystem.GameState != GameState.SaveLoadScreen || time > 0f || UICamera.currentTouchID < -1 || !isEnabled)
			{
				return;
			}
			StateSaveLoad state4 = gameSystem.GetStateObject() as StateSaveLoad;
			if (state4 == null)
			{
				return;
			}
			switch (base.name)
			{
			case "Save":
			case "0-Save":
				manager.Save(slot);
				break;
			case "Load":
			case "1-Load":
			{
				SaveEntry d = BurikoScriptSystem.Instance.GetSaveInfo(slot);
				if (d == null)
				{
					return;
				}
				StateDialogPrompt state2 = new StateDialogPrompt(PromptType.DialogLoad, delegate
				{
					state4.Leave(delegate
					{
						StateTitle stateTitle = gameSystem.GetStateObject() as StateTitle;
						if (!manager.CanSave())
						{
							stateTitle?.RequestLeaveImmediate();
						}
						(gameSystem.GetStateObject() as StateChapterScreen)?.RequestLeaveImmediate();
						GameSystem.Instance.ScriptSystem.LoadGame(slot);
						needUIRefresh = 2;
					});
				}, null);
				gameSystem.PushStateObject(state2);
				gameSystem.RegisterAction(delegate
				{
					PromptController promptController = state2.GetPromptController();
					if (!(promptController == null))
					{
						string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(d.Path);
						Texture2D image = AssetManager.Instance.LoadScreenshot(fileNameWithoutExtension + ".png");
						promptController.SetScreenshotDetails(image, d.Time.ToString(Loc.dateTimeFormat, Loc.cultureInfo), d.Text, d.TextJp);
					}
				});
				gameSystem.ExecuteActions();
				break;
			}
			case "2-Edit":
			{
				SaveEntry d3 = BurikoScriptSystem.Instance.GetSaveInfo(slot);
				if (d3 == null)
				{
					return;
				}
				SaveLoadManager.EditSlot = slot;
				StateDialogPrompt state3 = new StateDialogPrompt(PromptType.DialogEdit, delegate
				{
				}, null);
				gameSystem.PushStateObject(state3);
				gameSystem.RegisterAction(delegate
				{
					PromptController promptController2 = state3.GetPromptController();
					if (!(promptController2 == null))
					{
						string fileNameWithoutExtension2 = Path.GetFileNameWithoutExtension(d3.Path);
						Texture2D image2 = AssetManager.Instance.LoadScreenshot(fileNameWithoutExtension2 + ".png");
						promptController2.SetScreenshotDetails(image2, d3.Time.ToString(Loc.dateTimeFormat, Loc.cultureInfo), d3.Text, d3.TextJp);
					}
				});
				gameSystem.ExecuteActions();
				break;
			}
			case "3-Delete":
			{
				SaveEntry d2 = BurikoScriptSystem.Instance.GetSaveInfo(slot);
				if (d2 == null)
				{
					return;
				}
				StateDialogPrompt prompt = new StateDialogPrompt(PromptType.DialogDelete, delegate
				{
					BurikoScriptSystem.Instance.DeleteSave(slot);
					manager.RefreshList();
				}, null);
				GameSystem.Instance.PushStateObject(prompt);
				GameSystem.Instance.RegisterAction(delegate
				{
					PromptController promptController3 = prompt.GetPromptController();
					if (!(promptController3 == null))
					{
						string fileNameWithoutExtension3 = Path.GetFileNameWithoutExtension(d2.Path);
						Texture2D image3 = AssetManager.Instance.LoadScreenshot(fileNameWithoutExtension3 + ".png");
						promptController3.SetScreenshotDetails(image3, d2.Time.ToString(Loc.dateTimeFormat, Loc.cultureInfo), d2.Text, d2.TextJp);
					}
				});
				break;
			}
			case "Return":
				state4.Leave(null);
				break;
			default:
				Debug.LogWarning("Unhandled button action!");
				break;
			}
			AudioController.Instance.PlaySystemSound("wa_038.ogg", 1);
			time = 0.45f;
		}

		private void OnHover(bool hover)
		{
			if (gameSystem == null)
			{
				gameSystem = GameSystem.Instance;
			}
			_ = gameSystem.GameState;
			_ = 10;
		}

		public void Prepare(int slotnum, SaveLoadManager mg)
		{
			manager = mg;
			gameSystem = GameSystem.Instance;
			slot = slotnum;
		}

		private void Update()
		{
			if (time > 0f)
			{
				time -= Time.deltaTime;
			}
		}

		public static void QuicksaveButtonFixerUpdate()
		{
			if (needUIRefresh == 1)
			{
				GameSystem.Instance.HideUIControls();
				GameSystem.Instance.ShowUIControls();
			}

			if (needUIRefresh > 0)
			{
				needUIRefresh--;
			}
		}
	}
}
