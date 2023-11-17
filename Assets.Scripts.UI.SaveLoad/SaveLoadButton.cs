using Assets.Scripts.Core;
using Assets.Scripts.Core.AssetManagement;
using Assets.Scripts.Core.Audio;
using Assets.Scripts.Core.Buriko;
using Assets.Scripts.Core.State;
using Assets.Scripts.UI.Prompt;
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

		private void OnClick()
		{
			if (gameSystem == null)
			{
				gameSystem = GameSystem.Instance;
			}
			if (gameSystem.GameState == GameState.SaveLoadScreen && !(time > 0f) && UICamera.currentTouchID >= -1 && isEnabled)
			{
				StateSaveLoad state = gameSystem.GetStateObject() as StateSaveLoad;
				if (state != null)
				{
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
							state.Leave(delegate
							{
								StateTitle stateTitle = gameSystem.GetStateObject() as StateTitle;
								if (!manager.CanSave())
								{
									stateTitle?.RequestLeaveImmediate();
								}
								(gameSystem.GetStateObject() as StateChapterScreen)?.RequestLeaveImmediate();
								GameSystem.Instance.ScriptSystem.LoadGame(slot);
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
								promptController.SetScreenshotDetails(image, d.Time.ToString("ddd MMM dd, yyyy h:mm tt"), d.Text, d.TextJp);
							}
						});
						gameSystem.ExecuteActions();
						break;
					}
					case "2-Edit":
					{
						SaveEntry d2 = BurikoScriptSystem.Instance.GetSaveInfo(slot);
						if (d2 == null)
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
								string fileNameWithoutExtension2 = Path.GetFileNameWithoutExtension(d2.Path);
								Texture2D image2 = AssetManager.Instance.LoadScreenshot(fileNameWithoutExtension2 + ".png");
								promptController2.SetScreenshotDetails(image2, d2.Time.ToString("ddd MMM dd, yyyy h:mm tt"), d2.Text, d2.TextJp);
							}
						});
						gameSystem.ExecuteActions();
						break;
					}
					case "3-Delete":
					{
						SaveEntry d3 = BurikoScriptSystem.Instance.GetSaveInfo(slot);
						if (d3 == null)
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
								string fileNameWithoutExtension3 = Path.GetFileNameWithoutExtension(d3.Path);
								Texture2D image3 = AssetManager.Instance.LoadScreenshot(fileNameWithoutExtension3 + ".png");
								promptController3.SetScreenshotDetails(image3, d3.Time.ToString("ddd MMM dd, yyyy h:mm tt"), d3.Text, d3.TextJp);
							}
						});
						break;
					}
					case "Return":
						state.Leave(null);
						break;
					default:
						Debug.LogWarning("Unhandled button action!");
						break;
					}
					AudioController.Instance.PlaySystemSound("wa_038.ogg", 1);
					time = 0.45f;
				}
			}
		}

		private void OnHover(bool hover)
		{
			if (gameSystem == null)
			{
				gameSystem = GameSystem.Instance;
			}
			if (gameSystem.GameState == GameState.SaveLoadScreen && hover && time < 0f && isEnabled)
			{
				AudioController.Instance.PlaySystemSound("sysse01.ogg");
			}
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
	}
}
